using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Models
{
    public class StreamingCaptureConfigInfo
    {
        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public bool Focusable { get { return false; } }
    }
}
