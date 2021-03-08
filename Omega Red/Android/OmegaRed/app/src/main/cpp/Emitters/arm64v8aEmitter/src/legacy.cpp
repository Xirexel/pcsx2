/*  PCSX2 - PS2 Emulator for PCs
 *  Copyright (C) 2002-2010  PCSX2 Dev Team
 *
 *  PCSX2 is free software: you can redistribute it and/or modify it under the terms
 *  of the GNU Lesser General Public License as published by the Free Software Found-
 *  ation, either version 3 of the License, or (at your option) any later version.
 *
 *  PCSX2 is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 *  PURPOSE.  See the GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along with PCSX2.
 *  If not, see <http://www.gnu.org/licenses/>.
 */
/*
 * ix86 core v0.6.2
 * Authors: linuzappz <linuzappz@pcsx.net>
 *			alexey silinov
 *			goldfinger
 *			zerofrog(@gmail.com)
 *			cottonvibes(@gmail.com)
 */

//------------------------------------------------------------------
// ix86 legacy emitter functions
//------------------------------------------------------------------

#include "PrecompiledHeader.h"
#include "legacy_internal.h"

#include "../include/Arm64Emitter.h"



emitterT void x86SetJ8(std::shared_ptr<x86Emitter::xForwardJumpBase>& jmpBase)
{
	jmpBase->SetTarget();
}

emitterT void x86SetJ32(std::shared_ptr<x86Emitter::xForwardJumpBase>& jmpBase)
{
	jmpBase->SetTarget();
}

emitterT void x86SetJ32(x86Emitter::xForwardJumpBase& jmpBase)
{
	jmpBase.SetTarget();
}


emitterT void x86SetJ32A(std::shared_ptr<x86Emitter::xForwardJumpBase>& jmpBase)
{

	x86Emitter::xGetAlignedCallTarget();
//	while ((uptr)x86Ptr & 0xf)
//		*x86Ptr++ = 0x90;
	x86SetJ32(jmpBase);
}

emitterT void ModRM(uint mod, uint reg, uint rm)
{
    // Note: Following assertions are for legacy support only.
    // The new emitter performs these sanity checks during operand construction, so these
    // assertions can probably be removed once all legacy emitter code has been removed.
    pxAssert(mod < 4);
    pxAssert(reg < 8);
    pxAssert(rm < 8);
    xWrite8((mod << 6) | (reg << 3) | rm);
}

emitterT void SibSB(uint ss, uint index, uint base)
{
    // Note: Following asserts are for legacy support only.
    // The new emitter performs these sanity checks during operand construction, so these
    // assertions can probably be removed once all legacy emitter code has been removed.
    pxAssert(ss < 4);
    pxAssert(index < 8);
    pxAssert(base < 8);
    xWrite8((ss << 6) | (index << 3) | base);
}

using namespace x86Emitter;

//////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////
// From here on are instructions that have NOT been implemented in the new emitter.
//////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////

emitterT u8 *J8Rel(int cc, int to)
{
    throw "Unimplemented!!!";
//    xWrite8(cc);
//    xWrite8(to);
//    return (u8 *)(x86Ptr - 1);
}

emitterT u32 *J32Rel(int cc, u32 to)
{
    throw "Unimplemented!!!";
//    xWrite8(0x0F);
//    xWrite8(cc);
//    xWrite32(to);
//    return (u32 *)(x86Ptr - 4);
}

////////////////////////////////////////////////////
emitterT void x86SetPtr(u8 *ptr)
{
	g_Emitter.SetCodePointer(ptr, ptr);
}
ATTR_DEP extern u8 * x86GetPtr()
{
	return g_Emitter.GetWritableCodePtr();
}

//////////////////////////////////////////////////////////////////////////////////////////
// Jump Label API (as rough as it might be)
//
// I don't auto-inline these because of the console logging in case of error, which tends
// to cause quite a bit of code bloat.
//
void x86SetJ8(u8 *j8)
{
    throw "Unimplemented!!!";
//    u32 jump = (x86Ptr - j8) - 1;
//
//    if (jump > 0x7f) {
//        Console.Error("j8 greater than 0x7f!!");
//        assert(0);
//    }
//    *j8 = (u8)jump;
}

////////////////////////////////////////////////////
emitterT void x86SetJ32(u32 *j32)
{
    throw "Unimplemented!!!";
//    *j32 = (x86Ptr - (u8 *)j32) - 4;
}

