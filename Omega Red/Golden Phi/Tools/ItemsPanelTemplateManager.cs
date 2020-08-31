using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Golden_Phi.Tools
{
    public class ItemsPanelTemplateManager : ObservableObject
    {
        public ItemsPanelTemplateManager()
        {
        }
        
        public ItemsPanelTemplate
            SelectTemplate(string a_ItemsPanelManagerResourceKey)
        {
            var l_ItemsPanelTemplate = App.getResource(a_ItemsPanelManagerResourceKey) as ItemsPanelTemplate;

            return l_ItemsPanelTemplate;
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
