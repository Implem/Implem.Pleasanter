using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models.Issues
{
    [Serializable]
    public class IssueBulkUpsertApiModel
    {
        public long? SiteId { get; set; }
        public List<IssueApiModel> Data { get; set; }
        public List<string> Keys { get; set; }
        public bool KeyNotFoundCreate { get; set; } = true;
    }
}
