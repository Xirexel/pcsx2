using PCSXEmul.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PCSXEmul.Tools
{
    enum AspectRatio
    {
        None = 0,
        Ratio_4_3 = 1,
        Ratio_16_9 = 2
    }

    class ModuleControl
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr GetTouchPadCallback(UInt32 aPadIndex);

        public class PadControlConfig
        {
            public string Title_Key { get; set; }
            public string Instance_ID { get; set; }
            public string API { get; set; }
            public string Type { get; set; }
            public string Product_ID { get; set; }
            public string[] Bindings_Data { get; set; }
            public string[] Force_Feedback_Bindings_Data { get; set; }
        }

        private PadControlConfig m_PadInputConfig = new PadControlConfig()
        {
            Title_Key = "TouchPadTitle",
            Instance_ID = "Touch Pad 0",
            API = "18",
            Type = "3",
            Product_ID = "TOUCH PAD 0",
            Bindings_Data = new string[] {

                // old
                            //"0x00200000, 0, 20, 65536, 0, 0, 1, 0, 1",
                            //"0x00200001, 0, 22, 65536, 0, 0, 1, 0, 1",
                            //"0x00200002, 0, 23, 65536, 0, 0, 1, 0, 1",
                            //"0x00200003, 0, 21, 65536, 0, 0, 1, 0, 1",
                            //"0x00200004, 0, 19, 65536, 0, 0, 1, 0, 1",
                            //"0x00200005, 0, 16, 65536, 0, 0, 1, 0, 1",
                            //"0x00200006, 0, 17, 65536, 0, 0, 1, 0, 1",
                            //"0x00200007, 0, 18, 65536, 0, 0, 1, 0, 1",
                            //"0x00200008, 0, 24, 65536, 0, 0, 1, 0, 1",
                            //"0x00200009, 0, 25, 65536, 0, 0, 1, 0, 1",
                            //"0x0020000C, 0, 29, 65536, 0, 0, 1, 0, 1",
                            //"0x0020000D, 0, 30, 65536, 0, 0, 1, 0, 1",
                            //"0x0020000E, 0, 31, 65536, 0, 0, 1, 0, 1",
                            //"0x0020000F, 0, 28, 65536, 0, 0, 1, 0, 1",
                            //"0x00200010, 0, 26, 65536, 0, 0, 1, 0, 1",
                            //"0x00200011, 0, 27, 65536, 0, 0, 1, 0, 1",
                            //"0x01020013, 0, 32, 87183, 0, 0, 13172, 1, 1",
                            //"0x02020013, 0, 35, 65536, 0, 0, 13172, 0, 1",
                            //"0x01020014, 0, 40, 65536, 0, 0, 13172, 0, 1",
                            //"0x02020014, 0, 36, 65536, 0, 0, 13172, 0, 1",
                            //"0x01020015, 0, 36, 87183, 0, 0, 13172, 1, 1",
                            //"0x02020015, 0, 37, 65536, 0, 0, 13172, 0, 1",
                            //"0x01020016, 0, 38, 65536, 0, 0, 13172, 0, 1",
                            //"0x02020016, 0, 39, 65536, 0, 0, 13172, 0, 1"

                                                        
                            "0x0020000C, 0, 30, 65536, 0, 0, 1, 1",     // cross
                            "0x0020000D, 0, 29, 65536, 0, 0, 1, 1",     // circle
                            "0x0020000E, 0, 31, 65536, 0, 0, 1, 1",     // square
                            "0x0020000F, 0, 28, 65536, 0, 0, 1, 1",     // triangle
                            "0x00200005, 0, 16, 65536, 0, 0, 1, 0, 1",     // select/back      
                            "0x00200004, 0, 19, 65536, 0, 0, 1, 0, 1",    // start
							
                            "0x00200000, 0, 20, 65536, 0, 0, 1, 0, 1",     // D-Pad Up
                            "0x00200003, 0, 21, 65536, 0, 0, 1, 0, 1",     // D-Pad Right
                            "0x00200001, 0, 22, 65536, 0, 0, 1, 0, 1",     // D-Pad Down
                            "0x00200002, 0, 23, 65536, 0, 0, 1, 0, 1",     // D-Pad Left
														
                            "0x01020014, 0, 32, 65536, 0, 0, 13172, 1",	// L-Stick Up
                            "0x01020013, 0, 33, 65536, 0, 0, 13172, 1",	// L-Stick Right
                            "0x02020014, 0, 34, 65536, 0, 0, 13172, 1",	// L-Stick Down
                            "0x02020013, 0, 35, 65536, 0, 0, 13172, 1",	// L-Stick Left

                            "0x01020016, 0, 36, 65536, 0, 0, 13172, 1",	// R-Stick Up
                            "0x01020015, 0, 37, 65536, 0, 0, 13172, 1",	// R-Stick Right
                            "0x02020016, 0, 38, 65536, 0, 0, 13172, 1",	// R-Stick Down
                            "0x02020015, 0, 39, 65536, 0, 0, 13172, 1",	// R-Stick Left


                            "0x00200008, 0, 26, 65536, 0, 0, 1, 1",    	// L1
                            "0x00200010, 0, 24, 65536, 0, 0, 1, 1",		// L2
                            "0x00200006, 0, 17, 65536, 0, 0, 1, 1",    	// L3
                            "0x00200009, 0, 27, 65536, 0, 0, 1, 1",    	// R1
                            "0x00200011, 0, 25, 65536, 0, 0, 1, 1",		// R2
                            "0x00200007, 0, 18, 65536, 0, 0, 1, 1",    	// R3
            
            },
            Force_Feedback_Bindings_Data = new string[] {
                            "Constant 0, 0, 0, 1, 0, 65536, 1, 0",
                            "Constant 0, 1, 0, 1, 0, 0, 1, 65536"}
        };

        private int m_ports = 1;

        private IntPtr m_WindowHandler = IntPtr.Zero;

        private IntPtr m_VideoPanelHandler = IntPtr.Zero;

        private IntPtr m_TouchPadCallbackHandler = IntPtr.Zero;

        private IntPtr m_VideoTargetHandler = IntPtr.Zero;

        private IntPtr m_AudioCaptureTargetHandler = IntPtr.Zero;

        private string m_iso_file = "";

        private static ModuleControl m_Instance = null;

        public static ModuleControl Instance { get { if (m_Instance == null) m_Instance = new ModuleControl(); return m_Instance; } }


        public void setVideoPanelHandler(IntPtr a_VideoPanelHandler)
        {
            m_VideoPanelHandler = a_VideoPanelHandler;
        }

        public void setWindowHandler(IntPtr a_WindowHandler)
        {
            m_WindowHandler = a_WindowHandler;
        }

        public void setTouchPadCallbackHandler(IntPtr a_TouchPadCallbackHandler)
        {
            m_TouchPadCallbackHandler = a_TouchPadCallbackHandler;
        }

        public void setVideoTargetHandler(IntPtr a_VideoTargetHandler)
        {
            m_VideoTargetHandler = a_VideoTargetHandler;
        }

        public void setAudioCaptureTargetHandler(IntPtr a_AudioCaptureTargetHandler)
        {
            m_AudioCaptureTargetHandler = a_AudioCaptureTargetHandler;
        }
        public void setIsoFile(string a_iso_file)
        {
            m_iso_file = a_iso_file;
        }
        
        public void initPCSX()
        {
            foreach (var l_Module in PCSXModuleManager.Instance.Modules)
            {
                initPCSXModule(l_Module);
            }
        }

        private void initPCSXModule(PCSXModuleManager.Module a_Module)
        {

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Init");

            switch (a_Module.ModuleType)
            {
                case PCSXModuleManager.ModuleType.DFSound:
                    setAudioRendererConfig(l_XmlDocument, l_PropertyNode);
                    break;
                case PCSXModuleManager.ModuleType.DFXVideo:
                    setVideoRendererConfig(l_XmlDocument, l_PropertyNode);
                    break;
                case PCSXModuleManager.ModuleType.GPUHardware:
                    {
                        setVideoRendererConfig(l_XmlDocument, l_PropertyNode);
                    }
                    break;
                case PCSXModuleManager.ModuleType.Pad:
                    setPadConfig(l_XmlDocument, l_PropertyNode, m_TouchPadCallbackHandler);
                    break;
                case PCSXModuleManager.ModuleType.bladesio1:
                    break;
                default:
                    break;
            }

            rootNode.AppendChild(l_PropertyNode);

            if (a_Module.ModuleType == PCSXModuleManager.ModuleType.DFSound)
            {
                XmlNode l_OpenPropertyNode = l_XmlDocument.CreateElement("Open");
                setWindowHandlerOpenConfig(l_XmlDocument, l_OpenPropertyNode);
                rootNode.AppendChild(l_OpenPropertyNode);
            }

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                a_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }
        
        public void setIsFXAA(bool a_value)
        {
            var l_PCSXModule = PCSXModuleManager.Instance.getModule(PCSXModuleManager.ModuleType.GPUHardware);

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

                if (l_PCSXModule != null)
                    l_PCSXModule.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public void setMemoryCard(string a_file_path = null)
        {
            int l_slot = 0;

            if (string.IsNullOrWhiteSpace(a_file_path))
                l_slot = -1;

            PCSXNative.Instance.setMcd(a_file_path, l_slot);           

        }

        public void close()
        {
            foreach (var l_Module in PCSXModuleManager.Instance.Modules)
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
            }
        }
        
        public void shutdownPCSX()
        {
            foreach (var l_Module in PCSXModuleManager.Instance.Modules)
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
            }
        }
        
        private void setAudioRendererConfig(XmlDocument a_XmlDocument, XmlNode a_PropertyNode)
        {
            var l_Atrr = a_XmlDocument.CreateAttribute("CaptureHandler");

            l_Atrr.Value = m_AudioCaptureTargetHandler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);
        }

        private void setVideoRendererConfig(XmlDocument a_XmlDocument, XmlNode a_PropertyNode)
        {
            if (m_VideoPanelHandler == IntPtr.Zero)
                return;

            var l_Atrr = a_XmlDocument.CreateAttribute("ShareHandler");

            l_Atrr.Value = m_VideoPanelHandler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("CaptureHandler");

            l_Atrr.Value = m_VideoTargetHandler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);
        }

        private void setWindowHandlerOpenConfig(XmlDocument a_XmlDocument, XmlNode a_PropertyNode)
        {

            if (m_WindowHandler == IntPtr.Zero)
                return;

            var l_Atrr = a_XmlDocument.CreateAttribute("WindowHandle");

            l_Atrr.Value = m_WindowHandler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);
        }

        private void setPadConfig(XmlDocument a_XmlDocument, XmlNode a_PropertyNode, IntPtr a_getTouchPadCallbackHandler)
        {

            var l_Atrr = a_XmlDocument.CreateAttribute("GetTouchPadCallbackHandler");

            l_Atrr.Value = a_getTouchPadCallbackHandler.ToString();

            a_PropertyNode.Attributes.Append(l_Atrr);





            // Add devices

            var l_PadInputConfig = m_PadInputConfig;


            var l_touch_pad_node = a_XmlDocument.CreateElement("Device");


            l_Atrr = a_XmlDocument.CreateAttribute("Display_Name");

            l_Atrr.Value = m_PadInputConfig.ToString();

            l_touch_pad_node.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("Instance_ID");

            l_Atrr.Value = l_PadInputConfig.Instance_ID;

            l_touch_pad_node.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("API");

            l_Atrr.Value = l_PadInputConfig.API;

            l_touch_pad_node.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("Type");

            l_Atrr.Value = l_PadInputConfig.Type;

            l_touch_pad_node.Attributes.Append(l_Atrr);


            l_Atrr = a_XmlDocument.CreateAttribute("Product_ID");

            l_Atrr.Value = l_PadInputConfig.Product_ID;

            l_touch_pad_node.Attributes.Append(l_Atrr);

            // Bindings
            {
                // Regular Bindings

                foreach (var item in l_PadInputConfig.Bindings_Data)
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

                foreach (var item in l_PadInputConfig.Force_Feedback_Bindings_Data)
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



            //l_Atrr = a_XmlDocument.CreateAttribute("VibrationCallback");

            //l_Atrr.Value = Marshal.GetFunctionPointerForDelegate(PadControlManager.Instance.VibrationCallback).ToString();

            //a_PropertyNode.Attributes.Append(l_Atrr);

        }

        public void setSoundLevel(double a_value)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((object state) =>
            {
                var l_PCSXModule = PCSXModuleManager.Instance.getModule(PCSXModuleManager.ModuleType.DFSound);

                do
                {
                    if (l_PCSXModule == null)
                        break;

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

                        l_PCSXModule.execute(stringWriter.GetStringBuilder().ToString());
                    }

                } while (false);
            });
        }

        public void setIsMuted(bool a_value)
        {
            var l_PCSXModule = PCSXModuleManager.Instance.getModule(PCSXModuleManager.ModuleType.DFSound);

            System.Threading.ThreadPool.QueueUserWorkItem((object state) =>
            {
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
                    
                    l_PCSXModule.execute(stringWriter.GetStringBuilder().ToString());
                }

            });
        }

        public void setDiscSerial(string a_discSerial)
        {
            var l_PCSXModule = PCSXModuleManager.Instance.getModule(PCSXModuleManager.ModuleType.GPUHardware);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("DiscSerial");



            var l_Atrr = l_XmlDocument.CreateAttribute("Value");

            l_Atrr.Value = a_discSerial;

            l_PropertyNode.Attributes.Append(l_Atrr);



            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                l_PCSXModule.execute(stringWriter.GetStringBuilder().ToString());
            }
        }
        
        public void setLimitFrame(bool a_value)
        {
            var l_Module = PCSXModuleManager.Instance.getModule(PCSXModuleManager.ModuleType.GPUHardware);

            if (l_Module != null)
            {

                XmlDocument l_XmlDocument = new XmlDocument();

                XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                l_XmlDocument.AppendChild(ldocNode);

                XmlNode rootNode = l_XmlDocument.CreateElement("Config");

                XmlNode l_PropertyNode = l_XmlDocument.CreateElement("IsFrameLimit");



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

        public void closePad()
        {
            var l_Module = PCSXModuleManager.Instance.getModule(PCSXModuleManager.ModuleType.Pad);

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


        public void initPad()
        {
            var a_Module = PCSXModuleManager.Instance.getModule(PCSXModuleManager.ModuleType.Pad);

            XmlDocument l_XmlDocument = new XmlDocument();

            XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            l_XmlDocument.AppendChild(ldocNode);

            XmlNode rootNode = l_XmlDocument.CreateElement("Config");

            XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Init");

            setPadConfig(l_XmlDocument, l_PropertyNode, m_TouchPadCallbackHandler);

            rootNode.AppendChild(l_PropertyNode);

            l_XmlDocument.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                l_XmlDocument.WriteTo(xmlTextWriter);

                xmlTextWriter.Flush();

                a_Module.execute(stringWriter.GetStringBuilder().ToString());
            }
        }

        public void openPad()
        {
            var l_Module = PCSXModuleManager.Instance.getModule(PCSXModuleManager.ModuleType.Pad);

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
    }
}
