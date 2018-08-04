
#include "PrecompiledHeader.h"
#include "PCSX2Lib_API.h"
#include "pugixml.hpp"
#include "CDVD\CDVDaccess.h"
#include "CDVD.h"


PCSX2_EXPORT_C_(PCSX2Lib::API::CDVD_API*) getAPI()
{
	return (PCSX2Lib::API::CDVD_API*)&CDVDapi_Iso;
}

PCSX2_EXPORT_C execute(const wchar_t* a_command, wchar_t** a_result)
{
	g_CDVD.execute(a_command, a_result);
}

PCSX2_EXPORT_C releaseString(wchar_t* a_string)
{
	if (a_string != nullptr)
		delete[] a_string;
}