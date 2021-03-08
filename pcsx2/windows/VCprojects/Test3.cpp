//
// Created by evgen on 9/27/2020.
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
static u32 __pagealigned memoryBank2[100];

using namespace x86Emitter;

struct TestVariable
{
	u32 cpuRegsTest;

	u32 m_value = 0;

	u32 m_resultlow = 0;

	u32 m_resultHigh = 0;

	u32 UL[2];

	s16 cpuRegs_code;

	u32 checkResult;
};

static __aligned16 TestVariable s_TestVariable;


#define xmmT1 xmm0 // Used for regAlloc
#define xmmT2 xmm1 // Used for regAlloc
#define xmmT3 xmm2 // Used for regAlloc
#define xmmT4 xmm3 // Used for regAlloc
#define xmmT5 xmm4 // Used for regAlloc
#define xmmT6 xmm5 // Used for regAlloc
#define xmmT7 xmm6 // Used for regAlloc
#define xmmPQ xmm7 // Holds the Value and Backup Values of P and Q regs

#define _v0 0
#define _v1 0x55
#define _v2 0xaa
#define _v3 0xff

static u64 CallPtr(const void *ptr) {
    return ((u64(*)())ptr)();
}


class TestPSHUFD{



    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i;
            memoryBank1[i] = 0;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD0 = _DynGen_CodePSHUFD(_v0);

        auto DynGen_CodePSHUFD1 = _DynGen_CodePSHUFD(_v1);

        auto DynGen_CodePSHUFD2 = _DynGen_CodePSHUFD(_v2);

        auto DynGen_CodePSHUFD3 = _DynGen_CodePSHUFD(_v3);

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD0);

        if(memoryBank1[0] != memoryBank[0] ||
           memoryBank1[1] != memoryBank[0] ||
           memoryBank1[2] != memoryBank[0] ||
           memoryBank1[3] != memoryBank[0])
        {
            throw L"Unimplemented!!!";
        }

        for (size_t i = 0; i < 100; i++) {
            memoryBank1[i] = 0;
        }


        l_result = CallPtr((void *)DynGen_CodePSHUFD1);

        if(memoryBank1[0] != memoryBank[1] ||
           memoryBank1[1] != memoryBank[1] ||
           memoryBank1[2] != memoryBank[1] ||
           memoryBank1[3] != memoryBank[1])
        {
            throw L"Unimplemented!!!";
        }

        for (size_t i = 0; i < 100; i++) {
            memoryBank1[i] = 0;
        }


        l_result = CallPtr((void *)DynGen_CodePSHUFD2);

        if(memoryBank1[0] != memoryBank[2] ||
           memoryBank1[1] != memoryBank[2] ||
           memoryBank1[2] != memoryBank[2] ||
           memoryBank1[3] != memoryBank[2])
        {
            throw L"Unimplemented!!!";
        }

        for (size_t i = 0; i < 100; i++) {
            memoryBank1[i] = 0;
        }


        l_result = CallPtr((void *)DynGen_CodePSHUFD3);

        if(memoryBank1[0] != memoryBank[3] ||
           memoryBank1[1] != memoryBank[3] ||
           memoryBank1[2] != memoryBank[3] ||
           memoryBank1[3] != memoryBank[3])
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD(u8 order)
    {

        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif


            xMOVUPS(xmmT1, ptr32[memoryBank]);
            xPSHUF.D(xmmT2, xmmT1, order);
            xMOVUPS(ptr32[memoryBank1], xmmT2);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPSHUFD() {}
    void operator()() { execute(); }
};


