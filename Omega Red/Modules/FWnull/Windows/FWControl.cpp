#include "FWControl.h"
#include "../FW.h"
#include "PCSX2Lib_API.h"
#include "pugixml.hpp"

FWControl g_FWControl;

FWControl::FWControl()
{
}

FWControl::~FWControl()
{
}

void FWControl::execute(const wchar_t* a_command, wchar_t** a_result)
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
					FWsetSettingsDir(nullptr);

					FWinit();
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
						FWopen(l_WindowHandle);
				}
				else if (std::wstring(l_ChildNode.name()) == L"Close")
				{
					FWclose();
				}
				else if (std::wstring(l_ChildNode.name()) == L"Shutdown")
				{
					FWshutdown();
				}

				l_ChildNode = l_ChildNode.next_sibling();
			}
		}
	}
}



PCSX2Lib::API::FW_API g_API = {
	FWread32,
	FWwrite32,
	FWirqCallback
};