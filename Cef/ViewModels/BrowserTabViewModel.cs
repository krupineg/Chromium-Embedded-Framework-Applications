using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Event;
using CefSharp.Internals;
using CommonsLib;

namespace Cef
{
    public class BrowserTabViewModel : ViewModel
    {
        private readonly ILogger _logger;
        private readonly Dispatcher _dispatcher;
        private IWebBrowser _browser;
        
        public IWebBrowser Browser
        {
            get { return _browser; }
            set
            {
                SetProperty(ref _browser, value);
                if (_browser != null)
                {
                    BrowserIsAttached(_browser);
                }
            }
        }

        public Guid Id { get; private set; }

        public IWebPageObserver WebPageObserver { get; private set; }
        
        public TabHeaderViewModel TabHeaderViewModel { get; private set; }

        public NavigationElementViewModel NavigationItemViewModel { get; private set; }

        public AddressBarViewModel AddressBarViewModel { get; private set; }

        public IScriptRunner ScriptRunner { get; private set; }

        public BrowserTabViewModel(string startingAddress, ILogger logger, CloseTabCommandFactory closeTabCommandFactory, Dispatcher dispatcher)
        {
            Id = Guid.NewGuid();
            _logger = logger;
            _dispatcher = dispatcher;
            TabHeaderViewModel = new TabHeaderViewModel(closeTabCommandFactory.Create(this), Id);
            NavigationItemViewModel = new NavigationElementViewModel(dispatcher);
            AddressBarViewModel = new AddressBarViewModel(startingAddress, _logger);
            _logger.Info("open page tab " + Id + " with address: " + startingAddress, LogEventTypes.Common);
        }

        private void BrowserIsAttached(IWebBrowser browser)
        {
            NavigationItemViewModel.SetBrowser(Browser);
            TabHeaderViewModel.SetBrowser(Browser);
            ScriptRunner = new CefScriptRunner(browser);

            browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
            browser.ConsoleMessage += BrowserOnConsoleMessage;
            browser.LoadError += BrowserLoadError;
            browser.DisplayHandler = new DisplayHandler((favIcon) =>
            {
                _dispatcher.Invoke(() =>
                {
                    TabHeaderViewModel.FavIcon = favIcon;
                });
            });
            LazyRegisterWebObserver();

            WebPageObserver = new WebPageObserver(ScriptRunner, Id);

            WebPageObserver.FocusChanged += WebPageObserverOnFocusChanged;
            WebPageObserver.Mutated += WebPageObserverOnMutated;
            WebPageObserver.MouseOverChanged += WebPageObserverOnMouseOverChanged;
        }

        private void BrowserLoadError(object sender, LoadErrorEventArgs e)
        {
            if (e.ErrorCode == CefErrorCode.NameNotResolved)
            {
                _logger.Info("address unresolved so look for it in google : " + AddressBarViewModel.Address, LogEventTypes.Common);
                AddressBarViewModel.Address = "https://www.google.co.th/search?q=" + AddressBarViewModel.Address;
            }
        }

        private void BrowserOnConsoleMessage(object sender, ConsoleMessageEventArgs consoleMessageEventArgs)
        {
            _logger.Info(string.Format("console message : {0}", consoleMessageEventArgs.Message), LogEventTypes.Console);
        }

        private void BrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
        {
            if (frameLoadEndEventArgs.Frame.IsMain)
            {
                _logger.Info(string.Format("frame loaded : {0}", frameLoadEndEventArgs.Frame.Url), LogEventTypes.Common);
                if (Browser != null)
                {
                    new ObserverAttachingBootstrapper().Do(s => Browser.EvaluateScriptAsync(s).ConfigureAwait(false));
                }
            }
        }

        private void WebPageObserverOnFocusChanged(object o, FocusChangedEventArgs focusChangedEventArgs)
        {
            LogDataDictionary(LogEventTypes.Focus, focusChangedEventArgs.FocusedElementData);
        }

        private void WebPageObserverOnMutated(object o, MutationEventArgs mutationEventArgs)
        {
            _logger.Info("page is mutated", LogEventTypes.Mutation);
        }

        private void WebPageObserverOnMouseOverChanged(object o, MouseOverChangedEventArgs mouseOverChangedEventArgs)
        {
            LogDataDictionary(LogEventTypes.MouseOver, mouseOverChangedEventArgs.ChangedData);
        }

        private void LogDataDictionary(string whatChanged, IDictionary<string, object> dataDictionary)
        {
            var sb = new StringBuilder(whatChanged);
            foreach (var data in dataDictionary)
            {
                sb.Append(Environment.NewLine).Append(data.Key).Append(" => ").Append(data.Value);
            }
            _logger.Info(sb.ToString(), whatChanged);
        }

        void LazyRegisterWebObserver()
        {
            Browser.JavascriptObjectRepository.ResolveObject += JavascriptObjectRepositoryOnResolveObject;
        }

        private void JavascriptObjectRepositoryOnResolveObject(object sender, JavascriptBindingEventArgs javascriptBindingEventArgs)
        {
            if (javascriptBindingEventArgs.ObjectName == JavascriptNames.___Web_Observer)
            {
                _logger.Info(string.Format("bound object is being resolved : {0}", JavascriptNames.___Web_Observer), LogEventTypes.Common);
                javascriptBindingEventArgs.ObjectRepository.Register(JavascriptNames.___Web_Observer, WebPageObserver, true, BindingOptions.DefaultBinder);
            }
        }

        protected override void DisposeInternal()
        {
            _logger.Info("disposing page tab " + Id, LogEventTypes.Common);
            if (Browser != null)
            {
                Browser.FrameLoadEnd -= BrowserOnFrameLoadEnd;
                Browser.ConsoleMessage -= BrowserOnConsoleMessage;
                Browser.JavascriptObjectRepository.ResolveObject -= JavascriptObjectRepositoryOnResolveObject;
                Browser.Dispose();
            }
            _logger.Info("page tab is disposed " + Id, LogEventTypes.Common);
            WebPageObserver.FocusChanged -= WebPageObserverOnFocusChanged;
            WebPageObserver.MouseOverChanged -= WebPageObserverOnMouseOverChanged;
            WebPageObserver.Mutated -= WebPageObserverOnMutated;
        }
    }
}