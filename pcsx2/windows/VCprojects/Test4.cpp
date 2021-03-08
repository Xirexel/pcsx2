//
// Created by evgen on 9/28/2020.
//


#include "PrecompiledHeader.h"
#ifdef ANDROID_ABI_V8A
#include "include/Arm64Emitter.h"
#include "../Emitters/arm64v8aEmitter/include/Arm64Emitter.h"
#include "../../../../../../../PCSX2Lib/PCSX2Lib_API.h"
#include "include/legacy_instructions.h"
#else
#include "x86emitter/tools.h"
#include "x86emitter/x86types.h"
#include "x86emitter/instructions.h"
#include "x86emitter/legacy_types.h"
#include "x86emitter/legacy_instructions.h"
#endif
#include "vtlb.h"
#include "BaseblockEx.h"


static u32 CallPtr(const void *ptr)
{
    return ((u32(*)())ptr)();
}

static u64 CallPtr64(const void *ptr)
{
    return ((u64(*)())ptr)();
}


typedef void DynGenFunc();

struct cpuRegistersTest
{
    //    GPRregs GPR;		// GPR regs
    //    // NOTE: don't change order since recompiler uses it
    //    GPR_reg HI;
    //    GPR_reg LO;			// hi & log 128bit wide
    //    CP0regs CP0;		// is COP0 32bit?
    u32 sa;          // shift amount (32bit), needs to be 16 byte aligned
    u32 IsDelaySlot; // set true when the current instruction is a delay slot.
    u32 pc;          // Program counter, when changing offset in struct, check iR5900-X.S to make sure offset is correct
    u32 code;        // current instruction
    //    PERFregs PERF;
    u32 eCycle[32];
    u32 sCycle[32]; // for internal counters
    u32 cycle;      // calculate cpucycles..
    u32 interrupt;
    int branch;
    int opmode; // operating mode
    u32 tempcycles;


	u32 UL;

	s16 cpuRegs_code;

    s32 signNumber;

	u8 Main[20];

    sptr MainSptr[20];

	u32 m_value = 0;

	u32 mValue;

	u32 mResult;

	u64 Base;

	u64 Base1;

	u64 DirectData;
};

__aligned16 cpuRegistersTest cpuRegsTest1;
static __aligned16 uptr recLUTTest1[_64kb];
static __aligned16 u32 hwLUT[_64kb];

using namespace x86Emitter;

// Recompiled code buffer for EE recompiler dispatchers!
static u8 __pagealigned eeRecDispatchers[__pagesize];
static const size_t recLutSize = (Ps2MemSize::MainRam + Ps2MemSize::Rom + Ps2MemSize::Rom1 + Ps2MemSize::Rom2) * wordsize / 4;


static DynGenFunc *DispatcherReg = NULL;

static DynGenFunc *ExitRecompiledCode = NULL;
static u32* recConstBuf = NULL;			// 64-bit pseudo-immediates
static BASEBLOCK *recRAM = NULL;		// and the ptr to the blocks here
static BASEBLOCK *recROM = NULL;		// and here
static BASEBLOCK *recROM1 = NULL;		// also here
static BASEBLOCK *recROM2 = NULL;		// also here
static u8* recRAMCopy = NULL;
static u8* recLutReserve_RAM = NULL;

static u32 s_stateTest;

static u32 __pagealigned memoryBank[100];

static u32 __pagealigned memoryBank1[100];

static void recLUT_SetPage1(uptr reclut[0x10000], u32 hwlut[0x10000],
                            BASEBLOCK *mapbase, uint pagebase, uint pageidx, uint mappage)
{

    // this value is in 64k pages!
    uint page = pagebase + pageidx;

    pxAssert( page < 0x10000 );
    reclut[page] = (uptr)&mapbase[((s32)mappage - (s32)page) << 14];
    if (hwlut)
        hwlut[page] = 0u - (pagebase << 16);
}

static __ri void ClearRecLUT(BASEBLOCK* base, int memsize)
{
    for (int i = 0; i < memsize/(int)sizeof(uptr); i++)
        base[i].SetFnptr((uptr)ExitRecompiledCode);
}


class TestExecute
{

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        DispatcherReg = _DynGen_DispatcherReg();

