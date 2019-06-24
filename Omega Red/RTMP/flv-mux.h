/******************************************************************************
    Copyright (C) 2014 by Hugh Bailey <obs.jim@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

#ifndef __flv_mux__
#define __flv_mux__

#include "Common\pugiconfig.hpp"
#include "Common\pugixml.hpp"
#include "SimpleBinStream.h"

using namespace pugi;

#ifdef __cplusplus
extern "C"
{
#endif
	
	bool flv_meta_data(xml_node& node, simple::mem_ostream<std::true_type>& out, bool write_header, size_t audio_idx);

	void flv_packet_mux(int32_t dts_offset, int32_t sampleTime, simple::mem_ostream<std::true_type>& out, const char* buf, int size, bool is_keyframe, bool is_header, bool isVideo);
	
#ifdef __cplusplus
};
#endif

#endif