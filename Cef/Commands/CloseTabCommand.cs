using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using CefSharp;

namespace Cef
{
    public class CloseTabCommand : ICommand, IDisposable
    {
        private readonly BrowserTabSelectorViewModel _tabSelector;
        private readonly BrowserTabViewModel _currentPage;
        public event EventHandler CanExecuteChanged;

        public CloseTabCommand(BrowserTabSelectorViewModel tabSelector, BrowserTabViewModel currentPage)
        {
            _tabSelector = tabSelector;
            _currentPage = currentPage;
            _tabSelector.Tabs.CollectionChanged += TabsOnCollectionChanged;
        }

        public bool CanExecute(object parameter)
        {
            return _tabSelector.Tabs.Count > 0;
        }

        public void Execute(object parameter)
        {
            var currentTab = _currentPage;
            if (_tabSelector.SelectedTab == currentTab)
            {
                var index = _tabSelector.Tabs.IndexOf(currentTab);
                if (index < _tabSelector.Tabs.Count - 2)
                {
                    _tabSelector.SelectedTab = _tabSelector.Tabs[index + 1];
                }
                else
                {
                    _tabSelector.SelectedTab = _tabSelector.Tabs[index - 1];
                }
            }
            currentTab.Browser.CloseDevTools();
            currentTab.Dispose();
            _tabSelector.Tabs.Remove(currentTab);
        }
        
        public void Dispose()
        {
            _tabSelector.Tabs.CollectionChanged -= TabsOnCollectionChanged;
        }

        private void TabsOnCollectionChanged(object o, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}