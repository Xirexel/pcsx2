// Copyright (c) 2012- PPSSPP Project.

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.0 or later versions.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License 2.0 for more details.

// A copy of the GPL 2.0 should have been included with the program.
// If not, see http://www.gnu.org/licenses/

// Official git repository and contact information can be found at
// https://github.com/hrydgard/ppsspp and http://www.ppsspp.org/.

#include "stdafx.h"
#include <algorithm>
#include <cmath>
#include <functional>

#include "Common/CommonWindows.h"
#include "Common/OSVersion.h"
#include "Common/Vulkan/VulkanLoader.h"
#include "ppsspp_config.h"

#include <Wbemidl.h>
#include <shellapi.h>
#include <ShlObj.h>
#include <mmsystem.h>

#include "base/NativeApp.h"
#include "base/display.h"
#include "file/vfs.h"
#include "file/zip_read.h"
#include "i18n/i18n.h"
#include "profiler/profiler.h"
#include "thread/threadutil.h"
#include "util/text/utf8.h"
#include "net/resolve.h"

#include "Core/Config.h"
#include "Core/ConfigValues.h"
#include "Core/SaveState.h"
#include "Windows/EmuThread.h"
#include "Windows/WindowsAudio.h"
#include "ext/disarm.h"

#include "Common/LogManager.h"
#include "Common/ConsoleListener.h"

#include "Commctrl.h"

#include "UI/GameInfoCache.h"
#include "Windows/resource.h"

#include "Windows/MainWindow.h"
#include "Windows/Debugger/Debugger_Disasm.h"
#include "Windows/Debugger/Debugger_MemoryDlg.h"
#include "Windows/Debugger/Debugger_VFPUDlg.h"

#include "Windows/W32Util/DialogManager.h"
#include "Windows/W32Util/ShellUtil.h"

#include "Windows/Debugger/CtrlDisAsmView.h"
#include "Windows/Debugger/CtrlMemView.h"
#include "Windows/Debugger/CtrlRegisterList.h"
#include "Windows/InputBox.h"

#include "Windows/WindowsHost.h"
#include "Windows/main.h"



GetTouchPadCallback g_getTouchPad;

SetDataCallback g_setAudioData = nullptr;

static std::wstring s_boot_file;


// Nvidia OpenGL drivers >= v302 will check if the application exports a global
// variable named NvOptimusEnablement to know if it should run the app in high
// performance graphics mode or using the IGP.
extern "C" {
__declspec(dllexport) DWORD NvOptimusEnablement = 1;
}

// Also on AMD PowerExpress: https://community.amd.com/thread/169965
extern "C" {
__declspec(dllexport) int AmdPowerXpressRequestHighPerformance = 1;
}
#if PPSSPP_API(ANY_GL)
CGEDebugger *geDebuggerWindow = 0;
#endif

CDisasm *disasmWindow[MAX_CPUCOUNT] = {0};
CMemoryDlg *memoryWindow[MAX_CPUCOUNT] = {0};

static std::string langRegion;
static std::string osName;
static std::string gpuDriverVersion;

static std::string restartArgs;

HMENU g_hPopupMenus;
int g_activeWindow = 0;

void OpenDirectory(const char *path)
{
    PIDLIST_ABSOLUTE pidl = ILCreateFromPath(ConvertUTF8ToWString(ReplaceAll(path, "/", "\\")).c_str());
    if (pidl) {
        SHOpenFolderAndSelectItems(pidl, 0, NULL, 0);
        ILFree(pidl);
    }
}

void LaunchBrowser(const char *url)
{
    ShellExecute(NULL, L"open", ConvertUTF8ToWString(url).c_str(), NULL, NULL, SW_SHOWNORMAL);
}

void Vibrate(int length_ms)
{
    // Ignore on PC
}

// Adapted mostly as-is from http://www.gamedev.net/topic/495075-how-to-retrieve-info-about-videocard/?view=findpost&p=4229170
// so credit goes to that post's author, and in turn, the author of the site mentioned in that post (which seems to be down?).
std::string GetVideoCardDriverVersion()
{
    std::string retvalue = "";

    HRESULT hr;
    hr = CoInitializeEx(NULL, COINIT_MULTITHREADED);
    if (FAILED(hr)) {
        return retvalue;
    }

    IWbemLocator *pIWbemLocator = NULL;
    hr = CoCreateInstance(__uuidof(WbemLocator), NULL, CLSCTX_INPROC_SERVER,
                          __uuidof(IWbemLocator), (LPVOID *)&pIWbemLocator);
    if (FAILED(hr)) {
        CoUninitialize();
        return retvalue;
    }

    BSTR bstrServer = SysAllocString(L"\\\\.\\root\\cimv2");
    IWbemServices *pIWbemServices;
    hr = pIWbemLocator->ConnectServer(bstrServer, NULL, NULL, 0L, 0L, NULL, NULL, &pIWbemServices);
    if (FAILED(hr)) {
        pIWbemLocator->Release();
        SysFreeString(bstrServer);
        CoUninitialize();
        return retvalue;
    }

    hr = CoSetProxyBlanket(pIWbemServices, RPC_C_AUTHN_WINNT, RPC_C_AUTHZ_NONE,
                           NULL, RPC_C_AUTHN_LEVEL_CALL, RPC_C_IMP_LEVEL_IMPERSONATE, NULL, EOAC_DEFAULT);

    BSTR bstrWQL = SysAllocString(L"WQL");
    BSTR bstrPath = SysAllocString(L"select * from Win32_VideoController");
    IEnumWbemClassObject *pEnum;
    hr = pIWbemServices->ExecQuery(bstrWQL, bstrPath, WBEM_FLAG_FORWARD_ONLY, NULL, &pEnum);

    ULONG uReturned;
    VARIANT var;
    IWbemClassObject *pObj = NULL;
    if (!FAILED(hr)) {
        hr = pEnum->Next(WBEM_INFINITE, 1, &pObj, &uReturned);
    }

    if (!FAILED(hr) && uReturned) {
        hr = pObj->Get(L"DriverVersion", 0, &var, NULL, NULL);
        if (SUCCEEDED(hr)) {
            char str[MAX_PATH];
            WideCharToMultiByte(CP_ACP, 0, var.bstrVal, -1, str, sizeof(str), NULL, NULL);
            retvalue = str;
        }
    }

    pEnum->Release();
    SysFreeString(bstrPath);
    SysFreeString(bstrWQL);
    pIWbemServices->Release();
    pIWbemLocator->Release();
    SysFreeString(bstrServer);
    CoUninitialize();
    return retvalue;
}

