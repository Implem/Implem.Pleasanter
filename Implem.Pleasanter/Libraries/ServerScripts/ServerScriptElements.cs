using Implem.Libraries.Utilities;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptElements
    {
        public enum DisplayTypes : int
        {
            Normal = 0,
            Nothing = 1,
            Disabled = 2,
            Hidden = 3,
        }

        public Dictionary<string, DisplayTypes> DisplayTypeHash = new Dictionary<string, DisplayTypes>();

        public void DisplayType(string key, int type)
        {
            DisplayTypeHash.AddOrUpdate(key, (DisplayTypes)type);
        }

        public bool Normal(string key)
        {
            var value = DisplayTypeHash.Get(key);
            return value == DisplayTypes.Normal;
        }

        public bool Nothing(string key)
        {
            var value = DisplayTypeHash.Get(key);
            return value == DisplayTypes.Nothing;
        }

        public bool Disabled(string key)
        {
            var value = DisplayTypeHash.Get(key);
            return value == DisplayTypes.Disabled;
        }

        public bool Hidden(string key)
        {
            var value = DisplayTypeHash.Get(key);
            return value == DisplayTypes.Hidden;
        }
    }
}