class TestPMOVZXWD
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 50; i++) {
            memoryBank[i* 2] = 0xEFFF;
            memoryBank1[i * 2] = 0;
            memoryBank[1 + i * 2] = 0xEFFF;
            memoryBank1[1 + i * 2] = 0;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if(memoryBank1[0] != 0xefff ||
           memoryBank1[1] != 0      ||
           memoryBank1[2] != 0xefff ||
           memoryBank1[3] != 0)
        {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xPMOVZX.WD(xmmT1, ptr64[memoryBank]);
            xMOVUPS(ptr32[memoryBank1], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPMOVZXWD() {}
    void operator()() { execute(); }
};

class TestPMOVSXWD
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 50; i++) {
            memoryBank[i* 2] = 0xEFFF;
            memoryBank1[i * 2] = 0;
            memoryBank[1 + i * 2] = 0xEFFF;
            memoryBank1[1 + i * 2] = 0;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != 0xffffefff ||
            memoryBank1[1] != 0 ||
            memoryBank1[2] != 0xffffefff ||
            memoryBank1[3] != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xPMOVSX.WD(xmmT1, ptr64[memoryBank]);
            xMOVUPS(ptr32[memoryBank1], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPMOVSXWD() {}
    void operator()() { execute(); }
};




class TestPMOVZXBD
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 50; i++) {
            memoryBank[i * 2] = 0xEF00FF;
            memoryBank1[i * 2] = 0;
            memoryBank[1 + i * 2] = 0;
            memoryBank1[1 + i * 2] = 0;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != 0xff ||
            memoryBank1[1] != 0 ||
            memoryBank1[2] != 0xef ||
            memoryBank1[3] != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xPMOVZX.BD(xmmT1, ptr32[memoryBank]);
            xMOVUPS(ptr32[memoryBank1], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPMOVZXBD() {}
    void operator()() { execute(); }
};

class TestPMOVSXBD
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 50; i++) {
            memoryBank[i * 2] = 0xEF00FF;
            memoryBank1[i * 2] = 0;
            memoryBank[1 + i * 2] = 0;
            memoryBank1[1 + i * 2] = 0;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != 0xffffffff ||
            memoryBank1[1] != 0 ||
            memoryBank1[2] != 0xffffffef ||
            memoryBank1[3] != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xPMOVSX.BD(xmmT1, ptr32[memoryBank]);
            xMOVUPS(ptr32[memoryBank1], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPMOVSXBD() {}
    void operator()() { execute(); }
};



class TestxANDPS{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = 9;
            memoryBank1[i] = 0;
            memoryBank2[i] = 5;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != 1 ||
            memoryBank1[1] != 1 ||
            memoryBank1[2] != 1 ||
            memoryBank1[3] != 1) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr32[memoryBank]);

            xAND.PS(xmmT1, ptr128[memoryBank2]);

            xMOVUPS(ptr32[memoryBank1], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestxANDPS() {}
    void operator()() { execute(); }
};


#define xMOV8(regX, loc) xMOVSSZX(regX, loc)
#define xMOV16(regX, loc) xMOVSSZX(regX, loc)
#define xMOV32(regX, loc) xMOVSSZX(regX, loc)
#define xMOV64(regX, loc) xMOVUPS(regX, loc)
#define xMOV128(regX, loc) xMOVUPS(regX, loc)

class TestMOV16
{
    xAddressVoid srcIndirect;

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = 9;
            memoryBank1[i] = 4;
            memoryBank2[i] = 5;
        }

		s_TestVariable.cpuRegsTest = 80000;

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != s_TestVariable.cpuRegsTest ||
            memoryBank1[1] != 0 ||
            memoryBank1[2] != 0 ||
            memoryBank1[3] != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr64[memoryBank1]);

            xLEA(rdx, ptrNative[&s_TestVariable.cpuRegsTest]);

            xMOV16(xmmT1, ptr32[srcIndirect]);

            xMOVUPS(ptr64[memoryBank1], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOV16():
            srcIndirect(rdx)
    {
    }
    void operator()() { execute(); }
};


