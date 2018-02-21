using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CommonsLib
{
    public class WebPageObserver : IWebPageObserver
    {
        private readonly IScriptRunner _scriptRunner;
        private readonly Guid _pageId;
        public event EventHandler<MouseOverChangedEventArgs> MouseOverChanged;
        public event EventHandler<FocusChangedEventArgs> FocusChanged;
        public event EventHandler<MutationEventArgs> Mutated;

        public WebPageObserver(IScriptRunner scriptRunner, Guid pageId)
        {
            _scriptRunner = scriptRunner;
            _pageId = pageId;
        }

        private string[] queriesList = new[]
        {
            "document.activeElement.id",
            "document.activeElement.name",
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

        public void mouseOverChanged(object className, object id, string name, object x, object y)
        {
            //Debug.WriteLine("===============");
            //Debug.WriteLine("mouse over changed");

            //Debug.WriteLine("className : " + className);
            //Debug.WriteLine("id : " + id);
            //Debug.WriteLine("name : " + name);
            //Debug.WriteLine("element x : " + x);
            //Debug.WriteLine("element y : " + y);
            //Debug.WriteLine("===============");
            if (MouseOverChanged != null)
            {
                var dictionary = new Dictionary<string, object>()
                {
                    {"className", className},
                    {"id", id},
                    {"name", name},
                    {"element x", x},
                    {"element y", y},
                };
                MouseOverChanged(this, new MouseOverChangedEventArgs(_pageId, dictionary));
            }
        }

        public async void elementFocusChanged()
        {
            var result = new Dictionary<string, object>();
            //Debug.WriteLine("===============");
            //Debug.WriteLine("focus changed");
            foreach (var query in queriesList)
            {
                await _scriptRunner.RunWithResult(query).ContinueWith(task =>
                {
                    var evaluateJavaScriptResult =
                        task.Result.Success ? (task.Result.Result ?? "null") : task.Result.Message;

                    result.Add(query, evaluateJavaScriptResult);
                });
            }
            if (FocusChanged != null)
            {
                FocusChanged(this, new FocusChangedEventArgs(_pageId, result));
            }
        }

        public void mutationOccured()
        {
            if (Mutated != null)
            {
                Mutated(this, new MutationEventArgs(_pageId));
            }
        }
    }
}