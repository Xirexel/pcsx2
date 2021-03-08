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
    /// Interaction logic for AnalogStickPanel.xaml
    /// </summary>
    public partial class AnalogStickPanel : UserControl
    {
        double mInitCanvasLeft = 0.0;

        double mInitCanvasTop = 0.0;

        double mCenterX = 0.0;

        double mCenterY = 0.0;
        
        public AnalogStickPanel()
        {
            InitializeComponent();

            mInitCanvasLeft = (this.Width - mTouchEllipse.Width) / 2.0;

            mInitCanvasTop = (this.Height - mTouchEllipse.Height) / 2.0;

            mCenterX = Width/2.0;

            mCenterY = Height/2.0;
        }

        bool mIsPressed = false;

        bool mTouchIsPressed = false;

        Point mOffsetPosition = new Point();

       
        private void Ellipse_MouseCheck(object sender, MouseButtonEventArgs e)
        {
            mIsPressed = e.ButtonState == MouseButtonState.Pressed;

            if(mIsPressed)
            {
                var l_position = e.GetPosition(this);

                mOffsetPosition.X = l_position.X - mCenterX;

                mOffsetPosition.Y = l_position.Y - mCenterY;

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

                Axises = new TouchPadPanel.XY_Axises() { m_x_axis = 0, m_y_axis = 0 };
            }
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if(mIsPressed)
            {
                var l_position = e.GetPosition(this);

                var l_leftDeltaPosition = l_position.X - mCenterX;

                var l_topDeltaPosition = l_position.Y - mCenterY;

                var l_newLeftPosition = (mInitCanvasLeft + l_leftDeltaPosition) - mOffsetPosition.X;

                var l_newTopPosition = (mInitCanvasTop + l_topDeltaPosition) - mOffsetPosition.Y;

                l_leftDeltaPosition = l_newLeftPosition - (mCenterX - mInitCanvasLeft) + mTouchEllipse.Width / 4;

                l_topDeltaPosition = -(l_newTopPosition - (mCenterY - mInitCanvasTop) + mTouchEllipse.Height / 4);

                
                double l_d_X_axis = ((double)l_leftDeltaPosition / (double)mCenterX);

                double l_d_Y_axis = ((double)l_topDeltaPosition / (double)mCenterY);


                l_d_X_axis = Math.Abs(l_d_X_axis) >= 1.0 ? Math.Sign(l_d_X_axis) : l_d_X_axis;

                l_d_Y_axis = Math.Abs(l_d_Y_axis) >= 1.0 ? Math.Sign(l_d_Y_axis) : l_d_Y_axis;


                double l_d_X_sensetive_axis = l_d_X_axis * 2.0;

                double l_d_Y_sensetive_axis = l_d_Y_axis * 2.0;


                l_d_X_sensetive_axis = Math.Abs(l_d_X_sensetive_axis) >= 1.0 ? Math.Sign(l_d_X_sensetive_axis) : l_d_X_sensetive_axis;

                l_d_Y_sensetive_axis = Math.Abs(l_d_Y_sensetive_axis) >= 1.0 ? Math.Sign(l_d_Y_sensetive_axis) : l_d_Y_sensetive_axis;


                short l_x_axis = (short)(32767.0 * l_d_X_sensetive_axis);

                short l_y_axis = (short)(32767.0 * l_d_Y_sensetive_axis);

                double l_length = l_d_X_axis * l_d_X_axis + l_d_Y_axis * l_d_Y_axis;

                if(l_length < 0.1)
                {
                    l_x_axis = 0;

                    l_y_axis = 0;
                }

                double l_delta = 1.0;

                if (l_length > 1.0)
                {
                    l_delta = Math.Sqrt(l_length);

                    l_d_X_axis /= l_delta;

                    l_d_Y_axis /= l_delta;
                }

                l_newLeftPosition = mInitCanvasLeft + (mCenterX * l_d_X_axis);

                l_newTopPosition = mInitCanvasTop - (mCenterY * l_d_Y_axis);
                                

                Axises = new TouchPadPanel.XY_Axises() { m_x_axis = l_x_axis, m_y_axis = l_y_axis };


                Canvas.SetLeft(mTouchEllipse, l_newLeftPosition);

                Canvas.SetTop(mTouchEllipse, l_newTopPosition);
            }
        }
        
        public TouchPadPanel.XY_Axises Axises
        {
            get { return (TouchPadPanel.XY_Axises)GetValue(AxisesProperty); }
            set { SetValue(AxisesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Axises.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisesProperty =
            DependencyProperty.Register("Axises", typeof(TouchPadPanel.XY_Axises), typeof(AnalogStickPanel), new PropertyMetadata(new TouchPadPanel.XY_Axises() { m_x_axis = 0, m_y_axis = 0 }));
        
        private void UserControl_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (mTouchIsPressed)
            {
                var l_position = e.TouchDevice.GetTouchPoint(this).Position;

                var l_leftDeltaPosition = l_position.X - mCenterX;

                var l_topDeltaPosition = l_position.Y - mCenterY;

                var l_newLeftPosition = (mInitCanvasLeft + l_leftDeltaPosition) - mOffsetPosition.X;

                var l_newTopPosition = (mInitCanvasTop + l_topDeltaPosition) - mOffsetPosition.Y;

                l_leftDeltaPosition = l_newLeftPosition - (mCenterX - mInitCanvasLeft) + mTouchEllipse.Width / 4;

                l_topDeltaPosition = -(l_newTopPosition - (mCenterY - mInitCanvasTop) + mTouchEllipse.Height / 4);


                double l_d_X_axis = ((double)l_leftDeltaPosition / (double)mCenterX);

                double l_d_Y_axis = ((double)l_topDeltaPosition / (double)mCenterY);


                l_d_X_axis = Math.Abs(l_d_X_axis) >= 1.0 ? Math.Sign(l_d_X_axis) : l_d_X_axis;

                l_d_Y_axis = Math.Abs(l_d_Y_axis) >= 1.0 ? Math.Sign(l_d_Y_axis) : l_d_Y_axis;


                short l_x_axis = (short)(32767.0 * l_d_X_axis);

                short l_y_axis = (short)(32767.0 * l_d_Y_axis);

                double l_length = l_d_X_axis * l_d_X_axis + l_d_Y_axis * l_d_Y_axis;

                if (l_length < 0.1)
                {
                    l_x_axis = 0;

                    l_y_axis = 0;
                }

                double l_delta = 1.0;

                if (l_length > 1.0)
                {
                    l_delta = Math.Sqrt(l_length);

                    l_d_X_axis /= l_delta;

                    l_d_Y_axis /= l_delta;
                }

                l_newLeftPosition = mInitCanvasLeft + (mCenterX * l_d_X_axis);

                l_newTopPosition = mInitCanvasTop - (mCenterY * l_d_Y_axis);


                Axises = new TouchPadPanel.XY_Axises() { m_x_axis = l_x_axis, m_y_axis = l_y_axis };


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

                mOffsetPosition.X = l_position.X - mCenterX;

                mOffsetPosition.Y = l_position.Y - mCenterY;

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

                Axises = new TouchPadPanel.XY_Axises() { m_x_axis = 0, m_y_axis = 0 };
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
    }
}
