#include "stdafx.h"

#define _IN_GPU

#include "externals.h"
#include "draw.h"



void OutputLog(const char *szFormat, ...)
{
    char szBuff[1024];
    va_list arg;
    va_start(arg, szFormat);
    _vsnprintf_s(szBuff, sizeof(szBuff), sizeof(szBuff), szFormat, arg);
    va_end(arg);

    OutputDebugStringA(szBuff);
}

#define PSE_LT_GPU 2

static const char *libraryName = N_("Hardware GPU Driver");

uint32_t        ulStatusControl[256];

void SetFixes(void);
void ReadConfig(void);
void CheckVRamRead(int x, int y, int dx, int dy, BOOL bFront);
void updateFrontDisplay(void);
BOOL bSwapCheck(void);
void ChangeDispOffsetsX(void);
void ChangeDispOffsetsY(void);
void updateDisplay(void);
void updateDisplayIfChanged(void);

////////////////////////////////////////////////////////////////////////
// memory image of the PSX vram
////////////////////////////////////////////////////////////////////////

unsigned char  *psxVSecure;
unsigned char  *psxVub;
signed   char  *psxVsb;
unsigned short *psxVuw;
unsigned short *psxVuw_eom;
signed   short *psxVsw;
uint32_t       *psxVul;
signed   int   *psxVsl;


GLfloat         gl_z = 0.0f;
BOOL            bNeedInterlaceUpdate = FALSE;
BOOL            bNeedRGB24Update = FALSE;
BOOL            bChangeWinMode = FALSE;
BOOL			bNeedWriteUpload = FALSE;


////////////////////////////////////////////////////////////////////////
// global GPU vars
////////////////////////////////////////////////////////////////////////

static int      GPUdataRet;
int             lGPUstatusRet;
uint32_t        ulGPUInfoVals[16];
char            szGPUKeys[7];

char szKeyDefaults[7] = { VK_DELETE,VK_INSERT,VK_HOME,VK_END,VK_PRIOR,VK_NEXT,0x00 };


uint32_t        dwGPUVersion = 0;
int             iGPUHeight = 512;
int             iGPUHeightMask = 511;
int             GlobalTextIL = 0;
int             iTileCheat = 0;
int             iColDepth;
int             iUseScanLines = 0;
int             iBlurBuffer = 0;
int				iLastRGB24 = 0;                                      // special vars for checking when to skip two display updates
int				iSkipTwo = 0;
int             iWinSize;


// possible psx display widths
short dispWidths[8] = { 256,320,512,640,368,384,512,640 };

uint32_t lUsedAddr[3];


static uint32_t      gpuDataM[256];
static unsigned char gpuCommand = 0;
static int           gpuDataC = 0;
static int           gpuDataP = 0;

VRAMLoad_t      VRAMWrite;
VRAMLoad_t      VRAMRead;
int             iDataWriteMode;
int             iDataReadMode;

int             lClearOnSwap;
int             lClearOnSwapColor;
BOOL            bSkipNextFrame = FALSE;

PSXDisplay_t    PSXDisplay;
PSXDisplay_t    PreviousPSXDisplay;
TWin_t          TWin;
GLuint          uiScanLine = 0;
BOOL            bDisplayNotSet = TRUE;
BOOL            bWindowMode;
int             iNoScreenSaver = 0;
int             lSelectedSlot = 0;



uint32_t        vBlank = 0;
int             iRenderFVR = 0;
int             iScanBlend = 0;
unsigned char * pGfxCardScreen = 0;
int             iFakePrimBusy = 0;



////////////////////////////////////////////////////////////////////////
// vram read/write helpers
////////////////////////////////////////////////////////////////////////

static __inline void FinishedVRAMWrite(void)
{
	if (bNeedWriteUpload)
	{
		bNeedWriteUpload = FALSE;
		CheckWriteUpdate();
	}

	// set register to NORMAL operation
	iDataWriteMode = DR_NORMAL;

	// reset transfer values, to prevent mis-transfer of data
	VRAMWrite.ColsRemaining = 0;
	VRAMWrite.RowsRemaining = 0;
}

static __inline void FinishedVRAMRead(void)
{
	// set register to NORMAL operation
	iDataReadMode = DR_NORMAL;
	// reset transfer values, to prevent mis-transfer of data
	VRAMRead.x = 0;
	VRAMRead.y = 0;
	VRAMRead.Width = 0;
	VRAMRead.Height = 0;
	VRAMRead.ColsRemaining = 0;
	VRAMRead.RowsRemaining = 0;

	// indicate GPU is no longer ready for VRAM data in the STATUS REGISTER
	STATUSREG &= ~GPUSTATUS_READYFORVRAM;
}

////////////////////////////////////////////////////////////////////////
// stuff to make this a true PDK module
////////////////////////////////////////////////////////////////////////

EXPORT_C_(uint32) PSEgetLibType()
{
	return PSE_LT_GPU;
}

EXPORT_C_(const char*) PSEgetLibName()
{
	return libraryName;
}

EXPORT_C_(uint32) PSEgetLibVersion()
{
	static const uint32 version = 1;
	static const uint32 revision = 1;

	return version << 16 | revision << 8 | PLUGIN_VERSION;
}

EXPORT_C_(int32) GPUinit()
{
	memset(ulStatusControl, 0, 256 * sizeof(uint32_t));

	// different ways of accessing PSX VRAM

	psxVSecure = (unsigned char *)malloc((iGPUHeight * 2) * 1024 + (1024 * 1024)); // always alloc one extra MB for soft drawing funcs security
	if (!psxVSecure) return -1;

	psxVub = psxVSecure + 512 * 1024;                           // security offset into double sized psx vram!
	psxVsb = (signed char *)psxVub;
	psxVsw = (signed short *)psxVub;
	psxVsl = (signed int *)psxVub;
	psxVuw = (unsigned short *)psxVub;
	psxVul = (uint32_t *)psxVub;

	psxVuw_eom = psxVuw + 1024 * iGPUHeight;                    // pre-calc of end of vram

	memset(psxVSecure, 0x00, (iGPUHeight * 2) * 1024 + (1024 * 1024));
	memset(ulGPUInfoVals, 0x00, 16 * sizeof(uint32_t));

	InitFrameCap();                                       // init frame rate stuff

	PSXDisplay.RGB24 = 0;                          // init vars
	PreviousPSXDisplay.RGB24 = 0;
	PSXDisplay.Interlaced = 0;
	PSXDisplay.InterlacedTest = 0;
	PSXDisplay.DrawOffset.x = 0;
	PSXDisplay.DrawOffset.y = 0;
	PSXDisplay.DrawArea.x0 = 0;
	PSXDisplay.DrawArea.y0 = 0;
	PSXDisplay.DrawArea.x1 = 320;
	PSXDisplay.DrawArea.y1 = 240;
	PSXDisplay.DisplayMode.x = 320;
	PSXDisplay.DisplayMode.y = 240;
	PSXDisplay.Disabled = FALSE;
	PreviousPSXDisplay.Range.x0 = 0;
	PreviousPSXDisplay.Range.x1 = 0;
	PreviousPSXDisplay.Range.y0 = 0;
	PreviousPSXDisplay.Range.y1 = 0;
	PSXDisplay.Range.x0 = 0;
	PSXDisplay.Range.x1 = 0;
	PSXDisplay.Range.y0 = 0;
	PSXDisplay.Range.y1 = 0;
	PreviousPSXDisplay.DisplayPosition.x = 1;
	PreviousPSXDisplay.DisplayPosition.y = 1;
	PSXDisplay.DisplayPosition.x = 1;
	PSXDisplay.DisplayPosition.y = 1;
	PreviousPSXDisplay.DisplayModeNew.y = 0;
	PSXDisplay.Double = 1;
	GPUdataRet = 0x400;

	PSXDisplay.DisplayModeNew.x = 0;
	PSXDisplay.DisplayModeNew.y = 0;

	//PreviousPSXDisplay.Height = PSXDisplay.Height = 239;

	iDataWriteMode = DR_NORMAL;

	// Reset transfer values, to prevent mis-transfer of data
	memset(&VRAMWrite, 0, sizeof(VRAMLoad_t));
	memset(&VRAMRead, 0, sizeof(VRAMLoad_t));

	// device initialised already !
	//lGPUstatusRet = 0x74000000;
	vBlank = 0;

	STATUSREG = 0x14802000;
	GPUIsIdle;
	GPUIsReadyForCommands;

	return 0;
}

EXPORT_C_(int32) GPUshutdown()
{
	return 0;
}

EXPORT_C_(int32) CALLBACK GPUopen(HWND hwndGPU)
{
    GPU_Stub_open(hwndGPU);

	ReadConfig();                                       // -> read config from registry

	SetFrameRateConfig();                               // -> setup frame rate stuff
	   
	bIsFirstFrame = TRUE;                                 // flag: we have to init OGL later in windows!

	rRatioRect.left = rRatioRect.top = 0;
	rRatioRect.right = iResX;
	rRatioRect.bottom = iResY;
		
	bDisplayNotSet = TRUE;
	bSetClip = TRUE;

	SetFixes();                                           // setup game fixes

	InitializeTextureStore();                             // init texture mem

	resetGteVertices();

	// lGPUstatusRet = 0x74000000;

	// with some emus, we could do the OGL init right here... oh my
	// if(bIsFirstFrame) GLinitialize();

	return 0;
}


////////////////////////////////////////////////////////////////////////
// close
////////////////////////////////////////////////////////////////////////

long CALLBACK GPUclose()                               // WINDOWS CLOSE
{
	GLcleanup();                                          // close OGL
		
	if (pGfxCardScreen) free(pGfxCardScreen);              // free helper memory
	pGfxCardScreen = 0;
		
	GPU_Stub_close();

	return 0;
}

////////////////////////////////////////////////////////////////////////
// core read from vram
////////////////////////////////////////////////////////////////////////

