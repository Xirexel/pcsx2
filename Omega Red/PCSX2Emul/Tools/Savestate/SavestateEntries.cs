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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using PCSX2Emul.Util;

namespace PCSX2Emul.Tools.Savestate
{    
    class SavestateEntry_StateVersion : IBaseSavestateEntry
    {
        const Int32 g_SaveVersion = (0x9A0D << 16) | 0x0000;

        public string GetFilename()
        {
            return "PCSX2 Savestate Version.id";
        }
        
        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(g_SaveVersion);
        }

        public void FreezeIn(ILoadStateBase reader)
        {
        }
    }

    class SavestateEntry_Screenshot : IBaseSavestateEntry
    {
        public delegate byte[] Delegate();

        private Delegate m_Delegate = null;

        public SavestateEntry_Screenshot(Delegate a_Delegate = null)
        {
            m_Delegate = a_Delegate;
        }

        public string GetFilename()
        {
            return "Screenshot.jpg";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            if (m_Delegate != null)
            {
                byte[] l_data = m_Delegate();

                if (l_data != null && l_data.Length > 0)
                    writer.Freeze(l_data);
            }
        }

        public byte[] Parser(ILoadStateBase reader)
        {
            return reader.read();
        }

        public void FreezeIn(ILoadStateBase reader)
        {
        }
    }

    class SavestateEntry_TimeSession : IBaseSavestateEntry
    {
        private string mDate = "";

        private double mDurationInSeconds = 0.0;

        public SavestateEntry_TimeSession(string aDate = "", double aDurationInSeconds = 0.0)
        {
            mDate = aDate;
            mDurationInSeconds = aDurationInSeconds;
        }

        public string GetFilename()
        {
            return "Time";
        }

        public void FreezeOut(ISaveStateBase writer)
        {

            XmlDocument lXmlDocument = new XmlDocument();

            XmlNode ldocNode = lXmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            lXmlDocument.AppendChild(ldocNode);

            XmlElement rootNode = lXmlDocument.CreateElement("TimeSession");

            lXmlDocument.AppendChild(rootNode);

            var lAtrr = lXmlDocument.CreateAttribute("Date");

            lAtrr.Value = mDate;

            rootNode.Attributes.Append(lAtrr);

            lAtrr = lXmlDocument.CreateAttribute("DurationInSeconds");

            lAtrr.Value = String.Format("{0:0}", mDurationInSeconds);

            Console.WriteLine(string.Format("save lAtrr.Value: {0}", lAtrr.Value));
            
            rootNode.Attributes.Append(lAtrr);

            string mXmltext = "";

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                lXmlDocument.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                mXmltext = stringWriter.GetStringBuilder().ToString();
            }

            writer.Freeze(Encoding.ASCII.GetBytes(mXmltext));
        }

        public Tuple<string, double> Parser(ILoadStateBase reader)
        {
            XmlDocument lXmlDocument = new XmlDocument();

            lXmlDocument.LoadXml(Encoding.ASCII.GetString(reader.read()));

            XmlElement root = lXmlDocument.DocumentElement;

            if (root != null)
            {
                var lAttr = root.SelectSingleNode("@Date");

                if(lAttr != null && !string.IsNullOrWhiteSpace(lAttr.Value))
                {
                    mDate = lAttr.Value.Replace('.','/');
                }
                
                lAttr = root.SelectSingleNode("@DurationInSeconds");

                if (lAttr != null && !string.IsNullOrWhiteSpace(lAttr.Value))
                {
                    string lvalue = lAttr.Value;
                    
                    var lsplitList = lAttr.Value.Split(new char[] { '.', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (lsplitList != null && lsplitList.Length > 0)
                    {
                        if(lsplitList.Length == 1)
                            lvalue = lsplitList[0];
                        else
                        {
                            lvalue = "";

                            for (int i = 0; i < lsplitList.Length - 1; i++)
                            {
                                if (lsplitList[i].Contains('.') || lsplitList[i].Contains(','))
                                    continue;

                                lvalue += lsplitList[i];
                            }
                        }
                    }
                    
                    Double.TryParse(lvalue, out mDurationInSeconds);

                    Console.WriteLine(string.Format("load lvalue: {0}, mDurationInSeconds: {1}", lvalue, mDurationInSeconds));
                }
            }

            return Tuple.Create<string, double>(mDate, mDurationInSeconds);
        }

        public void FreezeIn(ILoadStateBase reader)
        {
        }
    }

    class SavestateEntry_InternalStructures : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "PCSX2 Internal Structures.dat";
        }
        
        public void FreezeOut(ISaveStateBase writer)
        {
            FreezeBios(writer);

            writer.Freeze(PCSX2LibNative.Instance.getFreezeInternalsFunc());
        }
                
        private void FreezeBios(ISaveStateBase a_writer)
        {
            a_writer.FreezeTag( "BIOS" );

	        // Check the BIOS, and issue a warning if the bios for this state
	        // doesn't match the bios currently being used (chances are it'll still
	        // work fine, but some games are very picky).

            a_writer.Freeze(EmulInstance.InternalInstance.CheckSum);

            byte[] biosdesc = new byte[256];

            var l_bytes = Encoding.ASCII.GetBytes(EmulInstance.InternalInstance.Zone);

            l_bytes.CopyTo(biosdesc, 0);

            a_writer.Freeze(biosdesc);           	

        }
        
        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setFreezeInternalsFunc(reader.read());
        }

        public uint Parser(ILoadStateBase reader)
        {
            uint l_checkSum = 0;

            var l_data = reader.read();

            if (l_data != null && l_data.Length >= 36)
            {
                l_checkSum = BitConverter.ToUInt32(l_data, 32);
            }

            return l_checkSum;
        }
    }

    class SavestateEntry_EmotionMemory : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "eeMemory.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getEmotionMemoryFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setEmotionMemoryFunc(reader.read());
        }
    }

    class SavestateEntry_IopMemory : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "iopMemory.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getIopMemoryFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setIopMemoryFunc(reader.read());
        }
    }

    class SavestateEntry_HwRegs : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "eeHwRegs.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getHwRegsFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setHwRegsFunc(reader.read());
        }
    }

    class SavestateEntry_IopHwRegs : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "iopHwRegs.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getIopHwRegsFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setIopHwRegsFunc(reader.read());
        }
    }

    class SavestateEntry_Scratchpad : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "Scratchpad.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getScratchpadFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setScratchpadFunc(reader.read());
        }
    }

    class SavestateEntry_VU0mem : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "vu0Memory.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getVU0memFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setVU0memFunc(reader.read());
        }
    }

    class SavestateEntry_VU1mem : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "vu1Memory.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getVU1memFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setVU1memFunc(reader.read());
        }
    }

    class SavestateEntry_VU0prog : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "vu0MicroMem.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getVU0progFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setVU0progFunc(reader.read());
        }
    }

    class SavestateEntry_VU1prog : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "vu1MicroMem.bin";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getVU1progFunc());
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setVU1progFunc(reader.read());
        }
    }

    class PluginSavestateEntry : IBaseSavestateEntry
    {

        private  PCSX2ModuleManager.ModuleType m_ModuleType;

        public PluginSavestateEntry( PCSX2ModuleManager.ModuleType a_ModuleType )
	    {
		    m_ModuleType = a_ModuleType;
	    }
        
        public string GetFilename()
        {
	        return String.Format( "Plugin {0}.dat", PCSX2ModuleManager.getModuleTypeName(m_ModuleType));
        }
         
        public void FreezeOut(ISaveStateBase writer)
        {
            writer.Freeze(PCSX2LibNative.Instance.getFreezeOutFunc(PCSX2ModuleManager.getModuleID(m_ModuleType)));
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setFreezeInFunc(reader.read(), PCSX2ModuleManager.getModuleID(m_ModuleType));
        }

	    public bool IsRequired() { return false; }
    };

    class SavestateEntry_PPSSPPSState : IBaseSavestateEntry
    {
        private string m_TempFilePath = "";
        
        public SavestateEntry_PPSSPPSState(string a_TempFilePath = "")
        {
            m_TempFilePath = a_TempFilePath;
        }

        public string GetFilename()
        {
            return "PPSSPPSState";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            if (File.Exists(m_TempFilePath))
            {
                using (var lFileStream = File.OpenRead(m_TempFilePath))
                {
                    using (var stream = new MemoryStream((int)lFileStream.Length))
                    {
                        var lLength = (int)lFileStream.Length;

                        byte[] ltempData = new byte[100];

                        while (lLength > 0)
                        {
                            var l_readLength = lFileStream.Read(ltempData, 0, ltempData.Length);

                            stream.Write(ltempData, 0, l_readLength);

                            lLength -= l_readLength;
                        }
                        
                        stream.Position = 0; 

                        writer.Freeze(stream.ToArray());
                    }
                }
            }
        }
        
        public void FreezeIn(ILoadStateBase reader)
        {
            if (File.Exists(m_TempFilePath))
            {
                File.Delete(m_TempFilePath);
            }

            using (var lFileStream = File.OpenWrite(m_TempFilePath))
            {
                var l_data = reader.read();

                lFileStream.Write(l_data, 0, l_data.Length);
            }
        }
    }
    
    class SavestateEntry_PPSSPPInternalStructures : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "PCSX2 Internal Structures.dat";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            FreezeBios(writer);
        }

        private void FreezeBios(ISaveStateBase a_writer)
        {
            a_writer.FreezeTag("BIOS");

            // Check the BIOS, and issue a warning if the bios for this state
            // doesn't match the bios currently being used (chances are it'll still
            // work fine, but some games are very picky).

            a_writer.Freeze(1);

            byte[] biosdesc = new byte[256];

            var l_bytes = Encoding.ASCII.GetBytes("PPSSPP");

            l_bytes.CopyTo(biosdesc, 0);

            a_writer.Freeze(biosdesc);

        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setFreezeInternalsFunc(reader.read());
        }

        public uint Parser(ILoadStateBase reader)
        {
            uint l_checkSum = 0;

            var l_data = reader.read();

            if (l_data != null && l_data.Length >= 36)
            {
                l_checkSum = BitConverter.ToUInt32(l_data, 32);
            }

            return l_checkSum;
        }
    }

    class SavestateEntry_PCSXState : IBaseSavestateEntry
    {
        private string m_TempFilePath = "";

        public SavestateEntry_PCSXState(string a_TempFilePath = "")
        {
            m_TempFilePath = a_TempFilePath;
        }

        public string GetFilename()
        {
            return "PCSXState";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            if (File.Exists(m_TempFilePath))
            {
                using (var lFileStream = File.OpenRead(m_TempFilePath))
                {
                    using (var stream = new MemoryStream((int)lFileStream.Length))
                    {
                        var lLength = (int)lFileStream.Length;

                        byte[] ltempData = new byte[100];

                        while (lLength > 0)
                        {
                            var l_readLength = lFileStream.Read(ltempData, 0, ltempData.Length);

                            stream.Write(ltempData, 0, l_readLength);

                            lLength -= l_readLength;
                        }

                        stream.Position = 0;

                        writer.Freeze(stream.ToArray());
                    }
                }
            }
        }

        public void FreezeIn(ILoadStateBase reader)
        {
            if (File.Exists(m_TempFilePath))
            {
                File.Delete(m_TempFilePath);
            }

            using (var lFileStream = File.OpenWrite(m_TempFilePath))
            {
                var l_data = reader.read();

                lFileStream.Write(l_data, 0, l_data.Length);
            }
        }
    }

    class SavestateEntry_PCSXInternalStructures : IBaseSavestateEntry
    {
        public string GetFilename()
        {
            return "PCSX2 Internal Structures.dat";
        }

        public void FreezeOut(ISaveStateBase writer)
        {
            FreezeBios(writer);
        }

        private void FreezeBios(ISaveStateBase a_writer)
        {
            a_writer.FreezeTag("BIOS");

            // Check the BIOS, and issue a warning if the bios for this state
            // doesn't match the bios currently being used (chances are it'll still
            // work fine, but some games are very picky).

            a_writer.Freeze(2);

            byte[] biosdesc = new byte[256];

            var l_bytes = Encoding.ASCII.GetBytes("PPSSPP");

            l_bytes.CopyTo(biosdesc, 0);

            a_writer.Freeze(biosdesc);

        }

        public void FreezeIn(ILoadStateBase reader)
        {
            PCSX2LibNative.Instance.setFreezeInternalsFunc(reader.read());
        }

        public uint Parser(ILoadStateBase reader)
        {
            uint l_checkSum = 0;

            var l_data = reader.read();

            if (l_data != null && l_data.Length >= 36)
            {
                l_checkSum = BitConverter.ToUInt32(l_data, 32);
            }

            return l_checkSum;
        }
    }
}
