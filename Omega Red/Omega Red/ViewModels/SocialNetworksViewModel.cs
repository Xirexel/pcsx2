using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Omega_Red.Managers;
using Omega_Red.SocialNetworks.Google;

namespace Omega_Red.ViewModels
{
    class SocialNetworksViewModel : BaseViewModel
    {
        public SocialNetworksViewModel()
        {
            GoogleAccountManager.Instance.mEnableStateEvent += Instance_mEnableStateEvent;
        }

        private void Instance_mEnableStateEvent(bool obj)
        {
            GoogleAccountIsChecked = obj;

            IsVisibilityGoogleAccount = obj ? Visibility.Visible : Visibility.Collapsed;

            IsVisibilityYouTubeVideo = Visibility.Collapsed;

            if (obj)
                GoogleAccountIsEnabled = GoogleAccountManager.Instance.isAuthorized;
            else
                GoogleAccountIsEnabled = false;
        }
                
        public ICommand SendVideoToYouTubeCommand
        {
            get
            {

                return new DelegateCommand<object>(async (object a_Item) => {

                    var lFE = a_Item as FrameworkElement;

                    if (lFE == null)
                        return;

                    string lfile_path = "";
                    string lTitle = "";
                    string lDescription = "";
                    string[] lTags = new string[] {};
                    string lCategoryId = "";
                    string lPrivacyStatus = "";

                    

                    var ltxtbx = lFE.FindName("mVideoTitleTxtbx") as TextBox;

                    if (ltxtbx != null && ltxtbx.Tag is Uri)
                    {
                        lfile_path = (ltxtbx.Tag as Uri).LocalPath;
                    }

                    if (ltxtbx != null && !string.IsNullOrWhiteSpace(ltxtbx.Text))
                        lTitle = ltxtbx.Text;

                    ltxtbx = lFE.FindName("mVideoDescriptionTxtbx") as TextBox;

                    if (ltxtbx != null && !string.IsNullOrWhiteSpace(ltxtbx.Text))
                        lDescription = ltxtbx.Text;

                    ltxtbx = lFE.FindName("YouTubeVideoTagsTitle") as TextBox;

                    if (ltxtbx != null && !string.IsNullOrWhiteSpace(ltxtbx.Text))
                        lTags = ltxtbx.Text.Split(new char[] {','});                  
                    
                    ltxtbx = lFE.FindName("mVideoCategoryIdTxtbx") as TextBox;

                    if (ltxtbx != null && !string.IsNullOrWhiteSpace(ltxtbx.Text))
                        lCategoryId = ltxtbx.Text;

                    var lCmbbx = lFE.FindName("mVideoPrivacyStatusCmbbx") as ComboBox;

                    if (lCmbbx != null)
                    {
                        var lCmbBx = lCmbbx.SelectedItem as ComboBoxItem;

                        if(lCmbBx != null)
                        {
                            var ltxtblk = lCmbBx.Content as TextBlock;

                            if (ltxtblk != null && !string.IsNullOrWhiteSpace(ltxtblk.Text))
                                lPrivacyStatus = ltxtblk.Text;
                        }
                    }

                    IsVisibilityYouTubeVideo = Visibility.Visible;

                    try
                    {
                        await GoogleAccountManager.Instance.startUploadingYouTubeVideoAsync(lfile_path,
                                                                        lTitle,
                                                                        lDescription,
                                                                        lTags,
                                                                        lCategoryId,
                                                                        lPrivacyStatus,
                                                                        (progress)=>{
                            YouTubeVideoUploadingProgress = progress;
                    });

                    }
                    finally
                    {
                        IsVisibilityYouTubeVideo = Visibility.Collapsed;
                    }

                });
                
            }
        }


        private float mYouTubeVideoUploadingProgress = 0;

        public float YouTubeVideoUploadingProgress
        {
            get { return mYouTubeVideoUploadingProgress; }
            set
            {
                mYouTubeVideoUploadingProgress = value;
                
                RaisePropertyChangedEvent("YouTubeVideoUploadingProgress");
            }
        }
               
        private bool mGoogleAccountIsChecked = false;

        public bool GoogleAccountIsChecked
        {
            get { return mGoogleAccountIsChecked; }
            set
            {
                mGoogleAccountIsChecked = value;

                GoogleAccountManager.Instance.enbleAccount(value);

                RaisePropertyChangedEvent("GoogleAccountIsChecked");
            }
        }

        private Visibility m_IsVisibilityGoogleAccount = Visibility.Collapsed;

        public Visibility IsVisibilityGoogleAccount
        {
            get { return m_IsVisibilityGoogleAccount; }
            set
            {
                m_IsVisibilityGoogleAccount = value;

                RaisePropertyChangedEvent("IsVisibilityGoogleAccount");
            }
        }


        private Visibility m_IsVisibilityYouTubeVideo = Visibility.Collapsed;

        public Visibility IsVisibilityYouTubeVideo
        {
            get { return m_IsVisibilityYouTubeVideo; }
            set
            {
                m_IsVisibilityYouTubeVideo = value;

                RaisePropertyChangedEvent("IsVisibilityYouTubeVideo");
            }
        }

        private bool mGoogleAccountIsEnabled = false;

        public bool GoogleAccountIsEnabled
        {
            get { return mGoogleAccountIsEnabled; }
            set
            {
                mGoogleAccountIsEnabled = value;
                
                RaisePropertyChangedEvent("GoogleAccountIsEnabled");
            }
        }

        protected override IManager Manager => null;
    }
}
