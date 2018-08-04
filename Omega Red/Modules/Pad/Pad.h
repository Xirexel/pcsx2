#pragma once
class Pad
{
public:

	void execute(const wchar_t* a_command, wchar_t** a_result);

	uint8 startPoll(int32 pad);

	uint8 poll(uint8 value);

	int32 setSlot(uint8 port, uint8 slot);
};

extern Pad g_Pad;