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

#include "librtmp\rtmp_sys.h"
#include "librtmp\rtmp.h"
#include "flv-mux.h"
#include "rtmp-helpers.h"

static inline void s_write(simple::mem_ostream<std::true_type>& out, const char* p, size_t length)
{
	out.write(p, length);
}

static inline void s_w8(simple::mem_ostream<std::true_type>& out, uint8_t u8)
{
	out.write(u8);
}

static inline void s_wb16(simple::mem_ostream<std::true_type>& out, uint16_t u16)
{
	s_w8(out, u16 >> 8);
	s_w8(out, (uint8_t)u16);
}

static inline void s_wb24(simple::mem_ostream<std::true_type>& out, uint32_t u24)
{
	s_wb16(out, (uint16_t)(u24 >> 8));
	s_w8(out, (uint8_t)u24);
}

static inline void s_wb32(simple::mem_ostream<std::true_type>& out, uint32_t u32)
{
	s_wb16(out, (uint16_t)(u32 >> 16));
	s_wb16(out, (uint16_t)u32);
}


static bool build_flv_meta_data(xml_node& node, simple::mem_ostream<std::true_type>& meta_data, size_t a_idx)
{
	char buf[4096];
	char *enc = buf;
	char *end = enc+sizeof(buf);


	auto lMediaTypes = node.select_nodes("/MediaTypes/MediaType");

	if (lMediaTypes.empty())
		return false;

	auto lVideoMediaType = node.select_node("/MediaTypes/MediaType[MediaTypeItem[@Name='MF_MT_MAJOR_TYPE']/SingleValue[@Value='MFMediaType_Video']]");

	auto lAudioMediaType = node.select_node("/MediaTypes/MediaType[MediaTypeItem[@Name='MF_MT_MAJOR_TYPE']/SingleValue[@Value='MFMediaType_Audio']]");

	int lAMFlength = 3;

	if (!lVideoMediaType.node().empty())
		lAMFlength += 5;

	if (!lAudioMediaType.node().empty())
		lAMFlength += 12;
		

	enc_str(&enc, end, "onMetaData");

	*enc++ = AMF_ECMA_ARRAY;
	enc = AMF_EncodeInt32(enc, end, lAMFlength);

	enc_num_val(&enc, end, "duration", 0.0);
	enc_num_val(&enc, end, "fileSize", 0.0);


	if (!lVideoMediaType.node().empty()) {

		auto lframe_size = lVideoMediaType.node().select_node("MediaTypeItem[@Name='MF_MT_FRAME_SIZE']");

		auto lAttr = lframe_size.node().select_node("Value.ValueParts/ValuePart[@Title='Width']/@Value").attribute();

		if (!lAttr.empty())
		{
			enc_num_val(&enc, end, "width", lAttr.as_double());
		}

		lAttr = lframe_size.node().select_node("Value.ValueParts/ValuePart[@Title='Height']/@Value").attribute();

		if (!lAttr.empty())
		{
			enc_num_val(&enc, end, "height", lAttr.as_double());
		}

		enc_str_val(&enc, end, "videocodecid", "avc1");

		lAttr = lVideoMediaType.node().select_node("MediaTypeItem[@Name='MF_MT_AVG_BITRATE']/SingleValue/@Value").attribute();

		if (!lAttr.empty())
		{
			enc_num_val(&enc, end, "videodatarate", lAttr.as_double());
		}

		lAttr = lVideoMediaType.node().select_node("MediaTypeItem[@Name='MF_MT_FRAME_RATE']/RatioValue/@Value").attribute();

		if (!lAttr.empty())
		{
			enc_num_val(&enc, end, "framerate", lAttr.as_double());
		}
	}
	
	if (!lAudioMediaType.node().empty())
	{
		enc_str_val(&enc, end, "audiocodecid", "mp4a");

		auto lAttr = lAudioMediaType.node().select_node("MediaTypeItem[@Name='MF_MT_AUDIO_AVG_BYTES_PER_SECOND']/SingleValue/@Value").attribute();

		if (!lAttr.empty())
		{
			enc_num_val(&enc, end, "audiodatarate", lAttr.as_double());
		}

		lAttr = lAudioMediaType.node().select_node("MediaTypeItem[@Name='MF_MT_AUDIO_SAMPLES_PER_SECOND']/SingleValue/@Value").attribute();

		if (!lAttr.empty())
		{
			enc_num_val(&enc, end, "audiosamplerate", lAttr.as_double());
		}

		lAttr = lAudioMediaType.node().select_node("MediaTypeItem[@Name='MF_MT_AUDIO_BITS_PER_SAMPLE']/SingleValue/@Value").attribute();

		if (!lAttr.empty())
		{
			enc_num_val(&enc, end, "audiosamplesize", lAttr.as_double());
		}

		lAttr = lAudioMediaType.node().select_node("MediaTypeItem[@Name='MF_MT_AUDIO_NUM_CHANNELS']/SingleValue/@Value").attribute();

		if (!lAttr.empty())
		{
			auto lvalue = lAttr.as_double();

			enc_num_val(&enc, end, "audiochannels", lvalue);

			enc_bool_val(&enc, end, "stereo",
				lvalue == 2);
			enc_bool_val(&enc, end, "2.1",
				lvalue == 3);
			enc_bool_val(&enc, end, "3.1",
				lvalue == 4);
			enc_bool_val(&enc, end, "4.0",
				lvalue == 4);
			enc_bool_val(&enc, end, "4.1",
				lvalue == 5);
			enc_bool_val(&enc, end, "5.1",
				lvalue == 6);
			enc_bool_val(&enc, end, "7.1",
				lvalue == 8);
		}
	}

	enc_str_val(&enc, end, "encoder", "CaptureManager");


	*enc++  = 0;
	*enc++  = 0;
	*enc++  = AMF_OBJECT_END;

	meta_data.write(buf, enc - buf);

	return true;
}

