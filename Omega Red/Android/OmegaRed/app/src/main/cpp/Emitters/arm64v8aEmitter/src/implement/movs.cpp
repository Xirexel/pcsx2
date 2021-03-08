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
#include "implement/helpers.h"

#include "../include/Arm64Emitter.h"
#include "../../include/types.h"

namespace x86Emitter
{

void _xMovRtoR(const xRegisterInt &to, const xRegisterInt &from)
{
    pxAssert(to.GetOperandSize() == from.GetOperandSize());

    if (to == from)
        return; // ignore redundant MOVs.

    g_Emitter.MOV(getReg(to), getReg(from));
}

void xImpl_Mov::operator()(const xRegisterInt &to, const xRegisterInt &from) const
{
    // FIXME WTF?
    _xMovRtoR(to, from);
}

void xImpl_Mov::operator()(const xIndirectVoid &dest, const xRegisterInt &from) const
{
    Arm64Gen::ARM64Reg addressReg = Arm64JitConstants::SCRATCH1_64;

    setAddress(addressReg, dest);

    switch(dest.DataType)
    {
        case Word:
            g_Emitter.STR(getWReg(from), addressReg);
            break;
        case Double_Word:
            g_Emitter.STR(getXReg(from), addressReg);
            break;
        default:
            throw L"Unimplemented!!!";
            break;
    }
}

void xImpl_Mov::operator()(const xRegisterInt &to, const xIndirectVoid &src) const
{
    // mov eax has a special from when reading directly from a DISP32 address
    // (sans any register index/base registers).

    if (src.Index.IsEmpty() && src.Base.IsEmpty()) {

        g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, src.Displacement);

        g_Emitter.LDR(getReg(to), Arm64JitConstants::SCRATCH2_64);
    }
    else
    {

        if(!src.Index.IsEmpty() && src.Base.IsEmpty() && src.Scale == 3)
        {
            g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, src.Displacement);

            Arm64Gen::ARM64Reg Rt = getReg(to);

            Arm64Gen::ARM64Reg Rn = getReg(src.Index);

            g_Emitter.LDR(Rt,
                          Arm64JitConstants::SCRATCH2_64,
                          Arm64Gen::ArithOption(
                                  Rn, true));
        }
        else if(!src.Index.IsEmpty() && src.Base.IsEmpty() && src.Scale == 0)
        {
            g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, src.Displacement);

            Arm64Gen::ARM64Reg Rt = getReg(to);

            Arm64Gen::ARM64Reg Rn = getReg(src.Index);

            g_Emitter.LDR(Rt,
                          Arm64JitConstants::SCRATCH2_64,
                          Arm64Gen::ArithOption(
                                  Rn));
        }
        else if(!src.Index.IsEmpty() && !src.Base.IsEmpty() && src.Scale == 3 && src.Displacement == 0)
        {
            Arm64Gen::ARM64Reg Rt = getReg(to);

            Arm64Gen::ARM64Reg Rn = getReg(src.Index);

            Arm64Gen::ARM64Reg Rb = getReg(src.Base);

            g_Emitter.LDR(Rt,
                          Rb,
                          Arm64Gen::ArithOption(
                                  Rn, true));
        }
        else if(!src.Index.IsEmpty() && !src.Base.IsEmpty() && src.Displacement == 0)
        {
            Arm64Gen::ARM64Reg Rt = getReg(to);

            Arm64Gen::ARM64Reg Rn = getReg(src.Index);

            Arm64Gen::ARM64Reg Rb = getReg(src.Base);

            g_Emitter.LSL(Arm64JitConstants::SCRATCH1_64,
                          Rn,
                          src.Scale);

            g_Emitter.ADD(Arm64JitConstants::SCRATCH1_64, Rb, Arm64JitConstants::SCRATCH1_64);

            g_Emitter.LDR(Rt, Arm64JitConstants::SCRATCH1_64);
        }
        else
        {
            throw L"Unimplemented!!!";
        }
    }
}

