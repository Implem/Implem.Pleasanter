using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class ExtendedApi : Api
    {
        public string Name { get; set; }
        public Dictionary<string, object> Params { get; set; }

        public ExtendedApi()
        {
        }
    }
}