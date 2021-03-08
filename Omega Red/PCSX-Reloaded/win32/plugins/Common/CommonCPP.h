#pragma once


#define PUGIXML_WCHAR_MODE

#define countof(a) (sizeof(a) / sizeof(a[0]))

#define LONG long

#define CALLBACK __stdcall


#include <stdint.h>

typedef int8_t int_least8_t;

typedef uint8_t uint_least8_t;

typedef int16_t int_least16_t;

typedef uint16_t uint_least16_t;

typedef int32_t int_least32_t;

typedef uint32_t uint_least32_t;

typedef int64_t int_least64_t;

typedef uint64_t uint_least64_t;


typedef int8_t int_fast8_t;

typedef uint8_t uint_fast8_t;

typedef int16_t int_fast16_t;

typedef uint16_t uint_fast16_t;

typedef int32_t int_fast32_t;

typedef uint32_t uint_fast32_t;

typedef int64_t int_fast64_t;

typedef uint64_t uint_fast64_t;

typedef int64_t intmax_t;

typedef uint64_t uintmax_t;

typedef unsigned char uint8;
typedef signed char int8;
typedef unsigned short uint16;
typedef signed short int16;
typedef unsigned int uint32;
typedef signed int int32;
typedef unsigned long long uint64;
typedef signed long long int64;

