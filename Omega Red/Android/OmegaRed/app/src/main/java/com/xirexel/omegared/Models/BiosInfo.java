package com.xirexel.omegared.Models;

import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import java.io.File;

public final class BiosInfo {

    public static BiosInfo create(
            String Zone,
            String Version,
            int VersionInt,
            String Data,
            String Build,
            long CheckSum,
            String FilePath,
            byte[] NVM,
            byte[] MEC
    )
    {
        BiosInfo l_BiosInfo = new BiosInfo();

        l_BiosInfo.Zone = Zone;
        l_BiosInfo.Version = Version;
                l_BiosInfo.VersionInt = VersionInt;
                l_BiosInfo.Data = Data;
                l_BiosInfo.Build = Build;
                l_BiosInfo.CheckSum = CheckSum;
                l_BiosInfo.FilePath = FilePath;
                l_BiosInfo.IsCurrent = false;
                l_BiosInfo.NVM = NVM;
                l_BiosInfo.MEC = MEC;

        return l_BiosInfo;
    }

    public String Zone;

    public String Version;

    public int VersionInt;

    public String Data;

    public String Build;

    public long CheckSum;

    public String FilePath;

    public boolean IsCurrent;

    public byte[] NVM;

    public byte[] MEC;

    @Override
    public boolean equals(Object obj) {

        if(obj == null)
            return false;

        BiosInfo l_BiosInfo = (BiosInfo)obj;

        if(l_BiosInfo == null)
            return false;

        return CheckSum == l_BiosInfo.CheckSum;
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

            if(l_node.getNodeName().equals("Zone"))
            {
                Zone = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("Version"))
            {
                Version = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("VersionInt"))
            {
                VersionInt = Integer.parseInt(l_node.getTextContent());
            }
            else
            if(l_node.getNodeName().equals("Data"))
            {
                Data = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("Build"))
            {
                Build = l_node.getTextContent();
            }
            else
            if(l_node.getNodeName().equals("CheckSum"))
            {
                CheckSum = Long.parseLong(l_node.getTextContent());
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
            else
            if(l_node.getNodeName().equals("NVM"))
            {
                String l_value = l_node.getTextContent();

                if(l_value != null)
                {
                    String[] l_splits = l_value.split(":");

                    NVM = new byte[l_splits.length];

                    int l_idex = 0;

                    for (String l_item :
                            l_splits) {

                        NVM[l_idex++] = Byte.parseByte(l_item);
                    }
                }
            }
            else
            if(l_node.getNodeName().equals("MEC"))
            {
                String l_value = l_node.getTextContent();

                if(l_value != null)
                {
                    String[] l_splits = l_value.split(":");

                    MEC = new byte[l_splits.length];

                    int l_idex = 0;

                    for (String l_item :
                            l_splits) {

                        MEC[l_idex++] = Byte.parseByte(l_item);
                    }
                }
            }
        }

        return l_result;
    }

    public void serialize(Element a_element)
    {
        Element l_element = a_element.getOwnerDocument().createElement("Zone");

        l_element.setTextContent(Zone);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("Version");

        l_element.setTextContent(Version);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("VersionInt");

        l_element.setTextContent(String.valueOf(VersionInt));

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("Data");

        l_element.setTextContent(Data);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("Build");

        l_element.setTextContent(Build);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("CheckSum");

        l_element.setTextContent(String.valueOf(CheckSum));

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("FilePath");

        l_element.setTextContent(FilePath);

        a_element.appendChild(l_element);



        l_element = a_element.getOwnerDocument().createElement("IsCurrent");

        l_element.setTextContent(String.valueOf(IsCurrent));

        a_element.appendChild(l_element);



        if(NVM != null)
        {
            l_element = a_element.getOwnerDocument().createElement("NVM");

            StringBuilder l_data = new StringBuilder();

            for(int i = 0; i < NVM.length; ++i)
            {
                byte l_value = NVM[i];

                l_data.append(l_value).append(":");
            }

            l_element.setTextContent(l_data.toString());

            a_element.appendChild(l_element);
        }



        if(MEC != null)
        {
            l_element = a_element.getOwnerDocument().createElement("MEC");

            StringBuilder l_data = new StringBuilder();

            for(int i = 0; i < MEC.length; ++i)
            {
                byte l_value = MEC[i];

                l_data.append(l_value).append(":");
            }

            l_element.setTextContent(l_data.toString());

            a_element.appendChild(l_element);
        }

    }
}
