
#include "stdafx.h"
#include "Global.h"
#include <VersionHelpers.h>
#include <xinput.h>
#include "VKey.h"
#include "InputManager.h"
#include "XInputEnum.h"
#include "Config.h"

//// Extra enum
//#define XINPUT_GAMEPAD_GUIDE 0x0400
//
//typedef struct
//{
//	float SCP_UP;
//	float SCP_RIGHT;
//	float SCP_DOWN;
//	float SCP_LEFT;
//
//	float SCP_LX;
//	float SCP_LY;
//
//	float SCP_L1;
//	float SCP_L2;
//	float SCP_L3;
//
//	float SCP_RX;
//	float SCP_RY;
//
//	float SCP_R1;
//	float SCP_R2;
//	float SCP_R3;
//
//	float SCP_T;
//	float SCP_C;
//	float SCP_X;
//	float SCP_S;
//
//	float SCP_SELECT;
//	float SCP_START;
//
//	float SCP_PS;
//
//} SCP_EXTN;
//
//
//// This way, I don't require that XInput junk be installed.
//typedef void(CALLBACK *_XInputEnable)(BOOL enable);
//typedef DWORD(CALLBACK *_XInputGetStateEx)(DWORD dwUserIndex, XINPUT_STATE *pState);
//typedef DWORD(CALLBACK *_XInputGetExtended)(DWORD dwUserIndex, SCP_EXTN *pPressure);
//typedef DWORD(CALLBACK *_XInputSetState)(DWORD dwUserIndex, XINPUT_VIBRATION *pVibration);
//
//_XInputEnable pXInputEnable = 0;
//_XInputGetStateEx pXInputGetStateEx = 0;
//_XInputGetExtended pXInputGetExtended = 0;
//_XInputSetState pXInputSetState = 0;


struct TOUCH_PAD_GAMEPAD {
	WORD  wButtons;
	BYTE  bLeftTrigger;
	BYTE  bRightTrigger;
	SHORT sThumbLX;
	SHORT sThumbLY;
	SHORT sThumbRX;
	SHORT sThumbRY;
};

struct TOUCH_PAD_STATE {
	DWORD          dwPacketNumber;
	TOUCH_PAD_GAMEPAD Gamepad;
};



typedef DWORD(STDAPICALLTYPE *VibrationCallback)(
    DWORD VibrationCombo);


// Completely unncessary, really.
__forceinline int ShortToAxis(int v)
{
	// If positive and at least 1 << 14, increment.
	v += (!((v >> 15) & 1)) & ((v >> 14) & 1);
	// Just double.
	return v * 2;
}

class TouchPadInput : public Device
{
	// Cached last vibration values by pad and motor.
	// Need this, as only one value is changed at a time.
	int ps2Vibration[2][4][2];
	// Minor optimization - cache last set vibration values
	// When there's no change, no need to do anything.
	XINPUT_VIBRATION xInputVibration;

	GetTouchPadCallback m_getTouchPad = nullptr;

public:
	int index;

	TouchPadInput(int index, wchar_t *displayName)
		: Device(TOUCH_PAD, OTHER, displayName)
	{
		memset(ps2Vibration, 0, sizeof(ps2Vibration));
		memset(&xInputVibration, 0, sizeof(xInputVibration));
		this->index = index;
		int i;
		for (i = 0; i < 17; i++) { // Skip empty bit
			AddPhysicalControl(PRESSURE_BTN, i + (i > 10), 0);
		}
		for (; i < 21; i++) {
			AddPhysicalControl(ABSAXIS, i + 2, 0);
		}
		AddFFAxis(L"Slow Motor", 0);
		AddFFAxis(L"Fast Motor", 1);
		AddFFEffectType(L"Constant Effect", L"Constant", EFFECT_CONSTANT);
	}

