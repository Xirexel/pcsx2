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
using static Omega_Red.Util.XInputNative;

namespace Omega_Red.Tools
{
    public interface IPadControl
    {
        XINPUT_STATE getState();

        void setVibration(uint a_vibrationCombo);
    }

    class PadInput
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr GetTouchPadCallback(UInt32 aPadIndex);

        public class PadControlConfig
        {
            public string Title_Key { get; set; }
            public string Instance_ID { get; set; }
            public string API { get; set; }
            public string Type { get; set; }
            public string Product_ID { get; set; }
            public string[] Bindings_Data { get; set; }
            public string[] Force_Feedback_Bindings_Data { get; set; }
        }

        private PadControlConfig m_PadInputConfig = new PadControlConfig()
        {
            Title_Key = "TouchPadTitle",
            Instance_ID = "Touch Pad 0",
            API = "18",
            Type = "3",
            Product_ID = "TOUCH PAD 0",
            Bindings_Data = new string[] {

                // old
                            //"0x00200000, 0, 20, 65536, 0, 0, 1, 0, 1",
                            //"0x00200001, 0, 22, 65536, 0, 0, 1, 0, 1",
                            //"0x00200002, 0, 23, 65536, 0, 0, 1, 0, 1",
                            //"0x00200003, 0, 21, 65536, 0, 0, 1, 0, 1",
                            //"0x00200004, 0, 19, 65536, 0, 0, 1, 0, 1",
                            //"0x00200005, 0, 16, 65536, 0, 0, 1, 0, 1",
                            //"0x00200006, 0, 17, 65536, 0, 0, 1, 0, 1",
                            //"0x00200007, 0, 18, 65536, 0, 0, 1, 0, 1",
                            //"0x00200008, 0, 24, 65536, 0, 0, 1, 0, 1",
                            //"0x00200009, 0, 25, 65536, 0, 0, 1, 0, 1",
                            //"0x0020000C, 0, 29, 65536, 0, 0, 1, 0, 1",
                            //"0x0020000D, 0, 30, 65536, 0, 0, 1, 0, 1",
                            //"0x0020000E, 0, 31, 65536, 0, 0, 1, 0, 1",
                            //"0x0020000F, 0, 28, 65536, 0, 0, 1, 0, 1",
                            //"0x00200010, 0, 26, 65536, 0, 0, 1, 0, 1",
                            //"0x00200011, 0, 27, 65536, 0, 0, 1, 0, 1",
                            //"0x01020013, 0, 32, 87183, 0, 0, 13172, 1, 1",
                            //"0x02020013, 0, 35, 65536, 0, 0, 13172, 0, 1",
                            //"0x01020014, 0, 40, 65536, 0, 0, 13172, 0, 1",
                            //"0x02020014, 0, 36, 65536, 0, 0, 13172, 0, 1",
                            //"0x01020015, 0, 36, 87183, 0, 0, 13172, 1, 1",
                            //"0x02020015, 0, 37, 65536, 0, 0, 13172, 0, 1",
                            //"0x01020016, 0, 38, 65536, 0, 0, 13172, 0, 1",
                            //"0x02020016, 0, 39, 65536, 0, 0, 13172, 0, 1"

                                                        
                            "0x0020000C, 0, 30, 65536, 0, 0, 1, 1",     // cross
                            "0x0020000D, 0, 29, 65536, 0, 0, 1, 1",     // circle
                            "0x0020000E, 0, 31, 65536, 0, 0, 1, 1",     // square
                            "0x0020000F, 0, 28, 65536, 0, 0, 1, 1",     // triangle
                            "0x00200005, 0, 16, 65536, 0, 0, 1, 0, 1",     // select/back      
                            "0x00200004, 0, 19, 65536, 0, 0, 1, 0, 1",    // start
							
                            "0x00200000, 0, 20, 65536, 0, 0, 1, 0, 1",     // D-Pad Up
                            "0x00200003, 0, 21, 65536, 0, 0, 1, 0, 1",     // D-Pad Right
                            "0x00200001, 0, 22, 65536, 0, 0, 1, 0, 1",     // D-Pad Down
                            "0x00200002, 0, 23, 65536, 0, 0, 1, 0, 1",     // D-Pad Left
														
                            "0x01020014, 0, 32, 65536, 0, 0, 13172, 1",	// L-Stick Up
                            "0x01020013, 0, 33, 65536, 0, 0, 13172, 1",	// L-Stick Right
                            "0x02020014, 0, 34, 65536, 0, 0, 13172, 1",	// L-Stick Down
                            "0x02020013, 0, 35, 65536, 0, 0, 13172, 1",	// L-Stick Left

                            "0x01020016, 0, 36, 65536, 0, 0, 13172, 1",	// R-Stick Up
                            "0x01020015, 0, 37, 65536, 0, 0, 13172, 1",	// R-Stick Right
                            "0x02020016, 0, 38, 65536, 0, 0, 13172, 1",	// R-Stick Down
                            "0x02020015, 0, 39, 65536, 0, 0, 13172, 1",	// R-Stick Left


                            "0x00200008, 0, 26, 65536, 0, 0, 1, 1",    	// L1
                            "0x00200010, 0, 24, 65536, 0, 0, 1, 1",		// L2
                            "0x00200006, 0, 17, 65536, 0, 0, 1, 1",    	// L3
                            "0x00200009, 0, 27, 65536, 0, 0, 1, 1",    	// R1
                            "0x00200011, 0, 25, 65536, 0, 0, 1, 1",		// R2
                            "0x00200007, 0, 18, 65536, 0, 0, 1, 1",    	// R3
            
            },
            Force_Feedback_Bindings_Data = new string[] {
                            "Constant 0, 0, 0, 1, 0, 65536, 1, 0",
                            "Constant 0, 1, 0, 1, 0, 0, 1, 65536"}
        };
        
        public PadControlConfig PadInputConfig { get { return m_PadInputConfig; } }

        public IPadControl PadControl { private get; set; }
        
        private IntPtr m_Ptr = IntPtr.Zero;

        private GetTouchPadCallback m_GetTouchPadCallback = null;

        private IntPtr m_TouchPadCallbackHandler = IntPtr.Zero;

        public GetTouchPadCallback TouchPadCallback { get { return m_GetTouchPadCallback; } }

        public IntPtr TouchPadCallbackHandler { get { return m_TouchPadCallbackHandler; } }


        private static PadInput m_PadInput = new PadInput();

        private PadInput() {

            m_GetTouchPadCallback = getTouchPad;

            m_TouchPadCallbackHandler = Marshal.GetFunctionPointerForDelegate(m_GetTouchPadCallback);

            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(XINPUT_STATE));

            m_Ptr = Marshal.AllocHGlobal(size);
        }

        private IntPtr getTouchPad(UInt32 aPadIndex)
        {
            var l_state = PadControl.getState();

            if(Managers.AdditionalControlManager.Instance.ButtonCheck(l_state.Gamepad.wButtons, PadControl))
                return m_Ptr;

            Marshal.StructureToPtr(l_state, m_Ptr, false);

            return m_Ptr;
        }

        ~PadInput()
        {
            if (m_Ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(m_Ptr);

            m_Ptr = IntPtr.Zero;
        }

        public static PadInput Instance { get { return m_PadInput; } }
    }
}
