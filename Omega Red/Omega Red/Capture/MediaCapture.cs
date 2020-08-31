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

using Omega_Red.Properties;
using Omega_Red.Util.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Omega_Red.Capture
{
    class MediaCapture
    {
        private StringBuilder m_Version_XMLstring = new StringBuilder();

        private int m_fileSizeMb = -1;

        private Timer m_CheckSize_Timer = null;

        private Assembly m_MediaCaptureAssembly = null;

        private object m_CaptureObj = null;

        private MethodInfo m_Start = null;

        private MethodInfo m_Stop = null;

        private MethodInfo m_GetVersion = null;

        private MethodInfo m_GetCollectionOfSources = null;

        private MethodInfo m_AddSource = null;

        private MethodInfo m_RemoveSource = null;

        private MethodInfo m_SetPosition = null;

        private MethodInfo m_SetOpacity = null;

        private MethodInfo m_SetRelativeVolume = null;

        private MethodInfo m_CurrentAvalableVideoMixers = null;

        private MethodInfo m_CurrentAvalableAudioMixers = null;

        private MethodInfo m_GetAudioBitRates = null;

        private MethodInfo m_GetVideoBitRates = null;

        private MethodInfo m_SetAudioBitRate = null;

        private MethodInfo m_SetVideoBitRate = null;

        private MethodInfo m_Restart = null;

        private string m_TempFileName = "";
                
        private static MediaCapture m_Instance = null;




        private ICollectionView mVideoBitRateCustomerView = null;

        private readonly ObservableCollection<BitRate> _videoBitRateCollection = new ObservableCollection<BitRate>();





        private ICollectionView mAudioBitRateCustomerView = null;

        private readonly ObservableCollection<BitRate> _audioBitRateCollection = new ObservableCollection<BitRate>();

        
        public static MediaCapture Instance { get { if (m_Instance == null) m_Instance = new MediaCapture(); return m_Instance; } }

        private MediaCapture()
        {

            try
            {
                var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                
                using (var lStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.AnyCPU.MediaCapture.dll"))
                {
                    if (lStream == null)
                        return;

                    byte[] buffer = new byte[(int)lStream.Length];

                    lStream.Read(buffer, 0, buffer.Length);

                    m_MediaCaptureAssembly = AppDomain.CurrentDomain.Load(buffer);

                    if (m_MediaCaptureAssembly != null)
                    {
                        Type l_CaptureType = m_MediaCaptureAssembly.GetType("MediaCapture.Capture");
                        
                        if (l_CaptureType != null)
                        {
                            var lProperites = l_CaptureType.GetProperties();

                            foreach (var item in lProperites)
                            {
                                if (item.Name.Contains("Instance"))
                                {
                                    m_CaptureObj = item.GetMethod.Invoke(null, null);

                                    break;
                                }
                            }                            

                            m_Start = l_CaptureType.GetMethod("start");

                            m_Stop = l_CaptureType.GetMethod("stop");

                            m_GetVersion = l_CaptureType.GetMethod("getVersion");

                            m_GetCollectionOfSources = l_CaptureType.GetMethod("getCollectionOfSources");

                            m_AddSource = l_CaptureType.GetMethod("addSource");

                            m_RemoveSource = l_CaptureType.GetMethod("removeSource");

                            m_SetPosition = l_CaptureType.GetMethod("setPosition");

                            m_SetOpacity = l_CaptureType.GetMethod("setOpacity");

                            m_SetRelativeVolume = l_CaptureType.GetMethod("setRelativeVolume");


                            m_CurrentAvalableVideoMixers = l_CaptureType.GetMethod("currentAvalableVideoMixers");

                            m_CurrentAvalableAudioMixers = l_CaptureType.GetMethod("currentAvalableAudioMixers");
                                                                                 


                            m_GetAudioBitRates = l_CaptureType.GetMethod("getAudioBitRates");

                            m_GetVideoBitRates = l_CaptureType.GetMethod("getVideoBitRates");



                            m_SetVideoBitRate = l_CaptureType.GetMethod("setVideoBitRate");

                            m_SetAudioBitRate = l_CaptureType.GetMethod("setAudioBitRate");



                            m_Restart = l_CaptureType.GetMethod("restart");
                            



                            m_GetVersion.Invoke(m_CaptureObj, new object[] { m_Version_XMLstring });
                        }
                    }
                }
                
                m_CheckSize_Timer.Dispose();
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
                a_resultMessage = "VideoFileRecordingFailedTitle";
                
                m_TempFileName = System.IO.Path.GetTempFileName();

                if (string.IsNullOrEmpty(m_TempFileName))
                    break;

                if (m_CaptureObj == null)
                    break;

                if (m_Start == null)
                    break;

                mFileExtention = m_Start.Invoke(m_CaptureObj, new object[] {
                    TargetTexture.Instance.TargetNative.ToString(),
                    AudioCaptureTarget.Instance.RegisterAction,
                    m_TempFileName,
                    Omega_Red.Properties.Settings.Default.CompressionQuality}) as string;

                if (string.IsNullOrWhiteSpace(mFileExtention))
                    break;

                l_result = true;

                if(m_fileSizeMb > 0)
                {
                    startCheckSizeTimer();
                }

                a_resultMessage = "VideoFileRecordingStartedTitle";

            } while (false);

            return l_result;
        }

        public bool stop()
        {
            bool l_result = false;

            do
            {
                if (string.IsNullOrEmpty(m_TempFileName))
                    break;

                if (m_CaptureObj == null)
                    break;

                if (m_Stop == null)
                    break;

                if(m_CheckSize_Timer != null)
                {
                    m_CheckSize_Timer.Dispose();

                    m_CheckSize_Timer = null;
                }



                m_Stop.Invoke(m_CaptureObj, null);

                if (!string.IsNullOrEmpty(m_TempFileName))
                {
                    Thread.Sleep(1000);

                    Omega_Red.Managers.MediaRecorderManager.Instance.createItem(m_TempFileName);

                    m_TempFileName = "";
                }

                l_result = true;

            } while (false);

            return l_result;
        }

        public string getCollectionOfSources()
        {
            string l_result = "";

            do
            {                
                if (m_CaptureObj == null)
                    break;

                if (m_GetCollectionOfSources == null)
                    break;
                               
                StringBuilder l_Sources_XMLstring = new StringBuilder();

                m_GetCollectionOfSources.Invoke(m_CaptureObj, new object[] { l_Sources_XMLstring });

                if (l_Sources_XMLstring.Length > 0)
                    l_result = l_Sources_XMLstring.ToString();



                var l_VideoBitRateList = getVideoBitRates();
                                
                var l_AudioBitRate = getAudioBitRates();

                if (l_VideoBitRateList == null || l_VideoBitRateList.Count == 0)
                    break;

                if (l_AudioBitRate == null)
                    break;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    if (mVideoBitRateCustomerView != null)
                    {
                        mVideoBitRateCustomerView.CurrentChanged += (object sender, EventArgs e) =>
                        {
                            if (mVideoBitRateCustomerView.CurrentItem == null)
                                return;

                            var l_SelectedVideoBitStream = (BitRate)mVideoBitRateCustomerView.CurrentItem;

                            setVideoBitRate(l_SelectedVideoBitStream.Value, l_SelectedVideoBitStream.EncoderCLSID, l_SelectedVideoBitStream.EncoderModeCLSID);


                            Settings.Default.RecordingVideoBitRate = l_SelectedVideoBitStream.Value;
                        };
                    }

                    if (mAudioBitRateCustomerView != null)
                    {
                        mAudioBitRateCustomerView.CurrentChanged += (object sender, EventArgs e) =>
                        {
                            if (mAudioBitRateCustomerView.CurrentItem == null)
                                return;

                            var l_SelectedAudioBitStream = (BitRate)mAudioBitRateCustomerView.CurrentItem;

                            setAudioBitRate(l_SelectedAudioBitStream.Value, l_SelectedAudioBitStream.EncoderModeCLSID);


                            Settings.Default.RecordingAudioBitRate = l_SelectedAudioBitStream.Value;
                        };
                    }

                    _videoBitRateCollection.Clear();

                    do
                    {
                        if (l_VideoBitRateList == null ||
                            l_VideoBitRateList.Count == 0)
                            break;

                        foreach (var l_VideoBitRate in l_VideoBitRateList)
                        {
                            var lVideoBitRateMin = l_VideoBitRate.Item1.Item2;

                            var lVideoBitRateMax = l_VideoBitRate.Item1.Item1;


                            do
                            {
                                _videoBitRateCollection.Add(new BitRate(lVideoBitRateMax, true, l_VideoBitRate.Item2.Item1, l_VideoBitRate.Item1.Item3, l_VideoBitRate.Item2.Item2));

                                lVideoBitRateMax = lVideoBitRateMax >> 1;

                                if (lVideoBitRateMin > lVideoBitRateMax)
                                    break;

                                if ((lVideoBitRateMin == lVideoBitRateMax) && (lVideoBitRateMin == 0))
                                    break;

                            } while (true);
                        }

                    } while (false);

                    if (mVideoBitRateCustomerView != null)
                    {
                        if (!mVideoBitRateCustomerView.MoveCurrentTo(new BitRate(Settings.Default.RecordingVideoBitRate))
                        && _videoBitRateCollection.Count > 0)
                            mVideoBitRateCustomerView.MoveCurrentTo(_videoBitRateCollection.First());
                    }

                    _audioBitRateCollection.Clear();

                    do
                    {

                        var l_AudioBitRateMin = l_AudioBitRate.Item2;

                        var l_AudioBitRateMax = l_AudioBitRate.Item1;


                        do
                        {
                            _audioBitRateCollection.Add(new BitRate(l_AudioBitRateMax, false, "", l_AudioBitRate.Item3));

                            l_AudioBitRateMax = l_AudioBitRateMax >> 1;

                            if (l_AudioBitRateMin > l_AudioBitRateMax)
                                break;

                            if ((l_AudioBitRateMin == l_AudioBitRateMax) && (l_AudioBitRateMin == 0))
                                break;

                        } while (true);

                        //_audioBitRateCollection.Add(new BitRate(lAudioBitRateMax, false));

                        //lAudioBitRateMax = lAudioBitRateMax >> 1;

                        //if (lAudioBitRateMin > lAudioBitRateMax)
                        //    break;

                    } while (false);

                    if (mAudioBitRateCustomerView != null)
                    {
                        if (!mAudioBitRateCustomerView.MoveCurrentTo(new BitRate(Settings.Default.RecordingAudioBitRate, false))
                        && _audioBitRateCollection.Count > 0)
                            mAudioBitRateCustomerView.MoveCurrentTo(_audioBitRateCollection.First());   
                    }
                });


            } while (false);

            return l_result;
        }

        public bool addSource(string a_SymbolicLink, uint a_MediaTypeIndex, IntPtr a_RenderTarget)
        {
            bool lresult = false;

            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_AddSource == null)
                    break;

                lresult = (bool)m_AddSource.Invoke(m_CaptureObj, new object[] { a_SymbolicLink, a_MediaTypeIndex, a_RenderTarget });
                
            } while (false);

            return lresult;
        }

        public void removeSource(string a_SymbolicLink)
        {
            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_RemoveSource == null)
                    break;

                m_RemoveSource.Invoke(m_CaptureObj, new object[] { a_SymbolicLink });
                
            } while (false);
        }

        public void setPosition(string a_SymbolicLink, float aLeft, float aRight, float aTop, float aBottom)
        {
            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_SetPosition == null)
                    break;

                m_SetPosition.Invoke(m_CaptureObj, new object[] { a_SymbolicLink, aLeft, aRight, aTop, aBottom });

            } while (false);
        }

        public int currentAvalableVideoMixers()
        {
            int lresult = 0;

            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_CurrentAvalableVideoMixers == null)
                    break;

                lresult = (int)m_CurrentAvalableVideoMixers.Invoke(m_CaptureObj, new object[] { });

            } while (false);

            return lresult;
        }

        public int currentAvalableAudioMixers()
        {
            int lresult = 0;

            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_CurrentAvalableAudioMixers == null)
                    break;

                lresult = (int)m_CurrentAvalableAudioMixers.Invoke(m_CaptureObj, new object[] { });

            } while (false);

            return lresult;
        }

        public void setOpacity(string a_SymbolicLink, float a_value)
        {
            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_SetOpacity == null)
                    break;

                m_SetOpacity.Invoke(m_CaptureObj, new object[] { a_SymbolicLink, a_value });

            } while (false);

        }

        public void setRelativeVolume(string a_SymbolicLink, float a_value)
        {
            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_SetRelativeVolume == null)
                    break;

                m_SetRelativeVolume.Invoke(m_CaptureObj, new object[] { a_SymbolicLink, a_value });

            } while (false);

        }
        
        private Tuple<uint, uint, string> getAudioBitRates()
        {
            Tuple<uint, uint, string> l_result = null;

            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_GetAudioBitRates == null)
                    break;

                l_result = m_GetAudioBitRates.Invoke(m_CaptureObj, new object[] { }) as Tuple<uint, uint, string>;
                
            } while (false);

            return l_result;
        }

        private List<Tuple<Tuple<uint, uint, string>, Tuple<string, string>>> getVideoBitRates()
        {
            List<Tuple<Tuple<uint, uint, string>, Tuple<string, string>>> l_result = null;

            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_GetVideoBitRates == null)
                    break;

                l_result = m_GetVideoBitRates.Invoke(m_CaptureObj, new object[] { }) as List<Tuple<Tuple<uint, uint, string>, Tuple<string, string>>>;
                
            } while (false);

            return l_result;
        }

        private void setVideoBitRate(uint a_selectedBitRate, string a_encoderCLSID, string a_encoderModeCLSID)
        {
            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_SetVideoBitRate == null)
                    break;

                m_SetVideoBitRate.Invoke(m_CaptureObj, new object[] { a_selectedBitRate, a_encoderCLSID, a_encoderModeCLSID });

            } while (false);
        }

        private void setAudioBitRate(uint a_selectedBitRate, string a_encoderModeCLSID)
        {
            do
            {
                if (m_CaptureObj == null)
                    break;

                if (m_SetAudioBitRate == null)
                    break;

                m_SetAudioBitRate.Invoke(m_CaptureObj, new object[] { a_selectedBitRate, a_encoderModeCLSID });

            } while (false);
        }

        public ICollectionView VideoBitRateCollection
        {
            get
            {
                if (mVideoBitRateCustomerView == null)
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

        public void setFileSize(int l_fileSizeMb)
        {
            m_fileSizeMb = l_fileSizeMb;
        }
                
        private void startCheckSizeTimer()
        {
            if (m_CheckSize_Timer != null)
            {
                m_CheckSize_Timer.Dispose();

                m_CheckSize_Timer = null;
            }


            m_CheckSize_Timer = new Timer(
            callback: new TimerCallback(checkFileSize_task),
            state: null,
            dueTime: 5000,
            period: 5000);
        }

        private void checkFileSize_task(object timerState)
        {
            System.IO.FileInfo l_fileInfo = new System.IO.FileInfo(m_TempFileName);

            if(l_fileInfo.Exists)
            {
                var l_MB_size = l_fileInfo.Length / 1048576L;

                if(l_MB_size >= (m_fileSizeMb - 1))
                {
                    do
                    {
                        m_CheckSize_Timer.Dispose();

                        var l_TempFileName = System.IO.Path.GetTempFileName();

                        if (string.IsNullOrEmpty(l_TempFileName))
                            break;

                        if (m_CaptureObj == null)
                            break;

                        if (m_Restart == null)
                            break;

                        var l_result = (bool)m_Restart.Invoke(m_CaptureObj, new object[] { l_TempFileName });

                        if(l_result)
                        {
                            if (!string.IsNullOrEmpty(m_TempFileName))
                            {
                                var l_temp = m_TempFileName;

                                m_TempFileName = l_TempFileName;

                                startCheckSizeTimer();

                                Thread.Sleep(1000);

                                Omega_Red.Managers.MediaRecorderManager.Instance.createItem(l_temp);
                            }
                        }

                    } while (false);

                }
            }
        }
    }
}
