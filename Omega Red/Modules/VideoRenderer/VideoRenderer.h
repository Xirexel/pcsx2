#pragma once

#include <Unknwnbase.h>

#include <memory>
#include "GS\GSRendererDX11.h"

typedef void(*Action)();

class VideoRenderer
{
	uint8* m_BaseMem;

	Action m_Irq;

	int m_AspectRatio;

	std::unique_ptr<GSRendererDX11> m_VideoRenderer;

	int init(void* sharedhandle, void* updateCallback);

	void shutdown();
	
public:
	VideoRenderer();
	virtual ~VideoRenderer();

	void execute(const wchar_t* a_command, wchar_t** a_result);

	void setBaseMem(void * a_ptr);

	void setIrqCallback(Action a_callback);

	void reset();

	void setVsync(bool a_state);

	void setGameCRC(int crc, int options);

	void setFrameSkip(int skip);

	void vsync(int field);

	void gifTransfer(const uint8* mem, uint32 size);

	void readFIFO2(uint8 *pMem, int32 qwc);

	void initReadFIFO2(uint8 *pMem, int32 qwc);

	void gifSoftReset(uint32 mask);
};

extern VideoRenderer g_VideoRenderer;