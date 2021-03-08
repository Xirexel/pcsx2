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
 * ix86 core v0.9.1
 *
 * Original Authors (v0.6.2 and prior):
 *		linuzappz <linuzappz@pcsx.net>
 *		alexey silinov
 *		goldfinger
 *		zerofrog(@gmail.com)
 *
 * Authors of v0.9.1:
 *		Jake.Stine(@gmail.com)
 *		cottonvibes(@gmail.com)
 *		sudonim(1@gmail.com)
 */

#include "PrecompiledHeader.h"
#include "internal.h"

#include "../include/Arm64Emitter.h"

namespace x86Emitter
{

void xImpl_JmpCall::operator()(const xAddressReg &absreg) const {

    if(isJmp)
    {
        if(absreg.IsWide())
        {
            Arm64Gen::ARM64Reg Rt = getReg(absreg);

            g_Emitter.BR(Rt);
        }
        else
        {
            throw L"Unimplemented!!!";
        }
    }
    else
    {
        throw L"Unimplemented!!!";

        // Jumps are always wide and don't need the rex.W
        xOpWrite(0, 0xff, isJmp ? 4 : 2, absreg.GetNonWide());
    }
}
void xImpl_JmpCall::operator()(const xIndirectNative &src) const {

    if(isJmp)
    {
        if(src.Base.IsEmpty() && !src.Index.IsEmpty() && src.Displacement != 0)
        {
            if(src.Scale == 2)
            {
                g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, src.Displacement);

                Arm64Gen::ARM64Reg Rt = getReg(src.Index);

                g_Emitter.LDR(Arm64JitConstants::SCRATCH1_64,
                              Arm64JitConstants::SCRATCH2_64,
                              Arm64Gen::ArithOption(
                                      Rt, true));

                g_Emitter.BR(Arm64JitConstants::SCRATCH1_64);
            }
            else
            {
                throw L"Unimplemented!!!";
            }
        }
        else if(!src.Base.IsEmpty() && !src.Index.IsEmpty() && src.Displacement == 0)
        {
            Arm64Gen::ARM64Reg Rt = getReg(src.Index);

            g_Emitter.LSL(Arm64JitConstants::SCRATCH2_64, Rt, src.Scale);

            Rt = getReg(src.Base);

            g_Emitter.ADD(Arm64JitConstants::SCRATCH2_64, Arm64JitConstants::SCRATCH2_64, Rt);

            g_Emitter.LDR(Arm64JitConstants::SCRATCH1_64,
                          Arm64JitConstants::SCRATCH2_64);

            g_Emitter.BR(Arm64JitConstants::SCRATCH1_64);
        }
        else if(src.Base.IsEmpty() && src.Index.IsEmpty() && src.Displacement != 0)
        {
            g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, src.Displacement);

            g_Emitter.LDR(Arm64JitConstants::SCRATCH1_64,
                          Arm64JitConstants::SCRATCH2_64);

            g_Emitter.BR(Arm64JitConstants::SCRATCH1_64);
        }
        else
        {
            throw L"Unimplemented!!!";
        }
    }
    else
    {
//        if(!src.Base.IsEmpty() && !src.Index.IsEmpty() && src.Scale == 0 && src.Displacement == 0)
//        {
//            Arm64Gen::ARM64Reg Rt = getReg(src.Index);
//
//            g_Emitter.LSL(Arm64JitConstants::SCRATCH2_64, Rt, src.Scale);
//
//            Rt = getReg(src.Base);
//
//            g_Emitter.ADD(Arm64JitConstants::SCRATCH2_64, Arm64JitConstants::SCRATCH2_64, Rt);
//
//            g_Emitter.LDR(Arm64JitConstants::SCRATCH1_64,
//                          Arm64JitConstants::SCRATCH2_64);
//
//            g_Emitter.BLR(Arm64JitConstants::SCRATCH1_64);
//        }
//        else if(!src.Base.IsEmpty() && !src.Index.IsEmpty() && src.Scale == 3 && src.Displacement == 0)
//        {
//            Arm64Gen::ARM64Reg Rt = Arm64JitConstants::SCRATCH1_64;
//
//            Arm64Gen::ARM64Reg Rn = getReg(src.Index);
//
//            Arm64Gen::ARM64Reg Rb = getReg(src.Base);
//
//            g_Emitter.LDR(Rt,
//                          Rb,
//                          Arm64Gen::ArithOption(
//                                  Rn, true));
//
//            g_Emitter.BLR(Rt);
//        }
//        else
            EmitSibMagic(isJmp ? 4 : 2, src);
    }
}

