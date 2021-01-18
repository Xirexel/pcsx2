using Golden_Phi.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Golden_Phi.ViewModels
{
    [DataTemplateNameAttribute("ScreenshotInfoItem")]
    class ScreenshotsInfoViewModel : BaseViewModel
    {
        public ScreenshotsInfoViewModel()
        {
            ScreenshotsManager.Instance.TakeScreenshotEvent += (a_state, a_source) => {

                if (a_state)
                {
                    LockVisibility = Visibility.Visible;
                }
                else
                {
                    LockVisibility = Visibility.Collapsed;
                }

                ScreenshotSource = a_source;
            };

            ScreenshotsManager.Instance.ShowImageEvent += (a_source) => {
                ImageSource = a_source;
            };
        }

        protected override IManager Manager
        {
            get { return ScreenshotsManager.Instance; }
        }

        private System.Windows.Visibility mLockVisibility = System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility LockVisibility
        {
            get { return mLockVisibility; }
            set
            {
                mLockVisibility = value;
                RaisePropertyChangedEvent("LockVisibility");
            }
        }

        private System.Windows.Media.ImageSource mScreenshotSource = null;

        public System.Windows.Media.ImageSource ScreenshotSource
        {
            get { return mScreenshotSource; }
            set
            {
                mScreenshotSource = value;
                RaisePropertyChangedEvent("ScreenshotSource");
            }
        }

        private System.Windows.Media.ImageSource mImageSource = null;

        public System.Windows.Media.ImageSource ImageSource
        {
            get { return mImageSource; }
            set
            {
                mImageSource = value;
                RaisePropertyChangedEvent("ImageSource");
            }
        }
    }
}
