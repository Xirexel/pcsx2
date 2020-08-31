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

using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Omega_Red.ViewModels
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

    public abstract class BaseViewModel : ItemsPanelTemplateManager
    {
        public BaseViewModel()
        { }
        
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
            get { if (m_InnerDataTemplateSelector == null) 
                m_InnerDataTemplateSelector = createDataTemplateSelector(this.GetType()); 
                return m_InnerDataTemplateSelector; }
        }
                
        abstract protected Omega_Red.Managers.IManager Manager { get; }
        
        public ICommand AddCommand
        {
            get { return new DelegateCommand(Manager.createItem); }
        }

        public ICommand RemoveCommand
        {
            get { return new DelegateCommand<object>((object a_Item) => {
                
                var l_ContextMenu = getItemTemplate(Application.Current.Resources, "ConfirmMenu") as ContextMenu;

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
            
            },()=> {
                return true;
            }); }
        }

        public ICommand SyncCommand
        {
            get
            {
                return new DelegateCommand<object>((object a_Item) => {

                    var l_ContextMenu = getItemTemplate(Application.Current.Resources, "SyncMenu") as ContextMenu;

                    dynamic l_CommandObject = new System.Dynamic.ExpandoObject();

                    l_CommandObject.PersistCommand = new DelegateCommand(() =>
                    {
                        l_ContextMenu.IsOpen = false;
                        Manager.persistItemAsync(a_Item);
                    },()=> {
                        return Manager.accessPersistItem(a_Item);
                    });

                    l_CommandObject.LoadCommand = new DelegateCommand(() =>
                    {
                        l_ContextMenu.IsOpen = false;
                        Manager.loadItemAsync(a_Item);
                    }, () => {
                        return Manager.accessLoadItem(a_Item);
                    });

                    l_ContextMenu.DataContext = l_CommandObject;

                    l_ContextMenu.IsOpen = true;

                }, () => {
                    return true;
                }, (a_item) => {
                    Manager.registerItem(a_item);
                });
            }
        }

        public ICollectionView Collection
        {
            get
            {
                if(Manager != null)
                    return Manager.Collection;
                else
                    return null;
            }
        }
    }
}
