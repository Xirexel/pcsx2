using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Util.Interop
{
    internal sealed class D3D11Texture2D : IDisposable
    {
        private ComInterface.ID3D11Texture2D comObject;
        private IntPtr native;

        public IntPtr Native { get { return native; } }


        internal D3D11Texture2D(ComInterface.ID3D11Texture2D obj)
        {
            this.comObject = obj;
            this.native = Marshal.GetIUnknownForObject(this.comObject);
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
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
            }
        }
               
        public DXGIResource getDXGIResource()
        {
            ComInterface.IDXGIResource l_obj = null;

            l_obj = comObject as ComInterface.IDXGIResource;

            return new DXGIResource(l_obj);
        }

    }
}
