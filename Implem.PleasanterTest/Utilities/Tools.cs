using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    public static class Tools
    {
        public static string TestCode(string json)
        {
            var ret = json.Deserialize<ResponseCollection>()
                .Select(o => o.Target.IsNullOrEmpty()
                    ? @$"                JsonData.ExistsOne(method: ""{o.Method}"")"
                    : @$"                JsonData.ExistsOne(
                    method: ""{o.Method}"",
                    target: ""{o.Target}"")")
                .Join(",\r\n");
            return ret;
        }
    }
}