        auto DynGen_Code1 = _DynGen_Code1();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());






        if (!recRAM)
        {
            recLutReserve_RAM = (u8*)_aligned_malloc(recLutSize, 4096);
        }

        ClearRecLUT((BASEBLOCK*)recLutReserve_RAM, recLutSize);

        BASEBLOCK* basepos = (BASEBLOCK*)recLutReserve_RAM;
        recRAM		= basepos; basepos += (Ps2MemSize::MainRam / 4);
        recROM		= basepos; basepos += (Ps2MemSize::Rom / 4);
        recROM1		= basepos; basepos += (Ps2MemSize::Rom1 / 4);
        recROM2		= basepos; basepos += (Ps2MemSize::Rom2 / 4);



        for (int i = 0; i < 0x10000; i++)
            recLUT_SetPage1(recLUTTest1, 0, 0, 0, i, 0);

        for ( int i = 0x0000; i < (int)(Ps2MemSize::MainRam / 0x10000); i++ )
        {
            recLUT_SetPage1(recLUTTest1, hwLUT, recRAM, 0x0000, i, i);
            recLUT_SetPage1(recLUTTest1, hwLUT, recRAM, 0x2000, i, i);
            recLUT_SetPage1(recLUTTest1, hwLUT, recRAM, 0x3000, i, i);
            recLUT_SetPage1(recLUTTest1, hwLUT, recRAM, 0x8000, i, i);
            recLUT_SetPage1(recLUTTest1, hwLUT, recRAM, 0xa000, i, i);
            recLUT_SetPage1(recLUTTest1, hwLUT, recRAM, 0xb000, i, i);
            recLUT_SetPage1(recLUTTest1, hwLUT, recRAM, 0xc000, i, i);
            recLUT_SetPage1(recLUTTest1, hwLUT, recRAM, 0xd000, i, i);
        }

        for ( int i = 0x1fc0; i < 0x2000; i++ )
        {
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM, 0x0000, i, i - 0x1fc0);
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM, 0x8000, i, i - 0x1fc0);
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM, 0xa000, i, i - 0x1fc0);
        }

        for ( int i = 0x1e00; i < 0x1e04; i++ )
        {
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM1, 0x0000, i, i - 0x1e00);
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM1, 0x8000, i, i - 0x1e00);
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM1, 0xa000, i, i - 0x1e00);
        }

        for (int i = 0x1e40; i < 0x1e48; i++)
        {
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM2, 0x0000, i, i - 0x1e40);
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM2, 0x8000, i, i - 0x1e40);
            recLUT_SetPage1(recLUTTest1, hwLUT, recROM2, 0xa000, i, i - 0x1e40);
        }



        cpuRegsTest1.pc = 0xbfc00000;

//        recLUTTest1[0xbfc0] = ((uptr)&ExitRecompiledCode) - cpuRegsTest1.pc;

//        recLUTTest1[0] = (uptr)&ExitRecompiledCode;

        auto l_result = CallPtr((void *)DynGen_Code1);

    }

    // called when jumping to variable pc address
    static DynGenFunc *_DynGen_DispatcherReg()
    {
        u8 *retval = xGetPtr(); // fallthrough target, can't align it!

        xMOV( eax, ptr[&cpuRegsTest1.pc] );
        xMOV( ebx, eax );
        xSHR( eax, 16 );
        xMOV( rcx, ptrNative[xComplexAddress(rcx, recLUTTest1, rax*wordsize)] );
        xJMP( ptrNative[rbx*(wordsize/4) + rcx] );

        return (DynGenFunc *)retval;
    }


    DynGenFunc *_DynGen_Code1()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif


            xJMP((void *)DispatcherReg);

            // Save an exit point
            ExitRecompiledCode = (DynGenFunc *)xGetPtr();
        }

        xRET();

        return (DynGenFunc *)retval;
    }

    static DynGenFunc *_DynGen_DispatchPageReset()
    {
        u8 *retval = xGetPtr();
        xJMP((void *)ExitRecompiledCode);
        return (DynGenFunc *)retval;
    }

public:
    TestExecute()
    {
    }
    void operator()() { execute(); }
};



class TestXOR
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeXOR = _DynGen_CodeXOR();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeXOR);

        if (l_result != 0) {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeXOR()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            // test silent hill if modding
            xMOV(eax, 12);

            xXOR(eax, eax);

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestXOR()
    {
    }
    void operator()() { execute(); }
};


class TestJNE8
{
    std::shared_ptr<x86Emitter::xForwardJumpBase> j8Ptr[32];
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

		cpuRegsTest1.cpuRegs_code = 10;

		cpuRegsTest1.UL = -10;

        auto DynGen_CodeJNE8 = _DynGen_CodeJNE8();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeJNE8);

        if (l_result != 1) {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeJNE8()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, 1);

            xCMP(ptr32[&cpuRegsTest1.UL], (s32)cpuRegsTest1.cpuRegs_code);

            // test silent hill if modding
            j8Ptr[0] = JNE8(0);

            xMOV(eax, 2);

            x86SetJ8(j8Ptr[0]);

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestJNE8()
    {
    }
    void operator()() { execute(); }
};


class TestJE32
{
    std::shared_ptr<x86Emitter::xForwardJumpBase> j32Ptr[32];

public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

		cpuRegsTest1.cpuRegs_code = 10;

		cpuRegsTest1.UL = 10;

        auto DynGen_CodeJE32 = _DynGen_CodeJE32();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeJE32);

        if (l_result != 1) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.UL = 9;

        l_result = CallPtr((void *)DynGen_CodeJE32);

        if (l_result != 2) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeJE32()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, 1);

            xCMP(ptr32[&cpuRegsTest1.UL], (s32)cpuRegsTest1.cpuRegs_code);

            // test silent hill if modding
            j32Ptr[0] = JE32(0);

            xMOV(eax, 2);

            x86SetJ32(j32Ptr[0]);

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestJE32()
    {
    }
    void operator()() { execute(); }
};



class TestJS32
{
    std::shared_ptr<x86Emitter::xForwardJumpBase> j32Ptr[32];

public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

		cpuRegsTest1.cpuRegs_code = 10;

