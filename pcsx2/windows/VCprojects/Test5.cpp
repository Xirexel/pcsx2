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

__aligned16 cpuRegistersTest cpuRegsTest2;
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

void __fastcall WriteCP0Status1(u32 value) {

		if (value != -1)
		{
			throw L"Unimplemented!!!";
		}
}

class TestFastCall2
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		for (size_t i = 0; i < 32; i++) {
			cpuRegsTest2.eCycle[i] = 0;
		}

		cpuRegsTest2.eCycle[0] = -1;

		auto DynGen_Code = _DynGen_CodeFastCall1();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());




		CallPtr((void *)DynGen_Code);
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
				
			xFastCall((void*)WriteCP0Status1, cpuRegsTest2.eCycle[0]);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestFastCall2()
	{
	}
	void operator()() { execute(); }
};

class TestINC
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);
		
		cpuRegsTest2.UL = 100;

		auto DynGen_Code = _DynGen_CodeFastCall1();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());			

		auto l_result = CallPtr((void *)DynGen_Code);

		if(l_result != cpuRegsTest2.UL + 1)
			throw L"Unimplemented!!!";
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

			xMOV(eax, cpuRegsTest2.UL);
			xINC(eax);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestINC()
	{
	}
	void operator()() { execute(); }
};

class TestADD1
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		cpuRegsTest2.UL = 100;

		u32 l_v = 235;

		cpuRegsTest2.m_value = l_v;

		auto DynGen_Code = _DynGen_CodeFastCall1();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		auto l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.m_value != (cpuRegsTest2.UL + l_v))
			throw L"Unimplemented!!!";
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

			xMOV(eax, cpuRegsTest2.UL);
			xADD(ptr[&cpuRegsTest2.m_value], eax);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestADD1()
	{
	}
	void operator()() { execute(); }
};

class TestSUB2
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		cpuRegsTest2.UL = 100;

		u32 l_v = 235;

		cpuRegsTest2.m_value = l_v;

		auto DynGen_Code = _DynGen_CodeFastCall1();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		auto l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.m_value != (l_v - cpuRegsTest2.UL))
			throw L"Unimplemented!!!";
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

			xMOV(eax, ptr32[&cpuRegsTest2.UL]);
			xSUB(ptr32[&cpuRegsTest2.m_value], eax);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestSUB2()
	{
	}
	void operator()() { execute(); }
};

class TestUDIV
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		cpuRegsTest2.UL = 100;
		
		cpuRegsTest2.m_value = 100;

		auto DynGen_Code = _DynGen_CodeFastCall1();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		auto l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 1 ||
			cpuRegsTest2.mResult != 0)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 543;

		cpuRegsTest2.m_value = 100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 5 ||
			cpuRegsTest2.mResult != 43)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 100;

		cpuRegsTest2.m_value = 543;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 0 ||
			cpuRegsTest2.mResult != 100)
			throw L"Unimplemented!!!";
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

			xMOV(eax, ptr32[&cpuRegsTest2.UL]);
			xMOV(ecx, ptr32[&cpuRegsTest2.m_value]);
			xXOR(edx, edx);
			xUDIV(ecx);

			xMOV(ptr32[&cpuRegsTest2.mValue], eax);
			xMOV(ptr32[&cpuRegsTest2.mResult], edx);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestUDIV()
	{
	}
	void operator()() { execute(); }
};

class TestDIV
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		cpuRegsTest2.UL = 100;

		cpuRegsTest2.m_value = 100;

		auto DynGen_Code = _DynGen_CodeFastCall1();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		auto l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 1 ||
			cpuRegsTest2.mResult != 0)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 543;

		cpuRegsTest2.m_value = 100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 5 ||
			cpuRegsTest2.mResult != 43)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 100;

		cpuRegsTest2.m_value = 543;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 0 ||
			cpuRegsTest2.mResult != 100)
			throw L"Unimplemented!!!";


		cpuRegsTest2.UL = -100;

		cpuRegsTest2.m_value = 100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != -1 ||
			cpuRegsTest2.mResult != 0)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -543;

		cpuRegsTest2.m_value = 100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != -5 ||
			cpuRegsTest2.mResult != -43)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -100;

		cpuRegsTest2.m_value = 543;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 0 ||
			cpuRegsTest2.mResult != -100)
			throw L"Unimplemented!!!";


		cpuRegsTest2.UL = -100;

		cpuRegsTest2.m_value = -100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 1 ||
			cpuRegsTest2.mResult != 0)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -543;

		cpuRegsTest2.m_value = -100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 5 ||
			cpuRegsTest2.mResult != -43)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -100;

		cpuRegsTest2.m_value = -543;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 0 ||
			cpuRegsTest2.mResult != -100)
			throw L"Unimplemented!!!";
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

			xMOV(eax, ptr32[&cpuRegsTest2.UL]);
			xMOV(ecx, ptr32[&cpuRegsTest2.m_value]);

			xCDQ();
			xDIV(ecx);

			xMOV(ptr32[&cpuRegsTest2.mValue], eax);
			xMOV(ptr32[&cpuRegsTest2.mResult], edx);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestDIV()
	{
	}
	void operator()() { execute(); }
};

