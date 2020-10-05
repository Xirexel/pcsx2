package com.xirexel.omegared.PCSX2;
import com.xirexel.omegared.Util.Util;

import org.w3c.dom.*;

import java.io.StringWriter;
import java.util.BitSet;

import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

public class Pcsx2Config {

    public enum VsyncMode
    {
        Off ,
        On ,
        Adaptive;
    };

    public enum GamefixId
    {
        GamefixId_FIRST,

        Fix_VuAddSub,
        Fix_VuClipFlag,
        Fix_FpuCompare,
        Fix_FpuMultiply,
        Fix_FpuNegDiv,
        Fix_XGKick,
        Fix_IpuWait,
        Fix_EETiming,
        Fix_SkipMpeg,
        Fix_OPHFlag,
        Fix_DMABusy,
        Fix_VIFFIFO,
        Fix_VIF1Stall,
        Fix_GIFFIFO,
        Fix_FMVinSoftware,
        Fix_GoemonTlbMiss,
        Fix_ScarfaceIbit,

        GamefixId_COUNT
    };

    // ------------------------------------------------------------------------
    public class CpuOptions
    {
        public CpuOptions()
        {
            Recompiler = new RecompilerOptions();

            sseMXCSR = new SSE_MXCSR();

            sseVUMXCSR = new SSE_MXCSR();
        }

        public void serialize(Element a_rootElement)
        {
            Element a_Cpu = a_rootElement.getOwnerDocument().createElement("Cpu");

            Recompiler.serialize(a_Cpu);

            Element a_sseMXCSR = a_Cpu.getOwnerDocument().createElement("sseMXCSR");

            sseMXCSR.serialize(a_sseMXCSR);

            a_Cpu.appendChild(a_sseMXCSR);



            Element a_sseVUMXCSR = a_Cpu.getOwnerDocument().createElement("sseVUMXCSR");

            sseVUMXCSR.serialize(a_sseVUMXCSR);

            a_Cpu.appendChild(a_sseVUMXCSR);

            a_rootElement.appendChild(a_Cpu);
        }

        public RecompilerOptions Recompiler;

        public SSE_MXCSR sseMXCSR;

        public SSE_MXCSR sseVUMXCSR;

    };

    // ------------------------------------------------------------------------

    public class RecompilerOptions
    {

        public void serialize(Element a_rootElement)
        {
            Element a_Recompiler = a_rootElement.getOwnerDocument().createElement("Recompiler");

            Attr attr = a_rootElement.getOwnerDocument().createAttribute("bitset");
            BitSet l_bitset = grabBitSet();
            attr.setValue(String.valueOf(Util.bits2Int(l_bitset)));
            a_Recompiler.setAttributeNode(attr);

            a_rootElement.appendChild(a_Recompiler);
        }

