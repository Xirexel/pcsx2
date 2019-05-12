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
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Omega_Red.Properties;
using Omega_Red.Panels;
using Omega_Red.Util;
using Omega_Red.Managers;
using System.Runtime.InteropServices;
using Omega_Red.Models;

namespace Omega_Red.Tools
{
    enum AspectRatio
    {
        None = 0,
        Ratio_4_3 = 1,
        Ratio_16_9 = 2        
    }

    class ModuleControl
    {
        private int m_ports = 1;

        private VideoPanel m_VideoPanel = null;

        private IntPtr m_WindowHandler = IntPtr.Zero;

        private static ModuleControl m_Instance = null;

        public static ModuleControl Instance { get { if (m_Instance == null) m_Instance = new ModuleControl(); return m_Instance; } }

        public void setVideoPanel(VideoPanel a_VideoPanel)
        {
            m_VideoPanel = a_VideoPanel;
        }

        public void setWindowHandler(IntPtr a_WindowHandler)
        {
            m_WindowHandler = a_WindowHandler;
        }

        public void init()
        {
            foreach (var l_Module in ModuleManager.Instance.Modules)
            {

                XmlDocument l_XmlDocument = new XmlDocument();

                XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                l_XmlDocument.AppendChild(ldocNode);

                XmlNode rootNode = l_XmlDocument.CreateElement("Config");

                XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Init");
                
                switch (l_Module.ModuleType)
                {
                    case ModuleManager.ModuleType.SPU2:
                        setAudioRendererConfig(l_XmlDocument, l_PropertyNode);
                        break;
                    case ModuleManager.ModuleType.VideoRenderer:
                        setVideoRendererConfig(l_XmlDocument, l_PropertyNode);
                        break;
                    case ModuleManager.ModuleType.DEV9:
                        break;
                    case ModuleManager.ModuleType.MemoryCard:
                        break;
                    case ModuleManager.ModuleType.Pad:
                        setPadConfig(l_Module, l_XmlDocument, l_PropertyNode);
                        break;
                    case ModuleManager.ModuleType.CDVD:
                        break;
                    case ModuleManager.ModuleType.FW:
                        break;
                    default:
                        break;
                }

                rootNode.AppendChild(l_PropertyNode);

                l_XmlDocument.AppendChild(rootNode);

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    l_XmlDocument.WriteTo(xmlTextWriter);

                    xmlTextWriter.Flush();

                    l_Module.execute(stringWriter.GetStringBuilder().ToString());
                }
            }
        }

