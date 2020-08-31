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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Omega_Red.Util;
using Omega_Red.Managers;

namespace Omega_Red.Tools
{
    class PCSX2ModuleManager
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate IntPtr getPCSX2Lib_API();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void execute([MarshalAs(UnmanagedType.LPWStr)] string a_command, out IntPtr a_result);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void releaseString(IntPtr a_result);

        public enum ModuleType : int
        {
            VideoRenderer = 0,
            Pad = 1,
            AudioRenderer = 2,
            CDVD = 3,
            USB = 4,
	        FW = 5,
            DEV9 = 6,
            MemoryCard = 7,
            None = -1
        }

        private enum FREEZE_TYPE
        {
            FREEZE_LOAD = 0,
            FREEZE_SAVE = 1,
            FREEZE_SIZE = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        struct freezeData
        {
            public int size;
            public IntPtr data;
        };

        public static ModuleType getModuleType(Int32 a_ModuleType)
        {
            ModuleType l_ModuleType = (ModuleType)Enum.ToObject(typeof(ModuleType), a_ModuleType);

            if (l_ModuleType == null)
                l_ModuleType = ModuleType.None;

            return l_ModuleType;
        }

        public static Int32 getModuleID(ModuleType a_ModuleType)
        {
            Int32 l_ModuleID = -1;

            l_ModuleID = (int)a_ModuleType;
            
            return l_ModuleID;
        }

        public static string getModuleTypeName(ModuleType a_ModuleType)
        {
            string l_result = "";

            switch (a_ModuleType)
            {
                case ModuleType.AudioRenderer:
                    break;
                case ModuleType.VideoRenderer:
                    l_result = "GS";
                    break;
                case ModuleType.DEV9:
                    break;
                case ModuleType.MemoryCard:
                    break;
                case ModuleType.Pad:
                    break;
                case ModuleType.CDVD:
                    break;
                case ModuleType.USB:
                    break;
                default:
                    break;
            }

            return l_result;
        }

        public static int GetFreezeSize(ModuleType a_ModuleType)
        {
	        freezeData fP = new freezeData(){ size = 0, data = IntPtr.Zero};
            if (!DoFreeze(a_ModuleType, FREEZE_TYPE.FREEZE_SIZE, fP)) return 0;
	        return fP.size;
        }

        private static bool DoFreeze( ModuleType a_ModuleType, FREEZE_TYPE mode, freezeData fP )
        {
	        if( (a_ModuleType == ModuleType.VideoRenderer) && !PCSX2LibNative.Instance.MTGS_IsSelfFunc())
	        {
		        // GS needs some thread safety love...

                //MTGS_FreezeData woot = { data, 0 };
                //GetMTGS().Freeze( mode, woot );
                //return woot.retval != -1;

                return false;
	        }
	        else
	        {
                //ScopedLock lock( m_mtx_PluginStatus );
                //return !m_info[pid] || m_info[pid]->CommonBindings.Freeze( mode, data ) != -1;

                return false;
	        }
        }

        class Module_API
        {
            public IntPtr getAPI = IntPtr.Zero;
            public IntPtr execute = IntPtr.Zero;
            public IntPtr releaseString = IntPtr.Zero;
        }

        public class Module_API_Func
        {
            private FieldInfo m_FieldInfo = null;

            private object m_api = null;

            public Module_API_Func(
                FieldInfo a_FieldInfo,
                object a_api)
            {
                m_FieldInfo = a_FieldInfo;

                m_api = a_api;
            }

            public string getName()
            {
                string l_result = "";

                do
                {
                    if (m_FieldInfo == null)
                        break;

                    l_result = m_FieldInfo.Name;

                } while (false);

                return l_result;
            }

            public void setValue(IntPtr a_value)
            {
                do
                {
                    if (m_FieldInfo == null)
                        break;

                    m_FieldInfo.SetValue(m_api, a_value);

                } while (false);
            }
        }

        public class Module: IModule
        {
            public bool m_initilized = false;
            private Module_API m_Module_API = new Module_API();
            private ModuleType m_ModuleType;
            private LibLoader m_LibLoader = null;

            public Module(ModuleType a_ModuleType)
            {
                m_ModuleType = a_ModuleType;

                var l_ModuleBeforeTitle = App.Current.Resources["ModuleBeforeTitle"];

                var l_ModuleAfterTitle = App.Current.Resources["ModuleAfterTitle"];

                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + Enum.GetName(m_ModuleType.GetType(), m_ModuleType)
                    + l_ModuleAfterTitle);

