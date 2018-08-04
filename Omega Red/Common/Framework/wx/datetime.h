
#pragma once

#include "ffile.h"
#include "filefn.h"

typedef unsigned short wxDateTime_t;


class wxDateTime
{
public:

	enum GMT
	{
		GMT0 = 0,
		GMT1 = 1,
		GMT2 = 2,
		GMT3 = 3,
		GMT4 = 4,
		GMT5 = 5,
		GMT6 = 6,
		GMT7 = 7,
		GMT8 = 8,
		GMT9 = 9,
	};

	enum Month
	{
		Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec, Inv_Month
	};

	struct Tm
	{
		wxDateTime_t msec, sec, min, hour,
			mday,  // Day of the month in 1..31 range.
			yday;  // Day of the year in 0..365 range.
		Month mon;
		int year;
	};

private:

	int m_sec = 0;
	int m_min = 0;
	int m_hour = 0;
	int m_mday = 0;
	int m_mon = 0;
	int m_year = 0;
	int m_yday = 0;

	s64 m_mill = 0;

	wxDateTime& MakeFromTimezone(const GMT& a_GMT, bool noDST);	

public:
	
	wxDateTime();

	wxDateTime(const Tm& tm);

	static wxDateTime GetTimeNow();

	static wxDateTime UNow();

	int GetSecond();

	int GetMinute();

	int GetHour(GMT a_GMT = GMT0);

	int GetDay(GMT a_GMT = GMT0);

	int GetMonth(GMT a_GMT = GMT0);

	int GetYear(GMT a_GMT = GMT0);

	int64_t GetMillisecond();

	wxTimeSpan Subtract(const wxDateTime& a_datetime);

	wxString Format(const wxString& format);

	bool IsValid() const;

	wxDateTime::Tm GetTm(const GMT& a_GMT) const;

	wxDateTime& FromTimezone(const GMT& a_GMT, bool noDST = false);

	inline time_t wxDateTime::GetTicks() const
	{
		return m_mill;
	}

	inline wxDateTime operator+(const wxTimeSpan& ts) const
	{
		wxDateTime dt(*this);
		dt.Add(ts);
		return dt;
	}

	inline wxDateTime& wxDateTime::Add(const wxTimeSpan& diff)
	{
		m_mill += diff.GetMilliseconds();

		return *this;
	}
};