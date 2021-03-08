
#include "stdafx.h"
#include "GPUDevice.h"

extern "C" {
	uint64_t g_currentClutId;
}

GPUDevice *GPUDevice::s_instance = nullptr;

GPUDevice::GPUDevice()
    : m_ResolitionX(0)
    , m_ResolitionY(0)
{
    s_instance = this;
}

GPUDevice::~GPUDevice()
{
}

bool GPUDevice::Create(const std::shared_ptr<GSWnd> &wnd, void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
    m_wnd = wnd;

    return true;
}

extern "C" void _stdcall SetTextureFunc(void *a_PtrMemory, LPSTR a_StringIDs)
{
    GPUDevice::s_instance->setRawTexture(a_PtrMemory, a_StringIDs);
}

void GPUDevice::setTexturePackMode(UINT32 a_TexturePackMode)
{
    m_TexturePackMode = (TexturePackMode)a_TexturePackMode;

	if (s_CallbackDelegate != nullptr)
	{
        m_ResolitionX = s_CallbackDelegate(1, 0);

        m_ResolitionY = s_CallbackDelegate(2, 0);
		
	
		s_CallbackDelegate(3, (UINT32)(SetTextureFunc));
	}
}

void GPUDevice::setTexturePacksPath(const std::wstring &a_RefTexturePacksPath)
{
    m_TexturePacksPath = a_RefTexturePacksPath;
}