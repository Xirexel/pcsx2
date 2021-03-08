package com.evgenypereguda.omegared.PCSX2;
import com.evgenypereguda.omegared.*;
import com.evgenypereguda.omegared.PCSX2.Modules.*;

import java.nio.ByteBuffer;

public final class PCSX2LibNative {

    static {
        System.loadLibrary("PCSX2Lib");
    }

    //PCSX2Init

    private native void registerPCSX2Lib();

    //Определяет тип процессора и поддерживаемые.
    private native void NativeDetectCpuAndUserMode();

    //Устанавливает конфигурацию компиляторов.
    private native void NativeAllocateCoreStuffs(String a_config);

    private native void NativeSetElfPathFunc(String a_config);

    private native void SysThreadBaseResumeFunc();

    private native void NativeMTGSCancelFunc();

    private native void NativeMTGSResumeFunc();

    private native void NativeOpenPluginSPU2Func();

    private native void NativeOpenPluginDEV9Func();

    private native void NativeOpenPluginUSBFunc();

    private native void NativeOpenPluginFWFunc();

    private native void NativeMTGSWaitForOpenFunc();


    //Устанавливает конфигурацию компиляторов.
    private native void NativeApplySettingsFunc(String a_XmlPcsx2Config);

//    //Выделить память по виртуальную таблицу данных
    private native void NativeVTLBAllocPpmapFinc();


    private native void CPUTestFinc();






    private boolean m_IsInitialized = false;

    private static PCSX2LibNative m_Instance = null;

    private PCSX2LibNative(){registerPCSX2Lib();}

    public static PCSX2LibNative getInstance()
    {
        if (m_Instance == null)
            m_Instance = new PCSX2LibNative();

        return m_Instance;
    }




    public void DetectCpuAndUserMode()
    {
        NativeDetectCpuAndUserMode();
    }

    public void AllocateCoreStuffs(String a_XmlPcsx2Config)
    {
        NativeAllocateCoreStuffs(a_XmlPcsx2Config);
    }

    public void PCSX2_Hle_SetElfPath(String a_elf)
    {
        NativeSetElfPathFunc(a_elf);
    }

    public void MTGS_CancelFunc(){NativeMTGSCancelFunc();}

    public void MTGS_ResumeFunc(){NativeMTGSResumeFunc();}


    public void openPlugin_SPU2Func(){NativeOpenPluginSPU2Func();}

    public void openPlugin_DEV9Func(){NativeOpenPluginDEV9Func();}

    public void openPlugin_USBFunc(){NativeOpenPluginUSBFunc();}

    public void openPlugin_FWFunc(){NativeOpenPluginFWFunc();}

    public void MTGS_WaitForOpenFunc(){NativeMTGSWaitForOpenFunc();}


    public void ApplySettings(String a_XmlPcsx2Config)
    {
        NativeApplySettingsFunc(a_XmlPcsx2Config);
    }

    public void VTLB_Alloc_Ppmap()
    {
        NativeVTLBAllocPpmapFinc();
    }





//    class PCSX2Init
//    {
//
//
//        public NinethDelegate ApplySettingsFunc;
//
//        public FirstDelegate VTLB_Alloc_PpmapFinc;
//
//        //Устанавливает конфигурацию компиляторов.
//        public NinethDelegate AllocateCoreStuffsFunc;
//
        //Определяет тип процессора и поддерживаемые.
        private native void SetPluginsInitCallback(String aMethod);

        public native void SetPluginsCloseCallback(String aMethod);

        public native void SetPluginsShutdownCallback(String aMethod);

        public native void SetPluginsOpenCallback(String aMethod);

        public native void SetPluginsAreLoadedCallback(String aMethod);

        public native void SetUIEnableSysActionsCallback(String aMethod);

        public native void SetLoadAllPatchesAndStuffCallback(String aMethod);

        public native void SetLoadBIOSCallbackCallback(String aMethod);

        public native void SetCDVDNVMCallback(String aMethod);

        public native void SetCDVDGetMechaVerCallback(String aMethod);

