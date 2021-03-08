using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Tools
{
    class DirectInputPadControl : IPadControl
    {
        private DirectInputDevice m_DirectInputDevice = null;

        IntPtr m_ptrDIJoystick = IntPtr.Zero;

        int m_DIJoystickSize = 0;

        public DirectInputPadControl(DirectInputDevice a_DirectInputDevice)
        {
            m_DirectInputDevice = a_DirectInputDevice;

            m_DIJoystickSize = Marshal.SizeOf(typeof(DIJOYSTATE));

            m_ptrDIJoystick = Marshal.AllocCoTaskMem(m_DIJoystickSize);   
        }

        ~DirectInputPadControl()
        {
            Marshal.FreeCoTaskMem(m_ptrDIJoystick);

            m_ptrDIJoystick = IntPtr.Zero;
        }

        public XInputNative.XINPUT_STATE getState()
        {
            var l_state = new XInputNative.XINPUT_STATE();

            try
            {
                m_DirectInputDevice.Poll();

                if(m_ptrDIJoystick != IntPtr.Zero)
                {
                    var l_res = m_DirectInputDevice.GetDeviceState(m_DIJoystickSize, m_ptrDIJoystick);

                    if(l_res == 0)
                    {

                        var l_DIJOYSTATE = (DIJOYSTATE)Marshal.PtrToStructure(m_ptrDIJoystick, typeof(DIJOYSTATE));

                        l_state.dwPacketNumber = 100;

                        l_state.Gamepad.sThumbLX = (short)(l_DIJOYSTATE.lX);

                        l_state.Gamepad.sThumbLY = (short)(-l_DIJOYSTATE.lY);

                        l_state.Gamepad.sThumbRX = (short)l_DIJOYSTATE.lZ;

                        l_state.Gamepad.sThumbRY = (short)(-l_DIJOYSTATE.lRz);


                        Console.WriteLine("DirectInputPadControl - sThumbLX: {0}, sThumbLY: {1}, sThumbRX: {2}, sThumbRY: {3}, ", 
                            l_state.Gamepad.sThumbLX, 
                            l_state.Gamepad.sThumbLY,
                            l_state.Gamepad.sThumbRX,
                            l_state.Gamepad.sThumbRY);
                                                
                        if (l_DIJOYSTATE.rgdwPOV[0] == 0)
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_UP);
                        else
                        if (l_DIJOYSTATE.rgdwPOV[0] < 9000)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_UP);
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_RIGHT);
                        }
                        else
                        if (l_DIJOYSTATE.rgdwPOV[0] == 9000)
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_RIGHT);
                        else
                        if (l_DIJOYSTATE.rgdwPOV[0] < 18000)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_RIGHT);
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_DOWN);
                        }
                        else
                        if (l_DIJOYSTATE.rgdwPOV[0] == 18000)
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_DOWN);
                        else
                        if (l_DIJOYSTATE.rgdwPOV[0] < 27000)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_LEFT);
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_DOWN);
                        }
                        else
                        if (l_DIJOYSTATE.rgdwPOV[0] == 27000)
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_LEFT);
                        else
                        if (l_DIJOYSTATE.rgdwPOV[0] > 27000 && l_DIJOYSTATE.rgdwPOV[0] < 36000)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_LEFT);
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_DPAD_UP);
                        }

                        if (l_DIJOYSTATE.rgbButtons[0] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_X);
                        }

                        if (l_DIJOYSTATE.rgbButtons[1] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_A);
                        }

                        if (l_DIJOYSTATE.rgbButtons[2] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_B);
                        }

                        if (l_DIJOYSTATE.rgbButtons[3] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_Y);
                        }

                        if (l_DIJOYSTATE.rgbButtons[4] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_LEFT_SHOULDER);
                        }

                        if (l_DIJOYSTATE.rgbButtons[5] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_RIGHT_SHOULDER);
                        }

                        if (l_DIJOYSTATE.rgbButtons[6] == 0x80)
                        {
                            l_state.Gamepad.bLeftTrigger = 255;
                        }

                        if (l_DIJOYSTATE.rgbButtons[7] == 0x80)
                        {
                            l_state.Gamepad.bRightTrigger = 255;
                        }

                        if (l_DIJOYSTATE.rgbButtons[8] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_BACK);
                        }

                        if (l_DIJOYSTATE.rgbButtons[9] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_START);
                        }

                        if (l_DIJOYSTATE.rgbButtons[10] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_LEFT_THUMB);
                        }

                        if (l_DIJOYSTATE.rgbButtons[11] == 0x80)
                        {
                            setKey(ref l_state, XInputNative.XINPUT_GAMEPAD_RIGHT_THUMB);
                        }
                    }

                }
            }
            catch (Exception)
            {
            }

            return l_state;
        }
        private void setKey(ref XInputNative.XINPUT_STATE a_state, UInt16 a_Key)
        {
            a_state.Gamepad.wButtons |= a_Key;
        }

        public void setVibration(uint a_vibrationCombo)
        {
        }
    }
}