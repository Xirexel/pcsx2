//
// Created by Evgeny Pereguda on 8/29/2019.
//

#define SIMD_FLOAT_SIZE 16

#if (defined(__ANDROID__) && !defined(ANDROID_ABI_X86))

#include <stdio.h>
#include <string.h>

#include "simd_intrin.h"


MM_GETCSR _mm_getcsr = nullptr;

MM_SETCSR _mm_setcsr = nullptr;

MM_SETZERO_SI128 _mm_setzero_si128 = nullptr;

MM_LOAD_SI128 _mm_load_si128 = nullptr;

MM_UNPACKLO_EPI8 _mm_unpacklo_epi8 = nullptr;

MM_SETZERO_PS _mm_setzero_ps = nullptr;

MM_STORE_PS _mm_store_ps = nullptr;

MM_SET_PS1 _mm_set_ps1 = nullptr;

MM_LOAD_PS _mm_load_ps = nullptr;

MM_STORE_SI128 _mm_store_si128 = nullptr;

MM_UNPACKHI_EPI8 _mm_unpackhi_epi8 = nullptr;

MM_SET1_EPI8 _mm_set1_epi8 = nullptr;

MM_SET1_EPI16 _mm_set1_epi16 = nullptr;

MM_LOADL_EPI64 _mm_loadl_epi64 = nullptr;

MM_XOR_SI128 _mm_xor_si128 = nullptr;

MM_MULHI_EPI16 _mm_mulhi_epi16 = nullptr;

MM_ADDS_EPI16 _mm_adds_epi16 = nullptr;

MM_SUBS_EPU8 _mm_subs_epu8 = nullptr;

MM_SLLI_EPI16 _mm_slli_epi16 = nullptr;

MM_ADD_SI128 _mm_and_si128 = nullptr;

MM_MULHI_EPU16 _mm_mulhi_epu16 = nullptr;

MM_ADD_EPI16 _mm_add_epi16 = nullptr;

MM_SRAI_EPI16 _mm_srai_epi16 = nullptr;

MM_PACKUS_EPI16 _mm_packus_epi16 = nullptr;

MM_UNPACKLO_EPI16 _mm_unpacklo_epi16 = nullptr;

MM_UNPACKHI_EPI16 _mm_unpackhi_epi16 = nullptr;

MM_SHUFFLE_EPI32 _mm_shuffle_epi32 = nullptr;

MM_LOADH_PI _mm_loadh_pi = nullptr;

MM_SETR_EPI32 _mm_setr_epi32 = nullptr;

MM_ADD_EPU8 _mm_adds_epu8 = nullptr;

MM_SRLI_EPI16 _mm_srli_epi16 = nullptr;

MM_CMPEQ_EPI16 _mm_cmpeq_epi16 = nullptr;

MM_OR_SI128 _mm_or_si128 = nullptr;




unsigned int soft_mm_getcsr (void){return 0;}

void soft_mm_setcsr (unsigned int a){}



__m128i soft_mm_setzero_si128 (){throw "Unimplemented!!!";}

__m128i soft_mm_load_si128 (__m128i const* mem_addr){throw "Unimplemented!!!";}

__m128i soft_mm_unpacklo_epi8 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128 soft_mm_setzero_ps (void){

	__m128 dest;

	memset(&dest, 0, SIMD_FLOAT_SIZE);

	return dest;}

void soft_mm_store_ps (float* mem_addr, __m128 a){

	memcpy(mem_addr, &a, SIMD_FLOAT_SIZE);
}

__m128 soft_mm_set_ps1 (float a){throw "Unimplemented!!!";}

__m128 soft_mm_load_ps (float const* mem_addr){

	__m128 l_dest;

	memcpy(&l_dest, mem_addr, SIMD_FLOAT_SIZE);

	return l_dest;
}

void soft_mm_store_si128 (__m128i* mem_addr, __m128i a){throw "Unimplemented!!!";}

__m128i soft_mm_unpackhi_epi8 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_set1_epi8 (char a){throw "Unimplemented!!!";}

