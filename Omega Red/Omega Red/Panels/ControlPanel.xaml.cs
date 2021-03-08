/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using Omega_Red.Emulators;
using Omega_Red.Managers;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Omega_Red.Panels
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : UserControl
    {
        public ControlPanel()
        {
            InitializeComponent();

            m_Panels.AddHandler(Expander.ExpandedEvent, new RoutedEventHandler(Expander_Expanded));

            ConfigManager.Instance.SwitchControlModeEvent += Instance_SwitchControlModeEvent;

            Emul.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;


            { 
                TextBlock lTextBlock = new TextBlock();

                lTextBlock.FontSize = 18;

                var lHyperlink = new Hyperlink();

                lHyperlink.Inlines.Add(@"rtsp://127.0.0.1:" + Properties.Settings.Default.OffScreenStreamerPort.ToString());

                lHyperlink.Click += delegate (object sender, RoutedEventArgs e)
                {
                    var lHyperlink1 = sender as Hyperlink;

                    if (lHyperlink1 == null)
                        return;

                    var lRun = lHyperlink1.Inlines.FirstInline as Run;

                    if (lRun == null)
                        return;

                    Clipboard.SetText(lRun.Text);

                    TextBlock linfoTextBlock = new TextBlock();

                    Popup mPopupCopy = new Popup();

                    linfoTextBlock.Padding = new Thickness(7);

                    linfoTextBlock.Foreground = Brushes.Black;

                    linfoTextBlock.Background = Brushes.LightBlue;

                    linfoTextBlock.FontSize = 20;

                    linfoTextBlock.Text = "Copying of " + lRun.Text + " to clipboard.";

                    mPopupCopy.Child = linfoTextBlock;

                    mPopupCopy.PlacementTarget = lTextBlock;

                    mPopupCopy.PopupAnimation = PopupAnimation.Slide;

                    mPopupCopy.IsOpen = true;

                    DoubleAnimation fadeInAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(1000)));

                    fadeInAnimation.Completed += delegate (object sender1, EventArgs e1)
                    {
                        mPopupCopy.IsOpen = false;
                    };
                    fadeInAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
                    mPopupCopy.Opacity = 0.0;
                    mPopupCopy.BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)fadeInAnimation.GetAsFrozen());
                };

                lTextBlock.Inlines.Add(lHyperlink);

                lTextBlock.Margin = new Thickness(10, 0, 0, 0);

                mIPList.Items.Add(lTextBlock);
            }

            foreach (var item in GetIPAddresses())
            {
                if(!item.IsIPv6LinkLocal)
                {
                    TextBlock lTextBlock = new TextBlock();

                    lTextBlock.FontSize = 18;

                    var lHyperlink = new Hyperlink();

                    lHyperlink.Inlines.Add(@"rtsp://" + item.ToString() + ":" + Properties.Settings.Default.OffScreenStreamerPort.ToString());

                    lHyperlink.Click += delegate (object sender, RoutedEventArgs e)
                    {
                        var lHyperlink1 = sender as Hyperlink;

                        if (lHyperlink1 == null)
                            return;

                        var lRun = lHyperlink1.Inlines.FirstInline as Run;

                        if (lRun == null)
                            return;

                        Clipboard.SetText(lRun.Text);

                        TextBlock linfoTextBlock = new TextBlock();

                        Popup mPopupCopy = new Popup();

                        linfoTextBlock.Padding = new Thickness(7);

                        linfoTextBlock.Foreground = Brushes.Black;

                        linfoTextBlock.Background = Brushes.LightBlue;

                        linfoTextBlock.FontSize = 20;

                        linfoTextBlock.Text = "Copying of " + lRun.Text + " to clipboard.";

                        mPopupCopy.Child = linfoTextBlock;

                        mPopupCopy.PlacementTarget = lTextBlock;

                        mPopupCopy.PopupAnimation = PopupAnimation.Slide;

                        mPopupCopy.IsOpen = true;

                        DoubleAnimation fadeInAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(1000)));

                        fadeInAnimation.Completed += delegate (object sender1, EventArgs e1)
                        {
                            mPopupCopy.IsOpen = false;
                        };
                        fadeInAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
                        mPopupCopy.Opacity = 0.0;
                        mPopupCopy.BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)fadeInAnimation.GetAsFrozen());
                    };

                    lTextBlock.Inlines.Add(lHyperlink);

                    lTextBlock.Margin = new Thickness(10, 0, 0, 0);

                    mIPList.Items.Add(lTextBlock);
                }
            }

            mIPList.Visibility = VisibilityStateIP;
        }

        void Instance_m_ChangeStatusEvent(Emul.StatusEnum a_Status)
        {
            switch (a_Status)
            {
                case Emul.StatusEnum.NoneInitilized:
                case Emul.StatusEnum.Initilized:
                case Emul.StatusEnum.Stopped:
                case Emul.StatusEnum.Paused:
                case Emul.StatusEnum.Started:
                    for (int i = 0; i < m_Panels.Items.Count; i++)
                    {
                        Expander l_ItemExpander = m_Panels.Items.GetItemAt(i) as Expander;
                        
                        l_ItemExpander.IsExpanded = false;
                    }
                    break;
            }           

        }

        void Instance_SwitchControlModeEvent(bool obj)
        {
            var l_ParentElement = this.Parent as FrameworkElement;

            if (l_ParentElement == null)
                return;

            var l_TouchDragBtnWidth = (double)App.Current.Resources["TouchDragBtnWidth"];

            //if(obj)
            //{
            //    var l_PanelWidth = (double)App.Current.Resources["PanelWidth"];

            //    this.Width = l_PanelWidth;

            //    Canvas.SetLeft(l_ParentElement, -l_PanelWidth);
            //}
            //else
            {
                Binding l_Binding = new Binding("ActualWidth");

                l_Binding.Source = App.Current.MainWindow;

                l_Binding.Mode = BindingMode.OneWay;

                l_Binding.Converter = new Omega_Red.Tools.Converters.WidthConverter() { Offset = -16 - l_TouchDragBtnWidth, Scale = 1.0 };

                this.SetBinding(UserControl.WidthProperty, l_Binding);
            }
        }
       
        protected override Size MeasureOverride(Size constraint)
        {
            resize();

            return base.MeasureOverride(constraint);
        }

        private void resize()
        {
            double l_expendedheight = 0.0;

            Expander l_extendedExpander = null;

            var l_actualHeight = m_Panels.ActualHeight;

            for (int i = 0; i < m_Panels.Items.Count; i++)
            {
                Expander l_ItemExpander = m_Panels.Items.GetItemAt(i) as Expander;

                if (l_ItemExpander == null)
                    continue;

                if (!l_ItemExpander.IsExpanded)
                {
                    double l_VertHeight = 0.0;

                    if (l_ItemExpander.Margin != null)
                    {
                        l_VertHeight = l_ItemExpander.Margin.Top + l_ItemExpander.Margin.Bottom;
                    }

                    if (l_ItemExpander.Padding != null)
                    {
                        l_VertHeight += l_ItemExpander.Padding.Top + l_ItemExpander.Padding.Bottom;
                    }

                    l_expendedheight += l_ItemExpander.ActualHeight + l_VertHeight;
                }
                else
                {
                    l_extendedExpander = l_ItemExpander;
                }
            }

            if (l_extendedExpander != null)
            {
                var l_control = (l_extendedExpander.Content as FrameworkElement);

                if (l_control != null)
                {
                    double l_VertHeight = 0.0;

                    if (l_extendedExpander.Margin != null)
                    {
                        l_VertHeight = l_extendedExpander.Margin.Top + l_extendedExpander.Margin.Bottom;
                    }

                    if (l_extendedExpander.Padding != null)
                    {
                        l_VertHeight += l_extendedExpander.Padding.Top + l_extendedExpander.Padding.Bottom;
                    }
                    
                    var l_newHeight = l_actualHeight - l_expendedheight - l_VertHeight + 10;

                    if (l_newHeight > 0.0)
                        l_control.Height = l_newHeight;
                }
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander l_Expander = e.OriginalSource as Expander;

            if (l_Expander == null)
                return;

            var l_index = m_Panels.Items.IndexOf(l_Expander);

            for (int i = 0; i < m_Panels.Items.Count; i++)
            {
                Expander l_ItemExpander = m_Panels.Items.GetItemAt(i) as Expander;

                if (l_ItemExpander == null || l_index == i)
                    continue;

                l_ItemExpander.IsExpanded = false;
            }

            this.UpdateLayout();
        }

        public Visibility VisibilityState
        {
            get { return Visibility.Visible; }
        }
        public Visibility VisibilityStateIP
        {
            get { return Visibility.Collapsed; }
        }

        private static IPAddress[] GetIPAddresses()
        {
            String strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);  
            return ipEntry.AddressList;
        }
    }
}
