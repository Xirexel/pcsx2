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

using Omega_Red.Managers;
using Omega_Red.Models;
using Omega_Red.Properties;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Omega_Red.ViewModels
{

    [DataTemplateNameAttribute("BiosInfoItem")]
    public class BiosInfoViewModel : BaseViewModel
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        protected override Managers.IManager Manager
        {
            get { return BiosManager.Instance; }
        }

        public BiosInfoViewModel()
        {
            PCSX2Controller.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;

            if(Collection != null)
                Collection.CollectionChanged += Collection_CollectionChanged;
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Collection.IsEmpty)
                VisibilityTipInfo = Visibility.Visible;
            else
                VisibilityTipInfo = Visibility.Collapsed;
        }

        private bool m_IsEnabled = true;
                
        void Instance_m_ChangeStatusEvent(PCSX2Controller.StatusEnum a_Status)
        {
            IsEnabled = a_Status == PCSX2Controller.StatusEnum.Stopped 
                || a_Status == PCSX2Controller.StatusEnum.Initilized
                || a_Status == PCSX2Controller.StatusEnum.NoneInitilized;
        }  

        public bool IsEnabled
        {
            get { return m_IsEnabled; }
            set
            {
                m_IsEnabled = value;

                RaisePropertyChangedEvent("IsEnabled");
            }
        }

        private Visibility mVisibilityTipInfo = Visibility.Collapsed;

        public Visibility VisibilityTipInfo
        {
            get { return mVisibilityTipInfo; }
            set
            {
                mVisibilityTipInfo = value;

                RaisePropertyChangedEvent("VisibilityTipInfo");
            }
        }

        public object TipInfo
        {
            get {

                Border lBackBorder = new Border();

                TextBlock lTextBlock = new TextBlock();

                lTextBlock.Foreground = System.Windows.Media.Brushes.Black;

                lTextBlock.TextWrapping = TextWrapping.Wrap;

                lBackBorder.Child = lTextBlock;

                lBackBorder.Margin = new Thickness(5);
                
                var lHyperlink = new Hyperlink();

                lHyperlink.Inlines.Add(@"https://www.google.com.au/search?client=opera&q=ps2+bios+pack&sourceid=opera&ie=UTF-8&oe=UTF-8");

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

                var l_BiosTipTitle = App.Current.Resources["BiosTipTitle"];
                               
                lTextBlock.Inlines.Add(l_BiosTipTitle as string);

                lTextBlock.Inlines.Add(new LineBreak());

                lTextBlock.Inlines.Add(lHyperlink);

                return lBackBorder;
            }
        }
    }
}


