using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PPSSPPEmul.Util
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate IntPtr GetTouchPadCallback(UInt32 aPadIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void SetDataCallback(IntPtr aPtrData, UInt32 aByteSize);

    class PPSSPPNative: IDisposable
    {
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void FirstDelegate(
            [MarshalAs(UnmanagedType.LPWStr)]String szCmdLine,
            IntPtr a_parent, 
            IntPtr a_CaptureHandler,
            IntPtr a_getTouchPadCallback,
            IntPtr a_setAudioDataCallbackHandler,
            [MarshalAs(UnmanagedType.LPWStr)]String szStickDirectory);
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void SecondDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void ThirdDelegate([MarshalAs(UnmanagedType.LPStr)]String a_filename);
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void FourthDelegate([MarshalAs(UnmanagedType.LPStr)]String a_filename, out IntPtr a_result);
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void FifthDelegate(float a_level);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void SixthDelegate(bool a_state);




        // Init

        class PPSSPPFunctions
        {
            //Запуск игрового диска.
            public FirstDelegate Launch;

            //Остановка игры.
            public SecondDelegate Shutdown;

            // Сохранение текущего состояния
            public ThirdDelegate Save;

            // Загрузка состояния
            public ThirdDelegate Load;

            // Получение информации о игровом диске
            public FourthDelegate GetGameInfo;
            
            // Установить игру с паузу
            public SecondDelegate Pause;

            // Снять игру с паузы
            public SecondDelegate Resume;

            // Установить гоомкость
            public FifthDelegate SetAudioVolume;

            public SixthDelegate SetLimitFrame;
        }



        private bool m_IsInitialized = false;

        private LibLoader m_LibLoader = null;                            
               
        private PPSSPPFunctions m_PPSSPPFunctions = new PPSSPPFunctions();
        
        public PPSSPPNative()
        {

            try
            {
                do
                {

                    m_LibLoader = LibLoader.create("PPSSPP.dll", true);

                    if (m_LibLoader == null)
                        break;

                    if (!m_LibLoader.isLoaded)
                        break;

                    reflectFunctions(m_PPSSPPFunctions);

                    m_IsInitialized = true;

                } while (false);
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

        public void launch(string szCmdLine, IntPtr a_VideoPanelHandler, IntPtr a_CaptureHandler, IntPtr a_getTouchPadCallback, IntPtr a_AudioCaptureTargetHandler, string szStickDirectory)
        {
            if (!m_IsInitialized)
                return;

            if (m_PPSSPPFunctions.Launch != null)
                m_PPSSPPFunctions.Launch(szCmdLine, a_VideoPanelHandler, a_CaptureHandler, a_getTouchPadCallback, a_AudioCaptureTargetHandler, szStickDirectory);
        }

        public void getGameInfo(string a_filename, out string a_title, out string a_id)
        {
            a_title = "";

            a_id = "";
            
            if (!m_IsInitialized)
                return;

            IntPtr l_result = IntPtr.Zero;

            if (m_PPSSPPFunctions.GetGameInfo != null)
                m_PPSSPPFunctions.GetGameInfo(a_filename, out l_result);

            if(l_result != IntPtr.Zero)
            {
                var ltext = Marshal.PtrToStringBSTR(l_result);

                var lsplits = ltext.Split(new char[] { '#' });

                if(lsplits !=  null && lsplits.Length == 2)
                {
                    a_title = lsplits[0];

                    a_id = lsplits[1];
                }

                Marshal.FreeBSTR(l_result);
            }
        }

        public void load(string a_filename)
        {
            if (!m_IsInitialized)
                return;

            if (m_PPSSPPFunctions.Load != null)
                m_PPSSPPFunctions.Load(a_filename);
        }

        public void pause()
        {
            if (!m_IsInitialized)
                return;

            if (m_PPSSPPFunctions.Pause != null)
                m_PPSSPPFunctions.Pause();
        }
        
        public void resume()
        {
            if (!m_IsInitialized)
                return;

            if (m_PPSSPPFunctions.Resume != null)
                m_PPSSPPFunctions.Resume();
        }

        public void save(string a_filename)
        {
            if (!m_IsInitialized)
                return;

            if (m_PPSSPPFunctions.Save != null)
                m_PPSSPPFunctions.Save(a_filename);
        }

        public void setAudioVolume(float a_level)
        {
            if (!m_IsInitialized)
                return;

            if (m_PPSSPPFunctions.SetAudioVolume != null)
                m_PPSSPPFunctions.SetAudioVolume(a_level);
        }

        public void setLimitFrame(bool a_state)
        {
            if (!m_IsInitialized)
                return;

            if (m_PPSSPPFunctions.SetLimitFrame != null)
                m_PPSSPPFunctions.SetLimitFrame(a_state);
        }

        public void shutdown()
        {
            if (!m_IsInitialized)
                return;

            if (m_PPSSPPFunctions.Shutdown != null)
                m_PPSSPPFunctions.Shutdown();
        }
        
        public void release()
        {
            if(m_LibLoader != null)
                m_LibLoader.release();
        }

        public void Dispose()
        {
            release();
        }
    }
}