class TestPSLLD
{
    xAddressVoid srcIndirect;

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = 2;
            memoryBank1[i] = 4;
            memoryBank2[i] = 5;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != 16 ||
            memoryBank1[1] != 16 ||
            memoryBank1[2] != 16 ||
            memoryBank1[3] != 16) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr32[memoryBank]);

            xPSLL.D(xmmT1, 3);

            xMOVUPS(ptr32[memoryBank1], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPSLLD()
            : srcIndirect(edx)
    {
    }
    void operator()() { execute(); }
};


class TestMOVAPS1
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i;
            memoryBank1[i] = 4;
            memoryBank2[i] = 5;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != memoryBank[0] ||
            memoryBank1[1] != memoryBank[1] ||
            memoryBank1[2] != memoryBank[2] ||
            memoryBank1[3] != memoryBank[3]) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr32[memoryBank]);

            xMOVAPS(xmmT2, xmmT1);

            xMOVUPS(ptr32[memoryBank1], xmmT2);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOVAPS1()
    {
    }
    void operator()() { execute(); }
};


class TestPSRLD
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = 16;
            memoryBank1[i] = 4;
            memoryBank2[i] = 5;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != 2 ||
            memoryBank1[1] != 2 ||
            memoryBank1[2] != 2 ||
            memoryBank1[3] != 2) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr32[memoryBank]);

            xPSRL.D(xmmT1, 3);

            xMOVUPS(ptr32[memoryBank1], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPSRLD()
    {
    }
    void operator()() { execute(); }
};





class TestMOVHLPS{

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i;
            memoryBank1[i] = 4;
            memoryBank2[i] = 5;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != memoryBank[2] ||
            memoryBank1[1] != memoryBank[3] ||
            memoryBank1[2] != 0 ||
            memoryBank1[3] != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr32[memoryBank]);

            xMOVHL.PS(xmmT2, xmmT1);

            xMOVUPS(ptr32[memoryBank1], xmmT2);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOVHLPS()
    {
    }
    void operator()() { execute(); }
};


class TestMOVSS
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i + 1;
            memoryBank1[i] = 40;
            memoryBank2[i] = 50;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != memoryBank[0] ||
            memoryBank1[1] != 0 ||
            memoryBank1[2] != 0 ||
            memoryBank1[3] != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr32[memoryBank]);

            xMOVSS(xmmT2, xmmT1);

            xMOVUPS(ptr32[memoryBank1], xmmT2);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOVSS()
    {
    }
    void operator()() { execute(); }
};


class TestMOVSD
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i + 1;
            memoryBank1[i] = 40;
            memoryBank2[i] = 50;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank1[0] != memoryBank[0] ||
            memoryBank1[1] != memoryBank[1] ||
            memoryBank1[2] != 0 ||
            memoryBank1[3] != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr32[memoryBank]);

            xMOVSD(xmmT2, xmmT1);

            xMOVUPS(ptr32[memoryBank1], xmmT2);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOVSD()
    {
    }
    void operator()() { execute(); }
};


class TestMOVDZX
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i + 1;
            memoryBank1[i] = 40;
            memoryBank2[i] = 50;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeMOVDZX = _DynGen_CodeMOVDZX();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeMOVDZX);

        if (memoryBank1[0] != memoryBank[0] ||
            memoryBank1[1] != 0 ||
            memoryBank1[2] != 0 ||
            memoryBank1[3] != 0) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeMOVDZX()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVDZX(xmmT2, ptr32[memoryBank]);

            xMOVUPS(ptr32[memoryBank1], xmmT2);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMOVDZX()
    {
    }
    void operator()() { execute(); }
};


