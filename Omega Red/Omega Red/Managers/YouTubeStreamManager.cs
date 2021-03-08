using Omega_Red.Models;
using Omega_Red.Properties;
using Omega_Red.SocialNetworks.Google;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Managers
{
    class YouTubeStreamManager : IManager
    {
        YouTubeManager m_Manager = null;

        public YouTubeStreamManager(YouTubeManager a_Manager)
        {
            m_Manager = a_Manager;
        }

        public ICollectionView Collection => m_Manager.StreamCollection;

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
            m_Manager.addStream();
        }

        public void loadItemAsync(object a_Item)
        { }

        public void registerItem(object a_Item)
        {
        }

        public void persistItemAsync(object a_Item)
        {
            var lYouTubeStreamInfo = a_Item as YouTubeStreamInfo;
            
            do
            {
                if (lYouTubeStreamInfo == null)
                    break;

                m_Manager.persistStream(lYouTubeStreamInfo);

            } while (false);

        }

        public void removeItem(object a_Item)
        {
            var lYouTubeStreamInfo = a_Item as YouTubeStreamInfo;

            do
            {
                if (lYouTubeStreamInfo == null)
                    break;

                m_Manager.deleteStream(lYouTubeStreamInfo);
                
                Settings.Default.YouTubeStreamSelectedId = "";

            } while (false);
        }
    }
}
