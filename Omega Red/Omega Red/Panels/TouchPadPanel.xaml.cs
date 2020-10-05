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
    public partial class TouchPadPanel : UserControl
    {
        public TouchPadPanel()
        {
            InitializeComponent();

            PadControlManager.Instance.ShowTouchPadPanelEvent += Instance_ShowTouchPadPanelEvent;

            ConfigManager.Instance.SwitchDisplayModeEvent += Instance_SwitchDisplayModeEvent;
            
            this.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Button_Release), true);

            this.AddHandler(Button.MouseLeaveEvent, new RoutedEventHandler(Button_Release), true);

            PCSX2Controller.Instance.ChangeStatusEvent += (a_Status)=> {

                if(a_Status == PCSX2Controller.StatusEnum.Stopped)
                {
                    LimitFrame = false;

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                    {
                        IsEnabledLimitFrame = false;
                    });
                }

                if (a_Status == PCSX2Controller.StatusEnum.Started)
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

                PadInput.Instance.removeKey(l_key);
            }
        }




        public PadInput.XY_Axises LeftStickAxises
        {
            get { return (PadInput.XY_Axises)GetValue(LeftStickAxisesProperty); }
            set { SetValue(LeftStickAxisesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftStickAxises.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftStickAxisesProperty =
            DependencyProperty.Register("LeftStickAxises", typeof(PadInput.XY_Axises), typeof(TouchPadPanel),
            new PropertyMetadata(new PadInput.XY_Axises() { m_x_axis = 0, m_y_axis = 0 }, new PropertyChangedCallback(OnLeftStickAxisesChanged)));

        private static void OnLeftStickAxisesChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var l_XY_Axises = (PadInput.XY_Axises)e.NewValue;

            PadInput.Instance.setLeftStickAxises(l_XY_Axises);
        }




        public PadInput.XY_Axises RightStickAxises
        {
            get { return (PadInput.XY_Axises)GetValue(RightStickAxisesProperty); }
            set { SetValue(RightStickAxisesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightStickAxises.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightStickAxisesProperty =
            DependencyProperty.Register("RightStickAxises", typeof(PadInput.XY_Axises), typeof(TouchPadPanel),
            new PropertyMetadata(new PadInput.XY_Axises() { m_x_axis = 0, m_y_axis = 0 }, new PropertyChangedCallback(OnRightStickAxisesChanged)));

        private static void OnRightStickAxisesChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var l_XY_Axises = (PadInput.XY_Axises)e.NewValue;

            PadInput.Instance.setRightStickAxises(l_XY_Axises);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && sender is FrameworkElement && (sender as FrameworkElement).Tag != null)
            {
                (sender as Button).Background = App.Current.Resources["PressedButtonBrush"] as Brush;

                UInt16 l_key = UInt16.Parse((sender as FrameworkElement).Tag.ToString(), System.Globalization.NumberStyles.HexNumber);

                PadInput.Instance.setKey(l_key);
            }
        }

        private void Button_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (sender != null && sender is FrameworkElement && (sender as FrameworkElement).Tag != null)
            {
                (sender as Button).Background = App.Current.Resources["PressedButtonBrush"] as Brush;

                UInt16 l_key = UInt16.Parse((sender as FrameworkElement).Tag.ToString(), System.Globalization.NumberStyles.HexNumber);

                PadInput.Instance.setKey(l_key);
            }
        }

        private void Button_PreviewTouchUp(object sender, TouchEventArgs e)
        {          
            if (sender != null && sender is FrameworkElement && (sender as FrameworkElement).Tag != null)
            {
                (sender as Button).Background = App.Current.Resources["UnpressedButtonBrush"] as Brush;

                UInt16 l_key = UInt16.Parse((sender as FrameworkElement).Tag.ToString(), System.Globalization.NumberStyles.HexNumber);

                PadInput.Instance.removeKey(l_key);
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

            PadInput.Instance.setLeftAnalogTrigger(l_value);
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

            PadInput.Instance.setRightAnalogTrigger(l_value);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var lCheckBox = sender as CheckBox;

            if(lCheckBox != null)
                PCSX2Controller.Instance.setLimitFrame(!(bool)lCheckBox.IsChecked);
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
    }
}
