using Golden_Phi.Emulators;
using Golden_Phi.Managers;
using Golden_Phi.Models;
using Golden_Phi.Tools;
using Golden_Phi.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Golden_Phi.ViewModels
{
    [DataTemplateNameAttribute("QuickStateInfoItem")]
    class QuickSaveStateViewModel : BaseViewModel
    {
        private enum CommandEnum
        {
            None,
            Load,
            Cancel
        }

        private System.Timers.Timer m_update_pad_Timer = new System.Timers.Timer(50);

        private IPadControl mPadControl = null;

        private int m_quickLoadLast = 0;

        private int m_quickLoadCount = 0;

        private bool m_button_is_pressed = false;

        private CommandEnum m_CommandEnum = CommandEnum.None;

        public QuickSaveStateViewModel()
        {
            AdditionalControlManager.Instance.ChangeControlEvent += Instance_ChangeControlEvent1;

            m_update_pad_Timer.Elapsed += update_pad_Timer_Elapsed;

            m_update_pad_Timer.AutoReset = false;
        }

        private void Instance_ChangeControlEvent1(AdditionalControlManager.ControlEnum aControlEnum, IPadControl aPadControl)
        {
            if (aControlEnum == AdditionalControlManager.ControlEnum.QuickSavePanel)
            {
                if (Collection != null)
                {
                    var l_IsoInfo = IsoManager.Instance.getIsoInfo(Emul.Instance.DiscSerial);

                    SaveStateManager.Instance.setIsoInfoFilter(l_IsoInfo);

                    if (Collection != null)
                        m_quickLoadCount = Collection.Cast<object>().Count();
                }
                
                m_quickLoadLast = 0;

                m_CommandEnum = CommandEnum.None;

                m_button_is_pressed = false;

                VisibilityState = Visibility.Visible;

                setPosition(0);

                mPadControl = aPadControl;

                m_update_pad_Timer.Start();
                
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    setPosition(0);
                });
            }
            else
            {
                VisibilityState = Visibility.Collapsed;

                m_update_pad_Timer.Stop();

                mPadControl = null;

                m_quickLoadCount = 0;
            }
        }

        private void setPosition(int aPosition)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
            {
                Collection.MoveCurrentToPosition(aPosition);
            });
        }

        private void update_pad_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var lPadControl = mPadControl;

            if (lPadControl != null)
            {
                var l_state = lPadControl.getState();

                if (l_state.Gamepad.wButtons == 0)
                {
                    lock (this)
                    {

                        if (m_button_is_pressed && m_CommandEnum != CommandEnum.None)
                        {
                            switch (m_CommandEnum)
                            {
                                case CommandEnum.None:
                                    break;
                                case CommandEnum.Load:
                                    {
                                        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
                                        {
                                            Emul.Instance.loadState((SaveStateInfo)Collection.CurrentItem);
                                            
                                            AdditionalControlManager.Instance.HidePanel();
                                        });
                                    }
                                    break;
                                case CommandEnum.Cancel:
                                    hide();
                                    break;
                                default:
                                    break;
                            }

                            return;
                        }

                        m_button_is_pressed = false;
                    }
                }

                if (!m_button_is_pressed)
                {
                    if ((l_state.Gamepad.wButtons & XInputNative.XINPUT_GAMEPAD_DPAD_UP) != 0)
                    {
                        nextQuickLoad(false);

                        m_button_is_pressed = true;
                    }
                    else if ((l_state.Gamepad.wButtons & XInputNative.XINPUT_GAMEPAD_DPAD_DOWN) != 0)
                    {
                        nextQuickLoad(true);

                        m_button_is_pressed = true;
                    }
                    else if ((l_state.Gamepad.wButtons & XInputNative.XINPUT_GAMEPAD_A) != 0)
                    {
                        m_CommandEnum = CommandEnum.Load;

                        SoundSchemaManager.Instance.playEvent(SoundSchemaManager.Event.Click);

                        m_button_is_pressed = true;
                    }
                    else if ((l_state.Gamepad.wButtons & XInputNative.XINPUT_GAMEPAD_Y) != 0)
                    {
                        m_CommandEnum = CommandEnum.Cancel;

                        SoundSchemaManager.Instance.playEvent(SoundSchemaManager.Event.Click);

                        m_button_is_pressed = true;
                    }
                }

                m_update_pad_Timer.Start();
            }
        }

        public void nextQuickLoad(bool aForwardDirection)
        {
            if (Collection == null)
                return;

            SoundSchemaManager.Instance.playEvent(SoundSchemaManager.Event.Switch);

            if (m_quickLoadCount == 0 && Collection != null)
                m_quickLoadCount = Collection.Cast<object>().Count();

            if (m_quickLoadCount > 0)
            {
                if (aForwardDirection)
                {
                    ++m_quickLoadLast;

                    m_quickLoadLast = m_quickLoadLast % m_quickLoadCount;

                    setPosition(m_quickLoadLast);
                }
                else
                {
                    --m_quickLoadLast;

                    m_quickLoadLast = m_quickLoadLast % m_quickLoadCount;

                    if (m_quickLoadLast < 0)
                        m_quickLoadLast = m_quickLoadCount - 1;

                    setPosition(m_quickLoadLast);
                }
            }
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

        public bool IsEnabled
        {
            get { return m_IsEnabled; }
            set
            {
                m_IsEnabled = value;

                RaisePropertyChangedEvent("IsEnabled");
            }
        }

        private void hide()
        {
            lock (this)
            {
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
                {
                    VisibilityState = Visibility.Collapsed;

                    Emul.Instance.unlockResume();

                    m_CommandEnum = CommandEnum.None;

                    m_button_is_pressed = false;
                });
            }
        }


        public ICommand CloseCommand
        {
            get { return new DelegateCommand(hide); }
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
                    return SaveStateManager.Instance.QuickSaveCollection;
                else
                    return null;
            }
        }
    }
}
