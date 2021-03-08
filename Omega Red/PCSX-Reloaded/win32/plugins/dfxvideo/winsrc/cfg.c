/***************************************************************************
                          cfg.c  -  description
                             -------------------
    begin                : Sun Oct 28 2001
    copyright            : (C) 2001 by Pete Bernert
    email                : BlackDove@addcom.de
 ***************************************************************************/

/***************************************************************************
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version. See also the license.txt file for *
 *   additional informations.                                              *
 *                                                                         *
 ***************************************************************************/

#define _IN_CFG

#include "externals.h"
#include "cfg.h"
#include "gpu.h"

/////////////////////////////////////////////////////////////////////////////
// globals

char szKeyDefaults[11]={VK_DELETE,VK_INSERT,VK_HOME,VK_END,VK_PRIOR,VK_NEXT,VK_MULTIPLY,VK_SUBTRACT,VK_ADD,VK_F12,0x00};
char szDevName[128];

////////////////////////////////////////////////////////////////////////
// prototypes

BOOL OnInitCfgDialog(HWND hW);     
void OnCfgOK(HWND hW); 
BOOL OnInitSoftDialog(HWND hW);
void OnSoftOK(HWND hW); 
void OnCfgCancel(HWND hW); 
void OnCfgDef1(HWND hW);
void OnCfgDef2(HWND hW);
void OnBugFixes(HWND hW);


void SelectDev(HWND hW);
void OnKeyConfig(HWND hW);
void OnClipboard(HWND hW);
char * pGetConfigInfos(int iCfg);

////////////////////////////////////////////////////////////////////////
// funcs

BOOL CALLBACK SoftDlgProc(HWND hW, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
 switch(uMsg)
  {
   case WM_INITDIALOG:
     return OnInitSoftDialog(hW);

   case WM_COMMAND:
    {
     switch(LOWORD(wParam))
      {
       case IDC_DISPMODE1: 
        {
         CheckDlgButton(hW,IDC_DISPMODE2,FALSE);
         return TRUE;
        }
       case IDC_DISPMODE2: 
        {
         CheckDlgButton(hW,IDC_DISPMODE1,FALSE);
         return TRUE;
        }
       case IDC_DEF1:      OnCfgDef1(hW);   return TRUE;
       case IDC_DEF2:      OnCfgDef2(hW);   return TRUE;
       case IDC_SELFIX:    OnBugFixes(hW);  return TRUE;
       case IDC_KEYCONFIG: OnKeyConfig(hW); return TRUE;
       case IDCANCEL:      OnCfgCancel(hW); return TRUE;
       case IDOK:          OnSoftOK(hW);    return TRUE;
       case IDC_CLIPBOARD: OnClipboard(hW); return TRUE;

      }
    }
  }
 return FALSE;
}

////////////////////////////////////////////////////////////////////////
// init dlg
////////////////////////////////////////////////////////////////////////

void ComboBoxAddRes(HWND hWC,char * cs)
{
 int i=ComboBox_FindString(hWC,-1,cs);
 if(i!=CB_ERR) return;
 ComboBox_AddString(hWC,cs);
}

