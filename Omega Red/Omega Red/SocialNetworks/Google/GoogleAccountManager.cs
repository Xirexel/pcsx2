using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omega_Red.Util;
using Omega_Red.Properties;
using System.Windows;
using System.Windows.Threading;
using Google.Apis.Services;
using File = Google.Apis.Drive.v3.Data.File;
using Upload = Google.Apis.Upload;
using Google.Apis.Upload;
using System.Windows.Controls;
using Google.Apis.Download;
using Omega_Red.Models;
using System.Globalization;
using Google.Apis.YouTube.v3.Data;

namespace Omega_Red.SocialNetworks.Google
{
    class GoogleAccountManager
    {
        private const int KB = 0x400;

        private const int ChunkSize = 256 * KB;

        public event Action<bool> mEnableStateEvent;

        private static bool mAuthorized = false;

        private UserCredential mCredential;

        private static GoogleAccountManager m_Instance = null;

        IDictionary<string, IList<SaveStateInfo>> m_Online_savings = new Dictionary<string, IList<SaveStateInfo>>();

        IList<SaveStateInfo> m_Current_online_savings = null;

        public static GoogleAccountManager Instance { get { if (m_Instance == null) m_Instance = new GoogleAccountManager(); return m_Instance; } }

        private GoogleAccountManager()
        {
        }

