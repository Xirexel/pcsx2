//
// Created by Evgeny Pereguda on 4/14/2019.
//

#ifndef OMEGARED_REDEFINE_H
#define OMEGARED_REDEFINE_H


#include <stdint.h>

class wxString;

#include "wx/string.h"
#include "wx/filefn.h"
#include <unistd.h>


#ifndef __packed
#define __packed __attribute__((packed))
#endif
#ifndef __aligned
#define __aligned(alig) __attribute__((aligned(alig)))
#endif
#define __aligned16 __attribute__((aligned(16)))
#define __aligned32 __attribute__((aligned(32)))
#define __pagealigned __attribute__((aligned(PCSX2_PAGESIZE)))
// Deprecated; use __align instead.
#define PCSX2_ALIGNED(alig, x) x __attribute((aligned(alig)))
#define PCSX2_ALIGNED16(x) x __attribute((aligned(16)))
#define PCSX2_ALIGNED_EXTERN(alig, x) extern x __attribute((aligned(alig)))
#define PCSX2_ALIGNED16_EXTERN(x) extern x __attribute((aligned(16)))

#define __assume(cond) ((void)0) // GCC has no equivalent for __assume
//#define CALLBACK __attribute__((stdcall))

#define __int64 long long


extern unsigned int __builtin_ia32_readeflags_u32();

extern void wcscpy_s(
	wchar_t* _Destination,
	size_t _SizeInWords,
	wchar_t const* _Source
);



#endif //OMEGARED_REDEFINE_H
