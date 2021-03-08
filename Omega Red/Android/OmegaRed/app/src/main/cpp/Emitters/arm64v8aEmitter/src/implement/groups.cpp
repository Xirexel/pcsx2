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

namespace x86Emitter
{

// =====================================================================================================
//  Group 1 Instructions - ADD, SUB, ADC, etc.
// =====================================================================================================

// Note on "[Indirect],Imm" forms : use int as the source operand since it's "reasonably inert" from a
// compiler perspective.  (using uint tends to make the compiler try and fail to match signed immediates
// with one of the other overloads).
    static void _g1_IndirectImm(G1Type InstType, const xIndirect64orLess &sibdest, int imm)
    {
        if (sibdest.Is8BitOp()) {
            throw L"Unimplemented!!!";
            xOpWrite(sibdest.GetPrefix16(), 0x80, InstType, sibdest);

            xWrite<s8>(imm);
        } else {

            Arm64Gen::ARM64Reg l_base = Arm64JitConstants::SCRATCH2_64;

            if(sibdest.Base.IsEmpty() && sibdest.Index.IsEmpty() && sibdest.Displacement != 0)
            {
                g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH1_64, sibdest.Displacement);

                switch (sibdest.DataType) {

                    case DataType::Word:
                    {
                        g_Emitter.LDR(Arm64JitConstants::SCRATCH2_32, Arm64JitConstants::SCRATCH1_64);
                        l_base = Arm64JitConstants::SCRATCH2_32;
                    }
                        break;

                    default:
                        throw L"Unimplemented!!!";
                        break;
                }

            }
            else
                throw L"Unimplemented!!!";

            g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH0_64, imm);

            if(InstType == G1Type::G1Type_CMP)
            {
                g_Emitter.CMP(l_base, Arm64JitConstants::SCRATCH0_64);
            } else
            {
                if(InstType == G1Type_SUB){
                    g_Emitter.SUBS(l_base, l_base, Arm64JitConstants::SCRATCH0_64);
                } else if(InstType == G1Type_AND){
                    g_Emitter.ANDS(l_base, l_base, Arm64JitConstants::SCRATCH0_64);
                } else if(InstType == G1Type_ADD){
                    g_Emitter.ADDS(l_base, l_base, Arm64JitConstants::SCRATCH0_64);
                } else if(InstType == G1Type_OR){
                    g_Emitter.ORR(l_base, l_base, Arm64JitConstants::SCRATCH0_64);
                }  else{
                    throw L"Unimplemented!!!";
                }

                g_Emitter.STR(l_base, Arm64JitConstants::SCRATCH1_64);
            }
        }
    }

void _g1_EmitOp(G1Type InstType, const xRegisterInt &to, const xRegisterInt &from)
{
    pxAssert(to.GetOperandSize() == from.GetOperandSize());

    if(InstType == G1Type_SUB && !to.Is8BitOp()){
        g_Emitter.SUBS(getReg(to), getReg(to), getReg(from));
    } else if(InstType == G1Type_ADD && !to.Is8BitOp()){
        g_Emitter.ADDS(getReg(to), getReg(to), getReg(from));
    } else if(InstType == G1Type_XOR && !to.Is8BitOp()){
        g_Emitter.EOR(getReg(to), getReg(to), getReg(from));
    } else if(InstType == G1Type_SBB && !to.Is8BitOp()){
        // DEST ← (DEST – (SRC + CF));
        g_Emitter.CSINC(Arm64JitConstants::SCRATCH1_32, getReg(from), getReg(from), CCFlags::CC_CS); // if (cond == true) getReg(to) = getReg(to), else getReg(to) = (getReg(to) + #1)
        g_Emitter.SUBS(getReg(to), getReg(to), Arm64JitConstants::SCRATCH1_32);
    } else{
        throw L"Unimplemented!!!";
    }

}

