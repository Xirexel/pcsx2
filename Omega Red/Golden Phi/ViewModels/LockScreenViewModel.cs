using Golden_Phi.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golden_Phi.ViewModels
{
    class LockScreenViewModel : BaseViewModel
    {
        public LockScreenViewModel()
        {
            LockScreenManager.Instance.StatusEvent += Instance_StatusEvent;

            LockScreenManager.Instance.MessageEvent += (aMessage) => { Message = aMessage; };
        }

        private void Instance_StatusEvent(LockScreenManager.Status aStatus)
        {
            Message = "";

            switch (aStatus)
            {
                case LockScreenManager.Status.None:
                    Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case LockScreenManager.Status.Show:
                    Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    break;
            }
        }


        private System.Windows.Visibility mVisibility = System.Windows.Visibility.Visible;

        public System.Windows.Visibility Visibility
        {
            get { return mVisibility; }
            set
            {
                mVisibility = value;
                RaisePropertyChangedEvent("Visibility");
            }
        }

        public System.Windows.Controls.Image BackImage
        {
            get { return LockScreenManager.Instance.BackImage; }
        }

        public System.Windows.Controls.Image IconImage
        {
            get { return LockScreenManager.Instance.IconImage; }
        }

        string mCurrentMessage = "";

        public string Message
        {
            get { return mCurrentMessage; }
            set
            {
                mCurrentMessage = value;
                RaisePropertyChangedEvent("Message");
            }
        }

        protected override IManager Manager => null;
    }
}
