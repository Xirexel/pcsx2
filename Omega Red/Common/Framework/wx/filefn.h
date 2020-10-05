
#pragma once

#include "string.h"

enum wxSeekMode
{
	wxFromStart,
	wxFromCurrent,
	wxFromEnd
};

extern wxString wxGetCwd();

int wxCRT_Open(const char *filename, int oflag, int mode);

int wxOpen(const wxString &path, int flags, int mode);