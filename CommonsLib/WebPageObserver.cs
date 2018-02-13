using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CommonsLib
{
    public class WebPageObserver
    {
        private readonly Dispatcher _dispatcher;
        private readonly Func<bool> _isLoading;
        private readonly Func<string, Task<ScriptRunResult>> _runJs;
        private readonly Action<string> _takeScreenshot;

        public WebPageObserver(Dispatcher dispatcher, Func<bool> isLoading, Func<string, Task<ScriptRunResult>> runJs, Action<string> takeScreenshot)
        {
            _dispatcher = dispatcher;
            _isLoading = isLoading;
            _runJs = runJs;
            _takeScreenshot = takeScreenshot;
        }

        private string[] queriesList = new[]
        {
            "document.activeElement.id",
            "document.activeElement.className",
            "document.activeElement.getBoundingClientRect().x",
            "document.activeElement.getBoundingClientRect().y",
            "document.activeElement.getBoundingClientRect().height",
            "document.activeElement.getBoundingClientRect().width",
            "document.activeElement.getBoundingClientRect().left",
            "document.activeElement.getBoundingClientRect().top",
            "document.activeElement.getBoundingClientRect().right",
            "document.activeElement.getBoundingClientRect().bottom",
            "document.activeElement.getBoundingClientRect().innerText",
            "document.activeElement.getBoundingClientRect().value",
            "document.activeElement.getBoundingClientRect().innerHtml",
        };

        public void mouseOverChanged(object className, object id, object x, object y)
        {
            Console.WriteLine("===============");
            Console.WriteLine("mouse over changed");

            Console.WriteLine("className : " + className);
            Console.WriteLine("id : " + id);
            Console.WriteLine("element x : " + x);
            Console.WriteLine("element y : " + y);
            Console.WriteLine("===============");
        }

        public void elementFocusChanged()
        {
            var result = new Dictionary<string, object>();
            Console.WriteLine("===============");
            Console.WriteLine("focus changed");
            foreach (var query in queriesList)
            {
                _dispatcher.Invoke(async () =>
                {
                    await _runJs(query).ContinueWith(task =>
                    {
                        object evaluateJavaScriptResult =
                            task.Result.Success ? (task.Result.Result ?? "null") : task.Result.Message;

                        result.Add(query, evaluateJavaScriptResult);
                        Console.WriteLine(query + " : " + evaluateJavaScriptResult);
                    });
                });
            }
            Console.WriteLine("===============");

        }

        public void mutationOccured()
        {
            var loading = true;

            while (loading)
            {
                _dispatcher.Invoke(() => loading = _isLoading());
                Console.WriteLine("loading: " + loading);
                Thread.Sleep(50);
            }
            _takeScreenshot("c:\\temp\\muta_screenshot.png");
        }
    }
}