
#include "stdafx.h"
#include "MemoryCard.h"

#include "PluginCallbacks.h"

BOOL PS2E_CALLBACK McdIsPresent(PS2E_THISPTR thisptr, uint32 port, uint32 slot)
{
	return g_MemoryCard.IsPresent(port, slot);
}

void PS2E_CALLBACK McdGetSizeInfo(PS2E_THISPTR thisptr, uint32 port, uint32 slot, PS2E_McdSizeInfo *outways)
{
	g_MemoryCard.GetSizeInfo(port, slot, (PCSX2Lib::API::McdSizeInfo*)outways);
}

bool PS2E_CALLBACK McdIsPSX(PS2E_THISPTR thisptr, uint32 port, uint32 slot)
{
	return g_MemoryCard.IsPSX(port, slot);
}

BOOL PS2E_CALLBACK McdRead(PS2E_THISPTR thisptr, uint32 port, uint32 slot, uint8 *dest, uint32 adr, int32 size)
{
	return g_MemoryCard.Read(port, slot, dest, adr, size);
}

BOOL PS2E_CALLBACK McdSave(PS2E_THISPTR thisptr, uint32 port, uint32 slot, const uint8 *src, uint32 adr, int32 size)
{
	return g_MemoryCard.Save(port, slot, src, adr, size);
}

BOOL PS2E_CALLBACK McdEraseBlock(PS2E_THISPTR thisptr, uint32 port, uint32 slot, uint32 adr)
{
	return g_MemoryCard.EraseBlock(port, slot, adr);
}

uint64 PS2E_CALLBACK McdGetCRC(PS2E_THISPTR thisptr, uint32 port, uint32 slot)
{
	return g_MemoryCard.GetCRC(port, slot);
}

void PS2E_CALLBACK McdNextFrame(PS2E_THISPTR thisptr, uint32 port, uint32 slot)
{
}

bool PS2E_CALLBACK McdReIndex(PS2E_THISPTR thisptr, uint32 port, uint32 slot, const wxString &filter)
{
	return false;
}

_PS2E_ComponentAPI Base;

PS2E_ComponentAPI_Mcd g_API = {
	Base,	
	McdIsPresent,
	McdGetSizeInfo,
	McdIsPSX,
	McdRead,
	McdSave,
	McdEraseBlock,
	McdGetCRC,
	McdNextFrame,
	McdReIndex
};

