package com.evgenypereguda.omegared.PCSX2.Tools;

import com.evgenypereguda.omegared.Adapters.BIOSAdapter;
import com.evgenypereguda.omegared.PCSX2.PCSX2Controller;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;

public final class BiosControl {


    private class NVMLayout
    {
        public NVMLayout(){}

        public NVMLayout(final int[] a_data)
        {
            biosVer = a_data[0];
                    config0 = a_data[1];
                    config1 = a_data[2];
                    config2 = a_data[3];
                    consoleId = a_data[4];
                    ilinkId = a_data[5];
                    modelNum = a_data[6];
                    regparams = a_data[7];
                    mac = a_data[8];
        }

        public int biosVer;	// bios version that this eeprom layout is for
        public int config0;	// offset of 1st config block
        public int config1;	// offset of 2nd config block
        public int config2;	// offset of 3rd config block
        public int consoleId;	// offset of console id (?)
        public int ilinkId;	// offset of ilink id (ilink mac address)
        public int modelNum;	// offset of ps2 model number (eg "SCPH-70002")
        public int regparams;	// offset of RegionParams for PStwo
        public int mac;		// offset of the value written to 0xFFFE0188 and 0xFFFE018C on PStwo

    };


    private NVMLayout getInstance(final int[] a_data)
    {
        if (a_data == null || a_data.length != 9)
            return new NVMLayout();
        else
            return new NVMLayout(a_data);
    }

    static NVMLayout[] nvmlayouts = new NVMLayout[2];

    private final int m_nvmSize = 1024;

    private static BiosControl m_Instance = null;

    private BiosControl(){

        nvmlayouts[0] = new NVMLayout(
                new int[]{0x000,  0x280, 0x300, 0x200, 0x1C8, 0x1C0, 0x1A0, 0x180, 0x198}); // eeproms from bios v0.00 and up

        nvmlayouts[1] = new NVMLayout(
                new int[]{0x146,  0x270, 0x2B0, 0x200, 0x1C8, 0x1E0, 0x1B0, 0x180, 0x198}); // eeproms from bios v1.70 and up

    }

    public static BiosControl getInstance()
    {
        if (m_Instance == null)
            m_Instance = new BiosControl();

        return m_Instance;
    }

    static {
        System.loadLibrary("BiosControl");
    }

    public native String IsBIOS(
            String a_filename
    );

    public native long getBIOSChecksum(
            String a_filename
    );

    private native void LoadBIOS(
            String a_filename, long arg1, int arg2
    );

    public void LoadBIOS(long arg1, int arg2)
    {
        //u8 ROM[Ps2MemSize::Rom];

        try
        {
            if (PCSX2Controller.getInstance().getBiosInfo() == null)
                return;

            String l_filePath = PCSX2Controller.getInstance().getBiosInfo().FilePath;

            LoadBIOS(l_filePath, arg1, arg2);

        }
        catch (Exception exc)
        {
            //// Rethrow as a Bios Load Failure, so that the user interface handling the exceptions
            //// can respond to it appropriately.
            //throw Exception::BiosLoadFailed( ex.StreamName )
            //    .SetDiagMsg( ex.DiagMsg() )
            //    .SetUserMsg( ex.UserMsg() );
        }
    }

    private static NVMLayout getNvmLayout()
    {
        NVMLayout nvmLayout = null;

        int BiosVersion = 0;

        if (PCSX2Controller.getInstance().getBiosInfo() != null)
            BiosVersion = PCSX2Controller.getInstance().getBiosInfo().VersionInt;

        if (nvmlayouts[1].biosVer <= BiosVersion)
            nvmLayout = nvmlayouts[1];
        else
            nvmLayout = nvmlayouts[0];

        return nvmLayout;
    }

