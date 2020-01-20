#pragma once
/*
**  CRC.H - header file for SNIPPETS CRC and checksum functions
*/


#include <stdlib.h>   /* For size_t                 */
#include <stdafx.h>


#ifdef __cplusplus
extern "C" {
#endif

void crc32compute(const BYTE *a_ptrdata, INT32 a_data_size, BOOL a_is_forward, DWORD *a_crc);

#ifdef __cplusplus
}
#endif

