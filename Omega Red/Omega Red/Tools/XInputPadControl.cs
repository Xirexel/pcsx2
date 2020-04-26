using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omega_Red.Util;

namespace Omega_Red.Tools
{
    class XInputPadControl : IPadControl
    {
        private XInputNative.XINPUT_STATE m_state = new XInputNative.XINPUT_STATE();

        private uint m_deviceIndex = uint.MaxValue;
        
        public XInputPadControl(uint a_deviceIndex)
        {
            m_deviceIndex = a_deviceIndex;
        }

        public XInputNative.XINPUT_STATE getState()
        {
            XInputNative.Instance.XInputGetState(m_deviceIndex, ref m_state);

            return m_state;
        }

        public void setVibration(uint a_vibrationCombo)
        {

            uint l_LeftMotorSpeed = a_vibrationCombo & 0xFFFF;
            uint l_RightMotorSpeed = (a_vibrationCombo >> 16) & 0xFFFF;

            var l_vibration = new XInputNative.XINPUT_VIBRATION();

            l_vibration.wLeftMotorSpeed = (ushort)l_LeftMotorSpeed;

            l_vibration.wRightMotorSpeed = (ushort)l_RightMotorSpeed;

            XInputNative.Instance.XInputSetState(m_deviceIndex, ref l_vibration);
        }
    }
}
