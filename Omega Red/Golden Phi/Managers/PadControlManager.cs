using Golden_Phi.Models;
using Golden_Phi.Panels;
using Golden_Phi.Tools;
using Golden_Phi.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using static Golden_Phi.Tools.PadInput;

namespace Golden_Phi.Managers
{
    delegate void VibrationCallbackDelegate(uint a_vibrationCombo);

    class PadControlManager
    {
        private static PadControlManager m_Instance = null;

        public event Action<object> PadConfigPanelEvent;

        public event Action<string> ShowWarningEvent;

        public VibrationCallbackDelegate VibrationCallback = null;

        private Tools.IPadControl m_currentPadControl = null;

        private readonly ObservableCollection<PadControlInfo> _padControlInfoCollection = new ObservableCollection<PadControlInfo>();

        private IList<uint> m_XInputGamePadList = new List<uint>();

        private System.Timers.Timer m_check_connecting_pad_Timer = new System.Timers.Timer(2000);

        private System.Timers.Timer m_check_vibration_activity_pad_Timer = new System.Timers.Timer(250);

        private bool m_is_vibration_activity = false;


        public static PadControlManager Instance { get { if (m_Instance == null) m_Instance = new PadControlManager(); return m_Instance; } }



        private PadControlManager()
        {
            VibrationCallback = VibrationCallbackInner;

            //load();

            mCustomerView = CollectionViewSource.GetDefaultView(_padControlInfoCollection);

            mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;


            m_check_connecting_pad_Timer.Elapsed += check_connecting_pad_Elapsed;

            m_check_connecting_pad_Timer.AutoReset = true;

            m_check_connecting_pad_Timer.Start();



            m_check_vibration_activity_pad_Timer.Elapsed += (sender, e) => {

                if (!m_is_vibration_activity)
                {
                    if (m_currentPadControl != null)
                        m_currentPadControl.setVibration(0);

                    m_check_vibration_activity_pad_Timer.Stop();
                }

                m_is_vibration_activity = false;
            };

            m_check_vibration_activity_pad_Timer.AutoReset = true;





            //mCustomerView = CollectionViewSource.GetDefaultView(
            //    new PadControlInfo[] {
            //        mTouchPadControlInfo, 
            //        //new PadControlInfo(){ 
            //        //Title_Key = "GamePadTitle", 
            //        //IsTouchPad=false,
            //        //Instance_ID="XInput Pad 0",
            //        //API = "4",
            //        //Type = "3",
            //        //Product_ID = "XInput PAD 0",
            //        //Bindings_Data = new string[] {

            //        //        "0x0020000C, 0, 30, 65536, 0, 0, 1, 1",     // cross
            //        //        "0x0020000D, 0, 29, 65536, 0, 0, 1, 1",     // circle
            //        //        "0x0020000E, 0, 31, 65536, 0, 0, 1, 1",     // square
            //        //        "0x0020000F, 0, 28, 65536, 0, 0, 1, 1",     // triangle
            //        //        "0x00200005, 0, 16, 65536, 0, 0, 1, 2",     // select/back      
            //        //        "0x00200004, 0, 19, 65536, 0, 0, 1, 2",     // start

            //        //        "0x00200000, 0, 20, 65536, 0, 0, 1, 2",     // D-Pad Up
            //        //        "0x00200003, 0, 21, 65536, 0, 0, 1, 1",     // D-Pad Right
            //        //        "0x00200001, 0, 22, 65536, 0, 0, 1, 2",     // D-Pad Down
            //        //        "0x00200002, 0, 23, 65536, 0, 0, 1, 1",     // D-Pad Left

            //        //        "0x01020014, 0, 32, 87183, 0, 0, 13172, 1",	// L-Stick Up
            //        //        "0x01020013, 0, 33, 87183, 0, 0, 13172, 1",	// L-Stick Right
            //        //        "0x02020014, 0, 34, 87183, 0, 0, 13172, 1",	// L-Stick Down
            //        //        "0x02020013, 0, 35, 87183, 0, 0, 13172, 1",	// L-Stick Left

            //        //        "0x01020016, 0, 36, 87183, 0, 0, 13172, 1",	// R-Stick Up
            //        //        "0x01020015, 0, 37, 87183, 0, 0, 13172, 1",	// R-Stick Right
            //        //        "0x02020016, 0, 38, 87183, 0, 0, 13172, 1",	// R-Stick Down
            //        //        "0x02020015, 0, 39, 87183, 0, 0, 13172, 1",	// R-Stick Left


            //        //        "0x00200008, 0, 26, 65536, 0, 0, 1, 1",    	// L1
            //        //        "0x00200010, 0, 24, 65536, 0, 0, 1, 1",		// L2
            //        //        "0x00200006, 0, 17, 65536, 0, 0, 1, 1",    	// L3
            //        //        "0x00200009, 0, 27, 65536, 0, 0, 1, 1",    	// R1
            //        //        "0x00200011, 0, 25, 65536, 0, 0, 1, 1",		// R2
            //        //        "0x00200007, 0, 18, 65536, 0, 0, 1, 1",    	// R3


            //        //        //"0x0020000C, 0, 30, 65536, 0, 0, 0, 0, 1",    // cross
            //        //        //"0x0020000D, 0, 29, 65536, 0, 0, 0, 0, 1",    // circle
            //        //        //"0x0020000E, 0, 31, 65536, 0, 0, 0, 0, 1",    // square
            //        //        //"0x0020000F, 0, 28, 65536, 0, 0, 0, 0, 1",    // triangle
            //        //        //"0x00200005, 0, 16, 65536, 0, 0, 0, 0, 1",    // select/back      
            //        //        //"0x00200004, 0, 19, 65536, 0, 0, 0, 0, 1",    // start

            //        //        //"0x00200000, 0, 20, 65536, 0, 0, 13172, 0, 1",// D-Pad Up
            //        //        //"0x00200003, 0, 21, 65536, 0, 0, 13172, 0, 1",// D-Pad Right
            //        //        //"0x00200001, 0, 22, 65536, 0, 0, 13172, 0, 1",// D-Pad Down
            //        //        //"0x00200002, 0, 23, 65536, 0, 0, 13172, 0, 1",// D-Pad Left

            //        //        //"0x02020001, 0, 32, 65536, 0, 0, 13172",// L-Stick Up
            //        //        //"0x01020000, 0, 33, 65536, 0, 0, 13172",// L-Stick Right
            //        //        //"0x01020001, 0, 34, 65536, 0, 0, 13172",// L-Stick Down
            //        //        //"0x02020000, 0, 35, 65536, 0, 0, 13172",// L-Stick Left

            //        //        //"0x02020004, 0, 36, 65536, 0, 0, 13172",// R-Stick Up
            //        //        //"0x01020003, 0, 37, 65536, 0, 0, 13172",// R-Stick Right
            //        //        //"0x01020004, 0, 38, 65536, 0, 0, 13172",// R-Stick Down
            //        //        //"0x02020003, 0, 39, 65536, 0, 0, 13172",// R-Stick Left


            //        //        //"0x00040004, 0, 26, 65536, 0, 0, 0",    // L1
            //        //        //"0x01020002, 0, 24, 65536, 0, 0, 13172",// L2
            //        //        //"0x00040008, 0, 17, 65536, 0, 0, 0",    // L3
            //        //        //"0x00040005, 0, 27, 65536, 0, 0, 0",    // R1
            //        //        //"0x02020002, 0, 25, 65536, 0, 0, 13172",// R2
            //        //        //"0x00040009, 0, 18, 65536, 0, 0, 0",    // R3

            //        //},
            //        //Force_Feedback_Bindings_Data = new string[] {
            //        //                "Constant 0, 0, 0, 1, 0, 65536, 1, 0",
            //        //                "Constant 0, 1, 0, 1, 0, 0, 1, 65536"
            //        //}
            //        //}, 
            //        //new PadControlInfo(){ Display_Name="Game Pad (DirectX Input)", 
            //        //IsTouchPad=false,
            //        //Instance_ID="DI Pad 0",
            //        //API = "1",
            //        //Type = "3",
            //        //Product_ID = "DI PAD 0",
            //        //Bindings_Data = new string[] {

            //        //        "0x00040000, 0, 30, 65536, 0, 0, 0",     // cross
            //        //        "0x00040001, 0, 29, 65536, 0, 0, 0",     // circle
            //        //        "0x00040002, 0, 31, 65536, 0, 0, 0",     // square
            //        //        "0x00040003, 0, 28, 65536, 0, 0, 0",     // triangle
            //        //        "0x00040006, 0, 16, 65536, 0, 0, 0",     // select/back      
            //        //        "0x00040007, 0, 19, 65536, 0, 0, 0",     // start

            //        //        "0x03100000, 0, 20, 65536, 0, 0, 13172",     // D-Pad Up
            //        //        "0x04100000, 0, 21, 65536, 0, 0, 13172",     // D-Pad Right
            //        //        "0x05100000, 0, 22, 65536, 0, 0, 13172",     // D-Pad Down
            //        //        "0x06100000, 0, 23, 65536, 0, 0, 13172",     // D-Pad Left

            //        //        "0x02020001, 0, 32, 65536, 0, 0, 13172",	// L-Stick Up
            //        //        "0x01020000, 0, 33, 65536, 0, 0, 13172",	// L-Stick Right
            //        //        "0x01020001, 0, 34, 65536, 0, 0, 13172",	// L-Stick Down
            //        //        "0x02020000, 0, 35, 65536, 0, 0, 13172",	// L-Stick Left

            //        //        "0x02020004, 0, 36, 65536, 0, 0, 13172",	// R-Stick Up
            //        //        "0x01020003, 0, 37, 65536, 0, 0, 13172",	// R-Stick Right
            //        //        "0x01020004, 0, 38, 65536, 0, 0, 13172",	// R-Stick Down
            //        //        "0x02020003, 0, 39, 65536, 0, 0, 13172",	// R-Stick Left


            //        //        "0x00040004, 0, 26, 65536, 0, 0, 0",    	// L1
            //        //        "0x01020002, 0, 24, 65536, 0, 0, 13172",		// L2
            //        //        "0x00040008, 0, 17, 65536, 0, 0, 0",    	// L3
            //        //        "0x00040005, 0, 27, 65536, 0, 0, 0",    	// R1
            //        //        "0x02020002, 0, 25, 65536, 0, 0, 13172",		// R2
            //        //        "0x00040009, 0, 18, 65536, 0, 0, 0",    	// R3

            //        //}
            //        //}                 
            //    }
            //    );

            //if(XInputNative.Instance.isInit)
            //{
            //    XInputNative.XINPUT_CAPABILITIES l_capabilities = new XInputNative.XINPUT_CAPABILITIES();

            //    XInputNative.Instance.XInputGetCapabilities(0, 0, ref l_capabilities);
            //}

            //if(DirectInput8Native.Instance.isInit)
            //{
            //    Omega_Red.Util.IDirectInput8 l_directInput8;

            //    DirectInput8Native.Instance.DirectInput8Create(out l_directInput8);

            //    if (l_directInput8 != null)
            //        l_directInput8.enumGamePads();
            //}
        }