        private BitSet grabBitSet()
        {
            BitSet l_bitset = new BitSet(32);

            l_bitset.set(0, EnableEE);

            l_bitset.set(1, EnableIOP);

            l_bitset.set(2, EnableVU0);

            l_bitset.set(3, EnableVU1);

            l_bitset.set(4, UseMicroVU0);

            l_bitset.set(5, UseMicroVU1);

            l_bitset.set(6, vuOverflow);

            l_bitset.set(7, vuExtraOverflow);

            l_bitset.set(8, vuSignOverflow);

            l_bitset.set(9, vuUnderflow);

            l_bitset.set(10, fpuOverflow);

            l_bitset.set(11, fpuExtraOverflow);

            l_bitset.set(12, fpuFullMode);

            l_bitset.set(13, StackFrameChecks);

            l_bitset.set(14, PreBlockCheckEE);

            l_bitset.set(15, PreBlockCheckIOP);

            l_bitset.set(16, EnableEECache);


            return l_bitset;
        }

//            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
        public boolean EnableEE;

//            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
        public boolean EnableIOP;

//            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
        public boolean EnableVU0;

//            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
        public boolean EnableVU1;

//            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
        public boolean UseMicroVU0;

//            [BitFieldInfo(5, 1), XmlIgnoreAttribute]
        public boolean UseMicroVU1;

//            [BitFieldInfo(6, 1), XmlIgnoreAttribute]
        public boolean vuOverflow;

//            [BitFieldInfo(7, 1), XmlIgnoreAttribute]
        public boolean vuExtraOverflow;

//            [BitFieldInfo(8, 1), XmlIgnoreAttribute]
        public boolean vuSignOverflow;

//            [BitFieldInfo(9, 1), XmlIgnoreAttribute]
        public boolean vuUnderflow;

//            [BitFieldInfo(10, 1), XmlIgnoreAttribute]
        public boolean fpuOverflow;

//            [BitFieldInfo(11, 1), XmlIgnoreAttribute]
        public boolean fpuExtraOverflow;

//            [BitFieldInfo(12, 1), XmlIgnoreAttribute]
        public boolean fpuFullMode;

//            [BitFieldInfo(13, 1), XmlIgnoreAttribute]
        public boolean StackFrameChecks;

//            [BitFieldInfo(14, 1), XmlIgnoreAttribute]
        public boolean PreBlockCheckEE;

//            [BitFieldInfo(15, 1), XmlIgnoreAttribute]
        public boolean PreBlockCheckIOP;

//            [BitFieldInfo(16, 1), XmlIgnoreAttribute]
        public boolean EnableEECache;
    };

    public class SSE_MXCSR
    {
        public void serialize(Element a_rootElement)
        {
            Attr attr = a_rootElement.getOwnerDocument().createAttribute("bitset");
            BitSet l_bitset = grabBitSet();
            attr.setValue(String.valueOf(Util.bits2Int(l_bitset)));
            a_rootElement.setAttributeNode(attr);
        }

        private BitSet grabBitSet()
        {
            BitSet l_bitset = new BitSet(32);

            l_bitset.set(0, InvalidOpFlag);

            l_bitset.set(1, DenormalFlag);

            l_bitset.set(2, DivideByZeroFlag);

            l_bitset.set(3, OverflowFlag);

            l_bitset.set(4, UnderflowFlag);

            l_bitset.set(5, PrecisionFlag);

            l_bitset.set(6, DenormalsAreZero);

            l_bitset.set(7, InvalidOpMask);

            l_bitset.set(8, DenormalMask);

            l_bitset.set(9, DivideByZeroMask);

            l_bitset.set(10, OverflowMask);

            l_bitset.set(11, UnderflowMask);

            l_bitset.set(12, PrecisionMask);

            l_bitset.set(13, (RoundingControl & 0x01) == 0x01);

            l_bitset.set(14, (RoundingControl & 0x02) == 0x02);

            l_bitset.set(15, FlushToZero);


            return l_bitset;
        }

//            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
        public boolean InvalidOpFlag;

//            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
        public boolean DenormalFlag;

//            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
        public boolean DivideByZeroFlag;

//            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
        public boolean OverflowFlag;

//            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
        public boolean UnderflowFlag;

//            [BitFieldInfo(5, 1), XmlIgnoreAttribute]
        public boolean PrecisionFlag;

//            [BitFieldInfo(6, 1), XmlIgnoreAttribute]
        public boolean DenormalsAreZero;

//            [BitFieldInfo(7, 1), XmlIgnoreAttribute]
        public boolean InvalidOpMask;

//            [BitFieldInfo(8, 1), XmlIgnoreAttribute]
        public boolean DenormalMask;

        // This bit is supported only on SSE2 or better CPUs.  Setting it to 1 on
        // SSE1 cpus will result in an invalid instruction exception when executing
        // LDMXSCR.
//            [BitFieldInfo(9, 1), XmlIgnoreAttribute]
        public boolean DivideByZeroMask;

//            [BitFieldInfo(10, 1), XmlIgnoreAttribute]
        public boolean OverflowMask;

//            [BitFieldInfo(11, 1), XmlIgnoreAttribute]
        public boolean UnderflowMask;

//            [BitFieldInfo(12, 1), XmlIgnoreAttribute]
        public boolean PrecisionMask;

//            [BitFieldInfo(13, 2), XmlIgnoreAttribute]
        public int RoundingControl;

//            [BitFieldInfo(15, 1), XmlIgnoreAttribute]
        public boolean FlushToZero;

