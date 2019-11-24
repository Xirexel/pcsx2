using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Omega_Red.Managers;
using Omega_Red.Models;
using Omega_Red.Properties;
using Omega_Red.SocialNetworks.Google;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("YouTubeItem")]
    class YouTubeStreamViewModel : BaseViewModel
    {
        private IManager m_Manager = null;

        public YouTubeStreamViewModel(IManager a_Manager)
        {
            m_Manager = a_Manager;

            Instance_SwitchControlModeEvent(true);

            IsSyncEnabled = false;

            m_Manager.Collection.CurrentChanged += (object sender, EventArgs e) => {

                var lICollectionView = sender as ICollectionView;

                IsSyncEnabled = false;

                if (lICollectionView == null)
                    return;

                SelectedItem = lICollectionView.CurrentItem;

                if (SelectedItem == null)
                    return;

                var lYouTubeStreamInfo = SelectedItem as YouTubeStreamInfo;

                if(lYouTubeStreamInfo != null)
                {
                    Settings.Default.YouTubeStreamSelectedId = lYouTubeStreamInfo.Id;
                }

                IsSyncEnabled = true;
            };
        }

        object m_SelectedItem = false;

        public object SelectedItem
        {
            get { return m_SelectedItem; }
            set
            {
                m_SelectedItem = value;
                RaisePropertyChangedEvent("SelectedItem");
            }
        }

        private bool m_IsSyncEnabled = false;
        
        public bool IsSyncEnabled
        {
            get { return m_IsSyncEnabled; }
            set
            {
                m_IsSyncEnabled = value;

                RaisePropertyChangedEvent("IsSyncEnabled");
            }
        }


        protected override IManager Manager => m_Manager;        
    }
}
