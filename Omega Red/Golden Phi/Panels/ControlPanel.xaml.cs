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
using System.Windows.Threading;

namespace Golden_Phi.Panels
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : UserControl
    {
        public ControlPanel()
        {
            InitializeComponent();

            Managers.IsoManager.Instance.ShowChooseIsoEvent += (a_state) => {
                if (!a_state)
                {
                    ScrollToEnd(RecentIsoListView);
                }
            };
        }

        public static void ScrollToEnd(ListView listView)
        {

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate {

                if (listView.Items.Count > 0)

                {

                    listView.ScrollIntoView(listView.Items[0]);

                }

            }));

        }
    }
}
