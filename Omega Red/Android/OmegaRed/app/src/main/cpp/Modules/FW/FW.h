/*  FWnull
 *  Copyright (C) 2004-20010  PCSX2 Dev Team
 *
 *  PCSX2 is free software: you can redistribute it and/or modify it under the terms
 *  of the GNU Lesser General Public License as published by the Free Software Found-
 *  ation, either version 3 of the License, or (at your option) any later version.
 *
 *  PCSX2 is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 *  PURPOSE.  See the GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along with PCSX2.
 *  If not, see <http://www.gnu.org/licenses/>.
 */

#ifndef __FW_H__
#define __FW_H__

#include <stdio.h>
#include <string>

#define FWdefs
#include "PS2Edefs.h"

#undef noexcept
#undef thread_local

#include <cstring>

#define EXPORT_C_(type) extern "C" __attribute__((stdcall, externally_visible, visibility("default"))) type


struct PluginLog
{
	bool WriteToFile, WriteToConsole;
	FILE *LogFile;

	bool Open(std::string logname)
	{
//		LogFile = px_fopen(logname, "w");
//
//		if (LogFile) {
//			setvbuf(LogFile, NULL, _IONBF, 0);
//			return true;
//		}
		return false;
	}

	void Close()
	{
		if (LogFile) {
			fclose(LogFile);
			LogFile = NULL;
		}
	}

	void Write(const char *fmt, ...)
	{
		if (LogFile == NULL)
			return;

		va_list list;
		if (WriteToFile) {
			va_start(list, fmt);
			vfprintf(LogFile, fmt, list);
			va_end(list);
		}
		if (WriteToConsole) {
			va_start(list, fmt);
			vfprintf(stdout, fmt, list);
			va_end(list);
		}
	}

	void WriteLn(const char *fmt, ...)
	{
		if (LogFile == NULL)
			return;

		va_list list;
		if (WriteToFile) {
			va_start(list, fmt);
			vfprintf(LogFile, fmt, list);
			va_end(list);
			fprintf(LogFile, "\n");
		}
		if (WriteToConsole) {
			va_start(list, fmt);
			vfprintf(stdout, fmt, list);
			va_end(list);
			fprintf(stdout, "\n");
		}
	}

#if !defined(_MSC_VER) || !defined(UNICODE)
	void Message(const char *fmt, ...)
	{
//		va_list list;
//		char buf[256];
//
//		if (LogFile == NULL)
//			return;
//
//		va_start(list, fmt);
//		vsprintf(buf, fmt, list);
//		va_end(list);
//
//		SysMessage(buf);
	}
#else
	void Message(const wchar_t *fmt, ...)
    {
        va_list list;
        wchar_t buf[256];

        if (LogFile == NULL)
            return;

        va_start(list, fmt);
        vswprintf(buf, 256, fmt, list);
        va_end(list);

        SysMessage(buf);
    }
#endif
};

struct PluginConf
{
	FILE *ConfFile;
	char *PluginName;

//	bool Open(std::string name, FileMode mode = READ_FILE)
//	{
////		if (mode == READ_FILE) {
////			ConfFile = px_fopen(name, "r");
////		} else {
////			ConfFile = px_fopen(name, "w");
////		}
////
////		if (ConfFile == NULL)
////			return false;
//
//		return true;
//	}

	void Close()
	{
		if (ConfFile) {
			fclose(ConfFile);
			ConfFile = NULL;
		}
	}

	int ReadInt(const std::string &item, int defval)
	{
		int value = defval;
		std::string buf = item + " = %d\n";

		if (ConfFile)
			if (fscanf(ConfFile, buf.c_str(), &value) < 0)
				fprintf(stderr, "Error reading %s\n", item.c_str());

		return value;
	}

	void WriteInt(std::string item, int value)
	{
		std::string buf = item + " = %d\n";

		if (ConfFile)
			fprintf(ConfFile, buf.c_str(), value);
	}
};




// Our main memory storage, and defines for accessing it.
extern s8 *fwregs;
#define fwRs32(mem) (*(s32 *)&fwregs[(mem)&0xffff])
#define fwRu32(mem) (*(u32 *)&fwregs[(mem)&0xffff])

//PHY Access Address for ease of use :P
#define PHYACC fwRu32(0x8414)

typedef struct
{
    s32 Log;
} Config;

extern Config conf;

extern void (*FWirq)();

extern void SaveConfig();
extern void LoadConfig();
extern void setLoggingState();

EXPORT_C_(void) FWsetSettingsDir(const char *dir);
EXPORT_C_(s32)
FWinit();
EXPORT_C_(s32)
FWopen(void *pDsp);
EXPORT_C_(void)
FWclose();
EXPORT_C_(void)
FWshutdown();
EXPORT_C_(u32)
FWread32(u32 addr);
EXPORT_C_(void)
FWwrite32(u32 addr, u32 value);
EXPORT_C_(void)
FWirqCallback(void (*callback)());

#endif
