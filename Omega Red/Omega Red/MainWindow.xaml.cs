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

            InitializeComponent();

            LockScreenManager.Instance.show();

            ConfigManager.Instance.SwitchControlModeEvent += Instance_SwitchControlModeEvent;

            PCSX2Controller.Instance.ChangeStatusEvent += Instance_ChangeStatusEvent;
                        
#if DEBUG

            WindowState = System.Windows.WindowState.Normal;

            WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
#endif
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
            var l_WidthConverter = Resources["mControlWidthOffset"] as WidthConverter;

            mButtonControl = obj;

            if(obj)
            {

                if (l_WidthConverter != null)
                    l_WidthConverter.Offset = 0.0;
            }
            else
            {
                var l_TouchDragBtnWidth = (double)App.Current.Resources["TouchDragBtnWidth"];

                if (l_WidthConverter != null)
                    l_WidthConverter.Offset = -l_TouchDragBtnWidth;
            }
        }

        public void loadModules()
        {
            do
            {

                if (!ModuleManager.Instance.isInit)
                {
                    break;
                }

                if (!PCSX2LibNative.Instance.isInit)
                {
                    break;
                }

                if (!PPSSPPNative.Instance.isInit)
                {
                    break;
                }
                               
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
                {

                    if (m_PadPanel.Content != null && m_PadPanel.Content is VideoPanel)
                    {
                        ModuleControl.Instance.setVideoPanel(m_PadPanel.Content as VideoPanel);

                        PPSSPPControl.Instance.setVideoPanelHandler((m_PadPanel.Content as VideoPanel).SharedHandle);

                        Tools.Savestate.SStates.Instance.setVideoPanel(m_PadPanel.Content as VideoPanel);

                        ScreenshotsManager.Instance.setVideoPanel(m_PadPanel.Content as VideoPanel);
                    }

                    var wih = new System.Windows.Interop.WindowInteropHelper(App.Current.MainWindow);

                    ModuleControl.Instance.setWindowHandler(wih.Handle);

                    PCSX2Controller.Instance.updateInitilize();

                });

            } while (false);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                if (ModuleManager.Instance.isInit && PCSX2LibNative.Instance.isInit)
                    LockScreenManager.Instance.hide();
            });
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadStart loadModulesStart = new ThreadStart(loadModules);

            Thread loadModulesThread = new Thread(loadModulesStart);

            loadModulesThread.Start();
        }


        bool mIsPressed = false;

        bool mIsOpened = false;

        Point mStartPosition = new Point();

        private void rebindControlPanel()
        {
            Binding binding = new Binding();
            binding.Source = mControlGrid;
            binding.Path = new PropertyPath(FrameworkElement.ActualWidthProperty);
            binding.Converter = new WidthConverter() { Offset = -5.0, Scale = -1.0 };
            mControlGrid.SetBinding(Canvas.LeftProperty, binding);
        }

        private void rebindMediaPanel()
        {
            Binding binding = new Binding();
            binding.Source = mMediaGrid;
            binding.Path = new PropertyPath(FrameworkElement.ActualWidthProperty);
            binding.Converter = new WidthConverter() { Offset = -5.0, Scale = -1.0 };
            mMediaGrid.SetBinding(Canvas.RightProperty, binding);
        }

        private Ellipse mTouchMark = null; 

        private void mTouchCtr_MouseChange(object sender, MouseButtonEventArgs e)
        {
            mIsPressed = e.ButtonState == MouseButtonState.Pressed;

            if (mIsPressed)
            {
                mStartPosition = e.GetPosition(this);

                e.MouseDevice.Capture(this, CaptureMode.SubTree);

                if(mTouchMark == null)
                {
                    mTouchMark = new Ellipse();

                    mTouchMark.Width = 100;

                    mTouchMark.Height = 100;

                    mTouchMark.Fill = new SolidColorBrush(Color.FromArgb(128 ,0 , 255, 0));

                    var l_position = e.GetPosition(mTouchCtr);

                    Canvas.SetLeft(mTouchMark, l_position.X - mTouchMark.Width / 2);

                    Canvas.SetTop(mTouchMark, l_position.Y - mTouchMark.Height / 2);

                    mTouchCtr.Children.Add(mTouchMark);
                }
            }
            else
            {
                if (mTouchMark != null)
                {
                    mTouchCtr.Children.Remove(mTouchMark);
                }

                mTouchMark = null;

                mStartPosition = new Point();

                if (!mIsOpened)
                {
                    rebindControlPanel();

                    rebindMediaPanel();
                }

                e.MouseDevice.Capture(null);
            }

            mIsOpened = false;
        }

        private void mTouchCtr_MouseMove(object sender, MouseEventArgs e)
        {
            if (mIsPressed)
            {
                var l_position = e.GetPosition(this);

                if (mTouchMark != null)
                {
                    var l_positionCanvas = e.GetPosition(mTouchCtr);

                    Canvas.SetLeft(mTouchMark, l_positionCanvas.X - mTouchMark.Width / 2);

                    Canvas.SetTop(mTouchMark, l_positionCanvas.Y - mTouchMark.Height / 2);
                }

                var l_x_Delta = l_position.X - mStartPosition.X;

                var l_TouchDragBtnWidth = (double)App.Current.Resources["TouchDragBtnWidth"];

                if(l_x_Delta > 0)
                {
                    Canvas.SetRight(mMediaGrid, -mMediaPanel.ActualWidth - l_TouchDragBtnWidth - 2);

                    rebindMediaPanel();

                    var l_leftProp = l_x_Delta / mControlPanel.ActualWidth;

                    if (l_leftProp > 0.45)
                    {
                        mIsPressed = false;

                        e.MouseDevice.Capture(null);

                        
                        int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;

                        MouseButton l_mouseButton = MouseButton.Left;

                        MouseButtonEventArgs l_mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, l_mouseButton);

                        l_mouseUpEvent.RoutedEvent = CheckBox.CheckedEvent;

                        l_mouseUpEvent.Source = mControlChkBtn;

                        mControlChkBtn.RaiseEvent(l_mouseUpEvent);

                        mControlChkBtn.Command.Execute(true);
                        

                        mIsOpened = true;
                    }
                    else
                    {
                        Canvas.SetLeft(mControlGrid, l_x_Delta - mControlPanel.ActualWidth - l_TouchDragBtnWidth);
                    }
                }
                else
                {
                    Canvas.SetLeft(mControlGrid, -mControlPanel.ActualWidth - l_TouchDragBtnWidth - 2);

                    rebindControlPanel();

                    l_x_Delta = Math.Abs(l_x_Delta);

                    var l_leftProp = l_x_Delta / mMediaPanel.ActualWidth;

                    if (l_leftProp > 0.45)
                    {
                        mIsPressed = false;

                        e.MouseDevice.Capture(null);


                        int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;

                        MouseButton l_mouseButton = MouseButton.Left;

                        MouseButtonEventArgs l_mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, l_mouseButton);

                        l_mouseUpEvent.RoutedEvent = CheckBox.CheckedEvent;

                        l_mouseUpEvent.Source = mControlChkBtn;

                        mMediaChkBtn.RaiseEvent(l_mouseUpEvent);

                        mMediaChkBtn.Command.Execute(true);


                        mIsOpened = true;
                    }
                    else
                    {
                        Canvas.SetRight(mMediaGrid, l_x_Delta - mMediaPanel.ActualWidth - l_TouchDragBtnWidth);
                    }
                }
            }
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

        private void mTouchCtr_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mTouchMark != null)
            {
                mTouchCtr.Children.Remove(mTouchMark);
            }

            mTouchMark = null;
        }
    }
}
