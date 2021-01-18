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

using Omega_Red.Models;
using Omega_Red.Properties;
using Omega_Red.Tools;
using Omega_Red.Panels;
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
using Omega_Red.Emulators;

namespace Omega_Red.Managers
{
    class ScreenshotsManager : IManager
    {
        public event Action<bool, System.Windows.Media.ImageSource> TakeScreenshotEvent;
        
        private VideoPanel m_VideoPanel = null;

        private ScreenshotsManager()
        {
            if (string.IsNullOrEmpty(Settings.Default.ScreenshotsFolder))
                Settings.Default.ScreenshotsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\Omega Red\";

            if (!System.IO.Directory.Exists(Settings.Default.ScreenshotsFolder))
            {
                System.IO.Directory.CreateDirectory(Settings.Default.ScreenshotsFolder);
            }

            mCustomerView = CollectionViewSource.GetDefaultView(_screenshotInfoCollection);
            
            mCustomerView.SortDescriptions.Add(
            new SortDescription("DateTime", ListSortDirection.Descending));
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                executeLoading();
            });

            mCustomerView.CurrentChanged += mCustomerView_CurrentChanged;
        }

        void mCustomerView_CurrentChanged(object sender, EventArgs e)
        {
            if (Emul.Instance.Status == Emul.StatusEnum.Started)
                Emul.Instance.pause();

            Managers.MediaRecorderManager.Instance.StartStop(false);

            if (mCustomerView.CurrentItem != null)
                LockScreenManager.Instance.showScreenshot();
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

                        if(fi.Exists)
                        {
                            Thread.Sleep(500);

                            var bitmap = new BitmapImage();

                            bitmap.BeginInit();
                            bitmap.DecodePixelWidth = 100;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(fi.FullName);
                            bitmap.EndInit();
                            bitmap.Freeze();

                            ScreenshotInfo l_ScreenshotInfo = new ScreenshotInfo();

                            l_ScreenshotInfo.SmallImageSource = bitmap;

                            l_ScreenshotInfo.FilePath = fi.FullName;

                            l_ScreenshotInfo.FileName = fi.Name;

                            l_ScreenshotInfo.DateTime = fi.LastWriteTime;

                            if (Application.Current != null)
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate()
                                {
                                    _screenshotInfoCollection.Add(l_ScreenshotInfo);
                                });
                            else
                                return;
                        }
                    }
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

            if(l_ScreenshotInfo != null)
            {
                _screenshotInfoCollection.Remove(l_ScreenshotInfo);

                File.Delete(l_ScreenshotInfo.FilePath);
            }     
        }
        
        public void createItem()
        {
            var l_isoInfo = Emul.Instance.IsoInfo;

            if (l_isoInfo == null)
                return;

            var l_gameData = GameIndex.Instance.convert(l_isoInfo.DiscSerial);

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
            _dispatcherTimer.Tick += (sender, e)=> {
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
    }
}
