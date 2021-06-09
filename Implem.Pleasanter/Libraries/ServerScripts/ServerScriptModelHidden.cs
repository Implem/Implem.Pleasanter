using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelHidden
    {
        private Dictionary<string, string> data;

        public ServerScriptModelHidden()
        {
            data = new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetAll()
        {
            return data;
        }

        public string Get(string key = null)
        {
            return data[key];
        }

        public void Add(string key = null, object value = null)
        {
            data.Add(key, value.ToString());
        }
    }
}