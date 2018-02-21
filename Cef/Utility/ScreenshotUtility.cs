using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using CommonsLib;

namespace Cef
{
    public class ScreenshotUtility
    {
        public void TakeScreenshotOffscreen(string url, string filename)
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
                        bitmap.Save(filename, ImageFormat.Png);
                        offscreenBrowser.Dispose();
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