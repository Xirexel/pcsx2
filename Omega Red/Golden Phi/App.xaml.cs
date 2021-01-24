using Golden_Phi.Emulators;
using Golden_Phi.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Golden_Phi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IntPtr CurrentWindowHandler { get; set; }

        public const string c_MainFolderName = "Golden Phi";

        public const string c_sstate_ext = ".p2s";

        private static string m_MainStoreDirectoryPath = "";

        public static string MainStoreDirectoryPath
        {
            get
            {

                if (string.IsNullOrWhiteSpace(m_MainStoreDirectoryPath))
                {
                    var lDirectory = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);

                    var lMainDirInfo = Directory.GetParent(lDirectory);

                    lMainDirInfo = Directory.GetParent(lMainDirInfo.FullName);

                    m_MainStoreDirectoryPath = lMainDirInfo.FullName;
                }

                return m_MainStoreDirectoryPath;
            }
        }
        
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

            Startup += (object sender, StartupEventArgs e) =>
            {
                this.StartupUri = new Uri("pack://application:,,,/Golden Phi;component/MainWindow.xaml");

                using (Process p = Process.GetCurrentProcess())
                    p.PriorityClass = ProcessPriorityClass.RealTime;
            };

            InitializeComponent();
        }

        public static void saveCopy()
        {
            if (File.Exists(MainStoreDirectoryPath + @"\Config.xml"))
            {
                File.Delete(MainStoreDirectoryPath + @"\Config.xml");
            }

            File.Copy(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath, MainStoreDirectoryPath + @"\Config.xml");
        }
        public static object getResource(string a_key)
        {
            if (Application.Current == null)
                return null;

            return getResource(Application.Current.Resources, a_key);
        }

        public static object getResource(ResourceDictionary a_resource, string a_key)
        {
            var l_itemTemplate = a_resource[a_key];

            if (l_itemTemplate != null)
                return l_itemTemplate;

            foreach (var l_Dictionary in a_resource.MergedDictionaries)
            {
                l_itemTemplate = getResource(l_Dictionary, a_key);

                if (l_itemTemplate != null)
                    break;
            }

            return l_itemTemplate;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Managers.PadControlManager.Instance.stopTimer();

            Emul.Instance.stop(false);

            Managers.IsoManager.Instance.save();

            Golden_Phi.Properties.Settings.Default.Save();

            saveCopy();
        }
    }
}
