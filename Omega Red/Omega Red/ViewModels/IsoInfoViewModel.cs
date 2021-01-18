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

using Microsoft.Win32;
using Omega_Red.Emulators;
using Omega_Red.Managers;
using Omega_Red.Models;
using Omega_Red.Panels;
using Omega_Red.Properties;
using Omega_Red.SocialNetworks.Google;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("IsoInfoItem")]
    class IsoInfoViewModel : BaseViewModel
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        protected override IManager Manager
        {
            get { return IsoManager.Instance; }
        }

        public IsoInfoViewModel()
        {
            Emul.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;

            if (Collection != null)
                Collection.CollectionChanged += Collection_CollectionChanged;

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

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Collection.IsEmpty)
                VisibilityTipInfo = Visibility.Visible;
            else
                VisibilityTipInfo = Visibility.Collapsed;
        }

        private bool m_IsEnabled = true;

        void Instance_m_ChangeStatusEvent(Emul.StatusEnum a_Status)
        {
            CurrentGameTitle = "";

            IsEnabled = a_Status == Emul.StatusEnum.Stopped
                || a_Status == Emul.StatusEnum.Initilized
                || a_Status == Emul.StatusEnum.NoneInitilized;

            if (Emul.Instance.IsoInfo == null)
                return;

            CurrentGameTitle = ": " + Emul.Instance.IsoInfo.Title;
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

        private Visibility mVisibilityTipInfo = Visibility.Collapsed;

        public Visibility VisibilityTipInfo
        {
            get { return mVisibilityTipInfo; }
            set
            {
                mVisibilityTipInfo = value;

                RaisePropertyChangedEvent("VisibilityTipInfo");
            }
        }

        public object TipInfo
        {
            get
            {
                TipInfoPanel l_TipInfoPanel = new TipInfoPanel();

                l_TipInfoPanel.addHyperLink(@"https://www.google.com.au/search?client=opera&hs=L8e&ei=qVuYXNvvO430rQGIqpz4Dw&q=ps2+iso&oq=ps2+iso&gs_l=psy-ab.3..0i67l4j0l5j0i67.6261.7402..9089...0.0..0.222.916.0j2j3......0....1..gws-wiz.......0i71j35i39j0i131i67.PaCwxXUwLWY");

                l_TipInfoPanel.addHyperLink(@"https://www.google.com.au/search?client=opera&hs=Soz&q=psp+isos&sa=X&ved=0ahUKEwiGhf78vJzhAhWEbysKHRRpDEsQ7xYIKygA&biw=1496&bih=723");


                var l_TipTitle = App.Current.Resources["IsoTipTitle"];

                if(l_TipTitle != null)
                    l_TipInfoPanel.setTipTitle(l_TipTitle.ToString());

                return l_TipInfoPanel;
            }
        }

        private string mCurrentGameTitle = "";

        public string CurrentGameTitle
        {
            get { return mCurrentGameTitle; }
            set
            {
                mCurrentGameTitle = value;

                RaisePropertyChangedEvent("CurrentGameTitle");
            }
        }
    }
}
