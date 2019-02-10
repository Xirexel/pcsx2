using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Panels.Video.Interop
{
    class Direct3DTexture9 : IDisposable
    {
        private ComInterface.IDirect3DTexture9 comObject;
        private IntPtr native;
        private ComInterface.GetLevelCount      getLevelCount;
        private ComInterface.GetSurfaceLevel    getSurfaceLevel;
        private IntPtr m_sharedhandle;

        //public ComInterface.IDirect3DTexture9   texture { get { return comObject; } }
        public IntPtr                           SharedHandle { get { return m_sharedhandle; } }

        internal Direct3DTexture9(ComInterface.IDirect3DTexture9 obj, IntPtr sharedhandle)
        {
            this.comObject = obj;
            this.native = Marshal.GetIUnknownForObject(this.comObject);
            this.m_sharedhandle = sharedhandle;
            ComInterface.GetComMethod(this.comObject, 13, out this.getLevelCount);
            ComInterface.GetComMethod(this.comObject, 18, out this.getSurfaceLevel);
        }

        ~Direct3DTexture9()
        {
            this.Release();
        }

        public uint GetLevelCount()
        {
            ComInterface.IDirect3DTexture9 obj = null;
            uint result = this.getLevelCount(this.comObject);
            return result;
        }

        public Direct3DSurface9 GetSurfaceLevel(uint level)
        {
            ComInterface.IDirect3DSurface9 obj = null;
            int result = this.getSurfaceLevel(this.comObject, level, out obj);
            Marshal.ThrowExceptionForHR(result);

            return new Direct3DSurface9(obj);
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
