using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCSXEmul.Util
{
    class LibLoader
    {
        private IntPtr m_handle = IntPtr.Zero;

        private LibLoader() { }

        public bool isLoaded { get { return m_handle.ToInt64() != 0; } }

        public Action Release = null;

        public void release()
        {
            try
            {
                if (m_handle != IntPtr.Zero)
                {
                    if (Release != null)
                        Release();

                    Win32NativeMethods.FreeLibrary(m_handle);

                    m_handle = IntPtr.Zero;
                }
            }
            catch (Exception)
            {
            }
        }

        public static LibLoader create(string a_module_name, bool external = false)
        {
            LibLoader l_LibLoader = new LibLoader();

            do
            {
                string l_path = "";

                if (!external)
                {
                    l_path = a_module_name + ".dll";
                }
                else
                {
                    l_path = a_module_name;
                }

                var l_handle = Win32NativeMethods.LoadLibrary(l_path);

                if (l_handle == IntPtr.Zero)
                    break;

                l_LibLoader.m_handle = l_handle;

            } while (false);

            return l_LibLoader;
        }

        ~LibLoader()
        {
            if (m_handle != IntPtr.Zero)
            {
                if (Release != null)
                    Release();

                Win32NativeMethods.FreeLibrary(m_handle);

                m_handle = IntPtr.Zero;
            }
        }

        public IntPtr getFunc(string a_func)
        {
            return Win32NativeMethods.GetProcAddress(m_handle, a_func);
        }
    }
}
