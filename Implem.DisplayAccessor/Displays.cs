using System.Collections.Generic;
using System.Linq;
namespace Implem.DisplayAccessor
{
    public static class Displays
    {
        public enum Types : int
        {
            Normal = 110,
            Date = 120,
            DateFormat = 130,
            Success = 210,
            Information = 220,
            Warning = 230,
            Error = 240,
            Confirmation = 310,
            Validation = 410
        }

        public static Dictionary<string, Display> DisplayHash;

        public static string Get(string id)
        {
            return DisplayHash[id].Languages.FirstOrDefault().Body;
        }
    }
}
