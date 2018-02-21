using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Cef.Views;
using CefSharp;
using CommonsLib;

namespace Cef
{
    public class MainViewModel : ViewModel
    {
        private readonly Dispatcher _dispatcher;
        private ToolsWindow _toolsWindow;
        public BrowserTabSelectorViewModel BrowserTabSelectorViewModel { get; private set; }

        public ToolsViewModel ObservationViewModel { get; set; }

        public ICommand OpenToolsCommand { get; private set; }

        public ICommand CloseApplicationCommand { get; private set; }

        public MainViewModel(string defaultUrl, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            var logger = new Logger(dispatcher);
            BrowserTabSelectorViewModel = new BrowserTabSelectorViewModel(defaultUrl, logger, dispatcher);
            ObservationViewModel = new ToolsViewModel(logger, BrowserTabSelectorViewModel);
            OpenToolsCommand = new DelegateCommand(OpenToolsExecute);
            CloseApplicationCommand = new DelegateCommand(CloseApplicationExecute);
        }

        private void CloseApplicationExecute()
        {
            BrowserTabSelectorViewModel.SelectedTab.Browser.CloseDevTools();
            _dispatcher.Invoke(_toolsWindow.Close);
        }
        
        private void OpenToolsExecute()
        {
            _dispatcher.InvokeAsync(() =>
            {
                _toolsWindow = new ToolsWindow() { Topmost = true};
                _toolsWindow.DataContext = ObservationViewModel;
                _toolsWindow.Show();
            });
        }
    }
}