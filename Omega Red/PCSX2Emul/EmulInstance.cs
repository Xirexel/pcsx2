using PCSX2Emul.Tools;
using PCSX2Emul.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace PCSX2Emul
{
    public class EmulInstance
    {
        public string Zone { get; private set; }

        public int VersionInt { get; private set; }

        public uint CheckSum { get; private set; }

        public string DiscSerial { get; private set; } = "";

        public bool DisableWideScreen { get; set; } = true;

        public uint ElfCRC { get; private set; } = 0;

        public byte[] Version = null;

        public byte[] NvmLayout = null;
                
        public Pcsx2Config m_Pcsx2Config = new Pcsx2Config();

        private Action m_updateAction = null;

        private bool m_is_paused = false;

        internal static EmulInstance InternalInstance = null;

        public static EmulInstance Instance { get { InternalInstance = new EmulInstance(); return InternalInstance; } }

        private EmulInstance()
        { }

        AutoResetEvent m_BlockEvent = new AutoResetEvent(false);

        public bool start(
            IntPtr a_VideoPanelHandler, 
            IntPtr a_TouchPadCallbackHandler,
            IntPtr a_VideoTargetHandler,
            IntPtr a_AudioCaptureTargetHandler,
            string a_iso_file,
            string a_discSerial,
            uint a_ElfCRC,
            string a_bios_file,
            string a_zone,
            int a_versionInt,
            uint a_checkSum,
            byte[] version,
            byte[] nvmLayout,
            Action a_updateAction)
        {
            var l_result = false;
            
            do
            {
                m_BlockEvent.Reset();

                ModuleControl.Instance.setVideoPanelHandler(a_VideoPanelHandler);

                ModuleControl.Instance.setTouchPadCallbackHandler(a_TouchPadCallbackHandler);

                ModuleControl.Instance.setVideoTargetHandler(a_VideoTargetHandler);

                ModuleControl.Instance.setAudioCaptureTargetHandler(a_AudioCaptureTargetHandler);

                ModuleControl.Instance.setIsoFile(a_iso_file);

                DiscSerial = a_discSerial;

                ElfCRC = a_ElfCRC;

                BiosControl.FilePath = a_bios_file;

                Zone = a_zone;

                VersionInt = a_versionInt;

                CheckSum = a_checkSum;

                Version = version;

                NvmLayout = nvmLayout;

                m_updateAction = a_updateAction;

                if (!PCSX2ModuleManager.Instance.isInit)
                {
                    break;
                }

                if (!PCSX2LibNative.Instance.isInit)
                {
                    break;
                }

                init();

                var l_wideScreen = Tools.Converters.WideScreenConverter.IsWideScreen(ElfCRC.ToString("x")) && !DisableWideScreen;

                ModuleControl.Instance.setVideoAspectRatio(l_wideScreen ? AspectRatio.Ratio_16_9 : AspectRatio.Ratio_4_3);

                //MemoryCardManager.Instance.setMemoryCard();

                PCSX2LibNative.Instance.MTGS_ResumeFunc();

                PCSX2LibNative.Instance.SysThreadBase_ResumeFunc();

                try
                {
                    l_result = m_BlockEvent.WaitOne(TimeSpan.FromSeconds(20));
                }
                catch (Exception)
                {
                    l_result = false;
                }

            } while (false);

            return l_result;
        }

        private bool m_isinitilized = false;

        private void init()
        {
            if (!m_isinitilized)
            {
                ModuleControl.Instance.initPCSX2();

                reset();

                PCSX2LibNative.Instance.DetectCpuAndUserModeFunc();

                PCSX2LibNative.Instance.AllocateCoreStuffsFunc(m_Pcsx2Config.serialize());

                PCSX2LibNative.Instance.PCSX2_Hle_SetElfPathFunc("");

                Bind();

                PatchAndGameFixManager.Instance.loadGameSettings(m_Pcsx2Config, DiscSerial);

                PCSX2LibNative.Instance.ApplySettings(m_Pcsx2Config.serialize());

                m_isinitilized = true;
            }

            if (!PCSX2LibNative.Instance.MTGS_IsSelfFunc())
                PCSX2LibNative.Instance.MTGS_CancelFunc();
        }
        private void Bind()
        {
            foreach (var l_Module in PCSX2ModuleManager.Instance.Modules)
            {
                PCSX2LibNative.Instance.setModule(l_Module);
            }

            PCSX2LibNative.Instance.setPluginsInitCallback = delegate ()
            {
            };

            PCSX2LibNative.Instance.setPluginsCloseCallback = delegate ()
            {
                ModuleControl.Instance.close();
            };

            PCSX2LibNative.Instance.setPluginsShutdownCallback = delegate ()
            {
                ModuleControl.Instance.shutdownPCSX();
            };

            PCSX2LibNative.Instance.setPluginsOpenCallback = delegate ()
            {
                ModuleControl.Instance.open();
            };

            PCSX2LibNative.Instance.setPluginsAreLoadedCallback = delegate ()
            {
                return ModuleControl.Instance.areLoaded();
            };

            PCSX2LibNative.Instance.setUI_EnableSysActionsCallback = delegate ()
            {
                InternalInstance.UI_EnableSysActionsCallback();
            };

            PCSX2LibNative.Instance.setLoadAllPatchesAndStuffCallback = delegate (uint a_FirstArg)
            {
                PatchAndGameFixManager.Instance.LoadAllPatchesAndStuff();
            };

            PCSX2LibNative.Instance.setLoadBIOSCallbackCallback = delegate (IntPtr a_FirstArg, Int32 a_SecondArg)
            {
                BiosControl.LoadBIOS(a_FirstArg, a_SecondArg);
            };

            PCSX2LibNative.Instance.setCDVDNVMCallback = delegate (IntPtr buffer, Int32 offset, Int32 bytes, Boolean read)
            {
                BiosControl.NVMFile(buffer, offset, bytes, read);
            };

            PCSX2LibNative.Instance.setCDVDGetMechaVerCallback = delegate (IntPtr buffer)
            {
                BiosControl.CDVDGetMechaVer(buffer);
            };

            PCSX2LibNative.Instance.setDoFreezeCallback = delegate (IntPtr a_FirstArg, Int32 a_mode, Int32 a_ModuleCode)
            {
                return ModuleControl.Instance.doFreeze(a_FirstArg, a_mode, a_ModuleCode);
            };
        }

        private void reset()
        {

            m_Pcsx2Config.CdvdVerboseReads = true;
            m_Pcsx2Config.CdvdDumpBlocks = false;
            m_Pcsx2Config.CdvdShareWrite = false;
            m_Pcsx2Config.EnablePatches = true;
            m_Pcsx2Config.EnableCheats = false;
            m_Pcsx2Config.EnableWideScreenPatches = false;// !Settings.Default.DisableWideScreen;
            m_Pcsx2Config.UseBOOT2Injection = false;
            m_Pcsx2Config.BackupSavestate = true;
            m_Pcsx2Config.McdEnableEjection = true;
            m_Pcsx2Config.McdFolderAutoManage = true;
            m_Pcsx2Config.MultitapPort0_Enabled = false;
            m_Pcsx2Config.MultitapPort1_Enabled = false;
            m_Pcsx2Config.ConsoleToStdio = false;
            m_Pcsx2Config.HostFs = false;




            m_Pcsx2Config.Cpu.Recompiler.EnableEE = true;
            m_Pcsx2Config.Cpu.Recompiler.EnableIOP = true;
            m_Pcsx2Config.Cpu.Recompiler.EnableVU0 = true;
            m_Pcsx2Config.Cpu.Recompiler.EnableVU1 = true;
            m_Pcsx2Config.Cpu.Recompiler.UseMicroVU0 = true;
            m_Pcsx2Config.Cpu.Recompiler.UseMicroVU1 = true;
            m_Pcsx2Config.Cpu.Recompiler.vuOverflow = true;
            m_Pcsx2Config.Cpu.Recompiler.vuExtraOverflow = false;
            m_Pcsx2Config.Cpu.Recompiler.vuSignOverflow = false;
            m_Pcsx2Config.Cpu.Recompiler.vuUnderflow = false;
            m_Pcsx2Config.Cpu.Recompiler.fpuOverflow = true;
            m_Pcsx2Config.Cpu.Recompiler.fpuExtraOverflow = false;
            m_Pcsx2Config.Cpu.Recompiler.fpuFullMode = false;
            m_Pcsx2Config.Cpu.Recompiler.StackFrameChecks = false;
            m_Pcsx2Config.Cpu.Recompiler.PreBlockCheckEE = false;
            m_Pcsx2Config.Cpu.Recompiler.PreBlockCheckIOP = false;
            m_Pcsx2Config.Cpu.Recompiler.EnableEECache = false;


            m_Pcsx2Config.Cpu.sseMXCSR.InvalidOpFlag = false;
            m_Pcsx2Config.Cpu.sseMXCSR.DenormalFlag = false;
            m_Pcsx2Config.Cpu.sseMXCSR.DivideByZeroFlag = false;
            m_Pcsx2Config.Cpu.sseMXCSR.OverflowFlag = false;
            m_Pcsx2Config.Cpu.sseMXCSR.UnderflowFlag = false;
            m_Pcsx2Config.Cpu.sseMXCSR.PrecisionFlag = false;
            m_Pcsx2Config.Cpu.sseMXCSR.DenormalsAreZero = true;
            m_Pcsx2Config.Cpu.sseMXCSR.InvalidOpMask = true;
            m_Pcsx2Config.Cpu.sseMXCSR.DenormalMask = true;
            m_Pcsx2Config.Cpu.sseMXCSR.DivideByZeroMask = true;
            m_Pcsx2Config.Cpu.sseMXCSR.OverflowMask = true;
            m_Pcsx2Config.Cpu.sseMXCSR.UnderflowMask = true;
            m_Pcsx2Config.Cpu.sseMXCSR.PrecisionMask = true;
            m_Pcsx2Config.Cpu.sseMXCSR.RoundingControl = 3;
            m_Pcsx2Config.Cpu.sseMXCSR.FlushToZero = true;


            m_Pcsx2Config.Cpu.sseVUMXCSR.InvalidOpFlag = false;
            m_Pcsx2Config.Cpu.sseVUMXCSR.DenormalFlag = false;
            m_Pcsx2Config.Cpu.sseVUMXCSR.DivideByZeroFlag = false;
            m_Pcsx2Config.Cpu.sseVUMXCSR.OverflowFlag = false;
            m_Pcsx2Config.Cpu.sseVUMXCSR.UnderflowFlag = false;
            m_Pcsx2Config.Cpu.sseVUMXCSR.PrecisionFlag = false;
            m_Pcsx2Config.Cpu.sseVUMXCSR.DenormalsAreZero = true;
            m_Pcsx2Config.Cpu.sseVUMXCSR.InvalidOpMask = true;
            m_Pcsx2Config.Cpu.sseVUMXCSR.DenormalMask = true;
            m_Pcsx2Config.Cpu.sseVUMXCSR.DivideByZeroMask = true;
            m_Pcsx2Config.Cpu.sseVUMXCSR.OverflowMask = true;
            m_Pcsx2Config.Cpu.sseVUMXCSR.UnderflowMask = true;
            m_Pcsx2Config.Cpu.sseVUMXCSR.PrecisionMask = true;
            m_Pcsx2Config.Cpu.sseVUMXCSR.RoundingControl = 3;
            m_Pcsx2Config.Cpu.sseVUMXCSR.FlushToZero = true;



            m_Pcsx2Config.GS.SynchronousMTGS = false;
            m_Pcsx2Config.GS.DisableOutput = false;
            m_Pcsx2Config.GS.VsyncQueueSize = 2;
            m_Pcsx2Config.GS.FrameLimitEnable = true;
            m_Pcsx2Config.GS.FrameSkipEnable = false;
            m_Pcsx2Config.GS.VsyncEnable = 0;// Pcsx2Config.GSOptions.VsyncMode.Off;
            m_Pcsx2Config.GS.FramesToDraw = 2;
            m_Pcsx2Config.GS.FramesToSkip = 2;
            m_Pcsx2Config.GS.LimitScalar = 100;
            m_Pcsx2Config.GS.FramerateNTSC = 5994;
            m_Pcsx2Config.GS.FrameratePAL = 5000;



            m_Pcsx2Config.Speedhacks.fastCDVD = false;
            m_Pcsx2Config.Speedhacks.IntcStat = true;
            m_Pcsx2Config.Speedhacks.WaitLoop = true;
            m_Pcsx2Config.Speedhacks.vuFlagHack = true;
            m_Pcsx2Config.Speedhacks.vuThread = false;
            m_Pcsx2Config.Speedhacks.EECycleRate = 0;
            m_Pcsx2Config.Speedhacks.EECycleSkip = 0;


            m_Pcsx2Config.Profiler.Enabled = false;
            m_Pcsx2Config.Profiler.RecBlocks_EE = true;
            m_Pcsx2Config.Profiler.RecBlocks_IOP = true;
            m_Pcsx2Config.Profiler.RecBlocks_VU0 = true;
            m_Pcsx2Config.Profiler.RecBlocks_VU1 = true;


            m_Pcsx2Config.Debugger.ShowDebuggerOnStart = false;
            m_Pcsx2Config.Debugger.AlignMemoryWindowStart = true;

        }


        public bool pause()
        {
            PCSX2LibNative.Instance.SysThreadBase_SuspendFunc();

            PCSX2LibNative.Instance.MTGS_SuspendFunc();

            m_is_paused = true;

            return true;
        }

        public bool resume()
        {
            PCSX2LibNative.Instance.MTGS_ResumeFunc();

            PCSX2LibNative.Instance.SysThreadBase_ResumeFunc();

            m_is_paused = false;

            return true;
        }

        public bool stop()
        {
            var l_result = false;

            do
            {

                Thread.Sleep(1500);

                PCSX2LibNative.Instance.SysThreadBase_CancelFunc();

                Thread.Sleep(1500);

                PCSX2LibNative.Instance.MTVU_CancelFunc();

                PCSX2LibNative.Instance.MTGS_CancelFunc();
                
                ModuleControl.Instance.shutdownPCSX();
                
                PCSX2LibNative.Instance.resetCallbacksFunc();

                PCSX2ModuleManager.release();

                Thread.Sleep(1500);

                PCSX2LibNative.Instance.release();

                l_result = true;

            } while (false);

            return l_result;
        }

        public void setLimitFrame(bool a_state)
        {
            do
            {
                m_Pcsx2Config.GS.FrameLimitEnable = a_state;

                PCSX2LibNative.Instance.ApplySettings(m_Pcsx2Config.serialize());

            } while (false);
        }

        public void loadState(string a_sstate_filepath)
        {
            var l_is_paused = m_is_paused;

            if (!l_is_paused)
                pause();

            if (System.IO.File.Exists(a_sstate_filepath))
            {
                Tools.Savestate.SStates.Instance.Load(a_sstate_filepath);

                PCSX2LibNative.Instance.ForgetLoadedPatchesFunc();

                PatchAndGameFixManager.Instance.loadGameSettings(m_Pcsx2Config, DiscSerial);

                PatchAndGameFixManager.Instance.LoadAllPatchesAndStuff();

                PCSX2LibNative.Instance.ApplySettings(m_Pcsx2Config.serialize());

                var l_wideScreen = Tools.Converters.WideScreenConverter.IsWideScreen(
                    ElfCRC.ToString("x")) && !DisableWideScreen;

                if (l_wideScreen)
                    PatchAndGameFixManager.Instance.LoadPatches(ElfCRC.ToString("x"));

                ModuleControl.Instance.setVideoAspectRatio(l_wideScreen ? AspectRatio.Ratio_16_9 : AspectRatio.Ratio_4_3);

                ModuleControl.Instance.setGameCRC(ElfCRC);
            }


            if (!l_is_paused)
                resume();
        }

        public void saveState(string a_sstate_filepath, string aDate, double aDurationInSeconds, byte[] aScreenshot)
        {
            var l_file_path = Path.GetTempPath() + "_temp";

            Tools.Savestate.SStates.Screenshot = aScreenshot;

            try
            {
                if (System.IO.File.Exists(l_file_path))
                    File.Delete(l_file_path);

                Tools.Savestate.SStates.Instance.Save(l_file_path, aDate, aDurationInSeconds);

                File.Delete(a_sstate_filepath);

                File.Move(l_file_path, a_sstate_filepath);
            }
            catch (Exception)
            {
            }
        }

        public void setAudioVolume(float a_value)
        {
            ModuleControl.Instance.setSoundLevel(a_value);
        }

        public void setMemoryCard(string a_file_path)
        {
            ModuleControl.Instance.setMemoryCard(a_file_path);
        }

        public void setAspectRatio(int a_AspectRatio)
        {
            EmulInstance.InternalInstance.DisableWideScreen = true;

            if (a_AspectRatio == 2)
            {
                EmulInstance.InternalInstance.DisableWideScreen = false;
            }

            reloadPath();
        }

        internal void update()
        {
            if (m_updateAction != null)
                m_updateAction();
        }

        private void UI_EnableSysActionsCallback()
        {
            Thread.Sleep(100);

            InternalInstance.m_BlockEvent.Set();
        }

        private void reloadPath()
        {
            PCSX2LibNative.Instance.ForgetLoadedPatchesFunc();

            m_Pcsx2Config.EnableWideScreenPatches = !EmulInstance.InternalInstance.DisableWideScreen;

            PatchAndGameFixManager.Instance.loadGameSettings(m_Pcsx2Config, DiscSerial);

            PatchAndGameFixManager.Instance.LoadAllPatchesAndStuff();

            PCSX2LibNative.Instance.ApplySettings(m_Pcsx2Config.serialize());

            var l_wideScreen = Tools.Converters.WideScreenConverter.IsWideScreen(
                ElfCRC.ToString("x")) && !EmulInstance.InternalInstance.DisableWideScreen;

            if (l_wideScreen)
                PatchAndGameFixManager.Instance.LoadPatches(ElfCRC.ToString("x"));

            ModuleControl.Instance.setVideoAspectRatio(l_wideScreen ? AspectRatio.Ratio_16_9 : AspectRatio.Ratio_4_3);

            ModuleControl.Instance.setGameCRC(ElfCRC);
        }
    }
}
