using Implem.PleasanterTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    public static class ApiJsonData
    {
        public static List<ApiJsonTest> Tests(params ApiJsonTest[] tests)
        {
            return tests.ToList();
        }

        public static ApiJsonTest StatusCode(int statusCode)
        {
            return new ApiJsonTest()
            {
                Type = ApiJsonTest.Types.StatusCode,
                Value = statusCode
            };
        }
    }
}
