using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonsLib;
using DotNetBrowser;
using DotNetBrowser.Events;
using DotNetBrowser.WPF;

namespace WpfDotNetBrowserApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WPFBrowserView _browser;

        public MainWindow()
        {
            InitializeComponent();
            var browser = BrowserFactory.Create(BrowserType.LIGHTWEIGHT);
            _browser = new WPFBrowserView(browser);
            Content = _browser;
            _browser.URL = "github.com";
            _browser.FinishLoadingFrameEvent += BrowserOnFinishLoadingFrameEvent;
            _browser.ConsoleMessageEvent += BrowserOnConsoleMessageEvent;
            Unloaded+= OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _browser.FinishLoadingFrameEvent -= BrowserOnFinishLoadingFrameEvent;
            _browser.ConsoleMessageEvent -= BrowserOnConsoleMessageEvent;
            Unloaded -= OnUnloaded;
            _browser.Dispose();
        }

        private void BrowserOnFinishLoadingFrameEvent(object sender, FinishLoadingEventArgs finishLoadingEventArgs)
        {
            if (finishLoadingEventArgs.IsMainFrame)
            {
                TakeScreenshot("c:\\temp\\amazing_screenshot.png");
                JSValue value = _browser.Browser.ExecuteJavaScriptAndReturnValue("window");
                value.AsObject().SetProperty(JavascriptNames.___Web_Observer, new WebPageObserver(Dispatcher, () => !_browser.IsLoaded, s => Task.Run(() =>
                {
                    var val = _browser.Browser.ExecuteJavaScriptAndReturnValue(s);
                    return new ScriptRunResult()
                    {
                        Success = val != null,
                        Result = val.ToString(),
                        Message = val.ToString()
                    };
                })));

                new ObserverAttachingBootstrapper().Do(s => _browser.Browser.ExecuteJavaScript(s));
            }
        }

        private void TakeScreenshot(string s)
        {
            var size = _browser.Browser.ExecuteJavaScriptAndReturnValue(JavascriptQueries.RequestForMaxSize).AsArray();
            var width = int.Parse(size[0].ToString());
            var height = int.Parse(size[1].ToString());

            var screenshot = _browser.Browser.ImageProvider.GetImage(width, height);
            screenshot.Save(s, ImageFormat.Png);
        }

        private void BrowserOnConsoleMessageEvent(object sender, ConsoleEventArgs consoleEventArgs)
        {
            Console.WriteLine(consoleEventArgs.Message);
        }
    }
}
