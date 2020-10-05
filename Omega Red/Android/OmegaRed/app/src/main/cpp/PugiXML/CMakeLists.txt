# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)



include_directories(
        ${PUGIXML_DIR}
        )

set(PUGIXML_SRC
        ${PUGIXML_DIR}/pugixml.cpp
        )

add_library(PugiXML STATIC ${PUGIXML_SRC})