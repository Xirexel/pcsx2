//
// Created by Evgeny Pereguda on 7/14/2019.
//

#include "BiosControl.h"
#include "../../../../../../../../common/include/Pcsx2Types.h"
#include <fstream>
#include <memory>

static const int m_biosSize = 512 * 1024;

static const int m_nvmSize = 1024;

static const int m_ROMsize = 1024 * 1024 * 4;



struct RomDir
{
public:

 	char fileName[10];

	unsigned short extInfoSize;

	unsigned int fileSize;

	std::string getFileName()
	{
		std::string l_result = std::string(fileName);

		return l_result;
	}
};

static RomDir ByteToType(std::ifstream& a_stream)
{
	RomDir l_RomDir;

	auto l = sizeof(RomDir);

	a_stream.read((char*)&l_RomDir, sizeof(RomDir));

	return l_RomDir;
}

template< size_t _size >
void ChecksumIt( u32& result, std::ifstream& stream )
{

	for( size_t i=0; i<_size/4; ++i )
	{
		u32 l_value = 0;

		stream.read((char*)&l_value, 4);

		result ^= l_value;
	}
}

#include <sys/stat.h>
#include <strstream>

s64 fsize(const char *filename) {
	struct stat st;

	if (stat(filename, &st) == 0)
		return st.st_size;

	return -1;
}


BiosControl::BiosControl(){}

BiosControl::~BiosControl(){};

static bool IsBIOS(
	std::ifstream& a_stream,
	std::string& a_zone,
	std::string& a_version,
	int& a_versionInt,
	std::string& a_data,
	std::string& a_build)
{
	bool l_result = false;

	do
	{

		int l_index;

		RomDir l_RomDir;

		for (l_index = 0; l_index < m_biosSize; l_index++)
		{
			l_RomDir = ByteToType(a_stream);

			if (l_RomDir.getFileName().find("RESET") != std::string::npos)
				break; /* found romdir */
		}

		if (l_index == m_biosSize)
		{
			break;
		}

		uint fileOffset = 0;

		while (l_RomDir.fileName[0] > 0)
		{
			if (l_RomDir.getFileName().find("ROMVER") != std::string::npos)
			{
				char romver[14+1];		// ascii version loaded from disk.

				auto filetablepos = a_stream.tellg();
				a_stream.seekg( fileOffset );
				a_stream.read( romver, 14 );
				a_stream.seekg( filetablepos );	//go back

				romver[14] = 0;

				const char zonefail[2] = { romver[4], '\0' };	// the default "zone" (unknown code)
				const char* zone = zonefail;

				switch(romver[4])
				{
					case 'T': zone = "T10K";	break;
					case 'X': zone = "Test";	break;
					case 'J': zone = "Japan";	break;
					case 'A': zone = "USA";		break;
					case 'E': zone = "Europe";	break;
					case 'H': zone = "HK";		break;
					case 'P': zone = "Free";	break;
					case 'C': zone = "China";	break;
				}

				char vermaj[3] = { romver[0], romver[1], 0 };
				char vermin[3] = { romver[2], romver[3], 0 };

				char l_desc[512];

//				sprintf(l_desc, "%-7s v%s.%s(%c%c/%c%c/%c%c%c%c)  %s",
//							  zone,
//							  vermaj, vermin,
//							  romver[12], romver[13],	// day
//							  romver[10], romver[11],	// month
//							  romver[6], romver[7], romver[8], romver[9],	// year!
//							  (romver[5]=='C') ? "Console" : (romver[5]=='D') ? "Devel" : ""
//				);

				sprintf(l_desc, "%c%c/%c%c/%c%c%c%c",
						romver[12], romver[13],	// day
						romver[10], romver[11],	// month
						romver[6], romver[7], romver[8], romver[9]	// year!
				);

				a_data = l_desc;

				unsigned int l_version = 0;

				l_version = strtol(vermaj, (char**)NULL, 0) << 8;

				l_version|= strtol(vermin, (char**)NULL, 0);



				sprintf(l_desc, "v%s.%s",
						vermaj, vermin
				);

				a_version = l_desc;

				a_versionInt = l_version;

				a_zone = zone;


				sprintf(l_desc, "v%s.%s",
						vermaj, vermin
				);

				sprintf(l_desc, "%s",
						(romver[5]=='C') ? "Console" : (romver[5]=='D') ? "Devel" : ""
				);

				a_build = l_desc;

				l_result = true;

				break;
			}

			if ((l_RomDir.fileSize % 0x10) == 0)
				fileOffset += l_RomDir.fileSize;
			else
				fileOffset += (l_RomDir.fileSize + 0x10) & 0xfffffff0;

			l_RomDir = ByteToType(a_stream);
		}

	} while (false);

	return l_result;
}

