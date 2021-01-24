using Golden_Phi.Managers;
using Golden_Phi.Models;
using Golden_Phi.Panels;
using Golden_Phi.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Golden_Phi.Emulators
{
    public class Emul
    {
        private VideoPanel m_VideoPanel = null;

        private Image mActiveStateImage = null;

        
        private double m_Audiolevel = 1.0;
        
        public event Action<StatusEnum> ChangeStatusEvent;

        public enum StatusEnum
        {
            NoneInitilized,
            Initilized,
            Stopped,
            Started,
            Paused
        }

        private AspectRatio m_AspectRatio = AspectRatio.Ratio_4_3;

        public event Action<string> ShowErrorEvent = null;

        public GameType GameType { get; private set; } = GameType.Unknown;

        public string DiscSerial { get; private set; } = "";

        private IEmul m_currentEmul = null;

        private string m_file_path = "";

        private StatusEnum m_Status = StatusEnum.Stopped;

        private DateTime mCurrentDateTime = DateTime.Now;
        
        private TimeSpan mGameSessionDuration = new TimeSpan();

        private static Emul m_Instance = null;

        public static Emul Instance { get { if (m_Instance == null) m_Instance = new Emul(); return m_Instance; } }

        private Emul()
        {
            try
            {
                mActiveStateImage = new Image();

                BitmapImage lBitmapImage = new BitmapImage();
                lBitmapImage.BeginInit();
                lBitmapImage.UriSource = new Uri("pack://application:,,,/Golden Phi;component/Assests/Images/Golden_Phi_Small.gif", UriKind.Absolute);
                lBitmapImage.EndInit();

                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(mActiveStateImage, lBitmapImage);

                bool lInitStopped = false;

                if (mActiveStateImage != null)
                    mActiveStateImage.Loaded += (object sender, RoutedEventArgs e) =>
                    {
                        if (lInitStopped)
                            return;

                        var l_Controller = WpfAnimatedGif.ImageBehavior.GetAnimationController(mActiveStateImage);

                        if (l_Controller != null)
                            l_Controller.Pause();

                        lInitStopped = true;
                    };
            }
            catch (Exception)
            { }
        }

        public StatusEnum Status { get { return m_Status; } }

        internal void setVideoPanel(VideoPanel a_VideoPanel)
        {
            m_VideoPanel = a_VideoPanel;
        }

        public void play(IsoInfo a_IsoInfo, SaveStateInfo a_SaveStateInfo = null, bool a_EmitStopEvent = true)
        {
            if (m_Status == StatusEnum.NoneInitilized)
                return;

            IsoManager.Instance.clearActiveState();
            
            IsoManager.Instance.refresh(a_IsoInfo);            
            
            if (a_IsoInfo == null)
                return;

            if (a_SaveStateInfo != null && a_SaveStateInfo.ImageSource != null)
                LockScreenManager.Instance.show(a_SaveStateInfo.ImageSource);
            else if (a_IsoInfo.ImageSource != null)
                LockScreenManager.Instance.show(a_IsoInfo.ImageSource);
            else
                LockScreenManager.Instance.show();
            
            if (m_currentEmul != null)
            {
                if (a_IsoInfo.GameType != m_currentEmul.GameType)
                {
                    stop_start(a_IsoInfo, a_SaveStateInfo, a_EmitStopEvent);

                    return;
                }
                else if (a_IsoInfo.DiscSerial != m_currentEmul.DiscSerial)
                {
                    stop_start(a_IsoInfo, a_SaveStateInfo, a_EmitStopEvent);

                    return;
                }
                else if (a_IsoInfo.BiosInfo != null && a_IsoInfo.BiosInfo.CheckSum.ToString("X8") != m_currentEmul.BiosCheckSum)
                {
                    stop_start(a_IsoInfo, a_SaveStateInfo, a_EmitStopEvent);

                    return;
                }
            }

            mCurrentDateTime = DateTime.Now;
            
            switch (m_Status)
            {
                case StatusEnum.Initilized:
                case StatusEnum.Stopped:
                    {
                        {
                            var l_bios_check_sum =
                            a_IsoInfo.BiosInfo != null ?
                            a_IsoInfo.BiosInfo.GameType == a_IsoInfo.GameType ? "_" + a_IsoInfo.BiosInfo.CheckSum.ToString("X8") : ""
                            : "";
                            
                            SaveStateManager.Instance.resetQuickSaveStateInfo(a_IsoInfo.DiscSerial, l_bios_check_sum);
                        }

                        startInner(a_IsoInfo, a_SaveStateInfo);
                    }
                    break;
                case StatusEnum.Paused:
                    startInner(a_IsoInfo);
                    break;
                case StatusEnum.Started:
                    break;
                default:
                    break;
            }
        }
        
        public void pause()
        {
            if (m_currentEmul != null)
            {
                m_currentEmul.pause();

                var l_IsoInfo = IsoManager.Instance.getIsoInfo(m_currentEmul.DiscSerial);

                if (l_IsoInfo != null && m_VideoPanel != null)
                {
                    var l_data = m_VideoPanel.takeScreenshot();

                    if (l_data != null && l_data.Length > 0)
                    {
                        var bitmap = new BitmapImage();

                        using (var stream = new MemoryStream(l_data))
                        {
                            stream.Position = 0; // here

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            bitmap.Freeze();
                        }

                        l_IsoInfo.ImageSource = bitmap;
                    }
                }

                if (mActiveStateImage.Source != null)
                    WpfAnimatedGif.ImageBehavior.GetAnimationController(mActiveStateImage).Play();

                if (m_Status != StatusEnum.Stopped)
                    setStatus(StatusEnum.Paused);

                mGameSessionDuration += DateTime.Now - mCurrentDateTime;
            }
        }

        public void lockPause()
        {
            if (m_currentEmul != null)
            {
                m_currentEmul.pause();

                var l_IsoInfo = IsoManager.Instance.getIsoInfo(m_currentEmul.DiscSerial);

                if (l_IsoInfo != null && m_VideoPanel != null)
                {
                    var l_data = m_VideoPanel.takeScreenshot();

                    if (l_data != null && l_data.Length > 0)
                    {
                        var bitmap = new BitmapImage();

                        using (var stream = new MemoryStream(l_data))
                        {
                            stream.Position = 0; // here

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            bitmap.Freeze();
                        }

                        l_IsoInfo.ImageSource = bitmap;
                    }
                }

                m_Status = StatusEnum.Paused;

                mGameSessionDuration += DateTime.Now - mCurrentDateTime;
            }
        }

        public void unlockResume()
        {
            if (m_currentEmul != null)
            {
                m_currentEmul.resume();

                m_Status = StatusEnum.Started;
            }
        }

        private void stopInner(bool a_EmitStopEvent = true)
        {
            if (m_currentEmul != null)
            {
                m_currentEmul.pause();

                var l_bios_check_sum =
                !string.IsNullOrWhiteSpace(m_currentEmul.BiosCheckSum) ? "_" + m_currentEmul.BiosCheckSum : "";

                var l_AutoSaveStateInfo = SaveStateManager.Instance.getAutoSaveStateInfo(
                    m_currentEmul.DiscSerial,
                    l_bios_check_sum);

                byte[] l_Screenshot = null;

                if (m_VideoPanel != null)
                    l_Screenshot = m_VideoPanel.takeScreenshot();

                m_currentEmul.saveState(l_AutoSaveStateInfo, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), mGameSessionDuration.TotalSeconds, l_Screenshot);

                m_currentEmul.stop();
            }

            m_currentEmul = null;

            if(a_EmitStopEvent)
                if (m_Status != StatusEnum.Stopped)
                    setStatus(StatusEnum.Stopped);

            m_Status = StatusEnum.Stopped;

            mGameSessionDuration = new TimeSpan();
        }

        public void stop(bool a_isthreaded = true)
        {
            if(a_isthreaded)
            {
                Thread l_resumeThread = new Thread(() => {

                    LockScreenManager.Instance.show();

                    stopInner();

                    LockScreenManager.Instance.hide();

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        SaveStateManager.Instance.updateSave(SaveStateManager.Instance.AutoSave);
                    });
                });

                l_resumeThread.Start();
            }
            else
                stopInner();
        }

        public void setLimitFrame(bool a_state)
        {
            if (m_currentEmul == null)
                return;

            m_currentEmul.setLimitFrame(a_state);
        }

        private void startInner(IsoInfo a_IsoInfo, SaveStateInfo a_SaveStateInfo = null)
        {
            do
            {
                DiscSerial = "";

                GameType = a_IsoInfo.GameType;

                mCurrentDateTime = DateTime.Now;
                
                Thread l_startThread = new Thread(() => {

                    EmulStartState l_result = EmulStartState.Failed;

                    do
                    {

                        if (m_currentEmul == null)
                        {
                            switch (a_IsoInfo.GameType)
                            {
                                case GameType.PS2:
                                    m_currentEmul = PCSX2Emul.Instance;
                                    break;
                                case GameType.PS1:
                                    m_currentEmul = PCSXEmul.Instance;
                                    break;
                                case GameType.PSP:
                                    m_currentEmul = PPSSPPEmul.Instance;
                                    break;
                                case GameType.Unknown:
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (m_currentEmul == null)
                            break;

                        l_result = m_currentEmul.start(a_IsoInfo, m_VideoPanel.SharedHandle);

                    } while (false);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {

                        if (l_result == EmulStartState.Failed)
                        {
                            var l_CannotStartGame = App.Current.Resources["CannotStartGame"] as string;

                            if (l_CannotStartGame != null)
                                l_CannotStartGame = l_CannotStartGame.Replace("STUB", a_IsoInfo.GameType.ToString());

                            showErrorEvent(l_CannotStartGame);
                        }
                        else if (l_result == EmulStartState.OK)
                        {
                            a_IsoInfo.ActiveStateImage = mActiveStateImage;

                            IsoManager.Instance.Collection.MoveCurrentTo(a_IsoInfo);

                            IsoManager.Instance.save();

                            if (mActiveStateImage.Source != null)
                                WpfAnimatedGif.ImageBehavior.GetAnimationController(mActiveStateImage).Pause();


                                                       
                            if(m_Status == StatusEnum.Stopped)
                                MemoryCardManager.Instance.setMemoryCard();

                            setStatus(StatusEnum.Started);

                            if (m_Status == StatusEnum.Started)
                                setMemoryCard(m_file_path);
                            
                            if (m_Status == StatusEnum.Started)
                                m_currentEmul.setVideoAspectRatio(m_AspectRatio);

                            if (a_SaveStateInfo != null)
                            {
                                m_currentEmul.loadState(a_SaveStateInfo);

                                mGameSessionDuration = a_SaveStateInfo.DurationNative;
                            }

                            DiscSerial = a_IsoInfo.DiscSerial;

                            setAudioVolume(Settings.Default.SoundLevel);

                            setIsMuted(Settings.Default.IsMuted);
                        }                                               

                        LockScreenManager.Instance.hide();
                    });
                });

                l_startThread.Start();

            } while (false);
        }

        private void setStatus(StatusEnum a_Status)
        {
            m_Status = a_Status;

            if(Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    if (ChangeStatusEvent != null)
                        ChangeStatusEvent(m_Status);
                });
        }

        private void showErrorEvent(string a_message)
        {
            if (ShowErrorEvent != null)
                ShowErrorEvent(a_message);
        }

        private void stop_start(IsoInfo a_IsoInfo, SaveStateInfo a_SaveStateInfo, bool a_EmitStopEvent)
        {
            Thread l_resumeThread = new Thread(() => {

                stopInner(a_EmitStopEvent);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    if (mActiveStateImage.Source != null)
                        WpfAnimatedGif.ImageBehavior.GetAnimationController(mActiveStateImage).Pause();

                    play(a_IsoInfo, a_SaveStateInfo);
                });
            });

            l_resumeThread.Start();
        }

        public string getFileSignature()
        {
            string l_file_signature = "";

            var l_bios_check_sum =
            !string.IsNullOrWhiteSpace(m_currentEmul.BiosCheckSum) ? "_" + m_currentEmul.BiosCheckSum : "";

            if (m_currentEmul != null)
                l_file_signature = m_currentEmul.DiscSerial + l_bios_check_sum;

            return l_file_signature;
        }

        public async Task saveState(SaveStateInfo a_SaveStateInfo)
        {
            try
            {

                if (m_Status == StatusEnum.Paused)
                {
                    await Task.Run(() => {

                        AutoResetEvent lBlockEvent = new AutoResetEvent(false);



                        byte[] l_Screenshot = null;

                        if (m_VideoPanel != null)
                            l_Screenshot = m_VideoPanel.takeScreenshot();

                        if (l_Screenshot != null && l_Screenshot.Length > 0)
                        {
                            var bitmap = new BitmapImage();

                            using (var stream = new MemoryStream(l_Screenshot))
                            {
                                stream.Position = 0; // here

                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                                bitmap.Freeze();
                            }

                            a_SaveStateInfo.ImageSource = bitmap;
                        }

                        LockScreenManager.Instance.showSaving(a_SaveStateInfo.ImageSource);

                        var l_DateTimeNow = DateTime.Now;

                        a_SaveStateInfo.Date = l_DateTimeNow.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));

                        mGameSessionDuration += l_DateTimeNow - mCurrentDateTime;

                        a_SaveStateInfo.Duration = mGameSessionDuration.ToString(@"dd\.hh\:mm\:ss");

                        a_SaveStateInfo.DurationNative = mGameSessionDuration;
                        

                        ThreadStart innerCallSaveStart = new ThreadStart(() =>
                        {
                            m_currentEmul.saveState(a_SaveStateInfo, a_SaveStateInfo.Date, a_SaveStateInfo.DurationNative.TotalSeconds, l_Screenshot);
                            
                            Thread.Sleep(200);

                            LockScreenManager.Instance.hide();

                            lBlockEvent.Set();
                        });

                        Thread innerCallSaveStartThread = new Thread(innerCallSaveStart);

                        innerCallSaveStartThread.Start();

                        lBlockEvent.WaitOne(TimeSpan.FromSeconds(20));

                    });
                }
            }
            catch (Exception exc)
            {
            }
        }

        public void loadState(SaveStateInfo a_SaveStateInfo)
        {

            if(m_currentEmul != null 
                && a_SaveStateInfo.Type == m_currentEmul.GameType
                && a_SaveStateInfo.DiscSerial == m_currentEmul.DiscSerial
                )
            {
                m_currentEmul.loadState(a_SaveStateInfo);

                var l_IsoInfo = IsoManager.Instance.getIsoInfo(a_SaveStateInfo.DiscSerial);

                play(l_IsoInfo);
            }
            else
            {
                var l_IsoInfo = IsoManager.Instance.getIsoInfo(a_SaveStateInfo.DiscSerial);

                if(l_IsoInfo != null)
                    play(l_IsoInfo, a_SaveStateInfo);
            }

            mGameSessionDuration = a_SaveStateInfo.DurationNative;
        }

        public async Task quickSave(SaveStateInfo a_SaveStateInfo)
        {
            if (m_Status == StatusEnum.Started)
            {
                m_currentEmul.pause();

                m_Status = StatusEnum.Paused;

                await saveState(a_SaveStateInfo);

                m_currentEmul.resume();

                mCurrentDateTime = DateTime.Now;

                m_Status = StatusEnum.Started;
            }
        }

        public void setAudioVolume(double a_level)
        {
            if (m_currentEmul != null && !Settings.Default.IsMuted)
                m_currentEmul.setAudioVolume((float)a_level);

            m_Audiolevel = a_level;
        }

        public void setIsMuted(bool a_state)
        {
            if (m_currentEmul != null)
            {
                if (a_state)
                    m_currentEmul.setAudioVolume(0.0f);
                else
                    m_currentEmul.setAudioVolume((float)m_Audiolevel);
            }
        }

        public void setMemoryCard(string a_file_path = null, int slot = 0)
        {
            m_file_path = "";

            if (m_currentEmul != null)
            {
                m_currentEmul.setMemoryCard(a_file_path, slot);
            }
            else
                m_file_path = a_file_path;
        }

        public void playActiveState()
        {
            if (Status == StatusEnum.Paused && mActiveStateImage.Source != null)
                WpfAnimatedGif.ImageBehavior.GetAnimationController(mActiveStateImage).Play();
        }

        public void setVideoAspectRatio(AspectRatio a_AspectRatio)
        {
            m_AspectRatio = a_AspectRatio;

            if (m_currentEmul != null)
            {
                m_currentEmul.setVideoAspectRatio(m_AspectRatio);
            }
        }
    }
}
