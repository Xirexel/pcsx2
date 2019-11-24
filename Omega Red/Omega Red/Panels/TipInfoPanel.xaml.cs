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
    /// Interaction logic for TipInfoPanel.xaml
    /// </summary>
    public partial class TipInfoPanel : UserControl
    {
        public TipInfoPanel()
        {
            InitializeComponent();
        }
        public void setTipTitle(string a_title)
        {
            m_TipInfoTitleTextBlock.Text = a_title;
        }

        public void addHyperLink(string a_url)
        {
            var lHyperlink = new Hyperlink();

            lHyperlink.Inlines.Add(a_url);

            lHyperlink.Click += delegate (object sender, RoutedEventArgs e)
            {
                var lHyperlink1 = sender as Hyperlink;

                if (lHyperlink1 == null)
                    return;

                var lRun = lHyperlink1.Inlines.FirstInline as Run;

                if (lRun == null)
                    return;

                Native.openURL(lRun.Text);
            };

            m_TipInfoTextBlock.Inlines.Add(new LineBreak());

            m_TipInfoTextBlock.Inlines.Add(lHyperlink);
        }
    }
}
