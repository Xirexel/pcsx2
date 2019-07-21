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


#include "string.h"
#include <cctype>
#include <clocale>
#include <stdio.h>
#include <wchar.h>

wxString::wxString(){}

wxString::wxString(const wchar_t *pwz){

		 try
		 {
			 m_content = pwz;
		 }
		 catch (...)
		 {
			 m_content = L"Unknown Error";
		 }

	updateCharBuffer();
}

wxString::wxString(std::wstring&& pwz)
	: m_content(pwz) {
	updateCharBuffer();
}

wxString::wxString(const std::wstring& pwz)
	: m_content(pwz) {
	updateCharBuffer();
}

wxString::wxString(const char *pcz) {

	size_t l_textlength = strlen(pcz);

	std::shared_ptr<wchar_t> l_CharBuffer(new wchar_t[l_textlength + 1], std::default_delete<wchar_t[]>());

	for (size_t i = 0; i < l_textlength; i++)
	{
		l_CharBuffer.get()[i] = pcz[i];
	}

	l_CharBuffer.get()[l_textlength] = '\0';

	m_content = std::wstring(l_CharBuffer.get());

	updateCharBuffer();
}

wxString::wxString(std::string& text) {

	size_t l_textlength = text.size();

	std::shared_ptr<wchar_t> l_CharBuffer(new wchar_t[l_textlength + 1], std::default_delete<wchar_t[]>());

	for (size_t i = 0; i < l_textlength; i++)
	{
		l_CharBuffer.get()[i] = text[i];
	}

	l_CharBuffer.get()[l_textlength] = '\0';

	m_content = std::wstring(l_CharBuffer.get());

	updateCharBuffer();
}

wxString::wxString(const wxChar& character, int intend) {

	for (int i = 0; i < intend; i++)
	{
		m_content += character;
	}

	updateCharBuffer();
}

// take nLen chars starting at nPos
wxString::wxString(const wxString& str, size_t nPos, size_t nLen)
{
	m_content = std::wstring(str.m_content, nPos, nLen);

	updateCharBuffer();
}

wxCharBuffer wxString::ToUTF8() const
{
	CharBuffer l_CharBuffer(new char[m_content.length() + 1], std::default_delete<char[]>());

	for (size_t i = 0; i < m_content.length(); i++)
	{
		l_CharBuffer.get()[i] = (char)m_content.c_str()[i];
	}

	l_CharBuffer.get()[m_content.length()] = '\0';

	return l_CharBuffer;
}

std::string wxString::ToStdString() const
{
	return std::string(ToUTF8());
};

std::wstring wxString::ToStdWstring() const
{
	return m_content;
}

const wxChar *wxString::data() const { return  m_content.c_str(); }
const wxChar *wxString::wc_str() const { return  m_content.c_str(); }
const wxChar *wxString::wx_str() const { return  m_content.c_str(); }

const char *wxString::To8BitData() const
{
	return m_charBuffer;
};

const char *wxString::c_str() const
{
	return m_charBuffer;
};


wxString& wxString::operator+=(const wxString& i)
{
	m_content += i;

	updateCharBuffer();

	return *this;
}

wxString& wxString::operator+=(const wxChar& i)
{
	m_content += i;

	updateCharBuffer();

	return *this;
}

const int wxString::Length()const{
	return m_content.length();
}

size_t wxString::length() const{
	return m_content.length();
}

wxString::Char wxString::operator[](const int& index)const
{
	return m_content.c_str()[index];
}

wxString wxString::operator()(const int& offset, const int& length)const
{
	wxString lresult = m_content.substr(offset, length);

	return lresult;
}

wxString wxString::substr(const int& offset, const int& length)const
{
	wxString lresult = m_content.substr(offset, length);

	return lresult;
}

void wxString::Append(const wxChar * psz, size_t nLen)
{
	m_content += std::wstring(psz);
}

