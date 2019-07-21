
#include "Pcsx2Defs.h"
#include "PCSX2Lib_API.h"
#include "AudioRenderer.h"
extern PCSX2Lib::API::SPU2_API g_API;

PCSX2_EXPORT_C_(PCSX2Lib::API::SPU2_API*) getAPI()
{
	return &g_API;
}

PCSX2_EXPORT_C execute(const wchar_t* a_command, wchar_t** a_result)
{
	g_AudioRenderer.execute(a_command, a_result);
}

PCSX2_EXPORT_C releaseString(wchar_t* a_string)
{
	if (a_string != nullptr)
		delete[] a_string;
}
