
#ifdef __ANDROID__
#include "MC.h"
#else
#include "stdafx.h"
#endif
#include "MemoryCard.h"

#include "PluginCallbacks.h"

extern PS2E_ComponentAPI_Mcd g_API;

PCSX2_EXPORT_C_(void*) getAPI()
{
	return &g_API;
}

PCSX2_EXPORT_C execute(const wchar_t* a_command, wchar_t** a_result)
{
	g_MemoryCard.execute(a_command, a_result);
}

PCSX2_EXPORT_C releaseString(wchar_t* a_string)
{
	if (a_string != nullptr)
		delete[] a_string;
}
