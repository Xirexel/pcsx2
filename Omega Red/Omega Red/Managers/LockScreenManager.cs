/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using Omega_Red.Capture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace Omega_Red.Managers
{
    internal delegate void StatusDelegate(LockScreenManager.Status aStatus);

    internal delegate void MessageDelegate(string aMessage);

    class LockScreenManager
    {
        public enum Status
        {
            None,
            Show,
            Starting,
            Saving,
            DisplayImage,
            DisplayVideo,
            DisplayAbout,
            DisplayStreamConfig
        }

        private Image mIconImage = null;

        private static LockScreenManager m_Instance = null;

        public static LockScreenManager Instance { get { if (m_Instance == null) m_Instance = new LockScreenManager(); return m_Instance; } }

        private LockScreenManager()
        {
            try
            {
                mIconImage = new Image();

                BitmapImage lBitmapImage = new BitmapImage();
                lBitmapImage.BeginInit();
                lBitmapImage.UriSource = new Uri("pack://application:,,,/Omega Red;component/Assests/Images/OmegaRed.gif", UriKind.Absolute);
                lBitmapImage.EndInit();

                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(mIconImage, lBitmapImage);

                if (mIconImage != null)
                    mIconImage.Loaded += (object sender, RoutedEventArgs e) =>
                    {
                        var l_Controller = WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage);

                        if (l_Controller != null)
                            l_Controller.Pause();
                    };
            }
            catch (Exception)
            {}

        }

        public Image IconImage { get { return mIconImage; } }

        public event StatusDelegate StatusEvent;

        public event MessageDelegate MessageEvent;

        public void hide()
        {
            if(StatusEvent == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                StatusEvent(Status.None);

                WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage).Pause();
            });
        }

        public void show()
        {
            if (StatusEvent == null)
                return;

            displayMessage("");

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                StatusEvent(Status.Show);

                WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage).Play();
            });
        }

        public void showSaving()
        {
            if (StatusEvent == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                var l_SavingTitle = App.Current.Resources["SavingTitle"];

                displayMessage(l_SavingTitle == null ? "" : l_SavingTitle as string);

                StatusEvent(Status.Show);

                WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage).Play();
            });
        }

        public void showStarting()
        {
            if (StatusEvent == null)
                return;

            displayMessage("");

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                StatusEvent(Status.Show);
                StatusEvent(Status.Starting);

                WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage).Play();
            });
        }

        public void showScreenshot()
        {
            if (MessageEvent == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                StatusEvent(Status.Show);
                StatusEvent(Status.DisplayImage);

                WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage).Pause();
            });
        }

        public void showStreamConfig()
        {
            if (MessageEvent == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                StatusEvent(Status.Show);
                StatusEvent(Status.DisplayStreamConfig);

                WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage).Pause();
            });
        }

        public void showVideo()
        {
            if (MessageEvent == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                StatusEvent(Status.Show);
                StatusEvent(Status.DisplayVideo);

                WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage).Pause();
            });
        }

        public void showAbout()
        {
            if (MessageEvent == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                StatusEvent(Status.Show);
                StatusEvent(Status.DisplayAbout);

                WpfAnimatedGif.ImageBehavior.GetAnimationController(mIconImage).Play();
            });
        }

        public void displayMessage(string aMessage)
        {
            if (MessageEvent == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
            {
                MessageEvent(aMessage);
            });
        }

        public string getCaptureManagerVersion()
        {
            string lresult = "";

            do
            {
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(MediaCapture.Instance.CaptureManagerVersion);

                // {<?xml version="1.0"?>
                //<!--XML Document of Capture Manager version-->
                //<CaptureManagerVersion MAJOR="1" MINOR="11" PATCH="0">Freeware</CaptureManagerVersion>
                //}

                var l_AttributeNode = doc.SelectSingleNode("CaptureManagerVersion/@MAJOR");

                if (l_AttributeNode == null)
                    break;

                lresult = l_AttributeNode.Value;


                l_AttributeNode = doc.SelectSingleNode("CaptureManagerVersion/@MINOR");

                if (l_AttributeNode == null)
                    break;

                lresult += "." + l_AttributeNode.Value;


                l_AttributeNode = doc.SelectSingleNode("CaptureManagerVersion/@PATCH");

                if (l_AttributeNode == null)
                    break;

                lresult += "." + l_AttributeNode.Value;

                lresult += " - " + doc.DocumentElement.InnerText;         
                
            } while (false);

            return lresult;
        }
    }
}
