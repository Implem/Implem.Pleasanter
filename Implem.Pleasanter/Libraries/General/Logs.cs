using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.General
{
    public class Logs : List<Log>
    {
        public void Add(string name, string value)
        {
            Add(new Log(name, value));
        }
    }
}