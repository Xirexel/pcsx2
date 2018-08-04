
#include "../zerospu2.h"
#include "AudioRenderer.h"
#include "PCSX2Lib_API.h"


void CALLBACK SPU2setClockPtr(u32 *ptr)
{
	//cyclePtr = ptr;
}

PCSX2Lib::API::SPU2_API g_API = {
	SPU2write,
	SPU2read,
	SPU2readDMA4Mem,
	SPU2writeDMA4Mem,
	SPU2interruptDMA4,
	SPU2readDMA7Mem,
	SPU2writeDMA7Mem,
	SPU2setDMABaseAddr,
	SPU2interruptDMA7,
	SPU2ReadMemAddr,
	SPU2WriteMemAddr,
	SPU2irqCallback,
	SPU2setClockPtr,
	SPU2async
};