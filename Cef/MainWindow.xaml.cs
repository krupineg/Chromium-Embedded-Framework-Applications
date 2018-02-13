using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using CefSharp;
using CefSharp.Event;
using CefSharp.Handler;
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
                return task.ContinueWith(task1 => new ScriptRunResult() { Success = task1.Result.Success, Result = task1.Result.Result, Message = task1.Result.Message });
            }, Browser.TakeScreenshot);
            Browser.RenderProcessMessageHandler = new RenderProcessMessageHandler(webPageObserver);
            Browser.RegisterJsObject(JavascriptNames.___Web_Observer,
                webPageObserver,
                BindingOptions.DefaultBinder);
            
            Browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
            Browser.ConsoleMessage += BrowserOnConsoleMessage;
        }
        
        private void BrowserOnConsoleMessage(object sender, ConsoleMessageEventArgs consoleMessageEventArgs)
        {
            Console.WriteLine(consoleMessageEventArgs.Message);
        }

        private async void BrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
        {
            if (frameLoadEndEventArgs.Frame.IsMain)
            {
                new ObserverAttachingBootstrapper().Do(s => Browser.EvaluateScriptAsync(s, TimeSpan.FromSeconds(5)).ConfigureAwait(false), s => Browser.TakeScreenshot(s), s => Browser.GetBrowserHost().PrintToPdf("c:\\temp\\pdfversion.pdf", new PdfPrintSettings() { BackgroundsEnabled = true }, new TaskPrintToPdfCallback()));
            }
        }
    }
}
