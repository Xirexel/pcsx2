
//#include "stdafx.h"
#include "Pcsx2Defs.h"
#include "VideoRenderer.h"
#include "PCSX2Lib_API.h"



int32 CALLBACK GScallbackopen()
{
	g_VideoRenderer.reset();

	return 0;
}

void CALLBACK GSvsync(int field)
{
	g_VideoRenderer.vsync(field);
}

void CALLBACK GSgifTransfer(const uint8 *pMem, uint32 size)
{
	g_VideoRenderer.gifTransfer(pMem, size);
}

void CALLBACK GSirqCallback(void(*callback)())
{
	g_VideoRenderer.setIrqCallback(callback);
}

void CALLBACK GSsetBaseMem(void * a_ptr)
{
	g_VideoRenderer.setBaseMem(a_ptr);
}

void CALLBACK GSsetGameCRC(int crc, int options)
{
	g_VideoRenderer.setGameCRC(crc, options);
}

void CALLBACK GSsetFrameSkip(int frameskip)
{
	g_VideoRenderer.setFrameSkip(frameskip);
}

void CALLBACK GSsetVsync(int enabled)
{
	g_VideoRenderer.setVsync(enabled != 0);
}

void CALLBACK GSreset()
{
	g_VideoRenderer.reset();
}

void CALLBACK GSinitReadFIFO(uint8 *pMem)
{
	g_VideoRenderer.initReadFIFO2(pMem, 1);
}

void CALLBACK GSreadFIFO(uint8 *pMem)
{
	g_VideoRenderer.readFIFO2(pMem, 1);
}

void CALLBACK GSinitReadFIFO2(uint8 *pMem, int32 qwc)
{
	g_VideoRenderer.initReadFIFO2(pMem, qwc);
}

void CALLBACK GSreadFIFO2(uint8 *pMem, int32 qwc)
{
	g_VideoRenderer.readFIFO2(pMem, qwc);
}

void CALLBACK GSgifSoftReset(uint32 mask)
{
	g_VideoRenderer.gifSoftReset(mask);
}


PCSX2Lib::API::GS_API g_API = {
	GScallbackopen,
	GSvsync,
	GSgifTransfer,
	GSirqCallback,
	GSsetBaseMem,
	GSsetGameCRC,
	GSsetFrameSkip,
	GSsetVsync,
	GSreset,
	GSinitReadFIFO,
	GSreadFIFO,
	GSinitReadFIFO2,
	GSreadFIFO2,
	GSgifSoftReset
};