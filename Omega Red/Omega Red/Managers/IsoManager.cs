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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using Omega_Red.Properties;
using Omega_Red.Tools;
using Omega_Red.Util;
using Omega_Red.Models;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using Omega_Red.SocialNetworks.Google;

namespace Omega_Red.Managers
{
    class IsoManager : IManager
    {

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

        public enum IsoType
        {
            ISOTYPE_ILLEGAL = 0,
            ISOTYPE_CD,
            ISOTYPE_DVD,
            ISOTYPE_AUDIO,
            ISOTYPE_DVDDL
        };

        private ICollectionView mCustomerView = null;

        private readonly ObservableCollection<IsoInfo> _isoInfoCollection = new ObservableCollection<IsoInfo>();

        private static IsoManager m_Instance = null;

        public static IsoManager Instance { get { if (m_Instance == null) m_Instance = new IsoManager(); return m_Instance; } }

        public IsoManager()
        {
            mCustomerView = CollectionViewSource.GetDefaultView(_isoInfoCollection);

            PropertyGroupDescription l_groupDescription = new PropertyGroupDescription("GameType");

            mCustomerView.GroupDescriptions.Add(l_groupDescription);

            foreach (var item in _isoInfoCollection)
            {
                if (item.IsCurrent)
                {
                    mCustomerView.MoveCurrentTo(item);

                    break;
                }
            }

            mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;
        }

        void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            var l_selectedIsoInfo = mCustomerView.CurrentItem as IsoInfo;

