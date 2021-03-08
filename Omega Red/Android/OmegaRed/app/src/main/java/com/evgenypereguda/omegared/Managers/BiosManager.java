package com.evgenypereguda.omegared.Managers;

import com.evgenypereguda.omegared.Adapters.BIOSAdapter;
import com.evgenypereguda.omegared.Models.BiosInfo;
import com.evgenypereguda.omegared.PCSX2.Tools.BiosControl;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.net.URI;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Enumeration;
import java.util.zip.*;

public final class BiosManager {



    private static BiosManager m_Instance = null;

    private BiosManager(){}

    public static BiosManager getInstance()
    {
        if (m_Instance == null)
            m_Instance = new BiosManager();

        return m_Instance;
    }

    public void addBIOS(String a_file_path)
    {
        final String l_file_path = a_file_path;

        Thread thread = new Thread(
                new Runnable() {
                    @Override
                    public void run() {
                        BiosManager.getInstance().readBios(l_file_path);
                    }
                }
        );

        thread.start();
    }

    private void readBios(String a_file_path)
    {
        do
        {
            String l_result = BiosControl.getInstance().IsBIOS(a_file_path);

            if (l_result != "")
            {
                String l_file_path = a_file_path;

                String zone = new String();
                String version = new String();
                String data = new String();
                String build = new String();
                int versionInt = 0;


                String[] l_splits = l_result.split(":");

                if(l_splits != null)
                {
                    zone = l_splits[0];
                    version = l_splits[1];
                    data = l_splits[2];
                    build = l_splits[3];

                    String l_majNum = version.substring(1, 3);

                    String l_minNum = version.substring(4, 6);


                    versionInt = Integer.parseInt(l_majNum);

                    versionInt = versionInt << 8;

                    versionInt |= Integer.parseInt(l_minNum);

                }


                byte[] l_NVM = null;

                String l_add_file_path = l_file_path.replace(".bin", ".nvm");

                File file = new File(l_add_file_path);

                if (file.exists())
                {
                    FileInputStream l_FileStream = null;
                    try {
                        l_FileStream = new FileInputStream(file);
                    } catch (FileNotFoundException e) {
                        e.printStackTrace();
                    }

                    try {
                        if (l_FileStream != null && l_FileStream.available() > 0)
                        {
                            int l_length = (int)file.length();

                            l_NVM = new byte[l_length];

                            int l_offset = 0;

                            do {

                                int l_readLength = l_FileStream.read(l_NVM, l_offset, l_length);

                                l_length -= l_readLength;

                                l_offset += l_readLength;
                            }
                            while (l_length > 0);
                        }
                    } catch (IOException e) {
                        e.printStackTrace();
                    }

                    try {
                        l_FileStream.close();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }

                byte[] l_MEC = null;

                l_add_file_path = l_file_path.replace(".bin", ".mec");

                file = new File(l_add_file_path);

                if (file.exists())
                {

                    FileInputStream l_FileStream = null;
                    try {
                        l_FileStream = new FileInputStream(file);
                    } catch (FileNotFoundException e) {
                        e.printStackTrace();
                    }

                    try {
                        if (l_FileStream != null && l_FileStream.available() > 0)
                        {
                            int l_length = (int)file.length();

                            l_MEC = new byte[l_length];

                            int l_offset = 0;

                            do {

                                int l_readLength = l_FileStream.read(l_MEC, l_offset, l_length);

                                l_length -= l_readLength;

                                l_offset += l_readLength;
                            }
                            while (l_length > 0);
                        }
                    } catch (IOException e) {
                        e.printStackTrace();
                    }

                    try {
                        l_FileStream.close();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }

                BiosManager.getInstance().addBiosInfo(BiosInfo.create(
                    zone,
                    version,
                    versionInt,
                    data,
                    build,
                    BiosControl.getInstance().getBIOSChecksum(a_file_path),
                    a_file_path,
                    l_NVM,
                    l_MEC
                ));
            }
            else
            {
                try
                {
//                    ZipInputStream l_ZipInputStream = new ZipInputStream(a_BIOSStream);
//
//
//                    ZipEntry entry;
//                    while((entry = l_ZipInputStream.getNextEntry())!=null) {
//                        if (entry.isDirectory())
//                            continue;
//
//                        long l_Name1 = entry.getCompressedSize();
//
//                        String l_Name = entry.getName();
//
//                        int l_m = entry.getMethod();
//
//                        InputStream l_uncompressedInputStream = convertZipInputStreamToInputStream(l_ZipInputStream);
//
////                        if (BiosControl.IsBIOS(
////                                l_uncompressedInputStream,
////                                zone,
////                                version,
////                                versionInt,
////                                data,
////                                build)) {
////
////                        }
//
//                    }

//                    Enumeration<? extends ZipEntry> l_entries = l_ZipFile.
//
//                    while (l_entries.hasMoreElements())
//                    {
//                        ZipEntry l_entry = l_entries.nextElement();
//
//                        if(l_entry != null)
//                        {
//                            if(l_entry.isDirectory())
//                                continue;
//
//
//                        }
//                    }


//                    using (FileStream zipToOpen = new FileStream(l_file_path, FileMode.Open))
//                    {
//                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
//                        {
//
//
//                            foreach (var item in archive.Entries)
//                            {
//                                if (item != null)
//                                {
//                                    using (BinaryReader reader = new BinaryReader(item.Open()))
//                                    {
//                                        try
//                                        {
//                                            MemoryStream l_memoryStream = new MemoryStream();
//
//                                            reader.BaseStream.CopyTo(l_memoryStream);
//
//                                            l_memoryStream.Position = 0;
//
//                                            using (BinaryReader memoryReader = new BinaryReader(l_memoryStream))
//                                            {
//                                                if (BiosControl.IsBIOS(
//                                                        memoryReader,
//                                                        ref zone,
//                                                        ref version,
//                                                        ref versionInt,
//                                                        ref data,
//                                                        ref build))
//                                                {
//                                                    var lname = item.Name.Remove(item.Name.Length - 4).ToLower();
//
//                                                    var lNVMname = lname + ".nvm";
//
//                                                    var lMECname = lname + ".mec";
//
//                                                    byte[] l_NVM = null;
//
//                                                    foreach (var NVMitem in archive.Entries)
//                                                    {
//                                                        if (NVMitem.Name.ToLower() == lNVMname)
//                                                        {
//                                                            if (NVMitem != null)
//                                                            {
//                                                                using (BinaryReader readerEntry = new BinaryReader(NVMitem.Open()))
//                                                                {
//                                                                    MemoryStream l_NVMmemoryStream = new MemoryStream();
//
//                                                                    readerEntry.BaseStream.CopyTo(l_NVMmemoryStream);
//
//                                                                    l_NVMmemoryStream.Position = 0;
//
//                                                                    l_NVM = new byte[l_NVMmemoryStream.Length];
//
//                                                                    l_NVMmemoryStream.Read(l_NVM, 0, l_NVM.Length);
//                                                                }
//                                                            }
//                                                        }
//                                                    }
//
//                                                    byte[] l_MEC = null;
//
//                                                    foreach (var MECitem in archive.Entries)
//                                                    {
//                                                        if (MECitem.Name.ToLower() == lMECname)
//                                                        {
//                                                            if (MECitem != null)
//                                                            {
//                                                                using (BinaryReader readerEntry = new BinaryReader(MECitem.Open()))
//                                                                {
//
//                                                                    MemoryStream l_MECMmemoryStream = new MemoryStream();
//
//                                                                    readerEntry.BaseStream.CopyTo(l_MECMmemoryStream);
//
//                                                                    l_MECMmemoryStream.Position = 0;
//
//                                                                    l_MEC = new byte[l_MECMmemoryStream.Length];
//
//                                                                    l_MECMmemoryStream.Read(l_MEC, 0, l_MEC.Length);
//                                                                }
//                                                            }
//                                                        }
//                                                    }
//
//                                                    LockScreenManager.Instance.displayMessage(l_file_path + "|" + item.Name);
//
//                                                    byte[] l_result = new byte[l_memoryStream.Length];
//
//                                                    var l_readLength = l_memoryStream.Read(l_result, 0, l_result.Length);
//
//                                                    BiosManager.Instance.addBiosInfo(new Models.BiosInfo()
//                                                    {
//                                                        Zone = zone,
//                                                        Version = version,
//                                                        VersionInt = versionInt,
//                                                        Data = data,
//                                                        Build = build,
//                                                        CheckSum = Omega_Red.Tools.BiosControl.getBIOSChecksum(l_result),
//                                                        FilePath = l_file_path + "|" + item.Name,
//                                                        NVM = l_NVM,
//                                                        MEC = l_MEC
//                                                    });
//                                                }
//                                            }
//                                        }
//                                        catch (Exception)
//                                        {
//                                        }
//                                    }
//                                }
//                            }
//
//
//
//
//                        }
//                    }
                }
                catch (Exception exc)
                {
                    exc.printStackTrace();
                }
            }

        } while (false);
    }


    public void addBiosInfo(BiosInfo a_BiosInfo)
    {
        BIOSAdapter.getInstance().addItem(a_BiosInfo);
    }
}