bool flv_meta_data(xml_node& node, simple::mem_ostream<std::true_type>& out,
	bool write_header, size_t audio_idx)
{
	uint32_t start_pos;

	simple::mem_ostream<std::true_type> meta_data;
	
	if (!build_flv_meta_data(node, meta_data, audio_idx)) {
	
		return false;
	}
	
	if (write_header) {
		out.write("FLV", 3);
		s_w8(out, 1);
		s_w8(out, 5);
		s_wb32(out, 9);
		s_wb32(out, 0);
	}

	start_pos = out.length();

	s_w8(out, RTMP_PACKET_TYPE_INFO);

	s_wb24(out, (uint32_t)meta_data.length());
	s_wb32(out, 0);
	s_wb24(out, 0);

	out.write(&meta_data.get_internal_vec()[0], meta_data.length());

	s_wb32(out, (uint32_t)out.length() - start_pos - 1);
	
	return true;
}

static void flv_video(simple::mem_ostream<std::true_type>& out, int32_t dts_offset,
	int32_t sampleTime,
	const char* data, int size, bool is_keyframe, bool is_header)
{
	int64_t offset = 0;// packet->pts - packet->dts;
	int32_t time_ms = sampleTime - dts_offset;// get_ms_time(packet, packet->dts) - dts_offset;

	if (!data || !size)
		return;

	s_w8(out, RTMP_PACKET_TYPE_VIDEO);
	
	s_wb24(out, (uint32_t)size + 5);
	s_wb24(out, time_ms);
	s_w8(out, (time_ms >> 24) & 0x7F);
	s_wb24(out, 0);

	/* these are the 5 extra bytes mentioned above */
	s_w8(out, is_keyframe ? 0x17 : 0x27);
	s_w8(out, is_header ? 0 : 1);
	s_wb24(out, offset);
	//s_wb24(out, get_ms_time(packet, offset));
	s_write(out, data, size);

	/* write tag size (starting byte doesn't count) */
	s_wb32(out, (uint32_t)out.length() - 1);
}

static void flv_audio(simple::mem_ostream<std::true_type>& out, int32_t dts_offset,
	int32_t sampleTime,
	const char* data, int size, bool is_keyframe, bool is_header)
{
	int32_t time_ms = sampleTime - dts_offset;// get_ms_time(packet, packet->dts) - dts_offset;

	if (!data || !size)
		return;

	s_w8(out, RTMP_PACKET_TYPE_AUDIO);
	
	s_wb24(out, (uint32_t)size + 2);
	s_wb24(out, time_ms);
	s_w8(out, (time_ms >> 24) & 0x7F);
	s_wb24(out, 0);

	/* these are the two extra bytes mentioned above */
	s_w8(out, 0xaf);
	s_w8(out, is_header ? 0 : 1);
	s_write(out, data, size);

	/* write tag size (starting byte doesn't count) */
	s_wb32(out, (uint32_t)out.length() - 1);
}

void flv_packet_mux(int32_t dts_offset, int32_t sampleTime, simple::mem_ostream<std::true_type>& out, const char* buf, int size, bool is_keyframe, bool is_header, bool isVideo)
{

	if (isVideo)
		flv_video(out, dts_offset, sampleTime, buf, size, is_keyframe, is_header);
	else
		flv_audio(out, dts_offset, sampleTime, buf, size, is_keyframe, is_header);
}