		cpuRegsTest1.UL = 9;

        auto DynGen_CodeJS32 = _DynGen_CodeJS32();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeJS32);

        if (l_result != 1) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.UL = 10;

        l_result = CallPtr((void *)DynGen_CodeJS32);

        if (l_result != 4) {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeJS32()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, 1);

            xCMP(ptr32[&cpuRegsTest1.UL], (s32)cpuRegsTest1.cpuRegs_code);

            // test silent hill if modding
            j32Ptr[0] = JS32(Jcc_Signed);

            xMOV(eax, 2);

            xMOV(eax, 3);

            xMOV(eax, 4);

            x86SetJ32(j32Ptr[0]);

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestJS32()
    {
    }
    void operator()() { execute(); }
};

class TestSETL
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

		cpuRegsTest1.cpuRegs_code = 10;

		cpuRegsTest1.UL = 9;

        auto DynGen_CodeSETL = _DynGen_CodeSETL();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeSETL);

        if (l_result != 0xFFFFFF01) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.UL = 10;

        l_result = CallPtr((void *)DynGen_CodeSETL);

        if (l_result != 0xFFFFFF00) {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeSETL()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif


            xMOV(eax, -1);
            xCMP(ptr32[&cpuRegsTest1.UL], (s32)cpuRegsTest1.cpuRegs_code);
            xSETL(al);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestSETL()
    {
    }
    void operator()() { execute(); }
};

class TestJLE
{
    DynGenFunc * subCode;

public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

		cpuRegsTest1.cpuRegs_code = 10;

		cpuRegsTest1.UL = 9;

        subCode = _DynGen_SubCode();

        auto DynGen_CodeJLE = _DynGen_CodeJLE();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeJLE);

        if (l_result != 2 && cpuRegsTest1.UL == 0xFFFFFFFF) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.UL = 10;

        l_result = CallPtr((void *)DynGen_CodeJLE);

        if (l_result != 2 && cpuRegsTest1.UL == 0) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.UL = 11;

        l_result = CallPtr((void *)DynGen_CodeJLE);

        if (l_result != 1 && cpuRegsTest1.UL == 1) {
            throw L"Unimplemented!!!";
        }

    }

    static DynGenFunc *_DynGen_SubCode()
    {
        u8 *retval = xGetPtr(); // fallthrough target, can't align it!

        xMOV( eax, 2 );
        xJMP( ptrNative[&ExitRecompiledCode]);

        return (DynGenFunc *)retval;
    }

    DynGenFunc *_DynGen_CodeJLE()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, 1);
            xSUB(ptr32[&cpuRegsTest1.UL], (s32)cpuRegsTest1.cpuRegs_code);
            xJLE(subCode);

            // Save an exit point
            ExitRecompiledCode = (DynGenFunc *)xGetPtr();
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestJLE()
    {
    }
    void operator()() { execute(); }
};

class TestNOT
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        u32 l_forward = 0xFED0;

        u32 l_revers = 0xFFFF012F;

		cpuRegsTest1.UL = l_forward;

        auto DynGen_CodeNOT = _DynGen_CodeNOT();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeNOT);

        if (l_result != l_revers) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.UL = l_revers;

        l_result = CallPtr((void *)DynGen_CodeNOT);

        if (l_result != l_forward) {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeNOT()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif


            xMOV(eax, ptr32[&cpuRegsTest1.UL]);
            xNOT(eax);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestNOT()
    {
    }
    void operator()() { execute(); }
};

class TestTEST
{

    DynGenFunc * subCode;

public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

		cpuRegsTest1.cpuRegs_code = 10;

		cpuRegsTest1.UL = 0;

        subCode = _DynGen_SubCode();

        auto DynGen_CodeTEST = _DynGen_CodeTEST();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeTEST);

        if (l_result != 2) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.UL = 0x8000000;

        l_result = CallPtr((void *)DynGen_CodeTEST);

        if (l_result != 2) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.UL = 0x10000000;

        l_result = CallPtr((void *)DynGen_CodeTEST);

        if (l_result != 1) {
            throw L"Unimplemented!!!";
        }

    }

    static DynGenFunc *_DynGen_SubCode()
    {
        u8 *retval = xGetPtr(); // fallthrough target, can't align it!

        xMOV( eax, 2 );
        xJMP( ptrNative[&ExitRecompiledCode]);

        return (DynGenFunc *)retval;
    }

    DynGenFunc *_DynGen_CodeTEST()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, 1);
            xMOV(ecx, ptr32[&cpuRegsTest1.UL]);
            xTEST(ecx, 0x10000000);
            xJZ(subCode);

            // Save an exit point
            ExitRecompiledCode = (DynGenFunc *)xGetPtr();
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestTEST()
    {
    }
    void operator()() { execute(); }
};

class TestComplexAddress
{

public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        memset(cpuRegsTest1.Main, 0, 20);

        auto DynGen_Code = _DynGen_CodeComplexAddress();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.Main[8] = 0xFF;
		cpuRegsTest1.Main[10] = 0xFF;

        l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0xFF00FF) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeComplexAddress()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, 1);
            xMOV(rcx, 8);
            xMOV(eax, ptrNative[xComplexAddress(rax, cpuRegsTest1.Main, rcx)]);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestComplexAddress()
    {
    }
    void operator()() { execute(); }
};

