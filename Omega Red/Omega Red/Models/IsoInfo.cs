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
    public class IsoInfo
    {
        public string Title { get; set; }

        public string IsoType { get; set; }

        public string GameDiscType { get; set; }

        public string DiscSerial { get; set; }

        public string DiscRegionType { get; set; }

        public string SoftwareVersion { get; set; }

        public uint    ElfCRC { get; set; }
        
        public string FilePath { get; set; }

        public bool IsCurrent { get; set; }

        public GameType GameType { get; set; }

        public bool IsCloudsave { get; set; }

        public bool IsCloudOnlysave { get; set; }

        public string SelectedMemoryCardFile { get; set; }
    }
}
