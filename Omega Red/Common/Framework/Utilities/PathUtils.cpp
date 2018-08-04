/*  PCSX2 - PS2 Emulator for PCs
 *  Copyright (C) 2002-2010  PCSX2 Dev Team
 *
 *  PCSX2 is free software: you can redistribute it and/or modify it under the terms
 *  of the GNU Lesser General Public License as published by the Free Software Found-
 *  ation, either version 3 of the License, or (at your option) any later version.
 *
 *  PCSX2 is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 *  PURPOSE.  See the GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along with PCSX2.
 *  If not, see <http://www.gnu.org/licenses/>.
 */

#include "Path.h"

#include <wx/file.h>

#include <fstream>

// Returns -1 if the file does not exist.
s64 Path::GetFileSize(const wxString &path)
{
	std::ifstream l_file;

	l_file.open(path.c_str());

	if (!l_file.is_open())
	{
		l_file.close();

		return -1;
	}

	l_file.seekg(0, l_file.end);
	
	auto l_fileSize = l_file.tellg();

	l_file.close();
	
	return l_fileSize;
}

// Concatenates two pathnames together, inserting delimiters (backslash on win32)
// as needed! Assumes the 'dest' is allocated to at least g_MaxPath length.
//
wxString Path::Combine(const wxString &srcPath, const wxString &srcFile)
{
	return L"";
    //return (wxDirName(srcPath) + srcFile).GetFullPath();
}

// Replaces the extension of the file with the one given.
// This function works for path names as well as file names.
wxString Path::ReplaceExtension(const wxString &src, const wxString &ext)
{
    wxFileName jojo(src);
    jojo.SetExt(ext);
    return jojo.GetFullPath();
}

wxString Path::GetFilename(const wxString &src)
{
    return wxFileName(src).GetFullName();
}

wxString Path::GetFilenameWithoutExt(const wxString &src)
{
    return wxFileName(src).GetName();
}


// ------------------------------------------------------------------------
// Launches the specified file according to its mime type
//
void pxLaunch(const wxString &filename)
{
}

void pxLaunch(const char *filename)
{
}

// ------------------------------------------------------------------------
// Launches a file explorer window on the specified path.  If the given path is not
// a qualified URI (with a prefix:// ), file:// is automatically prepended.  This
// bypasses wxWidgets internal filename checking, which can end up launching things
// through browser more often than desired.
//
void pxExplore(const wxString &path)
{
}

void pxExplore(const char *path)
{
}