        private void check_connecting_pad_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            IList<uint> l_XInputGamePadList = new List<uint>();

            IList<uint> l_removeXInputGamePadList = new List<uint>();

            if (XInputNative.Instance.isInit)
            {
                for (uint i = 0; i < XInputNative.XUSER_MAX_COUNT; i++)
                {
                    XInputNative.XINPUT_CAPABILITIES l_capabilities = new XInputNative.XINPUT_CAPABILITIES();

                    if (XInputNative.Instance.XInputGetCapabilities(i, 0, ref l_capabilities) == XInputNative.ERROR_SUCCESS)
                    {
                        if (l_capabilities.Type == XInputNative.XINPUT_DEVTYPE_GAMEPAD &&
                           l_capabilities.SubType == XInputNative.XINPUT_DEVSUBTYPE_GAMEPAD)
                        {
                            l_XInputGamePadList.Add(i);
                        }
                        else
                        {
                            l_removeXInputGamePadList.Add(i);
                        }
                    }
                    else
                    {
                        l_removeXInputGamePadList.Add(i);
                    }
                }
            }

            System.Windows.Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Background,
            (System.Threading.ThreadStart)delegate ()
            {
                foreach (var item in l_removeXInputGamePadList)
                {

                    var l_padControlInfo = _padControlInfoCollection.FirstOrDefault((a_item) => {

                        if (a_item.PadType == PadType.XInputPad)
                        {
                            var l_pad = (a_item as XInputPadControlInfoPanel);

                            if (l_pad != null)
                            {
                                return l_pad.DeviceIndex == item;
                            }

                            return false;
                        }
                        else
                            return false;
                    });


                    if (l_padControlInfo != null)
                        _padControlInfoCollection.Remove(l_padControlInfo);
                }

            });