        public void SetRoundMode(int mode)
        {
            RoundingControl = mode;
        }
    };

    // ------------------------------------------------------------------------
    public class GSOptions
    {
        public void serialize(Element a_rootElement)
        {
            Element a_GS = a_rootElement.getOwnerDocument().createElement("GS");



            Attr attr = a_rootElement.getOwnerDocument().createAttribute("SynchronousMTGS");

            attr.setValue(String.valueOf(SynchronousMTGS));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("DisableOutput");

            attr.setValue(String.valueOf(DisableOutput));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("VsyncQueueSize");

            attr.setValue(String.valueOf(VsyncQueueSize));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("FrameLimitEnable");

            attr.setValue(String.valueOf(FrameLimitEnable));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("FrameSkipEnable");

            attr.setValue(String.valueOf(FrameSkipEnable));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("VsyncEnable");

            attr.setValue(String.valueOf(VsyncEnable));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("FramesToDraw");

            attr.setValue(String.valueOf(FramesToDraw));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("FramesToSkip");

            attr.setValue(String.valueOf(FramesToSkip));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("LimitScalar");

            attr.setValue(String.valueOf(LimitScalar));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("FramerateNTSC");

            attr.setValue(String.valueOf(FramerateNTSC));

            a_GS.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("FrameratePAL");

            attr.setValue(String.valueOf(FrameratePAL));

            a_GS.setAttributeNode(attr);



            a_rootElement.appendChild(a_GS);
        }


        // forces the MTGS to execute tags/tasks in fully blocking/synchronous
        // style.  Useful for debugging potential bugs in the MTGS pipeline.
        public boolean SynchronousMTGS;

        public boolean DisableOutput;

        public int VsyncQueueSize;

        public boolean FrameLimitEnable;

        public boolean FrameSkipEnable;

        public int VsyncEnable;

        public int FramesToDraw;	// number of consecutive frames (fields) to render

        public int FramesToSkip;	// number of consecutive frames (fields) to skip

        public int LimitScalar;

        public int FramerateNTSC;

        public int FrameratePAL;


    };

    // ------------------------------------------------------------------------
    public class SpeedhackOptions
    {

        public void serialize(Element a_rootElement)
        {
            Element a_Recompiler = a_rootElement.getOwnerDocument().createElement("Speedhacks");

            Attr attr = a_rootElement.getOwnerDocument().createAttribute("bitset");
            BitSet l_bitset = grabBitSet();
            attr.setValue(String.valueOf(Util.bits2Int(l_bitset)));
            a_Recompiler.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("EECycleRate");
            attr.setValue(String.valueOf(EECycleRate));
            a_Recompiler.setAttributeNode(attr);


            attr = a_rootElement.getOwnerDocument().createAttribute("EECycleSkip");
            attr.setValue(String.valueOf(EECycleSkip));
            a_Recompiler.setAttributeNode(attr);


            a_rootElement.appendChild(a_Recompiler);
        }

        private BitSet grabBitSet()
        {
            BitSet l_bitset = new BitSet(32);

            l_bitset.set(0, fastCDVD);

            l_bitset.set(1, IntcStat);

            l_bitset.set(2, WaitLoop);

            l_bitset.set(3, vuFlagHack);

            l_bitset.set(4, vuThread);


            return l_bitset;
        }



//            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
        public boolean fastCDVD;

//            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
        public boolean IntcStat;

//            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
        public boolean WaitLoop;

//            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
        public boolean vuFlagHack;

//            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
        public boolean vuThread;


        public byte EECycleRate;		// EE cycle rate selector (1.0, 1.5, 2.0)

