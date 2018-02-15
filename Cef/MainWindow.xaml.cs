using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CefSharp;
using CefSharp.Event;
using CefSharp.Handler;
using CefSharp.OffScreen;
using CommonsLib;
using Path = System.Windows.Shapes.Path;

namespace Cef
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            var webPageObserver = new WebPageObserver(Dispatcher, ()=> Browser.IsLoading, (script) =>
            {
                var task = Browser.GetMainFrame().EvaluateScriptAsync(script);
                return task.ContinueWith(task1 => new ScriptRunResult()
                {
                    Success = task1.Result.Success,
                    Result = task1.Result.Result,
                    Message = task1.Result.Message
                });
            });
            Browser.RenderProcessMessageHandler = new RenderProcessMessageHandler(webPageObserver);
            Browser.RegisterJsObject(JavascriptNames.___Web_Observer,
                webPageObserver,
                BindingOptions.DefaultBinder);
            
            Browser.TitleChanged += BrowserOnTitleChanged;
            Browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
            Browser.ConsoleMessage += BrowserOnConsoleMessage;
            Browser.AddressChanged += (sender, s) =>
            {
                TakeScreenshotOffscreen(s);
            };
        }
        
        private void BrowserOnTitleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
        }

        private void BrowserOnConsoleMessage(object sender, ConsoleMessageEventArgs consoleMessageEventArgs)
        {
            Console.WriteLine(consoleMessageEventArgs.Message);
        }

        private void BrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
        {
            if (frameLoadEndEventArgs.Frame.IsMain)
            {
                new ObserverAttachingBootstrapper().Do(s => Browser.EvaluateScriptAsync(s).ConfigureAwait(false));
            }
        }

        private void TakeScreenshotOffscreen(string url)
        {
            var offscreenBrowser = new CefSharp.OffScreen.ChromiumWebBrowser();
            offscreenBrowser.BrowserInitialized += (o, args) =>
            {
                offscreenBrowser.Load(url);
                offscreenBrowser.FrameLoadEnd += async (sender1, eventArgs) =>
                {
                    if (eventArgs.Frame.IsMain)
                    {
                        var sizes = await offscreenBrowser.EvaluateScriptAsync(JavascriptQueries.RequestForMaxSize);
                        var widthAndHeight = (List<object>)sizes.Result;

                        var screenshotSize = new System.Drawing.Size((int)widthAndHeight[0], (int)widthAndHeight[1]);
                        offscreenBrowser.Size = screenshotSize;

                        await Task.Delay(500);
                        var bitmap = await offscreenBrowser.ScreenshotAsync(false, PopupBlending.Main);
                        var filename = "c:\\temp\\offscreen_screenshot.png";
                        bitmap.Save(filename, ImageFormat.Png);
                        Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = filename
                        });
                    }
                };

            };
        }
    }
}
