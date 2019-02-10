using Omega_Red.Tools;
using Omega_Red.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Omega_Red
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public enum AppType
        {
            Screen,
            OffScreen
        }

        public static bool m_is_enable_rendering_mode = true;

        public static AppType m_AppType = AppType.Screen;

        public App()
        {
            Startup += (object sender, StartupEventArgs e)=>
            {
                this.StartupUri = new Uri("pack://application:,,,/Omega Red;component/MainWindow.xaml");

                using (Process p = Process.GetCurrentProcess())
                    p.PriorityClass = ProcessPriorityClass.RealTime;

                for (int i = 0; i != e.Args.Length; ++i)
                {
                    if (e.Args[i] == "/OffScreen")
                    {
                        m_AppType = AppType.OffScreen;

                        this.StartupUri = new Uri("pack://application:,,,/Omega Red;component/OffScreenWindow.xaml");
                    }
                }
            };  
        }
        
        private void Application_Exit(object sender, ExitEventArgs e)
        {

            Capture.OffScreenStream.Instance.stopServer();

            PCSX2Controller.Instance.Stop(true);

            Thread.Sleep(1500);

            PCSX2LibNative.Instance.SysThreadBase_CancelFunc();

            Thread.Sleep(1500);

            PCSX2LibNative.Instance.MTVU_CancelFunc();

            PCSX2LibNative.Instance.MTGS_CancelFunc();

            ModuleControl.Instance.shutdown();

            PCSX2LibNative.Instance.resetCallbacksFunc();

            ModuleManager.Instance.release();

            Thread.Sleep(1500);

            PCSX2LibNative.Instance.release();
        }
    }
}
