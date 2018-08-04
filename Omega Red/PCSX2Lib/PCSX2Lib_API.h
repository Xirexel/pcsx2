/*  PCSX2Lib - PS2 Emulator for PCs
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

#pragma once


#ifdef EXPORTS 
#define PCSX2_EXPORT_C_(type)extern "C" type __stdcall
#define PCSX2_EXPORT_C PCSX2_EXPORT_C_(void)
#else
#define PCSX2_EXPORT_C_(type)extern "C" type __stdcall
#define PCSX2_EXPORT_C PCSX2_EXPORT_C_(void)
#endif





typedef unsigned char uint8;
typedef signed char int8;
typedef unsigned short uint16;
typedef signed short int16;
typedef unsigned int uint32;
typedef signed int int32;
typedef unsigned long long uint64;
typedef signed long long int64;
#ifdef __x86_64__
typedef uint64 uptr;
#else
typedef uint32 uptr;
#endif




namespace PCSX2Lib
{
	namespace API
	{

		// GS
		// NOTE: GSreadFIFOX/GSwriteCSR functions CANNOT use XMM/MMX regs
		// If you want to use them, need to save and restore current ones

		typedef int32(CALLBACK *_GScallbackopen)();
		typedef void(CALLBACK *_GSvsync)(int32 field);
		typedef void(CALLBACK *_GSgifTransfer)(const uint8 *pMem, uint32 size);
		typedef void(CALLBACK *_GSirqCallback)(void(*callback)());
		typedef void(CALLBACK *_GSsetBaseMem)(void *);
		typedef void(CALLBACK *_GSsetGameCRC)(int32, int32);
		typedef void(CALLBACK *_GSsetFrameSkip)(int32 frameskip);
		typedef void(CALLBACK *_GSsetVsync)(int32 enabled);
		typedef void(CALLBACK *_GSreset)();
		typedef void(CALLBACK *_GSinitReadFIFO)(uint8 *pMem);
		typedef void(CALLBACK *_GSreadFIFO)(uint8 *pMem);
		typedef void(CALLBACK *_GSinitReadFIFO2)(uint8 *pMem, int32 qwc);
		typedef void(CALLBACK *_GSreadFIFO2)(uint8 *pMem, int32 qwc);
		typedef void(CALLBACK *_GSgifSoftReset)(uint32 mask);


		// SPU2

		typedef void(CALLBACK *_SPU2write)(uint32 mem, uint16 value);
		typedef uint16(CALLBACK *_SPU2read)(uint32 mem);
		typedef void(CALLBACK *_SPU2readDMA4Mem)(uint16 *pMem, int32 size);
		typedef void(CALLBACK *_SPU2writeDMA4Mem)(uint16 *pMem, int32 size);
		typedef void(CALLBACK *_SPU2interruptDMA4)();
		typedef void(CALLBACK *_SPU2readDMA7Mem)(uint16 *pMem, int32 size);
		typedef void(CALLBACK *_SPU2writeDMA7Mem)(uint16 *pMem, int32 size);
		typedef void(CALLBACK *_SPU2setDMABaseAddr)(uptr baseaddr);
		typedef void(CALLBACK *_SPU2interruptDMA7)();
		typedef uint32(CALLBACK *_SPU2ReadMemAddr)(int32 core);
		typedef void(CALLBACK *_SPU2WriteMemAddr)(int32 core, uint32 value);
		typedef void(CALLBACK *_SPU2irqCallback)(void(*SPU2callback)(), void(*DMA4callback)(), void(*DMA7callback)());
		typedef void(CALLBACK *_SPU2setClockPtr)(uint32 *ptr);
		typedef void(CALLBACK *_SPU2async)(uint32 cycles);
	


		// PAD
		
		typedef uint8(CALLBACK *_PADstartPoll)(int32 pad);
		typedef uint8(CALLBACK *_PADpoll)(uint8 value);
		typedef int32(CALLBACK *_PADsetSlot)(uint8 port, uint8 slot);




		// CDVD


		typedef struct _cdvdSubQ
		{
			uint8 ctrl : 4;   // control and mode bits
			uint8 mode : 4;   // control and mode bits
			uint8 trackNum;   // current track number (1 to 99)
			uint8 trackIndex; // current index within track (0 to 99)
			uint8 trackM;     // current minute location on the disc (BCD encoded)
			uint8 trackS;     // current sector location on the disc (BCD encoded)
			uint8 trackF;     // current frame location on the disc (BCD encoded)
			uint8 pad;        // unused
			uint8 discM;      // current minute offset from first track (BCD encoded)
			uint8 discS;      // current sector offset from first track (BCD encoded)
			uint8 discF;      // current frame offset from first track (BCD encoded)
		} cdvdSubQ;

		typedef struct _cdvdTD
		{ // NOT bcd coded
			uint32 lsn;
			uint8 type;
		} cdvdTD;

		typedef struct _cdvdTN
		{
			uint8 strack; //number of the first track (usually 1)
			uint8 etrack; //number of the last track
		} cdvdTN;

		// NOTE: The read/write functions CANNOT use XMM/MMX regs
		// If you want to use them, need to save and restore current ones
		typedef int32(CALLBACK *_CDVDopen)(const char *pTitleFilename);

		// Initiates an asynchronous track read operation.
		// Returns -1 on error (invalid track)
		// Returns 0 on success.
		typedef int32(CALLBACK *_CDVDreadTrack)(uint32 lsn, int mode);

		// *OBSOLETE* returns a pointer to the buffer, or NULL if data hasn't finished
		// loading yet.
		typedef uint8 *(CALLBACK *_CDVDgetBuffer)();

		// Copies loaded data to the target buffer.
		// Returns -2 if the asynchronous read is still pending.
		// Returns -1 if the asyncronous read failed.
		// Returns 0 on success.
		typedef int32(CALLBACK *_CDVDgetBuffer2)(uint8 *buffer);
		typedef int32(CALLBACK *_CDVDreadSubQ)(uint32 lsn, cdvdSubQ *subq);
		typedef int32(CALLBACK *_CDVDgetTN)(cdvdTN *Buffer);
		typedef int32(CALLBACK *_CDVDgetTD)(uint8 Track, cdvdTD *Buffer);
		typedef int32(CALLBACK *_CDVDgetTOC)(void *toc);
		typedef int32(CALLBACK *_CDVDgetDiskType)();
		typedef int32(CALLBACK *_CDVDgetTrayStatus)();
		typedef int32(CALLBACK *_CDVDctrlTrayOpen)();
		typedef int32(CALLBACK *_CDVDctrlTrayClose)();
		typedef int32(CALLBACK *_CDVDreadSector)(uint8 *buffer, uint32 lsn, int mode);
		typedef int32(CALLBACK *_CDVDgetDualInfo)(int32 *dualType, uint32 *_layer1start);
		typedef void(CALLBACK *_CDVDnewDiskCB)(void(*callback)());


		// --------------------------------------------------------------------------------------
		//  PS2E_McdSizeInfo
		// --------------------------------------------------------------------------------------
		struct McdSizeInfo
		{
			uint16 SectorSize;              // Size of each sector, in bytes.  (only 512 and 1024 are valid)
			uint16 EraseBlockSizeInSectors; // Size of the erase block, in sectors (max is 16)
			uint32 McdSizeInSectors;        // Total size of the card, in sectors (no upper limit)
			uint8 Xor;                      // Checksum of previous data
		};

		typedef bool(CALLBACK *_McdIsPresent)(uint32 port, uint32 slot);
		typedef void(CALLBACK *_McdGetSizeInfo)(uint32 port, uint32 slot, McdSizeInfo *outways);
		typedef bool(CALLBACK *_McdIsPSX)(uint32 port, uint32 slot);
		typedef bool(CALLBACK *_McdRead)(uint32 port, uint32 slot, uint8 *dest, uint32 adr, int32 size);
		typedef bool(CALLBACK *_McdSave)(uint32 port, uint32 slot, const uint8 *src, uint32 adr, int32 size);
		typedef bool(CALLBACK *_McdEraseBlock)(uint32 port, uint32 slot, uint32 adr);
		typedef uint64(CALLBACK *_McdGetCRC)(uint32 port, uint32 slot);
		typedef void(CALLBACK *_McdNextFrame)(uint32 port, uint32 slot);
		typedef bool(CALLBACK *_McdReIndex)(uint32 port, uint32 slot, const char* filter);


		typedef void(*DEV9callback)(int cycles);
		typedef int(*DEV9handler)(void);

		// DEV9
		// NOTE: The read/write functions CANNOT use XMM/MMX regs
		// If you want to use them, need to save and restore current ones
		typedef int32(CALLBACK *_DEV9open)(void *pDsp);
		typedef uint8(CALLBACK *_DEV9read8)(uint32 mem);
		typedef uint16(CALLBACK *_DEV9read16)(uint32 mem);
		typedef uint32(CALLBACK *_DEV9read32)(uint32 mem);
		typedef void(CALLBACK *_DEV9write8)(uint32 mem, uint8 value);
		typedef void(CALLBACK *_DEV9write16)(uint32 mem, uint16 value);
		typedef void(CALLBACK *_DEV9write32)(uint32 mem, uint32 value);
#ifdef ENABLE_NEW_IOPDMA_DEV9
		typedef int32(CALLBACK *_DEV9dmaRead)(int32 channel, uint32 *data, uint32 bytesLeft, uint32 *bytesProcessed);
		typedef int32(CALLBACK *_DEV9dmaWrite)(int32 channel, uint32 *data, uint32 bytesLeft, uint32 *bytesProcessed);
		typedef void(CALLBACK *_DEV9dmaInterrupt)(int32 channel);
#else
		typedef void(CALLBACK *_DEV9readDMA8Mem)(uint32 *pMem, int size);
		typedef void(CALLBACK *_DEV9writeDMA8Mem)(uint32 *pMem, int size);
#endif
		typedef void(CALLBACK *_DEV9irqCallback)(DEV9callback callback);
		typedef DEV9handler(CALLBACK *_DEV9irqHandler)(void);
		typedef void(CALLBACK *_DEV9async)(uint32 cycles);


		//FW
		typedef int32(CALLBACK *_FWopen)(void *pDsp);
		typedef uint32(CALLBACK *_FWread32)(uint32 mem);
		typedef void(CALLBACK *_FWwrite32)(uint32 mem, uint32 value);
		typedef void(CALLBACK *_FWirqCallback)(void(*callback)());


		typedef void(*USBcallback)(int cycles);
		typedef int(*USBhandler)(void);


		// USB
		// NOTE: The read/write functions CANNOT use XMM/MMX regs
		// If you want to use them, need to save and restore current ones
		typedef int32(CALLBACK *_USBopen)(void *pDsp);
		typedef uint8(CALLBACK *_USBread8)(uint32 mem);
		typedef uint16(CALLBACK *_USBread16)(uint32 mem);
		typedef uint32(CALLBACK *_USBread32)(uint32 mem);
		typedef void(CALLBACK *_USBwrite8)(uint32 mem, uint8 value);
		typedef void(CALLBACK *_USBwrite16)(uint32 mem, uint16 value);
		typedef void(CALLBACK *_USBwrite32)(uint32 mem, uint32 value);
		typedef void(CALLBACK *_USBasync)(uint32 cycles);

		typedef void(CALLBACK *_USBirqCallback)(USBcallback callback);
		typedef USBhandler(CALLBACK *_USBirqHandler)(void);
		typedef void(CALLBACK *_USBsetRAM)(void *mem);


		struct GS_API
		{
			_GScallbackopen		GScallbackopen;
			_GSvsync			GSvsync;
			_GSgifTransfer		GSgifTransfer;
			_GSirqCallback		GSirqCallback;
			_GSsetBaseMem		GSsetBaseMem;
			_GSsetGameCRC		GSsetGameCRC;
			_GSsetFrameSkip		GSsetFrameSkip;
			_GSsetVsync			GSsetVsync;
			_GSreset			GSreset;
			_GSinitReadFIFO		GSinitReadFIFO;
			_GSreadFIFO			GSreadFIFO;
			_GSinitReadFIFO2	GSinitReadFIFO2;
			_GSreadFIFO2		GSreadFIFO2;
			_GSgifSoftReset		GSgifSoftReset;
		};
		
		struct SPU2_API
		{
			_SPU2write SPU2write;
			_SPU2read SPU2read;
			_SPU2readDMA4Mem SPU2readDMA4Mem;
			_SPU2writeDMA4Mem SPU2writeDMA4Mem;
			_SPU2interruptDMA4 SPU2interruptDMA4;
			_SPU2readDMA7Mem SPU2readDMA7Mem;
			_SPU2writeDMA7Mem SPU2writeDMA7Mem;
			_SPU2setDMABaseAddr SPU2setDMABaseAddr;
			_SPU2interruptDMA7 SPU2interruptDMA7;
			_SPU2ReadMemAddr SPU2ReadMemAddr;
			_SPU2WriteMemAddr SPU2WriteMemAddr;
			_SPU2irqCallback SPU2irqCallback;
			_SPU2setClockPtr SPU2setClockPtr;
			_SPU2async SPU2async;
		};

		struct PAD_API
		{
			_PADstartPoll PADstartPoll;
			_PADpoll PADpoll;
			_PADsetSlot PADsetSlot;
		};
		
		struct CDVD_API
		{
			void (CALLBACK *close)();
			// Don't need init or shutdown.  iso/nodisc have no init/shutdown and plugin's
			// is handled by the PluginManager.

			// Don't need plugin specific things like freeze, test, or other stuff here.
			// Those are handled by the plugin manager specifically.

			_CDVDopen          open;
			_CDVDreadTrack     readTrack;
			_CDVDgetBuffer     getBuffer;
			_CDVDreadSubQ      readSubQ;
			_CDVDgetTN         getTN;
			_CDVDgetTD         getTD;
			_CDVDgetTOC        getTOC;
			_CDVDgetDiskType   getDiskType;
			_CDVDgetTrayStatus getTrayStatus;
			_CDVDctrlTrayOpen  ctrlTrayOpen;
			_CDVDctrlTrayClose ctrlTrayClose;
			_CDVDnewDiskCB     newDiskCB;

			// special functions, not in external interface yet
			_CDVDreadSector    readSector;
			_CDVDgetBuffer2    getBuffer2;
			_CDVDgetDualInfo   getDualInfo;
		};



		// --------------------------------------------------------------------------------------
		//  PS2E_ComponentAPI_Mcd
		// --------------------------------------------------------------------------------------
		// Thread Safety:
		//  * Thread affinity is not guaranteed.  Calls may be made from either the main emu thread
		//    or an IOP child thread (if the emulator uses one).
		//
		//  * No locking required: All calls to the memory cards are interlocked by the emulator.
		//
		struct MCD_API
		{
			// McdIsPresent
			// Called by the emulator to detect the availability of a memory card.  This function
			// will be called frequently -- essentially whenever the SIO port for the memory card
			// has its status polled - so its overhead should be minimal when possible.
			//
			// Returns:
			//   False if the card is not available, or True if it is available.
			//
			_McdIsPresent McdIsPresent;

			// McdGetSectorSize  (can be NULL)
			// Requests memorycard formatting information from the Mcd provider.  See the description of
			// PS2E_McdSizeInfo for details on each field.  If the Mcd provider supports only standard 8MB
			// carts, then this function can be NULL.
			//
			// Returns:
			//   Assigned values for memorycard sector size and sector count in 'outways.'
			//
			_McdGetSizeInfo McdGetSizeInfo;

			// McdIsPSX
			// Checks if the memorycard is a PSX one from the Mcd provider.
			//
			// Returns:
			//   False: PS2, True: PSX
			//
			_McdIsPSX McdIsPSX;

			// McdRead
			// Requests that a block of data be loaded from the memorycard into the specified dest
			// buffer (which is allocated by the caller).  Bytes read should match the requested
			// size.  Reads *must* be performed synchronously (function cannot return until the
			// read op has finished).
			//
			// Returns:
			//   False on failure, and True on success.  Emulator may use GetLastError to retrieve additional
			//   information for logging or displaying to the user.
			//
			_McdRead McdRead;

			// McdSave
			// Saves the provided block of data to the memorycard at the specified seek address.
			// Writes *must* be performed synchronously (function cannot return until the write op
			// has finished).  Write cache flushing is optional.
			//
			// Returns:
			//   False on failure, and True on success.  Emulator may use GetLastError to retrieve additional
			//   information for logging or displaying to the user.
			//
			_McdSave McdSave;

			// McdEraseBlock
			// Saves "cleared" data to the memorycard at the specified seek address.  Cleared data
			// is a series of 0xff values (all bits set to 1).
			// Writes *must* be performed synchronously (function cannot return until the write op
			// has finished).  Write cache flushing is optional.
			//
			// Returns:
			//   False on failure, and True on success.  Emulator may use GetLastError to retrieve additional
			//   information for logging or displaying to the user.
			//
			_McdEraseBlock McdEraseBlock;

			_McdGetCRC McdGetCRC;

			// McdNextFrame
			// Inform the memory card that a frame of emulation time has passed.
			// Used by the FolderMemoryCard to find a good time to flush written data to the host file system.
			_McdNextFrame McdNextFrame;

			_McdReIndex McdReIndex;

		};

		struct DEV9_API
		{
			_DEV9read8 DEV9read8;
			_DEV9read16 DEV9read16;
			_DEV9read32 DEV9read32;
			_DEV9write8 DEV9write8;
			_DEV9write16 DEV9write16;
			_DEV9write32 DEV9write32;
			_DEV9readDMA8Mem DEV9readDMA8Mem;
			_DEV9writeDMA8Mem DEV9writeDMA8Mem;
			_DEV9irqCallback DEV9irqCallback;
			_DEV9irqHandler DEV9irqHandler;
			_DEV9async DEV9async;
		};
		
		struct FW_API
		{
			_FWread32          FWread32;
			_FWwrite32         FWwrite32;
			_FWirqCallback     FWirqCallback;
		};
		
		struct USB_API
		{
			_USBread8 USBread8;
			_USBread16 USBread16;
			_USBread32 USBread32;
			_USBwrite8 USBwrite8;
			_USBwrite16 USBwrite16;
			_USBwrite32 USBwrite32;
			_USBasync USBasync;

			_USBirqCallback USBirqCallback;
			_USBirqHandler USBirqHandler;
			_USBsetRAM USBsetRAM;
		};
	}
}