        public byte EECycleSkip;		// VU Cycle Stealer factor (0, 1, 2, or 3)

    };

    // ------------------------------------------------------------------------
    // NOTE: The GUI's GameFixes panel is dependent on the order of bits in this structure.
    public class GamefixOptions
    {

        public void serialize(Element a_rootElement)
        {
            Element a_Gamefixes = a_rootElement.getOwnerDocument().createElement("Gamefixes");

            Attr attr = a_rootElement.getOwnerDocument().createAttribute("bitset");
            BitSet l_bitset = grabBitSet();
            attr.setValue(String.valueOf(Util.bits2Int(l_bitset)));
            a_Gamefixes.setAttributeNode(attr);

            a_rootElement.appendChild(a_Gamefixes);
        }

        private BitSet grabBitSet()
        {
            BitSet l_bitset = new BitSet(32);

            l_bitset.set(0, VuAddSubHack);

            l_bitset.set(1, VuClipFlagHack);

            l_bitset.set(2, FpuCompareHack);

            l_bitset.set(3, FpuMulHack);

            l_bitset.set(4, FpuNegDivHack);

            l_bitset.set(5, XgKickHack);

            l_bitset.set(6, IPUWaitHack);

            l_bitset.set(7, EETimingHack);

            l_bitset.set(8, SkipMPEGHack);

            l_bitset.set(9, OPHFlagHack);

            l_bitset.set(10, DMABusyHack);

            l_bitset.set(11, VIFFIFOHack);

            l_bitset.set(12, VIF1StallHack);

            l_bitset.set(13, GIFFIFOHack);

            l_bitset.set(14, FMVinSoftwareHack);

            l_bitset.set(15, GoemonTlbHack);

            l_bitset.set(16, ScarfaceIbit);


            return l_bitset;
        }




//            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
        public boolean VuAddSubHack;

//            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
        public boolean VuClipFlagHack;

//            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
        public boolean FpuCompareHack;

//            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
        public boolean FpuMulHack;

//            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
        public boolean FpuNegDivHack;

//            [BitFieldInfo(5, 1), XmlIgnoreAttribute]
        public boolean XgKickHack;

//            [BitFieldInfo(6, 1), XmlIgnoreAttribute]
        public boolean IPUWaitHack;

//            [BitFieldInfo(7, 1), XmlIgnoreAttribute]
        public boolean EETimingHack;

//            [BitFieldInfo(8, 1), XmlIgnoreAttribute]
        public boolean SkipMPEGHack;

//            [BitFieldInfo(9, 1), XmlIgnoreAttribute]
        public boolean OPHFlagHack;

//            [BitFieldInfo(10, 1), XmlIgnoreAttribute]
        public boolean DMABusyHack;

//            [BitFieldInfo(11, 1), XmlIgnoreAttribute]
        public boolean VIFFIFOHack;

//            [BitFieldInfo(12, 1), XmlIgnoreAttribute]
        public boolean VIF1StallHack;

//            [BitFieldInfo(13, 1), XmlIgnoreAttribute]
        public boolean GIFFIFOHack;

//            [BitFieldInfo(14, 1), XmlIgnoreAttribute]
        public boolean FMVinSoftwareHack;

//            [BitFieldInfo(15, 1), XmlIgnoreAttribute]
        public boolean GoemonTlbHack;

//            [BitFieldInfo(16, 1), XmlIgnoreAttribute]
        public boolean ScarfaceIbit;



