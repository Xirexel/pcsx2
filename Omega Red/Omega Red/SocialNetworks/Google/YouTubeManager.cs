using Google.Apis.YouTube.v3;
using Services = Google.Apis.Services;
using YouTube = Google.Apis.YouTube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omega_Red.ViewModels;
using Omega_Red.Panels;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Omega_Red.Models;
using System.Windows.Input;
using System.Windows;
using Omega_Red.Managers;
using Omega_Red.Properties;
using System.Windows.Threading;
using System.Threading;

namespace Omega_Red.SocialNetworks.Google
{
    class YouTubeManager: Managers.IStreamingManager
    {
        private YouTubeService m_YoutubeService = null;

        private ICollectionView mCustomerStreamsView = null;
        
        private readonly ObservableCollection<YouTubeStreamInfo> _youTubeStreamInfoCollection = new ObservableCollection<YouTubeStreamInfo>();


        private YouTube.v3.Data.LiveStream m_currentLiveStream = null;

        private YouTube.v3.Data.LiveBroadcast m_currentLiveBroadcast = null;


        private ICollectionView mCustomerBroadcastsView = null;

        private readonly ObservableCollection<YouTubeBroadcastInfo> _youTubeBroadcastInfoCollection = new ObservableCollection<YouTubeBroadcastInfo>();

        public event Action<bool> m_IsAllowedCofirmEvent;

        public object Panel { get; private set; }

        public BaseViewModel Streams { get; private set; }

        public BaseViewModel Broadcasts { get; private set; }

