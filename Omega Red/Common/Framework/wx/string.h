
#pragma once

#include "defs.h"
#include <regex>
#include <cstdarg>
#include <string>
#include <memory>
#include <vector>
#include <cwctype>
#include <tchar.h>

#include <fcntl.h>
#include <direct.h>
#include <io.h>
#include "longlong.h"
#include "timespan.h"

#define wxNOT_FOUND       (-1)


#define wxT(x) L ## x

#if !defined(_T) && !defined(wxNO__T)
#define _T(x) wxT(x)
#endif

/* a helper macro allowing to make another macro Unicode-friendly, see below */
#define wxAPPLY_T(x) wxT(x)

/* Unicode-friendly __FILE__, __DATE__ and __TIME__ analogs */
#ifndef __TFILE__
#define __TFILE__ wxAPPLY_T(__FILE__)
#endif

#ifndef __TDATE__
#define __TDATE__ wxAPPLY_T(__DATE__)
#endif

#ifndef __TTIME__
#define __TTIME__ wxAPPLY_T(__TIME__)
#endif

typedef wchar_t wxChar;

typedef std::shared_ptr<char> CharBuffer;

//According to STL _must_ be a -1 size_t
static const size_t npos = (size_t)-1;

class wxCharBuffer
{
	CharBuffer m_CharBuffer;

public:

	wxCharBuffer();

	wxCharBuffer(CharBuffer& a_CharBuffer);

	wxCharBuffer(const char* astr);

	CharBuffer::element_type *data();
	const CharBuffer::element_type *data() const;
	operator const CharBuffer::element_type *() const;
	CharBuffer::element_type operator[](size_t n) const;
};

class wxString
{
	std::wstring m_content;

	wxCharBuffer m_charBuffer;

	void updateCharBuffer();

public:

	static const size_t npos = (size_t)-1;

	struct Char
	{
		wxChar m_char;

		Char();

		Char(const wxChar& a_char);

		Char(const char& a_char);

		int GetValue();

		bool IsSameAs(const Char& i) const;
	};

	wxString();

	wxString(const wchar_t *pwz);

	wxString(std::wstring&& pwz);

	wxString(const std::wstring& pwz);
	
	wxString(const char *pcz);

	wxString(std::string& text);

	wxString(const wxChar& character, int intend);

	wxCharBuffer ToUTF8() const;

	std::string ToStdString() const;

	std::wstring ToStdWstring() const;	

	const wxChar *data() const;
	const wxChar *wc_str() const;
	const wxChar *wx_str() const;
	operator const wxChar *() const { return m_content.c_str(); }

	const char *To8BitData() const;

	const char *c_str() const;


	wxString& operator+=(const wxString& i);

	wxString& operator+=(const wxChar& i);

	const int Length()const;

	size_t length() const;

	Char operator[](const int& index)const;

	wxString operator()(const int& offset, const int& length)const;

	wxString substr(const int& offset, const int& length)const;

	bool ToLong(long* a_value, const int& base = 10)const;

	bool StartsWith(const wxChar* pwx)const;

	size_t find(const wxString& key);

	size_t rfind(const wxString& key);

	wxString After(const wxChar& a_char)const;

	wxString BeforeLast(const wxChar& a_char)const;

	wxString AfterLast(const wxChar& a_char)const;

	wxString BeforeFirst(const wxChar* a_char)const;
	wxString BeforeFirst(const wxChar& a_char)const;
	wxString AfterFirst(const wxChar* a_char)const;
	wxString AfterFirst(const wxChar& a_char)const;

	bool EndsWith(const wxString& suffix, wxString *rest = nullptr)const;

	int Last(const char& a_char) const;

	bool IsEmpty() const;

	bool Empty() const;

	wxString& Trim(bool state = false);

	void clear();

	wxString Lower() const;

	wxString Upper() const;
	
	wxString& wxString::MakeUpper();
	
	bool CmpNoCase(const wxString& i) const;

	bool IsSameAs(const wxString& i) const;

	int Cmp(const wxString& i) const;

	void Replace(const wxString& ptemp, const wxString& newStr);

	// extract string of length nCount starting at nFirst
	wxString Mid(size_t nFirst, size_t nCount) const;

	// take nLen chars starting at nPos
	wxString(const wxString& str, size_t nPos, size_t nLen);

	void ToULong(unsigned long* val, int base = 10) const;

	void ToULongLong(unsigned long long* val, int base = 10) const;

	

	static wxString From8BitData(const char* pcz);


	// extract nCount first (leftmost) characters
	wxString Left(size_t nCount) const;

	wxString operator+(const wxString& string) const;

	wxString& operator=(const wxChar& i);

	static wxString Format(const wchar_t* fmt...);

	void Printf(const wchar_t* fmt...);

	// ----------------------------------------------------------------------------
	// misc other operations
	// ----------------------------------------------------------------------------

	// returns true if the string matches the pattern which may contain '*' and
	// '?' metacharacters (as usual, '?' matches any character and '*' any number
	// of them)
	bool Matches(const wxString& mask) const;

	void reserve(size_t size);

	void Append(const wxChar * psz, size_t nLen);
};

extern wxString wxEmptyString;

inline wxString operator+(const wxChar *pwz, const wxString& string)
{
	return wxString(pwz) + string;
}

class wxArrayString
{
	std::vector<wxString> m_strings;


public:

	void Clear();

	void Add(const wxChar *pwz);

	void Add(const wxString&pwz);

	int GetCount() const;

	bool IsEmpty() const;

	wxString& Item(size_t nIndex);

	const wxString& Item(size_t nIndex) const;

	// same as Item()
	wxString& operator[](size_t nIndex);
	const wxString& operator[](size_t nIndex) const;
};

extern wxString JoinString(const wxArrayString &src, const wxString &separator);

inline bool operator==(const wxString& s1, const wxString& s2)
{
	return s1.IsSameAs(s2);
}

inline bool operator==(const wxString& s1, const wxChar* s2)
{
	return s1.IsSameAs(s2);
}

inline bool operator==(const wxString::Char& s1, const wxString::Char& s2)
{
	return s1.IsSameAs(s2);
}

inline bool operator!=(const wxString& s1, const wxString& s2)
{
	return !s1.IsSameAs(s2);
}

extern int wxAtoi(const wxString& str);

extern wxChar wxTolower(const wxString::Char& s2);

extern wxChar wxToupper(const wxString::Char& s2);

extern int wxVsnprintf(wxChar *str, size_t size, const wxString& format, va_list argptr);
