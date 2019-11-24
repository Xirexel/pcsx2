#define ENABLE_SIO1API

#include "PCSXLib_API.h"



long CALLBACK Proxy_SIO1open(HWND);

// SIO1 Functions (link cable)
long CALLBACK Proxy_SIO1init(void);
long CALLBACK Proxy_SIO1shutdown(void);
long CALLBACK Proxy_SIO1close(void);
long CALLBACK Proxy_SIO1configure(void);
long CALLBACK Proxy_SIO1test(void);
void CALLBACK Proxy_SIO1about(void);
void CALLBACK Proxy_SIO1pause(void);
void CALLBACK Proxy_SIO1resume(void);
long CALLBACK Proxy_SIO1keypressed(int);
void CALLBACK Proxy_SIO1writeData8(u8);
void CALLBACK Proxy_SIO1writeData16(u16);
void CALLBACK Proxy_SIO1writeData32(u32);
void CALLBACK Proxy_SIO1writeStat16(u16);
void CALLBACK Proxy_SIO1writeStat32(u32);
void CALLBACK Proxy_SIO1writeMode16(u16);
void CALLBACK Proxy_SIO1writeMode32(u32);
void CALLBACK Proxy_SIO1writeCtrl16(u16);
void CALLBACK Proxy_SIO1writeCtrl32(u32);
void CALLBACK Proxy_SIO1writeBaud16(u16);
void CALLBACK Proxy_SIO1writeBaud32(u32);
u8 CALLBACK Proxy_SIO1readData8(void);
u16 CALLBACK Proxy_SIO1readData16(void);
u32 CALLBACK Proxy_SIO1readData32(void);
u16 CALLBACK Proxy_SIO1readStat16(void);
u32 CALLBACK Proxy_SIO1readStat32(void);
u16 CALLBACK Proxy_SIO1readMode16(void);
u32 CALLBACK Proxy_SIO1readMode32(void);
u16 CALLBACK Proxy_SIO1readCtrl16(void);
u32 CALLBACK Proxy_SIO1readCtrl32(void);
u16 CALLBACK Proxy_SIO1readBaud16(void);
u32 CALLBACK Proxy_SIO1readBaud32(void);
void CALLBACK Proxy_SIO1update(uint32_t);
void CALLBACK Proxy_SIO1registerCallback(void(CALLBACK *callback)(void));



SIO1_API g_API = {

    Proxy_SIO1init,
Proxy_SIO1shutdown,
Proxy_SIO1open,
Proxy_SIO1close,
Proxy_SIO1test,
Proxy_SIO1configure,
Proxy_SIO1about,
Proxy_SIO1pause,
Proxy_SIO1resume,
Proxy_SIO1keypressed,
Proxy_SIO1writeData8,
Proxy_SIO1writeData16,
Proxy_SIO1writeData32,
Proxy_SIO1writeStat16,
Proxy_SIO1writeStat32,
Proxy_SIO1writeMode16,
Proxy_SIO1writeMode32,
Proxy_SIO1writeCtrl16,
Proxy_SIO1writeCtrl32,
Proxy_SIO1writeBaud16,
Proxy_SIO1writeBaud32,
Proxy_SIO1readData8,
Proxy_SIO1readData16,
Proxy_SIO1readData32,
Proxy_SIO1readStat16,
Proxy_SIO1readStat32,
Proxy_SIO1readMode16,
Proxy_SIO1readMode32,
Proxy_SIO1readCtrl16,
Proxy_SIO1readCtrl32,
Proxy_SIO1readBaud16,
Proxy_SIO1readBaud32,
Proxy_SIO1update,
Proxy_SIO1registerCallback
};

void *g_ptrAPI = &g_API;

void executeExecute(const wchar_t *a_command, wchar_t **a_result)
{
}