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
    /// Interaction logic for AnalogTriggerPanel.xaml
    /// </summary>
    public partial class AnalogTriggerPanel : UserControl
    {
        double mInitCanvasLeft = 0.0;

        double mInitCanvasTop = 0.0;

        double mCenterX = 0.0;

        double mCenterY = 0.0;

        public AnalogTriggerPanel()
        {
            InitializeComponent();

            mInitCanvasLeft = Canvas.GetLeft(mTouchEllipse);

            mInitCanvasTop = Canvas.GetTop(mTouchEllipse);
            
            mCenterX = Width;

            mCenterY = Height;
        }

        bool mIsPressed = false;

        bool mTouchIsPressed = false;

        Point mOffsetPosition = new Point();


        private void Ellipse_MouseCheck(object sender, MouseButtonEventArgs e)
        {
            mIsPressed = e.ButtonState == MouseButtonState.Pressed;

            if (mIsPressed)
            {
                var l_position = e.GetPosition(this);

                mOffsetPosition.X = l_position.X;

                mOffsetPosition.Y = l_position.Y;

                e.MouseDevice.Capture(this, CaptureMode.SubTree);
            }
            else
            {
                int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;
                MouseButton l_mouseButton = MouseButton.Left;

                MouseButtonEventArgs l_mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, l_mouseButton);
                l_mouseUpEvent.RoutedEvent = Control.MouseUpEvent;
                l_mouseUpEvent.Source = mTouchEllipse;

                mTouchEllipse.RaiseEvent(l_mouseUpEvent);

                Canvas.SetLeft(mTouchEllipse, mInitCanvasLeft);

                Canvas.SetTop(mTouchEllipse, mInitCanvasTop);

                e.MouseDevice.Capture(null);

                AnalogValue = 0;
            }
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (mIsPressed)
            {
                var l_position = e.GetPosition(this);

                var l_leftDeltaPosition = mInitCanvasLeft + l_position.X - mOffsetPosition.X;


                if (l_leftDeltaPosition < 0)
                    return;
                

                var lhorizont = ((mCenterX - mTouchEllipse.Width) - l_leftDeltaPosition) / (mCenterX - mTouchEllipse.Width);

                if (lhorizont > 1.0)
                    return;

                if (lhorizont < 0.0)
                    return;

                var lvertical = Math.Sqrt(1 - lhorizont* lhorizont);

                if (lvertical > 0.995)
                    lvertical = 1.0;

                var l_newLeftPosition = (mCenterX - mTouchEllipse.Width) * (1.0 - lhorizont);// Math.Sin(l_leftDeltaPosition / mCenterX) * mCenterX;

                var l_newTopPosition = mCenterY *( 1.0 - lvertical) - mTouchEllipse.Height / 4;// mCenterY - (Math.Sin(l_leftDeltaPosition / mCenterX) * mCenterY);

                if (l_newTopPosition > mInitCanvasTop)
                    l_newTopPosition = mInitCanvasTop;

                AnalogValue = (byte)(lvertical * 255.0);

                Canvas.SetLeft(mTouchEllipse, l_newLeftPosition);

                Canvas.SetTop(mTouchEllipse, l_newTopPosition);
            }
        }

        public byte AnalogValue
        {
            get { return (byte)GetValue(AnalogValueProperty); }
            set { SetValue(AnalogValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalogValueProperty =
            DependencyProperty.Register("AnalogValue", typeof(byte), typeof(AnalogTriggerPanel), new PropertyMetadata((byte)0));

        private void UserControl_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (mTouchIsPressed)
            {
                var l_position = e.TouchDevice.GetTouchPoint(this).Position;

                var l_leftDeltaPosition = mInitCanvasLeft + l_position.X - mOffsetPosition.X;


                if (l_leftDeltaPosition < 0)
                    return;


                var lhorizont = ((mCenterX - mTouchEllipse.Width) - l_leftDeltaPosition) / (mCenterX - mTouchEllipse.Width);

                if (lhorizont > 1.0)
                    return;

                if (lhorizont < 0.0)
                    return;

                var lvertical = Math.Sqrt(1 - lhorizont * lhorizont);

                if (lvertical > 0.995)
                    lvertical = 1.0;

                var l_newLeftPosition = (mCenterX - mTouchEllipse.Width) * (1.0 - lhorizont);// Math.Sin(l_leftDeltaPosition / mCenterX) * mCenterX;

                var l_newTopPosition = mCenterY * (1.0 - lvertical) - mTouchEllipse.Height / 4;// mCenterY - (Math.Sin(l_leftDeltaPosition / mCenterX) * mCenterY);

                if (l_newTopPosition > mInitCanvasTop)
                    l_newTopPosition = mInitCanvasTop;

                AnalogValue = (byte)(lvertical * 255.0);

                Canvas.SetLeft(mTouchEllipse, l_newLeftPosition);

                Canvas.SetTop(mTouchEllipse, l_newTopPosition);
            }
        }

        private void UserControl_PreviewTouchCheck(object sender, TouchEventArgs e, MouseButtonState state)
        {
            mTouchIsPressed = state == MouseButtonState.Pressed;

            if (mTouchIsPressed)
            {
                var l_position = e.TouchDevice.GetTouchPoint(this).Position;

                mOffsetPosition.X = l_position.X;

                mOffsetPosition.Y = l_position.Y;

                e.TouchDevice.Capture(this, CaptureMode.SubTree);
            }
            else
            {
                int timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;
                MouseButton l_mouseButton = MouseButton.Left;

                MouseButtonEventArgs l_mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, l_mouseButton);
                l_mouseUpEvent.RoutedEvent = Control.MouseUpEvent;
                l_mouseUpEvent.Source = mTouchEllipse;

                mTouchEllipse.RaiseEvent(l_mouseUpEvent);

                Canvas.SetLeft(mTouchEllipse, mInitCanvasLeft);

                Canvas.SetTop(mTouchEllipse, mInitCanvasTop);

                e.TouchDevice.Capture(null);

                AnalogValue = 0;
            }
        }

        private void UserControl_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            UserControl_PreviewTouchCheck(sender, e, MouseButtonState.Pressed);
        }

        private void UserControl_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            UserControl_PreviewTouchCheck(sender, e, MouseButtonState.Released);
        }
        
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(AnalogTriggerPanel), new PropertyMetadata(""));
    }
}