std::string System_GetProperty(SystemProperty prop)
{
    static bool hasCheckedGPUDriverVersion = false;
    switch (prop) {
        case SYSPROP_NAME:
            return osName;
        case SYSPROP_LANGREGION:
            return langRegion;
        case SYSPROP_CLIPBOARD_TEXT: {
            std::string retval;
            if (OpenClipboard(MainWindow::GetDisplayHWND())) {
                HANDLE handle = GetClipboardData(CF_UNICODETEXT);
                const wchar_t *wstr = (const wchar_t *)GlobalLock(handle);
                if (wstr)
                    retval = ConvertWStringToUTF8(wstr);
                else
                    retval = "";
                GlobalUnlock(handle);
                CloseClipboard();
            }
            return retval;
        }
        case SYSPROP_GPUDRIVER_VERSION:
            if (!hasCheckedGPUDriverVersion) {
                hasCheckedGPUDriverVersion = true;
                gpuDriverVersion = GetVideoCardDriverVersion();
            }
            return gpuDriverVersion;
        default:
            return "";
    }
}

// Ugly!
extern WindowsAudioBackend *winAudioBackend;

#ifdef _WIN32
#if PPSSPP_PLATFORM(UWP)
static int ScreenDPI()
{
    return 96; // TODO UWP
}
#else
static int ScreenDPI()
{
    HDC screenDC = GetDC(nullptr);
    int dotsPerInch = GetDeviceCaps(screenDC, LOGPIXELSY);
    ReleaseDC(nullptr, screenDC);
    return dotsPerInch ? dotsPerInch : 96;
}
#endif
#endif

int System_GetPropertyInt(SystemProperty prop)
{
    switch (prop) {
        case SYSPROP_AUDIO_SAMPLE_RATE:
            return winAudioBackend ? winAudioBackend->GetSampleRate() : -1;
        case SYSPROP_DEVICE_TYPE:
            return DEVICE_TYPE_DESKTOP;
        case SYSPROP_DISPLAY_COUNT:
            return GetSystemMetrics(SM_CMONITORS);
        default:
            return -1;
    }
}

float System_GetPropertyFloat(SystemProperty prop)
{
    switch (prop) {
        case SYSPROP_DISPLAY_REFRESH_RATE:
            return 60.f;
        case SYSPROP_DISPLAY_DPI:
            return (float)ScreenDPI();
        case SYSPROP_DISPLAY_SAFE_INSET_LEFT:
        case SYSPROP_DISPLAY_SAFE_INSET_RIGHT:
        case SYSPROP_DISPLAY_SAFE_INSET_TOP:
        case SYSPROP_DISPLAY_SAFE_INSET_BOTTOM:
            return 0.0f;
        default:
            return -1;
    }
}

bool System_GetPropertyBool(SystemProperty prop)
{
    switch (prop) {
        case SYSPROP_HAS_FILE_BROWSER:
            return true;
        case SYSPROP_HAS_IMAGE_BROWSER:
            return true;
        case SYSPROP_HAS_BACK_BUTTON:
            return true;
        case SYSPROP_APP_GOLD:
#ifdef GOLD
            return true;
#else
            return false;
#endif
        default:
            return false;
    }
}