void CALLBACK GPUreadDataMem(uint32_t *pMem, int iSize)
{
	int i;

	if (iDataReadMode != DR_VRAMTRANSFER) return;

	GPUIsBusy;

	// adjust read ptr, if necessary
	while (VRAMRead.ImagePtr >= psxVuw_eom)
		VRAMRead.ImagePtr -= iGPUHeight * 1024;
	while (VRAMRead.ImagePtr < psxVuw)
		VRAMRead.ImagePtr += iGPUHeight * 1024;

	if ((iFrameReadType & 1 && iSize > 1) &&
		!(iDrawnSomething == 2 &&
			VRAMRead.x == VRAMWrite.x     &&
			VRAMRead.y == VRAMWrite.y     &&
			VRAMRead.Width == VRAMWrite.Width &&
			VRAMRead.Height == VRAMWrite.Height))
		CheckVRamRead(VRAMRead.x, VRAMRead.y,
			VRAMRead.x + VRAMRead.RowsRemaining,
			VRAMRead.y + VRAMRead.ColsRemaining,
			TRUE);

	for (i = 0; i < iSize; i++)
	{
		// do 2 seperate 16bit reads for compatibility (wrap issues)
		if ((VRAMRead.ColsRemaining > 0) && (VRAMRead.RowsRemaining > 0))
		{
			// lower 16 bit
			GPUdataRet = (uint32_t)*VRAMRead.ImagePtr;

			VRAMRead.ImagePtr++;
			if (VRAMRead.ImagePtr >= psxVuw_eom) VRAMRead.ImagePtr -= iGPUHeight * 1024;
			VRAMRead.RowsRemaining--;

			if (VRAMRead.RowsRemaining <= 0)
			{
				VRAMRead.RowsRemaining = VRAMRead.Width;
				VRAMRead.ColsRemaining--;
				VRAMRead.ImagePtr += 1024 - VRAMRead.Width;
				if (VRAMRead.ImagePtr >= psxVuw_eom) VRAMRead.ImagePtr -= iGPUHeight * 1024;
			}

			// higher 16 bit (always, even if it's an odd width)
			GPUdataRet |= (uint32_t)(*VRAMRead.ImagePtr) << 16;
			*pMem++ = GPUdataRet;

			if (VRAMRead.ColsRemaining <= 0)
			{
				FinishedVRAMRead(); goto ENDREAD;
			}

			VRAMRead.ImagePtr++;
			if (VRAMRead.ImagePtr >= psxVuw_eom) VRAMRead.ImagePtr -= iGPUHeight * 1024;
			VRAMRead.RowsRemaining--;
			if (VRAMRead.RowsRemaining <= 0)
			{
				VRAMRead.RowsRemaining = VRAMRead.Width;
				VRAMRead.ColsRemaining--;
				VRAMRead.ImagePtr += 1024 - VRAMRead.Width;
				if (VRAMRead.ImagePtr >= psxVuw_eom) VRAMRead.ImagePtr -= iGPUHeight * 1024;
			}
			if (VRAMRead.ColsRemaining <= 0)
			{
				FinishedVRAMRead(); goto ENDREAD;
			}
		}
		else { FinishedVRAMRead(); goto ENDREAD; }
	}

ENDREAD:
	GPUIsIdle;
}

uint32_t CALLBACK GPUreadData(void)
{
	uint32_t l;
	GPUreadDataMem(&l, 1);
	return GPUdataRet;
}

////////////////////////////////////////////////////////////////////////
// update lace is called every VSync. Basically we limit frame rate 
// here, and in interlaced mode we swap ogl display buffers.
////////////////////////////////////////////////////////////////////////

static unsigned short usFirstPos = 2;

void CALLBACK GPUupdateLace(void)
{
	//if(!(dwActFixes&0x1000))                               
	// STATUSREG^=0x80000000;                               // interlaced bit toggle, if the CC game fix is not active (see gpuReadStatus)

	if (!(dwActFixes & 128))                                 // normal frame limit func
		CheckFrameRate();

	if (iOffscreenDrawing == 4)                              // special check if high offscreen drawing is on
	{
		if (bSwapCheck()) return;
	}

	if (PSXDisplay.Interlaced)                             // interlaced mode?
	{
		STATUSREG ^= 0x80000000;
		if (PSXDisplay.DisplayMode.x > 0 && PSXDisplay.DisplayMode.y > 0)
		{
			updateDisplay();                                  // -> swap buffers (new frame)
		}
	}
	else if (bRenderFrontBuffer)                           // no interlace mode? and some stuff in front has changed?
	{
		updateFrontDisplay();                               // -> update front buffer
	}
	else if (usFirstPos == 1)                                // initial updates (after startup)
	{
		updateDisplay();
	}

#if defined(_WINDOWS) || defined(_MACGL)
	//if (bChangeWinMode) ChangeWindowMode();
#endif
}

////////////////////////////////////////////////////////////////////////
// process read request from GPU status register
////////////////////////////////////////////////////////////////////////

uint32_t CALLBACK GPUreadStatus(void)
{
	if (dwActFixes & 0x1000)                                 // CC game fix
	{
		static int iNumRead = 0;
		if ((iNumRead++) == 2)
		{
			iNumRead = 0;
			STATUSREG ^= 0x80000000;                            // interlaced bit toggle... we do it on every second read status... needed by some games (like ChronoCross)
		}
	}

	if (iFakePrimBusy)                                     // 27.10.2007 - emulating some 'busy' while drawing... pfff... not perfect, but since our emulated dma is not done in an extra thread...
	{
		iFakePrimBusy--;

		if (iFakePrimBusy & 1)                                 // we do a busy-idle-busy-idle sequence after/while drawing prims
		{
			GPUIsBusy;
			GPUIsNotReadyForCommands;
		}
		else
		{
			GPUIsIdle;
			GPUIsReadyForCommands;
		}
	}

	return STATUSREG | (vBlank ? 0x80000000 : 0);;
}

////////////////////////////////////////////////////////////////////////
// processes data send to GPU status register
// these are always single packet commands.
////////////////////////////////////////////////////////////////////////

