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
        public static Dictionary<string, ControlData> ColumnOptions(
            IEnumerable<ExportColumn> columns)
        {
            return columns.ToDictionary(
                o => new { SiteId = o.SiteId, Id=o.Id, ColumnName = o.ColumnName, LabelText = o.LabelText, Type = o.Type }.ToJson(),
                o => new ControlData(o.GetLabelText(withSiteTitle: true)));
        }

        public static Dictionary<string, ControlData> SourceColumnOptions(
            Context context, SiteSettings ss, Join join, string searchText = null)
        {
            var sources = new List<ExportColumn>();
            var allows = join?
                .Where(o => ss.JoinedSsHash?.ContainsKey(o.SiteId) == true)
                .ToList();
            allows?.Reverse();
            allows?.ForEach(link =>
                sources.AddRange(ss.JoinedSsHash.Get(link.SiteId)
                    .ExportColumns(
                        context: context,
                        searchText: searchText)));
            sources.AddRange(ss.ExportColumns(
                context: context,
                searchText: searchText));
            return ColumnOptions(sources);
        }

        public static ResponseFile Csv(Context context, SiteSettings ss, Export export)
        {
            ss.SetExports(context: context);
            ss.JoinedSsHash.Values.ForEach(currentSs => currentSs
                .SetChoiceHash(context: context, all: true));
            var data = new Dictionary<long, Dictionary<long, Dictionary<int, string>>>();
            Dictionary<long, long> keys = null;
            var keyColumns = KeyColumns(export, ss.SiteId);
            var view = Views.GetBySession(context: context, ss: ss);
            switch (ss.ReferenceType)
            {
                case "Issues":
                    keys = IssueData(
                        context: context,
                        ss: ss,
                        data: data,
                        where: view.Where(
                            context: context,
                            ss: ss),
                        orderBy: view.OrderBy(
                            context: context,
                            ss: ss)
                                .Issues_UpdatedTime(SqlOrderBy.Types.desc),
                        export: export,
                        keys: keys,
                        keyColumns: keyColumns);
                    break;
                case "Results":
                    keys = ResultData(
                        context: context,
                        ss: ss,
                        data: data,
                        where: view.Where(
                            context: context,
                            ss: ss),
                        orderBy: view.OrderBy(
                            context: context,
                            ss: ss)
                                .Results_UpdatedTime(SqlOrderBy.Types.desc),
                        export: export,
                        keys: keys,
                        keyColumns: keyColumns);
                    break;
            }
            export.Join?.ForEach(link =>
            {
                var currentSs = ss.JoinedSsHash.Get(link.SiteId);
                switch (currentSs.ReferenceType)
                {
                    case "Issues":
                        keys = IssueData(
                            context: context,
                            ss: currentSs,
                            data: data,
                            where: Rds.IssuesWhere().IssueId_In(keys.Values),
                            orderBy: null,
                            export: export,
                            keys: keys,
                            keyColumns: keyColumns);
                        break;
                    case "Results":
                        keys = ResultData(
                            context: context,
                            ss: currentSs,
                            data: data,
                            where: Rds.ResultsWhere().ResultId_In(keys.Values),
                            orderBy: null,
                            export: export,
                            keys: keys,
                            keyColumns: keyColumns);
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
            return new ResponseFile(
                fileContent: csv.ToString(),
                fileDownloadName: FileName(
                    context: context,
                    ss: ss,
                    name: export.Name));
        }

        public static string FileName(Context context, SiteSettings ss, string name)
        {
            return Files.ValidFileName("_".JoinParam(
                ss.Title, name, DateTime.Now.ToLocal(
                    context: context,
                    format: Displays.YmdhmsFormat(context: context))) + ".csv");
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
            Context context,
            SiteSettings ss,
            Dictionary<long, Dictionary<long, Dictionary<int, string>>> data,
            SqlWhereCollection where,
            SqlOrderByCollection orderBy,
            Export export,
            Dictionary<long, long> keys,
            Dictionary<long, string> keyColumns)
        {
            ss.SetColumnAccessControls(context: context);
            var keyColumn = keyColumns.Get(ss.SiteId);
            var column = IssuesColumn(ss, export, ss.GetColumn(
                context: context,
                columnName: keyColumn));
            var issueHash = new IssueCollection(
                context: context,
                ss: ss,
                column: column,
                join: ss.Join(
                    context: context,
                    join: new Implem.Libraries.DataSources.Interfaces.IJoin[]
                    {
                        column,
                        where,
                        orderBy
                    }),
                where: where,
                orderBy: orderBy)
                    .ToDictionary(o => o.IssueId, o => o);
            if (keys == null)
            {
                data.Add(ss.SiteId, issueHash.ToDictionary(
                    o => o.Value.IssueId,
                    o => IssueData(
                        context: context,
                        ss: ss,
                        export: export,
                        issueModel: o.Value)));
                return issueHash.ToDictionary(
                    o => o.Key,
                    o => o.Value.PropertyValue(
                        context: context, name: keyColumn).ToLong());
            }
            else
            {
                data.Add(ss.SiteId, keys.ToDictionary(
                    o => o.Key,
                    o => IssueData(
                        context: context,
                        ss: ss,
                        export: export,
                        issueModel: issueHash.Get(o.Value))));
                return keys.ToDictionary(
                    o => o.Key,
                    o => issueHash.Get(o.Value)?.PropertyValue(
                        context: context, name: keyColumn).ToLong() ?? 0);
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
            Context context, SiteSettings ss, Export export, IssueModel issueModel)
        {
            return export.Columns
                .Where(exportColumn => exportColumn.SiteId == ss.SiteId)
                .ToDictionary(
                    exportColumn => exportColumn.Id,
                    exportColumn => issueModel?.CsvData(
                        context: context,
                        ss: ss,
                        column: exportColumn.Column,
                        exportColumn: exportColumn,
                        mine: issueModel.Mine(context: context)));
        }

        private static Dictionary<long, long> ResultData(
            Context context,
            SiteSettings ss,
            Dictionary<long, Dictionary<long, Dictionary<int, string>>> data,
            SqlWhereCollection where,
            SqlOrderByCollection orderBy,
            Export export,
            Dictionary<long, long> keys,
            Dictionary<long, string> keyColumns)
        {
            ss.SetColumnAccessControls(context: context);
            var keyColumn = keyColumns.Get(ss.SiteId);
            var column = ResultsColumn(ss, export, ss.GetColumn(
                context: context,
                columnName: keyColumn));
            var resultHash = new ResultCollection(
                context: context,
                ss: ss,
                column: column,
                join: ss.Join(
                    context: context,
                    join: new Implem.Libraries.DataSources.Interfaces.IJoin[]
                    {
                        column,
                        where,
                        orderBy
                    }),
                where: where,
                orderBy: orderBy)
                    .ToDictionary(o => o.ResultId, o => o);
            if (keys == null)
            {
                data.Add(ss.SiteId, resultHash.ToDictionary(
                    o => o.Value.ResultId,
                    o => ResultData(
                        context: context,
                        ss: ss,
                        export: export,
                        resultModel: o.Value)));
                return resultHash.ToDictionary(
                    o => o.Key,
                    o => o.Value.PropertyValue(
                        context: context, name: keyColumn).ToLong());
            }
            else
            {
                data.Add(ss.SiteId, keys.ToDictionary(
                    o => o.Key,
                    o => ResultData(
                        context: context,
                        ss: ss,
                        export: export,
                        resultModel: resultHash.Get(o.Value))));
                return keys.ToDictionary(
                    o => o.Key,
                    o => resultHash.Get(o.Value)?.PropertyValue(
                        context: context, name: keyColumn).ToLong() ?? 0);
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
            Context context, SiteSettings ss, Export export, ResultModel resultModel)
        {
            return export.Columns
                .Where(exportColumn => exportColumn.SiteId == ss.SiteId)
                .ToDictionary(
                    exportColumn => exportColumn.Id,
                    exportColumn => resultModel?.CsvData(
                        context: context,
                        ss: ss,
                        column: exportColumn.Column,
                        exportColumn: exportColumn,
                        mine: resultModel.Mine(context: context)));
        }
    }
}
