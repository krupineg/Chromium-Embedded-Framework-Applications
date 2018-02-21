using System;
using CommonsLib;

namespace Cef
{
    internal class DisabledCommand : DelegateCommand
    {
        public DisabledCommand() : base(null, () => false)
        {
        }

        public DisabledCommand(Func<bool> canExecute) : base(null, () => false)
        {
        }

        public override void Execute(object parameter)
        {
            throw new InvalidOperationException("Execute should not be called on DisabledCommand");
        }
    }
}