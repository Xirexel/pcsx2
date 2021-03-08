#include "stdafx.h"
#include "externals.h"
#include <stdint.h>

typedef int64_t s64;



long CALLBACK GPUopen(HWND);
long CALLBACK GPUinit(void);
long CALLBACK GPUshutdown(void);
long CALLBACK GPUclose(void);
void CALLBACK GPUwriteStatus(uint32_t);
void CALLBACK GPUwriteData(uint32_t);
void CALLBACK GPUwriteDataMem(uint32_t *, int);
uint32_t CALLBACK GPUreadStatus(void);
uint32_t CALLBACK GPUreadData(void);
void CALLBACK GPUreadDataMem(uint32_t *, int);
long CALLBACK GPUdmaChain(uint32_t *, uint32_t);
void CALLBACK GPUupdateLace(void);
long CALLBACK GPUconfigure(void);
long CALLBACK GPUtest(void);
void CALLBACK GPUabout(void);
void CALLBACK GPUmakeSnapshot(void);
void CALLBACK GPUkeypressed(int);
void CALLBACK GPUdisplayText(char *);

long CALLBACK GPUfreeze(uint32_t, void *);
long CALLBACK GPUgetScreenPic(unsigned char *);
long CALLBACK GPUshowScreenPic(unsigned char *);
void CALLBACK GPUclearDynarec(void CALLBACK callback(void));
void CALLBACK GPUhSync(int);
void CALLBACK GPUvBlank(int);
void CALLBACK GPUvisualVibration(uint32_t, uint32_t);
void CALLBACK GPUcursor(int, int, int);
void CALLBACK GPUaddVertex(short, short, s64, s64, s64);



void CALLBACK Proxy_GPUupdateLace(void)
{
    GPUupdateLace();
}

long CALLBACK Proxy_GPUopen(HWND a_hwnd)
{
    return GPUopen(a_hwnd);
}

long CALLBACK Proxy_GPUinit(void)
{
    return GPUinit();
}

long CALLBACK Proxy_GPUshutdown(void)
{
    return GPUshutdown();
}

long CALLBACK Proxy_GPUclose(void)
{
    return GPUclose();
}

void CALLBACK Proxy_GPUwriteStatus(uint32_t a_firstArg)
{
    return GPUwriteStatus(a_firstArg);
}

void CALLBACK Proxy_GPUwriteData(uint32_t a_firstArg)
{
    GPUwriteData(a_firstArg);
}

void CALLBACK Proxy_GPUwriteDataMem(uint32_t *a_firstArg, int a_secondArg)
{
    GPUwriteDataMem(a_firstArg, a_secondArg);
}

uint32_t CALLBACK Proxy_GPUreadStatus(void)
{
    return GPUreadStatus();
}

uint32_t CALLBACK Proxy_GPUreadData(void)
{
    return GPUreadData();
}

void CALLBACK Proxy_GPUreadDataMem(uint32_t *a_firstArg, int a_secondArg)
{
    GPUreadDataMem(a_firstArg, a_secondArg);
}

long CALLBACK Proxy_GPUdmaChain(uint32_t *a_firstArg, uint32_t a_secondArg)
{
    return GPUdmaChain(a_firstArg, a_secondArg);
}


long CALLBACK Proxy_GPUconfigure(void)
{
    return 0; // GPUconfigure();
}

long CALLBACK Proxy_GPUtest(void)
{
    return 0; // GPUtest();
}

void CALLBACK Proxy_GPUabout(void)
{
    //GPUabout();
}

void CALLBACK Proxy_GPUmakeSnapshot(void)
{
    //GPUmakeSnapshot();
}

void CALLBACK Proxy_GPUkeypressed(int a_firstArg) {}

void CALLBACK Proxy_GPUdisplayText(char *a_firstArg)
{
    //GPUdisplayText(a_firstArg);
}


long CALLBACK Proxy_GPUfreeze(uint32_t a_firstArg, void *a_secondArg)
{
    GPUfreeze(a_firstArg, a_secondArg);
}

long CALLBACK Proxy_GPUgetScreenPic(unsigned char *a_firstArg)
{
    return 0; // GPUgetScreenPic(a_firstArg);
}

long CALLBACK Proxy_GPUshowScreenPic(unsigned char *a_firstArg)
{
    return 0; // GPUshowScreenPic(a_firstArg);
}

void CALLBACK Proxy_GPUclearDynarec(void CALLBACK Proxy_callback(void)) {}

void CALLBACK Proxy_GPUhSync(int a_firstArg)
{
    //GPUhSync(a_firstArg);
}

void CALLBACK Proxy_GPUvBlank(int a_firstArg)
{
    //GPUvBlank(a_firstArg);
}

void CALLBACK Proxy_GPUvisualVibration(uint32_t a_firstArg, uint32_t a_secondArg)
{
    //GPUvisualVibration(a_firstArg, a_secondArg);
}

void CALLBACK Proxy_GPUcursor(int a_firstArg, int a_secondArg, int a_thirdArg)
{
    //GPUcursor(a_firstArg, a_secondArg, a_thirdArg);
}

void CALLBACK Proxy_GPUaddVertex(short a_firstArg, short a_secondArg, s64 a_thirdArg, s64 a_fourthArg, s64 a_fifthArg) {}