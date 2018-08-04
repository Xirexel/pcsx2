
#include "stdafx.h"
#include "Pad.h"

uint8 CALLBACK PADstartPoll(int32 pad)
{
	return g_Pad.startPoll(pad);
}

uint8 CALLBACK PADpoll(uint8 value)
{
	return g_Pad.poll(value);
}

int32 CALLBACK PADsetSlot(uint8 port, uint8 slot)
{
	return g_Pad.setSlot(port, slot);
}

PCSX2Lib::API::PAD_API g_API = {
	PADstartPoll,
	PADpoll,
	PADsetSlot
};