using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Cef
{
    public class Logger : ILogger
    {
        private readonly Dispatcher _dispatcher;
        private IList<string> _ignoredTypes = new List<string>();

        public ObservableCollection<string> Logs { get; private set; }

        public void ToggleType(string type)
        {
            if (_ignoredTypes.Contains(type))
            {
                EnableType(type);
            }
            else
            {
                DisableType(type);
            }
        }

        public void EnableType(string type)
        {
            _ignoredTypes.Remove(type);
        }

        public void DisableType(string type)
        {
            _ignoredTypes.Add(type);
        }

        public Logger(Dispatcher dispatcher)
        {
            Logs = new ObservableCollection<string>();
            _dispatcher = dispatcher;
        }

        public void Info(string info, string type)
        {
            if (!_ignoredTypes.Contains(type))
            {
                var message = string.Format("{0} (TYPE={1}) [INFO]: {2}", DateTime.Now, type, info);
                _dispatcher.InvokeAsync(() =>
                {
                    Logs.Add(message);
                });
            }
        }
    }
}