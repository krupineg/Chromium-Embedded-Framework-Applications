using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using CefSharp;
using CommonsLib;

namespace Cef
{
    public class ToolsViewModel : ViewModel
    {
        private readonly BrowserTabSelectorViewModel _tabSelectorViewModel;

        public ILogger Logger { get; private set; }

        public ICommand TakeScreenshotCommand { get; private set; }
        
        public ICommand ToggleTypeCommand { get; private set; }

        public ICommand ShowDevToolsCommand { get; private set; }

        public ReadOnlyCollection<string> LogTypes { get; private set; }

        public ToolsViewModel(ILogger logger, BrowserTabSelectorViewModel tabSelectorViewModel)
        {
            _tabSelectorViewModel = tabSelectorViewModel;
            Logger = logger;
            TakeScreenshotCommand = new DelegateCommand(TakeScreenshotExecute);
            LogTypes = new ReadOnlyCollection<string>(
                new string[3]
                {
                    LogEventTypes.Focus, LogEventTypes.MouseOver, LogEventTypes.Mutation
                });
            ToggleTypeCommand = new DelegateCommand<object>((o) => Logger.ToggleType(o.ToString()));
            ShowDevToolsCommand = new DelegateCommand(() => tabSelectorViewModel.SelectedTab.Browser.ShowDevTools());
        }

        private void TakeScreenshotExecute()
        {
            var utility = new ScreenshotUtility();
            utility.TakeScreenshotOffscreen(_tabSelectorViewModel.SelectedTab.AddressBarViewModel.Address, "c:\\temp\\amazing_screenshot.png");
        }
    }
}