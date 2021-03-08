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
using System.Windows;

namespace Omega_Red.Models
{
    public enum SaveStateType
    {
        PCSX,
        PCSX2,
        PPSSPP
    }

    public class SaveStateInfo
    {
        public bool     IsAutosave { get; set; }
        public bool     IsQuicksave { get; set; }
        public bool     IsCloudOnlysave { get; set; }
        public bool     IsCloudsave { get; set; }
        public bool     Focusable { get { return false; } }
        public string Date { get; set; }
        public DateTime DateTime { get; set; }
        public string Duration { get; set; }
        public TimeSpan DurationNative { get; set; }
        public uint     CheckSum { get; set; }
        public int      Index { get; set; }
        public string   FilePath { get; set; }
        public bool     IsCurrent { get; set; }
        public Visibility Visibility { get; set; }
        public System.Windows.Media.ImageSource ImageSource { get; set; }
        public GameType Type { get; set; }
        public object Item { get; set; }
        public string DiscSerial { get; set; }


        public string CloudSaveDate { get; set; }
        public string CloudSaveDuration { get; set; }
    }
}
