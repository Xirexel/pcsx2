using Golden_Phi.Emulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Golden_Phi.Models
{
    public class SaveStateInfo
    {
        public bool IsAutosave { get; set; }
        public bool IsQuicksave { get; set; }
        public bool IsCloudOnlysave { get; set; }
        public bool IsCloudsave { get; set; }
        public bool Focusable { get { return false; } }
        public string Date { get; set; }
        public DateTime DateTime { get; set; }
        public string Duration { get; set; }
        public TimeSpan DurationNative { get; set; }
        public uint CheckSum { get; set; }
        public int Index { get; set; }
        public string FilePath { get; set; }
        public bool IsCurrent { get; set; }
        public Visibility Visibility { get; set; }
        public System.Windows.Media.ImageSource ImageSource { get; set; }
        public GameType Type { get; set; }
        public object Item { get; set; }


        public string CloudSaveDate { get; set; }
        public string CloudSaveDuration { get; set; }

        public string DiscSerial { get; set; }
    }
}