class TestCWDE
{

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeCWDE();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		cpuRegsTest1.m_value = 0x8000;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0xFFFF8000){
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.m_value = 0;

        l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeCWDE()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV( eax, ptr[&cpuRegsTest1.m_value] );
            xCWDE();

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestCWDE()
    {
    }
    void operator()() { execute(); }
};

class TestSignExtendSFtoM
{

public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeSignExtendSFtoM();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		cpuRegsTest1.mValue = 9;

        auto l_result = CallPtr((void *)DynGen_Code);



        if (l_result != 0) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.mValue = 10;

        l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0) {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.mValue = 11;

        l_result = CallPtr((void *)DynGen_Code);


        if (l_result != 0xFFFFFFFF) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeSignExtendSFtoM()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif


            xMOV(eax, 0);

            xMOV(ecx, 10);

            xSUB(ecx, ptr32[&cpuRegsTest1.mValue]);

            xLAHF();
            xSAR(ax, 15);
            xCWDE();
            xMOV(ptr[&cpuRegsTest1.mResult], eax);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestSignExtendSFtoM()
    {
    }
    void operator()() { execute(); }
};

class TestLEA_Writeback
{
    static const uint VTLB_PAGE_BITS = 12;

	uptr m_TargetPtrAddress = 0;

public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeLEA_Writeback();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        cpuRegsTest1.pc = 2 << VTLB_PAGE_BITS;

        memset(cpuRegsTest1.MainSptr, 0, 20 * wordsize);

		cpuRegsTest1.MainSptr[2] = 0xFF00FF;

        cpuRegsTest1.Base = 0;

		cpuRegsTest1.Base1 = 0;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (l_result != cpuRegsTest1.MainSptr[2] 
			|| cpuRegsTest1.Base != (u64)cpuRegsTest1.MainSptr
			|| cpuRegsTest1.Base1 != m_TargetPtrAddress
			) 
		{
            throw L"Unimplemented!!!";
        }
    }
	
    DynGenFunc *_DynGen_CodeLEA_Writeback()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            // Prepares eax, ecx, and, ebx for Direct or Indirect operations.
            // Returns the writeback pointer for ebx (return address from indirect handling)

            xMOV( eax, ptr[&cpuRegsTest1.pc] );
            xSHR( eax, VTLB_PAGE_BITS );
            xMOV( rax, ptrNative[xComplexAddress(rbx, cpuRegsTest1.MainSptr, rax*wordsize)] );
			xMOV(ptrNative[&cpuRegsTest1.Base], rbx);
            u32* writeback = xLEA_Writeback( rbx );
			xMOV(ecx, ptr[&cpuRegsTest1.pc]);
			xMOV(edx, ptr[&cpuRegsTest1.pc]);
     
			m_TargetPtrAddress = (uptr)xGetPtr();

			vtlb_SetWriteback(writeback);
			xMOV(ptrNative[&cpuRegsTest1.Base1], rbx);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestLEA_Writeback()
    {
    }
    void operator()() { execute(); }
};

class TestiMOV64_Smart
{
    static const uint VTLB_PAGE_BITS = 12;

    uptr m_TargetPtrAddress = 0;

public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeRegs = _DynGen_CodeTestiMOV64_SmartRegs();

		auto DynGen_CodeRegToPtr = _DynGen_CodeTestiMOV64_SmartRegToPtr();

		auto DynGen_CodePtrToReg = _DynGen_CodeTestiMOV64_SmartPtrToReg();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		cpuRegsTest1.Base = -1;

		cpuRegsTest1.Base1 = 12345;

		auto l_result = CallPtr((void *)DynGen_CodeRegs);

        if (cpuRegsTest1.Base != cpuRegsTest1.Base1)
        {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.Base = 123456;

		cpuRegsTest1.DirectData = -1;

		l_result = CallPtr((void *)DynGen_CodeRegToPtr);

		if (cpuRegsTest1.Base != cpuRegsTest1.DirectData)
		{
			throw L"Unimplemented!!!";
		}

		cpuRegsTest1.Base = -1;

		cpuRegsTest1.DirectData = 123456;

		l_result = CallPtr((void *)DynGen_CodePtrToReg);

		if (cpuRegsTest1.Base != cpuRegsTest1.DirectData)
		{
			throw L"Unimplemented!!!";
		}
    }

    DynGenFunc *_DynGen_CodeTestiMOV64_SmartRegs()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            // Prepares eax, ecx, and, ebx for Direct or Indirect operations.
            // Returns the writeback pointer for ebx (return address from indirect handling)

			xMOV(rax, 0);
			xComplexAddress(arg1reg, &cpuRegsTest1.Base1, rax);
			xMOV(rax, 0);
			xComplexAddress(arg2reg, &cpuRegsTest1.Base, rax);


