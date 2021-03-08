using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using Omega_Red.Models;
using Omega_Red.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using File = Google.Apis.Drive.v3.Data.File;
using Upload = Google.Apis.Upload;

namespace Omega_Red.SocialNetworks.Google
{
    delegate File getDriveDirectoryDel(DriveService service, File a_OmegaRedDirectory);

    class DriveManager
    {
        private const int KB = 0x400;

        private const int ChunkSize = 256 * KB;

        IList<SaveStateInfo> m_Current_online_savings = null;

        IList<MemoryCardInfo> m_Current_online_MemoryCards = null;

        IDictionary<string, IList<SaveStateInfo>> m_Online_savings = new Dictionary<string, IList<SaveStateInfo>>();

        IDictionary<string, IList<MemoryCardInfo>> m_Online_MemoryCards = new Dictionary<string, IList<MemoryCardInfo>>();



        private static DriveManager m_Instance = null;
        public static DriveManager Instance { get { if (m_Instance == null) m_Instance = new DriveManager(); return m_Instance; } }

        private DriveManager() { }

        public void reset()
        {
            m_Online_savings.Clear();
        }

        public IList<SaveStateInfo> getSaveStateList(string a_disk_serial)
        {
            if (!m_Online_savings.Keys.Contains(a_disk_serial))
            {
                IList<SaveStateInfo> l_list = new List<SaveStateInfo>();

                do
                {
                    var l_Initializer = GoogleAccountManager.Instance.getInitializer();

                    if (l_Initializer == null)
                        break;

                    using (var service = new DriveService(l_Initializer))
                    {
                        var lDirectory = getOmegaRedDirectory(service);

                        if (lDirectory == null)
                        {
                            break;
                        }

                        var lSStates = getSStates(service, lDirectory);

                        if (lSStates == null)
                        {
                            break;
                        }

                        FilesResource.ListRequest list = service.Files.List();

                        list.PageSize = 1000;

                        list.Q = "mimeType != 'application/vnd.google-apps.folder' and trashed=false and name contains '" + a_disk_serial + "'" + " and '" + lSStates.Id + "' in parents";

                        //string allFields = "appProperties,capabilities,contentHints,createdTime,description,explicitlyTrashed,fileExtension,folderColorRgb,fullFileExtension,headRevisionId,iconLink,id,imageMediaMetadata,isAppAuthorized,kind,lastModifyingUser,md5Checksum,mimeType,modifiedByMeTime,modifiedTime,name,originalFilename,ownedByMe,owners,parents,permissions,properties,quotaBytesUsed,shared,sharedWithMeTime,sharingUser,size,spaces,starred,thumbnailLink,trashed,version,videoMediaMetadata,viewedByMe,viewedByMeTime,viewersCanCopyContent,webContentLink,webViewLink,writersCanShare";

                        list.Fields = "nextPageToken, files(id, name, size, description)";

                        FileList filesFeed = list.Execute();

                        if (filesFeed.Files.Count > 0)
                        {
                            foreach (var item in filesFeed.Files)
                            {
                                addSaveState(l_list, item.Name, item.Description);
                            }
                        }
                    }

                } while (false);

                m_Online_savings[a_disk_serial] = l_list;
            }

            m_Current_online_savings = m_Online_savings[a_disk_serial];

            return m_Current_online_savings;
        }

