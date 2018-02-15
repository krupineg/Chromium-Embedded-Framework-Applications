using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Input;
using CefSharp;
using CefSharp.OffScreen;
using CommonsLib;

namespace Cef
{
    public class MainViewModel : ViewModel
    {
        private string _address;

        public string Address
        {
            get { return _address; }
            set { SetProperty(value, ref _address); }
        }

        public ICommand TakeScreenshotCommand { get; private set; }

        public MainViewModel(string startingAddress)
        {
            Address = startingAddress;
            TakeScreenshotCommand = new DelegateCommand(TakeScreenshotOffscreen);
        }

        private void TakeScreenshotOffscreen()
        {
            var offscreenBrowser = new CefSharp.OffScreen.ChromiumWebBrowser();
            offscreenBrowser.BrowserInitialized += (o, args) =>
            {
                offscreenBrowser.Load(Address);
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