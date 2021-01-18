/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using Omega_Red.Managers;
using Omega_Red.Models;
using Omega_Red.Properties;
using Omega_Red.Tools;
using Omega_Red.Panels;
using Omega_Red.Tools.Savestate;
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
using Omega_Red.SocialNetworks.Google;
using System.Xml.Serialization;
using Omega_Red.Emulators;

namespace Omega_Red.Managers
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

        private ICollectionView mAutoSaveCustomerView = null;

        private CollectionViewSource mAutoSaveCollectionViewSource = new CollectionViewSource();

        private ICollectionView mQuickSaveCustomerView = null;

        private CollectionViewSource mQuickSaveCollectionViewSource = new CollectionViewSource();

        private bool m_GoogleDriveAccess = false;

        private string m_file_signature = "";

        private Thread m_loadThread = null;
        
        public event Action RefreshEvent;


        


        private readonly ObservableCollection<SaveStateInfo> _saveStateInfoCollection = new ObservableCollection<SaveStateInfo>();

        private List<int> m_ListSlotIndexes = new List<int>();
        
        private SaveStateInfo m_autoSave = new SaveStateInfo();

        private List<SaveStateInfo> m_quickSaves = new List<SaveStateInfo>();

        private static SaveStateManager m_Instance = null;

        public static SaveStateManager Instance { get { if (m_Instance == null) m_Instance = new SaveStateManager(); return m_Instance; } }
        
        private SaveStateManager()
        {
            GoogleAccountManager.Instance.mEnableStateEvent += (obj)=>
            {
                m_GoogleDriveAccess = obj;

                m_file_signature = "";
                
                if (Emul.Instance.IsoInfo != null)
                {
                    var l_disk_serial = Emul.Instance.IsoInfo.DiscSerial;

                    var l_bios_check_sum =
                    Emul.Instance.BiosInfo != null ?
                    Emul.Instance.BiosInfo.GameType == Emul.Instance.IsoInfo.GameType ? "_" + Emul.Instance.BiosInfo.CheckSum.ToString("X8") : ""
                    : "";

                    load(l_disk_serial, l_bios_check_sum);
                }
            };

            if (string.IsNullOrEmpty(Settings.Default.SlotFolder))
                Settings.Default.SlotFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + App.m_MainFolderName + @"\sstates\";
            
            mCustomerView = CollectionViewSource.GetDefaultView(_saveStateInfoCollection);

            mCustomerView.SortDescriptions.Add(
                new SortDescription("DateTime", ListSortDirection.Descending));

            PropertyGroupDescription l_groupDescription = new PropertyGroupDescription("GameDiscType");

            mCustomerView.GroupDescriptions.Add(l_groupDescription);

            mAutoSaveCollectionViewSource.Source = _saveStateInfoCollection;

            mAutoSaveCustomerView = mAutoSaveCollectionViewSource.View;

            mAutoSaveCustomerView.Filter = new Predicate<object>(x => ((SaveStateInfo)x).IsAutosave);


            mQuickSaveCollectionViewSource.Source = _saveStateInfoCollection;

            mQuickSaveCustomerView = mQuickSaveCollectionViewSource.View;

            mQuickSaveCustomerView.Filter = new Predicate<object>(x => ((SaveStateInfo)x).IsQuicksave);

            mQuickSaveCustomerView.SortDescriptions.Add(new SortDescription("DateTime", ListSortDirection.Descending));
        }

        public void init()
        {
            if (string.IsNullOrEmpty(Settings.Default.SlotFolder))
                Settings.Default.SlotFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + App.m_MainFolderName + @"\sstates\";

            if (!System.IO.Directory.Exists(Settings.Default.SlotFolder))
            {
                System.IO.Directory.CreateDirectory(Settings.Default.SlotFolder);
            }

            Emul.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;


            var files = System.IO.Directory.GetFiles(Settings.Default.SlotFolder, "*.p2stempstate");

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        private void load(string a_disk_serial, string a_bios_check_sum)
        {
            lock (this)
            {
                string l_file_signature = a_disk_serial + a_bios_check_sum;

                if (m_file_signature == l_file_signature)
                    return;

                m_file_signature = l_file_signature;

                //load_inner_current(a_disk_serial, a_bios_check_sum);



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
                string[] files = System.IO.Directory.GetFiles(Settings.Default.SlotFolder, "*.p2s");

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
                                    addSaveStateInfo(SStates.Instance.readData(s, l_value), _saveStateInfoCollection);

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
                        l_index.ToString() + ".p2s",
                    Index = l_index

                };

                if (File.Exists(lautoSaveState.FilePath))
                {
                    try
                    {
                        addSaveStateInfo(SStates.Instance.readData(lautoSaveState.FilePath, l_index, lautoSaveState.IsAutosave), _saveStateInfoCollection);

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
                        l_index.ToString() + ".p2s",
                    Index = l_index,
                    DateTime = DateTime.Now,
                    Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")),
                    Duration = l_GameSessionDuration.ToString(@"dd\.hh\:mm\:ss"),
                    DurationNative = l_GameSessionDuration
                };

                if (File.Exists(lquickSaveState.FilePath))
                {
                    try
                    {
                        lquickSaveState = SStates.Instance.readData(lquickSaveState.FilePath, l_index, lquickSaveState.IsAutosave, lquickSaveState.IsQuicksave);

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

            fetchCloudState(l_file_signature, _saveStateInfoCollection);
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
                string[] files = System.IO.Directory.GetFiles(Settings.Default.SlotFolder, "*.p2s");

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

                    if (fi.Name.Contains(l_file_signature + "."))
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
                                        addSaveStateInfo(SStates.Instance.readData(s, l_value), _saveStateInfoCollection);
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
                        l_index.ToString() + ".p2s",
                    Index = l_index

                };

                if (File.Exists(lautoSaveState.FilePath))
                {
                    try
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            addSaveStateInfo(SStates.Instance.readData(lautoSaveState.FilePath, l_index, lautoSaveState.IsAutosave), _saveStateInfoCollection);
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
                        l_index.ToString() + ".p2s",
                    Index = l_index,
                    DateTime = DateTime.Now,
                    Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")),
                    Duration = l_GameSessionDuration.ToString(@"dd\.hh\:mm\:ss"),
                    DurationNative = l_GameSessionDuration
                };

                if (File.Exists(lquickSaveState.FilePath))
                {
                    try
                    {
                        lquickSaveState = SStates.Instance.readData(lquickSaveState.FilePath, l_index, lquickSaveState.IsAutosave, lquickSaveState.IsQuicksave);

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

            fetchCloudStateDispatcher(l_file_signature, _saveStateInfoCollection);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                mCustomerView.Refresh();

                if (RefreshEvent != null)
                    RefreshEvent();
            });
        }
        private void fetchCloudStateDispatcher(string a_disk_serial, ObservableCollection<SaveStateInfo> a_saveStateInfoCollection)
        {
            if (m_GoogleDriveAccess)
            {
                var l_SaveStateList = DriveManager.Instance.getSaveStateList(a_disk_serial);

                foreach (var item in l_SaveStateList)
                {
                    if (m_ListSlotIndexes.Contains(item.Index))
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            addSaveStateInfo(item, a_saveStateInfoCollection);
                        });

                        m_ListSlotIndexes.Remove(item.Index);
                    }
                    else
                    {
                        try
                        {
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                            {
                                var lsaveState = a_saveStateInfoCollection.ToList().FirstOrDefault(saveState => saveState.Index == item.Index);

                                if (lsaveState != null)
                                {
                                    lsaveState.IsCloudsave = true;

                                    lsaveState.CloudSaveDate = item.Date;

                                    lsaveState.CloudSaveDuration = item.Duration;
                                }
                            });
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        private void fetchCloudState(string a_disk_serial, ObservableCollection<SaveStateInfo> a_saveStateInfoCollection)
        {
            if (m_GoogleDriveAccess)
            {
                var l_SaveStateList = DriveManager.Instance.getSaveStateList(a_disk_serial);

                foreach (var item in l_SaveStateList)
                {
                    if (m_ListSlotIndexes.Contains(item.Index))
                    {
                        addSaveStateInfo(item, a_saveStateInfoCollection);

                        m_ListSlotIndexes.Remove(item.Index);
                    }
                    else
                    {
                        try
                        {
                            var lsaveState = a_saveStateInfoCollection.ToList().FirstOrDefault(saveState => saveState.Index == item.Index);

                            if (lsaveState != null)
                            {
                                lsaveState.IsCloudsave = true;

                                lsaveState.CloudSaveDate = item.Date;

                                lsaveState.CloudSaveDuration = item.Duration;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        
        void Instance_m_ChangeStatusEvent(Emul.StatusEnum a_Status)
        {
            if (a_Status != Emul.StatusEnum.NoneInitilized)
            {
                if (Emul.Instance.IsoInfo != null)
                {
                    var l_disk_serial = Emul.Instance.IsoInfo.DiscSerial;

                    var l_bios_check_sum =
                    Emul.Instance.BiosInfo != null ?
                    Emul.Instance.BiosInfo.GameType == Emul.Instance.IsoInfo.GameType ? "_" + Emul.Instance.BiosInfo.CheckSum.ToString("X8") : ""
                    : "";

                    load(l_disk_serial, l_bios_check_sum);
                }
            }
        }

        private SaveStateInfo createSaveStateInfo()
        {
            int lIndex = -1;

            uint lCheckSum = 0;

            string lFilePath = "";

            if (m_ListSlotIndexes.Count > 0)
            {
                lIndex = m_ListSlotIndexes[0];

                m_ListSlotIndexes.RemoveAt(0);

                if (Emul.Instance.IsoInfo.GameType == GameType.PSP)
                    lCheckSum = 1;
                else
                    lCheckSum = Emul.Instance.BiosInfo.CheckSum;


                var lIndexString = lIndex.ToString();
                
                if(lIndexString.Length == 1)
                    lIndexString = lIndexString.PadLeft(2, '0');

                lFilePath = Settings.Default.SlotFolder +
                         m_file_signature + "." +
                         lIndexString + ".p2s";

            }

            return new SaveStateInfo() { Index = lIndex, CheckSum = lCheckSum, Visibility = Visibility.Collapsed, FilePath = lFilePath };
        }

        public void addSaveStateInfo()
        {
            addSaveStateInfo(createSaveStateInfo());
        }
        
        private void addSaveStateInfo(SaveStateInfo a_SaveStateInfo, ObservableCollection<SaveStateInfo> a_saveStateInfoCollection = null)
        {
            if(Emul.Instance.IsoInfo != null)
                a_SaveStateInfo.DiscSerial = Emul.Instance.IsoInfo.DiscSerial;

            if (Emul.Instance.BiosInfo == null || Emul.Instance.BiosInfo.CheckSum != a_SaveStateInfo.CheckSum)
                if (1 != a_SaveStateInfo.CheckSum && 2 != a_SaveStateInfo.CheckSum)
                    return;
                else
                {
                    if(a_SaveStateInfo.CheckSum == 1)
                        a_SaveStateInfo.Type = GameType.PSP;
                    else if (a_SaveStateInfo.CheckSum == 2)
                        a_SaveStateInfo.Type = GameType.PS1;

                }
            else
                a_SaveStateInfo.Type = GameType.PS2;

            if(a_saveStateInfoCollection == null)
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

        private void addSaveStateInfo(string a_file_path, string a_file_name)
        {
            var l_splits = a_file_name.Split(new char[] { '.' });

            if (l_splits != null && l_splits.Length == 3)
            {
                int l_value = 0;

                if (int.TryParse(l_splits[1], out l_value))
                {
                    addSaveStateInfo(SStates.Instance.readData(a_file_path, l_value));
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
                    fetchCloudState(m_file_signature, _saveStateInfoCollection);
                }
            }

            if(a_clear)
            {

                System.Threading.ThreadPool.QueueUserWorkItem((object state) =>
                {
                    
                    System.IO.FileInfo fi = null;

                    try
                    {
                        fi = new System.IO.FileInfo(a_SaveStateInfo.FilePath);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        // To inform the user and continue is
                        // sufficient for this demonstration.
                        // Your application may require different behavior.
                        Console.WriteLine(e.Message);
                    }

                    if (fi.Name.Contains(Emul.Instance.IsoInfo.DiscSerial))
                    {
                        var l_splits = fi.Name.Split(new char[] { '.' });

                        if (l_splits != null && l_splits.Length == 3)
                        {
                            int l_value = 0;

                            if (int.TryParse(l_splits[1], out l_value))
                            {
                                var l_SaveStateInfo = SStates.Instance.readData(a_SaveStateInfo.FilePath, l_value);

                                if (l_SaveStateInfo != null)
                                {
                                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (System.Threading.ThreadStart)delegate()
                                    {
                                        addSaveStateInfo(l_SaveStateInfo);
                                    });
                                }
                            }
                        }
                        else
                        if (l_splits != null && l_splits.Length == 4 && l_splits[1] == "auto")
                        {
                            int l_value = 0;

                            if (int.TryParse(l_splits[2], out l_value))
                            {
                                var l_SaveStateInfo = SStates.Instance.readData(a_SaveStateInfo.FilePath, l_value);

                                if (l_SaveStateInfo != null)
                                {
                                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (System.Threading.ThreadStart)delegate()
                                    {
                                        addSaveStateInfo(l_SaveStateInfo);
                                    });
                                }
                            }
                        }
                    }
                });
            }
            else
            {
                if (File.Exists(a_SaveStateInfo.FilePath))
                    File.Delete(a_SaveStateInfo.FilePath);
            }
        }

        public async void save(SaveStateInfo a_SaveStateInfo)
        {
            if (System.IO.Directory.Exists(Settings.Default.SlotFolder))
            {
                await Emul.Instance.saveState(a_SaveStateInfo);

                updateSave(a_SaveStateInfo);
            }
        }

        public async void quickSave()
        {
            if (System.IO.Directory.Exists(Settings.Default.SlotFolder))
            {
                var l_quickSave = m_quickSaves.ElementAt(0);

                m_quickSaves.RemoveAt(0);

                await Emul.Instance.quickSave(l_quickSave);

                m_quickSaves.Add(l_quickSave);

                updateSave(l_quickSave);
            }
        }

        private void updateAutoSave()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                var lstateSave = SStates.Instance.readData(m_autoSave.FilePath, m_autoSave.Index, m_autoSave.IsAutosave);

                var lcurrentstateSave = _saveStateInfoCollection.FirstOrDefault(stateSave => stateSave.Index == m_autoSave.Index);

                _saveStateInfoCollection.Remove(lcurrentstateSave);

                if (lcurrentstateSave != null)
                    lstateSave.IsCloudsave = lcurrentstateSave.IsCloudsave;

                if (lcurrentstateSave != null && lcurrentstateSave.IsCloudOnlysave)
                    lstateSave.IsCloudsave = true;

                if (lcurrentstateSave != null)
                {
                    lstateSave.CloudSaveDate = lcurrentstateSave.CloudSaveDate;

                    lstateSave.CloudSaveDuration = lcurrentstateSave.CloudSaveDuration;
                }

                addSaveStateInfo(lstateSave);
            });
        }

        public void updateSave(SaveStateInfo a_saveState)
        {
            try
            {

                var lstateSave = SStates.Instance.readData(a_saveState.FilePath, a_saveState.Index, a_saveState.IsAutosave, a_saveState.IsQuicksave);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    var lcurrentstateSave = _saveStateInfoCollection.FirstOrDefault(stateSave => stateSave.Index == a_saveState.Index);

                    _saveStateInfoCollection.Remove(lcurrentstateSave);

                    if (lcurrentstateSave != null)
                        lstateSave.IsCloudsave = lcurrentstateSave.IsCloudsave;

                    if (lcurrentstateSave != null && lcurrentstateSave.IsCloudOnlysave)
                        lstateSave.IsCloudsave = true;

                    if (lcurrentstateSave != null)
                    {
                        lstateSave.CloudSaveDate = lcurrentstateSave.CloudSaveDate;

                        lstateSave.CloudSaveDuration = lcurrentstateSave.CloudSaveDuration;
                    }

                    addSaveStateInfo(lstateSave);

                    Collection.Refresh();
                });
            }
            catch (Exception)
            {
            }
        }
        
        public async void persistItemAsync(object a_Item, SaveStateInfo a_SaveStateInfo, Action a_callbackAction)
        {
            if (a_Item == null)
                return;

            var l_Grid = a_Item as System.Windows.Controls.Grid;

            if (l_Grid == null)
                return;
            
            if (a_SaveStateInfo != null)
            {
                string lDescription = "Date=" + a_SaveStateInfo.Date;

                lDescription += "|" + "Duration=" + a_SaveStateInfo.Duration;

                lDescription += "|" + "CheckSum=" + a_SaveStateInfo.CheckSum.ToString();

                var lProgressBannerGrid = l_Grid.FindName("mProgressBannerBorder") as System.Windows.FrameworkElement;

                if (lProgressBannerGrid != null)
                    DriveManager.Instance.startUploadingSState(a_SaveStateInfo.FilePath, lProgressBannerGrid, lDescription,
                        (a_state) => {

                            if (a_state)
                            {
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    l_Grid.DataContext = null;

                                    a_SaveStateInfo.IsCloudsave = true;

                                    a_SaveStateInfo.IsCloudOnlysave = false;

                                    a_SaveStateInfo.CloudSaveDate = a_SaveStateInfo.Date;

                                    a_SaveStateInfo.CloudSaveDuration = a_SaveStateInfo.Duration;

                                    mCustomerView.Refresh();
                                });
                            }
                        });

                if (a_callbackAction != null)
                    a_callbackAction();
            }
            else
            {
                if (a_callbackAction != null)
                    a_callbackAction();
            }
        }
                                     
        public async void removeItem(object a_Item)
        {
            var l_SaveStateInfo = a_Item as SaveStateInfo;

            if (l_SaveStateInfo != null)
            {
                if (l_SaveStateInfo.IsCloudOnlysave)
                {
                    await DriveManager.Instance.startDeletingSStateAsync(l_SaveStateInfo.FilePath);

                    if (_saveStateInfoCollection.Contains(l_SaveStateInfo, new Compare()))
                    {
                        _saveStateInfoCollection.Remove(l_SaveStateInfo);
                    }
                }
                else
                    removeSaveStateInfo(l_SaveStateInfo);
            }
        }

        public void createItem(){}

        public void persistItemAsync(object a_Item)
        {
            if (a_Item == null)
                return;

            var l_Grid = a_Item as System.Windows.Controls.Grid;

            if (l_Grid == null)
                return;

            var l_SaveStateInfo = l_Grid.DataContext as SaveStateInfo;

            if (l_SaveStateInfo != null)
            {
                string lDescription = "Date=" + l_SaveStateInfo.Date;

                lDescription += "|" + "Duration=" + l_SaveStateInfo.Duration;

                lDescription += "|" + "CheckSum=" + l_SaveStateInfo.CheckSum.ToString();

                var lProgressBannerGrid = l_Grid.FindName("mProgressBannerBorder") as System.Windows.FrameworkElement;

                if (lProgressBannerGrid != null)
                    DriveManager.Instance.startUploadingSState(
                        l_SaveStateInfo.FilePath,
                        lProgressBannerGrid, 
                        lDescription,
                        (a_state)=> {

                            if(a_state)
                            {
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    l_Grid.DataContext = null;

                                    l_SaveStateInfo.IsCloudsave = true;

                                    l_SaveStateInfo.IsCloudOnlysave = false;

                                    l_SaveStateInfo.CloudSaveDate = l_SaveStateInfo.Date;

                                    l_SaveStateInfo.CloudSaveDuration = l_SaveStateInfo.Duration;

                                    mCustomerView.Refresh();
                                });
                            }
                        });
            }
        }


        //public async void persistItemAsync(object a_Item)
        //{
        //    if (a_Item == null)
        //        return;

        //    var l_Grid = a_Item as System.Windows.Controls.Grid;

        //    if (l_Grid == null)
        //        return;

        //    var l_SaveStateInfo = l_Grid.DataContext as SaveStateInfo;

        //    if (l_SaveStateInfo != null)
        //    {
        //        string lDescription = "Date=" + l_SaveStateInfo.Date;

        //        lDescription += "|" + "Duration=" + l_SaveStateInfo.Duration;

        //        lDescription += "|" + "CheckSum=" + l_SaveStateInfo.CheckSum.ToString();                

        //        var lProgressBannerGrid = l_Grid.FindName("mProgressBannerBorder") as System.Windows.FrameworkElement;

        //        if (lProgressBannerGrid != null)
        //            await DriveManager.Instance.startUploadingSStateAsync(l_SaveStateInfo.FilePath, lProgressBannerGrid, lDescription);

        //        l_Grid.DataContext = null;

        //        l_SaveStateInfo.IsCloudsave = true;

        //        l_SaveStateInfo.IsCloudOnlysave = false;

        //        l_SaveStateInfo.CloudSaveDate = l_SaveStateInfo.Date;

        //        l_SaveStateInfo.CloudSaveDuration = l_SaveStateInfo.Duration;

        //        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
        //        {
        //            mCustomerView.Refresh();
        //        });
        //    }
        //}

        public async void loadItemAsync(object a_Item)
        {
            if (string.IsNullOrEmpty(Settings.Default.SlotFolder))
                return;

            var l_Grid = a_Item as System.Windows.Controls.Grid;

            var l_SaveStateInfo = l_Grid.DataContext as SaveStateInfo;

            if (l_SaveStateInfo != null)
            {
                var lProgressBannerGrid = l_Grid.FindName("mProgressBannerBorder") as System.Windows.FrameworkElement;

                if (lProgressBannerGrid != null)
                    await DriveManager.Instance.startDownloadingSStateAsync(
                        l_SaveStateInfo.FilePath, 
                        lProgressBannerGrid);
                               
                var lSaveState = SStates.Instance.readData(l_SaveStateInfo.FilePath, l_SaveStateInfo.Index, l_SaveStateInfo.IsAutosave, l_SaveStateInfo.IsQuicksave);

                lSaveState.IsCloudsave = true;

                lSaveState.IsCloudOnlysave = false;

                lSaveState.CloudSaveDate = l_SaveStateInfo.Date;

                lSaveState.CloudSaveDuration = l_SaveStateInfo.Duration;

                if (_saveStateInfoCollection.Contains(l_SaveStateInfo, new Compare()))
                {
                    _saveStateInfoCollection.Remove(l_SaveStateInfo);
                }

                addSaveStateInfo(lSaveState);

                m_ListSlotIndexes.Remove(l_SaveStateInfo.Index);
            }
        }

        public bool accessPersistItem(object a_Item)
        {
            var l_Grid = a_Item as System.Windows.Controls.Grid;

            var l_SaveStateInfo = l_Grid.DataContext as SaveStateInfo;

            if (l_SaveStateInfo != null)
            {
                return !l_SaveStateInfo.IsCloudOnlysave;
            }

            return true;
        }

        public bool accessLoadItem(object a_Item)
        {
            var l_Grid = a_Item as System.Windows.Controls.Grid;

            var l_SaveStateInfo = l_Grid.DataContext as SaveStateInfo;

            if (l_SaveStateInfo != null)
            {
                return l_SaveStateInfo.IsCloudsave;
            }

            return false;
        }

        public SaveStateInfo AutoSave { get { return m_autoSave; } }

        public void registerItem(object a_Item)
        {
        }

        public ICollectionView Collection
        {
            get { return mCustomerView; }
        }

        public ICollectionView AutoSaveCollection
        {
            get { return mAutoSaveCustomerView; }
        }

        public ICollectionView QuickSaveCollection
        {
            get { return mQuickSaveCustomerView; }
        }
    }
}
