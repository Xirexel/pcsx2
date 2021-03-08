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

using Omega_Red.Emulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Models
{
    public class ContainerFile
    {
        public ContainerFile()
        { }
        public ContainerFile(string a_fileName)
        {
            m_fileName = a_fileName;
        }

        private string m_fileName = "";

        public override string ToString()
        {
            return m_fileName;
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is ContainerFile)
            {
                return (obj as ContainerFile).m_fileName == m_fileName;
            }

            return false;
        }
    }

    public class BiosInfo
    {
        public string Zone { get; set; }

        public string Version { get; set; }

        public int VersionInt { get; set; }

        public string Data { get; set; }

        public string Build { get; set; }

        public uint CheckSum { get; set; }

        public string FilePath { get; set; }

        public bool IsCurrent { get; set; }

        public byte[] NVM { get; set; }

        public byte[] MEC { get; set; }

        public GameType GameType { get; set; }

        public ContainerFile ContainerFile { get; set; } = new ContainerFile();
    }
}
