/*
 * Copyright (C) 2014, Dan Aloni, Kernelim Ltd.
 * Copyright (C) 2014, Benjamin LaHaise <bcrl@kvack.org>.
 * Copyright (C) 2014, Red Hat, Inc.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 2 as
 * published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 */
/*
 * Description:
 *   This regression test ensures that submitting more events than can
 *   fit in the completion ring will not result in a hung task.
 */
#ifndef _GNU_SOURCE
#define _GNU_SOURCE 1
#endif
#include <assert.h>
#include <errno.h>
#include <libaio.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <sys/param.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/uio.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <sys/mman.h>
#include <sys/wait.h>

const int max_events = 32;
const int io_size = 0x1000;
struct iocb *io;
struct iocb **iops;
struct iovec *iovecs;
struct io_event *events;
char *data;

long submitted = 0;
long completed = 0;
long pending = 0;

#define SYS_IO_GETEVENTS 0
#define USER_GETEVENTS 1

static volatile sig_atomic_t done = 0;

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

int get_ring_size(int nr_events)
{
	io_context_t ctx;
	int ret, ring_size;
	struct aio_ring *ring;

	memset(&ctx, 0, sizeof(ctx));
	ret = io_setup(nr_events, &ctx);
	assert(!ret);

	ring = (void *)ctx;
	ring_size = ring->nr;

	ret = io_destroy(ctx);
	assert(!ret);

	return ring_size;
}

int user_getevents(io_context_t ctx, int nr_events, struct io_event *event)
{
	struct aio_ring *ring = (void *)ctx;
	int completed = 0;
	while ((completed < nr_events) && (ring->head != ring->tail)) {
		unsigned new_head = ring->head;
		*event = ring->io_events[new_head];
		new_head += 1;
		new_head %= ring->nr;
		ring->head = new_head;
		completed++;
	}
	return completed;
}

void prune(io_context_t io_ctx, int max_ios, int getevents_type)
{
	int ret;

	if (getevents_type == USER_GETEVENTS)
        	ret = user_getevents(io_ctx, max_ios, events);
	else
        	ret = io_getevents(io_ctx, pending, max_ios, events, NULL);
	if (ret > 0) {
		printf("Completed: %d\n", ret);
		completed += ret;
		pending -= ret;
	}
}

void run_test(int max_ios, int getevents_type)
{
	int fd, ret;
	long i, to_submit;
	struct iocb **iocb_sub;
	io_context_t io_ctx;
	const char *filename = "testfile";

	printf("MAX_IOS: %d, %s\n", max_ios, getevents_type == USER_GETEVENTS ?
	       "USER_GETEVENTS" : "IO_GETEVENTS");
	memset(&io_ctx, 0, sizeof(io_ctx));
	ret = io_setup(max_events, &io_ctx);
	assert(!ret);

	io = calloc(max_ios, sizeof(*io));
	iops = calloc(max_ios, sizeof(*iops));
	iovecs = calloc(max_ios, sizeof(*iovecs));
	events = calloc(max_ios, sizeof(*events));

	unlink(filename);
	fd = open(filename, O_CREAT | O_RDWR | O_DIRECT, 0644);
	assert(fd >= 0);

	ret = ftruncate(fd, max_ios * io_size);
	assert(!ret);

	data = mmap(NULL, io_size * max_ios, PROT_READ | PROT_WRITE,
		    MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);
	assert(data != MAP_FAILED);

	for (i = 0; i < max_ios; i++) {
		iops[i] = &io[i];
		io[i].data = io;
		iovecs[i].iov_base = &data[io_size * i];
		iovecs[i].iov_len = io_size;
		io_prep_preadv(&io[i], fd, &iovecs[i], 1, 0);
	}

	submitted = completed = pending = 0;

	to_submit = max_ios;
	iocb_sub = iops;

	while (submitted < max_ios) {
		printf("Submitting: %ld\n", to_submit);

		ret = io_submit(io_ctx, to_submit, iocb_sub);
		if (ret >= 0) {
			printf("Submitted: %d\n", ret);
			submitted += ret;
			iocb_sub += ret;
			pending += ret;
			to_submit -= ret;
		} else {
			if (ret == -EAGAIN) {
				printf("Submitted too much, that's okay\n");
				prune(io_ctx, max_ios, getevents_type);
			}
		}
	}

	prune(io_ctx, max_ios, getevents_type);
	io_destroy(io_ctx);
	close(fd);
	ret = munmap(data, io_size * max_ios);
	assert(!ret);

	printf("Verifying...\n");

	assert(completed == submitted);

	printf("OK\n");
}

void run_child(void)
{
	int ring_size;

	ring_size = get_ring_size(max_events);

	printf("aio ring size: %d\n", ring_size);

	run_test(ring_size-1, SYS_IO_GETEVENTS);
	run_test(ring_size, SYS_IO_GETEVENTS);
	run_test(ring_size+1, SYS_IO_GETEVENTS);
	run_test(ring_size*2, SYS_IO_GETEVENTS);
	run_test(ring_size*4, SYS_IO_GETEVENTS);

	run_test(ring_size-1, USER_GETEVENTS);
	run_test(ring_size, USER_GETEVENTS);
	run_test(ring_size+1, USER_GETEVENTS);
	run_test(ring_size*2, USER_GETEVENTS);
	run_test(ring_size*4, USER_GETEVENTS);

	exit(0);
}

void sighandler(int signo)
{
	assert(signo == SIGCHLD);
	done = 1;
}

int test_main(void)
{
	unsigned int ret;
	sighandler_t oldhandler;
	pid_t child;

	switch (child = fork()) {
	case 0: /* child */
		run_child();
		break;
	case -1:
		perror("fork");
		exit(1);
	default:
		oldhandler = signal(SIGCHLD, sighandler);
		assert(oldhandler != SIG_ERR);
		break;
	}

	ret = sleep(10);
	if (ret != 0) {
		pid_t pid;
		int status;

		assert(done);

		pid = wait(&status);
		if (pid != child) {
			perror("wait");
			exit(1);
		}

		return WEXITSTATUS(status);
	}

	return 1; /* failed */
}
/*
 * Local Variables:
 *  mode: c
 *  c-basic-offset: 8
 * End:
 */
