using Implem.Pleasanter.Models;
using Microsoft.ClearScript.V8;
namespace Implem.Pleasanter.Models
{
    public class ScriptEngine : IScriptEngine
    {
        private V8ScriptEngine v8ScriptEngine;

        public ScriptEngine()
        {
            v8ScriptEngine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDateTimeConversion);
        }

        public void AddHostObject(string itemName, object target)
        {
            v8ScriptEngine?.AddHostObject(itemName, target);
        }

        public void Dispose()
        {
            v8ScriptEngine?.Dispose();
        }

        public void Execute(string code)
        {
            v8ScriptEngine?.Execute(code);
        }
    }
}
