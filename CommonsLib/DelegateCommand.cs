using System;
using System.Windows.Input;

namespace CommonsLib
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _command;

        public DelegateCommand(Action command)
        {
            _command = command;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _command.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}