void xImpl_Mov::operator()(const xIndirect64orLess &dest, sptr imm) const
{

    g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH1_64, imm);

    switch (dest.GetOperandSize()) {
        case 1:
            throw L"Unimplemented!!!";
            pxAssertMsg(imm == (s8)imm || imm == (u8)imm, "Immediate won't fit!");
            break;
        case 2:
            throw L"Unimplemented!!!";
            pxAssertMsg(imm == (s16)imm || imm == (u16)imm, "Immediate won't fit!");
            break;
        case 4:
            pxAssertMsg(imm == (s32)imm || imm == (u32)imm, "Immediate won't fit!");
            break;
        case 8:
            pxAssertMsg(imm == (s32)imm, "Immediate won't fit in immediate slot, go through a register!");
            break;
        default:
            throw L"Unimplemented!!!";
            break;
    }

    Arm64Gen::ARM64Reg target = Arm64JitConstants::SCRATCH2_64;

    setAddress(target, dest);

    switch (dest.GetOperandSize()) {
        case 4:
            g_Emitter.STR(Arm64JitConstants::SCRATCH1_32, target);
            break;
        case 8:
            g_Emitter.STR(Arm64JitConstants::SCRATCH1_64, target);
            break;
        default:
            throw L"Unimplemented!!!";
            break;
    }
}

// preserve_flags  - set to true to disable optimizations which could alter the state of
//   the flags (namely replacing mov reg,0 with xor).
void xImpl_Mov::operator()(const xRegisterInt &to, sptr imm, bool preserve_flags) const
{
    switch (to.GetOperandSize()) {
        case 1:
            pxAssertMsg(imm == (s8)imm || imm == (u8)imm, "Immediate won't fit!");
            break;
        case 2:
            pxAssertMsg(imm == (s16)imm || imm == (u16)imm, "Immediate won't fit!");
            break;
        case 4:
            pxAssertMsg(imm == (s32)imm || imm == (u32)imm, "Immediate won't fit!");
            break;
        case 8:
            pxAssertMsg(imm == (s32)imm || imm == (u32)imm, "Immediate won't fit in immediate slot, use mov64 or lea!");
            break;
        default:
            pxAssertMsg(0, "Bad indirect size!");
    }


    if(to.IsWide())
        g_Emitter.MOVI2R(getReg(to), imm);
    else if(to.GetOperandSize() == 4)
    {
        if(imm == (s32)imm || imm == (u32)imm)
            g_Emitter.MOVI2R(getReg(to), imm);
        else
            g_Emitter.MOVI2R(getXReg(to), imm);
    }
    else if(to.GetOperandSize() == 1)
    {
        g_Emitter.MOVZ(Arm64JitConstants::SCRATCH1_32, imm);

        g_Emitter.BFI(getXReg(to), Arm64JitConstants::SCRATCH1_32, 0, 8);
    }
    else
    {
        throw L"Unimplemented!!!";
    }
}

const xImpl_Mov xMOV;

#ifdef __M_X86_64
void xImpl_MovImm64::operator()(const xRegister64& to, s64 imm, bool preserve_flags) const
{
    if (imm == (u32)imm || imm == (s32)imm) {
        xMOV(to, imm, preserve_flags);
    } else {
        u8 opcode = 0xb8 | to.Id;
        xOpAccWrite(to.GetPrefix16(), opcode, 0, to);
        xWrite64(imm);
    }
}

const xImpl_MovImm64 xMOV64;
#endif

// --------------------------------------------------------------------------------------
//  CMOVcc
// --------------------------------------------------------------------------------------

#define ccSane() pxAssertDev(ccType >= 0 && ccType <= 0x0f, "Invalid comparison type specifier.")

// Macro useful for trapping unwanted use of EBP.
//#define EbpAssert() pxAssert( to != ebp )
#define EbpAssert()



void xImpl_CMov::operator()(const xRegister16or32or64 &to, const xRegister16or32or64 &from) const
{
    pxAssert(to->GetOperandSize() == from->GetOperandSize());
    ccSane();
    xOpWrite0F(to->GetPrefix16(), 0x40 | ccType, to, from);
}

