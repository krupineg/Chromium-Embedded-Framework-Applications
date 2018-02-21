using System;
using CommonsLib;

namespace Cef
{
    internal class DisabledCommand : DelegateCommand
    {
        public override void Execute(object parameter)
        {
            throw new InvalidOperationException("Execute should not be called on DisabledCommand");
        }

        public DisabledCommand() : base((Action)null, () => false)
        {
        }

        public DisabledCommand(Action command, Func<bool> canExecute) : base(command, canExecute)
        {
        }

        public DisabledCommand(Action<object> command) : base(command)
        {
        }

        public DisabledCommand(Action<object> command, Func<bool> canExecute) : base(command, canExecute)
        {
        }
    }
}