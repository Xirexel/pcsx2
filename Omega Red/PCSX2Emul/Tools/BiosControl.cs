using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCSX2Emul.Tools
{
    class BiosControl
    {
        static public event Action<string> ShowErrorEvent;

        private const int m_biosSize = 512 * 1024;

        private const int m_nvmSize = 1024;

        public const int m_ROMsize = 1024 * 1024 * 4;

        // NVM (eeprom) layout info
        [StructLayout(LayoutKind.Sequential)]
        private struct NVMLayout
        {
            public int biosVer;	// bios version that this eeprom layout is for
            public int config0;	// offset of 1st config block
            public int config1;	// offset of 2nd config block
            public int config2;	// offset of 3rd config block
            public int consoleId;	// offset of console id (?)
            public int ilinkId;	// offset of ilink id (ilink mac address)
            public int modelNum;	// offset of ps2 model number (eg "SCPH-70002")
            public int regparams;	// offset of RegionParams for PStwo
            public int mac;		// offset of the value written to 0xFFFE0188 and 0xFFFE018C on PStwo

            public static implicit operator NVMLayout(int[] a_data)
            {
                if (a_data == null || a_data.Length != 9)
                    return new NVMLayout();
                else
                    return new NVMLayout()
                    {
                        biosVer = a_data[0],
                        config0 = a_data[1],
                        config1 = a_data[2],
                        config2 = a_data[3],
                        consoleId = a_data[4],
                        ilinkId = a_data[5],
                        modelNum = a_data[6],
                        regparams = a_data[7],
                        mac = a_data[8]
                    };
            }
        };

        static NVMLayout[] nvmlayouts = new NVMLayout[2]
        {
            new int[]{0x000,  0x280, 0x300, 0x200, 0x1C8, 0x1C0, 0x1A0, 0x180, 0x198},	// eeproms from bios v0.00 and up
	        new int[]{0x146,  0x270, 0x2B0, 0x200, 0x1C8, 0x1E0, 0x1B0, 0x180, 0x198},	// eeproms from bios v1.70 and up
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct RomDir
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] fileName;

            public UInt16 extInfoSize;

            public UInt32 fileSize;

            public string getFileName()
            {
                var l_result = System.Text.Encoding.ASCII.GetString(fileName);

                return l_result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RomBlock
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] fileName;

            public string getFileName()
            {
                var l_result = System.Text.Encoding.ASCII.GetString(fileName);

                return l_result;
            }
        }

        private static T ByteToType<T>(BinaryReader a_stream)
        {

            var l_buffer = a_stream.ReadBytes(Marshal.SizeOf(typeof(T))); //Read bytes 

            var pinnedRawData = GCHandle.Alloc(l_buffer,
                                   GCHandleType.Pinned);

            try
            {
                // Get the address of the data array
                var pinnedRawDataPtr = pinnedRawData.AddrOfPinnedObject();

                // overlay the data type on top of the raw data
                return (T)Marshal.PtrToStructure(pinnedRawDataPtr, typeof(T));
            }
            finally
            {
                // must explicitly release
                pinnedRawData.Free();
            }

        }

        public static string FilePath { get; set; }
                
        public static uint getBIOSChecksum(string filename)
        {
            uint l_result = 0;

            do
            {
                try
                {

                    if (!File.Exists(filename))
                        break;

                    FileInfo l_FileInfo = new FileInfo(filename);

                    if (l_FileInfo == null)
                        break;

                    if (l_FileInfo.Length < m_biosSize)
                        break;

                    using (var l_stream = l_FileInfo.OpenRead())
                    {
                        if (l_stream == null)
                            break;

                        var l_byteStream = new BinaryReader(l_stream);

                        byte[] l_buffer = new byte[m_ROMsize];

                        l_byteStream.Read(l_buffer, 0, l_buffer.Length);

                        ChecksumIt(ref l_result, l_buffer);
                    }

                }
                catch (Exception)
                {
                }

            } while (false);

            return l_result;
        }

        public static uint getBIOSChecksum(byte[] l_memory)
        {
            uint l_result = 0;

            do
            {
                try
                {
                    ChecksumIt(ref l_result, l_memory);

                }
                catch (Exception)
                {
                }

            } while (false);

            return l_result;
        }

        // Loads the configured bios rom file into PS2 memory.  PS2 memory must be allocated prior to
        // this method being called.
        //
        // Remarks:
        //   This function does not fail if rom1, rom2, or erom files are missing, since none are
        //   explicitly required for most emulation tasks.
        //
        // Exceptions:
        //   BadStream - Thrown if the primary bios file (usually .bin) is not found, corrupted, etc.
        //
        static public bool LoadBIOS(IntPtr a_FirstArg, Int32 a_SecondArg)
        {
            bool l_result = false;

            try
            {
                do
                {

                    if (!File.Exists(FilePath))
                    {
                        var l_splitsFilePath = FilePath.Split(new char[] { '|' });

                        if (l_splitsFilePath == null || l_splitsFilePath.Length != 2)
                            break;

                        if (!File.Exists(l_splitsFilePath[0]))
                            break;

                        try
                        {
                            using (ArchiveFile archive = new ArchiveFile(l_splitsFilePath[0]))
                            {
                                var l_entry = archive.Entries.FirstOrDefault(p => p.FileName == l_splitsFilePath[1]);

                                if (l_entry != null)
                                {
                                    using (MemoryStream l_memoryStream = new MemoryStream())
                                    {
                                        try
                                        {
                                            l_entry.Extract(l_memoryStream);

                                            l_memoryStream.Position = 0;

                                            byte[] l_memory = l_memoryStream.ToArray();

                                            Marshal.Copy(l_memory, 0, a_FirstArg, Math.Min(a_SecondArg, l_memory.Length));
                                        }
                                        catch (Exception exc)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                        }
                    }
                    else
                    {
                        var filesize = new System.IO.FileInfo(FilePath).Length;

                        if (filesize <= 0)
                        {
                            throw new FileNotFoundException();
                        }

                        using (var l_FileStream = File.Open(FilePath, FileMode.Open))
                        {
                            if (l_FileStream == null)
                                break;

                            byte[] l_memory = new byte[l_FileStream.Length];

                            l_FileStream.Read(l_memory, 0, l_memory.Length);

                            Marshal.Copy(l_memory, 0, a_FirstArg, Math.Min(a_SecondArg, l_memory.Length));
                        }
                    }

                    l_result = true;

                } while (false);
            }
            catch (Exception)
            {
            }

            return l_result;
        }

        public static void CDVDGetMechaVer(IntPtr buffer)
        {
            using (var l_MECFileStream = new MemoryStream(EmulInstance.InternalInstance.Version))
            {
                if (l_MECFileStream == null)
                    return;

                byte[] l_buffer = new byte[4];

                l_MECFileStream.Read(l_buffer, 0, l_buffer.Length);

                Marshal.Copy(l_buffer, 0, buffer, l_buffer.Length);
            }
        }

        public static void NVMFile(IntPtr buffer, Int32 offset, Int32 bytes, Boolean read)
        {
            using (var l_NVMFileStream = new MemoryStream(EmulInstance.InternalInstance.NvmLayout))
            {
                if (l_NVMFileStream == null)
                    return;

                l_NVMFileStream.Seek(offset, SeekOrigin.Begin);

                byte[] l_buffer = new byte[bytes];

                int ret;

                if (read)
                {
                    ret = l_NVMFileStream.Read(l_buffer, 0, bytes);

                    Marshal.Copy(l_buffer, 0, buffer, l_buffer.Length);
                }
                else
                {
                    Marshal.Copy(buffer, l_buffer, 0, l_buffer.Length);

                    l_NVMFileStream.Write(l_buffer, 0, bytes);

                    EmulInstance.InternalInstance.update();
                }
            }
        }

        private static NVMLayout getNvmLayout()
        {
            NVMLayout nvmLayout = null;

            int BiosVersion = 0;

            BiosVersion = EmulInstance.InternalInstance.VersionInt;

            if (nvmlayouts[1].biosVer <= BiosVersion)
                nvmLayout = nvmlayouts[1];
            else
                nvmLayout = nvmlayouts[0];

            return nvmLayout;
        }

        private static void ChecksumIt(ref UInt32 result, byte[] srcdata)
        {
            for (int i = 0; i < srcdata.Length / 4; ++i)
                result ^= BitConverter.ToUInt32(srcdata, i * 4);
        }
    }
}