#include "MemoryManager.h"
#include <string.h>

namespace apex
{
//	apex memmove (tiberium, kryptonite and mithril) memcpy/memmove functions written by Trevor Herselman in 2014

	apex::memcpy MemoryManager::memcpy = ::memcpy;
	
	memmove MemoryManager::memmove = ::memmove;
	

	static auto lres = MemoryManager::initialize();

	MemoryManager::MemoryManager()
	{
	}

	MemoryManager::~MemoryManager()
	{
	}

	bool MemoryManager::initialize()
	{
		return true;
	}

} // namespace apex


