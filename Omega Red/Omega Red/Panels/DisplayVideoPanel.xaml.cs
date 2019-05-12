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
using System.Windows.Threading;

namespace Omega_Red.Panels
{
    /// <summary>
    /// Interaction logic for DisplayVideoPanel.xaml
    /// </summary>
    public partial class DisplayVideoPanel : UserControl
    {
        enum Status
        {
            Playing,
            Pausing,
            Stopped           
        }

        private Status mStatus = Status.Stopped;

        private DispatcherTimer mTickTimer = new DispatcherTimer();

        private bool mIsSeek = false;
                
        private double mNewValue = -1.0;

        private long mMediaDuration = 0;

        public event Action<string> StatusEvent;

        DispatcherTimer l_HideControlTimer = new DispatcherTimer();

        public DisplayVideoPanel()
        {
            InitializeComponent();

            mTickTimer.Interval = TimeSpan.FromMilliseconds(100);

            mTickTimer.Tick += mTickTimer_Tick;

            l_HideControlTimer.Interval = new TimeSpan(0, 0, 5);

            l_HideControlTimer.Tick += (object send_arg, EventArgs e_arg) =>
            {
                mControlPanel.Visibility = Visibility.Hidden;

                l_HideControlTimer.Stop();
            };

            mVideoPlayer.Volume = 0.25;
        }

        void update(Status aStatus)
        {
            mStatus = aStatus;

            if(StatusEvent != null)
            {
                if (aStatus == Status.Playing)
                    StatusEvent("Started");
                else
                    StatusEvent("Stopped");
            }
        }

        void mTickTimer_Tick(object sender, EventArgs e)
        {
            
            if (mMediaDuration == 0)
            {
                if (mVideoPlayer.NaturalDuration.HasTimeSpan)
                {
                    mMediaDuration = mVideoPlayer.NaturalDuration.TimeSpan.Ticks;
                }
            }

            if (mMediaDuration == 0)
                return;

            if (mIsSeek)
                return;

            double l_propTime = ((double)((double)mVideoPlayer.Position.Ticks / (double)mMediaDuration));
            
            mTimelineSlider.Value = l_propTime * mTimelineSlider.Maximum;

            if (mVideoPlayer.NaturalDuration.HasTimeSpan)
                mCurrentTimeTextBlock.Text = String.Format("{0} / {1}", mVideoPlayer.Position.ToString(@"mm\:ss"), mVideoPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

            if (l_propTime >= 0.99)
                Stop();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mIsSeek)
            {
                mNewValue = e.NewValue;
            }   
        }

        public void Release()
        {
            Stop();

            mVideoPlayer.Close();

            mYouTubeChkBx.IsChecked = false;
        }

        public void Stop()
        {
            update(Status.Stopped);

            mTickTimer.Stop();

            mVideoPlayer.Stop();

            mTimelineSlider.Value = 0;

            mVideoPlayer.Position = new TimeSpan(0);

            var lDuration = new TimeSpan(0);

            if (mVideoPlayer.NaturalDuration.HasTimeSpan)
                lDuration = mVideoPlayer.NaturalDuration.TimeSpan;

            mCurrentTimeTextBlock.Text = String.Format("{0} / {1}", mVideoPlayer.Position.ToString(@"mm\:ss"), lDuration.ToString(@"mm\:ss"));
        }

        public void setSource(Uri a_UriMediaFile)
        {
            Release();

            mControlPanel.Visibility = Visibility.Visible;

            mMediaDuration = 0;

            mVideoPlayer.Source = a_UriMediaFile;

            mVideoTitleTxtbx.Tag = a_UriMediaFile;

            mVideoPlayer.Position = TimeSpan.FromMilliseconds(0);
         
            mVideoTitleTxtbx.Text = System.IO.Path.GetFileNameWithoutExtension(a_UriMediaFile.OriginalString);

            Play();
        }

        public void PlayPause()
        {
            if (mStatus != Status.Playing)
                Play();
            else
                if (mStatus == Status.Playing)
                    Pause();
        }

        private void Play()
        {
            update(Status.Playing);

            mVideoPlayer.Play();

            mTickTimer.Start();
        }

        private void Pause()
        {
            update(Status.Pausing);

            mVideoPlayer.Pause();

            mTickTimer.Stop();
        }

        private void mSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mIsSeek = true;

            if(mStatus != Status.Stopped)
            {
                mVideoPlayer.Pause();

                mTickTimer.Stop();
            }
        }

        private void mSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mIsSeek = false;

            if (mNewValue >= 0.0)
            {
                double lselectedProp = mNewValue / mTimelineSlider.Maximum;

                double startPosition = lselectedProp * (double)mMediaDuration;

                TimeSpan lStartTimeSpan = new TimeSpan((long)startPosition);

                mVideoPlayer.Position = lStartTimeSpan;

                if (mStatus == Status.Playing)
                    mVideoPlayer.Play();
            }

            mTickTimer.Start();
        }

        private void mSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            mIsSeek = false;

            if(mStatus == Status.Playing)
            {
                mVideoPlayer.Play();

                mTickTimer.Start();
            }
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            mControlPanel.Visibility = Visibility.Visible;

            l_HideControlTimer.Start();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            m_YouTubeVideoMetaDataPanel.Visibility = (bool)(sender as CheckBox).IsChecked? Visibility.Visible : Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mYouTubeChkBx.IsChecked = false;
        }
    }
}
