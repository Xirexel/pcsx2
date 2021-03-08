using Omega_Red.Emulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Omega_Red.Panels
{
    /// <summary>
    /// Interaction logic for DisplayControl.xaml
    /// </summary>
    public partial class DisplayControl : UserControl
    {
        public DisplayControl()
        {
            InitializeComponent();

            TouchPadPanel.VibrationEvent += Instance_VibrationEvent;

            Emul.Instance.ChangeStatusEvent += Instance_ChangeStatusEvent;
        }

        private void Instance_ChangeStatusEvent(Emul.StatusEnum obj)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Background,
            (System.Threading.ThreadStart)delegate ()
            {
                m_vibrationRotateTransform.Angle = 0;
            });
        }

        private void Instance_VibrationEvent(uint arg1, uint arg2)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Background,
            (System.Threading.ThreadStart)delegate ()
            {
                m_vibrationRotateTransform.Angle = 20.0 * ((double)arg2 / (double)0xFFFFF) - 20.0 * ((double)arg1 / (double)0xFFFFF);
            });
        }

        internal VideoPanel VideoPanel { get { return (VideoPanel)m_TargetRenderBorder.Child; } }

        internal void addSource(MediaSourcePanel a_VideoPanel)
        {
            m_SourceCanvas.Children.Add(a_VideoPanel);

            Canvas.SetLeft(a_VideoPanel, 0);

            Canvas.SetTop(a_VideoPanel, 0);
        }

        internal void removeSource(string a_SymbolicLink)
        {
            for (int i = 0; i < m_SourceCanvas.Children.Count; i++)
            {
                var lVideoPanel = m_SourceCanvas.Children[i] as MediaSourcePanel;

                if(lVideoPanel != null && lVideoPanel.SymbolicLink == a_SymbolicLink)
                {
                    m_SourceCanvas.Children.RemoveAt(i);

                    lVideoPanel.releaseResource();

                    GC.Collect();

                    GC.WaitForFullGCComplete();

                    break;
                }
            }
        }

        internal void setOpacity(string a_SymbolicLink, float a_Opacity)
        {
            for (int i = 0; i < m_SourceCanvas.Children.Count; i++)
            {
                var lVideoPanel = m_SourceCanvas.Children[i] as MediaSourcePanel;

                if (lVideoPanel != null && lVideoPanel.SymbolicLink == a_SymbolicLink)
                {
                    lVideoPanel.Opacity = a_Opacity;

                    break;
                }
            }
        }
    }
}
