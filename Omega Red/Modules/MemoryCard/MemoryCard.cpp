// MemoryCard.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "MemoryCard.h"

MemoryCard g_MemoryCard;


static const int MCD_SIZE = 1024 * 8 * 16;		// Legacy PSX card default size

static const int MC2_MBSIZE = 1024 * 528 * 2;		// Size of a single megabyte of card data
static const int MC2_SIZE = MC2_MBSIZE * 8;		// PS2 card default size (8MB)

MemoryCard::MemoryCard()
{
	memset(m_effeffs, 0xff, sizeof(m_effeffs));

	//memset8<0xff>(m_effeffs);
	m_chkaddr = 0;
}
void MemoryCard::execute(const wchar_t* a_command, wchar_t** a_result)
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
					auto l_Mcds = l_ChildNode.select_nodes(L"Mcd");

					m_McdFiles.clear();

					// Regular Bindings
					for (auto& l_item : l_Mcds)
					{
						auto l_FilePath = l_item.node().attribute(L"FilePath").value();

						auto l_Slot = l_item.node().attribute(L"Slot").as_uint();

						m_McdFiles[l_Slot].m_file_path = l_FilePath;

						m_McdFiles[l_Slot].m_file_stream = std::fstream();
					}

					open();
				}
				else if (std::wstring(l_ChildNode.name()) == L"Open")
				{
					open();
				}
				else if (std::wstring(l_ChildNode.name()) == L"Close")
				{
					close();

					for (auto &l_item : m_McdFiles) {

                        l_item.second.m_file_stream = std::fstream();
                    }
				}

				l_ChildNode = l_ChildNode.next_sibling();
			}


		}
	}
}

void MemoryCard::open()
{
	for (auto& l_item : m_McdFiles)
	{
		if (!l_item.second.m_file_stream.is_open())
			l_item.second.m_file_stream.open(l_item.second.m_file_path, std::ios::in | std::ios::out | std::ios::binary);

		if (l_item.second.m_file_stream.is_open())
		{
			l_item.second.m_file_stream.seekg(0, l_item.second.m_file_stream.end);
			int length = l_item.second.m_file_stream.tellg();
			l_item.second.m_file_stream.seekg(0, l_item.second.m_file_stream.beg);
	
			l_item.second.m_length = length;
			l_item.second.m_ispsx = length == 0x20000;
			m_chkaddr = 0x210;

			if (!l_item.second.m_ispsx && !!l_item.second.m_file_stream.seekg(m_chkaddr, l_item.second.m_file_stream.beg))
			{
				u64 l_temp_chksum = 0;

				l_item.second.m_file_stream.read((char*)&l_temp_chksum, sizeof(l_temp_chksum));

				l_item.second.m_chksum = l_temp_chksum;
			}
		}
		else
			break;
	}
}

void MemoryCard::close()
{
	for (auto& l_item : m_McdFiles)
	{
		l_item.second.m_file_stream.close();
	}
}

uint MemoryCard::findSlot(uint port, uint slot)
{
	if (slot == 0) return port;
	if (port == 0) return slot + 1;		// multitap 1
	return slot + 4;					// multitap 2
}

s32 MemoryCard::IsPresent(uint port, uint slot)
{
	const auto combinedSlot = findSlot(port, slot);

	auto l_itr = m_McdFiles.find(combinedSlot);

	if (l_itr != m_McdFiles.end())
		return m_McdFiles[combinedSlot].m_file_stream.is_open();
	else
		return false;
}

void MemoryCard::GetSizeInfo(uint port, uint slot, PCSX2Lib::API::McdSizeInfo* size)
{
	const auto combinedSlot = findSlot(port, slot);

	auto l_itr = m_McdFiles.find(combinedSlot);

	if (l_itr != m_McdFiles.end())
	{
		size->SectorSize = 512; // 0x0200
		size->EraseBlockSizeInSectors = 16;  // 0x0010
		size->Xor = 18;  // 0x12, XOR 02 00 00 10

		if (m_McdFiles[combinedSlot].m_file_stream.is_open())
			size->McdSizeInSectors = m_McdFiles[combinedSlot].m_length / (size->SectorSize + size->EraseBlockSizeInSectors);
		else
			size->McdSizeInSectors = 0x4000;

		u8 *pdata = (u8*)&size->McdSizeInSectors;
		size->Xor ^= pdata[0] ^ pdata[1] ^ pdata[2] ^ pdata[3];
	}
}

bool MemoryCard::IsPSX(uint port, uint slot)
{
	const auto combinedSlot = findSlot(port, slot);

	auto l_itr = m_McdFiles.find(combinedSlot);

	if (l_itr != m_McdFiles.end())
		return m_McdFiles[combinedSlot].m_ispsx;
	else
		return true;
}

bool MemoryCard::Read(uint port, uint slot, u8 *dest, u32 adr, int size)
{
	const auto combinedSlot = findSlot(port, slot);

	auto l_itr = m_McdFiles.find(combinedSlot);

	if (l_itr != m_McdFiles.end() && (*l_itr).second.m_file_stream.is_open())
	{
		(*l_itr).second.m_file_stream.seekg(adr, (*l_itr).second.m_file_stream.beg);

		(*l_itr).second.m_file_stream.read((char*)dest, size);

		return true;
	}
	else
		return false;

}

