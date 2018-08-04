/*  PCSX2Lib - Kernel of PCSX2 PS2 Emulator for PCs
*
*  PCSX2Lib is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  PCSX2Lib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with PCSX2Lib.
*  If not, see <http://www.gnu.org/licenses/>.
*/

#pragma once

#include "ExportDef.h"
#include "PS2Edefs.h"
#include "PCSX2Lib_API.h"

typedef s32(CALLBACK *_GScallbackopen)();

extern _GScallbackopen GScallbackopen;

// Plugins connector

PCSX2_EXPORT extern void STDAPICALLTYPE setSPU2(PCSX2Lib::API::SPU2_API* a_API);

PCSX2_EXPORT extern void STDAPICALLTYPE setDEV9(PCSX2Lib::API::DEV9_API* a_API);

PCSX2_EXPORT extern void STDAPICALLTYPE setFW(PCSX2Lib::API::FW_API* a_API);

PCSX2_EXPORT extern void STDAPICALLTYPE setUSB(PCSX2Lib::API::USB_API* a_API);

PCSX2_EXPORT extern void STDAPICALLTYPE setPAD(PCSX2Lib::API::PAD_API* a_API);

PCSX2_EXPORT extern void STDAPICALLTYPE setGS(PCSX2Lib::API::GS_API* a_API);

PCSX2_EXPORT extern void STDAPICALLTYPE setCDVD(PCSX2Lib::API::CDVD_API* aPtrCDVD);



PCSX2_EXPORT extern void* STDAPICALLTYPE getFreezeInternalsFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getEmotionMemoryFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getIopMemoryFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getHwRegsFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getIopHwRegsFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getScratchpadFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getVU0memFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getVU1memFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getVU0progFunc(uint32* a_PtrSizeInBytes);

PCSX2_EXPORT extern void* STDAPICALLTYPE getVU1progFunc(uint32* a_PtrSizeInBytes);



PCSX2_EXPORT extern void* STDAPICALLTYPE getFreezeOutFunc(uint32* a_PtrSizeInBytes, uint32 a_ModuleID);
PCSX2_EXPORT extern void STDAPICALLTYPE setFreezeInFunc(void* data, int32 a_ModuleID);


PCSX2_EXPORT extern void STDAPICALLTYPE setFreezeInternalsFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setEmotionMemoryFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setIopMemoryFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setHwRegsFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setIopHwRegsFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setScratchpadFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setVU0memFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setVU1memFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setVU0progFunc(void* a_PtrMemory, uint32 a_SizeInBytes);

PCSX2_EXPORT extern void STDAPICALLTYPE setVU1progFunc(void* a_PtrMemory, uint32 a_SizeInBytes);


PCSX2_EXPORT extern void STDAPICALLTYPE ApplySettingsFunc(const wchar_t* a_config);

PCSX2_EXPORT extern void STDAPICALLTYPE VTLB_Alloc_PpmapFinc();

PCSX2_EXPORT extern void STDAPICALLTYPE AllocateCoreStuffsFunc(const wchar_t* a_config);


PCSX2_EXPORT extern void STDAPICALLTYPE DetectCpuAndUserModeFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE PCSX2_Hle_SetElfPathFunc(const char* elfFileName);

PCSX2_EXPORT extern void STDAPICALLTYPE SysThreadBase_ResumeFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE SysThreadBase_SuspendFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE SysThreadBase_ResetFunc();


PCSX2_EXPORT extern void STDAPICALLTYPE vu1Thread_WaitVUFunc();

typedef void(CALLBACK *_Callback)();

extern _Callback PluginsInitCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setPluginsInitCallback(_Callback aPluginsInitCallback);

extern _Callback PluginsCloseCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setPluginsCloseCallback(_Callback aPluginsCloseCallback);

extern _Callback PluginsShutdownCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setPluginsShutdownCallback(_Callback aPluginsShutdownCallback);

