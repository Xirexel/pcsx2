#pragma once

#ifdef _WIN64
#if defined(_MSC_VER)
#define APEXCALL __cdecl //	32-bit on Visual Studio
#else
#define APEXCALL __attribute__((__cdecl__)) //	32-bit on GCC / LLVM (Clang)
#endif
#else
#define APEXCALL //	64-bit - __fastcall is default on 64-bit!
#endif

//_CRTIMP errno_t  __cdecl memcpy_s(_Out_writes_bytes_to_opt_(_DstSize, _MaxCount) void * _Dst, _In_ rsize_t _DstSize, _In_reads_bytes_opt_(_MaxCount) const void * _Src, _In_ rsize_t _MaxCount);
namespace apex
{
	typedef void *(APEXCALL *memcpy)(
		void *,
		const void *,
		size_t);

	typedef void *(APEXCALL *memmove)(
		void *,
		const void *,
		size_t);

	class MemoryManager
	{
	public:
		static memcpy memcpy;

		static memmove memmove;

		static bool initialize();


		MemoryManager();
		~MemoryManager();
	};	
}

