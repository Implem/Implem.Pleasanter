using Implem.PleasanterTest.Models;

namespace Implem.PleasanterTest.Utilities
{
    public static class ApiJsonData
    {
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
