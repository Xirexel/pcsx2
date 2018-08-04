
#pragma once

#include "string.h"

class wxFile
{
	FILE* m_pfile;

public:

	wxFile(const wxString& filename);
	
	~wxFile();

	size_t Read(uint8_t*, size_t);
};