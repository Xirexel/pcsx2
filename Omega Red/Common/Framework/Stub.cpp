/*  Framework - Framework stub for Omega Red PS2 Emulator for PCs
*
*  MediaCapture is free software: you can redistribute it and/or modify it under the terms
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


#include "Stub.h"
#include "Pcsx2Types.h"
#include <wx/string.h>
#include "Utilities/StringHelpers.h"


wxString wxEmptyString;

AppStub gAppStub;

MessageOutputDebugStub gMessageOutputDebugStub;



FILE* wxFopen(const char *filename, const char *mode)
{
	return nullptr;
}

FILE* wxFopen(const wchar_t *filename, const char *mode)
{
	return nullptr;
}

MessageOutputDebugStub& wxMessageOutputDebug()
{
	return gMessageOutputDebugStub;
}

AppStub& wxGetApp()
{
	return gAppStub;
}

bool wxIsDebuggerRunning()
{
	return true;
}

wxString wxGetTranslation(const wxChar *msg_diag)
{
	return msg_diag;
}

const wchar_t * __fastcall pxGetTranslation(const wchar_t* msg_diag)
{
	return msg_diag;
}

void AppStub::Yield(bool)
{

}

const wxChar *__fastcall pxExpandMsg(const wxChar *englishContent)
{
	return wxGetTranslation(englishContent).wc_str();
}

wxString fromUTF8(const char *src)
{
	return wxString(src);
}

wxString __cdecl fromAscii(const char *src)
{
	return wxString(src);
}

wxString __cdecl JoinString(class wxArrayString const &, class wxString const &)
{
	return L"";
}

void __thiscall u128::WriteTo(class FastFormatAscii &)const
{
}

wxString __thiscall u128::ToString(void)const
{
	return L"";
}

// returns TRUE if the parse is valid, or FALSE if it's a comment.
bool pxParseAssignmentString(const wxString &src, wxString &ldest, wxString &rdest)
{
	if (src.StartsWith(L"--") || src.StartsWith(L"//") || src.StartsWith(L";"))
		return false;

	ldest = src.BeforeFirst(L'=').Trim(true).Trim(false);
	rdest = src.AfterFirst(L'=').Trim(true).Trim(false);

	return true;
}

ParsedAssignmentString::ParsedAssignmentString(const wxString &src)
{
	IsComment = pxParseAssignmentString(src, lvalue, rvalue);
}

#include "Utilities/FixedPointTypes.inl"

template struct FixedInt<100>;
template struct FixedInt<256>;