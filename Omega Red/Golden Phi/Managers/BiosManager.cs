using Golden_Phi.Emulators;
using Golden_Phi.Models;
using Golden_Phi.Properties;
using Golden_Phi.Tools;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Golden_Phi.Managers
{
    class BiosManager : IManager
    {

        public const int m_nvmSize = 1024;

        class Compare : IEqualityComparer<BiosInfo>
        {
            public bool Equals(BiosInfo x, BiosInfo y)
            {
                if (x.CheckSum == y.CheckSum)
                {
                    return true;
                }
                else { return false; }
            }
            public int GetHashCode(BiosInfo codeh)
            {
                return 0;
            }

        }

        public event Action<IsoInfo> IsoInfoUpdated;

        public event Action<bool> ShowChooseBiosEvent;

        public event Action<object> ShowViewEvent;

        private IsoInfo m_IsoInfo = null;

        private ICollectionView mCustomerView = null;

        private readonly ObservableCollection<BiosInfo> _biosInfoCollection = new ObservableCollection<BiosInfo>();

        private static BiosManager m_Instance = null;

        public static BiosManager Instance { get { if (m_Instance == null) m_Instance = new BiosManager(); return m_Instance; } }

        private BiosManager()
        {
            View = App.getResource("BiosView");

            mCustomerView = CollectionViewSource.GetDefaultView(_biosInfoCollection);

            mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;
        }

        private void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            var l_BiosInfo = mCustomerView.CurrentItem as BiosInfo;

            if (l_BiosInfo != null && m_IsoInfo != null)
                selectBiosInfo(l_BiosInfo);
        }

        public void load()
        {
            loadInner();
        }

        private void loadInner()
        {
            if (string.IsNullOrEmpty(Settings.Default.BiosInfoCollection))
                return;
            
            _biosInfoCollection.Clear();

            XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<BiosInfo>));

            XmlReader xmlReader = XmlReader.Create(new StringReader(Settings.Default.BiosInfoCollection));

            if (ser.CanDeserialize(xmlReader))
            {
                var l_collection = ser.Deserialize(xmlReader) as ObservableCollection<BiosInfo>;

                if (l_collection != null)
                {
                    foreach (var item in l_collection)
                    {
                        addBiosInfo(item);
                    }
                }
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                foreach (var item in _biosInfoCollection)
                {
                    if (item.IsCurrent)
                    {
                        mCustomerView.MoveCurrentTo(item);

                        break;
                    }
                }
            });
        }

        public void addBiosInfo(BiosInfo a_BiosInfo)
        {
            if (string.IsNullOrWhiteSpace(a_BiosInfo.FilePath))
                return;

            if (!_biosInfoCollection.Contains(a_BiosInfo, new Compare()))
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                {
                    a_BiosInfo.ContainerFile = new ContainerFile();

                    var l_splits = a_BiosInfo.FilePath.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    if (l_splits == null || !File.Exists(l_splits[0]))
                        return;

                    if (l_splits != null && l_splits.Length == 2)
                    {
                        l_splits = l_splits[0].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                        if (l_splits != null && l_splits.Length >= 1)
                            a_BiosInfo.ContainerFile = new ContainerFile(l_splits[l_splits.Length - 1]);
                    }

                    _biosInfoCollection.Add(a_BiosInfo);

                });
            }
        }

        private void selectBiosInfo(BiosInfo a_BiosInfo)
        {
            if (_biosInfoCollection.Contains(a_BiosInfo, new Compare()))
            {
                foreach (var item in _biosInfoCollection)
                {
                    item.IsCurrent = false;
                }

                a_BiosInfo.IsCurrent = true;

                if(m_IsoInfo != null)
                {
                    var l_CurrentBiosZone = a_BiosInfo.Zone + "-" + a_BiosInfo.Version;

                    m_IsoInfo.BIOS = l_CurrentBiosZone;

                    m_IsoInfo.BIOSFile = a_BiosInfo.FilePath;

                    m_IsoInfo.BiosInfo = a_BiosInfo;

                    if (IsoInfoUpdated != null)
                        IsoInfoUpdated(m_IsoInfo);

                    IsoManager.Instance.updateImage(m_IsoInfo);

                    IsoManager.Instance.save();

                    CloseChooseBios();
                }
            }
        }

        public void setIsoInfoFilter(IsoInfo a_IsoInfo)
        {
            if (a_IsoInfo == null)
                return;

            m_IsoInfo = null;

            Collection.Filter = new Predicate<object>(x => ((BiosInfo)x).GameType == a_IsoInfo.GameType);

            Collection.MoveCurrentToPosition(-1);

            var l_BiosInfo = _biosInfoCollection.FirstOrDefault((a_BiosInfo) => a_BiosInfo.FilePath == a_IsoInfo.BIOSFile);

            if(l_BiosInfo != null)
                Collection.MoveCurrentTo(l_BiosInfo);

            m_IsoInfo = a_IsoInfo;
        }

        public bool checkBios(IsoInfo a_IsoInfo, bool a_show_menu)
        {
            bool l_result = false;

            do
            {
                if (a_IsoInfo == null)
                    break;

                if(a_IsoInfo.GameType == GameType.PSP)
                {
                    l_result = true;

                    break;
                }

                var l_BiosInfo = _biosInfoCollection.FirstOrDefault((a_BiosInfo) => a_BiosInfo.FilePath == a_IsoInfo.BIOSFile);

                if (l_BiosInfo == null && a_show_menu)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        if (ShowViewEvent != null)
                            ShowViewEvent(new ViewModels.BiosInfoViewModel());

                        setIsoInfoFilter(a_IsoInfo);
                    });
                }
                else
                {
                    a_IsoInfo.BiosInfo = l_BiosInfo;

                    l_result = true;
                }

            } while (false);

            return l_result;
        }

        public void setBios(IsoInfo a_IsoInfo)
        {
            do
            {
                if (a_IsoInfo == null)
                    break;

                if (a_IsoInfo.GameType == GameType.PSP)
                {
                    break;
                }

                var l_BiosInfo = _biosInfoCollection.FirstOrDefault((a_BiosInfo) => a_BiosInfo.FilePath == a_IsoInfo.BIOSFile);

                if (l_BiosInfo != null)
                {
                    a_IsoInfo.BiosInfo = l_BiosInfo;
                }

            } while (false);
        }


        public void save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<BiosInfo>));

            MemoryStream stream = new MemoryStream();

            ser.Serialize(stream, _biosInfoCollection);

            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);

            Settings.Default.BiosInfoCollection = reader.ReadToEnd();

            Settings.Default.Save();

            App.saveCopy();
        }

        public void createItem()
        {
            addBiosInfo();
        }

        private void addBiosInfo()
        {
            var lOpenFileDialog = new Microsoft.Win32.OpenFileDialog();

            lOpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            lOpenFileDialog.Filter = "Bios files (*.zip, *.7z, *.bin)|*.zip;*.7z;*.bin|All files (*.*)|*.*";

            bool l_result = (bool)lOpenFileDialog.ShowDialog();

            if (l_result &&
                File.Exists(lOpenFileDialog.FileName))
            {
                executeLoading(lOpenFileDialog.FileName);
            }
        }

        private void executeLoading(string a_file_path)
        {
            ParameterizedThreadStart loadModulesStart = new ParameterizedThreadStart(readBios);

            Thread loadModulesThread = new Thread(loadModulesStart);

            LockScreenManager.Instance.show();

            loadModulesThread.Start(a_file_path);
        }

        public void readBios(object a_file_path)
        {
            do
            {
                string zone = "";
                string version = "";
                string data = "";
                string build = "";
                int versionInt = 0;
                GameType gameType = GameType.Unknown;

                string l_file_path = (string)a_file_path;

                try
                {
                    using (ArchiveFile archive = new ArchiveFile(l_file_path))
                    {
                        var l_EntriesCount = archive.Entries.Count;

                        for (int l_EntryIndex = 0; l_EntryIndex < l_EntriesCount; l_EntryIndex++)
                        {
                            var item = archive.Entries[l_EntryIndex];

                            if (item != null && !item.IsFolder && !item.IsEncrypted)
                            {
                                using (MemoryStream l_memoryStream = new MemoryStream())
                                {
                                    try
                                    {
                                        item.Extract(l_memoryStream);

                                        LockScreenManager.Instance.displayMessage(l_file_path + "|" + string.Format(" {0,5:0.00} %", (float)l_EntryIndex * 100.0 / (float)l_EntriesCount));

                                        l_memoryStream.Position = 0;

                                        using (BinaryReader memoryReader = new BinaryReader(l_memoryStream))
                                        {
                                            if (BiosControl.IsBIOS(
                                                                    memoryReader,
                                                                    ref zone,
                                                                    ref version,
                                                                    ref versionInt,
                                                                    ref data,
                                                                    ref build,
                                                                    ref gameType))
                                            {
                                                var lname = item.FileName.Remove(item.FileName.Length - 4).ToLower();

                                                var lNVMname = lname + ".nvm";

                                                var lMECname = lname + ".mec";

                                                byte[] l_NVM = null;

                                                foreach (var NVMitem in archive.Entries)
                                                {
                                                    if (NVMitem.FileName.ToLower() == lNVMname)
                                                    {
                                                        if (NVMitem != null)
                                                        {
                                                            using (MemoryStream l_NVMmemoryStream = new MemoryStream())
                                                            {
                                                                NVMitem.Extract(l_NVMmemoryStream);

                                                                l_NVMmemoryStream.Position = 0;

                                                                l_NVM = new byte[l_NVMmemoryStream.Length];

                                                                l_NVMmemoryStream.Read(l_NVM, 0, l_NVM.Length);
                                                            }
                                                        }
                                                    }
                                                }

                                                byte[] l_MEC = null;

                                                foreach (var MECitem in archive.Entries)
                                                {
                                                    if (MECitem.FileName.ToLower() == lMECname)
                                                    {
                                                        if (MECitem != null)
                                                        {
                                                            using (MemoryStream l_MECMmemoryStream = new MemoryStream())
                                                            {
                                                                MECitem.Extract(l_MECMmemoryStream);

                                                                l_MECMmemoryStream.Position = 0;

                                                                l_MEC = new byte[l_MECMmemoryStream.Length];

                                                                l_MECMmemoryStream.Read(l_MEC, 0, l_MEC.Length);
                                                            }
                                                        }
                                                    }
                                                }

                                                byte[] l_result = new byte[l_memoryStream.Length];

                                                var l_readLength = l_memoryStream.Read(l_result, 0, l_result.Length);

                                                BiosManager.Instance.addBiosInfo(new Models.BiosInfo()
                                                {
                                                    Zone = zone,
                                                    Version = version,
                                                    VersionInt = versionInt,
                                                    Data = data,
                                                    Build = build,
                                                    CheckSum = Golden_Phi.Tools.BiosControl.getBIOSChecksum(l_result),
                                                    FilePath = l_file_path + "|" + item.FileName,
                                                    NVM = l_NVM,
                                                    MEC = l_MEC,
                                                    GameType = gameType
                                                });
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (exc.Message != null && exc.Message.Contains("is not a known archive type"))
                    {
                        try
                        {
                            if (BiosControl.IsBIOS(
                                                    l_file_path,
                                                    ref zone,
                                                    ref version,
                                                    ref versionInt,
                                                    ref data,
                                                    ref build,
                                                    ref gameType))
                            {

                                byte[] l_NVM = null;

                                var l_add_file_path = l_file_path.Replace(".bin", ".nvm");

                                if (File.Exists(l_add_file_path))
                                {
                                    using (var l_FileStream = File.Open(l_add_file_path, FileMode.Open))
                                    {
                                        if (l_FileStream == null)
                                            return;

                                        l_NVM = new byte[l_FileStream.Length];

                                        l_FileStream.Read(l_NVM, 0, l_NVM.Length);
                                    }
                                }

                                byte[] l_MEC = null;

                                l_add_file_path = l_file_path.Replace(".bin", ".mec");

                                if (File.Exists(l_add_file_path))
                                {
                                    using (var l_FileStream = File.Open(l_add_file_path, FileMode.Open))
                                    {
                                        if (l_FileStream == null)
                                            return;

                                        l_MEC = new byte[l_FileStream.Length];

                                        l_FileStream.Read(l_MEC, 0, l_MEC.Length);
                                    }
                                }

                                //LockScreenManager.Instance.displayMessage(l_add_file_path);

                                BiosManager.Instance.addBiosInfo(new Models.BiosInfo()
                                {
                                    Zone = zone,
                                    Version = version,
                                    VersionInt = versionInt,
                                    Data = data,
                                    Build = build,
                                    CheckSum = Golden_Phi.Tools.BiosControl.getBIOSChecksum(l_file_path),
                                    FilePath = l_file_path,
                                    NVM = l_NVM,
                                    MEC = l_MEC,
                                    GameType = gameType
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            //showErrorEvent(e.Message);
                        }
                    }
                    //else
                    //    showErrorEvent(exc.Message);
                }

            } while (false);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                LockScreenManager.Instance.hide();

                save();
            });
        }

        public ICollectionView Collection => mCustomerView;
        
        public object View { get; private set; }

        public bool IsConfirmed => false;

        public void CloseChooseBios()
        {
            if (ShowChooseBiosEvent != null)
                ShowChooseBiosEvent(false);
        }

        public void removeItem(object a_Item)
        {
        }
    }
}
