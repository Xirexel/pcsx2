package com.xirexel.omegared.Adapters;

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
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.xirexel.omegared.GlobalApplication;
import com.xirexel.omegared.Models.BiosInfo;
import com.xirexel.omegared.PCSX2.PCSX2Controller;
import com.xirexel.omegared.R;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;
import org.xml.sax.XMLReader;

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

public final class BIOSAdapter extends BaseAdapter {

    private static final String BIOS_INFO_COLLECTION = "BiosInfoCollection";

    ArrayList<BiosInfo> m_Items = new ArrayList<BiosInfo>();

    private TypedArray mIcons;

    private int m_selectedIndex = -1;

    final Handler mHandler = new Handler();

    private static BIOSAdapter m_Instance = null;

    private BIOSAdapter(){

        Resources res = GlobalApplication.getAppContext().getResources();

        mIcons = res.obtainTypedArray(R.array.country_icons);

        load();
    }

    public static BIOSAdapter getInstance()
    {
        if (m_Instance == null)
            m_Instance = new BIOSAdapter();

        return m_Instance;
    }

    public void addItem(BiosInfo a_item){

        if(!m_Items.contains(a_item))
        {
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

    public void save()
    {
        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(GlobalApplication.getAppContext());

        SharedPreferences.Editor editor = preferences.edit();

        try {

            String l_value = serialize();

            editor.putString(BIOS_INFO_COLLECTION, l_value);
        } catch (ParserConfigurationException e) {
            e.printStackTrace();
        } catch (TransformerException e) {
            e.printStackTrace();
        }

        editor.commit();
    }

    private void load()
    {
        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(GlobalApplication.getAppContext());

        try {

            String l_value = preferences.getString(BIOS_INFO_COLLECTION, "");

            deserialize(l_value);

            for (int i = 0; i < m_Items.size(); ++i)
            {
                BiosInfo l_BiosInfo = m_Items.get(i);

                if(l_BiosInfo != null && l_BiosInfo.IsCurrent)
                {
                    m_selectedIndex = i;

                    PCSX2Controller.getInstance().setBiosInfo(l_BiosInfo);

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

        if(doc == null)
            return;

        Element rootElement = doc.getDocumentElement();

        if(rootElement == null)
            return;

        if(!rootElement.getNodeName().equals("ArrayOfBiosInfo"))
            return;

        NodeList l_NodeList = rootElement.getChildNodes();

        if(l_NodeList == null)
            return;

        m_Items.clear();

        for (int i = 0; i < l_NodeList.getLength(); ++i)
        {
            Node l_node = l_NodeList.item(i);

            if(l_node == null)
                continue;

            if(!l_node.getNodeName().equals("BiosInfo"))
                continue;

            BiosInfo l_item = new BiosInfo();

            if(l_item.deserialize((Element)l_node))
                m_Items.add(l_item);
        }
    }

    private String serialize() throws ParserConfigurationException, TransformerException {

        DocumentBuilderFactory dbFactory =
                DocumentBuilderFactory.newInstance();
        DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
        Document doc = dBuilder.newDocument();

        Element rootElement = doc.createElement("ArrayOfBiosInfo");

        doc.appendChild(rootElement);


        for (BiosInfo l_item:
        m_Items) {

            Element l_BiosInfoNode = doc.createElement("BiosInfo");

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

    public void selectItem(int position)
    {
        BiosInfo l_BiosInfo = m_Items.get(position);

        if(l_BiosInfo != null)
        {
            for (int i = 0; i < m_Items.size(); ++i)
            {
                BiosInfo l_item = m_Items.get(i);

                if(l_item != null)
                    l_item.IsCurrent = false;
            }

            l_BiosInfo.IsCurrent = true;

            m_selectedIndex = position;

            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    notifyDataSetChanged();
                }
            });

            PCSX2Controller.getInstance().setBiosInfo(l_BiosInfo);

            save();
        }
    }

    public int selectedItem(){return m_selectedIndex;}


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

        convertView = ((LayoutInflater)GlobalApplication.getAppContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE)).
                inflate(R.layout.list_view_bios_item, null);

        BiosInfo l_BiosInfo = m_Items.get(position);

        if(l_BiosInfo != null)
        {
            LinearLayout l_LinearLayout = convertView.findViewById(R.id.BIOSItembackPanel);

            if(l_LinearLayout != null)
            {
                if(l_BiosInfo.IsCurrent)
                    l_LinearLayout.setBackgroundColor(ResourcesCompat.getColor(
                            GlobalApplication.getAppContext().getResources(), R.color.colorSelected, null));
                else
                    l_LinearLayout.setBackgroundColor(ResourcesCompat.getColor(
                            GlobalApplication.getAppContext().getResources(), R.color.colorUnselected, null));
            }




            TextView l_textViewZone = convertView.findViewById(R.id.textViewZone);

            l_textViewZone.setText(l_BiosInfo.Zone);



            TextView l_textViewVersion = convertView.findViewById(R.id.textViewVersion);

            l_textViewVersion.setText(l_BiosInfo.Version);



            TextView l_textViewDate = convertView.findViewById(R.id.textViewDate);

            l_textViewDate.setText(l_BiosInfo.Data);


            ImageView l_BIOS_Image_View = (ImageView)convertView.findViewById(R.id.BIOS_Image_View);

            if(l_BIOS_Image_View != null)
            {
                for(int i = 0; i < mIcons.length(); ++i)
                {
                    String l_name = mIcons.getString(i);

                    if(l_name != null && l_name.contains(l_BiosInfo.Zone.toLowerCase()))
                    {
                        l_BIOS_Image_View.setImageDrawable(mIcons.getDrawable(i));

                        break;
                    }
                }
            }



//            Button l_removeBISOBtn = convertView.findViewById(R.id.removeBISOBtn);
//
//            final int l_position = position;
//
//            if(l_removeBISOBtn != null)
//            {
//                l_removeBISOBtn.setOnClickListener(
//                        new View.OnClickListener(){
//                            @Override
//                            public void onClick(View v) {
//
//                                BIOSAdapter.getInstance().removeItem(l_position);
//                            }
//                        }
//
//                );
//            }
        }

        return convertView;
    }
}