void CALLBACK GPUwriteStatus(uint32_t gdata)
{
	uint32_t lCommand = (gdata >> 24) & 0xff;

#ifdef _WINDOWS
	if (bIsFirstFrame) GLinitialize();                     // real ogl startup (needed by some emus)
#endif

	ulStatusControl[lCommand] = gdata;

	switch (lCommand)
	{
		//--------------------------------------------------//
		// reset gpu
	case 0x00:
		memset(ulGPUInfoVals, 0x00, 16 * sizeof(uint32_t));
		lGPUstatusRet = 0x14802000;
		PSXDisplay.Disabled = 1;
		iDataWriteMode = iDataReadMode = DR_NORMAL;
		PSXDisplay.DrawOffset.x = PSXDisplay.DrawOffset.y = 0;
		drawX = drawY = 0; drawW = drawH = 0;
		sSetMask = 0; lSetMask = 0; bCheckMask = FALSE; iSetMask = 0;
		usMirror = 0;
		GlobalTextAddrX = 0; GlobalTextAddrY = 0;
		GlobalTextTP = 0; GlobalTextABR = 0;
		PSXDisplay.RGB24 = FALSE;
		PSXDisplay.Interlaced = FALSE;
		bUsingTWin = FALSE;
		return;

		// dis/enable display
	case 0x03:
		PreviousPSXDisplay.Disabled = PSXDisplay.Disabled;
		PSXDisplay.Disabled = (gdata & 1);

		if (PSXDisplay.Disabled)
			STATUSREG |= GPUSTATUS_DISPLAYDISABLED;
		else STATUSREG &= ~GPUSTATUS_DISPLAYDISABLED;

		if (iOffscreenDrawing == 4 &&
			PreviousPSXDisplay.Disabled &&
			!(PSXDisplay.Disabled))
		{

			if (!PSXDisplay.RGB24)
			{
				PrepareFullScreenUpload(TRUE);
				UploadScreen(TRUE);
				updateDisplay();
			}
		}

		return;

		// setting transfer mode
	case 0x04:
		gdata &= 0x03;                                     // only want the lower two bits

		iDataWriteMode = iDataReadMode = DR_NORMAL;
		if (gdata == 0x02) iDataWriteMode = DR_VRAMTRANSFER;
		if (gdata == 0x03) iDataReadMode = DR_VRAMTRANSFER;

		STATUSREG &= ~GPUSTATUS_DMABITS;                     // clear the current settings of the DMA bits
		STATUSREG |= (gdata << 29);                          // set the DMA bits according to the received data

		return;

		// setting display position
	case 0x05:
	{
		short sx = (short)(gdata & 0x3ff);
		short sy;

		if (iGPUHeight == 1024)
		{
			if (dwGPUVersion == 2)
				sy = (short)((gdata >> 12) & 0x3ff);
			else sy = (short)((gdata >> 10) & 0x3ff);
		}
		else sy = (short)((gdata >> 10) & 0x3ff);             // really: 0x1ff, but we adjust it later

		if (sy & 0x200)
		{
			sy |= 0xfc00;
			PreviousPSXDisplay.DisplayModeNew.y = sy / PSXDisplay.Double;
			sy = 0;
		}
		else PreviousPSXDisplay.DisplayModeNew.y = 0;

		if (sx > 1000) sx = 0;

		if (usFirstPos)
		{
			usFirstPos--;
			if (usFirstPos)
			{
				PreviousPSXDisplay.DisplayPosition.x = sx;
				PreviousPSXDisplay.DisplayPosition.y = sy;
				PSXDisplay.DisplayPosition.x = sx;
				PSXDisplay.DisplayPosition.y = sy;
			}
		}

		if (dwActFixes & 8)
		{
			if ((!PSXDisplay.Interlaced) &&
				PreviousPSXDisplay.DisplayPosition.x == sx &&
				PreviousPSXDisplay.DisplayPosition.y == sy)
				return;

			PSXDisplay.DisplayPosition.x = PreviousPSXDisplay.DisplayPosition.x;
			PSXDisplay.DisplayPosition.y = PreviousPSXDisplay.DisplayPosition.y;
			PreviousPSXDisplay.DisplayPosition.x = sx;
			PreviousPSXDisplay.DisplayPosition.y = sy;
		}
		else
		{
			if ((!PSXDisplay.Interlaced) &&
				PSXDisplay.DisplayPosition.x == sx &&
				PSXDisplay.DisplayPosition.y == sy)
				return;
			PreviousPSXDisplay.DisplayPosition.x = PSXDisplay.DisplayPosition.x;
			PreviousPSXDisplay.DisplayPosition.y = PSXDisplay.DisplayPosition.y;
			PSXDisplay.DisplayPosition.x = sx;
			PSXDisplay.DisplayPosition.y = sy;
		}

		PSXDisplay.DisplayEnd.x =
			PSXDisplay.DisplayPosition.x + PSXDisplay.DisplayMode.x;
		PSXDisplay.DisplayEnd.y =
			PSXDisplay.DisplayPosition.y + PSXDisplay.DisplayMode.y + PreviousPSXDisplay.DisplayModeNew.y;

		PreviousPSXDisplay.DisplayEnd.x =
			PreviousPSXDisplay.DisplayPosition.x + PSXDisplay.DisplayMode.x;
		PreviousPSXDisplay.DisplayEnd.y =
			PreviousPSXDisplay.DisplayPosition.y + PSXDisplay.DisplayMode.y + PreviousPSXDisplay.DisplayModeNew.y;

		bDisplayNotSet = TRUE;

        iDrawnSomething = 1;

		if (!(PSXDisplay.Interlaced))
		{
			updateDisplay();
		}
		else
			if (PSXDisplay.InterlacedTest &&
				((PreviousPSXDisplay.DisplayPosition.x != PSXDisplay.DisplayPosition.x) ||
				(PreviousPSXDisplay.DisplayPosition.y != PSXDisplay.DisplayPosition.y)))
				PSXDisplay.InterlacedTest--;

		return;
	}

	// setting width
	case 0x06:

		PSXDisplay.Range.x0 = gdata & 0x7ff;      //0x3ff;
		PSXDisplay.Range.x1 = (gdata >> 12) & 0xfff;//0x7ff;

		PSXDisplay.Range.x1 -= PSXDisplay.Range.x0;

		ChangeDispOffsetsX();

		return;

		// setting height
	case 0x07:

		PreviousPSXDisplay.Height = PSXDisplay.Height;

		PSXDisplay.Range.y0 = gdata & 0x3ff;
		PSXDisplay.Range.y1 = (gdata >> 10) & 0x3ff;

		PSXDisplay.Height = PSXDisplay.Range.y1 -
			PSXDisplay.Range.y0 +
			PreviousPSXDisplay.DisplayModeNew.y;

		if (PreviousPSXDisplay.Height != PSXDisplay.Height)
		{
			PSXDisplay.DisplayModeNew.y = PSXDisplay.Height*PSXDisplay.Double;
			ChangeDispOffsetsY();
			updateDisplayIfChanged();
		}
		return;

		// setting display infos
	case 0x08:

		PSXDisplay.DisplayModeNew.x = dispWidths[(gdata & 0x03) | ((gdata & 0x40) >> 4)];

		if (gdata & 0x04) PSXDisplay.Double = 2;
		else            PSXDisplay.Double = 1;
		PSXDisplay.DisplayModeNew.y = PSXDisplay.Height*PSXDisplay.Double;

		ChangeDispOffsetsY();

		PSXDisplay.PAL = (gdata & 0x08) ? TRUE : FALSE; // if 1 - PAL mode, else NTSC
		PSXDisplay.RGB24New = (gdata & 0x10) ? TRUE : FALSE; // if 1 - TrueColor
		PSXDisplay.InterlacedNew = (gdata & 0x20) ? TRUE : FALSE; // if 1 - Interlace

		STATUSREG &= ~GPUSTATUS_WIDTHBITS;                   // clear the width bits

		STATUSREG |=
			(((gdata & 0x03) << 17) |
			((gdata & 0x40) << 10));                // set the width bits

		PreviousPSXDisplay.InterlacedNew = FALSE;
		if (PSXDisplay.InterlacedNew)
		{
			if (!PSXDisplay.Interlaced)
			{
				PSXDisplay.InterlacedTest = 2;
				PreviousPSXDisplay.DisplayPosition.x = PSXDisplay.DisplayPosition.x;
				PreviousPSXDisplay.DisplayPosition.y = PSXDisplay.DisplayPosition.y;
				PreviousPSXDisplay.InterlacedNew = TRUE;
			}

			STATUSREG |= GPUSTATUS_INTERLACED;
		}
		else
		{
			PSXDisplay.InterlacedTest = 0;
			STATUSREG &= ~GPUSTATUS_INTERLACED;
		}

		if (PSXDisplay.PAL)
			STATUSREG |= GPUSTATUS_PAL;
		else STATUSREG &= ~GPUSTATUS_PAL;

		if (PSXDisplay.Double == 2)
			STATUSREG |= GPUSTATUS_DOUBLEHEIGHT;
		else STATUSREG &= ~GPUSTATUS_DOUBLEHEIGHT;

		if (PSXDisplay.RGB24New)
			STATUSREG |= GPUSTATUS_RGB24;
		else STATUSREG &= ~GPUSTATUS_RGB24;

		updateDisplayIfChanged();

		return;

		//--------------------------------------------------//
		// ask about GPU version and other stuff
	case 0x10:

		gdata &= 0xff;

		switch (gdata)
		{
		case 0x02:
			GPUdataRet = ulGPUInfoVals[INFO_TW];              // tw infos
			return;
		case 0x03:
			GPUdataRet = ulGPUInfoVals[INFO_DRAWSTART];       // draw start
			return;
		case 0x04:
			GPUdataRet = ulGPUInfoVals[INFO_DRAWEND];         // draw end
			return;
		case 0x05:
		case 0x06:
			GPUdataRet = ulGPUInfoVals[INFO_DRAWOFF];         // draw offset
			return;
		case 0x07:
			if (dwGPUVersion == 2)
				GPUdataRet = 0x01;
			else GPUdataRet = 0x02;                           // gpu type
			return;
		case 0x08:
		case 0x0F:                                       // some bios addr?
			GPUdataRet = 0xBFC03720;
			return;
		}
		return;
		//--------------------------------------------------//
	}
}


////////////////////////////////////////////////////////////////////////
// helper table to know how much data is used by drawing commands
////////////////////////////////////////////////////////////////////////

const unsigned char primTableCX[256] =
{
	// 00
	0,0,3,0,0,0,0,0,
	// 08
	0,0,0,0,0,0,0,0,
	// 10
	0,0,0,0,0,0,0,0,
	// 18
	0,0,0,0,0,0,0,0,
	// 20
	4,4,4,4,7,7,7,7,
	// 28
	5,5,5,5,9,9,9,9,
	// 30
	6,6,6,6,9,9,9,9,
	// 38
	8,8,8,8,12,12,12,12,
	// 40
	3,3,3,3,0,0,0,0,
	// 48
//    5,5,5,5,6,6,6,6,      //FLINE
	254,254,254,254,254,254,254,254,
	// 50
	4,4,4,4,0,0,0,0,
	// 58
//    7,7,7,7,9,9,9,9,    //    LINEG3    LINEG4
	255,255,255,255,255,255,255,255,
	// 60
	3,3,3,3,4,4,4,4,    //    TILE    SPRT
	// 68
	2,2,2,2,3,3,3,3,    //    TILE1
	// 70
	2,2,2,2,3,3,3,3,
	// 78
	2,2,2,2,3,3,3,3,
	// 80
	4,0,0,0,0,0,0,0,
	// 88
	0,0,0,0,0,0,0,0,
	// 90
	0,0,0,0,0,0,0,0,
	// 98
	0,0,0,0,0,0,0,0,
	// a0
	3,0,0,0,0,0,0,0,
	// a8
	0,0,0,0,0,0,0,0,
	// b0
	0,0,0,0,0,0,0,0,
	// b8
	0,0,0,0,0,0,0,0,
	// c0
	3,0,0,0,0,0,0,0,
	// c8
	0,0,0,0,0,0,0,0,
	// d0
	0,0,0,0,0,0,0,0,
	// d8
	0,0,0,0,0,0,0,0,
	// e0
	0,1,1,1,1,1,1,0,
	// e8
	0,0,0,0,0,0,0,0,
	// f0
	0,0,0,0,0,0,0,0,
	// f8
	0,0,0,0,0,0,0,0
};

////////////////////////////////////////////////////////////////////////
// processes data send to GPU data register
////////////////////////////////////////////////////////////////////////

