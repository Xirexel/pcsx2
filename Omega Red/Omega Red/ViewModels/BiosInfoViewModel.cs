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
using Omega_Red.Panels;
using Omega_Red.Properties;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Omega_Red.ViewModels
{

    [DataTemplateNameAttribute("BiosInfoItem")]
    public class BiosInfoViewModel : BaseViewModel
    {
        protected override Managers.IManager Manager
        {
            get { return BiosManager.Instance; }
        }

        public BiosInfoViewModel()
        {
            Emul.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;

            if(Collection != null)
                Collection.CollectionChanged += Collection_CollectionChanged;
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
            IsEnabled = a_Status == Emul.StatusEnum.Stopped 
                || a_Status == Emul.StatusEnum.Initilized
                || a_Status == Emul.StatusEnum.NoneInitilized;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                CurrentBiosZone = "";
                
                if (Emul.Instance.BiosInfo == null)
                    return;

                string l_prefix = ": ";

                if (!string.IsNullOrWhiteSpace(Emul.Instance.BiosInfo.ContainerFile.ToString()))
                {
                    CurrentContainerFile = l_prefix + Emul.Instance.BiosInfo.ContainerFile.ToString();

                    l_prefix = " - ";
                }

                CurrentBiosZone = l_prefix + Emul.Instance.BiosInfo.Zone + " " + Emul.Instance.BiosInfo.Version;
            });
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
            get {

                TipInfoPanel l_TipInfoPanel = new TipInfoPanel();

                l_TipInfoPanel.addHyperLink(@"https://www.google.com.au/search?client=opera&q=ps2+bios+pack&sourceid=opera&ie=UTF-8&oe=UTF-8");
                
                var l_TipTitle = App.Current.Resources["BiosTipTitle"];

                if (l_TipTitle != null)
                    l_TipInfoPanel.setTipTitle(l_TipTitle.ToString());

                return l_TipInfoPanel;
            }
        }

        private string mCurrentContainerFile = "";

        public string CurrentContainerFile
        {
            get { return mCurrentContainerFile; }
            set
            {
                mCurrentContainerFile = value;

                RaisePropertyChangedEvent("CurrentContainerFile");
            }
        }

        private string mCurrentBiosZone = "";

        public string CurrentBiosZone
        {
            get { return mCurrentBiosZone; }
            set
            {
                mCurrentBiosZone = value;

                RaisePropertyChangedEvent("CurrentBiosZone");
            }
        }        
    }
}


