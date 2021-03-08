# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set (USB_DIR ${MODULES_DIR}/USBnull)

include_directories(
        ${CMAKE_CURRENT_SOURCE_DIR}
        ${PUGIXML_DIR}
        ${EXTEND_DIR}
        ${EMITTER_DIR}
        ${FRAMEWORK_DIR}
        ${PCSX2LIB_DIR}
        ${PCSX2_DIR}/../common/include
        )

set(USB_SRC
        AndroidInterface.cpp
        ${EXTEND_DIR}/Util.cpp
        ${USB_DIR}/Windows/Interface.cpp
        ${USB_DIR}/Windows/USBControl.cpp
        USB.cpp
        )

add_library(USB SHARED ${USB_SRC})

find_library(
        log-lib
        log )

target_link_libraries(
        USB
        PugiXML
        ${log-lib}
        )
