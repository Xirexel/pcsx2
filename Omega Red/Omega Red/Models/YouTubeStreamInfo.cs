using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Models
{
    public class YouTubeStreamInfo
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCreated { get; set; }

        public string Id { get; set; }

        public Google.Apis.YouTube.v3.Data.LiveStream Stream { get; set; }

        public override string ToString()
        {
            if (Title != null)
                return Title;

            return base.ToString();
        }
    }
}