emitterT void x86SetJ32A(u32 *j32)
{
    throw "Unimplemented!!!";
//    while ((uptr)x86Ptr & 0xf)
//        *x86Ptr++ = 0x90;
//    x86SetJ32(j32);
}

////////////////////////////////////////////////////
emitterT void x86Align(int bytes)
{
    if(bytes != 16)
        throw L"Unimplemented!!!";

    g_Emitter.AlignCode16();
}

/********************/
/* IX86 instructions */
/********************/

////////////////////////////////////
// jump instructions				/
////////////////////////////////////

/* jmp rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JMP8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJMP8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jmp rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JMP32(uptr to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJMP32);

	if(to > 0)
		jump->SetTarget(to);
	// to - ( (uptr)x86Ptr + 5 )

	return jump;
}

/* jp rel8 */
emitterT u8 *JP8(u8 to)
{
    return J8Rel(0x7A, to);
}

/* jnp rel8 */
emitterT u8 *JNP8(u8 to)
{
    return J8Rel(0x7B, to);
}

/* je rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JE8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJE8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jz rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JZ8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJZ8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* js rel8 */
emitterT u8 *JS8(u8 to)
{
    return J8Rel(0x78, to);
}

/* jns rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JNS8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJNS8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jg rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JG8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJG8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jge rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JGE8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJGE8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jl rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JL8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJL8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* ja rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JA8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJA8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

emitterT std::shared_ptr<x86Emitter::xForwardJumpBase>JAE8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJAE8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jb rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JB8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJB8);

	if(to > 0)
		jump->SetRelTarget(to);

    return jump;
}

/* jbe rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JBE8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJBE8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jle rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JLE8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJLE8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jne rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JNE8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJNE8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jnz rel8 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JNZ8(u8 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJNZ8);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jng rel8 */
emitterT u8 *JNG8(u8 to)
{
    return J8Rel(0x7E, to);
}

/* jnge rel8 */
emitterT u8 *JNGE8(u8 to)
{
    return J8Rel(0x7C, to);
}

/* jnl rel8 */
emitterT u8 *JNL8(u8 to)
{
    return J8Rel(0x7D, to);
}

/* jnle rel8 */
emitterT u8 *JNLE8(u8 to)
{
    return J8Rel(0x7F, to);
}

/* jo rel8 */
emitterT u8 *JO8(u8 to)
{
    return J8Rel(0x70, to);
}

/* jno rel8 */
emitterT u8 *JNO8(u8 to)
{
    return J8Rel(0x71, to);
}
// jb rel32
emitterT u32 *JB32(u32 to)
{
    return J32Rel(0x82, to);
}

/* je rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JE32(u32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJE32);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jz rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JZ32(u32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJZ32);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* js rel32 */
emitterT u32 *JS32(u32 to)
{
    return J32Rel(0x88, to);
}

/* jns rel32 */
emitterT u32 *JNS32(u32 to)
{
    return J32Rel(0x89, to);
}

/* jg rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JG32(u32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJG32);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jge rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JGE32(u32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJGE32);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jl rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JL32(u32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJL32);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jle rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JLE32(u32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJLE32);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* ja rel32 */
emitterT u32 *JA32(u32 to)
{
    return J32Rel(0x87, to);
}

/* jae rel32 */
emitterT u32 *JAE32(u32 to)
{
    return J32Rel(0x83, to);
}

/* jne rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JNE32(u32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJNE32);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jnz rel32 */
emitterT std::shared_ptr<x86Emitter::xForwardJumpBase> JNZ32(u32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJNZ32);

	if(to > 0)
		jump->SetRelTarget(to);

	return jump;
}

/* jng rel32 */
emitterT u32 *JNG32(u32 to)
{
    return J32Rel(0x8E, to);
}

/* jnge rel32 */
emitterT u32 *JNGE32(u32 to)
{
    return J32Rel(0x8C, to);
}

/* jnl rel32 */
emitterT u32 *JNL32(u32 to)
{
    return J32Rel(0x8D, to);
}

/* jnle rel32 */
emitterT u32 *JNLE32(u32 to)
{
    return J32Rel(0x8F, to);
}

/* jo rel32 */
emitterT u32 *JO32(u32 to)
{
    return J32Rel(0x80, to);
}

/* jno rel32 */
emitterT u32 *JNO32(u32 to)
{
    return J32Rel(0x81, to);
}
