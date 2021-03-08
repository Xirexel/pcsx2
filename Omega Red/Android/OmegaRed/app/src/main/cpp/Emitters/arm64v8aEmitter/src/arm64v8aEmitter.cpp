//
// Created by evgen on 9/8/2020.
//


#include "PrecompiledHeader.h"
#include "tools.h"
#include "types.h"
#include "../include/Arm64Emitter.h"

//#include "implement/implement.h"

//__tls_emit u8 *x86Ptr;
__tls_emit XMMSSEType g_xmmtypes[iREGCNT_XMM] = {XMMT_INT};

Arm64Gen::ARM64XEmitter g_Emitter;
Arm64Gen::ARM64FloatEmitter g_fp(&g_Emitter);

static u128 xmmData[17];

Arm64Gen::ARM64Reg getWReg(const uint regfield)
{
    return (Arm64Gen::ARM64Reg)(Arm64Gen::ARM64Reg::W0 + regfield);
}

Arm64Gen::ARM64Reg getXReg(const uint regfield)
{
    return (Arm64Gen::ARM64Reg)(Arm64Gen::ARM64Reg::X0 + regfield);
}

Arm64Gen::ARM64Reg getWReg(const x86Emitter::xRegisterInt &reg)
{
    return getWReg(reg.GetId());
}

Arm64Gen::ARM64Reg getXReg(const x86Emitter::xRegisterInt &reg)
{
    return getXReg(reg.GetId());
}

Arm64Gen::ARM64Reg getReg(const x86Emitter::xRegisterBase &to)
{
    Arm64Gen::ARM64Reg l_reg;

    if(to.IsWide())
        l_reg = getXReg(to.GetId());
    else
    {
        if(to.GetOperandSize() != 4)
        {
            if(to.GetOperandSize() != 2)
                throw L"Unimplemented!!!";

            g_Emitter.SXTH(getWReg(to.GetId()), getWReg(to.GetId()));
        }

        l_reg = getWReg(to.GetId());
    }

    return l_reg;
}


void setAddress(Arm64Gen::ARM64Reg& target, const x86Emitter::xIndirectVoid &from)
{
    if(target < Arm64Gen::ARM64Reg::X0 || target > Arm64Gen::ARM64Reg::X17)
        throw L"Unimplemented!!!";

    if (from.Index.IsEmpty() && from.Base.IsEmpty() && from.Displacement != 0) {

        g_Emitter.MOVI2R(target, from.Displacement);

    } else if (!from.Index.IsEmpty() && from.Base.IsEmpty() && from.Scale == 0) {

        if(from.Displacement != 0)
        {
            g_Emitter.MOVI2R(target, from.Displacement);

            g_Emitter.ADD(target, target, getReg(from.Index));
        }
        else
            target = getReg(from.Index);

    }else {
        throw L"Unimplemented!!!";
    }
}


