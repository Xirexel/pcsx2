# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

include_directories(
        ${EXTEND_DIR}/simd
        )

set(SIMD_EMITTER_SRC
        SIMDEmitter.cpp
        simd_soft.cpp
        )

add_library(SIMDEmitter STATIC ${SIMD_EMITTER_SRC})