            iMOV64_Smart( ptr[arg1reg], ptr[arg2reg] );
        }

        xRET();

        return (DynGenFunc *)retval;
    }

	DynGenFunc *_DynGen_CodeTestiMOV64_SmartRegToPtr()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			// Prepares eax, ecx, and, ebx for Direct or Indirect operations.
			// Returns the writeback pointer for ebx (return address from indirect handling)

			xMOV(rax, 0);
			xComplexAddress(arg2reg, &cpuRegsTest1.Base, rax);
			
			iMOV64_Smart(ptr[(void*)&cpuRegsTest1.DirectData], ptr[arg2reg]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

	DynGenFunc *_DynGen_CodeTestiMOV64_SmartPtrToReg()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			// Prepares eax, ecx, and, ebx for Direct or Indirect operations.
			// Returns the writeback pointer for ebx (return address from indirect handling)

			xMOV(rax, 0);
			xComplexAddress(arg2reg, &cpuRegsTest1.Base, rax);
			
			iMOV64_Smart(ptr[arg2reg], ptr[(void*)&cpuRegsTest1.DirectData]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}


// Moves 64 bits of data from point B to point A, using either SSE, or x86 registers
//
    static void iMOV64_Smart( const xIndirectVoid& destRm, const xIndirectVoid& srcRm )
    {
        if (wordsize == 8) {
            xMOV(rax, srcRm);
            xMOV(destRm, rax);
            return;
        }
    }

public:
    TestiMOV64_Smart()
    {
    }
    void operator()() { execute(); }
};

class TestMOVSXPtr8Ptr16
{

public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		auto DynGen_CodeMOVSXPtr8 = _DynGen_CodeMOVSXPtr8();

		auto DynGen_CodeMOVZXPtr8 = _DynGen_CodeMOVZXPtr8();

		auto DynGen_CodeMOVSXPtr16 = _DynGen_CodeMOVSXPtr16();

		auto DynGen_CodeMOVZXPtr16 = _DynGen_CodeMOVZXPtr16();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		cpuRegsTest1.Base = -1;

		auto l_result = CallPtr((void *)DynGen_CodeMOVSXPtr8);

		if (l_result != (u32)cpuRegsTest1.Base)
		{
			throw L"Unimplemented!!!";
		}
		
		l_result = CallPtr((void *)DynGen_CodeMOVZXPtr8);

		if (l_result != (u8)cpuRegsTest1.Base)
		{
			throw L"Unimplemented!!!";
		}
		
		l_result = CallPtr((void *)DynGen_CodeMOVSXPtr16);

		if (l_result != (u32)cpuRegsTest1.Base)
		{
			throw L"Unimplemented!!!";
		}

		l_result = CallPtr((void *)DynGen_CodeMOVZXPtr16);

		if (l_result != (u16)cpuRegsTest1.Base)
		{
			throw L"Unimplemented!!!";
		}

	}

	DynGenFunc *_DynGen_CodeMOVSXPtr8()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
			xMOV(rax, 0);

			xComplexAddress(arg1reg, &cpuRegsTest1.Base, rax);

			xMOVSX(eax, ptr8[arg1reg]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

	DynGenFunc *_DynGen_CodeMOVZXPtr8()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
            xMOV(rax, 0);

            xComplexAddress(arg1reg, &cpuRegsTest1.Base, rax);

			xMOVZX(eax, ptr8[arg1reg]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

	DynGenFunc *_DynGen_CodeMOVSXPtr16()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
            xMOV(rax, 0);

            xComplexAddress(arg1reg, &cpuRegsTest1.Base, rax);

			xMOVSX(eax, ptr16[arg1reg]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

	DynGenFunc *_DynGen_CodeMOVZXPtr16()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
            xMOV(rax, 0);

            xComplexAddress(arg1reg, &cpuRegsTest1.Base, rax);
				
			xMOVZX(eax, ptr16[arg1reg]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestMOVSXPtr8Ptr16()
	{
	}
	void operator()() { execute(); }
};


class TestOR
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeOR();

        cpuRegsTest1.pc = 0x0F;
        
        auto DynGen_Code1 = _DynGen_CodeORPtrToPtr();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());


        cpuRegsTest1.mValue = 0xF0;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (cpuRegsTest1.mValue != 0xFF)
        {
            throw L"Unimplemented!!!";
        }

        cpuRegsTest1.pc = 0x0F;

        cpuRegsTest1.mValue = 0xF0;

        l_result = CallPtr((void *)DynGen_Code1);

        if (cpuRegsTest1.mValue != 0xFF)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeOR()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOV( ecx, ptr[&cpuRegsTest1.pc] );

            xOR(ptr32[&cpuRegsTest1.mValue], ecx);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

    DynGenFunc *_DynGen_CodeORPtrToPtr()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xOR(ptr32[&cpuRegsTest1.mValue], cpuRegsTest1.pc);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestOR()
    {
    }
    void operator()() { execute(); }
};

class TestAND
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeAND();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        cpuRegsTest1.pc = 0x0F;

        cpuRegsTest1.mValue = 0xF0;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (cpuRegsTest1.mValue != 0)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeAND()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOV( ecx, ptr[&cpuRegsTest1.pc] );

            xAND(ptr32[&cpuRegsTest1.mValue], ecx);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestAND()
    {
    }
    void operator()() { execute(); }
};

