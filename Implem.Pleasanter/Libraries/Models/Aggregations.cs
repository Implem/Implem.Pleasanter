using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Settings;
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

        public void Set(
            SiteSettings siteSettings,
            DataSet dataSet,
            IEnumerable<Aggregation> aggregationCollection)
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
                {
                    var groupByColumn = siteSettings.AllColumn(data.Aggregation.GroupBy);
                    dataSet.Tables["Aggregation" + data.Index]
                        .AsEnumerable()
                        .ForEach(dataRow =>
                        {
                            if (groupByColumn != null)
                            {
                                if (dataRow[1].ToDecimal() != 0)
                                {
                                    var key = Key(
                                        siteSettings,
                                        dataRow[0].ToString(),
                                        groupByColumn);
                                    if (data.Aggregation.Data.ContainsKey(key))
                                    {
                                        data.Aggregation.Data[key] +=
                                            dataRow[1].ToDecimal();
                                    }
                                    else
                                    {
                                        data.Aggregation.Data.Add(
                                            key, dataRow[1].ToDecimal());
                                    }
                                }
                            }
                            else
                            {
                                data.Aggregation.Data.Add(
                                    string.Empty, dataRow[0].ToDecimal());
                            }
                        });
                });
        }

        private static string Key(SiteSettings siteSettings, string key, Column groupByColumn)
        {
            return !(groupByColumn.UserColumn && key.ToInt() == 0)
                 ? key
                 : User.UserTypes.Anonymous.ToInt().ToString();
        }
    }
}