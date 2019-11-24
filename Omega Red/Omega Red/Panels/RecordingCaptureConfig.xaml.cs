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
    /// Interaction logic for RecordingCaptureConfig.xaml
    /// </summary>
    public partial class RecordingCaptureConfig : UserControl
    {
        public RecordingCaptureConfig()
        {
            InitializeComponent();
        }

        private void M_FileSizeCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var l_ComboBoxItem = m_FileSizeCmbBx.SelectedItem as ComboBoxItem;

            if(l_ComboBoxItem != null && l_ComboBoxItem.Tag != null)
            {
                var l_sizeFileString = l_ComboBoxItem.Tag.ToString();

                int l_sizeFile = -1;

                if(int.TryParse(l_sizeFileString, out l_sizeFile))
                {
                    Capture.MediaCapture.Instance.setFileSize(l_sizeFile);
                }
            }
        }
    }
}