class TestXOR1
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeXOR();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        cpuRegsTest1.pc = 0x1F;

        cpuRegsTest1.mValue = 0xF1;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (cpuRegsTest1.mValue != 0xEE)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeXOR()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOV( ecx, ptr[&cpuRegsTest1.pc] );

            xXOR(ptr32[&cpuRegsTest1.mValue], ecx);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestXOR1()
    {
    }
    void operator()() { execute(); }
};

class TestCMP1
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeCMP();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        cpuRegsTest1.pc = 1;

        cpuRegsTest1.mValue = 2;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 1)
        {
            throw L"Unimplemented!!!";
        }

        cpuRegsTest1.pc = 1;

        cpuRegsTest1.mValue = 1;

        l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeCMP()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, 1);
            xMOV(ecx, ptr32[&cpuRegsTest1.pc]);
            xCMP(ecx, ptr32[&cpuRegsTest1.mValue]);
            auto l_pbranchjmp = JNE32( 0 );
            xMOV(eax, 0);
            x86SetJ32A( l_pbranchjmp );
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestCMP1()
    {
    }
    void operator()() { execute(); }
};

class TestrpsxSLTU
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeSBB();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        cpuRegsTest1.pc = 1;

        cpuRegsTest1.mValue = 2;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 1)
        {
            throw L"Unimplemented!!!";
        }

        cpuRegsTest1.pc = 1;

        cpuRegsTest1.mValue = 1;

        l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeSBB()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, ptr32[&cpuRegsTest1.pc]);
            xCMP(eax, ptr32[&cpuRegsTest1.mValue]);
            xSBB(eax, eax);
            xNEG(eax);
            xMOV(ptr32[&cpuRegsTest1.mValue], eax);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestrpsxSLTU()
    {
    }
    void operator()() { execute(); }
};

class TestSBB
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeSBB();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        cpuRegsTest1.pc = 1;

        cpuRegsTest1.mValue = 2;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0xffffffff)
        {
            throw L"Unimplemented!!!";
        }

        cpuRegsTest1.pc = 1;

        cpuRegsTest1.mValue = 1;

        l_result = CallPtr((void *)DynGen_Code);

        if (l_result != 0)
        {
            throw L"Unimplemented!!!";
        }

		cpuRegsTest1.pc = 2;

		cpuRegsTest1.mValue = 0;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != 0)
		{
			throw L"Unimplemented!!!";
		}
    }

    DynGenFunc *_DynGen_CodeSBB()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, ptr32[&cpuRegsTest1.pc]);
            xCMP(eax, ptr32[&cpuRegsTest1.mValue]);
			xMOV(ptr32[&cpuRegsTest1.mResult], eax);
            xSBB(eax, eax);
            xMOV(ptr32[&cpuRegsTest1.mValue], eax);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestSBB()
    {
    }
    void operator()() { execute(); }
};

class TestNEG
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeNEG();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        cpuRegsTest1.mValue = 0;

        cpuRegsTest1.signNumber = 1;

        auto l_result = CallPtr((void *)DynGen_Code);

        if (l_result != -cpuRegsTest1.signNumber)
        {
            throw L"Unimplemented!!!";
        }

        cpuRegsTest1.signNumber = -1;

        l_result = CallPtr((void *)DynGen_Code);

        if (l_result != -cpuRegsTest1.signNumber)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeNEG()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV(eax, ptr32[&cpuRegsTest1.signNumber]);
            xCMP(eax, ptr32[&cpuRegsTest1.mValue]);
            xNEG(eax);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestNEG()
    {
    }
    void operator()() { execute(); }
};

class TestCMOVS
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		auto DynGen_Code = _DynGen_CodeCMOVS();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		cpuRegsTest1.mValue = 10;

		cpuRegsTest1.signNumber = 11;

		cpuRegsTest1.m_value = 0xFF;

		auto l_result = CallPtr((void *)DynGen_Code);

		if (l_result != cpuRegsTest1.m_value)
		{
			throw L"Unimplemented!!!";
		}

		cpuRegsTest1.signNumber = -1;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != cpuRegsTest1.mValue)
		{
			throw L"Unimplemented!!!";
		}
	}

	DynGenFunc *_DynGen_CodeCMOVS()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			xMOV(eax, ptr32[&cpuRegsTest1.mValue]);
			xCMP(eax, ptr32[&cpuRegsTest1.signNumber]);
			xCMOVS(eax, ptr32[&cpuRegsTest1.m_value]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestCMOVS()
	{
	}
	void operator()() { execute(); }
};

class TestCMOVE
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		auto DynGen_Code = _DynGen_CodeCMOVE();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		cpuRegsTest1.mValue = 10;

		cpuRegsTest1.signNumber = 10;

		cpuRegsTest1.m_value = 0xFF;

		auto l_result = CallPtr((void *)DynGen_Code);

		if (l_result != cpuRegsTest1.m_value)
		{
			throw L"Unimplemented!!!";
		}

		cpuRegsTest1.signNumber = -1;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != cpuRegsTest1.mValue)
		{
			throw L"Unimplemented!!!";
		}
	}

	DynGenFunc *_DynGen_CodeCMOVE()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			xMOV(eax, ptr32[&cpuRegsTest1.mValue]);
			xCMP(eax, ptr32[&cpuRegsTest1.signNumber]);
			xCMOVE(eax, ptr32[&cpuRegsTest1.m_value]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestCMOVE()
	{
	}
	void operator()() { execute(); }
};

