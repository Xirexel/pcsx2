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
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Omega_Red.Tools.Converters
{
    class WideScreenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            System.Windows.Visibility l_result = System.Windows.Visibility.Collapsed;

            do
	        {
                uint l_ElfCRC = (uint)value;

                if(l_ElfCRC == 0)
                    break;

                if(IsWideScreen(l_ElfCRC.ToString("x")))
                    l_result = System.Windows.Visibility.Visible;
	         
	        } while (false);

            return l_result;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }

        public static bool IsWideScreen(string gameCRC)
        {
            bool lresult = false;

            Assembly l_assembly = Assembly.GetExecutingAssembly();

            Stream l_Cheats_WSStream = l_assembly.GetManifestResourceStream("Omega_Red.Assests.cheats_ws.zip");

            if (l_Cheats_WSStream != null && l_Cheats_WSStream.CanRead)
            {
                using (ZipArchive archive = new ZipArchive(l_Cheats_WSStream, ZipArchiveMode.Read))
                {
                    foreach (var item in archive.Entries)
                    {
                        if (item.Name.ToLower().Contains(gameCRC.ToLower()) && item.Name.ToLower().Contains(".pnach"))
                        {
                            StreamReader lg = new StreamReader(item.Open());

                            var lpatchesString = lg.ReadToEnd();

                            lresult = !string.IsNullOrEmpty(lpatchesString);
                        }
                    }

                }

            }

            return lresult;
        }
    }
}

