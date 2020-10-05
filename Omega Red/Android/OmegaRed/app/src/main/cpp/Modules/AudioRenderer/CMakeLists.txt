# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set (AudioRenderer_DIR ${MODULES_DIR}/AudioRenderer)

include_directories(
        ${EXTEND_DIR}
        ${PCSX2LIB_DIR}
        ${PUGIXML_DIR}
        ${PCSX2_DIR}/../common/include
        ${AudioRenderer_DIR}
        ${AudioRenderer_DIR}/Extend
        ${AudioRenderer_DIR}/src
        ${FRAMEWORK_DIR}
#        ${PCSX2_DIR}/../common/include
#
#        ${LIBAIO_DIR}
#        ${PUGIXML_DIR}
#        ${PCSX2LIB_DIR}/Extend
#        ${PCSX2_DIR}
#        ${PCSX2_DIR}/CDVD
#        ${FRAMEWORK_UTILITIES_DIR}
        )

set(AudioRenderer_SRC
        AndroidInterface.cpp
        ${EXTEND_DIR}/Util.cpp
        ${AudioRenderer_DIR}/Interface.cpp
        ${AudioRenderer_DIR}/AudioRenderer.cpp
        ${AudioRenderer_DIR}/AudioRendererControl.cpp
        ${AudioRenderer_DIR}/Stub.cpp
        ${AudioRenderer_DIR}/src/spu2sys.cpp
        ${AudioRenderer_DIR}/src/RegTable.cpp
        ${AudioRenderer_DIR}/src/Reverb.cpp
        ${AudioRenderer_DIR}/src/ADSR.cpp
        ${AudioRenderer_DIR}/src/ReadInput.cpp
        ${AudioRenderer_DIR}/src/spu2freeze.cpp
        ${AudioRenderer_DIR}/Extend/SndOut.cpp
        ${AudioRenderer_DIR}/Extend/Mixer.cpp
        SndOut_Android.cpp
        ${AudioRenderer_DIR}/Extend/PS2E-spu2.cpp
        ${AudioRenderer_DIR}/Extend/Dma.cpp
        )

add_definitions(-DDISABLE_RECORDING -DPUGIXML_WCHAR_MODE)

add_library(AudioRenderer SHARED ${AudioRenderer_SRC})

#SET(CMAKE_FIND_LIBRARY_SUFFIXES ".a")


#find_package( ZLIB REQUIRED )
#
#find_library(
#        Zlib-lib
#        libz.a
#        )

find_library(
        log-lib
        log )

target_link_libraries(
        AudioRenderer
        PugiXML
        ${log-lib}
        OpenSLES
        MemoryManager
        )
