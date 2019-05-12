using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Capture
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate void SetDataCallback(IntPtr aPtrData, UInt32 aByteSize);
    
    class AudioCaptureTarget
    {
        private static AudioCaptureTarget m_Instance = null;

        private SetDataCallback m_SetDataCallback = null;

        private IntPtr m_SetDataCallbackHandler = IntPtr.Zero;

        private Action<IntPtr, uint> setData { get; set; }
    
        private AudioCaptureTarget() {

            RegisterAction = null;

            m_SetDataCallback = (aPtrData, aByteSize)=>{

                lock (this)
                {
                    if (setData != null)
                    {
                        setData(aPtrData, aByteSize);
                    }
                }
            };

            RegisterAction = (value) => {
                lock (this)
                {
                    setData = value;
                }
            };

            m_SetDataCallbackHandler = Marshal.GetFunctionPointerForDelegate(m_SetDataCallback);
        }

        public static AudioCaptureTarget Instance { get { if (m_Instance == null) m_Instance = new AudioCaptureTarget(); return m_Instance; } }

        public SetDataCallback DataCallback { get { return m_SetDataCallback; } }

        public IntPtr DataCallbackHandler { get { return m_SetDataCallbackHandler; } }

        public Action<Action<IntPtr, uint>> RegisterAction { get; private set; }
    }
}