void System_SendMessage(const char *command, const char *parameter)
{
    if (!strcmp(command, "finish")) {
        if (!NativeIsRestarting()) {
            PostMessage(MainWindow::GetHWND(), WM_CLOSE, 0, 0);
        }
    } else if (!strcmp(command, "graphics_restart")) {
        restartArgs = parameter == nullptr ? "" : parameter;
        if (IsDebuggerPresent()) {
            PostMessage(MainWindow::GetHWND(), MainWindow::WM_USER_RESTART_EMUTHREAD, 0, 0);
        } else {
            g_Config.bRestartRequired = true;
            PostMessage(MainWindow::GetHWND(), WM_CLOSE, 0, 0);
        }
    } else if (!strcmp(command, "graphics_failedBackend")) {
        auto err = GetI18NCategory("Error");
        const char *backendSwitchError = err->T("GenericBackendSwitchError", "PPSSPP crashed while initializing graphics. Try upgrading your graphics drivers.\n\nGraphics backend has been switched:");
        std::wstring full_error = ConvertUTF8ToWString(StringFromFormat("%s %s", backendSwitchError, parameter));
        std::wstring title = ConvertUTF8ToWString(err->T("GenericGraphicsError", "Graphics Error"));
        MessageBox(MainWindow::GetHWND(), full_error.c_str(), title.c_str(), MB_OK);
    } else if (!strcmp(command, "setclipboardtext")) {
        if (OpenClipboard(MainWindow::GetDisplayHWND())) {
            std::wstring data = ConvertUTF8ToWString(parameter);
            HANDLE handle = GlobalAlloc(GMEM_MOVEABLE, (data.size() + 1) * sizeof(wchar_t));
            wchar_t *wstr = (wchar_t *)GlobalLock(handle);
            memcpy(wstr, data.c_str(), (data.size() + 1) * sizeof(wchar_t));
            GlobalUnlock(wstr);
            SetClipboardData(CF_UNICODETEXT, handle);
            GlobalFree(handle);
            CloseClipboard();
        }
    } else if (!strcmp(command, "browse_file")) {
        MainWindow::BrowseAndBoot("");
    } else if (!strcmp(command, "browse_folder")) {
        auto mm = GetI18NCategory("MainMenu");
        std::string folder = W32Util::BrowseForFolder(MainWindow::GetHWND(), mm->T("Choose folder"));
        if (folder.size())
            NativeMessageReceived("browse_folderSelect", folder.c_str());
    } else if (!strcmp(command, "bgImage_browse")) {
        MainWindow::BrowseBackground();
    } else if (!strcmp(command, "toggle_fullscreen")) {
        bool flag = !g_Config.bFullScreen;
        if (strcmp(parameter, "0") == 0) {
            flag = false;
        } else if (strcmp(parameter, "1") == 0) {
            flag = true;
        }
        MainWindow::SendToggleFullscreen(flag);
    }
}

void System_AskForPermission(SystemPermission permission) {}
PermissionStatus System_GetPermissionStatus(SystemPermission permission) { return PERMISSION_STATUS_GRANTED; }

void EnableCrashingOnCrashes()
{
    typedef BOOL(WINAPI * tGetPolicy)(LPDWORD lpFlags);
    typedef BOOL(WINAPI * tSetPolicy)(DWORD dwFlags);
    const DWORD EXCEPTION_SWALLOWING = 0x1;

    HMODULE kernel32 = LoadLibrary(L"kernel32.dll");
    tGetPolicy pGetPolicy = (tGetPolicy)GetProcAddress(kernel32,
                                                       "GetProcessUserModeExceptionPolicy");
    tSetPolicy pSetPolicy = (tSetPolicy)GetProcAddress(kernel32,
                                                       "SetProcessUserModeExceptionPolicy");
    if (pGetPolicy && pSetPolicy) {
        DWORD dwFlags;
        if (pGetPolicy(&dwFlags)) {
            // Turn off the filter.
            pSetPolicy(dwFlags & ~EXCEPTION_SWALLOWING);
        }
    }
    FreeLibrary(kernel32);
}

void System_InputBoxGetString(const std::string &title, const std::string &defaultValue, std::function<void(bool, const std::string &)> cb)
{
    std::string out;
    if (InputBox_GetString(MainWindow::GetHInstance(), MainWindow::GetHWND(), ConvertUTF8ToWString(title).c_str(), defaultValue, out)) {
        NativeInputBoxReceived(cb, true, out);
    } else {
        NativeInputBoxReceived(cb, false, "");
    }
}

static std::string GetDefaultLangRegion()
{
    wchar_t lcLangName[256] = {};

    // LOCALE_SNAME is only available in WinVista+
    if (0 != GetLocaleInfo(LOCALE_NAME_USER_DEFAULT, LOCALE_SNAME, lcLangName, ARRAY_SIZE(lcLangName))) {
        std::string result = ConvertWStringToUTF8(lcLangName);
        std::replace(result.begin(), result.end(), '-', '_');
        return result;
    } else {
        // This should work on XP, but we may get numbers for some countries.
        if (0 != GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_SISO639LANGNAME, lcLangName, ARRAY_SIZE(lcLangName))) {
            wchar_t lcRegion[256] = {};
            if (0 != GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_SISO3166CTRYNAME, lcRegion, ARRAY_SIZE(lcRegion))) {
                return ConvertWStringToUTF8(lcLangName) + "_" + ConvertWStringToUTF8(lcRegion);
            }
        }
        // Unfortunate default.  We tried.
        return "en_US";
    }
}

static const int EXIT_CODE_VULKAN_WORKS = 42;

static bool DetectVulkanInExternalProcess()
{
    std::wstring workingDirectory;
    std::wstring moduleFilename;
    W32Util::GetSelfExecuteParams(workingDirectory, moduleFilename);

    const wchar_t *cmdline = L"--vulkan-available-check";

    SHELLEXECUTEINFO info{sizeof(SHELLEXECUTEINFO)};
    info.fMask = SEE_MASK_NOCLOSEPROCESS;
    info.lpFile = moduleFilename.c_str();
    info.lpParameters = cmdline;
    info.lpDirectory = workingDirectory.c_str();
    info.nShow = SW_HIDE;
    if (ShellExecuteEx(&info) != TRUE) {
        return false;
    }
    if (info.hProcess == nullptr) {
        return false;
    }

    DWORD result = WaitForSingleObject(info.hProcess, 10000);
    DWORD exitCode = 0;
    if (result == WAIT_FAILED || GetExitCodeProcess(info.hProcess, &exitCode) == 0) {
        CloseHandle(info.hProcess);
        return false;
    }
    CloseHandle(info.hProcess);

    return exitCode == EXIT_CODE_VULKAN_WORKS;
}

