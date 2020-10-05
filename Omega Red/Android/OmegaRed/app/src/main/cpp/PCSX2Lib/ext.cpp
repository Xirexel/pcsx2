//
// Created by Xirexel on 7/22/2019.
//


#include <xmmintrin.h>
#include "../Emitters/armeabiv7emitter/include/Pcsx2Types.h"

extern unsigned int s_TotalVUCycles;

extern uptr s_callstack;

extern void* SuperVUGetProgram(u32 startpc, int vuindex);


union SSE_MXCSR1
{
	u32 bitmask;
	struct
	{
		u32
			InvalidOpFlag : 1,
			DenormalFlag : 1,
			DivideByZeroFlag : 1,
			OverflowFlag : 1,
			UnderflowFlag : 1,
			PrecisionFlag : 1,

		// This bit is supported only on SSE2 or better CPUs.  Setting it to 1 on
		// SSE1 cpus will result in an invalid instruction exception when executing
		// LDMXSCR.
			DenormalsAreZero : 1,

			InvalidOpMask : 1,
			DenormalMask : 1,
			DivideByZeroMask : 1,
			OverflowMask : 1,
			UnderflowMask : 1,
			PrecisionMask : 1,

			RoundingControl : 2,
			FlushToZero : 1;
	};
};

SSE_MXCSR1 g_sseVUMXCSR1, g_sseMXCSR1;

int main(void) {
	s_TotalVUCycles = 5;

	s_callstack = 100;

	SuperVUGetProgram(0, 0);

	_mm_setcsr(g_sseMXCSR1.bitmask);

	return 0;
}