CCFlags ComparisonTypeToCCFlag(x86Emitter::JccComparisonType cctype)
{
//        enum JccComparisonType {
//            Jcc_Unknown = -2,
//            Jcc_Unconditional = -1,
//            Jcc_Overflow = 0x0,
//            Jcc_NotOverflow = 0x1,
//            Jcc_Below = 0x2,
//            Jcc_Carry = 0x2,
//            Jcc_AboveOrEqual = 0x3,
//            Jcc_NotCarry = 0x3,
//            Jcc_Zero = 0x4,
//            Jcc_Equal = 0x4,
//            Jcc_NotZero = 0x5,
//            Jcc_NotEqual = 0x5,
//            Jcc_BelowOrEqual = 0x6,
//            Jcc_Above = 0x7,
//            Jcc_Signed = 0x8,
//            Jcc_Unsigned = 0x9,
//            Jcc_ParityEven = 0xa,
//            Jcc_ParityOdd = 0xb,
//            Jcc_Less = 0xc,
//            Jcc_GreaterOrEqual = 0xd,
//            Jcc_LessOrEqual = 0xe,
//            Jcc_Greater = 0xf,
//        };


//        enum CCFlags
//        {
//            CC_EQ = 0, // Equal
//            CC_NEQ, // Not equal
//            CC_CS, // Carry Set
//            CC_CC, // Carry Clear
//            CC_MI, // Minus (Negative)
//            CC_PL, // Plus
//            CC_VS, // Overflow
//            CC_VC, // No Overflow
//            CC_HI, // Unsigned higher
//            CC_LS, // Unsigned lower or same
//            CC_GE, // Signed greater than or equal
//            CC_LT, // Signed less than
//            CC_GT, // Signed greater than
//            CC_LE, // Signed less than or equal
//            CC_AL, // Always (unconditional) 14
//            CC_HS = CC_CS, // Alias of CC_CS  Unsigned higher or same
//            CC_LO = CC_CC, // Alias of CC_CC  Unsigned lower
//        };

    using namespace x86Emitter;

    CCFlags l_CCFlags = CCFlags::CC_AL;

    switch(cctype)
    {
        case Jcc_Less:
            l_CCFlags = CCFlags::CC_LT;
            break;

        case Jcc_Unconditional:
            l_CCFlags = CCFlags::CC_AL;
            break;

        case Jcc_Equal:
            l_CCFlags = CCFlags::CC_EQ;
            break;

        case Jcc_Greater:
            l_CCFlags = CCFlags::CC_GT;
            break;

        case Jcc_Below:
            l_CCFlags = CCFlags::CC_LO;
            break;

        case Jcc_NotEqual:
            l_CCFlags = CCFlags::CC_NEQ;
            break;

        case Jcc_Signed:
            l_CCFlags = CCFlags::CC_MI;
            break;

        case Jcc_LessOrEqual:
            l_CCFlags = CCFlags::CC_LE;
            break;

        case Jcc_Unsigned:
            l_CCFlags = CCFlags::CC_PL;
            break;

        case Jcc_Above:
            l_CCFlags = CCFlags::CC_HI;
            break;


        default:
            throw L"Unimplemented!!!";
            break;
    }

    return l_CCFlags;
}

namespace x86Emitter
{
    void xWrite8(u8 val) {
        throw L"Unimplemented!!!";
    }
    void xWrite16(u16 val) {
        throw L"Unimplemented!!!";
    }
    void xWrite32(u32 val) {
        throw L"Unimplemented!!!";
    }
    void xWrite64(u64 val) {
        throw L"Unimplemented!!!";
    }


    void EmitSibMagic(uint regfield, const void *address, int extraRIPOffset = 0){
        throw L"Unimplemented!!!";
    }
    void EmitSibMagic(uint regfield, const xIndirectVoid &info, int extraRIPOffset = 0){
        throw L"Unimplemented!!!";
    }
    void EmitSibMagic(uint reg1, const xRegisterBase &reg2, int a1) {
        throw L"Unimplemented!!!";
    }
    void EmitSibMagic(const xRegisterBase &reg1, const xRegisterBase &reg2, int){
        throw L"Unimplemented!!!";
    }
    void EmitSibMagic(const xRegisterBase &reg1, const void *src, int extraRIPOffset){
        throw L"Unimplemented!!!";
    }
    void EmitSibMagic(const xRegisterBase &reg1, const xIndirectVoid &sib, int extraRIPOffset){
        throw L"Unimplemented!!!";
    }



    void EmitRex(uint regfield, const void *address){
        throw L"Unimplemented!!!";
    }
    void EmitRex(uint regfield, const xIndirectVoid &info){
        throw L"Unimplemented!!!";
    }
    void EmitRex(uint reg1, const xRegisterBase &reg2){
        throw L"Unimplemented!!!";
    }
    void EmitRex(const xRegisterBase &reg1, const xRegisterBase &reg2){
        throw L"Unimplemented!!!";
    }
    void EmitRex(const xRegisterBase &reg1, const void *src){
        throw L"Unimplemented!!!";
    }
    void EmitRex(const xRegisterBase &reg1, const xIndirectVoid &sib){
        throw L"Unimplemented!!!";
    }





