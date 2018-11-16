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
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Tools.Savestate
{
    class MemLoadingState : ILoadStateBase
    {        
        BinaryReader m_reader;

        public MemLoadingState(BinaryReader a_reader)
        {
            m_reader = a_reader;
        }

        public byte[] read()
        {
            if (m_reader == null)
                return new byte[0];

            if(m_reader.BaseStream.CanSeek)
            {
                byte[] l_result = new byte[m_reader.BaseStream.Length];

                var l_readLength = m_reader.Read(l_result, 0, l_result.Length);

                return l_result;
            }
            else
            {
                byte[] l_result = new byte[0];

                using (var ms = new MemoryStream())
                {
                    m_reader.BaseStream.CopyTo(ms);
                    ms.Position = 0; // rewind

                    l_result = new byte[ms.Length];

                    var l_readLength = ms.Read(l_result, 0, l_result.Length);
                }

                return l_result;
            }
        }
    }
}
