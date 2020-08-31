using Golden_Phi.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace Golden_Phi.Managers
{
    class ConfigManager : IManager
    {
        enum DisplayMode
        {
            Window = 0,
            Full = 1
        }

        class DisplayModeInfo
        {
            public DisplayMode Value { get; set; }

            public override string ToString()
            {
                return Value == DisplayMode.Window ? App.Current.Resources["DisplayModeWindowTitle"] as String :
                    App.Current.Resources["DisplayModeFullTitle"] as String;
            }
        }


        enum SkipFrameMode
        {
            None = 1,
            One = 2,
            Two = 3
        }

        class SkipFrameModeInfo
        {
            public SkipFrameMode Value { get; set; }

            public override string ToString()
            {
                switch (Value)
                {
                    case SkipFrameMode.None:
                        return App.Current.Resources["SkipFrameModeNoneTitle"] as String;
                    case SkipFrameMode.One:
                        return App.Current.Resources["SkipFrameModeOneTitle"] as String;
                    case SkipFrameMode.Two:
                        return App.Current.Resources["SkipFrameModeTwoTitle"] as String;
                    default:
                        return "";
                }
            }
        }

        class ResolutionModeInfo
        {
            public uint Value { get; set; } = 0;

            public override string ToString()
            {
                if (Value == 0)
                    return "";

                return Value.ToString() + "p";
            }
        }



        public event Action<uint> ResolutionEvent;

        public event Action<bool> SwitchDisplayModeEvent;

        public event Action<int> DisplayFrameEvent;

        public event Action<string> FrameRateEvent;



        private ICollectionView mResolutionModeView = null;

        private readonly ObservableCollection<ResolutionModeInfo> _resolutionModeCollection = new ObservableCollection<ResolutionModeInfo>();



        private ICollectionView mSkipFrameModeView = null;

        private readonly ObservableCollection<SkipFrameModeInfo> _skipFrameModeCollection = new ObservableCollection<SkipFrameModeInfo>();
        


        private ICollectionView mDisplayModeView = null;

        private readonly ObservableCollection<DisplayModeInfo> _displayModeCollection = new ObservableCollection<DisplayModeInfo>();


        private ICollectionView mCustomerView = null;
                     
        private static ConfigManager m_Instance = null;

        public static ConfigManager Instance { get { if (m_Instance == null) m_Instance = new ConfigManager(); return m_Instance; } }

        private ConfigManager()
        {
            var l_array = App.getResource("ConfigItemArray") as Array;

            if (l_array != null)
            {
                mCustomerView = CollectionViewSource.GetDefaultView(l_array.Cast<Object>());
            }

            View = App.getResource("ConfigView");

            _skipFrameModeCollection.Add(new SkipFrameModeInfo() { Value = SkipFrameMode.None });

            _skipFrameModeCollection.Add(new SkipFrameModeInfo() { Value = SkipFrameMode.One });

            _skipFrameModeCollection.Add(new SkipFrameModeInfo() { Value = SkipFrameMode.Two });

            mSkipFrameModeView = CollectionViewSource.GetDefaultView(_skipFrameModeCollection);

            mSkipFrameModeView.CurrentChanged += mSkipFrameModeView_CurrentChanged;



            _resolutionModeCollection.Add(new ResolutionModeInfo() { Value = 720 });

            _resolutionModeCollection.Add(new ResolutionModeInfo() { Value = 1080 });

            _resolutionModeCollection.Add(new ResolutionModeInfo() { Value = 1440 });

            _resolutionModeCollection.Add(new ResolutionModeInfo() { Value = 2160 });

            mResolutionModeView = CollectionViewSource.GetDefaultView(_resolutionModeCollection);

            mResolutionModeView.CurrentChanged += mResolutionModeView_CurrentChanged;



            _displayModeCollection.Add(new DisplayModeInfo() { Value = DisplayMode.Window });

            _displayModeCollection.Add(new DisplayModeInfo() { Value = DisplayMode.Full });

            mDisplayModeView = CollectionViewSource.GetDefaultView(_displayModeCollection);

            mDisplayModeView.CurrentChanged += mDisplayModeView_CurrentChanged;


            reset();
        }

        private void mSkipFrameModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mSkipFrameModeView.CurrentItem == null)
                return;

            var l_SkipFrameModeInfo = (SkipFrameModeInfo)mSkipFrameModeView.CurrentItem;
            
            var l_displayFrame = (int)l_SkipFrameModeInfo.Value;

            if (DisplayFrameEvent != null)
                DisplayFrameEvent(l_displayFrame);

            Settings.Default.SkipFrameMode = l_displayFrame;
        }

        private void mResolutionModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mResolutionModeView.CurrentItem == null)
                return;

            if(!(Emul.Emul.Instance.Status == Emul.Emul.StatusEnum.Stopped))
                return;

            var l_ResolutionModeInfo = (ResolutionModeInfo)mResolutionModeView.CurrentItem;

            if (ResolutionEvent != null)
                ResolutionEvent(l_ResolutionModeInfo.Value);

            Settings.Default.ResolutionMode = mResolutionModeView.CurrentPosition;
        }

        void mDisplayModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mDisplayModeView.CurrentItem == null)
                return;

            var l_DisplayMode = (DisplayModeInfo)mDisplayModeView.CurrentItem;

            if (SwitchDisplayModeEvent == null)
                return;

            switch (l_DisplayMode.Value)
            {
                case DisplayMode.Full:
                    SwitchDisplayModeEvent(true);
                    break;
                case DisplayMode.Window:
                    SwitchDisplayModeEvent(false);
                    break;
                default:
                    break;
            }

            Settings.Default.DisplayMode = l_DisplayMode.Value.ToString();
        }

        public ICollectionView Collection => mCustomerView;
        
        public ICollectionView DisplayModeCollection
        {
            get { return mDisplayModeView; }
        }

        public ICollectionView ResolutionModeCollection
        {
            get { return mResolutionModeView; }
        }

        public ICollectionView SkipFrameModeCollection
        {
            get { return mSkipFrameModeView; }
        }

        private void reset()
        {
            App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                mResolutionModeView.MoveCurrentToPosition(-1);

                mResolutionModeView.MoveCurrentToPosition(Settings.Default.ResolutionMode);



                DisplayMode l_DisplayMode = DisplayMode.Window;

                Enum.TryParse<DisplayMode>(Settings.Default.DisplayMode, out l_DisplayMode);

                mDisplayModeView.MoveCurrentToPosition(-1);

                mDisplayModeView.MoveCurrentToPosition((int)l_DisplayMode);
            });
        }

        public void setFrameRate(string a_frameRate)
        {
            if (FrameRateEvent != null)
                FrameRateEvent(a_frameRate);
        }

        public void createItem()
        {
        }

        public void removeItem(object a_Item)
        {
        }

        public object View { get; private set; }
    }
}
