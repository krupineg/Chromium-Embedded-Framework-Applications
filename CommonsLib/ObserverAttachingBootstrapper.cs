using System;
using System.IO;
using System.Threading.Tasks;

namespace CommonsLib
{
    public class ObserverAttachingBootstrapper
    {
        public async void Do(IJsRunner runScript)
        {
            string script = string.Format("(async function() {{ await CefSharp.BindObjectAsync(\"{0}\"); }})();",
                JavascriptNames.___Web_Observer);
             var result = await runScript.Evaluate(script);

            script = string.Format(
                "function detectFocus() {{ {0}.elementFocusChanged(); }}; window.addEventListener('focus', detectFocus, true)",
                JavascriptNames.___Web_Observer);
            result = await runScript.Evaluate(script);

            script = File.ReadAllText("Script/dom-recorder.js");

            result = await runScript.Evaluate(script);

            script = string.Format(
                "function detectMouseOver(className, id, name, x, y) {{ {0}.mouseOverChanged(className, id, name, (x === undefined) ? -9999 : x, (y === undefined) ? -9999 : y); }}; window.onmouseover = function(e) {{ detectMouseOver(e.target.className, e.target.id, e.target.name, e.target.getBoundingClientRect().x, e.target.getBoundingClientRect().y); }};",
                JavascriptNames.___Web_Observer);

            await runScript.Evaluate(script);
        }
    }


    public interface IJsRunner
    {
        Task<dynamic> Evaluate(string script);
    }
}