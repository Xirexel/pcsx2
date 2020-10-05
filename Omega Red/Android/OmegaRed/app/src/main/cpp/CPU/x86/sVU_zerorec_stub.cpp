//
// Created by Xirexel on 7/8/2019.
//

#include "PrecompiledHeader.h"

#include <float.h>
#include <list>
#include <map>

#include "Utilities/AsciiFile.h"

#ifndef _WIN32
#include <sys/types.h>
#endif

#include "Common.h"

#include "GS.h"
#include "Gif.h"
#include "VU.h"
#include "MTVU.h"

#include "R5900.h"
#include "iR5900.h"
#include "System/RecTypes.h"

#include "sVU_zerorec.h"
#include "NakedAsm.h"
#include "AppConfig.h"

// Needed in gcc for find.
#include <algorithm>

extern u32 s_TotalVUCycles; // total cycles since start of program execution
extern uptr s_vu1esp;
extern uptr s_callstack;

void SuperVUExecuteProgram(u32 startpc, int vuindex)
{


	// Stackframe setup for the recompiler:
	// We rewind the stack 4 bytes, which places the parameters of this function before
	// any calls we might make from recompiled code.  The return address for this function
	// call is subsequently stored in s_callstack.

//	__asm__ ("movl (%esp), %eax\n\t"
//			 "movl 	 	$0,   $ebx\n\t"
//			 //			 "movl %ecx, $label(%edx,%ebx,$4)\n\t"
//			 "movb %ah, (%ebx)");

//	__asm__ ("movl %eax, %ebx\n\t"
//			 "movl $56, %esi\n\t"
////			 "movl %ecx, $label(%edx,%ebx,$4)\n\t"
//			 "movb %ah, (%ebx)");

//	__asm
//	{
//	mov eax, dword ptr [esp]
//	mov s_TotalVUCycles, 0 // necessary to be here!
//	add esp, 4
//	mov s_callstack, eax
//	call SuperVUGetProgram
//
//	// save cpu state
//	//mov s_vu1ebp, ebp
//	mov s_vu1esi, esi
//	mov s_vuedi, edi
//	mov s_vuebx, ebx
//
//	mov s_vu1esp, esp
//	and esp, -16		// align stack for GCC compilance
//
//	//stmxcsr s_ssecsr
//	ldmxcsr g_sseVUMXCSR
//
//	// init vars
//	mov s_writeQ, 0xffffffff
//	mov s_writeP, 0xffffffff
//
//	jmp eax
//	}
}


// exit point of all vu programs
void SuperVUEndProgram()
{

//	throw L"Unimplemented!!!";

//	__asm
//	{
//	// restore cpu state
//	ldmxcsr g_sseMXCSR
//
//	//mov ebp, s_vu1ebp
//	mov esi, s_vu1esi
//	mov edi, s_vuedi
//	mov ebx, s_vuebx
//
//	mov esp, s_vu1esp	// restore from aligned stack
//
//	call SuperVUCleanupProgram
//	jmp s_callstack // so returns correctly
//	}
}
