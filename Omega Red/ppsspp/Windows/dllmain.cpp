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

#include "Common/CommonWindows.h"
#include "Common/OSVersion.h"

#include <Wbemidl.h>
#include <shellapi.h>
#include <mmsystem.h>

#include "base/display.h"
#include "file/vfs.h"
#include "file/zip_read.h"
#include "base/NativeApp.h"
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
#include "Windows/GEDebugger/GEDebugger.h"

#include "Windows/W32Util/DialogManager.h"

#include "Windows/Debugger/CtrlDisAsmView.h"
#include "Windows/Debugger/CtrlMemView.h"
#include "Windows/Debugger/CtrlRegisterList.h"
#include "Windows/InputBox.h"

#include "Windows/WindowsHost.h"
#include "Windows/dllmain.h"

#include <d3d11.h>
#include <d3d11_1.h>

#include "./GPU/ComPtrCustom.h"
#include "./Windows/AudioCaptureProcessor.h"


static std::string langRegion;
static std::string osName;
static std::string gpuDriverVersion;

// Ugly!
extern WindowsAudioBackend *winAudioBackend;


void *g_TouchPadHandler;
ICaptureProcessor* g_ICaptureProcessor;



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

static std::wstring s_boot_file;

std::vector<std::wstring> GetWideCmdLine()
{
    wchar_t **wargv;
    int wargc = -1;
    wargv = CommandLineToArgvW(GetCommandLineW(), &wargc);

    std::vector<std::wstring> wideArgs(wargv, wargv + wargc);
    LocalFree(wargv);

	if (!s_boot_file.empty()) {

		std::wstring l_boot_file = s_boot_file;

		auto lSplit = l_boot_file.find(L'|');

		while(lSplit != std::string::npos)
        {
            wideArgs.push_back(l_boot_file.substr(0, lSplit));

			l_boot_file = l_boot_file.substr(lSplit + 1, l_boot_file.size());

			lSplit = l_boot_file.find('|');
        }

        wideArgs.push_back(l_boot_file);
	}

    return wideArgs;
}

extern "C" int __stdcall Launch(LPWSTR szCmdLine, HWND a_VideoPanelHandler, HWND a_CaptureHandler, void *a_TouchPadHandler, LPWSTR szStickDirectory)
{
    g_TouchPadHandler = a_TouchPadHandler;

    s_boot_file = szCmdLine;

    MainWindow::SetDisplayHWND(a_VideoPanelHandler);

    MainWindow::SetCaptureHWND(a_CaptureHandler);

	auto l = ConvertWStringToUTF8(szStickDirectory);	
	
    g_ICaptureProcessor = new AudioCaptureProcessor();

	g_Config.memStickDirectory = l;

	g_Config.flash0Directory = l;

    PSP_CoreParameter().pixelWidth = 1280;
    PSP_CoreParameter().pixelHeight = 720;

    setCurrentThreadName("Main");

    net::Init(); // This needs to happen before we load the config. So on Windows we also run it in Main. It's fine to call multiple times.

    // Windows, API init stuff
    INITCOMMONCONTROLSEX comm;
    comm.dwSize = sizeof(comm);
    comm.dwICC = ICC_BAR_CLASSES | ICC_LISTVIEW_CLASSES | ICC_TAB_CLASSES;
    InitCommonControlsEx(&comm);


    PROFILE_INIT();

    bool showLog = false;

    const std::string &exePath = File::GetExeDirectory();
    VFSRegister("", new DirectoryAssetReader((exePath + "/assets/").c_str()));
    VFSRegister("", new DirectoryAssetReader(exePath.c_str()));

    langRegion = GetDefaultLangRegion();
    osName = GetWindowsVersion() + " " + GetWindowsSystemArchitecture();

    std::string configFilename = "";
    const std::wstring configOption = L"--config=";

    std::string controlsConfigFilename = "";
    const std::wstring controlsOption = L"--controlconfig=";

    LogManager::Init();

    // On Win32 it makes more sense to initialize the system directories here
    // because the next place it was called was in the EmuThread, and it's too late by then.
    InitSysDirectories();

    // Load config up here, because those changes below would be overwritten
    // if it's not loaded here first.
    g_Config.AddSearchPath("");
    g_Config.AddSearchPath(GetSysDirectory(DIRECTORY_SYSTEM));
    g_Config.SetDefaultPath(GetSysDirectory(DIRECTORY_SYSTEM));
    g_Config.Load(configFilename.c_str(), controlsConfigFilename.c_str());

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

    MainWindow::Show(NULL);

    //initialize custom controls
    CtrlDisAsmView::init();
    CtrlMemView::init();
    CtrlRegisterList::init();
    CGEDebugger::Init();

    // Emu thread (and render thread, if any) is always running!
    // Only OpenGL uses an externally managed render thread (due to GL's single-threaded context design). Vulkan
    // manages its own render thread.
    MainThread_Start(g_Config.iGPUBackend == (int)GPUBackend::OPENGL);
    InputDevice::BeginPolling();

	while (GetUIState() != UISTATE_INGAME) {
		Sleep(200);
    }

    return 0;
}

static std::mutex s_lock_mutex;

static std::condition_variable l_lock_condition;

using namespace std::chrono_literals;

extern "C" void __stdcall Save(LPSTR a_filename)
{
    std::unique_lock<std::mutex> l_lock(s_lock_mutex);

    SaveState::Save(a_filename, [](SaveState::Status status, const std::string &message, void *) {
        if (!message.empty()) {
        }

		l_lock_condition.notify_one();
    });

	l_lock_condition.wait_for(l_lock, 50 * 100ms);
}

extern "C" void __stdcall Load(LPSTR a_filename)
{
    std::unique_lock<std::mutex> l_lock(s_lock_mutex);

    SaveState::Load(a_filename, [](SaveState::Status status, const std::string &message, void *) {
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

	g_ICaptureProcessor->Release();
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

extern "C" void* __stdcall GetAudioCaptureProcessor()
{
    return g_ICaptureProcessor;
}


