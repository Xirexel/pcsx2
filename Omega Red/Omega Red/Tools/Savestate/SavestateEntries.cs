using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Omega_Red.Tools.Savestate
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

                if (lAttr != null && !string.IsNullOrWhiteSpace(lAttr.Value))
                {
                    mDate = lAttr.Value.Replace('.', '/');
                }

                lAttr = root.SelectSingleNode("@DurationInSeconds");

                if (lAttr != null && !string.IsNullOrWhiteSpace(lAttr.Value))
                {
                    string lvalue = lAttr.Value;

                    var lsplitList = lAttr.Value.Split(new char[] { '.', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (lsplitList != null && lsplitList.Length > 0)
                    {
                        if (lsplitList.Length == 1)
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
        }

        private void FreezeBios(ISaveStateBase a_writer)
        {
            a_writer.FreezeTag("BIOS");
        }

        public void FreezeIn(ILoadStateBase reader)
        {
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
