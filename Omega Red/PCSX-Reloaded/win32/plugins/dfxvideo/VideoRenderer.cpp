#include "CommonCPP.h"
#include "VideoRenderer.h"
#include <cstdint>

#include "pugixml.hpp" 
#include "GSDevice.h"


extern GSDevice g_GSDevice;

BOOL g_is_fxaa;

VideoRenderer g_VideoRenderer;

VideoRenderer::VideoRenderer()
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

                } else if (std::wstring(l_ChildNode.name()) == L"Shutdown") {
                    shutdown();
                } else if (std::wstring(l_ChildNode.name()) == L"IsFXAA") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    g_is_fxaa = l_Attribute.as_uint(0);

                    if (!l_Attribute.empty()) {
                        g_GSDevice.setFXAA(g_is_fxaa);
                    }
                }

                l_ChildNode = l_ChildNode.next_sibling();
            }
        }
    }
}

int VideoRenderer::init(void *sharedhandle, void *capturehandle, void *directXDeviceNative)
{
    int l_result = -1;

	do {

		g_GSDevice.Create(sharedhandle, capturehandle, directXDeviceNative);

    } while (false);

	return l_result;
}

void VideoRenderer::shutdown()
{
}

extern "C" void executeExecute(const wchar_t *a_command, wchar_t **a_result)
{
    g_VideoRenderer.execute(a_command, a_result);
}