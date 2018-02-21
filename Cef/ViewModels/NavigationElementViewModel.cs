using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Threading;
using CefSharp;
using CommonsLib;
using Newtonsoft.Json;

namespace Cef
{
    public class NavigationElementViewModel : ViewModel
    {
        private readonly Dispatcher _dispatcher;
        private bool _isLoading;
        private IWebBrowser _browser;
        private DelegateCommand _backCommand;
        private DelegateCommand _forwardCommand;
        private DelegateCommand _reloadCommand;
        private DelegateCommand _stopCommand;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        
        public DelegateCommand BackCommand
        {
            get { return _backCommand; }
            private set { SetProperty(ref _backCommand, value); }
        }

        public DelegateCommand ForwardCommand
        {
            get { return _forwardCommand; }
            private set { SetProperty(ref _forwardCommand, value); }
        }

        public DelegateCommand ReloadCommand
        {
            get { return _reloadCommand; }
            private set { SetProperty(ref _reloadCommand, value); }
        }

        public DelegateCommand StopCommand
        {
            get { return _stopCommand; }
            private set { SetProperty(ref _stopCommand, value); }
        }
        
        public NavigationElementViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            BackCommand = new DisabledCommand();
            ForwardCommand = new DisabledCommand();
            ReloadCommand = new DisabledCommand();
            StopCommand = new DisabledCommand();
        }

        public void SetBrowser(IWebBrowser browser)
        {
            _browser = browser;
            _browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
            BackCommand = new DelegateCommand(browser.Back, () => browser.CanGoBack);
            ForwardCommand = new DelegateCommand(browser.Forward, () => browser.CanGoForward);
            ReloadCommand = new DelegateCommand(browser.Reload);
            StopCommand = new DelegateCommand(browser.Stop);
        }

        private void BrowserOnLoadingStateChanged(object o, LoadingStateChangedEventArgs loadingStateChangedEventArgs)
        {
            _dispatcher.InvokeAsync(() =>
            {
                BackCommand.RaiseCanExecuteChanged();
                ForwardCommand.RaiseCanExecuteChanged();
            });
        }

        protected override void DisposeInternal()
        {
            _browser.LoadingStateChanged -= BrowserOnLoadingStateChanged;
        }
    }
}