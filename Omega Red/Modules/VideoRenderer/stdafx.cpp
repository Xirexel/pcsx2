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