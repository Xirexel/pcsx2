
#include "PrecompiledHeader.h"
#include "CDVD\CDVDisoReader.h"
#include "CDVD\IsoFS\SectorSource.h"
#include "CDVD\IsoFS\IsoFileDescriptor.h"
#include "CDVD\IsoFS\IsoDirectory.h"
#include "CDVD\IsoFS\IsoFile.h"
#include "CDVD\IsoFS\SectorSource.h"
#include "PS2Edefs.h"
#include "Elfheader.h"

const Pcsx2Config EmuConfig;


static s32 CALLBACK ISOreadSector(InputIsoFile& a_ISOFile, u8* tempbuffer, u32 lsn, int mode)
{
	static u8 cdbuffer[CD_FRAMESIZE_RAW] = { 0 };

	int _lsn = lsn;

	if (_lsn < 0) lsn = a_ISOFile.GetBlockCount() + _lsn;
	if (lsn > a_ISOFile.GetBlockCount()) return -1;

	if (mode == CDVD_MODE_2352)
	{
		a_ISOFile.ReadSync(tempbuffer, lsn);
		return 0;
	}

	a_ISOFile.ReadSync(cdbuffer, lsn);


	u8 *pbuffer = cdbuffer;
	int psize = 0;

	switch (mode)
	{
		//case CDVD_MODE_2352:
		// Unreachable due to shortcut above.
		//	pxAssume(false);
		//	break;

	case CDVD_MODE_2340:
		pbuffer += 12;
		psize = 2340;
		break;
	case CDVD_MODE_2328:
		pbuffer += 24;
		psize = 2328;
		break;
	case CDVD_MODE_2048:
		pbuffer += 24;
		psize = 2048;
		break;

		jNO_DEFAULT
	}

	memcpy(tempbuffer, pbuffer, psize);

	return 0;
}

static s32 CALLBACK ISOgetTD(InputIsoFile& a_ISOFile, u8 Track, cdvdTD *Buffer)
{
	if (Track == 0)
	{
		Buffer->lsn = a_ISOFile.GetBlockCount();
	}
	else
	{
		Buffer->type = CDVD_MODE1_TRACK;
		Buffer->lsn = 0;
	}

	return 0;
}

static s32 localDoCDVDreadSector(InputIsoFile& a_ISOFile, u8* buffer, u32 lsn, int mode)
{
	int ret = ISOreadSector(a_ISOFile, buffer, lsn, mode);
	
	return ret;
}

class InnerIsoFSCDVD : public SectorSource
{
	InputIsoFile& m_ISOFile;

public:
	InnerIsoFSCDVD(InputIsoFile& a_ISOFile) :m_ISOFile(a_ISOFile){}
	virtual ~InnerIsoFSCDVD() = default;

	virtual bool readSector(unsigned char* buffer, int lba)
	{
		return localDoCDVDreadSector(m_ISOFile, buffer, lba, CDVD_MODE_2048) >= 0;
	}

	virtual int  getNumSectors()
	{
		cdvdTD td;

		ISOgetTD(m_ISOFile, 0, &td);

		return td.lsn;
	}

};



// Sets ElfCRC to the CRC of the game bound to the CDVD plugin.
static __fi ElfObject* loadElf(const wxString filename, InputIsoFile& a_ISOFile)
{
	if (filename.StartsWith(L"host"))
		return new ElfObject(filename.After(':'), Path::GetFileSize(filename.After(':')));

	// Mimic PS2 behavior!
	// Much trial-and-error with changing the ISOFS and BOOT2 contents of an image have shown that
	// the PS2 BIOS performs the peculiar task of *ignoring* the version info from the parsed BOOT2
	// filename *and* the ISOFS, when loading the game's ELF image.  What this means is:
	//
	//   1. a valid PS2 ELF can have any version (ISOFS), and the version need not match the one in SYSTEM.CNF.
	//   2. the version info on the file in the BOOT2 parameter of SYSTEM.CNF can be missing, 10 chars long,
	//      or anything else.  Its all ignored.
	//   3. Games loading their own files do *not* exhibit this behavior; likely due to using newer IOP modules
	//      or lower level filesystem APIs (fortunately that doesn't affect us).
	//
	// FIXME: Properly mimicing this behavior is troublesome since we need to add support for "ignoring"
	// version information when doing file searches.  I'll add this later.  For now, assuming a ;1 should
	// be sufficient (no known games have their ELF binary as anything but version ;1)

	const wxString fixedname(wxStringTokenizer(filename, L';').GetNextToken() + L";1");

	if (fixedname != filename)
		Console.WriteLn(Color_Blue, "(LoadELF) Non-conforming version suffix detected and replaced.");

	InnerIsoFSCDVD isofs(a_ISOFile);
	IsoFile file(isofs, fixedname);
	return new ElfObject(fixedname, file);
}

// return value:
//   0 - Invalid or unknown disc.
//   1 - PS1 CD
//   2 - PS2 CD
int GetPS2ElfName(
	std::wstring& a_name,
	std::wstring& a_discRegionType,
	std::wstring& a_software_version,
	u32& a_ElfCRC,
	const std::wstring& a_file_path)
{
	int retype = 0;

	InputIsoFile l_ISOFile;

	auto l_isValid = l_ISOFile.Open(a_file_path.c_str());

	do
	{
		wxString elfpath;

		try {
			
			if (l_isValid)
			{				
				InnerIsoFSCDVD isofs(l_ISOFile);

				IsoFile file(isofs, L"SYSTEM.CNF;1");

				int size = file.getLength();
				if (size == 0)
				{
					retype = 0;

					break;
				}

				while (!file.eof())
				{
					const wxString original(fromUTF8(file.readLine().c_str()));
					const ParsedAssignmentString parts(original);

					if (parts.lvalue.IsEmpty() && parts.rvalue.IsEmpty()) continue;
					if (parts.rvalue.IsEmpty())
					{
						continue;
					}

					if (parts.lvalue == L"BOOT2")
					{
						elfpath = parts.rvalue;

						retype = 2;
					}
					else if (parts.lvalue == L"BOOT")
					{
						elfpath = parts.rvalue;

						retype = 1;
					}
					else if (parts.lvalue == L"VMODE")
					{
						a_discRegionType = parts.rvalue;
					}
					else if (parts.lvalue == L"VER")
					{
						a_software_version = parts.rvalue;
					}
				}
			}
		}
		catch (...)
		{
			retype = 0;

			break;
		}

		if (!l_isValid)
            break;

		wxString fname = elfpath.AfterLast('\\');
		if (!fname)
			fname = elfpath.AfterLast('/');
		if (!fname)
			fname = elfpath.AfterLast(':');
		if (fname.Matches(L"????_???.??*"))
			a_name = fname(0, 4) + L"-" + fname(5, 3) + fname(9, 2);

		if (retype != 2)
            break;
		
		std::unique_ptr<ElfObject> elfptr(loadElf(elfpath, l_ISOFile));

		elfptr->loadHeaders();
		a_ElfCRC = elfptr->getCRC();
		//ElfEntry = elfptr->header.e_entry;
		//ElfTextRange = elfptr->getTextRange();
		//Console.WriteLn(Color_StrongBlue, L"ELF (%s) Game CRC = 0x%08X, EntryPoint = 0x%08X", WX_STR(elfpath), ElfCRC, ElfEntry);


	} while (false);
	
	l_ISOFile.Close();

	return retype;
}