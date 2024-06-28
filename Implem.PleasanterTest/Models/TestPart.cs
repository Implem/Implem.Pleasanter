using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;

namespace Implem.PleasanterTest.Models
{
    public class TestPart
    {
        public string Title { get; set; }
        public Dictionary<string, string> RouteData { get; set; }
        public QueryStrings QueryStrings { get; set; }
        public Forms Forms { get; set; }
        public string Guid { get; set; }
        public string FileName { get; set; }
        public string ApiRequestBody { get; set; }
        public List<BaseTest> BaseTests { get; set; }
        public UserModel UserModel { get; set; }
        public Dictionary<string, string> ExtendedParams {  get; set; }

        public TestPart(
            string title = null,
            Dictionary<string, string> routeData = null,
            QueryStrings queryStrings = null,
            Forms forms = null,
            object apiRequestBody = null,
            string guid = null,
            string fileName = null,
            List<BaseTest> baseTests = null,
            UserData.UserTypes userType = UserData.UserTypes.General1,
            Dictionary<string, string> extendedParams = null)
        {
            Title = title;
            RouteData = routeData;
            QueryStrings = queryStrings;
            Forms = forms;
            Guid = guid;
            FileName = fileName;
            ApiRequestBody = apiRequestBody.ToJson();
            BaseTests = baseTests;
            UserModel = UserData.Get(userType: userType);
            ExtendedParams = extendedParams;
        }
    }
}
