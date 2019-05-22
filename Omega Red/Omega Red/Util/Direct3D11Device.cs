using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Util
{
    class Direct3D11Device
    {
        private Interop.Direct3D11 m_device = null;

        public Interop.Direct3D11 Device { get { return m_device; } }
        public IntPtr Native { get { return m_device.Native; } }

        private static Direct3D11Device m_Instance = null;

        public static Direct3D11Device Instance { get { if (m_Instance == null) m_Instance = new Direct3D11Device(); return m_Instance; } }

        private Direct3D11Device()
        {
            try
            {
                m_device = Interop.Direct3D11.Create();
            }
            catch (Exception)
            {
            }
        }

        ~Direct3D11Device()
        {
            if (m_device != null)
                m_device.Dispose();
        }
    }
}
