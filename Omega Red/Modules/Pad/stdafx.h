// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#ifdef _WIN32

#include "targetver.h"
#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>
#include <timeapi.h>


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

#elif __ANDROID__


typedef int                 BOOL;
typedef unsigned char       BYTE;
typedef unsigned short      WORD;
typedef short SHORT;



//
// Structures used by XInput APIs
//
typedef struct _XINPUT_GAMEPAD
{
	WORD                                wButtons;
	BYTE                                bLeftTrigger;
	BYTE                                bRightTrigger;
	SHORT                               sThumbLX;
	SHORT                               sThumbLY;
	SHORT                               sThumbRX;
	SHORT                               sThumbRY;
} XINPUT_GAMEPAD, *PXINPUT_GAMEPAD;

typedef struct _XINPUT_STATE
{
	unsigned int                               dwPacketNumber;
	XINPUT_GAMEPAD                      Gamepad;
} XINPUT_STATE, *PXINPUT_STATE;

typedef struct _XINPUT_VIBRATION
{
	WORD                                wLeftMotorSpeed;
	WORD                                wRightMotorSpeed;
} XINPUT_VIBRATION, *PXINPUT_VIBRATION;


#define MAX_PATH          260


#ifndef max
#define max(a,b)            (((a) > (b)) ? (a) : (b))
#endif

#ifndef min
#define min(a,b)            (((a) < (b)) ? (a) : (b))
#endif

#include "LilyPad/Global.h"
#include <wchar.h>

static wchar_t *_wcsdup(const wchar_t* a_text){

	auto l_length = wcslen(a_text);

	wchar_t* l_result = (wchar_t*)malloc((l_length + 1) * sizeof(wchar_t));

	for (int i = 0; i < l_length; ++i) {
		l_result[i] = a_text[i];
	}

	l_result[l_length] = 0;

	return l_result;
}

static int _wcsicmp(
	const wchar_t *string1,
	const wchar_t *string2
)
{
	return wcsicmp(string1, string2);
}

#define HWND void*

#define VK_BACK           0x08
#define VK_TAB            0x09

/*
 * 0x0A - 0x0B : reserved
 */

#define VK_CLEAR          0x0C
#define VK_RETURN         0x0D

#define VK_SHIFT          0x10
#define VK_CONTROL        0x11
#define VK_MENU           0x12
#define VK_PAUSE          0x13
#define VK_CAPITAL        0x14

#define VK_KANA           0x15
#define VK_HANGEUL        0x15  /* old name - should be here for compatibility */
#define VK_HANGUL         0x15
#define VK_JUNJA          0x17
#define VK_FINAL          0x18
#define VK_HANJA          0x19
#define VK_KANJI          0x19

#define VK_ESCAPE         0x1B

#define VK_CONVERT        0x1C
#define VK_NONCONVERT     0x1D
#define VK_ACCEPT         0x1E
#define VK_MODECHANGE     0x1F

#define VK_SPACE          0x20
#define VK_PRIOR          0x21
#define VK_NEXT           0x22
#define VK_END            0x23
#define VK_HOME           0x24
#define VK_LEFT           0x25
#define VK_UP             0x26
#define VK_RIGHT          0x27
#define VK_DOWN           0x28
#define VK_SELECT         0x29
#define VK_PRINT          0x2A
#define VK_EXECUTE        0x2B
#define VK_SNAPSHOT       0x2C
#define VK_INSERT         0x2D
#define VK_DELETE         0x2E
#define VK_HELP           0x2F


#define VK_F4             0x73


/*
 * 0x97 - 0x9F : unassigned
 */

/*
 * VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
 * Used only as parameters to GetAsyncKeyState() and GetKeyState().
 * No other API or message will distinguish left and right keys in this way.
 */
#define VK_LSHIFT         0xA0
#define VK_RSHIFT         0xA1
#define VK_LCONTROL       0xA2
#define VK_RCONTROL       0xA3
#define VK_LMENU          0xA4
#define VK_RMENU          0xA5


#endif




#include "PS2Edefs.h"
#undef noexcept
#include "PCSX2Lib_API.h"
#include "pugixml.hpp"




#define DEBUG_TEXT_OUT 0&&
#define DEBUG_IN 0&&
#define DEBUG_OUT 0&&

extern void DEBUG_NEW_SET();

extern void *ptrButtonUpdateCallback;
