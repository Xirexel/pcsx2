#pragma once
class USBControl
{
public:
	USBControl();
	virtual ~USBControl();

	void execute(const wchar_t* a_command, wchar_t** a_result);
};


extern USBControl g_USBControl;