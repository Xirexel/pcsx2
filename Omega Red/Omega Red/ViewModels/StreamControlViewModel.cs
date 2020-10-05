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
        public StreamControlViewModel()
        {
            //StreamingCaptureConfigManager.Instance.m_StreamConfigPanelEvent += (panel)=>{ Panel = panel; };
        }

        //public ICommand ConfirmCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(StreamingCaptureConfigManager.Instance.confirm,
        //            StreamingCaptureConfigManager.Instance.isAllowedConfirm
        //            );
        //    }
        //}

        //public ICommand CancelCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(StreamingCaptureConfigManager.Instance.cancel);
        //    }
        //}


        //object m_Panel = null;

        //public object Panel
        //{
        //    get { return m_Panel; }
        //    set
        //    {
        //        m_Panel = value;
        //        RaisePropertyChangedEvent("Panel");
        //    }
        //}

        protected override IManager Manager => StreamingCaptureConfigManager.Instance;



        public ICollectionView VideoBitRateCollection => Capture.MediaStream.Instance.VideoBitRateCollection;
        public ICollectionView AudioBitRateCollection => Capture.MediaStream.Instance.AudioBitRateCollection;

    }
}
