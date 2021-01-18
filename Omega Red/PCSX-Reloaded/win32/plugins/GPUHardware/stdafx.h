#pragma once


// add headers that you want to pre-compile here
#include "framework.h"
#include "fps.h"


typedef INT32(__stdcall *CallbackDelegate)(INT32, UINT32);

#define SHADETEXBIT(x) ((x >> 24) & 0x1)
#define SEMITRANSBIT(x) ((x >> 25) & 0x1)

#define COMBINE_EXT 0x8570

#define FUNC_ADD_EXT 0x8006
#define FUNC_REVERSE_SUBTRACT_EXT 0x800B

#define GL_UNSIGNED_SHORT_4_4_4_4_EXT 0x8033
#define GL_UNSIGNED_SHORT_5_5_5_1_EXT 0x8034


typedef unsigned char uint8;
typedef signed char int8;
typedef unsigned short uint16;
typedef signed short int16;
typedef unsigned int uint32;
typedef signed int int32;
typedef unsigned long long uint64;
typedef signed long long int64;


#define EXPORT_C_(type) type __stdcall
#define EXPORT_C EXPORT_C_(void)

#define PLUGIN_VERSION 0

#define _(x) (x)
#define N_(x) (x)

#ifdef __cplusplus

//#define _WIN32_WINNT 0x0600
#define WIN32_LEAN_AND_MEAN // Exclude rarely-used stuff from Windows headers

#define RESTRICT __restrict
#define ASSERT assert

//#define EXPORT_C_(type) extern "C" type __stdcall
//#define EXPORT_C EXPORT_C_(void)

#include <Unknwnbase.h>


#include <string>
#include <algorithm>
#include <cassert>
#include <d3dcompiler.h>
#include <d3d11.h>
#include <comutil.h>
#include <atlcomcli.h>
#include <sstream>
#include <iostream>
#include <vector>

#include "CommonCPP.h"

#if !defined(_M_SSE) && (!defined(_WIN32) || defined(_M_AMD64) || defined(_M_IX86_FP) && _M_IX86_FP >= 2)

#define _M_SSE 0x200

#endif

#if _M_SSE >= 0x200

#include <xmmintrin.h>
#include <emmintrin.h>

#ifndef _MM_DENORMALS_ARE_ZERO
#define _MM_DENORMALS_ARE_ZERO 0x0040
#endif

#define MXCSR (_MM_DENORMALS_ARE_ZERO | _MM_MASK_MASK | _MM_ROUND_NEAREST | _MM_FLUSH_ZERO_ON)

#define _MM_TRANSPOSE4_SI128(row0, row1, row2, row3)                                        \
    {                                                                                       \
        __m128 tmp0 = _mm_shuffle_ps(_mm_castsi128_ps(row0), _mm_castsi128_ps(row1), 0x44); \
        __m128 tmp2 = _mm_shuffle_ps(_mm_castsi128_ps(row0), _mm_castsi128_ps(row1), 0xEE); \
        __m128 tmp1 = _mm_shuffle_ps(_mm_castsi128_ps(row2), _mm_castsi128_ps(row3), 0x44); \
        __m128 tmp3 = _mm_shuffle_ps(_mm_castsi128_ps(row2), _mm_castsi128_ps(row3), 0xEE); \
        (row0) = _mm_castps_si128(_mm_shuffle_ps(tmp0, tmp1, 0x88));                        \
        (row1) = _mm_castps_si128(_mm_shuffle_ps(tmp0, tmp1, 0xDD));                        \
        (row2) = _mm_castps_si128(_mm_shuffle_ps(tmp2, tmp3, 0x88));                        \
        (row3) = _mm_castps_si128(_mm_shuffle_ps(tmp2, tmp3, 0xDD));                        \
    }

#else

#error TODO: GSVector4 and GSRasterizer needs SSE2

#endif

#undef min
#undef max
#undef abs


struct GSDXError
{
};
struct GSDXRecoverableError : GSDXError
{
};
struct GSDXErrorGlVertexArrayTooSmall : GSDXError
{
};


class GPURenderer;

extern GPURenderer *s_gpu;

extern "C" uint64_t g_currentClutId;
extern "C" int iHiResTextures;

extern CallbackDelegate s_CallbackDelegate;


#ifndef __FUNCTION__
#define __FUNCTION__ "Empty"
#endif

#ifdef _DEBUG
#define _DEBUG_RENDERER
#endif

#define OUTPUT_LOG(lresult)                 \
    {                                       \
        OutputLog(                          \
            "Line: %s; Error code: %08x\n", \
            __FUNCTIONW__,                  \
            lresult);                       \
        return;                             \
    }

#define CHECK_OUTPUT_LOG(hr) \
    if (FAILED(hr))          \
    OUTPUT_LOG(hr)


#ifdef _DEBUG_RENDERER
#define LOG_INVOKE(hr) CHECK_OUTPUT_LOG(hr)
#else
#define LOG_INVOKE(hr) \
    if (FAILED(hr))    \
        return;

#endif

#define LOG_INVOKE_QUERY_INTERFACE(Object, PtrPtrPointer) LOG_INVOKE(Object->QueryInterface(IID_PPV_ARGS(PtrPtrPointer)))

#ifdef _DEBUG_RENDERER
#define LOG_CHECK_PTR_MEMORY(Object) \
    if (Object == nullptr)           \
    OUTPUT_LOG(E_POINTER)

#else
#define LOG_CHECK_PTR_MEMORY(Object) \
    if (Object == nullptr)           \
        return;

#endif


#else

signed int __stdcall GPU_Stub_open(void *hWnd);
signed int __stdcall GPU_Stub_close();
void __stdcall swapBuffers();
void __stdcall clearBuffers(DWORD aMask);
void __stdcall setClearColor(float aRed, float aGreen, float aBlue, float aAlpha);
void __stdcall setOrtho(double aLeft, double aRight, double aBottom, double aTop, double azNear, double azFar);
void __stdcall draw(DWORD aDrawMode, void *aPtrVertexes, BOOL aIsTextured);
void __stdcall glBindTextureStub(GLenum target, GLuint texture);
void __stdcall glTexImage2DStub(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, const GLvoid *pixels);
void __stdcall glTexSubImage2DStub(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, const GLvoid *pixels);
void __stdcall glDeleteTexturesStub(GLsizei n, const GLuint *textures);
void __stdcall glBlendFuncStub(GLenum sfactor, GLenum dfactor);
void __stdcall glBlendEquationEXTExStub(GLenum mode);
void __stdcall glEnableStub(GLenum cap);
void __stdcall glDisableStub(GLenum cap);
void __stdcall glAlphaFuncStub(GLenum func, GLclampf ref);
void __stdcall glCopyTexSubImage2DStub(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint x, GLint y, GLsizei width, GLsizei height);
void __stdcall glDepthFuncStub(GLenum func);
void __stdcall glScissorStub(GLint x, GLint y, GLsizei width, GLsizei height);


typedef struct _VERTEX
{
    float x;
    float y;
    float z;
    float w;

    float sow;
    float tow;

    unsigned int lcol;

} VERTEX;

extern uint64_t g_currentClutId;

extern VERTEX g_border_vertexes[4];

void PRIMDirectXdrawBorder();

#endif