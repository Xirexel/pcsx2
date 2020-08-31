using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Golden_Phi.Managers;
using Golden_Phi.Models;

namespace Golden_Phi.ViewModels
{
    [DataTemplateNameAttribute("RecentIsoInfoItem")]
    class RecentIsoInfoViewModel : BaseViewModel
    {
        public RecentIsoInfoViewModel()
        {
            ItemsPanelTemplateSelector = SelectTemplate("HorizontalButtonItemsPanel");
        }
        
        public System.Windows.Input.ICommand DCCommand
        {
            get
            {
                return new Tools.DelegateCommand<IsoInfo>((a_Item) => {

                    SaveStateInfo l_SaveStateInfo = null;

                    if (a_Item != null 
                    )
                    {
                        var l_bios_check_sum =
                        a_Item.BiosInfo != null ?
                        a_Item.BiosInfo.GameType == a_Item.GameType ? "_" + a_Item.BiosInfo.CheckSum.ToString("X8") : ""
                        : "";

                        l_SaveStateInfo = SaveStateManager.Instance.getAutoSaveStateInfo(a_Item.DiscSerial, l_bios_check_sum);
                    }

                    Emul.Emul.Instance.play(a_Item, l_SaveStateInfo);
                });
            }
        }
        
        public System.Windows.Input.ICommand ShowSavingsCommand
        {
            get
            {
                return new Tools.DelegateCommand<IsoInfo>((a_Item) => {
                    if(a_Item != null)
                        SaveStateManager.Instance.showSavings(a_Item);
                });
            }
        }
        
        public new ICollectionView Collection
        {
            get { return IsoManager.Instance.RecentIsoCollection; }
        }

        protected override IManager Manager => IsoManager.Instance;
    }
}
