//
// Created by evgen on 9/22/2020.
//


#include "PrecompiledHeader.h"
#ifdef ANDROID_ABI_V8A
#include "include/Arm64Emitter.h"
#include "../Emitters/arm64v8aEmitter/include/Arm64Emitter.h"
#else
#include "x86emitter/tools.h"
#include "x86emitter/x86types.h"
#include "x86emitter/instructions.h"
#include "x86emitter/legacy_types.h"
#include "x86emitter/legacy_instructions.h"
#endif
#include "vtlb.h"

using namespace x86Emitter;

typedef void DynGenFunc();

// Recompiled code buffer for EE recompiler dispatchers!
static u8 __pagealigned eeRecDispatchers[__pagesize];


static u32 __pagealigned memoryBank[100];
static u32 __pagealigned memoryBank1[100];

using namespace x86Emitter;



#define xmmT1 xmm0 // Used for regAlloc
#define xmmT2 xmm1 // Used for regAlloc
#define xmmT3 xmm2 // Used for regAlloc
#define xmmT4 xmm3 // Used for regAlloc
#define xmmT5 xmm4 // Used for regAlloc
#define xmmT6 xmm5 // Used for regAlloc
#define xmmT7 xmm6 // Used for regAlloc
#define xmmPQ xmm7 // Holds the Value and Backup Values of P and Q regs

static u32 s_stateTest;
static u32 s_stateTest1;
static u32 s_stateTest2;

static __aligned(64) vtlb_private::MapData vtlbdata1;


static u64 CallPtr(const void *ptr) {
    return ((u64(*)())ptr)();
}


class TestEnterRecompiledCode
{
    static void __fastcall recRecompile(const u32 startpc, const u32 startpc1)
    {
    }

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

		for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i;
        }

        xSetPtr(eeRecDispatchers);

        auto EnterRecompiledCode = _DynGen_Code();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());


        auto l_result = CallPtr((void *)EnterRecompiledCode);

        if(memoryBank1[0] != memoryBank[3] ||
           memoryBank1[1] != memoryBank[3] ||
           memoryBank1[2] != memoryBank[3] ||
           memoryBank1[3] != memoryBank[3])
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_Code()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

			xMOVAPS(xmmT1, ptr128[memoryBank]);
            xSHUF.PS(xmmT1, xmmT1, 255);
            xMOVAPS(ptr128[memoryBank1], xmmT1);

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestEnterRecompiledCode() {}
    void operator()() { execute(); }
};

class TestPCMPEQD
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i;
            memoryBank1[i] = i;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeEQD = _DynGen_CodeEQD();

        auto DynGen_CodeNoneEQD = _DynGen_CodeNoneEQD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());


        auto l_result = CallPtr((void *)DynGen_CodeEQD);



        if(0xFFFFFFFF != memoryBank[3] ||
                0xFFFFFFFF != memoryBank[3] ||
                0xFFFFFFFF != memoryBank[3] ||
                0xFFFFFFFF != memoryBank[3])
        {
            throw L"Unimplemented!!!";
        }

        l_result = CallPtr((void *)DynGen_CodeNoneEQD);



        if(0 != memoryBank[3] ||
           0 != memoryBank[3] ||
           0 != memoryBank[3] ||
           0 != memoryBank[3])
        {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeEQD()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOVAPS(xmmT1, ptr32[memoryBank]);
            xPCMP.EQD(xmmT1, ptr32[memoryBank]);
            xMOVAPS(ptr32[memoryBank], xmmT1);

        }

        xRET();

        return (DynGenFunc *)retval;
    }

    DynGenFunc *_DynGen_CodeNoneEQD()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOVAPS(xmmT1, ptr32[memoryBank]);
            xPCMP.EQD(xmmT1, ptr32[memoryBank1]);
            xMOVAPS(ptr32[memoryBank], xmmT1);

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPCMPEQD() {}
    void operator()() { execute(); }
};

class TestPAND
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = 0x09;
            memoryBank1[i] = 0x05;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePAND = _DynGen_CodePAND();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePAND);



        if(1 != memoryBank[3] ||
           1 != memoryBank[3] ||
           1 != memoryBank[3] ||
           1 != memoryBank[3])
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePAND()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOVAPS(xmmT1, ptr32[memoryBank]);
            xMOVAPS(xmmT2, ptr32[memoryBank1]);
            xPAND(xmmT1, xmmT2);
            xMOVAPS(ptr32[memoryBank], xmmT1);


        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPAND() {}
    void operator()() { execute(); }
};

class TestMOVMSKPS
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 25; i++) {
            memoryBank[i * 4] = -1;
            memoryBank[1 + i * 4] = -1;
            memoryBank[2 + i * 4] = 0;
            memoryBank[3 + i * 4] = -1;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeMOVMSKPS = _DynGen_CodeMOVMSKPS();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeMOVMSKPS);

        if(l_result != 11)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeMOVMSKPS()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOVAPS(xmmT1, ptr32[memoryBank]);
            xMOVMSKPS(eax, xmmT1);
            xMOV(ecx, eax);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOVMSKPS() {}
    void operator()() { execute(); }
};

class TestCMP
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        s_stateTest = 0xc;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeCMP = _DynGen_CodeCMP();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeCMP);

        if(l_result != 0)
        {
            throw L"Unimplemented!!!";
        }

        s_stateTest = 0xf;

        l_result = CallPtr((void *)DynGen_CodeCMP);

        if(l_result != 1)
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
            xMOV( eax, ptr[&s_stateTest] );
            xCMP(eax, 0xf);
			xMOV(eax, 1);
			auto jump = JE32(0);
			xMOV(eax, 0);
			x86SetJ32(jump);


            //g_Emitter.MOVI2R(Arm64Gen::ARM64Reg::X1, 1);
            //g_Emitter.CSEL(getXReg(eax), Arm64Gen::ARM64Reg::X1, Arm64Gen::ARM64Reg::ZR, CCFlags::CC_EQ);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestCMP() {}
    void operator()() { execute(); }
};

