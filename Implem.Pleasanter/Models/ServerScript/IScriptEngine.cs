using System;
namespace Implem.Pleasanter.Models
{
    public interface IScriptEngine : IDisposable
    {
        void AddHostObject(string itemName, object target);
        void Execute(string code);
    }
}
