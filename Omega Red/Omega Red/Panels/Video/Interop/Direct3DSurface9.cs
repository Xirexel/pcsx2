using System;
using System.Runtime.InteropServices;

namespace Omega_Red.Panels.Video.Interop
{
    internal sealed class Direct3DSurface9 : IDisposable
    {
        private ComInterface.IDirect3DSurface9 comObject;
        private IntPtr native;
        
        public ComInterface.IDirect3DSurface9 texture { get { return comObject; } }

        internal Direct3DSurface9(ComInterface.IDirect3DSurface9 obj)
        {
            this.comObject = obj;
            this.native = Marshal.GetIUnknownForObject(this.comObject);
        }

        ~Direct3DSurface9()
        {
            this.Release();
        }

        public IntPtr NativeInterface
        {
            get { return this.native; }
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
                Marshal.Release(this.native);
                this.native = IntPtr.Zero;

                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
            }
        }
    }
}
