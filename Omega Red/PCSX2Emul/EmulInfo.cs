using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static PCSX2Emul.Tools.PCSX2ModuleManager;

namespace PCSX2Emul
{
    public class EmulInfo
    {
        public static string getGameDiscInfo(string aFilePath)
        {
            string l_result = "";
                          
            using (var l_module = new Module(ModuleType.CDVD))
            {                   

                XmlDocument l_XmlDocument = new XmlDocument();

                XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                l_XmlDocument.AppendChild(ldocNode);

                XmlNode rootNode = l_XmlDocument.CreateElement("Commands");


                XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Check");

                var l_Atrr = l_XmlDocument.CreateAttribute("FilePath");

                l_Atrr.Value = aFilePath;

                l_PropertyNode.Attributes.Append(l_Atrr);

                rootNode.AppendChild(l_PropertyNode);


                l_PropertyNode = l_XmlDocument.CreateElement("GetDiscSerial");

                l_Atrr = l_XmlDocument.CreateAttribute("FilePath");

                l_Atrr.Value = aFilePath;

                l_PropertyNode.Attributes.Append(l_Atrr);

                rootNode.AppendChild(l_PropertyNode);

                l_XmlDocument.AppendChild(rootNode);

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    l_XmlDocument.WriteTo(xmlTextWriter);

                    xmlTextWriter.Flush();

                    l_module.execute(stringWriter.GetStringBuilder().ToString(), out l_result);
                }

            }

            return l_result;
        }
    }
}
