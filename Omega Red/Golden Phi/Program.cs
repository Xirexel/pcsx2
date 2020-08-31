using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Golden_Phi
{
    class Program
    {

        static System.Threading.Mutex gMutex;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] args)
        {
            gMutex = new System.Threading.Mutex(false, App.c_MainFolderName);

            try
            {
                bool bAppIsRunning = !gMutex.WaitOne(10, false);
            }
            catch (Exception)
            {
                var l_currnetProcessID = Process.GetCurrentProcess().Id;

                foreach (var process in Process.GetProcessesByName("Golden Phi"))
                {
                    if(process.Id != l_currnetProcessID)
                        process.Kill();
                }
            }
            
            var t = new Thread(() =>
            {
                try
                {
                    App app = new App();

                    app.InitializeComponent();

                    app.Run();
                }
                catch (Exception)
                {
                }
                finally
                {
                }
            });

            t.SetApartmentState(ApartmentState.STA);

            t.Start();
        }
    }
}