static void _g1_EmitOp(G1Type InstType, const xIndirectVoid &sibdest, const xRegisterInt &from)
{

    Arm64Gen::ARM64Reg l_base = Arm64JitConstants::SCRATCH2_64;

    if(sibdest.Base.IsEmpty() && sibdest.Index.IsEmpty() && sibdest.Displacement != 0)
    {
        g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH1_64, sibdest.Displacement);

        switch (sibdest.DataType) {

            case DataType::Word:
            {
                g_Emitter.LDR(Arm64JitConstants::SCRATCH2_32, Arm64JitConstants::SCRATCH1_64);
                l_base = Arm64JitConstants::SCRATCH2_32;
            }
                break;

            case DataType::Double_Word:
            {
                g_Emitter.LDR(Arm64JitConstants::SCRATCH2_64, Arm64JitConstants::SCRATCH1_64);
                l_base = Arm64JitConstants::SCRATCH2_64;
            }
                break;

            default:
                throw L"Unimplemented!!!";
                break;
        }

    }
    else
        throw L"Unimplemented!!!";

    if(InstType == G1Type::G1Type_CMP)
    {
        throw L"Unimplemented!!!";
    } else
    {
        if(InstType == G1Type_OR){
            g_Emitter.ORR(l_base, l_base, getReg(from));
        }  else if(InstType == G1Type_AND){
            g_Emitter.ANDS(l_base, l_base, getReg(from));
        }  else if(InstType == G1Type_XOR){
            g_Emitter.EOR(l_base, l_base, getReg(from));
        }  else if(InstType == G1Type_ADD){
            g_Emitter.ADDS(l_base, l_base, getReg(from));
        }  else if(InstType == G1Type_SUB){
            g_Emitter.SUBS(l_base, l_base, getReg(from));
        }  else{
            throw L"Unimplemented!!!";
        }

        g_Emitter.STR(l_base, Arm64JitConstants::SCRATCH1_64);
    }
}

static void _g1_EmitOp(G1Type InstType, const xRegisterInt &to, const xIndirectVoid &sibsrc)
{
    pxAssert(to.GetOperandSize() == sibsrc.GetOperandSize());

    if (sibsrc.Index.IsEmpty() && sibsrc.Base.IsEmpty() && sibsrc.Displacement != 0) {

        g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, sibsrc.Displacement);

        switch (to.GetOperandSize()) {
            case 4:
                g_Emitter.LDR(Arm64JitConstants::SCRATCH1_32, Arm64JitConstants::SCRATCH2_64);
                break;
            case 8:
                g_Emitter.LDR(Arm64JitConstants::SCRATCH1_64, Arm64JitConstants::SCRATCH2_64);
                break;
            default:
                throw L"Unimplemented!!!";
                break;
        }
    }
    else
        throw L"Unimplemented!!!";

    if(InstType == G1Type_SUB && !to.Is8BitOp() && to.GetOperandSize() == 4){
        g_Emitter.SUBS(getWReg(to), getWReg(to), Arm64JitConstants::SCRATCH1_32);
    } else if(InstType == G1Type_CMP && !to.Is8BitOp() && to.GetOperandSize() == 4){
        g_Emitter.CMP(getWReg(to), Arm64JitConstants::SCRATCH1_32);
    } else if(InstType == G1Type_ADD && !to.Is8BitOp() && to.GetOperandSize() == 4){
        g_Emitter.ADDS(getWReg(to), getWReg(to), Arm64JitConstants::SCRATCH1_32);
    } else if(InstType == G1Type_OR){
        g_Emitter.ORR(getWReg(to), getWReg(to), Arm64JitConstants::SCRATCH1_32);
    }  else if(InstType == G1Type_AND){
        g_Emitter.ANDS(getWReg(to), getWReg(to), Arm64JitConstants::SCRATCH1_32);
    }  else if(InstType == G1Type_XOR){
        g_Emitter.EOR(getWReg(to), getWReg(to), Arm64JitConstants::SCRATCH1_32);
    }
    else{
        throw L"Unimplemented!!!";
    }
}

