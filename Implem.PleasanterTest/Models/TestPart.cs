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
        public QueryStrings QueryStrings { get; set; }
        public Forms Forms { get; set; }
        public string ApiRequestBody { get; set; }
        public List<HtmlTest> HtmlTests { get; set; }
        public List<JsonTest> JsonTests { get; set; }
        public List<ApiJsonTest> ApiJsonTests { get; set; }
        public List<FileTest> FileTests { get; set; }
        public UserModel UserModel { get; set; }

        public TestPart(
            string title = null,
            QueryStrings queryStrings = null,
            Forms forms = null,
            object apiRequestBody = null,
            List<HtmlTest> htmlTests = null,
            List<JsonTest> jsonTests = null,
            List<ApiJsonTest> apiJsonTests = null,
            List<FileTest> fileTests = null,
            UserData.UserTypes userType = UserData.UserTypes.General1)
        {
            Title = title;
            QueryStrings = queryStrings;
            Forms = forms;
            ApiRequestBody = apiRequestBody.ToJson();
            HtmlTests = htmlTests;
            JsonTests = jsonTests;
            ApiJsonTests = apiJsonTests;
            FileTests = fileTests;
            UserModel = UserData.Get(userType: userType);
        }
    }
}
