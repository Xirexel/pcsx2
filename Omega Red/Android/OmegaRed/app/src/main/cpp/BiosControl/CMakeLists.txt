# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

set(BIOSCONTROL_SRC
        BiosControl.cpp
		AndroidInterface.cpp
		${EXTEND_DIR}/Util.cpp
        )

add_library(BiosControl SHARED ${BIOSCONTROL_SRC})

find_package( ZLIB REQUIRED )

find_library(
        Zlib-lib
        libz.a
)

target_link_libraries(
        BiosControl
        ${Zlib-lib}
)
