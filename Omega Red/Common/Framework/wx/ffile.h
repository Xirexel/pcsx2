
#pragma once

#include "string.h"
#include "file.h"

class wxFFile
{
public:

	wxFFile(const wxString& src, const wxChar* mode = L"r");

	~wxFFile();

	bool IsOpened();

	void Close();

	size_t Write(const uint8_t*, size_t);

	size_t Write(const char*, size_t);

	size_t Read(uint8_t*, size_t);

	void Seek(int a_seek);	
};