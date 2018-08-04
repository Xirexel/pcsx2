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

#include "file.h"


wxFile::wxFile(const wxString& filename)
{
	m_pfile = fopen(filename.c_str(), "r");
}

wxFile::~wxFile()
{
	if (m_pfile != nullptr)
		fclose(m_pfile);

	m_pfile = nullptr;
}

size_t wxFile::Read(uint8_t* data, size_t length)
{
	if (m_pfile != nullptr)
		return fread(data, 1, length, m_pfile);
	else
		return 0;
}