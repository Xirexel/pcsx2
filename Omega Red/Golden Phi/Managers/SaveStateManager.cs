using Golden_Phi.Emul;
using Golden_Phi.Models;
using Golden_Phi.Properties;
using Golden_Phi.Tools.Savestate;
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

namespace Golden_Phi.Managers
{
    class SaveStateManager : IManager
    {
        class Compare : IEqualityComparer<SaveStateInfo>
        {
            public bool Equals(SaveStateInfo x, SaveStateInfo y)
            {
                if (x.Index == y.Index)
                {
                    return true;
                }
                else { return false; }
            }
            public int GetHashCode(SaveStateInfo codeh)
            {
                return 0;
            }

        }

        private ICollectionView mCustomerView = null;
        
        private ICollectionView mQuickSaveCustomerView = null;

        private CollectionViewSource mQuickSaveCollectionViewSource = new CollectionViewSource();
                
        private Thread m_loadThread = null;

        public event Action RefreshEvent;

        public string DiscSerial { get; private set; } = "";




        private readonly ObservableCollection<SaveStateInfo> _saveStateInfoCollection = new ObservableCollection<SaveStateInfo>();

        private List<int> m_ListSlotIndexes = new List<int>();

        private SaveStateInfo m_autoSave = new SaveStateInfo();

        private List<SaveStateInfo> m_quickSaves = new List<SaveStateInfo>();

        private static SaveStateManager m_Instance = null;

        public static SaveStateManager Instance { get { if (m_Instance == null) m_Instance = new SaveStateManager(); return m_Instance; } }

        private SaveStateManager()
        {
            if (string.IsNullOrEmpty(Settings.Default.SlotFolder))
                Settings.Default.SlotFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + App.c_MainFolderName + @"\sstates\";

            mCustomerView = CollectionViewSource.GetDefaultView(_saveStateInfoCollection);

            mCustomerView.SortDescriptions.Add(
                new SortDescription("DateTime", ListSortDirection.Descending));
            
            mQuickSaveCollectionViewSource.Source = _saveStateInfoCollection;

            mQuickSaveCustomerView = mQuickSaveCollectionViewSource.View;

            mQuickSaveCustomerView.Filter = new Predicate<object>(x => ((SaveStateInfo)x).IsQuicksave);

            mQuickSaveCustomerView.SortDescriptions.Add(new SortDescription("DateTime", ListSortDirection.Descending));
        }

        public void init()
        {
            if (string.IsNullOrEmpty(Settings.Default.SlotFolder))
                Settings.Default.SlotFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + App.c_MainFolderName + @"\sstates\";

            if (!System.IO.Directory.Exists(Settings.Default.SlotFolder))
            {
                System.IO.Directory.CreateDirectory(Settings.Default.SlotFolder);
            }

            var files = System.IO.Directory.GetFiles(Settings.Default.SlotFolder, App.c_sstate_ext + "tempstate");

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public void showSavings(IsoInfo a_IsoInfo)
        {
            DiscSerial = "";

            if (a_IsoInfo == null)
                return;
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                var l_ContextMenu = App.getResource("SStatesMenu") as ContextMenu;

                if (l_ContextMenu != null)
                {
                    setIsoInfoFilter(a_IsoInfo);

                    l_ContextMenu.IsOpen = true;
                }
            });

        }

        public void setIsoInfoFilter(IsoInfo a_IsoInfo)
        {
            if (a_IsoInfo == null)
                return;
            
            var l_disk_serial = a_IsoInfo.DiscSerial;

            var l_bios_check_sum =
            a_IsoInfo.BiosInfo != null ?
            a_IsoInfo.BiosInfo.GameType == a_IsoInfo.GameType ? "_" + a_IsoInfo.BiosInfo.CheckSum.ToString("X8") : ""
            : "";

            DiscSerial = l_disk_serial;

            load(l_disk_serial, l_bios_check_sum);
        }

        private void load(string a_disk_serial, string a_bios_check_sum)
        {
            lock (this)
            {
                string l_file_signature = a_disk_serial + a_bios_check_sum;
                                             
                ThreadStart l_loadThreadStart = new ThreadStart(() =>
                {
                    load_inner(a_disk_serial, a_bios_check_sum);
                });

                if (m_loadThread != null)
                    m_loadThread.Abort();

                m_loadThread = new Thread(l_loadThreadStart);

                m_loadThread.Start();
            }
        }

