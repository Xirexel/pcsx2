/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using Omega_Red.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Omega_Red.Models;
using Omega_Red.Util;
using System.Windows.Threading;
using System.Threading;
using Omega_Red.Managers;
using Omega_Red.Properties;
using System.IO;
using System.Globalization;

namespace Omega_Red.Tools
{    
    public class PCSX2Controller
    {
        public event Action<StatusEnum> ChangeStatusEvent;

        public enum StatusEnum
        {
            NoneInitilized,
            Initilized,
            Stopped,
            Started,
            Paused
        }

        public Pcsx2Config m_Pcsx2Config = new Pcsx2Config();

        private StatusEnum m_Status = StatusEnum.NoneInitilized;

        private Queue<Action> m_ActionQueue = new Queue<Action>();

        private DateTime mCurrentDateTime = DateTime.Now;

        private TimeSpan mGameSessionDuration = new TimeSpan();

        private static PCSX2Controller m_Instance = null;

        public static PCSX2Controller Instance { get { if (m_Instance == null) m_Instance = new PCSX2Controller(); return m_Instance; } }

        private PCSX2Controller() { }

        private void reset()
        {

            m_Pcsx2Config.CdvdVerboseReads = true;
            m_Pcsx2Config.CdvdDumpBlocks = false;
            m_Pcsx2Config.CdvdShareWrite = false;
            m_Pcsx2Config.EnablePatches = true;
            m_Pcsx2Config.EnableCheats = false;
            m_Pcsx2Config.EnableWideScreenPatches = !Settings.Default.DisableWideScreen;
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

        private BiosInfo m_BiosInfo = null;

        public BiosInfo BiosInfo {

            get
            {
                return m_BiosInfo;
            }
            set 
            {

                m_BiosInfo = value as BiosInfo;

                updateInitilize();
            }
        }

        private IsoInfo m_IsoInfo = null;

        public IsoInfo IsoInfo
        {
            get
            {
                return m_IsoInfo;
            }
            set
            {

                m_IsoInfo = value as IsoInfo;

                updateInitilize();
            }
        }

        public StatusEnum Status { get { return m_Status; } }

        public void updateInitilize()
        {
            if (m_Status != StatusEnum.NoneInitilized &&
                m_Status != StatusEnum.Initilized &&
                m_Status != StatusEnum.Stopped)
                return;

            if ((m_BiosInfo != null && m_IsoInfo != null) || (m_IsoInfo != null && m_IsoInfo.GameType == GameType.PSP))
            {
                setStatus(StatusEnum.Initilized);
            }
            else
            {
                setStatus(StatusEnum.NoneInitilized);
            }
        }

        private void setStatus(StatusEnum a_Status)
        {
            m_Status = a_Status;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                if (ChangeStatusEvent != null)
                    ChangeStatusEvent(m_Status);
            });
        }

        internal void setVideoAspectRatio(AspectRatio a_AspectRatio)
        {
            ModuleControl.Instance.setVideoAspectRatio(a_AspectRatio);
        }

        private bool m_isinitilized = false;

        private void init()
        {
            if (m_isinitilized)
                return;

            reset();

            PCSX2LibNative.Instance.DetectCpuAndUserModeFunc();

            PCSX2LibNative.Instance.AllocateCoreStuffsFunc(PCSX2Controller.Instance.m_Pcsx2Config.serialize());

            PCSX2LibNative.Instance.PCSX2_Hle_SetElfPathFunc("");

            Bind();

            m_isinitilized = true;
        }