        public void setGameCRC(uint a_CRC)
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.VideoRenderer);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("GameCRC");



            var l_Atrr = l_XmlDocument.CreateAttribute("Value");

            l_Atrr.Value = (a_CRC).ToString();

            l_PropertyNode.Attributes.Append(l_Atrr);



            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }
        
        public void setIsWired(bool a_value)
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.VideoRenderer);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("IsWired");



            var l_Atrr = l_XmlDocument.CreateAttribute("Value");

            l_Atrr.Value = (a_value?1:0).ToString();

            l_PropertyNode.Attributes.Append(l_Atrr);



            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }
               
        public void setIsTessellated(bool a_value)
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.VideoRenderer);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("IsTessellated");



            var l_Atrr = l_XmlDocument.CreateAttribute("Value");

            l_Atrr.Value = (a_value ? 1 : 0).ToString();

            l_PropertyNode.Attributes.Append(l_Atrr);



            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public void setIsFXAA(bool a_value)
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.VideoRenderer);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("IsFXAA");



            var l_Atrr = l_XmlDocument.CreateAttribute("Value");

            l_Atrr.Value = (a_value ? 1 : 0).ToString();

            l_PropertyNode.Attributes.Append(l_Atrr);



            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }
        
        public void setVideoAspectRatio(AspectRatio a_AspectRatio)
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.VideoRenderer);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("AspectRatio");



            var l_Atrr = l_XmlDocument.CreateAttribute("Value");

            l_Atrr.Value = ((int)a_AspectRatio).ToString();

            l_PropertyNode.Attributes.Append(l_Atrr);



            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }
        
        public void initPad()
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.Pad);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Init");

            setPadConfig(l_Module, l_XmlDocument, l_PropertyNode);

            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public void closePad()
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.Pad);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Close");

            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public void openPad()
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.Pad);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Open");
            
            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public void setMemoryCard(string a_file_path = null)
        {

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Close");
            
            rootNode.AppendChild(l_PropertyNode);

            if (!string.IsNullOrEmpty(a_file_path))
            {
                l_PropertyNode = l_XmlDocument.CreateElement("Init");

                var l_McdNode = l_XmlDocument.CreateElement("Mcd");


                var l_attributeNode = l_XmlDocument.CreateAttribute("FilePath");

                l_attributeNode.Value = a_file_path;

                l_McdNode.Attributes.Append(l_attributeNode);

                l_attributeNode = l_XmlDocument.CreateAttribute("Slot");

                l_attributeNode.Value = "0";

                l_McdNode.Attributes.Append(l_attributeNode);

                l_PropertyNode.AppendChild(l_McdNode);

                rootNode.AppendChild(l_PropertyNode);
            }

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.MemoryCard);

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public void close()
        {
            foreach (var l_Module in ModuleManager.Instance.Modules)
            {

                XmlDocument l_XmlDocument = new XmlDocument();

                XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                l_XmlDocument.AppendChild(ldocNode);

                XmlNode rootNode = l_XmlDocument.CreateElement("Config");

                XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Close");
                
                rootNode.AppendChild(l_PropertyNode);

                l_XmlDocument.AppendChild(rootNode);

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    l_XmlDocument.WriteTo(xmlTextWriter);

                    xmlTextWriter.Flush();

                    l_Module.execute(stringWriter.GetStringBuilder().ToString());
                }


                switch (l_Module.ModuleType)
                {
                    case ModuleManager.ModuleType.SPU2:
                        break;
                    case ModuleManager.ModuleType.VideoRenderer:
                        if(!PCSX2LibNative.Instance.MTGS_IsSelfFunc())
                            PCSX2LibNative.Instance.MTGS_SuspendFunc();
                        break;
                    case ModuleManager.ModuleType.DEV9:
                        break;
                    case ModuleManager.ModuleType.MemoryCard:
                        break;
                    case ModuleManager.ModuleType.Pad:
                        break;
                    case ModuleManager.ModuleType.CDVD:
                        break;
                    case ModuleManager.ModuleType.FW:
                        break;
                    default:
                        break;
                }
            }
        }

        public void shutdown()
        {
            foreach (var l_Module in ModuleManager.Instance.Modules)
            {

                XmlDocument l_XmlDocument = new XmlDocument();

                XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                l_XmlDocument.AppendChild(ldocNode);

                XmlNode rootNode = l_XmlDocument.CreateElement("Config");

                XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Shutdown");

                rootNode.AppendChild(l_PropertyNode);

                l_XmlDocument.AppendChild(rootNode);

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    l_XmlDocument.WriteTo(xmlTextWriter);

                    xmlTextWriter.Flush();

                    l_Module.execute(stringWriter.GetStringBuilder().ToString());
                }


                switch (l_Module.ModuleType)
                {
                    case ModuleManager.ModuleType.SPU2:
                        break;
                    case ModuleManager.ModuleType.VideoRenderer:
                        PCSX2LibNative.Instance.MTGS_CancelFunc();
                        break;
                    case ModuleManager.ModuleType.DEV9:
                        break;
                    case ModuleManager.ModuleType.MemoryCard:
                        break;
                    case ModuleManager.ModuleType.Pad:
                        break;
                    case ModuleManager.ModuleType.CDVD:
                        break;
                    case ModuleManager.ModuleType.FW:
                        break;
                    default:
                        break;
                }
            }

        }

        public void shutdownPad()
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.Pad);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Shutdown");

            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public Int32 doFreeze(IntPtr a_FirstArg, Int32 a_mode, Int32 a_ModuleCode)
        {
            Int32 l_result = -1;

            var l_ModuleType = ModuleManager.getModuleType(a_ModuleCode);

            foreach (var l_Module in ModuleManager.Instance.Modules)
            {
                if (l_Module.ModuleType != l_ModuleType)
                    continue;

                XmlDocument l_XmlDocument = new XmlDocument();

                XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                l_XmlDocument.AppendChild(ldocNode);

                XmlNode rootNode = l_XmlDocument.CreateElement("Config");

                XmlNode l_PropertyNode = l_XmlDocument.CreateElement("DoFreeze");


                var l_Atrr = l_XmlDocument.CreateAttribute("FreezeData");

                l_Atrr.Value = a_FirstArg.ToString();

                l_PropertyNode.Attributes.Append(l_Atrr);


                l_Atrr = l_XmlDocument.CreateAttribute("Mode");

                l_Atrr.Value = a_mode.ToString();

                l_PropertyNode.Attributes.Append(l_Atrr);


                
                
                rootNode.AppendChild(l_PropertyNode);

                l_XmlDocument.AppendChild(rootNode);

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    l_XmlDocument.WriteTo(xmlTextWriter);

                    xmlTextWriter.Flush();

                    l_Module.execute(stringWriter.GetStringBuilder().ToString());

                    l_result = 0;
                }
            }
            
            return l_result;
        }

        public void open()
        {
            foreach (var l_Module in ModuleManager.Instance.Modules)
            {

                XmlDocument l_XmlDocument = new XmlDocument();

                XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                l_XmlDocument.AppendChild(ldocNode);

                XmlNode rootNode = l_XmlDocument.CreateElement("Config");

                XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Open");

                switch (l_Module.ModuleType)
                {
                    case ModuleManager.ModuleType.SPU2:
                        setWindowHandlerOpenConfig(l_Module, l_XmlDocument, l_PropertyNode);
                        break;
                    case ModuleManager.ModuleType.VideoRenderer:
                        PCSX2LibNative.Instance.MTGS_ResumeFunc();
                        break;
                    case ModuleManager.ModuleType.DEV9:
                        break;
                    case ModuleManager.ModuleType.MemoryCard:
                        break;
                    case ModuleManager.ModuleType.Pad:
                        break;
                    case ModuleManager.ModuleType.CDVD:
                        setCDVDOpenConfig(l_Module, l_XmlDocument, l_PropertyNode);
                        break;
                    case ModuleManager.ModuleType.FW:
                        setWindowHandlerOpenConfig(l_Module, l_XmlDocument, l_PropertyNode);
                        break;
                    default:
                        break;
                }

                rootNode.AppendChild(l_PropertyNode);

                l_XmlDocument.AppendChild(rootNode);

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    l_XmlDocument.WriteTo(xmlTextWriter);

                    xmlTextWriter.Flush();

                    l_Module.execute(stringWriter.GetStringBuilder().ToString());
                }
            }

            if (App.m_AppType != App.AppType.Screen)
            {
                setVolume(0.0);
            }
            
            PCSX2LibNative.Instance.openPlugin_SPU2Func();

            PCSX2LibNative.Instance.openPlugin_DEV9Func();

            PCSX2LibNative.Instance.openPlugin_USBFunc();

            PCSX2LibNative.Instance.openPlugin_FWFunc();

            PCSX2LibNative.Instance.MTGS_WaitForOpenFunc();
        }

        public bool areLoaded()
        {
            return true;
        }
        
        public static string getAudioCaptureProcessor()
        {
            string l_result = "";

            do
            {
                var l_module = ModuleManager.Instance.getModule(Omega_Red.Tools.ModuleManager.ModuleType.SPU2);

                string l_commandResult = "";

                if (l_module != null)
                {
                    XmlDocument l_XmlDocument = new XmlDocument();

                    XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                    l_XmlDocument.AppendChild(ldocNode);

                    XmlNode rootNode = l_XmlDocument.CreateElement("Commands");


                    XmlNode l_PropertyNode = l_XmlDocument.CreateElement("GetCaptureProcessor");

                    rootNode.AppendChild(l_PropertyNode);

                    l_XmlDocument.AppendChild(rootNode);

                    using (var stringWriter = new StringWriter())
                    using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                    {
                        l_XmlDocument.WriteTo(xmlTextWriter);

                        xmlTextWriter.Flush();

                        l_module.execute(stringWriter.GetStringBuilder().ToString(), out l_commandResult);
                    }

                }

                if (!string.IsNullOrEmpty(l_commandResult))
                {
                    XmlDocument l_XmlDocument = new XmlDocument();

                    l_XmlDocument.LoadXml(l_commandResult);

                    if (l_XmlDocument.DocumentElement != null)
                    {

                        var l_bResult = false;

                        var l_CheckNode = l_XmlDocument.DocumentElement.SelectSingleNode("Result[@Command='GetCaptureProcessor']");

                        if (l_CheckNode != null)
                        {
                            var l_StateNode = l_CheckNode.SelectSingleNode("@State");

                            var l_Value = l_CheckNode.SelectSingleNode("@Value");

                            if (l_StateNode != null)
                            {
                                Boolean.TryParse(l_StateNode.Value, out l_bResult);
                            }

                            if (l_Value != null)
                            {
                                l_result = l_Value.Value;
                            }
                        }
                    }
                }

            } while (false);

            return l_result;
        }
        
        private void setAudioRendererConfig(XmlDocument a_XmlDocument, XmlNode a_PropertyNode)
        {
            if (m_VideoPanel == null)
                return;

            var l_Atrr = a_XmlDocument.CreateAttribute("CaptureHandler");

            l_Atrr.Value = Capture.AudioCaptureTarget.Instance.DataCallbackHandler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);
        }

        private void setVideoRendererConfig(XmlDocument a_XmlDocument, XmlNode a_PropertyNode)
        {
            if (m_VideoPanel == null)
                return;

            var l_Atrr = a_XmlDocument.CreateAttribute("ShareHandler");

            l_Atrr.Value = m_VideoPanel.SharedHandle.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);

            l_Atrr = a_XmlDocument.CreateAttribute("CaptureHandler");

            l_Atrr.Value = Capture.CaptureTargetTexture.Instance.CaptureHandler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);
        }

        private void setWindowHandlerOpenConfig(ModuleManager.Module a_module, XmlDocument a_XmlDocument, XmlNode a_PropertyNode)
        {

            if (m_WindowHandler == IntPtr.Zero)
                return;

            var l_Atrr = a_XmlDocument.CreateAttribute("WindowHandle");

            l_Atrr.Value = m_WindowHandler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);
        }


        private void setCDVDOpenConfig(ModuleManager.Module a_module, XmlDocument a_XmlDocument, XmlNode a_PropertyNode)
        {
            if (PCSX2Controller.Instance.IsoInfo == null)
                return;

            var l_filePath = PCSX2Controller.Instance.IsoInfo.FilePath;

            if (!File.Exists(l_filePath))
                return;

            var l_Atrr = a_XmlDocument.CreateAttribute("FilePath");

            l_Atrr.Value = l_filePath;

            a_PropertyNode.Attributes.Append(l_Atrr);
        }


        private void setPadConfig(ModuleManager.Module a_module, XmlDocument a_XmlDocument, XmlNode a_PropertyNode)
        {

            var l_Atrr = a_XmlDocument.CreateAttribute("TouchPadHandler");

            l_Atrr.Value = PadInput.Instance.Hanler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);





            // Add devices

            var lPadControlInfo = PadControlManager.Instance.PadControlInfo;

            if(App.m_AppType == App.AppType.OffScreen)
            {                
                if(PadControlManager.Instance.Collection.MoveCurrentToPosition(1))                    
                    lPadControlInfo = PadControlManager.Instance.Collection.CurrentItem as PadControlInfo;
            }


            var l_touch_pad_node = a_XmlDocument.CreateElement("Device");


            l_Atrr = a_XmlDocument.CreateAttribute("Display_Name");

            l_Atrr.Value = lPadControlInfo.ToString();

            l_touch_pad_node.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("Instance_ID");

            l_Atrr.Value = lPadControlInfo.Instance_ID;

            l_touch_pad_node.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("API");

            l_Atrr.Value = lPadControlInfo.API;

            l_touch_pad_node.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("Type");

            l_Atrr.Value = lPadControlInfo.Type;

            l_touch_pad_node.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("Product_ID");

            l_Atrr.Value = lPadControlInfo.Product_ID;

            l_touch_pad_node.Attributes.Append(l_Atrr);
                        
            // Bindings
            {
                // Regular Bindings

                foreach (var item in lPadControlInfo.Bindings_Data)
                {
                    var l_binding_node = a_XmlDocument.CreateElement("Binding");


                    l_Atrr = a_XmlDocument.CreateAttribute("Type");

                    l_Atrr.Value = "Regular";

                    l_binding_node.Attributes.Append(l_Atrr);


                    l_Atrr = a_XmlDocument.CreateAttribute("Data");

                    l_Atrr.Value = item;

                    l_binding_node.Attributes.Append(l_Atrr);


                    l_touch_pad_node.AppendChild(l_binding_node);
                }


                // Force Feedback Bindings

                foreach (var item in lPadControlInfo.Force_Feedback_Bindings_Data)
                {
                    var l_binding_node = a_XmlDocument.CreateElement("Binding");


                    l_Atrr = a_XmlDocument.CreateAttribute("Type");

                    l_Atrr.Value = "FF";

                    l_binding_node.Attributes.Append(l_Atrr);


                    l_Atrr = a_XmlDocument.CreateAttribute("Data");

                    l_Atrr.Value = item;

                    l_binding_node.Attributes.Append(l_Atrr);


                    l_touch_pad_node.AppendChild(l_binding_node);
                }

            }
            
            a_PropertyNode.AppendChild(l_touch_pad_node);

            l_Atrr = a_XmlDocument.CreateAttribute("Ports");

            l_Atrr.Value = m_ports.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);
            
            l_Atrr = a_XmlDocument.CreateAttribute("ButtonUpdateCallback");

            l_Atrr.Value = Marshal.GetFunctionPointerForDelegate(AdditionalControlManager.Instance.ButtonUpdateCallback).ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);

        }

        public void setVolume(double a_value)
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.SPU2);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Volume");



            var l_Atrr = l_XmlDocument.CreateAttribute("Value");

            l_Atrr.Value = String.Format("{0:0.##}", a_value * 100.0);

            l_PropertyNode.Attributes.Append(l_Atrr);



            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public void setIsMuted(bool a_value)
        {
            var l_Module = ModuleManager.Instance.getModule(ModuleManager.ModuleType.SPU2);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("IsMuted");



            var l_Atrr = l_XmlDocument.CreateAttribute("Value");

            l_Atrr.Value = (a_value ? 1 : 0).ToString();

            l_PropertyNode.Attributes.Append(l_Atrr);



            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }
    }
}
