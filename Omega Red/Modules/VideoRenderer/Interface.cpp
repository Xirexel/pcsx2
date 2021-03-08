// Interface.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "VideoRenderer.h"
#include "PCSX2Lib_API.h"

extern PCSX2Lib::API::GS_API g_API;

PCSX2_EXPORT_C_(PCSX2Lib::API::GS_API*) getAPI()
{
	return &g_API;
}

PCSX2_EXPORT_C execute(const wchar_t* a_command, wchar_t** a_result)
{
	g_VideoRenderer.execute(a_command, a_result);
}

PCSX2_EXPORT_C releaseString(wchar_t* a_string)
{
	if (a_string != nullptr)
		delete[] a_string;
}
