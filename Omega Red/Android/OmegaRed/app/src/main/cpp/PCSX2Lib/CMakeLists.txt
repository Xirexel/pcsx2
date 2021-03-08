# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

# Sets the minimum version of CMake required to build the native library.

cmake_minimum_required(VERSION 3.4.1)
project(assembler C ASM)
enable_language(C ASM)

SET(ASM_OPTIONS "-x assembler-with-cpp -save-temps")

SET(CMAKE_ASM_FLAGS "${CFLAGS} ${ASM_OPTIONS}" )
#SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -S")

set (CPU_EMULATOR ${PCSX2_DIR}/x86)

set (CPU_EMULATOR_SUB ix86-32)


#set (CPU_EMULATOR ${CMAKE_CURRENT_SOURCE_DIR}/${ANDROID_ABI})

add_definitions(-DENABLE_TOOLS)

add_compile_options(-fdeclspec)


include_directories(
            ${LIBAIO_DIR}
            ${CMAKE_CURRENT_SOURCE_DIR}
            ${EXTEND_DIR}
            ${EMITTER_DIR}
            ${PCSX2LIB_DIR}
            ${PCSX2LIB_DIR}/Extend
            ${PCSX2_DIR}
            ${PCSX2_DIR}/System
            ${CPU_EMULATOR}
            ${PCSX2_DIR}/DebugTools
            ${FRAMEWORK_DIR}
            ${FRAMEWORK_UTILITIES_DIR}
            ${PCSX2_DIR}/../common/include
            ${PCSX2_DIR}/IPU
            )

