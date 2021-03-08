# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

#set (VideoRenderer_DIR ${MODULES_DIR}/VideoRenderer)

include_directories(
#        ${EXTEND_DIR}
        ${FRAMEWORK_DIR}
        ${PCSX2LIB_DIR}
        ${PUGIXML_DIR}
        ${EXTEND_DIR}
        ${EMITTER_DIR}
        ${PCSX2_DIR}/../common/include
#        ${VideoRenderer_DIR}
        )

set(VideoRenderer_SRC
        ${EXTEND_DIR}/Util.cpp
        AndroidInterface.cpp
        Interface.cpp
        VideoRenderer.cpp
        VideoRendererControl.cpp
        )

add_library(VideoRenderer SHARED ${VideoRenderer_SRC})

find_package( ZLIB REQUIRED )

find_library(
        Zlib-lib
        libz.a
        )

find_library(
        log-lib
        log )

target_link_libraries(
        VideoRenderer
		MemoryManager
        PugiXML
        ${Zlib-lib}
        ${log-lib}
        )
