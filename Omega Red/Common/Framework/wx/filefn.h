
#pragma once

#include "string.h"

enum wxSeekMode
{
	wxFromStart,
	wxFromCurrent,
	wxFromEnd
};

extern wxString wxGetCwd();