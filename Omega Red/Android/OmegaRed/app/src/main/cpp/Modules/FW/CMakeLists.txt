# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set (FW_DIR ${MODULES_DIR}/FWnull)

include_directories(
        ${CMAKE_CURRENT_SOURCE_DIR}
        ${PUGIXML_DIR}
        ${EXTEND_DIR}
        ${FRAMEWORK_DIR}
        ${EMITTER_DIR}
        ${PCSX2LIB_DIR}
        ${PCSX2_DIR}/../common/include
        )

set(FW_SRC
        AndroidInterface.cpp
        ${EXTEND_DIR}/Util.cpp
        ${FW_DIR}/Windows/Interface.cpp
        ${FW_DIR}/Windows/FWControl.cpp
        FW.cpp
        )

add_library(FW SHARED ${FW_SRC})

find_library(
        log-lib
        log )

target_link_libraries(
        FW
        PugiXML
        ${log-lib}
        )
