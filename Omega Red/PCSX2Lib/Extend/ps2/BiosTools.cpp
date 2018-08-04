/*  PCSX2 - PS2 Emulator for PCs
 *  Copyright (C) 2002-2010  PCSX2 Dev Team
 *
 *  PCSX2 is free software: you can redistribute it and/or modify it under the terms
 *  of the GNU Lesser General Public License as published by the Free Software Found-
 *  ation, either version 3 of the License, or (at your option) any later version.
 *
 *  PCSX2 is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 *  PURPOSE.  See the GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along with PCSX2.
 *  If not, see <http://www.gnu.org/licenses/>.
 */

#include "PrecompiledHeader.h"
#include "Common.h"
#include "../pcsx2/ps2/BiosTools.h"


// FIXME: Temporary hack until we remove dependence on Pcsx2App.
#include "AppConfig.h"

#include "../../Interface.h"

#define DIRENTRY_SIZE 16

// --------------------------------------------------------------------------------------
// romdir structure (packing required!)
// --------------------------------------------------------------------------------------
//
#if defined(_MSC_VER)
#	pragma pack(1)
#endif

struct romdir
{
	char fileName[10];
	u16 extInfoSize;
	u32 fileSize;
} __packed;			// +16

#ifdef _MSC_VER
#	pragma pack()
#endif

static_assert( sizeof(romdir) == DIRENTRY_SIZE, "romdir struct not packed to 16 bytes" );

u32 BiosVersion;
u32 BiosChecksum;
wxString BiosDescription;
const BiosDebugInformation* CurrentBiosInformation;

const BiosDebugInformation biosVersions[] = {
	// USA     v02.00(14/06/2004)  Console
	{ 0x00000200, 0xD778DB8D, 0x8001a640 },
	// Europe  v02.00(14/06/2004)
	{ 0x00000200, 0X9C7B59D3, 0x8001a640 },
};

// --------------------------------------------------------------------------------------
//  Exception::BiosLoadFailed  (implementations)
// --------------------------------------------------------------------------------------
Exception::BiosLoadFailed::BiosLoadFailed( const wxString& filename )
{
	StreamName = filename;
}


// Loads the configured bios rom file into PS2 memory.  PS2 memory must be allocated prior to
// this method being called.
//
// Remarks:
//   This function does not fail if rom1, rom2, or erom files are missing, since none are
//   explicitly required for most emulation tasks.
//
// Exceptions:
//   BadStream - Thrown if the primary bios file (usually .bin) is not found, corrupted, etc.
//
void LoadBIOS()
{
	pxAssertDev( eeMem->ROM != NULL, "PS2 system memory has not been initialized yet." );

	try
	{
		LoadBIOSCallback(eeMem->ROM, Ps2MemSize::Rom);
	}
	catch (Exception::BadStream& ex)
	{
		// Rethrow as a Bios Load Failure, so that the user interface handling the exceptions
		// can respond to it appropriately.
		throw Exception::BiosLoadFailed( ex.StreamName )
			.SetDiagMsg( ex.DiagMsg() )
			.SetUserMsg( ex.UserMsg() );
	}
}