        public void sendEvent()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                if (mEnableStateEvent != null)
                    mEnableStateEvent(Settings.Default.GoogleAccountIsChecked);
            });
        }

        public async void tryAuthorize()
        {
            if (!mAuthorized)
            {
                try
                {
                    mAuthorized = await authorize();
                    
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                     {
                         if (mEnableStateEvent != null)
                         {
                             mEnableStateEvent(Settings.Default.GoogleAccountIsChecked);
                         }
                     });

                }
                catch (Exception)
                {
                    mAuthorized = false;
                }
            }
            else
            {
                try
                {
                    mAuthorized = await mCredential.RefreshTokenAsync(CancellationToken.None);

                    if(mAuthorized)
                    {
                        m_Online_savings.Clear();
                        
                        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                        {
                            if (mEnableStateEvent != null)
                            {
                                mEnableStateEvent(Settings.Default.GoogleAccountIsChecked);
                            }
                        });
                    }
                }
                catch (Exception)
                {
                    mAuthorized = false;
                }
            }
        }

        public bool isAuthorized { get { return mAuthorized; } }

        public void enbleAccount(bool a_status)
        {
            if (Settings.Default.GoogleAccountIsChecked != a_status)
            {
                Settings.Default.GoogleAccountIsChecked = a_status;

                Settings.Default.Save();

                if (a_status)
                    tryAuthorize();

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    if (mEnableStateEvent != null)
                        mEnableStateEvent(a_status);
                });
            }
        }

        private async Task<bool> authorize()
        {
            bool lresult = false;

            string lclient_secret = "";

            using (var stream = new StringStream(lclient_secret))
            {
                mCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] {
                        YouTubeService.Scope.YoutubeUpload,
                        YouTubeService.Scope.YoutubeForceSsl,
                        DriveService.Scope.DriveFile
                    },
                    Environment.UserName,
                    CancellationToken.None
                );
            }
            
            lresult = mCredential != null;

            return lresult;
        }

        public IList<SaveStateInfo> getSaveStateList(string a_disk_serial)
        {
            if (!m_Online_savings.Keys.Contains(a_disk_serial))
            {
                IList<SaveStateInfo> l_list = new List<SaveStateInfo>();

                do
                {
                    if (!mAuthorized)
                        break;

                    using (var service = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = mCredential,
                        ApplicationName = "Omega Red"
                    }))
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

            a_list.Remove(a_list.FirstOrDefault(saveState=> saveState.FilePath == l_SaveStateInfo.FilePath));

            a_list.Add(l_SaveStateInfo);
        }

        public async Task startUploadingAsync(string a_file_path, FrameworkElement a_banner_grid, string a_description)
        {
            if (!mAuthorized)
                return;

            using (var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = mCredential,
                ApplicationName = "Omega Red"
            }))
            {
                var lDirectory = getOmegaRedDirectory(service);

                if(lDirectory == null)
                {
                    return;
                }

                var lSStates = getSStates(service, lDirectory);

                if (lSStates == null)
                {
                    return;
                }

                if (System.IO.File.Exists(a_file_path))
                {
                    var lName = System.IO.Path.GetFileName(a_file_path);

                    FilesResource.ListRequest list = service.Files.List();

                    list.PageSize = 1000;
                    
                    list.Q = "mimeType != 'application/vnd.google-apps.folder' and trashed=false and name = '" + lName + "'" + " and '" + lSStates.Id + "' in parents";

                    FileList filesFeed = list.Execute();

                    a_banner_grid.Visibility = Visibility.Visible;

                    var lUploadProgressTitleTextBlock = a_banner_grid.FindName("mUploadProgressTitleTextBlock") as TextBlock;

                    lUploadProgressTitleTextBlock.Visibility = Visibility.Visible;

                    if (filesFeed.Files.Count == 0)
                    {
                        await createFile(service, a_file_path, a_description, lSStates.Id, a_banner_grid, lUploadProgressTitleTextBlock);
                    }
                    else
                        await updateFile(service, filesFeed.Files[0], a_file_path, a_description, lSStates.Id, a_banner_grid, lUploadProgressTitleTextBlock);                   
                }
                else
                {
                    MessageBox.Show("The file does not exist.", "404");
                }
            }
        }

        public async Task startDownloadingAsync(string a_file_path, FrameworkElement a_banner_grid)
        {
            if (!mAuthorized)
                return;

            using (var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = mCredential,
                ApplicationName = "Omega Red"
            }))
            {
                var lDirectory = getOmegaRedDirectory(service);

                if (lDirectory == null)
                {
                    return;
                }

                var lSStates = getSStates(service, lDirectory);

                if (lSStates == null)
                {
                    return;
                }


                var lName = System.IO.Path.GetFileName(a_file_path);

                FilesResource.ListRequest list = service.Files.List();

                list.PageSize = 1000;

                list.Q = "mimeType != 'application/vnd.google-apps.folder' and trashed=false and name = '" + lName + "'" + " and '" + lSStates.Id + "' in parents";

                //string allFields = "appProperties,capabilities,contentHints,createdTime,description,explicitlyTrashed,fileExtension,folderColorRgb,fullFileExtension,headRevisionId,iconLink,id,imageMediaMetadata,isAppAuthorized,kind,lastModifyingUser,md5Checksum,mimeType,modifiedByMeTime,modifiedTime,name,originalFilename,ownedByMe,owners,parents,permissions,properties,quotaBytesUsed,shared,sharedWithMeTime,sharingUser,size,spaces,starred,thumbnailLink,trashed,version,videoMediaMetadata,viewedByMe,viewedByMeTime,viewersCanCopyContent,webContentLink,webViewLink,writersCanShare";

                list.Fields = "nextPageToken, files(id, name, size)";

                FileList filesFeed = list.Execute();

                if (filesFeed.Files.Count > 0)
                {
                    a_banner_grid.Visibility = Visibility.Visible;

                    var lDownloadProgressTitleTextBlock = a_banner_grid.FindName("mDownloadProgressTitleTextBlock") as TextBlock;

                    lDownloadProgressTitleTextBlock.Visibility = Visibility.Visible;

                    await downloadFile(service, filesFeed.Files[0], a_banner_grid, lDownloadProgressTitleTextBlock);
                }
            }
        }

        public async Task startDeletingAsync(string a_file_path)
        {
            if (!mAuthorized)
                return;

            using (var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = mCredential,
                ApplicationName = "Omega Red"
            }))
            {
                var lDirectory = getOmegaRedDirectory(service);

                if (lDirectory == null)
                {
                    return;
                }

                var lSStates = getSStates(service, lDirectory);

                if (lSStates == null)
                {
                    return;
                }
                
                var lName = System.IO.Path.GetFileName(a_file_path);

                FilesResource.ListRequest list = service.Files.List();

                list.PageSize = 1000;

                list.Q = "mimeType != 'application/vnd.google-apps.folder' and trashed=false and name = '" + lName + "'" + " and '" + lSStates.Id + "' in parents";

                list.Fields = "nextPageToken, files(id)";

                FileList filesFeed = list.Execute();

                if (filesFeed.Files.Count > 0)
                {
                    await service.Files.Delete(filesFeed.Files[0].Id).ExecuteAsync();

                    if(m_Current_online_savings != null)
                        m_Current_online_savings.Remove(m_Current_online_savings.First(saveState => saveState.FilePath == a_file_path));
                }
            }
        }

        public async Task startUploadingYouTubeVideoAsync(string a_file_path,
                                            string a_title,
                                            string a_description,
                                            string[] a_tags,
                                            string a_categoryId,
                                            string a_privacyStatus,
                                            Action<float> a_progress_callback)
        {
            if (!mAuthorized)
                return;

            using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = mCredential,
                ApplicationName = "Omega Red"
            }))
            {
                var video = new Video();
                video.Snippet = new VideoSnippet();
                video.Snippet.Title = a_title;
                video.Snippet.Description = a_description;
                video.Snippet.Tags = a_tags;
                video.Snippet.CategoryId = a_categoryId;
                video.Status = new VideoStatus();
                video.Status.PrivacyStatus = a_privacyStatus;

                var lFile_length = new FileInfo(a_file_path).Length;
                
                using (var fileStream = new System.IO.FileStream(a_file_path,
                    System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                    videosInsertRequest.ChunkSize = ResumableUpload.MinimumChunkSize;

                    videosInsertRequest.ProgressChanged += (obj) =>
                    {
                        switch (obj.Status)
                        {
                            case UploadStatus.Starting:
                            case UploadStatus.Uploading:
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                                {
                                    a_progress_callback((float)obj.BytesSent * 100.0f / (float)lFile_length);
                                });
                                break;
                            default:
                                break;
                        }
                    };

                    await videosInsertRequest.UploadAsync();
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

        private Task<IUploadProgress> createFile(
            DriveService service, 
            string a_file_path, 
            string a_description, 
            string a_parent_id,
            FrameworkElement a_banner_grid, 
            TextBlock a_title_textblock)
        {

            var lProgressTextBlock = a_banner_grid.FindName("mProgressTextBlock") as TextBlock;


            File body = new File();
            body.Name = System.IO.Path.GetFileName(a_file_path);
            body.Description = a_description;
            body.MimeType = GetMimeType(a_file_path);
            body.Parents = new List<string>() { a_parent_id };
      
            byte[] byteArray = System.IO.File.ReadAllBytes(a_file_path);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
            try
            {
                var request = service.Files.Create(body, stream, GetMimeType(a_file_path));

                request.ChunkSize = ResumableUpload.MinimumChunkSize;

                request.ProgressChanged += (obj)=>
                {
                    switch (obj.Status)
                    {
                        case UploadStatus.Starting:
                        case UploadStatus.Uploading:
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                            {
                                lProgressTextBlock.Text = string.Format("{0:0.00} %", (float)obj.BytesSent * 100.0 / (float)byteArray.Length);
                            });
                            break;
                        case UploadStatus.Completed:
                            {
                                addSaveState(m_Current_online_savings, body.Name, body.Description);

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

                request.ResponseReceived += (File obj)=>
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        a_banner_grid.Visibility = Visibility.Collapsed;
                    });
                };

                var task = request.UploadAsync();

                task.ContinueWith(t =>
                {
                }, TaskContinuationOptions.NotOnRanToCompletion);
                task.ContinueWith(t =>
                {
                    stream.Dispose();
                });
                
                return task;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error Occured");
            }

            return null;
        }

        private Task<IUploadProgress> updateFile(DriveService service, File originalFile, string a_file_path, string a_description, string a_parent_id, FrameworkElement a_banner_grid, TextBlock a_title_textblock)
        {

            var lProgressTextBlock = a_banner_grid.FindName("mProgressTextBlock") as TextBlock;

            lProgressTextBlock.Text = "";

            File body = new File();
            body.Name = System.IO.Path.GetFileName(a_file_path);
            body.Description = a_description;
            body.MimeType = GetMimeType(a_file_path);            


            byte[] byteArray = System.IO.File.ReadAllBytes(a_file_path);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
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
                                lProgressTextBlock.Text = string.Format("{0:0.00} %", (float)obj.BytesSent * 100.0 / (float)byteArray.Length);
                            });
                            break;
                        case UploadStatus.Completed:
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                            {
                                addSaveState(m_Current_online_savings, body.Name, body.Description);

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

                task.ContinueWith(t =>
                {
                    // NotOnRanToCompletion - this code will be called if the upload fails
                    Console.WriteLine("Upload Failed. " + t.Exception);
                }, TaskContinuationOptions.NotOnRanToCompletion);
                task.ContinueWith(t =>
                {
                    stream.Dispose();
                });


                return task;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error Occured");
            }

            return null;
        }

        private async Task downloadFile(DriveService service, File downloadFile, FrameworkElement a_banner_grid, TextBlock a_title_textblock)
        {
            var lProgressTextBlock = a_banner_grid.FindName("mProgressTextBlock") as TextBlock;

            lProgressTextBlock.Text = "";

            var downloader = service.Files.Get(downloadFile.Id);

            var mediaDownloader = downloader.MediaDownloader;

            if(mediaDownloader != null)
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

            var l_tempFilePath = Settings.Default.SlotFolder + downloadFile.Name + "_downloaded";

            var l_FilePath = Settings.Default.SlotFolder + downloadFile.Name;

            using (var fileStream = new System.IO.FileStream(l_tempFilePath,
                System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {    
                await downloader.DownloadAsync(fileStream);
            }

            if (System.IO.File.Exists(l_FilePath))
                System.IO.File.Delete(l_FilePath);

            System.IO.File.Move(l_tempFilePath, l_FilePath);
        }


        public Managers.IStreamingManager createYouTubeManager()
        {
            return new YouTubeManager(new BaseClientService.Initializer()
            {
                HttpClientInitializer = mCredential,
                ApplicationName = "Omega Red"
            });
        }

    }
}
