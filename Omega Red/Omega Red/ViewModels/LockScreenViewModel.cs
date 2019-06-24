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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Omega_Red.ViewModels
{
    public class LockScreenViewModel : BaseViewModel
    {
        private System.Windows.Visibility mVisibility = System.Windows.Visibility.Visible;

        private System.Windows.Visibility mVisibilityDisplayStreamConfigPanel = System.Windows.Visibility.Collapsed;

        private System.Windows.Visibility mVisibilityDisplayImagePanel = System.Windows.Visibility.Collapsed;

        private System.Windows.Visibility mVisibilityDisplayVideoPanel = System.Windows.Visibility.Collapsed;

        private System.Windows.Visibility mVisibilityDisplayAboutPanel = System.Windows.Visibility.Collapsed;
        
        string mCurrentMessage = "";

        public LockScreenViewModel()
        {
            LockScreenManager.Instance.StatusEvent += Instance_StatusEvent;

            LockScreenManager.Instance.MessageEvent += Instance_MessageEvent;
        }

        private void reset()
        {
            VisibilityDisplayImagePanel = System.Windows.Visibility.Hidden;

            VisibilityDisplayAboutPanel = System.Windows.Visibility.Hidden;

            VisibilityDisplayVideoPanel = System.Windows.Visibility.Collapsed;

            VisibilityDisplayStreamConfigPanel = Visibility.Collapsed;
        }

        void Instance_MessageEvent(string aMessage)
        {
            Message = aMessage;
        }

        private void Instance_StatusEvent(LockScreenManager.Status aStatus)
        {
            reset();

            switch (aStatus)
            {
                case LockScreenManager.Status.None:
                    Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case LockScreenManager.Status.Show: 
                    Visibility = System.Windows.Visibility.Visible;
                    Message = "";
                    break;
                case LockScreenManager.Status.DisplayImage:
                    VisibilityDisplayImagePanel = System.Windows.Visibility.Visible;
                    break;
                case LockScreenManager.Status.DisplayVideo:
                    VisibilityDisplayVideoPanel = System.Windows.Visibility.Visible;
                    break;
                case LockScreenManager.Status.DisplayAbout:
                    VisibilityDisplayAboutPanel = System.Windows.Visibility.Visible;
                    break;
                case LockScreenManager.Status.DisplayStreamConfig:
                    VisibilityDisplayStreamConfigPanel = System.Windows.Visibility.Visible;
                    break;                    
                case LockScreenManager.Status.Starting:
                    var l_StartingTitle = App.Current.Resources["StartingTitle"];
                    Message = string.Format("{0}", l_StartingTitle);
                    break;
                case LockScreenManager.Status.Saving:
                    var l_SavingTitle = App.Current.Resources["SavingTitle"];
                    Message = string.Format("{0}", l_SavingTitle);
                    break;
                default:
                    break;
            }
        }

        public ICommand HideCommand
        {
            get { return new DelegateCommand(LockScreenManager.Instance.hide); }
        }

        public System.Windows.Controls.Image IconImage
        {
            get { return LockScreenManager.Instance.IconImage; }
        }

        public System.Windows.Visibility Visibility
        {
            get { return mVisibility; }
            set
            {
                mVisibility = value;
                RaisePropertyChangedEvent("Visibility");
            }
        }

        public System.Windows.Visibility VisibilityDisplayImagePanel
        {
            get { return mVisibilityDisplayImagePanel; }
            set
            {
                mVisibilityDisplayImagePanel = value;
                RaisePropertyChangedEvent("VisibilityDisplayImagePanel");
            }
        }

        public System.Windows.Visibility VisibilityDisplayVideoPanel
        {
            get { return mVisibilityDisplayVideoPanel; }
            set
            {
                mVisibilityDisplayVideoPanel = value;
                RaisePropertyChangedEvent("VisibilityDisplayVideoPanel");
            }
        }             

        public System.Windows.Visibility VisibilityDisplayAboutPanel
        {
            get { return mVisibilityDisplayAboutPanel; }
            set
            {
                mVisibilityDisplayAboutPanel = value;
                RaisePropertyChangedEvent("VisibilityDisplayAboutPanel");
            }
        }

        public System.Windows.Visibility VisibilityDisplayStreamConfigPanel
        {
            get { return mVisibilityDisplayStreamConfigPanel; }
            set
            {
                mVisibilityDisplayStreamConfigPanel = value;
                RaisePropertyChangedEvent("VisibilityDisplayStreamConfigPanel");
            }
        }

        public string Message
        {
            get { return mCurrentMessage; }
            set
            {
                mCurrentMessage = value;
                RaisePropertyChangedEvent("Message");
            }
        }               
        
        protected override Managers.IManager Manager
        {
            get { throw new NotImplementedException(); }
        }

        public string CaptureManagerVersion
        {
            get { return LockScreenManager.Instance.getCaptureManagerVersion(); }
        }  
    }
}
