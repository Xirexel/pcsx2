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

namespace Omega_Red.ViewModels
{
    class ConfigViewModel : ItemsPanelTemplateManager
    {
        public ConfigViewModel()
        {
            ConfigManager.Instance.SwitchControlModeEvent += Instance_SwitchControlModeEvent;

            ConfigManager.Instance.SwitchTopmostEvent += Instance_SwitchTopmostEvent;

            PCSX2Controller.Instance.ChangeStatusEvent += Instance_ChangeStatusEvent;
        }

        private void Instance_ChangeStatusEvent(PCSX2Controller.StatusEnum obj)
        {
            if (PCSX2Controller.Instance.IsoInfo != null && PCSX2Controller.Instance.IsoInfo.GameType == Models.GameType.PSP)
                VisibilityDiskState = Visibility.Collapsed;
            else
                VisibilityDiskState = Visibility.Visible;
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

                    Settings.Default.Save();
                }

                RaisePropertyChangedEvent("Topmost");
            }
        }

        public bool DisableWideScreen
        {
            get {

                return Settings.Default.DisableWideScreen;
            }
            set
            {

                Settings.Default.DisableWideScreen = value;

                Settings.Default.Save();

                PCSX2Controller.Instance.setVideoAspectRatio(value ? AspectRatio.Ratio_4_3 : AspectRatio.Ratio_16_9);

                RaisePropertyChangedEvent("DisableWideScreen");
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

                    Tools.ModuleControl.Instance.setIsWired(true);

                };

                l_checkBox.Unchecked += (object sender, RoutedEventArgs e) => {

                    Tools.ModuleControl.Instance.setIsWired(false);

                };

                return l_checkBox;
            }
        }
                       
        public double SoundLevel
        {
            get
            {
                ModuleControl.Instance.setVolume(Settings.Default.SoundLevel);

                PPSSPPControl.Instance.setAudioVolume(Settings.Default.SoundLevel);

                return Settings.Default.SoundLevel;
            }
            set
            {
                Settings.Default.SoundLevel = value;

                ModuleControl.Instance.setVolume(Settings.Default.SoundLevel);

                PPSSPPControl.Instance.setAudioVolume(Settings.Default.SoundLevel);

                Settings.Default.Save();

                RaisePropertyChangedEvent("SoundLevel");
            }
        }
        
        public bool IsMuted
        {
            get
            {
                ModuleControl.Instance.setIsMuted(Settings.Default.IsMuted);

                PPSSPPControl.Instance.setIsMuted(Settings.Default.IsMuted);

                return Settings.Default.IsMuted;
            }
            set
            {
                Settings.Default.IsMuted = value;

                ModuleControl.Instance.setIsMuted(Settings.Default.IsMuted);

                PPSSPPControl.Instance.setIsMuted(Settings.Default.IsMuted);

                Settings.Default.Save();

                RaisePropertyChangedEvent("IsMuted");
            }
        }

        public Visibility VisibilityState
        {
            get { return App.m_AppType == App.AppType.Screen ? Visibility.Visible : Visibility.Collapsed; }
        }
        
        public bool IsFXAA
        {
            get
            {
                ModuleControl.Instance.setIsFXAA(Settings.Default.IsFXAA);

                return Settings.Default.IsFXAA;
            }
            set
            {
                Settings.Default.IsFXAA = value;

                Settings.Default.Save();

                ModuleControl.Instance.setIsFXAA(Settings.Default.IsFXAA);

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
    }
}
