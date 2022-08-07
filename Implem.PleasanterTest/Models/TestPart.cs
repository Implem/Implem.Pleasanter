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
        public Forms Forms { get; set; }
        public string ApiRequestBody { get; set; }
        public List<HtmlTest> HtmlTests { get; set; }
        public List<JsonTest> JsonTests { get; set; }
        public List<ApiJsonTest> ApiJsonTests { get; set; }
        public UserModel UserModel { get; set; }

        public TestPart(
            string title = null,
            Forms forms = null,
            object apiRequestBody = null,
            List<HtmlTest> htmlTests = null,
            List<JsonTest> jsonTests = null,
            List<ApiJsonTest> apiJsonTests = null,
            UserData.UserTypes userType = UserData.UserTypes.General1)
        {
            Title = title;
            Forms = forms;
            ApiRequestBody = apiRequestBody.ToJson();
            HtmlTests = htmlTests;
            JsonTests = jsonTests;
            ApiJsonTests = apiJsonTests;
            UserModel = UserData.Get(userType: userType);
        }
    }
}
