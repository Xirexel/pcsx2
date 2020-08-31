using Golden_Phi.Managers;
using Golden_Phi.Models;
using Golden_Phi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Golden_Phi.ViewModels
{
    [DataTemplateNameAttribute("StateInfoItem")]
    class SaveStateInfoViewModel : BaseViewModel
    {
        public SaveStateInfoViewModel()
        {
            
            IsEnabled = Emul.Emul.Instance.Status != Emul.Emul.StatusEnum.Stopped;

            CommandManager.InvalidateRequerySuggested();
        }
              
        private bool m_IsEnabled = false;
                
        public bool IsEnabled
        {
            get { return m_IsEnabled; }
            set
            {
                m_IsEnabled = value;

                RaisePropertyChangedEvent("IsEnabled");
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new DelegateCommand(SaveStateManager.Instance.addSaveStateInfo,
                    () =>
                    {
                        return Emul.Emul.Instance.Status == Emul.Emul.StatusEnum.Paused &&
                        SaveStateManager.Instance.DiscSerial == Emul.Emul.Instance.DiscSerial;
                    });
            }
        }

        public ICommand LoadCommand
        {
            get { return new DelegateCommand<SaveStateInfo>((a_SaveStateInfo)=>
            {
                var l_ContextMenu = App.getResource("SStatesMenu") as System.Windows.Controls.ContextMenu;

                if (l_ContextMenu != null)
                    l_ContextMenu.IsOpen = false;

                Emul.Emul.Instance.loadState(a_SaveStateInfo);
            }                
            ); }
        }
        
        protected override IManager Manager
        {
            get { return SaveStateManager.Instance; }
        }
    }
}