std::vector<std::wstring> GetWideCmdLine()
{
    std::vector<std::wstring> wideArgs;

    if (!s_boot_file.empty()) {

        wideArgs.push_back(L"Empty_Stub");

        std::wstring l_boot_file = s_boot_file;

        auto lSplit = l_boot_file.find(L'|');

        while (lSplit != std::string::npos) {
            wideArgs.push_back(l_boot_file.substr(0, lSplit));

            l_boot_file = l_boot_file.substr(lSplit + 1, l_boot_file.size());

            lSplit = l_boot_file.find('|');
        }

        wideArgs.push_back(l_boot_file);
    }

    return wideArgs;
}

static void WinMainInit()
{
    CoInitializeEx(NULL, COINIT_MULTITHREADED);
    net::Init(); // This needs to happen before we load the config. So on Windows we also run it in Main. It's fine to call multiple times.

    // Windows, API init stuff
    INITCOMMONCONTROLSEX comm;
    comm.dwSize = sizeof(comm);
    comm.dwICC = ICC_BAR_CLASSES | ICC_LISTVIEW_CLASSES | ICC_TAB_CLASSES;
    InitCommonControlsEx(&comm);

    EnableCrashingOnCrashes();

#ifdef _DEBUG
    _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
    PROFILE_INIT();

#if defined(_M_X64) && defined(_MSC_VER) && _MSC_VER < 1900
    // FMA3 support in the 2013 CRT is broken on Vista and Windows 7 RTM (fixed in SP1). Just disable it.
    _set_FMA3_enable(0);
#endif
}

static void WinMainCleanup()
{
    net::Shutdown();
    CoUninitialize();

    if (g_Config.bRestartRequired) {
        W32Util::ExitAndRestart(!restartArgs.empty(), restartArgs);
    }
}

