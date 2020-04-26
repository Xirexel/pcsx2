// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>
#include <timeapi.h>


#include "PS2Edefs.h"
#undef noexcept
#include "PCSX2Lib_API.h"
#include "pugixml.hpp"


struct EnterScopedSection
{
	CRITICAL_SECTION &m_cs;

	EnterScopedSection(CRITICAL_SECTION &cs)
		: m_cs(cs)
	{
		EnterCriticalSection(&m_cs);
	}

	~EnterScopedSection()
	{
		LeaveCriticalSection(&m_cs);
	}
};


#define DEBUG_TEXT_OUT 0&&
#define DEBUG_IN 0&&
#define DEBUG_OUT 0&&

extern void DEBUG_NEW_SET();

extern void *ptrVibrationCallback;
