using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Omega_Red.Managers;

namespace Omega_Red.ViewModels
{
    class StreamControlViewModel : BaseViewModel
    {
        public StreamControlViewModel(){}
        
        protected override IManager Manager => StreamingCaptureConfigManager.Instance;
               
        public ICollectionView VideoBitRateCollection => Capture.MediaStream.Instance.VideoBitRateCollection;
        public ICollectionView AudioBitRateCollection => Capture.MediaStream.Instance.AudioBitRateCollection;
    }
}