static void _g1_EmitOp(G1Type InstType, const xRegisterInt &to, int imm)
{
    if(InstType == G1Type_CMP)
    {
        if (!to.Is8BitOp() && is_s8(imm))
        {
            g_Emitter.CMP(getWReg(to),  imm, false);
        } else{
            g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_32, imm);

            g_Emitter.CMP(getWReg(to),  Arm64JitConstants::SCRATCH2_32);
        }
    }
    else
    {
        g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH2_64, imm);

        if(InstType == G1Type_SUB && to.GetOperandSize() == 4){

            g_Emitter.SUBS(getWReg(to), getWReg(to), Arm64JitConstants::SCRATCH2_64);

        } else if(InstType == G1Type_ADD && to.GetOperandSize() == 4){

            g_Emitter.ADDS(getWReg(to), getWReg(to), Arm64JitConstants::SCRATCH2_64);

        } else if(InstType == G1Type_AND && to.GetOperandSize() == 4){

            g_Emitter.ANDS(getWReg(to), getWReg(to), Arm64JitConstants::SCRATCH2_64);

        } else{
            throw L"Unimplemented!!!";
        }
    }
}

#define ImplementGroup1(g1type, insttype)                                                                                \
    void g1type::operator()(const xRegisterInt &to, const xRegisterInt &from) const { _g1_EmitOp(insttype, to, from); }  \
    void g1type::operator()(const xIndirectVoid &to, const xRegisterInt &from) const { _g1_EmitOp(insttype, to, from); } \
    void g1type::operator()(const xRegisterInt &to, const xIndirectVoid &from) const { _g1_EmitOp(insttype, to, from); } \
    void g1type::operator()(const xRegisterInt &to, int imm) const { _g1_EmitOp(insttype, to, imm); }                    \
    void g1type::operator()(const xIndirect64orLess &sibdest, int imm) const { _g1_IndirectImm(insttype, sibdest, imm); }

ImplementGroup1(xImpl_Group1, InstType)
    ImplementGroup1(xImpl_G1Logic, InstType)
        ImplementGroup1(xImpl_G1Arith, InstType)
            ImplementGroup1(xImpl_G1Compare, G1Type_CMP)

                const xImpl_G1Logic xAND = {G1Type_AND, {0x00, 0x54}, {0x66, 0x54}};
const xImpl_G1Logic xOR = {G1Type_OR, {0x00, 0x56}, {0x66, 0x56}};
const xImpl_G1Logic xXOR = {G1Type_XOR, {0x00, 0x57}, {0x66, 0x57}};

const xImpl_G1Arith xADD = {G1Type_ADD, {0x00, 0x58}, {0x66, 0x58}, {0xf3, 0x58}, {0xf2, 0x58}};
const xImpl_G1Arith xSUB = {G1Type_SUB, {0x00, 0x5c}, {0x66, 0x5c}, {0xf3, 0x5c}, {0xf2, 0x5c}};
const xImpl_G1Compare xCMP = {{0x00, 0xc2}, {0x66, 0xc2}, {0xf3, 0xc2}, {0xf2, 0xc2}};

const xImpl_Group1 xADC = {G1Type_ADC};
const xImpl_Group1 xSBB = {G1Type_SBB};

// =====================================================================================================
//  Group 2 Instructions - SHR, SHL, etc.
// =====================================================================================================

void xImpl_Group2::operator()(const xRegisterInt &to, const xRegisterCL & /* from */) const
{
    xOpWrite(to.GetPrefix16(), to.Is8BitOp() ? 0xd2 : 0xd3, InstType, to);
}

void xImpl_Group2::operator()(const xRegisterInt &to, u8 imm) const
{
    if (imm == 0)
        return;

    Arm64Gen::ARM64Reg l_reg = getReg(to);

    if(InstType == G2Type_SAR)
        g_Emitter.ASR(l_reg,
                      l_reg,
                      imm);
    else if(InstType == G2Type_SHR)
        g_Emitter.LSR(l_reg,
                      l_reg,
                      imm);
    else if(InstType == G2Type_SHL)
        g_Emitter.LSL(l_reg,
                      l_reg,
                      imm);
    else
        throw L"Unimplemented!!!";
}

void xImpl_Group2::operator()(const xIndirect64orLess &sibdest, const xRegisterCL & /* from */) const
{
    xOpWrite(sibdest.GetPrefix16(), sibdest.Is8BitOp() ? 0xd2 : 0xd3, InstType, sibdest);
}