class TestBLENDPS
{
    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = i + 1;
            memoryBank1[i] = 40;
            memoryBank2[i] = 50;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeBLENDPS = _DynGen_CodeBLENDPS();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeBLENDPS);

        if (memoryBank2[0] != memoryBank[0] ||
                memoryBank2[1] != memoryBank1[1] ||
                memoryBank2[2] != memoryBank[2] ||
                memoryBank2[3] != memoryBank1[3]) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeBLENDPS()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif
            xMOVUPS(xmmT1, ptr32[memoryBank]);

            xMOVUPS(xmmT2, ptr32[memoryBank1]);

            xBLEND.PS(xmmT2, xmmT1, 5);

            xMOVUPS(ptr32[memoryBank2], xmmT2);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestBLENDPS()
    {
    }
    void operator()() { execute(); }
};




class TestPAND1
{

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = 9;
            memoryBank1[i] = 5;
            memoryBank2[i] = 10;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank2[0] != 1 ||
            memoryBank2[1] != 1 ||
            memoryBank2[2] != 1 ||
            memoryBank2[3] != 1) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOVAPS(xmmT1, ptr32[memoryBank]);

            xPAND(xmmT1, ptr32[memoryBank1]);

            xMOVUPS(ptr32[memoryBank2], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPAND1()
    {
    }
    void operator()() { execute(); }
};




class TestPOR
{

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        for (size_t i = 0; i < 100; i++) {
            memoryBank[i] = 9;
            memoryBank1[i] = 5;
            memoryBank2[i] = 10;
        }

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodePSHUFD = _DynGen_CodePSHUFD();

        auto DynGen_Code1 = _DynGen_Code1();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodePSHUFD);

        if (memoryBank2[0] != 13 ||
            memoryBank2[1] != 13 ||
            memoryBank2[2] != 13 ||
            memoryBank2[3] != 13) {
            throw L"Unimplemented!!!";
        }

        for (size_t i = 0; i < 100; i++) {
            memoryBank2[i] = 10;
        }

        l_result = CallPtr((void *)DynGen_Code1);

        if (memoryBank2[0] != 13 ||
            memoryBank2[1] != 13 ||
            memoryBank2[2] != 13 ||
            memoryBank2[3] != 13) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodePSHUFD()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOVAPS(xmmT1, ptr32[memoryBank]);

            xPOR(xmmT1, ptr32[memoryBank1]);

            xMOVUPS(ptr32[memoryBank2], xmmT1);
        }

        xRET();

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

            xMOVAPS(xmmT1, ptr32[memoryBank]);

            xMOVAPS(xmmT2, ptr32[memoryBank1]);

            xPOR(xmmT1, xmmT2);

            xMOVUPS(ptr32[memoryBank2], xmmT1);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestPOR()
    {
    }
    void operator()() { execute(); }
};

class TestCDQ
{

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeCDQ = _DynGen_CodeCDQ();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		s_TestVariable.m_value = 0x80000000;

        auto l_result = CallPtr((void *)DynGen_CodeCDQ);

        if (0xFFFFFFFF != s_TestVariable.m_resultHigh || s_TestVariable.m_value != s_TestVariable.m_resultlow){
            throw L"Unimplemented!!!";
        }

		s_TestVariable.m_value = 0;

        l_result = CallPtr((void *)DynGen_CodeCDQ);

        if (s_TestVariable.m_resultHigh != 0 || s_TestVariable.m_value != s_TestVariable.m_resultlow) {
            throw L"Unimplemented!!!";
        }
    }

    DynGenFunc *_DynGen_CodeCDQ()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            xMOV( eax, ptr[&s_TestVariable.m_value] );
            xCDQ();
            xMOV(ptr[&s_TestVariable.m_resultlow], eax );
            xMOV(ptr[&s_TestVariable.m_resultHigh], edx );

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestCDQ()
    {
    }
    void operator()() { execute(); }
};


