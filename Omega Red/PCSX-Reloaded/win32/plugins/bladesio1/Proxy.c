
#include "stdafx.h"
#include <stdint.h>

typedef int64_t s64;

typedef uint8_t u8;
typedef uint16_t u16;
typedef uint32_t u32;
typedef uint64_t u64;
#define CALLBACK __stdcall


long CALLBACK SIO1open(HWND);
long CALLBACK SIO1init(void);
long CALLBACK SIO1shutdown(void);
long CALLBACK SIO1close(void);
long CALLBACK SIO1configure(void);
long CALLBACK SIO1test(void);
void CALLBACK SIO1about(void);
void CALLBACK SIO1pause(void);
void CALLBACK SIO1resume(void);
long CALLBACK SIO1keypressed(int);
void CALLBACK SIO1writeData8(u8);
void CALLBACK SIO1writeData16(u16);
void CALLBACK SIO1writeData32(u32);
void CALLBACK SIO1writeStat16(u16);
void CALLBACK SIO1writeStat32(u32);
void CALLBACK SIO1writeMode16(u16);
void CALLBACK SIO1writeMode32(u32);
void CALLBACK SIO1writeCtrl16(u16);
void CALLBACK SIO1writeCtrl32(u32);
void CALLBACK SIO1writeBaud16(u16);
void CALLBACK SIO1writeBaud32(u32);
u8 CALLBACK SIO1readData8(void);
u16 CALLBACK SIO1readData16(void);
u32 CALLBACK SIO1readData32(void);
u16 CALLBACK SIO1readStat16(void);
u32 CALLBACK SIO1readStat32(void);
u16 CALLBACK SIO1readMode16(void);
u32 CALLBACK SIO1readMode32(void);
u16 CALLBACK SIO1readCtrl16(void);
u32 CALLBACK SIO1readCtrl32(void);
u16 CALLBACK SIO1readBaud16(void);
u32 CALLBACK SIO1readBaud32(void);
void CALLBACK SIO1update(uint32_t);
void CALLBACK SIO1registerCallback(void(CALLBACK *callback)(void));



long CALLBACK Proxy_SIO1open(HWND a_hwnd)
{
    return SIO1open(a_hwnd);
}

long CALLBACK Proxy_SIO1init(void)
{
    return SIO1init();
}

long CALLBACK Proxy_SIO1shutdown(void)
{
    return SIO1shutdown();
}

long CALLBACK Proxy_SIO1close(void)
{
    return SIO1close();
}

long CALLBACK Proxy_SIO1configure(void)
{
    return SIO1configure();
}

long CALLBACK Proxy_SIO1test(void)
{
    return SIO1test();
}

void CALLBACK Proxy_SIO1about(void)
{
    SIO1about();
}

void CALLBACK Proxy_SIO1pause(void)
{
    SIO1pause();
}

void CALLBACK Proxy_SIO1resume(void)
{
    SIO1resume();
}

long CALLBACK Proxy_SIO1keypressed(int a_firstArg)
{
    return SIO1keypressed(a_firstArg);
}

void CALLBACK Proxy_SIO1writeData8(u8 a_firstArg)
{
    SIO1writeData8(a_firstArg);
}

void CALLBACK Proxy_SIO1writeData16(u16 a_firstArg)
{
    SIO1writeData16(a_firstArg);
}

void CALLBACK Proxy_SIO1writeData32(u32 a_firstArg)
{
    SIO1writeData32(a_firstArg);
}

void CALLBACK Proxy_SIO1writeStat16(u16 a_firstArg)
{
    SIO1writeStat16(a_firstArg);
}

void CALLBACK Proxy_SIO1writeStat32(u32 a_firstArg)
{
    SIO1writeStat32(a_firstArg);
}

void CALLBACK Proxy_SIO1writeMode16(u16 a_firstArg)
{
    SIO1writeMode16(a_firstArg);
}

void CALLBACK Proxy_SIO1writeMode32(u32 a_firstArg)
{
    SIO1writeMode32(a_firstArg);
}

void CALLBACK Proxy_SIO1writeCtrl16(u16 a_firstArg)
{
    SIO1writeCtrl16(a_firstArg);
}

void CALLBACK Proxy_SIO1writeCtrl32(u32 a_firstArg)
{
    SIO1writeCtrl32(a_firstArg);
}

void CALLBACK Proxy_SIO1writeBaud16(u16 a_firstArg)
{
    SIO1writeBaud16(a_firstArg);
}

void CALLBACK Proxy_SIO1writeBaud32(u32 a_firstArg)
{
    SIO1writeBaud32(a_firstArg);
}

u8 CALLBACK Proxy_SIO1readData8(void)
{
    return SIO1readData8();
}

u16 CALLBACK Proxy_SIO1readData16(void)
{
    return SIO1readData16();
}

u32 CALLBACK Proxy_SIO1readData32(void)
{
    return SIO1readData32();
}

u16 CALLBACK Proxy_SIO1readStat16(void)
{
    return SIO1readStat16();
}

u32 CALLBACK Proxy_SIO1readStat32(void)
{
    return SIO1readStat32();
}

u16 CALLBACK Proxy_SIO1readMode16(void)
{
    return SIO1readMode16();
}

u32 CALLBACK Proxy_SIO1readMode32(void)
{
    return SIO1readMode32();
}

u16 CALLBACK Proxy_SIO1readCtrl16(void)
{
    return SIO1readCtrl16();
}

u32 CALLBACK Proxy_SIO1readCtrl32(void)
{
    return SIO1readCtrl32();
}

u16 CALLBACK Proxy_SIO1readBaud16(void)
{
    return SIO1readBaud16();
}

u32 CALLBACK Proxy_SIO1readBaud32(void)
{
    return SIO1readBaud32();
}

void CALLBACK Proxy_SIO1update(uint32_t a_firstArg)
{
    SIO1update(a_firstArg);
}

void CALLBACK Proxy_SIO1registerCallback(void(CALLBACK *callback)(void))
{
    SIO1registerCallback(callback);
}
