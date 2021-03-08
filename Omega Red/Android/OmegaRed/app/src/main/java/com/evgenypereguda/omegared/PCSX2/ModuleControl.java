package com.evgenypereguda.omegared.PCSX2;

import com.evgenypereguda.omegared.Models.IsoInfo;
import com.evgenypereguda.omegared.PCSX2.Modules.Module;
import com.evgenypereguda.omegared.PCSX2.Modules.ModuleManager;
import com.evgenypereguda.omegared.Util.Util;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

import java.io.File;
import java.io.IOException;
import java.io.StringReader;
import java.io.StringWriter;
import java.util.BitSet;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

public final class ModuleControl {

    private static ModuleControl m_Instance = null;

    private ModuleControl(){}

    public static ModuleControl getInstance()
    {
        if (m_Instance == null)
            m_Instance = new ModuleControl();

        return m_Instance;
    }

    public void init()
    {
        for(Module l_Module : ModuleManager.getInstance().getModules())
        {


            try {

                DocumentBuilderFactory dbFactory =
                        DocumentBuilderFactory.newInstance();
                DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
                Document doc = dBuilder.newDocument();

                Element rootElement = doc.createElement("Config");

                doc.appendChild(rootElement);

                Element l_PropertyNode = doc.createElement("Init");

                rootElement.appendChild(l_PropertyNode);



                switch (l_Module.ModuleType)
                {
                    case AudioRenderer:
                        break;
                    case VideoRenderer:
                        break;
                    case DEV9:
                        break;
                    case MemoryCard:
                        break;
                    case Pad:
                        break;
                    case CDVD:
                        break;
                    case FW:
                        break;
                    default:
                        break;
                }



                TransformerFactory tf = TransformerFactory.newInstance();
                Transformer transformer = tf.newTransformer();
                transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
                StringWriter writer = new StringWriter();
                transformer.transform(new DOMSource(doc), new StreamResult(writer));
                String l_result = writer.getBuffer().toString();//.replaceAll("\n|\r", "");


                l_Module.execute(l_result);
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
        }
    }

    public void close()
    {
        for(Module l_Module : ModuleManager.getInstance().getModules())
        {

            try {

                DocumentBuilderFactory dbFactory =
                        DocumentBuilderFactory.newInstance();
                DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
                Document doc = dBuilder.newDocument();

                Element rootElement = doc.createElement("Config");

                doc.appendChild(rootElement);

                Element l_PropertyNode = doc.createElement("Close");

                rootElement.appendChild(l_PropertyNode);


                TransformerFactory tf = TransformerFactory.newInstance();
                Transformer transformer = tf.newTransformer();
                transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
                StringWriter writer = new StringWriter();
                transformer.transform(new DOMSource(doc), new StreamResult(writer));
                String l_result = writer.getBuffer().toString();//.replaceAll("\n|\r", "");


                l_Module.execute(l_result);
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
        }
    }

    public void shutdown()
    {
        for(Module l_Module : ModuleManager.getInstance().getModules())
        {

            try {

                DocumentBuilderFactory dbFactory =
                        DocumentBuilderFactory.newInstance();
                DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
                Document doc = dBuilder.newDocument();

                Element rootElement = doc.createElement("Config");

                doc.appendChild(rootElement);

                Element l_PropertyNode = doc.createElement("Shutdown");

                rootElement.appendChild(l_PropertyNode);


                TransformerFactory tf = TransformerFactory.newInstance();
                Transformer transformer = tf.newTransformer();
                transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
                StringWriter writer = new StringWriter();
                transformer.transform(new DOMSource(doc), new StreamResult(writer));
                String l_result = writer.getBuffer().toString();//.replaceAll("\n|\r", "");


                l_Module.execute(l_result);
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }

            if(l_Module.ModuleType == ModuleManager.ModuleType.VideoRenderer)
                PCSX2LibNative.getInstance().MTGS_CancelFunc();
        }
    }

    public void open()
    {
        for(Module l_Module : ModuleManager.getInstance().getModules())
        {

            try {

                DocumentBuilderFactory dbFactory =
                        DocumentBuilderFactory.newInstance();
                DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
                Document doc = dBuilder.newDocument();

                Element rootElement = doc.createElement("Config");

                doc.appendChild(rootElement);

                Element l_PropertyNode = doc.createElement("Open");

                rootElement.appendChild(l_PropertyNode);



                switch (l_Module.ModuleType)
                {
                    case AudioRenderer:
                        break;
                    case VideoRenderer:
                        PCSX2LibNative.getInstance().MTGS_ResumeFunc();
                        break;
                    case DEV9:
                        break;
                    case MemoryCard:
                        break;
                    case Pad:
                        break;
                    case CDVD:
                        setCDVDOpenConfig(l_PropertyNode);
                        break;
                    case FW:
                        break;
                    default:
                        break;
                }



                TransformerFactory tf = TransformerFactory.newInstance();
                Transformer transformer = tf.newTransformer();
                transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
                StringWriter writer = new StringWriter();
                transformer.transform(new DOMSource(doc), new StreamResult(writer));
                String l_result = writer.getBuffer().toString();//.replaceAll("\n|\r", "");


                l_Module.execute(l_result);
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
        }


        PCSX2LibNative.getInstance().openPlugin_SPU2Func();

        PCSX2LibNative.getInstance().openPlugin_DEV9Func();

        PCSX2LibNative.getInstance().openPlugin_USBFunc();

        PCSX2LibNative.getInstance().openPlugin_FWFunc();

        PCSX2LibNative.getInstance().MTGS_WaitForOpenFunc();
    }


    private void setCDVDOpenConfig(Element a_PropertyNode)
    {
        if (PCSX2Controller.getInstance().getIsoInfo() == null)
            return;

        String l_filePath = PCSX2Controller.getInstance().getIsoInfo().FilePath;

        File l_file = new File(l_filePath);

        if (!l_file.exists())
            return;

        a_PropertyNode.setAttribute("FilePath", l_filePath);
    }

    public boolean areLoaded()
    {
        return true;
    }

    public static IsoInfo getGameDiscInfo(String aFilePath) throws ParserConfigurationException, IOException, SAXException {
        IsoInfo l_result = null;

        do
        {
            Module l_module = ModuleManager.getInstance().getModule(ModuleManager.ModuleType.CDVD);

            String l_commandResult = "";

            if (l_module != null)
            {

                try {

                    DocumentBuilderFactory dbFactory =
                            DocumentBuilderFactory.newInstance();
                    DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
                    Document doc = dBuilder.newDocument();

                    Element rootElement = doc.createElement("Commands");

                    doc.appendChild(rootElement);




                    Element l_PropertyNode = doc.createElement("Check");

                    rootElement.appendChild(l_PropertyNode);

                    l_PropertyNode.setAttribute("FilePath", aFilePath);



                    l_PropertyNode = doc.createElement("GetDiscSerial");

                    rootElement.appendChild(l_PropertyNode);

                    l_PropertyNode.setAttribute("FilePath", aFilePath);


                    TransformerFactory tf = TransformerFactory.newInstance();
                    Transformer transformer = tf.newTransformer();
                    transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
                    StringWriter writer = new StringWriter();
                    transformer.transform(new DOMSource(doc), new StreamResult(writer));
                    String l_xml_doc = writer.getBuffer().toString();//.replaceAll("\n|\r", "");


                    l_commandResult = l_module.execute(l_xml_doc);

                }
                catch (Exception e)
                {
                    e.printStackTrace();
                }

            }

            if (l_commandResult != null)
            {
                DocumentBuilderFactory dbFactory =
                        DocumentBuilderFactory.newInstance();
                DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();

                InputSource is = new InputSource(new StringReader(l_commandResult));

                Document doc = dBuilder.parse(is);

                if(doc == null)
                    break;

                Element rootElement = doc.getDocumentElement();

                if (rootElement != null)
                {

                    boolean l_bResult = false;

                    String l_isoType = "ISOTYPE_ILLEGAL";

                    NodeList l_NodeList = rootElement.getChildNodes();

                    if(l_NodeList == null)
                        break;

                    for(int i = 0; i < l_NodeList.getLength(); ++i)
                    {
                        Node l_node = l_NodeList.item(i);

                        if(l_node.getNodeName().equals("Result"))
                        {
                            NamedNodeMap l_Attributes = l_node.getAttributes();

                            if(l_Attributes != null)
                            {
                                Node l_attr = l_Attributes.getNamedItem("Command");

                                if(l_attr != null)
                                {
                                    if(l_attr.getNodeValue().compareToIgnoreCase("Check") == 0)
                                    {
                                        l_attr = l_Attributes.getNamedItem("State");

                                        if(l_attr != null)
                                        {
                                            l_bResult = Boolean.parseBoolean(l_attr.getNodeValue());
                                        }

                                        l_attr = l_Attributes.getNamedItem("IsoType");

                                        if(l_attr != null)
                                        {
                                            l_isoType = l_attr.getNodeValue();
                                        }
                                    }



                                    if(l_attr.getNodeValue().compareToIgnoreCase("GetDiscSerial") == 0)
                                    {

                                        IsoInfo local_result = new IsoInfo();

                                        local_result.IsoType = l_isoType.replace("ISOTYPE_", "");

                                        local_result.FilePath = aFilePath;

                                        Node l_GameDiscType = l_Attributes.getNamedItem("GameDiscType");

                                        Node l_DiscSerial = l_Attributes.getNamedItem("DiscSerial");

                                        Node l_DiscRegionType = l_Attributes.getNamedItem("DiscRegionType");

                                        Node l_SoftwareVersion = l_Attributes.getNamedItem("SoftwareVersion");

                                        Node l_ElfCRC = l_Attributes.getNamedItem("ElfCRC");

                                        if (l_GameDiscType == null
                                                || l_DiscSerial == null
                                                || l_DiscRegionType == null
                                                || l_SoftwareVersion == null
                                                || l_ElfCRC == null)
                                            break;

                                        if (l_GameDiscType != null)
                                        {
                                            local_result.GameDiscType = l_GameDiscType.getNodeValue();
                                        }

                                        if (l_DiscSerial != null)
                                        {
                                            local_result.DiscSerial = l_DiscSerial.getNodeValue();

                                            local_result.Title = local_result.DiscSerial;

//                                            var l_gameData = GameIndex.Instance.convert(local_result.DiscSerial);
//
//                                            if (l_gameData != null)
//                                            {
//                                                local_result.Title = l_gameData.FriendlyName;
//                                            }
                                        }

                                        if (l_DiscRegionType != null)
                                        {
                                            local_result.DiscRegionType = l_DiscRegionType.getNodeValue();
                                        }

                                        if (l_SoftwareVersion != null)
                                        {
                                            local_result.SoftwareVersion = l_SoftwareVersion.getNodeValue();
                                        }

                                        if (l_ElfCRC != null)
                                        {
                                            local_result.ElfCRC = Long.parseLong(l_ElfCRC.getNodeValue());
                                        }

                                        l_result = local_result;
                                    }

                                }

                            }
                        }
                    }



//                    l_CheckNode = l_XmlDocument.DocumentElement.SelectSingleNode("Result[@Command='GetDiscSerial']");
//
//                    if (l_CheckNode != null
//                            && l_bResult
//                            && (l_isoType != IsoType.ISOTYPE_ILLEGAL)
//                            && (l_isoType != IsoType.ISOTYPE_AUDIO))
                    {
                    }
                }
            }

        } while (false);

        return l_result;
    }
}
