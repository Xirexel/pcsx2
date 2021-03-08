using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Fixed100 = System.UInt32;

namespace PCSX2Emul.Util
{
    [BitFieldNumberOfBitsAttribute(32)]
    public class Pcsx2Config : IBitField
    {
        // ------------------------------------------------------------------------
        public class CpuOptions
        {
            public CpuOptions()
            {
                Recompiler = new Pcsx2Config.RecompilerOptions();

                sseMXCSR = new SSE_MXCSR();

                sseVUMXCSR = new SSE_MXCSR();
            }

            [XmlElement]
            public RecompilerOptions Recompiler { get; set; }

            [XmlElement]
            public SSE_MXCSR sseMXCSR { get; set; }

            [XmlElement]
            public SSE_MXCSR sseVUMXCSR { get; set; }

        };

        // ------------------------------------------------------------------------
        [BitFieldNumberOfBitsAttribute(32)]
        public class RecompilerOptions : IBitField
        {
            [XmlAttribute]
            public int bitset { get { return Pcsx2Config.ToInt32(this); } set { } }


            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
            public bool EnableEE { get; set; }

            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
            public bool EnableIOP { get; set; }

            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
            public bool EnableVU0 { get; set; }

            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
            public bool EnableVU1 { get; set; }

            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
            public bool UseMicroVU0 { get; set; }

            [BitFieldInfo(5, 1), XmlIgnoreAttribute]
            public bool UseMicroVU1 { get; set; }

            [BitFieldInfo(6, 1), XmlIgnoreAttribute]
            public bool vuOverflow { get; set; }

            [BitFieldInfo(7, 1), XmlIgnoreAttribute]
            public bool vuExtraOverflow { get; set; }

            [BitFieldInfo(8, 1), XmlIgnoreAttribute]
            public bool vuSignOverflow { get; set; }

            [BitFieldInfo(9, 1), XmlIgnoreAttribute]
            public bool vuUnderflow { get; set; }

            [BitFieldInfo(10, 1), XmlIgnoreAttribute]
            public bool fpuOverflow { get; set; }

            [BitFieldInfo(11, 1), XmlIgnoreAttribute]
            public bool fpuExtraOverflow { get; set; }

            [BitFieldInfo(12, 1), XmlIgnoreAttribute]
            public bool fpuFullMode { get; set; }

            [BitFieldInfo(13, 1), XmlIgnoreAttribute]
            public bool StackFrameChecks { get; set; }

            [BitFieldInfo(14, 1), XmlIgnoreAttribute]
            public bool PreBlockCheckEE { get; set; }

            [BitFieldInfo(15, 1), XmlIgnoreAttribute]
            public bool PreBlockCheckIOP { get; set; }

            [BitFieldInfo(16, 1), XmlIgnoreAttribute]
            public bool EnableEECache { get; set; }
        };

        [BitFieldNumberOfBitsAttribute(32)]
        public class SSE_MXCSR : IBitField
        {
            [XmlAttribute]
            public int bitmask { get { return Pcsx2Config.ToInt32(this); } set { } }


            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
            public bool InvalidOpFlag { get; set; }

            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
            public bool DenormalFlag { get; set; }

            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
            public bool DivideByZeroFlag { get; set; }

            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
            public bool OverflowFlag { get; set; }

            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
            public bool UnderflowFlag { get; set; }

            [BitFieldInfo(5, 1), XmlIgnoreAttribute]
            public bool PrecisionFlag { get; set; }

            [BitFieldInfo(6, 1), XmlIgnoreAttribute]
            public bool DenormalsAreZero { get; set; }

            [BitFieldInfo(7, 1), XmlIgnoreAttribute]
            public bool InvalidOpMask { get; set; }

            [BitFieldInfo(8, 1), XmlIgnoreAttribute]
            public bool DenormalMask { get; set; }

            // This bit is supported only on SSE2 or better CPUs.  Setting it to 1 on
            // SSE1 cpus will result in an invalid instruction exception when executing
            // LDMXSCR.
            [BitFieldInfo(9, 1), XmlIgnoreAttribute]
            public bool DivideByZeroMask { get; set; }

            [BitFieldInfo(10, 1), XmlIgnoreAttribute]
            public bool OverflowMask { get; set; }