                do
                {
                
                    m_LibLoader = LibLoader.create(Enum.GetName(m_ModuleType.GetType(), m_ModuleType));

                    if (m_LibLoader == null)
                        break;

                    if (!m_LibLoader.isLoaded)
                        break;

                    foreach (var l_Plugin_Func in getModuleAPI())
                    {
                        l_Plugin_Func.setValue(m_LibLoader.getFunc(l_Plugin_Func.getName()));
                    }

                    m_initilized = true;
                    
                } while (false);
                
                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + Enum.GetName(m_ModuleType.GetType(), m_ModuleType) 
                    + " "
                    + (m_initilized ? App.Current.Resources["ModuleIsLoadedTitle"] : App.Current.Resources["ModuleIsNotLoadedTitle"]));
            }
            
            public void release()
            {
                m_LibLoader.release();
            }

            public ModuleType ModuleType { get { return m_ModuleType; } }

            public string getModuleName()
            {
                string l_result = Enum.GetName(typeof(ModuleType), m_ModuleType);

                return l_result;
            }

            public IList<Module_API_Func> getModuleAPI()
            {
                Type type = m_Module_API.GetType();

                IList<Module_API_Func> lresult = new List<Module_API_Func>();

                foreach (var item in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    lresult.Add(new Module_API_Func(item, m_Module_API));
                }

                return lresult;
            }

            public IntPtr getPCSX2Lib_API()
            {
                IntPtr l_result = IntPtr.Zero;

                do
                {
                    if (m_Module_API.getAPI == IntPtr.Zero)
                        break;

                    var l_getAPI = Marshal.GetDelegateForFunctionPointer(m_Module_API.getAPI,
                        typeof(getPCSX2Lib_API))
                        as getPCSX2Lib_API;

                    if (l_getAPI == null)
                        break;

                    try
                    {
                        l_result = l_getAPI();
                    }
                    catch (Exception)
                    {
                    }


                } while (false);

                return l_result;
            }

            public void execute(string a_command)
            {
                string l_result;

                execute(a_command, out l_result);
            }

            public void execute(string a_command, out string a_result)
            {
                do
                {
                    a_result = "";

                    if (m_Module_API.getAPI == IntPtr.Zero)
                        break;

                    var l_execute = Marshal.GetDelegateForFunctionPointer(m_Module_API.execute,
                        typeof(execute))
                        as execute;

                    if (l_execute == null)
                        break;

                    var l_releaseString = Marshal.GetDelegateForFunctionPointer(m_Module_API.releaseString,
                        typeof(releaseString))
                        as releaseString;

                    if (l_releaseString == null)
                        break;

                    try
                    {
                        IntPtr l_PtrResult;

                        l_execute(a_command, out l_PtrResult);

                        a_result = Marshal.PtrToStringAuto(l_PtrResult);

                        l_releaseString(l_PtrResult);
                    }
                    catch (Exception)
                    {
                    }

                } while (false);
            }
        }

        private List<Module> m_Modules = new List<Module>()
        {
            new Module(ModuleType.AudioRenderer),
            //new Module(ModuleType.VideoRenderer),
            new Module(ModuleType.DEV9),
            new Module(ModuleType.MemoryCard),
            new Module(ModuleType.CDVD),
            new Module(ModuleType.Pad),
            new Module(ModuleType.USB),
            new Module(ModuleType.FW)    
        };


        private Module m_GPU = null;

        public Module GPU { get { return m_GPU; } }

        private PCSX2ModuleManager() { }

        private static PCSX2ModuleManager m_Instance = null;

        public static PCSX2ModuleManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new PCSX2ModuleManager();

                    m_Instance.init();
                }
                return m_Instance;
            }
        }

        private bool m_isInit = false;

        public bool isInit { get { return m_isInit; } }

        public List<Module> Modules { get { return m_Modules; } }

        public Module getModule(ModuleType a_ModuleType)
        {
            Module l_result = null;

            foreach (var item in Modules)
            {
                if(item.ModuleType == a_ModuleType)
                {
                    l_result = item;

                    break;
                }
            }

            return l_result;
        }

        private void init()
        {
            bool l_state = true;

            do
            {

                foreach (var item in m_Modules)
                {
                    l_state = item.m_initilized;

                    if (!l_state)
                        break;
                }

            } while (false);

            m_isInit = l_state;
        }

        public void release()
        {
            releaseInner();

            m_Instance = null;
        }

        public void releaseInner()
        {
            foreach (var item in m_Modules)
            {
                item.release();
            }

            m_Modules.Clear();
        }

        public void initGPU()
        {
            if (m_GPU == null)
            {
                m_GPU = new Module(ModuleType.VideoRenderer);

                m_Modules.Add(m_GPU);
            }
        }

        public void releaseGPU()
        {
            if (m_GPU != null)
            {
                m_Modules.Remove(m_GPU);

                m_GPU.release();
            }

            m_GPU = null;

            GC.Collect();
        }
    }
}