    const u8* xRegisterSSE::getData()const{
        if(Id > 15)
            return (u8 *)(xmmData + 16);
        else if(Id < 0)
            return (u8 *)(xmmData + 16);
        else
            return (u8 *)(xmmData + Id);
    }




// --------------------------------------------------------------------------------------
//  xSetPtr / xAlignPtr / xGetPtr / xAdvancePtr
// --------------------------------------------------------------------------------------

// Assigns the current emitter buffer target address.
// This is provided instead of using x86Ptr directly, since we may in the future find
// a need to change the storage class system for the x86Ptr 'under the hood.'
    __emitinline void xSetPtr(void *ptr)
    {
        g_Emitter.SetCodePointer((u8 *)ptr, (u8 *)ptr);
    }

// Retrieves the current emitter buffer target address.
// This is provided instead of using x86Ptr directly, since we may in the future find
// a need to change the storage class system for the x86Ptr 'under the hood.'
    __emitinline u8 *xGetPtr()
    {
        return g_Emitter.GetWritableCodePtr();
    }

    __emitinline void xAlignPtr(uint bytes)
    {
        if(bytes != 16)
            throw L"Unimplemented!!!";

        g_Emitter.AlignCode16();

        // forward align
//        x86Ptr = (u8 *)(((uptr)x86Ptr + bytes - 1) & ~(bytes - 1));
    }

    __emitinline u8 *xGetAlignedCallTarget()
    {
        const u8 *start = g_Emitter.AlignCode16();
        u8* lPtr = xGetPtr();
        return lPtr;
    }

// Performs best-case alignment for the target CPU, for use prior to starting a new
// function.  This is not meant to be used prior to jump targets, since it doesn't
// add padding (additionally, speed benefit from jump alignment is minimal, and often
// a loss).
    __emitinline void xAlignCallTarget()
    {
        throw L"Unimplemented!!!";
        // Core2/i7 CPUs prefer unaligned addresses.  Checking for SSSE3 is a decent filter.
        // (also align in debug modes for disasm convenience)

        if (IsDebugBuild || !x86caps.hasSupplementalStreamingSIMD3Extensions) {
            // - P4's and earlier prefer 16 byte alignment.
            // - AMD Athlons and Phenoms prefer 8 byte alignment, but I don't have an easy
            //   heuristic for it yet.
            // - AMD Phenom IIs are unknown (either prefer 8 byte, or unaligned).

            xAlignPtr(16);
        }
    }

    __emitinline void xAdvancePtr(uint bytes)
    {
        throw L"Unimplemented!!!";
//        if (IsDevBuild) {
//            // common debugger courtesy: advance with INT3 as filler.
//            for (uint i = 0; i < bytes; i++)
//                xWrite8(0xcc);
//        } else
//            x86Ptr += bytes;
    }


// clang-format off

    const xRegisterSSE
            xmm0(0), xmm1(1),
            xmm2(2), xmm3(3),
            xmm4(4), xmm5(5),
            xmm6(6), xmm7(7),
            xmm8(8), xmm9(9),
            xmm10(10), xmm11(11),
            xmm12(12), xmm13(13),
            xmm14(14), xmm15(15);

    const xAddressReg
            rax(0), rbx(3),
            rcx(1), rdx(2),
            rsp(4), rbp(5),
            rsi(6), rdi(7),
            r8(8), r9(9),
            r10(10), r11(11),
            r12(12), r13(13),
            r14(14), r15(15);

    const xRegister32
            eax(0), ebx(3),
            ecx(1), edx(2),
            esp(4), ebp(5),
            esi(6), edi(7),
            r8d(8), r9d(9),
            r10d(10), r11d(11),
            r12d(12), r13d(13),
            r14d(14), r15d(15);

    const xRegister16
            ax(0), bx(3),
            cx(1), dx(2),
            sp(4), bp(5),
            si(6), di(7);

    const xRegister8
            al(0),
            dl(2), bl(3),
            ah(4), ch(5),
            dh(6), bh(7);

#if defined(__M_X86_64)
    const xAddressReg
        arg1reg = rcx,
        arg2reg = rdx,
#ifdef __M_X86_64
arg3reg = r8,
    arg4reg = r9,
#else
        arg3reg = xRegisterEmpty(),
        arg4reg = xRegisterEmpty(),
#endif
        calleeSavedReg1 = rdi,
        calleeSavedReg2 = rsi;

const xRegister32
        arg1regd = ecx,
        arg2regd = edx,
        calleeSavedReg1d = edi,
        calleeSavedReg2d = esi;
#else
    const xAddressReg
            arg1reg = rdi,
            arg2reg = rsi,
            arg3reg = rdx,
            arg4reg = rcx,
            calleeSavedReg1 = r12,
            calleeSavedReg2 = r13;

