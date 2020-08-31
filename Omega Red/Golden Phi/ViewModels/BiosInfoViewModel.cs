using Golden_Phi.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golden_Phi.ViewModels
{
    [DataTemplateNameAttribute("BiosInfoItem")]
    internal class BiosInfoViewModel : BaseViewModel
    {
        public BiosInfoViewModel()
        {
            ItemsPanelTemplateSelector = SelectTemplate("TileItemsPanel");

            BiosManager.Instance.IsoInfoUpdated += (obj)=> {
                var l_ContextMenu = App.getResource("SelectBIOSMenu") as System.Windows.Controls.ContextMenu;

                if (l_ContextMenu != null)
                    l_ContextMenu.IsOpen = false;
            };
        }

        protected override IManager Manager => BiosManager.Instance;
    }
}
