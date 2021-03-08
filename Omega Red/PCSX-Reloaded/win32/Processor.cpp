#undef inline

#define WINVER 0x0500
#define _WIN32_WINNT WINVER

#define INT8_MIN (-127i8 - 1)
#define INT16_MIN (-32767i16 - 1)
#define INT32_MIN (-2147483647i32 - 1)
#define INT64_MIN (-9223372036854775807i64 - 1)
#define INT8_MAX 127i8
#define INT16_MAX 32767i16
#define INT32_MAX 2147483647i32
#define INT64_MAX 9223372036854775807i64
#define UINT8_MAX 0xffui8
#define UINT16_MAX 0xffffui16
#define UINT32_MAX 0xffffffffui32
#define UINT64_MAX 0xffffffffffffffffui64

#define INTMAX_MIN INT64_MIN
#define INTMAX_MAX INT64_MAX
#define UINTMAX_MAX UINT64_MAX

#include "Processor.h"
//#include "r3000a.h"
#include <stdint.h>
#include "./plugins/Common/CommonCPP.h"
#include <cstdint>

//#include <cstdlib>

#include <thread>
#include <mutex>
#include <condition_variable>
#include <memory>

static std::unique_ptr<std::thread> s_emul_thread;


#define u32 unsigned int

typedef struct
{
    int (*Init)();
    void (*Reset)();
    void (*Execute)();      /* executes up to a break */
    void (*ExecuteBlock)(); /* executes up to a jump */
    void (*Clear)(u32 Addr, u32 Size);
    void (*Shutdown)();
} R3000Acpu;

extern "C" R3000Acpu *psxCpu;

Processor g_Processor;

static std::mutex s_pause_mutex;

static std::condition_variable s_pause_cv;

static std::condition_variable s_resume_cv;

static std::mutex s_access_mutex;


Processor::Processor()
    : m_State(INACTIVE)
    , m_is_paused(false)
	, m_is_shutdowned(false)
{
}

Processor::~Processor() {}


int Processor::Start()
{
    std::lock_guard<std::mutex> l_lock(s_access_mutex);

    int l_result = -1;

    if (m_State == INACTIVE) {
        s_emul_thread.reset(new std::thread([&]() {
            for (;;) {
                psxCpu->ExecuteBlock();

                if (m_is_paused) {
                    {
                        std::lock_guard<std::mutex> l_lock(s_pause_mutex);

                        s_pause_cv.notify_all();
                    }
                    {
                        std::unique_lock<std::mutex> l_lock(s_pause_mutex);

                        s_resume_cv.wait(l_lock);
                    }
                }

				if (m_is_shutdowned) {
                					
					std::this_thread::sleep_for(std::chrono::milliseconds(200));

					break;
				}
            }
        }));

        l_result = 0;

        m_State = STARTED;

    } else if (m_State == PAUSED)
		l_result = resumeInner();

    return l_result;
}

int Processor::Pause()
{
    std::lock_guard<std::mutex> l_lock(s_access_mutex);

    int l_result = -1;
    if (m_State == STARTED) {

        std::unique_lock<std::mutex> l_lock(s_pause_mutex);

        m_is_paused = true;

        s_pause_cv.wait(l_lock);

        l_result = 0;

        m_State = PAUSED;
    }

    return l_result;
}

int Processor::resumeInner()
{
    std::lock_guard<std::mutex> l_lock(s_pause_mutex);

    int l_result = -1;
	   
    m_is_paused = false;

	s_resume_cv.notify_all();

    l_result = 0;

    m_State = STARTED;

    return l_result;
}

int Processor::Shutdown()
{
    std::lock_guard<std::mutex> l_lock(s_access_mutex);

	m_is_shutdowned = true;

	if (m_State == PAUSED)
        resumeInner();

    int l_result = -1;

    if (s_emul_thread && s_emul_thread->joinable()) {
        s_emul_thread->join();

        l_result = 0;
    }

    m_State = INACTIVE;

	m_is_paused = false;

	m_is_shutdowned = false;

    s_emul_thread.reset(nullptr);

    return l_result;
}


extern "C" int StartEmul()
{
    return g_Processor.Start();
}

extern "C" int PauseEmul()
{
    return g_Processor.Pause();
}

extern "C" int ShutdownEmul()
{
    return g_Processor.Shutdown();
}

extern "C" int ResumeEmul()
{
    return g_Processor.Start();
}
