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
#include "GSPng.h"
#include <zlib.h>

namespace GSPng {

    bool SaveFile(const std::string& file, const Format fmt, const uint8* const image,
        uint8* const row, const int width, const int height, const int pitch,
        const int compression, const bool rb_swapped = false, const bool first_image = false)
    {
        return true;
    }

    bool Save(GSPng::Format fmt, const std::string& file, uint8* image, int w, int h, int pitch, int compression, bool rb_swapped)
    {
        return true;
    }

    //Transaction::Transaction(GSPng::Format fmt, const std::string& file, const uint8* image, int w, int h, int pitch, int compression)
    //    : m_fmt(fmt), m_file(file), m_w(w), m_h(h), m_pitch(pitch), m_compression(compression)
    //{
    //    // Note: yes it would be better to use shared pointer
    //    m_image = (uint8*)_aligned_malloc(pitch*h, 32);
    //    if (m_image)
    //        memcpy(m_image, image, pitch*h);
    //}

    //Transaction::~Transaction()
    //{
    //    if (m_image)
    //        _aligned_free(m_image);
    //}

    //void Process(std::shared_ptr<Transaction>& item)
    //{
    //    Save(item->m_fmt, item->m_file, item->m_image, item->m_w, item->m_h, item->m_pitch, item->m_compression);
    //}

}
