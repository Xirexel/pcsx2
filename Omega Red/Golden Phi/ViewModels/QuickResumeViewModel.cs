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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Golden_Phi.ViewModels
{
    [DataTemplateNameAttribute("QuickResumeIsoInfoItem")]
    class QuickResumeViewModel : BaseViewModel
    {
        private enum CommandEnum
        {
            None,
            Resume,
            Cancel
        }

        private System.Timers.Timer m_update_pad_Timer = new System.Timers.Timer(50);

        private IPadControl mPadControl = null;

        private int m_quickResumeLast = 0;

        private int m_quickResumeCount = 0;

        private bool m_button_is_pressed = false;

        private CommandEnum m_CommandEnum = CommandEnum.None;

        public QuickResumeViewModel()
        {
            AdditionalControlManager.Instance.ChangeControlEvent += Instance_ChangeControlEvent1;

            m_update_pad_Timer.Elapsed += update_pad_Timer_Elapsed;

            m_update_pad_Timer.AutoReset = false;
        }

        private void Instance_ChangeControlEvent1(AdditionalControlManager.ControlEnum aControlEnum, IPadControl aPadControl)
        {
            if (aControlEnum == AdditionalControlManager.ControlEnum.QuickResumePanel)
            {
                if (Collection != null)
                    m_quickResumeCount = Collection.Cast<object>().Count();
                
                m_quickResumeLast = 0;

                m_CommandEnum = CommandEnum.None;

                m_button_is_pressed = false;

                VisibilityState = Visibility.Visible;

                setPosition(0);

                mPadControl = aPadControl;

                m_update_pad_Timer.Start();
            }
            else
            {
                VisibilityState = Visibility.Collapsed;

                m_update_pad_Timer.Stop();

                mPadControl = null;

                m_quickResumeCount = 0;
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
                                case CommandEnum.Resume:
                                    {
                                        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
                                        {
                                            var l_IsoInfo = (IsoInfo)Collection.CurrentItem;

                                            SaveStateInfo l_SaveStateInfo = null;

                                            if (l_IsoInfo != null
                                            )
                                            {
                                                var l_bios_check_sum =
                                                l_IsoInfo.BiosInfo != null ?
                                                l_IsoInfo.BiosInfo.GameType == l_IsoInfo.GameType ? "_" + l_IsoInfo.BiosInfo.CheckSum.ToString("X8") : ""
                                                : "";

                                                l_SaveStateInfo = SaveStateManager.Instance.getAutoSaveStateInfo(l_IsoInfo.DiscSerial, l_bios_check_sum);
                                            }

                                            Emul.Instance.play(l_IsoInfo, l_SaveStateInfo, false);

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
                        nextQuickResume(false);

                        m_button_is_pressed = true;
                    }
                    else if ((l_state.Gamepad.wButtons & XInputNative.XINPUT_GAMEPAD_DPAD_DOWN) != 0)
                    {
                        nextQuickResume(true);

                        m_button_is_pressed = true;
                    }
                    else if ((l_state.Gamepad.wButtons & XInputNative.XINPUT_GAMEPAD_A) != 0)
                    {
                        m_CommandEnum = CommandEnum.Resume;

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

        private void nextQuickResume(bool aForwardDirection)
        {
            if (Collection == null)
                return;

            SoundSchemaManager.Instance.playEvent(SoundSchemaManager.Event.Switch);

            if (m_quickResumeCount > 0)
            {
                if (aForwardDirection)
                {
                    ++m_quickResumeLast;

                    m_quickResumeLast = m_quickResumeLast % m_quickResumeCount;

                    setPosition(m_quickResumeLast);
                }
                else
                {
                    --m_quickResumeLast;

                    m_quickResumeLast = m_quickResumeLast % m_quickResumeCount;

                    if (m_quickResumeLast < 0)
                        m_quickResumeLast = m_quickResumeCount - 1;

                    setPosition(m_quickResumeLast);
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

        protected override IManager Manager => IsoManager.Instance;

        public new ICollectionView Collection
        {
            get
            {
                if (Manager != null)
                    return IsoManager.Instance.ResumeIsoCollection;
                else
                    return null;
            }
        }
    }
}