        private void load_inner_current(string a_disk_serial, string a_bios_check_sum)
        {
            string l_file_signature = a_disk_serial + a_bios_check_sum;

            _saveStateInfoCollection.Clear();

            m_ListSlotIndexes.Clear();

            for (int i = 0; i < 126; i++)
            {
                m_ListSlotIndexes.Add(i);
            }

            if (System.IO.Directory.Exists(Settings.Default.SlotFolder))
            {
                string[] files = System.IO.Directory.GetFiles(Settings.Default.SlotFolder, App.c_sstate_ext);

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

                    if (fi.Name.Contains(l_file_signature))
                    {
                        var l_splits = fi.Name.Split(new char[] { '.' });

                        if (l_splits != null && l_splits.Length == 3)
                        {
                            int l_value = 0;

                            if (int.TryParse(l_splits[1], out l_value))
                            {
                                try
                                {
                                    addSaveStateInfo(SStates.Instance.readData(a_disk_serial, s, l_value), _saveStateInfoCollection);

                                    m_ListSlotIndexes.Remove(l_value);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 1; i++)
            {
                int l_index = i + 100;

                var lautoSaveState = new SaveStateInfo()
                {
                    IsAutosave = true,
                    FilePath = Settings.Default.SlotFolder +
                        l_file_signature + ".auto." +
                        l_index.ToString() + App.c_sstate_ext,
                    Index = l_index,
                    DiscSerial = a_disk_serial

                };

                if (File.Exists(lautoSaveState.FilePath))
                {
                    try
                    {
                        addSaveStateInfo(SStates.Instance.readData(lautoSaveState.DiscSerial, lautoSaveState.FilePath, l_index, lautoSaveState.IsAutosave), _saveStateInfoCollection);

                        m_ListSlotIndexes.Remove(l_index);
                    }
                    catch (Exception)
                    {
                    }
                }

                m_autoSave = lautoSaveState;
            }

            List<SaveStateInfo> l_quickSaves = new List<SaveStateInfo>();

            for (int i = 0; i < 25; i++)
            {
                int l_index = i + 101;

                var l_GameSessionDuration = new TimeSpan(0);

                var lquickSaveState = new SaveStateInfo()
                {
                    IsQuicksave = true,
                    FilePath = Settings.Default.SlotFolder +
                        l_file_signature + ".quick." +
                        l_index.ToString() + App.c_sstate_ext,
                    Index = l_index,
                    DateTime = DateTime.Now,
                    Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")),
                    Duration = l_GameSessionDuration.ToString(@"dd\.hh\:mm\:ss"),
                    DurationNative = l_GameSessionDuration,
                    DiscSerial = a_disk_serial
                };

                if (File.Exists(lquickSaveState.FilePath))
                {
                    try
                    {
                        lquickSaveState = SStates.Instance.readData(lquickSaveState.DiscSerial, lquickSaveState.FilePath, l_index, lquickSaveState.IsAutosave, lquickSaveState.IsQuicksave);

                        addSaveStateInfo(lquickSaveState, _saveStateInfoCollection);

                        m_ListSlotIndexes.Remove(l_index);
                    }
                    catch (Exception)
                    {
                    }
                }

                l_quickSaves.Add(lquickSaveState);
            }

            m_quickSaves = l_quickSaves.OrderBy(o => o.DateTime).ToList();
        }

        private void load_inner(string a_disk_serial, string a_bios_check_sum)
        {
            string l_file_signature = a_disk_serial + a_bios_check_sum;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                _saveStateInfoCollection.Clear();
            });

            m_ListSlotIndexes.Clear();

            for (int i = 0; i < 126; i++)
            {
                m_ListSlotIndexes.Add(i);
            }

            if (System.IO.Directory.Exists(Settings.Default.SlotFolder))
            {
                string[] files = System.IO.Directory.GetFiles(Settings.Default.SlotFolder, l_file_signature + ".*" + App.c_sstate_ext);

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

                    if (fi.Name.Contains(l_file_signature))
                    {
                        var l_splits = fi.Name.Split(new char[] { '.' });

                        if (l_splits != null && l_splits.Length == 3)
                        {
                            int l_value = 0;

                            if (int.TryParse(l_splits[1], out l_value))
                            {
                                try
                                {
                                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                    {
                                        addSaveStateInfo(SStates.Instance.readData(a_disk_serial, s, l_value), _saveStateInfoCollection);
                                    });

                                    m_ListSlotIndexes.Remove(l_value);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 1; i++)
            {
                int l_index = i + 100;

                var lautoSaveState = new SaveStateInfo()
                {
                    IsAutosave = true,
                    FilePath = Settings.Default.SlotFolder +
                        l_file_signature + ".auto." +
                        l_index.ToString() + App.c_sstate_ext,
                    Index = l_index,
                    DiscSerial = a_disk_serial

                };

                if (File.Exists(lautoSaveState.FilePath))
                {
                    try
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            addSaveStateInfo(SStates.Instance.readData(lautoSaveState.DiscSerial, lautoSaveState.FilePath, l_index, lautoSaveState.IsAutosave), _saveStateInfoCollection);
                        });

                        m_ListSlotIndexes.Remove(l_index);
                    }
                    catch (Exception)
                    {
                    }
                }

                m_autoSave = lautoSaveState;
            }

            List<SaveStateInfo> l_quickSaves = new List<SaveStateInfo>();

            for (int i = 0; i < 25; i++)
            {
                int l_index = i + 101;

                var l_GameSessionDuration = new TimeSpan(0);

                var lquickSaveState = new SaveStateInfo()
                {
                    IsQuicksave = true,
                    FilePath = Settings.Default.SlotFolder +
                        l_file_signature + ".quick." +
                        l_index.ToString() + App.c_sstate_ext,
                    Index = l_index,
                    DateTime = DateTime.Now,
                    Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")),
                    Duration = l_GameSessionDuration.ToString(@"dd\.hh\:mm\:ss"),
                    DurationNative = l_GameSessionDuration,
                    DiscSerial = a_disk_serial
                };

                if (File.Exists(lquickSaveState.FilePath))
                {
                    try
                    {
                        lquickSaveState = SStates.Instance.readData(lquickSaveState.DiscSerial, lquickSaveState.FilePath, l_index, lquickSaveState.IsAutosave, lquickSaveState.IsQuicksave);

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            addSaveStateInfo(lquickSaveState, _saveStateInfoCollection);
                        });

                        m_ListSlotIndexes.Remove(l_index);
                    }
                    catch (Exception)
                    {
                    }
                }

                l_quickSaves.Add(lquickSaveState);
            }

            m_quickSaves = l_quickSaves.OrderBy(o => o.DateTime).ToList();
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                mCustomerView.Refresh();

                if (RefreshEvent != null)
                    RefreshEvent();
            });
        }

        public SaveStateInfo getAutoSaveStateInfo(string a_disk_serial, string a_bios_check_sum, int a_DecodePixelWidth = 100)
        {
            string l_file_signature = a_disk_serial + a_bios_check_sum;

            var l_fileName = l_file_signature + ".auto." + "100" + App.c_sstate_ext;

            SaveStateInfo l_autoSave = new SaveStateInfo();

            l_autoSave.FilePath = Settings.Default.SlotFolder + l_fileName;

            do
            {
                string[] files = System.IO.Directory.GetFiles(Settings.Default.SlotFolder, l_fileName);

                if(files != null && files.Length > 0)
                {
                    SaveStateInfo l_SaveStateInfo = SStates.Instance.readData(a_disk_serial, files[0], 100, true, false, a_DecodePixelWidth);

                    if (l_SaveStateInfo != null)
                        l_autoSave = l_SaveStateInfo;
                }
                else
                {
                }

            } while (false);

            return l_autoSave;
        }

        void Instance_m_ChangeStatusEvent(Emul.Emul.StatusEnum a_Status)
        {
        }
        
        public void addSaveStateInfo()
        {
            addSaveStateInfo(createSaveStateInfo());

            mCustomerView.Refresh();
        }

        private SaveStateInfo createSaveStateInfo()
        {
            SaveStateInfo l_SaveStateInfo = null;

            int lIndex = -1;

            uint lCheckSum = 0;

            string lFilePath = "";

            string l_file_signature = Emul.Emul.Instance.getFileSignature();

            if (m_ListSlotIndexes.Count > 0)
            {
                lIndex = m_ListSlotIndexes[0];

                m_ListSlotIndexes.RemoveAt(0);

                //if (PCSX2Controller.Instance.IsoInfo.GameType == GameType.PSP)
                //    lCheckSum = 1;
                //else
                //    lCheckSum = PCSX2Controller.Instance.BiosInfo.CheckSum;

                var lIndexString = lIndex.ToString();

                if (lIndexString.Length == 1)
                    lIndexString = lIndexString.PadLeft(2, '0');

                lFilePath = Settings.Default.SlotFolder +
                         l_file_signature + "." +
                         lIndexString + App.c_sstate_ext;

                l_SaveStateInfo = new SaveStateInfo() { Index = lIndex, CheckSum = lCheckSum, Visibility = Visibility.Collapsed, FilePath = lFilePath };

                Emul.Emul.Instance.saveState(l_SaveStateInfo);
            }

            return l_SaveStateInfo;
        }

        private void addSaveStateInfo(SaveStateInfo a_SaveStateInfo, ObservableCollection<SaveStateInfo> a_saveStateInfoCollection = null)
        {
            if (a_SaveStateInfo.Type == GameType.Unknown)
            {
                if (a_SaveStateInfo.CheckSum == 1)
                    a_SaveStateInfo.Type = GameType.PSP;
                else if (a_SaveStateInfo.CheckSum == 2)
                    a_SaveStateInfo.Type = GameType.PS1;
                else
                    a_SaveStateInfo.Type = GameType.PS2;
            }

            if (a_saveStateInfoCollection == null)
            {
                if (!_saveStateInfoCollection.Contains(a_SaveStateInfo, new Compare()))
                {
                    _saveStateInfoCollection.Add(a_SaveStateInfo);
                }
            }
            else
            {
                if (!a_saveStateInfoCollection.Contains(a_SaveStateInfo, new Compare()))
                {
                    a_saveStateInfoCollection.Add(a_SaveStateInfo);
                }
            }
        }

        private void addSaveStateInfo(string a_disk_serial, string a_file_path, string a_file_name)
        {
            var l_splits = a_file_name.Split(new char[] { '.' });

            if (l_splits != null && l_splits.Length == 3)
            {
                int l_value = 0;

                if (int.TryParse(l_splits[1], out l_value))
                {
                    addSaveStateInfo(SStates.Instance.readData(a_disk_serial, a_file_path, l_value));
                }
            }
        }

        public void removeSaveStateInfo(SaveStateInfo a_SaveStateInfo)
        {
            removeSaveStateInfo(a_SaveStateInfo, false);
        }

        public void removeSaveStateInfo(SaveStateInfo a_SaveStateInfo, bool a_clear)
        {
            if (_saveStateInfoCollection.Contains(a_SaveStateInfo, new Compare()))
            {
                _saveStateInfoCollection.Remove(a_SaveStateInfo);

                m_ListSlotIndexes.Add(a_SaveStateInfo.Index);

                if (a_SaveStateInfo.IsCloudsave)
                {
                }
            }

            //if (a_clear)
            //{

            //    System.Threading.ThreadPool.QueueUserWorkItem((object state) =>
            //    {
            //        throw new Exception();

            //        //System.IO.FileInfo fi = null;

            //        //try
            //        //{
            //        //    fi = new System.IO.FileInfo(a_SaveStateInfo.FilePath);
            //        //}
            //        //catch (System.IO.FileNotFoundException e)
            //        //{
            //        //    // To inform the user and continue is
            //        //    // sufficient for this demonstration.
            //        //    // Your application may require different behavior.
            //        //    Console.WriteLine(e.Message);
            //        //}

            //        //if (fi.Name.Contains(PCSX2Controller.Instance.IsoInfo.DiscSerial))
            //        //{
            //        //    var l_splits = fi.Name.Split(new char[] { '.' });

            //        //    if (l_splits != null && l_splits.Length == 3)
            //        //    {
            //        //        int l_value = 0;

            //        //        if (int.TryParse(l_splits[1], out l_value))
            //        //        {
            //        //            var l_SaveStateInfo = SStates.Instance.readData(a_SaveStateInfo.FilePath, l_value);

            //        //            if (l_SaveStateInfo != null)
            //        //            {
            //        //                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
            //        //                {
            //        //                    addSaveStateInfo(l_SaveStateInfo);
            //        //                });
            //        //            }
            //        //        }
            //        //    }
            //        //    else
            //        //    if (l_splits != null && l_splits.Length == 4 && l_splits[1] == "auto")
            //        //    {
            //        //        int l_value = 0;

            //        //        if (int.TryParse(l_splits[2], out l_value))
            //        //        {
            //        //            var l_SaveStateInfo = SStates.Instance.readData(a_SaveStateInfo.FilePath, l_value);

            //        //            if (l_SaveStateInfo != null)
            //        //            {
            //        //                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (System.Threading.ThreadStart)delegate ()
            //        //                {
            //        //                    addSaveStateInfo(l_SaveStateInfo);
            //        //                });
            //        //            }
            //        //        }
            //        //    }
            //        //}
            //    });
            //}
            //else
            {
                if (File.Exists(a_SaveStateInfo.FilePath))
                    File.Delete(a_SaveStateInfo.FilePath);
            }
        }

        public void createItem()
        {
        }

        public void removeItem(object a_Item)
        {
            removeSaveStateInfo(a_Item as SaveStateInfo);
        }

        public ICollectionView Collection
        {
            get { return mCustomerView; }
        }
        
        public ICollectionView QuickSaveCollection
        {
            get { return mQuickSaveCustomerView; }
        }

        public object View => null;
    }
}
