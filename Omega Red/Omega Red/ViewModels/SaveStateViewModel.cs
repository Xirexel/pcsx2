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
using Omega_Red.Tools.Savestate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Omega_Red.ViewModels
{
    [DataTemplateNameAttribute("StateInfoItem")]
    class SaveStateInfoViewModel : BaseViewModel
    {
        public SaveStateInfoViewModel()
        {
            PCSX2Controller.Instance.ChangeStatusEvent += Instance_m_ChangeStatusEvent;
        }
               
        private bool m_IsEnabled = false;

        private PCSX2Controller.StatusEnum m_Status = PCSX2Controller.StatusEnum.NoneInitilized;
        
        void Instance_m_ChangeStatusEvent(PCSX2Controller.StatusEnum a_Status)
        {
            m_Status = a_Status;

            IsEnabled = a_Status != PCSX2Controller.StatusEnum.NoneInitilized;
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
        
        public ICommand AddCommand
        {
            get
            {
                return new DelegateCommand(SaveStateManager.Instance.addSaveStateInfo,
                    () =>
                    {
                        return m_Status == PCSX2Controller.StatusEnum.Started ||
                            m_Status == PCSX2Controller.StatusEnum.Paused;
                    });
            }
        }

        public ICommand LoadCommand
        {
            get { return new DelegateCommand<SaveStateInfo>(PCSX2Controller.Instance.loadState); }
        }

        public ICommand SaveCommand
        {
            get { return new DelegateCommand<SaveStateInfo>(PCSX2Controller.Instance.saveState, 
                () => {
                return m_Status == PCSX2Controller.StatusEnum.Started ||
                    m_Status == PCSX2Controller.StatusEnum.Paused;
            }); }
        }
                
        protected override IManager Manager
        {
            get { return SaveStateManager.Instance; }
        }
    }
}
