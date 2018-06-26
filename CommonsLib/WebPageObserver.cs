using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonsLib
{
    public class WebPageObserver : IWebPageObserver
    {
        private readonly IScriptRunner _scriptRunner;
        private readonly Guid _pageId;
        public event EventHandler<MouseOverChangedEventArgs> MouseOverChanged;
        public event EventHandler<FocusChangedEventArgs> FocusChanged;
        public event EventHandler<MutationEventArgs> Mutated;
        public event EventHandler<BeaconEventArgs> BeaconEvent;

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
            "document.activeElement.tagName",
            "document.activeElement.type",
            "document.activeElement.jsaction",
            "document.activeElement.href",
            "document.activeElement.title",
            "document.activeElement.value",
            "document.activeElement.role",
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
            "document.activeElement.parentElement.tagName",
            "document.activeElement.parentElement.id",
            "document.activeElement.parentElement.name",
            "document.activeElement.parentElement.className",
            "document.activeElement.parentElement.href",
            "document.activeElement.parentElement.value",
        };

        public async void mouseOverChanged(object className, object id, string name, double x, double y)
        {
            var res = await _scriptRunner.RunWithResult($"domRecorder.api.highlight({x}, {y})");
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

        public void mutationCallback(string eventName, string beaconChanged)
        {
            if (BeaconEvent != null)
            {
                var jsonReader = new JsonTextReader(new StringReader(beaconChanged))
                {
                    DateParseHandling = DateParseHandling.DateTimeOffset
                };

                var entity = JObject.Load(jsonReader).ToObject<BeaconChangedEvent>();
                //var  entity = Parsers[eventName](beaconChanged);
                BeaconEvent(this, new BeaconEventArgs(_pageId, entity));
            }
        }

        private Dictionary<string, Func<string, BeaconChangedEvent>> Parsers =
            new Dictionary<string, Func<string, BeaconChangedEvent>>()
            {
                {"beacon-changed", JsonConvert.DeserializeObject<BeaconChangedEvent>},
                {"beacon-detected", JsonConvert.DeserializeObject<BeaconChangedEvent>},
                {"beacon-removed", JsonConvert.DeserializeObject<BeaconChangedEvent>}
            };

        public class BeaconEventArgs : EventArgs
        {
            public BeaconEventArgs(Guid pageId, BeaconChangedEvent beaconEvent)
            {
                PageId = pageId;
                Event = beaconEvent.type;
                Timestamp = beaconEvent.time;
                Beacon = beaconEvent.beacon;
                Tag = beaconEvent.beacon.tag;
            }

            public Guid PageId { get; }
            public string Event { get; }
            public long Timestamp { get; }
            public string Tag { get; }
            public Beacon Beacon { get; set; }
        }
    }

    public class BeaconChangedEvent
    {
        public string type { get; set; }
        public long time { get; set; }
        public Beacon beacon { get; set; }
        public string url { get; set; }
    }

    public class BeaconAppearedEvent : BeaconChangedEvent
    {
    }

    public class BeaconDisappearedEvent : BeaconChangedEvent
    {
    }

    public class Beacon
    {
        public string id { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public string tag { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> tags { get; set; }
    }
}