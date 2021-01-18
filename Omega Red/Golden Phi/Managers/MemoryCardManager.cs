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
using System.Windows.Data;
using System.Windows.Threading;
using Golden_Phi.Emulators;
using Golden_Phi.Models;
using Golden_Phi.Properties;

namespace Golden_Phi.Managers
{
    class MemoryCardManager : IManager
    {
        public bool IsConfirmed => false;

        class Compare : IEqualityComparer<MemoryCardInfo>
        {
            public bool Equals(MemoryCardInfo x, MemoryCardInfo y)
            {
                if (x.FileName == y.FileName)
                {
                    return true;
                }
                else { return false; }
            }
            public int GetHashCode(MemoryCardInfo codeh)
            {
                return 0;
            }


        }

        private const int MCDBLOCK_SIZE = 1024;

        private const int MCD_SIZE = 1024 * 8 * 16;		// Legacy PSX card default size

        private const int MC2_MBSIZE = 1024 * 528 * 2;		// Size of a single megabyte of card data

        private const int MC2_SIZE = MC2_MBSIZE * 8;		// PS2 card default size (8MB)

        private byte[] m_effeffs = new byte[528 * 16];
        
        private bool m_GoogleDriveAccess = false;

        private string m_disk_serial = "";

        private ICollectionView mCustomerView = null;

        private static MemoryCardManager m_Instance = null;

        public static MemoryCardManager Instance { get { if (m_Instance == null) m_Instance = new MemoryCardManager(); return m_Instance; } }

        private MemoryCardManager()
        {

            for (int i = 0; i < m_effeffs.Length; i++)
            {
                m_effeffs[i] = 0xff;
            }

            if (string.IsNullOrEmpty(Settings.Default.MemoryCardsFolder))
                Settings.Default.MemoryCardsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + App.c_MainFolderName + @"\memcards\";

            if (!System.IO.Directory.Exists(Settings.Default.MemoryCardsFolder))
            {
                System.IO.Directory.CreateDirectory(Settings.Default.MemoryCardsFolder);
            }

            mCustomerView = CollectionViewSource.GetDefaultView(_memoryCardInfoCollection);

            PropertyGroupDescription l_groupDescription = new PropertyGroupDescription("GameDiscType");

            mCustomerView.GroupDescriptions.Add(l_groupDescription);

            mCustomerView.SortDescriptions.Add(
            new SortDescription("DateTime", ListSortDirection.Descending));
            
            Emul.Instance.ChangeStatusEvent += Instance_ChangeStatusEvent;


            //GoogleAccountManager.Instance.mEnableStateEvent += (obj) =>
            //{
            //    m_GoogleDriveAccess = obj;

            //    m_disk_serial = "";

            //    if (Emul.Instance.IsoInfo != null)
            //        load();
            //};
        }

        void Instance_ChangeStatusEvent(Emul.StatusEnum a_Status)
        {
            if (a_Status != Emul.StatusEnum.Stopped)
                load();
        }
        
        private readonly ObservableCollection<MemoryCardInfo> _memoryCardInfoCollection = new ObservableCollection<MemoryCardInfo>();

