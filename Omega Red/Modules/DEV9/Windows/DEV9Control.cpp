#include "DEV9Control.h"
#include "../DEV9.h"
#include "PCSX2Lib_API.h"
#include "pugixml.hpp"

DEV9Control g_DEV9Control;

DEV9Control::DEV9Control()
{
}

DEV9Control::~DEV9Control()
{
}

void DEV9Control::execute(const wchar_t* a_command, wchar_t** a_result)
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
						if (DEV9open(l_WindowHandle)) return;
					}
				}
				l_ChildNode = l_ChildNode.next_sibling();
			}
		}
	}
}

void CALLBACK DEV9async(uint32 cycles)
{

}

PCSX2Lib::API::DEV9_API g_API = {
	DEV9read8,
	DEV9read16,
	DEV9read32,
	DEV9write8,
	DEV9write16,
	DEV9write32,
	DEV9readDMA8Mem,
	DEV9writeDMA8Mem,
	DEV9irqCallback,
	DEV9irqHandler,
	DEV9async
};