bool BiosControl::IsBIOS(
	const std::string& filename,
	std::string& zone,
	std::string& version,
	int& versionInt,
	std::string& data,
	std::string& build){

	bool l_result = false;

	do
	{
		try
		{
			std::ifstream l_fstream;

			l_fstream.open(filename, std::fstream::binary);

			if (!l_fstream.is_open())
				break;

			l_fstream.seekg(0, std::ios_base::seekdir::beg);

			auto l_length = fsize(filename.data());

			if (l_length < m_biosSize)
				break;

			l_result = ::IsBIOS(
				l_fstream,
				zone,
				version,
				versionInt,
				data,
				build);

			l_fstream.close();
		}
		catch (std::exception exc)
		{
		}

	} while (false);

	return l_result;

};


unsigned int BiosControl::getBIOSChecksum(const std::string& filename)
{
	uint l_result = 0;

	do
	{
		try
		{
			std::ifstream l_fstream;

			l_fstream.open(filename, std::fstream::binary);

			if (!l_fstream.is_open())
				break;

			l_fstream.seekg(0, std::ios_base::seekdir::end);

			auto l_length = l_fstream.tellg();

			if (l_length < m_biosSize)
				break;

			l_fstream.seekg(0, std::ios_base::seekdir::beg);

			ChecksumIt<m_ROMsize>(l_result, l_fstream);

		}
		catch (std::exception exc)
		{
		}

	} while (false);

	return l_result;
}

void BiosControl::loadBIOS(
	const std::string& filename,
	void* aPtrTarget,
	int aReadLength)
{

	do
	{
		try
		{
			std::ifstream l_fstream;

			l_fstream.open(filename, std::fstream::binary);

            if (!l_fstream.is_open())
            {
				l_fstream.close();

//                var l_splitsFilePath = l_filePath.Split(new char[] { '|' });
//
//                if (l_splitsFilePath == null || l_splitsFilePath.Length != 2)
//                    return;
//
//                if (!File.Exists(l_splitsFilePath[0]))
//                    return;
//
//                using (FileStream zipToOpen = new FileStream(l_splitsFilePath[0], FileMode.Open))
//                {
//                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
//                    {
//                        var l_entry = archive.GetEntry(l_splitsFilePath[1]);
//
//                        if (l_entry != null)
//                        {
//                            using (BinaryReader reader = new BinaryReader(l_entry.Open()))
//                            {
//                                try
//                                {
//                                    byte[] l_memory = reader.ReadBytes(a_SecondArg);
//
//                                    Marshal.Copy(l_memory, 0, a_FirstArg, Math.Min(a_SecondArg, l_memory.Length));
//                                }
//                                catch (Exception)
//                                {
//                                }
//                            }
//                        }
//                    }
//
//                }
            }
            else
            {
				auto filesize = fsize(filename.data());

                if (filesize <= 0)
                {
                    throw "Wrong file size!!!";
                }

				l_fstream.read((s8*)aPtrTarget, aReadLength > filesize? filesize: aReadLength);

				l_fstream.close();
            }





			//pxInputStream memfp(Bios, new wxMemoryInputStream(aPtrROM, aLength));
			//LoadBiosVersion( memfp, BiosVersion, BiosDescription, biosZone );

			//Console.SetTitle( pxsFmt( L"Running BIOS (%s v%u.%u)",
			//    WX_STR(biosZone), BiosVersion >> 8, BiosVersion & 0xff
			//));

			////injectIRX("host.irx");	//not fully tested; still buggy

			////LoadExtraRom( L"rom1", eeMem->ROM1 );
			////LoadExtraRom( L"rom2", eeMem->ROM2 );
			////LoadExtraRom( L"erom", eeMem->EROM );

			//if (g_Conf->CurrentIRX.Length() > 3)
			//    LoadIrx(g_Conf->CurrentIRX, &aPtrROM[0x3C0000]);

			//CurrentBiosInformation = NULL;
			//for (size_t i = 0; i < sizeof(biosVersions)/sizeof(biosVersions[0]); i++)
			//{
			//    if (biosVersions[i].biosChecksum == BiosChecksum && biosVersions[i].biosVersion == BiosVersion)
			//    {
			//        CurrentBiosInformation = &biosVersions[i];
			//        break;
			//    }
			//}

		}
		catch (std::exception exc)
		{
		}

	} while (false);
}