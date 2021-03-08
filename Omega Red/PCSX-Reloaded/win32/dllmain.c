
#include <windows.h>
#include <windowsx.h>
#include <commctrl.h>
#include <time.h>
#include <stdio.h>
#include <string.h>
#include <stdarg.h>

#include "resource.h"
#include "AboutDlg.h"

#include "psxcommon.h"
#include "plugin.h"
#include "debug.h"
#include "Win32.h"
#include "sio.h"
#include "misc.h"
#include "cheat.h"

#include <avrt.h>

#pragma comment(lib, "Avrt.lib")

typedef void(CALLBACK *_Callback)();

typedef int(CALLBACK *_CallbackMemory)(void* a_ptrBIOS, int a_size);

int InitPlugins();

void resetPSXCounters();

_Callback PluginsOpenCallback = NULL;

_Callback PluginsCloseCallback = NULL;

_CallbackMemory BIOSMemoryCallback = NULL;

int OpenLibPlugins()
{
    int l_result = OpenPlugins(NULL);

	if (l_result != -1 && PluginsOpenCallback != NULL)
        PluginsOpenCallback();

	return l_result;
}


void CloseLibPlugins()
{
    ClosePlugins();

    if (PluginsCloseCallback != NULL)
        PluginsCloseCallback();
}



int SysLibInit()
{

    if (EmuInit() == -1)
        return -1;

#ifdef EMU_LOG
    emuLog = fopen("emuLog.txt", "w");
    setvbuf(emuLog, NULL, _IONBF, 0);
#endif

    while (InitPlugins() == -1) {
        CancelQuit = 1;

        CancelQuit = 0;
    }
    //LoadMcds(Config.Mcd1, Config.Mcd2);
	
    return 0;
}

void SysLibClose()
{
    EmuShutdown();
    ReleasePlugins();

    StopDebugger();

    if (Config.PsxOut)
        CloseConsole();

    if (emuLog != NULL)
        fclose(emuLog);
}

static HANDLE mmcssHandle = NULL;

static DWORD mmcssTaskIndex = 0;

int StartGame()
{
    if (OpenLibPlugins() == -1) {
        CloseLibPlugins();
        RestoreWindow();
        return TRUE;
    }
    if (CheckCdrom() == -1) {
        fprintf(stderr, _("The CD does not appear to be a valid Playstation CD"));
        CloseLibPlugins();
        RestoreWindow();
        return TRUE;
    }

    // Auto-detect: region first, then rcnt reset
    SysReset();

    if (LoadCdrom() == -1) {
        fprintf(stderr, _("Could not load CD-ROM!"));
        CloseLibPlugins();
        RestoreWindow();
        return TRUE;
    }
    if (Config.HideCursor)
        ShowCursor(FALSE);
    Running = 1;

    mmcssHandle = AvSetMmThreadCharacteristics(L"Audio", &mmcssTaskIndex);

    if (mmcssHandle != NULL)
        AvSetMmThreadPriority(mmcssHandle, AVRT_PRIORITY_CRITICAL);

    return StartEmul();
}

int __stdcall Launch(LPSTR lpCmdLine)
{
    resetPSXCounters();

    int l_result = -1;

    char cdfile[MAXPATHLEN] = "";
    int loadstatenum = -1;

    strcpy(cfgfile, "Software\\Pcsxr");

    gApp.hInstance = NULL;
	
    Running = 0;

    GetCurrentDirectory(256, PcsxrDir);

    memset(&Config, 0, sizeof(PcsxConfig));
    strcpy(Config.Net, "Disabled");

    Config.PsxAuto = 1;
    strcpy(Config.Bios, "HLE");
		
    UseGui = FALSE;

	strcpy(cdfile, lpCmdLine);
		   
    if (SysLibInit() == -1)
        return 1;

    SetIsoFile(cdfile);

    l_result = StartGame();
	
    return l_result;
}

int __stdcall Shutdown()
{
    int l_result = ShutdownEmul();

    AvRevertMmThreadCharacteristics(mmcssHandle);

    SysLibClose();

	CloseLibPlugins();

	return l_result;
}

int __stdcall Pause()
{
    return PauseEmul();
}

int __stdcall Resume()
{
    return ResumeEmul();
}

void __stdcall setPluginsOpenCallback(_Callback aPluginsOpenCallback)
{
    PluginsOpenCallback = aPluginsOpenCallback;
}

void __stdcall setPluginsCloseCallback(_Callback aPluginsCloseCallback)
{
    PluginsCloseCallback = aPluginsCloseCallback;
}

int __stdcall SaveLibState(LPSTR a_filePath)
{
    int ret = -1;

	do {

        int ret = SaveState(a_filePath);

		if (ret != 0)
            break;
			   
        Running = 1;

        CheatSearchBackupMemory();
		
    } while (FALSE);

	return ret;
}

int __stdcall LoadLibState(LPSTR a_filePath)
{
    int ret = -1;

    do {
		
        ret = LoadState(a_filePath);
		
    } while (FALSE);

    return ret;
}

void __stdcall setBIOSMemoryCallback(_CallbackMemory aBIOSMemoryCallback)
{
    BIOSMemoryCallback = aBIOSMemoryCallback;
}

int __stdcall LoadBIOS(void *a_ptrBIOS, int a_size)
{
    int ret = FALSE;

    do {

		if (BIOSMemoryCallback == NULL)
            break;

		ret = BIOSMemoryCallback(a_ptrBIOS, a_size);

    } while (FALSE);

    return ret;
}