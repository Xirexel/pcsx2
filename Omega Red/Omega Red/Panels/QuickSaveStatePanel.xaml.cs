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
    /// Interaction logic for QuickSaveStatePanel.xaml
    /// </summary>
    public partial class QuickSaveStatePanel : UserControl
    {
        public QuickSaveStatePanel()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var l_listView = sender as ListView;

            if (l_listView != null && l_listView.SelectedItem != null)
            {
                l_listView.ScrollIntoView(l_listView.SelectedItem);
            }
        }
    }
}
