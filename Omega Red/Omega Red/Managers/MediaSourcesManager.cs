using Omega_Red.Models;
using Omega_Red.Panels;
using Omega_Red.Properties;
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
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Omega_Red.Managers
{
    class MediaSourcesManager : IManager
    {
        public event Action<bool> ChangeMediaSourcesAccessEvent;

        public event Action<bool> ChangeMediaSourceCheckEvent;

        public event Action<bool> ChangeVideoSourceAccessEvent;

        public event Action<bool> ChangeAudioSourceAccessEvent;


        private ICollectionView mCustomerView = null;

        private readonly ObservableCollection<MediaSourceInfo> _mediaSourceInfoCollection = new ObservableCollection<MediaSourceInfo>();

        private static MediaSourcesManager m_Instance = null;

        public static MediaSourcesManager Instance { get { if (m_Instance == null) m_Instance = new MediaSourcesManager(); return m_Instance; } }
        
        private MediaSourcesManager()
        {
            mCustomerView = CollectionViewSource.GetDefaultView(_mediaSourceInfoCollection);                        
        }

        public void openSources()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                if (ChangeMediaSourcesAccessEvent != null)
                    ChangeMediaSourcesAccessEvent(true);

                checkAvalableMixers();
            });
        }

        public void closeSources()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                if (ChangeMediaSourcesAccessEvent != null)
                    ChangeMediaSourcesAccessEvent(false);

                foreach (var litem in _mediaSourceInfoCollection)
                {
                    if (litem.IsSelected)
                        launchSourceAsync(litem);
                }

                if (ChangeMediaSourceCheckEvent != null)
                    ChangeMediaSourceCheckEvent(false);

                checkAvalableMixers();

                save();
            });
        }

        public DisplayControl DisplayControl { get; set; }

        public void load(Action method)
        {
            _mediaSourceInfoCollection.Clear();

            MediaRecorderManager.Instance.getCollectionOfSources((aXMLsources)=> {
                               
                if (!string.IsNullOrWhiteSpace(aXMLsources))
                {

                    XmlDocument doc = new XmlDocument();

                    doc.LoadXml(aXMLsources.Replace("MFVideoFormat_", "").Replace("MFAudioFormat_", ""));

                    if (doc.DocumentElement != null)
                    {

                        var lsources = doc.DocumentElement.SelectNodes("//*[Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_HW_SOURCE']/SingleValue[@Value='Hardware device']]");

                        if (lsources != null)
                        {
                            foreach (var litem in lsources)
                            {
                                var lSourceNode = litem as XmlNode;

                                if (lSourceNode != null)
                                {
                                    var lAttr = lSourceNode.SelectSingleNode("Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME']/SingleValue/@Value");

                                    if (lAttr != null)
                                    {
                                        var lMediaTypes = lSourceNode.SelectSingleNode("PresentationDescriptor/StreamDescriptor/MediaTypes");

                                        addSource(lAttr.Value, MediaSourceType.Audio, lSourceNode.SelectSingleNode("Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK']/SingleValue/@Value").Value, lMediaTypes, false);
                                    }
                                }
                            }
                        }

                        lsources = doc.DocumentElement.SelectNodes("//*[Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_MEDIA_TYPE']/Value.ValueParts/ValuePart[@Value='MFMediaType_Video'] and Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_HW_SOURCE']/SingleValue[@Value='Hardware device']]");

                        if (lsources != null)
                        {
                            foreach (var litem in lsources)
                            {
                                var lSourceNode = litem as XmlNode;

                                if (lSourceNode != null)
                                {
                                    var lAttr = lSourceNode.SelectSingleNode("Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME']/SingleValue/@Value");

                                    if (lAttr != null)
                                    {
                                        var lMediaTypes = lSourceNode.SelectSingleNode("PresentationDescriptor/StreamDescriptor/MediaTypes");

                                        addSource(lAttr.Value, MediaSourceType.Video, lSourceNode.SelectSingleNode("Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK']/SingleValue/@Value").Value, lMediaTypes, false);
                                    }
                                }
                            }
                        }
                    }
                    
                    load();
                }
                
                System.Threading.Thread.Sleep(5000);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    if (method != null)
                        method();
                });
            });

        }

        private void addSource(
            string aName, 
            MediaSourceType aType, 
            string aSymbolicLink, 
            XmlNode aMediaTypes, 
            bool a_persist, 
            System.Windows.Media.ImageSource aImageSource = null,
            MediaSourceInfo a_OriginalMediaSourceInfo = null)
        {
            MediaSourceInfo l_MediaSourceInfo = new MediaSourceInfo();

            l_MediaSourceInfo.Name = aName;

            l_MediaSourceInfo.Type = aType;

            l_MediaSourceInfo.SymbolicLink = aSymbolicLink;

            l_MediaSourceInfo.MediaTypes = aMediaTypes;

            l_MediaSourceInfo.SelectedMediaTypeIndex = 0;

            l_MediaSourceInfo.ImageSource = aImageSource;

            l_MediaSourceInfo.Variable = 1.0;

            switch (aType)
            {
                case MediaSourceType.Video:
                    break;
                case MediaSourceType.Image:
                    break;
                case MediaSourceType.Audio:
                    l_MediaSourceInfo.Variable = 0.5;
                    break;
                default:
                    break;
            }

            if(a_OriginalMediaSourceInfo == null)
            {
                l_MediaSourceInfo.LeftProp = 0;

                l_MediaSourceInfo.RightProp = 0.33;

                l_MediaSourceInfo.TopProp = 0;

                l_MediaSourceInfo.BottomProp = 0.33;
            }
            else
            {
                l_MediaSourceInfo.LeftProp = a_OriginalMediaSourceInfo.LeftProp;

                l_MediaSourceInfo.RightProp = a_OriginalMediaSourceInfo.RightProp;

                l_MediaSourceInfo.TopProp = a_OriginalMediaSourceInfo.TopProp;

                l_MediaSourceInfo.BottomProp = a_OriginalMediaSourceInfo.BottomProp;

                l_MediaSourceInfo.Variable = a_OriginalMediaSourceInfo.Variable;
            }
                       
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                _mediaSourceInfoCollection.Add(l_MediaSourceInfo);
            });

            if(a_persist)
                save();
        }
        
        public System.ComponentModel.ICollectionView Collection
        {
            get { return mCustomerView; }
        }

        public void removeItem(object a_Item)
        {
            var l_MediaSourceInfo = a_Item as MediaSourceInfo;

            if (l_MediaSourceInfo == null)
                return;

            _mediaSourceInfoCollection.Remove(l_MediaSourceInfo);

            if (l_MediaSourceInfo.IsSelected)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    launchSourceAsync(l_MediaSourceInfo);
                });
            }
        }

        public void persistItemAsync(object a_Item)
        {
        }

        public void loadItemAsync(object a_Item)
        {
        }

        public bool accessPersistItem(object a_Item)
        {
            return false;
        }

        public bool accessLoadItem(object a_Item)
        {
            return false;
        }

        public void registerItem(object a_Item)
        {
        }

        public void createItem()
        {
            var lOpenFileDialog = new Microsoft.Win32.OpenFileDialog();

            lOpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            lOpenFileDialog.Filter = "Image files (*.png, *.bmp, *.gif)|*.png;*.bmp;*.gif";

            bool l_result = (bool)lOpenFileDialog.ShowDialog();

            if (l_result &&
                File.Exists(lOpenFileDialog.FileName))
            {
                addImage(lOpenFileDialog.FileName);
            }
        }

        private void addImage(string a_FileName, MediaSourceInfo a_OriginalMediaSourceInfo = null)
        {

            using (var lImageStream = File.Open(a_FileName, FileMode.Open))
            {
                var lTempBitmap = BitmapDecoder.Create(lImageStream, BitmapCreateOptions.None, BitmapCacheOption.None);

                if (lTempBitmap == null)
                    return;

                if (lTempBitmap.Frames == null || lTempBitmap.Frames.Count == 0)
                    return;

                BitmapSource lbitmapSource = lTempBitmap.Frames[0];

                if (lbitmapSource == null)
                    return;
            }

            var lBitmap = new BitmapImage();

            using (var l_ImageStream = File.Open(a_FileName, FileMode.Open))
            {
                lBitmap.BeginInit();
                lBitmap.DecodePixelWidth = 100;
                lBitmap.CacheOption = BitmapCacheOption.OnLoad;
                lBitmap.StreamSource = l_ImageStream;
                lBitmap.EndInit();
                lBitmap.Freeze();
            }

            addSource(
                System.IO.Path.GetFileName(a_FileName),
                MediaSourceType.Image,
                a_FileName,
                null,
                true,
                lBitmap,
                a_OriginalMediaSourceInfo);
        }

        public async void launchSourceAsync(MediaSourceInfo a_MediaSourceInfo)
        {
            if(!a_MediaSourceInfo.IsSelected)
            {
                IntPtr l_RenderTarget = IntPtr.Zero;

                Panels.VideoPanel l_VideoPanel = null;

                switch (a_MediaSourceInfo.Type)
                {
                    case MediaSourceType.Video:
                        {

                            var lNode = a_MediaSourceInfo.SelectedMediaType.SelectSingleNode("MediaTypeItem[@Name='MF_MT_FRAME_SIZE']/Value.ValueParts/ValuePart[1]/@Value");

                            if (lNode == null)
                                return;

                            uint lVideoWidth = 0;

                            if (!uint.TryParse(lNode.Value, out lVideoWidth))
                            {
                                return;
                            }

                            lNode = a_MediaSourceInfo.SelectedMediaType.SelectSingleNode("MediaTypeItem[@Name='MF_MT_FRAME_SIZE']/Value.ValueParts/ValuePart[2]/@Value");

                            if (lNode == null)
                                return;

                            uint lVideoHeight = 0;

                            if (!uint.TryParse(lNode.Value, out lVideoHeight))
                            {
                                return;
                            }

                            
                            uint lWidth = Panels.VideoPanel.WIDTH / 3;

                            uint lHeight = (lWidth * lVideoHeight) / lVideoWidth;


                            l_VideoPanel = new Panels.VideoPanel(lWidth, lHeight, a_MediaSourceInfo.SymbolicLink);

                            l_RenderTarget = l_VideoPanel.SharedHandle;
                        }
                        break;
                    case MediaSourceType.Image:
                        {
                            using (var mImageStream = File.Open(a_MediaSourceInfo.SymbolicLink, FileMode.Open))
                            {
                                var lBitmap = BitmapDecoder.Create(mImageStream, BitmapCreateOptions.None, BitmapCacheOption.None);

                                if (lBitmap == null)
                                    return;

                                if (lBitmap.Frames == null || lBitmap.Frames.Count == 0)
                                    return;

                                BitmapSource lbitmapSource = lBitmap.Frames[0];

                                if (lbitmapSource == null)
                                    return;

                                uint lWidth = Panels.VideoPanel.WIDTH / 3;

                                uint lHeight = (lWidth * (uint)lbitmapSource.PixelHeight) / (uint)lbitmapSource.PixelWidth;
                                

                                l_VideoPanel = new Panels.VideoPanel(lWidth, lHeight, a_MediaSourceInfo.SymbolicLink);

                                l_RenderTarget = l_VideoPanel.SharedHandle;
                            }
                        }
                        break;
                    case MediaSourceType.Audio:
                        break;
                    default:
                        break;
                }

                a_MediaSourceInfo.IsSelected = await MediaRecorderManager.Instance.addSource(a_MediaSourceInfo.SymbolicLink, (uint)a_MediaSourceInfo.SelectedMediaTypeIndex, l_RenderTarget);

                if(a_MediaSourceInfo.IsSelected)
                {
                    if (DisplayControl != null && l_RenderTarget != IntPtr.Zero)
                        DisplayControl.addSource(new MediaSourcePanel(l_VideoPanel, a_MediaSourceInfo));

                    if(a_MediaSourceInfo.Type == MediaSourceType.Audio)
                    {
                        Managers.MediaRecorderManager.Instance.setRelativeVolume(
                        a_MediaSourceInfo.SymbolicLink,
                        (float)a_MediaSourceInfo.Variable);
                    }
                }
            }
            else
            {
                await MediaRecorderManager.Instance.removeSource(a_MediaSourceInfo.SymbolicLink);

                a_MediaSourceInfo.IsSelected = false;

                if (DisplayControl != null)
                    DisplayControl.removeSource(a_MediaSourceInfo.SymbolicLink);
            }
            
            checkAvalableMixers();
        }

        private void checkAvalableMixers()
        {
            if (ChangeVideoSourceAccessEvent != null)
                ChangeVideoSourceAccessEvent(MediaRecorderManager.Instance.currentAvalableVideoMixers() > 0);

            if (ChangeAudioSourceAccessEvent != null)
                ChangeAudioSourceAccessEvent(MediaRecorderManager.Instance.currentAvalableAudioMixers() > 0);            
        }
        
        public void changeVariable(object a_Item)
        {
            var l_MediaSourceInfo = a_Item as MediaSourceInfo;

            if (l_MediaSourceInfo == null)
                return;

            switch (l_MediaSourceInfo.Type)
            {
                case MediaSourceType.Video:
                    Managers.MediaRecorderManager.Instance.setOpacity(
                        l_MediaSourceInfo.SymbolicLink,
                        (float)l_MediaSourceInfo.Variable);
                    if (DisplayControl != null)
                        DisplayControl.setOpacity(
                        l_MediaSourceInfo.SymbolicLink,
                        (float)l_MediaSourceInfo.Variable);
                    break;
                case MediaSourceType.Image:
                    Managers.MediaRecorderManager.Instance.setOpacity(
                        l_MediaSourceInfo.SymbolicLink,
                        (float)l_MediaSourceInfo.Variable);
                    if (DisplayControl != null)
                        DisplayControl.setOpacity(
                        l_MediaSourceInfo.SymbolicLink,
                        (float)l_MediaSourceInfo.Variable);
                    break;
                case MediaSourceType.Audio:
                    Managers.MediaRecorderManager.Instance.setRelativeVolume(
                        l_MediaSourceInfo.SymbolicLink,
                        (float)l_MediaSourceInfo.Variable);
                    break;
                default:
                    break;
            }
        }
        
        public void save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<MediaSourceInfo>));

            MemoryStream stream = new MemoryStream();

            ser.Serialize(stream, _mediaSourceInfoCollection);

            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);

            Settings.Default.MediaSourceInfoCollection = reader.ReadToEnd();

            Settings.Default.Save();

            App.saveCopy();
        }

        public void load()
        {
            if (string.IsNullOrEmpty(Settings.Default.MediaSourceInfoCollection))
                return;

            XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<MediaSourceInfo>));

            XmlReader xmlReader = XmlReader.Create(new StringReader(Settings.Default.MediaSourceInfoCollection));

            if (ser.CanDeserialize(xmlReader))
            {
                var l_collection = ser.Deserialize(xmlReader) as ObservableCollection<MediaSourceInfo>;

                if (l_collection != null)
                {
                    foreach (var item in l_collection)
                    {
                        if (item.Type == MediaSourceType.Video ||
                           item.Type == MediaSourceType.Audio)
                        {
                            var lSource = _mediaSourceInfoCollection.FirstOrDefault(p => p.SymbolicLink == item.SymbolicLink);

                            if(lSource != null)
                            {
                                lSource.BottomProp = item.BottomProp;
                                lSource.TopProp = item.TopProp;
                                lSource.LeftProp = item.LeftProp;
                                lSource.RightProp = item.RightProp;
                                lSource.Variable = item.Variable;
                            }
                        }

                        if (item.Type == MediaSourceType.Image)
                        {
                            if (File.Exists(item.SymbolicLink))
                            {
                                addImage(item.SymbolicLink, item);
                            }
                        }
                    }
                }
            }
        }
    }
}
