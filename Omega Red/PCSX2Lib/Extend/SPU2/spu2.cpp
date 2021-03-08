/*  PCSX2 - PS2 Emulator for PCs
 *  Copyright (C) 2002-2020  PCSX2 Dev Team
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

#include "PrecompiledHeader.h"
#include "PCSX2Def.h"
#include "../pcsx2/SPU2/spu2.h"


extern _SPU2reset SPU2resetProxy;
extern _SPU2write SPU2writeProxy;
extern _SPU2read SPU2readProxy;
extern _SPU2async SPU2asyncProxy;
extern _SPU2readDMA4Mem SPU2readDMA4MemProxy;
extern _SPU2writeDMA4Mem SPU2writeDMA4MemProxy;
extern _SPU2interruptDMA4 SPU2interruptDMA4Proxy;
extern _SPU2readDMA7Mem SPU2readDMA7MemProxy;
extern _SPU2writeDMA7Mem SPU2writeDMA7MemProxy;
extern _SPU2irqCallback SPU2irqCallbackProxy;
extern _SPU2setClockPtr SPU2setClockPtrProxy;
extern _SPU2setDMABaseAddr SPU2setDMABaseAddrProxy;
extern _SPU2interruptDMA7 SPU2interruptDMA7Proxy;
extern _SPU2WriteMemAddr SPU2WriteMemAddrProxy;
extern _SPU2ReadMemAddr SPU2ReadMemAddrProxy;


using namespace Threading;

MutexRecursive mtx_SPU2Status;
bool SPU2_dummy_callback = false;

#include "svnrev.h"

#ifdef _MSC_VER
#define snprintf sprintf_s
#endif
int SampleRate = 48000;

static bool IsOpened = false;
static bool IsInitialized = false;

static u32 pClocks = 0;

u32* cyclePtr = nullptr;
u32 lClocks = 0;
//static bool cpu_detected = false;

static bool CheckSSE()
{
	return true;

#if 0
	if( !cpu_detected )
	{
		cpudetectInit();
		cpu_detected = true;
	}
	if( !x86caps.hasStreamingSIMDExtensions || !x86caps.hasStreamingSIMD2Extensions )
	{
		SysMessage( "Your CPU does not support SSE2 instructions.\nThe SPU2 plugin requires SSE2 to run." );
		return false;
	}
	return true;
#endif
}

void SPU2configure()
{
	if (!CheckSSE())
		return;
}

// --------------------------------------------------------------------------------------
//  DMA 4/7 Callbacks from Core Emulator
// --------------------------------------------------------------------------------------

u32 SPU2ReadMemAddr(int core)
{
	return SPU2ReadMemAddrProxy(core);
}
void SPU2WriteMemAddr(int core, u32 value)
{
	SPU2WriteMemAddrProxy(core, value);
}

void SPU2setDMABaseAddr(uptr baseaddr)
{
	SPU2setDMABaseAddrProxy(baseaddr);
}

void SPU2setSettingsDir(const char* dir)
{
}

void SPU2setLogDir(const char* dir)
{
}

void SPU2readDMA4Mem(u16* pMem, u32 size) // size now in 16bit units
{
	SPU2readDMA4MemProxy(pMem, size);
}

void SPU2writeDMA4Mem(u16* pMem, u32 size) // size now in 16bit units
{
	SPU2writeDMA4MemProxy(pMem, size);
}

void SPU2interruptDMA4()
{
	SPU2interruptDMA4Proxy();
}

void SPU2interruptDMA7()
{
	SPU2interruptDMA7Proxy();
}

void SPU2readDMA7Mem(u16* pMem, u32 size)
{
	SPU2readDMA7MemProxy(pMem, size);
}

void SPU2writeDMA7Mem(u16* pMem, u32 size)
{
	SPU2writeDMA7MemProxy(pMem, size);
}

s32 SPU2reset()
{
	SPU2resetProxy();

	return 0;
}

s32 SPU2ps1reset()
{

	return 0;
}

s32 SPU2init()
{
	return 0;
}

s32 SPU2open(void* pDsp)
{
//	ScopedLock lock(mtx_SPU2Status);
//	if (IsOpened)
//		return 0;
//
//	FileLog("[%10d] SPU2 Open\n", Cycles);
//
//	if (pDsp != nullptr)
//		gsWindowHandle = *(uptr*)pDsp;
//	else
//		gsWindowHandle = 0;
//
//#ifdef _MSC_VER
//#ifdef PCSX2_DEVBUILD // Define may not be needed but not tested yet. Better make sure.
//	if (IsDevBuild && VisualDebug())
//	{
//		if (debugDialogOpen == 0)
//		{
//			hDebugDialog = CreateDialogParam(nullptr, MAKEINTRESOURCE(IDD_DEBUG), 0, DebugProc, 0);
//			ShowWindow(hDebugDialog, SW_SHOWNORMAL);
//			debugDialogOpen = 1;
//		}
//	}
//	else if (debugDialogOpen)
//	{
//		DestroyWindow(hDebugDialog);
//		debugDialogOpen = 0;
//	}
//#endif
//#endif
//
//	IsOpened = true;
//	lClocks = (cyclePtr != nullptr) ? *cyclePtr : 0;
//
//	try
//	{
//		SndBuffer::Init();
//
//#ifndef __POSIX__
//		DspLoadLibrary(dspPlugin, dspPluginModule);
//#endif
//		WaveDump::Open();
//	}
//	catch (std::exception& ex)
//	{
//		fprintf(stderr, "SPU2 Error: Could not initialize device, or something.\nReason: %s", ex.what());
//		SPU2close();
//		return -1;
//	}
//	SPU2setDMABaseAddr((uptr)iopMem->Main);
//	SPU2setClockPtr(&psxRegs.cycle);
	return 0;
}

void SPU2close()
{
}

void SPU2shutdown()
{
}

void SPU2setClockPtr(u32* ptr)
{
	SPU2setClockPtrProxy(ptr);
}

#ifdef DEBUG_KEYS
static u32 lastTicks;
static bool lState[6];
#endif

void SPU2async(u32 cycles)
{
	SPU2asyncProxy(cycles);
}

u16 SPU2read(u32 rmem)
{
	return SPU2readProxy(rmem);
}

void SPU2write(u32 rmem, u16 value)
{
	SPU2writeProxy(rmem, value);
}

// if start is 1, starts recording spu2 data, else stops
// returns a non zero value if successful
// for now, pData is not used
int SPU2setupRecording(int start, std::wstring* filename)
{
	return 0;
}

s32 SPU2freeze(int mode, freezeData* data)
{

	throw "Unimplemented!!!";
	//pxAssume(data != nullptr);
	//if (!data)
	//{
	//	printf("SPU2 savestate null pointer!\n");
	//	return -1;
	//}

	//if (mode == FREEZE_SIZE)
	//{
	//	data->size = SPU2Savestate::SizeIt();
	//	return 0;
	//}

	//pxAssume(mode == FREEZE_LOAD || mode == FREEZE_SAVE);

	//if (data->data == nullptr)
	//{
	//	printf("SPU2 savestate null pointer!\n");
	//	return -1;
	//}

	//SPU2Savestate::DataBlock& spud = (SPU2Savestate::DataBlock&)*(data->data);

	//switch (mode)
	//{
	//	case FREEZE_LOAD:
	//		return SPU2Savestate::ThawIt(spud);
	//	case FREEZE_SAVE:
	//		return SPU2Savestate::FreezeIt(spud);

	//		jNO_DEFAULT;
	//}

	//// technically unreachable, but kills a warning:
	//return 0;
}

void SPU2DoFreezeOut(void* dest)
{
	throw "Unimplemented!!!";
	ScopedLock lock(mtx_SPU2Status);

	freezeData fP = {0, (s8*)dest};
	if (SPU2freeze(FREEZE_SIZE, &fP) != 0)
		return;
	if (!fP.size)
		return;

	Console.Indent().WriteLn("Saving SPU2");

	if (SPU2freeze(FREEZE_SAVE, &fP) != 0)
		throw std::runtime_error(" * SPU2: Error saving state!\n");
}


void SPU2DoFreezeIn(pxInputStream& infp)
{

	throw "Unimplemented!!!";
	//ScopedLock lock(mtx_SPU2Status);

	//freezeData fP = {0, nullptr};
	//if (SPU2freeze(FREEZE_SIZE, &fP) != 0)
	//	fP.size = 0;

	//Console.Indent().WriteLn("Loading SPU2");

	//if (!infp.IsOk() || !infp.Length())
	//{
	//	// no state data to read, but SPU2 expects some state data?
	//	// Issue a warning to console...
	//	if (fP.size != 0)
	//		Console.Indent().Warning("Warning: No data for SPU2 found. Status may be unpredictable.");

	//	return;

	//	// Note: Size mismatch check could also be done here on loading, but
	//	// some plugins may have built-in version support for non-native formats or
	//	// older versions of a different size... or could give different sizes depending
	//	// on the status of the plugin when loading, so let's ignore it.
	//}

	//ScopedAlloc<s8> data(fP.size);
	//fP.data = data.GetPtr();

	//infp.Read(fP.data, fP.size);
	//if (SPU2freeze(FREEZE_LOAD, &fP) != 0)
	//	throw std::runtime_error(" * SPU2: Error loading state!\n");
}
