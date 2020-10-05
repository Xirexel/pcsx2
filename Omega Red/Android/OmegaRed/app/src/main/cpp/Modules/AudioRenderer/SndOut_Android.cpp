/* SPU2-X, A plugin for Emulating the Sound Processing Unit of the Playstation 2
 * Developed and maintained by the Pcsx2 Development Team.
 *
 * Original portions from SPU2ghz are (c) 2008 by David Quintana [gigaherz]
 *
 * SPU2-X is free software: you can redistribute it and/or modify it under the terms
 * of the GNU Lesser General Public License as published by the Free Software Found-
 * ation, either version 3 of the License, or (at your option) any later version.
 *
 * SPU2-X is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE.  See the GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with SPU2-X.  If not, see <http://www.gnu.org/licenses/>.
 */

#include "src/Global.h"
#include <src/SndOut.h>
#include "../../../../../../../../Modules/AudioRenderer/androidconfig.h"


#include <SLES/OpenSLES.h>
#include <SLES/OpenSLES_Android.h>

#include <stdio.h>
#include <assert.h>


#include <android/log.h>

#define LOG_LEVEL 4
#define LOG_TAG "Omega Red"

#define LOGU(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_UNKNOWN, LOG_TAG, __VA_ARGS__);}
#define LOGD(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_DEFAULT, LOG_TAG, __VA_ARGS__);}
#define LOGV(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_VERBOSE, LOG_TAG, __VA_ARGS__);}
#define LOGDE(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_DEBUG, LOG_TAG, __VA_ARGS__);}
#define LOGI(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_INFO, LOG_TAG, __VA_ARGS__);}
#define LOGW(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_WARN, LOG_TAG, __VA_ARGS__);}
#define LOGE(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_ERROR, LOG_TAG, __VA_ARGS__);}
#define LOGF(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_FATAL, LOG_TAG, __VA_ARGS__);}
#define LOGS(level, ...) if (level <= LOG_LEVEL) {__android_log_print(ANDROID_LOG_SILENT, LOG_TAG, __VA_ARGS__);}



//extern SetDataCallback g_setAudioData;

//
//struct ds_device_data
//{
//    std::wstring name;
//    GUID guid;
//    bool hasGuid;
//};
//
//static void Verifyc(HRESULT hr, const char *fn)
//{
//    if (FAILED(hr)) {
//        assert(0);
//        throw std::runtime_error("DirectSound returned an error from %s");
//    }
//}
//
//// Items Specific to DirectSound
//#define STRFY(x) #x
//#define verifyc(x) Verifyc(x, STRFY(x))



// buffer queue player interfaces
SLObjectItf bqPlayerObject;
SLPlayItf bqPlayerPlay;
SLAndroidSimpleBufferQueueItf bqPlayerBufferQueue;
SLEffectSendItf bqPlayerEffectSend;
SLVolumeItf bqPlayerVolume;

static const uint MAX_BUFFER_COUNT = 8;
static const int PacketsPerBuffer = 8;
static const int BufferSize = SndOutPacketSize * PacketsPerBuffer;
static const int BufferSizeBytes = BufferSize * sizeof(short);
static short outBuffer[BufferSize];



// this callback handler is called every time a buffer finishes playing
void bqPlayerCallback(SLAndroidSimpleBufferQueueItf bq, void *context) {

	SLresult result;

	short *p1 = outBuffer;

	for (int p = 0; p < PacketsPerBuffer; p++, p1 += SndOutPacketSize)
		SndBuffer::ReadSamples(p1);

	result = (*bqPlayerBufferQueue)->Enqueue(bqPlayerBufferQueue, outBuffer,  BufferSizeBytes);
	// the most likely other result is SL_RESULT_BUFFER_INSUFFICIENT,
	// which for this code example would indicate a programming error
	assert(SL_RESULT_SUCCESS == result);
}


class OpenSLESOutModule : public SndOutModule
{
private:

	u8 m_NumBuffers;

	int noMoreData = 0;

//engine interfaces
	SLEngineItf engineEngine;
	SLObjectItf engineObject;


//output mix interfaces
	SLObjectItf outputMixObject;
	SLEnvironmentalReverbItf outputMixEnvironmentalReverb;

