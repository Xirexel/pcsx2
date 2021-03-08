# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set (CDVD_DIR ${MODULES_DIR}/CDVD)

include_directories(
        ${EXTEND_DIR}
        ${FRAMEWORK_DIR}
        ${EMITTER_DIR}
        ${PCSX2_DIR}/../common/include
        ${PCSX2LIB_DIR}
        ${LIBAIO_DIR}
        ${PUGIXML_DIR}
        ${PCSX2LIB_DIR}/Extend
        ${PCSX2_DIR}
        ${PCSX2_DIR}/CDVD
        ${FRAMEWORK_UTILITIES_DIR}
        )

set(CDVD_SRC
        AndroidInterface.cpp
        ${EXTEND_DIR}/Util.cpp
        ${CDVD_DIR}/CDVD.cpp
        ${CDVD_DIR}/Interface.cpp
        ${PCSX2_DIR}/CDVD/CDVDisoReader.cpp
        InputIsoFile.cpp
        ${CDVD_DIR}/Extend.cpp
        ${PCSX2LIB_DIR}/Extend/Pcsx2Config.cpp
        ${PCSX2_DIR}/CDVD/IsoFS/IsoFile.cpp
        ${PCSX2_DIR}/CDVD/IsoFS/IsoFS.cpp
        ${PCSX2_DIR}/CDVD/CsoFileReader.cpp
        ${PCSX2_DIR}/CDVD/CompressedFileReader.cpp
        ${PCSX2_DIR}/MultipartFileReader.cpp
        ${PCSX2_DIR}/CDVD/BlockdumpFileReader.cpp
#        ${PCSX2_DIR}/Linux/LnxFlatFileReader.cpp
        LnxFlatFileReader.cpp
        ${PCSX2_DIR}/CDVD/ChunksCache.cpp
        ${PCSX2_DIR}/CDVD/GzippedFileReader.cpp
        ${PCSX2LIB_DIR}/Extend/AppConfig.cpp
        ${CDVD_DIR}/Elfheader.cpp
        ${CDVD_DIR}/SecondStubSource.cpp
        ${PCSX2_UTILITIES_SRC_DIR}/Exceptions.cpp
        ${PCSX2_UTILITIES_SRC_DIR}/AlignedMalloc.cpp

        ${EXTEND_DIR}/Redefine.cpp

        ${FRAMEWORK_DIR}/wx/thread.cpp
        ${FRAMEWORK_DIR}/wx/datetime.cpp
        )

add_definitions(-DDISABLE_RECORDING -DPUGIXML_WCHAR_MODE)

add_library(CDVD SHARED ${CDVD_SRC})

SET(CMAKE_FIND_LIBRARY_SUFFIXES ".a")

find_library(
        log-lib
        log
        )




find_package( ZLIB REQUIRED )

find_library(
        Zlib-lib
        libz.a
        )

target_link_libraries(
        CDVD
        Framework
        Utilities
        PugiXML
        libaio
        ${log-lib}
        ${Zlib-lib}
        )