        public native void SetDoFreezeCallback(String aMethod);





//
//        //Определяет тип процессора и поддерживаемые.
//        public ThirdDelegate setUI_EnableSysActionsCallback;
//
//        //Определяет тип процессора и поддерживаемые.
//        public ThirdDelegate setLoadAllPatchesAndStuffCallback;
//
//        //Определяет тип процессора и поддерживаемые.
//        public ThirdDelegate setLoadBIOSCallbackCallback;
//
//        public FirstDelegate resetCallbacksFunc;
//
//        //Определяет тип процессора и поддерживаемые.
//        public ThirdDelegate setCDVDNVMCallback;
//
//        //Определяет тип процессора и поддерживаемые.
//        public ThirdDelegate setCDVDGetMechaVerCallback;
//
//        public ThirdDelegate setDoFreezeCallback;
//    }

//    class PCSX2Memory
//    {
//        public EleventhDelegate getFreezeInternalsFunc;
//
//        public EleventhDelegate getEmotionMemoryFunc;
//
//        public EleventhDelegate getIopMemoryFunc;
//
//        public EleventhDelegate getHwRegsFunc;
//
//        public EleventhDelegate getIopHwRegsFunc;
//
//        public EleventhDelegate getScratchpadFunc;
//
//        public EleventhDelegate getVU0memFunc;
//
//        public EleventhDelegate getVU1memFunc;
//
//        public EleventhDelegate getVU0progFunc;
//
//        public EleventhDelegate getVU1progFunc;
//
//        public ThirteenthDelegate getFreezeOutFunc;
//
//        public SeventhDelegate setFreezeInFunc;
//
//        public SeventhDelegate setFreezeInternalsFunc;
//
//        public SeventhDelegate setEmotionMemoryFunc;
//
//        public SeventhDelegate setIopMemoryFunc;
//
//        public SeventhDelegate setHwRegsFunc;
//
//        public SeventhDelegate setIopHwRegsFunc;
//
//        public SeventhDelegate setScratchpadFunc;
//
//        public SeventhDelegate setVU0memFunc;
//
//        public SeventhDelegate setVU1memFunc;
//
//        public SeventhDelegate setVU0progFunc;
//
//        public SeventhDelegate setVU1progFunc;
//    }
//
//    class PCSX2System
//    {
//        public ThirdDelegate releaseWCHARStringFunc;
//
//            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
//        public delegate void getSysGetBiosDiscIDFuncDelegate(out IntPtr aPtrPtrSysGetBiosDiscID);
//        public getSysGetBiosDiscIDFuncDelegate getSysGetBiosDiscIDFunc;
//
//            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
//        public delegate void getSysGetDiscIDFuncDelegate(out IntPtr aPtrPtrSysGetDiscID);
//        public getSysGetDiscIDFuncDelegate getSysGetDiscIDFunc;
//
//        public FirstDelegate gsUpdateFrequencyCallFunc;
//
//        public NinethDelegate PCSX2_Hle_SetElfPathFunc;
//
//        public FirstDelegate SysThreadBase_ResumeFunc;
//
//        public FirstDelegate SysThreadBase_SuspendFunc;
//
//        public FirstDelegate SysThreadBase_ResetFunc;
//
//        public FirstDelegate SysThreadBase_CancelFunc;
//
//        public NinethDelegate setSioSetGameSerialFunc;
//
//        public FirstDelegate MTGS_WaitGSFunc;
//
//        public FirstDelegate MTGS_WaitForOpenFunc;
//
//            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
//        public delegate bool MTGS_IsSelfFuncDelegate();
//        public MTGS_IsSelfFuncDelegate MTGS_IsSelfFunc;
//
//        public FirstDelegate MTGS_SuspendFunc;
//
//        public FirstDelegate MTGS_ResumeFunc;
//

//
//            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
//        public delegate void MTGS_FreezeFuncDelegate(Int32 mode, IntPtr data);
//        public MTGS_FreezeFuncDelegate MTGS_FreezeFunc;
//
//        public FirstDelegate MTVU_CancelFunc;
//
//
//
//
//            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
//        public delegate bool getGameStartedFuncDelegate();
//        public getGameStartedFuncDelegate getGameStartedFunc;
//
//            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
//        public delegate bool getGameLoadingFuncDelegate();
//        public getGameLoadingFuncDelegate getGameLoadingFunc;
//
//        public TenthDelegate getElfCRCFunc;
//
//        public FirstDelegate vu1Thread_WaitVUFunc;
//
//        public FirstDelegate ForgetLoadedPatchesFunc;
//
//        public NinethDelegate inifile_commandFunc;
//    }
//
//    class PCSX2Modules
//    {
//        public FifthDelegate openPlugin_SPU2Func;
//        public FirstDelegate openPlugin_DEV9Func;
//        public FirstDelegate openPlugin_USBFunc;
//        public FirstDelegate openPlugin_FWFunc;
    private native void SetSPU2(long a_ptr);
    private native void SetGS(long a_ptr);
    private native void SetDEV9(long a_ptr);
    private native void SetMcd(long a_ptr);
    private native void SetCDVD(long a_ptr);
    private native void SetPAD(long a_ptr);
    private native void SetFW(long a_ptr);
    private native void SetUSB(long a_ptr);