void CALLBACK GPUwriteDataMem(uint32_t *pMem, int iSize)
{
	unsigned char command;
	uint32_t gdata = 0;
	int i = 0;
	GPUIsBusy;
	GPUIsNotReadyForCommands;

STARTVRAM:

	if (iDataWriteMode == DR_VRAMTRANSFER)
	{
        BOOL bFinished = FALSE;

		// make sure we are in vram
		while (VRAMWrite.ImagePtr >= psxVuw_eom)
			VRAMWrite.ImagePtr -= iGPUHeight * 1024;
		while (VRAMWrite.ImagePtr < psxVuw)
			VRAMWrite.ImagePtr += iGPUHeight * 1024;

		// now do the loop
		while (VRAMWrite.ColsRemaining > 0)
		{
			while (VRAMWrite.RowsRemaining > 0)
			{
				if (i >= iSize) { goto ENDVRAM; }
				i++;

				gdata = *pMem++;

				// Write odd pixel - Wrap from beginning to next index if going past GPU width
				if (VRAMWrite.Width + VRAMWrite.x - VRAMWrite.RowsRemaining >= 1024) {
					*((VRAMWrite.ImagePtr++) - 1024) = (unsigned short)gdata;
				}
				else { *VRAMWrite.ImagePtr++ = (unsigned short)gdata; }
				if (VRAMWrite.ImagePtr >= psxVuw_eom) VRAMWrite.ImagePtr -= iGPUHeight * 1024;// Check if went past framebuffer
				VRAMWrite.RowsRemaining--;

				// Check if end at odd pixel drawn
				if (VRAMWrite.RowsRemaining <= 0)
				{
					VRAMWrite.ColsRemaining--;
					if (VRAMWrite.ColsRemaining <= 0)             // last pixel is odd width
					{
						gdata = (gdata & 0xFFFF) | (((uint32_t)(*VRAMWrite.ImagePtr)) << 16);
						FinishedVRAMWrite();
						goto ENDVRAM;
					}
					VRAMWrite.RowsRemaining = VRAMWrite.Width;
					VRAMWrite.ImagePtr += 1024 - VRAMWrite.Width;
				}

				// Write even pixel - Wrap from beginning to next index if going past GPU width
				if (VRAMWrite.Width + VRAMWrite.x - VRAMWrite.RowsRemaining >= 1024) {
					*((VRAMWrite.ImagePtr++) - 1024) = (unsigned short)(gdata >> 16);
				}
				else *VRAMWrite.ImagePtr++ = (unsigned short)(gdata >> 16);
				if (VRAMWrite.ImagePtr >= psxVuw_eom) VRAMWrite.ImagePtr -= iGPUHeight * 1024;// Check if went past framebuffer
				VRAMWrite.RowsRemaining--;
			}

			VRAMWrite.RowsRemaining = VRAMWrite.Width;
			VRAMWrite.ColsRemaining--;
			VRAMWrite.ImagePtr += 1024 - VRAMWrite.Width;

			bFinished = TRUE;
		}

		FinishedVRAMWrite();

        if (bFinished)
            iDrawnSomething = 1;
	}

ENDVRAM:

	if (iDataWriteMode == DR_NORMAL)
	{
		void(**primFunc)(unsigned char *);
		if (bSkipNextFrame) primFunc = primTableSkip;
		else               primFunc = primTableJ;

		for (; i < iSize;)
		{
			if (iDataWriteMode == DR_VRAMTRANSFER) goto STARTVRAM;

			gdata = *pMem++; i++;

			if (gpuDataC == 0)
			{
				command = (unsigned char)((gdata >> 24) & 0xff);

				if (primTableCX[command])
				{
					gpuDataC = primTableCX[command];
					gpuCommand = command;
					gpuDataM[0] = gdata;
					gpuDataP = 1;
				}
				else continue;
			}
			else
			{
				gpuDataM[gpuDataP] = gdata;
				if (gpuDataC > 128)
				{
					if ((gpuDataC == 254 && gpuDataP >= 3) ||
						(gpuDataC == 255 && gpuDataP >= 4 && !(gpuDataP & 1)))
					{
						if ((gpuDataM[gpuDataP] & 0xF000F000) == 0x50005000)
							gpuDataP = gpuDataC - 1;
					}
				}
				gpuDataP++;
			}

			if (gpuDataP == gpuDataC)
			{
				gpuDataC = gpuDataP = 0;
				for (unsigned int i = 0; i < 4; i++)	//iCB: remove stale vertex data
				{
					vertex[i].x = vertex[i].y = 0.f;
					vertex[i].z = 0.95f;
					vertex[i].w = 1.f;
					vertex[i].PGXP_flag = 0;
				}
				primFunc[gpuCommand]((unsigned char *)gpuDataM);

				if (dwEmuFixes & 0x0001 || dwActFixes & 0x20000)     // hack for emulating "gpu busy" in some games
					iFakePrimBusy = 4;
			}
		}
	}

	GPUdataRet = gdata;

	GPUIsReadyForCommands;
	GPUIsIdle;
}

////////////////////////////////////////////////////////////////////////

void CALLBACK GPUwriteData(uint32_t gdata)
{
	GPUwriteDataMem(&gdata, 1);
}



////////////////////////////////////////////////////////////////////////
// Pete Special: make an 'intelligent' dma chain check (<-Tekken3)
////////////////////////////////////////////////////////////////////////

static __inline BOOL CheckForEndlessLoop(uint32_t laddr)
{
	if (laddr == lUsedAddr[1]) return TRUE;
	if (laddr == lUsedAddr[2]) return TRUE;

	if (laddr < lUsedAddr[0]) lUsedAddr[1] = laddr;
	else                   lUsedAddr[2] = laddr;
	lUsedAddr[0] = laddr;
	return FALSE;
}

////////////////////////////////////////////////////////////////////////
// core gives a dma chain to gpu: same as the gpuwrite interface funcs
////////////////////////////////////////////////////////////////////////

long CALLBACK GPUdmaChain(uint32_t *baseAddrL, uint32_t addr)
{
	uint32_t dmaMem;
	unsigned char * baseAddrB;
	short count; unsigned int DMACommandCounter = 0;

	if (bIsFirstFrame) GLinitialize();

	GPUIsBusy;

	lUsedAddr[0] = lUsedAddr[1] = lUsedAddr[2] = 0xffffff;

	baseAddrB = (unsigned char*)baseAddrL;

	uint32_t depthCount = 0;
	do
	{
		if (iGPUHeight == 512) addr &= 0x1FFFFC;

		if (DMACommandCounter++ > 2000000) break;
		if (CheckForEndlessLoop(addr)) break;

		count = baseAddrB[addr + 3];

		dmaMem = addr + 4;

		if (count > 0)
		{
			PGXP_SetAddress(dmaMem >> 2, &baseAddrL[dmaMem >> 2], count);
			GPUwriteDataMem(&baseAddrL[dmaMem >> 2], count);
		}
		else
			PGXP_SetDepth(depthCount++);

		addr = baseAddrL[addr >> 2] & 0xffffff;
	} while (addr != 0xffffff);

	GPUIsIdle;

	return 0;
}


////////////////////////////////////////////////////////////////////////
// save state funcs
////////////////////////////////////////////////////////////////////////

typedef struct GPUFREEZETAG
{
	uint32_t ulFreezeVersion;      // should be always 1 for now (set by main emu)
	uint32_t ulStatus;             // current gpu status
	uint32_t ulControl[256];       // latest control register values
	unsigned char psxVRam[1024 * 1024 * 2]; // current VRam image (full 2 MB for ZN)
} GPUFreeze_t;

////////////////////////////////////////////////////////////////////////

long CALLBACK GPUfreeze(uint32_t ulGetFreezeData, GPUFreeze_t * pF)
{
	if (ulGetFreezeData == 2)
	{
		int lSlotNum = *((int *)pF);
		if (lSlotNum < 0) return 0;
		if (lSlotNum > 8) return 0;
		lSelectedSlot = lSlotNum + 1;
		return 1;
	}

	if (!pF)                    return 0;
	if (pF->ulFreezeVersion != 1) return 0;

	if (ulGetFreezeData == 1)
	{
		pF->ulStatus = STATUSREG;
		memcpy(pF->ulControl, ulStatusControl, 256 * sizeof(uint32_t));
		memcpy(pF->psxVRam, psxVub, 1024 * iGPUHeight * 2);

		return 1;
	}

	if (ulGetFreezeData != 0) return 0;

	STATUSREG = pF->ulStatus;
	memcpy(ulStatusControl, pF->ulControl, 256 * sizeof(uint32_t));
	memcpy(psxVub, pF->psxVRam, 1024 * iGPUHeight * 2);

	ResetTextureArea(TRUE);

	GPUwriteStatus(ulStatusControl[0]);
	GPUwriteStatus(ulStatusControl[1]);
	GPUwriteStatus(ulStatusControl[2]);
	GPUwriteStatus(ulStatusControl[3]);
	GPUwriteStatus(ulStatusControl[8]);
	GPUwriteStatus(ulStatusControl[6]);
	GPUwriteStatus(ulStatusControl[7]);
	GPUwriteStatus(ulStatusControl[5]);
	GPUwriteStatus(ulStatusControl[4]);

	return 1;
}

////////////////////////////////////////////////////////////////////////
// swap update check (called by psx vsync function)
////////////////////////////////////////////////////////////////////////

BOOL bSwapCheck(void)
{
	static int iPosCheck = 0;
	static PSXPoint_t pO;
	static PSXPoint_t pD;
	static int iDoAgain = 0;

	if (PSXDisplay.DisplayPosition.x == pO.x &&
		PSXDisplay.DisplayPosition.y == pO.y &&
		PSXDisplay.DisplayEnd.x == pD.x &&
		PSXDisplay.DisplayEnd.y == pD.y)
		iPosCheck++;
	else iPosCheck = 0;

	pO = PSXDisplay.DisplayPosition;
	pD = PSXDisplay.DisplayEnd;

	if (iPosCheck <= 4) return FALSE;

	iPosCheck = 4;

	if (PSXDisplay.Interlaced) return FALSE;

	if (bNeedInterlaceUpdate ||
		bNeedRGB24Update ||
		bNeedUploadAfter ||
		bNeedUploadTest ||
		iDoAgain
		)
	{
		iDoAgain = 0;
		if (bNeedUploadAfter)
			iDoAgain = 1;
		if (bNeedUploadTest && PSXDisplay.InterlacedTest)
			iDoAgain = 1;

		bDisplayNotSet = TRUE;
		updateDisplay();

		PreviousPSXDisplay.DisplayPosition.x = PSXDisplay.DisplayPosition.x;
		PreviousPSXDisplay.DisplayPosition.y = PSXDisplay.DisplayPosition.y;
		PreviousPSXDisplay.DisplayEnd.x = PSXDisplay.DisplayEnd.x;
		PreviousPSXDisplay.DisplayEnd.y = PSXDisplay.DisplayEnd.y;
		pO = PSXDisplay.DisplayPosition;
		pD = PSXDisplay.DisplayEnd;

		return TRUE;
	}

	return FALSE;
}

////////////////////////////////////////////////////////////////////////
// read registry
////////////////////////////////////////////////////////////////////////