class TestAND_OR_XOR
{
public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		cpuRegsTest2.UL = 0xF0F;

		cpuRegsTest2.m_value = 0xFFF;

		auto DynGen_CodeAND = _DynGen_CodeAND();

		auto DynGen_CodeOR = _DynGen_CodeOR();

		auto DynGen_CodeXOR = _DynGen_CodeXOR();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

		auto l_result = CallPtr((void *)DynGen_CodeAND);

		if (l_result != 0xF0F)
			throw L"Unimplemented!!!";
		
		l_result = CallPtr((void *)DynGen_CodeOR);

		if (l_result != 0xFFF)
			throw L"Unimplemented!!!";
		
		l_result = CallPtr((void *)DynGen_CodeXOR);

		if (l_result != 0x0F0)
			throw L"Unimplemented!!!";

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


			xMOV(eax, ptr32[&cpuRegsTest2.UL]);
			xAND(eax, ptr32[&cpuRegsTest2.m_value]); 

		}

		xRET();

		return (DynGenFunc *)retval;
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


			xMOV(eax, ptr32[&cpuRegsTest2.UL]);
			xOR(eax, ptr32[&cpuRegsTest2.m_value]);
		}

		xRET();

		return (DynGenFunc *)retval;
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


			xMOV(eax, ptr32[&cpuRegsTest2.UL]);
			xXOR(eax, ptr32[&cpuRegsTest2.m_value]);

		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestAND_OR_XOR()
	{
	}
	void operator()() { execute(); }
};


class TestMUL
{
public:

    void execute()
    {

        // In case init gets called multiple times:
        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

        // clear the buffer to 0xcc (easier debugging).
        memset(eeRecDispatchers, 0xcc, __pagesize);

        xSetPtr(eeRecDispatchers);

        cpuRegsTest2.UL = 100;

        cpuRegsTest2.m_value = 100;

        auto DynGen_Code = _DynGen_CodeFastCall1();

        HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());

        auto l_result = CallPtr((void *)DynGen_Code);

        if (cpuRegsTest2.mValue != (cpuRegsTest2.UL * cpuRegsTest2.m_value) ||
            cpuRegsTest2.mResult != 0)
            throw L"Unimplemented!!!";

        cpuRegsTest2.UL = 543000;

        cpuRegsTest2.m_value = 100000;

        l_result = CallPtr((void *)DynGen_Code);

        if (cpuRegsTest2.mValue != 2760392448 ||
            cpuRegsTest2.mResult != 12)
            throw L"Unimplemented!!!";

        cpuRegsTest2.UL = 100000;

        cpuRegsTest2.m_value = 543000;

        l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 2760392448 ||
			cpuRegsTest2.mResult != 12)
			throw L"Unimplemented!!!";




		cpuRegsTest2.UL = -100;

		cpuRegsTest2.m_value = 100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 4294957296 ||
			cpuRegsTest2.mResult != 4294967295)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -543000;

		cpuRegsTest2.m_value = 100000;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 1534574848 ||
			cpuRegsTest2.mResult != 4294967283)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -100000;

		cpuRegsTest2.m_value = 543000;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 1534574848 ||
			cpuRegsTest2.mResult != 4294967283)
			throw L"Unimplemented!!!";




		cpuRegsTest2.UL = -100;

		cpuRegsTest2.m_value = -100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != (cpuRegsTest2.UL * cpuRegsTest2.m_value) ||
			cpuRegsTest2.mResult != 0)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -543000;

		cpuRegsTest2.m_value = -100000;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 2760392448 ||
			cpuRegsTest2.mResult != 12)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -100000;

		cpuRegsTest2.m_value = -543000;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 2760392448 ||
			cpuRegsTest2.mResult != 12)
			throw L"Unimplemented!!!";




		cpuRegsTest2.UL = 100;

		cpuRegsTest2.m_value = -100;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 4294957296 ||
			cpuRegsTest2.mResult != 4294967295)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 543000;

		cpuRegsTest2.m_value = -100000;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 1534574848 ||
			cpuRegsTest2.mResult != 4294967283)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 100000;

		cpuRegsTest2.m_value = -543000;

		l_result = CallPtr((void *)DynGen_Code);

		if (cpuRegsTest2.mValue != 1534574848 ||
			cpuRegsTest2.mResult != 4294967283)
			throw L"Unimplemented!!!";
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

            xMOV(eax, ptr32[&cpuRegsTest2.UL]);
            
            xMUL(ptr32[&cpuRegsTest2.m_value]);

            xMOV(ptr32[&cpuRegsTest2.mValue], eax);
            xMOV(ptr32[&cpuRegsTest2.mResult], edx);
        }

        xRET();

        return (DynGenFunc *)retval;
    }