set(PCSX2LIB_SRC
#        ext.cpp
        AndroidInterface.cpp


        ${PCSX2_DIR}/windows/VCprojects/Test.cpp
        ${PCSX2_DIR}/windows/VCprojects/Test2.cpp
        ${PCSX2_DIR}/windows/VCprojects/Test3.cpp
        ${PCSX2_DIR}/windows/VCprojects/Test4.cpp
        ${PCSX2_DIR}/windows/VCprojects/Test5.cpp
        ${PCSX2_DIR}/windows/VCprojects/UnitTest.cpp

        ${EXTEND_DIR}/Util.cpp
        ${EXTEND_DIR}/Redefine.cpp
        ${PCSX2LIB_DIR}/Extend/DebugTools/DebugInterface.cpp
        ${PCSX2LIB_DIR}/Extend/System/SysCoreThread.cpp
        ${PCSX2LIB_DIR}/Extend/System/SysThreadBase.cpp
        ${PCSX2LIB_DIR}/Extend/ps2/BiosTools.cpp
        ${PCSX2LIB_DIR}/Extend/SPU2/spu2.cpp
        ${PCSX2LIB_DIR}/Extend/GS.cpp
        ${PCSX2LIB_DIR}/Extend/Sio.cpp
        ${PCSX2LIB_DIR}/Extend/AppConfig.cpp
        ${PCSX2LIB_DIR}/Extend/MTGS.cpp
        ${PCSX2LIB_DIR}/Extend/MTVU.cpp
        ${PCSX2LIB_DIR}/Extend.cpp
        ${PCSX2_DIR}/IopBios.cpp
        ${PCSX2_DIR}/System.cpp
        ${PCSX2_DIR}/Counters.cpp
        ${PCSX2_DIR}/SaveState.cpp
        ${PCSX2_DIR}/Memory.cpp
        ${CPU_EMULATOR}/iMisc.cpp
        ${PCSX2LIB_DIR}/Extend/Patch.cpp
        ${PCSX2_DIR}/R5900.cpp
        ${PCSX2_DIR}/Elfheader.cpp
        ${PCSX2_UTILITIES_SRC_DIR}/Exceptions.cpp
        ${PCSX2_UTILITIES_SRC_DIR}/AlignedMalloc.cpp
        ${PCSX2LIB_DIR}/Extend/Interface.cpp
        ${PCSX2_DIR}/R3000A.cpp
        ${PCSX2_DIR}/IopMem.cpp
        ${PCSX2_DIR}/SourceLog.cpp
        ${PCSX2_UTILITIES_SRC_DIR}/VirtualMemory.cpp
        ${PCSX2_UTILITIES_SRC_DIR}/Perf.cpp
        ${PCSX2LIB_DIR}/Extend/Pcsx2Config.cpp
        ${PCSX2_DIR}/VUmicroMem.cpp
        vtlb.cpp
        ${PCSX2_DIR}/Interpreter.cpp
        ${PCSX2_DIR}/R5900OpcodeImpl.cpp
        ${PCSX2_DIR}/VU0microInterp.cpp
        ${PCSX2_DIR}/VU1microInterp.cpp
        ${PCSX2_DIR}/VU0micro.cpp
        ${PCSX2_DIR}/VU1micro.cpp
        ${PCSX2_DIR}/Dump.cpp
        ${PCSX2_DIR}/Gif_Unit.cpp
        ${PCSX2_DIR}/IopCounters.cpp
        ${PCSX2_DIR}/IopDma.cpp
        ${PCSX2_DIR}/GSState.cpp
        ${PCSX2_DIR}/COP0.cpp
        ${PCSX2_DIR}/IopIrq.cpp
        ${PCSX2_DIR}/IopSio2.cpp
        ${PCSX2_DIR}/Vif.cpp
        ${PCSX2_DIR}/Sif.cpp
        ${PCSX2_DIR}/Gif.cpp
        ${PCSX2_DIR}/SPR.cpp
        ${PCSX2_DIR}/Cache.cpp
        ${PCSX2_DIR}/Hw.cpp
        ${PCSX2_DIR}/HwRead.cpp
        ${PCSX2_DIR}/HwWrite.cpp
        ${PCSX2_DIR}/FW.cpp
        ${PCSX2_DIR}/Patch_Memory.cpp
        ${PCSX2_DIR}/Vif0_Dma.cpp
        ${PCSX2_DIR}/Vif1_Dma.cpp
        ${PCSX2_DIR}/Vif1_MFIFO.cpp
        ${PCSX2_DIR}/Sif0.cpp
        ${PCSX2_DIR}/Sif1.cpp
        ${PCSX2_DIR}/sif2.cpp
        ${PCSX2_DIR}/VUops.cpp
        ${PCSX2_DIR}/VUmicro.cpp
        ${PCSX2_DIR}/VU0.cpp
        ${PCSX2_DIR}/FiFo.cpp
        ${PCSX2_DIR}/Vif_Codes.cpp
        ${PCSX2_DIR}/VUflags.cpp
        ${PCSX2_DIR}/Vif_Unpack.cpp
        ${PCSX2_DIR}/Vif_Transfer.cpp
        ${PCSX2_DIR}/FPU.cpp
        ${PCSX2_DIR}/COP2.cpp
        ${PCSX2_DIR}/MMI.cpp
        ${PCSX2_DIR}/R5900OpcodeTables.cpp
        ${PCSX2_DIR}/Mdec.cpp

        ${PCSX2_DIR}/DebugTools/ExpressionParser.cpp
        ${PCSX2_DIR}/DebugTools/DisR5900asm.cpp
        ${PCSX2_DIR}/DebugTools/DisVU0Micro.cpp
        ${PCSX2_DIR}/DebugTools/DisVU1Micro.cpp

        ${PCSX2_DIR}/R3000AOpcodeTables.cpp
        ${PCSX2_DIR}/IopGte.cpp
        ${PCSX2LIB_DIR}/Extend/R3000AInterpreter.cpp
        AndroidHostSys.cpp
        ${PCSX2LIB_DIR}/Extend/CDVD/CDVD.cpp
        ${PCSX2LIB_DIR}/Extend/CDVD/OutputIsoFile.cpp

        ${PCSX2_DIR}/DebugTools/MIPSAnalyst.cpp
        ${PCSX2_DIR}/DebugTools/SymbolMap.cpp
        ${PCSX2_DIR}/DebugTools/Breakpoints.cpp

        ${CPU_EMULATOR}/newVif_Unpack.cpp
        ${CPU_EMULATOR}/iR3000A.cpp
        ${CPU_EMULATOR}/newVif_Dynarec.cpp
        ${CPU_EMULATOR}/microVU.cpp
        ${CPU_EMULATOR}/newVif_UnpackSSE.cpp
        ${CPU_EMULATOR}/iR5900Misc.cpp
        ${CPU_EMULATOR}/iMMI.cpp
        ${CPU_EMULATOR}/iCOP0.cpp
        ${CPU_EMULATOR}/iFPUd.cpp
        ${CPU_EMULATOR}/iFPU.cpp
        ${CPU_EMULATOR}/iCore.cpp
        ${CPU_EMULATOR}/iR3000Atables.cpp
        ${CPU_EMULATOR}/BaseblockEx.cpp

        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900-32.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iCore-32.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900Arit.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900AritImm.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900Branch.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900Jump.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900LoadStore.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900Move.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900MultDiv.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900Shift.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/iR5900Templates.cpp
        ${CPU_EMULATOR}/${CPU_EMULATOR_SUB}/recVTLB.cpp

        ${PCSX2_DIR}/IPU/mpeg2lib/Mpeg.cpp
        ${PCSX2_DIR}/IPU/mpeg2lib/Idct.cpp
        ${PCSX2_DIR}/IPU/IPU.cpp
        ${PCSX2_DIR}/IPU/IPU_Fifo.cpp
        ${PCSX2_DIR}/IPU/IPUdma.cpp
        ${PCSX2_DIR}/IPU/yuv2rgb.cpp
        ${PCSX2_DIR}/IPU/IPUdither.cpp
#        Extend/yuv2rgb.cpp

        ${PCSX2_DIR}/CDVD/CdRom.cpp
        ${PCSX2_DIR}/CDVD/CDVDaccess.cpp
        ${PCSX2_DIR}/CDVD/ChunksCache.cpp

        ${PCSX2_DIR}/CDVD/IsoFS/IsoFile.cpp
        ${PCSX2_DIR}/CDVD/IsoFS/IsoFS.cpp
        ${PCSX2_DIR}/CDVD/IsoFS/IsoFSCDVD.cpp



        ${PCSX2_DIR}/ps2/Iop/IopHwRead.cpp
        ${PCSX2_DIR}/ps2/Iop/IopHwWrite.cpp
        ${PCSX2_DIR}/ps2/Iop/PsxBios.cpp
        ${PCSX2_DIR}/IopHw.cpp

        ${PCSX2_DIR}/ps2/pgif.cpp
        ${PCSX2_DIR}/ps2/LegacyDmac.cpp
        )

#set_source_files_properties(${SVU_ZEROREC_SRC} PROPERTIES COMPILE_FLAGS "-x assembler-with-cpp")

add_library(
        PCSX2Lib
        SHARED
        ${PCSX2LIB_SRC}
        )

find_library(
        log-lib
        log )

target_link_libraries(
        PCSX2Lib
        ${log-lib}
        Framework
        Utilities
        Emitter
        SIMDEmitter
        MemoryManager
        PugiXML
        )