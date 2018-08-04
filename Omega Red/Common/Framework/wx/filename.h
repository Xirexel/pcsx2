#pragma once

#include "string.h"

enum wxPathFormat
{
	wxPATH_NATIVE = 0,      // the path format for the current platform
	wxPATH_UNIX,
	wxPATH_BEOS = wxPATH_UNIX,
	wxPATH_MAC,
	wxPATH_DOS,
	wxPATH_WIN = wxPATH_DOS,
	wxPATH_OS2 = wxPATH_DOS,
	wxPATH_VMS,

	wxPATH_MAX // Not a valid value for specifying path format
};

class wxFileName
{
	wxString m_fullfilename;

	wxString m_driver;

	wxArrayString m_dirs;

	wxString m_filename;

	wxString m_ext;

	const wxPathFormat m_wxPathFormat;

	void process();

	void Assign(const wxFileName& filepath);

public:

	wxFileName(const wxPathFormat& a_wxPathFormat = wxPATH_NATIVE);

	wxFileName(const wxString& i, const wxPathFormat& a_wxPathFormat = wxPATH_NATIVE);

	wxFileName& operator=(const wxString& i);

	wxFileName& operator=(const wxFileName& filename);

	wxString GetFileName() const;

	wxString GetFullName() const;

	wxString GetName() const;

	const wxArrayString& GetDirs() const;

	// Construct full path with name and ext
	wxString GetFullPath(wxPathFormat format = wxPATH_NATIVE) const;

	void SetExt(const wxChar* pwx);

	bool IsDir() const;

	bool IsOk() const;
	
	uint GetDirCount() const;

	wxString GetExt() const;

	bool FileExists();

	static bool FileExists(const wxString& filename);

	static bool IsCaseSensitive();

	wxString GetPath()const;

	bool operator==(const wxFileName& s1) const;
};


class wxDirName
{
public:
	wxDirName();

	wxDirName(const wxString& path);

	void Mkdir();

	wxFileName Combine(const wxFileName &right) const;

	wxFileName operator+(const wxString &right) const;

	operator wxString() const;
};