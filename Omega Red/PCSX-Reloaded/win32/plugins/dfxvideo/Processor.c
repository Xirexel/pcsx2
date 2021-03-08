#define ENABLE_SIO1API


#include "PCSXLib_API.h"



long CALLBACK Proxy_GPUopen(HWND);
long CALLBACK Proxy_GPUinit(void);
long CALLBACK Proxy_GPUshutdown(void);
long CALLBACK Proxy_GPUclose(void);
void CALLBACK Proxy_GPUwriteStatus(uint32_t);
void CALLBACK Proxy_GPUwriteData(uint32_t);
void CALLBACK Proxy_GPUwriteDataMem(uint32_t *, int);
uint32_t CALLBACK Proxy_GPUreadStatus(void);
uint32_t CALLBACK Proxy_GPUreadData(void);
void CALLBACK Proxy_GPUreadDataMem(uint32_t *, int);
long CALLBACK Proxy_GPUdmaChain(uint32_t *, uint32_t);
void CALLBACK Proxy_GPUupdateLace(void);
long CALLBACK Proxy_GPUconfigure(void);
long CALLBACK Proxy_GPUtest(void);
void CALLBACK Proxy_GPUabout(void);
void CALLBACK Proxy_GPUmakeSnapshot(void);
void CALLBACK Proxy_GPUkeypressed(int);
void CALLBACK Proxy_GPUdisplayText(char *);

long CALLBACK Proxy_GPUfreeze(uint32_t, GPUFreeze_t *);
long CALLBACK Proxy_GPUgetScreenPic(unsigned char *);
long CALLBACK Proxy_GPUshowScreenPic(unsigned char *);
void CALLBACK Proxy_GPUclearDynarec(void CALLBACK Proxy_callback(void));
void CALLBACK Proxy_GPUhSync(int);
void CALLBACK Proxy_GPUvBlank(int);
void CALLBACK Proxy_GPUvisualVibration(uint32_t, uint32_t);
void CALLBACK Proxy_GPUcursor(int, int, int);
void CALLBACK Proxy_GPUaddVertex(short, short, s64, s64, s64);



GS_API g_API = {
    Proxy_GPUupdateLace,
    Proxy_GPUinit,
	Proxy_GPUshutdown,
	Proxy_GPUconfigure,
	Proxy_GPUtest,
	Proxy_GPUabout,
	Proxy_GPUopen,
	Proxy_GPUclose,
	Proxy_GPUreadStatus,
	Proxy_GPUreadData,
	Proxy_GPUreadDataMem,
	Proxy_GPUwriteStatus,
	Proxy_GPUwriteData,
	Proxy_GPUwriteDataMem,
	Proxy_GPUdmaChain,
	Proxy_GPUkeypressed,
	Proxy_GPUdisplayText,
	Proxy_GPUmakeSnapshot,
	Proxy_GPUfreeze,
	Proxy_GPUgetScreenPic,
	Proxy_GPUshowScreenPic,
	Proxy_GPUclearDynarec,
	Proxy_GPUhSync,
	Proxy_GPUvBlank,
	Proxy_GPUvisualVibration,
	Proxy_GPUcursor,
	Proxy_GPUaddVertex
};

void* g_ptrAPI = &g_API;