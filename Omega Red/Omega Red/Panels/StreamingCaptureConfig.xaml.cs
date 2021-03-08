using Omega_Red.Managers;
using Omega_Red.Properties;
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
    /// Interaction logic for StreamngCaptureConfig.xaml
    /// </summary>
    public partial class StreamingCaptureConfig : UserControl
    {
        public StreamingCaptureConfig()
        {
            InitializeComponent();

            bool lIsSelectedAddress = false;

            foreach (var litem in m_AddressCmbBx.Items)
            {
                var l_ComboBoxItem = litem as ComboBoxItem;

                if (l_ComboBoxItem != null)
                {
                    if ((string)l_ComboBoxItem.Content == Settings.Default.StreamAddress)
                    {
                        m_AddressCmbBx.SelectedItem = l_ComboBoxItem;

                        lIsSelectedAddress = true;

                        break;
                    }
                }
            }

            if(!lIsSelectedAddress && !string.IsNullOrWhiteSpace(Settings.Default.StreamAddress))
            {
                m_AddressCmbBx.SelectedItem = -1;

                m_AddressCmbBx.Text = Settings.Default.StreamAddress;
            }


            m_PassBx.Password = Settings.Default.StreamName;

            var lstate = !string.IsNullOrWhiteSpace(Settings.Default.StreamName);

            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
            {
                MediaRecorderManager.Instance.IsAllowedStreaming = lstate;

                Omega_Red.Capture.MediaStream.Instance.setConnectionToken(Tuple.Create<string, string>(Settings.Default.StreamAddress, Settings.Default.StreamName));
            });
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var l_PasswordBox = sender as PasswordBox;

            bool lstate = false;

            if(l_PasswordBox != null)
            {
                Settings.Default.StreamName = l_PasswordBox.Password;
                
                lstate = !string.IsNullOrWhiteSpace(Settings.Default.StreamName);
            }

            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
            {
                MediaRecorderManager.Instance.IsAllowedStreaming = lstate;

                Omega_Red.Capture.MediaStream.Instance.setConnectionToken(Tuple.Create<string, string>(Settings.Default.StreamAddress, Settings.Default.StreamName));
            });
        }

        private void m_AddressCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var l_ComboBox = sender as ComboBox;

            if (l_ComboBox != null)
            {
                var l_ComboBoxItem = l_ComboBox.SelectedItem as ComboBoxItem;

                if (l_ComboBoxItem != null)
                {
                    Settings.Default.StreamAddress = (string)l_ComboBoxItem.Content;
                }
            }
        }

        private void m_AddressCmbBx_TextChanged(object sender, TextChangedEventArgs e)
        {

            var l_ComboBox = sender as ComboBox;

            if (l_ComboBox != null)
            {
                Settings.Default.StreamAddress = l_ComboBox.Text;

                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
                {
                    Omega_Red.Capture.MediaStream.Instance.setConnectionToken(Tuple.Create<string, string>(Settings.Default.StreamAddress, Settings.Default.StreamName));
                });
            }
        }
    }
}
