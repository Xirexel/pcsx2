# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

include_directories(
        include
        ${FRAMEWORK_DIR}
        ${EXTEND_DIR}
        )

set(EMITTER_SRC
        src/Arm64Emitter.cpp
        src/arm64v8aEmitter.cpp
        src/cpudetect.cpp
        src/legacy.cpp
        src/MemoryUtil.cpp
        src/simd.cpp
        src/implement/xIndirectVoid.cpp
        src/implement/xAddressVoid.cpp
        src/implement/jmp.cpp
        src/implement/movs.cpp
        src/implement/groups.cpp
        )

add_library(Emitter STATIC ${EMITTER_SRC})