# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)


set (EXTEND_DIR ${CMAKE_CURRENT_SOURCE_DIR}/Extend)
set (PCSX2LIB_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../PCSX2Lib)
set (PCSX2_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../../pcsx2)
set (FRAMEWORK_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../Common/Framework)
set (UTILITIES_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../Common/Utilities)
set (MEMORYMANAGER_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../Common/MemoryManager)
set (PUGIXML_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../Common/PugiXML)
set (FRAMEWORK_UTILITIES_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../Common/Framework/Utilities)
set (PCSX2_UTILITIES_SRC_DIR ${PCSX2_DIR}/../common/src/Utilities)
set (PTHREADS_INC_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../Common/pthreads4w/include)
set (LIBAIO_DIR ${CMAKE_CURRENT_SOURCE_DIR}/libaio/src)
set (MODULES_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../../../../../Modules)


add_definitions(-DDISABLE_RECORDING -DPUGIXML_WCHAR_MODE)

#if(${ANDROID_ABI} STREQUAL "x86")
#    add_definitions(-DANDROID_ABI_X86)
#
#    set(SVU_ZEROREC_SRC
#            ${CMAKE_CURRENT_SOURCE_DIR}/CPU/x86/aVUzerorec.S
#            )
#endif()

#if(${ANDROID_ABI} STREQUAL "armeabi-v7a")
#    add_definitions(-DANDROID_ABI_V7A)
#endif()


if(${ANDROID_ABI} STREQUAL "arm64-v8a")
    add_definitions(-DANDROID_ABI_V8A)
    add_definitions(-D__M_X86_64)
    add_definitions(-D_M_X86_64)
    set (EMITTER_DIR ${CMAKE_CURRENT_SOURCE_DIR}/Emitters/arm64v8aEmitter/)
endif()

#if((${ANDROID_ABI} STREQUAL "armeabi-v7a")
#        OR
#    (${ANDROID_ABI} STREQUAL "arm64-v8a")
#        )
#    set(SVU_ZEROREC_SRC
#            ${CMAKE_CURRENT_SOURCE_DIR}/CPU/x86/sVU_zerorec_stub.cpp
#            )
#endif()

message(ANDROID_ABI = ${ANDROID_ABI})
if(
#(${ANDROID_ABI} STREQUAL "x86")
#        OR
(${ANDROID_ABI} STREQUAL "arm64-v8a")
)

    message(ANDROID_ABI = ${ANDROID_ABI})


    add_subdirectory(SIMDEmitter)
    add_subdirectory(libaio)
    add_subdirectory(Framework)
    add_subdirectory(Utilities)
    add_subdirectory(Emitters)
    add_subdirectory(MemoryManager)
    add_subdirectory(PugiXML)
    add_subdirectory(PCSX2Lib)
    add_subdirectory(Modules)
    add_subdirectory(BiosControl)
endif()
