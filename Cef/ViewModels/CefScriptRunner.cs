using System.Threading.Tasks;
using CefSharp;
using CommonsLib;

namespace Cef
{
    public class CefScriptRunner : IScriptRunner
    {
        private readonly IWebBrowser _browser;

        public CefScriptRunner(IWebBrowser browser)
        {
            _browser = browser;
        }

        public Task<ScriptRunResult> RunWithResult(string script)
        {
            var task = _browser.EvaluateScriptAsync(script);
            return task.ContinueWith(task1 => new ScriptRunResult()
            {
                Success = task1.Result.Success,
                Result = task1.Result.Result,
                Message = task1.Result.Message
            });
        }
    }
}