BOOL OnInitSoftDialog(HWND hW) 
{
 HWND hWC;char cs[256];int i;DEVMODE dv;

 ReadConfig();                                         // read registry stuff

 if(szDevName[0])
  SetDlgItemText(hW,IDC_DEVICETXT,szDevName);

 hWC=GetDlgItem(hW,IDC_RESOLUTION);

 memset(&dv,0,sizeof(DEVMODE));
 dv.dmSize=sizeof(DEVMODE);
 i=0;

 while(EnumDisplaySettings(NULL,i,&dv))
  {
   wsprintf(cs,"%4d x %4d - default",dv.dmPelsWidth,dv.dmPelsHeight);
   ComboBoxAddRes(hWC,cs);
   if(dv.dmDisplayFrequency > 40 && dv.dmDisplayFrequency < 200 )
    {
	 wsprintf(cs,"%4d x %4d , %4d Hz",dv.dmPelsWidth,dv.dmPelsHeight,dv.dmDisplayFrequency);
     ComboBoxAddRes(hWC,cs);
    }
   i++;
  }

 ComboBoxAddRes(hWC," 320 x  200 - default");
 ComboBoxAddRes(hWC," 320 x  240 - default");
 ComboBoxAddRes(hWC," 400 x  300 - default");
 ComboBoxAddRes(hWC," 512 x  384 - default");
 ComboBoxAddRes(hWC," 640 x  480 - default");
 ComboBoxAddRes(hWC," 800 x  600 - default");
 ComboBoxAddRes(hWC,"1024 x  768 - default");
 ComboBoxAddRes(hWC,"1152 x  864 - default");
 ComboBoxAddRes(hWC,"1280 x 1024 - default");
 ComboBoxAddRes(hWC,"1600 x 1200 - default");

 if(iRefreshRate)
      wsprintf(cs,"%4d x %4d , %4d Hz",iResX,iResY,iRefreshRate);
 else wsprintf(cs,"%4d x %4d - default",iResX,iResY);

 i=ComboBox_FindString(hWC,-1,cs);
 if(i==CB_ERR) i=0;
 ComboBox_SetCurSel(hWC,i);

 hWC=GetDlgItem(hW,IDC_COLDEPTH);
 ComboBox_AddString(hWC,"16 Bit");
 ComboBox_AddString(hWC,"32 Bit");
 wsprintf(cs,"%d Bit",iColDepth);                      // resolution
 i=ComboBox_FindString(hWC,-1,cs);
 if(i==CB_ERR) i=0;
 ComboBox_SetCurSel(hWC,i);

 hWC=GetDlgItem(hW,IDC_SCANLINES);
 ComboBox_AddString(hWC,"Scanlines disabled");
 ComboBox_AddString(hWC,"Scanlines enabled (standard)");
 ComboBox_AddString(hWC,"Scanlines enabled (double blitting - nVidia fix)");
 ComboBox_SetCurSel(hWC,iUseScanLines);


 if(UseFrameLimit)    CheckDlgButton(hW,IDC_USELIMIT,TRUE);
 if(UseFrameSkip)     CheckDlgButton(hW,IDC_USESKIPPING,TRUE);
 if(iWindowMode)      CheckRadioButton(hW,IDC_DISPMODE1,IDC_DISPMODE2,IDC_DISPMODE2);
 else    	          CheckRadioButton(hW,IDC_DISPMODE1,IDC_DISPMODE2,IDC_DISPMODE1);
 if(iSysMemory)       CheckDlgButton(hW,IDC_SYSMEMORY,TRUE);
 if(iStopSaver)       CheckDlgButton(hW,IDC_STOPSAVER,TRUE);
 if(iUseFixes)        CheckDlgButton(hW,IDC_GAMEFIX,TRUE);
 if(iShowFPS)         CheckDlgButton(hW,IDC_SHOWFPS,TRUE);
 if(bVsync)           CheckDlgButton(hW,IDC_VSYNC,TRUE);
 if(bTransparent)	  CheckDlgButton(hW,IDC_TRANSPARENT,TRUE);
 if(iDebugMode)	      CheckDlgButton(hW,IDC_DEBUGMODE,TRUE);

 hWC=GetDlgItem(hW,IDC_NOSTRETCH);                     // streching
 ComboBox_AddString(hWC,"Stretch to full window size");
 ComboBox_AddString(hWC,"1:1 (faster with some cards)");
 ComboBox_AddString(hWC,"Scale to window size, keep aspect ratio");
 ComboBox_AddString(hWC,"2xSaI stretching (needs a fast cpu)");
 ComboBox_AddString(hWC,"2xSaI unstretched (needs a fast cpu)");
 ComboBox_AddString(hWC,"Super2xSaI stretching (needs a very fast cpu)");
 ComboBox_AddString(hWC,"Super2xSaI unstretched (needs a very fast cpu)");
 ComboBox_AddString(hWC,"SuperEagle stretching (needs a fast cpu)");
 ComboBox_AddString(hWC,"SuperEagle unstretched (needs a fast cpu)");
 ComboBox_AddString(hWC,"Scale2x stretching (needs a fast cpu)");
 ComboBox_AddString(hWC,"Scale2x unstretched (needs a fast cpu)");
 ComboBox_AddString(hWC,"HQ2X unstretched (needs a fast cpu)");
 ComboBox_AddString(hWC,"HQ2X streched (needs a fast cpu)");
 ComboBox_AddString(hWC,"Scale3x stretching (needs a fast cpu)");
 ComboBox_AddString(hWC,"Scale3x unstretched (needs a fast cpu)");
 ComboBox_AddString(hWC,"HQ3X unstretched (needs a fast cpu)");
 ComboBox_SetCurSel(hWC,iUseNoStretchBlt);

 hWC=GetDlgItem(hW,IDC_DITHER);                        // dithering
 ComboBox_AddString(hWC,"No dithering (fastest)");
 ComboBox_AddString(hWC,"Game dependend dithering (slow)");
 ComboBox_AddString(hWC,"Always dither g-shaded polygons (slowest)");
 ComboBox_SetCurSel(hWC,iUseDither);

 if(iFrameLimit==2)                                    // frame limit wrapper
      CheckDlgButton(hW,IDC_FRAMEAUTO,TRUE);
 else CheckDlgButton(hW,IDC_FRAMEMANUELL,TRUE);

 sprintf(cs,"%.1f",fFrameRate);
 SetDlgItemText(hW,IDC_FRAMELIM,cs);                   // set frame rate

 return TRUE;
}
                         
