using Golden_Phi.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Golden_Phi.ViewModels
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DataTemplateNameAttribute : Attribute
    {
        public DataTemplateNameAttribute(string a_Name)
        {
            Name = a_Name;
        }

        public string Name { get; private set; }
    }

    internal abstract class BaseViewModel : ItemsPanelTemplateManager
    {
        public BaseViewModel()
        {
            ItemsPanelTemplateSelector = SelectTemplate("TileItemsPanel");
        }

        private ItemDataTemplateSelector m_InnerDataTemplateSelector = null;

        private static ItemDataTemplateSelector createDataTemplateSelector(Type a_Type)
        {
            string l_Name = "";

            DataTemplateNameAttribute l_DataTemplateNameAttribute;

            l_DataTemplateNameAttribute = (a_Type.GetCustomAttribute(typeof(DataTemplateNameAttribute)) as DataTemplateNameAttribute);

            if (l_DataTemplateNameAttribute != null)
            {
                l_Name = l_DataTemplateNameAttribute.Name;
            }

            return new ItemDataTemplateSelector(l_Name);
        }

        public System.Windows.Controls.DataTemplateSelector DataTemplateSelector
        {
            get
            {
                if (m_InnerDataTemplateSelector == null)
                    m_InnerDataTemplateSelector = createDataTemplateSelector(this.GetType());
                return m_InnerDataTemplateSelector;
            }
        }

        abstract protected Managers.IManager Manager { get; }

        public ICollectionView Collection
        {
            get
            {
                if (Manager != null)
                    return Manager.Collection;
                else
                    return null;
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new Tools.DelegateCommand(Manager.createItem);
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand<object>((a_Item) => {

                    if(Manager.IsConfirmed)
                    {

                        var l_ContextMenu = App.getResource("ConfirmMenu") as ContextMenu;

                        dynamic l_CommandObject = new System.Dynamic.ExpandoObject();

                        l_CommandObject.ConfirmCommand = new DelegateCommand(() =>
                        {
                            l_ContextMenu.IsOpen = false;
                            Manager.removeItem(a_Item);
                        });

                        l_CommandObject.CancelCommand = new DelegateCommand(() =>
                        {
                            l_ContextMenu.IsOpen = false;
                        });

                        l_ContextMenu.DataContext = l_CommandObject;

                        l_ContextMenu.IsOpen = true;
                    }
                    else
                        Manager.removeItem(a_Item);
                                                         
                }, () => {
                    return true;
                });
            }
        }

        public object View
        {
            get
            {
                if (Manager != null)
                    return Manager.View;
                else
                    return null;
            }
        }
    }
}
