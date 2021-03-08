#include "USBControl.h"
#include "../USB.h"
#include "PCSX2Lib_API.h"
#include "pugixml.hpp"

USBControl g_USBControl;

USBControl::USBControl()
{
}

USBControl::~USBControl()
{
}

void USBControl::execute(const wchar_t* a_command, wchar_t** a_result)
{
	using namespace pugi;

	xml_document l_xmlDoc;

	auto l_XMLRes = l_xmlDoc.load_string(a_command);

	if (l_XMLRes.status == xml_parse_status::status_ok)
	{
		auto l_document = l_xmlDoc.document_element();

		if (l_document.empty())
			return;

		if (std::wstring(l_document.name()) == L"Configs")
		{
			auto l_ChildNode = l_document.first_child();

			while (!l_ChildNode.empty())
			{
				if (std::wstring(l_ChildNode.name()) == L"Open")
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
					{
						if (USBopen(l_WindowHandle)) return;
					}
				}
				l_ChildNode = l_ChildNode.next_sibling();
			}
		}
	}
}

void CALLBACK USBasync(uint32 cycles)
{

}

PCSX2Lib::API::USB_API g_API = {
	USBread8,
	USBread16,
	USBread32,
	USBwrite8,
	USBwrite16,
	USBwrite32,
	USBasync,
	USBirqCallback,
	USBirqHandler,
	USBsetRAM
};