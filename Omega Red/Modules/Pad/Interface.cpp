
#include "stdafx.h"
#include "Pad.h"

extern PCSX2Lib::API::PAD_API g_API;

HINSTANCE hInst;

PCSX2_EXPORT_C_(PCSX2Lib::API::PAD_API*) getAPI()
{
	return &g_API;
}

PCSX2_EXPORT_C execute(const wchar_t* a_command, wchar_t** a_result)
{
	g_Pad.execute(a_command, a_result);
}

PCSX2_EXPORT_C releaseString(wchar_t* a_string)
{
	if (a_string != nullptr)
		delete[] a_string;
}

#ifdef _MSC_VER

CRITICAL_SECTION updateLock;

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD fdwReason, void *lpvReserved)
{
	hInst = hInstance;
	if (fdwReason == DLL_PROCESS_ATTACH) {
		InitializeCriticalSection(&updateLock);

		DisableThreadLibraryCalls(hInstance);
	}
	else if (fdwReason == DLL_PROCESS_DETACH) {
		//while (openCount)
		//	PADclose();
		//PADshutdown();
		//UninitLibUsb();
		DeleteCriticalSection(&updateLock);
	}
	return 1;
}
#endif