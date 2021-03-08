//
// Created by evgen on 9/7/2020.
//

#include "PrecompiledHeader.h"
#ifdef ANDROID_ABI_V8A
#include "include/Arm64Emitter.h"
#else
#include "x86emitter/tools.h"
#include "x86emitter/x86types.h"
#include "x86emitter/instructions.h"
#include "x86emitter/legacy_types.h"
#include "x86emitter/legacy_instructions.h"
#endif


using namespace x86Emitter;

typedef void DynGenFunc();


static DynGenFunc* ExitRecompiledCode = NULL;

struct cpuRegistersTest {
//    GPRregs GPR;		// GPR regs
//    // NOTE: don't change order since recompiler uses it
//    GPR_reg HI;
//    GPR_reg LO;			// hi & log 128bit wide
//    CP0regs CP0;		// is COP0 32bit?
    u32 sa;				// shift amount (32bit), needs to be 16 byte aligned
    u32 IsDelaySlot;	// set true when the current instruction is a delay slot.
    u32 pc;				// Program counter, when changing offset in struct, check iR5900-X.S to make sure offset is correct
    u32 code;			// current instruction
//    PERFregs PERF;
    u32 eCycle[32];
    u32 sCycle[32];		// for internal counters
    u32 cycle;			// calculate cpucycles..
    u32 interrupt;
    int branch;
    int opmode;			// operating mode
    u32 tempcycles;

	u64 Base;

	u32 state32Reg;
	u32 state32Reg1;
};

__aligned16 cpuRegistersTest cpuRegsTest;
static __aligned16 uptr recLUTTest[_64kb];

// Recompiled code buffer for EE recompiler dispatchers!
static u8 __pagealigned eeRecDispatchers[__pagesize];

static u64 s_stateTest;

void callbackFuncTest()
{

    return;
}

static u64 CallPtr(const void *ptr) {
    return ((u64(*)())ptr)();
}

class TestDynGen_DispatcherReg {

    void execute()
    {
        // In case init gets called multiple times:
        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ReadWrite() );

        // clear the buffer to 0xcc (easier debugging).
        memset( eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr( eeRecDispatchers );

//    Test DispatcherReg

        cpuRegsTest.pc = 2 << 16;
//    for (int i = 0; i < __pagesize; ++i) {
//        recLUTTest[i] = (uptr)callbackFuncTest;
//    }
        recLUTTest[2] = (uptr)callbackFuncTest;

        auto DispatcherReg_Test = DynGen_DispatcherReg_Test();

        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ExecOnly() );

        auto l_result = CallPtr((void *)DispatcherReg_Test);

        auto l_ref = (uptr)callbackFuncTest;

        if(l_result != l_ref
			&& cpuRegsTest.Base == (u64)recLUTTest
			)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc* DynGen_DispatcherReg_Test()
    {
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif


			xMOV(eax, ptr[&cpuRegsTest.pc]);
			xMOV(ebx, eax);
			xSHR(eax, 16);
			xMOV(rcx, ptrNative[xComplexAddress(rdx, recLUTTest, rax*wordsize)]);
			xMOV(ptrNative[&cpuRegsTest.Base], rdx);
			xMOV(rax, rcx);

		}
		
        xRET();
		
        return (DynGenFunc*)retval;
    }

public:
    TestDynGen_DispatcherReg()
    {
    }
    void operator()(){execute();}
};


class TestDynGen_FastCallOneArg {

    static u64 recRecompile( const u32 startpc)
    {
        if(startpc != cpuRegsTest.pc)
            throw L"Unimplemented!!!";

        return 99;
    }

    void execute()
    {
        // In case init gets called multiple times:
        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ReadWrite() );

        // clear the buffer to 0xcc (easier debugging).
        memset( eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr( eeRecDispatchers );

//    Test DispatcherReg

        s_stateTest = 0;

        recLUTTest[2] = (uptr)callbackFuncTest;

        cpuRegsTest.pc = 5;

        auto FastCall_Test = DynGen_FastCall_Test();

        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ExecOnly() );

        auto l_result = CallPtr((void *)FastCall_Test);

        if(l_result != 99)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc* DynGen_FastCall_Test()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif


            xMOV(ecx, cpuRegsTest.pc);

            xMOV(rax, 51);

            xFastCall((void*)recRecompile, ecx);

            xMOV(ptr[&s_stateTest], ecx);

        }

        xRET();
        return (DynGenFunc*)retval;
    }

public:
    TestDynGen_FastCallOneArg()
    {
    }

    void operator()(){execute();}
};


class TestDynGen_FastCallTwoArgs {

