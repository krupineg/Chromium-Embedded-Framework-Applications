using System.IO;
using Newtonsoft.Json;

namespace CommonsLib
{
    public class JavascriptNames
    {
        public static readonly string ___Web_Observer = GetName();

        private static string GetName()
        {
            var json = File.ReadAllText("Script/entry.config.json");
            var cfg = JsonConvert.DeserializeObject<Cfg>(json);
            return cfg.entryPoint;
        }

        internal class Cfg
        {
            public string entryPoint { get; set; }
        }
    }
}