	// aux effect on the output mix, used by the buffer queue player
	const SLEnvironmentalReverbSettings reverbSettings =
		SL_I3DL2_ENVIRONMENT_PRESET_STONECORRIDOR;




//    LONG CurrentVolume = DSBVOLUME_MAX;
//
//    //////////////////////////////////////////////////////////////////////////////////////////
//    // Configuration Vars
//
//    std::wstring m_Device;
//    bool m_DisableGlobalFocus;
//    bool m_UseHardware;
//
//    ds_device_data m_devices[32];
//    int ndevs;
//    GUID DevGuid; // currently employed GUID.
//    bool haveGuid;
//
//    //////////////////////////////////////////////////////////////////////////////////////////
//    // Instance vars
//
//    int channel;
//    int myLastWrite; // last write position, in bytes
//
//    bool dsound_running;
//    HANDLE thread;
//    DWORD tid;
//
//    IDirectSound8 *dsound;
//    IDirectSoundBuffer8 *buffer;
//    IDirectSoundNotify8 *buffer_notify;
//    HANDLE buffer_events[MAX_BUFFER_COUNT];
//
//    WAVEFORMATEX wfx;
//
//    HANDLE waitEvent;
//
//    template <typename T>
//    static DWORD CALLBACK RThread(DSound *obj)
//    {
//        return obj->Thread<T>();
//    }
//
//    template <typename T>
//    DWORD CALLBACK Thread()
//    {
//        static const int BufferSizeBytes = BufferSize * sizeof(T);
//
//        while (dsound_running) {
//            u32 rv = WaitForMultipleObjects(m_NumBuffers, buffer_events, FALSE, 200);
//
//            T *p1, *oldp1;
//            LPVOID p2;
//            DWORD s1, s2;
//
//            u32 poffset = BufferSizeBytes * rv;
//
//            if (FAILED(buffer->Lock(poffset, BufferSizeBytes, (LPVOID *)&p1, &s1, &p2, &s2, 0))) {
//                assert(0);
//                fputs("* SPU2-X: Directsound Warning > Buffer lock failure.  You may need to increase\n\tyour configured DSound buffer count.\n", stderr);
//                continue;
//            }
//            oldp1 = p1;
//

//
//            if (g_setAudioData != nullptr)
//                g_setAudioData(oldp1, BufferSizeBytes);
//
//            buffer->Unlock(oldp1, s1, p2, s2);
//
//            // Set the write pointer to the beginning of the next block.
//            myLastWrite = (poffset + BufferSizeBytes) & ~BufferSizeBytes;
//        }
//        return 0;
//    }

public:
    s32 Init()
    {

		m_NumBuffers = 8;



		SLresult result;

		// create engine
		result = slCreateEngine(&engineObject, 0, NULL, 0, NULL, NULL);
		assert(SL_RESULT_SUCCESS == result);

		// realize the engine
		result = (*engineObject)->Realize(engineObject, SL_BOOLEAN_FALSE);
		assert(SL_RESULT_SUCCESS == result);

		// get the engine interface, which is needed in order to create other objects
		result = (*engineObject)->GetInterface(engineObject, SL_IID_ENGINE, &engineEngine);
		assert(SL_RESULT_SUCCESS == result);

		// create output mix, with environmental reverb specified as a non-required interface
		const SLInterfaceID ids[1] = {SL_IID_ENVIRONMENTALREVERB};
		const SLboolean req[1] = {SL_BOOLEAN_FALSE};
		result = (*engineEngine)->CreateOutputMix(engineEngine, &outputMixObject, 1, ids, req);
		assert(SL_RESULT_SUCCESS == result);

		// realize the output mix
		result = (*outputMixObject)->Realize(outputMixObject, SL_BOOLEAN_FALSE);
		assert(SL_RESULT_SUCCESS == result);

		// get the environmental reverb interface
		// this could fail if the environmental reverb effect is not available,
		// either because the feature is not present, excessive CPU load, or
		// the required MODIFY_AUDIO_SETTINGS permission was not requested and granted
//		result = (*outputMixObject)->GetInterface(outputMixObject, SL_IID_ENVIRONMENTALREVERB,
//												  &outputMixEnvironmentalReverb);
//		if (SL_RESULT_SUCCESS == result) {
//			result = (*outputMixEnvironmentalReverb)->SetEnvironmentalReverbProperties(
//				outputMixEnvironmentalReverb, &reverbSettings);
//		}

		createBufferQueueAudioPlayer();

//        //
//        // Initialize DSound
//        //
//        GUID cGuid;
//
//        try {
//            if (m_Device.empty())
//                throw std::runtime_error("screw it");
//
//            if ((FAILED(IIDFromString(m_Device.c_str(), &cGuid))) ||
//                FAILED(DirectSoundCreate8(&cGuid, &dsound, NULL)))
//                throw std::runtime_error("try again?");
//        } catch (std::runtime_error &) {
//            // if the GUID failed, just open up the default dsound driver:
//            if (FAILED(DirectSoundCreate8(NULL, &dsound, NULL)))
//                throw std::runtime_error("DirectSound failed to initialize!");
//        }
//
//        if (FAILED(dsound->SetCooperativeLevel(GetDesktopWindow(), DSSCL_PRIORITY)))
//            throw std::runtime_error("DirectSound Error: Cooperative level could not be set.");
//
//        // Determine the user's speaker configuration, and select an expansion option as needed.
//        // FAIL : Directsound doesn't appear to support audio expansion >_<
//
//
//        //dsound->GetSpeakerConfig( &speakerConfig );
//
//        IDirectSoundBuffer *buffer_;
//        DSBUFFERDESC desc;
//
//        // Set up WAV format structure.
//
//        memset(&wfx, 0, sizeof(WAVEFORMATEX));
//        wfx.wFormatTag = WAVE_FORMAT_PCM;
//        wfx.nSamplesPerSec = SampleRate;
//        wfx.nChannels = (WORD)speakerConfig;
//        wfx.wBitsPerSample = 16;
//        wfx.nBlockAlign = 2 * (WORD)speakerConfig;
//        wfx.nAvgBytesPerSec = SampleRate * wfx.nBlockAlign;
//        wfx.cbSize = 0;
//
//        uint BufferSizeBytes = BufferSize * wfx.nBlockAlign;
//
//        // Set up DSBUFFERDESC structure.
//
//        memset(&desc, 0, sizeof(DSBUFFERDESC));
//        desc.dwSize = sizeof(DSBUFFERDESC);
//        desc.dwFlags = DSBCAPS_GETCURRENTPOSITION2 | DSBCAPS_CTRLPOSITIONNOTIFY | DSBCAPS_CTRLVOLUME;
//        desc.dwBufferBytes = BufferSizeBytes * m_NumBuffers;
//        desc.lpwfxFormat = &wfx;
//
//        // Try a hardware buffer first, and then fall back on a software buffer if
//        // that one fails.
//
//        desc.dwFlags |= m_UseHardware ? DSBCAPS_LOCHARDWARE : DSBCAPS_LOCSOFTWARE;
//        desc.dwFlags |= m_DisableGlobalFocus ? DSBCAPS_STICKYFOCUS : DSBCAPS_GLOBALFOCUS;
//
//        if (FAILED(dsound->CreateSoundBuffer(&desc, &buffer_, 0))) {
//            if (m_UseHardware) {
//                desc.dwFlags = DSBCAPS_GETCURRENTPOSITION2 | DSBCAPS_CTRLPOSITIONNOTIFY | DSBCAPS_LOCSOFTWARE;
//                desc.dwFlags |= m_DisableGlobalFocus ? DSBCAPS_STICKYFOCUS : DSBCAPS_GLOBALFOCUS;
//
//                if (FAILED(dsound->CreateSoundBuffer(&desc, &buffer_, 0)))
//                    throw std::runtime_error("DirectSound Error: Buffer could not be created.");
//            }
//
//            throw std::runtime_error("DirectSound Error: Buffer could not be created.");
//        }
//        if (FAILED(buffer_->QueryInterface(IID_IDirectSoundBuffer8, (void **)&buffer)) || buffer == NULL)
//            throw std::runtime_error("DirectSound Error: Interface could not be queried.");
//
//        buffer_->Release();
//        verifyc(buffer->QueryInterface(IID_IDirectSoundNotify8, (void **)&buffer_notify));
//
//        DSBPOSITIONNOTIFY lnot[MAX_BUFFER_COUNT];
//
//        for (uint i = 0; i < m_NumBuffers; i++) {
//            buffer_events[i] = CreateEvent(NULL, FALSE, FALSE, NULL);
//            lnot[i].dwOffset = (wfx.nBlockAlign + BufferSizeBytes * (i + 1)) % desc.dwBufferBytes;
//            lnot[i].hEventNotify = buffer_events[i];
//        }
//
//        buffer_notify->SetNotificationPositions(m_NumBuffers, lnot);
//
//        LPVOID p1 = 0, p2 = 0;
//        DWORD s1 = 0, s2 = 0;
//
//        verifyc(buffer->Lock(0, desc.dwBufferBytes, &p1, &s1, &p2, &s2, 0));
//        assert(p2 == 0);
//        memset(p1, 0, s1);
//        verifyc(buffer->Unlock(p1, s1, p2, s2));
//
//        //Play the buffer !
//        verifyc(buffer->Play(0, 0, DSBPLAY_LOOPING));
//
//        // Start Thread
//        myLastWrite = 0;
//        dsound_running = true;
//        thread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)RThread<StereoOut16>, this, 0, &tid);
//        SetThreadPriority(thread, THREAD_PRIORITY_ABOVE_NORMAL);
//
//		setVolume(CurrentVolume);

        return 0;
    }

