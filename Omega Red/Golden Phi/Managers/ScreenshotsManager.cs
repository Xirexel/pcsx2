using Golden_Phi.Emulators;
using Golden_Phi.Models;
using Golden_Phi.Panels;
using Golden_Phi.Properties;
using Golden_Phi.Tools;
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

namespace Golden_Phi.Managers
{
    class ScreenshotsManager : IManager
    {
        public bool IsConfirmed => true;


        public event Action<bool, System.Windows.Media.ImageSource> TakeScreenshotEvent;

        public event Action<System.Windows.Media.ImageSource> ShowImageEvent;

        private VideoPanel m_VideoPanel = null;

        private ScreenshotsManager()
        {
            if (string.IsNullOrEmpty(Settings.Default.ScreenshotsFolder))
                Settings.Default.ScreenshotsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\" + App.c_MainFolderName + @"\";

            if (!System.IO.Directory.Exists(Settings.Default.ScreenshotsFolder))
            {
                System.IO.Directory.CreateDirectory(Settings.Default.ScreenshotsFolder);
            }

            mCustomerView = CollectionViewSource.GetDefaultView(_screenshotInfoCollection);

            mCustomerView.SortDescriptions.Add(
            new SortDescription("DateTime", ListSortDirection.Descending));

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                executeLoading();
            });

            mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;

            View = App.getResource("ScreenshotsView");
        }

        void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            if (Emul.Instance.Status == Emul.StatusEnum.Started)
                Emul.Instance.pause();

           var l_ScreenshotInfo = mCustomerView.CurrentItem as ScreenshotInfo;

            if(l_ScreenshotInfo != null && ShowImageEvent != null)
            {
                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(l_ScreenshotInfo.FilePath);
                bitmap.EndInit();
                bitmap.Freeze();

                ShowImageEvent(bitmap);
            }

            //Managers.MediaRecorderManager.Instance.StartStop(false);

            //if (mCustomerView.CurrentItem != null)
            //    LockScreenManager.Instance.showScreenshot();
        }

        private ICollectionView mCustomerView = null;

        private readonly ObservableCollection<ScreenshotInfo> _screenshotInfoCollection = new ObservableCollection<ScreenshotInfo>();

        private static ScreenshotsManager m_Instance = null;

        public static ScreenshotsManager Instance { get { if (m_Instance == null) m_Instance = new ScreenshotsManager(); return m_Instance; } }

        private void executeLoading()
        {
            ThreadStart loadScreenshotsStart = new ThreadStart(load);

            Thread loadScreenshotsThread = new Thread(loadScreenshotsStart);

            loadScreenshotsThread.Start();
        }

        private void load()
        {
            do
            {
                if (System.IO.Directory.Exists(Settings.Default.ScreenshotsFolder))
                {
                    string[] files = System.IO.Directory.GetFiles(Settings.Default.ScreenshotsFolder, "*.jpg");

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
                            ScreenshotInfo l_ScreenshotInfo = new ScreenshotInfo();

                            l_ScreenshotInfo.SmallImageSource = null;

                            l_ScreenshotInfo.FilePath = fi.FullName;

                            l_ScreenshotInfo.FileName = fi.Name;

                            l_ScreenshotInfo.DateTime = fi.LastWriteTime;

                            if (Application.Current != null)
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
                                {
                                    _screenshotInfoCollection.Add(l_ScreenshotInfo);
                                });
                            else
                                return;
                        }
                    }


                    if (Application.Current != null)
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
                        {
                            ThreadStart loadScreenshotsStart = new ThreadStart(()=> {
                                
                                foreach (var l_item in mCustomerView)
                                {
                                    var screenshotInfo = l_item as ScreenshotInfo;

                                    // Create the FileInfo object only when needed to ensure
                                    // the information is as current as possible.
                                    System.IO.FileInfo fi = null;
                                    try
                                    {
                                        fi = new System.IO.FileInfo(screenshotInfo.FilePath);
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

                                        Thread.Sleep(100);

                                        var bitmap = new BitmapImage();

                                        bitmap.BeginInit();
                                        bitmap.DecodePixelWidth = 200;
                                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                        bitmap.UriSource = new Uri(screenshotInfo.FilePath);
                                        bitmap.EndInit();
                                        bitmap.Freeze();

                                        if (Application.Current != null)
                                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate ()
                                            {
                                                screenshotInfo.SmallImageSource = bitmap;
                                            });
                                        else
                                            return;
                                    }
                                }

                            });

                            Thread loadScreenshotsThread = new Thread(loadScreenshotsStart);

                            loadScreenshotsThread.Start();
                        });
                    else
                        return;
                }

            } while (false);
        }

        public void setVideoPanel(VideoPanel a_VideoPanel)
        {
            m_VideoPanel = a_VideoPanel;
        }

        private byte[] takeScreenshot()
        {
            byte[] l_result = null;

            if (m_VideoPanel != null)
                l_result = m_VideoPanel.takeScreenshot();

            return l_result;
        }

        public void removeItem(object a_Item)
        {
            var l_ScreenshotInfo = a_Item as ScreenshotInfo;

            if (l_ScreenshotInfo != null)
            {
                _screenshotInfoCollection.Remove(l_ScreenshotInfo);

                File.Delete(l_ScreenshotInfo.FilePath);
            }
        }

        public void createItem()
        {
            var l_gameData = GameIndex.Instance.convert(Emul.Instance.DiscSerial);

            string l_add_file_path = "Screenshot_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".jpg";

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

                if (!l_hasInvalidChar)
                    l_add_file_path = l_gameData.FriendlyName + "_" + l_add_file_path;
            }

            if (File.Exists(Settings.Default.ScreenshotsFolder + l_add_file_path))
                return;

            var l_data = takeScreenshot();

            if (l_data == null || l_data.Length <= 0)
                return;

            using (var l_FileStream = File.Open(Settings.Default.ScreenshotsFolder + l_add_file_path, FileMode.Create))
            {
                if (l_FileStream == null)
                    return;

                l_FileStream.Write(l_data, 0, l_data.Length);
            }

            var bitmap = new BitmapImage();

            using (var stream = new MemoryStream(l_data))
            {
                stream.Position = 0; // here

                bitmap.BeginInit();
                bitmap.DecodePixelWidth = 100;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            ScreenshotInfo a_ScreenshotInfo = new ScreenshotInfo();

            a_ScreenshotInfo.SmallImageSource = bitmap;

            a_ScreenshotInfo.FilePath = Settings.Default.ScreenshotsFolder + l_add_file_path;

            a_ScreenshotInfo.FileName = l_add_file_path;

            a_ScreenshotInfo.DateTime = DateTime.Now;

            _screenshotInfoCollection.Add(a_ScreenshotInfo);

            SoundSchemaManager.Instance.playEvent(SoundSchemaManager.Event.Click);

            bitmap = new BitmapImage();

            using (var stream = new MemoryStream(l_data))
            {
                stream.Position = 0; // here

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            if (TakeScreenshotEvent != null)
                TakeScreenshotEvent(true, bitmap);


            var _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 500);
            _dispatcherTimer.Tick += (sender, e) => {
                _dispatcherTimer.Stop();

                if (TakeScreenshotEvent != null)
                    TakeScreenshotEvent(false, null);
            };
            _dispatcherTimer.Start();

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

        public ICollectionView Collection
        {
            get { return mCustomerView; }
        }

        public object View { get; private set; }
    }
}
