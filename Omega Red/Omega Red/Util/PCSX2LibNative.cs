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
using Omega_Red.Tools;
using Omega_Red.Managers;

namespace Omega_Red.Util
{

    class PCSX2LibNative
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void FirstDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void SecondDelegate(UInt32 a_FirstArg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ThirdDelegate(IntPtr a_FirstArg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr FourthDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool FifthDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void SixthDelegate(Int32 a_FirstArg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void SeventhDelegate(IntPtr a_FirstArg, Int32 a_SecondArg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void EightthDelegate(IntPtr buffer, Int32 offset, Int32 bytes, Boolean read);
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void NinethDelegate([MarshalAs(UnmanagedType.LPWStr)] string serial);
                                
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate UInt32 TenthDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr EleventhDelegate(IntPtr a_FirstArg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate Int32 TwelvethDelegate(IntPtr a_FirstArg, Int32 a_mode, Int32 a_ModuleID);


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr ThirteenthDelegate(IntPtr a_FirstArg, Int32 a_ModuleID);


        
        // Init

        class PCSX2Init
        {
            //Определяет тип процессора и поддерживаемые.
            public FirstDelegate DetectCpuAndUserModeFunc;

            //Устанавливает конфигурацию компиляторов.
            public NinethDelegate ApplySettingsFunc;

            public FirstDelegate VTLB_Alloc_PpmapFinc;

            //Устанавливает конфигурацию компиляторов.
            public NinethDelegate AllocateCoreStuffsFunc;
            
            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setPluginsInitCallback;

            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setPluginsCloseCallback;

            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setPluginsShutdownCallback;

            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setPluginsOpenCallback;

            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setPluginsAreLoadedCallback;
            
            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setUI_EnableSysActionsCallback;
            
            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setLoadAllPatchesAndStuffCallback;

            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setLoadBIOSCallbackCallback;

            public FirstDelegate resetCallbacksFunc;

            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setCDVDNVMCallback;

            //Определяет тип процессора и поддерживаемые.
            public ThirdDelegate setCDVDGetMechaVerCallback;

            public ThirdDelegate setDoFreezeCallback;
        }
        
        class PCSX2Memory
        {
            public EleventhDelegate getFreezeInternalsFunc;

            public EleventhDelegate getEmotionMemoryFunc;

            public EleventhDelegate getIopMemoryFunc;

            public EleventhDelegate getHwRegsFunc;

            public EleventhDelegate getIopHwRegsFunc;

            public EleventhDelegate getScratchpadFunc;

            public EleventhDelegate getVU0memFunc;

            public EleventhDelegate getVU1memFunc;

            public EleventhDelegate getVU0progFunc;

            public EleventhDelegate getVU1progFunc;

            public ThirteenthDelegate getFreezeOutFunc;

            public SeventhDelegate setFreezeInFunc;
            
            public SeventhDelegate setFreezeInternalsFunc;

            public SeventhDelegate setEmotionMemoryFunc;

            public SeventhDelegate setIopMemoryFunc;

            public SeventhDelegate setHwRegsFunc;

            public SeventhDelegate setIopHwRegsFunc;

            public SeventhDelegate setScratchpadFunc;

            public SeventhDelegate setVU0memFunc;

            public SeventhDelegate setVU1memFunc;

            public SeventhDelegate setVU0progFunc;

            public SeventhDelegate setVU1progFunc;
        }
                
        class PCSX2System
        {
            public ThirdDelegate releaseWCHARStringFunc;
            
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate void getSysGetBiosDiscIDFuncDelegate(out IntPtr aPtrPtrSysGetBiosDiscID);
            public getSysGetBiosDiscIDFuncDelegate getSysGetBiosDiscIDFunc;
                        
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate void getSysGetDiscIDFuncDelegate(out IntPtr aPtrPtrSysGetDiscID);
            public getSysGetDiscIDFuncDelegate getSysGetDiscIDFunc;
                        
            public FirstDelegate gsUpdateFrequencyCallFunc;

            public NinethDelegate PCSX2_Hle_SetElfPathFunc;

            public FirstDelegate SysThreadBase_ResumeFunc;

            public FirstDelegate SysThreadBase_SuspendFunc;

            public FirstDelegate SysThreadBase_ResetFunc;

            public FirstDelegate SysThreadBase_CancelFunc;
            
            public NinethDelegate setSioSetGameSerialFunc;

            public FirstDelegate MTGS_WaitGSFunc;

            public FirstDelegate MTGS_WaitForOpenFunc; 
                        
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate bool MTGS_IsSelfFuncDelegate();
            public MTGS_IsSelfFuncDelegate MTGS_IsSelfFunc;

            public FirstDelegate MTGS_SuspendFunc;

            public FirstDelegate MTGS_ResumeFunc;

            public FirstDelegate MTGS_CancelFunc;   
                                    
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate void MTGS_FreezeFuncDelegate(Int32 mode, IntPtr data);
            public MTGS_FreezeFuncDelegate MTGS_FreezeFunc;

            public FirstDelegate MTVU_CancelFunc;  
            
            
                    

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate bool getGameStartedFuncDelegate();
            public getGameStartedFuncDelegate getGameStartedFunc;  
                        
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate bool getGameLoadingFuncDelegate();
            public getGameLoadingFuncDelegate getGameLoadingFunc;  

            public TenthDelegate getElfCRCFunc;

            public FirstDelegate vu1Thread_WaitVUFunc;

            public FirstDelegate ForgetLoadedPatchesFunc;

            public NinethDelegate inifile_commandFunc;
        }

        class PCSX2Modules
        {
            public FifthDelegate openPlugin_SPU2Func;                         
            public FirstDelegate openPlugin_DEV9Func;
            public FirstDelegate openPlugin_USBFunc;
            public FirstDelegate openPlugin_FWFunc;
            public ThirdDelegate setSPU2;
            public ThirdDelegate setGS;
            public ThirdDelegate setDEV9;  
            public ThirdDelegate setMcd;
            public ThirdDelegate setCDVD;
            public ThirdDelegate setPAD;
            public ThirdDelegate setFW;
            public ThirdDelegate setUSB;    
        }

        private PCSX2Init m_PCSX2Init = new PCSX2Init();

        private PCSX2Memory m_PCSX2Memory = new PCSX2Memory();

        private PCSX2System m_PCSX2System = new PCSX2System();

        private PCSX2Modules m_PCSX2Modules = new PCSX2Modules();

        private LibLoader m_LibLoader = null;

        private bool m_IsInitialized = false;


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
        
        private PCSX2LibNative() {

            try
            {


                var l_ModuleBeforeTitle = App.Current.Resources["ModuleBeforeTitle"];

                var l_ModuleAfterTitle = App.Current.Resources["ModuleAfterTitle"];
                
                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle 
                    + "PCSX2Lib"
                    + l_ModuleAfterTitle);

                do
                {
                    m_LibLoader = LibLoader.create("PCSX2Lib");

                    if (m_LibLoader == null)
                        break;

                    if (!m_LibLoader.isLoaded)
                        break;

                    reflectFunctions(m_PCSX2Init);

                    reflectFunctions(m_PCSX2Memory);

                    reflectFunctions(m_PCSX2System);

                    reflectFunctions(m_PCSX2Modules);

                    m_IsInitialized = true;

                } while (false);

                LockScreenManager.Instance.displayMessage(
                    l_ModuleBeforeTitle
                    + "PCSX2Lib"
                    + (m_IsInitialized ? App.Current.Resources["ModuleIsLoadedTitle"] : App.Current.Resources["ModuleIsNotLoadedTitle"]));

            }
            catch (Exception)
            {                
            }        
        }

        public void release()
        {
            if (m_LibLoader != null)
                m_LibLoader.release();
        }
        
        private Delegate parseFunction123(string a_func, Type a_type)
        {
            if (!m_LibLoader.isLoaded)
                return null;
            
            return System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(m_LibLoader.getFunc(a_func), a_type);
        }

        private static PCSX2LibNative m_Instance = null;

        public static PCSX2LibNative Instance { get { if (m_Instance == null) m_Instance = new PCSX2LibNative(); return m_Instance; } }

        public bool isInit { get { return m_IsInitialized; } }


        public void DetectCpuAndUserModeFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Init.DetectCpuAndUserModeFunc != null)
                m_PCSX2Init.DetectCpuAndUserModeFunc();
        }

        public FirstDelegate setPluginsInitCallback { set { m_PCSX2Init.setPluginsInitCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public FirstDelegate setPluginsCloseCallback { set { m_PCSX2Init.setPluginsCloseCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public FirstDelegate setPluginsShutdownCallback { set { m_PCSX2Init.setPluginsShutdownCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public FirstDelegate setPluginsOpenCallback { set { m_PCSX2Init.setPluginsOpenCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public FifthDelegate setPluginsAreLoadedCallback { set { m_PCSX2Init.setPluginsAreLoadedCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }
               
        public FirstDelegate setUI_EnableSysActionsCallback { set { m_PCSX2Init.setUI_EnableSysActionsCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }
        
        public SecondDelegate setLoadAllPatchesAndStuffCallback { set { m_PCSX2Init.setLoadAllPatchesAndStuffCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public SeventhDelegate setLoadBIOSCallbackCallback { set { m_PCSX2Init.setLoadBIOSCallbackCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public EightthDelegate setCDVDNVMCallback { set { m_PCSX2Init.setCDVDNVMCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public ThirdDelegate setCDVDGetMechaVerCallback { set { m_PCSX2Init.setCDVDGetMechaVerCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }

        public TwelvethDelegate setDoFreezeCallback { set { m_PCSX2Init.setDoFreezeCallback(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(value)); } }




        public void ApplySettings(string a_XmlPcsx2Config)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Init.ApplySettingsFunc != null)
                m_PCSX2Init.ApplySettingsFunc(a_XmlPcsx2Config);
        }

        public void VTLB_Alloc_PpmapFinc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Init.VTLB_Alloc_PpmapFinc != null)
                m_PCSX2Init.VTLB_Alloc_PpmapFinc();
        }

        

        public void AllocateCoreStuffsFunc(string a_XmlPcsx2Config)
        {
            if (!m_IsInitialized)
                return;
            
            if (m_PCSX2Init.AllocateCoreStuffsFunc != null)
                m_PCSX2Init.AllocateCoreStuffsFunc(a_XmlPcsx2Config);
        }

        

        public void MTVU_CancelFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.MTVU_CancelFunc != null)
                m_PCSX2System.MTVU_CancelFunc();
        }

        public void PCSX2_Hle_SetElfPathFunc(string elfFileName)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.PCSX2_Hle_SetElfPathFunc != null)
                m_PCSX2System.PCSX2_Hle_SetElfPathFunc(elfFileName);
        }

        public void SysThreadBase_ResumeFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.SysThreadBase_ResumeFunc != null)
                m_PCSX2System.SysThreadBase_ResumeFunc();
        }

        public void SysThreadBase_SuspendFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.SysThreadBase_SuspendFunc != null)
                m_PCSX2System.SysThreadBase_SuspendFunc();
        }

        public void SysThreadBase_ResetFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.SysThreadBase_ResetFunc != null)
                m_PCSX2System.SysThreadBase_ResetFunc();
        }

        public void SysThreadBase_CancelFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.SysThreadBase_CancelFunc != null)
                m_PCSX2System.SysThreadBase_CancelFunc();
        }

        public void resetCallbacksFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Init.resetCallbacksFunc != null)
                m_PCSX2Init.resetCallbacksFunc();
        }           
        
        public void MTGS_CancelFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.MTGS_CancelFunc != null)
                m_PCSX2System.MTGS_CancelFunc();
        }

        public void MTGS_ResumeFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.MTGS_ResumeFunc != null)
                m_PCSX2System.MTGS_ResumeFunc();
        }

        public void MTGS_SuspendFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.MTGS_SuspendFunc != null)
                m_PCSX2System.MTGS_SuspendFunc();
        }               

        public void openPlugin_SPU2Func()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Modules.openPlugin_SPU2Func != null)
                m_PCSX2Modules.openPlugin_SPU2Func();
        }

        public void openPlugin_DEV9Func()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Modules.openPlugin_DEV9Func != null)
                m_PCSX2Modules.openPlugin_DEV9Func();
        }

        public void openPlugin_USBFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Modules.openPlugin_USBFunc != null)
                m_PCSX2Modules.openPlugin_USBFunc();
        }

        public void openPlugin_FWFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Modules.openPlugin_FWFunc != null)
                m_PCSX2Modules.openPlugin_FWFunc();
        }

        public void MTGS_WaitForOpenFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.MTGS_WaitForOpenFunc != null)
                m_PCSX2System.MTGS_WaitForOpenFunc();
        }

        public UInt32 getElfCRCFunc()
        {
            if (!m_IsInitialized)
                return 0;

            if (m_PCSX2System.getElfCRCFunc != null)
                return m_PCSX2System.getElfCRCFunc();

            return 0;
        }

        public bool getGameStartedFunc()
        {
            if (!m_IsInitialized)
                return false;

            if (m_PCSX2System.getGameStartedFunc != null)
                return m_PCSX2System.getGameStartedFunc();

            return false;
        }

        public bool getGameLoadingFunc()
        {
            if (!m_IsInitialized)
                return false;

            if (m_PCSX2System.getGameLoadingFunc != null)
                return m_PCSX2System.getGameLoadingFunc();

            return false;
        }

        public string getSysGetDiscIDFunc()
        {
            string l_result = "";

            if (!m_IsInitialized)
                return l_result;

            if (m_PCSX2System.getSysGetDiscIDFunc != null && 
                m_PCSX2System.releaseWCHARStringFunc != null)
            {
                IntPtr l_PtrResult;

                m_PCSX2System.getSysGetDiscIDFunc(out l_PtrResult);

                l_result = Marshal.PtrToStringAuto(l_PtrResult);

                m_PCSX2System.releaseWCHARStringFunc(l_PtrResult);
            }


            return l_result;
        }

        public string getSysGetBiosDiscIDFunc()
        {
            string l_result = "";

            if (!m_IsInitialized)
                return l_result;

            if (m_PCSX2System.getSysGetBiosDiscIDFunc != null &&
                m_PCSX2System.releaseWCHARStringFunc != null)
            {
                IntPtr l_PtrResult;

                m_PCSX2System.getSysGetBiosDiscIDFunc(out l_PtrResult);

                l_result = Marshal.PtrToStringAuto(l_PtrResult);

                m_PCSX2System.releaseWCHARStringFunc(l_PtrResult);
            }


            return l_result;
        }

        public void gsUpdateFrequencyCallFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.gsUpdateFrequencyCallFunc != null)
                m_PCSX2System.gsUpdateFrequencyCallFunc();
        }

        public void setSioSetGameSerialFunc(string a_GameSerial)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.setSioSetGameSerialFunc != null)
                m_PCSX2System.setSioSetGameSerialFunc(a_GameSerial);
        }

        public void inifile_commandFunc(string a_patchString)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.inifile_commandFunc != null)
                m_PCSX2System.inifile_commandFunc(a_patchString);
        }

        

        private byte[] getMemorySection(EleventhDelegate a_ptrMemory)
        {

            if (!m_IsInitialized)
                return new byte[0];
            
            if (a_ptrMemory != null)
            {
                var l_memory = Marshal.AllocHGlobal(4);

                var l_ptrMemory = a_ptrMemory(l_memory);

                Int32[] lsize = new Int32[1];

                Marshal.Copy(l_memory, lsize, 0, lsize.Length);

                Marshal.FreeHGlobal(l_memory);

                byte[] l_buffer = new byte[lsize[0]];

                System.Runtime.InteropServices.Marshal.Copy(l_ptrMemory, l_buffer, 0, l_buffer.Length);

                return l_buffer;
            }

            return new byte[0];
        }

        private byte[] getMemorySection(ThirteenthDelegate a_ptrMemory, Int32 a_ModuleCode)
        {

            if (!m_IsInitialized)
                return new byte[0];

            if (a_ptrMemory != null)
            {
                var l_memory = Marshal.AllocHGlobal(4);

                var l_ptrMemory = a_ptrMemory(l_memory, a_ModuleCode);

                Int32[] lsize = new Int32[1];

                Marshal.Copy(l_memory, lsize, 0, lsize.Length);

                Marshal.FreeHGlobal(l_memory);

                byte[] l_buffer = new byte[lsize[0]];

                System.Runtime.InteropServices.Marshal.Copy(l_ptrMemory, l_buffer, 0, l_buffer.Length);

                return l_buffer;
            }

            return new byte[0];
        }
               

        public byte[] getFreezeInternalsFunc()
        {
            return getMemorySection(m_PCSX2Memory.getFreezeInternalsFunc);
        }

        public byte[] getEmotionMemoryFunc()
        {
            return getMemorySection(m_PCSX2Memory.getEmotionMemoryFunc);
        }

        public byte[] getIopMemoryFunc()
        {
            return getMemorySection(m_PCSX2Memory.getIopMemoryFunc);
        }

        public byte[] getHwRegsFunc()
        {
            return getMemorySection(m_PCSX2Memory.getHwRegsFunc);
        }

        public byte[] getIopHwRegsFunc()
        {
            return getMemorySection(m_PCSX2Memory.getIopHwRegsFunc);
        }

        public byte[] getScratchpadFunc()
        {
            return getMemorySection(m_PCSX2Memory.getScratchpadFunc);
        }

        public byte[] getVU0memFunc()
        {
            return getMemorySection(m_PCSX2Memory.getVU0memFunc);
        }

        public byte[] getVU1memFunc()
        {
            return getMemorySection(m_PCSX2Memory.getVU1memFunc);
        }

        public byte[] getVU0progFunc()
        {
            return getMemorySection(m_PCSX2Memory.getVU0progFunc);
        }

        public byte[] getVU1progFunc()
        {
            return getMemorySection(m_PCSX2Memory.getVU1progFunc);
        }

        public void ForgetLoadedPatchesFunc()
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2System.ForgetLoadedPatchesFunc != null)
                m_PCSX2System.ForgetLoadedPatchesFunc();
        }

        public byte[] getFreezeOutFunc(Int32 a_ModuleID)
        {
            return getMemorySection(m_PCSX2Memory.getFreezeOutFunc, a_ModuleID);
        }

        public void setFreezeInFunc(byte[] a_Memory, Int32 a_ModuleID)
        {
            if (!m_IsInitialized)
                return;

            if (m_PCSX2Memory.setFreezeInFunc != null)
            {
                var l_memory = Marshal.AllocHGlobal(a_Memory.Length);

                Marshal.Copy(a_Memory, 0, l_memory, a_Memory.Length);

                m_PCSX2Memory.setFreezeInFunc(l_memory, a_ModuleID);
                
                Marshal.FreeHGlobal(l_memory);
            }
        }
        
        private void setMemorySection(SeventhDelegate a_ptrMemory, byte[] a_memory)
        {

            if (!m_IsInitialized)
                return;

            if (a_ptrMemory != null)
            {
                var l_ptrMemory = Marshal.AllocHGlobal(a_memory.Length);

                System.Runtime.InteropServices.Marshal.Copy(a_memory, 0, l_ptrMemory, a_memory.Length);

                a_ptrMemory(l_ptrMemory, a_memory.Length);
                
                Marshal.FreeHGlobal(l_ptrMemory);
            }
        }

        public void setFreezeInternalsFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setFreezeInternalsFunc, a_memory);
        }

        public void setEmotionMemoryFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setEmotionMemoryFunc, a_memory);
        }

        public void setIopMemoryFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setIopMemoryFunc, a_memory);
        }

        public void setHwRegsFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setHwRegsFunc, a_memory);
        }

        public void setIopHwRegsFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setIopHwRegsFunc, a_memory);
        }

        public void setScratchpadFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setScratchpadFunc, a_memory);
        }

        public void setVU0memFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setVU0memFunc, a_memory);
        }

        public void setVU1memFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setVU1memFunc, a_memory);
        }

        public void setVU0progFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setVU0progFunc, a_memory);
        }

        public void setVU1progFunc(byte[] a_memory)
        {
            setMemorySection(m_PCSX2Memory.setVU1progFunc, a_memory);
        }




        public bool MTGS_IsSelfFunc()
        {
            if (!m_IsInitialized)
                return false;

            if (m_PCSX2System.MTGS_IsSelfFunc != null)
                return m_PCSX2System.MTGS_IsSelfFunc();

            return false;
        }
                       
        public void setModule(PCSX2ModuleManager.Module a_Module)
        {
            if (!m_IsInitialized)
                return;

            switch (a_Module.ModuleType)
	        {
                case PCSX2ModuleManager.ModuleType.AudioRenderer:
                    if (m_PCSX2Modules.setSPU2 != null)
                        m_PCSX2Modules.setSPU2(a_Module.getPCSX2Lib_API());
                 break;
                case PCSX2ModuleManager.ModuleType.VideoRenderer:
                 if (m_PCSX2Modules.setGS != null)
                     m_PCSX2Modules.setGS(a_Module.getPCSX2Lib_API());
                 break;
                case PCSX2ModuleManager.ModuleType.DEV9:
                 if (m_PCSX2Modules.setDEV9 != null)
                     m_PCSX2Modules.setDEV9(a_Module.getPCSX2Lib_API());
                 break;
                case PCSX2ModuleManager.ModuleType.MemoryCard:
                 if (m_PCSX2Modules.setMcd != null)
                     m_PCSX2Modules.setMcd(a_Module.getPCSX2Lib_API());
                 break;
                case PCSX2ModuleManager.ModuleType.CDVD:
                 if (m_PCSX2Modules.setCDVD != null)
                     m_PCSX2Modules.setCDVD(a_Module.getPCSX2Lib_API());
                 break;
                case PCSX2ModuleManager.ModuleType.Pad:
                 if (m_PCSX2Modules.setPAD != null)
                     m_PCSX2Modules.setPAD(a_Module.getPCSX2Lib_API());
                 break;
                case PCSX2ModuleManager.ModuleType.FW:
                 if (m_PCSX2Modules.setFW != null)
                     m_PCSX2Modules.setFW(a_Module.getPCSX2Lib_API());
                 break;
                case PCSX2ModuleManager.ModuleType.USB:
                 if (m_PCSX2Modules.setUSB != null)
                     m_PCSX2Modules.setUSB(a_Module.getPCSX2Lib_API());
                 break;
                default:
                 break;
	        }
        }        
    }
}