        public IList<MemoryCardInfo> getMemoryCardList(string a_disk_serial)
        {
            if (!m_Online_MemoryCards.Keys.Contains(a_disk_serial))
            {
                IList<MemoryCardInfo> l_list = new List<MemoryCardInfo>();

                do
                {
                    var l_Initializer = GoogleAccountManager.Instance.getInitializer();

                    if (l_Initializer == null)
                        break;

                    using (var service = new DriveService(l_Initializer))
                    {
                        var lDirectory = getOmegaRedDirectory(service);

                        if (lDirectory == null)
                        {
                            break;
                        }

                        var l_MemoryCards = getMemoryCards(service, lDirectory);

                        if (l_MemoryCards == null)
                        {
                            break;
                        }

                        FilesResource.ListRequest list = service.Files.List();

                        list.PageSize = 1000;

                        list.Q = "mimeType != 'application/vnd.google-apps.folder' and trashed=false and name contains '" + a_disk_serial + "'" + " and '" + l_MemoryCards.Id + "' in parents";

                        //string allFields = "appProperties,capabilities,contentHints,createdTime,description,explicitlyTrashed,fileExtension,folderColorRgb,fullFileExtension,headRevisionId,iconLink,id,imageMediaMetadata,isAppAuthorized,kind,lastModifyingUser,md5Checksum,mimeType,modifiedByMeTime,modifiedTime,name,originalFilename,ownedByMe,owners,parents,permissions,properties,quotaBytesUsed,shared,sharedWithMeTime,sharingUser,size,spaces,starred,thumbnailLink,trashed,version,videoMediaMetadata,viewedByMe,viewedByMeTime,viewersCanCopyContent,webContentLink,webViewLink,writersCanShare";

                        list.Fields = "nextPageToken, files(id, name, size, description)";

                        FileList filesFeed = list.Execute();

                        if (filesFeed.Files.Count > 0)
                        {
                            foreach (var item in filesFeed.Files)
                            {
                                addMemoryCard(l_list, item.Name, item.Description);
                            }
                        }
                    }

                } while (false);

                m_Online_MemoryCards[a_disk_serial] = l_list;
            }

            m_Current_online_MemoryCards = m_Online_MemoryCards[a_disk_serial];

            return m_Current_online_MemoryCards;
        }