    const xRegister32
            arg1regd = edi,
            arg2regd = esi,
            calleeSavedReg1d = r12d,
            calleeSavedReg2d = r13d;
#endif

    const xRegisterCL cl;

// Empty initializers are due to frivolously pointless GCC errors (it demands the
// objects be initialized even though they have no actual variable members).

    const xAddressIndexer<xIndirectVoid> ptr = {};
    const xAddressIndexer<xIndirectNative> ptrNative = {};
    const xAddressIndexer<xIndirect128> ptr128 = {};
    const xAddressIndexer<xIndirect64> ptr64 = {};
    const xAddressIndexer<xIndirect32> ptr32 = {};
    const xAddressIndexer<xIndirect16> ptr16 = {};
    const xAddressIndexer<xIndirect8> ptr8 = {};

    const xRegisterEmpty xEmptyReg = {};


//////////////////////////////////////////////////////////////////////////////////////////


// ------------------------------------------------------------------------
// Internal implementation of EmitSibMagic which has been custom tailored
// to optimize special forms of the Lea instructions accordingly, such
// as when a LEA can be replaced with a "MOV reg,imm" or "MOV reg,reg".
//
// preserve_flags - set to ture to disable use of SHL on [Index*Base] forms
// of LEA, which alters flags states.
//
    static void EmitLeaMagic(const xRegisterInt &to, const xIndirectVoid &src, bool preserve_flags)
    {
        throw L"Unimplemented!!!";

//    int displacement_size = (src.Displacement == 0) ? 0 :
//                            ((src.IsByteSizeDisp()) ? 1 : 2);
//
//    // See EmitSibMagic for commenting on SIB encoding.
//
//    // We should allow native-sized addressing regs (e.g. lea eax, [rax])
//    const xRegisterInt& sizeMatchedIndex = to.IsWide() ? src.Index : src.Index.GetNonWide();
//    const xRegisterInt& sizeMatchedBase = to.IsWide() ? src.Base : src.Base.GetNonWide();
//
//    if (!NeedsSibMagic(src) && src.Displacement == (s32)src.Displacement) {
//        // LEA Land: means we have either 1-register encoding or just an offset.
//        // offset is encodable as an immediate MOV, and a register is encodable
//        // as a register MOV.
//
//        if (src.Index.IsEmpty()) {
//            xMOV(to, src.Displacement);
//            return;
//        }
//        else if (displacement_size == 0) {
//            _xMovRtoR(to, sizeMatchedIndex);
//            return;
//        } else if (!preserve_flags) {
//            // encode as MOV and ADD combo.  Make sure to use the immediate on the
//            // ADD since it can encode as an 8-bit sign-extended value.
//
//            _xMovRtoR(to, sizeMatchedIndex);
//            xADD(to, src.Displacement);
//            return;
//        }
//    } else {
//        if (src.Base.IsEmpty()) {
//            if (!preserve_flags && (displacement_size == 0)) {
//                // Encode [Index*Scale] as a combination of Mov and Shl.
//                // This is more efficient because of the bloated LEA format which requires
//                // a 32 bit displacement, and the compact nature of the alternative.
//                //
//                // (this does not apply to older model P4s with the broken barrel shifter,
//                //  but we currently aren't optimizing for that target anyway).
//
//                _xMovRtoR(to, src.Index);
//                xSHL(to, src.Scale);
//                return;
//            }
//        } else {
//            if (src.Scale == 0) {
//                if (!preserve_flags) {
//                    if (src.Index == rsp) {
//                        // ESP is not encodable as an index (ix86 ignores it), thus:
//                        _xMovRtoR(to, sizeMatchedBase); // will do the trick!
//                        if (src.Displacement)
//                            xADD(to, src.Displacement);
//                        return;
//                    } else if (src.Displacement == 0) {
//                        _xMovRtoR(to, sizeMatchedBase);
//                        _g1_EmitOp(G1Type_ADD, to, sizeMatchedIndex);
//                        return;
//                    }
//                } else if ((src.Index == rsp) && (src.Displacement == 0)) {
//                    // special case handling of ESP as Index, which is replaceable with
//                    // a single MOV even when preserve_flags is set! :D
//
//                    _xMovRtoR(to, sizeMatchedBase);
//                    return;
//                }
//            }
//        }
//    }
//
//    xOpWrite(0, 0x8d, to, src);
    }