bool MemoryCard::Save(uint port, uint slot, const u8 *src, u32 adr, int size)
{
	const auto combinedSlot = findSlot(port, slot);

	auto l_itr = m_McdFiles.find(combinedSlot);

	if (l_itr != m_McdFiles.end() && (*l_itr).second.m_file_stream.is_open())
	{
		if ((*l_itr).second.m_ispsx)
		{
			m_currentdata.resize(size);
			for (int i = 0; i<size; i++) m_currentdata[i] = src[i];
		}
		else
		{
			if (!Seek((*l_itr).second.m_file_stream, (*l_itr).second.m_length, adr)) return 0;
			m_currentdata.resize(size);
			(*l_itr).second.m_file_stream.read((char*)m_currentdata.data(), size);


			for (int i = 0; i<size; i++)
			{
				//if ((m_currentdata[i] & src[i]) != src[i])
				//	Console.Warning("(FileMcd) Warning: writing to uncleared data. (%d) [%08X]", slot, adr);
				m_currentdata[i] &= src[i];
			}

			// Checksumness
		{
			//if (adr == m_chkaddr)
			//	Console.Warning("(FileMcd) Warning: checksum sector overwritten. (%d)", slot);

			u64 *pdata = (u64*)&m_currentdata[0];
			u32 loops = size / 8;

			for (u32 i = 0; i < loops; i++)
				(*l_itr).second.m_chksum ^= pdata[i];
		}
		}

		if (!Seek((*l_itr).second.m_file_stream, (*l_itr).second.m_length, adr)) return 0;

		auto status =(*l_itr).second.m_file_stream.write((char*)m_currentdata.data(), size).good();

		if (status) {
			//static auto last = std::chrono::time_point<std::chrono::system_clock>();

			//std::chrono::duration<float> elapsed = std::chrono::system_clock::now() - last;
			//if (elapsed > std::chrono::seconds(5)) {
			//	wxString name, ext;
			//	wxFileName::SplitPath(m_file[slot].GetName(), NULL, NULL, &name, &ext);
			//	OSDlog(Color_StrongYellow, false, "Memory Card %s written.", (const char *)(name + "." + ext).c_str());
			//	last = std::chrono::system_clock::now();
			//}
			return true;
		}
	}
	else
		return true;
	
	return false;
}

bool MemoryCard::EraseBlock(uint port, uint slot, u32 adr)
{
	const auto combinedSlot = findSlot(port, slot);

	auto l_itr = m_McdFiles.find(combinedSlot);

	if (l_itr != m_McdFiles.end() && (*l_itr).second.m_file_stream.is_open())
	{
		(*l_itr).second.m_file_stream.seekg(adr, (*l_itr).second.m_file_stream.beg);

		auto l_result = (*l_itr).second.m_file_stream.write((char*)m_effeffs, sizeof(m_effeffs)).good();

		return l_result;
	}
	else
		return false;
}

u64 MemoryCard::GetCRC(uint port, uint slot)
{
	const auto combinedSlot = findSlot(port, slot);

	auto l_itr = m_McdFiles.find(combinedSlot);

	if (l_itr != m_McdFiles.end() && (*l_itr).second.m_file_stream.is_open())
	{
		u64 retval = 0;

		if ((*l_itr).second.m_ispsx)
		{
			if (!Seek((*l_itr).second.m_file_stream, (*l_itr).second.m_length, 0)) return 0;

			// Process the file in 4k chunks.  Speeds things up significantly.

			u64 buffer[528 * 8];		// use 528 (sector size), ensures even divisibility

			const uint filesize = (*l_itr).second.m_length / sizeof(buffer);
			for (uint i = filesize; i; --i)
			{
				(*l_itr).second.m_file_stream.read((char*)&buffer, sizeof(buffer));
				for (uint t = 0; t<ArraySize(buffer); ++t)
					retval ^= buffer[t];
			}
		}
		else
		{
			retval = (*l_itr).second.m_chksum;
		}
		return retval;
	}
	else
		return 0;
}


// Returns FALSE if the seek failed (is outside the bounds of the file).
bool MemoryCard::Seek(std::fstream& f, u32 a_file_length, u32 adr)
{
	const u32 size = a_file_length;

	// If anyone knows why this filesize logic is here (it appears to be related to legacy PSX
	// cards, perhaps hacked support for some special emulator-specific memcard formats that
	// had header info?), then please replace this comment with something useful.  Thanks!  -- air

	u32 offset = 0;

	if (size == MCD_SIZE + 64)
		offset = 64;
	else if (size == MCD_SIZE + 3904)
		offset = 3904;
	else
	{
		// perform sanity checks here?
	}

	f.seekg(adr + offset, f.beg);

	u32 l_pos = f.tellg();

	return l_pos == (adr + offset);
}