        private void addSaveState(IList<SaveStateInfo> a_list, string a_name, string a_description)
        {
            if (a_list == null || string.IsNullOrWhiteSpace(a_name) || string.IsNullOrWhiteSpace(a_description))
                return;

            string l_Date = "";

            string l_Duration = "";

            string l_CheckSumString = "";

            var l_descrList = a_description.Split(new char[] { '|' });

            if (l_descrList != null && l_descrList.Length > 0)
            {
                foreach (var descr in l_descrList)
                {
                    if (descr.Contains("Date="))
                    {
                        var l_value = descr.Split(new char[] { '=' });

                        if (l_value != null && l_value.Length == 2)
                        {
                            l_Date = l_value[1];
                        }
                    }

                    if (descr.Contains("Duration="))
                    {
                        var l_value = descr.Split(new char[] { '=' });

                        if (l_value != null && l_value.Length == 2)
                        {
                            l_Duration = l_value[1];
                        }
                    }

                    if (descr.Contains("CheckSum="))
                    {
                        var l_value = descr.Split(new char[] { '=' });

                        if (l_value != null && l_value.Length == 2)
                        {
                            l_CheckSumString = l_value[1];
                        }
                    }
                }
            }

            int l_index = -1;

            uint l_CheckSum = 0;

            var l_nameSplitList = a_name.Split(new char[] { '.' });

            if (l_nameSplitList != null && l_nameSplitList.Length == 4)
            {
                var l_index_string = l_nameSplitList[2];

                if (!string.IsNullOrWhiteSpace(l_index_string))
                {
                    int l_value = 0;

                    if (int.TryParse(l_index_string, out l_value))
                    {
                        l_index = l_value;
                    }
                }
            }
            else if (l_nameSplitList != null && l_nameSplitList.Length == 3)
            {
                var l_index_string = l_nameSplitList[1];

                if (!string.IsNullOrWhiteSpace(l_index_string))
                {
                    int l_value = 0;

                    if (int.TryParse(l_index_string, out l_value))
                    {
                        l_index = l_value;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(l_CheckSumString))
            {
                uint l_value = 0;

                if (uint.TryParse(l_CheckSumString, out l_value))
                {
                    l_CheckSum = l_value;
                }
            }

            var l_DataFixed = l_Date.Replace('\n', ' ');

            DateTime lDateTime = DateTime.Now;

            if (!DateTime.TryParseExact(l_DataFixed, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out lDateTime))
                lDateTime = DateTime.Now;

            SaveStateInfo l_SaveStateInfo = new SaveStateInfo()
            {
                FilePath = Settings.Default.SlotFolder + a_name,
                Date = l_Date,
                DateTime = lDateTime,
                Duration = l_Duration,
                IsQuicksave = a_name.Contains(".quick."),
                IsAutosave = a_name.Contains(".auto."),
                Index = l_index,
                CheckSum = l_CheckSum,
                IsCloudOnlysave = true,
                IsCloudsave = true
            };

            a_list.Remove(a_list.FirstOrDefault(saveState => saveState.FilePath == l_SaveStateInfo.FilePath));

            a_list.Add(l_SaveStateInfo);
        }

        private void addMemoryCard(IList<MemoryCardInfo> a_list, string a_name, string a_description)
        {
            if (a_list == null || string.IsNullOrWhiteSpace(a_name) || string.IsNullOrWhiteSpace(a_description))
                return;

            string l_Date = "";

            var l_descrList = a_description.Split(new char[] { '|' });

            if (l_descrList != null && l_descrList.Length > 0)
            {
                foreach (var descr in l_descrList)
                {
                    if (descr.Contains("Date="))
                    {
                        var l_value = descr.Split(new char[] { '=' });

                        if (l_value != null && l_value.Length == 2)
                        {
                            l_Date = l_value[1];
                        }
                    }
                }
            }

            int l_index = -1;

            var l_nameSplitList = a_name.Split(new char[] { '.' });

            if (l_nameSplitList != null && l_nameSplitList.Length == 4)
            {
                var l_index_string = l_nameSplitList[2];

                if (!string.IsNullOrWhiteSpace(l_index_string))
                {
                    int l_value = 0;

                    if (int.TryParse(l_index_string, out l_value))
                    {
                        l_index = l_value;
                    }
                }
            }
            else if (l_nameSplitList != null && l_nameSplitList.Length == 3)
            {
                var l_index_string = l_nameSplitList[1];

                if (!string.IsNullOrWhiteSpace(l_index_string))
                {
                    int l_value = 0;

                    if (int.TryParse(l_index_string, out l_value))
                    {
                        l_index = l_value;
                    }
                }
            }

            var l_DataFixed = l_Date.Replace('\n', ' ');

            DateTime lDateTime = DateTime.Now;

            if (!DateTime.TryParseExact(l_DataFixed, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out lDateTime))
                lDateTime = DateTime.Now;

            var l_Number = string.Format("{0:#}", l_index + 1);

            if (l_index == -1)
                l_Number = "Shared";

            MemoryCardInfo l_MemoryCardInfo = new MemoryCardInfo()
            {
                FileName = a_name,
                FilePath = Settings.Default.MemoryCardsFolder + a_name,
                CloudSaveDate = l_Date,
                Date = l_Date,
                DateTime = lDateTime,
                Index = l_index,
                Number = l_Number,
                IsCloudOnlysave = true,
                IsCloudsave = true
            };

            a_list.Remove(a_list.FirstOrDefault(saveState => saveState.FilePath == l_MemoryCardInfo.FilePath));

            a_list.Add(l_MemoryCardInfo);
        }

        public void startUploadingSState(string a_file_path, FrameworkElement a_banner_grid, string a_description, Action<bool> a_callback)
        {
            TextBlock lUploadProgressTitleTextBlock = a_banner_grid.FindName("mUploadProgressTitleTextBlock") as TextBlock;

            TextBlock lProgressTextBlock = a_banner_grid.FindName("mProgressTextBlock") as TextBlock;

            ThreadStart l_uploadThreadStart = new ThreadStart(() =>
            {
                startUploading(
                a_file_path,
                a_banner_grid,
                lUploadProgressTitleTextBlock,
                lProgressTextBlock,
                a_description,
                getSStates,
                (Name, Description) =>
                {
                    if (a_callback != null)
                    {
                        a_callback(true);
                    }

                    addSaveState(m_Current_online_savings, Name, Description);
                });
                
            });
            
            Thread l_uploadThread = new Thread(l_uploadThreadStart);

            l_uploadThread.Start();
        }

        public void startUploadingMemoryCard(string a_disk_serial, string a_file_path, FrameworkElement a_banner_grid, string a_description, Action<bool> a_callback)
        {
            IList<MemoryCardInfo> l_Current_online_MemoryCards = DriveManager.Instance.getMemoryCardList(a_disk_serial);

            if (l_Current_online_MemoryCards == null)
                return;
                       
            TextBlock lUploadProgressTitleTextBlock = a_banner_grid.FindName("mUploadProgressTitleTextBlock") as TextBlock;

            TextBlock lProgressTextBlock = a_banner_grid.FindName("mProgressTextBlock") as TextBlock;

            ThreadStart l_uploadThreadStart = new ThreadStart(() =>
            {
                startUploading(
                a_file_path,
                a_banner_grid,
                lUploadProgressTitleTextBlock,
                lProgressTextBlock,
                a_description,
                getMemoryCards,
                (Name, Description) =>
                {
                    if (a_callback != null)
                    {
                        a_callback(true);
                    }

                    addMemoryCard(l_Current_online_MemoryCards, Name, Description);
                });

            });

            Thread l_uploadThread = new Thread(l_uploadThreadStart);

            l_uploadThread.Start();
        }


        public void startUploadingDisc(string a_file_path, FrameworkElement a_banner_grid, string a_description, Action<bool> a_callback)
        {

            TextBlock lUploadProgressTitleTextBlock = a_banner_grid.FindName("mUploadProgressTitleTextBlock") as TextBlock;

            TextBlock lProgressTextBlock = a_banner_grid.FindName("mProgressTextBlock") as TextBlock;

            ThreadStart l_uploadThreadStart = new ThreadStart(() =>
            {
                startUploading(
                a_file_path,
                a_banner_grid,
                lUploadProgressTitleTextBlock,
                lProgressTextBlock,
                a_description,
                getDiscs,
                (Name, Description) => {
                    if (a_callback != null)
                    {
                        a_callback(true);
                    }
                });

            });

            Thread l_uploadThread = new Thread(l_uploadThreadStart);

            l_uploadThread.Start();
        }

        private void startUploading(
            string a_file_path,
            FrameworkElement a_banner_grid,
            TextBlock a_UploadProgressTitleTextBlock,
            TextBlock a_ProgressTextBlock,
            string a_description,
            getDriveDirectoryDel a_getDriveDirectoryDel,
            Action<string, string> a_addAction)
        {

            if (a_addAction == null)
                return;

            var l_Initializer = GoogleAccountManager.Instance.getInitializer();

            if (l_Initializer == null)
                return;

            if (a_getDriveDirectoryDel == null)
                return;

            using (var service = new DriveService(l_Initializer) { HttpClient = { Timeout = TimeSpan.FromSeconds(15) } })
            {
                var lDirectory = getOmegaRedDirectory(service);

                if (lDirectory == null)
                {
                    return;
                }

                var l_DriveDirectory = a_getDriveDirectoryDel(service, lDirectory);

                if (l_DriveDirectory == null)
                {
                    return;
                }

                if (System.IO.File.Exists(a_file_path))
                {
                    var lName = System.IO.Path.GetFileName(a_file_path);

                    FilesResource.ListRequest list = service.Files.List();

                    list.PageSize = 1000;

                    list.Q = "mimeType != 'application/vnd.google-apps.folder' and trashed=false and name = '" + lName + "'" + " and '" + l_DriveDirectory.Id + "' in parents";

                    FileList filesFeed = list.Execute();

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        a_banner_grid.Visibility = Visibility.Visible;

                        a_UploadProgressTitleTextBlock.Visibility = Visibility.Visible;
                    });


                    if (filesFeed.Files.Count == 0)
                    {
                        createFile(service, a_file_path, a_description, l_DriveDirectory.Id, a_banner_grid, a_UploadProgressTitleTextBlock, a_ProgressTextBlock, a_addAction);
                    }
                    else
                        updateFile(service, filesFeed.Files[0], a_file_path, a_description, l_DriveDirectory.Id, a_banner_grid, a_UploadProgressTitleTextBlock, a_ProgressTextBlock, a_addAction);
                }
                else
                {
                }
            }
        }
        
