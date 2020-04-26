using Omega_Red.Managers;
using Omega_Red.Util.Interop;
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
    internal class DirectInputDeviceInfo
    {

    }

    internal interface IDirectInput8
    {
        IList<DirectInputDeviceInfo> enumGamePads();
    }

    class DirectInput8Native
    {
        [ComImport, Guid("BF798031-483A-4DA2-AA99-5D64ED369700"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDirectInput8
        {
        }

        [ComImport, Guid("54D41081-DC15-4833-A41B-748F73A38179"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDirectInputDevice8
        {
        }
        
        private sealed class DirectInput8 : IDisposable, Omega_Red.Util.IDirectInput8
        {
            private const uint DIEDFL_ATTACHEDONLY = 0x00000001;

            [StructLayout(LayoutKind.Sequential)]
            struct DIDEVICEINSTANCE
            {
                public UInt32 dwSize;
                public Guid guidInstance;
                public Guid guidProduct;
                public UInt32 dwDevType;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
                public Char[] tszInstanceName;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
                public Char[] tszProductName;
                public Guid guidFFDriver;
                public UInt16 wUsagePage;
                public UInt16 wUsage;
            }
            
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int CreateDevice(IDirectInput8 a_resource, [In] ref Guid a_guidInstance, [MarshalAs(UnmanagedType.IUnknown)]object a_unknown);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate uint DIEnumDevicesCallback(DIDEVICEINSTANCE a_pddi, IntPtr pvRef);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int EnumDevices(
                    IDirectInput8 a_resource, 
                    UInt32 a_devType, 
                    DIEnumDevicesCallback lpCallback,
                    IntPtr pvRef,
                    UInt32 dwFlags);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int GetDeviceStatus(IDirectInput8 a_resource, [In] ref Guid a_guidInstance);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int RunControlPanel(IDirectInput8 a_resource, IntPtr a_hwnd, UInt32 a_reserved);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int Initialize(IDirectInput8 a_resource, IntPtr a_hinst, uint a_version);

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int FindDevice(IDirectInput8 a_resource, [In] ref Guid a_guidClass, [MarshalAs(UnmanagedType.LPWStr)] string a_deviceName, [Out] out Guid a_guidInstance);
            
            ///*** IDirectInput8W methods ***/
            //STDMETHOD(CreateDevice)(THIS_ REFGUID, LPDIRECTINPUTDEVICE8W*, LPUNKNOWN) PURE;
            //STDMETHOD(EnumDevices)(THIS_ DWORD, LPDIENUMDEVICESCALLBACKW, LPVOID, DWORD) PURE;
            //STDMETHOD(GetDeviceStatus)(THIS_ REFGUID) PURE;
            //STDMETHOD(RunControlPanel)(THIS_ HWND, DWORD) PURE;
            //STDMETHOD(Initialize)(THIS_ HINSTANCE, DWORD) PURE;
            //STDMETHOD(FindDevice)(THIS_ REFGUID, LPCWSTR, LPGUID) PURE;
            //STDMETHOD(EnumDevicesBySemantics)(THIS_ LPCWSTR, LPDIACTIONFORMATW, LPDIENUMDEVICESBYSEMANTICSCBW, LPVOID, DWORD) PURE;
            //STDMETHOD(ConfigureDevices)(THIS_ LPDICONFIGUREDEVICESCALLBACK, LPDICONFIGUREDEVICESPARAMSW, DWORD, LPVOID) PURE;




            private DirectInput8Native.IDirectInput8           comObject;
            private CreateDevice            createDevice;
            private EnumDevices             enumDevices;
            private GetDeviceStatus         getDeviceStatus;
            private RunControlPanel         runControlPanel;
            private Initialize              initialize;
            private FindDevice              findDevice;

            internal DirectInput8(DirectInput8Native.IDirectInput8 obj)
            {
                this.comObject = obj;
                ComInterface.GetComMethod(this.comObject, 3, out this.createDevice);
                ComInterface.GetComMethod(this.comObject, 4, out this.enumDevices);
                ComInterface.GetComMethod(this.comObject, 5, out this.getDeviceStatus);
                ComInterface.GetComMethod(this.comObject, 6, out this.runControlPanel);
                ComInterface.GetComMethod(this.comObject, 7, out this.initialize);
                ComInterface.GetComMethod(this.comObject, 8, out this.findDevice);  
            }


            public void Dispose()
            {
                this.Release();
                GC.SuppressFinalize(this);
            }

            private void Release()
            {
                if (this.comObject != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this.comObject);
                    this.comObject = null;
                    this.createDevice = null;
                    this.enumDevices = null;
                    this.getDeviceStatus = null;
                    this.runControlPanel = null;
                    this.initialize = null;
                    this.findDevice = null;
                }
            }

            IList<DirectInputDeviceInfo> m_directInputDeviceInfo = new List<DirectInputDeviceInfo>();

            public IList<DirectInputDeviceInfo> enumGamePads()
            {
                m_directInputDeviceInfo = new List<DirectInputDeviceInfo>();

                IList < DirectInputDeviceInfo > l_directInputDeviceInfo = null;

                int l_hr = -1;

                if (this.enumDevices != null)
                {
                    l_hr = this.enumDevices(this.comObject, DI8DEVCLASS_GAMECTRL, enumDevicesCallback, IntPtr.Zero, DIEDFL_ATTACHEDONLY);
                }
                
                l_directInputDeviceInfo = m_directInputDeviceInfo;

                return l_directInputDeviceInfo;
            }

            private uint enumDevicesCallback(DIDEVICEINSTANCE a_pddi, IntPtr pvRef)
            {
                ManagementObjectCollection collection;
                using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PNPEntity"))
                    collection = searcher.Get();

                foreach (var device in collection)
                {
                    var f = (string)device.GetPropertyValue("DeviceID");

                    var ghf = (string)device.GetPropertyValue("PNPDeviceID");

                    var lDescription = (string)device.GetPropertyValue("PNPDeviceID");

                }



                return 0;
            }
        }



        private const uint DIRECTINPUT_LATEST_VERSION = 0x0800;

        private const uint DI8DEVCLASS_GAMECTRL = 4;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FirstDelegate(
            IntPtr a_hinst,
            uint a_version, //C++ DWORD = C++ unsigned long = C# unsigned int
            [In] ref Guid a_riidltf,
            out IDirectInput8 a_IDirectInput8,
            [MarshalAs(UnmanagedType.IUnknown)]object a_unknown);


        // Init

        class DirectInput8Func
        {
            public FirstDelegate DirectInput8Create;
        }

        private DirectInput8Func m_DirectInput8Func = new DirectInput8Func();

        private LibLoader m_LibLoader = null;
        
        private bool m_IsInitialized = false;

        private static DirectInput8Native m_Instance = null;

        public static DirectInput8Native Instance { get { if (m_Instance == null) m_Instance = new DirectInput8Native(); return m_Instance; } }

        private DirectInput8Native()
        {
            try
            {
                var l_ModuleBeforeTitle = App.Current.Resources["ModuleBeforeTitle"];

                var l_ModuleAfterTitle = App.Current.Resources["ModuleAfterTitle"];

                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + "DirectInput8"
                    + l_ModuleAfterTitle);

                do
                {
                    m_LibLoader = LibLoader.create("dinput8.dll", true);

                    if (m_LibLoader == null)
                        break;

                    if (!m_LibLoader.isLoaded)
                        break;

                    reflectFunctions(m_DirectInput8Func);

                    m_IsInitialized = true;

                } while (false);

                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + "DirectInput8 "
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

        internal int DirectInput8Create(out Omega_Red.Util.IDirectInput8 a_directInput8)
        {
            a_directInput8 = null;

            if (!m_IsInitialized)
                return -1;

            int l_hr = -1;

            var l_GUID = typeof(DirectInput8Native.IDirectInput8).GUID;

            DirectInput8Native.IDirectInput8 l_IDirectInput8 = null;

            IntPtr hInstance = Marshal.GetHINSTANCE(GetType().Module);

            if (m_DirectInput8Func.DirectInput8Create != null)
                l_hr = m_DirectInput8Func.DirectInput8Create(
                    hInstance,
                    DIRECTINPUT_LATEST_VERSION,
                    ref l_GUID,
                    out l_IDirectInput8,
                    null);

            if(l_hr != 0)
                return -1;

            if (l_IDirectInput8 == null)
                return -1;

            a_directInput8 = new DirectInput8(l_IDirectInput8);

            return 0;
        }
    }
}