	wchar_t *GetPhysicalControlName(PhysicalControl *c)
	{
		const static wchar_t *names[] = {
			L"D-pad Up",
			L"D-pad Down",
			L"D-pad Left",
			L"D-pad Right",
			L"Start",
			L"Back",
			L"Left Thumb",
			L"Right Thumb",
			L"Left Shoulder",
			L"Right Shoulder",
			L"Guide",
			L"A",
			L"B",
			L"X",
			L"Y",
			L"Left Trigger",
			L"Right Trigger",
			L"Left Thumb X",
			L"Left Thumb Y",
			L"Right Thumb X",
			L"Right Thumb Y",
		};
		unsigned int i = (unsigned int)(c - physicalControls);
		if (i < 21) {
			return (wchar_t *)names[i];
		}
		return Device::GetPhysicalControlName(c);
	}

	int Activate(InitInfo *initInfo)
	{
		if (active)
			Deactivate();

		active = 1;
		AllocState();

		if (initInfo->m_getTouchPad != nullptr)
            m_getTouchPad = initInfo->m_getTouchPad;

		return 1;
	}

	int Update()
	{
		if (!active)
			return 0;
		XINPUT_STATE state;

		if (m_getTouchPad != nullptr)
		{
            state = *((_XINPUT_STATE *)m_getTouchPad(index));

			auto buttons = state.Gamepad.wButtons;
			for (int i = 0; i < 15; i++) {

				//if (buttons > 0)
				{
					auto h = buttons >> physicalControls[i].id;

					auto jk = h & 1;

					auto j = jk << 16;

					physicalControlState[i] = j;
				}
			}
			physicalControlState[15] = (int)(state.Gamepad.bLeftTrigger * 257.005);
			physicalControlState[16] = (int)(state.Gamepad.bRightTrigger * 257.005);
			physicalControlState[17] = ShortToAxis(state.Gamepad.sThumbLX);
			physicalControlState[18] = ShortToAxis(state.Gamepad.sThumbLY);
			physicalControlState[19] = ShortToAxis(state.Gamepad.sThumbRX);
			physicalControlState[20] = ShortToAxis(state.Gamepad.sThumbRY);
		}

		//if (ERROR_SUCCESS != pXInputGetStateEx(index, &state)) {
		//	Deactivate();
		//	return 0;
		//}
		//SCP_EXTN pressure;
		//if (!pXInputGetExtended || (ERROR_SUCCESS != pXInputGetExtended(index, &pressure))) {
		//	int buttons = state.Gamepad.wButtons;
		//	for (int i = 0; i < 15; i++) {
		//		physicalControlState[i] = ((buttons >> physicalControls[i].id) & 1) << 16;
		//	}
		//	physicalControlState[15] = (int)(state.Gamepad.bLeftTrigger * 257.005);
		//	physicalControlState[16] = (int)(state.Gamepad.bRightTrigger * 257.005);
		//	physicalControlState[17] = ShortToAxis(state.Gamepad.sThumbLX);
		//	physicalControlState[18] = ShortToAxis(state.Gamepad.sThumbLY);
		//	physicalControlState[19] = ShortToAxis(state.Gamepad.sThumbRX);
		//	physicalControlState[20] = ShortToAxis(state.Gamepad.sThumbRY);
		//}
		//else {
		//	physicalControlState[0] = (int)(pressure.SCP_UP * FULLY_DOWN);
		//	physicalControlState[1] = (int)(pressure.SCP_DOWN * FULLY_DOWN);
		//	physicalControlState[2] = (int)(pressure.SCP_LEFT * FULLY_DOWN);
		//	physicalControlState[3] = (int)(pressure.SCP_RIGHT * FULLY_DOWN);
		//	physicalControlState[4] = (int)(pressure.SCP_START * FULLY_DOWN);
		//	physicalControlState[5] = (int)(pressure.SCP_SELECT * FULLY_DOWN);
		//	physicalControlState[6] = (int)(pressure.SCP_L3 * FULLY_DOWN);
		//	physicalControlState[7] = (int)(pressure.SCP_R3 * FULLY_DOWN);
		//	physicalControlState[8] = (int)(pressure.SCP_L1 * FULLY_DOWN);
		//	physicalControlState[9] = (int)(pressure.SCP_R1 * FULLY_DOWN);
		//	physicalControlState[10] = (int)(pressure.SCP_PS * FULLY_DOWN);
		//	physicalControlState[11] = (int)(pressure.SCP_X * FULLY_DOWN);
		//	physicalControlState[12] = (int)(pressure.SCP_C * FULLY_DOWN);
		//	physicalControlState[13] = (int)(pressure.SCP_S * FULLY_DOWN);
		//	physicalControlState[14] = (int)(pressure.SCP_T * FULLY_DOWN);
		//	physicalControlState[15] = (int)(pressure.SCP_L2 * FULLY_DOWN);
		//	physicalControlState[16] = (int)(pressure.SCP_R2 * FULLY_DOWN);
		//	physicalControlState[17] = (int)(pressure.SCP_LX * FULLY_DOWN);
		//	physicalControlState[18] = (int)(pressure.SCP_LY * FULLY_DOWN);
		//	physicalControlState[19] = (int)(pressure.SCP_RX * FULLY_DOWN);
		//	physicalControlState[20] = (int)(pressure.SCP_RY * FULLY_DOWN);
		//}
		return 1;
	}

