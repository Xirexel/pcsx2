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
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Omega_Red.Managers
{
    class MediaRecorderManager : IManager
    {

        private ICollectionView mCustomerView = null;

        private readonly ObservableCollection<MediaRecorderInfo> _mediaRecorderInfoCollection = new ObservableCollection<MediaRecorderInfo>();

        private static MediaRecorderManager m_Instance = null;

        public static MediaRecorderManager Instance { get { if (m_Instance == null) m_Instance = new MediaRecorderManager(); return m_Instance; } }

        private MediaRecorderManager()
        {

            PCSX2Controller.Instance.ChangeStatusEvent += (PCSX2Controller.StatusEnum obj) =>
            {

                switch (obj)
                {
                    case PCSX2Controller.StatusEnum.Stopped:
                    case PCSX2Controller.StatusEnum.Paused:
                        StartStop(false);
                        break;
                    case PCSX2Controller.StatusEnum.NoneInitilized:
                    case PCSX2Controller.StatusEnum.Initilized:
                    case PCSX2Controller.StatusEnum.Started:
                    default:
                        break;
                }

            };

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
            if (aState is Boolean)
            {
                if ((bool)aState)
                {
                    MediaCapture.Instance.start();
                }
                else
                {
                    MediaCapture.Instance.stop();
                }

            }
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
                l_add_file_path = l_gameData.FriendlyName + "_" + l_add_file_path;
            }

            if (File.Exists(Settings.Default.VideosFolder + l_add_file_path))
            {
                File.Delete(aTempFile);

                return;
            }

            File.Move(aTempFile, Settings.Default.VideosFolder + l_add_file_path);

            MediaRecorderInfo l_MediaRecorderInfo = new MediaRecorderInfo();

            l_MediaRecorderInfo.FilePath = Settings.Default.VideosFolder + l_add_file_path;

            l_MediaRecorderInfo.FileName = l_add_file_path;

            l_MediaRecorderInfo.DateTime = DateTime.Now;

            _mediaRecorderInfoCollection.Add(l_MediaRecorderInfo);
        }

        public System.ComponentModel.ICollectionView Collection
        {
            get { return mCustomerView; }
        }
    }
}