            [BitFieldInfo(11, 1), XmlIgnoreAttribute]
            public bool UnderflowMask { get; set; }

            [BitFieldInfo(12, 1), XmlIgnoreAttribute]
            public bool PrecisionMask { get; set; }

            [BitFieldInfo(13, 2), XmlIgnoreAttribute]
            public int RoundingControl { get; set; }

            [BitFieldInfo(15, 1), XmlIgnoreAttribute]
            public bool FlushToZero { get; set; }

            public void SetRoundMode(int mode)
            {
                RoundingControl = mode;
            }
        };

        // ------------------------------------------------------------------------        
        public class GSOptions
        {
            public enum VsyncMode
            {
                Off = 0,
                On = 1,
                Adaptive = 2,
            };

            // forces the MTGS to execute tags/tasks in fully blocking/synchronous
            // style.  Useful for debugging potential bugs in the MTGS pipeline.
            [XmlAttribute]
            public bool SynchronousMTGS { get; set; }

            [XmlAttribute]
            public bool DisableOutput { get; set; }

            [XmlAttribute]
            public int VsyncQueueSize { get; set; }


            [XmlAttribute]
            public bool FrameLimitEnable { get; set; }

            [XmlAttribute]
            public bool FrameSkipEnable { get; set; }

            [XmlAttribute]
            public int VsyncEnable { get; set; }


            [XmlAttribute]
            public int FramesToDraw { get; set; }	// number of consecutive frames (fields) to render

            [XmlAttribute]
            public int FramesToSkip { get; set; }	// number of consecutive frames (fields) to skip


            [XmlAttribute]
            public Fixed100 LimitScalar { get; set; }

            [XmlAttribute]
            public Fixed100 FramerateNTSC { get; set; }

            [XmlAttribute]
            public Fixed100 FrameratePAL { get; set; }


        };

        // ------------------------------------------------------------------------
        [BitFieldNumberOfBitsAttribute(32)]
        public class SpeedhackOptions : IBitField
        {
            [XmlAttribute]
            public int bitset { get { return Pcsx2Config.ToInt32(this); } set { } }


            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
            public bool fastCDVD { get; set; }

            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
            public bool IntcStat { get; set; }

            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
            public bool WaitLoop { get; set; }

            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
            public bool vuFlagHack { get; set; }

            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
            public bool vuThread { get; set; }


            [XmlAttribute]
            public sbyte EECycleRate;		// EE cycle rate selector (1.0, 1.5, 2.0)

            [XmlAttribute]
            public byte EECycleSkip;		// VU Cycle Stealer factor (0, 1, 2, or 3)

        };

        // ------------------------------------------------------------------------
        // NOTE: The GUI's GameFixes panel is dependent on the order of bits in this structure.
        [BitFieldNumberOfBitsAttribute(32)]
        public class GamefixOptions : IBitField
        {

            public enum GamefixId
            {
                GamefixId_FIRST = 0,

                Fix_VuAddSub = GamefixId_FIRST,
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

            [XmlAttribute]
            public int bitset { get { return Pcsx2Config.ToInt32(this); } set { } }


            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
            public bool VuAddSubHack { get; set; }

            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
            public bool VuClipFlagHack { get; set; }

            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
            public bool FpuCompareHack { get; set; }

            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
            public bool FpuMulHack { get; set; }

            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
            public bool FpuNegDivHack { get; set; }

            [BitFieldInfo(5, 1), XmlIgnoreAttribute]
            public bool XgKickHack { get; set; }

            [BitFieldInfo(6, 1), XmlIgnoreAttribute]
            public bool IPUWaitHack { get; set; }

            [BitFieldInfo(7, 1), XmlIgnoreAttribute]
            public bool EETimingHack { get; set; }

            [BitFieldInfo(8, 1), XmlIgnoreAttribute]
            public bool SkipMPEGHack { get; set; }

            [BitFieldInfo(9, 1), XmlIgnoreAttribute]
            public bool OPHFlagHack { get; set; }

            [BitFieldInfo(10, 1), XmlIgnoreAttribute]
            public bool DMABusyHack { get; set; }

            [BitFieldInfo(11, 1), XmlIgnoreAttribute]
            public bool VIFFIFOHack { get; set; }

