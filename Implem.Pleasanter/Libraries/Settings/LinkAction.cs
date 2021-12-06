using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class LinkAction
    {
        public string Type;
        public View View;
        public string CharToAddWhenCopying;
        public bool? CopyWithComments;

        public LinkAction GetRecordingData(Context context, SiteSettings ss)
        {
            var linkAction = new LinkAction();
            linkAction.Type = Type;
            linkAction.View = View?.GetRecordingData(
                context: context,
                ss: ss);
            linkAction.CharToAddWhenCopying = CharToAddWhenCopying;
            linkAction.CopyWithComments = CopyWithComments == true
                ? (bool?)true
                : null;
            return linkAction;
        }

        public void CopyWithLinks(
            Context context,
            SiteSettings ss,
            string columnName,
            long from,
            long to)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);
            if (column == null)
            {
                return;
            }
            if (View == null)
            {
                View = new View();
            }
            if (View.ColumnFilterHash == null)
            {
                View.ColumnFilterHash = new Dictionary<string, string>();
            }
            View.GridColumns = new List<string>()
            {
                Rds.IdColumn(ss.ReferenceType),
                columnName
            };
            View.ColumnFilterHash.AddOrUpdate(columnName, $"[\"{from}\"]");
            if (View.ColumnSorterHash == null)
            {
                View.ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
            }
            View.ColumnSorterHash.AddOrUpdate("CreatedTime", SqlOrderBy.Types.asc);
            var dataRows = new GridData(
                context: context,
                ss: ss,
                view: View,
                count: false)
                    .DataRows;
            switch (ss.ReferenceType)
            {
                case "Issues":
                    dataRows.ForEach(dataRow =>
                    {
                        var issueId = dataRow.Long("IssueId");
                        var data = GetData(
                            dataRow: dataRow,
                            column: column,
                            from: from,
                            to: to);
                        var formData = new Dictionary<string, string>()
                        {
                            { $"Issues_{columnName}", data }
                        };
                        if (!context.ContractSettings.ItemsLimit(
                            context: context,
                            siteId: ss.SiteId))
                        {
                            var issueModel = new IssueModel(
                                context: context,
                                ss: ss,
                                issueId: issueId,
                                formData: formData);
                            var invalid = IssueValidators.OnCreating(
                                context: context,
                                ss: ss,
                                issueModel: issueModel);
                            switch (invalid.Type)
                            {
                                case Error.Types.None:
                                    issueModel.IssueId = 0;
                                    issueModel.Ver = 1;
                                    issueModel.Title.Value += CharToAddWhenCopying ?? ss.CharToAddWhenCopying;
                                    if (CopyWithComments != true) issueModel.Comments.Clear();
                                    issueModel.SetCopyDefault(
                                        context: context,
                                        ss: ss);
                                    var errorData = issueModel.Create(
                                        context: context,
                                        ss: ss,
                                        copyFrom: issueModel.SavedIssueId,
                                        forceSynchronizeSourceSummary: true,
                                        notice: false,
                                        otherInitValue: true);
                                    break;
                            }
                        }
                    });
                    break;
                case "Results":
                    dataRows.ForEach(dataRow =>
                    {
                        var resultId = dataRow.Long("ResultId");
                        var data = GetData(
                            dataRow: dataRow,
                            column: column,
                            from: from,
                            to: to);
                        var formData = new Dictionary<string, string>()
                        {
                            { $"Results_{columnName}", data }
                        };
                        if (!context.ContractSettings.ItemsLimit(
                            context: context,
                            siteId: ss.SiteId))
                        {
                            var resultModel = new ResultModel(
                                context: context,
                                ss: ss,
                                resultId: resultId,
                                formData: formData);
                            var invalid = ResultValidators.OnCreating(
                                context: context,
                                ss: ss,
                                resultModel: resultModel);
                            switch (invalid.Type)
                            {
                                case Error.Types.None:
                                    resultModel.ResultId = 0;
                                    resultModel.Ver = 1;
                                    resultModel.Title.Value += CharToAddWhenCopying ?? ss.CharToAddWhenCopying;
                                    if (CopyWithComments != true) resultModel.Comments.Clear();
                                    resultModel.SetCopyDefault(
                                        context: context,
                                        ss: ss);
                                    var errorData = resultModel.Create(
                                        context: context,
                                        ss: ss,
                                        copyFrom: resultModel.SavedResultId,
                                        forceSynchronizeSourceSummary: true,
                                        notice: false,
                                        otherInitValue: true);
                                    break;
                            }
                        }
                    });
                    break;
            }
        }

        private string GetData(
            DataRow dataRow,
            Column column,
            long from,
            long to)
        {
            var data = to.ToString();
            if (column.MultipleSelections == true)
            {
                var list = dataRow.String(column.ColumnName) ?? string.Empty;
                data = list.Replace($"\"{from}\"", $"\"{to}\"");
            }
            return data;
        }

        public void DeleteWithLinks(
            Context context,
            SiteSettings ss,
            string columnName,
            SqlSelect sub)
        {
            if (!context.CanDelete(ss: ss))
            {
                return;
            }
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);
            if (column == null)
            {
                return;
            }
            if (View == null)
            {
                View = new View();
            }
            if (View.ColumnFilterHash == null)
            {
                View.ColumnFilterHash = new Dictionary<string, string>();
            }
            var where = View.Where(
                context: context,
                ss: ss,
                itemJoin: false);
            var linksSub = Rds.SelectItems(
                column: Rds.ItemsColumn().ReferenceId(),
                join: new Rds.LinksJoinCollection().Add(new SqlJoin(
                    tableBracket: "\"Links\"",
                    joinType: SqlJoin.JoinTypes.Inner,
                    joinExpression: "\"Links\".\"SourceId\"=\"Items\".\"ReferenceId\"")),
                where: Rds.LinksWhere().DestinationId_In(sub: sub));
            ErrorData invalid;
            switch (ss.ReferenceType)
            {
                case "Issues":
                    invalid = IssueUtilities.ExistsLockedRecord(
                        context: context,
                        ss: ss,
                        where: where,
                        param: null,
                        orderBy: View.OrderBy(
                            context: context,
                            ss: ss));
                    where.AddRange(new Rds.IssuesWhereCollection()
                        .IssueId_In(sub: linksSub));
                    switch (invalid.Type)
                    {
                        case Error.Types.None:
                            IssueUtilities.BulkDelete(
                                context: context,
                                ss: ss,
                                where: where,
                                param: null);
                            break;
                    }
                    Summaries.Synchronize(
                        context: context,
                        ss: ss);
                    break;
                case "Results":
                    invalid = ResultUtilities.ExistsLockedRecord(
                        context: context,
                        ss: ss,
                        where: where,
                        param: null,
                        orderBy: View.OrderBy(
                            context: context,
                            ss: ss));
                    where.AddRange(new Rds.ResultsWhereCollection()
                        .ResultId_In(sub: linksSub));
                    switch (invalid.Type)
                    {
                        case Error.Types.None:
                            ResultUtilities.BulkDelete(
                                context: context,
                                ss: ss,
                                where: where,
                                param: null);
                            break;
                    }
                    Summaries.Synchronize(
                        context: context,
                        ss: ss);
                    break;
            }
        }
    }
}
