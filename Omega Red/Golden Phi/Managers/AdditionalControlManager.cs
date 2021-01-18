using Golden_Phi.Emulators;
using Golden_Phi.Tools;
using Golden_Phi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Golden_Phi.Managers
{
    delegate void ButtonUpdateCallbackDelegate(ushort a_buttons);

    class AdditionalControlManager
    {
        public event Action<ControlEnum, IPadControl> ChangeControlEvent;

        public enum ControlEnum
        {
            None,
            QuickSavePanel,
            QuickResumePanel
        }


        private bool m_button_is_pressed = false;

        private static AdditionalControlManager m_Instance = null;

        public static AdditionalControlManager Instance { get { if (m_Instance == null) m_Instance = new AdditionalControlManager(); return m_Instance; } }

        private AdditionalControlManager() { }
               
        public bool ButtonCheck(ushort aButtons, IPadControl aPadControl)
        {
            bool l_result = false;
                                 
            if (((aButtons & XInputNative.XINPUT_GAMEPAD_START) != 0) &&
                ((aButtons & ~XInputNative.XINPUT_GAMEPAD_START) != 0))
            {
                if ((aButtons & XInputNative.XINPUT_GAMEPAD_LEFT_SHOULDER) > 0)
                {
                    if (!m_button_is_pressed)
                    {
                        if (Emul.Instance.Status == Emul.StatusEnum.Started)
                        {
                            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
                            {
                                SaveStateManager.Instance.quickSave();
                            });

                            l_result = true;

                            m_button_is_pressed = true;
                        }
                    }
                }
                else
                if ((aButtons & XInputNative.XINPUT_GAMEPAD_RIGHT_SHOULDER) > 0)
                {
                    if (!m_button_is_pressed)
                    {
                        if (Emul.Instance.Status == Emul.StatusEnum.Started)
                        {
                            if (ChangeControlEvent != null)
                            {
                                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
                                {
                                    ChangeControlEvent(ControlEnum.QuickSavePanel, aPadControl);

                                    Emul.Instance.lockPause();
                                });
                            }

                            l_result = true;

                            m_button_is_pressed = true;
                        }
                    }
                }
                else
                if ((aButtons & XInputNative.XINPUT_GAMEPAD_LEFT_THUMB) > 0)
                {
                    if (!m_button_is_pressed)
                    {
                        if (Emul.Instance.Status == Emul.StatusEnum.Started)
                        {
                            if (ChangeControlEvent != null)
                            {
                                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
                                {
                                    ChangeControlEvent(ControlEnum.QuickResumePanel, aPadControl);

                                    Emul.Instance.lockPause();
                                });
                            }

                            l_result = true;

                            m_button_is_pressed = true;
                        }
                    }
                }
                else
                    m_button_is_pressed = false;
            }
            else
                m_button_is_pressed = false;


            return l_result;
        }

        public void HidePanel()
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
            {
                ChangeControlEvent(ControlEnum.None, null);

                Emul.Instance.unlockResume();
            });
        }
    }
}