            System.Windows.Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Background,
            (System.Threading.ThreadStart)delegate ()
            {
                foreach (var item in l_XInputGamePadList)
                {

                    var l_padControlInfo = _padControlInfoCollection.FirstOrDefault((a_item) => {

                        if (a_item.PadType == PadType.XInputPad)
                        {
                            var l_pad = (a_item as XInputPadControlInfoPanel);

                            if (l_pad != null)
                            {
                                return l_pad.DeviceIndex == item;
                            }

                            return false;
                        }
                        else
                            return false;
                    });

                    if (l_padControlInfo == null)
                        _padControlInfoCollection.Add(
                            new XInputPadControlInfoPanel(item)
                            );
                }

            });
        }

        void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            if (PadConfigPanelEvent != null && mCustomerView.CurrentItem != null)
            {
                select(mCustomerView.CurrentItem as PadControlInfo);

                PadConfigPanelEvent((mCustomerView.CurrentItem as PadControlInfo).PadConfigPanel);
            }
            else
                PadConfigPanelEvent(null);
        }

        public void setTouchPadControl(PadControlInfo a_padControlInfo)
        {
            if (a_padControlInfo.PadType != PadType.TouchPad)
                return;

            System.Windows.Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Background,
            (System.Threading.ThreadStart)delegate ()
            {
                var l_padControlInfo = _padControlInfoCollection.FirstOrDefault((a_item) => {

                    if (a_item.PadType == PadType.TouchPad)
                    {
                        if (a_item != null)
                        {
                            return a_item.PadType == a_padControlInfo.PadType;
                        }

                        return false;
                    }
                    else
                        return false;
                });


                if (l_padControlInfo != null)
                    _padControlInfoCollection.Remove(l_padControlInfo);

                _padControlInfoCollection.Insert(0, a_padControlInfo);

                if (_padControlInfoCollection.Count != 0)
                    mCustomerView.MoveCurrentTo(_padControlInfoCollection[0]);

            });
        }

        public object PadConfigPanel { get { return null; } }

        public event Action<bool> ShowTouchPadPanelEvent;

        private ICollectionView mCustomerView = null;

        public ICollectionView Collection
        {
            get { return mCustomerView; }
        }

        private void select(PadControlInfo a_PadControlInfo)
        {
            if (a_PadControlInfo == null)
                return;

            if (ShowTouchPadPanelEvent != null)
                ShowTouchPadPanelEvent(a_PadControlInfo.PadType == PadType.TouchPad);

            m_currentPadControl = a_PadControlInfo.PadControl;

            PadInput.Instance.PadControl = m_currentPadControl;
        }

        private void load()
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<PadControlConfig>));

            XmlReader xmlReader = XmlReader.Create(new StringReader(Properties.Settings.Default.PadControlConfigCollection));

            if (ser.CanDeserialize(xmlReader))
            {
                var l_collection = ser.Deserialize(xmlReader) as List<PadControlConfig>;

                if (l_collection != null)
                {
                    //m_PadControlConfig.AddRange(l_collection);
                }
            }
        }

        private void save()
        {

            XmlSerializer ser = new XmlSerializer(typeof(List<PadControlConfig>));

            MemoryStream stream = new MemoryStream();

            //ser.Serialize(stream, m_PadControlConfig);

            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);

            Properties.Settings.Default.PadControlConfigCollection = reader.ReadToEnd();
        }


        private void VibrationCallbackInner(uint a_vibrationCombo)
        {
            if (!m_check_vibration_activity_pad_Timer.Enabled)
                m_check_vibration_activity_pad_Timer.Start();

            if (m_currentPadControl != null)
                m_currentPadControl.setVibration(a_vibrationCombo);

            m_is_vibration_activity = true;
        }

        public void reset()
        {
            var l_currentPosition = mCustomerView.CurrentPosition;

            mCustomerView.MoveCurrentToPosition(-1);

            mCustomerView.MoveCurrentToPosition(l_currentPosition);
        }

        public void ShowWarning(string a_message)
        {
            if (ShowWarningEvent != null)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    ShowWarningEvent(a_message);
                });
        }

        public void stopTimer()
        {
            m_check_connecting_pad_Timer.AutoReset = false;

            m_check_connecting_pad_Timer.Stop();

            m_check_connecting_pad_Timer.Dispose();

            m_check_vibration_activity_pad_Timer.AutoReset = false;

            m_check_vibration_activity_pad_Timer.Stop();

            m_check_vibration_activity_pad_Timer.Dispose();
        }
    }
}