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
            PCSX2Controller.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;
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

        private PCSX2Controller.StatusEnum m_Status = PCSX2Controller.StatusEnum.NoneInitilized;

        void Instance_m_ChangeStatusEvent(PCSX2Controller.StatusEnum a_Status)
        {
            m_Status = a_Status;

            IsEnabled = a_Status != PCSX2Controller.StatusEnum.NoneInitilized;

            VisibilityState = 
                (a_Status == PCSX2Controller.StatusEnum.Initilized || a_Status == PCSX2Controller.StatusEnum.Stopped)
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
            get { return new DelegateCommand<SaveStateInfo>(PCSX2Controller.Instance.loadState); }
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