void xImpl_CMov::operator()(const xRegister16or32or64 &to, const xIndirectVoid &sibsrc) const
{
//    temp ← SRC
//    IF condition TRUE
//              THEN
//                  DEST ← temp;
//          FI;
//    ELSE
//          IF (OperandSize = 32 and IA-32e mode active)
//              THEN
//                  DEST[63:32] ← 0;
//          FI;
//    FI;

    ccSane();

    auto l_CCFlag = ComparisonTypeToCCFlag(ccType);

    Arm64Gen::ARM64Reg lsrcReg = Arm64JitConstants::SCRATCH2_32;

    if(to->IsWide())
        lsrcReg = Arm64JitConstants::SCRATCH2_64;

    if(sibsrc.Base.IsEmpty() && sibsrc.Index.IsEmpty() && sibsrc.Displacement != 0)
    {
        g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH1_64, sibsrc.Displacement);

        g_Emitter.LDR(lsrcReg, Arm64JitConstants::SCRATCH1_64);

        g_Emitter.CSEL(getReg(to), lsrcReg, getReg(to), l_CCFlag);

    } else {
        throw "Unimplemeneted!!!";
    }
}

//void xImpl_CMov::operator()( const xDirectOrIndirect32& to, const xDirectOrIndirect32& from ) const { ccSane(); _DoI_helpermess( *this, to, from ); }
//void xImpl_CMov::operator()( const xDirectOrIndirect16& to, const xDirectOrIndirect16& from ) const { ccSane(); _DoI_helpermess( *this, to, from ); }

void xImpl_Set::operator()(const xRegister8 &to) const
{
    ccSane();
//    IF condition
//    THEN DEST ← 1;
//    ELSE DEST ← 0;
//    FI;
    auto l_CCFlag = ComparisonTypeToCCFlag(ccType);

//    g_Emitter.MOVZ(getXReg(to), 1);
//
//    g_Emitter.CSEL(getXReg(to), getXReg(to), Arm64Gen::ZR, l_CCFlag);



    g_Emitter.MOVZ(Arm64JitConstants::SCRATCH1_64, 1);

    g_Emitter.CSEL(Arm64JitConstants::SCRATCH1_64, Arm64JitConstants::SCRATCH1_64, Arm64Gen::ZR, l_CCFlag);

    g_Emitter.BFI(getXReg(to), Arm64JitConstants::SCRATCH1_64, 0, 8);
}
void xImpl_Set::operator()(const xIndirect8 &dest) const
{
    ccSane();
    xOpWrite0F(0x90 | ccType, 0, dest);
}
//void xImpl_Set::operator()( const xDirectOrIndirect8& dest ) const		{ ccSane(); _DoI_helpermess( *this, dest ); }

void xImpl_MovExtend::operator()(const xRegister16or32or64 &to, const xRegister8 &from) const
{
    if(to->GetOperandSize() == 2)
    {
        throw "Unimplemeneted!!!";
    }
    else
    {
        if(SignExtend)
        {
            g_Emitter.SXTB(
                    getReg(to),
                    getWReg(from));
        } else
        {
            g_Emitter.UXTB(
                    getReg(to),
                    getWReg(from));
        }
    }
}

void xImpl_MovExtend::operator()(const xRegister16or32or64 &to, const xIndirect8 &sibsrc) const
{
    if(to->GetOperandSize() == 2)
    {
        throw "Unimplemeneted!!!";
    }
    else
    {
        if(sibsrc.Base.IsEmpty() && !sibsrc.Index.IsEmpty() && sibsrc.Displacement == 0)
        {
            g_Emitter.LDR(Arm64JitConstants::SCRATCH1_32, getXReg(sibsrc.Index));

            if(SignExtend)
            {
                g_Emitter.SXTB(
                        getReg(to),
                        Arm64JitConstants::SCRATCH1_32);
            } else
            {
                g_Emitter.UXTB(
                        getReg(to),
                        Arm64JitConstants::SCRATCH1_32);
            }
        } else {
            throw "Unimplemeneted!!!";
        }
    }
}

void xImpl_MovExtend::operator()(const xRegister32or64 &to, const xRegister16 &from) const
{
    if(SignExtend)
    {
        g_Emitter.SXTH(
                getReg(to),
                getWReg(from));
    } else
    {
        g_Emitter.UXTH(
                getReg(to),
                getWReg(from));
    }
}

