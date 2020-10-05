#include "stdafx.h"
#include "CommonCPP.h"
#include "VideoRenderer.h"
#include "Window/GSWndDX.h"
#include "Renderers/DX11/GPURendererDX11.h"
#include "Renderers/DX11/GPUDevice11.h"
#include "GPUPng.h"
#include "crc.h"

#include <cstdint>
#include <algorithm>

#include "pugixml.hpp" 


#define EXPORT_C_(type) extern "C" type __stdcall
#define EXPORT_C EXPORT_C_(void)

VideoRenderer g_VideoRenderer;

static void* s_sharedhandle = nullptr;
static void *s_capturehandle = nullptr;
static void *s_directXDeviceNative = nullptr;

extern "C" BOOL bUseFrameLimit;
static std::wstring s_TexturePacksPath;
static UINT32 s_TexturePackMode = 0;

enum TexturePackMode {
    NONE = 0,
    LOAD = NONE + 1,
    SAVE = LOAD + 1
};


CallbackDelegate s_CallbackDelegate = NULL;

VideoRenderer::VideoRenderer():
	m_TexturePacksPath(L"")
	,m_is_fxaa(FALSE)
{
}

VideoRenderer::~VideoRenderer()
{
}

void VideoRenderer::execute(const wchar_t *a_command, wchar_t **a_result)
{
    using namespace pugi;

    xml_document l_xmlDoc;

    auto l_XMLRes = l_xmlDoc.load_string(a_command);

    if (l_XMLRes.status == xml_parse_status::status_ok) {
        auto l_document = l_xmlDoc.document_element();

        if (l_document.empty())
            return;

        if (std::wstring(l_document.name()) == L"Config") {
            auto l_ChildNode = l_document.first_child();

            while (!l_ChildNode.empty()) {
                if (std::wstring(l_ChildNode.name()) == L"Init") {
                    void *l_SharedHandle = nullptr;

                    auto l_Attribute = l_ChildNode.attribute(L"ShareHandler");

                    if (!l_Attribute.empty()) {
                        auto l_value = l_Attribute.as_llong();

                        if (l_value != 0) {
                            try {

                                l_SharedHandle = (void *)l_value;

                            } catch (...) {
                            }
                        }
                    }

                    void *l_CaptureHandler = nullptr;

                    l_Attribute = l_ChildNode.attribute(L"CaptureHandler");

                    if (!l_Attribute.empty()) {
                        auto l_value = l_Attribute.as_llong();

                        if (l_value != 0) {
                            try {

                                l_CaptureHandler = (void *)l_value;

                            } catch (...) {
                            }
                        }
                    }

                    void *l_DirectXDeviceNative = nullptr;

                    l_Attribute = l_ChildNode.attribute(L"DirectXDeviceNative");

                    if (!l_Attribute.empty()) {
                        auto l_value = l_Attribute.as_llong();

                        if (l_value != 0) {
                            try {

                                l_DirectXDeviceNative = (void *)l_value;

                            } catch (...) {
                            }
                        }
                    }

                    if (l_SharedHandle != nullptr)
                        init(l_SharedHandle, l_CaptureHandler, l_DirectXDeviceNative);
								
                    l_Attribute = l_ChildNode.attribute(L"TexturePacksPath");

                    if (!l_Attribute.empty()) {
                        setTexturePacksPath(l_Attribute.value());
                    }

                    l_Attribute = l_ChildNode.attribute(L"TexturePackCallbackHandler");

                    if (!l_Attribute.empty()) {
                        setTexturePackCallbackHandler(l_Attribute.as_int(0));
                    }					
					
                } else if (std::wstring(l_ChildNode.name()) == L"Shutdown") {

                } else if (std::wstring(l_ChildNode.name()) == L"IsFXAA") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    m_is_fxaa = l_Attribute.as_uint(0);

                    if (!l_Attribute.empty()) {
                        if (m_VideoRenderer) {
                            m_VideoRenderer->setFXAA(m_is_fxaa);
                        }
                    }
					
                } else if (std::wstring(l_ChildNode.name()) == L"IsFrameLimit") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    if (!l_Attribute.empty()) {
						
                        bUseFrameLimit = l_Attribute.as_uint(0);
                    }
					

                } else if (std::wstring(l_ChildNode.name()) == L"TexturePackMode") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    setTexturePacksMode(l_Attribute.as_uint(0));
                } else if (std::wstring(l_ChildNode.name()) == L"DiscSerial") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    setDiscSerial(l_Attribute.value());
                }

				

                l_ChildNode = l_ChildNode.next_sibling();
            }
        }
    }
}

int VideoRenderer::init(void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
    s_sharedhandle = sharedhandle;
    s_capturehandle = capturehandle;
    s_directXDeviceNative = directXDeviceNative;

	return 0;
}