public:
    TestMUL()
    {
    }
    void operator()() { execute(); }
};


class TestJA8
{
	s32 m_threashold;

public:

	void execute()
	{

		// In case init gets called multiple times:
		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ReadWrite());

		// clear the buffer to 0xcc (easier debugging).
		memset(eeRecDispatchers, 0xcc, __pagesize);

		xSetPtr(eeRecDispatchers);

		m_threashold = 10;

		cpuRegsTest2.UL = 10;
		
		auto DynGen_Code = _DynGen_CodeFastCall1();

		m_threashold = -10;

		auto DynGen_Code_Min = _DynGen_CodeFastCall1();

		HostSys::MemProtectStatic(eeRecDispatchers, PageAccess_ExecOnly());
					   
		auto l_result = CallPtr((void *)DynGen_Code);
		
		if (l_result != 2)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 9;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != 2)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 11;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != 1)
			throw L"Unimplemented!!!";





		cpuRegsTest2.UL = 10;

		l_result = CallPtr((void *)DynGen_Code_Min);

		if (l_result != 2)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 9;

		l_result = CallPtr((void *)DynGen_Code_Min);

		if (l_result != 2)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 11;

		l_result = CallPtr((void *)DynGen_Code_Min);

		if (l_result != 2)
			throw L"Unimplemented!!!";






		cpuRegsTest2.UL = -10;

		l_result = CallPtr((void *)DynGen_Code_Min);

		if (l_result != 2)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -9;

		l_result = CallPtr((void *)DynGen_Code_Min);

		if (l_result != 1)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = -11;

		l_result = CallPtr((void *)DynGen_Code_Min);

		if (l_result != 2)
			throw L"Unimplemented!!!";

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
			xMOV(eax, 1);
			xCMP(ptr32[&cpuRegsTest2.UL], (s32)m_threashold);
			auto jump = JA8(0);
			xMOV(eax, 2);
			x86SetJ8(jump);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestJA8()
	{
	}
	void operator()() { execute(); }
};

class TestJNE8_1
{
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

		cpuRegsTest2.UL = 0x80000000;

		cpuRegsTest2.m_value = 0xffffffff;

		auto l_result = CallPtr((void *)DynGen_Code);
		
		if (l_result != 3)
			throw L"Unimplemented!!!";
		
		cpuRegsTest2.m_value = 0xfffffffe;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != 2)
			throw L"Unimplemented!!!";

		cpuRegsTest2.UL = 11;

		l_result = CallPtr((void *)DynGen_Code);

		if (l_result != 1)
			throw L"Unimplemented!!!";	 
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

			xMOV(eax, 1);
			xMOV(ecx, ptr32[&cpuRegsTest2.UL]);
			xCMP(ecx, 0x80000000);
			auto cont1 = JNE8(0);
			xMOV(eax, 2);
			xMOV(edx, ptr32[&cpuRegsTest2.m_value]);
			xCMP(edx, 0xffffffff);
			auto cont2 = JNE8(0);
			//overflow case:
			xMOV(eax, 3);

			x86SetJ8(cont1);
			x86SetJ8(cont2);
		}

		xRET();

		return (DynGenFunc *)retval;
	}

public:
	TestJNE8_1()
	{
	}
	void operator()() { execute(); }
};


void CPUTest5Finc()
{
	TestFastCall2 l_TestFastCall2;

	l_TestFastCall2();

	TestINC l_TestINC;

	l_TestINC();

	TestADD1 l_TestADD1;

	l_TestADD1();

	TestSUB2 l_TestSUB2;

	l_TestSUB2();

	TestUDIV l_TestUDIV;

	l_TestUDIV();

	TestDIV l_TestDIV;

	l_TestDIV();

	TestAND_OR_XOR l_TestAND_OR_XOR;

	l_TestAND_OR_XOR();

    TestMUL l_TestMUL;

    l_TestMUL();

	TestJA8 l_TestJA8;

	l_TestJA8();

	TestJNE8_1 l_TestJNE8_1;

	l_TestJNE8_1();
}