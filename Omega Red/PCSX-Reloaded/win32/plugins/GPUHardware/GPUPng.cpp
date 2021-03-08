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
#include "GPUPng.h"
#include <zlib.h>
#include <png.h>

struct {
    int type;
    int bytes_per_pixel_in;
    int bytes_per_pixel_out;
    int channel_bit_depth;
    const wchar_t *extension[2];
} static const pixel[GPUPng::Format::COUNT] = {
    {PNG_COLOR_TYPE_RGBA, 4, 4, 8 , {L".png",     nullptr}},         // RGBA_PNG
    {PNG_COLOR_TYPE_RGB, 4, 3, 8, {L".png", nullptr}},                   // RGB_PNG
    {PNG_COLOR_TYPE_RGB, 4, 3, 8, {L".png", L"_alpha.png"}},              // RGB_A_PNG
    {PNG_COLOR_TYPE_GRAY, 4, 1, 8, {L"_alpha.png", nullptr}},            // ALPHA_PNG
    {PNG_COLOR_TYPE_GRAY, 1, 1, 8, {L"_R8I.png", nullptr}},              // R8I_PNG
    {PNG_COLOR_TYPE_GRAY, 2, 2, 16, {L"_R16I.png", nullptr}},            // R16I_PNG
    {PNG_COLOR_TYPE_GRAY, 4, 2, 16, {L"_R32I_lsb.png", L"_R32I_msb.png"}}, // R32I_PNG
};

namespace GPUPng
{

    bool SaveFile(const std::wstring& file, const Format fmt, const uint8* const image,
        uint8* const row, const int width, const int height, const int pitch,
        const int compression, const bool rb_swapped = false, const bool first_image = false)
    {
        const int channel_bit_depth = pixel[fmt].channel_bit_depth;
        const int bytes_per_pixel_in = pixel[fmt].bytes_per_pixel_in;

        const int type = first_image ? pixel[fmt].type : PNG_COLOR_TYPE_GRAY;
        const int offset = first_image ? 0 : pixel[fmt].bytes_per_pixel_out;
        const int bytes_per_pixel_out = first_image ? pixel[fmt].bytes_per_pixel_out : bytes_per_pixel_in - offset;

        FILE *fp = NULL;
			
		_wfopen_s(&fp, file.c_str(), L"wb");
        if (fp == nullptr)
            return false;

        png_structp png_ptr = png_create_write_struct(PNG_LIBPNG_VER_STRING, nullptr, nullptr, nullptr);
        png_infop info_ptr = nullptr;

        bool success;
        try {
            if (png_ptr == nullptr)
                throw GSDXRecoverableError();

            info_ptr = png_create_info_struct(png_ptr);
            if (info_ptr == nullptr)
                throw GSDXRecoverableError();

            if (setjmp(png_jmpbuf(png_ptr)))
                throw GSDXRecoverableError();

            png_init_io(png_ptr, fp);
            png_set_compression_level(png_ptr, compression);
            png_set_IHDR(png_ptr, info_ptr, width, height, channel_bit_depth, type,
                PNG_INTERLACE_NONE, PNG_COMPRESSION_TYPE_DEFAULT, PNG_FILTER_TYPE_DEFAULT);
            png_write_info(png_ptr, info_ptr);

            if (channel_bit_depth > 8)
                png_set_swap(png_ptr);
            if (rb_swapped && type != PNG_COLOR_TYPE_GRAY)
                png_set_bgr(png_ptr);

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x)
                    for (int i = 0; i < bytes_per_pixel_out; ++i)
                        row[bytes_per_pixel_out * x + i] = image[y * pitch + bytes_per_pixel_in * x + i + offset];
                png_write_row(png_ptr, row);
            }
            png_write_end(png_ptr, nullptr);

