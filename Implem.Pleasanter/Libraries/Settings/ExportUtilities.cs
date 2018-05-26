using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class ExportUtilities
    {
        public static Dictionary<string, ControlData> CurrentColumnOptions(
            IEnumerable<ExportColumn> columns)
        {
            return columns.ToDictionary(
                o => o.Id.ToString(),
                o => new ControlData(o.GetLabelText(withSiteTitle: true)));
        }

        public static Dictionary<string, ControlData> SourceColumnOptions(
            IEnumerable<ExportColumn> columns)
        {
            return columns.ToDictionary(
                o => new { SiteId = o.SiteId, ColumnName = o.ColumnName }.ToJson(),
                o => new ControlData(o.GetLabelText(withSiteTitle: true)));
        }

        public static Dictionary<string, ControlData> SourceColumnOptions(
            SiteSettings ss, Join join, string searchText = null)
        {
            var sources = new List<ExportColumn>();
            var allows = join?
                .Where(o => ss.JoinedSsHash?.ContainsKey(o.SiteId) == true)
                .ToList();
            allows?.Reverse();
            allows?.ForEach(link =>
                sources.AddRange(ss.JoinedSsHash.Get(link.SiteId)
                    .ExportColumns(searchText)));
            sources.AddRange(ss.ExportColumns(searchText));
            return SourceColumnOptions(sources);
        }

        public static ResponseFile Csv(SiteSettings ss, Export export)
        {
            ss.SetExports();
            ss.JoinedSsHash.Values.ForEach(currentSs => currentSs.SetChoiceHash(all: true));
            var data = new Dictionary<long, Dictionary<long, Dictionary<int, string>>>();
            Dictionary<long, long> keys = null;
            var keyColumns = KeyColumns(export, ss.SiteId);
            var view = Views.GetBySession(ss);
            switch (ss.ReferenceType)
            {
                case "Issues":
                    keys = IssueData(
                        ss,
                        data,
                        view.Where(ss),
                        view.OrderBy(ss, Rds.IssuesOrderBy()
                            .UpdatedTime(SqlOrderBy.Types.desc)),
                        export,
                        keys,
                        keyColumns);
                    break;
                case "Results":
                    keys = ResultData(
                        ss,
                        data,
                        view.Where(ss),
                        view.OrderBy(ss, Rds.ResultsOrderBy()
                            .UpdatedTime(SqlOrderBy.Types.desc)),
                        export,
                        keys,
                        keyColumns);
                    break;
            }
            export.Join?.ForEach(link =>
            {
                var currentSs = ss.JoinedSsHash.Get(link.SiteId);
                switch (currentSs.ReferenceType)
                {
                    case "Issues":
                        keys = IssueData(
                            currentSs,
                            data,
                            Rds.IssuesWhere().IssueId_In(keys.Values),
                            null,
                            export,
                            keys,
                            keyColumns);
                        break;
                    case "Results":
                        keys = ResultData(
                            currentSs,
                            data,
                            Rds.ResultsWhere().ResultId_In(keys.Values),
                            null,
                            export,
                            keys,
                            keyColumns);
                        break;
                }
            });
            var csv = new System.Text.StringBuilder();
            if (export.Header == true)
            {
                csv.Append(export.Columns.Select(column =>
                    "\"" + column.GetLabelText() + "\"").Join(","), "\n");
            }
            data
                .FirstOrDefault(o => o.Key == ss.SiteId)
                .Value?
                .ForEach(dataRow =>
                    csv.Append(export.Columns.Select(column =>
                        data
                            .Get(column.SiteId)?
                            .Get(dataRow.Key)?
                            .Get(column.Id) ?? string.Empty).Join(",") + "\n"));
            return new ResponseFile(csv.ToString(), FileName(ss, export.Name));
        }

        public static string FileName(SiteSettings ss, string name)
        {
            return Files.ValidFileName("_".JoinParam(
                ss.Title, name, DateTime.Now.ToLocal(Displays.YmdhmsFormat())) + ".csv");
        }

        private static Dictionary<long, string> KeyColumns(Export export, long siteId)
        {
            var keyColumns = new Dictionary<long, string>();
            export.Join?.ForEach(link =>
            {
                keyColumns.Add(siteId, link.ColumnName);
                siteId = link.SiteId;
            });
            return keyColumns;
        }

        private static Dictionary<long, long> IssueData(
            SiteSettings ss,
            Dictionary<long, Dictionary<long, Dictionary<int, string>>> data,
            SqlWhereCollection where,
            SqlOrderByCollection orderBy,
            Export export,
            Dictionary<long, long> keys,
            Dictionary<long, string> keyColumns)
        {
            ss.SetColumnAccessControls();
            var keyColumn = keyColumns.Get(ss.SiteId);
            var issueHash = new IssueCollection(
                ss: ss,
                column: IssuesColumn(ss, export, ss.GetColumn(keyColumn)),
                join: ss.Join(withColumn: true),
                where: where,
                orderBy: orderBy)
                    .ToDictionary(o => o.IssueId, o => o);
            if (keys == null)
            {
                data.Add(ss.SiteId, issueHash.ToDictionary(
                    o => o.Value.IssueId,
                    o => IssueData(ss, export, o.Value)));
                return issueHash.ToDictionary(
                    o => o.Key,
                    o => o.Value.PropertyValue(keyColumn).ToLong());
            }
            else
            {
                data.Add(ss.SiteId, keys.ToDictionary(
                    o => o.Key,
                    o => IssueData(ss, export, issueHash.Get(o.Value))));
                return keys.ToDictionary(
                    o => o.Key,
                    o => issueHash.Get(o.Value)?.PropertyValue(keyColumn).ToLong() ?? 0);
            }
        }

        private static SqlColumnCollection IssuesColumn(
            SiteSettings ss, Export export, Column keyColumn)
        {
            return new SqlColumnCollection(export
                .Columns
                .Where(o => o.SiteId == ss.SiteId)
                .SelectMany(o => o.Column.SqlColumnCollection(ss))
                .GroupBy(o => o.ColumnBracket + o.As)
                .Select(o => o.First())
                .ToArray())
                    .Issues_IssueId()
                    .Add(ss, keyColumn, _using: keyColumn != null);
        }

        private static Dictionary<int, string> IssueData(
            SiteSettings ss, Export export, IssueModel issueModel)
        {
            return export.Columns
                .Where(o => o.SiteId == ss.SiteId)
                .ToDictionary(
                    o => o.Id,
                    o => issueModel?.CsvData(
                        ss,
                        o.Column,
                        o,
                        issueModel.Mine()));
        }

        private static Dictionary<long, long> ResultData(
            SiteSettings ss,
            Dictionary<long, Dictionary<long, Dictionary<int, string>>> data,
            SqlWhereCollection where,
            SqlOrderByCollection orderBy,
            Export export,
            Dictionary<long, long> keys,
            Dictionary<long, string> keyColumns)
        {
            ss.SetColumnAccessControls();
            var keyColumn = keyColumns.Get(ss.SiteId);
            var resultHash = new ResultCollection(
                ss: ss,
                column: ResultsColumn(ss, export, ss.GetColumn(keyColumn)),
                join: ss.Join(withColumn: true),
                where: where,
                orderBy: orderBy)
                    .ToDictionary(o => o.ResultId, o => o);
            if (keys == null)
            {
                data.Add(ss.SiteId, resultHash.ToDictionary(
                    o => o.Value.ResultId,
                    o => ResultData(ss, export, o.Value)));
                return resultHash.ToDictionary(
                    o => o.Key,
                    o => o.Value.PropertyValue(keyColumn).ToLong());
            }
            else
            {
                data.Add(ss.SiteId, keys.ToDictionary(
                    o => o.Key,
                    o => ResultData(ss, export, resultHash.Get(o.Value))));
                return keys.ToDictionary(
                    o => o.Key,
                    o => resultHash.Get(o.Value)?.PropertyValue(keyColumn).ToLong() ?? 0);
            }
        }

        private static SqlColumnCollection ResultsColumn(
            SiteSettings ss, Export export, Column keyColumn)
        {
            return new SqlColumnCollection(export
                .Columns
                .Where(o => o.SiteId == ss.SiteId)
                .SelectMany(o => o.Column.SqlColumnCollection(ss))
                .GroupBy(o => o.ColumnBracket + o.As)
                .Select(o => o.First())
                .ToArray())
                    .Results_ResultId()
                    .Add(ss, keyColumn, _using: keyColumn != null);
        }

        private static Dictionary<int, string> ResultData(
            SiteSettings ss, Export export, ResultModel resultModel)
        {
            return export.Columns
                .Where(o => o.SiteId == ss.SiteId)
                .ToDictionary(
                    o => o.Id,
                    o => resultModel?.CsvData(
                        ss,
                        o.Column,
                        o,
                        resultModel.Mine()));
        }
    }
}