bool wxString::ToLong(long* a_value, const int& base)const
{
	bool l_result = false;

	try
	{
		*a_value = std::stol(m_content, nullptr, base);

		l_result = true;
	}
	catch (...)
	{

	}

	return l_result;
}

bool wxString::StartsWith(const wxChar* pwx)const
{
	auto lpos = m_content.find(pwx);

	return lpos == 0;
}

void wxString::updateCharBuffer()
{
	m_charBuffer = ToUTF8();
}

size_t wxString::find(const wxString& key)
{
	return m_content.find(key);
}

size_t wxString::rfind(const wxString& key)
{
	return m_content.rfind(key);
}

wxString wxString::After(const wxChar& a_char)const
{
	wxString l_result;

	auto lpos = m_content.find(a_char);

	if (lpos != std::wstring::npos)
	{
		l_result = m_content.substr(++lpos, m_content.size());
	}

	return l_result;
}

wxString wxString::BeforeLast(const wxChar& a_char)const
{
	wxString l_result;

	auto lpos = m_content.find_last_of(a_char);

	if (lpos != std::wstring::npos)
	{
		l_result = m_content.substr(0, lpos);
	}

	return l_result;
}

wxString wxString::AfterLast(const wxChar& a_char)const
{
	wxString l_result;

	auto lpos = m_content.find_last_of(a_char);

	if (lpos != std::wstring::npos)
	{
		l_result = m_content.substr(++lpos, m_content.size());
	}

	return l_result;
}

wxString wxString::BeforeFirst(const wxChar* a_char)const
{
	wxString l_result;

	auto lpos = m_content.find(a_char);

	if (lpos != std::wstring::npos)
	{
		l_result = m_content.substr(0, lpos);
	}

	return l_result;
}
wxString wxString::BeforeFirst(const wxChar& a_char)const
{
	wxString l_result = m_content;

	auto lpos = m_content.find(a_char);

	if (lpos != std::wstring::npos)
	{
		l_result = m_content.substr(0, lpos);
	}

	return l_result;
}
wxString wxString::AfterFirst(const wxChar* a_char)const
{
	wxString l_result;

	auto lpos = m_content.find(a_char);

	if (lpos != std::wstring::npos)
	{
		l_result = m_content.substr(lpos + std::wstring(a_char).size(), m_content.size());
	}
	else
	{
		l_result = wxString(m_content.c_str());
	}

	return l_result;
}
wxString wxString::AfterFirst(const wxChar& a_char)const
{
	wxString l_result;

	auto lpos = m_content.find(a_char);

	if (lpos != std::wstring::npos)
	{
		l_result = m_content.substr(++lpos, m_content.size());
	}

	return l_result;
}

bool wxString::EndsWith(const wxString& suffix, wxString *rest)const
{
	int start = length() - suffix.length();

	if (start < 0 || m_content.compare(start, npos, suffix) != 0)
		return false;

	if (rest)
	{
		// put the rest of the string into provided pointer
		//rest->assign(*this, 0, start);
	}

	return true;
}

int wxString::Last(const char& a_char) const
{
	return m_content.find_last_of(a_char);
}

bool wxString::IsEmpty() const{
	return m_content.empty();
}

bool wxString::Empty() const{
	return m_content.empty();
}

bool wxString::empty() const{
	return m_content.empty();
}

wxString& wxString::Trim(bool state)
{
	bool lspaceState = false;

	wxChar lspaceChar = L' ';

	if (state)
	{
		auto lpos = m_content.begin();

		auto lend = m_content.end();

		int loffset = 0;

		for (; lpos != lend; ++lpos)
		{
			if (*lpos != lspaceChar)
			{
				break;
			}

			++loffset;
		}

		m_content = m_content.substr(loffset, m_content.size());
	}
	else
	{
		auto lpos = m_content.rbegin();

		auto lend = m_content.rend();

		int loffset = 0;

		for (; lpos != lend; ++lpos)
		{
			if (*lpos != lspaceChar)
			{
				break;
			}

			++loffset;
		}

		m_content = m_content.substr(0, m_content.size() - loffset);
	}

	updateCharBuffer();

	return *this;
}

