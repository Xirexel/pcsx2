#pragma once
class FWControl
{
public:
	FWControl();
	virtual ~FWControl();

	void execute(const wchar_t* a_command, wchar_t** a_result);
};


extern FWControl g_FWControl;