////////////////////////////////////////////////////////////////////////
// on ok: take vals
////////////////////////////////////////////////////////////////////////

void OnSoftOK(HWND hW)                                 
{	 
 EndDialog(hW,TRUE);
}

////////////////////////////////////////////////////////////////////////
// on clipboard button
////////////////////////////////////////////////////////////////////////

void OnClipboard(HWND hW)
{
 HWND hWE=GetDlgItem(hW,IDC_CLPEDIT);
 char * pB;
 pB=pGetConfigInfos(1);

 if(pB)
  {
   SetDlgItemText(hW,IDC_CLPEDIT,pB);
   SendMessage(hWE,EM_SETSEL,0,-1);
   SendMessage(hWE,WM_COPY,0,0);
   free(pB);
   MessageBox(hW,"Configuration info successfully copied to the clipboard\nJust use the PASTE function in another program to retrieve the data!","Copy Info",MB_ICONINFORMATION|MB_OK);
  }
}

////////////////////////////////////////////////////////////////////////
// Cancel
////////////////////////////////////////////////////////////////////////

void OnCfgCancel(HWND hW) 
{
 EndDialog(hW,FALSE);
}

////////////////////////////////////////////////////////////////////////
// Bug fixes
////////////////////////////////////////////////////////////////////////

BOOL CALLBACK BugFixesDlgProc(HWND hW, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
 switch(uMsg)
  {
   case WM_INITDIALOG:
    {
     int i;

     for(i=0;i<32;i++)
      {
       if(dwCfgFixes&(1<<i))
        CheckDlgButton(hW,IDC_FIX1+i,TRUE);
      }
    }

   case WM_COMMAND:
    {
     switch(LOWORD(wParam))
      {
       case IDCANCEL: EndDialog(hW,FALSE);return TRUE;

       case IDOK:     
        {
         int i;
         dwCfgFixes=0;
         for(i=0;i<32;i++)
          {
           if(IsDlgButtonChecked(hW,IDC_FIX1+i))
            dwCfgFixes|=(1<<i);
          }
         EndDialog(hW,TRUE);
         return TRUE;
        }
      }
    }
  }
 return FALSE;
}

void OnBugFixes(HWND hW)
{
 DialogBox(hInst,MAKEINTRESOURCE(IDD_FIXES),
           hW,(DLGPROC)BugFixesDlgProc);
}


////////////////////////////////////////////////////////////////////////
// default 1: fast
////////////////////////////////////////////////////////////////////////

void OnCfgDef1(HWND hW) 
{
 HWND hWC;
 
 hWC=GetDlgItem(hW,IDC_RESOLUTION);
 ComboBox_SetCurSel(hWC,1);
 hWC=GetDlgItem(hW,IDC_COLDEPTH);
 ComboBox_SetCurSel(hWC,0);
 hWC=GetDlgItem(hW,IDC_SCANLINES);
 ComboBox_SetCurSel(hWC,0);
 CheckDlgButton(hW,IDC_USELIMIT,FALSE);
 CheckDlgButton(hW,IDC_USESKIPPING,TRUE);
 CheckRadioButton(hW,IDC_DISPMODE1,IDC_DISPMODE2,IDC_DISPMODE1);
 CheckDlgButton(hW,IDC_FRAMEAUTO,FALSE);
 CheckDlgButton(hW,IDC_FRAMEMANUELL,TRUE);
 CheckDlgButton(hW,IDC_SHOWFPS,FALSE);
 hWC=GetDlgItem(hW,IDC_NOSTRETCH);
 ComboBox_SetCurSel(hWC,1);
 hWC=GetDlgItem(hW,IDC_DITHER);
 ComboBox_SetCurSel(hWC,0);
 SetDlgItemInt(hW,IDC_FRAMELIM,200,FALSE);
 SetDlgItemInt(hW,IDC_WINX,320,FALSE);
 SetDlgItemInt(hW,IDC_WINY,240,FALSE);
 CheckDlgButton(hW,IDC_VSYNC,FALSE);
 CheckDlgButton(hW,IDC_TRANSPARENT,TRUE);
 CheckDlgButton(hW,IDC_DEBUGMODE,FALSE);
}                

