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

using Omega_Red.Emulators;
using Omega_Red.Managers;
using Omega_Red.Tools;
using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Omega_Red.Panels
{
    /// <summary>
    /// Interaction logic for PadPanel.xaml
    /// </summary>
    public partial class TouchPadPanel : UserControl, IPadControl
    {
        public struct XY_Axises
        {
            public short m_x_axis;

            public short m_y_axis;
        }

        public static event Action<uint, uint> VibrationEvent;

        private static Util.XInputNative.XINPUT_STATE m_state = new Util.XInputNative.XINPUT_STATE();


        private void setKey(UInt16 a_Key)
        {
            m_state.Gamepad.wButtons |= a_Key;
        }

        static private void setLeftStickAxises(XY_Axises a_Axises)
        {
            m_state.Gamepad.sThumbLX = a_Axises.m_x_axis;

            m_state.Gamepad.sThumbLY = a_Axises.m_y_axis;
        }

        static private void setLeftAnalogTrigger(byte a_value)
        {
            m_state.Gamepad.bLeftTrigger = a_value;
        }

        static private void setRightAnalogTrigger(byte a_value)
        {
            m_state.Gamepad.bRightTrigger = a_value;
        }

        static private void setRightStickAxises(XY_Axises a_Axises)
        {
            m_state.Gamepad.sThumbRX = a_Axises.m_x_axis;

            m_state.Gamepad.sThumbRY = a_Axises.m_y_axis;
        }

        private void removeKey(UInt16 a_Key)
        {
            m_state.Gamepad.wButtons &= (ushort)(~a_Key);
        }

        public TouchPadPanel()
        {
            InitializeComponent();

            Models.PadControlInfo l_TouchPadControlInfo = new Models.TouchPadControlInfo()
            {
                PadControl = this
            };

            PadControlManager.Instance.setTouchPadControl(l_TouchPadControlInfo);

            PadInput.Instance.PadControl = this;

            PadControlManager.Instance.ShowTouchPadPanelEvent += Instance_ShowTouchPadPanelEvent;

            ConfigManager.Instance.SwitchDisplayModeEvent += Instance_SwitchDisplayModeEvent;
            
            this.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Button_Release), true);

            this.AddHandler(Button.MouseLeaveEvent, new RoutedEventHandler(Button_Release), true);

            Emul.Instance.ChangeStatusEvent += (a_Status)=> {

                if(a_Status == Emul.StatusEnum.Stopped)
                {
                    LimitFrame = false;

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                    {
                        IsEnabledLimitFrame = false;
                    });
                }

                if (a_Status == Emul.StatusEnum.Started)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                    {
                        IsEnabledLimitFrame = true;
                    });
                }
            };

            IsEnabledLimitFrame = false;
        }

        void Instance_SwitchDisplayModeEvent(bool obj)
        {
            if(obj)
            {
                GridColumnContent = 0;

                GridColumnSpanContent = 3;

                GridRowSpanContent = 2;

                CurrentBorderThickness = new Thickness(0);
            }
            else
            {
                GridColumnContent = 0;

                GridColumnSpanContent = 3;

                GridRowSpanContent = 1;

                CurrentBorderThickness = (Thickness)App.Current.Resources["StandardBorderThickness"];
            }
        }

        void Instance_ShowTouchPadPanelEvent(bool obj)
        {
            if(obj)
            {
                TouchPadPanelVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TouchPadPanelVisibility = System.Windows.Visibility.Hidden;
            }
        }

        public Visibility TouchPadPanelVisibility
        {
            get { return (Visibility)GetValue(TouchPadPanelVisibilityProperty); }
            set { SetValue(TouchPadPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TouchPadPanelVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TouchPadPanelVisibilityProperty =
            DependencyProperty.Register("TouchPadPanelVisibility", typeof(Visibility), typeof(TouchPadPanel), new PropertyMetadata(Visibility.Visible));




        public Thickness CurrentBorderThickness
        {
            get { return (Thickness)GetValue(CurrentBorderThicknessProperty); }
            set { SetValue(CurrentBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentBorderThicknessProperty =
            DependencyProperty.Register("CurrentBorderThickness", typeof(Thickness), typeof(TouchPadPanel), new PropertyMetadata(new Thickness(0)));

        


        public int GridColumnContent
        {
            get { return (int)GetValue(GridColumnContentProperty); }
            set { SetValue(GridColumnContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridColumnContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColumnContentProperty =
            DependencyProperty.Register("GridColumnContent", typeof(int), typeof(TouchPadPanel), new PropertyMetadata(1));




        public int GridColumnSpanContent
        {
            get { return (int)GetValue(GridColumnSpanContentProperty); }
            set { SetValue(GridColumnSpanContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridColumnSpanContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColumnSpanContentProperty =
            DependencyProperty.Register("GridColumnSpanContent", typeof(int), typeof(TouchPadPanel), new PropertyMetadata(1));                       

        public int GridRowSpanContent
        {
            get { return (int)GetValue(GridRowSpanContentProperty); }
            set { SetValue(GridRowSpanContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridRowSpanContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridRowSpanContentProperty = 
            DependencyProperty.Register("GridRowSpanContent", typeof(int), typeof(TouchPadPanel), new PropertyMetadata(1));
                
        private void Button_Release(object sender, RoutedEventArgs e)
        {
            if (e != null && e.OriginalSource is FrameworkElement && (e.OriginalSource as FrameworkElement).Tag != null)
            {
                (e.OriginalSource as Button).Background = App.Current.Resources["UnpressedButtonBrush"] as Brush;

                UInt16 l_key = UInt16.Parse((e.OriginalSource as FrameworkElement).Tag.ToString(), System.Globalization.NumberStyles.HexNumber);

                removeKey(l_key);
            }
        }




        public XY_Axises LeftStickAxises
        {
            get { return (XY_Axises)GetValue(LeftStickAxisesProperty); }
            set { SetValue(LeftStickAxisesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftStickAxises.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftStickAxisesProperty =
            DependencyProperty.Register("LeftStickAxises", typeof(XY_Axises), typeof(TouchPadPanel),
            new PropertyMetadata(new XY_Axises() { m_x_axis = 0, m_y_axis = 0 }, new PropertyChangedCallback(OnLeftStickAxisesChanged)));

        private static void OnLeftStickAxisesChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var l_XY_Axises = (XY_Axises)e.NewValue;

            setLeftStickAxises(l_XY_Axises);
        }




        public XY_Axises RightStickAxises
        {
            get { return (XY_Axises)GetValue(RightStickAxisesProperty); }
            set { SetValue(RightStickAxisesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightStickAxises.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightStickAxisesProperty =
            DependencyProperty.Register("RightStickAxises", typeof(XY_Axises), typeof(TouchPadPanel),
            new PropertyMetadata(new XY_Axises() { m_x_axis = 0, m_y_axis = 0 }, new PropertyChangedCallback(OnRightStickAxisesChanged)));

        private static void OnRightStickAxisesChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var l_XY_Axises = (XY_Axises)e.NewValue;

            setRightStickAxises(l_XY_Axises);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && sender is FrameworkElement && (sender as FrameworkElement).Tag != null)
            {
                (sender as Button).Background = App.Current.Resources["PressedButtonBrush"] as Brush;

                UInt16 l_key = UInt16.Parse((sender as FrameworkElement).Tag.ToString(), System.Globalization.NumberStyles.HexNumber);

                setKey(l_key);
            }
        }

        private void Button_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (sender != null && sender is FrameworkElement && (sender as FrameworkElement).Tag != null)
            {
                (sender as Button).Background = App.Current.Resources["PressedButtonBrush"] as Brush;

                UInt16 l_key = UInt16.Parse((sender as FrameworkElement).Tag.ToString(), System.Globalization.NumberStyles.HexNumber);

                setKey(l_key);
            }
        }

        private void Button_PreviewTouchUp(object sender, TouchEventArgs e)
        {          
            if (sender != null && sender is FrameworkElement && (sender as FrameworkElement).Tag != null)
            {
                (sender as Button).Background = App.Current.Resources["UnpressedButtonBrush"] as Brush;

                UInt16 l_key = UInt16.Parse((sender as FrameworkElement).Tag.ToString(), System.Globalization.NumberStyles.HexNumber);

                removeKey(l_key);
            }
        }







        public byte LeftAnalogTrigger
        {
            get { return (byte)GetValue(LeftAnalogTriggerProperty); }
            set { SetValue(LeftAnalogTriggerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftStickAxises.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftAnalogTriggerProperty =
            DependencyProperty.Register("LeftAnalogTrigger", typeof(byte), typeof(TouchPadPanel),
            new PropertyMetadata((byte)0, new PropertyChangedCallback(OnLeftAnalogTriggerChanged)));

        private static void OnLeftAnalogTriggerChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var l_value = (byte)e.NewValue;

            Console.WriteLine(l_value);

            setLeftAnalogTrigger(l_value);
        }




        public byte RightAnalogTrigger
        {
            get { return (byte)GetValue(RightAnalogTriggerProperty); }
            set { SetValue(RightAnalogTriggerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightStickAxises.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightAnalogTriggerProperty =
            DependencyProperty.Register("RightAnalogTrigger", typeof(byte), typeof(TouchPadPanel),
            new PropertyMetadata((byte) 0, new PropertyChangedCallback(OnRightAnalogTriggerChanged)));

        private static void OnRightAnalogTriggerChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var l_value = (byte)e.NewValue;

            setRightAnalogTrigger(l_value);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var lCheckBox = sender as CheckBox;

            if(lCheckBox != null)
                Emul.Instance.setLimitFrame(!(bool)lCheckBox.IsChecked);
        }





        public bool LimitFrame
        {
            get { return (bool)GetValue(LimitFrameProperty); }
            set { SetValue(LimitFrameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LimitFrameProperty =
            DependencyProperty.Register("LimitFrame", typeof(bool), typeof(TouchPadPanel), new PropertyMetadata(false));




        public bool IsEnabledLimitFrame
        {
            get { return (bool)GetValue(IsEnabledLimitFrameProperty); }
            set { SetValue(IsEnabledLimitFrameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledLimitFrameProperty =
            DependencyProperty.Register("IsEnabledLimitFrame", typeof(bool), typeof(TouchPadPanel), new PropertyMetadata(false));




        private bool mIsPressed = false;

        private bool mIsOpened = false;

        private Point mStartPosition = new Point();

        private Ellipse mTouchMark = null;

        private Canvas mTouchCtr = null;

        private void mTouchCtr_MouseChange(object sender, MouseButtonEventArgs e)
        {
            mIsPressed = e.ButtonState == MouseButtonState.Pressed;

            if (mIsPressed)
            {
                mStartPosition = e.GetPosition(this);

                e.MouseDevice.Capture(this, CaptureMode.SubTree);

                mTouchCtr = sender as Canvas;

                if (mTouchMark == null)
                {
                    mTouchMark = new Ellipse();

                    mTouchMark.Width = 100;

                    mTouchMark.Height = 100;

                    mTouchMark.Fill = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

                    var l_position = e.GetPosition(mTouchCtr);

                    Canvas.SetLeft(mTouchMark, l_position.X - mTouchMark.Width / 2);

                    Canvas.SetTop(mTouchMark, l_position.Y - mTouchMark.Height / 2);

                    mTouchCtr.Children.Add(mTouchMark);
                }
            }
            else
            {
                if (mTouchMark != null && mTouchCtr != null)
                {
                    mTouchCtr.Children.Remove(mTouchMark);
                }

                mTouchMark = null;

                mTouchCtr = null;

                mStartPosition = new Point();

                if (!mIsOpened)
                {
                    var l_MainWindow = App.Current.MainWindow as MainWindow;

                    if(l_MainWindow != null)
                    {
                        l_MainWindow.rebindControlPanel();

                        l_MainWindow.rebindMediaPanel();
                    }
                }

                e.MouseDevice.Capture(null);
            }

            mIsOpened = false;
        }

        private void mTouchCtr_MouseMove(object sender, MouseEventArgs e)
        {
            var l_MainWindow = App.Current.MainWindow as MainWindow;

            if (l_MainWindow == null)
            {
                return;
            }

            if (mIsPressed)
            {
                var l_position = e.GetPosition(this);

                if (mTouchMark != null && mTouchCtr != null)
                {
                    var l_positionCanvas = e.GetPosition(mTouchCtr);

                    Canvas.SetLeft(mTouchMark, l_positionCanvas.X - mTouchMark.Width / 2);

                    Canvas.SetTop(mTouchMark, l_positionCanvas.Y - mTouchMark.Height / 2);
                }

                var l_x_Delta = l_position.X - mStartPosition.X;

                var l_TouchDragBtnWidth = (double)App.Current.Resources["TouchDragBtnWidth"];

                if (l_x_Delta > 0)
                {
                    Canvas.SetRight(l_MainWindow.getMediaGrid(), -l_MainWindow.getMediaPanel().ActualWidth - l_TouchDragBtnWidth - 2);

                    l_MainWindow.rebindMediaPanel();

                    var l_leftProp = l_x_Delta / l_MainWindow.getControlPanel().ActualWidth;

                    if (l_leftProp > 0.45)
                    {
                        mIsPressed = false;

                        e.MouseDevice.Capture(null);


                        int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;

                        MouseButton l_mouseButton = MouseButton.Left;

                        MouseButtonEventArgs l_mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, l_mouseButton);

                        l_mouseUpEvent.RoutedEvent = CheckBox.CheckedEvent;

                        l_mouseUpEvent.Source = l_MainWindow.getControlChkBtn();

                        l_MainWindow.getControlChkBtn().RaiseEvent(l_mouseUpEvent);

                        l_MainWindow.getControlChkBtn().Command.Execute(true);


                        mIsOpened = true;
                    }
                    else
                    {
                        Canvas.SetLeft(l_MainWindow.getControlGrid(), l_x_Delta - l_MainWindow.getControlPanel().ActualWidth - l_TouchDragBtnWidth);
                    }
                }
                else
                {
                    Canvas.SetLeft(l_MainWindow.getControlGrid(), -l_MainWindow.getControlPanel().ActualWidth - l_TouchDragBtnWidth - 2);

                    l_MainWindow.rebindControlPanel();

                    l_x_Delta = Math.Abs(l_x_Delta);

                    var l_leftProp = l_x_Delta / l_MainWindow.getMediaPanel().ActualWidth;

                    if (l_leftProp > 0.45)
                    {
                        mIsPressed = false;

                        e.MouseDevice.Capture(null);


                        int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;

                        MouseButton l_mouseButton = MouseButton.Left;

                        MouseButtonEventArgs l_mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, l_mouseButton);

                        l_mouseUpEvent.RoutedEvent = CheckBox.CheckedEvent;

                        l_mouseUpEvent.Source = l_MainWindow.getControlChkBtn();

                        l_MainWindow.getMediaChkBtn().RaiseEvent(l_mouseUpEvent);

                        if (l_MainWindow.getMediaChkBtn().Command != null)
                            l_MainWindow.getMediaChkBtn().Command.Execute(true);


                        mIsOpened = true;
                    }
                    else
                    {
                        Canvas.SetRight(l_MainWindow.getMediaGrid(), l_x_Delta - l_MainWindow.getMediaPanel().ActualWidth - l_TouchDragBtnWidth);
                    }
                }
            }
        }

        private void mTouchCtr_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mTouchMark != null && mTouchCtr != null)
            {
                mTouchCtr.Children.Remove(mTouchMark);
            }

            mTouchMark = null;

            mTouchCtr = null;
        }

        public XInputNative.XINPUT_STATE getState()
        {
            return m_state;
        }

        public void setVibration(uint a_vibrationCombo)
        {
            if (!Properties.Settings.Default.IsVisualVibrationEnabled)
                return;

            uint l_LeftMotorSpeed = a_vibrationCombo & 0xFFFF;
            uint l_RightMotorSpeed = (a_vibrationCombo >> 16) & 0xFFFF;

            if (VibrationEvent != null)
                VibrationEvent(l_LeftMotorSpeed, l_RightMotorSpeed);
        }
    }
}