void ReadConfig(void)                                  // read all config vals
{
	HKEY myKey;
	DWORD temp;
	DWORD type;
	DWORD size;

	// pre-init all relevant globals

	//iResX = 800; iResY = 600;
	iColDepth = 32;
	bWindowMode = FALSE;
	bFullVRam = FALSE;
	iFilterType = 0;
	bAdvancedBlend = FALSE;
	bDrawDither = FALSE;
	bUseLines = FALSE;
	bUseFrameLimit = TRUE;
	bUseFrameSkip = FALSE;
	iFrameLimit = 2;
	fFrameRate = 200.0f;
	iOffscreenDrawing = 1;
	bOpaquePass = TRUE;
	bUseAntiAlias = FALSE;
	iTexQuality = 0;
	iWinSize = MAKELONG(400, 300);
	iUseMask = 0;
	iZBufferDepth = 0;
	bUseFastMdec = FALSE;
	bUse15bitMdec = FALSE;
	dwCfgFixes = 0;
	bUseFixes = FALSE;
	bGteAccuracy = FALSE;
	iUseExts = TRUE;
	iUseScanLines = 0;
	iFrameTexType = 0;
	iFrameReadType = 0;
	iLineHackMode = 0;
	iShowFPS = 0;
	bKeepRatio = FALSE;
	bForceRatio43 = FALSE;
	iScanBlend = 0;
	iVRamSize = 0;
	iTexGarbageCollection = 1;
	iBlurBuffer = 0;
	iHiResTextures = 0;
	iNoScreenSaver = 1;
	iForceVSync = -1;

	lstrcpy(szGPUKeys, szKeyDefaults);

	// registry key: still "psemu pro/pete tnt" for compatibility reasons

//	if (RegOpenKeyEx(HKEY_CURRENT_USER, "Software\\Vision Thing\\PSEmu Pro\\GPU\\PeteTNT", 0, KEY_ALL_ACCESS, &myKey) == ERROR_SUCCESS)
	{ 
			//iResX = 960; 
			//iResY = 720; 
			iWinSize = MAKELONG(iResX, iResY); 
			iFilterType = 6; 
			bDrawDither = FALSE; 
			bUseLines = FALSE; 
			bWindowMode = TRUE; 
			iColDepth = 32; 
			iForceVSync = 0; 
			bUseFrameLimit = TRUE; 
			bUseFrameSkip = FALSE; 
			iFrameLimit = 2; 

		if (!iFrameLimit) { bUseFrameLimit = TRUE; bUseFrameSkip = FALSE; iFrameLimit = 2; }

		// try to get the float framerate... if none: take int framerate
		fFrameRate = 0.0f;
		size = 4;
		temp = 0x43480000; 
			fFrameRate = *((float *)(&temp));

		if (fFrameRate == 0.0f)
		{
			fFrameRate = 200.0f; 

				fFrameRate = (float)temp;
		}
		 
			bAdvancedBlend = FALSE; 
		iOffscreenDrawing = -1; 
			iOffscreenDrawing = 3; 
			iOffscreenDrawing = 1 * 2; 
			bOpaquePass = TRUE; 
			iVRamSize = 128;
		//   size = 4;
		//   if(RegQueryValueEx(myKey,"UseAntiAlias",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
		//    bUseAntiAlias=(BOOL)temp; 
			iTexQuality = 4; 
			dwCfgFixes = 0; 
			bUseFixes = FALSE; 
			bGteAccuracy = TRUE; 
			iUseExts = TRUE; 
			iUseMask = 1; 
			bUseFastMdec = FALSE; 
			bUse15bitMdec = FALSE; 
			iUseScanLines = FALSE; 
			iShowFPS = FALSE; 
			bKeepRatio = FALSE; 
			bForceRatio43 = FALSE; 
			iScanBlend = FALSE; 
			iFrameTexType = 2; 
			iFrameReadType = 0; 
			iLineHackMode = 1; 
			iBlurBuffer = 0; 
			iHiResTextures = 1; 
			iNoScreenSaver = 0;

	}
	if (!iColDepth)        iColDepth = 32;                   // adjust color info
	if (iUseMask)          iZBufferDepth = 16;               // set zbuffer depth 
	else                  iZBufferDepth = 0;
	if (bUseFixes)         dwActFixes = dwCfgFixes;          // init game fix global
	if (iFrameReadType == 4) bFullVRam = TRUE;                 // set fullvram render flag (soft gpu)
}

////////////////////////////////////////////////////////////////////////
// sets all kind of act fixes
////////////////////////////////////////////////////////////////////////

void SetFixes(void)
{
	ReInitFrameCap();

	if (dwActFixes & 0x2000)
		dispWidths[4] = 384;
	else dispWidths[4] = 368;
}

////////////////////////////////////////////////////////////////////////
// vram read check ex (reading from card's back/frontbuffer if needed...
// slow!)
////////////////////////////////////////////////////////////////////////

void CheckVRamReadEx(int x, int y, int dx, int dy)
{
	unsigned short sArea;
	int ux, uy, udx, udy, wx, wy;
	unsigned short * p1, *p2;
	float XS, YS;
	unsigned char * ps;
	unsigned char * px;
	unsigned short s, sx;

	if (STATUSREG&GPUSTATUS_RGB24) return;

	if (((dx > PSXDisplay.DisplayPosition.x) &&
		(x < PSXDisplay.DisplayEnd.x) &&
		(dy > PSXDisplay.DisplayPosition.y) &&
		(y < PSXDisplay.DisplayEnd.y)))
		sArea = 0;
	else
		if ((!(PSXDisplay.InterlacedTest) &&
			(dx > PreviousPSXDisplay.DisplayPosition.x) &&
			(x < PreviousPSXDisplay.DisplayEnd.x) &&
			(dy > PreviousPSXDisplay.DisplayPosition.y) &&
			(y < PreviousPSXDisplay.DisplayEnd.y)))
			sArea = 1;
		else
		{
			return;
		}

	//////////////

	if (iRenderFVR)
	{
		bFullVRam = TRUE; iRenderFVR = 2; return;
	}
	bFullVRam = TRUE; iRenderFVR = 2;

	//////////////

	p2 = 0;

	if (sArea == 0)
	{
		ux = PSXDisplay.DisplayPosition.x;
		uy = PSXDisplay.DisplayPosition.y;
		udx = PSXDisplay.DisplayEnd.x - ux;
		udy = PSXDisplay.DisplayEnd.y - uy;
		if ((PreviousPSXDisplay.DisplayEnd.x -
			PreviousPSXDisplay.DisplayPosition.x) == udx &&
			(PreviousPSXDisplay.DisplayEnd.y -
				PreviousPSXDisplay.DisplayPosition.y) == udy)
			p2 = (psxVuw + (1024 * PreviousPSXDisplay.DisplayPosition.y) +
				PreviousPSXDisplay.DisplayPosition.x);
	}
	else
	{
		ux = PreviousPSXDisplay.DisplayPosition.x;
		uy = PreviousPSXDisplay.DisplayPosition.y;
		udx = PreviousPSXDisplay.DisplayEnd.x - ux;
		udy = PreviousPSXDisplay.DisplayEnd.y - uy;
		if ((PSXDisplay.DisplayEnd.x -
			PSXDisplay.DisplayPosition.x) == udx &&
			(PSXDisplay.DisplayEnd.y -
				PSXDisplay.DisplayPosition.y) == udy)
			p2 = (psxVuw + (1024 * PSXDisplay.DisplayPosition.y) +
				PSXDisplay.DisplayPosition.x);
	}

	p1 = (psxVuw + (1024 * uy) + ux);
	if (p1 == p2) p2 = 0;

	x = 0; y = 0;
	wx = dx = udx; wy = dy = udy;

	if (udx <= 0) return;
	if (udy <= 0) return;
	if (dx <= 0)  return;
	if (dy <= 0)  return;
	if (wx <= 0)  return;
	if (wy <= 0)  return;

	XS = (float)rRatioRect.right / (float)wx;
	YS = (float)rRatioRect.bottom / (float)wy;

	dx = (int)((float)(dx)*XS);
	dy = (int)((float)(dy)*YS);

	if (dx > iResX) dx = iResX;
	if (dy > iResY) dy = iResY;

	if (dx <= 0) return;
	if (dy <= 0) return;

	// ogl y adjust
	y = iResY - y - dy;

	x += rRatioRect.left;
	y -= rRatioRect.top;

	if (y < 0) y = 0; if ((y + dy) > iResY) dy = iResY - y;

	if (!pGfxCardScreen)
	{
		//glPixelStorei(GL_PACK_ALIGNMENT, 1);
		pGfxCardScreen = (unsigned char *)malloc(iResX*iResY * 4);
	}

	ps = pGfxCardScreen;

	if (!sArea) glReadBuffer(GL_FRONT);

	glReadPixels(x, y, dx, dy, GL_RGB, GL_UNSIGNED_BYTE, ps);

	if (!sArea) glReadBuffer(GL_BACK);

	s = 0;

	XS = (float)dx / (float)(udx);
	YS = (float)dy / (float)(udy + 1);

	for (y = udy; y > 0; y--)
	{
		for (x = 0; x < udx; x++)
		{
			if (p1 >= psxVuw && p1 < psxVuw_eom)
			{
				px = ps + (3 * ((int)((float)x * XS)) +
					(3 * dx)*((int)((float)y*YS)));
				sx = (*px) >> 3; px++;
				s = sx;
				sx = (*px) >> 3; px++;
				s |= sx << 5;
				sx = (*px) >> 3;
				s |= sx << 10;
				s &= ~0x8000;
				*p1 = s;
			}
			if (p2 >= psxVuw && p2 < psxVuw_eom) *p2 = s;

			p1++;
			if (p2) p2++;
		}

		p1 += 1024 - udx;
		if (p2) p2 += 1024 - udx;
	}
}

////////////////////////////////////////////////////////////////////////
// vram read check (reading from card's back/frontbuffer if needed... 
// slow!)
////////////////////////////////////////////////////////////////////////

