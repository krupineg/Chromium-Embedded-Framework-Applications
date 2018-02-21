using System;
using System.Windows.Input;

namespace CommonsLib
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _actionWithoutParameter;
        private readonly Action<object> _actionWithParameter;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;
        
        public DelegateCommand(Action<object> command)
        {
            _actionWithParameter = command;
            _canExecute = () => true;
        }

        public DelegateCommand(Action command)
        {
            _actionWithoutParameter = command;
            _canExecute = () => true;
        }

        public DelegateCommand(Action<object> command, Func<bool> canExecute)
        {
            _actionWithParameter = command;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action command, Func<bool> canExecute)
        {
            _actionWithoutParameter = command;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public virtual void Execute(object parameter)
        {
            if (_actionWithoutParameter != null)
            {
                _actionWithoutParameter.Invoke();
            }
            else
            {
                _actionWithParameter.Invoke(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}