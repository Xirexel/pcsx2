#define ENABLE_SIO1API

#include "PCSXLib_API.h"

extern "C" void* g_ptrAPI;

extern "C" void executeExecute(const wchar_t *a_command, wchar_t **a_result);

PCSX_EXPORT_C_(void*) getAPI()
{
    return g_ptrAPI;
}

PCSX_EXPORT_C execute(const wchar_t *a_command, wchar_t **a_result)
{
    executeExecute(a_command, a_result);
}

PCSX_EXPORT_C releaseString(wchar_t *a_string)
{
    if (a_string != nullptr)
        delete[] a_string;
}