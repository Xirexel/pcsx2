using Golden_Phi.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golden_Phi.ViewModels
{
    class PadControlViewModel : BaseViewModel
    {
        public PadControlViewModel()
        {
            PadControlManager.Instance.PadConfigPanelEvent += Instance_PadConfigPanelEvent;

            PadConfigPanel = PadControlManager.Instance.PadConfigPanel;
        }

        private void Instance_PadConfigPanelEvent(object obj)
        {
            PadConfigPanel = obj;
        }

        public ICollectionView PadControlCollection
        {
            get { return PadControlManager.Instance.Collection; }
        }

        private object mPadConfigPanel = null;

        public object PadConfigPanel
        {
            get { return mPadConfigPanel; }
            set
            {
                mPadConfigPanel = value;

                RaisePropertyChangedEvent("PadConfigPanel");
            }
        }

        protected override IManager Manager => null;
    }
}
