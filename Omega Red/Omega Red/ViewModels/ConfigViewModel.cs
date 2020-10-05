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
using Omega_Red.Properties;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Omega_Red.Emulators.Emul;

namespace Omega_Red.ViewModels
{
    class ConfigViewModel : ItemsPanelTemplateManager
    {
        public ConfigViewModel()
        {
            ConfigManager.Instance.SwitchControlModeEvent += Instance_SwitchControlModeEvent;

            ConfigManager.Instance.SwitchTopmostEvent += Instance_SwitchTopmostEvent;

            ConfigManager.Instance.SwitchCaptureConfigEvent += Instance_SwitchCaptureConfigEvent;            

            Emul.Instance.ChangeStatusEvent += Instance_ChangeStatusEvent;

            ConfigManager.Instance.FrameRateEvent += (a_framerate) => { FrameRate = a_framerate; };
                        
            MediaRecorderManager.Instance.RecordingStateEvent += (a_state) => {

                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
                {
                    checkIsStopped();

                    IsCaptureStopped = !a_state;
                });
            };
        }

        private void Instance_SwitchCaptureConfigEvent(object obj)
        {
            CaptureConfig = obj;
        }

        private void Instance_ChangeStatusEvent(Emul.StatusEnum obj)
        {
            if (Emul.Instance.IsoInfo != null && Emul.Instance.IsoInfo.GameType == GameType.PSP)
                VisibilityDiskState = Visibility.Collapsed;
            else
                VisibilityDiskState = Visibility.Visible;



            VisibilityTexturePackMode = Visibility.Collapsed;

            if (obj == Emul.StatusEnum.Stopped ||
                obj == Emul.StatusEnum.Initilized)
                VisibilityTexturePackMode = Visibility.Visible;

            checkIsStopped();
        }

        private void checkIsStopped()
        {

            IsStopped = false;

            if ((Emul.Instance.Status == Emul.StatusEnum.Stopped ||
                Emul.Instance.Status == Emul.StatusEnum.Initilized) 
                &&
                !MediaRecorderManager.Instance.State)
                IsStopped = true;
        }

        void Instance_SwitchTopmostEvent(bool obj)
        {
            Topmost = obj;
        }

        void Instance_SwitchControlModeEvent(bool obj)
        {
            if (obj)
            {
                TouchControlVisibility = Visibility.Collapsed;

                TouchControlHideVisibility = Visibility.Visible;
            }
            else
            {
                TouchControlVisibility = Visibility.Visible;

                TouchControlHideVisibility = Visibility.Collapsed;
            }
        }

        public ICollectionView DisplayModeCollection
        {
            get { return ConfigManager.Instance.DisplayModeCollection; }
        }
               
        public ICollectionView SkipFrameModeCollection
        {
            get { return ConfigManager.Instance.SkipFrameModeCollection; }
        }

        public ICollectionView ResolutionModeCollection
        {
            get { return ConfigManager.Instance.ResolutionModeCollection; }
        }

        public ICollectionView ControlModeCollection
        {
            get { return ConfigManager.Instance.ControlModeCollection; }
        }
  

        public ICollectionView LanguageCollection
        {
            get { return ConfigManager.Instance.LanguageCollection; }
        }

        public ICollectionView ColourSchemaCollection
        {
            get { return ConfigManager.Instance.ColourSchemaCollection; }
        }

        public ICollectionView TexturePackModeCollection
        {
            get { return ConfigManager.Instance.TexturePackModeCollection; }
        }

        


        public ICollectionView MediaOutputTypeCollection
        {
            get { return ConfigManager.Instance.MediaOutputTypeCollection; }
        }

        private System.Windows.Visibility mTouchControlVisibility;

        public System.Windows.Visibility TouchControlVisibility
        {
            get { return mTouchControlVisibility; }
            set
            {
                mTouchControlVisibility = value;
                RaisePropertyChangedEvent("TouchControlVisibility");
            }
        }

        private System.Windows.Visibility mTouchControlHideVisibility;

        public System.Windows.Visibility TouchControlHideVisibility
        {
            get { return mTouchControlHideVisibility; }
            set
            {
                mTouchControlHideVisibility = value;
                RaisePropertyChangedEvent("TouchControlHideVisibility");
            }
        }

        public bool Topmost
        {
            get {

                if (App.Current != null && App.Current.MainWindow != null)
                {
                    return App.Current.MainWindow.Topmost;
                }
                else
                    return false; }
            set
            {
                if (App.Current != null && App.Current.MainWindow != null)
                {
                    App.Current.MainWindow.Topmost = value;

                    Settings.Default.Topmost = value;
                }

                RaisePropertyChangedEvent("Topmost");
            }
        }

