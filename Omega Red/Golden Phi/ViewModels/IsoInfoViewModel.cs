using Golden_Phi.Managers;
using Golden_Phi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golden_Phi.ViewModels
{
    [DataTemplateNameAttribute("IsoInfoItem")]
    class IsoInfoViewModel : BaseViewModel
    {
        public IsoInfoViewModel()
        {
        }            

        protected override IManager Manager => IsoManager.Instance;
    }
}
