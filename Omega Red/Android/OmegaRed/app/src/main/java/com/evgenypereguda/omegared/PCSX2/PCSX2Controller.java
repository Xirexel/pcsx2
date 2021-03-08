package com.evgenypereguda.omegared.PCSX2;

import com.evgenypereguda.omegared.*;
import com.evgenypereguda.omegared.Models.BiosInfo;
import com.evgenypereguda.omegared.Models.IsoInfo;
import com.evgenypereguda.omegared.PCSX2.Modules.Module;
import com.evgenypereguda.omegared.PCSX2.Modules.ModuleManager;
import com.evgenypereguda.omegared.PCSX2.Tools.BiosControl;

import java.nio.ByteBuffer;

public final class PCSX2Controller {

    private boolean m_isinitilized = false;

    private boolean m_reloadSettings = true;

    private Pcsx2Config m_Pcsx2Config = new Pcsx2Config();

    private static PCSX2Controller m_Instance = null;

    private PCSX2Controller(){}

    public static PCSX2Controller getInstance()
    {
        if (m_Instance == null)
            m_Instance = new PCSX2Controller();

        return m_Instance;
    }

    private BiosInfo m_BiosInfo = null;

    public BiosInfo getBiosInfo() {
        return m_BiosInfo;
    }

    public void setBiosInfo(BiosInfo a_BiosInfo) {

        m_BiosInfo = a_BiosInfo;

        updateInitilize();
    }




    private IsoInfo m_IsoInfo = null;

    public IsoInfo getIsoInfo() {
        return m_IsoInfo;
    }

    public void setIsoInfo(IsoInfo a_IsoInfo) {

        m_IsoInfo = a_IsoInfo;

        updateInitilize();
    }





    private void updateInitilize()
    {
        if (m_BiosInfo != null && m_IsoInfo != null)
        {
            GameController.getInstance().setStatus(GameController.StatusEnum.Initilized);
        }
        else
        {
            GameController.getInstance().setStatus(GameController.StatusEnum.NoneInitilized);
        }
    }

    public void Start() {

        init();

        if (m_reloadSettings)
        {
            reloadSettings();

            m_reloadSettings = false;
        }

        PCSX2LibNative.getInstance().SysThreadBase_ResumeFunc();
    }

    private void reloadSettings()
    {
        m_Pcsx2Config.reset();

//        if (m_IsoInfo != null)
//            PatchAndGameFixManager.Instance.loadGameSettings(PCSX2Controller.Instance.m_Pcsx2Config, m_IsoInfo.DiscSerial);

        PCSX2LibNative.getInstance().ApplySettings(m_Pcsx2Config.serialize());
    }

    private void init()
    {
        if (m_isinitilized)
            return;

        m_Pcsx2Config.reset();

        PCSX2LibNative.getInstance().DetectCpuAndUserMode();

        PCSX2LibNative.getInstance().AllocateCoreStuffs(m_Pcsx2Config.serialize());

        PCSX2LibNative.getInstance().PCSX2_Hle_SetElfPath("");

        Bind();

        m_isinitilized = true;
    }

    private void Bind() {

        for (Module l_Module : ModuleManager.getInstance().getModules())
        {
            PCSX2LibNative.getInstance().setModule(l_Module);
        }

        PCSX2LibNative.getInstance().setPluginsInitCallback(
                new Action() {
                    @Override
                    public void run() {
                        ModuleControl.getInstance().init();
                    }
                });

        PCSX2LibNative.getInstance().setPluginsCloseCallback(
                new Action() {
                    @Override
                    public void run() {
                        ModuleControl.getInstance().close();
                    }
                }
        );

        PCSX2LibNative.getInstance().setPluginsShutdownCallback(
                new Action() {
                    @Override
                    public void run() {
                        ModuleControl.getInstance().shutdown();
                    }
                }
        );

        PCSX2LibNative.getInstance().setPluginsOpenCallback(
                new Action() {
                    @Override
                    public void run() {
                        ModuleControl.getInstance().open();
                    }
                }
        );

        PCSX2LibNative.getInstance().setPluginsAreLoadedCallback(
                new ActionBoolean() {
                    @Override
                    public boolean run() {
                        return ModuleControl.getInstance().areLoaded();
                    }
                }
        );

        PCSX2LibNative.getInstance().setUI_EnableSysActionsCallback(
                new Action() {
                    @Override
                    public void run() {

                    }
                }
        );

        PCSX2LibNative.getInstance().setLoadAllPatchesAndStuffCallback(
                new ActionOneIntArg() {
                    @Override
                    public void run(int arg) {
                        PatchAndGameFixManager.getInstance().LoadAllPatchesAndStuff();
                    }
                }
        );

        PCSX2LibNative.getInstance().setLoadBIOSCallback(
                new ActionOneLongOneIntArg() {
                    @Override
                    public void run(long arg1, int arg2) {
                      BiosControl.getInstance().LoadBIOS(arg1, arg2);
                    }
                }
        );

        PCSX2LibNative.getInstance().setCDVDNVMCallback(
                new ActionOneObjectOneIntOneBooleanArg() {
                    @Override
                    public void run(ByteBuffer arg1, int arg2, boolean arg3) {
                        BiosControl.getInstance().NVMFile(arg1, arg2, arg3);
                    }
                }
        );

        PCSX2LibNative.getInstance().setCDVDGetMechaVerCallback(
                new ActionByteBufferArg() {
                    @Override
                    public void run(ByteBuffer arg) {
                        BiosControl.getInstance().CDVDGetMechaVer(arg);
                    }
                }
        );

        PCSX2LibNative.getInstance().setDoFreezeCallback(
                new ActionThreeIntArgBoolean() {
                    @Override
                    public boolean run(int arg1, int arg2, int arg3) {
//                        return ModuleControl.Instance.doFreeze(a_FirstArg, a_mode, a_ModuleCode);
                        return false;
                    }
                }

        );
    }

}
