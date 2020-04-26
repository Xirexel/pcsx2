using Omega_Red.Models;
using Omega_Red.SocialNetworks.Google;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Omega_Red.Managers
{
    class StreamingCaptureConfigManager : IManager
    {
        public Action<bool> m_UncheckEvent;

        public Action<object> m_StreamConfigPanelEvent;

        private IStreamingManager m_Manager = null;

        private bool m_IsAllowedConfirm = false;

        private ICollectionView mCustomerView = null;

        private StreamingCaptureConfigInfo lm_PrevStreamingCaptureConfigInfo = null;

        private readonly ObservableCollection<StreamingCaptureConfigInfo> _streamingCaptureConfigInfoCollection = new ObservableCollection<StreamingCaptureConfigInfo>();

        private static StreamingCaptureConfigManager m_Instance = null;

        public static StreamingCaptureConfigManager Instance { get { if (m_Instance == null) m_Instance = new StreamingCaptureConfigManager(); return m_Instance; } }
        
        private StreamingCaptureConfigManager()
        {
            _streamingCaptureConfigInfoCollection.Add(new StreamingCaptureConfigInfo() { Name = "YouTube" });

            mCustomerView = CollectionViewSource.GetDefaultView(_streamingCaptureConfigInfoCollection);
        }

        public bool openConfig(object a_Item)
        {
            MediaRecorderManager.Instance.StartStop(false);

            MediaRecorderManager.Instance.IsAllowedStreaming = false;

            if (m_Manager != null)
                m_Manager.Dispose();

            m_Manager = null;

            if (m_StreamConfigPanelEvent != null)
                m_StreamConfigPanelEvent(null);

            bool lresult = false;

            do
            {

                lm_PrevStreamingCaptureConfigInfo = null;

                var lStreamingCaptureConfigInfo = a_Item as StreamingCaptureConfigInfo;

                if (lStreamingCaptureConfigInfo == null)
                    break;

                if(lStreamingCaptureConfigInfo.IsSelected)
                {
                    lStreamingCaptureConfigInfo.IsSelected = false;

                    lm_PrevStreamingCaptureConfigInfo = null;

                    break;
                }

                if (lm_PrevStreamingCaptureConfigInfo != null)
                    lm_PrevStreamingCaptureConfigInfo.IsSelected = false;

                if (lStreamingCaptureConfigInfo.Name == "YouTube")
                {
                    m_Manager = GoogleAccountManager.Instance.createYouTubeManager();
                }

                if (m_Manager == null)
                    break;


                if (m_StreamConfigPanelEvent != null)
                    m_StreamConfigPanelEvent(m_Manager.Panel);

                m_Manager.m_IsAllowedCofirmEvent += (obj) => {

                    m_IsAllowedConfirm = obj;
                };

                lStreamingCaptureConfigInfo.IsSelected = true;

                lm_PrevStreamingCaptureConfigInfo = lStreamingCaptureConfigInfo;

                lresult = true;

            } while (false);

            return lresult;
        }
        
        public ICollectionView Collection => mCustomerView;

        public bool accessLoadItem(object a_Item)
        {
            return false;
        }

        public bool accessPersistItem(object a_Item)
        {
            return false;
        }

        public void createItem()
        {
        }

        public void loadItemAsync(object a_Item)
        {
        }

        public void persistItemAsync(object a_Item)
        {
        }

        public void removeItem(object a_Item)
        {
        }

        public void registerItem(object a_Item)
        {
        }

    }
}
