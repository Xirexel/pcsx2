using Omega_Red.Properties;
using Omega_Red.Tools;
using Omega_Red.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Omega_Red
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool m_is_exit = false;
        
        public const string m_MainFolderName = "OmegaRed";

        private static string m_MainStoreDirectoryPath = "";

        public static string MainStoreDirectoryPath { get {

                if(string.IsNullOrWhiteSpace(m_MainStoreDirectoryPath))
                {
                    var lDirectory = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);

                    var lMainDirInfo = Directory.GetParent(lDirectory);

                    lMainDirInfo = Directory.GetParent(lMainDirInfo.FullName);

                    m_MainStoreDirectoryPath = lMainDirInfo.FullName;
                }

                return m_MainStoreDirectoryPath;
            } }

        public App()
        {
            string l_arch = "x86";

            if (IntPtr.Size == 8)
                l_arch = "x64";

            Win32NativeMethods.SetDllDirectory(@".\" + l_arch);
            
            if (File.Exists(MainStoreDirectoryPath + @"\Config.xml"))
            {
                if (File.Exists(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath))
                    File.Delete(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);
                else
                {
                    var lMainDirectory = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);

                    System.IO.Directory.CreateDirectory(lMainDirectory);
                }

                File.Copy(MainStoreDirectoryPath + @"\Config.xml", ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);
            }

            Startup += (object sender, StartupEventArgs e)=>
            {                
                this.StartupUri = new Uri("pack://application:,,,/Omega Red;component/MainWindow.xaml");

                using (Process p = Process.GetCurrentProcess())
                    p.PriorityClass = ProcessPriorityClass.RealTime;
            };

            InitializeComponent();
        }
        
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Managers.PadControlManager.Instance.stopTimer();
            
            saveCopy();

            m_is_exit = true;

            Capture.MediaCapture.Instance.stop();

            Capture.MediaStream.Instance.stop(true);

            Capture.OffScreenStream.Instance.stopServer();

            PCSX2Controller.Instance.Stop(true);
            
            Thread.Sleep(1500);

            PCSX2LibNative.Instance.SysThreadBase_CancelFunc();

            Thread.Sleep(1500);

            PCSX2LibNative.Instance.MTVU_CancelFunc();

            PCSX2LibNative.Instance.MTGS_CancelFunc();

            ModuleControl.Instance.shutdownPCSX2();

            ModuleControl.Instance.shutdownPCSX();

            PCSX2LibNative.Instance.resetCallbacksFunc();

            PCSX2ModuleManager.Instance.release();

            Thread.Sleep(1500);

            PCSX2LibNative.Instance.release();

            PPSSPPNative.Instance.release();

            PCSXNative.Instance.release();
        }

        public static void saveCopy()
        {
            if (File.Exists(MainStoreDirectoryPath + @"\Config.xml"))
            {
                File.Delete(MainStoreDirectoryPath + @"\Config.xml");
            }

            File.Copy(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath, MainStoreDirectoryPath + @"\Config.xml");
        }
    }
}
