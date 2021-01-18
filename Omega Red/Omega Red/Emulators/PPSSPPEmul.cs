using Omega_Red.Capture;
using Omega_Red.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Emulators
{
    class PPSSPPEmul : IEmul
    {
        private string m_current_iso_file = "";
               
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

        private MethodInfo m_SetMemoryCard = null;






        private bool m_restart = false;
        
        public GameType GameType => GameType.PSP;

        private static PPSSPPEmul m_Instance = null;

        public static PPSSPPEmul Instance { get { if (m_Instance == null) m_Instance = new PPSSPPEmul(); return m_Instance; } }

        public string DiscSerial { get; private set; } = "";

        public string BiosCheckSum { get; private set; } = "1";

        private PPSSPPEmul()
        {

            try
            {
                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.AnyCPU.PPSSPPEmul.dll"))
                {
                    if (lStream == null)
                        return;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    m_PCSX2EmulAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (m_PCSX2EmulAssembly != null)
                    {
                        Type l_CaptureType = m_PCSX2EmulAssembly.GetType("PPSSPPEmul.EmulInstance");

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

                            m_SetMemoryCard = l_CaptureType.GetMethod("setMemoryCard");
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

                if (m_InstanceObj == null)
                {
                    initInstance();

                    if (m_InstanceObj == null)
                        break;
                }

                if (m_current_iso_file == a_IsoInfo.FilePath)
                {
                    l_result = resume() ? EmulStartState.OK : EmulStartState.Failed;

                    break;
                }

                if (m_Start == null)
                    break;

                DiscSerial = "";

                BiosCheckSum = "";

                m_current_iso_file = a_IsoInfo.FilePath;
                                
                var l_Start_Result = (bool)m_Start.Invoke(m_InstanceObj, new object[] {
                        a_IsoInfo.FilePath,
                        a_SharedHandle,
                        Tools.PadInput.Instance.TouchPadCallbackHandler,
                        TargetTexture.Instance.TargetHandler,
                        AudioCaptureTarget.Instance.DataCallbackHandler,
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\"+ App.m_MainFolderName + @"\"});

                if (l_Start_Result)
                {
                    DiscSerial = a_IsoInfo.DiscSerial;
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
                Type l_CaptureType = m_PCSX2EmulAssembly.GetType("PPSSPPEmul.EmulInstance");

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

        public bool resume()
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

        public static IsoInfo getGameDiscInfo(string aFilePath)
        {
            IsoInfo l_IsoInfo = null;

            string l_result = null;

            do
            {

                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.AnyCPU.PPSSPPEmul.dll"))
                {
                    if (lStream == null)
                        break;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    var l_PCSX2EmulAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (l_PCSX2EmulAssembly != null)
                    {
                        Type l_CaptureType = l_PCSX2EmulAssembly.GetType("PPSSPPEmul.EmulInfo");

                        if (l_CaptureType != null)
                        {
                            var l_GetGameDiscInfo = l_CaptureType.GetMethod("getGameDiscInfo");

                            if (l_GetGameDiscInfo != null)
                            {
                                l_result = (string)l_GetGameDiscInfo.Invoke(null, new object[] { aFilePath });
                            }

                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(l_result))
                {
                    var l_splits = l_result.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    if(l_splits != null && l_splits.Length == 2)
                    {
                        l_IsoInfo = new IsoInfo();

                        l_IsoInfo.Title = l_splits[0];

                        l_IsoInfo.GameDiscType = "PSP Disc";

                        l_IsoInfo.GameType = GameType.PSP;

                        l_IsoInfo.DiscSerial = l_splits[1].ToUpper();

                        l_IsoInfo.DiscRegionType = "PAL";

                        l_IsoInfo.SoftwareVersion = "1.0";

                        l_IsoInfo.FilePath = aFilePath;
                    }
                }

            } while (false);

            return l_IsoInfo;
        }
        public void setMemoryCard(string a_file_path)
        {
            do
            {
                if (m_InstanceObj == null)
                    break;

                if (m_SetMemoryCard == null)
                    break;

                m_SetMemoryCard.Invoke(m_InstanceObj, new object[] { a_file_path });

            } while (false);
        }

        public void setVideoAspectRatio(AspectRatio a_AspectRatio)
        {
        }
    }
}
