
#pragma once

class wxTimeSpan
{
	long m_hours = 0;
	long m_min = 0;
	long m_sec = 0;
	int64_t m_msec = 0;

public:
	wxTimeSpan(long hours, long min, long sec, int64_t msec)
		:m_hours(hours),
		m_min(min),
		m_sec(sec),
		m_msec(msec)
	{}
	wxTimeSpan(int64_t msec)
		:m_hours(0),
		m_min(0),
		m_sec(0),
		m_msec(msec)
	{}

	long getSeconds() const
	{
		return m_sec + (long)(m_msec / 1000);
	}

	int64_t GetMilliseconds() const
	{
		return (m_sec * 1000) + m_msec;
	}

	inline bool operator<(const wxTimeSpan &ts) const
	{
		return GetMilliseconds() < ts.GetMilliseconds();
	}


	// subtract another timespan
	wxTimeSpan& operator-=(const wxTimeSpan& diff) { return Subtract(diff); }
	
	inline wxTimeSpan Subtract(const wxTimeSpan& diff) const
	{
		return wxTimeSpan(GetMilliseconds() - diff.GetMilliseconds());
	}

	inline wxTimeSpan& Subtract(const wxTimeSpan& diff)
	{
		m_msec = GetMilliseconds() - diff.GetMilliseconds();

		m_hours = 0;

		m_min = 0;

		m_sec = 0;

		return *this;
	}
};