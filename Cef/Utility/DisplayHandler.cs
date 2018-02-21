using System;
using System.Collections.Generic;
using System.Linq;
using CefSharp;
using CefSharp.Structs;

namespace Cef
{
    internal class DisplayHandler : IDisplayHandler
    {
        private readonly Action<string> _favIconChangedCallback;

        public DisplayHandler(Action<string> favIconChangedCallback)
        {
            _favIconChangedCallback = favIconChangedCallback;
        }

        public void OnAddressChanged(IWebBrowser browserControl, AddressChangedEventArgs addressChangedArgs)
        {
        }

        public bool OnAutoResize(IWebBrowser browserControl, IBrowser browser, Size newSize)
        {
            return true;
        }

        public void OnTitleChanged(IWebBrowser browserControl, TitleChangedEventArgs titleChangedArgs)
        {
        }

        public void OnFaviconUrlChange(IWebBrowser browserControl, IBrowser browser, IList<string> urls)
        {
            _favIconChangedCallback.Invoke(urls.FirstOrDefault());
        }

        public void OnFullscreenModeChange(IWebBrowser browserControl, IBrowser browser, bool fullscreen)
        {
        }

        public bool OnTooltipChanged(IWebBrowser browserControl, ref string text)
        {
            return true;
        }

        public void OnStatusMessage(IWebBrowser browserControl, StatusMessageEventArgs statusMessageArgs)
        {
        }

        public bool OnConsoleMessage(IWebBrowser browserControl, ConsoleMessageEventArgs consoleMessageArgs)
        {
            return true;
        }
    }
}