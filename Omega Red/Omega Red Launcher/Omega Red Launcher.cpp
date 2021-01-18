// Omega Red Launcher.cpp : Defines the entry point for the application.
//

#include "framework.h"
#include "Omega Red Launcher.h"
using namespace System;

#ifdef _DEBUG
#using "../Omega Red/bin/Debug/Omega Red.exe"
#else
#using "../Omega Red/bin/Release/Omega Red.exe"
#endif // _DEBUG



[System::MTAThread] 
int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                                          _In_opt_ HINSTANCE hPrevInstance,
                                          _In_ LPWSTR lpCmdLine,
                                          _In_ int nCmdShow) {
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    Omega_Red::Program::Main(nullptr);

    return 0;
}

// Nvidia OpenGL drivers >= v302 will check if the application exports a global
// variable named NvOptimusEnablement to know if it should run the app in high
// performance graphics mode or using the IGP.
extern "C"
{
    __declspec(dllexport) unsigned long NvOptimusEnablement = 1;
}

// Also on AMD PowerExpress: https://community.amd.com/thread/169965
extern "C" {
__declspec(dllexport) int AmdPowerXpressRequestHighPerformance = 1;
}