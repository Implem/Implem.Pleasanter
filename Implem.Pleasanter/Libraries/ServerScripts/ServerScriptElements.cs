using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptElements
    {
        public Dictionary<string, View.CommandDisplayTypes> DisplayTypeHash = new Dictionary<string, View.CommandDisplayTypes>();

        public void DisplayType(string key, int type)
        {
            DisplayTypeHash.AddOrUpdate(key, (View.CommandDisplayTypes)type);
        }

        public bool Displayed(string key)
        {
            var value = DisplayTypeHash.Get(key);
            return value == View.CommandDisplayTypes.Displayed;
        }

        public bool None(string key)
        {
            var value = DisplayTypeHash.Get(key);
            return value == View.CommandDisplayTypes.None;
        }

        public bool Disabled(string key)
        {
            var value = DisplayTypeHash.Get(key);
            return value == View.CommandDisplayTypes.Disabled;
        }

        public bool Hidden(string key)
        {
            var value = DisplayTypeHash.Get(key);
            return value == View.CommandDisplayTypes.Hidden;
        }
    }
}