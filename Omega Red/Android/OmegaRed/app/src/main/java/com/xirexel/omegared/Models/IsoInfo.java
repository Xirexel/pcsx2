package com.xirexel.omegared.Models;

import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import java.io.File;

public class IsoInfo {

    public static IsoInfo create(
            String DiscRegionType,
            String DiscSerial,
            long ElfCRC,
            String GameDiscType,
            String IsoType,
            String SoftwareVersion,
            String Title,
            String FilePath
    )
    {
        IsoInfo l_IsoInfo = new IsoInfo();

        l_IsoInfo.DiscRegionType = DiscRegionType;
        l_IsoInfo.DiscSerial = DiscSerial;
        l_IsoInfo.ElfCRC = ElfCRC;
        l_IsoInfo.GameDiscType = GameDiscType;
        l_IsoInfo.IsoType = IsoType;
        l_IsoInfo.SoftwareVersion = SoftwareVersion;
        l_IsoInfo.Title = Title;
        l_IsoInfo.FilePath = FilePath;
        l_IsoInfo.IsCurrent = false;

        return l_IsoInfo;
    }

    public String Title;

    public String IsoType;

    public String GameDiscType;

    public String DiscSerial;

    public String DiscRegionType;

    public String SoftwareVersion;

    public long    ElfCRC;

    public String FilePath;

    public boolean IsCurrent;

    @Override
    public boolean equals(Object obj) {

        if(obj == null)
            return false;

        IsoInfo l_IsoInfo = (IsoInfo)obj;

        if(l_IsoInfo == null)
            return false;

        return DiscSerial == l_IsoInfo.DiscSerial;
    }

    public boolean deserialize(Element a_element)
    {
        if(a_element == null)
            return false;

        NodeList l_NodeList = a_element.getChildNodes();

        if(l_NodeList == null)
            return false;

        boolean l_result = true;

        for (int i = 0; i < l_NodeList.getLength(); ++i)
        {
            Node l_node = l_NodeList.item(i);

            if(l_node == null)
                continue;

            if(l_node.getNodeName().equals("Title"))
            {
                Title = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("IsoType"))
            {
                IsoType = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("GameDiscType"))
            {
                GameDiscType = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("DiscSerial"))
            {
                DiscSerial = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("DiscRegionType"))
            {
                DiscRegionType = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("SoftwareVersion"))
            {
                SoftwareVersion = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("ElfCRC"))
            {
                ElfCRC = Long.parseLong(l_node.getTextContent());
            }
            else
            if(l_node.getNodeName().equals("FilePath"))
            {
                FilePath = l_node.getTextContent();

                File file = new File(FilePath);

                if (!file.exists())
                {
                    l_result = false;

                    break;
                }
            }
            else
            if(l_node.getNodeName().equals("IsCurrent"))
            {
                IsCurrent = Boolean.parseBoolean(l_node.getTextContent());
            }
        }

        return l_result;
    }

    public void serialize(Element a_element)
    {
        Element l_element = a_element.getOwnerDocument().createElement("Title");

        l_element.setTextContent(Title);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("IsoType");

        l_element.setTextContent(IsoType);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("GameDiscType");

        l_element.setTextContent(GameDiscType);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("DiscSerial");

        l_element.setTextContent(DiscSerial);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("DiscRegionType");

        l_element.setTextContent(DiscRegionType);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("SoftwareVersion");

        l_element.setTextContent(SoftwareVersion);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("ElfCRC");

        l_element.setTextContent(String.valueOf(ElfCRC));

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("FilePath");

        l_element.setTextContent(FilePath);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("IsCurrent");

        l_element.setTextContent(String.valueOf(IsCurrent));

        a_element.appendChild(l_element);
    }
}
