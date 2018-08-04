#pragma once

class CDVDinner
{
public:
	
	void execute(const wchar_t* a_command, wchar_t** a_result);
};

extern CDVDinner g_CDVD;