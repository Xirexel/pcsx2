
#pragma once

#include "stdafx.h"

class LilyPad
{
	// Used to toggle mouse listening.
	u8 miceEnabled;

	int openCount;

	int activeWindow = 0;

	void UpdateEnabledDevices(int updateList = 0);

	void Update(unsigned int port, unsigned int slot);

public:

	LilyPad();

	s32 init(u32 flags, pugi::xml_node& a_init_node);

	s32 open();

	void close();

	void shutdown();

	uint8 startPoll(int32 pad);

	uint8 poll(uint8 value);

	int32 setSlot(uint8 port, uint8 slot);
};
