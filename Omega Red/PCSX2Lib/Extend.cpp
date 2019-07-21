/*  PCSX2Lib - Kernel of PCSX2 PS2 Emulator for PCs
*
*  PCSX2Lib is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  PCSX2Lib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with PCSX2Lib.
*  If not, see <http://www.gnu.org/licenses/>.
*/

#include "PrecompiledHeader.h"
#include "Plugins.h"
#include "System.h"

#include "GS.h"
#include "Patch.h"
#include "MTVU.h"
#include "Pcsx2App.h"

#include "Sio.h"
#include "CDVD\IsoFileFormats.h"
#include "../pcsx2/Utilities/AsciiFile.h"

#include "Interface.h"
#include "Extend.h"


_GSopen            GSopen;
_GSopen2           GSopen2;
_PADupdate         PADupdate;

SysPluginBindings SysPlugins;

uptr pDsp[2];

bool renderswitch;

std::unique_ptr<SysCpuProviderPack> m_CpuProviders;

void InitPlugins()
{
	if (PluginsInitCallback != nullptr)
		PluginsInitCallback();
}

void ClosePlugins()
{
	if (PluginsCloseCallback != nullptr)
		PluginsCloseCallback();
}

void ShutdownPlugins()
{
	if (PluginsShutdownCallback != nullptr)
		PluginsShutdownCallback();
}

void OpenPlugins()
{
	if (PluginsOpenCallback != nullptr)
		PluginsOpenCallback();
}

bool AreLoadedPlugins()
{
	if (PluginsAreLoadedCallback != nullptr)
		return PluginsAreLoadedCallback();
	else
		return false;
}

SysCorePlugins& GetCorePlugins()
{
	throw;
}

bool SysCorePlugins::AreLoaded() const
{
	return true;
}


static std::unique_ptr<SysMainMemory> m_VmReserve;

SysMainMemory& GetVmReserve()
{
	if (!m_VmReserve) m_VmReserve = std::unique_ptr<SysMainMemory>(new SysMainMemory());
	return *m_VmReserve;
}

SysMainMemory& GetVmMemory()
{
	return GetVmReserve();
}

void UI_EnableSysActions()
{
	UI_EnableSysActionsCallback();
}

__aligned16 SysMtgsThread mtgsThread;

SysMtgsThread& GetMTGS()
{
	return mtgsThread;
}

wxString __cdecl ShiftJIS_ConvertString(char const *, int)
{
	return L"";
}

SysCpuProviderPack& GetCpuProviders()
{
	return *m_CpuProviders;
}

wxString __cdecl ShiftJIS_ConvertString(char const *)
{
	return L"";
}

void LoadAllPatchesAndStuff(const Pcsx2Config& cfg)
{
	LoadAllPatchesAndStuffCallback(cfg.bitset);
}

void __cdecl AsciiFile::Printf(char const *, ...)
{
}

wxString __thiscall Exception::SaveStateLoadError::FormatDisplayMessage(void)const
{
	return L"";
}

wxString __thiscall Exception::SaveStateLoadError::FormatDiagnosticMessage(void)const
{
	return L"";
}

wxString __thiscall Exception::PluginError::FormatDisplayMessage(void)const
{
	return L"";
}

wxString __thiscall Exception::PluginError::FormatDiagnosticMessage(void)const
{
	return L"";
}

void __thiscall DisassemblyDialog::update(void)
{
}

Exception::PluginOpenError::PluginOpenError(PluginsEnum_t)
{
}


PS2E_ComponentAPI_Mcd* g_Mcd;

void setInnerMcd(void* aMcd)
{
	g_Mcd = (PS2E_ComponentAPI_Mcd*)aMcd;
}


bool SysPluginBindings::McdIsPresent(uint port, uint slot)
{
	return !!g_Mcd->McdIsPresent((PS2E_THISPTR)g_Mcd, port, slot);
}

void SysPluginBindings::McdGetSizeInfo(uint port, uint slot, PS2E_McdSizeInfo& outways)
{
	if (g_Mcd->McdGetSizeInfo)
		g_Mcd->McdGetSizeInfo((PS2E_THISPTR)g_Mcd, port, slot, &outways);
}

bool SysPluginBindings::McdIsPSX(uint port, uint slot)
{
	return g_Mcd->McdIsPSX((PS2E_THISPTR)g_Mcd, port, slot);
}

void SysPluginBindings::McdRead(uint port, uint slot, u8 *dest, u32 adr, int size)
{
	g_Mcd->McdRead((PS2E_THISPTR)g_Mcd, port, slot, dest, adr, size);
}

void SysPluginBindings::McdSave(uint port, uint slot, const u8 *src, u32 adr, int size)
{
	g_Mcd->McdSave((PS2E_THISPTR)g_Mcd, port, slot, src, adr, size);
}

void SysPluginBindings::McdEraseBlock(uint port, uint slot, u32 adr)
{
	g_Mcd->McdEraseBlock((PS2E_THISPTR)g_Mcd, port, slot, adr);
}

u64 SysPluginBindings::McdGetCRC(uint port, uint slot)
{
	return g_Mcd->McdGetCRC((PS2E_THISPTR)g_Mcd, port, slot);
}

void SysPluginBindings::McdNextFrame(uint port, uint slot) {
	g_Mcd->McdNextFrame((PS2E_THISPTR)g_Mcd, port, slot);
}

bool SysPluginBindings::McdReIndex(uint port, uint slot, const wxString& filter) {
	return g_Mcd->McdReIndex((PS2E_THISPTR)g_Mcd, port, slot, filter);
}

 void SaveStateBase::InputRecordingFreeze(){}