        public async Task startDownloadingMemoryCardAsync(string a_file_path, FrameworkElement a_banner_grid)
        {
            await startDownloadingAsync(a_file_path, a_banner_grid, getMemoryCards, Settings.Default.MemoryCardsFolder);
        }

        public async Task startDownloadingSStateAsync(string a_file_path, FrameworkElement a_banner_grid)
        {
            await startDownloadingAsync(a_file_path, a_banner_grid, getSStates, Settings.Default.SlotFolder);
        }

        public async Task startDownloadingDiscAsync(string a_file_path, FrameworkElement a_banner_grid)
        {
            await startDownloadingAsync(a_file_path, a_banner_grid, getDiscs, "");
        }

        public async Task startDownloadingAsync(
            string a_file_path,
            FrameworkElement a_banner_grid,
            getDriveDirectoryDel a_getDriveDirectoryDel,
            string a_target_folder)
        {

            var l_Initializer = GoogleAccountManager.Instance.getInitializer();

            if (l_Initializer == null)
                return;

            if (a_getDriveDirectoryDel == null)
                return;

            using (var service = new DriveService(l_Initializer) { HttpClient = { Timeout = TimeSpan.FromSeconds(15) } })
            {
                var lDirectory = getOmegaRedDirectory(service);

                if (lDirectory == null)
                {
                    return;
                }

                var l_DriveDirectory = a_getDriveDirectoryDel(service, lDirectory);

                if (l_DriveDirectory == null)
                {
                    return;
                }


                var lName = System.IO.Path.GetFileName(a_file_path);

                FilesResource.ListRequest list = service.Files.List();

                list.PageSize = 1000;

                list.Q = "mimeType != 'application/vnd.google-apps.folder' and trashed=false and name = '" + lName + "'" + " and '" + l_DriveDirectory.Id + "' in parents";

                //string allFields = "appProperties,capabilities,contentHints,createdTime,description,explicitlyTrashed,fileExtension,folderColorRgb,fullFileExtension,headRevisionId,iconLink,id,imageMediaMetadata,isAppAuthorized,kind,lastModifyingUser,md5Checksum,mimeType,modifiedByMeTime,modifiedTime,name,originalFilename,ownedByMe,owners,parents,permissions,properties,quotaBytesUsed,shared,sharedWithMeTime,sharingUser,size,spaces,starred,thumbnailLink,trashed,version,videoMediaMetadata,viewedByMe,viewedByMeTime,viewersCanCopyContent,webContentLink,webViewLink,writersCanShare";

                list.Fields = "nextPageToken, files(id, name, size)";

                FileList filesFeed = list.Execute();

                if (filesFeed.Files.Count > 0)
                {
                    a_banner_grid.Visibility = Visibility.Visible;

                    var lDownloadProgressTitleTextBlock = a_banner_grid.FindName("mDownloadProgressTitleTextBlock") as TextBlock;

                    lDownloadProgressTitleTextBlock.Visibility = Visibility.Visible;

                    await downloadFile(service, filesFeed.Files[0], a_banner_grid, lDownloadProgressTitleTextBlock, a_target_folder);
                }
            }
        }

