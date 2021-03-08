
//#include "plugins.h"
//#include "cdriso.h"

#include "PCSXLib_API.h"



void CALLBACK GPU_Stub_displayText(char *pText)
{
    SysPrintf("%s\n", pText);
}

long CALLBACK GPU_Stub_configure(void) { return 0; }
long CALLBACK GPU_Stub_test(void) { return 0; }
void CALLBACK GPU_Stub_about(void) {}
void CALLBACK GPU_Stub_makeSnapshot(void) {}
void CALLBACK GPU_Stub_keypressed(int key) {}
long CALLBACK GPU_Stub_getScreenPic(unsigned char *pMem) { return -1; }
long CALLBACK GPU_Stub_showScreenPic(unsigned char *pMem) { return -1; }
void CALLBACK GPU_Stub_clearDynarec(void(CALLBACK *callback)(void)) {}
void CALLBACK GPU_Stub_hSync(int val) {}
void CALLBACK GPU_Stub_vBlank(int val) {}
void CALLBACK GPU_Stub_visualVibration(unsigned long iSmall, unsigned long iBig) {}
void CALLBACK GPU_Stub_cursor(int player, int x, int y) {}
void CALLBACK GPU_Stub_addVertex(short sx, short sy, s64 fx, s64 fy, s64 fz) {}


#define BindGpu(dest, name) \
    GPU_##dest = a_API->GPU_##dest;