            success = true;
        } catch (GSDXRecoverableError&) {
            fprintf(stderr, "Failed to write image %s\n", file.c_str());

			success = false;
        }

        if (png_ptr)
            png_destroy_write_struct(&png_ptr, info_ptr ? &info_ptr : nullptr);
        fclose(fp);

        return success;
    }

    bool Save(GPUPng::Format fmt, const std::wstring &file, uint8 *image, int w, int h, int pitch, int compression, bool rb_swapped)
    {
        std::wstring root = file;
        root.replace(file.length() - 4, 4, L"");

        ASSERT(fmt >= Format::START && fmt < Format::COUNT);

        if (compression < 0 || compression > Z_BEST_COMPRESSION)
            compression = Z_BEST_SPEED;

        std::unique_ptr<uint8[]> row(new uint8[pixel[fmt].bytes_per_pixel_out * w]);

        std::wstring filename = root + pixel[fmt].extension[0];
        if (!SaveFile(filename, fmt, image, row.get(), w, h, pitch, compression, rb_swapped, true))
            return false;

        // Second image
        if (pixel[fmt].extension[1] == nullptr)
            return true;

        filename = root + pixel[fmt].extension[1];
        return SaveFile(filename, fmt, image, row.get(), w, h, pitch, compression);
    }

    Transaction::Transaction(GPUPng::Format fmt, const std::wstring &file, const uint8 *image, int w, int h, int pitch, int compression)
        : m_fmt(fmt), m_file(file), m_w(w), m_h(h), m_pitch(pitch), m_compression(compression)
    {
        // Note: yes it would be better to use shared pointer
        m_image = (uint8*)_aligned_malloc(pitch*h, 32);
        if (m_image)
            memcpy(m_image, image, pitch*h);
    }

    Transaction::~Transaction()
    {
        if (m_image)
            _aligned_free(m_image);
    }

    void Process(std::shared_ptr<Transaction>& item)
    {
        Save(item->m_fmt, item->m_file, item->m_image, item->m_w, item->m_h, item->m_pitch, item->m_compression);
    }

	void ReadDataFromMemory(png_structp png_ptr, png_bytep outBytes,
                            png_size_t byteCountToRead);

	uint8_t *seek; // Yeah, this isn't threadsafe, so only load 1 png at a time.

    void parseRGB(uint32_t *dest, uint32_t width, uint32_t height,
                  png_structp png_ptr, png_infop info_ptr)
    {

        png_uint_32 bytesPerRow = png_get_rowbytes(png_ptr, info_ptr);
        uint8_t *rowData = (uint8_t*)malloc(bytesPerRow);

        // Read each row
        int row, col;
        for (row = 0; row < height; row++) {
            png_read_row(png_ptr, (png_bytep)rowData, NULL);

            int index = 0;
            for (col = 0; col < width; col++) {
                uint8_t red = rowData[index++];
                uint8_t green = rowData[index++];
                uint8_t blue = rowData[index++];

                //*dest = red << 16 | green << 8 | blue;
                *dest = 0xff << 24 | blue << 16 | green << 8 | red;
                //*dest = blue << 24 | green << 16 | red << 8 | 0xff;
                //*dest = red << 24 | green << 16 | blue << 8 | 0xff;
                dest++;
            }
        }

        free(rowData);
    }

    void parseRGBA(uint32_t *dest, uint32_t width, uint32_t height,
                   png_structp png_ptr, png_infop info_ptr)
    {

        png_uint_32 bytesPerRow = png_get_rowbytes(png_ptr, info_ptr);
        uint8_t *rowData = (uint8_t *)malloc(bytesPerRow);

        // Read each row
        int row, col;
        for (row = 0; row < height; row++) {
            png_read_row(png_ptr, (png_bytep)rowData, NULL);

            int index = 0;
            for (col = 0; col < width; col++) {
                uint8_t red = rowData[index++];
                uint8_t green = rowData[index++];
                uint8_t blue = rowData[index++];
                uint8_t alpha = rowData[index++];

                //*dest = alpha << 24 | red << 16 | green << 8 | blue;
                *dest = alpha << 24 | blue << 16 | green << 8 | red;
                //*dest = blue << 24 | green << 16 | red << 8 | alpha;
                //*dest = red << 24 | green << 16 | blue << 8 | alpha;
                dest++;
            }
        }
        free(rowData);
    }

	
	std::unique_ptr<uint8[]> Load(const uint8 *png_memory,
                                  uint32 &a_refwidth,
                                  uint32 &a_refheight,
                                  int &a_refbitDepth,
                                  int &a_refcolorType)
	{
        // Make sure we have a valid png here.
        assert(png_sig_cmp((png_bytep)png_memory, 0, 8) == 0);

        // get PNG file info struct
        png_structp png_ptr = NULL;
        png_ptr = png_create_read_struct(PNG_LIBPNG_VER_STRING, NULL, NULL, NULL);
        assert(png_ptr != NULL);

        // get PNG image data info struct
        png_infop info_ptr = NULL;
        info_ptr = png_create_info_struct(png_ptr);
        assert(info_ptr != NULL);

        png_set_read_fn(png_ptr, (png_bytep)png_memory, ReadDataFromMemory);

        // seek to start of png.
        seek = NULL;

        png_read_info(png_ptr, info_ptr);

        png_uint_32 width = 0;
        png_uint_32 height = 0;
        int bitDepth = 0;
        int colorType = -1;
        assert(png_get_IHDR(png_ptr, info_ptr,
                            &width,
                            &height,
                            &bitDepth,
                            &colorType,
                            NULL, NULL, NULL) == 1);

		a_refwidth = width;
        a_refheight = height;
        a_refbitDepth = bitDepth;
        a_refcolorType = colorType;

        std::unique_ptr<uint8[]> image(new uint8[4 * width * height]);

        switch (colorType) {
            case PNG_COLOR_TYPE_RGB:
                parseRGB((uint32*)image.get(), width, height, png_ptr, info_ptr);
                break;
            case PNG_COLOR_TYPE_RGBA:
                parseRGBA((uint32 *)image.get(), width, height, png_ptr, info_ptr);
                break;
            default:
                printf("Unsupported png type\n");
                abort();
        }

        png_destroy_read_struct(&png_ptr, &info_ptr, NULL);

        return image;
	}
	
    void ReadDataFromMemory(png_structp png_ptr, png_bytep outBytes,
                            png_size_t byteCountToRead)
    {
        if (seek == NULL)
            seek = (uint8_t*)png_get_io_ptr(png_ptr);

        memcpy(outBytes, seek, byteCountToRead);

        seek = seek + byteCountToRead;
    }
}
