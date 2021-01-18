using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using Golden_Phi.Managers;
using Golden_Phi.Models;
using Golden_Phi.Tools;
using static Golden_Phi.Managers.IsoManager;

namespace Golden_Phi.Emulators
{
    class PCSX2Emul : IEmul
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

        private MethodInfo m_SetMemoryCard = null;

        private MethodInfo m_SetAspectRatio = null;


        


        private bool m_restart = false;
        
        public GameType GameType => GameType.PS2;
        
        private static PCSX2Emul m_Instance = null;

        public static PCSX2Emul Instance { get { if (m_Instance == null) m_Instance = new PCSX2Emul(); return m_Instance; } }

        public string DiscSerial { get; private set; } = "";

        public string BiosCheckSum { get; private set; } = "";

        private PCSX2Emul()
        {

            try
            {
                BiosManager.Instance.IsoInfoUpdated += (a_IsoInfo) => {
                    
                    if(m_restart && a_IsoInfo != null && !string.IsNullOrWhiteSpace(a_IsoInfo.BIOSFile))
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            Emul.Instance.play(a_IsoInfo);
                        });
                    }
                };

                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Golden_Phi.Modules.AnyCPU.PCSX2Emul.dll"))
                {
                    if (lStream == null)
                        return;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    m_PCSX2EmulAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (m_PCSX2EmulAssembly != null)
                    {
                        Type l_CaptureType = m_PCSX2EmulAssembly.GetType("PCSX2Emul.EmulInstance");

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

                            m_SetAspectRatio = l_CaptureType.GetMethod("setAspectRatio");  

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

                if(m_current_iso_file == a_IsoInfo.FilePath && m_current_bios_file == a_IsoInfo.BIOSFile)
                {
                    l_result = resume()? EmulStartState.OK: EmulStartState.Failed;

                    break;
                }

                if (m_Start == null)
                    break;

                DiscSerial = "";

                BiosCheckSum = "";

                m_current_iso_file = a_IsoInfo.FilePath;

                m_current_bios_file = a_IsoInfo.BIOSFile;

                if (a_IsoInfo.BiosInfo.MEC == null ||
                    a_IsoInfo.BiosInfo.MEC.Length < 4)
                {
                    a_IsoInfo.BiosInfo.MEC = new byte[4];

                    byte[] version = { 0x3, 0x6, 0x2, 0x0 };

                    using (MemoryStream l_memoryStream = new MemoryStream(a_IsoInfo.BiosInfo.MEC))
                    {
                        l_memoryStream.Write(version, 0, version.Length);
                    }

                    BiosManager.Instance.save();
                }

                if (a_IsoInfo.BiosInfo.NVM == null ||
                    a_IsoInfo.BiosInfo.NVM.Length < BiosControl.m_nvmSize)
                {
                    a_IsoInfo.BiosInfo.NVM = new byte[BiosControl.m_nvmSize];

                    BiosControl.NVMLayout nvmLayout = BiosControl.getNvmLayout(a_IsoInfo.BiosInfo.VersionInt);

                    byte[] ILinkID_Data = { 0x00, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0xB9, 0x86 };

                    using (MemoryStream l_memoryStream = new MemoryStream(a_IsoInfo.BiosInfo.NVM))
                    {
                        l_memoryStream.Seek(nvmLayout.ilinkId, SeekOrigin.Begin);

                        l_memoryStream.Write(ILinkID_Data, 0, ILinkID_Data.Length);
                    }

                    BiosManager.Instance.save();
                }

                var l_Start_Result = (bool)m_Start.Invoke(m_InstanceObj, new object[] {
                        a_SharedHandle,
                        Tools.PadInput.Instance.TouchPadCallbackHandler,
                        Tools.PadInput.Instance.VibrationCallbackHandler,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        a_IsoInfo.FilePath,
                        a_IsoInfo.DiscSerial,
                        a_IsoInfo.ElfCRC,
                        a_IsoInfo.BiosInfo.FilePath,
                        a_IsoInfo.BiosInfo.Zone,
                        a_IsoInfo.BiosInfo.VersionInt,
                        a_IsoInfo.BiosInfo.CheckSum,
                        a_IsoInfo.BiosInfo.MEC,
                        a_IsoInfo.BiosInfo.NVM,
                        new Action(BiosManager.Instance.save)});

                if(l_Start_Result)
                {
                    DiscSerial = a_IsoInfo.DiscSerial;

                    BiosCheckSum = a_IsoInfo.BiosInfo.CheckSum.ToString("X8");
                }

                l_result = l_Start_Result? EmulStartState.OK: EmulStartState.Failed;

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

                m_current_bios_file = "";

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
                Type l_CaptureType = m_PCSX2EmulAssembly.GetType("PCSX2Emul.EmulInstance");

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
            IsoInfo l_result = null;

            string l_commandResult = "";

            do
            {

                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Golden_Phi.Modules.AnyCPU.PCSX2Emul.dll"))
                {
                    if (lStream == null)
                        break;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    var l_PCSX2EmulAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (l_PCSX2EmulAssembly != null)
                    {
                        Type l_CaptureType = l_PCSX2EmulAssembly.GetType("PCSX2Emul.EmulInfo");

                        if (l_CaptureType != null)
                        {
                            var l_GetGameDiscInfo = l_CaptureType.GetMethod("getGameDiscInfo");

                            if(l_GetGameDiscInfo != null)
                            {
                                l_commandResult = (string)l_GetGameDiscInfo.Invoke(null, new object[] { aFilePath });
                            }

                        }
                    }
                }
                               
                if (!string.IsNullOrEmpty(l_commandResult))
                {
                    XmlDocument l_XmlDocument = new XmlDocument();

                    l_XmlDocument.LoadXml(l_commandResult);

                    if (l_XmlDocument.DocumentElement != null)
                    {

                        var l_bResult = false;

                        var l_isoType = IsoType.ISOTYPE_ILLEGAL;

                        var l_CheckNode = l_XmlDocument.DocumentElement.SelectSingleNode("Result[@Command='Check']");

                        if (l_CheckNode != null)
                        {
                            var l_StateNode = l_CheckNode.SelectSingleNode("@State");

                            var l_IsoTypeNode = l_CheckNode.SelectSingleNode("@IsoType");

                            if (l_StateNode != null)
                            {
                                Boolean.TryParse(l_StateNode.Value, out l_bResult);
                            }

                            if (l_IsoTypeNode != null)
                            {
                                Enum.TryParse<IsoType>(l_IsoTypeNode.Value, true, out l_isoType);
                            }
                        }

                        l_CheckNode = l_XmlDocument.DocumentElement.SelectSingleNode("Result[@Command='GetDiscSerial']");

                        if (l_CheckNode != null
                            && l_bResult
                            && (l_isoType != IsoType.ISOTYPE_ILLEGAL)
                            && (l_isoType != IsoType.ISOTYPE_AUDIO))
                        {
                            IsoInfo local_result = new IsoInfo();

                            local_result.IsoType = l_isoType.ToString().Replace("ISOTYPE_", "");

                            local_result.FilePath = aFilePath;

                            var l_GameDiscType = l_CheckNode.SelectSingleNode("@GameDiscType");

                            var l_DiscSerial = l_CheckNode.SelectSingleNode("@DiscSerial");

                            var l_DiscRegionType = l_CheckNode.SelectSingleNode("@DiscRegionType");

                            var l_SoftwareVersion = l_CheckNode.SelectSingleNode("@SoftwareVersion");

                            var l_ElfCRC = l_CheckNode.SelectSingleNode("@ElfCRC");

                            if (l_GameDiscType == null
                                || l_DiscSerial == null
                                || l_DiscRegionType == null
                                || l_SoftwareVersion == null
                                || l_ElfCRC == null)
                                break;

                            if (l_GameDiscType != null && !string.IsNullOrWhiteSpace(l_GameDiscType.Value))
                            {
                                local_result.GameDiscType = l_GameDiscType.Value;

                                local_result.GameType = GameType.Unknown;

                                if (l_GameDiscType.Value.Contains("PS1"))
                                    local_result.GameType = GameType.PS1;
                                else if (l_GameDiscType.Value.Contains("PS2"))
                                    local_result.GameType = GameType.PS2;
                            }

                            if (l_DiscSerial != null && l_DiscSerial.Value != null)
                            {
                                local_result.DiscSerial = l_DiscSerial.Value.ToUpper();

                                var l_gameData = GameIndex.Instance.convert(local_result.DiscSerial);

                                if (l_gameData != null)
                                {
                                    local_result.Title = l_gameData.FriendlyName;
                                }
                                else
                                {
                                    FileInfo l_FileInfo = new FileInfo(aFilePath);

                                    var l_name = l_FileInfo.Name;

                                    if (!string.IsNullOrWhiteSpace(l_FileInfo.Extension))
                                        l_name = l_name.Replace(l_FileInfo.Extension, "");

                                    local_result.Title = l_name;
                                }
                            }

                            if (l_DiscRegionType != null)
                            {
                                local_result.DiscRegionType = l_DiscRegionType.Value;
                            }

                            if (l_SoftwareVersion != null)
                            {
                                local_result.SoftwareVersion = l_SoftwareVersion.Value;
                            }

                            if (l_ElfCRC != null)
                            {
                                uint l_value = 0;

                                if (uint.TryParse(l_ElfCRC.Value, out l_value))
                                {
                                    local_result.ElfCRC = l_value;
                                }
                            }

                            l_result = local_result;
                        }
                    }
                }

            } while (false);

            return l_result;
        }

        public void setMemoryCard(string a_file_path, int a_slot)
        {
            do
            {
                if (m_InstanceObj == null)
                    break;

                if (m_SetMemoryCard == null)
                    break;

                m_SetMemoryCard.Invoke(m_InstanceObj, new object[] { a_file_path, a_slot });

            } while (false);            
        }

        public void setVideoAspectRatio(AspectRatio a_AspectRatio)
        {
            do
            {
                if (m_InstanceObj == null)
                    break;

                if (m_SetAspectRatio == null)
                    break;

                m_SetAspectRatio.Invoke(m_InstanceObj, new object[] { (int)a_AspectRatio });

            } while (false);
        }
    }
}
