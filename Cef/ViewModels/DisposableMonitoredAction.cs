using System;
using System.Threading;

namespace Cef
{
    internal class DisposableMonitoredAction : IDisposable
    {
        private readonly object _monitor;
        private readonly bool _entered;

        public DisposableMonitoredAction(object monitor, out bool entered)
        {
            _monitor = monitor;
            _entered = (!Monitor.IsEntered(_monitor) && Monitor.TryEnter(_monitor));
            entered = _entered;
        }

        public void Dispose()
        {
            if (_entered)
            {
                Monitor.Exit(_monitor);
            }
        }
    }
}