extern "C" int __stdcall Launch(LPWSTR szCmdLine, IUnknown *a_PtrUnkDirectX11Device, HWND a_VideoPanelHandler, HWND a_CaptureHandler, GetTouchPadCallback a_getTouchPad, SetDataCallback a_setAudioData, LPWSTR szStickDirectory)
{
    g_getTouchPad = a_getTouchPad;

    s_boot_file = szCmdLine;

    MainWindow::SetUnkDirectX11Device(a_PtrUnkDirectX11Device);

    MainWindow::SetDisplayHWND(a_VideoPanelHandler);

    MainWindow::SetCaptureHWND(a_CaptureHandler);

    auto l = ConvertWStringToUTF8(szStickDirectory);

    g_setAudioData = a_setAudioData;

    g_Config.memStickDirectory = l;

    g_Config.flash0Directory = l;

    //PSP_CoreParameter().pixelWidth = 1280;
    //PSP_CoreParameter().pixelHeight = 720;

    setCurrentThreadName("Main");

    //WinMainInit();

#ifndef _DEBUG
    bool showLog = false;
#else
    bool showLog = true;
#endif

    const std::string &exePath = File::GetExeDirectory();
    VFSRegister("", new DirectoryAssetReader((exePath + "/assets/").c_str()));
    VFSRegister("", new DirectoryAssetReader(exePath.c_str()));

    langRegion = GetDefaultLangRegion();
    osName = GetWindowsVersion() + " " + GetWindowsSystemArchitecture();

    std::string configFilename = "";
    const std::wstring configOption = L"--config=";

    std::string controlsConfigFilename = "";
    const std::wstring controlsOption = L"--controlconfig=";

    std::vector<std::wstring> wideArgs = GetWideCmdLine();

    for (size_t i = 1; i < wideArgs.size(); ++i) {
        if (wideArgs[i][0] == L'\0')
            continue;
        if (wideArgs[i][0] == L'-') {
            if (wideArgs[i].find(configOption) != std::wstring::npos && wideArgs[i].size() > configOption.size()) {
                const std::wstring tempWide = wideArgs[i].substr(configOption.size());
                configFilename = ConvertWStringToUTF8(tempWide);
            }

            if (wideArgs[i].find(controlsOption) != std::wstring::npos && wideArgs[i].size() > controlsOption.size()) {
                const std::wstring tempWide = wideArgs[i].substr(controlsOption.size());
                controlsConfigFilename = ConvertWStringToUTF8(tempWide);
            }
        }
    }

    LogManager::Init();

    // On Win32 it makes more sense to initialize the system directories here
    // because the next place it was called was in the EmuThread, and it's too late by then.
    g_Config.internalDataDirectory = W32Util::UserDocumentsPath();
    InitSysDirectories();

    // Load config up here, because those changes below would be overwritten
    // if it's not loaded here first.
    g_Config.AddSearchPath("");
    g_Config.AddSearchPath(GetSysDirectory(DIRECTORY_SYSTEM));
    g_Config.SetDefaultPath(GetSysDirectory(DIRECTORY_SYSTEM));
    g_Config.Load(configFilename.c_str(), controlsConfigFilename.c_str());

    g_Config.bFirstRun = false;

    bool debugLogLevel = false;

    g_Config.iGPUBackend = (int)GPUBackend::DIRECT3D11;
    g_Config.bSoftwareRendering = false;



    g_Config.bSaveSettings = false;
    g_Config.bGameSpecific = false;
    g_Config.bAutoSaveSymbolMap = false;
    g_Config.iAutoLoadSaveState = 0;
    g_Config.bSavedataUpgrade = false;


    timeBeginPeriod(1); // TODO: Evaluate if this makes sense to keep.

    MainWindow::Init(NULL);

    g_hPopupMenus = LoadMenu(NULL, (LPCWSTR)IDR_POPUPMENUS);

    //MainWindow::Show(NULL);

//    HWND hwndMain = MainWindow::GetHWND();
//    HWND hwndDisplay = MainWindow::GetDisplayHWND();
//
//    //initialize custom controls
//    CtrlDisAsmView::init();
//    CtrlMemView::init();
//    CtrlRegisterList::init();
//#if PPSSPP_API(ANY_GL)
//    CGEDebugger::Init();
//#endif
//    DialogManager::AddDlg(vfpudlg = new CVFPUDlg(_hInstance, hwndMain, currentDebugMIPS));
//
//    MainWindow::CreateDebugWindows();
//
//    const bool minimized = iCmdShow == SW_MINIMIZE || iCmdShow == SW_SHOWMINIMIZED || iCmdShow == SW_SHOWMINNOACTIVE;
//    if (minimized) {
//        MainWindow::Minimize();
//    }
//
//    // Emu thread (and render thread, if any) is always running!
//    // Only OpenGL uses an externally managed render thread (due to GL's single-threaded context design). Vulkan
//    // manages its own render thread.
    MainThread_Start(g_Config.iGPUBackend == (int)GPUBackend::OPENGL);
    InputDevice::BeginPolling();

    //HACCEL hAccelTable = LoadAccelerators(_hInstance, (LPCTSTR)IDR_ACCELS);
    //HACCEL hDebugAccelTable = LoadAccelerators(_hInstance, (LPCTSTR)IDR_DEBUGACCELS);

    ////so.. we're at the message pump of the GUI thread
    //for (MSG msg; GetMessage(&msg, NULL, 0, 0);) // for no quit
    //{
    //    if (msg.message == WM_KEYDOWN) {
    //        //hack to enable/disable menu command accelerate keys
    //        MainWindow::UpdateCommands();

    //        //hack to make it possible to get to main window from floating windows with Esc
    //        if (msg.hwnd != hwndMain && msg.wParam == VK_ESCAPE)
    //            BringWindowToTop(hwndMain);
    //    }

    //    //Translate accelerators and dialog messages...
    //    HWND wnd;
    //    HACCEL accel;
    //    switch (g_activeWindow) {
    //        case WINDOW_MAINWINDOW:
    //            wnd = hwndMain;
    //            accel = hAccelTable;
    //            break;
    //        case WINDOW_CPUDEBUGGER:
    //            wnd = disasmWindow[0] ? disasmWindow[0]->GetDlgHandle() : 0;
    //            accel = hDebugAccelTable;
    //            break;
    //        case WINDOW_GEDEBUGGER:
    //        default:
    //            wnd = 0;
    //            accel = 0;
    //            break;
    //    }

    //    if (!TranslateAccelerator(wnd, accel, &msg)) {
    //        if (!DialogManager::IsDialogMessage(&msg)) {
    //            //and finally translate and dispatch
    //            TranslateMessage(&msg);
    //            DispatchMessage(&msg);
    //        }
    //    }
    //}

    //MainThread_Stop();

    //VFSShutdown();

    //MainWindow::DestroyDebugWindows();
    //DialogManager::DestroyAll();

	while (GetUIState() != UISTATE_INGAME) {
        Sleep(200);
    }

    timeEndPeriod(1);



    //LogManager::Shutdown();
    //WinMainCleanup();

    return 0;
}


static std::mutex s_lock_mutex;

static std::condition_variable l_lock_condition;

using namespace std::chrono_literals;

extern "C" void __stdcall Save(LPSTR a_filename)
{
    std::unique_lock<std::mutex> l_lock(s_lock_mutex);

    SaveState::Save(a_filename, -1, [](SaveState::Status status, const std::string &message, void *) {
        if (!message.empty()) {
        }

        l_lock_condition.notify_one();
    });

    l_lock_condition.wait_for(l_lock, 50 * 100ms);

    Sleep(500);
}

extern "C" void __stdcall Load(LPSTR a_filename)
{
    std::unique_lock<std::mutex> l_lock(s_lock_mutex);

    SaveState::Load(a_filename, -1, [](SaveState::Status status, const std::string &message, void *) {
        if (!message.empty()) {
        }

        l_lock_condition.notify_one();
    });

    l_lock_condition.wait_for(l_lock, 50 * 100ms);
}



#include "Core\Loaders.h"


#include "thread/prioritizedworkqueue.h"
#include "Common/FileUtil.h"
#include "Common/StringUtils.h"
#include "Core/FileSystems/ISOFileSystem.h"
#include "Core/FileSystems/DirectoryFileSystem.h"
#include "Core/FileSystems/VirtualDiscFileSystem.h"


static bool ReadFileToString(IFileSystem *fs, const char *filename, std::string *contents, std::mutex *mtx)
{
    PSPFileInfo info = fs->GetFileInfo(filename);
    if (!info.exists) {
        return false;
    }

    int handle = fs->OpenFile(filename, FILEACCESS_READ);
    if (!handle) {
        return false;
    }

    if (mtx) {
        std::lock_guard<std::mutex> lock(*mtx);
        contents->resize(info.size);
        fs->ReadFile(handle, (u8 *)contents->data(), info.size);
    } else {
        contents->resize(info.size);
        fs->ReadFile(handle, (u8 *)contents->data(), info.size);
    }
    fs->CloseFile(handle);
    return true;
}

