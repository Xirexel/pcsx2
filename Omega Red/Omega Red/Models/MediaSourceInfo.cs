using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Omega_Red.Models
{
    public enum MediaSourceType
    {
        Video,
        Image,
        Audio
    }

    public class MediaSourceInfo
    {
        public string Name { get; set; }

        public MediaSourceType Type { get; set; }

        public string SymbolicLink { get; set; }

        public System.Xml.XmlNode MediaTypes { get; set; }

        public int SelectedMediaTypeIndex { get; set; }

        public System.Xml.XmlNode SelectedMediaType { get; set; }

        public bool IsSelected { get; set; }

        [XmlIgnoreAttribute]
        public System.Windows.Media.ImageSource ImageSource { get; set; }
        public bool Focusable { get { return false; } }

        public double Variable { get; set; }


        public double LeftProp { get; set; }

        public double RightProp { get; set; }
        
        public double TopProp { get; set; }

        public double BottomProp { get; set; }
    }
}
