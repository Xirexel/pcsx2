# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)

add_subdirectory(AudioRenderer)
add_subdirectory(CDVD)
add_subdirectory(DEV9)
add_subdirectory(FW)
add_subdirectory(MemoryCard)
add_subdirectory(Pad)
add_subdirectory(USB)
add_subdirectory(VideoRenderer)