class TestCMOVNS
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		auto DynGen_Code = _DynGen_CodeCMOVNS();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		cpuRegsTest1.mValue = 10;

		cpuRegsTest1.signNumber = 11;

		cpuRegsTest1.m_value = 0xFF;

		auto l_result = CallPtr((void *)DynGen_Code);

		if (l_result != cpuRegsTest1.mValue)
		{
			throw L"Unimplemented!!!";
		}

		cpuRegsTest1.signNumber = -1;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != cpuRegsTest1.m_value)
		{
			throw L"Unimplemented!!!";
		}
	}

	DynGenFunc *_DynGen_CodeCMOVNS()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			xMOV(eax, ptr32[&cpuRegsTest1.mValue]);
			xCMP(eax, ptr32[&cpuRegsTest1.signNumber]);
			xCMOVNS(eax, ptr32[&cpuRegsTest1.m_value]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestCMOVNS()
	{
	}
	void operator()() { execute(); }
};

class TestADD
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		auto DynGen_Code = _DynGen_CodeADD();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());
		
		cpuRegsTest1.signNumber = 10;
		
		auto l_result = CallPtr((void *)DynGen_Code);

		if (l_result != (10 + cpuRegsTest1.signNumber))
		{
			throw L"Unimplemented!!!";
		}

		cpuRegsTest1.signNumber = 0;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != (10 + cpuRegsTest1.signNumber))
		{
			throw L"Unimplemented!!!";
		}

		cpuRegsTest1.signNumber = -10;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != (10 + cpuRegsTest1.signNumber))
		{
			throw L"Unimplemented!!!";
		}

		cpuRegsTest1.signNumber = -20;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != (10 + cpuRegsTest1.signNumber))
		{
			throw L"Unimplemented!!!";
		}

	}

	DynGenFunc *_DynGen_CodeADD()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
			xMOV(eax, 10);

			xADD(eax, ptr32[&cpuRegsTest1.signNumber]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestADD()
	{
	}
	void operator()() { execute(); }
};

class TestSHL_SHR_SAR
{
	u8 m_shift;

public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

        m_shift = 5;

		auto DynGen_Code = _DynGen_CodeSHL_SHR_SAR();
		
		
        m_shift = 1;

		auto DynGen_CodeSHL_Reg = _DynGen_CodeSHL_Reg();
		
		auto DynGen_CodeSHR_Reg = _DynGen_CodeSHR_Reg();
		
		auto DynGen_CodeSAR_Reg = _DynGen_CodeSAR_Reg();
		

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		cpuRegsTest1.mValue = 10;

		cpuRegsTest1.signNumber = -50000;

		cpuRegsTest1.m_value = -50000;

		auto l_result = CallPtr((void *)DynGen_Code);

		if (320 != cpuRegsTest1.mValue &&
			134216165 != cpuRegsTest1.m_value &&
			-1563 != cpuRegsTest1.signNumber)
		{
			throw L"Unimplemented!!!";
		}


		cpuRegsTest1.mValue = 10;

		cpuRegsTest1.signNumber = -50000;

		cpuRegsTest1.m_value = -50000;

		l_result = CallPtr((void *)DynGen_CodeSHL_Reg);

		if (20 != l_result)
		{
			throw L"Unimplemented!!!";
		}

		l_result = CallPtr((void *)DynGen_CodeSHR_Reg);

		if (2147458648 != l_result)
		{
			throw L"Unimplemented!!!";
		}

		l_result = CallPtr((void *)DynGen_CodeSAR_Reg);

		if (4294942296 != l_result)
		{
			throw L"Unimplemented!!!";
		}

	}

	DynGenFunc *_DynGen_CodeSHL_SHR_SAR()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
			xSHL(ptr32[&cpuRegsTest1.mValue], m_shift); 
			xSHR(ptr32[&cpuRegsTest1.m_value], m_shift); 
			xSAR(ptr32[&cpuRegsTest1.signNumber], m_shift); 
		}

		xRET();

		return (DynGenFunc *)retval;
	}
	

	DynGenFunc *_DynGen_CodeSHL_Reg()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
			xMOV(eax, ptr32[&cpuRegsTest1.mValue]); 
			xSHL(eax, m_shift); 
		}

		xRET();

		return (DynGenFunc *)retval;
	}

	DynGenFunc *_DynGen_CodeSHR_Reg()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
			xMOV(eax, ptr32[&cpuRegsTest1.m_value]); 
			xSHR(eax, m_shift); 
		}

		xRET();

		return (DynGenFunc *)retval;
	}

	DynGenFunc *_DynGen_CodeSAR_Reg()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
			xMOV(eax, ptr32[&cpuRegsTest1.signNumber]); 
			xSAR(eax, m_shift); 
		}

		xRET();

		return (DynGenFunc *)retval;
	}
	

public:
	TestSHL_SHR_SAR()
	{
	}
	void operator()() { execute(); }
};

int __fastcall mVUexecuteVU01(u32 startPC, u32 cycles) {
    return 123;
}