    __emitinline void xLEA(xRegister64 to, const xIndirectVoid &src, bool preserve_flags)
    {
//    IF OperandSize = 16 and AddressSize = 16
//    THEN
//    DEST ← EffectiveAddress(SRC); (* 16-bit address *)
//    ELSE IF OperandSize = 16 and AddressSize = 32
//    THEN
//    temp ← EffectiveAddress(SRC); (* 32-bit address *)
//    DEST ← temp[0:15]; (* 16-bit address *)
//    FI;
//    ELSE IF OperandSize = 32 and AddressSize = 16
//    THEN
//    temp ← EffectiveAddress(SRC); (* 16-bit address *)
//    DEST ← ZeroExtend(temp); (* 32-bit address *)
//    FI;
//    ELSE IF OperandSize = 32 and AddressSize = 32
//    THEN
//    DEST ← EffectiveAddress(SRC); (* 32-bit address *)
//    FI;
//    ELSE IF OperandSize = 16 and AddressSize = 64
//    THEN
//    temp ← EffectiveAddress(SRC); (* 64-bit address *)
//    DEST ← temp[0:15]; (* 16-bit address *)
//    FI;
//    ELSE IF OperandSize = 32 and AddressSize = 64
//    THEN
//    temp ← EffectiveAddress(SRC); (* 64-bit address *)
//    DEST ← temp[0:31]; (* 16-bit address *)
//    FI;
//    ELSE IF OperandSize = 64 and AddressSize = 64
//    THEN
//    DEST ← EffectiveAddress(SRC); (* 64-bit address *)
//    FI;
//    FI;

        if(!(src.Index.IsEmpty() && src.Base.IsEmpty() && src.Displacement != 0))
            throw L"Unimplemented!!!";

        if(!to.IsWide())
            throw L"Unimplemented!!!";

        Arm64Gen::ARM64Reg Rt = getReg(to);

        setAddress(Rt, src);
    }

    __emitinline void xLEA(xRegister32 to, const xIndirectVoid &src, bool preserve_flags)
    {
        throw L"Unimplemented!!!";
        EmitLeaMagic(to, src, preserve_flags);
    }

    __emitinline void xLEA(xRegister16 to, const xIndirectVoid &src, bool preserve_flags)
    {
        throw L"Unimplemented!!!";
        xWrite8(0x66);
        EmitLeaMagic(to, src, preserve_flags);
    }

    __emitinline u32* xLEA_Writeback(xAddressReg to)
    {
        auto val = (u32*)xGetPtr();

        for (int i = 0; i < 4; ++i)
            g_Emitter.HINT(Arm64Gen::HINT_NOP);

        *val = getReg(to);

        return val;
    }

    __emitinline void vtlb_SetWriteback(u32 *writeback)
    {
        auto targetRegister = (Arm64Gen::ARM64Reg)(*writeback);

        auto targetPtr = xGetPtr();

        *writeback = 0xD503201F;

        g_Emitter.SetCodePointer((u8*)writeback, (u8*)writeback);

        g_Emitter.MOVI2R(targetRegister, (u64)targetPtr);

        g_Emitter.SetCodePointer(targetPtr, targetPtr);
    }





    xAddressVoid xComplexAddress(const xAddressReg& tmpRegister, void *base, const xAddressVoid& offset) {

        xLEA(tmpRegister, ptr[base]);

        if(offset.Base.IsEmpty() && !offset.Index.IsEmpty())
        {
            return offset + tmpRegister;
        }
        else
        {
            throw L"Unimplemented!!!";
        }
    }


// --------------------------------------------------------------------------------------
//  xAddressReg  (operator overloads)
// --------------------------------------------------------------------------------------
    xAddressVoid xAddressReg::operator+(const xAddressReg &right) const
    {
        throw L"Unimplemented!!!";
        pxAssertMsg(right.Id != -1 || Id != -1, "Uninitialized x86 register.");
        return xAddressVoid(*this, right);
    }

    xAddressVoid xAddressReg::operator+(sptr right) const
    {
        pxAssertMsg(Id != -1, "Uninitialized x86 register.");
        return xAddressVoid(*this, right);
    }

