using Omega_Red.Emulators;
using Omega_Red.Models;
using Omega_Red.Tools;
using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Omega_Red.Panels
{
    /// <summary>
    /// Interaction logic for XInputPadControlInfoPanel.xaml
    /// </summary>
    public partial class XInputPadControlInfoPanel : UserControl, PadControlInfo
    {
        private bool m_has_battery = false;

        private byte m_previousButteryLevel = XInputNative.BATTERY_LEVEL_FULL;

        private System.Timers.Timer m_update_pad_Timer = new System.Timers.Timer(100);

        public XInputPadControlInfoPanel(uint a_index)
        {
            InitializeComponent();

            PadType = PadType.XInputPad;
            DeviceIndex = a_index;
            Title_Key = string.Format("GamePad {0}\n", DeviceIndex);
            PadControl = new XInputPadControl(a_index);
            m_Index.Text = " " + a_index.ToString();

            m_battery_gliph.Visibility = Visibility.Collapsed;

            XInputNative.XINPUT_CAPABILITIES l_capabilities = new XInputNative.XINPUT_CAPABILITIES();

            if (XInputNative.Instance.XInputGetCapabilities(DeviceIndex, 0, ref l_capabilities) == XInputNative.ERROR_SUCCESS)
            {
                if((l_capabilities.Flags & XInputNative.XINPUT_CAPS_WIRELESS) != 0)
                {
                    m_wirelese_gliph.Visibility = Visibility.Visible;

                    m_wire_gliph.Visibility = Visibility.Collapsed;

                    m_has_battery = true;

                    m_battery_gliph.Visibility = Visibility.Visible;
                }

                if ((l_capabilities.Flags & XInputNative.XINPUT_CAPS_FFB_SUPPORTED) != 0)
                {

                    m_vibration_gliph.Visibility = Visibility.Visible;
                }
            }

            XInputNative.XINPUT_BATTERY_INFORMATION l_batteryInformation = new XInputNative.XINPUT_BATTERY_INFORMATION();

            if (XInputNative.Instance.XInputGetBatteryInformation(DeviceIndex, XInputNative.BATTERY_DEVTYPE_GAMEPAD, ref l_batteryInformation) == XInputNative.ERROR_SUCCESS)
            {
                if(l_batteryInformation.BatteryType == XInputNative.BATTERY_TYPE_DISCONNECTED ||
                   l_batteryInformation.BatteryType == XInputNative.BATTERY_TYPE_WIRED)
                {
                    m_has_battery = true;
                }
            }


            Emul.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;

            m_update_pad_Timer.Elapsed += update_pad_Timer_Elapsed;

            m_update_pad_Timer.AutoReset = true;

            m_update_pad_Timer.Start();
        }

        private void Instance_m_ChangeStatusEvent(Emul.StatusEnum obj)
        {
            if (obj == Emul.StatusEnum.Started)
                m_update_pad_Timer.Interval = 2000;
            else
                m_update_pad_Timer.Interval = 100;
        }

        private void update_pad_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool l_activity = false;

            var l_state = PadControl.getState();

            if (l_state.Gamepad.wButtons != 0 || l_state.Gamepad.bLeftTrigger != 0 || l_state.Gamepad.bRightTrigger != 0)
                l_activity = true;
                        
            if(m_has_battery)
            {
                XInputNative.XINPUT_BATTERY_INFORMATION l_batteryInformation = new XInputNative.XINPUT_BATTERY_INFORMATION();

                if (XInputNative.Instance.XInputGetBatteryInformation(DeviceIndex, XInputNative.BATTERY_DEVTYPE_GAMEPAD, ref l_batteryInformation) == XInputNative.ERROR_SUCCESS)
                {
                    if (l_batteryInformation.BatteryType != XInputNative.BATTERY_TYPE_DISCONNECTED &&
                       l_batteryInformation.BatteryType != XInputNative.BATTERY_TYPE_WIRED)
                    {
                        setBatteryLevel(l_batteryInformation.BatteryLevel);
                    }
                }
            }

            setActivity(l_activity);
        }

        ~XInputPadControlInfoPanel()
        {
            stopTimer();
        }

        public uint DeviceIndex { get; private set; }

        public string Title_Key { get; set; }

        public PadType PadType { get; set; }

        public Tools.IPadControl PadControl { get; set; }

        public object PadConfigPanel { get; set; }
        public void stopTimer()
        {
            m_update_pad_Timer.AutoReset = false;

            m_update_pad_Timer.Stop();
        }

        private void setActivity(bool a_activity)
        {
            if(System.Windows.Application.Current != null)
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Send,
            (System.Threading.ThreadStart)delegate ()
            {
                if (a_activity)
                    m_Activity_Marker.Fill = m_pressed_store.Fill;
                else
                    m_Activity_Marker.Fill = m_unpressed_store.Fill;

            });
        }

        private void setBatteryLevel(byte a_level)
        {

            System.Windows.Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Send,
            (System.Threading.ThreadStart)delegate ()
            {
                switch (a_level)
                {
                    case XInputNative.BATTERY_LEVEL_FULL:
                        m_battery_path.Fill = (Brush)this.Resources["m_full_batter_brush"];
                        break;


                    case XInputNative.BATTERY_LEVEL_MEDIUM:
                        m_battery_path.Fill = (Brush)this.Resources["m_medium_batter_brush"];
                        break;


                    case XInputNative.BATTERY_LEVEL_LOW:
                        m_battery_path.Fill = (Brush)this.Resources["m_low_batter_brush"];
                        break;


                    case XInputNative.BATTERY_LEVEL_EMPTY:
                        m_battery_path.Fill = (Brush)this.Resources["m_empty_batter_brush"];
                        break;

                    default:
                        break;
                }

                if(m_previousButteryLevel != a_level && a_level == XInputNative.BATTERY_LEVEL_LOW)
                {
                    Managers.PadControlManager.Instance.ShowWarning(m_titleTxtBlk.Text + " " + m_Index.Text + " " + m_low_battery_TxtBlk.Text);
                }

                if (m_previousButteryLevel != a_level && a_level == XInputNative.BATTERY_LEVEL_EMPTY)
                {
                    Managers.PadControlManager.Instance.ShowWarning(m_titleTxtBlk.Text + " " + m_Index.Text + " " + m_empty_battery_TxtBlk.Text);
                }

                m_previousButteryLevel = a_level;

            });
        }

    }
}
