using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models.Shared;
using System;

namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class DemoApiModel : _BaseApiModel
    {
        public string MailAddress = string.Empty;

        public DemoApiModel()
        {
        }
    }
}