const xImpl_JmpCall xJMP = {true};
const xImpl_JmpCall xCALL = {false};


template <typename Reg1, typename Reg2>
void prepareRegsForFastcall(const Reg1 &a1, const Reg2 &a2) {

    if (a1.IsEmpty()) return;

    // Make sure we don't mess up if someone tries to fastcall with a1 in arg2reg and a2 in arg1reg
    if (a2.Id != rax.Id) {
        xMOV(Reg1(rax.Id), a1);
        if (!a2.IsEmpty()) {
            xMOV(Reg2(rcx.Id), a2);
        }
    } else if (a1.Id != arg2reg.Id) {
        throw L"Unimplemented!!!";
        xMOV(Reg2(arg2reg.Id), a2);
        xMOV(Reg1(arg1reg.Id), a1);
    } else {
        throw L"Unimplemented!!!";
        xPUSH(a1);
        xMOV(Reg2(arg2reg.Id), a2);
        xPOP(Reg1(arg1reg.Id));
    }
}

void xImpl_FastCall::operator()(void *f, const xRegister32 &a1, const xRegister32 &a2) const {
    prepareRegsForFastcall(a1, a2);

    g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, (u64)f);

    g_Emitter.BLR(Arm64JitConstants::SCRATCH2_64);
}

#ifdef __M_X86_64
void xImpl_FastCall::operator()(void *f, const xRegisterLong &a1, const xRegisterLong &a2) const {

    prepareRegsForFastcall(a1, a2);

    g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, (u64)f);

    g_Emitter.BLR(Arm64JitConstants::SCRATCH2_64);
}

void xImpl_FastCall::operator()(void *f, u32 a1, const xRegisterLong &a2) const {
    if (!a2.IsEmpty()) { xMOV(arg2reg, a2); }
    xMOV(arg1reg, a1);
    (*this)(f, arg1reg, arg2reg);
}
#endif

void xImpl_FastCall::operator()(void *f, void *a1) const {
    throw L"Unimplemented!!!";
    xLEA(arg1reg, ptr[a1]);
    (*this)(f, arg1reg, arg2reg);
}

void xImpl_FastCall::operator()(void *f, u32 a1, const xRegister32 &a2) const {
    throw L"Unimplemented!!!";
    if (!a2.IsEmpty()) { xMOV(arg2regd, a2); }
    xMOV(arg1regd, a1);
    (*this)(f, arg1regd, arg2regd);
}

void xImpl_FastCall::operator()(void *f, const xIndirect32 &a1) const {
    xMOV(eax, a1);
    (*this)(f, eax);
}

void xImpl_FastCall::operator()(void *f, u32 a1, u32 a2) const {
    xMOV(eax, a1);
    xMOV(ecx, a2);
    (*this)(f, eax, ecx);
}

void xImpl_FastCall::operator()(const xIndirectNative &f, const xRegisterLong &a1, const xRegisterLong &a2) const {

    xAddressReg lScratchReg(Arm64JitConstants::SCRATCH1_64 - Arm64Gen::ARM64Reg::X0);

    xMOV(lScratchReg, f);

    prepareRegsForFastcall(a1, a2);

    g_Emitter.BLR(Arm64JitConstants::SCRATCH1_64);
}

const xImpl_FastCall xFastCall = {};

