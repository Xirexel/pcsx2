using Omega_Red.Managers;
using Omega_Red.Models;
using Omega_Red.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("YouTubeItem")]
    class YouTubeBroadcastViewModel : BaseViewModel
    {
        private IManager m_Manager = null;

        public YouTubeBroadcastViewModel(IManager a_Manager)
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

                var lYouTubeBroadcastInfo = SelectedItem as YouTubeBroadcastInfo;

                if (lYouTubeBroadcastInfo != null)
                {
                    Settings.Default.YouTubeBroadcastSelectedId = lYouTubeBroadcastInfo.Id;
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
