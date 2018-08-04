/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Omega_Red.Properties;
using System.IO.Compression;
using Omega_Red.Managers;

namespace Omega_Red.Tools
{
    class BiosControl
    {
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
                    return new NVMLayout() { 
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

        private static bool readBIOS(BinaryReader stream, ref string description)
        {            
            bool l_result = false;

            do
            {

                int l_index;

                RomDir l_RomDir = new RomDir();

                for (l_index = 0; l_index < m_biosSize; l_index++)
                {
                    l_RomDir = ByteToType<RomDir>(stream);
                    
                    if (l_RomDir.getFileName().Contains("RESET"))
                        break; /* found romdir */
                }

                if (l_index == 512*1024)
                {
                    throw new Exception("BIOS version check failed: 'RESET' tag could not be found.");
                }

                uint fileOffset = 0;

                while (l_RomDir.fileName[0] > 0)
                {
                    if (l_RomDir.getFileName().Contains("ROMVER"))
                    {

                        var l_startPosition = stream.BaseStream.Position;

                        stream.BaseStream.Position = fileOffset;

                        var l_romver = stream.ReadBytes(14);

                        stream.BaseStream.Position = l_startPosition;


                        string zone = Convert.ToString(Convert.ToChar(l_romver[4]));

                        switch(Convert.ToChar(l_romver[4]))
                        {
                            case 'T': zone = "T10K";	break;
                            case 'X': zone = "Test";	break;
                            case 'J': zone = "Japan";	break;
                            case 'A': zone = "USA";		break;
                            case 'E': zone = "Europe";	break;
                            case 'H': zone = "HK";		break;
                            case 'P': zone = "Free";	break;
                            case 'C': zone = "China";	break;
                        }

                        string vermaj = System.Text.Encoding.ASCII.GetString(new byte[]{ l_romver[0], l_romver[1] });
                        string vermin = System.Text.Encoding.ASCII.GetString(new byte[]{ l_romver[2], l_romver[3] });

                        description = string.Format("{0} v{1}.{2}({3}{4}/{5}{6}/{7}{8}{9}{10})  {11}",
                            zone,
                            vermaj, vermin,
                            Convert.ToChar(l_romver[12]), Convert.ToChar(l_romver[13]),	// day
                            Convert.ToChar(l_romver[10]), Convert.ToChar(l_romver[11]),	// month
                            Convert.ToChar(l_romver[6]), Convert.ToChar(l_romver[7]), Convert.ToChar(l_romver[8]), Convert.ToChar(l_romver[9]),	// year!
                            (Convert.ToChar(l_romver[5]) == 'C') ? "Console" : (Convert.ToChar(l_romver[5]) == 'D') ? "Devel" : "");

                        l_result = true;

                        break;
                    }

                    if ((l_RomDir.fileSize % 0x10) == 0)
                        fileOffset += l_RomDir.fileSize;
                    else
                        fileOffset += (l_RomDir.fileSize + 0x10) & 0xfffffff0;
                    
                    l_RomDir = ByteToType<RomDir>(stream);
                }
                                
            } while (false);

            return l_result;
        }

        public static bool IsBIOS(
            BinaryReader stream, 
            ref string zone, 
            ref string version, 
            ref int versionInt,
            ref string data, 
            ref string build)
        {
            bool l_result = false;

            do
            {

                int l_index;

                RomDir l_RomDir = new RomDir();

                for (l_index = 0; l_index < m_biosSize; l_index++)
                {
                    l_RomDir = ByteToType<RomDir>(stream);

                    if (l_RomDir.getFileName().Contains("RESET"))
                        break; /* found romdir */
                }

                if (l_index == m_biosSize)
                {
                    break;
                }

                uint fileOffset = 0;

                while (l_RomDir.fileName[0] > 0)
                {
                    if (l_RomDir.getFileName().Contains("ROMVER"))
                    {

                        var l_startPosition = stream.BaseStream.Position;

                        stream.BaseStream.Position = fileOffset;

                        var l_romver = stream.ReadBytes(14);

                        stream.BaseStream.Position = l_startPosition;


                        zone = Convert.ToString(Convert.ToChar(l_romver[4]));

                        switch (Convert.ToChar(l_romver[4]))
                        {
                            case 'T': zone = "T10K"; break;
                            case 'X': zone = "Test"; break;
                            case 'J': zone = "Japan"; break;
                            case 'A': zone = "USA"; break;
                            case 'E': zone = "Europe"; break;
                            case 'H': zone = "HK"; break;
                            case 'P': zone = "Free"; break;
                            case 'C': zone = "China"; break;
                        }

                        string vermaj = System.Text.Encoding.ASCII.GetString(new byte[] { l_romver[0], l_romver[1] });
                        string vermin = System.Text.Encoding.ASCII.GetString(new byte[] { l_romver[2], l_romver[3] });

                        version = string.Format("v{0}.{1}",
                            vermaj, vermin);

                        data = string.Format("{0}{1}/{2}{3}/{4}{5}{6}{7}",
                            Convert.ToChar(l_romver[12]), Convert.ToChar(l_romver[13]),	// day
                            Convert.ToChar(l_romver[10]), Convert.ToChar(l_romver[11]),	// month
                            Convert.ToChar(l_romver[6]), Convert.ToChar(l_romver[7]), Convert.ToChar(l_romver[8]), Convert.ToChar(l_romver[9])	// year!
                        );
                        
                        build = string.Format("{0}",
                            (Convert.ToChar(l_romver[5]) == 'C') ? "Console" : (Convert.ToChar(l_romver[5]) == 'D') ? "Devel" : "");

                        int l_temp = 0;
                        
                        if(int.TryParse(vermaj, out l_temp))
                        {
                            versionInt = l_temp << 8;
                        }

                        if (int.TryParse(vermin, out l_temp))
                        {
                            versionInt |= l_temp;
                        }                        

                        l_result = true;

                        break;
                    }

                    if ((l_RomDir.fileSize % 0x10) == 0)
                        fileOffset += l_RomDir.fileSize;
                    else
                        fileOffset += (l_RomDir.fileSize + 0x10) & 0xfffffff0;

                    l_RomDir = ByteToType<RomDir>(stream);
                }

            } while (false);

            return l_result;
        }

        public static bool IsBIOS(string filename)
        {
            string l_description = "";

            return IsBIOS(filename, ref l_description);
        }

        public static bool IsBIOS(
            string filename, 
            ref string zone,
            ref string version,
            ref int versionInt,
            ref string data,
            ref string build)
        {
            bool l_result = false;

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

                    using(var l_stream = l_FileInfo.OpenRead())
                    {
                        using (BinaryReader reader = new BinaryReader(l_stream))
                        {
                            l_result = IsBIOS(
                                reader,
                                ref zone,
                                ref version,
                                ref versionInt,
                                ref data,
                                ref build);
                        }
                    }
                }
                catch (Exception)
                {
                }

            } while (false);