void CheckVRamRead(int x, int y, int dx, int dy, BOOL bFront)
{
	unsigned short sArea; unsigned short * p;
	int ux, uy, udx, udy, wx, wy; float XS, YS;
	unsigned char * ps, *px;
	unsigned short s = 0, sx;

	if (STATUSREG&GPUSTATUS_RGB24) return;

	if (((dx > PSXDisplay.DisplayPosition.x) &&
		(x < PSXDisplay.DisplayEnd.x) &&
		(dy > PSXDisplay.DisplayPosition.y) &&
		(y < PSXDisplay.DisplayEnd.y)))
		sArea = 0;
	else
		if ((!(PSXDisplay.InterlacedTest) &&
			(dx > PreviousPSXDisplay.DisplayPosition.x) &&
			(x < PreviousPSXDisplay.DisplayEnd.x) &&
			(dy > PreviousPSXDisplay.DisplayPosition.y) &&
			(y < PreviousPSXDisplay.DisplayEnd.y)))
			sArea = 1;
		else
		{
			return;
		}

	if (dwActFixes & 0x40)
	{
		if (iRenderFVR)
		{
			bFullVRam = TRUE; iRenderFVR = 2; return;
		}
		bFullVRam = TRUE; iRenderFVR = 2;
	}

	ux = x; uy = y; udx = dx; udy = dy;

	if (sArea == 0)
	{
		x -= PSXDisplay.DisplayPosition.x;
		dx -= PSXDisplay.DisplayPosition.x;
		y -= PSXDisplay.DisplayPosition.y;
		dy -= PSXDisplay.DisplayPosition.y;
		wx = PSXDisplay.DisplayEnd.x - PSXDisplay.DisplayPosition.x;
		wy = PSXDisplay.DisplayEnd.y - PSXDisplay.DisplayPosition.y;
	}
	else
	{
		x -= PreviousPSXDisplay.DisplayPosition.x;
		dx -= PreviousPSXDisplay.DisplayPosition.x;
		y -= PreviousPSXDisplay.DisplayPosition.y;
		dy -= PreviousPSXDisplay.DisplayPosition.y;
		wx = PreviousPSXDisplay.DisplayEnd.x - PreviousPSXDisplay.DisplayPosition.x;
		wy = PreviousPSXDisplay.DisplayEnd.y - PreviousPSXDisplay.DisplayPosition.y;
	}
	if (x < 0) { ux -= x; x = 0; }
	if (y < 0) { uy -= y; y = 0; }
	if (dx > wx) { udx -= (dx - wx); dx = wx; }
	if (dy > wy) { udy -= (dy - wy); dy = wy; }
	udx -= ux;
	udy -= uy;

	p = (psxVuw + (1024 * uy) + ux);

	if (udx <= 0) return;
	if (udy <= 0) return;
	if (dx <= 0)  return;
	if (dy <= 0)  return;
	if (wx <= 0)  return;
	if (wy <= 0)  return;

	XS = (float)rRatioRect.right / (float)wx;
	YS = (float)rRatioRect.bottom / (float)wy;

	dx = (int)((float)(dx)*XS);
	dy = (int)((float)(dy)*YS);
	x = (int)((float)x*XS);
	y = (int)((float)y*YS);

	dx -= x;
	dy -= y;

	if (dx > iResX) dx = iResX;
	if (dy > iResY) dy = iResY;

	if (dx <= 0) return;
	if (dy <= 0) return;

	// ogl y adjust
	y = iResY - y - dy;

	x += rRatioRect.left;
	y -= rRatioRect.top;

	if (y < 0) y = 0; if ((y + dy) > iResY) dy = iResY - y;

	if (!pGfxCardScreen)
	{
		//glPixelStorei(GL_PACK_ALIGNMENT, 1);
		pGfxCardScreen = (unsigned char *)malloc(iResX*iResY * 4);
	}

	ps = pGfxCardScreen;

	if (bFront) glReadBuffer(GL_FRONT);

	glReadPixels(x, y, dx, dy, GL_RGB, GL_UNSIGNED_BYTE, ps);

	if (bFront) glReadBuffer(GL_BACK);

	XS = (float)dx / (float)(udx);
	YS = (float)dy / (float)(udy + 1);

	for (y = udy; y > 0; y--)
	{
		for (x = 0; x < udx; x++)
		{
			if (p >= psxVuw && p < psxVuw_eom)
			{
				px = ps + (3 * ((int)((float)x * XS)) +
					(3 * dx)*((int)((float)y*YS)));
				sx = (*px) >> 3; px++;
				s = sx;
				sx = (*px) >> 3; px++;
				s |= sx << 5;
				sx = (*px) >> 3;
				s |= sx << 10;
				s &= ~0x8000;
				*p = s;
			}
			p++;
		}
		p += 1024 - udx;
	}
}

////////////////////////////////////////////////////////////////////////
// paint it black: simple func to clean up optical border garbage
////////////////////////////////////////////////////////////////////////

void PaintBlackBorders(void)
{
	short s;

	glDisableStub(GL_SCISSOR_TEST);
	if (bTexEnabled) { glDisableStub(GL_TEXTURE_2D); bTexEnabled = FALSE; }
	if (bOldSmoothShaded) { bOldSmoothShaded = FALSE; }
	if (bBlendEnable) { glDisableStub(GL_BLEND); bBlendEnable = FALSE; }
	glDisableStub(GL_ALPHA_TEST);
	
	vertex[0].c.lcol = 0xff000000;
	SETCOL(vertex[0]);

	if (PreviousPSXDisplay.Range.x0)
	{
		s = PreviousPSXDisplay.Range.x0 + 1;
		//glVertex3f(0, 0, 0.99996f);
		//glVertex3f(0, PSXDisplay.DisplayMode.y, 0.99996f);
		//glVertex3f(s, PSXDisplay.DisplayMode.y, 0.99996f);
		//glVertex3f(s, 0, 0.99996f);




		g_border_vertexes[0].x = 0.0f;
		g_border_vertexes[0].y = 0.0f;
		g_border_vertexes[0].z = 0.99996f;

		g_border_vertexes[1].x = 0.0f;
		g_border_vertexes[1].y = PSXDisplay.DisplayMode.y;
		g_border_vertexes[1].z = 0.99996f;

		g_border_vertexes[3].x = s;
		g_border_vertexes[3].y = PSXDisplay.DisplayMode.y;
		g_border_vertexes[3].z = 0.99996f;

		g_border_vertexes[2].x = s;
		g_border_vertexes[2].y = 0.0f;
		g_border_vertexes[2].z = 0.99996f;


		PRIMDirectXdrawBorder();

		s += PreviousPSXDisplay.Range.x1 - 2;

		//glVertex3f(s, 0, 0.99996f);
		//glVertex3f(s, PSXDisplay.DisplayMode.y, 0.99996f);
		//glVertex3f(PSXDisplay.DisplayMode.x, PSXDisplay.DisplayMode.y, 0.99996f);
		//glVertex3f(PSXDisplay.DisplayMode.x, 0, 0.99996f);




		g_border_vertexes[0].x = s;
		g_border_vertexes[0].y = 0.0f;
		g_border_vertexes[0].z = 0.99996f;

		g_border_vertexes[1].x = s;
		g_border_vertexes[1].y = PSXDisplay.DisplayMode.y;
		g_border_vertexes[1].z = 0.99996f;

		g_border_vertexes[3].x = PSXDisplay.DisplayMode.x;
		g_border_vertexes[3].y = PSXDisplay.DisplayMode.y;
		g_border_vertexes[3].z = 0.99996f;

		g_border_vertexes[2].x = PSXDisplay.DisplayMode.x;
		g_border_vertexes[2].y = 0.0f;
		g_border_vertexes[2].z = 0.99996f;

		PRIMDirectXdrawBorder();
	}

	if (PreviousPSXDisplay.Range.y0)
	{
		s = PreviousPSXDisplay.Range.y0 + 1;
		//glVertex3f(0, 0, 0.99996f);
		//glVertex3f(0, s, 0.99996f);
		//glVertex3f(PSXDisplay.DisplayMode.x, s, 0.99996f);
		//glVertex3f(PSXDisplay.DisplayMode.x, 0, 0.99996f);




		g_border_vertexes[0].x = 0.0f;
		g_border_vertexes[0].y = 0.0f;
		g_border_vertexes[0].z = 0.99996f;

		g_border_vertexes[1].x = 0.0f;
		g_border_vertexes[1].y = s;
		g_border_vertexes[1].z = 0.99996f;

		g_border_vertexes[3].x = PSXDisplay.DisplayMode.x;
		g_border_vertexes[3].y = s;
		g_border_vertexes[3].z = 0.99996f;

		g_border_vertexes[2].x = PSXDisplay.DisplayMode.x;
		g_border_vertexes[2].y = 0.0f;
		g_border_vertexes[2].z = 0.99996f;

		PRIMDirectXdrawBorder();
	}
	
	glEnableStub(GL_ALPHA_TEST);
	glEnableStub(GL_SCISSOR_TEST);
}

////////////////////////////////////////////////////////////////////////
// helper to draw scanlines
////////////////////////////////////////////////////////////////////////

static __inline void XPRIMdrawTexturedQuad(OGLVertex* vertex1, OGLVertex* vertex2,
	OGLVertex* vertex3, OGLVertex* vertex4)
{

	//glBegin(GL_QUAD_STRIP);
	//glTexCoord2fv(&vertex1->sow);
	//PGXP_glVertexfv(&vertex1->x);

	//glTexCoord2fv(&vertex2->sow);
	//PGXP_glVertexfv(&vertex2->x);

	//glTexCoord2fv(&vertex4->sow);
	//PGXP_glVertexfv(&vertex4->x);

	//glTexCoord2fv(&vertex3->sow);
	//PGXP_glVertexfv(&vertex3->x);
	//glEnd();
}

////////////////////////////////////////////////////////////////////////
// scanlines
////////////////////////////////////////////////////////////////////////