    public void setModule(Module a_Module)
    {
        long l_ptr = a_Module.getPCSX2LibAPI();

        switch (a_Module.ModuleType)
        {
            case AudioRenderer:
                SetSPU2(l_ptr);
                break;
            case VideoRenderer:
                SetGS(l_ptr);
                break;
            case DEV9:
                SetDEV9(l_ptr);
                break;
            case MemoryCard:
                SetMcd(l_ptr);
                break;
            case CDVD:
                SetCDVD(l_ptr);
                break;
            case Pad:
                SetPAD(l_ptr);
                break;
            case FW:
                SetFW(l_ptr);
                break;
            case USB:
                SetUSB(l_ptr);
                break;
            default:
                break;
        }
    }

    private Action m_PluginsInitCallback = null;

    public void setPluginsInitCallback(Action a_PluginsInitCallback)
    {
        m_PluginsInitCallback = a_PluginsInitCallback;

        SetPluginsInitCallback("innerPluginsInitCallback");
    }

    public void innerPluginsInitCallback()
    {
        if(m_PluginsInitCallback != null)
            m_PluginsInitCallback.run();
    }

    private Action m_PluginsCloseCallback = null;

    public void setPluginsCloseCallback(Action a_PluginsCloseCallback)
    {
        m_PluginsCloseCallback = a_PluginsCloseCallback;

        SetPluginsCloseCallback("innerPluginsCloseCallback");
    }

    public void innerPluginsCloseCallback()
    {
        if(m_PluginsCloseCallback != null)
            m_PluginsCloseCallback.run();
    }

    private Action m_PluginsShutdownCallback = null;

    public void setPluginsShutdownCallback(Action a_PluginsShutdownCallback)
    {
        m_PluginsShutdownCallback = a_PluginsShutdownCallback;

        SetPluginsShutdownCallback("innerPluginsShutdownCallback");
    }

    public void innerPluginsShutdownCallback()
    {
        if(m_PluginsShutdownCallback != null)
            m_PluginsShutdownCallback.run();
    }


    private Action m_PluginsOpenCallback = null;

    public void setPluginsOpenCallback(Action a_PluginsOpenCallback)
    {
        m_PluginsOpenCallback = a_PluginsOpenCallback;

        SetPluginsOpenCallback("innerPluginsOpenCallback");
    }

    public void innerPluginsOpenCallback()
    {
        if(m_PluginsOpenCallback != null)
            m_PluginsOpenCallback.run();
    }

    private ActionBoolean m_PluginsAreLoadedCallback= null;

    public void setPluginsAreLoadedCallback(ActionBoolean a_PluginsAreLoadedCallback)
    {
        m_PluginsAreLoadedCallback = a_PluginsAreLoadedCallback;

        SetPluginsAreLoadedCallback("innerPluginsAreLoadedCallback");
    }