            if (l_selectedIsoInfo != null)
            {
                selectIsoInfo(l_selectedIsoInfo);
            }
        }
        public void load()
        {
            loadInner();

            foreach (var item in _isoInfoCollection)
            {
                if (item.IsCurrent)
                {
                    mCustomerView.MoveCurrentTo(item);

                    break;
                }
            }
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
                        var l_gameData = GameIndex.Instance.convert(item.DiscSerial);

                        if (l_gameData != null)
                        {
                            item.Title = l_gameData.FriendlyName;
                        }

                        _isoInfoCollection.Add(item);
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

        public IEnumerable<IsoInfo> IsoInfoCollection
        {
            get { return _isoInfoCollection; }
        }

        public void addIsoInfo(IsoInfo a_IsoInfo)
        {
            if (!_isoInfoCollection.Contains(a_IsoInfo, new Compare()))
            {
                _isoInfoCollection.Add(a_IsoInfo);

                IsoManager.Instance.save();
            }
        }

        private void removeIsoInfo(IsoInfo a_IsoInfo)
        {
            if (_isoInfoCollection.Contains(a_IsoInfo, new Compare()))
            {
                _isoInfoCollection.Remove(a_IsoInfo);

                if (a_IsoInfo.IsCurrent)
                    PCSX2Controller.Instance.IsoInfo = null;

                IsoManager.Instance.save();
            }
        }

        private void selectIsoInfo(IsoInfo a_IsoInfo)
        {
            if (_isoInfoCollection.Contains(a_IsoInfo, new Compare()))
            {
                foreach (var item in _isoInfoCollection)
                {
                    item.IsCurrent = false;
                }

                a_IsoInfo.IsCurrent = true;

                PCSX2Controller.Instance.IsoInfo = a_IsoInfo;

                IsoManager.Instance.save();
            }
        }

        public static IsoInfo getGameDiscInfo(string aFilePath)
        {
            IsoInfo l_result = null;
            
            do
            {
                var l_module = PCSX2ModuleManager.Instance.getModule(Omega_Red.Tools.PCSX2ModuleManager.ModuleType.CDVD);

                string l_commandResult = "";

                if (l_module != null)
                {
                    XmlDocument l_XmlDocument = new XmlDocument();

                    XmlNode ldocNode = l_XmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                    l_XmlDocument.AppendChild(ldocNode);

                    XmlNode rootNode = l_XmlDocument.CreateElement("Commands");


                    XmlNode l_PropertyNode = l_XmlDocument.CreateElement("Check");

                    var l_Atrr = l_XmlDocument.CreateAttribute("FilePath");

                    l_Atrr.Value = aFilePath;

                    l_PropertyNode.Attributes.Append(l_Atrr);

                    rootNode.AppendChild(l_PropertyNode);


                    l_PropertyNode = l_XmlDocument.CreateElement("GetDiscSerial");

                    l_Atrr = l_XmlDocument.CreateAttribute("FilePath");

                    l_Atrr.Value = aFilePath;

                    l_PropertyNode.Attributes.Append(l_Atrr);

                    rootNode.AppendChild(l_PropertyNode);

                    l_XmlDocument.AppendChild(rootNode);

                    using (var stringWriter = new StringWriter())
                    using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                    {
                        l_XmlDocument.WriteTo(xmlTextWriter);

                        xmlTextWriter.Flush();

                        l_module.execute(stringWriter.GetStringBuilder().ToString(), out l_commandResult);
                    }

                }
                
                if (!string.IsNullOrEmpty(l_commandResult))
                {
                    XmlDocument l_XmlDocument = new XmlDocument();

                    l_XmlDocument.LoadXml(l_commandResult);

                    if (l_XmlDocument.DocumentElement != null)
                    {

                        var l_bResult = false;

                        IsoType l_isoType = IsoType.ISOTYPE_ILLEGAL;

                        var l_CheckNode = l_XmlDocument.DocumentElement.SelectSingleNode("Result[@Command='Check']");

                        if (l_CheckNode != null)
                        {
                            var l_StateNode = l_CheckNode.SelectSingleNode("@State");

                            var l_IsoTypeNode = l_CheckNode.SelectSingleNode("@IsoType");

                            if (l_StateNode != null)
                            {
                                Boolean.TryParse(l_StateNode.Value, out l_bResult);
                            }

                            if (l_IsoTypeNode != null)
                            {
                                Enum.TryParse<IsoType>(l_IsoTypeNode.Value, true, out l_isoType);
                            }
                        }
                        
                        l_CheckNode = l_XmlDocument.DocumentElement.SelectSingleNode("Result[@Command='GetDiscSerial']");

                        if (l_CheckNode != null 
                            && l_bResult
                            && (l_isoType != IsoType.ISOTYPE_ILLEGAL)
                            && (l_isoType != IsoType.ISOTYPE_AUDIO))
                        {
                            IsoInfo local_result = new IsoInfo();

                            local_result.IsoType = l_isoType.ToString().Replace("ISOTYPE_", "");

                            local_result.FilePath = aFilePath;

                            var l_GameDiscType = l_CheckNode.SelectSingleNode("@GameDiscType");

                            var l_DiscSerial = l_CheckNode.SelectSingleNode("@DiscSerial");

                            var l_DiscRegionType = l_CheckNode.SelectSingleNode("@DiscRegionType");

                            var l_SoftwareVersion = l_CheckNode.SelectSingleNode("@SoftwareVersion");

                            var l_ElfCRC = l_CheckNode.SelectSingleNode("@ElfCRC");                            

                            if (l_GameDiscType == null
                                || l_DiscSerial == null
                                || l_DiscRegionType == null
                                || l_SoftwareVersion == null
                                || l_ElfCRC == null)
                                break;

                            if (l_GameDiscType != null && !string.IsNullOrWhiteSpace(l_GameDiscType.Value))
                            {
                                local_result.GameDiscType = l_GameDiscType.Value;

                                local_result.GameType = GameType.Unknown;

                                if (l_GameDiscType.Value.Contains("PS1"))
                                    local_result.GameType = GameType.PS1;
                                else if (l_GameDiscType.Value.Contains("PS2"))
                                    local_result.GameType = GameType.PS2;
                            }

                            if (l_DiscSerial != null && l_DiscSerial.Value != null)
                            {
                                local_result.DiscSerial = l_DiscSerial.Value.ToUpper();

                                var l_gameData = GameIndex.Instance.convert(local_result.DiscSerial);

                                if (l_gameData != null)
                                {
                                    local_result.Title = l_gameData.FriendlyName;
                                }
                                else
                                {
                                    FileInfo l_FileInfo = new FileInfo(aFilePath);

                                    var l_name = l_FileInfo.Name;

                                    if (!string.IsNullOrWhiteSpace(l_FileInfo.Extension))
                                        l_name = l_name.Replace(l_FileInfo.Extension, "");

                                    local_result.Title = l_name;
                                }
                            }

                            if (l_DiscRegionType != null)
                            {
                                local_result.DiscRegionType = l_DiscRegionType.Value;
                            }

                            if (l_SoftwareVersion != null)
                            {
                                local_result.SoftwareVersion = l_SoftwareVersion.Value;
                            }

                            if (l_ElfCRC != null)
                            {
                                uint l_value = 0;

                                if (uint.TryParse(l_ElfCRC.Value, out l_value))
                                {
                                    local_result.ElfCRC = l_value;
                                }
                            }                                                       

                            l_result = local_result;
                        }
                    }
                }

            } while (false);

            return l_result;
        }

        private void addIsoInfo()
        {
            OpenFileDialog l_OpenFileDialog = new OpenFileDialog();

            l_OpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            l_OpenFileDialog.Filter = "ISO File|*.iso" +
                                      "|BIN File|*.bin";

            bool l_result = (bool)l_OpenFileDialog.ShowDialog();

            if (l_result)
            {
                var l_IsoInfo = IsoManager.getGameDiscInfo(l_OpenFileDialog.FileName);

                if (l_IsoInfo != null && l_IsoInfo.GameDiscType != "Invalid or unknown disc.")
                {
                    IsoManager.Instance.addIsoInfo(l_IsoInfo);
                }
                else
                {
                    var l_info = PPSSPPControl.Instance.getGameInfo(l_OpenFileDialog.FileName);

                    if(!string.IsNullOrWhiteSpace(l_info.Item1) && !string.IsNullOrWhiteSpace(l_info.Item2))
                    {
                        l_IsoInfo.Title = l_info.Item1;

                        l_IsoInfo.GameDiscType = "PSP Disc";

                        l_IsoInfo.GameType = GameType.PSP;

                        l_IsoInfo.DiscSerial = l_info.Item2.ToUpper();

                        l_IsoInfo.DiscRegionType = "PAL";

                        l_IsoInfo.SoftwareVersion = "1.0";

                        IsoManager.Instance.addIsoInfo(l_IsoInfo);
                    }
                }
            }
        }

        public void removeItem(object a_Item)
        {
            var l_IsoInfo = a_Item as IsoInfo;

            if (l_IsoInfo != null)
            {
                removeIsoInfo(l_IsoInfo);
            }  
        }

        public void createItem()
        {
            addIsoInfo();
        }



        public async void persistItemAsync(object a_Item)
        {
            //var l_Grid = a_Item as System.Windows.Controls.Grid;

            //var l_IsoInfo = l_Grid.DataContext as IsoInfo;

            //if (l_IsoInfo != null)
            //{
            //    string lDescription = "DiscSerial=" + l_IsoInfo.DiscSerial;

            //    var lProgressBannerGrid = l_Grid.FindName("mProgressBannerBorder") as System.Windows.FrameworkElement;

            //    if (lProgressBannerGrid != null)
            //        await DriveManager.Instance.startUploadingDiscAsync(l_IsoInfo.FilePath, lProgressBannerGrid, lDescription);

            //    l_Grid.DataContext = null;

            //    l_IsoInfo.IsCloudsave = true;

            //    l_Grid.DataContext = l_IsoInfo;
            //}
        }

        public async void loadItemAsync(object a_Item)
        {
            if (string.IsNullOrEmpty(Settings.Default.SlotFolder))
                return;

            var l_Grid = a_Item as System.Windows.Controls.Grid;

            var l_IsoInfo = l_Grid.DataContext as IsoInfo;

            if (l_IsoInfo != null)
            {
                var lProgressBannerGrid = l_Grid.FindName("mProgressBannerBorder") as System.Windows.FrameworkElement;

                if (lProgressBannerGrid != null)
                    await DriveManager.Instance.startDownloadingDiscAsync(
                        l_IsoInfo.FilePath,
                        lProgressBannerGrid);

                l_IsoInfo = IsoManager.getGameDiscInfo(l_IsoInfo.FilePath);

                if (l_IsoInfo != null && l_IsoInfo.GameDiscType != "Invalid or unknown disc.")
                {
                    IsoManager.Instance.addIsoInfo(l_IsoInfo);

                    l_IsoInfo.IsCloudsave = true;

                    l_IsoInfo.IsCloudOnlysave = false;
                }
                else
                {
                    var l_info = PPSSPPControl.Instance.getGameInfo(l_IsoInfo.FilePath);

                    if (!string.IsNullOrEmpty(l_info.Item1) && !string.IsNullOrWhiteSpace(l_info.Item2))
                    {
                        l_IsoInfo.Title = l_info.Item1;

                        l_IsoInfo.GameDiscType = "PSP Disc";

                        l_IsoInfo.GameType = GameType.PSP;

                        l_IsoInfo.DiscSerial = l_info.Item2.ToUpper();

                        l_IsoInfo.DiscRegionType = "PAL";

                        l_IsoInfo.SoftwareVersion = "1.0";

                        l_IsoInfo.IsCloudsave = true;

                        l_IsoInfo.IsCloudOnlysave = false;

                        IsoManager.Instance.addIsoInfo(l_IsoInfo);
                    }
                }



                //var lSaveState = IsoManager.Instance.addIsoInfo.readData(l_IsoInfo.FilePath, l_IsoInfo.Index, l_IsoInfo.IsAutosave, l_IsoInfo.IsQuicksave);



                //if (_saveStateInfoCollection.Contains(l_IsoInfo, new Compare()))
                //{
                //    _saveStateInfoCollection.Remove(l_IsoInfo);
                //}

                //addSaveStateInfo(lSaveState);

                //m_ListSlotIndexes.Remove(l_IsoInfo.Index);
            }
        }

        public bool accessPersistItem(object a_Item)
        {
            var l_Grid = a_Item as System.Windows.Controls.Grid;

            var l_IsoInfo = l_Grid.DataContext as IsoInfo;

            if (l_IsoInfo != null)
            {
                return !l_IsoInfo.IsCloudOnlysave;
            }

            return true;
        }

        public bool accessLoadItem(object a_Item)
        {
            var l_Grid = a_Item as System.Windows.Controls.Grid;

            var l_IsoInfo = l_Grid.DataContext as IsoInfo;

            if (l_IsoInfo != null)
            {
                return l_IsoInfo.IsCloudsave;
            }

            return false;
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