        public void Set( GamefixId id, boolean enabled )
        {
            switch(id)
            {
                case Fix_VuAddSub:		VuAddSubHack		= enabled;	break;
                case Fix_VuClipFlag:	VuClipFlagHack		= enabled;	break;
                case Fix_FpuCompare:	FpuCompareHack		= enabled;	break;
                case Fix_FpuMultiply:	FpuMulHack			= enabled;	break;
                case Fix_FpuNegDiv:		FpuNegDivHack		= enabled;	break;
                case Fix_XGKick:		XgKickHack			= enabled;	break;
                case Fix_IpuWait:		IPUWaitHack			= enabled;	break;
                case Fix_EETiming:		EETimingHack		= enabled;	break;
                case Fix_SkipMpeg:		SkipMPEGHack		= enabled;	break;
                case Fix_OPHFlag:		OPHFlagHack			= enabled;  break;
                case Fix_DMABusy:		DMABusyHack			= enabled;  break;
                case Fix_VIFFIFO:		VIFFIFOHack			= enabled;  break;
                case Fix_VIF1Stall:		VIF1StallHack		= enabled;  break;
                case Fix_GIFFIFO:		GIFFIFOHack			= enabled;  break;
                case Fix_FMVinSoftware:	FMVinSoftwareHack	= enabled;  break;
                case Fix_GoemonTlbMiss: GoemonTlbHack		= enabled;  break;
                case Fix_ScarfaceIbit:  ScarfaceIbit        = enabled;  break;
                default:
                    break;
            }
        }
    };

    public class ProfilerOptions
    {

        public void serialize(Element a_rootElement)
        {
            Element a_Profiler = a_rootElement.getOwnerDocument().createElement("Profiler");

            Attr attr = a_rootElement.getOwnerDocument().createAttribute("bitset");
            BitSet l_bitset = grabBitSet();
            attr.setValue(String.valueOf(Util.bits2Int(l_bitset)));
            a_Profiler.setAttributeNode(attr);

            a_rootElement.appendChild(a_Profiler);
        }

        private BitSet grabBitSet()
        {
            BitSet l_bitset = new BitSet(32);

            l_bitset.set(0, Enabled);

            l_bitset.set(1, RecBlocks_EE);

            l_bitset.set(2, RecBlocks_IOP);

            l_bitset.set(3, RecBlocks_VU0);

            l_bitset.set(4, RecBlocks_VU1);


            return l_bitset;
        }


//            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
        public boolean Enabled;

//            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
        public boolean RecBlocks_EE;

//            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
        public boolean RecBlocks_IOP;

//            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
        public boolean RecBlocks_VU0;

//            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
        public boolean RecBlocks_VU1;
    };

    public class DebugOptions
    {

        public void serialize(Element a_rootElement)
        {
            Element a_Debugger = a_rootElement.getOwnerDocument().createElement("Debugger");

            Attr attr = a_rootElement.getOwnerDocument().createAttribute("bitset");
            BitSet l_bitset = grabBitSet();
            attr.setValue(String.valueOf(Util.bits2Int(l_bitset)));
            a_Debugger.setAttributeNode(attr);

            a_rootElement.appendChild(a_Debugger);
        }

        private BitSet grabBitSet()
        {
            BitSet l_bitset = new BitSet(32);

            l_bitset.set(0, ShowDebuggerOnStart);

            l_bitset.set(1, AlignMemoryWindowStart);


            return l_bitset;
        }


//            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
        public boolean ShowDebuggerOnStart;

//            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
        public boolean AlignMemoryWindowStart;


    };



    public String serialize()
    {
        String l_result = "";

        try {

            DocumentBuilderFactory dbFactory =
                    DocumentBuilderFactory.newInstance();
            DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
            Document doc = dBuilder.newDocument();

            Element rootElement = doc.createElement("Pcsx2Config");
            doc.appendChild(rootElement);

            Attr attr = doc.createAttribute("bitset");
            BitSet l_bitset = grabBitSet();
            attr.setValue(String.valueOf(Util.bits2Int(l_bitset)));
            rootElement.setAttributeNode(attr);

            Cpu.serialize(rootElement);

            GS.serialize(rootElement);

            Speedhacks.serialize(rootElement);

            Gamefixes.serialize(rootElement);

            Profiler.serialize(rootElement);

            Debugger.serialize(rootElement);

            TransformerFactory tf = TransformerFactory.newInstance();
            Transformer transformer = tf.newTransformer();
            transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
            StringWriter writer = new StringWriter();
            transformer.transform(new DOMSource(doc), new StreamResult(writer));
            l_result = writer.getBuffer().toString();//.replaceAll("\n|\r", "");


        } catch (Exception e) {

        }

        return l_result;
    }

