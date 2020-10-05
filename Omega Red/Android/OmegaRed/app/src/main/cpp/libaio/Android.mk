LOCAL_PATH := $(my-dir)

include $(CLEAR_VARS)
LOCAL_MODULE := libaio

LOCAL_SRC_FILES := \
	src/io_queue_init.c src/io_queue_release.c \
	src/io_queue_wait.c src/io_queue_run.c \
	src/io_getevents.c src/io_submit.c src/io_cancel.c \
	src/io_setup.c src/io_destroy.c \
	src/raw_syscall.c \
	src/compat-0_1.c \


# TODO: remove liblog as whole static library, once we don't have prebuilt that requires
# liblog symbols present in libcutils.
LOCAL_WHOLE_STATIC_LIBRARIES :=
LOCAL_SHARED_LIBRARIES :=

LOCAL_CFLAGS_arm += -D__arm__

NAME=libaio
SPECFILE=$(NAME).spec
VERSION=$(shell awk '/Version:/ { print $$2 }' $(SPECFILE))
RELEASE=$(shell awk '/Release:/ { print $$2 }' $(SPECFILE))

LOCAL_CFLAGS += -Werror -nostdlib -nostartfiles -Wall -fPIC -g -fomit-frame-pointer -O2 -DANDROID
LOCAL_C_INCLUDES += $(LOCAL_PATH)/src
LOCAL_ADDITIONAL_DEPENDENCIES := $(LOCAL_PATH)/Android.mk
LOCAL_EXPORT_C_INCLUDE_DIRS := $(LOCAL_PATH)/src/
include $(BUILD_SHARED_LIBRARY)