            [BitFieldInfo(12, 1), XmlIgnoreAttribute]
            public bool VIF1StallHack { get; set; }

            [BitFieldInfo(13, 1), XmlIgnoreAttribute]
            public bool GIFFIFOHack { get; set; }

            [BitFieldInfo(14, 1), XmlIgnoreAttribute]
            public bool FMVinSoftwareHack { get; set; }

            [BitFieldInfo(15, 1), XmlIgnoreAttribute]
            public bool GoemonTlbHack { get; set; }

            [BitFieldInfo(16, 1), XmlIgnoreAttribute]
            public bool ScarfaceIbit { get; set; }



            public void Set(GamefixId id, bool enabled)
            {
                switch (id)
                {
                    case GamefixId.Fix_VuAddSub: VuAddSubHack = enabled; break;
                    case GamefixId.Fix_VuClipFlag: VuClipFlagHack = enabled; break;
                    case GamefixId.Fix_FpuCompare: FpuCompareHack = enabled; break;
                    case GamefixId.Fix_FpuMultiply: FpuMulHack = enabled; break;
                    case GamefixId.Fix_FpuNegDiv: FpuNegDivHack = enabled; break;
                    case GamefixId.Fix_XGKick: XgKickHack = enabled; break;
                    case GamefixId.Fix_IpuWait: IPUWaitHack = enabled; break;
                    case GamefixId.Fix_EETiming: EETimingHack = enabled; break;
                    case GamefixId.Fix_SkipMpeg: SkipMPEGHack = enabled; break;
                    case GamefixId.Fix_OPHFlag: OPHFlagHack = enabled; break;
                    case GamefixId.Fix_DMABusy: DMABusyHack = enabled; break;
                    case GamefixId.Fix_VIFFIFO: VIFFIFOHack = enabled; break;
                    case GamefixId.Fix_VIF1Stall: VIF1StallHack = enabled; break;
                    case GamefixId.Fix_GIFFIFO: GIFFIFOHack = enabled; break;
                    case GamefixId.Fix_FMVinSoftware: FMVinSoftwareHack = enabled; break;
                    case GamefixId.Fix_GoemonTlbMiss: GoemonTlbHack = enabled; break;
                    case GamefixId.Fix_ScarfaceIbit: ScarfaceIbit = enabled; break;
                    default:
                        break;
                }
            }
        };

        [BitFieldNumberOfBitsAttribute(32)]
        public class ProfilerOptions : IBitField
        {
            [XmlAttribute]
            public int bitset { get { return Pcsx2Config.ToInt32(this); } set { } }


            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
            public bool Enabled { get; set; }

            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
            public bool RecBlocks_EE { get; set; }

            [BitFieldInfo(2, 1), XmlIgnoreAttribute]
            public bool RecBlocks_IOP { get; set; }

            [BitFieldInfo(3, 1), XmlIgnoreAttribute]
            public bool RecBlocks_VU0 { get; set; }

            [BitFieldInfo(4, 1), XmlIgnoreAttribute]
            public bool RecBlocks_VU1 { get; set; }
        };

        [BitFieldNumberOfBitsAttribute(32)]
        public class DebugOptions : IBitField
        {
            [XmlAttribute]
            public int bitset { get { return Pcsx2Config.ToInt32(this); } set { } }


            [BitFieldInfo(0, 1), XmlIgnoreAttribute]
            public bool ShowDebuggerOnStart { get; set; }

            [BitFieldInfo(1, 1), XmlIgnoreAttribute]
            public bool AlignMemoryWindowStart { get; set; }


            //u8 FontWidth;
            //u8 FontHeight;
            //u32 WindowWidth;
            //u32 WindowHeight;
            //u32 MemoryViewBytesPerRow;

        };

        public Pcsx2Config()
        {
            Cpu = new CpuOptions();

            GS = new GSOptions();

            Speedhacks = new SpeedhackOptions();

            Gamefixes = new GamefixOptions();

            Profiler = new ProfilerOptions();

            Debugger = new DebugOptions();
        }

        public string serialize()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Pcsx2Config));

            MemoryStream stream = new MemoryStream();

