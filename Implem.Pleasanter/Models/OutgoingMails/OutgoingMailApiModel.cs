using Implem.Pleasanter.Libraries.Requests;
using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class OutgoingMailApiModel : Api
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}