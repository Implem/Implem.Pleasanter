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
            DeptModel deptModel)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: From);
            if (column?.CanRead(
                context: context,
                ss: ss,
                mine: deptModel.Mine(context: context)) == true)
            {
                switch (column.ColumnName)
                {
                    case "TenantId":
                        return deptModel.TenantId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "DeptId":
                        return deptModel.DeptId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Ver":
                        return deptModel.Ver.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "DeptCode":
                        return deptModel.DeptCode.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Dept":
                        return deptModel.Dept.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "DeptName":
                        return deptModel.DeptName.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Body":
                        return deptModel.Body.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Title":
                        return deptModel.Title.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Disabled":
                        return deptModel.Disabled.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Comments":
                        return deptModel.Comments.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Creator":
                        return deptModel.Creator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Updator":
                        return deptModel.Updator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "CreatedTime":
                        return deptModel.CreatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "UpdatedTime":
                        return deptModel.UpdatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "VerUp":
                        return deptModel.VerUp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Timestamp":
                        return deptModel.Timestamp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                return deptModel.Class(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Num":
                                return deptModel.Num(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Date":
                                return deptModel.Date(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Description":
                                return deptModel.Description(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Check":
                                return deptModel.Check(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Attachments":
                                return deptModel.Attachments(column: column).ToLookup(
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
            GroupModel groupModel)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: From);
            if (column?.CanRead(
                context: context,
                ss: ss,
                mine: groupModel.Mine(context: context)) == true)
            {
                switch (column.ColumnName)
                {
                    case "TenantId":
                        return groupModel.TenantId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "GroupId":
                        return groupModel.GroupId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Ver":
                        return groupModel.Ver.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "GroupName":
                        return groupModel.GroupName.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Body":
                        return groupModel.Body.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Title":
                        return groupModel.Title.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Disabled":
                        return groupModel.Disabled.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Comments":
                        return groupModel.Comments.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Creator":
                        return groupModel.Creator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Updator":
                        return groupModel.Updator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "CreatedTime":
                        return groupModel.CreatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "UpdatedTime":
                        return groupModel.UpdatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "VerUp":
                        return groupModel.VerUp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Timestamp":
                        return groupModel.Timestamp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                return groupModel.Class(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Num":
                                return groupModel.Num(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Date":
                                return groupModel.Date(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Description":
                                return groupModel.Description(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Check":
                                return groupModel.Check(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Attachments":
                                return groupModel.Attachments(column: column).ToLookup(
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
            UserModel userModel)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: From);
            if (column?.CanRead(
                context: context,
                ss: ss,
                mine: userModel.Mine(context: context)) == true)
            {
                switch (column.ColumnName)
                {
                    case "TenantId":
                        return userModel.TenantId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "UserId":
                        return userModel.UserId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Ver":
                        return userModel.Ver.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "LoginId":
                        return userModel.LoginId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Name":
                        return userModel.Name.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "UserCode":
                        return userModel.UserCode.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "RememberMe":
                        return userModel.RememberMe.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "LastName":
                        return userModel.LastName.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "FirstName":
                        return userModel.FirstName.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Birthday":
                        return userModel.Birthday.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Gender":
                        return userModel.Gender.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Language":
                        return userModel.Language.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "TimeZone":
                        return userModel.TimeZone.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "DeptCode":
                        return userModel.DeptCode.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "DeptId":
                        return userModel.DeptId.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Dept":
                        return userModel.Dept.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Theme":
                        return userModel.Theme.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Title":
                        return userModel.Title.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Body":
                        return userModel.Body.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "LastLoginTime":
                        return userModel.LastLoginTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "PasswordExpirationTime":
                        return userModel.PasswordExpirationTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "PasswordChangeTime":
                        return userModel.PasswordChangeTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "NumberOfLogins":
                        return userModel.NumberOfLogins.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "NumberOfDenial":
                        return userModel.NumberOfDenial.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "TenantManager":
                        return userModel.TenantManager.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "ServiceManager":
                        return userModel.ServiceManager.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "AllowCreationAtTopSite":
                        return userModel.AllowCreationAtTopSite.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "AllowGroupAdministration":
                        return userModel.AllowGroupAdministration.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Disabled":
                        return userModel.Disabled.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Lockout":
                        return userModel.Lockout.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "LockoutCounter":
                        return userModel.LockoutCounter.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Developer":
                        return userModel.Developer.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Comments":
                        return userModel.Comments.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Creator":
                        return userModel.Creator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Updator":
                        return userModel.Updator.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "CreatedTime":
                        return userModel.CreatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "UpdatedTime":
                        return userModel.UpdatedTime.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "VerUp":
                        return userModel.VerUp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    case "Timestamp":
                        return userModel.Timestamp.ToLookup(
                            context: context,
                            ss: ss,
                            column: column,
                            type: Type);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                return userModel.Class(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Num":
                                return userModel.Num(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Date":
                                return userModel.Date(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Description":
                                return userModel.Description(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Check":
                                return userModel.Check(column: column).ToLookup(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: Type);
                            case "Attachments":
                                return userModel.Attachments(column: column).ToLookup(
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
