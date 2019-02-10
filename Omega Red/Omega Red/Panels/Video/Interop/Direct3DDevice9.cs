using System;
using System.Runtime.InteropServices;

namespace Omega_Red.Panels.Video.Interop
{
    internal sealed class Direct3DDevice9 : IDisposable
    {
        private ComInterface.IDirect3DDevice9 comObject;
        private ComInterface.CreateRenderTarget createRenderTarget;
        private ComInterface.CreateTexture createTexture;
        private ComInterface.CreateOffscreenPlainSurface createOffscreenPlainSurface;

        

        internal Direct3DDevice9(ComInterface.IDirect3DDevice9 obj)
        {
            this.comObject = obj;
            ComInterface.GetComMethod(this.comObject, 23, out this.createTexture);
            ComInterface.GetComMethod(this.comObject, 28, out this.createRenderTarget);
            ComInterface.GetComMethod(this.comObject, 36, out this.createOffscreenPlainSurface);
        }

        ~Direct3DDevice9()
        {
            this.Release();
        }

        public void Dispose()
        {
            this.Release();
            GC.SuppressFinalize(this);
        }

        public Direct3DSurface9 CreateRenderTarget(uint Width, uint Height, int Format, int MultiSample, uint MultisampleQuality, int Lockable, ref IntPtr pSharedHandle)
        {
            ComInterface.IDirect3DSurface9 obj = null;
            int result = this.createRenderTarget(this.comObject, Width, Height, Format, MultiSample, MultisampleQuality, Lockable, out obj, ref pSharedHandle);
            Marshal.ThrowExceptionForHR(result);

            return new Direct3DSurface9(obj);
        }

        public Direct3DTexture9 CreateTexture(uint Width, uint Height, uint Levels, uint Usage, int Format, int Pool)
        {
            IntPtr handle = new IntPtr(0);
            ComInterface.IDirect3DTexture9 obj = null;
            int result = this.createTexture(this.comObject, Width, Height, Levels, Usage, Format, Pool, out obj, ref handle);
            Marshal.ThrowExceptionForHR(result);

            return new Direct3DTexture9(obj, handle);
        }  

        public Direct3DTexture9 CreateOffscreenPlainSurface(uint Width, uint Height, int Format, int Pool)
        {
            IntPtr handle = new IntPtr(0);
            ComInterface.IDirect3DTexture9 obj = null;
            int result = this.createOffscreenPlainSurface(this.comObject, Width, Height, Format, Pool, out obj, ref handle);
            Marshal.ThrowExceptionForHR(result);

            return new Direct3DTexture9(obj, handle);
        } 

        private void Release()
        {
            if (this.comObject != null)
            {
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
                this.createRenderTarget = null;
            }
        }
    }
}