__m128i soft_mm_set1_epi16 (short a){throw "Unimplemented!!!";}

__m128i soft_mm_loadl_epi64 (__m128i const* mem_addr){throw "Unimplemented!!!";}

__m128i soft_mm_xor_si128 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_mulhi_epi16 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_adds_epi16 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_subs_epu8 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_slli_epi16 (__m128i a, int imm8){throw "Unimplemented!!!";}

__m128i soft_mm_and_si128 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_mulhi_epu16 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_add_epi16 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_srai_epi16 (__m128i a, int imm8){throw "Unimplemented!!!";}

__m128i soft_mm_packus_epi16 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_unpacklo_epi16 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_unpackhi_epi16 (__m128i a, __m128i b){throw "Unimplemented!!!";}

__m128i soft_mm_shuffle_epi32 (__m128i a, int imm8){throw "Unimplemented!!!";}

__m128 soft_mm_loadh_pi (__m128 a, __m64 const* mem_addr){throw "Unimplemented!!!";}

__m128i soft_mm_setr_epi32(int e3, int e2, int e1, int e0) {throw "Unimplemented!!!";}

__m128i soft_mm_adds_epu8(__m128i a, __m128i b) {throw "Unimplemented!!!";}

__m128i soft_mm_srli_epi16(__m128i a, int imm8) {throw "Unimplemented!!!";}

__m128i soft_mm_cmpeq_epi16(__m128i a, __m128i b) {throw "Unimplemented!!!";}

__m128i soft_mm_or_si128(__m128i a, __m128i b) {throw "Unimplemented!!!";}






void bind_csr_soft()
{
	_mm_getcsr = soft_mm_getcsr;

	_mm_setcsr = soft_mm_setcsr;
}

void bind_simd_soft()
{
	bind_csr_soft();



	_mm_setzero_si128 = soft_mm_setzero_si128;

	_mm_load_si128 = soft_mm_load_si128;

	_mm_unpacklo_epi8 = soft_mm_unpacklo_epi8;

	_mm_setzero_ps = soft_mm_setzero_ps;

	_mm_store_ps = soft_mm_store_ps;

	_mm_set_ps1 = soft_mm_set_ps1;

	_mm_load_ps = soft_mm_load_ps;

	_mm_store_si128 = soft_mm_store_si128;

	_mm_unpackhi_epi8 = soft_mm_unpackhi_epi8;

	_mm_set1_epi8 = soft_mm_set1_epi8;

	_mm_set1_epi16 = soft_mm_set1_epi16;

	_mm_loadl_epi64 = soft_mm_loadl_epi64;

	_mm_xor_si128 = soft_mm_xor_si128;

	_mm_mulhi_epi16 = soft_mm_mulhi_epi16;

	_mm_adds_epi16 = soft_mm_adds_epi16;

	_mm_subs_epu8 = soft_mm_subs_epu8;

	_mm_slli_epi16 = soft_mm_slli_epi16;

	_mm_and_si128 = soft_mm_and_si128;

	_mm_mulhi_epu16 = soft_mm_mulhi_epu16;

	_mm_add_epi16 = soft_mm_add_epi16;

	_mm_srai_epi16 = soft_mm_srai_epi16;

	_mm_packus_epi16 = soft_mm_packus_epi16;

	_mm_unpacklo_epi16 = soft_mm_unpacklo_epi16;

	_mm_unpackhi_epi16 = soft_mm_unpackhi_epi16;

	_mm_shuffle_epi32 = soft_mm_shuffle_epi32;

	_mm_loadh_pi = soft_mm_loadh_pi;

	_mm_setr_epi32 = soft_mm_setr_epi32;

    _mm_adds_epu8 = soft_mm_adds_epu8;

    _mm_srli_epi16 = soft_mm_srli_epi16;

    _mm_cmpeq_epi16 = soft_mm_cmpeq_epi16;

    _mm_or_si128 = soft_mm_or_si128;
}

#endif