    void Close()
    {

		// destroy buffer queue audio player object, and invalidate all associated interfaces
		if (bqPlayerObject != NULL) {
			(*bqPlayerObject)->Destroy(bqPlayerObject);
			bqPlayerObject = NULL;
			bqPlayerPlay = NULL;
			bqPlayerBufferQueue = NULL;
			bqPlayerEffectSend = NULL;
			bqPlayerVolume = NULL;
		}

		// destroy output mix object, and invalidate all associated interfaces
		if (outputMixObject != NULL) {
			(*outputMixObject)->Destroy(outputMixObject);
			outputMixObject = NULL;
			outputMixEnvironmentalReverb = NULL;
		}

		// destroy engine object, and invalidate all associated interfaces
		if (engineObject != NULL) {
			(*engineObject)->Destroy(engineObject);
			engineObject = NULL;
			engineEngine = NULL;
		}

//        // Stop Thread
//        fprintf(stderr, "* SPU2-X: Waiting for DSound thread to finish...");
//        dsound_running = false;
//
//        WaitForSingleObject(thread, INFINITE);
//        CloseHandle(thread);
//
//        fprintf(stderr, " Done.\n");
//
//        //
//        // Clean up
//        //
//        if (buffer != NULL) {
//            buffer->Stop();
//
//            for (u32 i = 0; i < m_NumBuffers; i++) {
//                if (buffer_events[i] != NULL)
//                    CloseHandle(buffer_events[i]);
//                buffer_events[i] = NULL;
//            }
//
//            safe_release(buffer_notify);
//            safe_release(buffer);
//        }
//
//        safe_release(dsound);
//        CoUninitialize();
    }

private:

	// create buffer queue audio player
	void createBufferQueueAudioPlayer() {

		SLuint32 speakerConfig = 2;

		SLuint32 speakers = SL_SPEAKER_FRONT_LEFT | SL_SPEAKER_FRONT_RIGHT;

		SLresult result;
		// configure audio source
		SLDataLocator_AndroidSimpleBufferQueue loc_bufq = {SL_DATALOCATOR_ANDROIDSIMPLEBUFFERQUEUE, m_NumBuffers};

		SLDataFormat_PCM format_pcm = {
			SL_DATAFORMAT_PCM,
			speakerConfig,
			SL_SAMPLINGRATE_48,
			SL_PCMSAMPLEFORMAT_FIXED_16,
			SL_PCMSAMPLEFORMAT_FIXED_16,
			speakers,
			SL_BYTEORDER_LITTLEENDIAN};

		SLDataSource audioSrc = {&loc_bufq, &format_pcm};
		// configure audio sink
		SLDataLocator_OutputMix loc_outmix = {SL_DATALOCATOR_OUTPUTMIX, outputMixObject};
		SLDataSink audioSnk = {&loc_outmix, NULL};
		// create audio player
		const SLInterfaceID ids[] = {SL_IID_ANDROIDSIMPLEBUFFERQUEUE};
		const SLboolean req[] = {SL_BOOLEAN_TRUE};
		result = (*engineEngine)->CreateAudioPlayer(engineEngine, &bqPlayerObject, &audioSrc, &audioSnk,
													1, ids, req);
		assert(SL_RESULT_SUCCESS == result);
		// realize the player
		result = (*bqPlayerObject)->Realize(bqPlayerObject, SL_BOOLEAN_FALSE);
		assert(SL_RESULT_SUCCESS == result);
		// get the play interface
		result = (*bqPlayerObject)->GetInterface(bqPlayerObject, SL_IID_PLAY, &bqPlayerPlay);
		assert(SL_RESULT_SUCCESS == result);
		// get the buffer queue interface
		result = (*bqPlayerObject)->GetInterface(bqPlayerObject, SL_IID_ANDROIDSIMPLEBUFFERQUEUE,
												 &bqPlayerBufferQueue);
		if (SL_RESULT_SUCCESS!=result) {
			LOGE(1, "cannot get buffer queue interface");
		}
		assert(SL_RESULT_SUCCESS == result);
		// register callback on the buffer queue
		result = (*bqPlayerBufferQueue)->RegisterCallback(bqPlayerBufferQueue, bqPlayerCallback, NULL);
		assert(SL_RESULT_SUCCESS == result);
		// get the effect send interface
//		result = (*bqPlayerObject)->GetInterface(bqPlayerObject, SL_IID_EFFECTSEND,
//												 &bqPlayerEffectSend);
//		assert(SL_RESULT_SUCCESS == result);
//		// get the volume interface
//		result = (*bqPlayerObject)->GetInterface(bqPlayerObject, SL_IID_VOLUME, &bqPlayerVolume);
//		assert(SL_RESULT_SUCCESS == result);
//		LOGI(2, "player created");

		// set the player's state to playing
		result = (*bqPlayerPlay)->SetPlayState(bqPlayerPlay, SL_PLAYSTATE_PLAYING);
		assert(SL_RESULT_SUCCESS == result);
		LOGI(2, "player playing");

		bqPlayerCallback(bqPlayerBufferQueue, NULL);

	}


public:
    virtual void Configure(uptr parent)
    {

    }

