# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set (MemoryCard_DIR ${MODULES_DIR}/MemoryCard)

include_directories(
        ${PCSX2LIB_DIR}
        ${CMAKE_CURRENT_SOURCE_DIR}
        ${PUGIXML_DIR}
        ${EXTEND_DIR}
        ${PCSX2_DIR}/../common/include
        )

set(MemoryCard_SRC
        AndroidInterface.cpp
        ${EXTEND_DIR}/Util.cpp
        MemoryCard.cpp
        ${MemoryCard_DIR}/MemoryCardControl.cpp
        ${MemoryCard_DIR}/MemoryCardFile.cpp
        ${MemoryCard_DIR}/MemoryCardFolder.cpp
        ${MemoryCard_DIR}/Interface.cpp

#        ${PCSX2_DIR}/MemoryCard/MemoryCardisoReader.cpp
#        InputIsoFile.cpp
#        ${MemoryCard_DIR}/Extend.cpp
#        ${PCSX2LIB_DIR}/Extend/Pcsx2Config.cpp
#        ${PCSX2_DIR}/MemoryCard/IsoFS/IsoFile.cpp
#        ${PCSX2_DIR}/MemoryCard/IsoFS/IsoFS.cpp
#        ${PCSX2_DIR}/MemoryCard/CsoFileReader.cpp
#        ${PCSX2_DIR}/MemoryCard/CompressedFileReader.cpp
#        ${PCSX2_DIR}/MultipartFileReader.cpp
#        ${PCSX2_DIR}/MemoryCard/BlockdumpFileReader.cpp
##        ${PCSX2_DIR}/Linux/LnxFlatFileReader.cpp
#        LnxFlatFileReader.cpp
#        ${PCSX2_DIR}/MemoryCard/ChunksCache.cpp
#        ${PCSX2_DIR}/MemoryCard/GzippedFileReader.cpp
#        ${PCSX2LIB_DIR}/Extend/AppConfig.cpp
#        ${MemoryCard_DIR}/Elfheader.cpp
#        ${MemoryCard_DIR}/SecondStubSource.cpp
#        ${PCSX2_UTILITIES_SRC_DIR}/Exceptions.cpp
#        ${PCSX2_UTILITIES_SRC_DIR}/AlignedMalloc.cpp
#
#        ${EXTEND_DIR}/Redefine.cpp
#
#        ${FRAMEWORK_DIR}/wx/thread.cpp
#        ${FRAMEWORK_DIR}/wx/datetime.cpp
        )

add_definitions(-DDISABLE_RECORDING -DPUGIXML_WCHAR_MODE)

add_library(MemoryCard SHARED ${MemoryCard_SRC})

find_library(
        log-lib
        log )

target_link_libraries(
        MemoryCard
#        Framework
#        Utilities
        PugiXML
        ${log-lib}
#        ${Zlib-lib}
        )
