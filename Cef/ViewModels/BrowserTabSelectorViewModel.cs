using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using CommonsLib;

namespace Cef
{
    public class BrowserTabSelectorViewModel : ViewModel
    {
        private readonly string _defaultUrl;
        private readonly ILogger _logger;
        private BrowserTabViewModel _selectedTab;
        private BrowserTabViewModel _emptyTab;
        private readonly Dispatcher _dispatcher;
        private readonly object _monitor = new object();
        private CloseTabCommandFactory _closeTabCommandFactory;

        public BrowserTabViewModel SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                // dirty hack, we should decide how to implement new tab better
                using (WithoutReentrance(_monitor, out bool entered))
                {
                    if (entered)
                    {
                        SetProperty(ref _selectedTab, value);
                        if (_selectedTab == _emptyTab)
                        {
                            _emptyTab = NewTab();
                        }
                    }
                }
            }
        }
       
        public ObservableCollection<BrowserTabViewModel> Tabs { get; set; }
        
        public BrowserTabSelectorViewModel(string defaultUrl, ILogger logger, Dispatcher dispatcher)
        {
            _closeTabCommandFactory = new CloseTabCommandFactory(this);
            _defaultUrl = defaultUrl;
            _logger = logger;
            _dispatcher = dispatcher;
            Tabs = new ObservableCollection<BrowserTabViewModel>();
            SelectedTab = NewTab();
            _emptyTab = NewTab();
            
        }

        private BrowserTabViewModel NewTab()
        {
            var newTab = new BrowserTabViewModel(_defaultUrl, _logger, _closeTabCommandFactory, _dispatcher);
            Tabs.Add(newTab);
            return newTab;
        }

        private IDisposable WithoutReentrance(object monitor, out bool entered)
        {
            return new DisposableMonitoredAction(monitor, out entered);
        }
    }
}