    public void NVMFile(ByteBuffer arg1, int offset, boolean read)
    {
        if (PCSX2Controller.getInstance().getBiosInfo() == null)
            return;

        if(PCSX2Controller.getInstance().getBiosInfo().NVM == null ||
                PCSX2Controller.getInstance().getBiosInfo().NVM.length < m_nvmSize)
        {
            PCSX2Controller.getInstance().getBiosInfo().NVM = new byte[m_nvmSize];

            NVMLayout nvmLayout = getNvmLayout();

            byte[] ILinkID_Data = { 0x00, (byte)0xAC, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xB9, (byte)0x86 };

            for(int i = 0; i < ILinkID_Data.length; ++i)
            {
                PCSX2Controller.getInstance().getBiosInfo().NVM[i + nvmLayout.ilinkId] = ILinkID_Data[i];
            }

            BIOSAdapter.getInstance().save();


            //l_FileStream.Seek(*(int*)(((u8*)nvmLayout) + offsetof(NVMLayout, ilinkId)), SeekOrigin.Begin);


            //NVMLayout nvmLayout = getNvmLayout();

            //byte[] ILinkID_Data = { 0x00, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0xB9, 0x86 };

            //int lposition = nvmLayout.ilinkId;

            //foreach (var item in ILinkID_Data)
            //{
            //    PCSX2Controller.Instance.BiosInfo.NVM[lposition++] = item;
            //}


            //l_FileStream.Seek(*(int*)(((u8*)nvmLayout) + offsetof(NVMLayout, ilinkId)), SeekOrigin.Begin);

        }


        //var l_filePath = PCSX2Controller.Instance.BiosInfo.FilePath;

        //if (!File.Exists(l_filePath))
        //    return;

        //string l_NVMFilePath = Path.ChangeExtension(l_filePath, ".nvm");

        //var filesize = new System.IO.FileInfo(l_NVMFilePath).Length;

        //if(filesize < 1024)
        //{
        //    using(var l_FileStream = File.Open(l_NVMFilePath, FileMode.OpenOrCreate))
        //    {
        //        if (l_FileStream == null)
        //            return;

        //        byte[] zero = new byte[1024];

        //        l_FileStream.Write(zero, 0, zero.Length);

        //        NVMLayout nvmLayout = getNvmLayout();

        //        byte[] ILinkID_Data = { 0x00, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0xB9, 0x86 };

        //        //l_FileStream.Seek(*(int*)(((u8*)nvmLayout) + offsetof(NVMLayout, ilinkId)), SeekOrigin.Begin);

        //        l_FileStream.Seek(nvmLayout.ilinkId, SeekOrigin.Begin);

        //        l_FileStream.Write(ILinkID_Data, 0, ILinkID_Data.Length);

        //        l_FileStream.Flush();

        //        l_FileStream.Close();
        //    }
        //}

        //var l_NVMFileStream = File.Open(l_NVMFilePath, FileMode.Open);


        if (read)
        {
            byte[] l_buffer = new byte[arg1.capacity()];

            arg1.clear();

            for(int i = 0; i < l_buffer.length; ++i)
            {
                l_buffer[i] = PCSX2Controller.getInstance().getBiosInfo().NVM[i + offset];
            }

            arg1.put(l_buffer);

            arg1.flip();
        }
        else
        {
            byte[] l_newData = arg1.array();

            byte[] l_NVM = PCSX2Controller.getInstance().getBiosInfo().NVM;

            for(int i = 0; i < l_newData.length; ++i)
            {
                l_NVM[i + offset] = l_newData[i];
            }

            BIOSAdapter.getInstance().save();
        }




//        using (var l_NVMFileStream = new MemoryStream(PCSX2Controller.Instance.BiosInfo.NVM))
//        {
//            if (l_NVMFileStream == null)
//                return;
//
//            l_NVMFileStream.Seek(offset, SeekOrigin.Begin);
//
//            byte[] l_buffer = new byte[bytes];
//
//            int ret;
//
//            if (read)
//            {
//                ret = l_NVMFileStream.Read(l_buffer, 0, bytes);
//
//                Marshal.Copy(l_buffer, 0, buffer, l_buffer.Length);
//            }
//            else
//            {
//                Marshal.Copy(buffer, l_buffer, 0, l_buffer.Length);
//
//                l_NVMFileStream.Write(l_buffer, 0, bytes);
//
//                BiosManager.Instance.save();
//            }
//        }


        //if (ret != bytes)
        //    Console.Error(L"Failed to %s %s. Did only %zu/%zu bytes",
        //    read ? L"read from" : L"write to", WX_STR(fname), ret, bytes);

    }

    public void CDVDGetMechaVer(ByteBuffer arg1)
    {
        if (PCSX2Controller.getInstance().getBiosInfo() == null)
            return;

        if (PCSX2Controller.getInstance().getBiosInfo().MEC == null ||
                PCSX2Controller.getInstance().getBiosInfo().MEC.length < 4)
        {
            PCSX2Controller.getInstance().getBiosInfo().MEC = new byte[4];

            byte[] version = { 0x3, 0x6, 0x2, 0x0 };

            for (int i = 0; i < 4; ++ i)
            {
                PCSX2Controller.getInstance().getBiosInfo().MEC[i] = version[i];
            }

            BIOSAdapter.getInstance().save();
        }

        arg1.clear();

        arg1.put(PCSX2Controller.getInstance().getBiosInfo().MEC);

        arg1.flip();
    }
}