class TestFastCall
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeFastCall();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_Code);

        if (123 != l_result)
        {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeFastCall()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOV(arg1reg, 10);

            xMOV(arg2reg, 30);

            xFastCall((void*)mVUexecuteVU01, arg1reg, arg2reg);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestFastCall()
    {
    }
    void operator()() { execute(); }
};

class TestOffsetIndex
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_Code = _DynGen_CodeOffsetIndex();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());
		


		for (size_t i = 0; i < 100; i++) {
			memoryBank[i] = 0;
			memoryBank1[i] = 110;
		}

		memoryBank[8] = 0xFF00FF;


        auto l_result = CallPtr((void *)DynGen_Code);

        if (memoryBank[8] != memoryBank1[0])
        {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeOffsetIndex()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

			xMOV(eax, 1);
			xMOV(rcx, 0);
			xMOV(eax, ptrNative[xComplexAddress(arg1reg, memoryBank, rcx)]);

            xMOVAPS(xmm0, ptr32[arg1reg + 0x20]);

            xMOVUPS(ptr32[memoryBank1], xmm0);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestOffsetIndex()
    {
    }
    void operator()() { execute(); }
};



class TestFastCall1
{
    static u64 recRecompile( const u32 startpc, const u32 startpc1 )
    {
        if(startpc != 98 || startpc1 != 400)
            throw L"Unimplemented!!!";

        return 99;
    }
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		auto DynGen_Code = _DynGen_CodeFastCall1();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());



		for (size_t i = 0; i < _64kb; i++) {
			recLUTTest1[i] = 0;
		}

		recLUTTest1[2] = (uptr)recRecompile;


		auto l_result = CallPtr64((void *)DynGen_Code);

		if (l_result != 99)
		{
			throw L"Unimplemented!!!";
		}

	}

	DynGenFunc *_DynGen_CodeFastCall1()
	{
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif
			xMOV(eax, -1);

			xMOV(al, 2);

			xMOV(arg2reg, 400);

			xMOV(arg1regd, 100);

			xMOVZX(eax, al);
			if (wordsize != 8) xSUB(arg1regd, 0x80000000);
			xSUB(arg1regd, eax);
			
			// jump to the indirect handler, which is a __fastcall C++ function.
			// [ecx is address, edx is data]
			sptr table = (sptr)recLUTTest1;
			if (table == (s32)table) {
				xFastCall(ptrNative[(rax*wordsize) + table], arg1reg, arg2reg);
			}
			else {
				xLEA(arg3reg, ptr[(void*)table]);
//                xMOV(rax, ptrNative[(rax*wordsize) + arg3reg]);
				xFastCall(ptrNative[(rax*wordsize) + arg3reg], arg1reg, arg2reg);
			}
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestFastCall1()
	{
	}
	void operator()() { execute(); }
};



void CPUTest4Finc()
{
    TestExecute l_TestExecute;

    l_TestExecute();

    TestXOR l_TestXOR;

    l_TestXOR();

    TestJNE8 l_TestJNE8;

    l_TestJNE8();

    TestJE32 l_TestJE32;

    l_TestJE32();

    TestJS32 l_TestJS32;

    l_TestJS32();

    TestSETL l_TestSETL;

    l_TestSETL();

    TestJLE l_TestJLE;

    l_TestJLE();

    TestNOT l_TestNOT;

    l_TestNOT();

    TestTEST l_TestTEST;

    l_TestTEST();


    TestComplexAddress l_TestComplexAddress;

    l_TestComplexAddress();


    TestCWDE l_TestCWDE;

    l_TestCWDE();


    TestSignExtendSFtoM l_TestSignExtendSFtoM;

    l_TestSignExtendSFtoM();


    TestLEA_Writeback l_TestLEA_Writeback;

    l_TestLEA_Writeback();


    TestiMOV64_Smart l_TestiMOV64_Smart;

    l_TestiMOV64_Smart();


	TestMOVSXPtr8Ptr16 l_TestMOVSXPtr8Ptr16;

	l_TestMOVSXPtr8Ptr16();


    TestOR l_TestOR;

    l_TestOR();


    TestAND l_TestAND;

    l_TestAND();


    TestXOR1 l_TestXOR1;

    l_TestXOR1();

    TestCMP1 l_TestCMP1;

    l_TestCMP1();

    TestSBB l_TestSBB;

    l_TestSBB();

    TestNEG l_TestNEG;

    l_TestNEG();

    TestrpsxSLTU l_TestrpsxSLTU;

    l_TestrpsxSLTU();

	TestCMOVS l_TestCMOVS;

	l_TestCMOVS();

	TestCMOVE l_TestCMOVE;

	l_TestCMOVE();

	TestCMOVNS l_TestCMOVNS;

	l_TestCMOVNS();

	TestADD l_TestADD;

	l_TestADD();

	TestSHL_SHR_SAR l_TestSHL_SHR_SAR;

	l_TestSHL_SHR_SAR();

    TestFastCall l_TestFastCall;

    l_TestFastCall();

    TestOffsetIndex l_TestOffsetIndex;

    l_TestOffsetIndex();

	TestFastCall1 l_TestFastCall1;

	l_TestFastCall1();
}