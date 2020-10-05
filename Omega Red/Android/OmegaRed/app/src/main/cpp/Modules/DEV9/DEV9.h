/*  dev9null
 *  Copyright (C) 2002-2010 pcsx2 Team
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 */

#ifndef __DEV9_H__
#define __DEV9_H__

#include <stdio.h>
#include <string>

#define DEV9defs
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


typedef struct
{
    s32 Log;
} Config;

extern Config conf;

extern const unsigned char version;
extern const unsigned char revision;
extern const unsigned char build;
extern const unsigned int minor;

void SaveConfig();
void LoadConfig();

extern void (*DEV9irq)(int);

extern __aligned16 s8 dev9regs[0x10000];
#define dev9Rs8(mem) dev9regs[(mem)&0xffff]
#define dev9Rs16(mem) (*(s16 *)&dev9regs[(mem)&0xffff])
#define dev9Rs32(mem) (*(s32 *)&dev9regs[(mem)&0xffff])
#define dev9Ru8(mem) (*(u8 *)&dev9regs[(mem)&0xffff])
#define dev9Ru16(mem) (*(u16 *)&dev9regs[(mem)&0xffff])
#define dev9Ru32(mem) (*(u32 *)&dev9regs[(mem)&0xffff])

extern void setLoggingState();

#endif
