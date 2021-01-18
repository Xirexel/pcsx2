using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Golden_Phi.Models
{
    public class MemoryCardInfo
    {
        public Visibility Visibility { get; set; }

        public string Number { get; set; }

        public int Index { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string Date { get; set; }

        public DateTime DateTime { get; set; }

        public string CloudSaveDate { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsCloudOnlysave { get; set; }

        public bool IsCloudsave { get; set; }

        public bool Focusable { get { return !IsCloudOnlysave; } }
    }
}
