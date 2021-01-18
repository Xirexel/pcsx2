using Omega_Red.Emulators;
using Omega_Red.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Omega_Red.Tools
{
    class TexturePackControl
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate Int32 CallbackDelegate(Int32 a_Index, Int32 a_Arg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void SetTextureDelegate(IntPtr a_PtrMemory, [MarshalAs(UnmanagedType.LPStr)]String a_IDs);

        private int m_ResolutionX = 0;
        private int m_ResolutionY = 0;


        private CallbackDelegate m_Callback = null;

        private IntPtr m_TexturePackCallbackHandler = IntPtr.Zero;

        public IntPtr TexturePackCallbackHandler { get { return m_TexturePackCallbackHandler; } }

        private Dictionary<string, string> m_TexturePack = new Dictionary<string, string>();

        private string m_disk_serial = "";

        private string m_file_path = "";

        private static TexturePackControl m_Instance = null;

        public static TexturePackControl Instance { get { if (m_Instance == null) m_Instance = new TexturePackControl(); return m_Instance; } }


        private TexturePackControl()
        {
            Emul.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;

            m_Callback = (a_Index, a_Arg) =>
            {
                if (a_Index == 1)
                    return m_ResolutionX;
                else if (a_Index == 2)
                    return m_ResolutionY;
                else if (a_Index == 3)
                {
                    var l_SetTextureDelegate = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<SetTextureDelegate>(new IntPtr(a_Arg));

                    if(l_SetTextureDelegate != null)
                    {
                        setTextures(l_SetTextureDelegate);
                    }
                }

                return 0;
            };

            m_TexturePackCallbackHandler = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(m_Callback);
        }

        public void init()
        {
            if (string.IsNullOrEmpty(Settings.Default.TexturePacksFolder))
                Settings.Default.TexturePacksFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + App.m_MainFolderName + @"\TexturePacks";

            if (!System.IO.Directory.Exists(Settings.Default.TexturePacksFolder))
            {
                System.IO.Directory.CreateDirectory(Settings.Default.TexturePacksFolder);
            }

            if (!System.IO.Directory.Exists(Settings.Default.TexturePacksFolder + @"\Dump"))
            {
                System.IO.Directory.CreateDirectory(Settings.Default.TexturePacksFolder + @"\Dump");
            }
        }

        void Instance_m_ChangeStatusEvent(Emul.StatusEnum a_Status)
        {
            //if (a_Status != Emul.StatusEnum.NoneInitilized && Emul.Instance.IsoInfo != null)
            //    load(Emul.Instance.IsoInfo.DiscSerial);
        }

        private void load(string a_disk_serial)
        {
            if (m_disk_serial == a_disk_serial)
                return;

            do
            {

                string[] l_files = System.IO.Directory.GetFiles(Settings.Default.TexturePacksFolder, a_disk_serial + ".*");

                if(l_files == null || l_files.Length == 0)
                    break;

                if (!checkTexturePackValidation(l_files[0]))
                    break;

                m_disk_serial = a_disk_serial;

                m_file_path = l_files[0];

            } while (false);
        }

        private bool checkTexturePackValidation(string a_file_path)
        {
            bool l_result = false;

            do
            {
                m_TexturePack = new Dictionary<string, string>();

                using (FileStream zipToOpen = new FileStream(a_file_path, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                    {
                        ZipArchiveEntry l_ConfigEntry = archive.GetEntry("Config.xml");

                        if (l_ConfigEntry != null)
                        {
                            using (BinaryReader reader = new BinaryReader(l_ConfigEntry.Open()))
                            {
                                MemoryStream l_memoryStream = new MemoryStream();

                                reader.BaseStream.CopyTo(l_memoryStream);

                                l_memoryStream.Position = 0;

                                XmlDocument l_doc = new XmlDocument();

                                l_doc.Load(l_memoryStream);

                                if (l_doc.DocumentElement != null)
                                {
                                    var l_node = l_doc.DocumentElement.SelectSingleNode("/TexturePack");

                                    if (l_node != null)
                                    {
                                        var l_attr = l_node.Attributes["ResolutionX"];

                                        if (l_attr != null)
                                        {
                                            int l_temp = 0;

                                            if (int.TryParse(l_attr.Value, out l_temp))
                                                m_ResolutionX = l_temp;
                                        }

                                        l_attr = l_node.Attributes["ResolutionY"];

                                        if (l_attr != null)
                                        {
                                            int l_temp = 0;

                                            if (int.TryParse(l_attr.Value, out l_temp))
                                                m_ResolutionY = l_temp;
                                        }
                                    }

                                    var l_nodes = l_doc.DocumentElement.SelectNodes("/TexturePack/Texture");

                                    if(l_nodes != null)
                                    {
                                        foreach (var item in l_nodes)
                                        {
                                            var l_textureNode = item as XmlNode;

                                            string l_name = "";

                                            if (l_textureNode != null)
                                            {
                                                var l_attr = l_textureNode.Attributes["Name"];

                                                if (l_attr == null)
                                                    continue;

                                                l_name = l_attr.Value;

                                                l_attr = l_textureNode.Attributes["Ids"];

                                                if (l_attr == null || string.IsNullOrWhiteSpace(l_attr.Value))
                                                    continue;

                                                m_TexturePack.Add(l_name, l_attr.Value);
                                            }
                                        }
                                    }
                                }
                                                               
                                l_result = true;
                            }
                        }
                    }
                }

            } while (false);

            return l_result;
        }
        
        private void setTextures(SetTextureDelegate a_SetTextureDelegate)
        {
            do
            {
                if (string.IsNullOrWhiteSpace(m_file_path))
                    break;

                if (a_SetTextureDelegate == null)
                    break;

                using (FileStream zipToOpen = new FileStream(m_file_path, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                    {
                        foreach (var item in m_TexturePack)
                        {
                            ZipArchiveEntry l_ConfigEntry = archive.GetEntry(item.Key);

                            if (l_ConfigEntry != null)
                            {
                                using (BinaryReader reader = new BinaryReader(l_ConfigEntry.Open()))
                                {
                                    MemoryStream l_memoryStream = new MemoryStream();

                                    reader.BaseStream.CopyTo(l_memoryStream);

                                    l_memoryStream.Position = 0;

                                    var l_PtrMemory = Marshal.AllocHGlobal((int)l_memoryStream.Length);

                                    if(l_PtrMemory != IntPtr.Zero)
                                    {
                                        Marshal.Copy(l_memoryStream.GetBuffer(), 0, l_PtrMemory,(int)l_memoryStream.Length);

                                        a_SetTextureDelegate(l_PtrMemory, item.Value);

                                        Marshal.FreeHGlobal(l_PtrMemory);
                                    }
                                }
                            }
                        }

                    }
                }

            } while (false);
        }
    }
}
