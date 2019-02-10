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

using Omega_Red.Managers;
using Omega_Red.Models;
using Omega_Red.Panels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Omega_Red.ViewModels
{
    class VideoDisplayViewModel : BaseViewModel
    {
        private DisplayVideoPanel mDisplayVideoPanel = new DisplayVideoPanel();

        public VideoDisplayViewModel()
        {
            MediaSource = mDisplayVideoPanel;

            Omega_Red.Managers.MediaRecorderManager.Instance.Collection.CurrentChanged += Collection_CurrentChanged;

            LockScreenManager.Instance.StatusEvent += Instance_StatusEvent;

            mDisplayVideoPanel.StatusEvent += mDisplayVideoPanel_StatusEvent;
        }

        void mDisplayVideoPanel_StatusEvent(string obj)
        {
            Status = obj;
        }

        void Instance_StatusEvent(LockScreenManager.Status aStatus)
        {
            if (aStatus == LockScreenManager.Status.None)
            {
                mDisplayVideoPanel.Release();

                Omega_Red.Managers.MediaRecorderManager.Instance.Collection.MoveCurrentToPosition(-1);
            }

        }

        void Collection_CurrentChanged(object sender, EventArgs e)
        {
            var lICollectionView = sender as ICollectionView;

            if (lICollectionView == null)
                return;

            var lMediaRecorderInfo = lICollectionView.CurrentItem as MediaRecorderInfo;

            if (lMediaRecorderInfo == null)
                return;

            if (mDisplayVideoPanel != null)
            {
                mDisplayVideoPanel.setSource(new Uri(lMediaRecorderInfo.FilePath, UriKind.RelativeOrAbsolute));

            }
                        
            FileName = lMediaRecorderInfo.FileName;
        }               

        public UserControl MediaSource
        {
            get { return (UserControl)GetValue(MediaSourceProperty); }
            set { SetValue(MediaSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaSourceProperty =
            DependencyProperty.Register(
            "MediaSource",
            typeof(UserControl),
            typeof(VideoDisplayViewModel),
            new PropertyMetadata(null));

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(VideoDisplayViewModel), new PropertyMetadata(""));

        public ICommand MoveToPreviousCommand
        {
            get
            {
                return new DelegateCommand(
                  () =>
                  {
                      Collection.MoveCurrentToPrevious();

                      if (Collection.IsCurrentBeforeFirst)
                         Collection.MoveCurrentToLast();
                  },
                  () =>
                  {
                      return true;
                  }

                    );
            }
        }

        public ICommand MoveToNextRemoveCommand
        {
            get
            {
                return new DelegateCommand(
                    () =>
                    {
                        Collection.MoveCurrentToNext();

                        if (Collection.IsCurrentAfterLast)
                            Collection.MoveCurrentToFirst();
                    },
                    () =>
                    {
                        return true;
                    }
                    );
            }
        }

        string m_Status = "Stopped";

        public string Status
        {
            get { return m_Status; }
            set
            {
                m_Status = value;
                RaisePropertyChangedEvent("Status");
            }
        }

        public ICommand PlayPauseCommand
        {
            get
            {
                return new DelegateCommand(mDisplayVideoPanel.PlayPause);
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return new DelegateCommand(mDisplayVideoPanel.Stop);
            }
        }

        protected override IManager Manager
        {
            get { return MediaRecorderManager.Instance; }
        }
    }
}
