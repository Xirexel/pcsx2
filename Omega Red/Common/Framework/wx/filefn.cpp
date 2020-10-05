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


#include "filefn.h"
#ifdef __ANDROID__
#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>
#include <errno.h>
#include <sys/types.h>
#include <unistd.h>
#endif		

wxString wxGetCwd()
{
	return L"";
}

int wxOpen(const wxString &path, int flags, int mode)
{
	return wxCRT_Open(path.c_str(), flags, mode);
}

int wxCRT_Open(const char *filename, int oflag, int mode)
{
    int fd = 0;
#ifdef __ANDROID__
    fd = open(filename, oflag);
#endif		
    return fd;
}