        public void quickSave()
        {
            if (m_Status == StatusEnum.Started)
            {
                LockScreenManager.Instance.showSaving();

                if (m_IsoInfo.GameType == GameType.PSP)
                {            
                    ThreadStart innerCallQuickSaveStart = new ThreadStart(()=> {  
                        PlayPause();
                        SaveStateManager.Instance.quickSavePPSSPP(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), mGameSessionDuration.TotalSeconds);
                        PlayPause();
                        LockScreenManager.Instance.hide();
                    });

                    Thread innerCallQuickSaveStartThread = new Thread(innerCallQuickSaveStart);

                    innerCallQuickSaveStartThread.Start();
                }
                else
                {

                    ThreadStart innerCallQuickSaveStart = new ThreadStart(()=> {  
                        PlayPause();
                        SaveStateManager.Instance.quickSave(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), mGameSessionDuration.TotalSeconds);
                        PlayPause();
                        LockScreenManager.Instance.hide();
                    });

                    Thread innerCallQuickSaveStartThread = new Thread(innerCallQuickSaveStart);

                    innerCallQuickSaveStartThread.Start();
                }
            }
        }

        public void quickLoad()
        {
            var lSaveState = SaveStateManager.Instance.quickLoad();

            if (lSaveState != null)
                PCSX2Controller.Instance.loadState(lSaveState);
        }

        public void saveState(SaveStateInfo a_SaveStateInfo)
        {
            if (m_Status == StatusEnum.Paused)
            {
                a_SaveStateInfo.Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));

                a_SaveStateInfo.Duration = mGameSessionDuration.ToString(@"dd\.hh\:mm\:ss");

                a_SaveStateInfo.DurationNative = mGameSessionDuration;

                LockScreenManager.Instance.showSaving();

                if (m_IsoInfo.GameType == GameType.PSP)
                {

                    ThreadStart innerCallSaveStart = new ThreadStart(() =>
                    {
                        SaveStateManager.Instance.savePPSSPPState(a_SaveStateInfo, a_SaveStateInfo.Date, a_SaveStateInfo.DurationNative.TotalSeconds);

                        LockScreenManager.Instance.hide();
                    });

                    Thread innerCallSaveStartThread = new Thread(innerCallSaveStart);

                    innerCallSaveStartThread.Start();
                }
                else
                {
                    ThreadStart innerCallSaveStart = new ThreadStart(() =>
                    {

                        SaveStateManager.Instance.saveState(a_SaveStateInfo, a_SaveStateInfo.Date, a_SaveStateInfo.DurationNative.TotalSeconds);

                        LockScreenManager.Instance.hide();
                    });

                    Thread innerCallSaveStartThread = new Thread(innerCallSaveStart);

                    innerCallSaveStartThread.Start();
                }
            }
        }

        private bool innerCall()
        {
            bool lresult = false;

            if (m_ActionQueue.Count > 0)
            {
                var l_action = m_ActionQueue.Dequeue();

                if (l_action != null)
                    l_action();

                lresult = true;
            }

            return lresult;
        }
                
        SaveStateInfo m_SaveStateInfo = null;

        string m_tempFile = "";
        
        private void innerCallLoadState()
        {
            if (m_Status == StatusEnum.Started)
                PauseInner();

            switch (m_Status)
            {
                case StatusEnum.Started:
                case StatusEnum.Paused:
                    SaveStateManager.Instance.loadState(m_SaveStateInfo);

                    if (m_IsoInfo != null)
                    {
                        PCSX2LibNative.Instance.ForgetLoadedPatchesFunc();

                        PatchAndGameFixManager.Instance.loadGameSettings(PCSX2Controller.Instance.m_Pcsx2Config, m_IsoInfo.DiscSerial);

                        PatchAndGameFixManager.Instance.LoadAllPatchesAndStuff();

                        PCSX2LibNative.Instance.ApplySettings(PCSX2Controller.Instance.m_Pcsx2Config.serialize());

                        var l_wideScreen = Omega_Red.Tools.Converters.WideScreenConverter.IsWideScreen(
                            m_IsoInfo.ElfCRC.ToString("x")) && !Settings.Default.DisableWideScreen;

                        if (l_wideScreen)
                            PatchAndGameFixManager.Instance.LoadPatches(m_IsoInfo.ElfCRC.ToString("x"));
                        
                        ModuleControl.Instance.setVideoAspectRatio(l_wideScreen ? AspectRatio.Ratio_16_9 : AspectRatio.Ratio_4_3);

                        ModuleControl.Instance.setGameCRC(m_IsoInfo.ElfCRC);

                    }

                    break;
                case StatusEnum.Stopped:
                case StatusEnum.Initilized:
                default:
                    break;
            }

            if (m_Status == StatusEnum.Started)
                StartInner();
        }
        
        public void loadState(SaveStateInfo a_SaveStateInfo)
        {
            m_SaveStateInfo = a_SaveStateInfo;

            m_tempFile = a_SaveStateInfo.FilePath + "tempstate";

            switch (a_SaveStateInfo.Type)
            {
                case SaveStateType.PCSX2:
                    {
                        LockScreenManager.Instance.showStarting();

                        if (m_Status == StatusEnum.Initilized ||
                            m_Status == StatusEnum.Stopped ||
                            m_Status == StatusEnum.Paused)
                        {
                            m_ActionQueue.Enqueue(() => {

                                ThreadStart innerCallLoadStateStart = new ThreadStart(innerCallLoadState);

                                Thread innerCallLoadStateThread = new Thread(innerCallLoadStateStart);

                                innerCallLoadStateThread.Start();

                            });

                            PlayPause();

                        }
                        else
                        {
                            ThreadStart innerCallLoadStateStart = new ThreadStart(innerCallLoadState);

                            Thread innerCallLoadStateThread = new Thread(innerCallLoadStateStart);

                            innerCallLoadStateThread.Start();
                        }
                    }
                    break;
                case SaveStateType.PPSSPP:
                    {
                        LockScreenManager.Instance.showStarting();

                        SaveStateManager.Instance.loadPPSSPPState(m_SaveStateInfo, m_tempFile);

                        if (m_Status == StatusEnum.Paused)
                        {
                            PlayPause();

                            PPSSPPControl.Instance.loadState(m_tempFile,
                            () => {

                                Thread.Sleep(500);

                                if (File.Exists(m_tempFile))
                                    File.Delete(m_tempFile);

                                LockScreenManager.Instance.hide();

                                PPSSPPControl.Instance.resumeGame();
                            });
                        }
                        else
                        {                            
                            PlayPause();
                        }
                    }
                    break;
                default:
                    break;
            }

            mGameSessionDuration = m_SaveStateInfo.DurationNative;
        }

        public void PlayPause()
        {
            switch (m_Status)
            {
                case StatusEnum.Stopped:
                case StatusEnum.Initilized:
                case StatusEnum.Paused:
                    LockScreenManager.Instance.showStarting();
                    StartInner();
                    setStatus(StatusEnum.Started);
                    break;
                case StatusEnum.Started:
                    PauseInner();
                    setStatus(StatusEnum.Paused);
                    break;
                default:
                    break;
            }

        }

        AutoResetEvent mBlockEvent = new AutoResetEvent(false);

        public void callStop()
        {
            if (m_Status == StatusEnum.Started)
            {
                LockScreenManager.Instance.show();
                PlayPause();
            }

            if (m_Status == StatusEnum.Paused)
            {
                if (m_IsoInfo.GameType == GameType.PSP)
                {
                    SaveStateManager.Instance.autoPPSSPPSave(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), mGameSessionDuration.TotalSeconds);
                }
                else
                {
                    SaveStateManager.Instance.autoSave(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), mGameSessionDuration.TotalSeconds);
                }
            }

            switch (m_Status)
            {
                case StatusEnum.Paused:
                case StatusEnum.Started:
                    StopInner();
                    setStatus(StatusEnum.Stopped);
                    break;
                case StatusEnum.Stopped:
                default:
                    break;
            }

            LockScreenManager.Instance.hide();

            mBlockEvent.Set();
        }
        
        public void Stop(bool aBlock = false)
        {
            if (m_IsoInfo != null && m_IsoInfo.GameType != GameType.PSP)
            {
                ModuleControl.Instance.setMemoryCard();
            }

            ThreadStart callStopStart = new ThreadStart(callStop);

            Thread callStopThread = new Thread(callStopStart);

            callStopThread.Start();

            if (aBlock)
                mBlockEvent.WaitOne();

        }

        private void Bind()
        {
            foreach (var l_Module in ModuleManager.Instance.Modules)
            {
                PCSX2LibNative.Instance.setModule(l_Module);
            }

            PCSX2LibNative.Instance.setPluginsInitCallback = delegate()
            {
                ModuleControl.Instance.init();
            };

            PCSX2LibNative.Instance.setPluginsCloseCallback = delegate()
            {
                ModuleControl.Instance.close();
            };

            PCSX2LibNative.Instance.setPluginsShutdownCallback = delegate()
            {
                //MediaCapture.Instance.stop();

                ModuleControl.Instance.shutdown();
            };

            PCSX2LibNative.Instance.setPluginsOpenCallback = delegate()
            {
                ModuleControl.Instance.open();
            };

            PCSX2LibNative.Instance.setPluginsAreLoadedCallback = delegate()
            {
                return ModuleControl.Instance.areLoaded();
            };
            
            PCSX2LibNative.Instance.setUI_EnableSysActionsCallback = delegate()
            {
                if (!PCSX2Controller.Instance.innerCall())
                    LockScreenManager.Instance.hide();
            };

            PCSX2LibNative.Instance.setLoadAllPatchesAndStuffCallback = delegate(uint a_FirstArg)
            {
                PatchAndGameFixManager.Instance.LoadAllPatchesAndStuff();
            };

            PCSX2LibNative.Instance.setLoadBIOSCallbackCallback = delegate(IntPtr a_FirstArg, Int32 a_SecondArg)
            {
                Omega_Red.Tools.BiosControl.LoadBIOS(a_FirstArg, a_SecondArg);
            };

            PCSX2LibNative.Instance.setCDVDNVMCallback = delegate(IntPtr buffer, Int32 offset, Int32 bytes, Boolean read)
            {
                Omega_Red.Tools.BiosControl.NVMFile(buffer, offset, bytes, read);
            };

            PCSX2LibNative.Instance.setCDVDGetMechaVerCallback = delegate(IntPtr buffer)
            {
                Omega_Red.Tools.BiosControl.CDVDGetMechaVer(buffer);
            };

            PCSX2LibNative.Instance.setDoFreezeCallback = delegate(IntPtr a_FirstArg, Int32 a_mode, Int32 a_ModuleCode)
            {
                return ModuleControl.Instance.doFreeze(a_FirstArg, a_mode, a_ModuleCode);
            };
        }

        private bool m_reloadSettings = true;

        private void reloadSettings()
        {
            reset();

            if (m_IsoInfo != null)
                PatchAndGameFixManager.Instance.loadGameSettings(PCSX2Controller.Instance.m_Pcsx2Config, m_IsoInfo.DiscSerial);

            PCSX2LibNative.Instance.ApplySettings(PCSX2Controller.Instance.m_Pcsx2Config.serialize());
        }

        private void StartInner()
        {
            if(m_IsoInfo != null)
            {
                mCurrentDateTime = DateTime.Now;

                if (m_IsoInfo.GameType == GameType.PSP)
                {
                    if(m_Status == StatusEnum.Paused ||
                        m_Status == StatusEnum.Started)
                    {
                        PPSSPPControl.Instance.resumeGame();

                        LockScreenManager.Instance.hide();
                    }
                    else
                    {
                        PPSSPPControl.Instance.Launch(
                            m_IsoInfo.FilePath + "|--state=" + m_tempFile,
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\PCSX2\",
                        ()=> {
                            LockScreenManager.Instance.hide();

                            Thread.Sleep(2000);

                            if (File.Exists(m_tempFile))
                                File.Delete(m_tempFile);

                            m_tempFile = "";
                        });
                    }
                }
                else
                {
                    init();

                    var l_wideScreen = Omega_Red.Tools.Converters.WideScreenConverter.IsWideScreen(
                        m_IsoInfo.ElfCRC.ToString("x")) && !Settings.Default.DisableWideScreen;

                    ModuleControl.Instance.setVideoAspectRatio(l_wideScreen ? AspectRatio.Ratio_16_9 : AspectRatio.Ratio_4_3);
                    
                    if (m_reloadSettings)
                    {
                        reloadSettings();

                        m_reloadSettings = false;
                    }

                    PCSX2LibNative.Instance.SysThreadBase_ResumeFunc();
                }
            }
        }

        private void PauseInner()
        {
            if (m_IsoInfo != null)
            {

                if (m_IsoInfo.GameType == GameType.PSP)
                {
                    PPSSPPControl.Instance.pauseGame();
                }
                else
                {
                    PCSX2LibNative.Instance.SysThreadBase_SuspendFunc();
                }

                mGameSessionDuration += DateTime.Now - mCurrentDateTime;
            }
        }

        private void StopInner()
        {

            if (m_IsoInfo != null)
            {

                if (m_IsoInfo.GameType == GameType.PSP)
                {
                    PPSSPPControl.Instance.close();
                }
                else
                {
                    PCSX2LibNative.Instance.SysThreadBase_ResetFunc();
                }

                m_reloadSettings = true;

                mGameSessionDuration = new TimeSpan();
            }
        }

        public void setLimitFrame(bool a_state)
        {
            if (m_IsoInfo != null)
            {
                if (m_IsoInfo.GameType == GameType.PSP)
                {
                    PPSSPPControl.Instance.setLimitFrame(a_state);
                }
                else
                {
                    PCSX2Controller.Instance.m_Pcsx2Config.GS.FrameLimitEnable = a_state;

                    PCSX2LibNative.Instance.ApplySettings(PCSX2Controller.Instance.m_Pcsx2Config.serialize());
                }

                m_reloadSettings = true;
            }
        }
    }
}