void xImpl_Group2::operator()(const xIndirect64orLess &sibdest, u8 imm) const
{
    if (imm == 0)
        return;


    Arm64Gen::ARM64Reg l_base = Arm64JitConstants::SCRATCH2_64;

    if(sibdest.Base.IsEmpty() && sibdest.Index.IsEmpty() && sibdest.Displacement != 0)
    {
        g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH1_64, sibdest.Displacement);

        switch (sibdest.DataType) {

            case DataType::Word:
            {
                g_Emitter.LDR(Arm64JitConstants::SCRATCH2_32, Arm64JitConstants::SCRATCH1_64);
                l_base = Arm64JitConstants::SCRATCH2_32;
            }
                break;

            default:
                throw L"Unimplemented!!!";
                break;
        }

    }
    else
        throw L"Unimplemented!!!";

    switch (InstType) {

        case G2Type_SHL:
            g_Emitter.LSL(l_base, l_base, imm);
            break;

        case G2Type_SHR:
            g_Emitter.LSR(l_base, l_base, imm);
            break;

        case G2Type_SAR:
            g_Emitter.ASR(l_base, l_base, imm);
            break;

        default:
            throw L"Unimplemented!!!";
            break;
    }


    g_Emitter.STR(l_base, Arm64JitConstants::SCRATCH1_64);
}

const xImpl_Group2 xROL = {G2Type_ROL};
const xImpl_Group2 xROR = {G2Type_ROR};
const xImpl_Group2 xRCL = {G2Type_RCL};
const xImpl_Group2 xRCR = {G2Type_RCR};
const xImpl_Group2 xSHL = {G2Type_SHL};
const xImpl_Group2 xSHR = {G2Type_SHR};
const xImpl_Group2 xSAR = {G2Type_SAR};


// =====================================================================================================
//  Group 3 Instructions - NOT, NEG, MUL, DIV
// =====================================================================================================

static void _g3_EmitOp(G3Type InstType, const xRegisterInt &from)
{
    if(InstType == G3Type::G3Type_NOT)
    {
        g_Emitter.MVN(getReg(from), getReg(from));
    } else if(InstType == G3Type::G3Type_NEG)
    {
//        IF DEST = 0
//        THEN CF ← 0;
//        ELSE CF ← 1;
//        FI;
//        DEST ← [– (DEST)]

        g_Emitter.SUBS(getReg(from), Arm64Gen::ZR, getReg(from));
    }else if(InstType == G3Type::G3Type_DIV)
    {
//if(Source == 0) Exception(DE); //divide error
//
//if(OperandSize == 8) { //word/byte operation
//	Temporary = AX / Source;
//	if(Temporary > 0xFF) Exception(DE); //divide error
//	else {
//		AL = Temporary;
//		AH = AX % Source;
//	}
//}
//else if(OperandSize == 16) { //doubleword/word operation
//	Temporary = DX:AX / Source;
//	if(Temporary > 0xFFFF) Exception(DE); //divide error
//	else {
//		AX = Temporary;
//		DX = DX:AX % Source;
//	}
//}
//else { //quadword/doubleword operation
//	Temporary = EDX:EAX / Source;
//	if(Temporary > 0xFFFFFFFF) Exception(DE); //divide error
//	else {
//		EAX = Temporary;
//		EDX = EDX:EAX % Source;
//	}
//}

        if(from.GetOperandSize() != 4)
            throw L"Unimplemented!!!";

        g_Emitter.BFI(getXReg(eax), getXReg(edx), 32, 32);

        g_Emitter.MOV(Arm64JitConstants::SCRATCH0_64, Arm64Gen::ZR);

        g_Emitter.BFI(Arm64JitConstants::SCRATCH0_64, getXReg(from), 0, 32);

        g_Emitter.UDIV(Arm64JitConstants::SCRATCH1_64, getXReg(eax), Arm64JitConstants::SCRATCH0_64); // u = (v / 5)

        // v = (v - (u * 5))
        g_Emitter.UMULL(Arm64JitConstants::SCRATCH2_64, Arm64JitConstants::SCRATCH1_64, Arm64JitConstants::SCRATCH0_64);

        g_Emitter.SUB(getReg(edx), getReg(eax), Arm64JitConstants::SCRATCH2_32);

        g_Emitter.MOV(getReg(eax), Arm64JitConstants::SCRATCH1_32);

    }else if(InstType == G3Type::G3Type_iDIV)
    {
//if(Source == 0) Exception(DE); //divide error
//
//if(OperandSize == 8) { //word/byte operation
//	Temporary = AX / Source;
//	if(Temporary > 0xFF) Exception(DE); //divide error
//	else {
//		AL = Temporary;
//		AH = AX % Source;
//	}
//}
//else if(OperandSize == 16) { //doubleword/word operation
//	Temporary = DX:AX / Source;
//	if(Temporary > 0xFFFF) Exception(DE); //divide error
//	else {
//		AX = Temporary;
//		DX = DX:AX % Source;
//	}
//}
//else { //quadword/doubleword operation
//	Temporary = EDX:EAX / Source;
//	if(Temporary > 0xFFFFFFFF) Exception(DE); //divide error
//	else {
//		EAX = Temporary;
//		EDX = EDX:EAX % Source;
//	}
//}

        if(from.GetOperandSize() != 4)
            throw L"Unimplemented!!!";

        g_Emitter.BFI(getXReg(eax), getXReg(edx), 32, 32);

        g_Emitter.MOV(Arm64JitConstants::SCRATCH0_64, Arm64Gen::ZR);

        g_Emitter.SXTH(Arm64JitConstants::SCRATCH0_64, getReg(from));

        g_Emitter.SDIV(Arm64JitConstants::SCRATCH1_64, getXReg(eax), Arm64JitConstants::SCRATCH0_64); // u = (v / 5)

        // v = (v - (u * 5))
        g_Emitter.SMULL(Arm64JitConstants::SCRATCH2_64, Arm64JitConstants::SCRATCH1_64, Arm64JitConstants::SCRATCH0_64);

        g_Emitter.SUB(getReg(edx), getReg(eax), Arm64JitConstants::SCRATCH2_32);

        g_Emitter.MOV(getReg(eax), Arm64JitConstants::SCRATCH1_32);

    }
    else
        xOpWrite(from.GetPrefix16(), from.Is8BitOp() ? 0xf6 : 0xf7, InstType, from);
}

