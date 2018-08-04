
#pragma once

#include "string.h"

class wxStandardPaths
{
public:

	static wxStandardPaths& Get();

	wxString GetExecutablePath() const;
};