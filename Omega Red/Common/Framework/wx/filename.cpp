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


#include "filename.h"
#include <string>
#include <set>
#include <tuple>

#include <fstream>


static std::vector<std::string> splitpath(
	const std::string& str
	, const std::set<char> delimiters)
{
	std::vector<std::string> result;

	char const* pch = str.c_str();
	char const* start = pch;
	for (; *pch; ++pch)
	{
		if (delimiters.find(*pch) != delimiters.end())
		{
			if (start != pch)
			{
				std::string str(start, pch);
				result.push_back(str);
			}
			else
			{
				result.push_back("");
			}
			start = pch + 1;
		}
	}
	result.push_back(start);

	return result;
}

#define wxFILE_SEP_EXT        wxT('.')

static std::tuple<wxString, wxString> getSep(const wxPathFormat& a_wxPathFormat)
{
	wxString l_driver_sep;

	wxString l_dir_sep;

	switch (a_wxPathFormat)
	{

	case wxPATH_DOS:
	default:

		l_driver_sep = L":\\";

		l_dir_sep = L"\\";

		break;

	}

	return std::make_tuple(l_driver_sep, l_dir_sep);
}

wxFileName::wxFileName(const wxPathFormat& a_wxPathFormat) :m_wxPathFormat(a_wxPathFormat){}

void wxFileName::process()
{

	auto l_sep = getSep(m_wxPathFormat);

	m_ext.clear();

	m_filename.clear();

	m_dirs.Clear();

	m_driver.clear();

	m_driver = m_fullfilename.BeforeFirst(std::get<0>(l_sep));

	auto l_after_dir = m_fullfilename.AfterFirst(std::get<0>(l_sep));

	do
	{
		wxString l_tempDir = l_after_dir.BeforeFirst(std::get<1>(l_sep));

		if (l_tempDir.IsEmpty())
		{
			if (!l_after_dir.IsEmpty())
			{
				m_ext = l_after_dir.AfterLast(wxFILE_SEP_EXT);

				m_filename = l_after_dir.BeforeLast(wxFILE_SEP_EXT);
			}

			break;
		}

		m_dirs.Add(l_tempDir);

		l_after_dir = l_after_dir.AfterFirst(std::get<1>(l_sep));

	} while (true);

}

wxFileName::wxFileName(const wxString& i, const wxPathFormat& a_wxPathFormat)
	:m_fullfilename(i), m_wxPathFormat(a_wxPathFormat){
	process();
}

void wxFileName::Assign(const wxFileName &filepath)
{
	m_driver = filepath.m_driver;
	m_dirs = filepath.GetDirs();
	m_filename = filepath.GetName();
	m_ext = filepath.GetExt();
}

wxFileName& wxFileName::operator=(const wxString& i)
{
	m_fullfilename = i;

	process();

	return *this;
}

wxFileName& wxFileName::operator=(const wxFileName& filename)
{
	if (this != &filename) Assign(filename); return *this;
}

wxString wxFileName::GetName() const
{
	return m_filename;
}

wxString wxFileName::GetFileName() const
{
	return m_filename;
}

wxString wxFileName::GetFullName() const
{
	wxString l_filename = m_filename;

	if (!m_ext.IsEmpty())
	{
		l_filename += wxFILE_SEP_EXT;

		l_filename += m_ext;
	}

	return l_filename;
}

const wxArrayString& wxFileName::GetDirs() const
{
	return m_dirs;
}

// Construct full path with name and ext
wxString wxFileName::GetFullPath(wxPathFormat format) const
{
	auto l_sep = getSep(m_wxPathFormat);

	wxString l_fullpath = m_driver;

	l_fullpath += std::get<0>(l_sep);

	for (decltype(m_dirs.GetCount()) i = 0; i < m_dirs.GetCount(); i++)
	{
		l_fullpath += m_dirs[i];

		l_fullpath += std::get<1>(l_sep);
	}

	l_fullpath += GetFullName();

	return l_fullpath;
}

void wxFileName::SetExt(const wxChar* pwx)
{
	m_ext = pwx;
}

bool wxFileName::IsDir() const
{
	return m_filename.IsEmpty() && m_ext.IsEmpty();
}

bool wxFileName::IsOk() const
{
	return !m_filename.IsEmpty() || !m_ext.IsEmpty();
}

uint wxFileName::GetDirCount() const
{
	return m_dirs.GetCount();
}

bool wxFileName::FileExists()
{
	std::ifstream l_bios_stream(GetFullPath().c_str(), std::ifstream::binary);

	bool l_result = l_bios_stream.is_open();

	l_bios_stream.close();

	return l_result;
}

bool wxFileName::FileExists(const wxString& filename)
{
	std::ifstream l_bios_stream(filename.c_str(), std::ifstream::binary);

	bool l_result = l_bios_stream.is_open();

	l_bios_stream.close();

	return l_result;
}

bool wxFileName::IsCaseSensitive()
{
	return false;
}

wxString wxFileName::GetExt() const
{
	return wxString(m_ext);
}

wxString wxFileName::GetPath()const {
	return GetFullPath();
}

bool wxFileName::operator==(const wxFileName& s1) const{

	return this->m_fullfilename == s1.m_fullfilename;
}


wxDirName::wxDirName(){}

wxDirName::wxDirName(const wxString& path){
}

void wxDirName::Mkdir(){}

wxFileName wxDirName::Combine(const wxFileName &right) const{
	return right;
}

wxFileName wxDirName::operator+(const wxString &right) const { return Combine(wxFileName(right)); }

wxDirName::operator wxString() const
{
	return L"";
}