////////////////////////////////////////////////////////////////////////
// default 2: nice
////////////////////////////////////////////////////////////////////////
                
void OnCfgDef2(HWND hW) 
{
 HWND hWC;
 
 hWC=GetDlgItem(hW,IDC_RESOLUTION);
 ComboBox_SetCurSel(hWC,2);
 hWC=GetDlgItem(hW,IDC_COLDEPTH);
 ComboBox_SetCurSel(hWC,0);
 hWC=GetDlgItem(hW,IDC_SCANLINES);
 ComboBox_SetCurSel(hWC,0);
 CheckDlgButton(hW,IDC_USELIMIT,TRUE);
 CheckDlgButton(hW,IDC_USESKIPPING,FALSE);
 CheckRadioButton(hW,IDC_DISPMODE1,IDC_DISPMODE2,IDC_DISPMODE1);
 CheckDlgButton(hW,IDC_FRAMEAUTO,TRUE);
 CheckDlgButton(hW,IDC_FRAMEMANUELL,FALSE);
 CheckDlgButton(hW,IDC_SHOWFPS,FALSE);
 CheckDlgButton(hW,IDC_VSYNC,FALSE);
 CheckDlgButton(hW,IDC_TRANSPARENT,TRUE);
 CheckDlgButton(hW,IDC_DEBUGMODE,FALSE);
 hWC=GetDlgItem(hW,IDC_NOSTRETCH);
 ComboBox_SetCurSel(hWC,0);
 hWC=GetDlgItem(hW,IDC_DITHER);
 ComboBox_SetCurSel(hWC,2);

 SetDlgItemInt(hW,IDC_FRAMELIM,200,FALSE);
 SetDlgItemInt(hW,IDC_WINX,640,FALSE);
 SetDlgItemInt(hW,IDC_WINY,480,FALSE);
}
                
////////////////////////////////////////////////////////////////////////
// read registry
////////////////////////////////////////////////////////////////////////

