/*
 * Copyright 2015, Red Hat, Inc.
 *
 * This test remaps the aio ring buffer and ensures that I/O completions
 * can still be reaped from userspace.
 *
 * Author: Jeff Moyer <jmoyer@redhat.com>
 */
#define _GNU_SOURCE
#include <stdio.h>
#include <unistd.h>
#include <stdlib.h>
#include <string.h>
#include <sys/mman.h>
#include <signal.h>
#include <sched.h>
#include <libaio.h>

#define BUFLEN 4096
#define TEMPLATE "19.XXXXXX"

volatile sig_atomic_t timed_out = 0;

struct aio_ring {
	unsigned	id;	/* kernel internal index number */
	unsigned	nr;	/* number of io_events */
	volatile unsigned	head;
	volatile unsigned	tail;

	unsigned	magic;
	unsigned	compat_features;
	unsigned	incompat_features;
	unsigned	header_length;	/* size of aio_ring */

	struct io_event io_events[0];
};

int
open_temp_file(void)
{
	int fd;
	char template[sizeof(TEMPLATE)];

	strncpy(template, TEMPLATE, sizeof(TEMPLATE));
	fd = mkostemp(template, O_DIRECT);
	if (fd < 0) {
		perror("mkstemp");
		exit(1);
	}
	unlink(template);
	return fd;
}

/*
 * mmap will do the address space search for us.  when remapping the ring,
 * the use of MREMAP_FIXED will cause this mapping to be unmapped.
 *
 * len - length in bytes
 *
 * Returns the available virtual address, or MAP_FAILED on error.
 */
void *
find_unused_va(size_t len)
{
	return mmap(0, len, PROT_NONE, MAP_PRIVATE|MAP_ANONYMOUS, -1, 0);
}

void
alarm_handler(int __attribute__((unused))signo)
{
	timed_out = 1;
}

int
user_getevents(io_context_t ctx, int nr_events, struct io_event *event)
{
	struct aio_ring *ring = (void *)ctx;
	int completed = 0;

	timed_out = 0;
	signal(SIGALRM, alarm_handler);
	alarm(30);

	while ((completed < nr_events) && !timed_out) {
		unsigned new_head;

		if (ring->head == ring->tail) {
			sched_yield();
			continue;
		}

		new_head = ring->head;
		*event = ring->io_events[new_head];
		new_head += 1;
		new_head %= ring->nr;
		ring->head = new_head;
		completed++;
	}

	alarm(0);
	signal(SIGALRM, SIG_DFL);

	return completed;
}

struct aio_ring *
remap_ring(struct aio_ring *ring)
{
	struct aio_ring *new_ring;
	size_t ring_size;

	/*
	 * No need to round up to page size as ring->nr was adjusted
	 * already to fill the last page in the ring.
	 */
	ring_size = sizeof(struct aio_ring) + ring->nr * sizeof(struct io_event);

	/*
	 * Remap the ring.
	 */
	new_ring = find_unused_va(ring_size);
	if (new_ring == MAP_FAILED) {
		fprintf(stderr, "Unable to find suitable va for ring\n");
		return NULL;
	}

	new_ring = mremap(ring, ring_size, ring_size,
			  MREMAP_FIXED|MREMAP_MAYMOVE, new_ring);
	if (new_ring == MAP_FAILED || new_ring == ring) {
		perror("mremap");
		return NULL;
	}

	return new_ring;
}

io_context_t
remap_io_context(io_context_t ctx)
{
	struct aio_ring *ring, *new_ring;

	ring = (void *)ctx;
	new_ring = remap_ring(ring);
	if (!new_ring)
		return NULL;

	ctx = (io_context_t)new_ring;
	return ctx;
}

int
do_io(io_context_t ctx, struct iocb *iocbp, int fd)
{
	int ret;
	char buf[BUFLEN];

	io_prep_pwrite(iocbp, fd, buf, BUFLEN, 0);
	ret = io_submit(ctx, 1, &iocbp);
	if (ret != 1) {
		fprintf(stderr, "io_submit failed with %d\n", ret);
		return 1;
	}
	return 0;
}

int
check_completion(io_context_t ctx, struct iocb *iocbp)
{
	int ret;
	struct io_event event;

	ret = user_getevents(ctx, 1, &event);
	if (ret != 1) {
		fprintf(stderr, "user_getevents timed out.\n");
		return 1;
	}

	if (event.obj != iocbp) {
		fprintf(stderr,
			"Error: event->opj (%p) does not match iocbp (%p)\n",
			event.obj, iocbp);
		return 1;
	}

	return 0;
}

int
test_main()
{
	int fd;
	int ret;
	io_context_t ctx;
	struct iocb iocb;

	memset(&ctx, 0, sizeof(ctx));
	fd = open_temp_file();

	ret = io_setup(1, &ctx);
	if (ret != 0) {
		fprintf(stderr, "io_setup failed with %d\n", ret);
		return 1;
	}

	/*
	 * First, try remapping the ring buffer in-between io_setup and
	 * io_submit.
	 */
	ctx = remap_io_context(ctx);
	if (ctx == NULL)
		return 1;

	ret = do_io(ctx, &iocb, fd);
	if (ret != 0)
		return 1;

	ret = check_completion(ctx, &iocb);
	if (ret != 0)
		return 1;

	/*
	 * Now remap the ring in between io_submit and getevents.
	 */
	ret = do_io(ctx, &iocb, fd);
	if (ret != 0)
		return 1;

	ctx = remap_io_context(ctx);
	if (ctx == NULL)
		return 1;

	ret = check_completion(ctx, &iocb);
	if (ret != 0)
		return 1;

	/*
	 * Success, clean up.
	 */
	ret = io_destroy(ctx);
	if (ret != 0) {
		fprintf(stderr, "io_destroy failed with %d\n", ret);
		return 1;
	}
	close(fd);

	return 0;
}
/*
 * Local variables:
 *  mode: c
 *  c-basic-offset: 8
 * End:
 */
