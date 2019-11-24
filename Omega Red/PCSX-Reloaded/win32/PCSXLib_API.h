#pragma once

#include "plugins.h"

#ifdef PCSXLIB_EXPORTS
#define PCSX_EXPORT __declspec(dllexport)
#else
#define PCSX_EXPORT __declspec(dllimport)
#endif

#define STDAPICALLTYPE __stdcall



#ifdef EXPORTS
#define PCSX_EXPORT_C_(type) extern "C" type __stdcall
#define PCSX_EXPORT_C PCSX_EXPORT_C_(void)
#else
#define PCSX_EXPORT_C_(type) extern "C" type __stdcall
#define PCSX_EXPORT_C PCSX_EXPORT_C_(void)
#endif

typedef struct
{
    GPUupdateLace GPU_updateLace;
    GPUinit GPU_init;
    GPUshutdown GPU_shutdown;
    GPUconfigure GPU_configure;
    GPUtest GPU_test;
    GPUabout GPU_about;
    GPUopen GPU_open;
    GPUclose GPU_close;
    GPUreadStatus GPU_readStatus;
    GPUreadData GPU_readData;
    GPUreadDataMem GPU_readDataMem;
    GPUwriteStatus GPU_writeStatus;
    GPUwriteData GPU_writeData;
    GPUwriteDataMem GPU_writeDataMem;
    GPUdmaChain GPU_dmaChain;
    GPUkeypressed GPU_keypressed;
    GPUdisplayText GPU_displayText;
    GPUmakeSnapshot GPU_makeSnapshot;
    GPUfreeze GPU_freeze;
    GPUgetScreenPic GPU_getScreenPic;
    GPUshowScreenPic GPU_showScreenPic;
    GPUclearDynarec GPU_clearDynarec;
    GPUhSync GPU_hSync;
    GPUvBlank GPU_vBlank;
    GPUvisualVibration GPU_visualVibration;
    GPUcursor GPU_cursor;
    GPUaddVertex GPU_addVertex;
} GS_API;

typedef struct
{
    SPUconfigure SPU_configure;
    SPUabout SPU_about;
    SPUinit SPU_init;
    SPUshutdown SPU_shutdown;
    SPUtest SPU_test;
    SPUopen SPU_open;
    SPUclose SPU_close;
    SPUplaySample SPU_playSample;
    SPUwriteRegister SPU_writeRegister;
    SPUreadRegister SPU_readRegister;
    SPUwriteDMA SPU_writeDMA;
    SPUreadDMA SPU_readDMA;
    SPUwriteDMAMem SPU_writeDMAMem;
    SPUreadDMAMem SPU_readDMAMem;
    SPUplayADPCMchannel SPU_playADPCMchannel;
    SPUfreeze SPU_freeze;
    SPUregisterCallback SPU_registerCallback;
    SPUasync SPU_async;
    SPUplayCDDAchannel SPU_playCDDAchannel;
} SPU_API;

//typedef struct{
//    PADconfigure PAD_configure;
//    PADabout PAD_about;
//    PADinit PAD_init;
//    PADshutdown PAD_shutdown;
//    PADtest PAD_test;
//    PADopen PAD_open;
//    PADclose PAD_close;
//    PADquery PAD_query;
//    PADreadPort1 PAD_readPort1;
//    PADreadPort2 PAD_readPort2;
//    PADkeypressed PAD_keypressed;
//    PADstartPoll PAD_startPoll;
//    PADpoll PAD_poll;
//    PADsetSensitive PAD_setSensitive;
//    PADregisterVibration PAD_registerVibration;
//    PADregisterCursor PAD_registerCursor;
//} PAD_API;

typedef struct
{
    SIO1init SIO1_init;
    SIO1shutdown SIO1_shutdown;
    SIO1open SIO1_open;
    SIO1close SIO1_close;
    SIO1test SIO1_test;
    SIO1configure SIO1_configure;
    SIO1about SIO1_about;
    SIO1pause SIO1_pause;
    SIO1resume SIO1_resume;
    SIO1keypressed SIO1_keypressed;
    SIO1writeData8 SIO1_writeData8;
    SIO1writeData16 SIO1_writeData16;
    SIO1writeData32 SIO1_writeData32;
    SIO1writeStat16 SIO1_writeStat16;
    SIO1writeStat32 SIO1_writeStat32;
    SIO1writeMode16 SIO1_writeMode16;
    SIO1writeMode32 SIO1_writeMode32;
    SIO1writeCtrl16 SIO1_writeCtrl16;
    SIO1writeCtrl32 SIO1_writeCtrl32;
    SIO1writeBaud16 SIO1_writeBaud16;
    SIO1writeBaud32 SIO1_writeBaud32;
    SIO1readData8 SIO1_readData8;
    SIO1readData16 SIO1_readData16;
    SIO1readData32 SIO1_readData32;
    SIO1readStat16 SIO1_readStat16;
    SIO1readStat32 SIO1_readStat32;
    SIO1readMode16 SIO1_readMode16;
    SIO1readMode32 SIO1_readMode32;
    SIO1readCtrl16 SIO1_readCtrl16;
    SIO1readCtrl32 SIO1_readCtrl32;
    SIO1readBaud16 SIO1_readBaud16;
    SIO1readBaud32 SIO1_readBaud32;
    SIO1update SIO1_update;
    SIO1registerCallback SIO1_registerCallback;
} SIO1_API;


