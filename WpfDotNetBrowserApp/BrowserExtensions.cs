using System.Threading;
using DotNetBrowser;
using DotNetBrowser.Events;

namespace WpfDotNetBrowserApp
{
    public static class BrowserExtensions
    {
        public static void LoadUrlAndWait(this Browser browser, string url)
        {
            ManualResetEvent waitEvent = new ManualResetEvent(false);
            FinishLoadingFrameHandler callback = delegate (object sender, FinishLoadingEventArgs e)
            {
                // Wait until main document of the web page is loaded completely.
                if (e.IsMainFrame)
                {
                    waitEvent.Set();
                }
            };
            try
            {
                browser.FinishLoadingFrameEvent += callback;
                browser.LoadURL(url);
                waitEvent.WaitOne();
            }
            finally
            {
                browser.FinishLoadingFrameEvent -= callback;
            }
        }
    }
}