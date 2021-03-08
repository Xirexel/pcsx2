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
                        DriveManager.Instance.reset();

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

                if (a_status)
                    tryAuthorize();

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    if (mEnableStateEvent != null)
                        mEnableStateEvent(a_status);
                });
            }
        }

        public BaseClientService.Initializer getInitializer()
        {
            BaseClientService.Initializer l_initializer = null;

            do
            {
                if (!mAuthorized)
                    break;

                l_initializer = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = mCredential,
                    ApplicationName = "Omega Red"
                };

            } while (false);

            return l_initializer;
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

            if(lresult)
                lresult = await mCredential.RefreshTokenAsync(CancellationToken.None);

            return lresult;
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


        public Managers.IStreamingManager createYouTubeManager()
        {
            return new YouTubeManager(GoogleAccountManager.Instance.getInitializer());
        }

    }
}