        public bool EnableWideScreen
        {
            get
            {

                Emul.Instance.setVideoAspectRatio(!Settings.Default.DisableWideScreen ? AspectRatio.Ratio_16_9 : AspectRatio.Ratio_4_3);

                return !Settings.Default.DisableWideScreen;
            }
            set
            {

                Settings.Default.DisableWideScreen = !value;

                Emul.Instance.setVideoAspectRatio(value ? AspectRatio.Ratio_16_9 : AspectRatio.Ratio_4_3);

                RaisePropertyChangedEvent("EnableWideScreen");
            }
        }

        object mMediaOutputConfig = null;

        public object MediaOutputConfig {
            get
            {
                return mMediaOutputConfig;// MediaOutputTypeCollection.CurrentItem;
            }
            set
            {
                var lMediaOutputTypeInfo = (MediaOutputTypeInfo)value;

                if(lMediaOutputTypeInfo != null)
                    mMediaOutputConfig = lMediaOutputTypeInfo.InfoPanel();
                else
                    mMediaOutputConfig = null;

                RaisePropertyChangedEvent("MediaOutputConfig");
            }
        }

        double mTouchPadScale = 100;

        public double TouchPadScale {
            get
            {
                return (uint)Settings.Default.TouchPadScale;
            }
            set
            {
                mTouchPadScale = value;

                Settings.Default.TouchPadScale = (uint)value;

                RaisePropertyChangedEvent("TouchPadScale");
            }
        }

        public ICollectionView RenderingSchemaCollection
        {
            get { return ConfigManager.Instance.RenderingSchemaCollection; }
        }
                
        public object RenderingConfig
        {
            get
            {
                CheckBox l_checkBox = new CheckBox();

                l_checkBox.Content = "Is wired.";

                l_checkBox.VerticalContentAlignment = VerticalAlignment.Center;

                l_checkBox.Checked += (object sender, RoutedEventArgs e)=> {


                };

                l_checkBox.Unchecked += (object sender, RoutedEventArgs e) => {


                };

                return l_checkBox;
            }
        }
                       
        public double SoundLevel
        {
            get
            {
                Emul.Instance.setAudioVolume(Settings.Default.SoundLevel);

                return Settings.Default.SoundLevel;
            }
            set
            {
                Settings.Default.SoundLevel = value;
                
                Emul.Instance.setAudioVolume(Settings.Default.SoundLevel);

                RaisePropertyChangedEvent("SoundLevel");
            }
        }
        
        public bool IsMuted
        {
            get
            {
                Emul.Instance.setIsMuted(Settings.Default.IsMuted);

                return Settings.Default.IsMuted;
            }
            set
            {
                Settings.Default.IsMuted = value;
                
                Emul.Instance.setIsMuted(Settings.Default.IsMuted);

                RaisePropertyChangedEvent("IsMuted");
            }
        }

        public Visibility VisibilityState
        {
            get { return Visibility.Visible; }
        }

        public Visibility VisibilityVideoRecordingState
        {
            get { return Visibility.Visible; }
        }

        

        public bool IsFXAA
        {
            get
            {
                //ModuleControl.Instance.setIsFXAA(Settings.Default.IsFXAA);

                return Settings.Default.IsFXAA;
            }
            set
            {
                Settings.Default.IsFXAA = value;
                
                RaisePropertyChangedEvent("IsFXAA");
            }
        }

        private Visibility mVisibilityDiskState = Visibility.Visible;

        public Visibility VisibilityDiskState
        {
            get
            {
                return mVisibilityDiskState;
            }
            set
            {
                mVisibilityDiskState = value;

                RaisePropertyChangedEvent("VisibilityDiskState");
            }
        }


        private Object mCaptureConfig = null;

        public Object CaptureConfig
        {
            get
            {
                return mCaptureConfig;
            }
            set
            {
                mCaptureConfig = value;

                RaisePropertyChangedEvent("CaptureConfig");
            }
        }


        private Visibility mVisibilityTexturePackMode = Visibility.Visible;

        public Visibility VisibilityTexturePackMode
        {
            get
            {
                return mVisibilityTexturePackMode;
            }
            set
            {
                mVisibilityTexturePackMode = value;

                RaisePropertyChangedEvent("VisibilityTexturePackMode");
            }
        }


        private bool mIsStopped = false;

        public bool IsStopped
        {
            get
            {
                return mIsStopped;
            }
            set
            {
                mIsStopped = value;

                RaisePropertyChangedEvent("IsStopped");
            }
        }

        private bool mIsCaptureStopped = true;

        public bool IsCaptureStopped
        {
            get
            {
                return mIsCaptureStopped;
            }
            set
            {
                mIsCaptureStopped = value;

                RaisePropertyChangedEvent("IsCaptureStopped");
            }
        }

        public bool ShowFrameRate
        {
            get
            {
                return Settings.Default.ShowFrameRate;
            }
            set
            {
                Settings.Default.ShowFrameRate = value;

                RaisePropertyChangedEvent("ShowFrameRate");
            }
        }

        private string mFrameRate = "";

        public string FrameRate
        {
            get
            {
                return mFrameRate;
            }
            set
            {
                mFrameRate = value;

                RaisePropertyChangedEvent("FrameRate");
            }
        }
    }
}