void SetScanLines(void)
{
	//glOrtho(0, iResX, iResY, 0, -1, 1);

	setOrtho(0, iResX, iResY, 0, -1, 1);

	if (bKeepRatio)
		glViewport(0, 0, iResX, iResY);

	glDisableStub(GL_SCISSOR_TEST);
	glDisableStub(GL_ALPHA_TEST);
	if (bOldSmoothShaded) {bOldSmoothShaded = FALSE; }

	if (iScanBlend < 0)                                      // special texture mask scanline mode
	{
		if (!bTexEnabled) { glEnableStub(GL_TEXTURE_2D); bTexEnabled = TRUE; }
		gTexName = gTexScanName;
		glBindTextureStub(GL_TEXTURE_2D, gTexName);
		if (bGLBlend) glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
		if (!bBlendEnable) { glEnableStub(GL_BLEND); bBlendEnable = TRUE; }
		SetScanTexTrans();

		vertex[0].x = 0;
		vertex[0].y = iResY;
		vertex[0].z = 0.99996f;

		vertex[1].x = iResX;
		vertex[1].y = iResY;
		vertex[1].z = 0.99996f;

		vertex[2].x = iResX;
		vertex[2].y = 0;
		vertex[2].z = 0.99996f;

		vertex[3].x = 0;
		vertex[3].y = 0;
		vertex[3].z = 0.99996f;

		vertex[0].sow = 0;
		vertex[0].tow = 0;
		vertex[1].sow = (float)iResX / 4.0f;
		vertex[1].tow = 0;
		vertex[2].sow = vertex[1].sow;
		vertex[2].tow = (float)iResY / 4.0f;
		vertex[3].sow = 0;
		vertex[3].tow = vertex[2].tow;

		vertex[0].c.lcol = 0xffffffff;
		SETCOL(vertex[0]);

		XPRIMdrawTexturedQuad(&vertex[0], &vertex[1], &vertex[2], &vertex[3]);

		if (bGLBlend) glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, COMBINE_EXT);
	}
	else                                                  // typical line mode
	{
		if (bTexEnabled) { glDisableStub(GL_TEXTURE_2D); bTexEnabled = FALSE; }

		if (iScanBlend == 0)
		{
			if (bBlendEnable) { glDisableStub(GL_BLEND); bBlendEnable = FALSE; }
			vertex[0].c.lcol = 0xff000000;
		}
		else
		{
			if (!bBlendEnable) { glEnableStub(GL_BLEND); bBlendEnable = TRUE; }
			SetScanTrans();
			vertex[0].c.lcol = iScanBlend << 24;
		}

		SETCOL(vertex[0]);
	}
	   
	setOrtho(0, PSXDisplay.DisplayMode.x,
		PSXDisplay.DisplayMode.y, 0, -1, 1);

	//PGXP_SetMatrix(0, PSXDisplay.DisplayMode.x, PSXDisplay.DisplayMode.y, 0, -1, 1);

	if (bKeepRatio)
		glViewport(rRatioRect.left,
			iResY - (rRatioRect.top + rRatioRect.bottom),
			rRatioRect.right,
			rRatioRect.bottom);                         // init viewport

	glEnableStub(GL_ALPHA_TEST);
	glEnableStub(GL_SCISSOR_TEST);
}



////////////////////////////////////////////////////////////////////////
// Update display (swap buffers)... called in interlaced mode on 
// every emulated vsync, otherwise whenever the displayed screen region
// has been changed
////////////////////////////////////////////////////////////////////////

void updateDisplay(void)                               // UPDATE DISPLAY
{
	BOOL bBlur = FALSE;

	bFakeFrontBuffer = FALSE;
	bRenderFrontBuffer = FALSE;

	if (iRenderFVR)                                        // frame buffer read fix mode still active?
	{
		iRenderFVR--;                                       // -> if some frames in a row without read access: turn off mode
		if (!iRenderFVR) bFullVRam = FALSE;
	}

	if (iLastRGB24 && iLastRGB24 != PSXDisplay.RGB24 + 1)      // (mdec) garbage check
	{
		iSkipTwo = 2;                                         // -> skip two frames to avoid garbage if color mode changes
	}
	iLastRGB24 = 0;

	if (PSXDisplay.RGB24)// && !bNeedUploadAfter)          // (mdec) upload wanted?
	{
		PrepareFullScreenUpload(-1);
		UploadScreen(PSXDisplay.Interlaced);                // -> upload whole screen from psx vram
		bNeedUploadTest = FALSE;
		bNeedInterlaceUpdate = FALSE;
		bNeedUploadAfter = FALSE;
		bNeedRGB24Update = FALSE;
	}
	else
		if (bNeedInterlaceUpdate)                              // smaller upload?
		{
			bNeedInterlaceUpdate = FALSE;
			xrUploadArea = xrUploadAreaIL;                        // -> upload this rect
			UploadScreen(TRUE);
		}

	if (dwActFixes & 512) bCheckFF9G4(NULL);                 // special game fix for FF9 

	if (PreviousPSXDisplay.Range.x0 ||                      // paint black borders around display area, if needed
		PreviousPSXDisplay.Range.y0)
		PaintBlackBorders();

	if (PSXDisplay.Disabled)                               // display disabled?
	{
		// moved here
		glDisableStub(GL_SCISSOR_TEST);                      
		setClearColor(0, 0, 0, 128); // -> clear whole backbuffer
		clearBuffers(uiBufferBits);
		glEnableStub(GL_SCISSOR_TEST);
		gl_z = 0.0f;
		bDisplayNotSet = TRUE;
	}

	if (iSkipTwo)                                          // we are in skipping mood?
	{
		iSkipTwo--;
		iDrawnSomething = 0;                                  // -> simply lie about something drawn
	}

	if (iBlurBuffer && !bSkipNextFrame)                    // "blur display" activated?
	{
		//BlurBackBuffer(); bBlur = TRUE;
	}                       // -> blur it

	if (iUseScanLines) SetScanLines();                     // "scan lines" activated? do it

	//if (usCursorActive) ShowGunCursor();                   // "gun cursor" wanted? show 'em

	if (dwActFixes & 128)                                    // special FPS limitation mode?
	{
		if (bUseFrameLimit) PCFrameCap();                    // -> ok, do it
		if (bUseFrameSkip)
			PCcalcfps();
	}

	//if (gTexPicName) DisplayPic();                         // some gpu info picture active? display it

	//if (bSnapShot) DoSnapShot();                           // snapshot key pressed? cheeeese :)

	//if (ulKeybits&KEY_SHOWFPS)                             // wanna see FPS?
	//{
	//	//sprintf(szDispBuf, "%06.1f", fps_cur);
	//	DisplayText();                                      // -> show it
	//}

	//----------------------------------------------------//
	// main buffer swapping (well, or skip it)

	if (bUseFrameSkip)                                     // frame skipping active ?
	{
		if (!bSkipNextFrame)
		{
			if (iDrawnSomething)
#ifdef _WINDOWS
				//SwapBuffers(wglGetCurrentDC());                  // -> to skip or not to skip
			swapBuffers();
#elif defined(_MACGL)
				DoBufferSwap();
#else
				glXSwapBuffers(display, window);
#endif
		}
		if (dwActFixes & 0x180)                                // -> special old frame skipping: skip max one in a row
		{
			if ((fps_skip < fFrameRateHz) && !(bSkipNextFrame))
			{
				bSkipNextFrame = TRUE; fps_skip = fFrameRateHz;
			}
			else bSkipNextFrame = FALSE;
		}
		else FrameSkip();
	}
	else                                                  // no skip ?
	{
		if (iDrawnSomething)
#ifdef _WINDOWS
			//SwapBuffers(wglGetCurrentDC());                    // -> swap
		swapBuffers();
#elif defined(_MACGL)
			DoBufferSwap();
#else
			glXSwapBuffers(display, window);
#endif
	}

	iDrawnSomething = 0;

	//----------------------------------------------------//

	if (lClearOnSwap)                                      // clear buffer after swap?
	{
		GLclampf g, b, r;

		if (bDisplayNotSet)                                  // -> set new vals
			SetOGLDisplaySettings(1);

		g = ((GLclampf)GREEN(lClearOnSwapColor)) / 255.0f;      // -> get col
		b = ((GLclampf)BLUE(lClearOnSwapColor)) / 255.0f;
		r = ((GLclampf)RED(lClearOnSwapColor)) / 255.0f;

		glDisableStub(GL_SCISSOR_TEST);                    
		setClearColor(r, g, b, 128);							// -> clear 
		//glClear(uiBufferBits);
		clearBuffers(uiBufferBits);
		glEnableStub(GL_SCISSOR_TEST);
		lClearOnSwap = 0;                                     // -> done
	}
	else
	{
		//if (bBlur) UnBlurBackBuffer();                       // unblur buff, if blurred before

		if (iZBufferDepth)                                   // clear zbuffer as well (if activated)
		{
			glDisableStub(GL_SCISSOR_TEST);
			clearBuffers(GL_DEPTH_BUFFER_BIT);
			glEnableStub(GL_SCISSOR_TEST);
		}
	}
	gl_z = 0.0f;

	//----------------------------------------------------//
	// additional uploads immediatly after swapping

	if (bNeedUploadAfter)                                  // upload wanted?
	{
		bNeedUploadAfter = FALSE;
		bNeedUploadTest = FALSE;
		UploadScreen(-1);                                   // -> upload
	}

	if (bNeedUploadTest)
	{
		bNeedUploadTest = FALSE;
		if (PSXDisplay.InterlacedTest &&
			//iOffscreenDrawing>2 &&
			PreviousPSXDisplay.DisplayPosition.x == PSXDisplay.DisplayPosition.x &&
			PreviousPSXDisplay.DisplayEnd.x == PSXDisplay.DisplayEnd.x &&
			PreviousPSXDisplay.DisplayPosition.y == PSXDisplay.DisplayPosition.y &&
			PreviousPSXDisplay.DisplayEnd.y == PSXDisplay.DisplayEnd.y)
		{
			PrepareFullScreenUpload(TRUE);
			UploadScreen(TRUE);
		}
	}

	//----------------------------------------------------//
	// rumbling (main emu pad effect)

	//if (iRumbleTime)                                       // shake screen by modifying view port
	//{
	//	int i1 = 0, i2 = 0, i3 = 0, i4 = 0;

	//	iRumbleTime--;
	//	if (iRumbleTime)
	//	{
	//		i1 = ((rand()*iRumbleVal) / RAND_MAX) - (iRumbleVal / 2);
	//		i2 = ((rand()*iRumbleVal) / RAND_MAX) - (iRumbleVal / 2);
	//		i3 = ((rand()*iRumbleVal) / RAND_MAX) - (iRumbleVal / 2);
	//		i4 = ((rand()*iRumbleVal) / RAND_MAX) - (iRumbleVal / 2);
	//	}

	//	glViewport(rRatioRect.left + i1,
	//		iResY - (rRatioRect.top + rRatioRect.bottom) + i2,
	//		rRatioRect.right + i3,
	//		rRatioRect.bottom + i4);
	//}

}

