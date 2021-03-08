using Omega_Red.Capture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Omega_Red
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        public static void Main(string[] args)
        {
            var t = new Thread(() =>
            {
                try
                {
                    initCaptureManager();

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

        private static void initCaptureManager()
        {
            AutoResetEvent lBlockEvent = new AutoResetEvent(false);

            var t = new Thread(() =>
            {
                try
                {
                    var l_ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                    System.Reflection.Assembly l_lCaptureManagerToCSharpProxyAssembly = null;

                    using (var lCaptureManagerToCSharpProxyStream = l_ExecutingAssembly.GetManifestResourceStream("Omega_Red.Modules.AnyCPU.CaptureManagerToCSharpProxy.dll"))
                    {
                        if (lCaptureManagerToCSharpProxyStream != null)
                        {
                            byte[] lCaptureManagerToCSharpProxybuffer = new byte[(int)lCaptureManagerToCSharpProxyStream.Length];

                            lCaptureManagerToCSharpProxyStream.Read(lCaptureManagerToCSharpProxybuffer, 0, lCaptureManagerToCSharpProxybuffer.Length);

                            l_lCaptureManagerToCSharpProxyAssembly = System.Reflection.Assembly.Load(lCaptureManagerToCSharpProxybuffer);
                        }
                    }

                    AppDomain.CurrentDomain.AssemblyResolve += (sender, argument) =>
                    {
                        return l_lCaptureManagerToCSharpProxyAssembly;
                    };

                    var lCaptureManagerVersion = MediaCapture.Instance.CaptureManagerVersion;

                    lCaptureManagerVersion = MediaStream.Instance.CaptureManagerVersion;
                }
                finally
                {
                    lBlockEvent.Set();
                }
            });

            t.SetApartmentState(ApartmentState.MTA);

            t.Start();

            lBlockEvent.WaitOne(3000, true);
        }
    }
}