    static u64 recRecompile( const u32 startpc, const u32 startpc1 )
    {
        if(startpc != 4 || startpc1 != 2)
            throw L"Unimplemented!!!";

        return 99;
    }

    void execute()
    {
        // In case init gets called multiple times:
        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ReadWrite() );

        // clear the buffer to 0xcc (easier debugging).
        memset( eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr( eeRecDispatchers );

//    Test DispatcherReg
		
		cpuRegsTest.state32Reg = 0;
		cpuRegsTest.state32Reg1 = -1;

        recLUTTest[2] = (uptr)callbackFuncTest;
        auto FastCall_Test = DynGen_FastCall_Test();

        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ExecOnly() );

        auto l_result = CallPtr((void *)FastCall_Test);

        if(l_result != 99 && cpuRegsTest.state32Reg == cpuRegsTest.state32Reg1)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc* DynGen_FastCall_Test()
    {
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif


			xMOV(ecx, 5);

			xMOV(rax, 51);

			xMOV(ptr[&cpuRegsTest.state32Reg], ecx);

			xFastCall((void*)recRecompile, 4, 2);

			xMOV(ptr[&cpuRegsTest.Base], r8);

			xMOV(ptr[&cpuRegsTest.state32Reg1], ecx);

		}
		
        xRET();
        return (DynGenFunc*)retval;
    }

public:
    TestDynGen_FastCallTwoArgs()
    {
    }

    void operator()(){execute();}
};

class TestDynGen_JITCompile {

    static void __fastcall recRecompile( const u32 startpc)
    {

    }

    void execute()
    {
        // In case init gets called multiple times:
        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ReadWrite() );

        // clear the buffer to 0xcc (easier debugging).
        memset( eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr( eeRecDispatchers );

//    Test DispatcherReg

        cpuRegsTest.pc = 2 << 16;
        recLUTTest[2] = (uptr)callbackFuncTest;
        auto DispatcherReg_Test = DynGen_JITCompile_Test();

        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ExecOnly() );

        auto l_result = CallPtr((void *)DispatcherReg_Test);

        if(l_result != (uptr)callbackFuncTest)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc* DynGen_JITCompile_Test()
    {
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			xFastCall((void*)recRecompile, ptr32[&cpuRegsTest.pc]);

			// C equivalent:
			// u32 addr = cpuRegs.pc;
			// void(**base)() = (void(**)())recLUT[addr >> 16];
			// base[addr >> 2]();
			xMOV(eax, ptr[&cpuRegsTest.pc]);
			xMOV(ebx, eax);
			xSHR(eax, 16);
			xMOV(rcx, ptrNative[xComplexAddress(rcx, recLUTTest, rax*wordsize)]);
			//        xJMP( ptrNative[rbx*(wordsize/4) + rcx] );
			xMOV(rax, rcx);

		}
		
        xRET();

        return (DynGenFunc*)retval;
    }

public:
    TestDynGen_JITCompile()
    {
    }

    void operator()(){execute();}
};


class TestDynGen_JITCompileInBlock {

    static void __fastcall recRecompile( const u32 startpc )
    {

    }

    void execute()
    {
        // In case init gets called multiple times:
        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ReadWrite() );

        // clear the buffer to 0xcc (easier debugging).
        memset( eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr( eeRecDispatchers );

//    Test DispatcherReg

        cpuRegsTest.pc = 2 << 16;
//    for (int i = 0; i < __pagesize; ++i) {
//        recLUTTest[i] = (uptr)callbackFuncTest;
//    }
        recLUTTest[2] = (uptr)callbackFuncTest;
        auto DispatcherReg_Test = DynGen_JITCompileInBlock_Test();

        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ExecOnly() );

        auto l_result = CallPtr((void *)DispatcherReg_Test);

        if(l_result != (uptr)callbackFuncTest)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc* DynGen_JITCompile_Test()
    {
        //   u8* retval = xGetPtr();		// fallthrough target, can't align it!

        u8* retval = xGetAlignedCallTarget();

        xFastCall((void*)recRecompile, ptr32[&cpuRegsTest.pc] );

        // C equivalent:
        // u32 addr = cpuRegs.pc;
        // void(**base)() = (void(**)())recLUT[addr >> 16];
        // base[addr >> 2]();
        xMOV( eax, ptr[&cpuRegsTest.pc] );
        xMOV( ebx, eax );
        xSHR( eax, 16 );
        xMOV( rcx, ptrNative[xComplexAddress(rcx, recLUTTest, rax*wordsize)] );
//        xJMP( ptrNative[rbx*(wordsize/4) + rcx] );
        xMOV( rax, rcx );

		xMOV(rcx, ptrNative[&ExitRecompiledCode]);

		xJMP(rcx);

        return (DynGenFunc*)retval;
    }

