using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Omega_Red.Managers;
using Omega_Red.Models;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("MediaSourceInfoItem")]
    class MediaSourcesInfoViewModel : BaseViewModel
    {
        public MediaSourcesInfoViewModel()
        {
            MediaSourcesManager.Instance.ChangeMediaSourcesAccessEvent += (state)=>{ IsEnabledMediaSources = state; };
            
            MediaSourcesManager.Instance.ChangeMediaSourceCheckEvent += (state) => { IsCheckedMediaSource = state; };

            MediaSourcesManager.Instance.ChangeVideoSourceAccessEvent += (state) => { LockVideoSourceVisibility = state? System.Windows.Visibility.Collapsed: System.Windows.Visibility.Visible; };

            MediaSourcesManager.Instance.ChangeAudioSourceAccessEvent += (state) => { LockAudioSourceVisibility = state ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible; };
        }

        public ICommand LaunchSource => new DelegateCommand<MediaSourceInfo>(MediaSourcesManager.Instance.launchSourceAsync);



        bool m_IsEnabledMediaSources = false;

        public bool IsEnabledMediaSources
        {
            get { return m_IsEnabledMediaSources; }
            set
            {
                m_IsEnabledMediaSources = value;
                RaisePropertyChangedEvent("IsEnabledMediaSources");
            }
        }



        bool m_IsCheckedMediaSource = false;

        public bool IsCheckedMediaSource
        {
            get { return m_IsCheckedMediaSource; }
            set
            {
                m_IsCheckedMediaSource = value;
                RaisePropertyChangedEvent("IsCheckedMediaSource");
            }
        }


        private System.Windows.Visibility mLockVideoSourceVisibility = System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility LockVideoSourceVisibility
        {
            get { return mLockVideoSourceVisibility; }
            set
            {
                mLockVideoSourceVisibility = value;
                RaisePropertyChangedEvent("LockVideoSourceVisibility");
            }
        }


        private System.Windows.Visibility mLockAudioSourceVisibility = System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility LockAudioSourceVisibility
        {
            get { return mLockAudioSourceVisibility; }
            set
            {
                mLockAudioSourceVisibility = value;
                RaisePropertyChangedEvent("LockAudioSourceVisibility");
            }
        }


        public ICommand ValueChangedCommand
        {
            get { return new DelegateCommand<object>(MediaSourcesManager.Instance.changeVariable); }
        }               

        protected override IManager Manager => MediaSourcesManager.Instance;
    }
}
