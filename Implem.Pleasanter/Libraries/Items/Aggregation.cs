using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Items
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

        public Aggregation(Aggregation source)
        {
            Id = source.Id;
            GroupBy = source.GroupBy;
            Type = source.Type;
            Target = source.Target;
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
    }
}