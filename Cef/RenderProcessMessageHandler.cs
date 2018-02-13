using System;
using CefSharp;
using CommonsLib;

namespace Cef
{
    internal class RenderProcessMessageHandler : IRenderProcessMessageHandler
    {
        private readonly WebPageObserver _observer;

        public RenderProcessMessageHandler(WebPageObserver observer)
        {
            _observer = observer;
        }

        void IRenderProcessMessageHandler.OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node)
        {
            var message = node == null ? "lost focus" : node.ToString();

            Console.WriteLine("OnFocusedNodeChanged() - " + message);
        }

        void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            string loadedScript = string.Format("document.addEventListener('DOMContentLoaded', function(){{ {0}.domLoaded(); }});", JavascriptNames.___Web_Observer);
            frame.EvaluateScriptAsync(loadedScript).ConfigureAwait(false);


            const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
            Console.WriteLine("OnContextCreated()" );
            //frame.ExecuteJavaScriptAsync(script);
        }

        void IRenderProcessMessageHandler.OnContextReleased(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            Console.WriteLine("OnContextCreated()");
            //The V8Context is about to be released, use this notification to cancel any long running tasks your might have
        }
    }
}