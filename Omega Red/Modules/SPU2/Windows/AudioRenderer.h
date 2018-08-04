#pragma once
class AudioRenderer
{
public:
	AudioRenderer();
	virtual ~AudioRenderer();

	void execute(const wchar_t* a_command, wchar_t** a_result);
};

extern AudioRenderer g_AudioRenderer;

