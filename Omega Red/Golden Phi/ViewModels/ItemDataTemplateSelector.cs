using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Golden_Phi.ViewModels
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
            if (m_DataTemplate == null)
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