static void _g3_EmitOp(G3Type InstType, const xIndirect64orLess &from)
{
    Arm64Gen::ARM64Reg src = Arm64JitConstants::SCRATCH0_64;

    Arm64Gen::ARM64Reg arg = Arm64JitConstants::SCRATCH1_64;

    setAddress(src, from);

    switch (from.DataType) {

        case Word:
            arg = Arm64JitConstants::SCRATCH1_32;

            g_Emitter.LDR(arg, src);

            break;

        default:
            throw L"Unimplemented!!!";
    }

    if(InstType == G3Type_iMUL)
    {
        g_Emitter.SMULL(Arm64JitConstants::SCRATCH2_64, getReg(eax), arg);

        g_Emitter.MOV(getXReg(eax), Arm64Gen::ZR);

        g_Emitter.BFI(getReg(eax), Arm64JitConstants::SCRATCH2_64, 0, 32);

        g_Emitter.MOV(getXReg(edx), Arm64Gen::ZR);

        g_Emitter.ASR(getXReg(edx), Arm64JitConstants::SCRATCH2_64, 32);
    }
    else
        xOpWrite(from.GetPrefix16(), from.Is8BitOp() ? 0xf6 : 0xf7, InstType, from);
}

void xImpl_Group3::operator()(const xRegisterInt &from) const { _g3_EmitOp(InstType, from); }
void xImpl_Group3::operator()(const xIndirect64orLess &from) const { _g3_EmitOp(InstType, from); }

void xImpl_iDiv::operator()(const xRegisterInt &from) const { _g3_EmitOp(G3Type_iDIV, from); }
void xImpl_iDiv::operator()(const xIndirect64orLess &from) const { _g3_EmitOp(G3Type_iDIV, from); }

