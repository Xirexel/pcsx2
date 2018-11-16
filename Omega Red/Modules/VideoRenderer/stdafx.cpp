// stdafx.cpp : source file that includes just the standard includes
// VideoRenderer.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

// TODO: reference any additional headers you need in STDAFX.H
// and not in this file

void* vmalloc(size_t size, bool code)
{
	return VirtualAlloc(NULL, size, MEM_COMMIT | MEM_RESERVE, code ? PAGE_EXECUTE_READWRITE : PAGE_READWRITE);
}

void vmfree(void* ptr, size_t size)
{
	VirtualFree(ptr, 0, MEM_RELEASE);
}

string format(const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);

	int result = -1, length = 256;

	char* buffer = NULL;

	while (result == -1)
	{
		if (buffer) delete[] buffer;

		buffer = new char[length + 1];

		memset(buffer, 0, length + 1);

		result = _vsnprintf_s(buffer, length, length, fmt, args);

		length *= 2;
	}

	va_end(args);

	string s(buffer);

	delete[] buffer;

	return s;
}

extern void fifo_free(void *ptr, size_t size, size_t repeat);

static HANDLE s_fh = NULL;
static uint8 *s_Next[8];

void *fifo_alloc(size_t size, size_t repeat)
{
    ASSERT(s_fh == NULL);

    if (repeat >= countof(s_Next)) {
        fprintf(stderr, "Memory mapping overflow (%zu >= %u)\n", repeat, countof(s_Next));
        return vmalloc(size * repeat, false); // Fallback to default vmalloc
    }

    s_fh = CreateFileMapping(INVALID_HANDLE_VALUE, nullptr, PAGE_READWRITE, 0, size, nullptr);
    DWORD errorID = ::GetLastError();
    if (s_fh == NULL) {
        fprintf(stderr, "Failed to reserve memory. WIN API ERROR:%u\n", errorID);
        return vmalloc(size * repeat, false); // Fallback to default vmalloc
    }

    int mmap_segment_failed = 0;
    void *fifo = MapViewOfFile(s_fh, FILE_MAP_ALL_ACCESS, 0, 0, size);
    for (size_t i = 1; i < repeat; i++) {
        void *base = (uint8 *)fifo + size * i;
        s_Next[i] = (uint8 *)MapViewOfFileEx(s_fh, FILE_MAP_ALL_ACCESS, 0, 0, size, base);
        errorID = ::GetLastError();
        if (s_Next[i] != base) {
            mmap_segment_failed++;
            if (mmap_segment_failed > 4) {
                fprintf(stderr, "Memory mapping failed after %d attempts, aborting. WIN API ERROR:%u\n", mmap_segment_failed, errorID);
                fifo_free(fifo, size, repeat);
                return vmalloc(size * repeat, false); // Fallback to default vmalloc
            }
            do {
                UnmapViewOfFile(s_Next[i]);
                s_Next[i] = 0;
            } while (--i > 0);

            fifo = MapViewOfFile(s_fh, FILE_MAP_ALL_ACCESS, 0, 0, size);
        }
    }

    return fifo;
}

void fifo_free(void *ptr, size_t size, size_t repeat)
{
    ASSERT(s_fh != NULL);

    if (s_fh == NULL) {
        if (ptr != NULL)
            vmfree(ptr, size);
        return;
    }

    UnmapViewOfFile(ptr);

    for (size_t i = 1; i < countof(s_Next); i++) {
        if (s_Next[i] != 0) {
            UnmapViewOfFile(s_Next[i]);
            s_Next[i] = 0;
        }
    }

    CloseHandle(s_fh);
    s_fh = NULL;
}