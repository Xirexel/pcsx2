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
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] args)
        {
            try
            {
                var lCaptureManagerVersion = MediaCapture.Instance.CaptureManagerVersion;

                lCaptureManagerVersion = MediaStream.Instance.CaptureManagerVersion;
            }
            catch (System.Exception)
            {
            }

            var t = new Thread(()=>
               {
                   try
                   {                                                                  
                       new App().Run();
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
