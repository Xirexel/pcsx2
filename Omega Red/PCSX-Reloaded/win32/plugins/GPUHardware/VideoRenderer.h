#pragma once

#include <Unknwnbase.h>

#include <memory>
#include "Renderers/Common/GPURenderer.h"

class VideoRenderer
{
public:
    VideoRenderer();
    virtual ~VideoRenderer();
	
    void execute(const wchar_t *a_command, wchar_t **a_result);

	int open();

    void close();

private:

	UINT32 m_TexturePackMode;

    BOOL m_is_fxaa;

    std::unique_ptr<GPURenderer> m_VideoRenderer;
	
	std::wstring m_TexturePacksPath;
    std::wstring m_DiscSerial;


    int init(void *sharedhandle, void *capturehandle, void *directXDeviceNative);

	void setTexturePacksMode(UINT32 a_TexturePackMode);
    void setTexturePacksPath(const std::wstring &a_RefTexturePackPath);
    void setTexturePackCallbackHandler(int a_TexturePackCallbackHandler);
    void setDiscSerial(const std::wstring &a_RefDiscSerial);
    
};