class TestForwardJL8
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        s_stateTest = 0xf;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeForwardJL = _DynGen_CodeForwardJL();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeForwardJL);

        if(l_result != 0)
        {
            throw L"Unimplemented!!!";
        }

        s_stateTest = 0xc;

        l_result = CallPtr((void *)DynGen_CodeForwardJL);

        if(l_result != 1)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeForwardJL()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif


			xMOV(ecx, 1);
            xMOV( eax, ptr[&s_stateTest] );
            xCMP(eax, 0xf);

            xForwardJL8 exitPoint;
			xMOV(ecx, 0);
            exitPoint.SetTarget();

            xMOV( eax, ecx);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestForwardJL8() {}
    void operator()() { execute(); }
};

class TestMOVZX
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        s_stateTest = 0xFEF;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeMOVZX = _DynGen_CodeMOVZX();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeMOVZX);

        if(l_result != 239)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeMOVZX()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV( eax, ptr[&s_stateTest] );

            xMOVZX( eax, al );
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOVZX() {}
    void operator()() { execute(); }
};

class TestMOVSX
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        s_stateTest = 0xFEF;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeMOVSX32 = _DynGen_CodeMOVSX32();

		auto DynGen_CodeMOVSX64 = _DynGen_CodeMOVSX64();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeMOVSX32);

		if (l_result != 0x00000000FFFFFFEF)
		{
			throw L"Unimplemented!!!";
		}

		l_result = CallPtr((void *)DynGen_CodeMOVSX64);

        if(l_result != 0xFFFFFFFFFFFFFFEF)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeMOVSX32()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV( eax, ptr[&s_stateTest] );

            xMOVSX( eax, al );
        }

        xRET();

        return (DynGenFunc *)retval;
    }

	DynGenFunc *_DynGen_CodeMOVSX64()
	{

		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			xMOV(eax, ptr[&s_stateTest]);

			xMOVSX(rax, al);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
    TestMOVSX() {}
    void operator()() { execute(); }
};

class TestMOVZX1
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        s_stateTest = 0xFEF;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeMOVZX = _DynGen_CodeMOVZX();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeMOVZX);

        if(l_result != 0xFEF)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeMOVZX()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV( eax, ptr[&s_stateTest] );

            xMOVZX( eax, ax );
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOVZX1() {}
    void operator()() { execute(); }
};

class TestMOVSX1
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        s_stateTest = 0xFFEF;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeMOVSX32 = _DynGen_CodeMOVSX32();

		auto DynGen_CodeMOVSX64 = _DynGen_CodeMOVSX64();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeMOVSX32);

        if(l_result != 0x00000000FFFFFFEF)
        {
            throw L"Unimplemented!!!";
        }

		l_result = CallPtr((void *)DynGen_CodeMOVSX64);

		if (l_result != 0xFFFFFFFFFFFFFFEF)
		{
			throw L"Unimplemented!!!";
		}
    }

    DynGenFunc *_DynGen_CodeMOVSX32()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV( eax, ptr[&s_stateTest] );

            xMOVSX( eax, ax );
        }

        xRET();

        return (DynGenFunc *)retval;
    }

	DynGenFunc *_DynGen_CodeMOVSX64()
	{

		u8 *retval = xGetAlignedCallTarget();

		{ // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
			xScopedStackFrame frame(true);
#else
			xScopedStackFrame frame(IsDevBuild);
#endif

			xMOV(eax, ptr[&s_stateTest]);

			xMOVSX(rax, ax);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
    TestMOVSX1() {}
    void operator()() { execute(); }
};

class TestSUB
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        s_stateTest = 0x10;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeSUB = _DynGen_CodeSUB();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeSUB);

        if(l_result != 2147483664)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeSUB()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV( eax, ptr[&s_stateTest] );

            xSUB( eax, 0x80000000 );
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestSUB() {}
    void operator()() { execute(); }
};

class TestSUB1
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        s_stateTest = 0x10;

        s_stateTest1 = 0x80000000;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeSUB = _DynGen_CodeSUB();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeSUB);

        if(l_result != 2147483664)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeSUB()
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV( eax, ptr[&s_stateTest] );

            xMOV( ecx, ptr[&s_stateTest1] );

            xSUB( eax, ecx );
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestSUB1() {}
    void operator()() { execute(); }
};

void CPUTest2Finc()
{
    TestEnterRecompiledCode l_TestEnterRecompiledCode;

    l_TestEnterRecompiledCode();

    TestPCMPEQD l_TestPCMPEQD;

    l_TestPCMPEQD();

    TestPAND l_TestPAND;

    l_TestPAND();

    TestMOVMSKPS l_TestMOVMSKPS;

    l_TestMOVMSKPS();

    TestCMP l_TestCMP;

    l_TestCMP();

    TestForwardJL8 l_TestForwardJL8;

    l_TestForwardJL8();

    TestMOVZX l_TestMOVZX;

    l_TestMOVZX();

    TestMOVSX l_TestMOVSX;

    l_TestMOVSX();

    TestMOVZX1 l_TestMOVZX1;

    l_TestMOVZX1();

    TestMOVSX1 l_TestMOVSX1;

    l_TestMOVSX1();

    TestSUB l_TestSUB;

    l_TestSUB();

    TestSUB1 l_TestSUB1;

    l_TestSUB1();
}