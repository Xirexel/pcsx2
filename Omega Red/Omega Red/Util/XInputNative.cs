using Omega_Red.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Util
{
    public class XInputNative
    {
        public const UInt32 ERROR_SUCCESS = 0;

        public const UInt32 ERROR_INVALID_FUNCTION = 1;

        public const uint XUSER_MAX_COUNT = 4;


        public const uint XINPUT_DEVTYPE_GAMEPAD = 0x01;

        public const uint XINPUT_DEVSUBTYPE_GAMEPAD = 0x01;

        public const ushort XINPUT_CAPS_FFB_SUPPORTED = 0x0001;

        public const ushort XINPUT_CAPS_WIRELESS = 0x0002;

        public const byte BATTERY_DEVTYPE_GAMEPAD = 0x00;

        public const byte BATTERY_TYPE_DISCONNECTED = 0x00;    // This device is not connected
        public const byte BATTERY_TYPE_WIRED = 0x01;    // Wired device, no battery
        public const byte BATTERY_TYPE_ALKALINE = 0x02;    // Alkaline battery source
        public const byte BATTERY_TYPE_NIMH = 0x03;    // Nickel Metal Hydride battery source
        public const byte BATTERY_TYPE_UNKNOWN = 0xFF;    // Cannot determine the battery type


        // These are only valid for wireless, connected devices, with known battery types
        // The amount of use time remaining depends on the type of device.
        public const byte BATTERY_LEVEL_EMPTY = 0x00;
        public const byte BATTERY_LEVEL_LOW = 0x01;
        public const byte BATTERY_LEVEL_MEDIUM = 0x02;
        public const byte BATTERY_LEVEL_FULL = 0x03;

        
        public const ushort XINPUT_GAMEPAD_DPAD_UP = 0x0001;
        public const ushort XINPUT_GAMEPAD_DPAD_DOWN = 0x0002;
        public const ushort XINPUT_GAMEPAD_DPAD_LEFT = 0x0004;
        public const ushort XINPUT_GAMEPAD_DPAD_RIGHT = 0x0008;
        public const ushort XINPUT_GAMEPAD_START = 0x0010;
        public const ushort XINPUT_GAMEPAD_BACK = 0x0020;
        public const ushort XINPUT_GAMEPAD_LEFT_THUMB = 0x0040;
        public const ushort XINPUT_GAMEPAD_RIGHT_THUMB = 0x0080;
        public const ushort XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100;
        public const ushort XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200;
        public const ushort XINPUT_GAMEPAD_A = 0x1000;
        public const ushort XINPUT_GAMEPAD_B = 0x2000;
        public const ushort XINPUT_GAMEPAD_X = 0x4000;
        public const ushort XINPUT_GAMEPAD_Y = 0x8000;


        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_GAMEPAD
        {
            public UInt16 wButtons;
            public Byte bLeftTrigger;
            public Byte bRightTrigger;
            public Int16 sThumbLX;
            public Int16 sThumbLY;
            public Int16 sThumbRX;
            public Int16 sThumbRY;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_STATE
        {
            public UInt32 dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_VIBRATION
        {
            public UInt16 wLeftMotorSpeed;
            public UInt16 wRightMotorSpeed;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_CAPABILITIES
        {
            public Byte Type;
            public Byte SubType;
            public UInt16 Flags;
            public XINPUT_GAMEPAD Gamepad;
            public XINPUT_VIBRATION Vibration;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_BATTERY_INFORMATION
        {
            public Byte BatteryType;
            public Byte BatteryLevel;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_KEYSTROKE
        {
            public UInt16 VirtualKey;
            public Char Unicode;
            public UInt16 Flags;
            public Byte UserIndex;
            public Byte HidCode;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void FirstDelegate(Int32 a_enable);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 SecondDelegate(UInt32 a_deviceIndex, out XINPUT_STATE a_state);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 ThirdDelegate(UInt32 a_deviceIndex, ref XINPUT_VIBRATION a_vibration);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 FourthDelegate(UInt32 a_deviceIndex, UInt32 a_flags, out XINPUT_CAPABILITIES a_capabilities);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 FifthDelegate
        (
            UInt32 a_deviceIndex,        // Index of the gamer associated with the device
            IntPtr a_renderDeviceId,    // Windows Core Audio device ID string for render (speakers)
            IntPtr a_renderCount,       // Size of render device ID string buffer (in wide-chars)
            IntPtr a_captureDeviceId,   // Windows Core Audio device ID string for capture (microphone)
            IntPtr a_captureCount       // Size of capture device ID string buffer (in wide-chars)
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 SixthDelegate(UInt32 a_deviceIndex, Byte a_devType, out XINPUT_BATTERY_INFORMATION a_batteryInformation);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 SeventhDelegate(UInt32 a_deviceIndex, UInt32 a_reserved, out XINPUT_KEYSTROKE a_keystroke);


        // Init

        class XInputFunc
        {
            public FirstDelegate XInputEnable;

            public SecondDelegate XInputGetState;

            public ThirdDelegate XInputSetState;

            public FourthDelegate XInputGetCapabilities;

            public FifthDelegate XInputGetAudioDeviceIds;

            public SixthDelegate XInputGetBatteryInformation;

            public SeventhDelegate XInputGetKeystroke;
        }

        private bool m_IsInitialized = false;

        private XInputFunc m_XInputFunc = new XInputFunc();

        private LibLoader m_LibLoader = null;

        private static XInputNative m_Instance = null;

        public static XInputNative Instance { get { if (m_Instance == null) m_Instance = new XInputNative(); return m_Instance; } }

        private XInputNative()
        {
            try
            {
                var l_ModuleBeforeTitle = App.Current.Resources["ModuleBeforeTitle"];

                var l_ModuleAfterTitle = App.Current.Resources["ModuleAfterTitle"];

                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + "XInput"
                    + l_ModuleAfterTitle);

                do
                {
                    m_LibLoader = LibLoader.create("XInput1_4.dll", true);

                    if (m_LibLoader == null)
                        break;

                    if (!m_LibLoader.isLoaded)
                        break;

                    reflectFunctions(m_XInputFunc);

                    m_IsInitialized = true;

                } while (false);

                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + "XInput "
                    + (m_IsInitialized ? App.Current.Resources["ModuleIsLoadedTitle"] : App.Current.Resources["ModuleIsNotLoadedTitle"]));

            }
            catch (Exception)
            {
            }
        }

        public bool isInit { get { return m_IsInitialized; } }

        private void parseFunction(FieldInfo a_FieldInfo, object a_api)
        {
            if (!m_LibLoader.isLoaded)
                return;

            if (a_FieldInfo == null)
                return;

            var fd = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(m_LibLoader.getFunc(a_FieldInfo.Name), a_FieldInfo.FieldType);

            a_FieldInfo.SetValue(a_api, fd);
        }

        private void reflectFunctions<T>(T a_Instance)
        {
            Type type = a_Instance.GetType();

            foreach (var item in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                parseFunction(item, a_Instance);
            }
        }


        public int XInputEnable(Int32 a_enable)
        {
            if (!m_IsInitialized)
                return -1;

            if (m_XInputFunc.XInputEnable != null)
                m_XInputFunc.XInputEnable(a_enable);

            return -1;
        }

        public UInt32 XInputGetState(UInt32 a_deviceIndex, ref XINPUT_STATE a_state)
        {
            if (!m_IsInitialized)
                return ERROR_INVALID_FUNCTION;

            if (m_XInputFunc.XInputEnable != null)
                return m_XInputFunc.XInputGetState(a_deviceIndex, out a_state);

            return ERROR_INVALID_FUNCTION;
        }

        public UInt32 XInputSetState(UInt32 a_deviceIndex, ref XINPUT_VIBRATION a_vibration)
        {
            if (!m_IsInitialized)
                return ERROR_INVALID_FUNCTION;

            if (m_XInputFunc.XInputEnable != null)
                return m_XInputFunc.XInputSetState(a_deviceIndex, ref a_vibration);

            return ERROR_INVALID_FUNCTION;
        }

        public UInt32 XInputGetCapabilities(UInt32 a_deviceIndex, UInt32 a_flags, ref XINPUT_CAPABILITIES a_capabilities)
        {
            if (!m_IsInitialized)
                return ERROR_INVALID_FUNCTION;

            if (m_XInputFunc.XInputEnable != null)
                return m_XInputFunc.XInputGetCapabilities(a_deviceIndex, a_flags, out a_capabilities);

            return ERROR_INVALID_FUNCTION;
        }

        public UInt32 XInputGetAudioDeviceIds(
            UInt32 a_deviceIndex,        // Index of the gamer associated with the device
            IntPtr a_renderDeviceId,    // Windows Core Audio device ID string for render (speakers)
            IntPtr a_renderCount,       // Size of render device ID string buffer (in wide-chars)
            IntPtr a_captureDeviceId,   // Windows Core Audio device ID string for capture (microphone)
            IntPtr a_captureCount       // Size of capture device ID string buffer (in wide-chars))
            )
        {
            if (!m_IsInitialized)
                return ERROR_INVALID_FUNCTION;

            if (m_XInputFunc.XInputEnable != null)
                return m_XInputFunc.XInputGetAudioDeviceIds(
                    a_deviceIndex, 
                    a_renderDeviceId,    // Windows Core Audio device ID string for render (speakers)
                    a_renderCount,       // Size of render device ID string buffer (in wide-chars)
                    a_captureDeviceId,   // Windows Core Audio device ID string for capture (microphone)
                    a_captureCount);

            return ERROR_INVALID_FUNCTION;
        }

        public UInt32 XInputGetBatteryInformation(UInt32 a_deviceIndex, Byte a_devType, ref XINPUT_BATTERY_INFORMATION a_batteryInformation)
        {
            if (!m_IsInitialized)
                return ERROR_INVALID_FUNCTION;

            if (m_XInputFunc.XInputEnable != null)
                return m_XInputFunc.XInputGetBatteryInformation(a_deviceIndex, a_devType, out a_batteryInformation);

            return ERROR_INVALID_FUNCTION;
        }

        public UInt32 XInputGetKeystroke(UInt32 a_deviceIndex, UInt32 a_reserved, ref XINPUT_KEYSTROKE a_keystroke)
        {
            if (!m_IsInitialized)
                return ERROR_INVALID_FUNCTION;

            if (m_XInputFunc.XInputEnable != null)
                return m_XInputFunc.XInputGetKeystroke(a_deviceIndex, a_reserved, out a_keystroke);

            return ERROR_INVALID_FUNCTION;
        }
    }
}
