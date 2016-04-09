using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Utilities
{
    public static class OptionCollections
    {
        public static Dictionary<string, ControlData> Enum<T>()
        {
            var hash = new Dictionary<string, ControlData>();
            foreach (var enumPart in System.Enum.GetValues(typeof(T)))
            {
                hash.Add(
                    ((int)enumPart).ToString(),
                    new ControlData(enumPart.ToString()));
            }
            return hash;
        }
    }
}