    private BitSet grabBitSet()
    {
        BitSet l_bitset = new BitSet(32);

        l_bitset.set(0, CdvdVerboseReads);

        l_bitset.set(1, CdvdDumpBlocks);

        l_bitset.set(2, CdvdShareWrite);

        l_bitset.set(3, EnablePatches);

        l_bitset.set(4, EnableCheats);

        l_bitset.set(5, EnableWideScreenPatches);

        l_bitset.set(6, UseBOOT2Injection);

        l_bitset.set(7, BackupSavestate);

        l_bitset.set(8, McdEnableEjection);

        l_bitset.set(9, McdFolderAutoManage);

        l_bitset.set(10, MultitapPort0_Enabled);

        l_bitset.set(11, MultitapPort1_Enabled);

        l_bitset.set(12, ConsoleToStdio);

        l_bitset.set(13, HostFs);


        return l_bitset;
    }

//         [BitFieldInfo(0, 1), XmlIgnoreAttribute]
    public boolean CdvdVerboseReads;

//         [BitFieldInfo(1, 1), XmlIgnoreAttribute]
    public boolean CdvdDumpBlocks;

//         [BitFieldInfo(2, 1), XmlIgnoreAttribute]
    public boolean CdvdShareWrite;

//         [BitFieldInfo(3, 1), XmlIgnoreAttribute]
    public boolean EnablePatches;

//         [BitFieldInfo(4, 1), XmlIgnoreAttribute]
    public boolean EnableCheats;

//         [BitFieldInfo(5, 1), XmlIgnoreAttribute]
    public boolean EnableWideScreenPatches;

//         [BitFieldInfo(6, 1), XmlIgnoreAttribute]
    public boolean UseBOOT2Injection;

//         [BitFieldInfo(7, 1), XmlIgnoreAttribute]
    public boolean BackupSavestate;

//         [BitFieldInfo(8, 1), XmlIgnoreAttribute]
    public boolean McdEnableEjection;

//         [BitFieldInfo(9, 1), XmlIgnoreAttribute]
    public boolean McdFolderAutoManage;

//         [BitFieldInfo(10, 1), XmlIgnoreAttribute]
    public boolean MultitapPort0_Enabled;

//         [BitFieldInfo(11, 1), XmlIgnoreAttribute]
    public boolean MultitapPort1_Enabled;

//         [BitFieldInfo(12, 1), XmlIgnoreAttribute]
    public boolean ConsoleToStdio;

//         [BitFieldInfo(13, 1), XmlIgnoreAttribute]
    public boolean HostFs;

    public CpuOptions Cpu = new CpuOptions();

    public GSOptions GS = new GSOptions();

    public SpeedhackOptions Speedhacks = new SpeedhackOptions();

    public GamefixOptions Gamefixes = new GamefixOptions();

    public ProfilerOptions Profiler = new ProfilerOptions();

    public DebugOptions Debugger = new DebugOptions();

