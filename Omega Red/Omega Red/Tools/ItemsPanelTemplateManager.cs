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
using Omega_Red.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Omega_Red.Tools
{
    public class ItemsPanelTemplateManager : ObservableObject
    {
        public ItemsPanelTemplateManager()
        {
            ConfigManager.Instance.SwitchControlModeEvent += Instance_SwitchControlModeEvent;
        }

        protected void Instance_SwitchControlModeEvent(bool obj)
        {
            //if (obj)
            //{
            //    ItemsPanelTemplateSelector = SelectTemplate("ButtonItemsPanel");
            //}
            //else
            {
                ItemsPanelTemplateSelector = SelectTemplate("TouchItemsPanel");
            }
        }

        private ItemsPanelTemplate
            SelectTemplate(string a_ItemsPanelManagerResourceKey)
        {
            var l_ItemsPanelTemplate = getItemTemplate(Application.Current.Resources, a_ItemsPanelManagerResourceKey) as ItemsPanelTemplate;

            return l_ItemsPanelTemplate;
        }

        protected object getItemTemplate(ResourceDictionary a_resource, string a_key)
        {
            var l_itemTemplate = a_resource[a_key];

            if (l_itemTemplate != null)
                return l_itemTemplate;

            foreach (var l_Dictionary in a_resource.MergedDictionaries)
            {
                l_itemTemplate = getItemTemplate(l_Dictionary, a_key);

                if (l_itemTemplate != null)
                    break;
            }

            return l_itemTemplate;
        }

        public object ItemsPanelTemplateSelector
        {
            get { return (object)GetValue(ItemsPanelTemplateSelectorProperty); }
            set { SetValue(ItemsPanelTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPanelTemplateSelectorProperty =
            DependencyProperty.Register("ItemsPanelTemplateSelector", typeof(object), typeof(ItemsPanelTemplateManager), new PropertyMetadata(null));

    }
}
