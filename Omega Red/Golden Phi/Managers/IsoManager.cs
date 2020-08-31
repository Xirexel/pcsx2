using Golden_Phi.Emul;
using Golden_Phi.Models;
using Golden_Phi.Properties;
using Golden_Phi.Tools;
using Microsoft.Win32;
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
    class IsoManager : IManager
    {

        public enum IsoType
        {
            ISOTYPE_ILLEGAL = 0,
            ISOTYPE_CD,
            ISOTYPE_DVD,
            ISOTYPE_AUDIO,
            ISOTYPE_DVDDL
        };

        class Compare : IEqualityComparer<IsoInfo>
        {
            public bool Equals(IsoInfo x, IsoInfo y)
            {
                if (x.DiscSerial == y.DiscSerial)
                {
                    return true;
                }
                else { return false; }
            }
            public int GetHashCode(IsoInfo codeh)
            {
                return 0;
            }

        }

        private const int c_maxRecent = 5;

        private int m_current_count = 0;

        private ICollectionView mCustomerView = null;

        private readonly ObservableCollection<IsoInfo> _isoInfoCollection = new ObservableCollection<IsoInfo>();




        private ICollectionView mRecentIsoCustomerView = null;

        private CollectionViewSource mRecentIsoCollectionViewSource = new CollectionViewSource();


        public event Action<bool> ShowChooseIsoEvent;


        private static IsoManager m_Instance = null;

        public static IsoManager Instance { get { if (m_Instance == null) m_Instance = new IsoManager(); return m_Instance; } }

        private IsoManager()
        {
            View = App.getResource("IsoView");

            mCustomerView = CollectionViewSource.GetDefaultView(_isoInfoCollection);

            mCustomerView.SortDescriptions.Add(
            new SortDescription("LastLaunchTime", ListSortDirection.Descending));
            

            mRecentIsoCollectionViewSource.Source = _isoInfoCollection;

            mRecentIsoCollectionViewSource.SortDescriptions.Add(
            new SortDescription("LastLaunchTime", ListSortDirection.Descending));


            mRecentIsoCollectionViewSource.Filter += (s, e) =>
            {
                e.Accepted = ++m_current_count <= c_maxRecent;
            };

            mRecentIsoCustomerView = mRecentIsoCollectionViewSource.View;

            mRecentIsoCustomerView.CurrentChanged += mRecentIsoCustomerView_CurrentChanged;

            mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;
        }

        void mRecentIsoCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            var l_selectedIsoInfo = mRecentIsoCustomerView.CurrentItem as IsoInfo;

            if (l_selectedIsoInfo != null)
            {
                selectIsoInfo(l_selectedIsoInfo);
            }
        }

        void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            var l_selectedIsoInfo = mCustomerView.CurrentItem as IsoInfo;

            if (l_selectedIsoInfo != null)
            {
                _isoInfoCollection.Remove(l_selectedIsoInfo);

                int l_insertIndex = 0;

                l_selectedIsoInfo.LastLaunchTime = DateTime.Now;

                if (_isoInfoCollection.Count > 0)
                    if (_isoInfoCollection[0].ActiveStateImage != null)
                    {
                        l_insertIndex = 1;

                        _isoInfoCollection[0].LastLaunchTime = DateTime.Now;
                    }

                _isoInfoCollection.Insert(l_insertIndex, l_selectedIsoInfo);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    Emul.Emul.Instance.playActiveState();
                });

                m_current_count = 0;

                mCustomerView.Refresh();

                mRecentIsoCustomerView.Refresh();

                mRecentIsoCustomerView.MoveCurrentToPosition(0);

                CloseChooseIso();
            }
        }

        public void selectIsoInfo(IsoInfo a_IsoInfo)
        {
            if (_isoInfoCollection.Contains(a_IsoInfo, new Compare()))
            {
                foreach (var item in _isoInfoCollection)
                {
                    item.IsCurrent = false;
                }

                a_IsoInfo.IsCurrent = true;

                mRecentIsoCustomerView.MoveCurrentTo(a_IsoInfo);
            }
        }

        public void refresh(IsoInfo a_IsoInfo)
        {
            if (_isoInfoCollection.Contains(a_IsoInfo, new Compare()))
            {
                foreach (var item in _isoInfoCollection)
                {
                    item.IsCurrent = false;
                }
                
                a_IsoInfo.LastLaunchTime = DateTime.Now;

                a_IsoInfo.IsCurrent = true;

                //m_current_count = 0;

                //mCustomerView.Refresh();

                m_current_count = 0;

                mRecentIsoCustomerView.Refresh();

                m_current_count = 0;
            }
        }


        public void load()
        {
            loadInner();
        }

        public void loadInner()
        {
            if (string.IsNullOrEmpty(Settings.Default.IsoInfoCollection))
                return;

            XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<IsoInfo>));

            XmlReader xmlReader = XmlReader.Create(new StringReader(Settings.Default.IsoInfoCollection));

            if (ser.CanDeserialize(xmlReader))
            {
                var l_collection = ser.Deserialize(xmlReader) as ObservableCollection<IsoInfo>;

                if (l_collection != null)
                {
                    foreach (var item in l_collection)
                    {
                        if(File.Exists(item.FilePath))
                        {
                            var l_gameData = GameIndex.Instance.convert(item.DiscSerial);

                            if (l_gameData != null)
                            {
                                item.Title = l_gameData.FriendlyName;
                            }

                            _isoInfoCollection.Add(item);

                            BiosManager.Instance.setBios(item);

                            updateImage(item);
                        }
                    }
                }
            }

        }

        public void save()
        {

            XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<IsoInfo>));

            MemoryStream stream = new MemoryStream();

            ser.Serialize(stream, _isoInfoCollection);

            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);

            Settings.Default.IsoInfoCollection = reader.ReadToEnd();

            Settings.Default.Save();

            App.saveCopy();
        }

        public void clearActiveState()
        {
            foreach (var item in _isoInfoCollection)
            {
                item.ActiveStateImage = null;
            }
        }

        public ICollectionView Collection
        {
            get { return mCustomerView; }
        }

        public ICollectionView RecentIsoCollection
        {
            get { return mRecentIsoCustomerView; }
        }               

        public void updateImage(IsoInfo a_IsoInfo)
        {
            if (a_IsoInfo == null)
                return;

            BiosManager.Instance.checkBios(a_IsoInfo, false);

            var l_disk_serial = a_IsoInfo.DiscSerial;

            var l_bios_check_sum =
            a_IsoInfo.BiosInfo != null ?
            a_IsoInfo.BiosInfo.GameType == a_IsoInfo.GameType ? "_" + a_IsoInfo.BiosInfo.CheckSum.ToString("X8") : ""
            : "";

            var l_AutoSaveStateInfo = SaveStateManager.Instance.getAutoSaveStateInfo(l_disk_serial, l_bios_check_sum, -1);

            if(l_AutoSaveStateInfo != null)
                a_IsoInfo.ImageSource = l_AutoSaveStateInfo.ImageSource;
            else
                a_IsoInfo.ImageSource = null;
        }

        public IsoInfo getIsoInfo(string a_discSerial)
        {
            return _isoInfoCollection.FirstOrDefault((IsoInfo) => IsoInfo.DiscSerial == a_discSerial);
        }

        public void createItem()
        {
            addIsoInfo();
        }

        private void addIsoInfo()
        {
            OpenFileDialog l_OpenFileDialog = new OpenFileDialog();

            l_OpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            l_OpenFileDialog.Filter = "Game Image Files|*.iso;*.bin";

            bool l_result = (bool)l_OpenFileDialog.ShowDialog();
            
            if (l_result)
            {
                var l_IsoInfo = PCSX2Emul.getGameDiscInfo(l_OpenFileDialog.FileName);

                if (l_IsoInfo != null && l_IsoInfo.GameDiscType != "Invalid or unknown disc.")
                    addIsoInfo(l_IsoInfo);
                else
                {
                    l_IsoInfo = PPSSPPEmul.getGameDiscInfo(l_OpenFileDialog.FileName);
                    
                    if (l_IsoInfo != null && l_IsoInfo.GameDiscType != "Invalid or unknown disc.")
                        addIsoInfo(l_IsoInfo);
                }
            }
        }

        public void addIsoInfo(IsoInfo a_IsoInfo)
        {
            if (!_isoInfoCollection.Contains(a_IsoInfo, new Compare()))
            {
                _isoInfoCollection.Add(a_IsoInfo);

                IsoManager.Instance.save();
            }
        }

        public object View { get; private set; }

        public void CloseChooseIso()
        {
            if (ShowChooseIsoEvent != null)
                ShowChooseIsoEvent(false);
        }

        public void removeItem(object a_Item)
        {
        }
    }
}