void wxString::clear()
{
	m_content.clear();
}

wxString wxString::Lower() const
{
	return wxString(*this);
}

wxString wxString::Upper() const
{
	return wxString(*this);
}

wxString& wxString::MakeUpper()
{
	for (auto it = m_content.begin(), en = m_content.end(); it != en; ++it)
		*it = (wxChar)std::towupper(*it);

	return *this;
}

bool wxString::CmpNoCase(const wxString& i) const
{
	return i.m_content == m_content;
}

bool wxString::IsSameAs(const wxString& i) const
{
	return i.m_content == m_content;
}

int wxString::Cmp(const wxString& i) const
{
	return m_content.compare(i.m_content);
}

void wxString::Replace(const wxString& ptemp, const wxString& newStr)
{
	size_t index = m_content.find(ptemp.m_content);

	if (index != std::string::npos)
		m_content.replace(index, ptemp.m_content.length(), newStr);

	updateCharBuffer();
}

// extract string of length nCount starting at nFirst
wxString wxString::Mid(size_t nFirst, size_t nCount) const
{
	size_t nLen = length();

	// default value of nCount is npos and means "till the end"
	if (nCount == npos)
	{
		nCount = nLen - nFirst;
	}

	// out-of-bounds requests return sensible things
	if (nFirst > nLen)
	{
		// AllocCopy() will return empty string
		return wxString();
	}

	if (nCount > nLen - nFirst)
	{
		nCount = nLen - nFirst;
	}

	wxString dest(*this, nFirst, nCount);
	if (dest.length() != nCount)
	{
		//wxFAIL_MSG(wxT("out of memory in Mid"));
	}

	return dest;
}

void wxString::ToULong(unsigned long* val, int base) const
{
	*val = wcstoul(m_content.c_str(), nullptr, base);
}

void wxString::ToULongLong(unsigned long long* val, int base) const
{
	*val = wcstoull(m_content.c_str(), nullptr, base);
}

wxString wxString::From8BitData(const char* pcz)
{
	return wxString(pcz);
}


// extract nCount first (leftmost) characters
wxString wxString::Left(size_t nCount) const
{
	if (nCount > length())
		nCount = length();

	wxString dest(*this, 0, nCount);
	if (dest.length() != nCount) {
		//wxFAIL_MSG(wxT("out of memory in Left"));
	}
	return dest;
}

wxString wxString::operator+(const wxString& string) const
{
	std::wstring ltemp = m_content + string.m_content;

	return wxString(std::move(ltemp));
}

wxString& wxString::operator=(const wxChar& i)
{
	m_content = i;

	updateCharBuffer();

	return *this;
}

wxString wxString::Format(const wchar_t* fmt...)
{
	return wxString();
}

void wxString::Printf(const wchar_t* fmt...)
{
}

// ----------------------------------------------------------------------------
// misc other operations
// ----------------------------------------------------------------------------

// returns true if the string matches the pattern which may contain '*' and
// '?' metacharacters (as usual, '?' matches any character and '*' any number
// of them)
bool wxString::Matches(const wxString& mask) const
{
	wxString pattern;
	
	auto pszMask = mask.wc_str();

	pattern += wxT('^');
	while (*pszMask)
	{
		switch (*pszMask)
		{
		case wxT('?'):
			pattern += wxT('.');
			break;

		case wxT('*'):
			pattern += wxT(".*");
			break;

		case wxT('^'):
		case wxT('.'):
		case wxT('$'):
		case wxT('('):
		case wxT(')'):
		case wxT('|'):
		case wxT('+'):
		case wxT('\\'):
			// these characters are special in a RE, quote them
			// (however note that we don't quote '[' and ']' to allow
			// using them for Unix shell like matching)
			pattern += wxT('\\');
			// fall through

		default:
			pattern += *pszMask;
		}

		pszMask++;
	}
	pattern += wxT('$');
	
	std::regex re(pattern.c_str());

	std::cmatch l_match;

	auto l = std::regex_match(c_str(), l_match, re) && l_match.size();

	return l;
}

