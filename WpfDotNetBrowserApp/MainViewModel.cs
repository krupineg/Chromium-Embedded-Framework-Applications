using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using CommonsLib;
using DotNetBrowser;
using DotNetBrowser.Events;

namespace WpfDotNetBrowserApp
{
    public class MainViewModel : ViewModel, IDisposable
    {
        private string _address;
        private Browser _browser1;

        public string Address
        {
            get { return _address; }
            set
            {
                SetProperty(ref _address, value);
                Browser.LoadURL(value);
            }
        }

        public Browser Browser
        {
            get { return _browser1; }
            set
            {
                SetProperty(ref _browser1, value);
            }
        }

        public ICommand TakeScreenshotCommand { get; private set; }

        public MainViewModel()
        {
            Browser = BrowserFactory.Create(BrowserType.HEAVYWEIGHT);
            TakeScreenshotCommand = new DelegateCommand(() => TakeScreenshot(Browser.URL, "c:\\temp\\amazing_screenshot.png"));
        }

        private void TakeScreenshot(string url, string s)
        {
            Task.Run(() =>
            {
                int viewWidth = 1024;
                int viewHeight = 20000;
                string[] switches =
                {
                    "--disable-gpu",
                    "--max-texture-size=" + viewHeight
                };

                BrowserPreferences.SetChromiumSwitches(switches);
                Browser browser = BrowserFactory.Create(BrowserType.LIGHTWEIGHT);

                browser.SetSize(viewWidth, viewWidth);
                ManualResetEvent waitEvent = new ManualResetEvent(false);
                browser.FinishLoadingFrameEvent += delegate (object sender, FinishLoadingEventArgs e)
                {
                    // Wait until main document of the web page is loaded completely.
                    if (e.IsMainFrame)
                    {
                        waitEvent.Set();
                    }
                };
                browser.LoadURL(url);
                waitEvent.WaitOne();

                // #3 Set the required document size.
                JSValue documentHeight = browser.ExecuteJavaScriptAndReturnValue(
                    "Math.max(document.body.scrollHeight, " +
                    "document.documentElement.scrollHeight, document.body.offsetHeight, " +
                    "document.documentElement.offsetHeight, document.body.clientHeight, " +
                    "document.documentElement.clientHeight);");
                JSValue documentWidth = browser.ExecuteJavaScriptAndReturnValue(
                    "Math.max(document.body.scrollWidth, " +
                    "document.documentElement.scrollWidth, document.body.offsetWidth, " +
                    "document.documentElement.offsetWidth, document.body.clientWidth, " +
                    "document.documentElement.clientWidth);");

                int scrollBarSize = 25;

                viewWidth = (int)documentWidth.GetNumber() + scrollBarSize;
                viewHeight = (int)documentHeight.GetNumber() + scrollBarSize;

                Debug.WriteLine("GetImage: {0} x {1}", viewWidth, viewHeight);

                var img = browser.ImageProvider.GetImage(viewWidth, viewHeight);
                img.Save(s, System.Drawing.Imaging.ImageFormat.Png);

                browser.Dispose();

                Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = s
                });
            });

        }

        private void BrowserOnFinishLoadingFrameEvent(object sender, FinishLoadingEventArgs finishLoadingEventArgs)
        {
            if (finishLoadingEventArgs.IsMainFrame)
            {
                JSValue value = Browser.ExecuteJavaScriptAndReturnValue("window");
                value.AsObject().SetProperty(JavascriptNames.___Web_Observer,
                    new WebPageObserver(new ScriptRunner(Browser), Guid.NewGuid()));

                new ObserverAttachingBootstrapper().Do(s => Browser.ExecuteJavaScript(s));
            }
        }

        internal class ScriptRunner : IScriptRunner
        {
            private readonly Browser _browser;

            public ScriptRunner(Browser browser)
            {
                _browser = browser;
            }
            public Task<ScriptRunResult> RunWithResult(string script)
            {
                return Task.Run(() =>
                {
                    var val = _browser.ExecuteJavaScriptAndReturnValue(script);
                    return new ScriptRunResult()
                    {
                        Success = val != null,
                        Result = val.ToString(),
                        Message = val.ToString()
                    };
                });
            }
        }
        private void BrowserOnConsoleMessageEvent(object sender, ConsoleEventArgs consoleEventArgs)
        {
            Console.WriteLine(consoleEventArgs.Message);
        }

        protected override void DisposeInternal()
        {
            _browser1.FinishLoadingFrameEvent -= BrowserOnFinishLoadingFrameEvent;
            _browser1.ConsoleMessageEvent -= BrowserOnConsoleMessageEvent;
            _browser1?.Dispose();
        }
    }
}