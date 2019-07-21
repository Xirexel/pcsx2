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
    class RTMPNative
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int FirstDelegate([MarshalAs(UnmanagedType.LPStr)]String a_streamsXml, [MarshalAs(UnmanagedType.LPStr)]String a_url);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void SecondDelegate(int handler);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ThirdDelegate(int handler, int sampleTime, IntPtr buf, int size, uint is_keyframe, int streamIdx, int isVideo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int FourthDelegate(int handler);



        private LibLoader m_LibLoader = null;

        private bool m_IsInitialized = false;



        // Init

        class RTMPFunc
        {
            //Соединение с RTMP сервером.
            public FirstDelegate Connect;

            //Отключение от RTMP сервера.
            public SecondDelegate Disconnect;

            //Запись на RTMP сервер.
            public ThirdDelegate Write;

            //Проверка соединения с RTMP сервером.
            public FourthDelegate IsConnected;
        }


        private RTMPFunc m_RTMPFunc = new RTMPFunc();


        private static RTMPNative m_Instance = null;

        public static RTMPNative Instance { get { if (m_Instance == null) m_Instance = new RTMPNative(); return m_Instance; } }

        private RTMPNative()
        {
            try
            {
                var l_ModuleBeforeTitle = App.Current.Resources["ModuleBeforeTitle"];

                var l_ModuleAfterTitle = App.Current.Resources["ModuleAfterTitle"];

                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + "RTMP"
                    + l_ModuleAfterTitle);

                do
                {
                    m_LibLoader = LibLoader.create("RTMP");

                    if (m_LibLoader == null)
                        break;

                    if (!m_LibLoader.isLoaded)
                        break;

                    reflectFunctions(m_RTMPFunc);
                    
                    m_IsInitialized = true;

                } while (false);

                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + "RTMP "
                    + (m_IsInitialized ? App.Current.Resources["ModuleIsLoadedTitle"] : App.Current.Resources["ModuleIsNotLoadedTitle"]));

            }
            catch (Exception)
            {
            }
        }

        public void release()
        {
            if(m_LibLoader != null)
                m_LibLoader.release();
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

        public int Connect(string a_streamsXml, string a_url)
        {
            if (!m_IsInitialized)
                return -1;

            if (m_RTMPFunc.Connect != null)
                return m_RTMPFunc.Connect(a_streamsXml, a_url);

            return -1;
        }

        public void Disconnect(int handler)
        {
            if (!m_IsInitialized)
                return;

            if (m_RTMPFunc.Disconnect != null)
                m_RTMPFunc.Disconnect(handler);
        }

        public void Write(int handler, int sampleTime, IntPtr buf, int size, uint is_keyframe, int streamIdx, int isVideo)
        {
            if (!m_IsInitialized)
                return;

            if (m_RTMPFunc.Write != null)
                m_RTMPFunc.Write(handler, sampleTime, buf, size, is_keyframe, streamIdx, isVideo);
        }

        public  bool IsConnected(int handler)
        {
            if (!m_IsInitialized)
                return false;

            if (m_RTMPFunc.IsConnected != null)
                return m_RTMPFunc.IsConnected(handler) > 0;

            return false;
        }        
    }
}