    s32 Test() const
    {
        return 0;
    }

    int GetEmptySampleCount()
    {
//        DWORD play, write;
//        buffer->GetCurrentPosition(&play, &write);
//
//        // Note: Dsound's write cursor is bogus.  Use our own instead:
//
//        int empty = play - myLastWrite;
//        if (empty < 0)
//            empty = -empty;
//
//        return empty / 2;

		return 0;
    }

    const wchar_t *GetIdent() const
    {
        return L"dsound";
    }

    const wchar_t *GetLongName() const
    {
        return L"DirectSound (Nice)";
    }

    void ReadSettings()
    {
//        m_Device = L"default";
//        m_NumBuffers = 5;
//        m_DisableGlobalFocus = false;
//        m_UseHardware = false;
//
//        Clampify(m_NumBuffers, (u8)3, (u8)8);
    }

    void SetApiSettings(wxString)
    {
    }

    void WriteSettings() const
    {
    }

//    void setVolume(LONG aVolume)
//    {
//        HRESULT lres = DS_OK;
//
//        CurrentVolume = aVolume;
//
//        if (buffer != NULL)
//            lres = IDirectSoundBuffer_SetVolume(buffer, aVolume);
//    }
//
} static OpenSLES;

void SetVolume(LONG aVolume)
{
//    DS.setVolume(aVolume);
}

SndOutModule *SoundOut = &OpenSLES;
