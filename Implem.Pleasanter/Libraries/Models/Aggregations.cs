﻿using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
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

        public Aggregations(Context context, SiteSettings ss, View view)
        {
            var tableType = (view.ShowHistory == true)
                ? Sqls.TableTypes.NormalAndHistory
                : ss.TableType;
            var where = view.Where(
                context: context,
                ss: ss);
            Set(
                context: context,
                ss: ss,
                dataSet: Rds.ExecuteDataSet(
                    context: context,
                    statements: Rds.Aggregations(
                        ss: ss,
                        tableType: tableType,
                        join: ss.Join(
                            context: context,
                            join: where),
                        where: where).ToArray()));
        }

        private void Set(
            Context context,
            SiteSettings ss,
            DataSet dataSet)
        {
            AggregationCollection = ss.Aggregations;
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
                    var groupByColumn = ss?.GetColumn(
                        context: context, columnName: data.Aggregation.GroupBy);
                    dataSet.Tables["Aggregation" + data.Index]
                        .AsEnumerable()
                        .ForEach(dataRow =>
                        {
                            if (groupByColumn != null)
                            {
                                if (dataRow[1].ToDecimal() != 0)
                                {
                                    var key = Key(
                                        ss,
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
                                if (!data.Aggregation.Data.ContainsKey(string.Empty))
                                {
                                    data.Aggregation.Data.Add(
                                        string.Empty, dataRow[0].ToDecimal());
                                }
                            }
                        });
                });
        }

        private static string Key(SiteSettings ss, string key, Column groupByColumn)
        {
            return !(groupByColumn.UserColumn && key.ToInt() == 0)
                 ? key
                 : User.UserTypes.Anonymous.ToInt().ToString();
        }
    }
}