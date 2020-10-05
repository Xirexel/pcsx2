
cmake_minimum_required(VERSION 3.4.1)

include_directories(
        src/
)

set(libaio_SRC
        src/io_queue_init.c
        src/io_queue_release.c
        src/io_queue_wait.c
        src/io_queue_run.c
        src/io_getevents.c
        src/io_submit.c
        src/io_cancel.c
        src/io_setup.c
        src/io_destroy.c
        src/raw_syscall.c
        src/compat-0_1.c
        )

add_library(libaio STATIC ${libaio_SRC})

#src/io_queue_init.c src/io_queue_release.c \
#src/io_queue_wait.c src/io_queue_run.c \
#src/io_getevents.c src/io_submit.c src/io_cancel.c \
#src/io_setup.c src/io_destroy.c \
#src/raw_syscall.c \
#src/compat-0_1.c \