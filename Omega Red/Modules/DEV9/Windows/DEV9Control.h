#pragma once
class DEV9Control
{
public:
	DEV9Control();
	virtual ~DEV9Control();

	void execute(const wchar_t* a_command, wchar_t** a_result);
};


extern DEV9Control g_DEV9Control;