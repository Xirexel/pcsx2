# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

include_directories(
            ${CMAKE_CURRENT_SOURCE_DIR}
            ${EXTEND_DIR}
            ${PCSX2LIB_DIR}
            ${PCSX2LIB_DIR}/Extend
            ${PCSX2_DIR}
            ${PCSX2_DIR}/System
            ${PCSX2_DIR}/DebugTools
            ${FRAMEWORK_DIR}
            ${PCSX2_DIR}/../common/include
            )



set(FRAMEWORK_SRC
        ${FRAMEWORK_DIR}/Stub.cpp
        ${FRAMEWORK_DIR}/Utilities/PathUtils.cpp
        ${FRAMEWORK_DIR}/wx/datetime.cpp
        ${FRAMEWORK_DIR}/wx/dir.cpp
        ${FRAMEWORK_DIR}/wx/ffile.cpp
        ${FRAMEWORK_DIR}/wx/file.cpp
        ${FRAMEWORK_DIR}/wx/filefn.cpp
        ${FRAMEWORK_DIR}/wx/filename.cpp
        ${FRAMEWORK_DIR}/wx/log.cpp
        ${FRAMEWORK_DIR}/wx/longlong.cpp
        ${FRAMEWORK_DIR}/wx/stdpaths.cpp
        ${FRAMEWORK_DIR}/wx/string.cpp
        ${FRAMEWORK_DIR}/wx/thread.cpp
        ${FRAMEWORK_DIR}/wx/wfstream.cpp
        )

include_directories(
        ${FRAMEWORK_DIR}
            )

add_library(Framework STATIC ${FRAMEWORK_SRC})