void wxString::reserve(size_t size)
{
}

int wxAtoi(const wxString& str) { 
	return atoi(str.c_str());
}

wxChar wxTolower(const wxString::Char& s2) { 
	return std::tolower(static_cast<wxChar>(s2.m_char));
}

wxChar wxToupper(const wxString::Char& s2) {
	return std::toupper(static_cast<wxChar>(s2.m_char));
}

int wxVsnprintf(wxChar *str, size_t size, const wxString& format, va_list argptr)
{
	int rv;

	rv = vswprintf(str, size, format.wc_str(), argptr);

	// VsnprintfTestCase reveals that glibc's implementation of vswprintf
	// doesn't nul terminate on truncation.
	str[size - 1] = 0;

	return rv;
}

// wxArrayString

void wxArrayString::Clear()
{
	m_strings.clear();
}

void wxArrayString::Add(const wxChar *pwz)
{
	m_strings.push_back(pwz);
}

void wxArrayString::Add(const wxString&pwz)
{
	m_strings.push_back(pwz);
}

int wxArrayString::GetCount() const
{
	return m_strings.size();
}

int wxArrayString::Count() const
{
    return m_strings.size();
}

bool wxArrayString::IsEmpty() const { return m_strings.empty(); }

wxString& wxArrayString::Item(size_t nIndex) { return m_strings[nIndex]; }

const wxString& wxArrayString::Item(size_t nIndex) const { return m_strings[nIndex]; }

// same as Item()
wxString& wxArrayString::operator[](size_t nIndex) { return Item(nIndex); }
const wxString& wxArrayString::operator[](size_t nIndex) const { return Item(nIndex); }



// wxString::Char

wxString::Char::Char()
{

}

wxString::Char::Char(const wxChar& a_char)
{
	m_char = a_char;
}

wxString::Char::Char(const char& a_char)
{
	m_char = (wxChar)a_char;
}

int wxString::Char::GetValue()
{
	return m_char;
}

bool wxString::Char::IsSameAs(const Char& i) const
{
	return m_char == i.m_char;
}


// wxCharBuffer

wxCharBuffer::wxCharBuffer(){}

wxCharBuffer::wxCharBuffer(CharBuffer& a_CharBuffer)
	: m_CharBuffer(a_CharBuffer) {}

wxCharBuffer::wxCharBuffer(const char* astr){

	size_t l_textlength = strlen(astr);

	CharBuffer l_CharBuffer(new char[l_textlength + 1], std::default_delete<char[]>());

	for (size_t i = 0; i < l_textlength; i++)
	{
		l_CharBuffer.get()[i] = astr[i];
	}

	l_CharBuffer.get()[l_textlength] = '\0';

	m_CharBuffer = l_CharBuffer;
}

CharBuffer::element_type *wxCharBuffer::data() { return m_CharBuffer.get(); }
const CharBuffer::element_type *wxCharBuffer::data() const { return  m_CharBuffer.get(); }
wxCharBuffer::operator const CharBuffer::element_type *() const { return m_CharBuffer.get(); }
CharBuffer::element_type wxCharBuffer::operator[](size_t n) const { return data()[n]; }


// Splits a string into parts and adds the parts into the given SafeList.
// This list is not cleared, so concatenating many splits into a single large list is
// the 'default' behavior, unless you manually clear the SafeList prior to subsequent calls.
//
// Note: wxWidgets 2.9 / 3.0 has a wxSplit function, but we're using 2.8 so I had to make
// my own.
void SplitString(wxArrayString &dest, const wxString &src, const wxString &delims, wxStringTokenizerMode mode)
{
    wxStringTokenizer parts(src, delims, mode);
    while (parts.HasMoreTokens())
        dest.Add(parts.GetNextToken());
}
