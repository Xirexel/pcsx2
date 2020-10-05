# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set (DEV9_DIR ${MODULES_DIR}/DEV9)

include_directories(
        ${CMAKE_CURRENT_SOURCE_DIR}
        ${PUGIXML_DIR}
        ${EXTEND_DIR}
        ${PCSX2LIB_DIR}
        ${PCSX2_DIR}/../common/include
        )

set(DEV9_SRC
        DEV9.h
        AndroidInterface.cpp
        ${EXTEND_DIR}/Util.cpp
        ${DEV9_DIR}/Windows/Interface.cpp
        ${DEV9_DIR}/Windows/DEV9Control.cpp
        DEV9.cpp
        )

add_library(DEV9 SHARED ${DEV9_SRC})

find_library(
        log-lib
        log )

target_link_libraries(
        DEV9
        PugiXML
        ${log-lib}
        )
