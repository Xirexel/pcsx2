using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Golden_Phi.Managers;
using Golden_Phi.Tools;

namespace Golden_Phi.ViewModels
{
    class EmulViewModel : BaseViewModel
    {
        public ICommand InteractCommand
        {
            get
            {
                return new DelegateCommand(() => {

                    Emul.Emul.Instance.pause();
                
                });
            }
        }

        protected override IManager Manager => null;
    }
}
