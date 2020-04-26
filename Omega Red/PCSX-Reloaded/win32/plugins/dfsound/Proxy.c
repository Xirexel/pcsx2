
#include "stdafx.h"
#include "externals.h"
#include <stdint.h>

typedef int64_t s64;
#define CALLBACK __stdcall

long CALLBACK SPUinit(void);
long CALLBACK SPUopen(HWND);
long CALLBACK SPUclose(void);
long CALLBACK SPUshutdown(void);
void CALLBACK SPUplaySample(unsigned char);
void CALLBACK SPUwriteRegister(unsigned long, unsigned short);
unsigned short CALLBACK SPUreadRegister(unsigned long);
void CALLBACK SPUwriteDMA(unsigned short);
unsigned short CALLBACK SPUreadDMA(void);
void CALLBACK SPUwriteDMAMem(unsigned short *, int);
void CALLBACK SPUreadDMAMem(unsigned short *, int);
void CALLBACK SPUplayADPCMchannel(void *);
void CALLBACK SPUregisterCallback(void(CALLBACK *callback)(void));
long CALLBACK SPUconfigure(void);
long CALLBACK SPUtest(void);
void CALLBACK SPUabout(void);
long CALLBACK SPUfreeze(uint32_t, void *);
void CALLBACK SPUasync(uint32_t);
void CALLBACK SPUplayCDDAchannel(short *, int);

void *g_WindowHandle = NULL;

void unblock();

long CALLBACK Proxy_SPUopen(HWND a_hwnd)
{
    if (g_WindowHandle == NULL)
        return 0;

	SPUinit();

	unblock();

    return SPUopen(g_WindowHandle);
}

long CALLBACK Proxy_SPUinit(void){}

long CALLBACK Proxy_SPUshutdown(void){}

long CALLBACK Proxy_SPUclose(void)
{
	SPUclose();
    SPUshutdown();
    //return SPUclose();
}

void CALLBACK Proxy_SPUplaySample(unsigned char a_firstArg)
{
    SPUplaySample(a_firstArg);
}

void CALLBACK Proxy_SPUwriteRegister(unsigned long a_firstArg, unsigned short a_secondArg)
{
    SPUwriteRegister(a_firstArg, a_secondArg);
}

unsigned short CALLBACK Proxy_SPUreadRegister(unsigned long a_firstArg)
{
    return SPUreadRegister(a_firstArg);
}

void CALLBACK Proxy_SPUwriteDMA(unsigned short a_firstArg)
{
    SPUwriteDMA(a_firstArg);
}

unsigned short CALLBACK Proxy_SPUreadDMA(void)
{
    return SPUreadDMA();
}

void CALLBACK Proxy_SPUwriteDMAMem(unsigned short *a_firstArg, int a_secondArg)
{
    SPUwriteDMAMem(a_firstArg, a_secondArg);
}

void CALLBACK Proxy_SPUreadDMAMem(unsigned short *a_firstArg, int a_secondArg)
{
    SPUreadDMAMem(a_firstArg, a_secondArg);
}

void CALLBACK Proxy_SPUplayADPCMchannel(void *a_firstArg)
{
    SPUplayADPCMchannel(a_firstArg);
}

void CALLBACK Proxy_SPUregisterCallback(void(CALLBACK *callback)(void))
{
    SPUregisterCallback(callback);
}

long CALLBACK Proxy_SPUconfigure(void)
{
    return SPUconfigure();
}

long CALLBACK Proxy_SPUtest(void)
{
    return SPUtest();
}

void CALLBACK Proxy_SPUabout(void)
{
    SPUabout();
}

long CALLBACK Proxy_SPUfreeze(uint32_t a_firstArg, void *a_secondArg)
{
    return SPUfreeze(a_firstArg, a_secondArg);
}

void CALLBACK Proxy_SPUasync(uint32_t a_firstArg)
{
    SPUasync(a_firstArg);
}

void CALLBACK Proxy_SPUplayCDDAchannel(short *a_firstArg, int a_secondArg)
{
    //SPUplayCDDAchannel(a_firstArg, a_secondArg);
}
