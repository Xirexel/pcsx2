#pragma once

#ifndef WINVER
// the only exception to the above is MSVC 6 which has a time bomb in its
// headers: they warn against using them with WINVER >= 0x0500 as they
// contain only part of the declarations and they're not always correct, so
// don't define WINVER for it at all as this allows everything to work as
// expected both with standard VC6 headers (which define WINVER as 0x0400
// by default) and headers from a newer SDK (which may define it as 0x0500)
#if !defined(__VISUALC__) || (__VISUALC__ >= 1300)
#define WINVER 0x0600
#endif
#endif

// define _WIN32_WINNT and _WIN32_IE to the highest possible values because we
// always check for the version of installed DLLs at runtime anyway (see
// wxGetWinVersion() and wxApp::GetComCtl32Version()) unless the user really
// doesn't want to use APIs only available on later OS versions and had defined
// them to (presumably lower) values
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0600
#endif

#ifndef _WIN32_IE
#define _WIN32_IE 0x0700
#endif

/* Deal with clash with __WINDOWS__ include guard */
#if defined(__WXWINCE__) && defined(__WINDOWS__)
#undef __WINDOWS__
#endif

// For IPv6 support, we must include winsock2.h before winsock.h, and
// windows.h include winsock.h so do it before including it
#if wxUSE_IPV6
#include <winsock2.h>
#endif

#include <windows.h>

#if defined(__WXWINCE__) && !defined(__WINDOWS__)
#define __WINDOWS__
#endif