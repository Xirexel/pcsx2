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

namespace Golden_Phi.Emul
{
    public class Emul
    {
        private VideoPanel m_VideoPanel = null;

        private Image mActiveStateImage = null;

        private double m_Audiolevel = 1.0;
        
        public event Action<StatusEnum> ChangeStatusEvent;

        public enum StatusEnum
        {
            Stopped,
            Started,
            Paused
        }

        public event Action<string> ShowErrorEvent = null;

        public GameType GameType { get; private set; } = GameType.Unknown;

        public string DiscSerial { get; private set; } = "";

        private IEmul m_currentEmul = null;

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
                lBitmapImage.UriSource = new Uri("pack://application:,,,/Golden Phi;component/Assests/Images/Golden_Phi.gif", UriKind.Absolute);
                lBitmapImage.EndInit();

                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(mActiveStateImage, lBitmapImage);

                if (mActiveStateImage != null)
                    mActiveStateImage.Loaded += (object sender, RoutedEventArgs e) =>
                    {
                        var l_Controller = WpfAnimatedGif.ImageBehavior.GetAnimationController(mActiveStateImage);

                        if (l_Controller != null)
                            l_Controller.Pause();
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

        public void play(IsoInfo a_IsoInfo, SaveStateInfo a_SaveStateInfo = null)
        {
            if (a_IsoInfo == null)
                return;

            IsoManager.Instance.clearActiveState();
            
            IsoManager.Instance.refresh(a_IsoInfo);            
            
            LockScreenManager.Instance.show();

            if (m_currentEmul != null)
            {
                if (a_IsoInfo.GameType != m_currentEmul.GameType)
                {
                    stop_start(a_IsoInfo, a_SaveStateInfo);

                    return;
                }
                else if (a_IsoInfo.DiscSerial != m_currentEmul.DiscSerial)
                {
                    stop_start(a_IsoInfo, a_SaveStateInfo);

                    return;
                }
                else if (a_IsoInfo.BiosInfo != null && a_IsoInfo.BiosInfo.CheckSum.ToString("X8") != m_currentEmul.BiosCheckSum)
                {
                    stop_start(a_IsoInfo, a_SaveStateInfo);

                    return;
                }
            }

            mCurrentDateTime = DateTime.Now;
            
            switch (m_Status)
            {
                case StatusEnum.Stopped:
                    startInner(a_IsoInfo, a_SaveStateInfo);
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

        public void stop()
        {
            if(m_currentEmul != null)
            {
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

            if(m_Status != StatusEnum.Stopped)
                setStatus(StatusEnum.Stopped);

            mGameSessionDuration = new TimeSpan();
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

                GameType = a_IsoInfo.GameType;

                mCurrentDateTime = DateTime.Now;
                
                Thread l_startThread = new Thread(() => {

                    EmulStartState l_result = EmulStartState.Failed;

                    do
                    {
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

                            setStatus(StatusEnum.Started);

                            if (a_SaveStateInfo != null)
                                m_currentEmul.loadState(a_SaveStateInfo);

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

        private void stop_start(IsoInfo a_IsoInfo, SaveStateInfo a_SaveStateInfo)
        {
            Thread l_resumeThread = new Thread(() => {

                stop();

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

        public void saveState(SaveStateInfo a_SaveStateInfo)
        {
            if (m_Status == StatusEnum.Paused)
            {
                a_SaveStateInfo.Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));

                a_SaveStateInfo.Duration = mGameSessionDuration.ToString(@"dd\.hh\:mm\:ss");

                a_SaveStateInfo.DurationNative = mGameSessionDuration;



                byte[] l_Screenshot = null;

                if (m_VideoPanel != null)
                    l_Screenshot = m_VideoPanel.takeScreenshot();

                m_currentEmul.saveState(a_SaveStateInfo, a_SaveStateInfo.Date, a_SaveStateInfo.DurationNative.TotalSeconds, l_Screenshot);

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



                //LockScreenManager.Instance.showSaving();

                //if (m_IsoInfo.GameType == GameType.PSP)
                //{

                //    ThreadStart innerCallSaveStart = new ThreadStart(() =>
                //    {
                //        SaveStateManager.Instance.savePPSSPPState(a_SaveStateInfo, a_SaveStateInfo.Date, a_SaveStateInfo.DurationNative.TotalSeconds);

                //        LockScreenManager.Instance.hide();
                //    });

                //    Thread innerCallSaveStartThread = new Thread(innerCallSaveStart);

                //    innerCallSaveStartThread.Start();
                //}
                //else if (m_IsoInfo.GameType == GameType.PS1)
                //{

                //    ThreadStart innerCallSaveStart = new ThreadStart(() =>
                //    {
                //        SaveStateManager.Instance.savePCSXState(a_SaveStateInfo, a_SaveStateInfo.Date, a_SaveStateInfo.DurationNative.TotalSeconds);

                //        LockScreenManager.Instance.hide();
                //    });

                //    Thread innerCallSaveStartThread = new Thread(innerCallSaveStart);

                //    innerCallSaveStartThread.Start();
                //}
                //else if (m_IsoInfo.GameType == GameType.PS2)
                //{
                //    ThreadStart innerCallSaveStart = new ThreadStart(() =>
                //    {

                //        SaveStateManager.Instance.saveState(a_SaveStateInfo, a_SaveStateInfo.Date, a_SaveStateInfo.DurationNative.TotalSeconds);

                //        LockScreenManager.Instance.hide();
                //    });

                //    Thread innerCallSaveStartThread = new Thread(innerCallSaveStart);

                //    innerCallSaveStartThread.Start();
                //}
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

        public void quickSave()
        {
            if (m_Status == StatusEnum.Started)
            {
                //LockScreenManager.Instance.showSaving();

                //if (m_IsoInfo.GameType == GameType.PSP)
                //{
                //    ThreadStart innerCallQuickSaveStart = new ThreadStart(() => {
                //        PlayPause();
                //        SaveStateManager.Instance.quickSavePPSSPP(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), mGameSessionDuration.TotalSeconds);
                //        PlayPause();
                //        LockScreenManager.Instance.hide();
                //    });

                //    Thread innerCallQuickSaveStartThread = new Thread(innerCallQuickSaveStart);

                //    innerCallQuickSaveStartThread.Start();
                //}
                //else if (m_IsoInfo.GameType == GameType.PS1)
                //{
                //    ThreadStart innerCallQuickSaveStart = new ThreadStart(() => {
                //        PlayPause();
                //        SaveStateManager.Instance.quickSavePCSX(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), mGameSessionDuration.TotalSeconds);
                //        PlayPause();
                //        LockScreenManager.Instance.hide();
                //    });

                //    Thread innerCallQuickSaveStartThread = new Thread(innerCallQuickSaveStart);

                //    innerCallQuickSaveStartThread.Start();
                //}
                //else if (m_IsoInfo.GameType == GameType.PS2)
                //{

                //    ThreadStart innerCallQuickSaveStart = new ThreadStart(() => {
                //        PlayPause();
                //        SaveStateManager.Instance.quickSave(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), mGameSessionDuration.TotalSeconds);
                //        PlayPause();
                //        LockScreenManager.Instance.hide();
                //    });

                //    Thread innerCallQuickSaveStartThread = new Thread(innerCallQuickSaveStart);

                //    innerCallQuickSaveStartThread.Start();
                //}
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

        public void playActiveState()
        {            
            if (Status == StatusEnum.Paused && mActiveStateImage.Source != null)
                WpfAnimatedGif.ImageBehavior.GetAnimationController(mActiveStateImage).Play();
        }

        public void stopActiveState()
        {
            if (mActiveStateImage.Source != null)
                WpfAnimatedGif.ImageBehavior.GetAnimationController(mActiveStateImage).Pause();
        }
    }
}
