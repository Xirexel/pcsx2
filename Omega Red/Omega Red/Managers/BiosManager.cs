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

using Omega_Red.Emulators;
using Omega_Red.Models;
using Omega_Red.Properties;
using Omega_Red.SocialNetworks.Google;
using Omega_Red.Tools;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Omega_Red.Managers
{
    class BiosManager: IManager
    {

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

        private bool m_GoogleDriveAccess = false;

        public event Action<string> ShowInfoEvent;

        public event Action<string> ShowErrorEvent;

        private ICollectionView mCustomerView = null;

        private readonly ObservableCollection<BiosInfo> _biosInfoCollection = new ObservableCollection<BiosInfo>();

        private static BiosManager m_Instance = null;

        public static BiosManager Instance { get { if (m_Instance == null) m_Instance = new BiosManager(); return m_Instance; } }

        private BiosManager() {
            
            mCustomerView = CollectionViewSource.GetDefaultView(_biosInfoCollection);

            PropertyGroupDescription l_GameTypeGroupDescription = new PropertyGroupDescription("GameType");

            mCustomerView.GroupDescriptions.Add(l_GameTypeGroupDescription);

            PropertyGroupDescription l_ArchiveFileGroupDescription = new PropertyGroupDescription("ContainerFile");

            mCustomerView.GroupDescriptions.Add(l_ArchiveFileGroupDescription);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                load();

                foreach (var item in _biosInfoCollection)
                {
                    if (item.IsCurrent)
                    {
                        mCustomerView.MoveCurrentTo(item);

                        break;
                    }
                }
            });

            mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;
                       
            GoogleAccountManager.Instance.mEnableStateEvent += (obj) =>
            {
                m_GoogleDriveAccess = obj;

                if (Emul.Instance.IsoInfo != null)
                    load();
            };
        }


        void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            var l_BiosInfo = mCustomerView.CurrentItem as BiosInfo;

            if (l_BiosInfo != null)
                selectBiosInfo(l_BiosInfo);
        }

        private void showErrorEvent(string a_message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                if (ShowErrorEvent != null)
                    ShowErrorEvent(a_message);
            });
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
        public void load()
        {
            loadInner();
        }

        private void loadInner()
        {
            if (string.IsNullOrEmpty(Settings.Default.BiosInfoCollection))
                return;

            if (Emul.Instance.IsoInfo != null &&
                Emul.Instance.IsoInfo.GameType != GameType.PS2)
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
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate()
                {
                    a_BiosInfo.ContainerFile = new ContainerFile();

                    var l_splits = a_BiosInfo.FilePath.Split(new char[] { '|'}, StringSplitOptions.RemoveEmptyEntries);

                    if(l_splits != null && l_splits.Length == 2)
                    {
                        l_splits = l_splits[0].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                        if (l_splits != null && l_splits.Length >= 1)
                            a_BiosInfo.ContainerFile = new ContainerFile(l_splits[l_splits.Length - 1]);
                    }

                    _biosInfoCollection.Add(a_BiosInfo);

                    save();
                });
            }
        }

        private void removeBiosInfo(BiosInfo a_BiosInfo)
        {
            if (_biosInfoCollection.Contains(a_BiosInfo, new Compare()))
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
                {
                    _biosInfoCollection.Remove(a_BiosInfo);

                    save();
                });

                if (a_BiosInfo.IsCurrent)
                    Emul.Instance.BiosInfo = null;
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

                Emul.Instance.BiosInfo = a_BiosInfo;

                save();
            }
        }

        public void removeItem(object a_Item)
        {
            var l_BiosInfo = a_Item as BiosInfo;

            if(l_BiosInfo != null)
            {
                removeBiosInfo(l_BiosInfo);
            }
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
                                                    CheckSum = Omega_Red.Tools.BiosControl.getBIOSChecksum(l_result),
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

                                LockScreenManager.Instance.displayMessage(l_add_file_path);

                                BiosManager.Instance.addBiosInfo(new Models.BiosInfo()
                                {
                                    Zone = zone,
                                    Version = version,
                                    VersionInt = versionInt,
                                    Data = data,
                                    Build = build,
                                    CheckSum = Omega_Red.Tools.BiosControl.getBIOSChecksum(l_file_path),
                                    FilePath = l_file_path,
                                    NVM = l_NVM,
                                    MEC = l_MEC,
                                    GameType = gameType
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            showErrorEvent(e.Message);
                        }
                    }
                    else
                        showErrorEvent(exc.Message);
                }                

            } while (false);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                LockScreenManager.Instance.hide();
            });
        }

        private void executeLoading(string a_file_path)
        {
            ParameterizedThreadStart loadModulesStart = new ParameterizedThreadStart(readBios);

            Thread loadModulesThread = new Thread(loadModulesStart);

            LockScreenManager.Instance.show();

            loadModulesThread.Start(a_file_path);
        }

        public void createItem()
        {
            addBiosInfo();
        }

        public void persistItemAsync(object a_Item)
        {
        }

        public void loadItemAsync(object a_Item)
        {
        }

        public bool accessPersistItem(object a_Item)
        {
            return true;
        }

        public bool accessLoadItem(object a_Item)
        {
            return true;
        }

        public void registerItem(object a_Item)
        {
        }

        public System.ComponentModel.ICollectionView Collection
        {
            get { return mCustomerView; }
        }
    }
}
