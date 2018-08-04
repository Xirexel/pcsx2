#include "../zerospu2.h"
#include "AudioRenderer.h"
#include "pugixml.hpp"



AudioRenderer g_AudioRenderer;

AudioRenderer::AudioRenderer()
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
				}

				l_ChildNode = l_ChildNode.next_sibling();
			}

		}
	}
}


