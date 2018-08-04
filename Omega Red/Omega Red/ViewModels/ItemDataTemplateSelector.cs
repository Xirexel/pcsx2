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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Omega_Red.ViewModels
{
    internal class ItemDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        private string m_itemDataTemplateResourceKey = "";

        private DataTemplate m_DataTemplate = null;

        public ItemDataTemplateSelector(string a_itemDataTemplateResourceKey)
        {
            m_itemDataTemplateResourceKey = a_itemDataTemplateResourceKey;
        }

        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            if(m_DataTemplate == null)
                m_DataTemplate = getItemTemplate(Application.Current.Resources, m_itemDataTemplateResourceKey) as DataTemplate;
            
            return m_DataTemplate;
        }

        private object getItemTemplate(ResourceDictionary a_resource, string a_key)
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
    }
}
