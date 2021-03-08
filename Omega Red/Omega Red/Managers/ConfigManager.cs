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

using Omega_Red.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Omega_Red.Managers
{
    class ConfigManager
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

        enum ControlMode
        {
            Button = 0,
            Touch = 1
        }

        class ControlModeInfo
        {
            public ControlMode Value { get; set; }

            public override string ToString()
            {
                return Value == ControlMode.Button ? App.Current.Resources["ControlModeButtonTitle"] as String :
                    App.Current.Resources["ControlModeTouchTitle"] as String;
            }
        }

        enum TexturePackMode
        {
            None = 0,
            Load = 1,
            Save = 2
        }

        class TexturePackModeInfo
        {
            public TexturePackMode Value { get; set; }

            public override string ToString()
            {
                switch (Value)
                {
                    case TexturePackMode.None:
                        return App.Current.Resources["TexturePackModeNoneTitle"] as String;
                    case TexturePackMode.Load:
                        return App.Current.Resources["TexturePackModeLoadTitle"] as String;
                    case TexturePackMode.Save:
                        return App.Current.Resources["TexturePackModeSaveTitle"] as String;
                    default:
                        return "";
                }
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

        private ICollectionView mDisplayModeView = null;

        private readonly ObservableCollection<DisplayModeInfo> _displayModeCollection = new ObservableCollection<DisplayModeInfo>();
       


        private ICollectionView mControlModeView = null;

        private readonly ObservableCollection<ControlModeInfo> _controlModeCollection = new ObservableCollection<ControlModeInfo>();



        private ICollectionView mSkipFrameModeView = null;

        private readonly ObservableCollection<SkipFrameModeInfo> _skipFrameModeCollection = new ObservableCollection<SkipFrameModeInfo>();



        private ICollectionView mResolutionModeView = null;

        private readonly ObservableCollection<ResolutionModeInfo> _resolutionModeCollection = new ObservableCollection<ResolutionModeInfo>();


        




        private ICollectionView mLanguageModeView = null;

        private readonly ObservableCollection<String> _languageModeCollection = new ObservableCollection<String>();




        private ICollectionView mColourSchemaModeView = null;

        private readonly ObservableCollection<String> _colourSchemaCollection = new ObservableCollection<String>();




        private ICollectionView mTexturePackModeView = null;

        private readonly ObservableCollection<TexturePackModeInfo> _TexturePackModeCollection = new ObservableCollection<TexturePackModeInfo>();






        private ICollectionView mMediaOutputTypeModeView = null;

        private readonly ObservableCollection<MediaOutputTypeInfo> _mediaOutputTypeCollection = new ObservableCollection<MediaOutputTypeInfo>();




        private ICollectionView mRenderingSchemaModeView = null;

        private readonly ObservableCollection<string> _renderingSchemaCollection = new ObservableCollection<string>();



        private ResourceDictionary m_languageResource = null;

        private ResourceDictionary m_colourSchemaResource = null;

        private static ConfigManager m_Instance = null;

        public static ConfigManager Instance { get { if (m_Instance == null) m_Instance = new ConfigManager(); return m_Instance; } }

        public event Action<bool> SwitchDisplayModeEvent;

        public event Action<bool> SwitchControlModeEvent;

        public event Action<bool> SwitchTopmostEvent;

        public event Action<Object> SwitchCaptureConfigEvent;

        public event Action<int> DisplayFrameEvent;

        public event Action<string> FrameRateEvent;

        public event Action<uint> ResolutionEvent;

        private ConfigManager()
        {
            System.Reflection.Assembly l_assembly = Assembly.GetExecutingAssembly();

            _languageModeCollection.Add("Русский");

            foreach (var item in l_assembly.GetManifestResourceNames())
            {
                if (item.Contains("Omega_Red.Captions.Translates."))
                {
                    _languageModeCollection.Add(item.Replace("Omega_Red.Captions.Translates.", "").Replace(".xaml", ""));
                }
            }

            mLanguageModeView = CollectionViewSource.GetDefaultView(_languageModeCollection);

            mLanguageModeView.MoveCurrentTo(Settings.Default.Language);
            

            App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {

                _displayModeCollection.Add(new DisplayModeInfo() { Value = DisplayMode.Window });

                _displayModeCollection.Add(new DisplayModeInfo() { Value = DisplayMode.Full });

                mDisplayModeView = CollectionViewSource.GetDefaultView(_displayModeCollection);




                _skipFrameModeCollection.Add(new SkipFrameModeInfo() { Value = SkipFrameMode.None });

                _skipFrameModeCollection.Add(new SkipFrameModeInfo() { Value = SkipFrameMode.One });

                _skipFrameModeCollection.Add(new SkipFrameModeInfo() { Value = SkipFrameMode.Two });

                mSkipFrameModeView = CollectionViewSource.GetDefaultView(_skipFrameModeCollection);




                _resolutionModeCollection.Add(new ResolutionModeInfo() { Value = 720 });

                _resolutionModeCollection.Add(new ResolutionModeInfo() { Value = 1080 });

                _resolutionModeCollection.Add(new ResolutionModeInfo() { Value = 1440 });

                _resolutionModeCollection.Add(new ResolutionModeInfo() { Value = 2160 });

                mResolutionModeView = CollectionViewSource.GetDefaultView(_resolutionModeCollection); 




                _controlModeCollection.Add(new ControlModeInfo() { Value = ControlMode.Button });

                _controlModeCollection.Add(new ControlModeInfo() { Value = ControlMode.Touch });

                mControlModeView = CollectionViewSource.GetDefaultView(_controlModeCollection);


                                             


                



                _colourSchemaCollection.Add("Default");

                foreach (var item in l_assembly.GetManifestResourceNames())
                {
                    if (item.Contains("Omega_Red.ColourSchemes."))
                    {
                        _colourSchemaCollection.Add(item.Replace("Omega_Red.ColourSchemes.", "").Replace(".xaml", ""));
                    }
                }

                mColourSchemaModeView = CollectionViewSource.GetDefaultView(_colourSchemaCollection);
                



                mMediaOutputTypeModeView = CollectionViewSource.GetDefaultView(_mediaOutputTypeCollection);





                _renderingSchemaCollection.Add("Default");

                _renderingSchemaCollection.Add("Tessellated");

                mRenderingSchemaModeView = CollectionViewSource.GetDefaultView(_renderingSchemaCollection);





                _TexturePackModeCollection.Add(new TexturePackModeInfo() { Value = TexturePackMode.None });

                _TexturePackModeCollection.Add(new TexturePackModeInfo() { Value = TexturePackMode.Load });

                _TexturePackModeCollection.Add(new TexturePackModeInfo() { Value = TexturePackMode.Save });

                


                mTexturePackModeView = CollectionViewSource.GetDefaultView(_TexturePackModeCollection);










                mDisplayModeView.CurrentChanged += mDisplayModeView_CurrentChanged;

                mSkipFrameModeView.CurrentChanged += mSkipFrameModeView_CurrentChanged;

                mResolutionModeView.CurrentChanged += mResolutionModeView_CurrentChanged;

                mControlModeView.CurrentChanged += mControlModeView_CurrentChanged;
                
                mLanguageModeView.CurrentChanged += mLanguageModeView_CurrentChanged;

                mColourSchemaModeView.CurrentChanged += mColourSchemaModeView_CurrentChanged;

                mMediaOutputTypeModeView.CurrentChanged += mMediaOutputTypeModeView_CurrentChanged;

                mRenderingSchemaModeView.CurrentChanged += mRenderingSchemaCollection_CurrentChanged;

                mTexturePackModeView.CurrentChanged += mTexturePackModeView_CurrentChanged;


                if (SwitchTopmostEvent != null)
                    SwitchTopmostEvent(Settings.Default.Topmost);


                _mediaOutputTypeCollection.Add(new MediaOutputTypeInfo(MediaOutputType.Capture));

                _mediaOutputTypeCollection.Add(new MediaOutputTypeInfo(MediaOutputType.Stream));

                reset();
            });

        }

        private void mResolutionModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mResolutionModeView.CurrentItem == null)
                return;

            var l_ResolutionModeInfo = (ResolutionModeInfo)mResolutionModeView.CurrentItem;

            if (ResolutionEvent != null)
                ResolutionEvent(l_ResolutionModeInfo.Value);

            Settings.Default.ResolutionMode = mResolutionModeView.CurrentPosition;
        }

        private void mSkipFrameModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mSkipFrameModeView.CurrentItem == null)
                return;

            var l_SkipFrameModeInfo = (SkipFrameModeInfo)mSkipFrameModeView.CurrentItem;

            if (SwitchDisplayModeEvent == null)
                return;

            var l_displayFrame = (int)l_SkipFrameModeInfo.Value;

            if (DisplayFrameEvent != null)
                DisplayFrameEvent(l_displayFrame);

            Settings.Default.SkipFrameMode = l_displayFrame;
        }

        private void mTexturePackModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mTexturePackModeView == null)
                return;

            if (mTexturePackModeView.CurrentItem == null)
                return;

            if (App.Current == null)
                return;

            if (App.Current.MainWindow == null)
                return;


            var l_TexturePackModeInfo = (TexturePackModeInfo)mTexturePackModeView.CurrentItem;

            if (l_TexturePackModeInfo == null)
                return;
            
            Settings.Default.TexturePackMode = l_TexturePackModeInfo.Value.ToString();
        }

        private void reset()
        {

            App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                mColourSchemaModeView.MoveCurrentToPosition(-1);

                mColourSchemaModeView.MoveCurrentTo(Settings.Default.ColourSchema);


                DisplayMode l_DisplayMode = DisplayMode.Window;

                Enum.TryParse<DisplayMode>(Settings.Default.DisplayMode, out l_DisplayMode);

                mDisplayModeView.MoveCurrentToPosition(-1);

                mDisplayModeView.MoveCurrentToPosition((int)l_DisplayMode);

                
                SkipFrameMode l_SkipFrameMode = (SkipFrameMode)Enum.ToObject(typeof(SkipFrameMode), Settings.Default.SkipFrameMode);
                
                mSkipFrameModeView.MoveCurrentToPosition(-1);

                mSkipFrameModeView.MoveCurrentToPosition(((int)l_SkipFrameMode) - 1);



                mResolutionModeView.MoveCurrentToPosition(-1);

                mResolutionModeView.MoveCurrentToPosition(Settings.Default.ResolutionMode);




                ControlMode l_ControlMode = ControlMode.Button;

                Enum.TryParse<ControlMode>(Settings.Default.ControlMode, out l_ControlMode);

                mControlModeView.MoveCurrentToPosition(-1);

                mControlModeView.MoveCurrentToPosition((int)l_ControlMode);

                               
                if (SwitchTopmostEvent != null)
                    SwitchTopmostEvent(Settings.Default.Topmost);



                mRenderingSchemaModeView.MoveCurrentToPosition(-1);
                mRenderingSchemaModeView.MoveCurrentTo(Settings.Default.RenderingSchema);



                mColourSchemaModeView.MoveCurrentToPosition(-1);
                mColourSchemaModeView.MoveCurrentTo(Settings.Default.ColourSchema);




                MediaOutputType l_MediaOutputType = MediaOutputType.Capture;

                Enum.TryParse<MediaOutputType>(Settings.Default.MediaOutputType, out l_MediaOutputType);

                mMediaOutputTypeModeView.MoveCurrentToPosition(-1);
                mMediaOutputTypeModeView.MoveCurrentToPosition((int)l_MediaOutputType);



                TexturePackMode l_TexturePackMode = TexturePackMode.None;

                Enum.TryParse<TexturePackMode>(Settings.Default.TexturePackMode, out l_TexturePackMode);

                mTexturePackModeView.MoveCurrentToPosition(-1);
                mTexturePackModeView.MoveCurrentToPosition((int)l_TexturePackMode);


                PadControlManager.Instance.reset();
            });
        }


        private void mRenderingSchemaCollection_CurrentChanged(object sender, EventArgs e)
        {
            if (mRenderingSchemaModeView == null)
                return;

            if (mRenderingSchemaModeView.CurrentItem == null)
                return;

            if (App.Current == null)
                return;

            if (App.Current.MainWindow == null)
                return;

            string ltitle = (string)mRenderingSchemaModeView.CurrentItem;

            if (string.IsNullOrEmpty(ltitle))
                return;

            //if(ltitle == "Tessellated")
            //{
            //    Tools.ModuleControl.Instance.setIsTessellated(true);
            //}
            //else
            //{
            //    Tools.ModuleControl.Instance.setIsTessellated(false);
            //}

            Settings.Default.RenderingSchema = ltitle;
        }

        private void mMediaOutputTypeModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mMediaOutputTypeModeView == null)
                return;

            if (mMediaOutputTypeModeView.CurrentItem == null)
                return;

            if (App.Current == null)
                return;

            if (App.Current.MainWindow == null)
                return;

            var l_MediaOutputType = (MediaOutputTypeInfo)mMediaOutputTypeModeView.CurrentItem;

            if(SwitchCaptureConfigEvent != null)
                SwitchCaptureConfigEvent(l_MediaOutputType.InfoPanel());

            Managers.MediaRecorderManager.Instance.setMediaOutputType(l_MediaOutputType.Value);

            Settings.Default.MediaOutputType = l_MediaOutputType.Value.ToString();
        }

        void mColourSchemaModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mColourSchemaModeView == null)
                return;

            if (mColourSchemaModeView.CurrentItem == null)
                return;

            if (App.Current == null)
                return;

            if (App.Current.MainWindow == null)
                return;

            loadColourSchema(mColourSchemaModeView.CurrentItem as string);
        }

        private void loadColourSchema(string a_colourSchema)
        {
            System.Reflection.Assembly l_assembly = Assembly.GetExecutingAssembly();

            using (var lStream = l_assembly.GetManifestResourceStream(
                string.Format("Omega_Red.ColourSchemes.{0}.xaml", a_colourSchema)))
            {
                if (m_colourSchemaResource != null)
                    App.Current.Resources.MergedDictionaries.Remove(m_colourSchemaResource);

                m_colourSchemaResource = null;

                if (lStream == null)
                {
                    Settings.Default.ColourSchema = "Default";

                    return;
                }

                m_colourSchemaResource = (ResourceDictionary)XamlReader.Load(lStream);

                if (m_colourSchemaResource != null)
                {
                    App.Current.Resources.MergedDictionaries.Add(m_colourSchemaResource);

                    Settings.Default.ColourSchema = a_colourSchema;
                }
            }
        }

        void mLanguageModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mLanguageModeView == null)
                return;

            if (mLanguageModeView.CurrentItem == null)
                return;

            if (App.Current == null)
                return;

            if (App.Current.MainWindow == null)
                return;

            loadLanguage(mLanguageModeView.CurrentItem as string);

            reset();
        }

        private void loadLanguage(string a_language)
        {
            System.Reflection.Assembly l_assembly = Assembly.GetExecutingAssembly();

            using (var lStream = l_assembly.GetManifestResourceStream(
                string.Format("Omega_Red.Captions.Translates.{0}.xaml", a_language)))
            {
                if (m_languageResource != null)
                    App.Current.Resources.MergedDictionaries.Remove(m_languageResource);

                m_languageResource = null;

                if (lStream == null)
                {
                    Settings.Default.Language = "Русский";

                    return;
                }

                m_languageResource = (ResourceDictionary)XamlReader.Load(lStream);

                if (m_languageResource != null)
                {
                    App.Current.Resources.MergedDictionaries.Add(m_languageResource);

                    Settings.Default.Language = a_language;
                }
            }
        }

        void mControlModeView_CurrentChanged(object sender, EventArgs e)
        {
            if (mControlModeView == null)
                return;

            if (mControlModeView.CurrentItem == null)
                return;

            if (App.Current == null)
                return;

            if (App.Current.MainWindow == null)
                return;

            var l_ControlMode = (ControlModeInfo)mControlModeView.CurrentItem;

            if (SwitchControlModeEvent == null)
                return;

            switch (l_ControlMode.Value)
            {
                case ControlMode.Button:
                    {
                        SwitchControlModeEvent(true);
                    }
                    break;
                case ControlMode.Touch:
                    {
                        SwitchControlModeEvent(false);
                    }
                    break;
                default:
                    break;
            }


            Settings.Default.ControlMode = l_ControlMode.Value.ToString();
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
        
        public void setFrameRate(string a_frameRate)
        {
            if (FrameRateEvent != null)
                FrameRateEvent(a_frameRate);
        }

        public ICollectionView DisplayModeCollection
        {
            get { return mDisplayModeView; }
        }

        public ICollectionView SkipFrameModeCollection
        {
            get { return mSkipFrameModeView; }
        }

        public ICollectionView ResolutionModeCollection
        {
            get { return mResolutionModeView; }
        }                      

        public ICollectionView ControlModeCollection
        {
            get { return mControlModeView; }
        }
        
        public ICollectionView LanguageCollection
        {
            get { return mLanguageModeView; }
        }

        public ICollectionView ColourSchemaCollection
        {
            get { return mColourSchemaModeView; }
        }        

        public ICollectionView MediaOutputTypeCollection
        {
            get { return mMediaOutputTypeModeView; }
        }
        public ICollectionView RenderingSchemaCollection
        {
            get { return mRenderingSchemaModeView; }
        }

        public ICollectionView TexturePackModeCollection
        {
            get { return mTexturePackModeView; }
        }
    }
}
