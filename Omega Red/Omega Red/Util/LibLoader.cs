/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Util
{
    class LibLoader
    {
        private IntPtr m_handle = IntPtr.Zero;

        private LibLoader() { }

        public bool isLoaded { get { return m_handle.ToInt64() != 0; } }

        private TempFile m_TempFile = null;

        public Action Release = null;

        public void release()
        {
            if (m_handle != IntPtr.Zero)
            {
                if (Release != null)
                    Release();

                Win32NativeMethods.FreeLibrary(m_handle);

                m_handle = IntPtr.Zero;
            }

            if (m_TempFile != null)
                m_TempFile.Dispose();

            m_TempFile = null;
        }

        public static LibLoader create(string a_module_name, bool external = false)
        {
            LibLoader l_LibLoader = new LibLoader();

            do
            {
                TempFile l_TempFile = null;

                string l_path = "";

                if (!external)
                {
                    l_TempFile = TempFile.createInstance(a_module_name);

                    if (l_TempFile == null)
                        break;

                    l_path = l_TempFile.Path;
                }
                else
                {
                    l_path = a_module_name;
                }
                
                var l_handle = Win32NativeMethods.LoadLibrary(l_path);

                if (l_handle == IntPtr.Zero)
                    break;

                l_LibLoader.m_handle = l_handle;

                l_LibLoader.m_TempFile = l_TempFile;
                                
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

            if (m_TempFile != null)
                m_TempFile.Dispose();
        }

        public IntPtr getFunc(string a_func)
        {
            return Win32NativeMethods.GetProcAddress(m_handle, a_func);
        }
    }
}
