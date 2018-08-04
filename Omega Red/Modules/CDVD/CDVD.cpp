// CDVD.cpp : Defines the exported functions for the DLL application.
//

#include "PrecompiledHeader.h"
#include "CDVD.h"
#include "CDVD\CDVDaccess.h"
#include "CDVD\CDVDisoReader.h"
#include "pugixml.hpp"
#include "wx\string.h"

#include <sstream>

CDVDinner g_CDVD;

void CDVDinner::execute(const wchar_t* a_command, wchar_t** a_result)
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
				if (std::wstring(l_ChildNode.name()) == L"Open")
				{
					auto l_Attribute = l_ChildNode.attribute(L"FilePath");

					if (!l_Attribute.empty())
					{
						wxString l_file_path(l_Attribute.value());

						CDVDapi_Iso.open(l_file_path.c_str());
					}
				}
				else
				if (std::wstring(l_ChildNode.name()) == L"Close")
				{
					CDVDapi_Iso.close();
				}

				l_ChildNode = l_ChildNode.next_sibling();
			}
		}
		else if (std::wstring(l_document.name()) == L"Commands")
		{
			auto l_ChildNode = l_document.first_child();

			xml_document l_xmlResultDoc;

			auto l_declNode = l_xmlResultDoc.append_child(node_declaration);

			l_declNode.append_attribute(L"version") = L"1.0";

			xml_node l_commentNode = l_xmlResultDoc.append_child(node_comment);

			l_commentNode.set_value(L"XML Document of results");

			auto l_RootXMLElement = l_xmlResultDoc.append_child(L"Results");

			while (!l_ChildNode.empty())
			{
				auto l_resultXMLElement = l_RootXMLElement.append_child(L"Result");
				
				l_resultXMLElement.append_attribute(L"Command").set_value(l_ChildNode.name());

				if (std::wstring(l_ChildNode.name()) == L"Check")
				{
					bool l_isValid = false;

					auto l_Attribute = l_ChildNode.attribute(L"FilePath");

					l_resultXMLElement.append_attribute(L"FilePath").set_value(l_Attribute.value());

					if (!l_Attribute.empty())
					{
						wxString l_file_path(l_Attribute.value());

						InputIsoFile l_ISOFile;

						isoType l_isoType = isoType::ISOTYPE_ILLEGAL;

						try {
							l_isValid = l_ISOFile.Open(l_file_path.c_str(), true);

							l_isoType = l_ISOFile.GetType();

							l_ISOFile.Close();
						}
						catch (BaseException& ex)
						{
							Console.Error(ex.FormatDiagnosticMessage());
							l_isValid = false;
						}

						std::wstring l_stringIsoType;

						switch (l_isoType)
						{
						case ISOTYPE_CD:
							l_stringIsoType = L"ISOTYPE_CD";
							break;
						case ISOTYPE_DVD:
							l_stringIsoType = L"ISOTYPE_DVD";
							break;
						case ISOTYPE_AUDIO:
							l_stringIsoType = L"ISOTYPE_AUDIO";
							break;
						case ISOTYPE_DVDDL:
							l_stringIsoType = L"ISOTYPE_DVDDL";
							break;
						case ISOTYPE_ILLEGAL:
						default:
							l_stringIsoType = L"ISOTYPE_ILLEGAL";
							break;
						}

						l_resultXMLElement.append_attribute(L"IsoType").set_value(l_stringIsoType.c_str());
					}

					l_resultXMLElement.append_attribute(L"State").set_value(l_isValid);
				}
				else
				if (std::wstring(l_ChildNode.name()) == L"GetDiscSerial")
				{					
					auto l_Attribute = l_ChildNode.attribute(L"FilePath");

					l_resultXMLElement.append_attribute(L"FilePath").set_value(l_Attribute.value());

					if (!l_Attribute.empty())
					{

						std::wstring l_DiscSerial;
						
						std::wstring l_gameDiscType;

						std::wstring l_discRegionType;

						std::wstring l_software_version;

						u32 l_ElfCRC = 0;

						extern int GetPS2ElfName(
							std::wstring& a_name,
							std::wstring& a_discRegionType,
							std::wstring& a_software_version,
							u32& a_ElfCRC,
							const std::wstring& a_file_path);

						auto l_gameDiskTypeId = GetPS2ElfName(
							l_DiscSerial,
							l_discRegionType,
							l_software_version,
							l_ElfCRC,
							l_Attribute.value());

						switch (l_gameDiskTypeId)
						{
						case 1:
							l_gameDiscType = L"PS1 Disc";
							break;
						case 2:
							l_gameDiscType = L"PS2 Disc";
							break;
						default:
							l_gameDiscType = L"Invalid or unknown disc.";
							break;
						}
						
						// return value:
						//   0 - Invalid or unknown disc.
						//   1 - PS1 CD
						//   2 - PS2 CD

						l_resultXMLElement.append_attribute(L"GameDiscType").set_value(l_gameDiscType.c_str());

						l_resultXMLElement.append_attribute(L"DiscSerial").set_value(l_DiscSerial.c_str());

						l_resultXMLElement.append_attribute(L"DiscRegionType").set_value(l_discRegionType.c_str());

						l_resultXMLElement.append_attribute(L"SoftwareVersion").set_value(l_software_version.c_str());

						l_resultXMLElement.append_attribute(L"ElfCRC").set_value(l_ElfCRC);
					}
				}

				l_ChildNode = l_ChildNode.next_sibling();
			}
			
			if (a_result != nullptr)
			{
				std::wstringstream l_wstringstream;

				l_xmlResultDoc.print(l_wstringstream);

				auto l_XMLDocumentString = l_wstringstream.str();

				*a_result = new wchar_t[l_XMLDocumentString.size() + 1];

				wcscpy_s(*a_result, l_XMLDocumentString.size() + 1, l_XMLDocumentString.c_str());
			}
		}
	}
}