
void __cdecl ConLog(char const *, ...) {}

void __cdecl FileLog(char const *, ...) {}

void __cdecl SysMessage(char const *, ...) {}

void SPU2writeLog(const char *action, unsigned int rmem, unsigned short value) {}

void __cdecl RecordWrite(struct StereoOut16 const &) {}

void __cdecl UpdateDebugDialog(void) {}


int __cdecl DspProcess(short *, int) { return 0; }


namespace WaveDump
{
enum CoreSourceType {
    // Core's input stream, usually pulled from ADMA streams.
    CoreSrc_Input = 0,

    // Output of the actual 24 input voices which have dry output enabled.
    CoreSrc_DryVoiceMix,

    // Output of the actual 24 input voices that have wet output enabled.
    CoreSrc_WetVoiceMix,

    // Wet mix including inputs and externals, prior to the application of reverb.
    CoreSrc_PreReverb,

    // Wet mix after reverb has turned it into a pile of garbly gook.
    CoreSrc_PostReverb,

    // Final output of the core.  For core 0, it's the feed into Core1.
    // For Core1, it's the feed into SndOut.
    CoreSrc_External,

    CoreSrc_Count
};

void Open() {}
void Close() {}
void WriteCore(unsigned int coreidx, CoreSourceType src, short left, short right) {}
void WriteCore(unsigned int coreidx, CoreSourceType src, const StereoOut16 &sample) {}
} // namespace WaveDump



bool _MsgToConsole = false;
bool _MsgKeyOnOff = false;
bool _MsgVoiceOff = false;
bool _DMALog = false;
bool _MsgDMA = false;
bool _MsgAutoDMA = false;
bool _MsgOverruns = false;
bool _MsgCache = false;


bool DebugEnabled = false;
bool WavRecordEnabled = false;

int SndOutLatencyMS = 100;
int SynchMode = 0; // Time Stretch, Async or Disabled

bool dspPluginEnabled = false;
unsigned int delayCycles = 4;


unsigned int OutputModule = 1;

float VolumeAdjustC;
float VolumeAdjustFR;
float VolumeAdjustBL;
float VolumeAdjustSR;
float VolumeAdjustFL;
float VolumeAdjustBR;
float VolumeAdjustLFE;
float VolumeAdjustSL;

bool AdvancedVolumeControl = false;

bool EffectsDisabled = false;
bool postprocess_filter_dealias = false;

// MIXING
int Interpolation = 0;
/* values:
		0: no interpolation (use nearest)
		1. linear interpolation
		2. cubic interpolation
		3. hermite interpolation
		4. catmull-rom interpolation
*/