    DynGenFunc* DynGen_JITCompileInBlock_Test()
    {
        auto JITCompile = DynGen_JITCompile_Test();
		
		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			xJMP((void*)JITCompile);


			ExitRecompiledCode = (DynGenFunc*)xGetPtr();

		}

		xRET();
		
        return (DynGenFunc*)retval;
    }

public:
    TestDynGen_JITCompileInBlock()
    {
    }

    void operator()(){execute();}
};


class TestDynGen_EnterRecompiledCode {

    static void __fastcall recRecompile( const u32 startpc )
    {

    }
	
    void execute()
    {
        // In case init gets called multiple times:
        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ReadWrite() );

        // clear the buffer to 0xcc (easier debugging).
        memset( eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr( eeRecDispatchers );

//    Test DispatcherReg

        cpuRegsTest.pc = 2 << 16;
//    for (int i = 0; i < __pagesize; ++i) {
//        recLUTTest[i] = (uptr)callbackFuncTest;
//    }
        recLUTTest[2] = (uptr)callbackFuncTest;
        auto DispatcherReg_Test = DynGen_EnterRecompiledCode_Test();

        HostSys::MemProtectStatic( eeRecDispatchers, PageAccess_ExecOnly() );

        auto l_result = CallPtr((void *)DispatcherReg_Test);

        if(l_result != (uptr)callbackFuncTest)
        {
            throw L"Unimplemented!!!";
        }
    }

// called when jumping to variable pc address
    DynGenFunc* _DynGen_DispatcherReg()
    {
        u8* retval = xGetPtr();		// fallthrough target, can't align it!


        // C equivalent:
        // u32 addr = cpuRegs.pc;
        // void(**base)() = (void(**)())recLUT[addr >> 16];
        // base[addr >> 2]();
        xMOV( eax, ptr[&cpuRegsTest.pc] );
        xMOV( ebx, eax );
        xSHR( eax, 16 );
        xMOV( rcx, ptrNative[xComplexAddress(rcx, recLUTTest, rax*wordsize)] );
//        xJMP( ptrNative[rbx*(wordsize/4) + rcx] );
        xMOV( rax, rcx );

		xMOV(rcx, ptrNative[&ExitRecompiledCode]);

		xJMP(rcx);

        return (DynGenFunc*)retval;
    }

    DynGenFunc* DynGen_EnterRecompiledCode_Test()
    {

//        uint32_t regs_to_save = Arm64Gen::ALL_CALLEE_SAVED;
//        uint32_t regs_to_save_fp = Arm64Gen::ALL_CALLEE_SAVED_FP;
//
//        auto DispatcherReg	= _DynGen_DispatcherReg();
//
//        u8* retval = xGetAlignedCallTarget();
//
//        { // Properly scope the frame prologue/epilogue
//#ifdef ENABLE_VTUNE
//            xScopedStackFrame frame(true);
//#else
//            xScopedStackFrame frame(IsDevBuild);
//#endif
//
//            xJMP((void*)DispatcherReg);
//
//            // Save an exit point
//            ExitRecompiledCode = (DynGenFunc*)xGetPtr();
//        }
//
//        xRET();
		
        auto DispatcherReg = _DynGen_DispatcherReg();
				
        xRET();

        u8* retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xJMP((void*)DispatcherReg);

            // Save an exit point
            ExitRecompiledCode = (DynGenFunc*)xGetPtr();
        }

        xRET();

        return (DynGenFunc*)retval;
    }

public:
    TestDynGen_EnterRecompiledCode()
    {
    }

    void operator()(){execute();}
};








void CPUTestFinc()
{
    TestDynGen_DispatcherReg l_TestDynGen_DispatcherReg;

    l_TestDynGen_DispatcherReg();

    TestDynGen_FastCallOneArg l_TestDynGen_FastCallOneArg;

    l_TestDynGen_FastCallOneArg();

    TestDynGen_FastCallTwoArgs l_TestDynGen_FastCallTwoArgs;

    l_TestDynGen_FastCallTwoArgs();

    TestDynGen_JITCompile l_TestDynGen_JITCompile;

    l_TestDynGen_JITCompile();

    TestDynGen_JITCompileInBlock l_TestDynGen_JITCompileInBlock;

    l_TestDynGen_JITCompileInBlock();

    TestDynGen_EnterRecompiledCode l_TestDynGen_EnterRecompiledCode;

    l_TestDynGen_EnterRecompiledCode();
}