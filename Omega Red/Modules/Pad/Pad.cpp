#include "stdafx.h"
#include "Pad.h"
#include "LilyPad\LilyPad.h"

static LilyPad s_LilyPad;

void *ptrVibrationCallback = nullptr;



void Pad::execute(const wchar_t* a_command, wchar_t** a_result)
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
					u32 l_ports = 1;
					
					auto l_Attribute = l_ChildNode.attribute(L"Ports");

					if (!l_Attribute.empty())
					{
						l_ports = l_Attribute.as_uint(l_ports);
					}

                    l_Attribute = l_ChildNode.attribute(L"VibrationCallback");

                    if (!l_Attribute.empty()) {

                        auto l_value = l_Attribute.as_llong();

                        if (l_value != 0) {
                            try {

                                ptrVibrationCallback = (void *)l_value;

                            } catch (...) {
                            }
                        }
                    }

					s_LilyPad.init(l_ports, l_ChildNode);
				}
				else if (std::wstring(l_ChildNode.name()) == L"Open")
				{
					s_LilyPad.open();
				}
				else if (std::wstring(l_ChildNode.name()) == L"Close")
				{
					s_LilyPad.close();
				}
				else if (std::wstring(l_ChildNode.name()) == L"Shutdown")
				{
					s_LilyPad.shutdown();
				}

				l_ChildNode = l_ChildNode.next_sibling();
			}

		}
	}
}

uint8 Pad::startPoll(int32 port)
{
	return s_LilyPad.startPoll(port);
}

uint8 Pad::poll(uint8 value)
{
	return s_LilyPad.poll(value);
}

int32 Pad::setSlot(uint8 port, uint8 slot)
{
	return s_LilyPad.setSlot(port, slot);
}




Pad g_Pad;