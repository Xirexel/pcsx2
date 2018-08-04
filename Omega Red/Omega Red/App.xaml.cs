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
        public App()
        {
            Startup += delegate
            {
                using (Process p = Process.GetCurrentProcess())
                    p.PriorityClass = ProcessPriorityClass.RealTime; 
            };
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
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
