using System;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;

namespace Cef
{
    public static class BrowserExtension
    {
        public static void WaitForBrowserInitialization(this ChromiumWebBrowser webBrowser, int timeout)
        {
            using (var manualResetEvent = new ManualResetEvent(false))
            {
                EventHandler initialized = delegate (object sender, EventArgs args) { manualResetEvent.Set(); };
                try
                {
                    webBrowser.BrowserInitialized += initialized;
                    if (!manualResetEvent.WaitOne(timeout))
                    {
                        throw new TimeoutException();
                    }
                }
                finally
                {
                    webBrowser.BrowserInitialized -= initialized;
                }
            }
        }

        public static void WaitForBrowserMainFrame(this ChromiumWebBrowser webBrowser, int timeout)
        {
            using (var manualResetEvent = new ManualResetEvent(false))
            {
                EventHandler<FrameLoadEndEventArgs> frameLoadEnd = delegate(object sender, FrameLoadEndEventArgs args)
                {
                    if (args.Frame.IsMain)
                    {
                        manualResetEvent.Set();
                    }
                };

                try
                {
                    webBrowser.FrameLoadEnd += frameLoadEnd;
                    if (!manualResetEvent.WaitOne(timeout))
                    {
                        throw new TimeoutException();
                    }
                }
                finally
                {
                    webBrowser.FrameLoadEnd -= frameLoadEnd;
                }
            }
        }
    }
}