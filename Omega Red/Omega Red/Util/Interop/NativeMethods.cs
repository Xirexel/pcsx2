using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Util.Interop
{
    internal static class NativeMethods
    {
        [DllImport("d3d11.dll")]
        public static extern int D3D11CreateDevice(IntPtr a_Adapter, uint a_D3D_DRIVER_TYPE, IntPtr a_HMODULE, uint a_Flags, IntPtr a_D3D_FEATURE_LEVELs, uint a_levels_Count, uint a_D3D11_SDK_VERSION, out ComInterface.ID3D11Device a_ID3D11Device, IntPtr a_D3D_FEATURE_LEVEL, out IntPtr a_ID3D11DeviceContext);
    }
}
