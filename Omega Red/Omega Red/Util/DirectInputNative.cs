using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Omega_Red.Util
{
    class DirectInputNative
    {
        private DirectInput m_DirectInput = null;

        private bool m_IsInitialized = false;

        private LibLoader m_LibLoader = null;

        private static DirectInputNative m_Instance = null;

        public static DirectInputNative Instance { get { if (m_Instance == null) m_Instance = new DirectInputNative(); return m_Instance; } }

        private DirectInputNative()
        {
            try
            {

                do
                {
                    try
                    {
                        m_DirectInput = new DirectInput();

                        m_DirectInput.EnumDevices(DI8DEVTYPE.GAMEPAD, DIEDFL.ATTACHEDONLY);

                        m_IsInitialized = true;
                    }
                    catch (Exception)
                    {
                    }

                } while (false);
            }
            catch (Exception)
            {
            }

        }
        public bool isInit { get { return m_IsInitialized; } }

        public IList<Tuple<string, Guid>> getGamePadList()
        {
            var l_gamePads = new List<Tuple<string, Guid>>();

            if(m_DirectInput != null)
            {
                var l_devices =  m_DirectInput.EnumDevices(DI8DEVTYPE.GAMEPAD, DIEDFL.ATTACHEDONLY);

                for (int i = 0; i < l_devices.Count; i++)
                {
                    var item = l_devices[i];

                    if (IsXInputDevice(ref item.guidProduct))
                        continue;

                    string l_deviceName = "Unknown";

                    if (!string.IsNullOrWhiteSpace(item.tszInstanceName))
                    {
                        l_deviceName = item.tszInstanceName;
                    }
                    else if (!string.IsNullOrWhiteSpace(item.tszProductName))
                    {
                        l_deviceName = item.tszProductName;
                    }
                    else
                    {
                        l_deviceName = string.Format("Device {0}", i);
                    }
                    
                    l_gamePads.Add(Tuple.Create(l_deviceName, item.guidInstance));
                }
            }

            return l_gamePads;            
        }

        public DirectInputDevice createDevice(Guid a_DeviceGuid)
        {
            if(m_DirectInput != null)
            {
                return m_DirectInput.CreateDevice(a_DeviceGuid);
            }

            return null;
        }

        //-----------------------------------------------------------------------------
        // Enum each PNP device using WMI and check each device ID to see if it contains 
        // "IG_" (ex. "VID_045E&PID_028E&IG_00").  If it does, then it's an XInput device
        // Unfortunately this information can not be found by just using DirectInput 
        //-----------------------------------------------------------------------------
        bool IsXInputDevice( ref Guid pGuidProductFromDirectInput )
        {
            bool bIsXinputDevice = false;

            ManagementObjectSearcher s = new ManagementObjectSearcher(
                                            "root\\CIMV2",
                                            "SELECT * FROM Win32_PNPEntity",
                                            new EnumerationOptions(
                                            null, System.TimeSpan.MaxValue,
                                            1, true, false, true,
                                            true, false, true, true));

            foreach (ManagementObject obj in s.Get())
            {
                var l_DeviceID = obj["DeviceID"].ToString();

                if(!string.IsNullOrWhiteSpace(l_DeviceID))
                {
                    if(l_DeviceID.Contains("IG_"))
                    {
                        uint l_Pid = 0;

                        uint l_Vid = 0;

                        var l_split = l_DeviceID.Split(new string[]{ "VID_"}, StringSplitOptions.RemoveEmptyEntries);

                        if(l_split.Length == 2)
                        {
                            l_split = l_split[1].Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

                            if(l_split.Length >= 1)
                            {
                                uint l_temp = 0;

                                if(uint.TryParse(l_split[0], System.Globalization.NumberStyles.HexNumber, null, out l_temp))
                                {
                                    l_Vid = l_temp;
                                }
                            }
                        }

                        l_split = l_DeviceID.Split(new string[] { "PID_" }, StringSplitOptions.RemoveEmptyEntries);

                        if (l_split.Length == 2)
                        {
                            l_split = l_split[1].Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

                            if (l_split.Length >= 1)
                            {
                                uint l_temp = 0;

                                if (uint.TryParse(l_split[0], System.Globalization.NumberStyles.HexNumber, null, out l_temp))
                                {
                                    l_Pid = l_temp;
                                }
                            }
                        }

                        var l_VidPid = Win32NativeMethods.MAKELONG(l_Vid, l_Pid);

                        var l_strGuid = pGuidProductFromDirectInput.ToString().ToUpper();

                        var l_startId = l_VidPid.ToString("X").ToUpper();

                        if (l_strGuid.StartsWith(l_startId))
                        {
                            bIsXinputDevice = true;

                            break;
                        }
                    }
                }
            }
            
            return bIsXinputDevice;
        }
    }
}
