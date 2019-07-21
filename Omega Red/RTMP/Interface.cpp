
#include "librtmp\rtmp_sys.h"
#include "flv-mux.h"

#define CleanPoint 1

#define HeaderFlag  1 << 31


static uint64_t get_clockfreq(void)
{
	static BOOL have_clockfreq = FALSE;
	static LARGE_INTEGER clock_freq;

	if (!have_clockfreq) {
		QueryPerformanceFrequency(&clock_freq);
		have_clockfreq = TRUE;
	}

	return clock_freq.QuadPart;
}

extern "C" uint64_t os_gettime_ns(void)
{
	LARGE_INTEGER current_time;
	double time_val;

	QueryPerformanceCounter(&current_time);
	time_val = (double)current_time.QuadPart;
	time_val *= 1000000000.0;
	time_val /= (double)get_clockfreq();

	return (uint64_t)time_val;
}

#include "librtmp\rtmp.h"
#include <map>
#include <memory>

#define RTMP_EXPORT_(type)__declspec(dllexport) type __stdcall
#define RTMP_EXPORT RTMP_EXPORT_(void)

using namespace pugi;


struct RTMPDeleterFunctor {
	void operator()(RTMP* r) {

		if (r != nullptr)
		{
			RTMP_Close(r);

			RTMP_Free(r);
		}

		r = nullptr;
	}
};

struct Server
{
	std::unique_ptr<RTMP, RTMPDeleterFunctor> m_RTMP;

	std::string m_URL;
};

std::map<int, std::unique_ptr<Server>> g_Servers;


static inline void set_rtmp_str(AVal *val, const char *str)
{
	bool valid = (str && *str);
	val->av_val = valid ? (char*)str : NULL;
	val->av_len = valid ? (int)strlen(str) : 0;
}

static bool send_meta_data(xml_node& node, RTMP *r, size_t idx)
{
	bool    success = true;

	simple::mem_ostream<std::true_type> out;

	success = flv_meta_data(node, out, false, idx);

	if (success) {
		success = RTMP_Write(r, (char*)out.get_internal_vec().data(),
			(int)out.length(), (int)idx) >= 0;
	}

	return success;
}

RTMP_EXPORT_(int)
Connect(const char* a_streamsXml, const char* a_url)
{
	int result = -1;

	do
	{
		auto l_Server = std::make_unique<Server>();

		l_Server->m_RTMP.reset(RTMP_Alloc());

		if (l_Server->m_RTMP.get() == nullptr)
			break;

		l_Server->m_URL = a_url;

		RTMP_Init(l_Server->m_RTMP.get());

		if (!RTMP_SetupURL(l_Server->m_RTMP.get(), (char*)l_Server->m_URL.c_str()))
			break;

		RTMP_EnableWrite(l_Server->m_RTMP.get());			

		set_rtmp_str(&l_Server->m_RTMP->Link.flashVer, "FMLE/3.0 (compatible; FMSc/1.0)");

		l_Server->m_RTMP->Link.swfUrl = l_Server->m_RTMP->Link.tcUrl;


		xml_document lStreamsXmlDoc;

		auto l_XMLRes = lStreamsXmlDoc.load_string(a_streamsXml);

		if (l_XMLRes.status != xml_parse_status::status_ok)
			break;

		auto l_document = lStreamsXmlDoc.document_element();

		if (l_document.empty())
			break;

		RTMP_AddStream(l_Server->m_RTMP.get(), l_document.attribute("StreamName").as_string());			

		l_Server->m_RTMP->m_outChunkSize = 4096;
		l_Server->m_RTMP->m_bSendChunkSizeInfo = true;
		l_Server->m_RTMP->m_bUseNagle = true;

		if (!RTMP_Connect(l_Server->m_RTMP.get(), NULL))
			break;

		if (!RTMP_ConnectStream(l_Server->m_RTMP.get(), 0))
			break;
		

		size_t idx = 0;

		if (!send_meta_data(l_document, l_Server->m_RTMP.get(), 0)) {
			break;
		}



		int handler = -1;

		auto l_find = g_Servers.find(handler);

		do
		{
			handler = std::rand();

			l_find = g_Servers.find(handler);

		} while (l_find != g_Servers.end());

		g_Servers[handler] = std::unique_ptr<Server>(l_Server.release());
				
		result = handler;

	} while (false);

	return result;
}

RTMP_EXPORT
Disconnect(int handler)
{
	auto l_find = g_Servers.find(handler);

	if (l_find != g_Servers.end())
	{
		g_Servers.erase(l_find);
	}
}



static int send_packet(RTMP* r, int32_t sampleTime, const char* buf, int size, bool is_keyframe, bool is_header, size_t idx, bool isVideo)
{
	int     ret = 0;

	simple::mem_ostream<std::true_type> out;

	flv_packet_mux(
		0,
		is_header ? 0 : sampleTime,
		out, buf, size, is_keyframe, is_header, isVideo);
	
	ret = RTMP_Write(
		r, 
		(char*)out.get_internal_vec().data(),
		(int)out.length(), 
		(int)idx
		);
	
	return ret;
}

RTMP_EXPORT_(int)
Write(int handler, int32_t sampleTime, const char* buf, int size, int Flags, int streamIdx, int isVideo)
{
	auto l_find = g_Servers.find(handler);

	if (l_find != g_Servers.end())
	{
		if (Flags & HeaderFlag)
		{
			return send_packet((*l_find).second->m_RTMP.get(), sampleTime, buf, size, true, true, streamIdx, isVideo != 0);
		}
		else
		{
			return send_packet((*l_find).second->m_RTMP.get(), sampleTime, buf, size, (Flags & CleanPoint) != FALSE, false, streamIdx, isVideo != 0);
		}
	}

	return -1;
}

RTMP_EXPORT_(int)
IsConnected(int handler)
{
    auto l_find = g_Servers.find(handler);

    if (l_find != g_Servers.end()) {
        return RTMP_IsConnected((*l_find).second->m_RTMP.get());
    }

    return FALSE;
}