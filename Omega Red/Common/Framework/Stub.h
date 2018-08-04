#pragma once

#include "wx\string.h"

#undef Yield // release the burden of windows.h global namespace spam.

struct DisassemblyWindow
{
	void update(){}
};

class AppStub
{
	DisassemblyWindow mDisassemblyWindow;

public:

	void Yield(bool);

	DisassemblyWindow* GetDisassemblyPtr()
	{
		return &mDisassemblyWindow;
	}
};

extern AppStub gAppStub;

#define wxTheApp static_cast<AppStub*>(&gAppStub)

class MessageOutputDebugStub
{
public:

	void __cdecl Printf(wchar_t const *, ...)
	{

	}
};


extern MessageOutputDebugStub& wxMessageOutputDebug();

extern AppStub& wxGetApp();

extern bool wxIsDebuggerRunning();

extern wxString wxGetTranslation(const wxChar *msg_diag);

extern FILE* wxFopen(const char *filename, const char *mode);

extern FILE* wxFopen(const wchar_t *filename, const char *mode);