class TestSLTI
{
public:
    std::shared_ptr<x86Emitter::xForwardJumpBase> j8Ptr[32];


    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        auto DynGen_CodeSLTI = _DynGen_CodeSLTI();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_CodeSLTI);

        if (l_result != s_TestVariable.checkResult) {
            throw L"Unimplemented!!!";
        }

    }

    DynGenFunc *_DynGen_CodeSLTI()
    {
        u8 *retval = xGetAlignedCallTarget();

        { // Properly scope the frame prologue/epilogue
#ifdef ENABLE_VTUNE
            xScopedStackFrame frame(true);
#else
            xScopedStackFrame frame(IsDevBuild);
#endif

            // test silent hill if modding
            xMOV(eax, 1);

            xCMP(ptr32[&s_TestVariable.UL[1]], s_TestVariable.cpuRegs_code >= 0 ? 0 : 0xffffffff);
            j8Ptr[0] = JL8(0);
            xMOV(eax, 2);
            j8Ptr[2] = JG8(0);
            xMOV(eax, 3);

            xCMP(ptr32[&s_TestVariable.UL[0]], (s32)s_TestVariable.cpuRegs_code);
            j8Ptr[1] = JB8(0);
            xMOV(eax, 4);

            x86SetJ8(j8Ptr[2]);
            //xXOR(eax, eax);

            x86SetJ8(j8Ptr[0]);
            x86SetJ8(j8Ptr[1]);

            xMOV(ptr32[&s_TestVariable.UL[0]], eax);
            xMOV(ptr32[&s_TestVariable.UL[1]], 0);

        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestSLTI()
    {
    }
    void operator()() { execute(); }
};



void CPUTest3Finc()
{
    TestPSHUFD l_TestPSHUFD;

    l_TestPSHUFD();

    TestPMOVZXWD l_TestPMOVZXWD;

    l_TestPMOVZXWD();

    TestPMOVSXWD l_TestPMOVSXWD;

    l_TestPMOVSXWD();

    TestPMOVZXBD l_TestPMOVZXBD;

    l_TestPMOVZXBD();

    TestPMOVSXBD l_TestPMOVSXBD;

    l_TestPMOVSXBD();

    TestxANDPS l_TestxANDPS;

    l_TestxANDPS();


    TestMOV16 l_TestMOV16;

    l_TestMOV16();


    TestPSLLD l_TestPSLLD;

    l_TestPSLLD();


    TestMOVAPS1 l_TestMOVAPS1;

    l_TestMOVAPS1();


    TestPSRLD l_TestPSRLD;

    l_TestPSRLD();


    TestMOVHLPS l_TestMOVHLPS;

    l_TestMOVHLPS();


    TestMOVSS l_TestMOVSS;

    l_TestMOVSS();


    TestMOVSD l_TestMOVSD;

    l_TestMOVSD();

    TestMOVDZX l_TestMOVDZX;

    l_TestMOVDZX();

    TestPAND1 l_TestPAND1;

    l_TestPAND1();

    TestBLENDPS l_TestBLENDPS;

    l_TestBLENDPS();


    TestPOR l_TestPOR;

    l_TestPOR();

    TestCDQ l_TestCDQ;

    l_TestCDQ();




    TestSLTI l_TestSLTI;

	s_TestVariable.UL[0] = 0xffffffff;

	s_TestVariable.UL[1] = 0xffffffff;

	s_TestVariable.cpuRegs_code = -1;

	s_TestVariable.checkResult = 4;

    l_TestSLTI();

	s_TestVariable.UL[0] = 0xffffffff;

	s_TestVariable.UL[1] = 0xffffffff;

	s_TestVariable.cpuRegs_code = 1;

	s_TestVariable.checkResult = 1;

    l_TestSLTI();

	s_TestVariable.UL[0] = 0xffffffff;

	s_TestVariable.UL[1] = 1;

	s_TestVariable.cpuRegs_code = -1;

	s_TestVariable.checkResult = 2;

    l_TestSLTI();

	s_TestVariable.UL[0] = 0;

	s_TestVariable.UL[1] = 0;

	s_TestVariable.cpuRegs_code = 1;

	s_TestVariable.checkResult = 3;

    l_TestSLTI();
}