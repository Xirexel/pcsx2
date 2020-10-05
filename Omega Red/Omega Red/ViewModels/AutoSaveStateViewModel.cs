using Omega_Red.Emulators;
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
using System.Windows.Input;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("AutoStateInfoItem")]
    class AutoSaveStateViewModel : BaseViewModel
    {
        public AutoSaveStateViewModel()
        {
            Emul.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;

            SaveStateManager.Instance.RefreshEvent += Instance_RefreshEvent;
        }

        private void Instance_RefreshEvent()
        {
            VisibilityState =
                (m_Status == Emul.StatusEnum.Initilized || m_Status == Emul.StatusEnum.Stopped)
                && !SaveStateManager.Instance.AutoSaveCollection.IsEmpty ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility mVisibilityState = Visibility.Collapsed;

        public Visibility VisibilityState
        {
            get { return mVisibilityState; }
            set
            {
                mVisibilityState = value;

                RaisePropertyChangedEvent("VisibilityState");
            }
        }

        private bool m_IsEnabled = false;

        private Emul.StatusEnum m_Status = Emul.StatusEnum.NoneInitilized;

        void Instance_m_ChangeStatusEvent(Emul.StatusEnum a_Status)
        {
            m_Status = a_Status;

            IsEnabled = a_Status != Emul.StatusEnum.NoneInitilized;

            VisibilityState = 
                (a_Status == Emul.StatusEnum.Initilized || a_Status == Emul.StatusEnum.Stopped)
                && !SaveStateManager.Instance.AutoSaveCollection.IsEmpty? 
                Visibility.Visible : Visibility.Collapsed;

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

        public ICommand LoadCommand
        {
            get { return new DelegateCommand<SaveStateInfo>(Emul.Instance.loadState); }
        }
        
        protected override IManager Manager
        {
            get { return SaveStateManager.Instance; }
        }

        public new ICollectionView Collection
        {
            get
            {
                if (Manager != null)
                    return SaveStateManager.Instance.AutoSaveCollection;
                else
                    return null;
            }
        }
    }
}
