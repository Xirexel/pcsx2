/*  Framework - Framework stub for Omega Red PS2 Emulator for PCs
*
*  Framework is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Framework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Framework.
*  If not, see <http://www.gnu.org/licenses/>.
*/

#include "datetime.h"
#include <chrono>
#include <ctime>

using namespace std;
using namespace std::chrono;


// returns the time zone in the C sense, i.e. the difference UTC - local
// (in seconds)
int wxGetTimeZone()
{
#ifdef WX_GMTOFF_IN_TM
	// set to true when the timezone is set
	static bool s_timezoneSet = false;
	static long gmtoffset = LONG_MAX; // invalid timezone

	// ensure that the timezone variable is set by calling wxLocaltime_r
	if (!s_timezoneSet)
	{
		// just call wxLocaltime_r() instead of figuring out whether this
		// system supports tzset(), _tzset() or something else
		time_t t = time(NULL);
		struct tm tm;

		wxLocaltime_r(&t, &tm);
		s_timezoneSet = true;

		// note that GMT offset is the opposite of time zone and so to return
		// consistent results in both WX_GMTOFF_IN_TM and !WX_GMTOFF_IN_TM
		// cases we have to negate it
		gmtoffset = -tm.tm_gmtoff;

		// this function is supposed to return the same value whether DST is
		// enabled or not, so we need to use an additional offset if DST is on
		// as tm_gmtoff already does include it
		if (tm.tm_isdst)
			gmtoffset += 3600;
	}
	return (int)gmtoffset;
#elif defined(__DJGPP__) || defined(__WINE__)
	struct timeb tb;
	ftime(&tb);
	return tb.timezone * 60;
#elif defined(__VISUALC__)
	// We must initialize the time zone information before using it (this will
	// be done only once internally).
	_tzset();

	// Starting with VC++ 8 timezone variable is deprecated and is not even
	// available in some standard library version so use the new function for
	// accessing it instead.
//#if wxCHECK_VISUALC_VERSION(8)
	long t;
	_get_timezone(&t);
	return t;
//#else // VC++ < 8
//	return timezone;
//#endif
#else // Use some kind of time zone variable.
	// In any case we must initialize the time zone before using it.
	tzset();

#if defined(WX_TIMEZONE) // If WX_TIMEZONE was defined by configure, use it.
	return WX_TIMEZONE;
#elif defined(__BORLANDC__) || defined(__MINGW32__) || defined(__VISAGECPP__)
	return _timezone;
#else // unknown platform -- assume it has timezone
	return timezone;
#endif // different time zone variables
#endif // different ways to determine time zone
}


wxDateTime::wxDateTime()
{

}

wxDateTime::wxDateTime(const Tm& tm)
{
	m_year = tm.year;
	m_yday = tm.yday; // use C convention for day number
	m_mon = tm.mon; // algorithm yields 1 for January, not 0
	m_mday = tm.mday;
	m_sec = tm.sec;
	m_min = tm.min;
	m_hour = tm.hour;
}

wxDateTime wxDateTime::GetTimeNow()
{	
	return wxDateTime::UNow();
}

wxDateTime wxDateTime::UNow()
{
	wxDateTime l_DateTime;

	system_clock::time_point now = system_clock::now();

	auto duration = now.time_since_epoch();

	l_DateTime.m_mill = std::chrono::duration_cast<std::chrono::milliseconds>(duration).count();
	
	time_t tt = system_clock::to_time_t(now);
	tm utc_tm = *gmtime(&tt);
	tm local_tm = *localtime(&tt);
	l_DateTime.m_year = utc_tm.tm_year + 1900;
	l_DateTime.m_mon = utc_tm.tm_mon;
	l_DateTime.m_mday = utc_tm.tm_mday;
	l_DateTime.m_yday = utc_tm.tm_yday;
	l_DateTime.m_hour = utc_tm.tm_hour;
	l_DateTime.m_min = utc_tm.tm_min;
	l_DateTime.m_sec = utc_tm.tm_sec;

	return l_DateTime;
}

int wxDateTime::GetSecond()
{
	return m_sec;
}

int wxDateTime::GetMinute()
{
	return m_min;
}

int wxDateTime::GetHour(GMT a_GMT)
{
	return m_hour + a_GMT;
}

int wxDateTime::GetDay(GMT a_GMT)
{
	return m_mday;
}

int wxDateTime::GetMonth(GMT a_GMT)
{
	return m_mon;
}

int wxDateTime::GetYear(GMT a_GMT)
{
	return m_year;
}

int64_t wxDateTime::GetMillisecond()
{
	return m_mill;
}

wxTimeSpan wxDateTime::Subtract(const wxDateTime& a_datetime)
{
	return wxTimeSpan(0, 0, 0, m_mill - a_datetime.m_mill);
}

wxString wxDateTime::Format(const wxString& format)
{
	return wxString();
}

bool wxDateTime::IsValid()const
{
	return true;
}

// ----------------------------------------------------------------------------
// time_t <-> broken down time conversions
// ----------------------------------------------------------------------------

wxDateTime::Tm wxDateTime::GetTm(const GMT& a_GMT) const
{
	auto l_now = UNow();

	Tm tm;

	tm.year = (int)l_now.m_year;
	tm.yday = (wxDateTime_t)(l_now.m_yday); // use C convention for day number
	tm.mon = (Month)(l_now.m_mon); // algorithm yields 1 for January, not 0
	tm.mday = (wxDateTime_t)l_now.m_mday;
	tm.msec = (wxDateTime_t)(l_now.m_mill % 1000);
	tm.sec = (wxDateTime_t)(l_now.m_sec);

	tm.min = (wxDateTime_t)(l_now.m_min);

	tm.hour = (wxDateTime_t)(l_now.GetHour(a_GMT));

	return tm;
}

wxDateTime& wxDateTime::MakeFromTimezone(const GMT& a_GMT, bool noDST)
{
	return *this;
}

wxDateTime& wxDateTime::FromTimezone(const GMT& a_GMT, bool noDST)
{
	return MakeFromTimezone(a_GMT, noDST);
}