/*
 *	Copyright (C) 2015-2015 Gregory hainaut
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA USA.
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#include "stdafx.h"

#include "GSOsdManager.h"

FT_Error FT_New_Face(FT_Library   library,
	const char*  filepathname,
	FT_Long      face_index,
	FT_Face     *aface)
{
    return 1;
}

FT_Error FT_Set_Pixel_Sizes( FT_Face  face,
                      FT_UInt  pixel_width,
                            FT_UInt pixel_height)
{
    return 1;
}

FT_Error FT_Init_FreeType(FT_Library *alibrary)
{
    return 1;
}

FT_Error FT_Done_FreeType(FT_Library library)
{
    return 1;
}

FT_Error FT_Load_Char( FT_Face   face,
                FT_ULong  char_code,
                      FT_Int32 load_flags)
{
    return 1;
}

FT_Error FT_Get_Kerning( FT_Face     face,
                  FT_UInt     left_glyph,
                  FT_UInt     right_glyph,
                  FT_UInt     kern_mode,
                        FT_Vector *akerning)
{
    return 1;
}

FT_UInt FT_Get_Char_Index( FT_Face   face,
                          FT_ULong charcode)
{
    return 0;
}