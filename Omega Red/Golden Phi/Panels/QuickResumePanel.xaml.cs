﻿using System;
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

namespace Golden_Phi.Panels
{
    /// <summary>
    /// Interaction logic for QuickResumePanel.xaml
    /// </summary>
    public partial class QuickResumePanel : UserControl
    {
        public QuickResumePanel()
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

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            m_closeVB.Visibility = Visibility.Hidden;

            m_closeVB.Visibility = Visibility.Visible;
        }
    }
}
