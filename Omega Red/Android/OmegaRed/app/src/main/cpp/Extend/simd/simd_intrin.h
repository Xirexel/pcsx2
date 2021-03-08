//
// Created by Evgeny Pereguda on 8/29/2019.
//

#ifndef OMEGARED_SIMD_INTRIN_H
#define OMEGARED_SIMD_INTRIN_H




/* Or together 2 sets of indexes (X and Y) with the zeroing bits (Z) to create
   an index suitable for _mm_insert_ps.  */
#define _MM_MK_INSERTPS_NDX(X, Y, Z) (((X) << 6) | ((Y) << 4) | (Z))

#define _MM_SHUFFLE(z, y, x, w) (((z) << 6) | ((y) << 4) | ((x) << 2) | (w))


typedef long long __m128i __attribute__((__vector_size__(16)));

typedef float __m128 __attribute__((__vector_size__(16)));

typedef long long __m64 __attribute__((__vector_size__(8)));



typedef unsigned int (*MM_GETCSR)();

typedef void (*MM_SETCSR)(unsigned int a);

typedef __m128i (*MM_SETZERO_SI128)();

typedef __m128i (*MM_LOAD_SI128)(__m128i const* mem_addr);

typedef __m128i (*MM_UNPACKLO_EPI8)(__m128i a, __m128i b);

typedef __m128 (*MM_SETZERO_PS)();

typedef void (*MM_STORE_PS)(float* mem_addr, __m128 a);

typedef __m128 (*MM_SET_PS1)(float a);

typedef __m128 (*MM_LOAD_PS)(float const* mem_addr);

typedef void (*MM_STORE_SI128)(__m128i* mem_addr, __m128i a);

typedef __m128i (*MM_UNPACKHI_EPI8)(__m128i a, __m128i b);

typedef __m128i (*MM_SET1_EPI8)(char a);

typedef __m128i (*MM_SET1_EPI16)(short a);

typedef __m128i (*MM_LOADL_EPI64)(__m128i const* mem_addr);

typedef __m128i (*MM_XOR_SI128)(__m128i a, __m128i b);

typedef __m128i (*MM_MULHI_EPI16)(__m128i a, __m128i b);

typedef __m128i (*MM_ADDS_EPI16)(__m128i a, __m128i b);

typedef __m128i (*MM_SUBS_EPU8)(__m128i a, __m128i b);

typedef __m128i (*MM_SLLI_EPI16)(__m128i a, int imm8);

typedef __m128i (*MM_ADD_SI128)(__m128i a, __m128i b);

typedef __m128i (*MM_MULHI_EPU16)(__m128i a, __m128i b);

typedef __m128i (*MM_ADD_EPI16)(__m128i a, __m128i b);

typedef __m128i (*MM_SRAI_EPI16)(__m128i a, int imm8);

typedef __m128i (*MM_PACKUS_EPI16)(__m128i a, __m128i b);

typedef __m128i (*MM_UNPACKLO_EPI16)(__m128i a, __m128i b);

typedef __m128i (*MM_UNPACKHI_EPI16)(__m128i a, __m128i b);

typedef __m128i (*MM_SHUFFLE_EPI32)(__m128i a, int imm8);

typedef __m128 (*MM_LOADH_PI)(__m128 a, __m64 const* mem_addr);

typedef __m128i (*MM_SETR_EPI32)(int e3, int e2, int e1, int e0);

typedef __m128i (*MM_ADD_EPU8)(__m128i a, __m128i b);

typedef __m128i (*MM_SRLI_EPI16)(__m128i a, int imm8);

typedef __m128i (*MM_CMPEQ_EPI16)(__m128i a, __m128i b);

typedef __m128i (*MM_OR_SI128)(__m128i a, __m128i b);


extern MM_GETCSR _mm_getcsr;

extern MM_SETCSR _mm_setcsr;

extern MM_SETZERO_SI128 _mm_setzero_si128;

extern MM_LOAD_SI128 _mm_load_si128;

extern MM_UNPACKLO_EPI8 _mm_unpacklo_epi8;

extern MM_SETZERO_PS _mm_setzero_ps;

extern MM_STORE_PS _mm_store_ps;

extern MM_SET_PS1 _mm_set_ps1;

extern MM_LOAD_PS _mm_load_ps;

extern MM_STORE_SI128 _mm_store_si128;

extern MM_UNPACKHI_EPI8 _mm_unpackhi_epi8;

extern MM_SET1_EPI8 _mm_set1_epi8;

extern MM_SET1_EPI16 _mm_set1_epi16;

extern MM_LOADL_EPI64 _mm_loadl_epi64;

extern MM_XOR_SI128 _mm_xor_si128;

extern MM_MULHI_EPI16 _mm_mulhi_epi16;

extern MM_ADDS_EPI16 _mm_adds_epi16;

extern MM_SUBS_EPU8 _mm_subs_epu8;

extern MM_SLLI_EPI16 _mm_slli_epi16;

extern MM_ADD_SI128 _mm_and_si128;

extern MM_MULHI_EPU16 _mm_mulhi_epu16;

extern MM_ADD_EPI16 _mm_add_epi16;

extern MM_SRAI_EPI16 _mm_srai_epi16;

extern MM_PACKUS_EPI16 _mm_packus_epi16;

extern MM_UNPACKLO_EPI16 _mm_unpacklo_epi16;

extern MM_UNPACKHI_EPI16 _mm_unpackhi_epi16;

extern MM_SHUFFLE_EPI32 _mm_shuffle_epi32;

extern MM_LOADH_PI _mm_loadh_pi;

extern MM_SETR_EPI32 _mm_setr_epi32;

extern MM_ADD_EPU8 _mm_adds_epu8;

extern MM_SRLI_EPI16 _mm_srli_epi16;

extern MM_CMPEQ_EPI16 _mm_cmpeq_epi16;

extern MM_OR_SI128 _mm_or_si128;


#endif //OMEGARED_SIMD_INTRIN_H