void xImpl_MovExtend::operator()(const xRegister32or64 &to, const xIndirect16 &sibsrc) const
{
    if(to->GetOperandSize() == 2)
    {
        throw "Unimplemeneted!!!";
    }
    else
    {
        if(sibsrc.Base.IsEmpty() && !sibsrc.Index.IsEmpty() && sibsrc.Displacement == 0)
        {
            g_Emitter.LDR(Arm64JitConstants::SCRATCH1_32, getXReg(sibsrc.Index));

            if(SignExtend)
            {
                g_Emitter.SXTH(
                        getReg(to),
                        Arm64JitConstants::SCRATCH1_32);
            } else
            {
                g_Emitter.UXTH(
                        getReg(to),
                        Arm64JitConstants::SCRATCH1_32);
            }
        } else {
            throw "Unimplemeneted!!!";
        }
    }
}

#if 0
void xImpl_MovExtend::operator()( const xRegister32& to, const xDirectOrIndirect16& src ) const
{
	EbpAssert();
	_DoI_helpermess( *this, to, src );
}

void xImpl_MovExtend::operator()( const xRegister16or32& to, const xDirectOrIndirect8& src ) const
{
	EbpAssert();
	_DoI_helpermess( *this, to, src );
}
#endif

const xImpl_MovExtend xMOVSX = {true};
const xImpl_MovExtend xMOVZX = {false};

const xImpl_CMov xCMOVA = {Jcc_Above};
const xImpl_CMov xCMOVAE = {Jcc_AboveOrEqual};
const xImpl_CMov xCMOVB = {Jcc_Below};
const xImpl_CMov xCMOVBE = {Jcc_BelowOrEqual};

const xImpl_CMov xCMOVG = {Jcc_Greater};
const xImpl_CMov xCMOVGE = {Jcc_GreaterOrEqual};
const xImpl_CMov xCMOVL = {Jcc_Less};
const xImpl_CMov xCMOVLE = {Jcc_LessOrEqual};

const xImpl_CMov xCMOVZ = {Jcc_Zero};
const xImpl_CMov xCMOVE = {Jcc_Equal};
const xImpl_CMov xCMOVNZ = {Jcc_NotZero};
const xImpl_CMov xCMOVNE = {Jcc_NotEqual};

const xImpl_CMov xCMOVO = {Jcc_Overflow};
const xImpl_CMov xCMOVNO = {Jcc_NotOverflow};
const xImpl_CMov xCMOVC = {Jcc_Carry};
const xImpl_CMov xCMOVNC = {Jcc_NotCarry};

const xImpl_CMov xCMOVS = {Jcc_Signed};
const xImpl_CMov xCMOVNS = {Jcc_Unsigned};
const xImpl_CMov xCMOVPE = {Jcc_ParityEven};
const xImpl_CMov xCMOVPO = {Jcc_ParityOdd};


const xImpl_Set xSETA = {Jcc_Above};
const xImpl_Set xSETAE = {Jcc_AboveOrEqual};
const xImpl_Set xSETB = {Jcc_Below};
const xImpl_Set xSETBE = {Jcc_BelowOrEqual};

const xImpl_Set xSETG = {Jcc_Greater};
const xImpl_Set xSETGE = {Jcc_GreaterOrEqual};
const xImpl_Set xSETL = {Jcc_Less};
const xImpl_Set xSETLE = {Jcc_LessOrEqual};

const xImpl_Set xSETZ = {Jcc_Zero};
const xImpl_Set xSETE = {Jcc_Equal};
const xImpl_Set xSETNZ = {Jcc_NotZero};
const xImpl_Set xSETNE = {Jcc_NotEqual};

const xImpl_Set xSETO = {Jcc_Overflow};
const xImpl_Set xSETNO = {Jcc_NotOverflow};
const xImpl_Set xSETC = {Jcc_Carry};
const xImpl_Set xSETNC = {Jcc_NotCarry};

const xImpl_Set xSETS = {Jcc_Signed};
const xImpl_Set xSETNS = {Jcc_Unsigned};
const xImpl_Set xSETPE = {Jcc_ParityEven};
const xImpl_Set xSETPO = {Jcc_ParityOdd};

} // end namespace x86Emitter
