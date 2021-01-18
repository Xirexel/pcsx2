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
using Golden_Phi.Properties;
using System.IO.Compression;
using Golden_Phi.Managers;
using SevenZipExtractor;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using Golden_Phi.Emulators;

namespace Golden_Phi.Tools
{
    class BiosControl
    {
        static public event Action<string> ShowErrorEvent;

        private const int m_biosSize = 512 * 1024;

        public const int m_nvmSize = 1024;

        public const int m_ROMsize = 1024 * 1024 * 4;

        // NVM (eeprom) layout info
        [StructLayout(LayoutKind.Sequential)]
        public struct NVMLayout
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

        public static bool IsPSXBIOS(
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
                
                RomBlock l_RomBlock = new RomBlock();

                for (l_index = 0; l_index < stream.BaseStream.Length; l_index += l_RomBlock.fileName.Length)
                {
                    l_RomBlock = ByteToType<RomBlock>(stream);

                    if (l_RomBlock.getFileName().Contains("PS-X Realtime"))
                        break; /* found romdir */
                }

                if (l_index == stream.BaseStream.Length)
                {
                    break;
                }

                stream.BaseStream.Seek(0x7FF30, SeekOrigin.Begin);

                l_RomBlock = ByteToType<RomBlock>(stream);

                zone = "Unknown";

                if (l_RomBlock.getFileName().Contains("System ROM Ver"))
                {
                    if(stream.ReadByte() == 0x73 
                        && stream.ReadByte() == 0x69
                        && stream.ReadByte() == 0x6F
                        && stream.ReadByte() == 0x6E)
                    {
                        List<byte> l_bytes = new List<byte>();

                        var l_byte = stream.ReadByte();

                        bool l_start = false;

                        while (true)
                        {
                            if (l_start)
                                l_bytes.Add(l_byte);

                            if (l_byte == 0x20)
                            {
                                l_start = true;
                            }

                            l_byte = stream.ReadByte();

                            if (l_start && l_byte == 0x20)
                                break;
                        }

                        var l_version = System.Text.Encoding.ASCII.GetString(l_bytes.ToArray());
                        
                        var l_split_version = l_version.Split('.');

                        if(l_split_version != null &&
                            l_split_version.Length == 2)
                        {
                            int l_temp = 0;

                            version = "v";

                            if (int.TryParse(l_split_version[0], out l_temp))
                            {
                                versionInt = l_temp << 8;

                                version += string.Format("{0:00}", l_temp);
                            }

                            version += ".";

                            if (int.TryParse(l_split_version[1], out l_temp))
                            {
                                versionInt |= l_temp;

                                version += string.Format("{0:00}", l_temp);
                            }
                        }

                        l_bytes.Clear();

                        l_bytes.AddRange(stream.ReadBytes(8));

                        byte l_zoneByte = stream.ReadByte();

                        if(l_zoneByte == 0x20)
                        {
                            if(Convert.ToChar(l_bytes[6]) == '0')
                            {
                                l_bytes.Insert(6, Convert.ToByte('0'));

                                l_bytes.Insert(6, Convert.ToByte('2'));
                            }
                            else
                            {
                                l_bytes.Insert(6, Convert.ToByte('9'));

                                l_bytes.Insert(6, Convert.ToByte('1'));
                            }

                            l_zoneByte = stream.ReadByte();
                        }
                        else
                        {
                            l_bytes.Add(l_zoneByte);

                            l_bytes.Add(stream.ReadByte());

                            l_zoneByte = Convert.ToByte('J');
                        }

                        data = System.Text.Encoding.ASCII.GetString(l_bytes.ToArray());


                        switch (Convert.ToChar(l_zoneByte))
                        {
                            case 'A': zone = "USA"; break;
                            case 'E': zone = "Europe"; break;
                            case 'J':
                            default: zone = "Japan"; break;
                        }

                        build = "Console";

                        l_result = true;
                    }
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
            ref string build,
            ref GameType gameType)
        {
            bool l_result = false;

            do
            {

                int l_index;

                if (m_biosSize == stream.BaseStream.Length)
                {
                    l_result = IsPSXBIOS(
                        stream,
                        ref zone,
                        ref version,
                        ref versionInt,
                        ref data,
                        ref build);

                    gameType = GameType.PS1;

                    break;
                }

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
                               
                gameType = GameType.PS2;

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
            ref string build,
            ref GameType gameType)
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
                                ref build,
                                ref gameType);
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

        static private void showErrorEvent(string a_message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                if (ShowErrorEvent != null)
                    ShowErrorEvent(a_message);
            });
        }
        
        public static NVMLayout getNvmLayout(int BiosVersion)
        {
            NVMLayout nvmLayout = null;
            
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
