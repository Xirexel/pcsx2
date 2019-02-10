using System;
using System.Runtime.InteropServices;

namespace Omega_Red.Panels.Video.Interop
{
    internal sealed class Direct3D9 : IDisposable
    {
        private ComInterface.IDirect3D9 comObject;
        private ComInterface.CreateDevice createDevice;

        private Direct3D9(ComInterface.IDirect3D9 obj)
        {
            this.comObject = obj;
            ComInterface.GetComMethod(this.comObject, 16, out this.createDevice);
        }

        ~Direct3D9()
        {
            this.Release();
        }

        public void Dispose()
        {
            this.Release();
            GC.SuppressFinalize(this);
        }

        public static Direct3D9 Create(int SDKVersion)
        {
            ComInterface.IDirect3D9 obj;
           
			obj = NativeMethods.Direct3DCreate9(SDKVersion);

            return new Direct3D9(obj);
        }

        public Direct3DDevice9 CreateDevice(uint Adapter, int DeviceType, IntPtr hFocusWindow, int BehaviorFlags,
                                                NativeStructs.D3DPRESENT_PARAMETERS pPresentationParameters, NativeStructs.D3DDISPLAYMODE pFullscreenDisplayMode)
        {
            ComInterface.IDirect3DDevice9 obj = null;
            int result = this.createDevice(this.comObject, Adapter, DeviceType, hFocusWindow, BehaviorFlags, pPresentationParameters, out obj);
            
            Marshal.ThrowExceptionForHR(result);

            return new Direct3DDevice9(obj);
        }

        private void Release()
        {
            if (this.comObject != null)
            {
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
                this.createDevice = null;
            }
        }
    }
}
