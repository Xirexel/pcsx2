#include "../zerospu2.h"
#include "../Targets/dsound51.h"
#include "AudioRenderer.h"
#include "AudioCaptureProcessor.h"
#include "pugixml.hpp"
#include <math.h>
#include <sstream>

#define DSBVOLUME_MIN -10000
#define DSBVOLUME_MAX 0

AudioRenderer g_AudioRenderer;

AudioRenderer::AudioRenderer(): 
	m_is_muted(0)
    , m_volume(0.5)
{
}

AudioRenderer::~AudioRenderer()
{
}

void AudioRenderer::execute(const wchar_t* a_command, wchar_t** a_result)
{
	using namespace pugi;

	xml_document l_xmlDoc;

	auto l_XMLRes = l_xmlDoc.load_string(a_command);

	if (l_XMLRes.status == xml_parse_status::status_ok)
	{
		auto l_document = l_xmlDoc.document_element();

		if (l_document.empty())
			return;

		if (std::wstring(l_document.name()) == L"Config")
		{
			auto l_ChildNode = l_document.first_child();

			while (!l_ChildNode.empty())
			{
				if (std::wstring(l_ChildNode.name()) == L"Init")
				{
					SPU2setSettingsDir(nullptr);

					SPU2init();

					CComPtrCustom<ICaptureProcessor> l_ICaptureProcessor(new AudioCaptureProcessor());

					m_ICaptureProcessor = l_ICaptureProcessor;
				}
				else if (std::wstring(l_ChildNode.name()) == L"Open")
				{

					void* l_WindowHandle = nullptr;

					auto l_Attribute = l_ChildNode.attribute(L"WindowHandle");

					if (!l_Attribute.empty())
					{
						auto l_value = l_Attribute.as_llong();

						if (l_value != 0)
						{
							try
							{

								l_WindowHandle = (void*)l_value;

							}
							catch (...)
							{

							}

						}
					}

					if (l_WindowHandle != nullptr)
						SPU2open(l_WindowHandle);					
				}
				else if (std::wstring(l_ChildNode.name()) == L"Close")
				{
					SPU2close();
				}
				else if (std::wstring(l_ChildNode.name()) == L"Shutdown")
				{
					SPU2shutdown();

					m_ICaptureProcessor.Release();

                    g_ISourceRequestResult.Release();
				}
				else if (std::wstring(l_ChildNode.name()) == L"DoFreeze")
				{

					freezeData* l_FreezeDataHandle = nullptr;

					auto l_Attribute = l_ChildNode.attribute(L"FreezeData");

					if (!l_Attribute.empty())
					{
						auto l_value = l_Attribute.as_llong();

						if (l_value != 0)
						{
							try
							{

								l_FreezeDataHandle = (freezeData*)l_value;

							}
							catch (...)
							{

							}

						}
					}

					int l_mode = -1;

					l_Attribute = l_ChildNode.attribute(L"Mode");

					if (!l_Attribute.empty())
					{
						auto l_value = l_Attribute.as_int(-1);

						if (l_value != -1)
						{
							l_mode = l_value;
						}
					}

					if (l_FreezeDataHandle != nullptr)
					{
						SPU2freeze(l_mode, l_FreezeDataHandle);
					}
                } else if (std::wstring(l_ChildNode.name()) == L"Volume") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    m_volume = l_Attribute.as_double(0.5);

					if (m_volume > 0)
                        m_volume = exp2(exp2(m_volume)/2.0)/2.0;


                    if (!l_Attribute.empty()) {

						if (!m_is_muted)
							DSSetVolume(((m_volume * -1) * (double)DSBVOLUME_MIN) + DSBVOLUME_MIN);
                        else
                            DSSetVolume(DSBVOLUME_MIN);

                    }
                } else if (std::wstring(l_ChildNode.name()) == L"IsMuted") {

                    auto l_Attribute = l_ChildNode.attribute(L"Value");

                    m_is_muted = l_Attribute.as_uint(0);

                    if (!l_Attribute.empty()) {

                        if (!m_is_muted)
                            DSSetVolume(((m_volume * -1) * (double)DSBVOLUME_MIN) + DSBVOLUME_MIN);
                        else
                            DSSetVolume(DSBVOLUME_MIN);
                    }
                }

				l_ChildNode = l_ChildNode.next_sibling();
			}

		} 
		else if (std::wstring(l_document.name()) == L"Commands") {
            auto l_ChildNode = l_document.first_child();

            xml_document l_xmlResultDoc;

            auto l_declNode = l_xmlResultDoc.append_child(node_declaration);

            l_declNode.append_attribute(L"version") = L"1.0";

            xml_node l_commentNode = l_xmlResultDoc.append_child(node_comment);

            l_commentNode.set_value(L"XML Document of results");

            auto l_RootXMLElement = l_xmlResultDoc.append_child(L"Results");

            while (!l_ChildNode.empty()) {
                auto l_resultXMLElement = l_RootXMLElement.append_child(L"Result");

                l_resultXMLElement.append_attribute(L"Command").set_value(l_ChildNode.name());

                if (std::wstring(l_ChildNode.name()) == L"GetCaptureProcessor") {
                    bool l_isValid = false;

                    if (m_ICaptureProcessor != nullptr) {

                        wchar_t lvalue[256];

                        _itow_s((DWORD)m_ICaptureProcessor.get(), lvalue, 10);

                        l_resultXMLElement.append_attribute(L"Value").set_value(lvalue);

                        l_isValid = true;
                    }

                    l_resultXMLElement.append_attribute(L"State").set_value(l_isValid);
                }

                l_ChildNode = l_ChildNode.next_sibling();
            }

            if (a_result != nullptr) {
                std::wstringstream l_wstringstream;

                l_xmlResultDoc.print(l_wstringstream);

                auto l_XMLDocumentString = l_wstringstream.str();

                *a_result = new wchar_t[l_XMLDocumentString.size() + 1];

                wcscpy_s(*a_result, l_XMLDocumentString.size() + 1, l_XMLDocumentString.c_str());
            }
        }
	}
}


