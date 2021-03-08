/* This is an implementation of the threads API of POSIX 1003.1-2001.
 *
 * --------------------------------------------------------------------------
 *
 *      Pthreads-win32 - POSIX Threads Library for Win32
 *      Copyright(C) 1998 John E. Bossom
 *      Copyright(C) 1999,2005 Pthreads-win32 contributors
 * 
 *      Contact Email: rpj@callisto.canberra.edu.au
 * 
 *      The current list of contributors is contained
 *      in the file CONTRIBUTORS included with the source
 *      code distribution. The list can also be seen at the
 *      following World Wide Web location:
 *      http://sources.redhat.com/pthreads-win32/contributors.html
 * 
 *      This library is free software; you can redistribute it and/or
 *      modify it under the terms of the GNU Lesser General Public
 *      License as published by the Free Software Foundation; either
 *      version 2 of the License, or (at your option) any later version.
 * 
 *      This library is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *      Lesser General Public License for more details.
 * 
 *      You should have received a copy of the GNU Lesser General Public
 *      License along with this library in the file COPYING.LIB;
 *      if not, write to the Free Software Foundation, Inc.,
 *      59 Temple Place - Suite 330, Boston, MA 02111-1307, USA
 */

#if !defined( PTHREAD1_H )
#define PTHREAD1_H

#define PTHREAD_CANCEL_DISABLE 1


//enum {
///*
// * pthread_attr_{get,set}detachstate
// */
//  PTHREAD_CREATE_JOINABLE       = 0,  /* Default */
//  PTHREAD_CREATE_DETACHED       = 1,
//
///*
// * pthread_attr_{get,set}inheritsched
// */
//  PTHREAD_INHERIT_SCHED         = 0,
//  PTHREAD_EXPLICIT_SCHED        = 1,  /* Default */
//
///*
// * pthread_{get,set}scope
// */
//  PTHREAD_SCOPE_PROCESS         = 0,
//  PTHREAD_SCOPE_SYSTEM          = 1,  /* Default */
//
///*
// * pthread_setcancelstate paramters
// */
//  PTHREAD_CANCEL_ENABLE         = 0,  /* Default */
//  PTHREAD_CANCEL_DISABLE        = 1,
//
///*
// * pthread_setcanceltype parameters
// */
//  PTHREAD_CANCEL_ASYNCHRONOUS   = 0,
//  PTHREAD_CANCEL_DEFERRED       = 1,  /* Default */
//
///*
// * pthread_mutexattr_{get,set}pshared
// * pthread_condattr_{get,set}pshared
// */
//  PTHREAD_PROCESS_PRIVATE       = 0,
//  PTHREAD_PROCESS_SHARED        = 1,
//
///*
// * pthread_mutexattr_{get,set}robust
// */
//  PTHREAD_MUTEX_STALLED         = 0,  /* Default */
//  PTHREAD_MUTEX_ROBUST          = 1,
//
///*
// * pthread_barrier_wait
// */
//  PTHREAD_BARRIER_SERIAL_THREAD = -1
//};
//
///*
// * ====================
// * ====================
// * Cancelation
// * ====================
// * ====================
// */
//#define PTHREAD_CANCELED       ((void *)(size_t) -1)
//
//
///*
// * ====================
// * ====================
// * Once Key
// * ====================
// * ====================
// */
//#define PTHREAD_ONCE_INIT       { PTW32_FALSE, 0, 0, 0}


#endif /* PTHREAD1_H */