static bool ReadVFSToString(const char *filename, std::string *contents, std::mutex *mtx)
{
    size_t sz;
    uint8_t *data = VFSReadFile(filename, &sz);
    if (data) {
        if (mtx) {
            std::lock_guard<std::mutex> lock(*mtx);
            *contents = std::string((const char *)data, sz);
        } else {
            *contents = std::string((const char *)data, sz);
        }
    }
    delete[] data;
    return data != nullptr;
}



class GameInfoWorkItemCopy : public PrioritizedWorkQueueItem
{
public:
    GameInfoWorkItemCopy(const std::string &gamePath, std::shared_ptr<GameInfo> &info)
        : gamePath_(gamePath)
        , info_(info)
    {
    }

    ~GameInfoWorkItemCopy() override
    {
        info_->DisposeFileLoader();
    }

    void run() override
    {
        if (!info_->LoadFromPath(gamePath_)) {
            info_->pending = false;
            return;
        }
        // In case of a remote file, check if it actually exists before locking.
        if (!info_->GetFileLoader()->Exists()) {
            info_->pending = false;
            return;
        }

        info_->working = true;
        info_->fileType = Identify_File(info_->GetFileLoader().get());
        switch (info_->fileType) {
            case IdentifiedFileType::PSP_PBP:
            case IdentifiedFileType::PSP_PBP_DIRECTORY: {
                auto pbpLoader = info_->GetFileLoader();
                if (info_->fileType == IdentifiedFileType::PSP_PBP_DIRECTORY) {
                    std::string ebootPath = ResolvePBPFile(gamePath_);
                    if (ebootPath != gamePath_) {
                        pbpLoader.reset(ConstructFileLoader(ebootPath));
                    }
                }

                PBPReader pbp(pbpLoader.get());
                if (!pbp.IsValid()) {
                    if (pbp.IsELF()) {
                        goto handleELF;
                    }
                    ERROR_LOG(LOADER, "invalid pbp %s\n", pbpLoader->Path().c_str());
                    info_->pending = false;
                    info_->working = false;
                    return;
                }

                // First, PARAM.SFO.
                std::vector<u8> sfoData;
                if (pbp.GetSubFile(PBP_PARAM_SFO, &sfoData)) {
                    std::lock_guard<std::mutex> lock(info_->lock);
                    info_->paramSFO.ReadSFO(sfoData);
                    info_->ParseParamSFO();

                    // Assuming PSP_PBP_DIRECTORY without ID or with disc_total < 1 in GAME dir must be homebrew
                    if ((info_->id.empty() || !info_->disc_total) && gamePath_.find("/PSP/GAME/") != std::string::npos && info_->fileType == IdentifiedFileType::PSP_PBP_DIRECTORY) {
                        info_->id = g_paramSFO.GenerateFakeID(gamePath_);
                        info_->id_version = info_->id + "_1.00";
                        info_->region = GAMEREGION_MAX + 1; // Homebrew
                    }
                }

                // Then, ICON0.PNG.
                if (pbp.GetSubFileSize(PBP_ICON0_PNG) > 0) {
                    std::lock_guard<std::mutex> lock(info_->lock);
                    pbp.GetSubFileAsString(PBP_ICON0_PNG, &info_->icon.data);
                } else {
                    std::string screenshot_jpg = GetSysDirectory(DIRECTORY_SCREENSHOT) + info_->id + "_00000.jpg";
                    std::string screenshot_png = GetSysDirectory(DIRECTORY_SCREENSHOT) + info_->id + "_00000.png";
                    // Try using png/jpg screenshots first
                    if (File::Exists(screenshot_png))
                        readFileToString(false, screenshot_png.c_str(), info_->icon.data);
                    else if (File::Exists(screenshot_jpg))
                        readFileToString(false, screenshot_jpg.c_str(), info_->icon.data);
                    else
                        // Read standard icon
                        ReadVFSToString("unknown.png", &info_->icon.data, &info_->lock);
                }
                info_->icon.dataLoaded = true;

                if (info_->wantFlags & GAMEINFO_WANTBG) {
                    if (pbp.GetSubFileSize(PBP_PIC0_PNG) > 0) {
                        std::lock_guard<std::mutex> lock(info_->lock);
                        pbp.GetSubFileAsString(PBP_PIC0_PNG, &info_->pic0.data);
                        info_->pic0.dataLoaded = true;
                    }
                    if (pbp.GetSubFileSize(PBP_PIC1_PNG) > 0) {
                        std::lock_guard<std::mutex> lock(info_->lock);
                        pbp.GetSubFileAsString(PBP_PIC1_PNG, &info_->pic1.data);
                        info_->pic1.dataLoaded = true;
                    }
                }
                if (info_->wantFlags & GAMEINFO_WANTSND) {
                    if (pbp.GetSubFileSize(PBP_SND0_AT3) > 0) {
                        std::lock_guard<std::mutex> lock(info_->lock);
                        pbp.GetSubFileAsString(PBP_SND0_AT3, &info_->sndFileData);
                        info_->sndDataLoaded = true;
                    }
                }
            } break;

            case IdentifiedFileType::PSP_ELF:
            handleELF:
                // An elf on its own has no usable information, no icons, no nothing.
                {
                    std::lock_guard<std::mutex> lock(info_->lock);
                    info_->id = g_paramSFO.GenerateFakeID(gamePath_);
                    info_->id_version = info_->id + "_1.00";
                    info_->region = GAMEREGION_MAX + 1; // Homebrew

                    info_->paramSFOLoaded = true;
                }
                {
                    std::string screenshot_jpg = GetSysDirectory(DIRECTORY_SCREENSHOT) + info_->id + "_00000.jpg";
                    std::string screenshot_png = GetSysDirectory(DIRECTORY_SCREENSHOT) + info_->id + "_00000.png";
                    // Try using png/jpg screenshots first
                    if (File::Exists(screenshot_png)) {
                        readFileToString(false, screenshot_png.c_str(), info_->icon.data);
                    } else if (File::Exists(screenshot_jpg)) {
                        readFileToString(false, screenshot_jpg.c_str(), info_->icon.data);
                    } else {
                        // Read standard icon
                        VERBOSE_LOG(LOADER, "Loading unknown.png because there was an ELF");
                        ReadVFSToString("unknown.png", &info_->icon.data, &info_->lock);
                    }
                    info_->icon.dataLoaded = true;
                }
                break;

            case IdentifiedFileType::PSP_SAVEDATA_DIRECTORY: {
                SequentialHandleAllocator handles;
                VirtualDiscFileSystem umd(&handles, gamePath_.c_str());

                // Alright, let's fetch the PARAM.SFO.
                std::string paramSFOcontents;
                if (ReadFileToString(&umd, "/PARAM.SFO", &paramSFOcontents, 0)) {
                    std::lock_guard<std::mutex> lock(info_->lock);
                    info_->paramSFO.ReadSFO((const u8 *)paramSFOcontents.data(), paramSFOcontents.size());
                    info_->ParseParamSFO();
                }

                ReadFileToString(&umd, "/ICON0.PNG", &info_->icon.data, &info_->lock);
                info_->icon.dataLoaded = true;
                if (info_->wantFlags & GAMEINFO_WANTBG) {
                    ReadFileToString(&umd, "/PIC1.PNG", &info_->pic1.data, &info_->lock);
                    info_->pic1.dataLoaded = true;
                }
                break;
            }

            case IdentifiedFileType::PPSSPP_SAVESTATE: {
                info_->SetTitle(SaveState::GetTitle(gamePath_));

                std::lock_guard<std::mutex> guard(info_->lock);

                // Let's use the screenshot as an icon, too.
                std::string screenshotPath = ReplaceAll(gamePath_, ".ppst", ".jpg");
                if (File::Exists(screenshotPath)) {
                    if (readFileToString(false, screenshotPath.c_str(), info_->icon.data)) {
                        info_->icon.dataLoaded = true;
                    } else {
                        ERROR_LOG(G3D, "Error loading screenshot data: '%s'", screenshotPath.c_str());
                    }
                }
                break;
            }

            case IdentifiedFileType::PSP_DISC_DIRECTORY: {
                info_->fileType = IdentifiedFileType::PSP_ISO;
                SequentialHandleAllocator handles;
                VirtualDiscFileSystem umd(&handles, gamePath_.c_str());

                // Alright, let's fetch the PARAM.SFO.
                std::string paramSFOcontents;
                if (ReadFileToString(&umd, "/PSP_GAME/PARAM.SFO", &paramSFOcontents, 0)) {
                    std::lock_guard<std::mutex> lock(info_->lock);
                    info_->paramSFO.ReadSFO((const u8 *)paramSFOcontents.data(), paramSFOcontents.size());
                    info_->ParseParamSFO();
                }

                ReadFileToString(&umd, "/PSP_GAME/ICON0.PNG", &info_->icon.data, &info_->lock);
                info_->icon.dataLoaded = true;
                if (info_->wantFlags & GAMEINFO_WANTBG) {
                    ReadFileToString(&umd, "/PSP_GAME/PIC0.PNG", &info_->pic0.data, &info_->lock);
                    info_->pic0.dataLoaded = true;
                    ReadFileToString(&umd, "/PSP_GAME/PIC1.PNG", &info_->pic1.data, &info_->lock);
                    info_->pic1.dataLoaded = true;
                }
                if (info_->wantFlags & GAMEINFO_WANTSND) {
                    ReadFileToString(&umd, "/PSP_GAME/SND0.AT3", &info_->sndFileData, &info_->lock);
                    info_->pic1.dataLoaded = true;
                }
                break;
            }

            case IdentifiedFileType::PSP_ISO:
            case IdentifiedFileType::PSP_ISO_NP: {
                info_->fileType = IdentifiedFileType::PSP_ISO;
                SequentialHandleAllocator handles;
                // Let's assume it's an ISO.
                // TODO: This will currently read in the whole directory tree. Not really necessary for just a
                // few files.
                auto fl = info_->GetFileLoader();
                if (!fl) {
                    info_->pending = false;
                    info_->working = false;
                    return; // Happens with UWP currently, TODO...
                }
                BlockDevice *bd = constructBlockDevice(info_->GetFileLoader().get());
                if (!bd) {
                    info_->pending = false;
                    info_->working = false;
                    return; // nothing to do here..
                }
                ISOFileSystem umd(&handles, bd);

                // Alright, let's fetch the PARAM.SFO.
                std::string paramSFOcontents;
                if (ReadFileToString(&umd, "/PSP_GAME/PARAM.SFO", &paramSFOcontents, nullptr)) {
                    std::lock_guard<std::mutex> lock(info_->lock);
                    info_->paramSFO.ReadSFO((const u8 *)paramSFOcontents.data(), paramSFOcontents.size());
                    info_->ParseParamSFO();

                    if (info_->wantFlags & GAMEINFO_WANTBG) {
                        ReadFileToString(&umd, "/PSP_GAME/PIC0.PNG", &info_->pic0.data, nullptr);
                        info_->pic0.dataLoaded = true;
                        ReadFileToString(&umd, "/PSP_GAME/PIC1.PNG", &info_->pic1.data, nullptr);
                        info_->pic1.dataLoaded = true;
                    }
                    if (info_->wantFlags & GAMEINFO_WANTSND) {
                        ReadFileToString(&umd, "/PSP_GAME/SND0.AT3", &info_->sndFileData, nullptr);
                        info_->pic1.dataLoaded = true;
                    }
                }

                // Fall back to unknown icon if ISO is broken/is a homebrew ISO, override is allowed though
                if (!ReadFileToString(&umd, "/PSP_GAME/ICON0.PNG", &info_->icon.data, &info_->lock)) {
                    std::string screenshot_jpg = GetSysDirectory(DIRECTORY_SCREENSHOT) + info_->id + "_00000.jpg";
                    std::string screenshot_png = GetSysDirectory(DIRECTORY_SCREENSHOT) + info_->id + "_00000.png";
                    // Try using png/jpg screenshots first
                    if (File::Exists(screenshot_png))
                        readFileToString(false, screenshot_png.c_str(), info_->icon.data);
                    else if (File::Exists(screenshot_jpg))
                        readFileToString(false, screenshot_jpg.c_str(), info_->icon.data);
                    else {
                        DEBUG_LOG(LOADER, "Loading unknown.png because no icon was found");
                        ReadVFSToString("unknown.png", &info_->icon.data, &info_->lock);
                    }
                }
                info_->icon.dataLoaded = true;
                break;
            }

            case IdentifiedFileType::ARCHIVE_ZIP:
                info_->paramSFOLoaded = true;
                {
                    ReadVFSToString("zip.png", &info_->icon.data, &info_->lock);
                    info_->icon.dataLoaded = true;
                }
                break;

            case IdentifiedFileType::ARCHIVE_RAR:
                info_->paramSFOLoaded = true;
                {
                    ReadVFSToString("rargray.png", &info_->icon.data, &info_->lock);
                    info_->icon.dataLoaded = true;
                }
                break;

            case IdentifiedFileType::ARCHIVE_7Z:
                info_->paramSFOLoaded = true;
                {
                    ReadVFSToString("7z.png", &info_->icon.data, &info_->lock);
                    info_->icon.dataLoaded = true;
                }
                break;

            case IdentifiedFileType::NORMAL_DIRECTORY:
            default:
                info_->paramSFOLoaded = true;
                break;
        }

        info_->hasConfig = g_Config.hasGameConfig(info_->id);

        if (info_->wantFlags & GAMEINFO_WANTSIZE) {
            std::lock_guard<std::mutex> lock(info_->lock);
            info_->gameSize = info_->GetGameSizeInBytes();
            info_->saveDataSize = info_->GetSaveDataSizeInBytes();
            info_->installDataSize = info_->GetInstallDataSizeInBytes();
        }

        info_->pending = false;
        info_->working = false;
        // ILOG("Completed writing info for %s", info_->GetTitle().c_str());
    }

