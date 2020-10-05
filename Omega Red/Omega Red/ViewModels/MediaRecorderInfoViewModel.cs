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
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("VideoInfoItem")]
    class MediaRecorderInfoViewModel : BaseViewModel
    {
        public MediaRecorderInfoViewModel()
        {
            Emul.Instance.ChangeStatusEvent += (Emul.StatusEnum obj) => {

                switch (obj)
                {
                    case Emul.StatusEnum.Stopped:
                        //IsCheckedStatus = false;
                        break;
                    case Emul.StatusEnum.Paused:
                    case Emul.StatusEnum.NoneInitilized:
                    case Emul.StatusEnum.Initilized:
                    case Emul.StatusEnum.Started:
                    default:
                        break;
                }
            
            };

            Omega_Red.Managers.MediaRecorderManager.Instance.ChangeLockEvent += (a_state) => {

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    if (a_state)
                    {
                        LockVisibility = Visibility.Visible;
                    }
                    else
                    {
                        LockVisibility = Visibility.Collapsed;
                    }
                });
            };




            Omega_Red.Managers.MediaRecorderManager.Instance.RecordingStateEvent += (a_state) => {

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    IsCheckedStatus = a_state;
                });
            };
        }


        public ICommand StartStopRecordingCommand
        {
            get
            {
                return new DelegateCommand<Object>(Omega_Red.Managers.MediaRecorderManager.Instance.StartStop, () =>
                {
                    return Managers.MediaRecorderManager.Instance.isRecodingAllowed();
                });
            }
        }

        bool m_IsCheckedStatus = false;
        
        public bool IsCheckedStatus
        {
            get { return m_IsCheckedStatus; }
            set
            {
                m_IsCheckedStatus = value;
                RaisePropertyChangedEvent("IsCheckedStatus");
            }
        }

        private System.Windows.Visibility mLockVisibility = System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility LockVisibility
        {
            get { return mLockVisibility; }
            set
            {
                mLockVisibility = value;
                RaisePropertyChangedEvent("LockVisibility");
            }
        }
               
        public Visibility VisibilityVideoRecordingState
        {
            get { return Visibility.Visible; }
        }

        protected override Managers.IManager Manager
        {
            get { return Omega_Red.Managers.MediaRecorderManager.Instance; }
        }
    }
}