        public YouTubeManager(Services.BaseClientService.Initializer a_Initializer)
        {
            mCustomerStreamsView = CollectionViewSource.GetDefaultView(_youTubeStreamInfoCollection);

            mCustomerBroadcastsView = CollectionViewSource.GetDefaultView(_youTubeBroadcastInfoCollection);

            m_YoutubeService = new YouTubeService(a_Initializer);



            var l_panel = new YouTubeStreamControl();

            Streams = new YouTubeStreamViewModel(new YouTubeStreamManager(this));

            l_panel.DataContext = this;

            Panel = l_panel;
            

            Broadcasts = new YouTubeBroadcastViewModel(new YouTubeBroadcastManager(this));


            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                updateLiveBroadcasts();

                updateLiveStreamList();
            });
        }
        
        private void checkAllow()
        {
            if (m_IsAllowedCofirmEvent == null)
                return;

            if (m_currentLiveStream != null && m_currentLiveBroadcast != null)
                m_IsAllowedCofirmEvent(true);
            else
                m_IsAllowedCofirmEvent(false);
        }

        private void updateLiveStreamList()
        {
            if (m_YoutubeService == null)
                return;
            
            var l_list = m_YoutubeService.LiveStreams.List("snippet,cdn,contentDetails,status");

            l_list.Mine = true;

            var response = l_list.Execute();

            if (response != null)
            {
                if (response.Items.Count > 0)
                {
                    foreach (var l_item in response.Items)
                    {
                        var l_YouTubeStreamInfo = new YouTubeStreamInfo()
                        {
                            Title = l_item.Snippet.Title,
                            Description = l_item.Snippet.Description,
                            Id = l_item.Id,
                            Stream = l_item,
                            IsCreated = false
                        };

                        _youTubeStreamInfoCollection.Add(l_YouTubeStreamInfo);

                        if(Settings.Default.YouTubeStreamSelectedId == l_item.Id)
                        {
                            mCustomerStreamsView.MoveCurrentTo(l_YouTubeStreamInfo);

                            m_currentLiveStream = l_item;
                        }
                    }
                }
            }

            checkAllow();
        }

        private void updateLiveBroadcasts()
        {
            if (m_YoutubeService == null)
                return;


            var l_list = m_YoutubeService.LiveBroadcasts.List("snippet,contentDetails,status");

            l_list.Mine = true;

            var response = l_list.Execute();

            if (response != null)
            {
                if (response.Items.Count > 0)
                {
                    foreach (var l_item in response.Items)
                    {
                        var l_YouTubeBroadcastInfo = new YouTubeBroadcastInfo()
                        {
                            Title = l_item.Snippet.Title,
                            Description = l_item.Snippet.Description,
                            Id = l_item.Id,
                            Broadcast = l_item,
                            IsCreated = false,
                            PrivacyStatus = l_item.Status.PrivacyStatus
                        };
                                                
                        _youTubeBroadcastInfoCollection.Add(l_YouTubeBroadcastInfo);
                        
                        if (Settings.Default.YouTubeBroadcastSelectedId == l_item.Id)
                        {
                            mCustomerBroadcastsView.MoveCurrentTo(l_YouTubeBroadcastInfo);

                            m_currentLiveBroadcast = l_item;
                        }
                    }
                }
            }

            checkAllow();
        }

        public void Dispose()
        {
            if (m_YoutubeService != null)
                m_YoutubeService.Dispose();

            m_YoutubeService = null;
        }
        public ICollectionView StreamCollection => mCustomerStreamsView;

        public void addStream()
        {
            _youTubeStreamInfoCollection.Add(new YouTubeStreamInfo() {
                Title = "New Stream",
                Description = "",
                IsCreated =true });
        }

        public void persistStream(YouTubeStreamInfo a_stream)
        {
            do
            {
                if (a_stream == null)
                    break;

                if(a_stream.IsCreated)
                {
                    a_stream.IsCreated = createStream(a_stream);
                }
                else
                {
                    updateStream(a_stream);
                }
                
            } while (false);

            checkAllow();
        }

        private bool createStream(YouTubeStreamInfo a_stream)
        {
            bool lresult = true;

            do
            {
                try
                {

                    YouTube.v3.Data.LiveStream l_liveStream = new YouTube.v3.Data.LiveStream();


                    l_liveStream.Snippet = new YouTube.v3.Data.LiveStreamSnippet();

                    l_liveStream.Snippet.Description = a_stream.Description;

                    l_liveStream.Snippet.Title = a_stream.Title;

                    l_liveStream.Kind = "youtube#liveStream";

                    l_liveStream.Cdn = new YouTube.v3.Data.CdnSettings();

                    l_liveStream.Cdn.IngestionType = "rtmp";

                    l_liveStream.Cdn.Resolution = "720p";

                    l_liveStream.Cdn.FrameRate = "30fps";


                    var request = m_YoutubeService.LiveStreams.Insert(l_liveStream, "snippet,cdn,contentDetails,status");

                    var lstream = request.Execute();

                    if (lstream != null)
                    {
                        a_stream.Id = lstream.Id;

                        a_stream.Stream = lstream;

                        m_currentLiveStream = lstream;

                        lresult = false;
                    }
                }
                catch (Exception)
                {
                }

            } while (false);

            checkAllow();

            return lresult;
        }

        private bool updateStream(YouTubeStreamInfo a_stream)
        {
            bool lresult = true;

            do
            {
                try
                {

                    YouTube.v3.Data.LiveStream l_liveStream = a_stream.Stream;

                    
                    l_liveStream.Snippet.Description = a_stream.Description;

                    l_liveStream.Snippet.Title = a_stream.Title;
                    

                    var request = m_YoutubeService.LiveStreams.Update(l_liveStream, "snippet,cdn,contentDetails,status");

                    var lstream = request.Execute();

                    if (lstream != null)
                    {
                        a_stream.Id = lstream.Id;

                        a_stream.Stream = lstream;

                        m_currentLiveStream = lstream;

                        lresult = false;
                    }
                }
                catch (Exception)
                {
                }

            } while (false);

            checkAllow();

            return lresult;
        }

        public bool deleteStream(YouTubeStreamInfo a_stream)
        {
            bool lresult = true;

            do
            {
                try
                {
                    var request = m_YoutubeService.LiveStreams.Delete(a_stream.Id);

                    var lstream = request.Execute();
                    
                    _youTubeStreamInfoCollection.Remove(a_stream);

                    mCustomerStreamsView.MoveCurrentToPosition(-1);

                    m_currentLiveStream = null;
                }
                catch (Exception)
                {
                }

                checkAllow();

            } while (false);

            return lresult;
        }

        public ICollectionView BroadcastCollection => mCustomerBroadcastsView;


        public void addBroadcast()
        {
            _youTubeBroadcastInfoCollection.Add(new YouTubeBroadcastInfo() {
                Title = "New Broadcast",
                Description = "",
                IsCreated = true,
                PrivacyStatus = "private" });
        }


        public void persistBroadcast(YouTubeBroadcastInfo a_broadcast)
        {
            do
            {
                if (a_broadcast == null)
                    break;

                if (a_broadcast.IsCreated)
                {
                    a_broadcast.IsCreated = createBroadcast(a_broadcast);
                }
                else
                {
                    updateBroadcast(a_broadcast);
                }
                
                Settings.Default.YouTubeBroadcastSelectedId = a_broadcast.Id;

            } while (false);

            checkAllow();
        }

        private bool createBroadcast(YouTubeBroadcastInfo a_broadcast)
        {
            bool lresult = true;

            do
            {
                try
                {

                    YouTube.v3.Data.LiveBroadcast l_liveBroadcast = new YouTube.v3.Data.LiveBroadcast();


                    l_liveBroadcast.Snippet = new YouTube.v3.Data.LiveBroadcastSnippet();

                    l_liveBroadcast.Snippet.Description = a_broadcast.Description;

                    l_liveBroadcast.Snippet.Title = a_broadcast.Title;

                    //l_liveBroadcast.Snippet.ScheduledStartTime = DateTime.Now;



                    l_liveBroadcast.Status = new YouTube.v3.Data.LiveBroadcastStatus();

                    l_liveBroadcast.Status.PrivacyStatus = a_broadcast.PrivacyStatus;
                    

                    l_liveBroadcast.Kind = "youtube#liveBroadcast";
                    

                    var request = m_YoutubeService.LiveBroadcasts.Insert(l_liveBroadcast, "snippet,contentDetails,status");

                    var lbroadcast = request.Execute();

                    if (lbroadcast != null)
                    {
                        a_broadcast.Id = lbroadcast.Id;

                        a_broadcast.Broadcast = lbroadcast;

                        m_currentLiveBroadcast = lbroadcast;

                        lresult = false;
                    }
                }
                catch (Exception)
                {
                }

            } while (false);

            checkAllow();

            return lresult;
        }

        private bool updateBroadcast(YouTubeBroadcastInfo a_broadcast)
        {
            bool lresult = true;

            do
            {
                try
                {

                    var l_Broadcast = a_broadcast.Broadcast;


                    l_Broadcast.Snippet.Description = a_broadcast.Description;

                    l_Broadcast.Snippet.Title = a_broadcast.Title;


                    var request = m_YoutubeService.LiveBroadcasts.Update(l_Broadcast, "snippet,contentDetails,status");

                    var lbroadcast = request.Execute();

                    if (lbroadcast != null)
                    {
                        a_broadcast.Id = lbroadcast.Id;

                        a_broadcast.Broadcast = lbroadcast;

                        lresult = false;

                        m_currentLiveBroadcast = lbroadcast;
                    }
                }
                catch (Exception)
                {
                }

            } while (false);

            return lresult;
        }

        public bool deleteBroadcast(YouTubeBroadcastInfo a_broadcast)
        {
            bool lresult = true;

            do
            {
                try
                {
                    if(!a_broadcast.IsCreated)
                    {
                        var request = m_YoutubeService.LiveBroadcasts.Delete(a_broadcast.Id);

                        var lbroadcast = request.Execute();
                    }

                    _youTubeBroadcastInfoCollection.Remove(a_broadcast);

                    mCustomerBroadcastsView.MoveCurrentToPosition(-1);

                    m_currentLiveBroadcast = null;
                }
                catch (Exception)
                {
                }

            } while (false);

            checkAllow();

            return lresult;
        }

        public Tuple<string, string> getConnectionToken()
        {
            string l_IngestionAddress = "";

            string l_StreamName = "";

            if(m_currentLiveStream != null && m_currentLiveBroadcast != null)
            {
                do
                {
                    try
                    {
                        var request = m_YoutubeService.LiveBroadcasts.Bind(m_currentLiveBroadcast.Id, "snippet");
                        
                        request.StreamId = m_currentLiveStream.Id;
                        
                        var lbroadcast = request.Execute();

                        if (lbroadcast != null)
                        {
                            l_IngestionAddress = m_currentLiveStream.Cdn.IngestionInfo.IngestionAddress;

                            l_StreamName = m_currentLiveStream.Cdn.IngestionInfo.StreamName;
                            
                            checkState();
                        }
                    }
                    catch (Exception)
                    {
                    }

                } while (false);
            }

            return Tuple.Create<string, string>(l_IngestionAddress, l_StreamName);
        }

        private void checkState()
        {
            if (m_YoutubeService == null)
                return;

            try
            {
                var l_list = m_YoutubeService.LiveStreams.List("snippet,contentDetails,status");

                l_list.Mine = true;

                var response = l_list.Execute();

                if (response != null)
                {
                    if (response.Items.Count > 0)
                    {
                        foreach (var l_item in response.Items)
                        {
                            if (l_item.Id == m_currentLiveStream.Id)
                            {
                                if (l_item.Status.StreamStatus == "active")
                                {
                                    //var ltransition = m_YoutubeService.LiveBroadcasts.Transition(LiveBroadcastsResource.TransitionRequest.BroadcastStatusEnum.Live, m_currentLiveBroadcast.Id, "snippet,status");

                                    //var lbroadcast = ltransition.Execute();

                                    return;
                                }

                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            finally
            {

            }

            ThreadPool.QueueUserWorkItem((state) => {
                checkState();
            });
        }
    }
}
