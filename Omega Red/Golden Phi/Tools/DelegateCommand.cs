using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Golden_Phi.Tools
{

    internal delegate bool CheckStateDelegate();

    class DelegateCommand : ICommand
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

    class DelegateCommand<T> : ICommand where T : class
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
