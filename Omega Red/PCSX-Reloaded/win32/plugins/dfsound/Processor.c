#define ENABLE_SIO1API

#include "PCSXLib_API.h"



long CALLBACK Proxy_SPUopen(HWND);
long CALLBACK Proxy_SPUinit(void);
long CALLBACK Proxy_SPUshutdown(void);
long CALLBACK Proxy_SPUclose(void);
void CALLBACK Proxy_SPUplaySample(unsigned char);
void CALLBACK Proxy_SPUwriteRegister(unsigned long, unsigned short);
unsigned short CALLBACK Proxy_SPUreadRegister(unsigned long);
void CALLBACK Proxy_SPUwriteDMA(unsigned short);
unsigned short CALLBACK Proxy_SPUreadDMA(void);
void CALLBACK Proxy_SPUwriteDMAMem(unsigned short *, int);
void CALLBACK Proxy_SPUreadDMAMem(unsigned short *, int);
void CALLBACK Proxy_SPUplayADPCMchannel(xa_decode_t *);
void CALLBACK Proxy_SPUregisterCallback(void CALLBACK Proxy_callback(void));
long CALLBACK Proxy_SPUconfigure(void);
long CALLBACK Proxy_SPUtest(void);
void CALLBACK Proxy_SPUabout(void);
long CALLBACK Proxy_SPUfreeze(uint32_t, void *);
void CALLBACK Proxy_SPUasync(uint32_t);
void CALLBACK Proxy_SPUplayCDDAchannel(short *, int);






SPU_API g_API = {
    Proxy_SPUconfigure,
    Proxy_SPUabout,
    Proxy_SPUinit,
    Proxy_SPUshutdown,
    Proxy_SPUtest,
    Proxy_SPUopen,
    Proxy_SPUclose,
    Proxy_SPUplaySample,
    Proxy_SPUwriteRegister,
    Proxy_SPUreadRegister,
    Proxy_SPUwriteDMA,
    Proxy_SPUreadDMA,
    Proxy_SPUwriteDMAMem,
    Proxy_SPUreadDMAMem,
    Proxy_SPUplayADPCMchannel,
    Proxy_SPUfreeze,
    Proxy_SPUregisterCallback,
    Proxy_SPUasync,
    Proxy_SPUplayCDDAchannel
};

void *g_ptrAPI = &g_API;