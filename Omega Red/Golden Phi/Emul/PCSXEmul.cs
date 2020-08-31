using Golden_Phi.Managers;
using Golden_Phi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Golden_Phi.Emul
{
    class PCSXEmul : IEmul
    {
        private string m_current_iso_file = "";

        private string m_current_bios_file = "";

        private Assembly m_PCSX2EmulAssembly = null;

        private object m_InstanceObj = null;

        private MethodInfo m_Start = null;

        private MethodInfo m_Pause = null;

        private MethodInfo m_Resume = null;

        private MethodInfo m_Stop = null;

        private MethodInfo m_SetLimitFrame = null;

        private MethodInfo m_LoadState = null;

        private MethodInfo m_SaveState = null;

        private MethodInfo m_SetAudioVolume = null;






        private bool m_restart = false;

        public GameType GameType => GameType.PSP;

        private static PCSXEmul m_Instance = null;

        public static PCSXEmul Instance { get { if (m_Instance == null) m_Instance = new PCSXEmul(); return m_Instance; } }

        public string DiscSerial { get; private set; } = "";

        public string BiosCheckSum { get; private set; } = "1";

        private PCSXEmul()
        {

            try
            {
                BiosManager.Instance.IsoInfoUpdated += (a_IsoInfo) => {

                    if (m_restart && a_IsoInfo != null && !string.IsNullOrWhiteSpace(a_IsoInfo.BIOSFile))
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            Emul.Instance.play(a_IsoInfo);
                        });
                    }
                };

                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Golden_Phi.Modules.AnyCPU.PCSXEmul.dll"))
                {
                    if (lStream == null)
                        return;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    m_PCSX2EmulAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (m_PCSX2EmulAssembly != null)
                    {
                        Type l_CaptureType = m_PCSX2EmulAssembly.GetType("PCSXEmul.EmulInstance");

                        if (l_CaptureType != null)
                        {
                            m_Start = l_CaptureType.GetMethod("start");

                            m_Pause = l_CaptureType.GetMethod("pause");

                            m_Resume = l_CaptureType.GetMethod("resume");

                            m_Stop = l_CaptureType.GetMethod("stop");

                            m_SetLimitFrame = l_CaptureType.GetMethod("setLimitFrame");

                            m_LoadState = l_CaptureType.GetMethod("loadState");

                            m_SaveState = l_CaptureType.GetMethod("saveState");

                            m_SetAudioVolume = l_CaptureType.GetMethod("setAudioVolume");
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }

        public EmulStartState start(IsoInfo a_IsoInfo, IntPtr a_SharedHandle)
        {
            EmulStartState l_result = EmulStartState.Failed;

            do
            {
                m_restart = false;

                if (!BiosManager.Instance.checkBios(a_IsoInfo, true))
                {
                    m_restart = true;

                    l_result = EmulStartState.Postpone;

                    break;
                }

                if (m_InstanceObj == null)
                {
                    initInstance();

                    if (m_InstanceObj == null)
                        break;
                }

                if (m_current_iso_file == a_IsoInfo.FilePath && m_current_bios_file == a_IsoInfo.BIOSFile)
                {
                    l_result = resume() ? EmulStartState.OK : EmulStartState.Failed;

                    break;
                }

                if (m_Start == null)
                    break;

                DiscSerial = "";

                BiosCheckSum = "";

                m_current_iso_file = a_IsoInfo.FilePath;

                m_current_bios_file = a_IsoInfo.BIOSFile;
                
                var l_Start_Result = (bool)m_Start.Invoke(m_InstanceObj, new object[] {
                        a_SharedHandle,
                        Tools.PadInput.Instance.TouchPadCallbackHandler,
                        App.CurrentWindowHandler,
                        a_IsoInfo.FilePath,
                        a_IsoInfo.DiscSerial,
                        a_IsoInfo.BIOSFile});

                if (l_Start_Result)
                {
                    DiscSerial = a_IsoInfo.DiscSerial;

                    BiosCheckSum = a_IsoInfo.BiosInfo.CheckSum.ToString("X8");
                }

                l_result = l_Start_Result ? EmulStartState.OK : EmulStartState.Failed;

            } while (false);

            return l_result;
        }

        public bool pause()
        {
            bool l_result = false;

            do
            {
                if (m_InstanceObj == null)
                    break;

                if (m_Pause == null)
                    break;

                l_result = (bool)m_Pause.Invoke(m_InstanceObj, new object[] { });

            } while (false);

            return l_result;
        }

        public bool stop()
        {
            bool l_result = false;

            do
            {
                m_restart = false;

                if (m_InstanceObj == null)
                    break;

                if (m_Stop == null)
                    break;

                l_result = (bool)m_Stop.Invoke(m_InstanceObj, new object[] { });

                m_current_iso_file = "";

                DiscSerial = "";

                BiosCheckSum = "";

                m_InstanceObj = null;

            } while (false);

            return l_result;
        }

        private void initInstance()
        {
            if (m_PCSX2EmulAssembly != null)
            {
                Type l_CaptureType = m_PCSX2EmulAssembly.GetType("PCSXEmul.EmulInstance");

                if (l_CaptureType != null)
                {
                    var lProperites = l_CaptureType.GetProperties();

                    foreach (var item in lProperites)
                    {
                        if (item.Name.Contains("Instance"))
                        {
                            m_InstanceObj = item.GetMethod.Invoke(null, null);

                            break;
                        }
                    }
                }
            }
        }

        private bool resume()
        {
            bool l_result = false;

            do
            {
                if (m_InstanceObj == null)
                    break;

                if (m_Resume == null)
                    break;

                l_result = (bool)m_Resume.Invoke(m_InstanceObj, new object[] { });

            } while (false);

            return l_result;
        }

        public void setLimitFrame(bool a_state)
        {
            do
            {
                if (m_InstanceObj == null)
                    break;

                if (m_SetLimitFrame == null)
                    break;

                m_SetLimitFrame.Invoke(m_InstanceObj, new object[] { a_state });

            } while (false);
        }

        public void loadState(SaveStateInfo a_SaveStateInfo)
        {
            do
            {
                if (a_SaveStateInfo == null)
                    break;

                if (m_InstanceObj == null)
                    break;

                if (m_LoadState == null)
                    break;

                m_LoadState.Invoke(m_InstanceObj, new object[] { a_SaveStateInfo.FilePath });

            } while (false);
        }

        public void saveState(SaveStateInfo a_SaveStateInfo, string aDate, double aDurationInSeconds, byte[] aScreenshot)
        {
            do
            {
                if (a_SaveStateInfo == null)
                    break;

                if (m_InstanceObj == null)
                    break;

                if (m_SaveState == null)
                    break;

                m_SaveState.Invoke(m_InstanceObj, new object[] { a_SaveStateInfo.FilePath, aDate, aDurationInSeconds, aScreenshot });

            } while (false);
        }

        public void setAudioVolume(float a_level)
        {
            do
            {
                if (m_InstanceObj == null)
                    break;

                if (m_SetAudioVolume == null)
                    break;

                m_SetAudioVolume.Invoke(m_InstanceObj, new object[] { a_level });

            } while (false);
        }
    }
}