	void SetEffects(unsigned char port, unsigned int slot, unsigned char motor, unsigned char force)
	{
		ps2Vibration[port][slot][motor] = force;
		int newVibration[2] = { 0, 0 };
		for (int p = 0; p < 2; p++) {
			for (int s = 0; s < 4; s++) {
				int padtype = config.padConfigs[p][s].type;
				for (int i = 0; i < pads[p][s][padtype].numFFBindings; i++) {
					// Technically should also be a *65535/BASE_SENSITIVITY, but that's close enough to 1 for me.
					ForceFeedbackBinding *ffb = &pads[p][s][padtype].ffBindings[i];
					newVibration[0] += (int)((ffb->axes[0].force * (__int64)ps2Vibration[p][s][ffb->motor]) / 255);
					newVibration[1] += (int)((ffb->axes[1].force * (__int64)ps2Vibration[p][s][ffb->motor]) / 255);
				}
			}
		}
		newVibration[0] = abs(newVibration[0]);
		if (newVibration[0] > 65535) {
			newVibration[0] = 65535;
		}
		newVibration[1] = abs(newVibration[1]);
		if (newVibration[1] > 65535) {
			newVibration[1] = 65535;
		}
		if (newVibration[0] || newVibration[1] || newVibration[0] != xInputVibration.wLeftMotorSpeed || newVibration[1] != xInputVibration.wRightMotorSpeed) {
			XINPUT_VIBRATION newv = { (WORD)newVibration[0], (WORD)newVibration[1] };
			//if (ERROR_SUCCESS == pXInputSetState(index, &newv)) {
			//	xInputVibration = newv;
			//}

			DWORD l_VibrationCombo = (WORD)newVibration[0] | (((WORD)newVibration[1]) << 16);

			if (ptrVibrationCallback != nullptr)
                ((VibrationCallback)ptrVibrationCallback)(l_VibrationCombo);

			xInputVibration = newv;
		}
	}

	void SetEffect(ForceFeedbackBinding *binding, unsigned char force)
	{
		PadBindings pBackup = pads[0][0][0];
		pads[0][0][0].ffBindings = binding;
		pads[0][0][0].numFFBindings = 1;
		SetEffects(0, 0, binding->motor, 255);
		pads[0][0][0] = pBackup;
	}

	void Deactivate()
	{
		memset(&xInputVibration, 0, sizeof(xInputVibration));
		memset(ps2Vibration, 0, sizeof(ps2Vibration));
		FreeState();

		if (active) {
			active = 0;
		}
	}

	~TouchPadInput()
	{
	}
};

void EnumTouchPadDevices()
{
	wchar_t temp[30];

	for (int i = 0; i < 4; i++) {
		wsprintfW(temp, L"Touch Pad %i", i);
		dm->AddDevice(new TouchPadInput(i, temp));
	}
}