using Omega_Red.Properties;
using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Omega_Red.Capture
{
    class BitRate
    {
        public uint Value { get; private set; }
        public string EncoderCLSID { get; private set; }

        public string EncoderModeCLSID { get; private set; }

        private string m_formatedValue = "";

        private string m_encoderName = "";

        public BitRate(uint a_bitrate, bool a_isVideo = true, string a_encoderCLSID="", string a_encoderModeCLSID = "", string a_encoderName="")
        {
            Value = a_bitrate;

            EncoderCLSID = a_encoderCLSID;

            EncoderModeCLSID = a_encoderModeCLSID;

            m_encoderName = a_encoderName;

            if (a_isVideo)
            {
                if(a_bitrate >= (1 << 20))
                {
                    m_formatedValue = (a_bitrate / (1 << 20)).ToString() + " Mbit";
                }
                else
                {
                    m_formatedValue = (a_bitrate / (1 << 10)).ToString() + " Kbit";
                }
            }
            else
            {
                m_formatedValue = (a_bitrate / (1000)).ToString() + " Kbit";
            }
        }
        
        public override string ToString()
        {
            string l_formatedValue = m_formatedValue;

            if (Value == 0)
            {
                l_formatedValue = "Variable bitrate";

                try
                {
                    var l_Title = new System.Windows.Controls.TextBlock();

                    l_Title.SetResourceReference(System.Windows.Controls.TextBlock.TextProperty, "VariableBitrateTitle");

                    l_formatedValue = l_Title.Text;
                }
                finally
                {
                }

            }

            if (!string.IsNullOrWhiteSpace(m_encoderName))
                l_formatedValue += " - " + m_encoderName;

            return l_formatedValue;
        }

        public override bool Equals(object obj)
        {
            var lBitRate = obj as BitRate;

            if (lBitRate == null)
                return false;

            return lBitRate.Value == Value;
        }
    }
    class MediaStream
    {
        private Action<bool> m_isConnected = null;

        private StringBuilder m_Version_XMLstring = new StringBuilder();

        private Assembly m_MediaStreamAssembly = null;

        private object m_StreamObj = null;

        private MethodInfo m_Start = null;

        private MethodInfo m_Stop = null;

        private MethodInfo m_GetVersion = null;

        private MethodInfo m_GetCollectionOfSources = null;

        private MethodInfo m_AddSource = null;

        private MethodInfo m_RemoveSource = null;

        private MethodInfo m_SetPosition = null;

        private MethodInfo m_SetOpacity = null;

        private MethodInfo m_SetRelativeVolume = null;



        private MethodInfo m_GetAudioBitRates = null;

        private MethodInfo m_GetVideoBitRates = null;




        private MethodInfo m_SetAudioBitRate = null;

        private MethodInfo m_SetVideoBitRate = null;





        private MethodInfo m_CurrentAvalableVideoMixers = null;

        private MethodInfo m_CurrentAvalableAudioMixers = null;

        private MethodInfo m_SetConnectionToken = null;





        private MethodInfo m_SetConnectionFunc = null;

        private MethodInfo m_SetDisconnectFunc = null;

        private MethodInfo m_SetWriteFunc = null;

        private MethodInfo m_SetIsConnectedFunc = null;

        


        private ICollectionView mVideoBitRateCustomerView = null;

        private readonly ObservableCollection<BitRate> _videoBitRateCollection = new ObservableCollection<BitRate>();





        private ICollectionView mAudioBitRateCustomerView = null;

        private readonly ObservableCollection<BitRate> _audioBitRateCollection = new ObservableCollection<BitRate>();


        public Action<string> Warning = null;


        private static MediaStream m_Instance = null;

        public static MediaStream Instance { get { if (m_Instance == null) m_Instance = new MediaStream(); return m_Instance; } }

        private MediaStream()
        {

            try
            {
                m_isConnected = isConnected;

                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                
                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.AnyCPU.MediaStream.dll"))
                {
                    if (lStream == null)
                        return;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    m_MediaStreamAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (m_MediaStreamAssembly != null)
                    {
                        Type l_StreamType = m_MediaStreamAssembly.GetType("MediaStream.Stream");

                        if (l_StreamType != null)
                        {
                            var lProperites = l_StreamType.GetProperties();

                            foreach (var item in lProperites)
                            {
                                if (item.Name.Contains("Instance"))
                                {
                                    m_StreamObj = item.GetMethod.Invoke(null, null);

                                    break;
                                }
                            }

                            m_Start = l_StreamType.GetMethod("start");

                            m_Stop = l_StreamType.GetMethod("stop");

                            m_GetVersion = l_StreamType.GetMethod("getVersion");

                            m_GetCollectionOfSources = l_StreamType.GetMethod("getCollectionOfSources");

                            m_AddSource = l_StreamType.GetMethod("addSource");

                            m_RemoveSource = l_StreamType.GetMethod("removeSource");

                            m_SetPosition = l_StreamType.GetMethod("setPosition");

                            m_SetOpacity = l_StreamType.GetMethod("setOpacity");

                            m_SetRelativeVolume = l_StreamType.GetMethod("setRelativeVolume");



                            m_GetAudioBitRates = l_StreamType.GetMethod("getAudioBitRates");

                            m_GetVideoBitRates = l_StreamType.GetMethod("getVideoBitRates");



                            m_SetVideoBitRate = l_StreamType.GetMethod("setVideoBitRate");

                            m_SetAudioBitRate = l_StreamType.GetMethod("setAudioBitRate");









                            m_CurrentAvalableVideoMixers = l_StreamType.GetMethod("currentAvalableVideoMixers");

                            m_CurrentAvalableAudioMixers = l_StreamType.GetMethod("currentAvalableAudioMixers");

                            m_SetConnectionToken = l_StreamType.GetMethod("setConnectionToken");



                            m_SetConnectionFunc = l_StreamType.GetMethod("setConnectionFunc");

                            m_SetDisconnectFunc = l_StreamType.GetMethod("setDisconnectFunc");

                            m_SetWriteFunc = l_StreamType.GetMethod("setWriteFunc");

                            m_SetIsConnectedFunc = l_StreamType.GetMethod("setIsConnectedFunc");

                            


                            m_GetVersion.Invoke(m_StreamObj, new object[] { m_Version_XMLstring });
                        }
                    }
                }
            }
            catch (System.Exception exc)
            {
                var f = exc.Message;
            }
        }

        public string CaptureManagerVersion { get { return m_Version_XMLstring.ToString(); } }                

        private string mFileExtention = "";

        public string FileExtention
        {
            get { return mFileExtention; }
        }

        public bool start(ref string a_resultMessage)
        {
            bool l_result = false;

            do
            {
                a_resultMessage = "VideoStreamingFailedTitle";
                
                if (m_StreamObj == null)
                    break;

                if (m_Start == null)
                    break;
                
                mFileExtention = m_Start.Invoke(m_StreamObj, new object[] {
                    TargetTexture.Instance.TargetNative.ToString(),
                    AudioCaptureTarget.Instance.RegisterAction,
                    m_isConnected
                }) as string;

                if (string.IsNullOrWhiteSpace(mFileExtention))
                    break;

                l_result = true;

                a_resultMessage = "VideoStreamingStartedTitle";

            } while (false);

            return l_result;
        }

        public bool stop(bool a_is_explicitly = false)
        {
            bool l_result = false;

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_Stop == null)
                    break;

                m_Stop.Invoke(m_StreamObj, new object[] {a_is_explicitly});
                   
                l_result = true;

            } while (false);

            return l_result;
        }

        private void isConnected(bool a_isConnectedState)
        {
            if(!a_isConnectedState)
            {
                Managers.MediaRecorderManager.Instance.StartStop(false);

                if (Warning != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        string l_title = "";

                        try
                        {
                            var l_Title = new System.Windows.Controls.TextBlock();

                            l_Title.SetResourceReference(System.Windows.Controls.TextBlock.TextProperty, "StreamConnectionWarningTitle");

                            l_title = l_Title.Text;
                        }
                        finally
                        {
                            Warning(l_title);
                        }
                    });
                }
            }
        }

        public string getCollectionOfSources()
        {
            string l_result = "";

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_GetCollectionOfSources == null)
                    break;

                StringBuilder l_Sources_XMLstring = new StringBuilder();

                m_GetCollectionOfSources.Invoke(m_StreamObj, new object[] { l_Sources_XMLstring });

                if (l_Sources_XMLstring.Length > 0)
                    l_result = l_Sources_XMLstring.ToString();

                uint lVideoBitRateMax = 0;

                uint lVideoBitRateMin = 0;

                getVideoBitRates(out lVideoBitRateMax, out lVideoBitRateMin);

                if (lVideoBitRateMax == 0 || lVideoBitRateMin == 0)
                    break;


                uint lAudioBitRateMax = 0;

                uint lAudioBitRateMin = 0;

                getAudioBitRates(out lAudioBitRateMax, out lAudioBitRateMin);

                if (lAudioBitRateMax == 0 || lAudioBitRateMin == 0)
                    break;


                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    if (mVideoBitRateCustomerView != null)
                    {
                        mVideoBitRateCustomerView.CurrentChanged += (object sender, EventArgs e)=>
                        {
                            if (mVideoBitRateCustomerView.CurrentItem == null)
                                return;

                            var l_SelectedVideoBitStream = (BitRate)mVideoBitRateCustomerView.CurrentItem;
                            
                            setVideoBitRate(l_SelectedVideoBitStream.Value);


                            Settings.Default.VideoBitRate = l_SelectedVideoBitStream.Value;
                        };
                    }

                    if (mAudioBitRateCustomerView != null)
                    {
                        mAudioBitRateCustomerView.CurrentChanged += (object sender, EventArgs e) =>
                        {
                            if (mAudioBitRateCustomerView.CurrentItem == null)
                                return;

                            var l_SelectedAudioBitStream = (BitRate)mAudioBitRateCustomerView.CurrentItem;
                            
                            setAudioBitRate(l_SelectedAudioBitStream.Value);


                            Settings.Default.AudioBitRate = l_SelectedAudioBitStream.Value;
                        };
                    }

                    _videoBitRateCollection.Clear();

                    do
                    {
                        _videoBitRateCollection.Add(new BitRate(lVideoBitRateMax));

                        lVideoBitRateMax = lVideoBitRateMax >> 1;

                        if (lVideoBitRateMin > lVideoBitRateMax)
                            break;

                    } while (true);
                    
                    if (mVideoBitRateCustomerView != null)
                        mVideoBitRateCustomerView.MoveCurrentTo(new BitRate(Settings.Default.VideoBitRate));

                    _audioBitRateCollection.Clear();

                    do
                    {
                        _audioBitRateCollection.Add(new BitRate(lAudioBitRateMax, false));

                        lAudioBitRateMax = lAudioBitRateMax >> 1;

                        if (lAudioBitRateMin > lAudioBitRateMax)
                            break;

                    } while (true);

                    if(mAudioBitRateCustomerView != null)
                        mAudioBitRateCustomerView.MoveCurrentTo(new BitRate(Settings.Default.AudioBitRate, false));
                });


            } while (false);

            return l_result;
        }
        
        public bool addSource(string a_SymbolicLink, uint a_MediaTypeIndex, IntPtr a_RenderTarget)
        {
            bool lresult = false;

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_AddSource == null)
                    break;

                lresult = (bool)m_AddSource.Invoke(m_StreamObj, new object[] { a_SymbolicLink, a_MediaTypeIndex, a_RenderTarget });

            } while (false);

            return lresult;
        }

        public void removeSource(string a_SymbolicLink)
        {
            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_RemoveSource == null)
                    break;

                m_RemoveSource.Invoke(m_StreamObj, new object[] { a_SymbolicLink });

            } while (false);
        }

        public void setPosition(string a_SymbolicLink, float aLeft, float aRight, float aTop, float aBottom)
        {
            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetPosition == null)
                    break;

                m_SetPosition.Invoke(m_StreamObj, new object[] { a_SymbolicLink, aLeft, aRight, aTop, aBottom });

            } while (false);
        }

        public int currentAvalableVideoMixers()
        {
            int lresult = 0;

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_CurrentAvalableVideoMixers == null)
                    break;

                lresult = (int)m_CurrentAvalableVideoMixers.Invoke(m_StreamObj, new object[] { });

            } while (false);

            return lresult;
        }

        public int currentAvalableAudioMixers()
        {
            int lresult = 0;

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_CurrentAvalableAudioMixers == null)
                    break;

                lresult = (int)m_CurrentAvalableAudioMixers.Invoke(m_StreamObj, new object[] { });

            } while (false);

            return lresult;
        }

        public void setConnectionToken(Tuple<string, string> a_Tuple)
        {

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetConnectionToken == null)
                    break;
                
                m_SetConnectionToken.Invoke(m_StreamObj, new object[] { a_Tuple.Item1, a_Tuple.Item2 });
                
            } while (false);
        }

        public void setConnectionFunc(RTMPNative.FirstDelegate a_FuncDelegate)
        {

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetConnectionFunc == null)
                    break;

                m_SetConnectionFunc.Invoke(m_StreamObj, new object[] { new Func<string, string, int>((a_streamsXml, a_url) =>{ return a_FuncDelegate(a_streamsXml, a_url); })});

            } while (false);
        }

        public void setDisconnectFunc(RTMPNative.SecondDelegate a_FuncDelegate)
        {

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetDisconnectFunc == null)
                    break;

                m_SetDisconnectFunc.Invoke(m_StreamObj, new object[] { new Action<int>((handler) => { a_FuncDelegate(handler); }) });

            } while (false);
        }

        public void setWriteFunc(RTMPNative.ThirdDelegate a_FuncDelegate)
        {

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetWriteFunc == null)
                    break;

                m_SetWriteFunc.Invoke(m_StreamObj, new object[] { new Action<int, int, IntPtr, int, uint, int, int>((
                    handler, sampleTime, buf, size, is_keyframe, streamIdx, isVideo) => { a_FuncDelegate(handler, sampleTime, buf, size, is_keyframe, streamIdx, isVideo); }) });

            } while (false);
        }

        public void setIsConnectedFunc(Func<int, bool> a_FuncDelegate)
        {

            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetIsConnectedFunc == null)
                    break;

                m_SetIsConnectedFunc.Invoke(m_StreamObj, new object[] { new Func<int, bool>((
                    handler) => { return a_FuncDelegate(handler); }) });

            } while (false);
        }

        public void setOpacity(string a_SymbolicLink, float a_value)
        {
            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetOpacity == null)
                    break;

                m_SetOpacity.Invoke(m_StreamObj, new object[] { a_SymbolicLink, a_value });

            } while (false);
        }

        public void setRelativeVolume(string a_SymbolicLink, float a_value)
        {
            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetRelativeVolume == null)
                    break;

                m_SetRelativeVolume.Invoke(m_StreamObj, new object[] { a_SymbolicLink, a_value });

            } while (false);
        }

        private void getAudioBitRates(out uint a_MaxBitRate, out uint a_MinBitRate)
        {
            do
            {

                a_MaxBitRate = 0;

                a_MinBitRate = 0;

                if (m_StreamObj == null)
                    break;

                if (m_GetAudioBitRates == null)
                    break;

                var lMaxMinBitRate = m_GetAudioBitRates.Invoke(m_StreamObj, new object[] {}) as Tuple<uint, uint>;

                if (lMaxMinBitRate == null)
                    break;

                a_MaxBitRate = lMaxMinBitRate.Item1;

                a_MinBitRate = lMaxMinBitRate.Item2;

            } while (false);
        }

        private void getVideoBitRates(out uint a_MaxBitRate, out uint a_MinBitRate)
        {
            do
            {
                a_MaxBitRate = 0;

                a_MinBitRate = 0;

                if (m_StreamObj == null)
                    break;

                if (m_GetVideoBitRates == null)
                    break;

                var lMaxMinBitRate = m_GetVideoBitRates.Invoke(m_StreamObj, new object[] { }) as Tuple<uint, uint>;

                if (lMaxMinBitRate == null)
                    break;

                a_MaxBitRate = lMaxMinBitRate.Item1;

                a_MinBitRate = lMaxMinBitRate.Item2;

            } while (false);
        }

        private void setVideoBitRate(uint a_selectedBitRate)
        {
            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetVideoBitRate == null)
                    break;

                m_SetVideoBitRate.Invoke(m_StreamObj, new object[] { a_selectedBitRate });

            } while (false);
        }

        private void setAudioBitRate(uint a_selectedBitRate)
        {
            do
            {
                if (m_StreamObj == null)
                    break;

                if (m_SetAudioBitRate == null)
                    break;

                m_SetAudioBitRate.Invoke(m_StreamObj, new object[] { a_selectedBitRate });

            } while (false);
        }

        public ICollectionView VideoBitRateCollection
        {
            get
            {
                if(mVideoBitRateCustomerView == null)
                    mVideoBitRateCustomerView = CollectionViewSource.GetDefaultView(_videoBitRateCollection);

                return mVideoBitRateCustomerView;
            }
        }

        public ICollectionView AudioBitRateCollection
        {
            get
            {
                if (mAudioBitRateCustomerView == null)
                    mAudioBitRateCustomerView = CollectionViewSource.GetDefaultView(_audioBitRateCollection);

                return mAudioBitRateCustomerView;
            }
        }
    }
}