    public boolean innerPluginsAreLoadedCallback()
    {
        if(m_PluginsAreLoadedCallback != null)
            return m_PluginsAreLoadedCallback.run();

        return false;
    }

    private Action m_UI_EnableSysActionsCallback= null;

    public void setUI_EnableSysActionsCallback(Action a_UI_EnableSysActionsCallback)
    {
        m_UI_EnableSysActionsCallback = a_UI_EnableSysActionsCallback;

        SetUIEnableSysActionsCallback("innerUIEnableSysActionsCallback");
    }

    public void innerUIEnableSysActionsCallback()
    {
        if(m_UI_EnableSysActionsCallback != null)
            m_UI_EnableSysActionsCallback.run();
    }

    private ActionOneIntArg m_LoadAllPatchesAndStuffCallback = null;

    public void setLoadAllPatchesAndStuffCallback(ActionOneIntArg a_LoadAllPatchesAndStuffCallback)
    {
        m_LoadAllPatchesAndStuffCallback = a_LoadAllPatchesAndStuffCallback;

        SetLoadAllPatchesAndStuffCallback("innerLoadAllPatchesAndStuffCallback");
    }

    public void innerLoadAllPatchesAndStuffCallback(int arg)
    {
        if(m_LoadAllPatchesAndStuffCallback != null)
            m_LoadAllPatchesAndStuffCallback.run(arg);
    }


    ActionOneLongOneIntArg m_LoadBIOSCallback = null;

    public void setLoadBIOSCallback(ActionOneLongOneIntArg a_LoadBIOSCallback)
    {
        m_LoadBIOSCallback = a_LoadBIOSCallback;

        SetLoadBIOSCallbackCallback("innerLoadBIOSCallback");
    }

    public void innerLoadBIOSCallback(long arg1, int arg2)
    {
        if(m_LoadBIOSCallback != null)
            m_LoadBIOSCallback.run(arg1, arg2);
    }

    ActionOneObjectOneIntOneBooleanArg m_CDVDNVMCallback = null;

    public void setCDVDNVMCallback(
            ActionOneObjectOneIntOneBooleanArg a_CDVDNVMCallback
    )
    {
        m_CDVDNVMCallback = a_CDVDNVMCallback;

        SetCDVDNVMCallback("innerCDVDNVMCallback");
    }

    public void innerCDVDNVMCallback(ByteBuffer arg1, int arg2, boolean arg3)
    {

        if(m_CDVDNVMCallback != null)
            m_CDVDNVMCallback.run(arg1, arg2, arg3);
    }

    ActionByteBufferArg m_CDVDGetMechaVerCallback = null;

    public void setCDVDGetMechaVerCallback(ActionByteBufferArg a_CDVDGetMechaVerCallback)
    {
        m_CDVDGetMechaVerCallback = a_CDVDGetMechaVerCallback;

        SetCDVDGetMechaVerCallback("innerCDVDGetMechaVerCallback");
    }

    public void innerCDVDGetMechaVerCallback(ByteBuffer arg)
    {
        if(m_CDVDGetMechaVerCallback != null)
            m_CDVDGetMechaVerCallback.run(arg);
    }

    ActionThreeIntArgBoolean m_DoFreezeCallback = null;

    public void setDoFreezeCallback(ActionThreeIntArgBoolean a_DoFreezeCallback)
    {
        m_DoFreezeCallback = a_DoFreezeCallback;

        SetDoFreezeCallback("innerDoFreezeCallback");
    }

    public void innerDoFreezeCallback(int arg1, int arg2, int arg3)
    {
        if(m_DoFreezeCallback != null)
            m_DoFreezeCallback.run(arg1, arg2, arg3);
    }

    public void SysThreadBase_ResumeFunc()
    {
        SysThreadBaseResumeFunc();
    }




    public void CPU_test()
    {
        CPUTestFinc();
    }


}
