/*  PCSX2Lib - Kernel of PCSX2 PS2 Emulator for PCs
*
*  PCSX2Lib is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  PCSX2Lib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with PCSX2Lib.
*  If not, see <http://www.gnu.org/licenses/>.
*/

#pragma once

#ifdef PCSX2LIB_EXPORTS
#define PCSX2_EXPORT __declspec(dllexport)
#else
#define PCSX2_EXPORT __declspec(dllimport)
#endif

#define STDAPICALLTYPE          __stdcall