void xSmartJump::SetTarget()
{
    throw L"Unimplemented!!!";
    u8 *target = xGetPtr();
    if (m_baseptr == NULL)
        return;

    xSetPtr(m_baseptr);
    u8 *const saveme = m_baseptr + GetMaxInstructionSize();
    xJccKnownTarget(m_cc, target, true);

    // Copy recompiled data inward if the jump instruction didn't fill the
    // alloted buffer (means that we optimized things to a j8!)

    const int spacer = (sptr)saveme - (sptr)xGetPtr();
    if (spacer != 0) {
        u8 *destpos = xGetPtr();
        const int copylen = (sptr)target - (sptr)saveme;

        memcpy(destpos, saveme, copylen);
        xSetPtr(target - spacer);
    }
}

xSmartJump::~xSmartJump()
{
    SetTarget();
    m_baseptr = NULL; // just in case (sometimes helps in debugging too)
}

// ------------------------------------------------------------------------
// Emits a 32 bit jump, and returns a pointer to the 32 bit displacement.
// (displacements should be assigned relative to the end of the jump instruction,
// or in other words *(retval+1) )
__emitinline s32 *xJcc32Inner(JccComparisonType comparison)
{
    throw L"Unimplemented!!!";
    if (comparison == Jcc_Unconditional)
        xWrite8(0xe9);
    else {
        xWrite8(0x0f);
        xWrite8(0x80 | comparison);
    }
    xWrite<s32>(0);

    return ((s32 *)xGetPtr()) - 1;
}

__emitinline std::shared_ptr<x86Emitter::xForwardJumpBase> xJcc32(JccComparisonType comparison)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJMP32(comparison));
	
	return jump;
}

__emitinline std::shared_ptr<x86Emitter::xForwardJumpBase> JS32(JccComparisonType comparison, s32 to)
{
	std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJS32);

	//jump->SetRelTarget(to);

	return jump;
}

// ------------------------------------------------------------------------
// Emits a 32 bit jump, and returns a pointer to the 8 bit displacement.
// (displacements should be assigned relative to the end of the jump instruction,
// or in other words *(retval+1) )
__emitinline s8 *xJcc8(JccComparisonType comparison, s8 displacement)
{
    throw L"Unimplemented!!!";
    xWrite8((comparison == Jcc_Unconditional) ? 0xeb : (0x70 | comparison));
    xWrite<s8>(displacement);
    return (s8 *)xGetPtr() - 1;
}

__emitinline std::shared_ptr<x86Emitter::xForwardJumpBase> xJcc8(JccComparisonType comparison)
{
    std::shared_ptr<x86Emitter::xForwardJumpBase> jump(new xForwardJMP8(comparison));

    return jump;
}

// ------------------------------------------------------------------------
// Writes a jump at the current x86Ptr, which targets a pre-established target address.
// (usually a backwards jump)
//
// slideForward - used internally by xSmartJump to indicate that the jump target is going
// to slide forward in the event of an 8 bit displacement.
//
__emitinline void xJccKnownTarget(JccComparisonType comparison, const void *target, bool slideForward)
{

    // Calculate the potential j8 displacement first, assuming an instruction length of 2:
    sptr displacement = (sptr)target - (sptr)xGetPtr();

    const int maxJump = (comparison == Jcc_Unconditional ? 134217720 : 1048568) ; // 134217728 : 1048576
    auto l_abs_diff = abs(displacement);

    if (maxJump > l_abs_diff)
    {
        auto jump = xJcc8(comparison);

        jump->SetRelTarget(displacement);
    }
    else {
        // Perform a 32 bit jump instead. :(
        auto jump = xJcc32(comparison);

        jump->SetTarget((uptr)target);
    }
}

// Low-level jump instruction!  Specify a comparison type and a target in void* form, and
// a jump (either 8 or 32 bit) is generated.
__emitinline void xJcc(JccComparisonType comparison, const void *target)
{
    xJccKnownTarget(comparison, target, false);
}

