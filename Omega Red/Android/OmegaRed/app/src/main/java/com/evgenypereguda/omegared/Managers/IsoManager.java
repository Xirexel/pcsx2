package com.evgenypereguda.omegared.Managers;

import com.evgenypereguda.omegared.Adapters.ISOAdapter;
import com.evgenypereguda.omegared.Models.IsoInfo;
import com.evgenypereguda.omegared.PCSX2.ModuleControl;
import com.evgenypereguda.omegared.PCSX2.Modules.Module;
import com.evgenypereguda.omegared.PCSX2.Modules.ModuleManager;

import org.xml.sax.SAXException;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;

import javax.xml.parsers.ParserConfigurationException;

public class IsoManager {

    private static IsoManager m_Instance = null;

    private IsoManager(){}

    public static IsoManager getInstance()
    {
        if (m_Instance == null)
            m_Instance = new IsoManager();

        return m_Instance;
    }

    public void addGame(String a_file_path)
    {
        final String l_file_path = a_file_path;

        Thread thread = new Thread(
                new Runnable() {
                    @Override
                    public void run() {
                        IsoManager.getInstance().readGame(l_file_path);
                    }
                }
        );

        thread.start();
    }

    private void readGame(String a_file_path)
    {
        do
        {
            IsoInfo l_IsoInfo = null;
            try {
                l_IsoInfo = ModuleControl.getGameDiscInfo(a_file_path);
            } catch (Exception e) {
                e.printStackTrace();
            }

            if(l_IsoInfo != null)
            {
                IsoManager.getInstance().addIsoInfo(l_IsoInfo);
            }



//                IsoManager.getInstance().addIsoInfo(IsoInfo.create(
//                        DiscRegionType,
//                        DiscSerial,
//                        ElfCRC,
//                        GameDiscType,
//                        IsoType,
//                        SoftwareVersion,
//                        Title,
//                        l_file_path
//                ));


        } while (false);
    }

    public void addIsoInfo(IsoInfo a_BIsoInfo)
    {
        ISOAdapter.getInstance().addItem(a_BIsoInfo);
    }
}
