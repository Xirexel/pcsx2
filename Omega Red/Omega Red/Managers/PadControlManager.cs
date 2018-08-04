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

using Omega_Red.Models;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Omega_Red.Managers
{
    class PadControlManager
    {
        private static PadControlManager m_Instance = null;

        public static PadControlManager Instance { get { if (m_Instance == null) m_Instance = new PadControlManager(); return m_Instance; } }

        private PadControlInfo mPadControlInfo = new PadControlInfo(){
            Title_Key = "TouchPadTitle", 
            IsTouchPad=true,
            Instance_ID="Touch Pad 0",
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
        
        private PadControlManager() {
            
            mCustomerView = CollectionViewSource.GetDefaultView(
                new PadControlInfo[] { 
                    mPadControlInfo, 
                    new PadControlInfo(){ 
                    Title_Key = "GamePadTitle", 
                    IsTouchPad=false,
                    Instance_ID="XInput Pad 0",
                    API = "4",
                    Type = "3",
                    Product_ID = "XInput PAD 0",
                    Bindings_Data = new string[] {
                        
                            "0x0020000C, 0, 30, 65536, 0, 0, 1, 1",     // cross
                            "0x0020000D, 0, 29, 65536, 0, 0, 1, 1",     // circle
                            "0x0020000E, 0, 31, 65536, 0, 0, 1, 1",     // square
                            "0x0020000F, 0, 28, 65536, 0, 0, 1, 1",     // triangle
                            "0x00200005, 0, 16, 65536, 0, 0, 1, 2",     // select/back      
                            "0x00200004, 0, 19, 65536, 0, 0, 1, 2",     // start
							
                            "0x00200000, 0, 20, 65536, 0, 0, 1, 2",     // D-Pad Up
                            "0x00200003, 0, 21, 65536, 0, 0, 1, 1",     // D-Pad Right
                            "0x00200001, 0, 22, 65536, 0, 0, 1, 2",     // D-Pad Down
                            "0x00200002, 0, 23, 65536, 0, 0, 1, 1",     // D-Pad Left
														
                            "0x01020014, 0, 32, 87183, 0, 0, 13172, 1",	// L-Stick Up
                            "0x01020013, 0, 33, 87183, 0, 0, 13172, 1",	// L-Stick Right
                            "0x02020014, 0, 34, 87183, 0, 0, 13172, 1",	// L-Stick Down
                            "0x02020013, 0, 35, 87183, 0, 0, 13172, 1",	// L-Stick Left

                            "0x01020016, 0, 36, 87183, 0, 0, 13172, 1",	// R-Stick Up
                            "0x01020015, 0, 37, 87183, 0, 0, 13172, 1",	// R-Stick Right
                            "0x02020016, 0, 38, 87183, 0, 0, 13172, 1",	// R-Stick Down
                            "0x02020015, 0, 39, 87183, 0, 0, 13172, 1",	// R-Stick Left


                            "0x00200008, 0, 26, 65536, 0, 0, 1, 1",    	// L1
                            "0x00200010, 0, 24, 65536, 0, 0, 1, 1",		// L2
                            "0x00200006, 0, 17, 65536, 0, 0, 1, 1",    	// L3
                            "0x00200009, 0, 27, 65536, 0, 0, 1, 1",    	// R1
                            "0x00200011, 0, 25, 65536, 0, 0, 1, 1",		// R2
                            "0x00200007, 0, 18, 65536, 0, 0, 1, 1",    	// R3


                            //"0x0020000C, 0, 30, 65536, 0, 0, 0, 0, 1",    // cross
                            //"0x0020000D, 0, 29, 65536, 0, 0, 0, 0, 1",    // circle
                            //"0x0020000E, 0, 31, 65536, 0, 0, 0, 0, 1",    // square
                            //"0x0020000F, 0, 28, 65536, 0, 0, 0, 0, 1",    // triangle
                            //"0x00200005, 0, 16, 65536, 0, 0, 0, 0, 1",    // select/back      
                            //"0x00200004, 0, 19, 65536, 0, 0, 0, 0, 1",    // start
							
                            //"0x00200000, 0, 20, 65536, 0, 0, 13172, 0, 1",// D-Pad Up
                            //"0x00200003, 0, 21, 65536, 0, 0, 13172, 0, 1",// D-Pad Right
                            //"0x00200001, 0, 22, 65536, 0, 0, 13172, 0, 1",// D-Pad Down
                            //"0x00200002, 0, 23, 65536, 0, 0, 13172, 0, 1",// D-Pad Left
														
                            //"0x02020001, 0, 32, 65536, 0, 0, 13172",// L-Stick Up
                            //"0x01020000, 0, 33, 65536, 0, 0, 13172",// L-Stick Right
                            //"0x01020001, 0, 34, 65536, 0, 0, 13172",// L-Stick Down
                            //"0x02020000, 0, 35, 65536, 0, 0, 13172",// L-Stick Left

                            //"0x02020004, 0, 36, 65536, 0, 0, 13172",// R-Stick Up
                            //"0x01020003, 0, 37, 65536, 0, 0, 13172",// R-Stick Right
                            //"0x01020004, 0, 38, 65536, 0, 0, 13172",// R-Stick Down
                            //"0x02020003, 0, 39, 65536, 0, 0, 13172",// R-Stick Left


                            //"0x00040004, 0, 26, 65536, 0, 0, 0",    // L1
                            //"0x01020002, 0, 24, 65536, 0, 0, 13172",// L2
                            //"0x00040008, 0, 17, 65536, 0, 0, 0",    // L3
                            //"0x00040005, 0, 27, 65536, 0, 0, 0",    // R1
                            //"0x02020002, 0, 25, 65536, 0, 0, 13172",// R2
                            //"0x00040009, 0, 18, 65536, 0, 0, 0",    // R3

                    },
                    Force_Feedback_Bindings_Data = new string[] {
                                    "Constant 0, 0, 0, 1, 0, 65536, 1, 0",
                                    "Constant 0, 1, 0, 1, 0, 0, 1, 65536"
                    }
                    }, 
                    //new PadControlInfo(){ Display_Name="Game Pad (DirectX Input)", 
                    //IsTouchPad=false,
                    //Instance_ID="DI Pad 0",
                    //API = "1",
                    //Type = "3",
                    //Product_ID = "DI PAD 0",
                    //Bindings_Data = new string[] {
                        
                    //        "0x00040000, 0, 30, 65536, 0, 0, 0",     // cross
                    //        "0x00040001, 0, 29, 65536, 0, 0, 0",     // circle
                    //        "0x00040002, 0, 31, 65536, 0, 0, 0",     // square
                    //        "0x00040003, 0, 28, 65536, 0, 0, 0",     // triangle
                    //        "0x00040006, 0, 16, 65536, 0, 0, 0",     // select/back      
                    //        "0x00040007, 0, 19, 65536, 0, 0, 0",     // start
							
                    //        "0x03100000, 0, 20, 65536, 0, 0, 13172",     // D-Pad Up
                    //        "0x04100000, 0, 21, 65536, 0, 0, 13172",     // D-Pad Right
                    //        "0x05100000, 0, 22, 65536, 0, 0, 13172",     // D-Pad Down
                    //        "0x06100000, 0, 23, 65536, 0, 0, 13172",     // D-Pad Left
														
                    //        "0x02020001, 0, 32, 65536, 0, 0, 13172",	// L-Stick Up
                    //        "0x01020000, 0, 33, 65536, 0, 0, 13172",	// L-Stick Right
                    //        "0x01020001, 0, 34, 65536, 0, 0, 13172",	// L-Stick Down
                    //        "0x02020000, 0, 35, 65536, 0, 0, 13172",	// L-Stick Left

                    //        "0x02020004, 0, 36, 65536, 0, 0, 13172",	// R-Stick Up
                    //        "0x01020003, 0, 37, 65536, 0, 0, 13172",	// R-Stick Right
                    //        "0x01020004, 0, 38, 65536, 0, 0, 13172",	// R-Stick Down
                    //        "0x02020003, 0, 39, 65536, 0, 0, 13172",	// R-Stick Left


                    //        "0x00040004, 0, 26, 65536, 0, 0, 0",    	// L1
                    //        "0x01020002, 0, 24, 65536, 0, 0, 13172",		// L2
                    //        "0x00040008, 0, 17, 65536, 0, 0, 0",    	// L3
                    //        "0x00040005, 0, 27, 65536, 0, 0, 0",    	// R1
                    //        "0x02020002, 0, 25, 65536, 0, 0, 13172",		// R2
                    //        "0x00040009, 0, 18, 65536, 0, 0, 0",    	// R3

                    //}
                    //}                 
                }

                );


            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                (System.Threading.ThreadStart)delegate()
                {
                    mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;

                    mCustomerView.MoveCurrentToPosition(Omega_Red.Properties.Settings.Default.PadIndex);
                });

        }

        void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            select(mCustomerView.CurrentItem as PadControlInfo);
        }

        public PadControlInfo PadControlInfo { get { return mPadControlInfo; } }

        public event Action<bool> ShowTouchPadPanelEvent;
        
        private ICollectionView mCustomerView = null;

        public ICollectionView Collection
        {
            get { return mCustomerView; }
        }

        private void select(PadControlInfo a_PadControlInfo)
        {
            if (a_PadControlInfo != null)
            {
                mPadControlInfo = a_PadControlInfo;

                if (ShowTouchPadPanelEvent != null)
                    ShowTouchPadPanelEvent(a_PadControlInfo.IsTouchPad);

                Omega_Red.Properties.Settings.Default.PadIndex = mCustomerView.CurrentPosition;

                Omega_Red.Properties.Settings.Default.Save();

                ModuleControl.Instance.closePad();

                ModuleControl.Instance.shutdownPad();

                ModuleControl.Instance.initPad();
            }
        }
    }
}
