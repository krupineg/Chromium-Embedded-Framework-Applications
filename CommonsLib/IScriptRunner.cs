using System.Threading.Tasks;

namespace CommonsLib
{
    public interface IScriptRunner
    {
        Task<ScriptRunResult> RunWithResult(string script);
    }
}