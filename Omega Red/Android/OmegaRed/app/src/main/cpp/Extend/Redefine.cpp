//
// Created by Xirexel on 4/14/2019.
//

//#include "Redefine.h"


#include <cstdlib>
#include <cstring>

void wcscpy_s(
	wchar_t* _Destination,
	size_t _SizeInWords,
	wchar_t const* _Source
)
{
	memcpy(_Destination, _Source, _SizeInWords * sizeof(wchar_t));
}


#ifdef __ANDROID__
int pthread_setcancelstate (int state, int *oldstate){ return 0;}


#endif