    float priority() override
    {
        auto fl = info_->GetFileLoader();
        if (fl && fl->IsRemote()) {
            // Increase the value so remote info loads after non-remote.
            return info_->lastAccessedTime + 1000.0f;
        }
        return info_->lastAccessedTime;
    }

private:
    std::string gamePath_;
    std::shared_ptr<GameInfo> info_;
    DISALLOW_COPY_AND_ASSIGN(GameInfoWorkItemCopy);
};

extern "C" void __stdcall GetGameInfo(const char *a_filename, BSTR *a_result)
{
    auto info = std::make_shared<GameInfo>();

    auto l_GameInfoWorkItem = std::make_shared<GameInfoWorkItemCopy>(a_filename, info);

    l_GameInfoWorkItem->run();

    if (info->fileType == IdentifiedFileType::PSP_ISO) {

        std::string lout = info->GetTitle() + "#" + info->id;

        OLECHAR *oleChar = NULL;
        oleChar = (OLECHAR *)calloc(strlen(lout.c_str()) + 1, sizeof(OLECHAR));
        MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, lout.c_str(), -1, oleChar, strlen(lout.c_str()) + 1);
        *a_result = SysAllocString(oleChar);
        free(oleChar);
        oleChar = NULL;
    }
}

extern "C" void __stdcall Shutdown()
{
    MainThread_Stop();

    VFSShutdown();

    InputDevice::StopPolling();

    MainWindow::DestroyDebugWindows();
    DialogManager::DestroyAll();
    timeEndPeriod(1);

    LogManager::Shutdown();

    net::Shutdown();

    g_setAudioData = nullptr;
}

extern "C" void __stdcall Pause()
{
    Core_EnableStepping(true);
}

extern "C" void __stdcall Resume()
{
    Core_EnableStepping(false);
}

extern void setVolume(float a_level);

extern "C" void __stdcall SetAudioVolume(float a_level)
{
    setVolume(a_level);
}

extern "C" void __stdcall SetLimitFrame(bool a_limit)
{
    PSP_CoreParameter().unthrottle = !a_limit;
}
