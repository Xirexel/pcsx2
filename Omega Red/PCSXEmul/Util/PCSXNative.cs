using PCSXEmul.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCSXEmul.Util
{
    class PCSXNative
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void FirstDelegate(
            [MarshalAs(UnmanagedType.LPStr)]String szCmdLine);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void SecondDelegate(IntPtr a_FirstArg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int ThirdDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void FourthDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int FifthDelegate(
            [MarshalAs(UnmanagedType.LPStr)]String a_filePath);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int SixthDelegate(
            [MarshalAs(UnmanagedType.LPWStr)]String a_filePath,
            Int32 a_slot);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate Int32 SeventhDelegate(IntPtr a_FirstArg, Int32 a_SecondArg);



        // Init
        class PCSXFunctions
        {
            //Запуск игрового диска.
            public FirstDelegate Launch;

            public ThirdDelegate Pause;

            public ThirdDelegate Resume;

            public ThirdDelegate Shutdown;

            public FifthDelegate SaveLibState;

            public FifthDelegate LoadLibState;

            public SixthDelegate SetMcd;
        }

        class PCSXModules
        {
            public SecondDelegate setGS;
            public SecondDelegate setSPU;
            public SecondDelegate setPAD;
            public SecondDelegate setSIO1;
        }


        class PCSXInit
        {
            public SecondDelegate setPluginsOpenCallback;

            public SecondDelegate setPluginsCloseCallback;

            public SecondDelegate setBIOSMemoryCallback;
        }


        private PCSXInit m_PCSXInit = new PCSXInit();

        private PCSXFunctions m_PCSXFunctions = new PCSXFunctions();

        private PCSXModules m_PCSXModules = new PCSXModules();


        private bool m_IsInitialized = false;

        private LibLoader m_LibLoader = null;

        private static PCSXNative m_Instance = null;

        public static PCSXNative Instance { get { if (m_Instance == null) m_Instance = new PCSXNative(); return m_Instance; } }

        private PCSXNative()
        {

            try
            {
                do
                {


                    m_LibLoader = LibLoader.create("pcsxr");

                    if (m_LibLoader == null)
                        break;

                    if (!m_LibLoader.isLoaded)
                        break;

                    reflectFunctions(m_PCSXInit);

                    reflectFunctions(m_PCSXFunctions);

                    reflectFunctions(m_PCSXModules);

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

        public void launch(string szCmdLine)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSXFunctions.Launch != null)
                m_PCSXFunctions.Launch(szCmdLine);
        }

        public void shutdown()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSXFunctions.Shutdown != null)
                m_PCSXFunctions.Shutdown();
        }

        public void pause()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSXFunctions.Pause != null)
                m_PCSXFunctions.Pause();
        }

        public void resume()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSXFunctions.Resume != null)
                m_PCSXFunctions.Resume();
        }

        public void save(string a_filename)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSXFunctions.SaveLibState != null)
                m_PCSXFunctions.SaveLibState(a_filename);
        }

        public void load(string a_filename)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSXFunctions.LoadLibState != null)
                m_PCSXFunctions.LoadLibState(a_filename);
        }

        public void setMcd(string a_filename, int a_slot)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSXFunctions.SetMcd != null)
                m_PCSXFunctions.SetMcd(a_filename, a_slot);
        }






        public void getGameInfo(string a_filename, out string l_title, out string l_id)
        {
            l_title = "";
            l_id = "";
        }

        public void setAudioVolume(float a_value)
        {
            ModuleControl.Instance.setSoundLevel(a_value);
        }

        public void setLimitFrame(bool a_value) { }

        public void clearModules()
        {
            if (m_PCSXModules.setGS != null)
                m_PCSXModules.setGS(IntPtr.Zero);
        }

        public void setModule(PCSXModuleManager.Module a_Module)
        {
            if (!m_IsInitialized)
                return;

            switch (a_Module.ModuleType)
            {
                case PCSXModuleManager.ModuleType.DFXVideo:
                    if (m_PCSXModules.setGS != null)
                        m_PCSXModules.setGS(a_Module.getModule_API());
                    break;
                case PCSXModuleManager.ModuleType.GPUHardware:
                    if (m_PCSXModules.setGS != null)
                        m_PCSXModules.setGS(a_Module.getModule_API());
                    break;
                case PCSXModuleManager.ModuleType.DFSound:
                    if (m_PCSXModules.setSPU != null)
                        m_PCSXModules.setSPU(a_Module.getModule_API());
                    break;
                case PCSXModuleManager.ModuleType.bladesio1:
                    if (m_PCSXModules.setSIO1 != null)
                        m_PCSXModules.setSIO1(a_Module.getModule_API());
                    break;
                case PCSXModuleManager.ModuleType.Pad:
                    if (m_PCSXModules.setPAD != null)
                        m_PCSXModules.setPAD(a_Module.getModule_API());
                    break;
                default:
                    break;
            }
        }

        public void release()
        {
            if (m_LibLoader != null)
                m_LibLoader.release();
        }

        public FourthDelegate setPluginsOpenCallback { set { m_PCSXInit.setPluginsOpenCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public FourthDelegate setPluginsCloseCallback { set { m_PCSXInit.setPluginsCloseCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }
        public SeventhDelegate setBIOSMemoryCallback { set { m_PCSXInit.setBIOSMemoryCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }
    }
}