////////////////////////////////////////////////////////////////////////
// check if update needed
////////////////////////////////////////////////////////////////////////

void ChangeDispOffsetsX(void)                          // CENTER X
{
	int lx, l; short sO;

	if (!PSXDisplay.Range.x1) return;                      // some range given?

	l = PSXDisplay.DisplayMode.x;

	l *= (int)PSXDisplay.Range.x1;                          // some funky calculation
	l /= 2560; lx = l; l &= 0xfffffff8;

	if (l == PreviousPSXDisplay.Range.x1) return;            // some change?

	sO = PreviousPSXDisplay.Range.x0;                       // store old

	if (lx >= PSXDisplay.DisplayMode.x)                      // range bigger?
	{
		PreviousPSXDisplay.Range.x1 =                        // -> take display width
			PSXDisplay.DisplayMode.x;
		PreviousPSXDisplay.Range.x0 = 0;                      // -> start pos is 0
	}
	else                                                  // range smaller? center it
	{
		PreviousPSXDisplay.Range.x1 = l;                      // -> store width (8 pixel aligned)
		PreviousPSXDisplay.Range.x0 =                       // -> calc start pos
			(PSXDisplay.Range.x0 - 500) / 8;
		if (PreviousPSXDisplay.Range.x0 < 0)                   // -> we don't support neg. values yet
			PreviousPSXDisplay.Range.x0 = 0;

		if ((PreviousPSXDisplay.Range.x0 + lx) >                // -> uhuu... that's too much
			PSXDisplay.DisplayMode.x)
		{
			PreviousPSXDisplay.Range.x0 =                      // -> adjust start
				PSXDisplay.DisplayMode.x - lx;
			PreviousPSXDisplay.Range.x1 += lx - l;                // -> adjust width
		}
	}

	if (sO != PreviousPSXDisplay.Range.x0)                   // something changed?
	{
		bDisplayNotSet = TRUE;                                // -> recalc display stuff
	}
}

////////////////////////////////////////////////////////////////////////

void ChangeDispOffsetsY(void)                          // CENTER Y
{
	int iT; short sO;                                      // store previous y size

	if (PSXDisplay.PAL) iT = 48; else iT = 28;                 // different offsets on PAL/NTSC

	if (PSXDisplay.Range.y0 >= iT)                           // crossed the security line? :)
	{
		PreviousPSXDisplay.Range.y1 =                        // -> store width
			PSXDisplay.DisplayModeNew.y;

		sO = (PSXDisplay.Range.y0 - iT - 4)*PSXDisplay.Double;    // -> calc offset
		if (sO < 0) sO = 0;

		PSXDisplay.DisplayModeNew.y += sO;                    // -> add offset to y size, too
	}
	else sO = 0;                                            // else no offset

	if (sO != PreviousPSXDisplay.Range.y0)                   // something changed?
	{
		PreviousPSXDisplay.Range.y0 = sO;
		bDisplayNotSet = TRUE;                                // -> recalc display stuff
	}
}

////////////////////////////////////////////////////////////////////////
// Aspect ratio of ogl screen: simply adjusting ogl view port
////////////////////////////////////////////////////////////////////////

void SetAspectRatio(void)
{
	float xs, ys, s, resx, resy; RECT r;

	if (!PSXDisplay.DisplayModeNew.x) return;
	if (!PSXDisplay.DisplayModeNew.y) return;

	resx = bForceRatio43 ? 640.0f : (float)PSXDisplay.DisplayModeNew.x;
	resy = bForceRatio43 ? 480.0f : (float)PSXDisplay.DisplayModeNew.y;

	xs = (float)iResX / resx;
	ys = (float)iResY / resy;

	s = min(xs, ys);
	r.right = (int)(resx*s);
	r.bottom = (int)(resy*s);
	if (r.right > iResX) r.right = iResX;
	if (r.bottom > iResY) r.bottom = iResY;
	if (r.right < 1)     r.right = 1;
	if (r.bottom < 1)     r.bottom = 1;

	r.left = (iResX - r.right) / 2;
	r.top = (iResY - r.bottom) / 2;

	if (r.bottom < rRatioRect.bottom ||
		r.right < rRatioRect.right)
	{
		RECT rC;
		setClearColor(0, 0, 0, 128);

		if (r.right < rRatioRect.right)
		{
			rC.left = 0;
			rC.top = 0;
			rC.right = r.left;
			rC.bottom = iResY;
			glScissorStub(rC.left, rC.top, rC.right, rC.bottom);			
			clearBuffers(uiBufferBits);
			rC.left = iResX - rC.right;
			glScissorStub(rC.left, rC.top, rC.right, rC.bottom);
			clearBuffers(uiBufferBits);
		}

		if (r.bottom < rRatioRect.bottom)
		{
			rC.left = 0;
			rC.top = 0;
			rC.right = iResX;
			rC.bottom = r.top;
			glScissorStub(rC.left, rC.top, rC.right, rC.bottom);
			clearBuffers(uiBufferBits);
			rC.top = iResY - rC.bottom;
			glScissorStub(rC.left, rC.top, rC.right, rC.bottom);
			clearBuffers(uiBufferBits);
		}

		bSetClip = TRUE;
		bDisplayNotSet = TRUE;
	}

	rRatioRect = r;


	glViewport(rRatioRect.left,
		iResY - (rRatioRect.top + rRatioRect.bottom),
		rRatioRect.right,
		rRatioRect.bottom);                         // init viewport
}

////////////////////////////////////////////////////////////////////////
// big ass check, if an ogl swap buffer is needed
////////////////////////////////////////////////////////////////////////

void updateDisplayIfChanged(void)
{
	BOOL bUp;

	if ((PSXDisplay.DisplayMode.y == PSXDisplay.DisplayModeNew.y) &&
		(PSXDisplay.DisplayMode.x == PSXDisplay.DisplayModeNew.x))
	{
		if ((PSXDisplay.RGB24 == PSXDisplay.RGB24New) &&
			(PSXDisplay.Interlaced == PSXDisplay.InterlacedNew))
			return;                                          // nothing has changed? fine, no swap buffer needed
	}
	else                                                  // some res change?
	{
		//glLoadIdentity();
		//glOrtho(0, PSXDisplay.DisplayModeNew.x,              // -> new psx resolution
		//	PSXDisplay.DisplayModeNew.y, 0, -1, 1);
		setOrtho(0, PSXDisplay.DisplayModeNew.x,              // -> new psx resolution
			PSXDisplay.DisplayModeNew.y, 0, -1, 1);

		//  PGXP_SetMatrix(0, PSXDisplay.DisplayModeNew.x, PSXDisplay.DisplayModeNew.y, 0, -1, 1);

		if (bKeepRatio) SetAspectRatio();
	}

	bDisplayNotSet = TRUE;                                // re-calc offsets/display area

	bUp = FALSE;
	if (PSXDisplay.RGB24 != PSXDisplay.RGB24New)             // clean up textures, if rgb mode change (usually mdec on/off)
	{
		PreviousPSXDisplay.RGB24 = 0;                         // no full 24 frame uploaded yet
		ResetTextureArea(FALSE);
		bUp = TRUE;
	}

	PSXDisplay.RGB24 = PSXDisplay.RGB24New;       // get new infos
	PSXDisplay.DisplayMode.y = PSXDisplay.DisplayModeNew.y;
	PSXDisplay.DisplayMode.x = PSXDisplay.DisplayModeNew.x;
	PSXDisplay.Interlaced = PSXDisplay.InterlacedNew;

	PSXDisplay.DisplayEnd.x =                              // calc new ends
		PSXDisplay.DisplayPosition.x + PSXDisplay.DisplayMode.x;
	PSXDisplay.DisplayEnd.y =
		PSXDisplay.DisplayPosition.y + PSXDisplay.DisplayMode.y + PreviousPSXDisplay.DisplayModeNew.y;
	PreviousPSXDisplay.DisplayEnd.x =
		PreviousPSXDisplay.DisplayPosition.x + PSXDisplay.DisplayMode.x;
	PreviousPSXDisplay.DisplayEnd.y =
		PreviousPSXDisplay.DisplayPosition.y + PSXDisplay.DisplayMode.y + PreviousPSXDisplay.DisplayModeNew.y;

	ChangeDispOffsetsX();

	if (iFrameLimit == 2) SetAutoFrameCap();                 // set new fps limit vals (depends on interlace)

	if (bUp) updateDisplay();                              // yeah, real update (swap buffer)
}

////////////////////////////////////////////////////////////////////////
// update front display: smaller update func, if something has changed 
// in the frontbuffer... dirty, but hey... real men know no pain
////////////////////////////////////////////////////////////////////////

void updateFrontDisplay(void)
{
	if (PreviousPSXDisplay.Range.x0 ||
		PreviousPSXDisplay.Range.y0)
		PaintBlackBorders();

	//if (iBlurBuffer) BlurBackBuffer();

	if (iUseScanLines) SetScanLines();
	
	bFakeFrontBuffer = FALSE;
	bRenderFrontBuffer = FALSE;
	
	if (iDrawnSomething)
		swapBuffers();                    // -> swap
	
	//if (iBlurBuffer) UnBlurBackBuffer();
}

