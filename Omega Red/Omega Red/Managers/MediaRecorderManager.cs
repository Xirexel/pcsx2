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

using Omega_Red.Capture;
using Omega_Red.Models;
using Omega_Red.Panels;
using Omega_Red.Properties;
using Omega_Red.Tools;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Omega_Red.Managers
{
    enum MediaOutputType
    {
        Capture = 0,
        Stream = 1
    }

    class MediaOutputTypeInfo
    {

        TextBlock mTitle = new TextBlock();

        public MediaOutputTypeInfo(MediaOutputType aValue)
        {
            Value = aValue;
        }

        private void refresh()
        {
            switch (Value)
            {
                case MediaOutputType.Capture:
                    mTitle.SetResourceReference(TextBlock.TextProperty, "MediaOutputTypeCaptureTitle");
                    break;
                case MediaOutputType.Stream:
                    mTitle.SetResourceReference(TextBlock.TextProperty, "MediaOutputTypeStreamTitle");
                    break;
                default:
                    break;
            }
        }

        public MediaOutputType Value { get; private set; }
        
        public object InfoPanel ()
        {            
            if(Value == MediaOutputType.Stream)
            {
                return MediaRecorderManager.Instance.mStreamingCaptureConfigPanel;
            }
            else if (Value == MediaOutputType.Capture)
            {
                return MediaRecorderManager.Instance.mRecordingCaptureConfig;
            }                                            

            return null;
        }

        public override string ToString()
        {
            refresh();

            if (mTitle != null)
                return mTitle.Text;

            return base.ToString();
        }
    }

    class MediaRecorderManager : IManager
    {
        public StreamingCaptureConfig mStreamingCaptureConfigPanel = new StreamingCaptureConfig();

        public RecordingCaptureConfig mRecordingCaptureConfig = new RecordingCaptureConfig();

        

        private ICollectionView mCustomerView = null;

        private readonly ObservableCollection<MediaRecorderInfo> _mediaRecorderInfoCollection = new ObservableCollection<MediaRecorderInfo>();

        private static MediaRecorderManager m_Instance = null;

        public static MediaRecorderManager Instance { get { if (m_Instance == null) m_Instance = new MediaRecorderManager(); return m_Instance; } }

        public event Action<bool> ChangeLockEvent;

        public event Action<bool> RecordingStateEvent;

        public event Action<string> ShowWarningEvent;

        public event Action<string> ShowInfoEvent;

        public string CaptureManagerVersion { get {
                switch (MediaOutputType)
                {
                    case MediaOutputType.Capture:
                        return MediaCapture.Instance.CaptureManagerVersion;
                    case MediaOutputType.Stream:
                        return MediaStream.Instance.CaptureManagerVersion;
                    default:
                        return "";
                }
            }
        }


        private MediaOutputType MediaOutputType { get; set; }

        public bool IsAllowedStreaming { get; set; }

        public void setMediaOutputType(MediaOutputType a_MediaOutputType)
        {
            MediaOutputType = a_MediaOutputType;
                       
            switch (a_MediaOutputType)
            {
                case MediaOutputType.Capture:
                    IsAllowedStreaming = false;
                    break;
                case MediaOutputType.Stream:
                    IsAllowedStreaming = !string.IsNullOrWhiteSpace(Settings.Default.StreamName);
                    MediaStream.Instance.Warning = ShowWarning;
                    break;
                default:
                    break;
            }
        }

        public bool isRecodingAllowed()
        {
            if (MediaOutputType == MediaOutputType.Capture)
                return true;

            if (MediaOutputType == MediaOutputType.Stream)
            {
                return IsAllowedStreaming;
            }

            return false;
        }


        private MediaRecorderManager()
        {
            MediaOutputType = MediaOutputType.Capture;

            if (string.IsNullOrEmpty(Settings.Default.VideosFolder))
                Settings.Default.VideosFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\Omega Red\";

            if (!System.IO.Directory.Exists(Settings.Default.VideosFolder))
            {
                System.IO.Directory.CreateDirectory(Settings.Default.VideosFolder);
            }

            mCustomerView = CollectionViewSource.GetDefaultView(_mediaRecorderInfoCollection);

            mCustomerView.SortDescriptions.Add(
            new SortDescription("DateTime", ListSortDirection.Descending));

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                load();
            });

            mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;
        }

        void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            if(PCSX2Controller.Instance.Status == PCSX2Controller.StatusEnum.Started)
                PCSX2Controller.Instance.PlayPause();

            Managers.MediaRecorderManager.Instance.StartStop(false);

            if (mCustomerView.CurrentItem != null)
                LockScreenManager.Instance.showVideo();
        }

        private void load()
        {
            do
            {
                if (System.IO.Directory.Exists(Settings.Default.VideosFolder))
                {
                    string[] files = System.IO.Directory.GetFiles(Settings.Default.VideosFolder, "*.*");
                    
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

                        if (fi.Exists)
                        {
                            MediaRecorderInfo l_MediaRecorderInfo = new MediaRecorderInfo();
                            
                            l_MediaRecorderInfo.FilePath = fi.FullName;

                            l_MediaRecorderInfo.FileName = fi.Name;

                            l_MediaRecorderInfo.DateTime = fi.LastWriteTime;
                            
                            _mediaRecorderInfoCollection.Add(l_MediaRecorderInfo);
                            
                        }
                    }
                }

            } while (false);
        }
                
        public void StartStop(Object aState)
        {
            var t = new Thread(() =>
            {
                ChangeLockEvent(true);

                try
                {
                    bool l_result = true;

                    if (aState is Boolean)
                    {
                        if ((bool)aState)
                        {
                            switch (MediaOutputType)
                            {
                                case MediaOutputType.Capture:
                                    l_result = MediaCapture.Instance.start();
                                    break;
                                case MediaOutputType.Stream:
                                    MediaStream.Instance.start();
                                    break;
                                default:
                                    break;
                            }

                            if(l_result)
                                MediaSourcesManager.Instance.openSources();
                            else
                            {
                                if (RecordingStateEvent != null)
                                    RecordingStateEvent(false);
                                
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    string l_title = "Recording cannot be started!!!";

                                    try
                                    {
                                        var l_Title = new System.Windows.Controls.TextBlock();

                                        l_Title.SetResourceReference(System.Windows.Controls.TextBlock.TextProperty, "VideoFileRecordingFailedTitle");

                                        l_title = l_Title.Text;
                                    }
                                    finally
                                    {
                                        ShowWarning(l_title);
                                    }
                                });
                            }
                        }
                        else
                        {

                            MediaSourcesManager.Instance.closeSources();

                            switch (MediaOutputType)
                            {
                                case MediaOutputType.Capture:
                                    MediaCapture.Instance.stop();
                                    break;
                                case MediaOutputType.Stream:
                                    MediaStream.Instance.stop();
                                    break;
                                default:
                                    break;
                            }

                            if (RecordingStateEvent != null)
                                RecordingStateEvent(false);
                        }
                    }
                }
                finally
                {
                    ChangeLockEvent(false);
                }
            });

            t.SetApartmentState(ApartmentState.MTA);
            
            t.Start();
        }
               
        public void getCollectionOfSources(Action<string> method)
        {            
            var t = new Thread(() =>
            {
                string linnerresult = "";

                try
                {
                    linnerresult = MediaStream.Instance.getCollectionOfSources();

                    linnerresult = MediaCapture.Instance.getCollectionOfSources();
                }
                finally
                {
                    if (method != null)
                        method(linnerresult);
                }
            });

            t.SetApartmentState(ApartmentState.MTA);

            t.Start();
        }

        public void removeItem(object a_Item)
        {
            var l_MediaRecorderInfo = a_Item as MediaRecorderInfo;

            if (l_MediaRecorderInfo != null)
            {
                _mediaRecorderInfoCollection.Remove(l_MediaRecorderInfo);

                File.Delete(l_MediaRecorderInfo.FilePath);
            }     
        }

        public void createItem(){}

        public void createItem(string aTempFile)
        {
            if (!File.Exists(aTempFile))
            {
                return;
            }

            var l_isoInfo = PCSX2Controller.Instance.IsoInfo;

            if (l_isoInfo == null)
                return;

            var l_gameData = GameIndex.Instance.convert(l_isoInfo.DiscSerial);

            string l_add_file_path = "Video_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "." + MediaCapture.Instance.FileExtention;

            if (l_gameData != null)
            {
                var l_invalidChars = Path.GetInvalidFileNameChars();

                bool l_hasInvalidChar = false;

                foreach (var l_invalidChar in l_invalidChars)
                {
                    l_hasInvalidChar = l_gameData.FriendlyName.Contains(l_invalidChar);

                    if (l_hasInvalidChar)
                        break;
                }

                if(!l_hasInvalidChar)
                    l_add_file_path = l_gameData.FriendlyName + "_" + l_add_file_path;
            }

            if (File.Exists(Settings.Default.VideosFolder + l_add_file_path))
            {
                File.Delete(aTempFile);

                return;
            }
            
            do
            {
                try
                {                    
                    Thread.Sleep(300);

                    File.Move(aTempFile, Settings.Default.VideosFolder + l_add_file_path);

                    break;
                }
                catch (Exception)
                {
                }

            } while (true);
            
            if (!File.Exists(Settings.Default.VideosFolder + l_add_file_path))
                return;

            MediaRecorderInfo l_MediaRecorderInfo = new MediaRecorderInfo();

            l_MediaRecorderInfo.FilePath = Settings.Default.VideosFolder + l_add_file_path;

            l_MediaRecorderInfo.FileName = l_add_file_path;

            l_MediaRecorderInfo.DateTime = DateTime.Now;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                _mediaRecorderInfoCollection.Add(l_MediaRecorderInfo);

                string l_title = l_MediaRecorderInfo.FileName;

                try
                {
                    var l_Title = new System.Windows.Controls.TextBlock();

                    l_Title.SetResourceReference(System.Windows.Controls.TextBlock.TextProperty, "VideoFileRecordedInfoTitle");

                    l_title += l_Title.Text;
                }
                finally
                {
                    ShowInfo(l_title);
                }
            });
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

        public System.ComponentModel.ICollectionView Collection
        {
            get { return mCustomerView; }
        }

        public async Task<bool> addSource(string a_SymbolicLink, uint a_MediaTypeIndex, IntPtr a_RenderTarget)
        {
            bool lresult = await Task.Run(() =>
            {
                bool linnerresult = false;

                AutoResetEvent lBlockEvent = new AutoResetEvent(false);

                var t = new Thread(() =>
                {
                    try
                    {
                        switch (MediaOutputType)
                        {
                            case MediaOutputType.Capture:
                                linnerresult = MediaCapture.Instance.addSource(a_SymbolicLink, a_MediaTypeIndex, a_RenderTarget);
                                break;
                            case MediaOutputType.Stream:
                                linnerresult = MediaStream.Instance.addSource(a_SymbolicLink, a_MediaTypeIndex, a_RenderTarget);
                                break;
                            default:
                                break;
                        }
                    }
                    finally
                    {
                        lBlockEvent.Set();
                    }
                });

                t.SetApartmentState(ApartmentState.MTA);

                t.Start();

                lBlockEvent.WaitOne();

                return linnerresult;
            }
                );
            
            return lresult;
        }
        
        public async Task removeSource(string a_SymbolicLink)
        {
            await Task.Run(() =>
            {
                AutoResetEvent lBlockEvent = new AutoResetEvent(false);

                var t = new Thread(() =>
                {
                    try
                    {
                        switch (MediaOutputType)
                        {
                            case MediaOutputType.Capture:
                                MediaCapture.Instance.removeSource(a_SymbolicLink);
                                break;
                            case MediaOutputType.Stream:
                                MediaStream.Instance.removeSource(a_SymbolicLink);
                                break;
                            default:
                                break;
                        }
                    }
                    finally
                    {
                        lBlockEvent.Set();
                    }
                });

                t.SetApartmentState(ApartmentState.MTA);

                t.Start();

                lBlockEvent.WaitOne();
            }
                );
        }

        public async void setPosition(string a_SymbolicLink, float aLeft, float aRight, float aTop, float aBottom)
        {
            await Task.Run(() =>
            {
                AutoResetEvent lBlockEvent = new AutoResetEvent(false);

                var t = new Thread(() =>
                {
                    try
                    {
                        switch (MediaOutputType)
                        {
                            case MediaOutputType.Capture:
                                MediaCapture.Instance.setPosition(a_SymbolicLink, aLeft, aRight, aTop, aBottom);
                                break;
                            case MediaOutputType.Stream:
                                MediaStream.Instance.setPosition(a_SymbolicLink, aLeft, aRight, aTop, aBottom);                   
                                break;
                            default:
                                break;
                        }
                    }
                    finally
                    {
                        lBlockEvent.Set();
                    }
                });

                t.SetApartmentState(ApartmentState.MTA);

                t.Start();

                lBlockEvent.WaitOne();
                }
            );
        }
        
        public int currentAvalableVideoMixers()
        {
            switch (MediaOutputType)
            {
                case MediaOutputType.Capture:
                    return MediaCapture.Instance.currentAvalableVideoMixers();
                case MediaOutputType.Stream:
                    return MediaStream.Instance.currentAvalableVideoMixers();
                default:
                    return 0;
            }
        }

        public int currentAvalableAudioMixers()
        {
            switch (MediaOutputType)
            {
                case MediaOutputType.Capture:
                    return MediaCapture.Instance.currentAvalableAudioMixers();
                case MediaOutputType.Stream:
                    return MediaStream.Instance.currentAvalableAudioMixers();
                default:
                    return 0;
            }
        }

        public async void setOpacity(string a_SymbolicLink, float a_value)
        {
            await Task.Run(() =>
            {
                AutoResetEvent lBlockEvent = new AutoResetEvent(false);

                var t = new Thread(() =>
                {
                    try
                    {
                        switch (MediaOutputType)
                        {
                            case MediaOutputType.Capture:
                                MediaCapture.Instance.setOpacity(a_SymbolicLink, a_value);
                                break;
                            case MediaOutputType.Stream:
                                MediaStream.Instance.setOpacity(a_SymbolicLink, a_value);
                                break;
                            default:
                                break;
                        }
                    }
                    finally
                    {
                        lBlockEvent.Set();
                    }
                });

                t.SetApartmentState(ApartmentState.MTA);

                t.Start();

                lBlockEvent.WaitOne();
            }
            );
        }

        public async void setRelativeVolume(string a_SymbolicLink, float a_value)
        {
            await Task.Run(() =>
            {
                AutoResetEvent lBlockEvent = new AutoResetEvent(false);

                var t = new Thread(() =>
                {
                    try
                    {
                        switch (MediaOutputType)
                        {
                            case MediaOutputType.Capture:
                                MediaCapture.Instance.setRelativeVolume(a_SymbolicLink, a_value);
                                break;
                            case MediaOutputType.Stream:
                                MediaStream.Instance.setRelativeVolume(a_SymbolicLink, a_value);
                                break;
                            default:
                                break;
                        }
                    }
                    finally
                    {
                        lBlockEvent.Set();
                    }
                });

                t.SetApartmentState(ApartmentState.MTA);

                t.Start();

                lBlockEvent.WaitOne();
            }
            );
        }

        private void ShowWarning(string a_message)
        {
            if (ShowWarningEvent != null)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    ShowWarningEvent(a_message);
                });
        }

        private void ShowInfo(string a_message)
        {
            if (ShowInfoEvent != null)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    ShowInfoEvent(a_message);
                });
        }        
    }
}
