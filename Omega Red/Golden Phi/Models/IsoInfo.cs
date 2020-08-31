using Golden_Phi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Golden_Phi.Models
{
    public class IsoInfo : ObservableObject
    {
        public string Title { get; set; }

        public string IsoType { get; set; }

        public string GameDiscType { get; set; }

        public string DiscSerial { get; set; }

        public string DiscRegionType { get; set; }

        public string SoftwareVersion { get; set; }

        public uint ElfCRC { get; set; }

        public string FilePath { get; set; }

        [XmlIgnoreAttribute]
        private bool mIsCurrent = false;

        public bool IsCurrent
        {
            get { return mIsCurrent; }
            set
            {
                mIsCurrent = value;
                RaisePropertyChangedEvent("IsCurrent");
            }
        }

        public Emul.GameType GameType { get; set; }

        public bool IsCloudsave { get; set; }

        public bool IsCloudOnlysave { get; set; }

        [XmlIgnoreAttribute]
        private DateTime mLastLaunchTime = DateTime.Now;

        public DateTime LastLaunchTime
        {
            get { return mLastLaunchTime; }
            set
            {
                mLastLaunchTime = value;
                RaisePropertyChangedEvent("LastLaunchTime");
            }
        }

        public string SelectedMemoryCardFile { get; set; }

        public string BIOSFile { get; set; } = "";

        [XmlIgnoreAttribute]
        private string mBIOS = "";

        public string BIOS
        {
            get { return mBIOS; }
            set
            {
                mBIOS = value;
                RaisePropertyChangedEvent("BIOS");
            }
        }

        [XmlIgnoreAttribute]
        public BiosInfo BiosInfo { get; set; } = null;

        [XmlIgnoreAttribute]
        private Image mActiveStateImage = null;

        [XmlIgnoreAttribute]
        public Image ActiveStateImage
        {
            get { return mActiveStateImage; }
            set
            {
                mActiveStateImage = value;
                RaisePropertyChangedEvent("ActiveStateImage");
            }
        }

        [XmlIgnoreAttribute]
        private System.Windows.Media.ImageSource mImageSource = null;

        [XmlIgnoreAttribute]
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
