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
    /// Interaction logic for SoundControl.xaml
    /// </summary>
    public partial class SoundControl : UserControl
    {
        public SoundControl()
        {
            InitializeComponent();
            
            DataContext = this;
        }
               
        public double SoundLevel
        {
            get
            {
                var lvalue = Volume * m_volumeSlider.Maximum;

                if (!IsMuted)
                {

                    l_lowSoundMarker.Visibility = Visibility.Hidden;

                    l_mediumSoundMarker.Visibility = Visibility.Hidden;

                    l_highSoundMarker.Visibility = Visibility.Hidden;

                    l_MutedSoundMarker.Visibility = Visibility.Hidden;

                    if (lvalue > 0.0)
                    {
                        l_lowSoundMarker.Visibility = Visibility.Visible;
                    }

                    if (lvalue >= 45.0)
                    {
                        l_mediumSoundMarker.Visibility = Visibility.Visible;
                    }

                    if (lvalue >= 90.0)
                    {
                        l_highSoundMarker.Visibility = Visibility.Visible;
                    }
                }

                return lvalue;
            }
            set
            {
                Volume = value / m_volumeSlider.Maximum;
                
                l_lowSoundMarker.Visibility = Visibility.Hidden;

                l_mediumSoundMarker.Visibility = Visibility.Hidden;

                l_highSoundMarker.Visibility = Visibility.Hidden;

                l_MutedSoundMarker.Visibility = Visibility.Hidden;

                if (value > 0.0)
                {
                    l_lowSoundMarker.Visibility = Visibility.Visible;
                }

                if (value >= 45.0)
                {
                    l_mediumSoundMarker.Visibility = Visibility.Visible;
                }

                if (value >= 90.0)
                {
                    l_highSoundMarker.Visibility = Visibility.Visible;
                }
            }
        }
        
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            l_lowSoundMarker.Visibility = Visibility.Hidden;

            l_mediumSoundMarker.Visibility = Visibility.Hidden;

            l_highSoundMarker.Visibility = Visibility.Hidden;

            l_MutedSoundMarker.Visibility = Visibility.Visible;

            m_volumeSlider.IsEnabled = false;

            m_volumeSlider.Opacity = 0.25;

            IsMuted = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            l_MutedSoundMarker.Visibility = Visibility.Hidden;

            m_volumeSlider.IsEnabled = true;

            m_volumeSlider.Opacity = 1.0;

            m_volumeSlider.Value = Volume * m_volumeSlider.Maximum;

            IsMuted = false;
        }



        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SoundControl), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        private static void OnOrientationChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var l_SoundControl = d as SoundControl;

            if(l_SoundControl != null)
            {
                var l_Orientation = (Orientation)e.NewValue;

                l_SoundControl.setOrientationValue(l_Orientation);
            }
        }

        private void setOrientationValue(Orientation a_value)
        {
            m_volumeSlider.Orientation = a_value;

            switch (a_value)
            {
                case Orientation.Vertical:
                    {
                        Grid.SetColumn(m_volumeSlider, 0);

                        Grid.SetColumn(m_MuteBtn, 0);



                        Grid.SetRow(m_volumeSlider, 0);

                        Grid.SetRow(m_MuteBtn, 1);


                        m_volumeSlider.VerticalAlignment = VerticalAlignment.Stretch;

                        m_volumeSlider.HorizontalAlignment = HorizontalAlignment.Center;


                        var ltemp = Width;

                        Width = Height;

                        Height = ltemp;

                    }
                    break;
                case Orientation.Horizontal:
                default:
                    break;
            }
        }

        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set { SetValue(IsMutedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMuted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register("IsMuted", typeof(bool), typeof(SoundControl), new PropertyMetadata(false));




        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(SoundControl), new PropertyMetadata(0.0));

        private void Slider_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }
    }
}
