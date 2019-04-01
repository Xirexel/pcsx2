using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Managers
{
    delegate void ButtonUpdateCallbackDelegate(ushort a_buttons);

    class AdditionalControlManager
    {
        private static AdditionalControlManager m_Instance = null;

        public static AdditionalControlManager Instance { get { if (m_Instance == null) m_Instance = new AdditionalControlManager(); return m_Instance; } }

        public ButtonUpdateCallbackDelegate ButtonUpdateCallback = null;

        private AdditionalControlManager()
        {
            ButtonUpdateCallback = ButtonUpdateCallbackInner;
        }

        const ushort XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100;

        const ushort XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200;


        private void ButtonUpdateCallbackInner(ushort aButtons)
        {
            if((aButtons & XINPUT_GAMEPAD_LEFT_SHOULDER) > 0)
            {
                PCSX2Controller.Instance.quickSave();
            }
            else
            if ((aButtons & XINPUT_GAMEPAD_RIGHT_SHOULDER) > 0)
            {
                PCSX2Controller.Instance.quickLoad();
            }
        }
    }
}