    public void reset() {

        CdvdVerboseReads = true;
        CdvdDumpBlocks = false;
        CdvdShareWrite = false;
        EnablePatches = true;
        EnableCheats = false;
        EnableWideScreenPatches = false;// !Settings.Default.DisableWideScreen;
        UseBOOT2Injection = false;
        BackupSavestate = true;
        McdEnableEjection = true;
        McdFolderAutoManage = true;
        MultitapPort0_Enabled = false;
        MultitapPort1_Enabled = false;
        ConsoleToStdio = false;
        HostFs = false;




        Cpu.Recompiler.EnableEE = true;
        Cpu.Recompiler.EnableIOP = true;
        Cpu.Recompiler.EnableVU0 = true;
        Cpu.Recompiler.EnableVU1 = true;
        Cpu.Recompiler.UseMicroVU0 = true;
        Cpu.Recompiler.UseMicroVU1 = true;
        Cpu.Recompiler.vuOverflow = true;
        Cpu.Recompiler.vuExtraOverflow = false;
        Cpu.Recompiler.vuSignOverflow = false;
        Cpu.Recompiler.vuUnderflow = false;
        Cpu.Recompiler.fpuOverflow = true;
        Cpu.Recompiler.fpuExtraOverflow = false;
        Cpu.Recompiler.fpuFullMode = false;
        Cpu.Recompiler.StackFrameChecks = false;
        Cpu.Recompiler.PreBlockCheckEE = false;
        Cpu.Recompiler.PreBlockCheckIOP = false;
        Cpu.Recompiler.EnableEECache = false;


        Cpu.sseMXCSR.InvalidOpFlag = false;
        Cpu.sseMXCSR.DenormalFlag = false;
        Cpu.sseMXCSR.DivideByZeroFlag = false;
        Cpu.sseMXCSR.OverflowFlag = false;
        Cpu.sseMXCSR.UnderflowFlag = false;
        Cpu.sseMXCSR.PrecisionFlag = false;
        Cpu.sseMXCSR.DenormalsAreZero = true;
        Cpu.sseMXCSR.InvalidOpMask = true;
        Cpu.sseMXCSR.DenormalMask = true;
        Cpu.sseMXCSR.DivideByZeroMask = true;
        Cpu.sseMXCSR.OverflowMask = true;
        Cpu.sseMXCSR.UnderflowMask = true;
        Cpu.sseMXCSR.PrecisionMask = true;
        Cpu.sseMXCSR.RoundingControl = 3;
        Cpu.sseMXCSR.FlushToZero = true;


        Cpu.sseVUMXCSR.InvalidOpFlag = false;
        Cpu.sseVUMXCSR.DenormalFlag = false;
        Cpu.sseVUMXCSR.DivideByZeroFlag = false;
        Cpu.sseVUMXCSR.OverflowFlag = false;
        Cpu.sseVUMXCSR.UnderflowFlag = false;
        Cpu.sseVUMXCSR.PrecisionFlag = false;
        Cpu.sseVUMXCSR.DenormalsAreZero = true;
        Cpu.sseVUMXCSR.InvalidOpMask = true;
        Cpu.sseVUMXCSR.DenormalMask = true;
        Cpu.sseVUMXCSR.DivideByZeroMask = true;
        Cpu.sseVUMXCSR.OverflowMask = true;
        Cpu.sseVUMXCSR.UnderflowMask = true;
        Cpu.sseVUMXCSR.PrecisionMask = true;
        Cpu.sseVUMXCSR.RoundingControl = 3;
        Cpu.sseVUMXCSR.FlushToZero = true;



        GS.SynchronousMTGS = false;
        GS.DisableOutput = false;
        GS.VsyncQueueSize = 2;
        GS.FrameLimitEnable = true;
        GS.FrameSkipEnable = false;
        GS.VsyncEnable = 0;// Pcsx2Config.GSOptions.VsyncMode.Off;
        GS.FramesToDraw = 2;
        GS.FramesToSkip = 2;
        GS.LimitScalar = 100;
        GS.FramerateNTSC = 5994;
        GS.FrameratePAL = 5000;



        Speedhacks.fastCDVD = false;
        Speedhacks.IntcStat = true;
        Speedhacks.WaitLoop = true;
        Speedhacks.vuFlagHack = true;
        Speedhacks.vuThread = false;
        Speedhacks.EECycleRate = 0;
        Speedhacks.EECycleSkip = 0;


        Profiler.Enabled = false;
        Profiler.RecBlocks_EE = true;
        Profiler.RecBlocks_IOP = true;
        Profiler.RecBlocks_VU0 = true;
        Profiler.RecBlocks_VU1 = true;


        Debugger.ShowDebuggerOnStart = false;
        Debugger.AlignMemoryWindowStart = true;

    }
}
