#pragma once

#include "Common\CaptureManagerTypeInfo.h"
#include "Common\ComPtrCustom.h"

class AudioRenderer
{
public:
	AudioRenderer();
	virtual ~AudioRenderer();

	void execute(const wchar_t* a_command, wchar_t** a_result);

private:

	bool m_is_muted;

    double m_volume;

	CComPtrCustom<ICaptureProcessor> m_ICaptureProcessor;
};

extern AudioRenderer g_AudioRenderer;

