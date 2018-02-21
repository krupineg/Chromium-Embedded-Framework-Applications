using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Cef.Views;
using CommonsLib;

namespace Cef
{
    public class MainViewModel : ViewModel
    {
        private readonly Dispatcher _dispatcher;
        public BrowserTabSelectorViewModel BrowserTabSelectorViewModel { get; private set; }

        public ToolsViewModel ObservationViewModel { get; set; }

        public ICommand OpenToolsCommand { get; private set; }
        
        public MainViewModel(string defaultUrl, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            var logger = new Logger(dispatcher);
            BrowserTabSelectorViewModel = new BrowserTabSelectorViewModel(defaultUrl, logger, dispatcher);
            ObservationViewModel = new ToolsViewModel(logger, BrowserTabSelectorViewModel);
            OpenToolsCommand = new DelegateCommand(OpenToolsExecute);
        }
        
        private void OpenToolsExecute()
        {
            _dispatcher.InvokeAsync(() =>
            {
                var window = new ToolsWindow() { Topmost = true};
                window.DataContext = ObservationViewModel;
                window.Show();
            });
        }
    }
}