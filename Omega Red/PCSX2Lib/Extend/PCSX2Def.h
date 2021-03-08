
#pragma once

#include "../pcsx2/CDVD/CDVDaccess.h"



// SPU2
typedef s32(CALLBACK *_SPU2open)(void *pDsp);
typedef void(CALLBACK *_SPU2reset)();
typedef void(CALLBACK *_SPU2write)(u32 mem, u16 value);
typedef u16(CALLBACK *_SPU2read)(u32 mem);
typedef void(CALLBACK *_SPU2readDMA4Mem)(u16 *pMem, int size);
typedef void(CALLBACK *_SPU2writeDMA4Mem)(u16 *pMem, int size);
typedef void(CALLBACK *_SPU2interruptDMA4)();
typedef void(CALLBACK *_SPU2readDMA7Mem)(u16 *pMem, int size);
typedef void(CALLBACK *_SPU2writeDMA7Mem)(u16 *pMem, int size);
typedef void(CALLBACK *_SPU2setDMABaseAddr)(uptr baseaddr);
typedef void(CALLBACK *_SPU2interruptDMA7)();
typedef void(CALLBACK *_SPU2irqCallback)(void(*SPU2callback)(), void(*DMA4callback)(), void(*DMA7callback)());
typedef u32(CALLBACK *_SPU2ReadMemAddr)(int core);
typedef void(CALLBACK *_SPU2WriteMemAddr)(int core, u32 value);

typedef int(CALLBACK *_SPU2setupRecording)(int, void *);

typedef void(CALLBACK *_SPU2setClockPtr)(u32 *ptr);
typedef void(CALLBACK *_SPU2setTimeStretcher)(short int enable);

typedef void(CALLBACK *_SPU2async)(u32 cycles);


// CDVD
// NOTE: The read/write functions CANNOT use XMM/MMX regs
// If you want to use them, need to save and restore current ones
typedef s32(CALLBACK *_CDVDopen)(const char *pTitleFilename);

// Initiates an asynchronous track read operation.
// Returns -1 on error (invalid track)
// Returns 0 on success.
typedef s32(CALLBACK *_CDVDreadTrack)(u32 lsn, int mode);

//// *OBSOLETE* returns a pointer to the buffer, or NULL if data hasn't finished
//// loading yet.
//typedef u8 *(CALLBACK *_CDVDgetBuffer)();

// Copies loaded data to the target buffer.
// Returns -2 if the asynchronous read is still pending.
// Returns -1 if the asyncronous read failed.
// Returns 0 on success.
typedef s32(CALLBACK *_CDVDgetBuffer2)(u8 *buffer);

typedef s32(CALLBACK *_CDVDreadSubQ)(u32 lsn, cdvdSubQ *subq);
typedef s32(CALLBACK *_CDVDgetTN)(cdvdTN *Buffer);
typedef s32(CALLBACK *_CDVDgetTD)(u8 Track, cdvdTD *Buffer);
typedef s32(CALLBACK *_CDVDgetTOC)(void *toc);
typedef s32(CALLBACK *_CDVDgetDiskType)();
typedef s32(CALLBACK *_CDVDgetTrayStatus)();
typedef s32(CALLBACK *_CDVDctrlTrayOpen)();
typedef s32(CALLBACK *_CDVDctrlTrayClose)();
typedef s32(CALLBACK *_CDVDreadSector)(u8 *buffer, u32 lsn, int mode);
typedef s32(CALLBACK *_CDVDgetDualInfo)(s32 *dualType, u32 *_layer1start);

typedef void(CALLBACK *_CDVDnewDiskCB)(void(*callback)());


