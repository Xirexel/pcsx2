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
using Omega_Red.Models;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("ScreenshotInfoItem")]
    class ScreenshotsInfoViewModel : BaseViewModel
    {     
        public ScreenshotsInfoViewModel()
        {
            ScreenshotsManager.Instance.TakeScreenshotEvent += (a_state, a_source) => {

                if(a_state)
                {
                    LockVisibility = Visibility.Visible;
                }
                else
                {
                    LockVisibility = Visibility.Collapsed;
                }

                ScreenshotSource = a_source;
            };
        }

        protected override IManager Manager
        {
            get { return ScreenshotsManager.Instance; }
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

        private System.Windows.Media.ImageSource mScreenshotSource = null;

        public System.Windows.Media.ImageSource ScreenshotSource
        {
            get { return mScreenshotSource; }
            set
            {
                mScreenshotSource = value;
                RaisePropertyChangedEvent("ScreenshotSource");
            }
        }
    }
}

