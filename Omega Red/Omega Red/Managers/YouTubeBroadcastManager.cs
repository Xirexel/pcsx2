using Omega_Red.Models;
using Omega_Red.Properties;
using Omega_Red.SocialNetworks.Google;
using System.ComponentModel;

namespace Omega_Red.Managers
{
    class YouTubeBroadcastManager : IManager
    {
        YouTubeManager m_Manager = null;

        public YouTubeBroadcastManager(YouTubeManager a_Manager)
        {
            m_Manager = a_Manager;
        }

        public ICollectionView Collection => m_Manager.BroadcastCollection;

        public bool accessLoadItem(object a_Item)
        {
            return false;
        }

        public bool accessPersistItem(object a_Item)
        {
            return true;
        }

        public void createItem()
        {
            m_Manager.addBroadcast();
        }

        public void loadItemAsync(object a_Item)
        { }

        public void persistItemAsync(object a_Item)
        {
            var lYouTubeBroadcastInfo = a_Item as YouTubeBroadcastInfo;

            do
            {
                if (lYouTubeBroadcastInfo == null)
                    break;

                m_Manager.persistBroadcast(lYouTubeBroadcastInfo);

            } while (false);

        }

        public void removeItem(object a_Item)
        {
            var lYouTubeBroadcastInfo = a_Item as YouTubeBroadcastInfo;

            do
            {
                if (lYouTubeBroadcastInfo == null)
                    break;

                m_Manager.deleteBroadcast(lYouTubeBroadcastInfo);

                Settings.Default.YouTubeBroadcastSelectedId = "";

            } while (false);
        }

        public void registerItem(object a_Item)
        {
        }
    }
}
