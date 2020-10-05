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

namespace Omega_Red.Models
{
    class PadControlInfo
    {
        public string Title_Key { get; set; }
        public bool IsTouchPad { get; set; }
        public string Instance_ID { get; set; }
        public string API { get; set; }
        public string Type { get; set; }
        public string Product_ID { get; set; }
        public string[] Bindings_Data { get; set; }
        public string[] Force_Feedback_Bindings_Data { get; set; }

        public override string ToString()
        {
            return App.Current.Resources[Title_Key] as String;
        }
    }
}