void ReadConfig(void)
{
// HKEY myKey;
// DWORD temp;
// DWORD type;               
// DWORD size;

 // predefines
 iResX=960;iResY=720;
 iColDepth=16;
 iWindowMode=0;
 UseFrameLimit=1;
 UseFrameSkip=0;
 iFrameLimit=2;
 fFrameRate=200.0f;
 dwCfgFixes=0;
 iUseFixes=0;
 iUseGammaVal=2048;
 iUseScanLines=0;
 iUseNoStretchBlt=0;
 iUseDither=0;
 iShowFPS=0;
 iSysMemory=0;
 iStopSaver=1;
 bVsync=FALSE;
 bTransparent=FALSE;
 iRefreshRate=0;
 iDebugMode=0;

 memset(szDevName,0,128);
 memset(&guiDev,0,sizeof(GUID));

// // standard Windows psx config (registry)
// if(RegOpenKeyEx(HKEY_CURRENT_USER,"Software\\Vision Thing\\PSEmu Pro\\GPU\\DFXVideo",0,KEY_ALL_ACCESS,&myKey)==ERROR_SUCCESS)
//  {
//   size = 4;
//   if(RegQueryValueEx(myKey,"ResX",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iResX=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"ResY",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iResY=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"RefreshRate",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iRefreshRate=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"WinSize",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iWinSize=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"WindowMode",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iWindowMode=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"ColDepth",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iColDepth=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"UseFrameLimit",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    UseFrameLimit=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"UseFrameSkip",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    UseFrameSkip=(int)temp;
//   size = 4;                     
//   if(RegQueryValueEx(myKey,"FrameLimit",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iFrameLimit=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"CfgFixes",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    dwCfgFixes=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"UseFixes",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iUseFixes=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"UseScanLines",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iUseScanLines=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"ShowFPS",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iShowFPS=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"UseNoStrechBlt",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iUseNoStretchBlt=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"UseDither",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iUseDither=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"UseGamma",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iUseGammaVal=(int)temp;
//   if(!iFrameLimit) {UseFrameLimit=0;UseFrameSkip=0;iFrameLimit=2;}
//
//   // try to get the float framerate... if none: take int framerate
//   fFrameRate=0.0f;
//   size = 4;
//   if(RegQueryValueEx(myKey,"FrameRateFloat",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    fFrameRate=*((float *)(&temp));
//   if(fFrameRate==0.0f)
//    {
//     fFrameRate=200.0f;
//     size = 4;
//     if(RegQueryValueEx(myKey,"FrameRate",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//      fFrameRate=(float)temp;
//    }
//
//   size = 4;
//   if(RegQueryValueEx(myKey,"UseSysMemory",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iSysMemory=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"StopSaver",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iStopSaver=(int)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"WaitVSYNC",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    bVsync=bVsync_Key=(BOOL)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"Transparent",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    bTransparent=(BOOL)temp;
//   size = 4;
//   if(RegQueryValueEx(myKey,"DebugMode",0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS)
//    iDebugMode=(BOOL)temp;
//   size=11;
//   RegQueryValueEx(myKey,"GPUKeys",0,&type,(LPBYTE)&szGPUKeys,&size);
//   size=128;
//   RegQueryValueEx(myKey,"DeviceName",0,&type,(LPBYTE)szDevName,&size);
//   size=sizeof(GUID);
//   RegQueryValueEx(myKey,"GuiDev",0,&type,(LPBYTE)&guiDev,&size);
//
////
//// Recording options
////
//#define GetDWORD(xa,xb) size=4;if(RegQueryValueEx(myKey,xa,0,&type,(LPBYTE)&temp,&size)==ERROR_SUCCESS) xb=(unsigned long)temp;
//#define GetBINARY(xa,xb) size=sizeof(xb);RegQueryValueEx(myKey,xa,0,&type,(LPBYTE)&xb,&size);
//	
//   GetDWORD("RecordingMode",				    RECORD_RECORDING_MODE);
//   GetDWORD("RecordingVideoSize",			    RECORD_VIDEO_SIZE);
//   GetDWORD("RecordingWidth",				    RECORD_RECORDING_WIDTH);
//   GetDWORD("RecordingHeight",				RECORD_RECORDING_HEIGHT);
//   GetDWORD("RecordingFrameRateScale",		RECORD_FRAME_RATE_SCALE);
//   GetDWORD("RecordingCompressionMode",	    RECORD_COMPRESSION_MODE);
//   GetBINARY("RecordingCompression1",		    RECORD_COMPRESSION1);
//   GetBINARY("RecordingCompressionState1",	RECORD_COMPRESSION_STATE1);
//   GetBINARY("RecordingCompression2",		    RECORD_COMPRESSION2);
//   GetBINARY("RecordingCompressionState2",	RECORD_COMPRESSION_STATE2);
//
//   if(RECORD_RECORDING_WIDTH>1024) RECORD_RECORDING_WIDTH = 1024;
//   if(RECORD_RECORDING_HEIGHT>768) RECORD_RECORDING_HEIGHT = 768;
//   if(RECORD_VIDEO_SIZE>2) RECORD_VIDEO_SIZE = 2;
//   if(RECORD_FRAME_RATE_SCALE>7) RECORD_FRAME_RATE_SCALE = 7;
//   if(RECORD_COMPRESSION1.cbSize != sizeof(RECORD_COMPRESSION1))
//    {
//     memset(&RECORD_COMPRESSION1,0,sizeof(RECORD_COMPRESSION1));
//     RECORD_COMPRESSION1.cbSize = sizeof(RECORD_COMPRESSION1);
//    }
//   RECORD_COMPRESSION1.lpState = RECORD_COMPRESSION_STATE1;
//   if(RECORD_COMPRESSION2.cbSize != sizeof(RECORD_COMPRESSION2))
//    {
//     memset(&RECORD_COMPRESSION2,0,sizeof(RECORD_COMPRESSION2));
//     RECORD_COMPRESSION2.cbSize = sizeof(RECORD_COMPRESSION2);
//    }
//   RECORD_COMPRESSION2.lpState = RECORD_COMPRESSION_STATE2;
//
////
//// end of recording options
////
//
//   RegCloseKey(myKey);
//  }
//
// if(!iColDepth) iColDepth=32;
// if(iUseFixes) dwActFixes=dwCfgFixes;
// SetFixes();
//
// if(iUseGammaVal<0 || iUseGammaVal>1536) iUseGammaVal=2048;
}

