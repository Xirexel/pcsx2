using Golden_Phi.Emulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Golden_Phi.Models
{
    public class ContainerFile
    {
        public ContainerFile()
        { }
        public ContainerFile(string a_fileName)
        {
            m_fileName = a_fileName;
        }

        private string m_fileName = "";

        public override string ToString()
        {
            return m_fileName;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ContainerFile)
            {
                return (obj as ContainerFile).m_fileName == m_fileName;
            }

            return false;
        }
    }

    public class BiosInfo
    {
        public string Zone { get; set; }

        public string Version { get; set; }

        public int VersionInt { get; set; }

        public string Data { get; set; }

        public string Build { get; set; }

        public uint CheckSum { get; set; }

        public string FilePath { get; set; }

        public bool IsCurrent { get; set; }

        public byte[] NVM { get; set; }

        public byte[] MEC { get; set; }

        public GameType GameType { get; set; }

        public ContainerFile ContainerFile { get; set; } = new ContainerFile();

        [XmlIgnoreAttribute]
        public bool Focusable { get; set; } = true;
    }
}
