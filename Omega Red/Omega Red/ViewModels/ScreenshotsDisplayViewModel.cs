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

using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.ComponentModel;
using Omega_Red.Models;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Omega_Red.Managers;
using System.Windows.Threading;
using System.Threading;

namespace Omega_Red.ViewModels
{
    public class ScreenshotsDisplayViewModel : BaseViewModel
    {        
        public ScreenshotsDisplayViewModel()
        {
            ScreenshotsManager.Instance.Collection.CurrentChanged += ScreenshotInfoCollection_CurrentChanged;

            LockScreenManager.Instance.StatusEvent += Instance_StatusEvent;
        }

        void Instance_StatusEvent(LockScreenManager.Status aStatus)
        {
            if (aStatus == LockScreenManager.Status.None)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
                {
                    ScreenshotsManager.Instance.Collection.MoveCurrentToPosition(-1);
                });

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate()
                {
                    ScreenshotsManager.Instance.Collection.MoveCurrentToPosition(-1);
                });

            }
        }

        void ScreenshotInfoCollection_CurrentChanged(object sender, EventArgs e)
        {
            var lICollectionView = sender as ICollectionView;

            if (lICollectionView == null)
                return;

            var lScreenshotInfo = lICollectionView.CurrentItem as ScreenshotInfo;

            if (lScreenshotInfo == null)
                return;

            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(lScreenshotInfo.FilePath);
            bitmap.EndInit();
            bitmap.Freeze();

            //ImageSource = bitmap;

            var l_Image = new System.Windows.Controls.Image();

            l_Image.Source = bitmap;

            MediaSource = l_Image;

            FileName = lScreenshotInfo.FileName;
        }

        public System.Windows.Controls.Image MediaSource
        {
            get { return (System.Windows.Controls.Image)GetValue(MediaSourceProperty); }
            set { SetValue(MediaSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaSourceProperty =
            DependencyProperty.Register(
            "MediaSource",
            typeof(System.Windows.Controls.Image),
            typeof(ScreenshotsDisplayViewModel),
            new PropertyMetadata(null));
        
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(ScreenshotsDisplayViewModel), new PropertyMetadata(""));

        public ICommand MoveToPreviousCommand
        {
            get { return new DelegateCommand(
                    ()=>
                    {
                        ScreenshotsManager.Instance.Collection.MoveCurrentToPrevious();

                        if (ScreenshotsManager.Instance.Collection.IsCurrentBeforeFirst)
                            ScreenshotsManager.Instance.Collection.MoveCurrentToLast();                
                    },
                    ()=>
                    {
                        return true;
                    }
                
                ); }
        }

        public ICommand MoveToNextRemoveCommand
        {
            get
            {
                return new DelegateCommand(
                    () =>
                    {
                        ScreenshotsManager.Instance.Collection.MoveCurrentToNext();

                        if (ScreenshotsManager.Instance.Collection.IsCurrentAfterLast)
                            ScreenshotsManager.Instance.Collection.MoveCurrentToFirst();
                    },
                    ()=>
                    {
                        return true;
                    }
                    );
            }
        }

        protected override Managers.IManager Manager
        {
            get { return ScreenshotsManager.Instance; }
        }
    }
}
