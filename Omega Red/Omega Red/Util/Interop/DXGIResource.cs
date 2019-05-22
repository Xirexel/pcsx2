using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Util.Interop
{
    internal sealed class DXGIResource : IDisposable
    {
        private ComInterface.IDXGIResource comObject;
        private ComInterface.GetSharedHandle getSharedHandle;

        internal DXGIResource(ComInterface.IDXGIResource obj)
        {
            this.comObject = obj;
            ComInterface.GetComMethod(this.comObject, 8, out this.getSharedHandle);
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
                System.Runtime.InteropServices.Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
                this.getSharedHandle = null;
            }
        }

        public IntPtr GetSharedHandle()
        {
            IntPtr l_handler = IntPtr.Zero;
            
            int l_result = getSharedHandle(this.comObject, out l_handler);

            return l_handler;
        }
    }
}