template <typename SrcType>
static void _imul_ImmStyle(const xRegisterInt &param1, const SrcType &param2, int imm)
{
    pxAssert(param1.GetOperandSize() == param2.GetOperandSize());

    xOpWrite0F(param1.GetPrefix16(), is_s8(imm) ? 0x6b : 0x69, param1, param2, is_s8(imm) ? 1 : param1.GetImmSize());

    if (is_s8(imm))
        xWrite8((u8)imm);
    else
        param1.xWriteImm(imm);
}

void xImpl_iMul::operator()(const xRegisterInt &from) const { _g3_EmitOp(G3Type_iMUL, from); }
void xImpl_iMul::operator()(const xIndirect64orLess &from) const { _g3_EmitOp(G3Type_iMUL, from); }

void xImpl_iMul::operator()(const xRegister32 &to, const xRegister32 &from) const { xOpWrite0F(0xaf, to, from); }
void xImpl_iMul::operator()(const xRegister32 &to, const xIndirectVoid &src) const { xOpWrite0F(0xaf, to, src); }
void xImpl_iMul::operator()(const xRegister16 &to, const xRegister16 &from) const { xOpWrite0F(0x66, 0xaf, to, from); }
void xImpl_iMul::operator()(const xRegister16 &to, const xIndirectVoid &src) const { xOpWrite0F(0x66, 0xaf, to, src); }

void xImpl_iMul::operator()(const xRegister32 &to, const xRegister32 &from, s32 imm) const { _imul_ImmStyle(to, from, imm); }
void xImpl_iMul::operator()(const xRegister32 &to, const xIndirectVoid &from, s32 imm) const { _imul_ImmStyle(to, from, imm); }
void xImpl_iMul::operator()(const xRegister16 &to, const xRegister16 &from, s16 imm) const { _imul_ImmStyle(to, from, imm); }
void xImpl_iMul::operator()(const xRegister16 &to, const xIndirectVoid &from, s16 imm) const { _imul_ImmStyle(to, from, imm); }

const xImpl_Group3 xNOT = {G3Type_NOT};
const xImpl_Group3 xNEG = {G3Type_NEG};
const xImpl_Group3 xUMUL = {G3Type_MUL};
const xImpl_Group3 xUDIV = {G3Type_DIV};

const xImpl_iDiv xDIV = {{0x00, 0x5e}, {0x66, 0x5e}, {0xf3, 0x5e}, {0xf2, 0x5e}};
const xImpl_iMul xMUL = {{0x00, 0x59}, {0x66, 0x59}, {0xf3, 0x59}, {0xf2, 0x59}};

// =====================================================================================================
//  Group 8 Instructions
// =====================================================================================================

void xImpl_Group8::operator()(const xRegister16or32or64 &bitbase, const xRegister16or32or64 &bitoffset) const
{
    pxAssert(bitbase->GetOperandSize() == bitoffset->GetOperandSize());
    xOpWrite0F(bitbase->GetPrefix16(), 0xa3 | (InstType << 3), bitbase, bitoffset);
}
void xImpl_Group8::operator()(const xIndirect64 &bitbase, u8 bitoffset) const { xOpWrite0F(0xba, InstType, bitbase, bitoffset); }
void xImpl_Group8::operator()(const xIndirect32 &bitbase, u8 bitoffset) const { xOpWrite0F(0xba, InstType, bitbase, bitoffset); }
void xImpl_Group8::operator()(const xIndirect16 &bitbase, u8 bitoffset) const { xOpWrite0F(0x66, 0xba, InstType, bitbase, bitoffset); }

void xImpl_Group8::operator()(const xRegister16or32or64 &bitbase, u8 bitoffset) const
{
    xOpWrite0F(bitbase->GetPrefix16(), 0xba, InstType, bitbase, bitoffset);
}

void xImpl_Group8::operator()(const xIndirectVoid &bitbase, const xRegister16or32or64 &bitoffset) const
{
    xOpWrite0F(bitoffset->GetPrefix16(), 0xa3 | (InstType << 3), bitoffset, bitbase);
}

const xImpl_Group8 xBT = {G8Type_BT};
const xImpl_Group8 xBTR = {G8Type_BTR};
const xImpl_Group8 xBTS = {G8Type_BTS};
const xImpl_Group8 xBTC = {G8Type_BTC};



} // End namespace x86Emitter
