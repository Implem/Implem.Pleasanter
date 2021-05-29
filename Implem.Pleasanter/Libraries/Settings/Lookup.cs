using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Lookup
    {
        public enum Types
        {
            Value,
            DisplayName
        }

        public string From { get; set; }
        public string To { get; set; }
        public Types? Type { get; set; }

        public Lookup GetRecordingData()
        {
            var lookup = new Lookup();
            lookup.From = From;
            lookup.To = To;
            if (Type != Types.Value) lookup.Type = Type;
            return lookup;
        }

        public string Data(
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: From);
            if (column?.CanRead(
                context: context,
                ss: ss,
                mine: issueModel.Mine(context: context)) == true)
            {
                switch (column.ColumnName)
                {
                    case "SiteId":
                        return issueModel.SiteId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "UpdatedTime":
                        return issueModel.UpdatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "IssueId":
                        return issueModel.IssueId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Ver":
                        return issueModel.Ver.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Title":
                        return issueModel.Title.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Body":
                        return issueModel.Body.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "TitleBody":
                        return issueModel.TitleBody.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "StartTime":
                        return issueModel.StartTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "CompletionTime":
                        return issueModel.CompletionTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "WorkValue":
                        return issueModel.WorkValue.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "ProgressRate":
                        return issueModel.ProgressRate.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "RemainingWorkValue":
                        return issueModel.RemainingWorkValue.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Status":
                        return issueModel.Status.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Manager":
                        return issueModel.Manager.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Owner":
                        return issueModel.Owner.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Locked":
                        return issueModel.Locked.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "SiteTitle":
                        return issueModel.SiteTitle.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Comments":
                        return issueModel.Comments.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Creator":
                        return issueModel.Creator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Updator":
                        return issueModel.Updator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "CreatedTime":
                        return issueModel.CreatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "VerUp":
                        return issueModel.VerUp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Timestamp":
                        return issueModel.Timestamp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                return issueModel.Class(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Num":
                                return issueModel.Num(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Date":
                                return issueModel.Date(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Description":
                                return issueModel.Description(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Check":
                                return issueModel.Check(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Attachments":
                                return issueModel.Attachments(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            default:
                                return string.Empty;
                        }
                }
            }
            return string.Empty;
        }

        public string Data(
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: From);
            if (column?.CanRead(
                context: context,
                ss: ss,
                mine: resultModel.Mine(context: context)) == true)
            {
                switch (column.ColumnName)
                {
                    case "SiteId":
                        return resultModel.SiteId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "UpdatedTime":
                        return resultModel.UpdatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "ResultId":
                        return resultModel.ResultId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Ver":
                        return resultModel.Ver.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Title":
                        return resultModel.Title.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Body":
                        return resultModel.Body.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "TitleBody":
                        return resultModel.TitleBody.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Status":
                        return resultModel.Status.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Manager":
                        return resultModel.Manager.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Owner":
                        return resultModel.Owner.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Locked":
                        return resultModel.Locked.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "SiteTitle":
                        return resultModel.SiteTitle.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Comments":
                        return resultModel.Comments.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Creator":
                        return resultModel.Creator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Updator":
                        return resultModel.Updator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "CreatedTime":
                        return resultModel.CreatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "VerUp":
                        return resultModel.VerUp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Timestamp":
                        return resultModel.Timestamp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                return resultModel.Class(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Num":
                                return resultModel.Num(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Date":
                                return resultModel.Date(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Description":
                                return resultModel.Description(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Check":
                                return resultModel.Check(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Attachments":
                                return resultModel.Attachments(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            default:
                                return string.Empty;
                        }
                }
            }
            return string.Empty;
        }
    }
}
