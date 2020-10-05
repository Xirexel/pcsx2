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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Tools
{
    public class PadInput
    {
        public struct XY_Axises
        {
            public short m_x_axis;

            public short m_y_axis;
        }

        private XINPUT_STATE m_state = new XINPUT_STATE();

        private IntPtr m_Ptr = IntPtr.Zero;

        public IntPtr Hanler { get { return m_Ptr; } }

        private static PadInput m_PadInput = new PadInput();

        private PadInput() {

            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(XINPUT_STATE));

            m_Ptr = Marshal.AllocHGlobal(size);

            update();
        }

        ~PadInput()
        {
            if (m_Ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(m_Ptr);

            m_Ptr = IntPtr.Zero;
        }

        public static PadInput Instance { get { return m_PadInput; } }

        public void setKey(UInt16 a_Key)
        {
            m_state.Gamepad.wButtons |= a_Key;

            update();
        }

        public void setLeftStickAxises(XY_Axises a_Axises)
        {
            m_state.Gamepad.sThumbLX = a_Axises.m_x_axis;

            m_state.Gamepad.sThumbLY = a_Axises.m_y_axis;
            
            update();
        }

        public void setLeftAnalogTrigger(byte a_value)
        {
            m_state.Gamepad.bLeftTrigger = a_value;

            update();
        }

        public void setRightAnalogTrigger(byte a_value)
        {
            m_state.Gamepad.bRightTrigger = a_value;

            update();
        }

        public void setRightStickAxises(XY_Axises a_Axises)
        {
            m_state.Gamepad.sThumbRX = a_Axises.m_x_axis;

            m_state.Gamepad.sThumbRY = a_Axises.m_y_axis;

            update();
        }

        public void removeKey(UInt16 a_Key)
        {
            m_state.Gamepad.wButtons &= (ushort)(~a_Key);

            update();
        }

        public void enable(bool a_state)
        {
            if(a_state)
                m_state.dwPacketNumber = 0;
            else
                m_state.dwPacketNumber = uint.MaxValue;

            update();
        }

        private void update()
        {
            Marshal.StructureToPtr(m_state, m_Ptr, false);
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct XINPUT_GAMEPAD {
            public UInt16 wButtons;
          public byte bLeftTrigger;
          public byte bRightTrigger;
          public Int16 sThumbLX;
          public Int16 sThumbLY;
          public Int16 sThumbRX;
          public Int16 sThumbRY;
        };
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct XINPUT_STATE {
            public UInt32 dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        };
    }
}
