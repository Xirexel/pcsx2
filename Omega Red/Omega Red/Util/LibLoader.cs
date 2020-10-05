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
