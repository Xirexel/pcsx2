using Omega_Red.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.ViewModels
{
    class RecordControlViewModel : BaseViewModel
    {
        public RecordControlViewModel() { }

        protected override IManager Manager => StreamingCaptureConfigManager.Instance;

        public ICollectionView VideoBitRateCollection => Capture.MediaCapture.Instance.VideoBitRateCollection;
        public ICollectionView AudioBitRateCollection => Capture.MediaCapture.Instance.AudioBitRateCollection;
    }
}
