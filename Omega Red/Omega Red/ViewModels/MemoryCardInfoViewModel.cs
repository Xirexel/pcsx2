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
using Omega_Red.Models;
using Omega_Red.SocialNetworks.Google;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("MemoryCardInfoItem")]
    class MemoryCardInfoViewModel : BaseViewModel
    {
        public MemoryCardInfoViewModel()
        {
            Emul.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;

            GoogleAccountManager.Instance.mEnableStateEvent += Instance_mEnableStateEvent;
        }

        private void Instance_mEnableStateEvent(bool obj)
        {
            IsVisibilityGoogleAccount = obj ? Visibility.Visible : Visibility.Collapsed;

            if (obj)
                GoogleAccountIsEnabled = GoogleAccountManager.Instance.isAuthorized;
            else
                GoogleAccountIsEnabled = false;
        }

        protected override Managers.IManager Manager
        {
            get { return MemoryCardManager.Instance; }
        }
               
        private bool m_IsEnabled = false;

        private Emul.StatusEnum m_Status = Emul.StatusEnum.NoneInitilized;
        
        void Instance_m_ChangeStatusEvent(Emul.StatusEnum a_Status)
        {
            m_Status = a_Status;

            IsEnabled = a_Status != Emul.StatusEnum.NoneInitilized;
        } 

        public bool IsEnabled
        {
            get { return m_IsEnabled; }
            set
            {
                m_IsEnabled = value;

                RaisePropertyChangedEvent("IsEnabled");
            }
        }


        private Visibility m_IsVisibilityGoogleAccount = Visibility.Collapsed;

        public Visibility IsVisibilityGoogleAccount
        {
            get { return m_IsVisibilityGoogleAccount; }
            set
            {
                m_IsVisibilityGoogleAccount = value;

                RaisePropertyChangedEvent("IsVisibilityGoogleAccount");
            }
        }


        private bool mGoogleAccountIsEnabled = false;

        public bool GoogleAccountIsEnabled
        {
            get { return mGoogleAccountIsEnabled; }
            set
            {
                mGoogleAccountIsEnabled = value;

                RaisePropertyChangedEvent("GoogleAccountIsEnabled");
            }
        }
    }
}
