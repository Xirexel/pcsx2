using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Golden_Phi.Emulators;
using Golden_Phi.Managers;
using Golden_Phi.Tools;

namespace Golden_Phi.ViewModels
{
    class ControlViewModel : BaseViewModel
    {
        public ControlViewModel()
        {

            Emul.Instance.ChangeStatusEvent += (a_Status) =>
            {
                StateVisibility = Visibility.Visible;

                if (a_Status == Emul.StatusEnum.Started)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                    {
                        StateVisibility = Visibility.Hidden;
                    });
                }
            };

            IsoManager.Instance.ShowChooseIsoEvent += (a_state) => {
                if (!a_state)
                {
                    CloseCommand.Execute(null);
                }
            };

            BiosManager.Instance.ShowChooseBiosEvent += (a_state) => {
                if (!a_state)
                {
                    CloseCommand.Execute(null);
                }
            };

            BiosManager.Instance.ShowViewEvent += (a_view) => {

                InteractCommand.Execute(a_view);

            };
        }


        public ICommand InteractCommand
        {
            get
            {
                return new DelegateCommand<object>((object a_Item) => {

                    var l_BaseViewModel = a_Item as BaseViewModel;

                    if(l_BaseViewModel != null)
                    {
                        InteractVisibility = System.Windows.Visibility.Visible;

                        InteractDataContext = l_BaseViewModel;

                        if (l_BaseViewModel.Collection != null)
                            l_BaseViewModel.Collection.MoveCurrentToPosition(-1);
                    }
                });
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new DelegateCommand(() => {
                    InteractVisibility = System.Windows.Visibility.Collapsed;

                    InteractDataContext = null;


                });
            }
        }
        
        private System.Windows.Visibility mInteractVisibility = System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility InteractVisibility
        {
            get { return mInteractVisibility; }
            set
            {
                mInteractVisibility = value;
                RaisePropertyChangedEvent("InteractVisibility");
            }
        }



        private System.Windows.Visibility mStateVisibility = System.Windows.Visibility.Visible;

        public System.Windows.Visibility StateVisibility
        {
            get { return mStateVisibility; }
            set
            {
                mStateVisibility = value;
                RaisePropertyChangedEvent("StateVisibility");
            }
        }


        private object mInteractDataContext = null;

        public object InteractDataContext
        {
            get { return mInteractDataContext; }
            set
            {
                mInteractDataContext = value;
                RaisePropertyChangedEvent("InteractDataContext");
            }
        }
               
        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand(() => {
                    App.Current.MainWindow.Close();
                });
            }
        }

        protected override IManager Manager => null;
    }
}
