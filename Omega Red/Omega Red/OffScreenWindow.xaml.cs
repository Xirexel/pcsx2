using Omega_Red.Capture;
using Omega_Red.Managers;
using Omega_Red.Panels;
using Omega_Red.Tools;
using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Screen = System.Windows.Forms.Screen;

namespace Omega_Red
{
    /// <summary>
    /// Interaction logic for OffScreenWindow.xaml
    /// </summary>
    public partial class OffScreenWindow : Window
    {
        private VideoPanel m_VideoPanel;

        private HwndSource m_HwndSource;

        public OffScreenWindow()
        {
            SaveStateManager.Instance.init();

            InitializeComponent();

            Hide();

            m_VideoPanel = new VideoPanel();


            HwndSourceParameters myparms = new HwndSourceParameters();
            m_HwndSource = new HwndSource(myparms);
            myparms.HwndSourceHook = new HwndSourceHook(ApplicationMessageFilter);

            OffScreenStream.Instance.startServer();
            
            PCSX2Controller.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;
                        
            mTaskbarIcon.ShowBalloonTip(Title, "", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);

        }

        PCSX2Controller.StatusEnum m_Status = PCSX2Controller.StatusEnum.NoneInitilized;

        void Instance_m_ChangeStatusEvent(PCSX2Controller.StatusEnum a_Status)
        {
            switch (a_Status)
            {
                case PCSX2Controller.StatusEnum.NoneInitilized:
                    break;
                case PCSX2Controller.StatusEnum.Initilized:
                    break;
                case PCSX2Controller.StatusEnum.Stopped:
                    OffScreenStream.Instance.stop();
                    break;
                case PCSX2Controller.StatusEnum.Paused:
                    break;
                case PCSX2Controller.StatusEnum.Started:
                    if(m_Status != PCSX2Controller.StatusEnum.Paused)
                        OffScreenStream.Instance.start();
                    break;
            }

            m_Status = a_Status;

            string Message = "";

            switch (a_Status)
            {
                case PCSX2Controller.StatusEnum.NoneInitilized:
                    Message = App.Current.Resources["NoneInitilizedTitle"] as String;
                    break;
                case PCSX2Controller.StatusEnum.Initilized:
                    Message = App.Current.Resources["InitilizedTitle"] as String;
                    break;
                case PCSX2Controller.StatusEnum.Stopped:
                    Message = App.Current.Resources["StoppedTitle"] as String;
                    break;
                case PCSX2Controller.StatusEnum.Paused:
                    Message = App.Current.Resources["PausedTitle"] as String;
                    break;
                case PCSX2Controller.StatusEnum.Started:
                default:
                    Message = App.Current.Resources["StartedTitle"] as String;
                    break;
            }

            mTaskbarIcon.ShowBalloonTip(Title, Message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        static IntPtr ApplicationMessageFilter(
    IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }


        public void loadModules()
        {
            do
            {
                lock (this)
                {
                    if (!ModuleManager.Instance.isInit)
                    {
                        break;
                    }

                    if (!PCSX2LibNative.Instance.isInit)
                    {
                        break;
                    }

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        ModuleControl.Instance.setVideoPanel(m_VideoPanel);

                        Tools.Savestate.SStates.Instance.setVideoPanel(m_VideoPanel);
                        
                        ModuleControl.Instance.setWindowHandler(m_HwndSource.Handle);

                        PCSX2Controller.Instance.updateInitilize();
                    });
                }

            } while (false);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                if (ModuleManager.Instance.isInit && PCSX2LibNative.Instance.isInit)
                    LockScreenManager.Instance.hide();
            });
        }

        private void MTaskbarIcon_TrayPopupOpen(object sender, RoutedEventArgs e)
        {
            mMainGrid.UpdateDefaultStyle();

#if DEBUG
            mMainGrid.Height = 800;
#else

            mMainGrid.Height = (double)Screen.PrimaryScreen.Bounds.Height > 768? 768: (double)Screen.PrimaryScreen.Bounds.Height;

#endif

            LockScreenManager.Instance.show();

            ThreadStart loadModulesStart = new ThreadStart(loadModules);

            Thread loadModulesThread = new Thread(loadModulesStart);

            loadModulesThread.Start();
        }
    }
}
