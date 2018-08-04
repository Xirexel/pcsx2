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

namespace Omega_Red.ViewModels
{
    class ConfigViewModel : ItemsPanelTemplateManager
    {
        public ConfigViewModel()
        {
            ConfigManager.Instance.SwitchControlModeEvent += Instance_SwitchControlModeEvent;

            ConfigManager.Instance.SwitchTopmostEvent += Instance_SwitchTopmostEvent;
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


        
    }
}