        public void load()
        {
            if (string.IsNullOrWhiteSpace(Emul.Instance.DiscSerial))
                return;

            string l_ext = getCurrentMemoryCardExtention();

            if (string.IsNullOrWhiteSpace(l_ext))
                return;

            if (m_disk_serial == Emul.Instance.DiscSerial)
                return;

            m_disk_serial = Emul.Instance.DiscSerial;

            _memoryCardInfoCollection.Clear();

            if (Emul.Instance.GameType == GameType.PS1 ||
                Emul.Instance.GameType == GameType.PS2)
            {
                try
                {
                    var Name = "MemoryCard.shared" + l_ext;

                    var FullName = Settings.Default.MemoryCardsFolder + Name;

                    DateTime l_LastWriteTime = DateTime.Now;

                    if (!File.Exists(FullName))
                    {
                        if(Emul.Instance.GameType == GameType.PS1)
                            createPS1Mcd(FullName);
                        else if (Emul.Instance.GameType == GameType.PS2)
                            createPS2Mcd(FullName);
                    }

                    if (File.Exists(FullName))
                    {
                        System.IO.FileInfo fi = null;
                        try
                        {
                            fi = new System.IO.FileInfo(FullName);

                            l_LastWriteTime = fi.LastWriteTime;
                        }
                        catch (System.IO.FileNotFoundException e)
                        {
                            // To inform the user and continue is
                            // sufficient for this demonstration.
                            // Your application may require different behavior.
                            Console.WriteLine(e.Message);
                        }
                    }

                    addMemoryCardInfo(new MemoryCardInfo()
                    {
                        Visibility = System.Windows.Visibility.Hidden,
                        Index = -1,
                        Number = "Shared",
                        FileName = Name,
                        FilePath = FullName,
                        DateTime = l_LastWriteTime,
                        Date = l_LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss")
                    });
                }
                catch (Exception)
                {
                }
            }
            
            if (System.IO.Directory.Exists(Settings.Default.MemoryCardsFolder))
            {
                var l_IsoInfo = IsoManager.Instance.getIsoInfo(Emul.Instance.DiscSerial);

                var l_SelectedMemoryCardFile = l_IsoInfo.SelectedMemoryCardFile;

                string[] files = System.IO.Directory.GetFiles(Settings.Default.MemoryCardsFolder, "*" + l_ext);

                foreach (string s in files)
                {
                    // Create the FileInfo object only when needed to ensure
                    // the information is as current as possible.
                    System.IO.FileInfo fi = null;
                    try
                    {
                        fi = new System.IO.FileInfo(s);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        // To inform the user and continue is
                        // sufficient for this demonstration.
                        // Your application may require different behavior.
                        Console.WriteLine(e.Message);
                        continue;
                    }

                    var l_splits = fi.Name.Split(new char[] { '.' });

                    if (l_splits != null && l_splits.Length == 3)
                    {
                        if (fi.Name.Contains(Emul.Instance.DiscSerial))
                        {
                            int l_value = 0;

                            if (int.TryParse(l_splits[1], out l_value))
                            {
                                addMemoryCardInfo(new MemoryCardInfo()
                                {
                                    Visibility = System.Windows.Visibility.Visible,
                                    Index = l_value,
                                    Number = string.Format("{0:#}", l_value + 1),
                                    FileName = fi.Name,
                                    FilePath = fi.FullName,
                                    DateTime = fi.LastWriteTime,
                                    Date = fi.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss")
                                });
                            }
                        }
                    }
                }

                if(_memoryCardInfoCollection.Count < 2)
                {
                    var lIndexString = "0";

                    if (lIndexString.Length == 1)
                        lIndexString = lIndexString.PadLeft(2, '0');
                    
                    var Name = (l_IsoInfo.Title + "-" + Emul.Instance.DiscSerial) + "." + lIndexString + l_ext;

                    var FullName = Settings.Default.MemoryCardsFolder + Name;

                    if (Emul.Instance.GameType == GameType.PS1)
                    {
                        createPS1Mcd(FullName);
                    }
                    else if (Emul.Instance.GameType == GameType.PS2)
                    {
                        try
                        {
                            createPS2Mcd(FullName);
                        }
                        catch (Exception)
                        {
                            Name = Emul.Instance.DiscSerial + "." + lIndexString + l_ext;

                            FullName = Settings.Default.MemoryCardsFolder + Name;

                            createPS2Mcd(FullName);
                        }
                    }

                    addMemoryCardInfo(new MemoryCardInfo()
                    {
                        Visibility = System.Windows.Visibility.Visible,
                        Index = 0,
                        Number = string.Format("{0:#}", 0 + 1),
                        FileName = Name,
                        FilePath = FullName,
                        DateTime = DateTime.Now,
                        Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    });
                }
                
                for (int i = 0; i < _memoryCardInfoCollection.Count; i++)
                {
                    Emul.Instance.setMemoryCard(_memoryCardInfoCollection[i].FilePath, i);
                }
                               
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    foreach (var item in _memoryCardInfoCollection)
                    {
                        if (item.IsCurrent)
                        {
                            mCustomerView.MoveCurrentTo(item);

                            break;
                        }
                    }
                });
            }
        }

        private void addMemoryCardInfo(MemoryCardInfo aMemoryCardInfo)
        {
            if (!_memoryCardInfoCollection.Contains(aMemoryCardInfo, new Compare()))
            {
                _memoryCardInfoCollection.Add(aMemoryCardInfo);
            }
        }

        private void addMemoryCardInfo()
        {
            if (string.IsNullOrWhiteSpace(Emul.Instance.DiscSerial))
                return;

            string l_ext = getCurrentMemoryCardExtention();

            if (string.IsNullOrWhiteSpace(l_ext))
                return;
        }

        private void fetchCloudMemoryCard(string a_disk_serial)
        {
            //if (m_GoogleDriveAccess)
            //{
            //    var l_MemoryCardList = DriveManager.Instance.getMemoryCardList(a_disk_serial);

            //    foreach (var item in l_MemoryCardList)
            //    {
            //        if (m_ListSlotIndexes.Contains(item.Index))
            //        {
            //            addMemoryCardInfo(item);

            //            m_ListSlotIndexes.Remove(item.Index);
            //        }
            //        else
            //        {
            //            try
            //            {
            //                var l_memoryCard = _memoryCardInfoCollection.ToList().First(memoryCard => memoryCard.Index == item.Index);

            //                if (l_memoryCard != null)
            //                {
            //                    l_memoryCard.IsCloudsave = true;
            //                }
            //            }
            //            catch (Exception)
            //            {
            //            }
            //        }
            //    }
            //}
        }

        public void setMemoryCard()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {

                bool l_isSelected = false;

                foreach (var l_item in _memoryCardInfoCollection)
                {
                    if (l_item.IsCurrent)
                    {
                        l_isSelected = true;

                        break;
                    }
                }

                if (l_isSelected)
                    return;

                foreach (var l_item in _memoryCardInfoCollection)
                {
                    if (!l_item.IsCloudOnlysave)
                    {
                        l_isSelected = true;

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            mCustomerView.MoveCurrentTo(l_item);
                        });

                        break;
                    }
                }

                if (l_isSelected)
                    return;

                addMemoryCardInfo();

                foreach (var l_item in _memoryCardInfoCollection)
                {
                    if (!l_item.IsCloudOnlysave)
                    {
                        l_isSelected = true;

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            mCustomerView.MoveCurrentTo(l_item);
                        });

                        break;
                    }
                }

            });
        }

        private void removeMemoryCardInfo(MemoryCardInfo aMemoryCardInfo)
        {
            if (_memoryCardInfoCollection.Contains(aMemoryCardInfo, new Compare()))
            {
                _memoryCardInfoCollection.Remove(aMemoryCardInfo);

                if (aMemoryCardInfo.IsCurrent)
                {
                    Emul.Instance.setMemoryCard();
                }

                File.Delete(aMemoryCardInfo.FilePath);
                
                aMemoryCardInfo.IsCurrent = false;
            }
        }

        private string getCurrentMemoryCardExtention()
        {
            string l_result = "";

            do
            {
                switch (Emul.Instance.GameType)
                {
                    case GameType.Unknown:
                        break;
                    case GameType.PS1:
                        l_result = ".ps1";
                        break;
                    case GameType.PS2:
                        l_result = ".ps2";
                        break;
                    case GameType.PSP:
                        break;
                    default:
                        break;
                }

            } while (false);

            return l_result;
        }


        void createPS1Mcd(string a_filePath)
        {
            using (var fs = new FileStream(a_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int s = MCD_SIZE;

                fs.WriteByte(Convert.ToByte('M'));
                s--;
                fs.WriteByte(Convert.ToByte('C'));
                s--;
                while (s-- > (MCD_SIZE - 127))
                    fs.WriteByte(0);

                fs.WriteByte(0xe);
                s--;

                int i = 0;

                int j = 0;

                for (i = 0; i < 15; i++)
                { // 15 blocks
                    fs.WriteByte(0xa0);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0xff);
                    s--;
                    fs.WriteByte(0xff);
                    s--;
                    for (j = 0; j < 117; j++)
                    {
                        fs.WriteByte(0x00);
                        s--;
                    }
                    fs.WriteByte(0xa0);
                    s--;
                }

                for (i = 0; i < 20; i++)
                {
                    fs.WriteByte(0xff);
                    s--;
                    fs.WriteByte(0xff);
                    s--;
                    fs.WriteByte(0xff);
                    s--;
                    fs.WriteByte(0xff);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0x00);
                    s--;
                    fs.WriteByte(0xff);
                    s--;
                    fs.WriteByte(0xff);
                    s--;
                    for (j = 0; j < 118; j++)
                    {
                        fs.WriteByte(0x00);
                        s--;
                    }
                }

                while ((s--) >= 0)
                    fs.WriteByte(0);
            }
        }

        void createPS2Mcd(string a_filePath)
        {
            using (var fs = new FileStream(a_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                for (uint i = 0; i < (MC2_SIZE) / m_effeffs.Length; i++)
                {
                    try
                    {
                        fs.Write(m_effeffs, 0, m_effeffs.Length);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }
        }

        public async void removeItem(object a_Item)
        {
            var l_MemoryCardInfo = a_Item as MemoryCardInfo;

            if (l_MemoryCardInfo != null)
            {
                //if (l_MemoryCardInfo.IsCloudOnlysave)
                //{
                //    await DriveManager.Instance.startDeletingMemoryCardAsync(l_MemoryCardInfo.FileName, l_MemoryCardInfo.FilePath);

                //    if (_memoryCardInfoCollection.Contains(l_MemoryCardInfo, new Compare()))
                //    {
                //        _memoryCardInfoCollection.Remove(l_MemoryCardInfo);

                //        if (l_MemoryCardInfo.Index >= 0)
                //            m_ListSlotIndexes.Insert(0, l_MemoryCardInfo.Index);
                //    }
                //}
                //else
                    removeMemoryCardInfo(l_MemoryCardInfo);
            }
        }

        public void createItem()
        {
            addMemoryCardInfo();
        }

        public async void persistItemAsync(object a_Item)
        {
            //var l_Grid = a_Item as System.Windows.Controls.Grid;

            //var l_MemoryCardInfo = l_Grid.DataContext as MemoryCardInfo;

            //if (l_MemoryCardInfo != null)
            //{
            //    string lDescription = "Date=" + l_MemoryCardInfo.Date;

            //    var lProgressBannerGrid = l_Grid.FindName("mProgressBannerBorder") as System.Windows.FrameworkElement;


            //    var l_TempFileName = System.IO.Path.GetTempPath() + l_MemoryCardInfo.FileName;

            //    File.Copy(l_MemoryCardInfo.FilePath, l_TempFileName, true);

            //    string l_disk_serial = Emul.Instance.IsoInfo.Title + "-" + Emul.Instance.IsoInfo.DiscSerial;

            //    if (l_MemoryCardInfo.Index == -1)
            //        l_disk_serial = "MemoryCard.shared";

            //    if (lProgressBannerGrid != null)
            //        DriveManager.Instance.startUploadingMemoryCard(
            //            l_disk_serial,
            //            l_TempFileName,
            //            lProgressBannerGrid,
            //            lDescription,
            //            (a_state) => {
            //                if (a_state)
            //                {
            //                    try
            //                    {
            //                        File.Delete(l_TempFileName);
            //                    }
            //                    catch (Exception)
            //                    {
            //                    }

            //                    l_Grid.DataContext = null;

            //                    l_MemoryCardInfo.IsCloudsave = true;

            //                    l_Grid.DataContext = l_MemoryCardInfo;
            //                }
            //            });
            //}
        }

        public async void loadItemAsync(object a_Item)
        {
            //if (string.IsNullOrEmpty(Settings.Default.MemoryCardsFolder))
            //    return;

            //var l_Grid = a_Item as System.Windows.Controls.Grid;

            //var l_MemoryCardInfo = l_Grid.DataContext as MemoryCardInfo;

            //if (l_MemoryCardInfo != null)
            //{
            //    if (l_MemoryCardInfo.IsCurrent)
            //    {
            //        ModuleControl.Instance.setMemoryCard();
            //    }

            //    var lProgressBannerGrid = l_Grid.FindName("mProgressBannerBorder") as System.Windows.FrameworkElement;

            //    if (lProgressBannerGrid != null)
            //        await DriveManager.Instance.startDownloadingMemoryCardAsync(
            //            l_MemoryCardInfo.FilePath,
            //            lProgressBannerGrid);

            //    l_MemoryCardInfo.IsCloudsave = true;

            //    l_MemoryCardInfo.IsCloudOnlysave = false;

            //    if (_memoryCardInfoCollection.Contains(l_MemoryCardInfo, new Compare()))
            //    {
            //        _memoryCardInfoCollection.Remove(l_MemoryCardInfo);
            //    }

            //    addMemoryCardInfo(l_MemoryCardInfo);

            //    m_ListSlotIndexes.Remove(l_MemoryCardInfo.Index);

            //    if (l_MemoryCardInfo.IsCurrent)
            //    {
            //        mCustomerView.MoveCurrentTo(l_MemoryCardInfo);
            //    }
            //}
        }

        public bool accessPersistItem(object a_Item)
        {
            var l_Grid = a_Item as System.Windows.Controls.Grid;

            var l_MemoryCardInfo = l_Grid.DataContext as MemoryCardInfo;

            if (l_MemoryCardInfo != null)
            {
                return !l_MemoryCardInfo.IsCloudOnlysave;
            }

            return true;
        }

        public bool accessLoadItem(object a_Item)
        {
            var l_Grid = a_Item as System.Windows.Controls.Grid;

            var l_MemoryCardInfo = l_Grid.DataContext as MemoryCardInfo;

            if (l_MemoryCardInfo != null)
            {
                return l_MemoryCardInfo.IsCloudsave;
            }

            return false;
        }

        public void registerItem(object a_Item)
        {
        }

        public ICollectionView Collection
        {
            get { return mCustomerView; }
        }

        public object View { get; private set; }
    }
}
