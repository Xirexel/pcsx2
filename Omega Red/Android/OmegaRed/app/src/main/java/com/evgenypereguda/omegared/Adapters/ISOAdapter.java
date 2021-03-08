package com.evgenypereguda.omegared.Adapters;

import android.content.Context;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.content.res.TypedArray;
import android.os.Handler;
import android.preference.PreferenceManager;
import android.support.v4.content.res.ResourcesCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.evgenypereguda.omegared.GlobalApplication;
import com.evgenypereguda.omegared.Models.*;
import com.evgenypereguda.omegared.PCSX2.PCSX2Controller;
import com.evgenypereguda.omegared.R;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

import java.io.IOException;
import java.io.StringReader;
import java.io.StringWriter;
import java.util.ArrayList;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

public class ISOAdapter extends BaseAdapter {

    private static final String ISO_INFO_COLLECTION = "IsoInfoCollection";

    ArrayList<IsoInfo> m_Items = new ArrayList<IsoInfo>();

    private int m_selectedIndex = -1;

    final Handler mHandler = new Handler();

    private static ISOAdapter m_Instance = null;

    private ISOAdapter() {

        load();
    }

    public static ISOAdapter getInstance() {
        if (m_Instance == null)
            m_Instance = new ISOAdapter();

        return m_Instance;
    }

    public void addItem(IsoInfo a_item) {

        if (!m_Items.contains(a_item)) {
            m_Items.add(a_item);

            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    notifyDataSetChanged();
                }
            });

            save();
        }
    }

    private void save() {
        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(GlobalApplication.getAppContext());

        SharedPreferences.Editor editor = preferences.edit();

        try {

            String l_value = serialize();

            editor.putString(ISO_INFO_COLLECTION, l_value);
        } catch (ParserConfigurationException e) {
            e.printStackTrace();
        } catch (TransformerException e) {
            e.printStackTrace();
        }

        editor.commit();
    }

    private void load() {
        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(GlobalApplication.getAppContext());

        try {

            String l_value = preferences.getString(ISO_INFO_COLLECTION, "");

            deserialize(l_value);

            for (int i = 0; i < m_Items.size(); ++i) {
                IsoInfo l_IsoInfo = m_Items.get(i);

                if (l_IsoInfo != null && l_IsoInfo.IsCurrent) {
                    m_selectedIndex = i;

                    PCSX2Controller.getInstance().setIsoInfo(l_IsoInfo);

                    break;
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void deserialize(String a_value) throws ParserConfigurationException, IOException, SAXException {
        DocumentBuilderFactory dbFactory =
                DocumentBuilderFactory.newInstance();
        DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();

        InputSource is = new InputSource(new StringReader(a_value));

        Document doc = dBuilder.parse(is);

        if (doc == null)
            return;

        Element rootElement = doc.getDocumentElement();

        if (rootElement == null)
            return;

        if (!rootElement.getNodeName().equals("ArrayOfIsoInfo"))
            return;

        NodeList l_NodeList = rootElement.getChildNodes();

        if (l_NodeList == null)
            return;

        m_Items.clear();

        for (int i = 0; i < l_NodeList.getLength(); ++i) {
            Node l_node = l_NodeList.item(i);

            if (l_node == null)
                continue;

            if (!l_node.getNodeName().equals("IsoInfo"))
                continue;

            IsoInfo l_item = new IsoInfo();

            if (l_item.deserialize((Element) l_node))
                m_Items.add(l_item);
        }
    }

    private String serialize() throws ParserConfigurationException, TransformerException {

        DocumentBuilderFactory dbFactory =
                DocumentBuilderFactory.newInstance();
        DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
        Document doc = dBuilder.newDocument();

        Element rootElement = doc.createElement("ArrayOfIsoInfo");

        doc.appendChild(rootElement);


        for (IsoInfo l_item :
                m_Items) {

            Element l_BiosInfoNode = doc.createElement("IsoInfo");

            l_item.serialize(l_BiosInfoNode);

            rootElement.appendChild(l_BiosInfoNode);
        }

        TransformerFactory tf = TransformerFactory.newInstance();
        Transformer transformer = tf.newTransformer();
        transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "no");
        StringWriter writer = new StringWriter();
        transformer.transform(new DOMSource(doc), new StreamResult(writer));

        return writer.getBuffer().toString();//.replaceAll("\n|\r", "");
    }

    public void selectItem(int position) {
        IsoInfo l_IsoInfo = m_Items.get(position);

        if (l_IsoInfo != null) {
            for (int i = 0; i < m_Items.size(); ++i) {
                IsoInfo l_item = m_Items.get(i);

                if (l_item != null)
                    l_item.IsCurrent = false;
            }

            l_IsoInfo.IsCurrent = true;

            m_selectedIndex = position;

            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    notifyDataSetChanged();
                }
            });

            PCSX2Controller.getInstance().setIsoInfo(l_IsoInfo);

            save();
        }
    }

    public void removeItem(int a_position){

        if(m_Items.size() > a_position)
        {
            m_Items.remove(a_position);

            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    notifyDataSetChanged();
                }
            });

            save();
        }
    }

    public int selectedItem() {
        return m_selectedIndex;
    }


    @Override
    public int getCount() {
        return m_Items.size();
    }

    @Override
    public Object getItem(int position) {
        return m_Items.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {

        convertView = ((LayoutInflater) GlobalApplication.getAppContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE)).
                inflate(R.layout.list_view_game_item, null);

        IsoInfo l_IsoInfo = m_Items.get(position);

        if (l_IsoInfo != null) {
            LinearLayout l_LinearLayout = convertView.findViewById(R.id.GameItemBackPanel);

            if (l_LinearLayout != null) {
                if (l_IsoInfo.IsCurrent)
                    l_LinearLayout.setBackgroundColor(ResourcesCompat.getColor(
                            GlobalApplication.getAppContext().getResources(), R.color.colorSelected, null));
                else
                    l_LinearLayout.setBackgroundColor(ResourcesCompat.getColor(
                            GlobalApplication.getAppContext().getResources(), R.color.colorUnselected, null));
            }


            TextView l_textView = convertView.findViewById(R.id.textViewDiscRegionType);

            l_textView.setText(l_IsoInfo.DiscRegionType);


            l_textView = convertView.findViewById(R.id.textViewGameDiscType);

            l_textView.setText(l_IsoInfo.GameDiscType);


            l_textView = convertView.findViewById(R.id.textViewIsoType);

            l_textView.setText(l_IsoInfo.IsoType);


            l_textView = convertView.findViewById(R.id.textViewSoftwareVersion);

            l_textView.setText(l_IsoInfo.SoftwareVersion);


            l_textView = convertView.findViewById(R.id.textViewGameTitle);

            l_textView.setText(l_IsoInfo.Title);







        }

        return convertView;
    }
}