        public async Task startDeletingMemoryCardAsync(string a_id_name, string a_file_path)
        {
            await startDeletingAsync(a_id_name, getMemoryCards,
                () => {
                    if (m_Current_online_MemoryCards != null)
                        m_Current_online_MemoryCards.Remove(m_Current_online_MemoryCards.First(memoryCard => memoryCard.FilePath == a_file_path));
                });
        }

        public async Task startDeletingSStateAsync(string a_file_path)
        {
            await startDeletingAsync(a_file_path, getSStates,
                () => {
                    if (m_Current_online_savings != null)
                        m_Current_online_savings.Remove(m_Current_online_savings.First(saveState => saveState.FilePath == a_file_path));
                });
        }

        private async Task startDeletingAsync(
            string a_file_path,
            getDriveDirectoryDel a_getDriveDirectoryDel,
            Action a_deleteAction)
        {

            var l_Initializer = GoogleAccountManager.Instance.getInitializer();

            if (l_Initializer == null)
                return;

            if (a_getDriveDirectoryDel == null)
                return;

            if (a_deleteAction == null)
                return;

            using (var service = new DriveService(l_Initializer))
            {
                var lDirectory = getOmegaRedDirectory(service);

                if (lDirectory == null)
                {
                    return;
                }

                var lSStates = a_getDriveDirectoryDel(service, lDirectory);

                if (lSStates == null)
                {
                    return;
                }

                var lName = a_file_path;// System.IO.Path.GetFileName(a_file_path);

                FilesResource.ListRequest list = service.Files.List();

                list.PageSize = 1000;

                list.Q = "mimeType != 'application/vnd.google-apps.folder' and trashed=false and name = '" + lName + "'" + " and '" + lSStates.Id + "' in parents";

                list.Fields = "nextPageToken, files(id)";

                FileList filesFeed = list.Execute();

                if (filesFeed.Files.Count > 0)
                {
                    await service.Files.Delete(filesFeed.Files[0].Id).ExecuteAsync();

                    a_deleteAction();
                }
            }
        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        private File getOmegaRedDirectory(DriveService service)
        {
            File lDirectory = null;

            try
            {
                FilesResource.ListRequest list = service.Files.List();
                list.PageSize = 1000;

                list.Q = "mimeType='application/vnd.google-apps.folder' and trashed=false and name = 'Omega Red'";

                FileList filesFeed = list.Execute();

                if (filesFeed.Files.Count == 0)
                {
                    lDirectory = createFolder(service, "Omega Red");
                }
                else
                    lDirectory = filesFeed.Files[0];
            }
            catch (Exception)
            {
            }


            return lDirectory;
        }

        private File getMemoryCards(DriveService service, File a_OmegaRedDirectory)
        {
            File lDirectory = null;

            try
            {
                FilesResource.ListRequest list = service.Files.List();
                list.PageSize = 1000;

                list.Q = "mimeType='application/vnd.google-apps.folder' and trashed=false and name = 'MemoryCards'";

                FileList filesFeed = list.Execute();

                if (filesFeed.Files.Count == 0)
                {
                    lDirectory = createFolder(service, "MemoryCards", a_OmegaRedDirectory.Id);
                }
                else
                    lDirectory = filesFeed.Files[0];

            }
            catch (Exception)
            {
            }

            return lDirectory;
        }

        private File getSStates(DriveService service, File a_OmegaRedDirectory)
        {
            File lDirectory = null;

            try
            {
                FilesResource.ListRequest list = service.Files.List();
                list.PageSize = 1000;

                list.Q = "mimeType='application/vnd.google-apps.folder' and trashed=false and name = 'SStates'";

                FileList filesFeed = list.Execute();

                if (filesFeed.Files.Count == 0)
                {
                    lDirectory = createFolder(service, "SStates", a_OmegaRedDirectory.Id);
                }
                else
                    lDirectory = filesFeed.Files[0];

            }
            catch (Exception)
            {
            }

            return lDirectory;
        }
        private File getDiscs(DriveService service, File a_OmegaRedDirectory)
        {
            File lDirectory = null;

            try
            {
                FilesResource.ListRequest list = service.Files.List();
                list.PageSize = 1000;

                list.Q = "mimeType='application/vnd.google-apps.folder' and trashed=false and name = 'Discs'";

                FileList filesFeed = list.Execute();

                if (filesFeed.Files.Count == 0)
                {
                    lDirectory = createFolder(service, "Discs", a_OmegaRedDirectory.Id);
                }
                else
                    lDirectory = filesFeed.Files[0];

            }
            catch (Exception)
            {
            }

            return lDirectory;
        }

        private File createFolder(DriveService service, string name, string a_parent_id = "root")
        {
            File lfolder = null;

            var folderFileMetadata = new File()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>() { a_parent_id }
            };

            var request = service.Files.Create(folderFileMetadata);

            request.Fields = "id";

            lfolder = request.Execute();

            return lfolder;
        }

