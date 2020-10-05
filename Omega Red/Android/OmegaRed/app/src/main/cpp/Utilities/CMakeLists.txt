# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

#include_directories(
#            ${CMAKE_CURRENT_SOURCE_DIR}
#            ${EXTEND_DIR}
#            ${PCSX2LIB_DIR}
#            ${PCSX2LIB_DIR}/Extend
#            ${PCSX2_DIR}
#            ${PCSX2_DIR}/System
#            ${PCSX2_DIR}/DebugTools
#            ${UTILITIES_DIR}
#            ${PCSX2_DIR}/../common/include
#            )

set(UTILITIES_SRC
        ${UTILITIES_DIR}/Mutex.cpp
        ${UTILITIES_DIR}/Semaphore.cpp
        ${UTILITIES_DIR}/ThreadTools.cpp
        ${PCSX2_UTILITIES_SRC_DIR}/Console.cpp
        ${EXTEND_DIR}/LnxThreads.cpp
        ${UTILITIES_DIR}/LnxMisc.cpp
        ${PCSX2_UTILITIES_SRC_DIR}/FastFormatString.cpp
        )

include_directories(
        ${PCSX2_UTILITIES_SRC_DIR}/Linux
        ${EXTEND_DIR}
        ${FRAMEWORK_DIR}
        ${UTILITIES_DIR}
        ${FRAMEWORK_UTILITIES_DIR}
        ${PCSX2_DIR}/../common/include
            )

add_library(Utilities STATIC ${UTILITIES_SRC})