#define BindGpuWithStub(dest, name) \
    GPU_##dest = a_API->GPU_##dest;  \
    if (GPU_##dest == NULL)         \
        GPU_##dest = (GPU##dest)GPU_Stub_##dest;

void STDAPICALLTYPE setGS(GS_API* a_API)
{
    if (a_API == NULL)
        return;

    BindGpu(init, "GPUinit");
    BindGpu(shutdown, "GPUshutdown");
    BindGpu(open, "GPUopen");
    BindGpu(close, "GPUclose");
    BindGpu(readData, "GPUreadData");
    BindGpu(readDataMem, "GPUreadDataMem");
    BindGpu(readStatus, "GPUreadStatus");
    BindGpu(writeData, "GPUwriteData");
    BindGpu(writeDataMem, "GPUwriteDataMem");
    BindGpu(writeStatus, "GPUwriteStatus");
    BindGpu(dmaChain, "GPUdmaChain");
    BindGpu(updateLace, "GPUupdateLace");
    BindGpu(freeze, "GPUfreeze");
    BindGpuWithStub(keypressed, "GPUkeypressed");
    BindGpuWithStub(displayText, "GPUdisplayText");
    BindGpuWithStub(makeSnapshot, "GPUmakeSnapshot");
    BindGpuWithStub(getScreenPic, "GPUgetScreenPic");
    BindGpuWithStub(showScreenPic, "GPUshowScreenPic");
    BindGpuWithStub(clearDynarec, "GPUclearDynarec");
    BindGpuWithStub(hSync, "GPUhSync");
    BindGpuWithStub(vBlank, "GPUvBlank");
    BindGpuWithStub(visualVibration, "GPUvisualVibration");
    BindGpuWithStub(cursor, "GPUcursor");
    BindGpuWithStub(addVertex, "GPUaddVertex");
    BindGpuWithStub(configure, "GPUconfigure");
    BindGpuWithStub(test, "GPUtest");
    BindGpuWithStub(about, "GPUabout");
}



long CALLBACK SPU_Stub_configure(void) { return 0; }
void CALLBACK SPU_Stub_about(void) {}
long CALLBACK SPU_Stub_test(void) { return 0; }


#define BindSpu(dest, name) \
    SPU_##dest = a_API->SPU_##dest;

#define BindSpuWithStub(dest, name) \
    SPU_##dest = a_API->SPU_##dest; \
    if (SPU_##dest == NULL)         \
        SPU_##dest = (SPU##dest)SPU_Stub_##dest;

void STDAPICALLTYPE setSPU(SPU_API *a_API)
{
    if (a_API == NULL)
        return;
	
    BindSpu(init, "SPUinit");
    BindSpu(shutdown, "SPUshutdown");
    BindSpu(open, "SPUopen");
    BindSpu(close, "SPUclose");
    BindSpu(writeRegister, "SPUwriteRegister");
    BindSpu(readRegister, "SPUreadRegister");
    BindSpu(writeDMA, "SPUwriteDMA");
    BindSpu(readDMA, "SPUreadDMA");
    BindSpu(writeDMAMem, "SPUwriteDMAMem");
    BindSpu(readDMAMem, "SPUreadDMAMem");
    BindSpu(playADPCMchannel, "SPUplayADPCMchannel");
    BindSpu(freeze, "SPUfreeze");
    BindSpu(registerCallback, "SPUregisterCallback");
    BindSpu(async, "SPUasync");
    BindSpu(playCDDAchannel, "SPUplayCDDAchannel");
    BindSpuWithStub(configure, "SPUconfigure");
    BindSpuWithStub(about, "SPUabout");
    BindSpuWithStub(test, "SPUtest");

    return 0;
}

extern unsigned char CALLBACK PAD1__startPoll(int pad);

extern unsigned char CALLBACK PAD1__poll(unsigned char value);

long CALLBACK PAD1_Stub_configure(void) { return 0; }
void CALLBACK PAD1_Stub_about(void) {}
long CALLBACK PAD1_Stub_test(void) { return 0; }
long CALLBACK PAD1_Stub_query(void) { return 3; }
long CALLBACK PAD1_Stub_keypressed() { return 0; }
void CALLBACK PAD1_Stub_registerVibration(void(CALLBACK *callback)(unsigned long, unsigned long)) {}
void CALLBACK PAD1_Stub_registerCursor(void(CALLBACK *callback)(int, int, int)) {}


#define BindPad1(dest, name) \
    PAD1_##dest = a_API->PAD_##dest;

#define BindPad1WithStub(dest, name) \
    PAD1_##dest = a_API->PAD_##dest; \
    if (PAD1_##dest == NULL)         \
        PAD1_##dest = (PAD##dest)PAD1_Stub_##dest;

#define BindPad1WithFunc(dest, name)  \
    PAD1_##dest = a_API->PAD_##dest; \
    if (PAD1_##dest == NULL)          \
        PAD1_##dest = (PAD##dest)PAD1__##dest;





unsigned char CALLBACK PAD2_Stub_startPoll(int pad)
{
    PadDataS padd;

    PAD2_readPort2(&padd);

    return _PADstartPoll(&padd);
}

unsigned char CALLBACK PAD2_Stub_poll(unsigned char value)
{
    return _PADpoll(value);
}

long CALLBACK PAD2_Stub_configure(void) { return 0; }
void CALLBACK PAD2_Stub_about(void) {}
long CALLBACK PAD2_Stub_test(void) { return 0; }
long CALLBACK PAD2_Stub_query(void) { return PSE_PAD_USE_PORT1 | PSE_PAD_USE_PORT2; }
long CALLBACK PAD2_Stub_keypressed() { return 0; }
void CALLBACK PAD2_Stub_registerVibration(void(CALLBACK *callback)(unsigned long, unsigned long)) {}
void CALLBACK PAD2_Stub_registerCursor(void(CALLBACK *callback)(int, int, int)) {}


//#define BindPad2(dest, name) \
//    PAD2_##dest = a_API->PAD_##dest;
//
//#define BindPad2WithStub(dest, name)  \
//    PAD2_##dest = a_API->PAD_##dest; \
//    if (PAD2_##dest == NULL)          \
//        PAD2_##dest = (PAD##dest)PAD2_Stub_##dest;
//
//#define BindPad2WithFunc(dest, name)  \
//    PAD2_##dest = a_API->PAD_##dest; \
//    if (PAD2_##dest == NULL)          \
//        PAD2_##dest = (PAD##dest)PAD2_Stub_##dest;


//
//void STDAPICALLTYPE setPAD(PAD_API *a_API)
//{
//    if (a_API == NULL)
//        return;
//
//    BindPad1(init, "PADinit");
//    BindPad1(shutdown, "PADshutdown");
//    BindPad1(open, "PADopen");
//    BindPad1(close, "PADclose");
//    BindPad1(readPort1, "PADreadPort1");
//    BindPad1(setSensitive, "PADsetSensitive");
//    BindPad1WithStub(configure, "PADconfigure");
//    BindPad1WithStub(test, "PADtest");
//    BindPad1WithStub(about, "PADabout");
//    BindPad1WithStub(keypressed, "PADkeypressed");
//    BindPad1WithFunc(startPoll, "PADstartPoll");
//    BindPad1WithFunc(poll, "PADpoll");
//    BindPad1WithStub(registerVibration, "PADregisterVibration");
//    BindPad1WithStub(registerCursor, "PADregisterCursor");
//    BindPad1WithStub(query, "PADquery");
//
//
//    BindPad2(init, "PADinit");
//    BindPad2(shutdown, "PADshutdown");
//    BindPad2(open, "PADopen");
//    BindPad2(close, "PADclose");
//    BindPad2(readPort2, "PADreadPort2");
//    BindPad2(setSensitive, "PADsetSensitive");
//    BindPad2WithStub(configure, "PADconfigure");
//    BindPad2WithStub(test, "PADtest");
//    BindPad2WithStub(about, "PADabout");
//    BindPad2WithStub(keypressed, "PADkeypressed");
//    BindPad2WithFunc(startPoll, "PADstartPoll");
//    BindPad2WithFunc(poll, "PADpoll");
//    BindPad2WithStub(registerVibration, "PADregisterVibration");
//    BindPad2WithStub(registerCursor, "PADregisterCursor");
//    BindPad2WithStub(query, "PADquery");
//
//    return 0;
//}


//typedef void(CALLBACK *GPUaddVertex)(short, short, s64, s64, s64);

		// PAD

//typedef uint8_t(CALLBACK *PADstartPoll_proxy)(int32_t);
//typedef uint8_t(CALLBACK *PADpoll_proxy)(uint8_t);
typedef int32_t(CALLBACK *PADsetSlot)(uint8_t, uint8_t);

typedef struct
{
    PADstartPoll PADstartPoll;
    PADpoll PADpoll;
    PADsetSlot PADsetSlot;
} PAD_API;




long CALLBACK Stub_PADkeypressed(void)
{
    return 0;
}

void CALLBACK Stub_PADregisterCursor(void(CALLBACK *callback)(int, int, int))
{
}

void CALLBACK Stub_PADsetSensitive(int a_firstArg)
{
}

void CALLBACK Stub_PADconfigure(void)
{
}

void CALLBACK Stub_PADabout(void)
{
}

s32 CALLBACK Stub_PADtest(void)
{
    return 0;
}

void CALLBACK Stub_PADregisterVibration(void(CALLBACK *callback)(unsigned long, unsigned long))
{
}

s32 CALLBACK Stub_PADinit(u32 flags)
{
    return 0;
}

void CALLBACK Stub_PADshutdown(void)
{
}

s32 CALLBACK Stub_PADopen(HWND hWnd)
{
    return 0;
}

void CALLBACK Stub_PADclose(void)
{
}

u32 CALLBACK Stub_PADquery(void)
{
    return 3;
}


long CALLBACK Stub_PADreadPort1(PadDataS *pads)
{
    return 0;
}

long CALLBACK Stub_PADreadPort2(PadDataS *pads)
{
    return 0;
}


#define BindPad2(dest, name) \
    PAD2_##dest = name;

#define BindPad2WithStub(dest, name) \
    PAD2_##dest = name; \
    if (PAD2_##dest == NULL)         \
        PAD2_##dest = (PAD##dest)PAD2_Stub_##dest;

#define BindPad2WithFunc(dest, name) \
    PAD2_##dest = a_API->PAD_##dest; \
    if (PAD2_##dest == NULL)         \
        PAD2_##dest = (PAD##dest)PAD2_Stub_##dest;


#define BindPad1(dest, name) \
    PAD1_##dest = name;

#define BindPad1WithStub(dest, name) \
    PAD1_##dest = name; \
    if (PAD1_##dest == NULL)         \
        PAD1_##dest = (PAD##dest)PAD1_Stub_##dest;

#define BindPad1WithFunc(dest, name) \
    PAD1_##dest = a_API->PAD_##dest; \
    if (PAD1_##dest == NULL)         \
        PAD1_##dest = (PAD##dest)PAD1__##dest;



void STDAPICALLTYPE setPAD(PAD_API *a_API)
{
    if (a_API == NULL)
        return;

    BindPad1(init, Stub_PADinit);
    BindPad1(shutdown, Stub_PADshutdown);
    BindPad1(open, Stub_PADopen);
    BindPad1(close, Stub_PADclose);
    BindPad1(readPort1, Stub_PADreadPort1);
    BindPad1(setSensitive, Stub_PADsetSensitive);
    BindPad1WithStub(configure, Stub_PADconfigure);
    BindPad1WithStub(test, Stub_PADtest);
    BindPad1WithStub(about, Stub_PADabout);
    BindPad1WithStub(keypressed, Stub_PADkeypressed);
    PAD1_startPoll = a_API->PADstartPoll;
    PAD1_poll = a_API->PADpoll;
    BindPad1WithStub(registerVibration, Stub_PADregisterVibration);
    BindPad1WithStub(registerCursor, Stub_PADregisterCursor);
    BindPad1WithStub(query, Stub_PADquery);


    BindPad2(init, Stub_PADinit);
    BindPad2(shutdown, Stub_PADshutdown);
    BindPad2(open, Stub_PADopen);
    BindPad2(close, Stub_PADclose);
    BindPad2(readPort2, Stub_PADreadPort2);
    BindPad2(setSensitive, Stub_PADsetSensitive);
    BindPad2WithStub(configure, Stub_PADconfigure);
    BindPad2WithStub(test, Stub_PADtest);
    BindPad2WithStub(about, Stub_PADabout);
    BindPad2WithStub(keypressed, Stub_PADkeypressed);
    PAD2_startPoll = a_API->PADstartPoll;
    PAD2_poll = a_API->PADpoll;
    BindPad2WithStub(registerVibration, Stub_PADregisterVibration);
    BindPad2WithStub(registerCursor, Stub_PADregisterCursor);
    BindPad2WithStub(query, Stub_PADquery);

    return 0;
}




long CALLBACK SIO1_Stub_init(void) { return 0; }
long CALLBACK SIO1_Stub_shutdown(void) { return 0; }
long CALLBACK SIO1_Stub_open(void) { return 0; }
long CALLBACK SIO1_Stub_close(void) { return 0; }
long CALLBACK SIO1_Stub_configure(void) { return 0; }
long CALLBACK SIO1_Stub_test(void) { return 0; }
void CALLBACK SIO1_Stub_about(void) {}
void CALLBACK SIO1_Stub_pause(void) {}
void CALLBACK SIO1_Stub_resume(void) {}
long CALLBACK SIO1_Stub_keypressed(int key) { return 0; }
void CALLBACK SIO1_Stub_writeData8(u8 val) {}
void CALLBACK SIO1_Stub_writeData16(u16 val) {}
void CALLBACK SIO1_Stub_writeData32(u32 val) {}
void CALLBACK SIO1_Stub_writeStat16(u16 val) {}
void CALLBACK SIO1_Stub_writeStat32(u32 val) {}
void CALLBACK SIO1_Stub_writeMode16(u16 val) {}
void CALLBACK SIO1_Stub_writeMode32(u32 val) {}
void CALLBACK SIO1_Stub_writeCtrl16(u16 val) {}
void CALLBACK SIO1_Stub_writeCtrl32(u32 val) {}
void CALLBACK SIO1_Stub_writeBaud16(u16 val) {}
void CALLBACK SIO1_Stub_writeBaud32(u32 val) {}
u8 CALLBACK SIO1_Stub_readData8(void) { return 0; }
u16 CALLBACK SIO1_Stub_readData16(void) { return 0; }
u32 CALLBACK SIO1_Stub_readData32(void) { return 0; }
u16 CALLBACK SIO1_Stub_readStat16(void) { return 0; }
u32 CALLBACK SIO1_Stub_readStat32(void) { return 0; }
u16 CALLBACK SIO1_Stub_readMode16(void) { return 0; }
u32 CALLBACK SIO1_Stub_readMode32(void) { return 0; }
u16 CALLBACK SIO1_Stub_readCtrl16(void) { return 0; }
u32 CALLBACK SIO1_Stub_readCtrl32(void) { return 0; }
u16 CALLBACK SIO1_Stub_readBaud16(void) { return 0; }
u32 CALLBACK SIO1_Stub_readBaud32(void) { return 0; }
void CALLBACK SIO1_Stub_update(uint32_t t){};
void CALLBACK SIO1_Stub_registerCallback(void(CALLBACK *callback)(void)){};


#define BindSio1WithStub(dest, name)  \
    SIO1_##dest = a_API->SIO1_##dest; \
    if (SIO1_##dest == NULL)          \
        SIO1_##dest = (SIO1##dest)SIO1_Stub_##dest;

void STDAPICALLTYPE setSIO1(SIO1_API *a_API)
{
    if (a_API == NULL)
        return;

    BindSio1WithStub(init, "SIO1init");
    BindSio1WithStub(shutdown, "SIO1shutdown");
    BindSio1WithStub(open, "SIO1open");
    BindSio1WithStub(close, "SIO1close");
    BindSio1WithStub(pause, "SIO1pause");
    BindSio1WithStub(resume, "SIO1resume");
    BindSio1WithStub(keypressed, "SIO1keypressed");
    BindSio1WithStub(configure, "SIO1configure");
    BindSio1WithStub(test, "SIO1test");
    BindSio1WithStub(about, "SIO1about");
    BindSio1WithStub(writeData8, "SIO1writeData8");
    BindSio1WithStub(writeData16, "SIO1writeData16");
    BindSio1WithStub(writeData32, "SIO1writeData32");
    BindSio1WithStub(writeStat16, "SIO1writeStat16");
    BindSio1WithStub(writeStat32, "SIO1writeStat32");
    BindSio1WithStub(writeMode16, "SIO1writeMode16");
    BindSio1WithStub(writeMode32, "SIO1writeMode32");
    BindSio1WithStub(writeCtrl16, "SIO1writeCtrl16");
    BindSio1WithStub(writeCtrl32, "SIO1writeCtrl32");
    BindSio1WithStub(writeBaud16, "SIO1writeBaud16");
    BindSio1WithStub(writeBaud32, "SIO1writeBaud32");
    BindSio1WithStub(readData8, "SIO1readData8");
    BindSio1WithStub(readData16, "SIO1readData16");
    BindSio1WithStub(readData32, "SIO1readData32");
    BindSio1WithStub(readStat16, "SIO1readStat16");
    BindSio1WithStub(readStat32, "SIO1readStat32");
    BindSio1WithStub(readMode16, "SIO1readMode16");
    BindSio1WithStub(readMode32, "SIO1readMode32");
    BindSio1WithStub(readCtrl16, "SIO1readCtrl16");
    BindSio1WithStub(readCtrl32, "SIO1readCtrl32");
    BindSio1WithStub(readBaud16, "SIO1readBaud16");
    BindSio1WithStub(readBaud32, "SIO1readBaud32");
    BindSio1WithStub(update, "SIO1update");
    BindSio1WithStub(registerCallback, "SIO1registerCallback");

    return 0;
}

int InitPlugins()
{
    long ret;
    char Plugin[MAXPATHLEN];

    ReleasePlugins();

	if (UsingIso()) {
        cdrIsoInit();
    } else {
            return -1;
    }
	   	 
    //    sprintf(Plugin, "%s/%s", Config.PluginsDir, Config.Pad2);
    //    if (LoadPAD2plugin(Plugin) == -1)
    //        return -1;
    //
    //    if (strcmp("Disabled", Config.Net) == 0 || strcmp("", Config.Net) == 0)
            Config.UseNet = FALSE;
    //    else {
    //        Config.UseNet = TRUE;
    //        sprintf(Plugin, "%s/%s", Config.PluginsDir, Config.Net);
    //        if (LoadNETplugin(Plugin) == -1)
    //            Config.UseNet = FALSE;
    //    }



    ret = CDR_init();
    if (ret < 0) {
        SysMessage(_("Error initializing CD-ROM plugin: %d"), ret);
        return -1;
    }
    ret = GPU_init();
    if (ret < 0) {
        SysMessage(_("Error initializing GPU plugin: %d"), ret);
        return -1;
    }
    ret = SPU_init();
    if (ret < 0) {
        SysMessage(_("Error initializing SPU plugin: %d"), ret);
        return -1;
    }
    ret = PAD1_init(1);
    if (ret < 0) {
        SysMessage(_("Error initializing Controller 1 plugin: %d"), ret);
        return -1;
    }
    ret = PAD2_init(2);
    if (ret < 0) {
        SysMessage(_("Error initializing Controller 2 plugin: %d"), ret);
        return -1;
    }

    if (Config.UseNet) {
        ret = NET_init();
        if (ret < 0) {
            SysMessage(_("Error initializing NetPlay plugin: %d"), ret);
            return -1;
        }
    }

#ifdef ENABLE_SIO1API
    ret = SIO1_init();
    if (ret < 0) {
        SysMessage(_("Error initializing SIO1 plugin: %d"), ret);
        return -1;
    }
#endif

    SysPrintf("%s", _("Plugins loaded.\n"));
    return 0;
}
