#pragma once

#include <string>
#include <vector>
#include <map>
#include <fstream>


class MemoryCard
{
	struct McdFile
	{
		std::wstring m_file_path;

		std::fstream m_file_stream;

		int m_length = 0;

		bool m_ispsx = false;

		u64 m_chksum;
	};
			
	std::map<uint, McdFile> m_McdFiles;
	
	u32				m_chkaddr;

	std::vector<u8>	m_currentdata;

	u8				m_effeffs[528 * 16];

	uint findSlot(uint port, uint slot);

	bool Seek(std::fstream& f, u32 a_file_length, u32 adr);

public:

	MemoryCard();

	void execute(const wchar_t* a_command, wchar_t** a_result);

	void open();

	void close();

	s32 IsPresent(uint port, uint slot);

	void GetSizeInfo(uint port, uint slot, PCSX2Lib::API::McdSizeInfo* size);

	bool IsPSX(uint port, uint slot);

	bool Read(uint port, uint slot, u8 *dest, u32 adr, int size);

	bool Save(uint port, uint slot, const u8 *src, u32 adr, int size);

	bool EraseBlock(uint port, uint slot, u32 adr);

	u64 GetCRC(uint port, uint slot);
};

extern MemoryCard g_MemoryCard;