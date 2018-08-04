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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Util
{

    sealed class TempFile : IDisposable
    {
        private static object mLockObject = new object();

        string path = "";
        
        private TempFile(string a_module_name)
        {
            string l_arch = "x86";

            if (IntPtr.Size == 8)
                l_arch = "x64";

            Assembly l_assembly = Assembly.GetExecutingAssembly();

            Stream l_CaptureManagerStream = l_assembly.GetManifestResourceStream("Omega_Red.Modules." + l_arch + "." + a_module_name + ".dll");


            if (l_CaptureManagerStream != null && l_CaptureManagerStream.CanRead)
            {
                var lFullFilePath = System.IO.Path.GetTempFileName();

                String limageSourceDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                
                using (FileStream fs = new FileStream(lFullFilePath, FileMode.OpenOrCreate))
                {
                    var l_byte = l_CaptureManagerStream.ReadByte();

                    while (l_byte != -1)
                    {
                        fs.WriteByte((byte)l_byte);

                        l_byte = l_CaptureManagerStream.ReadByte();
                    }
                }

                if (File.Exists(lFullFilePath))
                {
                    this.path = lFullFilePath;
                }
            }
        }

        static public TempFile createInstance(string a_module_name)
        {
            lock (mLockObject)
            {
                return new TempFile(a_module_name);
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
        }
        ~TempFile()
        {

            if (path != "")
            {
                try
                {
                    File.Delete(path);
                    path = "";
                }
                catch
                {
                } // best effort

            }
        }

        public void Dispose()
        {
            if (path != "")
            {
                try { 
                    File.Delete(path); 
                }
                catch { } // best effort
                path = "";
            }
        }
    }
}