            ser.Serialize(stream, this);

            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// Converts the members of the bit field to an integer value.
        /// </summary>
        /// <param name="obj">An instance of a struct that implements the interface IBitField.</param>
        /// <returns>An integer representation of the bit field.</returns>
        public static int ToInt32(IBitField obj)
        {
            int result = 0;

            // Loop through all the properties
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                // Check if the property has an attribute of type BitFieldLengthAttribute
                BitFieldInfoAttribute bitField;
                bitField = (pi.GetCustomAttribute(typeof(BitFieldInfoAttribute)) as BitFieldInfoAttribute);
                if (bitField != null)
                {
                    // Calculate a bitmask using the length of the bit field
                    int mask = 0;
                    for (byte i = 0; i < bitField.Length; i++)
                        mask |= 1 << i;

                    // This conversion makes it possible to use different types in the bit field
                    int value = Convert.ToInt32(pi.GetValue(obj));

                    result |= (value & mask) << bitField.Offset;
                }
            }

            return result;
        }

        //public static T CreateBitField<T>(int value) where T : struct
        //{
        //    // The created struct has to be boxed, otherwise PropertyInfo.SetValue
        //    // will work on a copy instead of the actual object
        //    object boxedValue = new T();

        //    // Loop through the properties and set a value to each one
        //    foreach (PropertyInfo pi in boxedValue.GetType().GetProperties())
        //    {
        //        BitFieldInfoAttribute bitField;
        //        bitField = (pi.GetCustomAttribute(typeof(BitFieldInfoAttribute)) as BitFieldInfoAttribute);
        //        if (bitField != null)
        //        {
        //            int mask = (int)Math.Pow(2, bitField.Length) - 1;
        //            object setVal = Convert.ChangeType((value >> bitField.Offset) & mask, pi.PropertyType);
        //            pi.SetValue(boxedValue, setVal);
        //        }
        //    }
        //    // Unboxing the object
        //    return (T)boxedValue;
        //}


        [XmlAttribute]
        public int bitset { get { return ToInt32(this); } set { } }


        [BitFieldInfo(0, 1), XmlIgnoreAttribute]
        public bool CdvdVerboseReads { get; set; }

        [BitFieldInfo(1, 1), XmlIgnoreAttribute]
        public bool CdvdDumpBlocks { get; set; }

        [BitFieldInfo(2, 1), XmlIgnoreAttribute]
        public bool CdvdShareWrite { get; set; }

        [BitFieldInfo(3, 1), XmlIgnoreAttribute]
        public bool EnablePatches { get; set; }

        [BitFieldInfo(4, 1), XmlIgnoreAttribute]
        public bool EnableCheats { get; set; }

        [BitFieldInfo(5, 1), XmlIgnoreAttribute]
        public bool EnableWideScreenPatches { get; set; }

        [BitFieldInfo(6, 1), XmlIgnoreAttribute]
        public bool UseBOOT2Injection { get; set; }

        [BitFieldInfo(7, 1), XmlIgnoreAttribute]
        public bool BackupSavestate { get; set; }

        [BitFieldInfo(8, 1), XmlIgnoreAttribute]
        public bool McdEnableEjection { get; set; }

        [BitFieldInfo(9, 1), XmlIgnoreAttribute]
        public bool McdFolderAutoManage { get; set; }

        [BitFieldInfo(10, 1), XmlIgnoreAttribute]
        public bool MultitapPort0_Enabled { get; set; }

        [BitFieldInfo(11, 1), XmlIgnoreAttribute]
        public bool MultitapPort1_Enabled { get; set; }

        [BitFieldInfo(12, 1), XmlIgnoreAttribute]
        public bool ConsoleToStdio { get; set; }

        [BitFieldInfo(13, 1), XmlIgnoreAttribute]
        public bool HostFs { get; set; }

        [XmlElement]
        public CpuOptions Cpu { get; set; }

        [XmlElement]
        public GSOptions GS { get; set; }

        [XmlElement]
        public SpeedhackOptions Speedhacks { get; set; }

        [XmlElement]
        public GamefixOptions Gamefixes { get; set; }

        [XmlElement]
        public ProfilerOptions Profiler { get; set; }

        [XmlElement]
        public DebugOptions Debugger { get; set; }
    }
}