////////////////////////////////////////////////////////////////////////

HWND gHWND;


////////////////////////////////////////////////////////////////////////

void FreeGui(HWND hW)
{
 int i,iCnt;
 HWND hWC=GetDlgItem(hW,IDC_DEVICE);
 iCnt=ComboBox_GetCount(hWC);
 for(i=0;i<iCnt;i++)
  {
   free((GUID *)ComboBox_GetItemData(hWC,i));
  }
}

////////////////////////////////////////////////////////////////////////
// define key dialog
////////////////////////////////////////////////////////////////////////

typedef struct KEYSETSTAG
{
 char szName[10];
 char cCode;
}
KEYSETS;

KEYSETS tMKeys[]=
{
 {"SPACE",          0x20},
 {"PRIOR",          0x21},
 {"NEXT",           0x22},
 {"END",            0x23},
 {"HOME",           0x24},
 {"LEFT",           0x25},
 {"UP",             0x26},
 {"RIGHT",          0x27},
 {"DOWN",           0x28},
 {"SELECT",         0x29},
 {"PRINT",          0x2A},
 {"EXECUTE",        0x2B},
 {"SNAPSHOT",       0x2C},
 {"INSERT",         0x2D},
 {"DELETE",         0x2E},
 {"HELP",           0x2F},
 {"NUMPAD0",        0x60},
 {"NUMPAD1",        0x61},
 {"NUMPAD2",        0x62},
 {"NUMPAD3",        0x63},
 {"NUMPAD4",        0x64},
 {"NUMPAD5",        0x65},
 {"NUMPAD6",        0x66},
 {"NUMPAD7",        0x67},
 {"NUMPAD8",        0x68},
 {"NUMPAD9",        0x69},
 {"MULTIPLY",       0x6A},
 {"ADD",            0x6B},
 {"SEPARATOR",      0x6C},
 {"SUBTRACT",       0x6D},
 {"DECIMAL",        0x6E},
 {"DIVIDE",         0x6F},
 {"F9",             VK_F9},
 {"F10",            VK_F10},
 {"F11",            VK_F11},
 {"F12",            VK_F12},
 {"",               0x00}
};

void SetGPUKey(HWND hWC,char szKey)
{
 int i,iCnt=ComboBox_GetCount(hWC);
 for(i=0;i<iCnt;i++)
  {
   if(ComboBox_GetItemData(hWC,i)==szKey) break;
  }
 if(i!=iCnt) ComboBox_SetCurSel(hWC,i);
}                                 

BOOL CALLBACK KeyDlgProc(HWND hW, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
 switch(uMsg)
  {
   case WM_INITDIALOG:
    {
     int i,j,k;char szB[2];HWND hWC;
     for(i=IDC_KEY1;i<=IDC_KEY10;i++)
      {
       hWC=GetDlgItem(hW,i);

       for(j=0;tMKeys[j].cCode!=0;j++)
        {
         k=ComboBox_AddString(hWC,tMKeys[j].szName);
         ComboBox_SetItemData(hWC,k,tMKeys[j].cCode);
        }
       for(j=0x30;j<=0x39;j++)
        {
         wsprintf(szB,"%c",j);
         k=ComboBox_AddString(hWC,szB);
         ComboBox_SetItemData(hWC,k,j);
        }
       for(j=0x41;j<=0x5a;j++)
        {
         wsprintf(szB,"%c",j);
         k=ComboBox_AddString(hWC,szB);
         ComboBox_SetItemData(hWC,k,j);
        }                              
      }
    }return TRUE;

   case WM_COMMAND:
    {
     switch(LOWORD(wParam))
      {
       case IDC_DEFAULT:                 
        {
         int i;
         for(i=IDC_KEY1;i<=IDC_KEY10;i++)
          SetGPUKey(GetDlgItem(hW,i),szKeyDefaults[i-IDC_KEY1]);
        }break;

       case IDCANCEL:     EndDialog(hW,FALSE); return TRUE;
       case IDOK:
        {
         EndDialog(hW,TRUE);  
         return TRUE;
        }
      }
    }
  }
 return FALSE;
}

void OnKeyConfig(HWND hW) 
{
 DialogBox(hInst,MAKEINTRESOURCE(IDD_KEYS),
           hW,(DLGPROC)KeyDlgProc);
}

