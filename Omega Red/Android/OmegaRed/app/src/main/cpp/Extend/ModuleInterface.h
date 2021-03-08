//
// Created by Evgeny Pereguda on 7/10/2019.
//

#ifndef OMEGARED_MODULEINTERFACE_H
#define OMEGARED_MODULEINTERFACE_H

#define CALLBACK
#include "PCSX2Lib_API.h"

PCSX2_EXPORT_C_(void*) getAPI();
PCSX2_EXPORT_C execute(const wchar_t* a_command, wchar_t** a_result);
PCSX2_EXPORT_C releaseString(wchar_t* a_string);

#endif //OMEGARED_MODULEINTERFACE_H
