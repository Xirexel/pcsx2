# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set (Pad_DIR ${MODULES_DIR}/Pad)

include_directories(
        ${PCSX2LIB_DIR}
        ${CMAKE_CURRENT_SOURCE_DIR}
        ${PUGIXML_DIR}
        ${EXTEND_DIR}
        ${FRAMEWORK_DIR}
        ${PCSX2_DIR}/../common/include
        ${Pad_DIR}
        )

set(Pad_SRC
        AndroidInterface.cpp
        ${EXTEND_DIR}/Util.cpp
        ${Pad_DIR}/PadControl.cpp
        ${Pad_DIR}/Interface.cpp
        ${Pad_DIR}/LilyPad/LilyPad.cpp
        ${Pad_DIR}/LilyPad/Config.cpp
        ${Pad_DIR}/LilyPad/InputManager.cpp
        ${Pad_DIR}/LilyPad/DeviceEnumerator.cpp
        ${Pad_DIR}/TouchPadInput.cpp
        Pad.cpp
        Stub.cpp
        )

add_library(Pad SHARED ${Pad_SRC})

find_library(
        log-lib
        log )

target_link_libraries(
        Pad
        PugiXML
        ${log-lib}
        )
