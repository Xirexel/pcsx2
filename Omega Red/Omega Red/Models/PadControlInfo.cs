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

using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Models
{
    public enum PadType
    {
        TouchPad,
        XInputPad,
        DirectInputPad
    }

    interface PadControlInfo
    {
        string Title_Key { get; set; }

        PadType PadType { get; set; }

        Tools.IPadControl PadControl { get; set; }

        object PadConfigPanel { get; set; }

        void stopTimer();
    }

    class TouchPadControlInfo: PadControlInfo
    {
        public TouchPadControlInfo()
        {
            PadType = Models.PadType.TouchPad;
            Title_Key = "TouchPadTitle";
            PadConfigPanel = new Panels.TouchPadConfigPanel();
        }

        public string Title_Key { get; set; }
        public PadType PadType { get; set; }
        public IPadControl PadControl { get; set; }
        public object PadConfigPanel { get; set; }

        public void stopTimer()
        {
        }

        public override string ToString()
        {
            if (PadType == PadType.TouchPad)
                return App.Current.Resources[Title_Key] as String;
            else
                return Title_Key;
        }
    }
}
