using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public class Aggregations
    {
        public int TotalCount;
        public int OverdueCount;
        public IEnumerable<Aggregation> AggregationCollection;

        public void Set(DataSet dataSet, IEnumerable<Aggregation> aggregationCollection)
        {
            AggregationCollection = aggregationCollection;
            TotalCount = Rds.Count(dataSet);
            if (dataSet.Tables.Contains("OverdueCount") &&
                dataSet.Tables["OverdueCount"].Rows.Count == 1)
            {
                OverdueCount = dataSet.Tables["OverdueCount"].Rows[0]["OverdueCount"].ToInt();
            }
            AggregationCollection?
                .Select((o, i) => new { Aggregation = o, Index = i })
                .Where(o => dataSet.Tables.Contains("Aggregation" + o.Index))
                .ForEach(data =>
                    dataSet.Tables["Aggregation" + data.Index]
                        .AsEnumerable()
                        .ForEach(dataRow =>
                        {
                            if (data.Aggregation.GroupBy != "[NotGroupBy]")
                            {
                                if (dataRow[1].ToDecimal() != 0)
                                {
                                    if (data.Aggregation.Data.ContainsKey(dataRow[0].ToString()))
                                    {
                                        data.Aggregation.Data[dataRow[0].ToString()] +=
                                            dataRow[1].ToDecimal();
                                    }
                                    else
                                    {
                                        data.Aggregation.Data.Add(
                                            dataRow[0].ToString(), dataRow[1].ToDecimal());
                                    }
                                }
                            }
                            else
                            {
                                data.Aggregation.Data.Add(
                                    string.Empty, dataRow[0].ToDecimal());
                            }
                        }));
        }
    }
}