    xAddressVoid xAddressReg::operator+(const void *right) const
    {
        throw L"Unimplemented!!!";
        pxAssertMsg(Id != -1, "Uninitialized x86 register.");
        return xAddressVoid(*this, (sptr)right);
    }

    xAddressVoid xAddressReg::operator-(sptr right) const
    {
        throw L"Unimplemented!!!";
        pxAssertMsg(Id != -1, "Uninitialized x86 register.");
        return xAddressVoid(*this, -right);
    }

    xAddressVoid xAddressReg::operator-(const void *right) const
    {
        throw L"Unimplemented!!!";
        pxAssertMsg(Id != -1, "Uninitialized x86 register.");
        return xAddressVoid(*this, -(sptr)right);
    }

    xAddressVoid xAddressReg::operator*(int factor) const
    {
        pxAssertMsg(Id != -1, "Uninitialized x86 register.");
        return xAddressVoid(xEmptyReg, *this, factor);
    }

    xAddressVoid xAddressReg::operator<<(u32 shift) const
    {
        throw L"Unimplemented!!!";
        pxAssertMsg(Id != -1, "Uninitialized x86 register.");
        return xAddressVoid(xEmptyReg, *this, 1 << shift);
    }

    void xLoadFarAddr(const xAddressReg& dst, void *addr) {
        throw L"Unimplemented!!!";
//#ifdef __M_X86_64
//    sptr iaddr = (sptr)addr;
//    sptr rip = (sptr)xGetPtr() + 7; // LEA will be 7 bytes
//    sptr disp = iaddr - rip;
//    if (disp == (s32)disp) {
//        xLEA(dst, ptr[addr]);
//    } else {
//        xMOV64(dst, iaddr);
//    }
//#else
//    xMOV(dst, (sptr)addr);
//#endif
    }

//////////////////////////////////////////////////////////////////////////////////////////
//
    __fi void xRET() {
        g_Emitter.RET();
    }

    __fi void xCDQ() {

//        IF OperandSize = 16 (* CWD instruction *)
//        THEN
//        DX ← SignExtend(AX);
//        ELSE IF OperandSize = 32 (* CDQ instruction *)
//        EDX ← SignExtend(EAX); FI;
//        ELSE IF 64-Bit Mode and OperandSize = 64 (* CQO instruction*)
//        RDX ← SignExtend(RAX); FI;
//        FI;

        g_Emitter.LSR(Arm64JitConstants::SCRATCH1_32, getWReg(eax), 31);

        g_Emitter.BFI(Arm64JitConstants::SCRATCH1_32, Arm64JitConstants::SCRATCH1_32, 1, 1);
        g_Emitter.BFI(Arm64JitConstants::SCRATCH1_32, Arm64JitConstants::SCRATCH1_32, 2, 2);
        g_Emitter.BFI(Arm64JitConstants::SCRATCH1_32, Arm64JitConstants::SCRATCH1_32, 4, 4);

        g_Emitter.SXTB(getWReg(edx), Arm64JitConstants::SCRATCH1_32);
    }

    __fi void xCWDE() {
//        IF OperandSize = 16 (* Instruction = CBW *)
//        THEN
//        AX ← SignExtend(AL);
//        ELSE IF (OperandSize = 32, Instruction = CWDE)
//        EAX ← SignExtend(AX); FI;
//        ELSE (* 64-Bit Mode, OperandSize = 64, Instruction = CDQE*)
//        RAX ← SignExtend(EAX);
//        FI;

        g_Emitter.SXTH(Arm64JitConstants::SCRATCH1_32, getWReg(eax));

        g_Emitter.MOV(getWReg(eax), Arm64JitConstants::SCRATCH1_32);
    }


//    MRS  x1, NZCV          ; copy N, Z, C, and V flags into general-purpose x1
//    MOV  x2, #0x30000000
//    BIC  x1,x1,x2          ; clears the C and V flags (bits 29,28)
//    ORR  x1,x1,#0xC0000000 ; sets the N and Z flags (bits 31,30)
//    MSR  NZCV, x1          ; copy x1 back into NZCV register to update the condition flags

    // AH ← EFLAGS(SF:ZF:0:AF:0:PF:1:CF);

