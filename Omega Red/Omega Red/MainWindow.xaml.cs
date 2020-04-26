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

using Omega_Red.Managers;
using Omega_Red.Tools;
using Omega_Red.Tools.Converters;
using Omega_Red.Panels;
using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Omega_Red.Properties;

namespace Omega_Red
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            SaveStateManager.Instance.init();

            TexturePackControl.Instance.init();

            InitializeComponent();

            LockScreenManager.Instance.show();

            ConfigManager.Instance.SwitchControlModeEvent += Instance_SwitchControlModeEvent;

            PCSX2Controller.Instance.ChangeStatusEvent += Instance_ChangeStatusEvent;

            MediaRecorderManager.Instance.ShowWarningEvent += Instance_ShowWarningEvent;

            PadControlManager.Instance.ShowWarningEvent += Instance_ShowWarningEvent;

            MediaRecorderManager.Instance.ShowInfoEvent += Instance_ShowInfoEvent;

#if DEBUG

            WindowState = System.Windows.WindowState.Normal;

            WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
#endif
        }

        private void Instance_ShowWarningEvent(string a_message)
        {
            mTaskbarIcon.ShowBalloonTip(Title, a_message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
        }
        private void Instance_ShowInfoEvent(string a_message)
        {
            mTaskbarIcon.ShowBalloonTip(Title, a_message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        void Instance_ChangeStatusEvent(PCSX2Controller.StatusEnum a_Status)
        {
            if (!mButtonControl && a_Status == PCSX2Controller.StatusEnum.Started)
            {
                mMediaCloseBtn_Click(null, null);

                mControlCloseBtn_Click(null, null);
            }
        }

        private bool mButtonControl = false;

        void Instance_SwitchControlModeEvent(bool obj)
        {
            var l_LeftWidthConverter = Resources["mControlLeftWidthOffset"] as WidthConverter;

            var l_RightWidthConverter = Resources["mControlRightWidthOffset"] as WidthConverter;

            mButtonControl = obj;

            if(obj)
            {

                if (l_LeftWidthConverter != null)
                    l_LeftWidthConverter.Offset = 0.0;

                if (l_RightWidthConverter != null)
                    l_RightWidthConverter.Offset = 0.0;
            }
            else
            {
                var l_TouchDragBtnWidth = (double)App.Current.Resources["TouchDragBtnWidth"];

                if (l_LeftWidthConverter != null)
                    l_LeftWidthConverter.Offset = -l_TouchDragBtnWidth - 10;

                if (l_RightWidthConverter != null)
                    l_RightWidthConverter.Offset = -l_TouchDragBtnWidth - 10;
            }
        }

        public void loadModules()
        {
            string l_warning = "";

            do
            {

                if (Settings.Default.GoogleAccountIsChecked)
                    SocialNetworks.Google.GoogleAccountManager.Instance.tryAuthorize();


                if (!RTMPNative.Instance.isInit)
                {
                    l_warning = "RTMP is not nitialized!!!";

                    break;
                }

                if (!PCSX2ModuleManager.Instance.isInit)
                {
                    l_warning = "PCSX2 modules are not nitialized!!!";

                    break;
                }

                if (!PCSXModuleManager.Instance.isInit)
                {
                    l_warning = "PCSX modules are not nitialized!!!";

                    break;
                }

                if (!PCSX2LibNative.Instance.isInit)
                {
                    l_warning = "PCSX2 library is not nitialized!!!";

                    break;
                }

                if (!PPSSPPNative.Instance.isInit)
                {
                    l_warning = "PPSSPP library is not nitialized!!!";

                    break;
                }

                if (!PCSXNative.Instance.isInit)
                {
                    l_warning = "PCSX library is not nitialized!!!";

                    break;
                }

                foreach (var l_Module in PCSXModuleManager.Instance.Modules)
                {
                    PCSXNative.Instance.setModule(l_Module);
                }
                
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {

                    if (m_PadPanel.Content != null && m_PadPanel.Content is DisplayControl)
                    {
                        ModuleControl.Instance.setVideoPanel((m_PadPanel.Content as DisplayControl).VideoPanel);

                        PPSSPPControl.Instance.setVideoPanelHandler(((m_PadPanel.Content as DisplayControl).VideoPanel).SharedHandle);

                        Tools.Savestate.SStates.Instance.setVideoPanel((m_PadPanel.Content as DisplayControl).VideoPanel);

                        ScreenshotsManager.Instance.setVideoPanel((m_PadPanel.Content as DisplayControl).VideoPanel);

                        MediaSourcesManager.Instance.DisplayControl = m_PadPanel.Content as DisplayControl;
                    }

                    var wih = new System.Windows.Interop.WindowInteropHelper(App.Current.MainWindow);

                    ModuleControl.Instance.setWindowHandler(wih.Handle);

                    MediaSourcesManager.Instance.load(()=>
                    {
                        SocialNetworks.Google.GoogleAccountManager.Instance.sendEvent();

                        PCSX2Controller.Instance.updateInitilize();

                        LockScreenManager.Instance.hide();
                    });

                    ModuleControl.Instance.initPCSX();

                    ModuleControl.Instance.initPCSX2();

                    IsoManager.Instance.load();
                });
                
                Capture.MediaStream.Instance.setConnectionFunc(RTMPNative.Instance.Connect);

                Capture.MediaStream.Instance.setDisconnectFunc(RTMPNative.Instance.Disconnect);

                Capture.MediaStream.Instance.setWriteFunc(RTMPNative.Instance.Write);

                Capture.MediaStream.Instance.setIsConnectedFunc(RTMPNative.Instance.IsConnected);

            } while (false);

            if(!string.IsNullOrWhiteSpace(l_warning))
            {
                LockScreenManager.Instance.hide();

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    Instance_ShowWarningEvent(l_warning);
                });
            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadStart loadModulesStart = new ThreadStart(loadModules);

            Thread loadModulesThread = new Thread(loadModulesStart);

            loadModulesThread.Start();
        }

        public void rebindControlPanel()
        {
            Binding binding = new Binding();
            binding.Source = mControlGrid;
            binding.Path = new PropertyPath(FrameworkElement.ActualWidthProperty);
            binding.Converter = new WidthConverter() { Offset = -5.0, Scale = -1.0 };
            mControlGrid.SetBinding(Canvas.LeftProperty, binding);
        }

        public void rebindMediaPanel()
        {
            Binding binding = new Binding();
            binding.Source = mMediaGrid;
            binding.Path = new PropertyPath(FrameworkElement.ActualWidthProperty);
            binding.Converter = new WidthConverter() { Offset = -5.0, Scale = -1.0 };
            mMediaGrid.SetBinding(Canvas.RightProperty, binding);
        }

        private void mControlCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;

            MouseButton l_mouseButton = MouseButton.Left;

            MouseButtonEventArgs l_UncheckedEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, l_mouseButton);

            l_UncheckedEvent.RoutedEvent = CheckBox.UncheckedEvent;

            l_UncheckedEvent.Source = mControlChkBtn;

            mControlChkBtn.RaiseEvent(l_UncheckedEvent);
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (!mButtonControl)
            {
                rebindControlPanel();
            }
        }

        private void mMediaCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;

            MouseButton l_mouseButton = MouseButton.Left;

            MouseButtonEventArgs l_UncheckedEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, l_mouseButton);

            l_UncheckedEvent.RoutedEvent = CheckBox.UncheckedEvent;

            l_UncheckedEvent.Source = mControlChkBtn;

            mMediaChkBtn.RaiseEvent(l_UncheckedEvent);
        }

        private void Storyboard_Completed_1(object sender, EventArgs e)
        {
            if (!mButtonControl)
            {
                rebindMediaPanel();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
                WindowStyle = System.Windows.WindowStyle.None;
            else
                WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
        }
        
        public Grid getMediaGrid()
        {
            return mMediaGrid;
        }

        public MediaPanel getMediaPanel()
        {
            return mMediaPanel;
        }

        public ControlPanel getControlPanel()
        {
            return mControlPanel;
        }

        public Grid getControlGrid()
        {
            return mControlGrid;
        }

        public CheckBox getControlChkBtn()
        {
            return mControlChkBtn;
        }

        public CheckBox getMediaChkBtn()
        {
            return mMediaChkBtn;
        }
               
        private void Window_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();

            mTaskbarIcon.Dispose();
        }
    }
}