        private void createFile(
            DriveService service,
            string a_file_path,
            string a_description,
            string a_parent_id,
            FrameworkElement a_banner_grid,
            TextBlock a_title_textblock,
            TextBlock a_ProgressTextBlock,
            Action<string, string> a_addAction)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                a_ProgressTextBlock.Text = "";
            });


            File body = new File();
            body.Name = System.IO.Path.GetFileName(a_file_path);
            body.Description = a_description;
            body.MimeType = GetMimeType(a_file_path);
            body.Parents = new List<string>() { a_parent_id };

            byte[] byteArray = System.IO.File.ReadAllBytes(a_file_path);

            using (var stream = new System.IO.MemoryStream(byteArray))
            {

                try
                {
                    var request = service.Files.Create(body, stream, GetMimeType(a_file_path));

                    request.ChunkSize = ResumableUpload.MinimumChunkSize;

                    request.ProgressChanged += (obj) =>
                    {
                        switch (obj.Status)
                        {
                            case UploadStatus.Starting:
                            case UploadStatus.Uploading:
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    a_ProgressTextBlock.Text = string.Format("{0:0.00} %", (float)obj.BytesSent * 100.0 / (float)byteArray.Length);
                                });
                                break;
                            case UploadStatus.Completed:
                                {
                                    a_addAction(body.Name, body.Description);

                                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                    {
                                        a_banner_grid.Visibility = Visibility.Collapsed;

                                        a_title_textblock.Visibility = Visibility.Collapsed;

                                    });
                                }
                                break;
                            default:
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    a_banner_grid.Visibility = Visibility.Collapsed;

                                    a_title_textblock.Visibility = Visibility.Collapsed;

                                });
                                break;
                        }
                    };

                    request.ResponseReceived += (File obj) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            a_banner_grid.Visibility = Visibility.Collapsed;
                        });
                    };

                    var task = request.UploadAsync();

                    if (task != null)
                        task.Wait();
                }
                catch (Exception)
                {
                }
            }
        }

        private void updateFile(
            DriveService service,
            File originalFile,
            string a_file_path,
            string a_description,
            string a_parent_id,
            FrameworkElement a_banner_grid,
            TextBlock a_title_textblock,
            TextBlock a_ProgressTextBlock,
            Action<string, string> a_addAction)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                a_ProgressTextBlock.Text = "";
            });

            File body = new File();
            body.Name = System.IO.Path.GetFileName(a_file_path);
            body.Description = a_description;
            body.MimeType = GetMimeType(a_file_path);


            byte[] byteArray = System.IO.File.ReadAllBytes(a_file_path);

            using (var stream = new System.IO.MemoryStream(byteArray))
            {
                try
                {
                    var request = service.Files.Update(body, originalFile.Id, stream, GetMimeType(a_file_path));

                    request.ChunkSize = ResumableUpload.MinimumChunkSize;

                    request.ProgressChanged += (Upload.IUploadProgress obj) =>
                    {
                        switch (obj.Status)
                        {
                            case UploadStatus.Starting:
                            case UploadStatus.Uploading:
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    a_ProgressTextBlock.Text = string.Format("{0:0.00} %", (float)obj.BytesSent * 100.0 / (float)byteArray.Length);
                                });
                                break;
                            case UploadStatus.Completed:
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    a_addAction(body.Name, body.Description);

                                    a_banner_grid.Visibility = Visibility.Collapsed;

                                    a_title_textblock.Visibility = Visibility.Collapsed;
                                });
                                break;
                            default:
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    a_banner_grid.Visibility = Visibility.Collapsed;

                                    a_title_textblock.Visibility = Visibility.Collapsed;
                                });
                                break;
                        }
                    };

                    request.ResponseReceived += (File obj) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            a_banner_grid.Visibility = Visibility.Collapsed;
                        });
                    };

                    var task = request.UploadAsync();

                    if (task != null)
                        task.Wait();
                }
                catch (Exception)
                {
                }
            }
        }

        private async Task downloadFile(
            DriveService service,
            File downloadFile,
            FrameworkElement a_banner_grid,
            TextBlock a_title_textblock,
            string a_target_folder)
        {
            var lProgressTextBlock = a_banner_grid.FindName("mProgressTextBlock") as TextBlock;

            lProgressTextBlock.Text = "";

            var downloader = service.Files.Get(downloadFile.Id);

            var mediaDownloader = downloader.MediaDownloader;

            if (mediaDownloader != null)
            {
                mediaDownloader.ChunkSize = ChunkSize;

                mediaDownloader.ProgressChanged += (IDownloadProgress obj) =>
                {
                    switch (obj.Status)
                    {
                        case DownloadStatus.Downloading:
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                            {
                                lProgressTextBlock.Text = string.Format("{0:0.00} %", (float)obj.BytesDownloaded * 100.0 / (float)downloadFile.Size);
                            });
                            break;
                        case DownloadStatus.Completed:
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                            {
                                a_banner_grid.Visibility = Visibility.Collapsed;

                                a_title_textblock.Visibility = Visibility.Collapsed;
                            });
                            break;
                        case DownloadStatus.NotStarted:
                        case DownloadStatus.Failed:
                        default:
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                            {
                                a_banner_grid.Visibility = Visibility.Collapsed;

                                a_title_textblock.Visibility = Visibility.Collapsed;
                            });
                            break;
                    }
                };
            }

            var l_tempFilePath = a_target_folder + downloadFile.Name + "_downloaded";

            var l_FilePath = a_target_folder + downloadFile.Name;

            using (var fileStream = new System.IO.FileStream(l_tempFilePath,
                System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                await downloader.DownloadAsync(fileStream);
            }

            if (System.IO.File.Exists(l_FilePath))
                System.IO.File.Delete(l_FilePath);

            System.IO.File.Move(l_tempFilePath, l_FilePath);
        }

    }
}
