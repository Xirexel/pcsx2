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
using System.Windows.Input;

namespace Omega_Red.ViewModels
{

    internal delegate bool CheckStateDelegate();

    class DelegateCommand: ICommand
    {
        private readonly Action _action;

        private CheckStateDelegate m_CheckStateDelegate;

        public DelegateCommand(Action action = null, CheckStateDelegate a_CheckStateDelegate = null)
        {
            _action = action;

            m_CheckStateDelegate = a_CheckStateDelegate;
        }
        
        public DelegateCommand(Action confirm, bool isAllowedConfirm)
        {
        }

        public void Execute(object parameter)
        {
            if (_action != null)
                _action();
        }

        public bool CanExecute(object parameter)
        {
            if (m_CheckStateDelegate == null)
                return true;

            return m_CheckStateDelegate();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    class DelegateCommand<T> : ICommand where T: class
    {
        private readonly Action<T> _action;

        private Action<object> m_callback_action;

        private CheckStateDelegate m_CheckStateDelegate;
        private Action<object, bool> startStop;
        private Func<bool> p;

        public DelegateCommand(Action<T> action, CheckStateDelegate a_CheckStateDelegate = null, Action<object> a_callback_action = null)
        {
            _action = action;

            m_CheckStateDelegate = a_CheckStateDelegate;

            m_callback_action = a_callback_action;
        }
        
        public void Execute(object parameter)
        {
            _action(parameter as T);
        }

        public bool CanExecute(object parameter)
        {
            if (m_callback_action != null && parameter != null)
                m_callback_action(parameter);

            if (m_CheckStateDelegate == null)
                return true;

            return m_CheckStateDelegate();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
