using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models.Results
{
    [Serializable]
    public class ResultBulkUpsertApiModel
    {
        public long? SiteId { get; set; }
        public List<ResultApiModel>? Data { get; set; }
        public List<string>? Keys { get; set; }
        public bool KeyNotFoundCreate { get; set; } = true;
    }
}
