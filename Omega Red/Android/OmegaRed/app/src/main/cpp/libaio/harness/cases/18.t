/*
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
 * Author: Anatol Pomozov <anatol.pomozov@gmail.com>
 *
 * Description: This code tests to make sure that when io_destroy
 * returns, all outstanding I/Os have been completed.  It does this by
 * issuing one I/O and then calling io_destroy (without calling
 * io_getevents).  After the call to io_destroy, the buffer is checked
 * to ensure that the data was retrieved.  This is done simultaneously
 * from 100 threads.
 */

#define _GNU_SOURCE

#include <pthread.h>
#include <stdlib.h>
#include <stdio.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <assert.h>
#include <unistd.h>
#include <string.h>
#include <libaio.h>
#include <stdbool.h>

#define FILENAME "tempfile"
#define FILEPATTERN '1'
#define DESTROY_PATTERN '2'

#define THREADS_NUM 100

void
aio_worker(void *ptr)
{
	int i, j, fd;
	char buffer[PAGE_SIZE] __attribute__((aligned(PAGE_SIZE)));

	fd = open(FILENAME, O_DIRECT|O_RDONLY);
	assert(fd >= 0);

	for (i = 0; i < 1000; i++) {
		io_context_t ctx;
		struct iocb cb;
		struct iocb *cbs[1];

		assert(!io_queue_init(1, &ctx));
		io_prep_pread(&cb, fd, buffer, PAGE_SIZE, 0);
		cbs[0] = &cb;

		memset(buffer, '0', PAGE_SIZE);
		assert(io_submit(ctx, 1, &cbs[0]) == 1);
		// wait random time (0-500ms) ?

		io_destroy(ctx);
		memset(buffer, DESTROY_PATTERN, PAGE_SIZE);
		// wait random for (0-500ms) ?

		// check it is still DESTROY_PATTERN
		for (j = 0; j < PAGE_SIZE; j++) {
			if (buffer[j] != DESTROY_PATTERN) {
				fprintf(stderr,
					"Buffer has unexpected character: %c\n",
					buffer[j]);
				exit(EXIT_FAILURE);
			}
		}
	}

	close(fd);
}

int
test_main(void)
{
	int i, fd, ret;
	char buffer[PAGE_SIZE];
	pthread_t threads[THREADS_NUM];

	fd = open(FILENAME, O_CREAT|O_TRUNC|O_APPEND|O_RDWR, S_IRUSR|S_IWUSR);
	assert(fd != -1);

	memset(buffer, FILEPATTERN, PAGE_SIZE);
	write(fd, buffer, PAGE_SIZE);
	close(fd);

	for (i = 0; i < THREADS_NUM; i++) {
		ret = pthread_create(&threads[i], NULL,
				     (void *)&aio_worker, NULL);
		assert(ret == 0);
	}
	for (i = 0; i < THREADS_NUM; i++) {
		ret = pthread_join(threads[i], NULL);
		assert(ret == 0);
	}

	return EXIT_SUCCESS;
}
/*
 * Local Variables:
 *  mode: c
 *  c-basic-offset: 8
 * End:
 */
