using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Golden_Phi.Emulators;
using Golden_Phi.Managers;
using Golden_Phi.Properties;

namespace Golden_Phi.ViewModels
{
    [DataTemplateNameAttribute("ConfigItem")]
    class ConfigViewModel : BaseViewModel
    {
        public ConfigViewModel()
        {
            ItemsPanelTemplateSelector = SelectTemplate("VerticalButtonItemsPanel");

            if(Manager != null && Manager.Collection != null)
            {
                Manager.Collection.CurrentChanged += (sender, e)=>{
                    var l_element = Manager.Collection.CurrentItem as System.Windows.FrameworkElement;

                    CurrentItemDataContext = null;

                    if (l_element != null)
                        CurrentItemDataContext = l_element.DataContext;
                };
            }

            ConfigManager.Instance.FrameRateEvent += (a_framerate) => { FrameRate = a_framerate; };

            Emul.Instance.ChangeStatusEvent += (a_State) => { IsStopped = a_State == Emul.StatusEnum.Stopped; };
        }
        
        private object mCurrentItemDataContext = null;

        public object CurrentItemDataContext
        {
            get { return mCurrentItemDataContext; }
            set
            {
                mCurrentItemDataContext = value;
                RaisePropertyChangedEvent("CurrentItemDataContext");
            }
        }

        protected override IManager Manager => ConfigManager.Instance;
        
        public ICollectionView DisplayModeCollection
        {
            get { return ConfigManager.Instance.DisplayModeCollection; }
        }

        public ICollectionView SkipFrameModeCollection
        {
            get { return ConfigManager.Instance.SkipFrameModeCollection; }
        }

        public ICollectionView ResolutionModeCollection
        {
            get { return ConfigManager.Instance.ResolutionModeCollection; }
        }

        public ICollectionView LanguageCollection
        {
            get { return ConfigManager.Instance.LanguageCollection; }
        }

        private string mFrameRate = "";

        public string FrameRate
        {
            get
            {
                return mFrameRate;
            }
            set
            {
                mFrameRate = value;

                RaisePropertyChangedEvent("FrameRate");
            }
        }

        private bool mIsStopped = true;

        public bool IsStopped
        {
            get
            {
                return mIsStopped;
            }
            set
            {
                mIsStopped = value;

                RaisePropertyChangedEvent("IsStopped");
            }
        }

        public double SoundLevel
        {
            get
            {
                Emul.Instance.setAudioVolume(Settings.Default.SoundLevel);

                return Settings.Default.SoundLevel;
            }
            set
            {
                Settings.Default.SoundLevel = value;

                Emul.Instance.setAudioVolume(Settings.Default.SoundLevel);

                RaisePropertyChangedEvent("SoundLevel");
            }
        }

        public bool IsMuted
        {
            get
            {
                Emul.Instance.setIsMuted(Settings.Default.IsMuted);

                return Settings.Default.IsMuted;
            }
            set
            {
                Settings.Default.IsMuted = value;

                Emul.Instance.setIsMuted(Settings.Default.IsMuted);

                RaisePropertyChangedEvent("IsMuted");
            }
        }
    }
}
