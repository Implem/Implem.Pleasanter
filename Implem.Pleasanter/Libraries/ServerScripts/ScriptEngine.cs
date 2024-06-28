using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ScriptEngine : IDisposable
    {
        private V8ScriptEngine v8ScriptEngine;

        public Func<bool> ContinuationCallback
        {
            set
            {
                v8ScriptEngine.ContinuationCallback = value == null
                    ? null
                    : new Microsoft.ClearScript.ContinuationCallback(value);
            }
        }

        public ScriptEngine(bool debug)
        {
            var flags = V8ScriptEngineFlags.EnableDateTimeConversion;
            if (debug)
            {
                flags |= V8ScriptEngineFlags.EnableDebugging
                | V8ScriptEngineFlags.EnableRemoteDebugging;
            }
            v8ScriptEngine = new V8ScriptEngine(flags);
        }

        public void AddHostType(Type type)
        {
            v8ScriptEngine?.AddHostType(type);
        }

        public void AddHostObject(string itemName, object target)
        {
            v8ScriptEngine?.AddHostObject(itemName, target);
        }

        public void Dispose()
        {
            v8ScriptEngine?.Dispose();
        }

        public void Execute(string code, bool debug)
        {
            v8ScriptEngine?.Execute(
                new DocumentInfo()
                {
                    Flags = debug ? DocumentFlags.AwaitDebuggerAndPause : DocumentFlags.None
                },
                code);
        }

        public object Evaluate(string code)
        {
            return v8ScriptEngine?.Evaluate(code);
        }
    }
}
