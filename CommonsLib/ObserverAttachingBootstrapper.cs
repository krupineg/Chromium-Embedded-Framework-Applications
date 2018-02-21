using System;
using System.IO;

namespace CommonsLib
{
    public class ObserverAttachingBootstrapper
    {
        public void Do(Action<string> runScript)
        {
            runScript(string.Format("(async function() {{ await CefSharp.BindObjectAsync(\"{0}\"); }})();",
                JavascriptNames.___Web_Observer));

            string script = string.Format(
                "function detectFocus() {{ {0}.elementFocusChanged(); }}; window.addEventListener('focus', detectFocus, true)",
                JavascriptNames.___Web_Observer);
            runScript(script);

            script = File.ReadAllText("Scripts/mutation_aware.js").Replace("//{ SOME USEFUL ACTION HERE } ",
                string.Format("{0}.mutationOccured(); ", JavascriptNames.___Web_Observer));

            runScript(script);

            script = string.Format(
                "function detectMouseOver(className, id, name, x, y) {{ {0}.mouseOverChanged(className, id, name, (x === undefined) ? -9999 : x, (y === undefined) ? -9999 : y); }}; window.onmouseover = function(e) {{ detectMouseOver(e.target.className, e.target.id, e.target.name, e.target.getBoundingClientRect().x, e.target.getBoundingClientRect().y); }};",
                JavascriptNames.___Web_Observer);

            runScript(script);
        }
    }
}