xForwardJumpBase::xForwardJumpBase(uint opsize, JccComparisonType cctype)
{
    pxAssert(opsize == 1 || opsize == 4);
    pxAssertDev(cctype != Jcc_Unknown, "Invalid ForwardJump conditional type.");

    pxAssert(opsize == 1 || opsize == 4);
    pxAssertDev(cctype != Jcc_Unknown, "Invalid ForwardJump conditional type.");

    auto l_CCFlag = ComparisonTypeToCCFlag(cctype);

    if(opsize == 1)
    {
        Arm64Gen::FixupBranch l_branch;

        if(l_CCFlag != CC_AL)
            l_branch = g_Emitter.B(l_CCFlag);
        else
            l_branch = g_Emitter.B();

        _fixupBranchJump.ptr = l_branch.ptr;
        _fixupBranchJump.type = l_branch.type;
        _fixupBranchJump.cond = l_branch.cond;
        _fixupBranchJump.bit = l_branch.bit;
        _fixupBranchJump.reg = l_branch.reg;
    }
    else if(opsize == 4)
    {
        Arm64Gen::FixupBranch l_branch;

        if(l_CCFlag != CC_AL)
            l_branch = g_Emitter.B(l_CCFlag, 32);
        else
            l_branch = g_Emitter.B(32);

        _fixupBranchJump.ptr = l_branch.ptr;
        _fixupBranchJump.type = l_branch.type;
        _fixupBranchJump.cond = l_branch.cond;
        _fixupBranchJump.bit = l_branch.bit;
        _fixupBranchJump.reg = l_branch.reg;
    }
    else
        throw L"Unimplemented!!!";
}

void xForwardJumpBase::_setTarget(uint opsize) const
{

    Arm64Gen::FixupBranch l_FixupBranch;



    l_FixupBranch.ptr = _fixupBranchJump.ptr;
    l_FixupBranch.type = _fixupBranchJump.type;
    l_FixupBranch.cond = (CCFlags)_fixupBranchJump.cond;
    l_FixupBranch.bit = _fixupBranchJump.bit;
    l_FixupBranch.reg = (Arm64Gen::ARM64Reg)_fixupBranchJump.reg;


    g_Emitter.SetJumpTarget(l_FixupBranch);
}

void xForwardJumpBase::_setTarget(uptr target, uint opsize) const
{

    Arm64Gen::FixupBranch l_FixupBranch;



    l_FixupBranch.ptr = _fixupBranchJump.ptr;
    l_FixupBranch.type = _fixupBranchJump.type;
    l_FixupBranch.cond = (CCFlags)_fixupBranchJump.cond;
    l_FixupBranch.bit = _fixupBranchJump.bit;
    l_FixupBranch.reg = (Arm64Gen::ARM64Reg)_fixupBranchJump.reg;


    g_Emitter.SetJumpTarget(l_FixupBranch, (s64)target);
}

void xForwardJumpBase::_setTarget(sptr target, uint opsize) const
{

    Arm64Gen::FixupBranch l_FixupBranch;



    l_FixupBranch.ptr = _fixupBranchJump.ptr;
    l_FixupBranch.type = _fixupBranchJump.type;
    l_FixupBranch.cond = (CCFlags)_fixupBranchJump.cond;
    l_FixupBranch.bit = _fixupBranchJump.bit;
    l_FixupBranch.reg = (Arm64Gen::ARM64Reg)_fixupBranchJump.reg;


    g_Emitter.SetJumpTarget(l_FixupBranch, (s64)target);
}



// returns the inverted conditional type for this Jcc condition.  Ie, JNS will become JS.
__fi JccComparisonType xInvertCond(JccComparisonType src)
{
    pxAssert(src != Jcc_Unknown);
    if (Jcc_Unconditional == src)
        return Jcc_Unconditional;

    // x86 conditionals are clever!  To invert conditional types, just invert the lower bit:
    return (JccComparisonType)((int)src ^ 1);
}
}
