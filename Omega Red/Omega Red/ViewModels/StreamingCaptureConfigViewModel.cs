using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Omega_Red.Managers;
using Omega_Red.Models;
using Omega_Red.SocialNetworks.Google;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("StreamingCaptureConfigInfoItem")]
    class StreamingCaptureConfigViewModel : BaseViewModel
    {
        public StreamingCaptureConfigViewModel(){}
                        
        string m_StreamName;

        public string StreamName
        {
            get { return m_StreamName; }
            set
            {
                m_StreamName = value;
                RaisePropertyChangedEvent("StreamName");
            }
        }

        protected override Managers.IManager Manager
        {
            get { return StreamingCaptureConfigManager.Instance; }
        }
    }
}
