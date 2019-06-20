using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Aggregation
    {
        public enum Types
        {
            Count,
            Total,
            Average
        }

        public long Id;
        public string GroupBy;
        public Types Type;
        public string Target;
        [NonSerialized]
        public Dictionary<string, decimal> Data = new Dictionary<string, decimal>();

        public Aggregation()
        {
        }

        public Aggregation(
            long id,
            string groupBy,
            Types aggregationType = Types.Count,
            string aggregationTarget = "")
        {
            Id = id;
            GroupBy = groupBy;
            Type = aggregationType;
            Target = aggregationTarget;
        }

        public Aggregation GetRecordingData()
        {
            var aggregation = new Aggregation();
            aggregation.Id = Id;
            aggregation.GroupBy = GroupBy;
            aggregation.Type = Type;
            aggregation.Target = Target;
            return aggregation;
        }
    }
}