void VideoRenderer::setTexturePacksMode(UINT32 a_TexturePackMode)
{
    s_TexturePackMode = a_TexturePackMode;
	
    iHiResTextures = 1;

	if (m_VideoRenderer)
            m_VideoRenderer->setTexturePacksMode(s_TexturePackMode);
}

void VideoRenderer::setTexturePacksPath(const std::wstring &a_RefTexturePacksPath)
{
    m_TexturePacksPath = a_RefTexturePacksPath;
	
    s_TexturePacksPath = m_TexturePacksPath + (s_TexturePackMode == 2 ? L"\\Dump" : L"") + L"\\" + m_DiscSerial + L"\\";
		
    if (m_VideoRenderer)
        m_VideoRenderer->setTexturePacksPath(s_TexturePacksPath);
}

void VideoRenderer::setTexturePackCallbackHandler(int a_TexturePackCallbackHandler)
{
    s_CallbackDelegate = NULL;

	if (a_TexturePackCallbackHandler == 0)
        return;

    s_CallbackDelegate = (CallbackDelegate)a_TexturePackCallbackHandler;
}



void VideoRenderer::setDiscSerial(const std::wstring &a_RefDiscSerial)
{
    m_DiscSerial = a_RefDiscSerial;

    if (m_VideoRenderer)
        m_VideoRenderer->setTexturePacksPath(m_TexturePacksPath + L"\\" + m_DiscSerial + L"\\");
}

int VideoRenderer::open()
{
    if (s_sharedhandle == nullptr)
        return -1;

    std::unique_ptr<GPUDevice> dev;

    std::shared_ptr<GSWnd> window = std::make_shared<GSWndDX>();
	
    dev = std::make_unique<GPUDevice11>();

    m_VideoRenderer = std::make_unique<GPURendererDX11>();

    m_VideoRenderer->m_wnd = window;

    if (!m_VideoRenderer->CreateDevice(dev.get(), s_sharedhandle, s_capturehandle, s_directXDeviceNative)) {
        return -1;
    }

    dev.release();

    s_gpu = m_VideoRenderer.get();

	setTexturePacksMode(s_TexturePackMode);

	setTexturePacksPath(m_TexturePacksPath);

    m_VideoRenderer->setFXAA(m_is_fxaa);	
 
    return 0;
}

void VideoRenderer::close()
{
    m_VideoRenderer.reset();
}

extern "C" void saveTexture(INT32 aXoffset, INT32 aYoffset, INT32 aWidth, INT32 aHeight, DWORD aFormat, DWORD aType, const void *aPtrPixels)
{
    if (s_TexturePackMode != TexturePackMode::SAVE)
        return;


    int iTSize = 256;

    UINT32 l_source_pixel_bytes = 4;

    if ((aFormat == GL_RGB) || aFormat == GL_BGR_EXT)
        l_source_pixel_bytes = 3;

    if (aType == GL_UNSIGNED_SHORT_5_5_5_1_EXT)
        l_source_pixel_bytes = 2;

    GPUPng::Format l_format = GPUPng::RGBA_PNG;


    if (l_source_pixel_bytes == 3)
        l_format = GPUPng::RGB_PNG;
    if (l_source_pixel_bytes == 2)
        l_format = GPUPng::R16I_PNG;

	
	
	INT32 l_sourceRowPitch = l_source_pixel_bytes * aWidth;

    auto l_RowPitch = l_source_pixel_bytes * iTSize;


    std::unique_ptr<uint8[]> l_data(new uint8[l_RowPitch * iTSize]);

    ZeroMemory(l_data.get(), l_RowPitch * iTSize);

    auto l_ptrdata = l_data.get();

    uint8 *l_sourcePixels = (uint8 *)aPtrPixels;
	
    l_ptrdata += (aYoffset * l_RowPitch + aXoffset * l_source_pixel_bytes);

    for (size_t h = 0; h < aHeight; h++) {

        memcpy(l_ptrdata, l_sourcePixels, l_sourceRowPitch);

        l_sourcePixels += l_sourceRowPitch;

        l_ptrdata += l_RowPitch;
    }

	DWORD l_crc;
	
	crc32compute(l_data.get(), l_RowPitch * iTSize, TRUE, &l_crc);

    wchar_t l_stringID[256];

    _ui64tow_s(l_crc, l_stringID, 256, 16);

    std::wstring l_path = std::wstring(L"C:\\Users\\evgen\\Documents\\Images\\") + l_stringID + L".png";
	   
    GPUPng::Save(GPUPng::RGBA_PNG, l_path, l_data.get(), iTSize, iTSize, l_RowPitch, 9, true);
}

extern "C" void executeExecute(const wchar_t *a_command, wchar_t **a_result)
{
    g_VideoRenderer.execute(a_command, a_result);
}

EXPORT_C_(int32) GPU_Stub_open(void *hWnd)
{
    g_VideoRenderer.open();

    return 0;
}

EXPORT_C_(int32) GPU_Stub_close()
{
    g_VideoRenderer.close();

    return 0;
}