    // Bit   Label    Desciption
    //	-------------------------- -
    //	0      CF      Carry flag
    //	2      PF      Parity flag (Set if low-order eight bits of result contain an even number of "1" bits; cleared otherwise)
    //	4      AF      Auxiliary carry flag (1-carry out from bit 3 on addition or borrow into bit 3 on subtraction, 0 - otherwise)
    //	6      ZF      Zero flag
    //	7      SF      Sign flag
    //	8      TF      Trap flag
    //	9      IF      Interrupt enable flag
    //	10     DF      Direction flag
    //	11     OF      Overflow flag
    //	12 - 13  IOPL    I / O Priviledge level
    //	14     NT      Nested task flag
    //	16     RF      Resume flag
    //	17     VM      Virtual 8086 mode flag
    //	18     AC      Alignment check flag(486 + )
    //	19     VIF     Virutal interrupt flag
    //	20     VIP     Virtual interrupt pending flag
    //	21     ID      ID flag

    __fi void xLAHF() {

        g_Emitter.MRS(Arm64JitConstants::SCRATCH1_64, Arm64Gen::FIELD_NZCV);

        g_Emitter.MOVZ(getReg(eax), 0x200);

        //  N[31]Z[30]C[29]
        g_Emitter.LSR(Arm64JitConstants::SCRATCH1_64, Arm64JitConstants::SCRATCH1_64, 29);

        g_Emitter.MOV(Arm64JitConstants::SCRATCH2_32, Arm64Gen::WZR);

        g_Emitter.BFI(Arm64JitConstants::SCRATCH2_32, Arm64JitConstants::SCRATCH1_32, 0, 1);

        g_Emitter.MVN(Arm64JitConstants::SCRATCH2_32, Arm64JitConstants::SCRATCH2_32);

        // SZ0A 0P1C 0000 0000
        g_Emitter.BFI(getReg(eax), Arm64JitConstants::SCRATCH2_32, 8, 1);

        g_Emitter.LSR(Arm64JitConstants::SCRATCH1_64, Arm64JitConstants::SCRATCH1_64, 1);

        g_Emitter.BFI(getReg(eax), Arm64JitConstants::SCRATCH1_32, 14, 2);

    }


    static __aligned16 u64 xmm_data[iREGCNT_XMM * 2];

    __emitinline void xStoreReg(const xRegisterSSE &src)
    {
        throw L"Unimplemented!!!";
        xMOVDQA(ptr[&xmm_data[src.Id * 2]], src);
    }

    __emitinline void xRestoreReg(const xRegisterSSE &dest)
    {
        throw L"Unimplemented!!!";
        xMOVDQA(dest, ptr[&xmm_data[dest.Id * 2]]);
    }


// =====================================================================================================
//  TEST / INC / DEC
// =====================================================================================================
    void xImpl_Test::operator()(const xRegisterInt &to, const xRegisterInt &from) const
    {
        throw L"Unimplemented!!!";
//    pxAssert(to.GetOperandSize() == from.GetOperandSize());
//    xOpWrite(to.GetPrefix16(), to.Is8BitOp() ? 0x84 : 0x85, from, to);
    }

    void xImpl_Test::operator()(const xRegisterInt &to, int imm) const
    {
        g_Emitter.MOVI2R(Arm64JitConstants::SCRATCH1_32, imm);

        g_Emitter.TST(getReg(to), Arm64JitConstants::SCRATCH1_32);
    }

    void xImpl_Test::operator()(const xIndirect64orLess &dest, int imm) const
    {
        throw L"Unimplemented!!!";
//    xOpWrite(dest.GetPrefix16(), dest.Is8BitOp() ? 0xf6 : 0xf7, 0, dest, dest.GetImmSize());
//    dest.xWriteImm(imm);
    }


    void xImpl_BitScan::operator()(const xRegister16or32or64 &to, const xRegister16or32or64 &from) const
    {
        throw L"Unimplemented!!!";
//    pxAssert(to->GetOperandSize() == from->GetOperandSize());
//    xOpWrite0F(from->GetPrefix16(), Opcode, to, from);
    }



    void xImpl_IncDec::operator()(const xRegisterInt &to) const
    {
        // DEST ← DEST + 1;
        //The CF flag is not affected. The OF, SF, ZF, AF, and PF flags are set according to the result.

        g_Emitter.ADD(getReg(to), getReg(to), 1);
    }


