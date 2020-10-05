
#pragma once

#include <excpt.h>


class wxThread
{
public:

	static bool IsMain();

	static unsigned int GetCPUCount();
};
