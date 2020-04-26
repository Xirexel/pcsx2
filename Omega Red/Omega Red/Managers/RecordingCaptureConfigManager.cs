using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Omega_Red.Managers
{
    class RecordingCaptureConfigManager : IManager
    {
        public Action<bool> m_UncheckEvent;

        private bool m_IsAllowedConfirm = false;

        private ICollectionView mCustomerView = null;
        
        private static RecordingCaptureConfigManager m_Instance = null;

        public static RecordingCaptureConfigManager Instance { get { if (m_Instance == null) m_Instance = new RecordingCaptureConfigManager(); return m_Instance; } }

        private RecordingCaptureConfigManager(){}

        public bool openConfig(object a_Item)
        {
            MediaRecorderManager.Instance.StartStop(false);

            MediaRecorderManager.Instance.IsAllowedStreaming = false;

            return false;
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

