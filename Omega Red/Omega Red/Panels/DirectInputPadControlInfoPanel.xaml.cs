using Omega_Red.Emulators;
using Omega_Red.Models;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for DirectInputPadControlInfoPanel.xaml
    /// </summary>
    public partial class DirectInputPadControlInfoPanel : UserControl, PadControlInfo
    {
        private bool m_has_battery = false;
        
        private System.Timers.Timer m_update_pad_Timer = new System.Timers.Timer(100);
        
        public DirectInputPadControlInfoPanel(Tuple<string, Guid> a_gamePad, IPadControl a_IPadControl)
        {
            PadControl = a_IPadControl;

            InitializeComponent();

            PadType = PadType.DirectInputPad;
            DeviceGuid = a_gamePad.Item2;
            m_titleTxtBlk.Text = a_gamePad.Item1;


            m_battery_gliph.Visibility = Visibility.Collapsed;
            

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

            if (PadControl == null)
                return;

            var l_state = PadControl.getState();

            if (l_state.Gamepad.wButtons != 0 || l_state.Gamepad.bLeftTrigger != 0 || l_state.Gamepad.bRightTrigger != 0)
                l_activity = true;
            
            setActivity(l_activity);
        }

        ~DirectInputPadControlInfoPanel()
        {
            stopTimer();
        }

        public Guid DeviceGuid { get; private set; }

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

    }
}
