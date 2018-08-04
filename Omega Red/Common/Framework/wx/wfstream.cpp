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
#include "wfstream.h"


wxFileInputStream::wxFileInputStream(const wxString& a_file_path)
	:m_file_path(a_file_path){}


void wxFileInputStream::SeekI(const int& pos)
{
}

int wxFileInputStream::Read(void* a_data, int a_length)
{
	return 0;
}

wxFileOffset wxFileInputStream::GetLength() const
{
	return 0;
}

wxFileOffset wxFileInputStream::LastRead()
{
	return 0;
}