            return l_result;
        }

        public static bool IsBIOS(string filename, ref string description)
        {
            bool l_result = false;

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

                    var l_stream = l_FileInfo.OpenRead();

                    if (l_stream == null)
                        break;

                    l_result = readBIOS(new BinaryReader(l_stream), ref description);

                }
                catch (Exception)
                {
                }
                
            } while (false);

            return l_result;
        }

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

                    using(var l_stream = l_FileInfo.OpenRead())
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
        static public void LoadBIOS(IntPtr a_FirstArg, Int32 a_SecondArg)
        {
	        //u8 ROM[Ps2MemSize::Rom];
	
	        try
	        {
                if (PCSX2Controller.Instance.BiosInfo == null)
                    return;

                var l_filePath = PCSX2Controller.Instance.BiosInfo.FilePath;


                if (!File.Exists(l_filePath))
                {
                    var l_splitsFilePath = l_filePath.Split(new char[] { '|' });

                    if (l_splitsFilePath == null || l_splitsFilePath.Length != 2)
                        return;

                    if (!File.Exists(l_splitsFilePath[0]))
                        return;

                    using (FileStream zipToOpen = new FileStream(l_splitsFilePath[0], FileMode.Open))
                    {
                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                        {
                            var l_entry = archive.GetEntry(l_splitsFilePath[1]);

                            if (l_entry != null)
                            {
                                using (BinaryReader reader = new BinaryReader(l_entry.Open()))
                                {
                                    try
                                    {
                                        byte[] l_memory = reader.ReadBytes(a_SecondArg);

                                        Marshal.Copy(l_memory, 0, a_FirstArg, Math.Min(a_SecondArg, l_memory.Length));
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }

                    }
                }
                else
                {
                    var filesize = new System.IO.FileInfo(l_filePath).Length;

                    if (filesize <= 0)
                    {
                        throw new FileNotFoundException();
                    }

                    using( var l_FileStream = File.Open(l_filePath, FileMode.Open))
                    {
                        if (l_FileStream == null)
                            return;

                        byte[] l_memory = new byte[l_FileStream.Length];

                        l_FileStream.Read(l_memory, 0, l_memory.Length);

                        Marshal.Copy(l_memory, 0, a_FirstArg, Math.Min(a_SecondArg, l_memory.Length));
                    }
                }
                




                //pxInputStream memfp(Bios, new wxMemoryInputStream(aPtrROM, aLength));
                //LoadBiosVersion( memfp, BiosVersion, BiosDescription, biosZone );

                //Console.SetTitle( pxsFmt( L"Running BIOS (%s v%u.%u)",
                //    WX_STR(biosZone), BiosVersion >> 8, BiosVersion & 0xff
                //));

                ////injectIRX("host.irx");	//not fully tested; still buggy

                ////LoadExtraRom( L"rom1", eeMem->ROM1 );
                ////LoadExtraRom( L"rom2", eeMem->ROM2 );
                ////LoadExtraRom( L"erom", eeMem->EROM );

                //if (g_Conf->CurrentIRX.Length() > 3)
                //    LoadIrx(g_Conf->CurrentIRX, &aPtrROM[0x3C0000]);

                //CurrentBiosInformation = NULL;
                //for (size_t i = 0; i < sizeof(biosVersions)/sizeof(biosVersions[0]); i++)
                //{
                //    if (biosVersions[i].biosChecksum == BiosChecksum && biosVersions[i].biosVersion == BiosVersion)
                //    {
                //        CurrentBiosInformation = &biosVersions[i];
                //        break;
                //    }
                //}
	        }
	        catch (Exception ex)
	        {
                //// Rethrow as a Bios Load Failure, so that the user interface handling the exceptions
                //// can respond to it appropriately.
                //throw Exception::BiosLoadFailed( ex.StreamName )
                //    .SetDiagMsg( ex.DiagMsg() )
                //    .SetUserMsg( ex.UserMsg() );
	        }
        }

        public static void CDVDGetMechaVer(IntPtr buffer)
        {
            if (PCSX2Controller.Instance.BiosInfo == null)
                return;

            if (PCSX2Controller.Instance.BiosInfo.MEC == null ||
                PCSX2Controller.Instance.BiosInfo.MEC.Length < 4)
            {
                PCSX2Controller.Instance.BiosInfo.MEC = new byte[4];

                byte[] version = { 0x3, 0x6, 0x2, 0x0 };

                using (MemoryStream l_memoryStream = new MemoryStream(PCSX2Controller.Instance.BiosInfo.MEC))
                {
                    l_memoryStream.Write(version, 0, version.Length);
                }

                BiosManager.Instance.save();
            }

            using (var l_MECFileStream = new MemoryStream(PCSX2Controller.Instance.BiosInfo.MEC))
            {
                if (l_MECFileStream == null)
                    return;

                byte[] l_buffer = new byte[4];

                l_MECFileStream.Read(l_buffer, 0, l_buffer.Length);

                Marshal.Copy(l_buffer, 0, buffer, l_buffer.Length);
            }


            //var l_filePath = PCSX2Controller.Instance.BiosInfo.FilePath;

            //if (!File.Exists(l_filePath))
            //    return;

            //string l_MECFilePath = Path.ChangeExtension(l_filePath, ".mec");

            //var filesize = new System.IO.FileInfo(l_MECFilePath).Length;
            
            //if (filesize < 4) {
                                
            //    using(var l_FileStream = File.Open(l_MECFilePath, FileMode.OpenOrCreate))
            //    {
            //        if(l_FileStream == null)
            //            return;

            //        byte[] version = { 0x3, 0x6, 0x2, 0x0 };
                                    
            //        l_FileStream.Write(version, 0, version.Length);

            //        l_FileStream.Flush();

            //        l_FileStream.Close();
            //    }
            //}

            //using(var l_FileStream = File.Open(l_MECFilePath, FileMode.OpenOrCreate))
            //{
            //    if(l_FileStream == null)
            //        return;

            //    byte[] l_buffer = new byte[4];
                
            //    l_FileStream.Read(l_buffer, 0, l_buffer.Length);

            //    Marshal.Copy(l_buffer, 0, buffer, l_buffer.Length);

            //    l_FileStream.Flush();

            //    l_FileStream.Close();
            //}
        }

        public static void NVMFile(IntPtr buffer, Int32 offset, Int32 bytes, Boolean read)
        {
            if (PCSX2Controller.Instance.BiosInfo == null)
                return;

            if(PCSX2Controller.Instance.BiosInfo.NVM == null ||
                PCSX2Controller.Instance.BiosInfo.NVM.Length < m_nvmSize)
            {
                PCSX2Controller.Instance.BiosInfo.NVM = new byte[m_nvmSize];

                NVMLayout nvmLayout = getNvmLayout();

                byte[] ILinkID_Data = { 0x00, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0xB9, 0x86 };

                using (MemoryStream l_memoryStream = new MemoryStream(PCSX2Controller.Instance.BiosInfo.NVM))
                {
                    l_memoryStream.Seek(nvmLayout.ilinkId, SeekOrigin.Begin);

                    l_memoryStream.Write(ILinkID_Data, 0, ILinkID_Data.Length);
                }

                BiosManager.Instance.save();
                

                //l_FileStream.Seek(*(int*)(((u8*)nvmLayout) + offsetof(NVMLayout, ilinkId)), SeekOrigin.Begin);

                
                //NVMLayout nvmLayout = getNvmLayout();

                //byte[] ILinkID_Data = { 0x00, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0xB9, 0x86 };

                //int lposition = nvmLayout.ilinkId;

                //foreach (var item in ILinkID_Data)
                //{
                //    PCSX2Controller.Instance.BiosInfo.NVM[lposition++] = item;
                //}
                               

                //l_FileStream.Seek(*(int*)(((u8*)nvmLayout) + offsetof(NVMLayout, ilinkId)), SeekOrigin.Begin);

            }


            //var l_filePath = PCSX2Controller.Instance.BiosInfo.FilePath;

            //if (!File.Exists(l_filePath))
            //    return;

            //string l_NVMFilePath = Path.ChangeExtension(l_filePath, ".nvm");

            //var filesize = new System.IO.FileInfo(l_NVMFilePath).Length;

            //if(filesize < 1024)
            //{
            //    using(var l_FileStream = File.Open(l_NVMFilePath, FileMode.OpenOrCreate))
            //    {
            //        if (l_FileStream == null)
            //            return;

            //        byte[] zero = new byte[1024];

            //        l_FileStream.Write(zero, 0, zero.Length);

            //        NVMLayout nvmLayout = getNvmLayout();

            //        byte[] ILinkID_Data = { 0x00, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0xB9, 0x86 };

            //        //l_FileStream.Seek(*(int*)(((u8*)nvmLayout) + offsetof(NVMLayout, ilinkId)), SeekOrigin.Begin);

            //        l_FileStream.Seek(nvmLayout.ilinkId, SeekOrigin.Begin);

            //        l_FileStream.Write(ILinkID_Data, 0, ILinkID_Data.Length);

            //        l_FileStream.Flush();

            //        l_FileStream.Close();
            //    }
            //}

            //var l_NVMFileStream = File.Open(l_NVMFilePath, FileMode.Open);

            using (var l_NVMFileStream = new MemoryStream(PCSX2Controller.Instance.BiosInfo.NVM))
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

                    BiosManager.Instance.save();
                }
            }

            
            //if (ret != bytes)
            //    Console.Error(L"Failed to %s %s. Did only %zu/%zu bytes",
            //    read ? L"read from" : L"write to", WX_STR(fname), ret, bytes);
        }

        private static NVMLayout getNvmLayout()
        {
            NVMLayout nvmLayout = null;

            int BiosVersion = 0;

            if (PCSX2Controller.Instance.BiosInfo != null)
                BiosVersion = PCSX2Controller.Instance.BiosInfo.VersionInt;

            if (nvmlayouts[1].biosVer <= BiosVersion)
                nvmLayout = nvmlayouts[1];
            else
                nvmLayout = nvmlayouts[0];

            return nvmLayout;
        }

        private static void ChecksumIt(ref UInt32 result, byte[]  srcdata)
        {
            for (int i = 0; i < srcdata.Length / 4; ++i)
                result ^= BitConverter.ToUInt32(srcdata, i * 4);
        }
    }
}
