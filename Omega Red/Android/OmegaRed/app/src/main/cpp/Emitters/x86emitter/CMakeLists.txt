# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -mfxsr")

set (EMITTER_DIR ${PCSX2_DIR}/../common/src/x86emitter)
set (EMITTER_INC_DIR ${PCSX2_DIR}/../common/include/x86emitter)

include_directories(
        ${FRAMEWORK_DIR}
        ${EXTEND_DIR}
        ${PCSX2_DIR}/../common/include
        ${EMITTER_INC_DIR}
        ${EMITTER_DIR}
        )

set(EMITTER_SRC
        ${EMITTER_DIR}/cpudetect.cpp
        ${EMITTER_DIR}/simd.cpp
        ${EMITTER_DIR}/x86emitter.cpp
        ${EMITTER_DIR}/bmi.cpp
        ${EMITTER_DIR}/fpu.cpp
        ${EMITTER_DIR}/groups.cpp
        ${EMITTER_DIR}/jmp.cpp
        ${EMITTER_DIR}/legacy.cpp
        ${EMITTER_DIR}/legacy_sse.cpp
        ${EMITTER_DIR}/movs.cpp
        ${EMITTER_DIR}/LnxCpuDetect.cpp
        )

add_library(Emitter STATIC ${EMITTER_SRC})