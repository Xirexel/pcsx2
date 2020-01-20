#include "stdafx.h"
#include "CommonCPP.h"
#include "VideoRenderer.h"
#include "Window/GSWndDX.h"
#include "Renderers/DX11/GPURendererDX11.h"
#include "Renderers/DX11/GPUDevice11.h"

#include <cstdint>
#include <algorithm>

#include "pugixml.hpp" 


#define EXPORT_C_(type) extern "C" type __stdcall
#define EXPORT_C EXPORT_C_(void)

VideoRenderer g_VideoRenderer;

static void* s_sharedhandle = nullptr;
static void *s_capturehandle = nullptr;
static void *s_directXDeviceNative = nullptr;


CallbackDelegate s_CallbackDelegate = NULL;

VideoRenderer::VideoRenderer():
	m_TexturePackMode(0),
	m_TexturePacksPath(L"")
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
    //m_TexturePackMode = a_TexturePackMode;

	//if (m_TexturePackMode == 0)
        iHiResTextures = 1;
    //else
    //    iHiResTextures = 0;

	if (m_VideoRenderer)
        m_VideoRenderer->setTexturePacksMode(m_TexturePackMode);
}

void VideoRenderer::setTexturePacksPath(const std::wstring &a_RefTexturePacksPath)
{
    m_TexturePacksPath = a_RefTexturePacksPath;
		
    if (m_VideoRenderer)
        m_VideoRenderer->setTexturePacksPath(m_TexturePacksPath + (m_TexturePackMode == 2 ? L"\\Dump" : L"") + L"\\" + m_DiscSerial + L"\\");
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
    if (s_sharedhandle == nullptr ||
        s_capturehandle == nullptr)
        return -1;

    std::unique_ptr<GPUDevice> dev;

    std::shared_ptr<GSWnd> window = std::make_shared<GSWndDX>();

    //window->Create(L"DirectX11 window", 800, 600);

    //window->Show();

    dev = std::make_unique<GPUDevice11>();

    m_VideoRenderer = std::make_unique<GPURendererDX11>();

    m_VideoRenderer->m_wnd = window;

    if (!m_VideoRenderer->CreateDevice(dev.get(), s_sharedhandle, s_capturehandle, s_directXDeviceNative)) {
        return -1;
    }

    dev.release();

    s_gpu = m_VideoRenderer.get();

	setTexturePacksMode(m_TexturePackMode);

	setTexturePacksPath(m_TexturePacksPath);
 
    return 0;
}

void VideoRenderer::close()
{
    m_VideoRenderer.reset();
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