extern _Callback PluginsOpenCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setPluginsOpenCallback(_Callback aPluginsOpenCallback);

typedef bool(CALLBACK *_BoolCallback)();

extern _BoolCallback PluginsAreLoadedCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setPluginsAreLoadedCallback(_BoolCallback aPluginsAreLoadedCallback);

PCSX2_EXPORT extern void STDAPICALLTYPE resetCallbacksFunc();




PCSX2_EXPORT extern void STDAPICALLTYPE MTGS_ResumeFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE MTGS_WaitForOpenFunc();

PCSX2_EXPORT extern bool STDAPICALLTYPE MTGS_IsSelfFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE MTGS_SuspendFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE MTGS_CancelFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE MTGS_FreezeFunc(int mode, void* data);

PCSX2_EXPORT extern void STDAPICALLTYPE MTGS_WaitGSFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE MTVU_CancelFunc();

PCSX2_EXPORT extern bool STDAPICALLTYPE openPlugin_SPU2Func();

PCSX2_EXPORT extern void STDAPICALLTYPE openPlugin_DEV9Func();

PCSX2_EXPORT extern void STDAPICALLTYPE openPlugin_USBFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE openPlugin_FWFunc();



PCSX2_EXPORT extern void STDAPICALLTYPE ForgetLoadedPatchesFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE inifile_commandFunc(const wchar_t* cmd);



extern _Callback UI_EnableSysActionsCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setUI_EnableSysActionsCallback(_Callback aUI_EnableSysActionsCallback);




typedef void(CALLBACK *_CallbackOneUInt)(unsigned int);

extern _CallbackOneUInt LoadAllPatchesAndStuffCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setLoadAllPatchesAndStuffCallback(_CallbackOneUInt aLoadAllPatchesAndStuffCallback);

PCSX2_EXPORT extern void STDAPICALLTYPE setSioSetGameSerialFunc(const wchar_t* serial);

PCSX2_EXPORT extern bool STDAPICALLTYPE getGameStartedFunc();

PCSX2_EXPORT extern bool STDAPICALLTYPE getGameLoadingFunc();

PCSX2_EXPORT extern unsigned int STDAPICALLTYPE getElfCRCFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE releaseWCHARStringFunc(wchar_t* aPtrString);

PCSX2_EXPORT extern void STDAPICALLTYPE getSysGetBiosDiscIDFunc(wchar_t** aPtrPtrSysGetBiosDiscID);

PCSX2_EXPORT extern void STDAPICALLTYPE setMcd(void* aVoidMcd);

PCSX2_EXPORT extern void STDAPICALLTYPE gsUpdateFrequencyCallFunc();

PCSX2_EXPORT extern void STDAPICALLTYPE getSysGetDiscIDFunc(wchar_t** aPtrPtrSysGetDiscID);



typedef void(CALLBACK *_CallbackOneUINT8PtrOneUINT)(u8*, s32);

extern _CallbackOneUINT8PtrOneUINT LoadBIOSCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setLoadBIOSCallbackCallback(_CallbackOneUINT8PtrOneUINT aLoadBIOSCallback);



typedef void(CALLBACK *_CDVDNVMCallback)(u8*, s32, s32, bool);

extern _CDVDNVMCallback CDVDNVMCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setCDVDNVMCallback(_CDVDNVMCallback aCDVDNVMCallback);



typedef void(CALLBACK *_CallbackOneUINT8Ptr)(u8*);

extern _CallbackOneUINT8Ptr CDVDGetMechaVerCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setCDVDGetMechaVerCallback(_CallbackOneUINT8Ptr aCDVDGetMechaVerCallback);




typedef s32(CALLBACK *_DoFreezeCallback)(void*, s32, s32);

extern _DoFreezeCallback DoFreezeCallback;

PCSX2_EXPORT extern void STDAPICALLTYPE setDoFreezeCallback(_DoFreezeCallback aDoFreezeCallback);




