using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Event;
using CefSharp.Internals;
using CommonsLib;
using Raven.Client;
using Raven.Client.Embedded;

namespace Cef
{
    public class BrowserTabViewModel : ViewModel
    {
        private readonly ILogger _logger;
        private readonly Dispatcher _dispatcher;
        private IWebBrowser _browser;
        private IDocumentStore _embeddedDb;

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
            _embeddedDb = new EmbeddableDocumentStore()
            {
                DataDirectory = "data" + Id
            }.Initialize();
            TabHeaderViewModel = new TabHeaderViewModel(closeTabCommandFactory.Create(this), Id);
            NavigationItemViewModel = new NavigationElementViewModel(dispatcher);
            AddressBarViewModel = new AddressBarViewModel(startingAddress, _logger);
            _logger.Info("open page tab " + Id + " with address: " + startingAddress, LogEventTypes.Common);
        }

        private void BrowserIsAttached(IWebBrowser browser)
        {
            NavigationItemViewModel.SetBrowser(browser);
            TabHeaderViewModel.SetBrowser(browser);
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
            WebPageObserver.BeaconEvent += WebPageObserverOnBeaconEvent;
        }

        private async void WebPageObserverOnBeaconEvent(object sender, WebPageObserver.BeaconEventArgs beaconEventArgs)
        {
            using (var session = _embeddedDb.OpenAsyncSession())
            {
                await session.StoreAsync(beaconEventArgs);
                await session.SaveChangesAsync();
            }
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
                    new ObserverAttachingBootstrapper().Do(new JsRunner(Browser));
                }
            }
        }

        private class JsRunner : IJsRunner
        {
            private readonly IWebBrowser _browser;

            public JsRunner(IWebBrowser browser)
            {
                _browser = browser;
            }
            public async Task<dynamic> Evaluate(string s)
            {
                var result = await _browser.EvaluateScriptAsync(s).ConfigureAwait(false);
                return result;
            }
        }


        private async void WebPageObserverOnFocusChanged(object o, FocusChangedEventArgs focusChangedEventArgs)
        {
            using (var session = _embeddedDb.OpenAsyncSession())
            {
                await session.StoreAsync(focusChangedEventArgs);
                await session.SaveChangesAsync();
            }
            LogDataDictionary(LogEventTypes.Focus, focusChangedEventArgs.FocusedElementData);
        }

        private async void WebPageObserverOnMutated(object o, MutationEventArgs mutationEventArgs)
        {
            using (var session = _embeddedDb.OpenAsyncSession())
            {
                await session.StoreAsync(mutationEventArgs);
                await session.SaveChangesAsync();
            }
            _logger.Info("page is mutated", LogEventTypes.Mutation);
        }

        private async void WebPageObserverOnMouseOverChanged(object o, MouseOverChangedEventArgs mouseOverChangedEventArgs)
        {
            using (var session = _embeddedDb.OpenAsyncSession())
            {
                await session.StoreAsync(mouseOverChangedEventArgs);
                await session.SaveChangesAsync();
            }
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