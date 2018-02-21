using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Cef
{
    public class QueryPredictor
    {
        public const string QueryBase = "http://suggestqueries.google.com/complete/search?client=firefox&q=";
        private object _locker = new object();

        public async Task<IEnumerable<string>> Predict(string part)
        {
            if (Monitor.TryEnter(_locker))
            {
                var query = string.Format("{0}{1}", QueryBase, part);
                using (var wc = new WebClient())
                {
                    var stringResult = await wc.DownloadStringTaskAsync(query);
                    var arrayOuter = JArray.Parse(stringResult);
                    var last = arrayOuter.Last();
                    var strings = last.ToObject<string[]>();
                    return strings;
                }
            }
            else
            {
                return new string[0];
            }
        }
    }
}