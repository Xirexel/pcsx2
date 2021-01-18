using Golden_Phi.Emulators;
using Golden_Phi.Managers;
using Golden_Phi.Panels;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Golden_Phi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LockScreenManager.Instance.show();
            
            InitializeComponent();

            SaveStateManager.Instance.init();

            BiosManager.Instance.load();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                IsoManager.Instance.load();
            });

            Emul.Instance.ShowErrorEvent += Instance_ShowWarningEvent;

#if DEBUG

            WindowState = System.Windows.WindowState.Normal;

            WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
#endif
        }

        private void Instance_ShowWarningEvent(string a_message)
        {
            mTaskbarIcon.ShowBalloonTip(Title, a_message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
        }

        private void Instance_ShowInfoEvent(string a_message)
        {
            mTaskbarIcon.ShowBalloonTip(Title, a_message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        private void Instance_ShowErrorEvent(string a_message)
        {
            mTaskbarIcon.ShowBalloonTip(Title, a_message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mTaskbarIcon.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Emul.Instance.stop();

            Emul.Instance.setVideoPanel((m_PadPanel.Content as DisplayControl).VideoPanel);

            ScreenshotsManager.Instance.setVideoPanel((m_PadPanel.Content as DisplayControl).VideoPanel);

            var wih = new System.Windows.Interop.WindowInteropHelper(App.Current.MainWindow);

            App.CurrentWindowHandler = wih.Handle;

            LockScreenManager.Instance.hide();
        }


    }
}
