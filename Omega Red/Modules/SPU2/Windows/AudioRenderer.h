#pragma once

class AudioRenderer
{
public:
	AudioRenderer();
	virtual ~AudioRenderer();

	void execute(const wchar_t* a_command, wchar_t** a_result);

private:

	bool m_is_muted;

    double m_volume;
};

extern AudioRenderer g_AudioRenderer;