    void xImpl_DwordShift::operator()(const xIndirectVoid &dest, const xRegister16or32or64 &from, u8 shiftcnt) const
    {
        throw L"Unimplemented!!!";
//    if (shiftcnt != 0)
//        xOpWrite0F(from->GetPrefix16(), OpcodeBase, from, dest, shiftcnt);
    }


    const xImpl_Test xTEST = {};

    const xImpl_BitScan xBSR = {0xbd};

    const xImpl_DwordShift xSHLD = {0xa4};

    const xImpl_IncDec xINC = {false};
    const xImpl_IncDec xDEC = {true};



    void setNOP(void* ptrMem, int value, u32 byteSize)
    {
        memset(ptrMem, 0, byteSize);

//        auto l_uiPtr = (u32 *)ptrMem;
//
//        auto l_instructionSize = byteSize / 4;
//
//        for (size_t i = 0; i < l_instructionSize; i++)
//            l_uiPtr[i] = 0;
//            l_uiPtr[i] = 0xD503201F;
    }


    uint32_t regs_to_save = Arm64Gen::ALL_CALLEE_SAVED;
    uint32_t regs_to_save_fp = Arm64Gen::ALL_CALLEE_SAVED_FP;

    xScopedStackFrame::xScopedStackFrame(bool base_frame, bool save_base_pointer, int offset)
    {
        m_base_frame = base_frame;
        m_save_base_pointer = save_base_pointer;
        m_offset = offset;

        g_fp.ABI_PushRegisters(regs_to_save, regs_to_save_fp);

        g_Emitter.PUSH(Arm64Gen::X3);
        g_Emitter.POP(Arm64Gen::X3);

        g_Emitter.PUSH2(Arm64Gen::X3, Arm64Gen::X4);
        g_Emitter.POP2(Arm64Gen::X3, Arm64Gen::X4);


        g_fp.SCVTF(Arm64Gen::S0, Arm64Gen::W3, 12);
        g_fp.SCVTF(Arm64Gen::S3, Arm64Gen::W12);
    }

    xScopedStackFrame::~xScopedStackFrame()
    {
        g_fp.ABI_PopRegisters(regs_to_save, regs_to_save_fp);

        g_Emitter.FlushIcache();
    }

    xScopedSavedRegisters::xScopedSavedRegisters(std::initializer_list<std::reference_wrapper<const xAddressReg>> regs)
            : regs(regs)
    {
        throw L"Unimplemented!!!";
//        for (auto reg : regs)
//        {
//            const xAddressReg& regRef = reg;
//            xPUSH(regRef);
//        }
//        stackAlign(regs.size() * wordsize, true);
    }

    xScopedSavedRegisters::~xScopedSavedRegisters() {
        throw L"Unimplemented!!!";
//        stackAlign(regs.size() * wordsize, false);
//        for (auto it = regs.rbegin(); it < regs.rend(); ++it) {
//            const xAddressReg& regRef = *it;
//            xPOP(regRef);
//        }
    }
}


//*********************
// SSE   instructions *
//*********************
void SSE_MAXSS_XMM_to_XMM(x86SSERegType to, x86SSERegType from) {
    throw L"Unimplemented!!!";}
void SSE_MINSS_XMM_to_XMM(x86SSERegType to, x86SSERegType from){
    throw L"Unimplemented!!!";}
void SSE_ADDSS_XMM_to_XMM(x86SSERegType to, x86SSERegType from){
    throw L"Unimplemented!!!";}
void SSE_SUBSS_XMM_to_XMM(x86SSERegType to, x86SSERegType from){
    throw L"Unimplemented!!!";}

//*********************
//  SSE 2 Instructions*
//*********************

void SSE2_MAXSD_XMM_to_XMM(x86SSERegType to, x86SSERegType from){
    throw L"Unimplemented!!!";}
void SSE2_MINSD_XMM_to_XMM(x86SSERegType to, x86SSERegType from){
    throw L"Unimplemented!!!";}
void SSE2_ADDSD_XMM_to_XMM(x86SSERegType to, x86SSERegType from){
    throw L"Unimplemented!!!";}
void SSE2_SUBSD_XMM_to_XMM(x86SSERegType to, x86SSERegType from){
    throw L"Unimplemented!!!";}