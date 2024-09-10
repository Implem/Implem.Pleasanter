using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class GroupModel : BaseModel
    {
        public List<string> GroupMembers;
        public List<string> GroupChildren;
        public int TenantId = 0;
        public int GroupId = 0;
        public string GroupName = string.Empty;
        public string Body = string.Empty;
        public bool Disabled = false;
        public string MemberType = string.Empty;
        public string MemberKey = string.Empty;
        public string MemberName = string.Empty;
        public bool? MemberIsAdmin = null;
        public bool LdapSync = false;
        public string LdapGuid = string.Empty;
        public string LdapSearchRoot = string.Empty;
        public DateTime SynchronizedTime = 0.ToDateTime();

        public Title Title
        {
            get
            {
                return new Title(GroupId, GroupName);
            }
        }

        public int SavedTenantId = 0;
        public int SavedGroupId = 0;
        public string SavedGroupName = string.Empty;
        public string SavedBody = string.Empty;
        public bool SavedDisabled = false;
        public string SavedMemberType = string.Empty;
        public string SavedMemberKey = string.Empty;
        public string SavedMemberName = string.Empty;
        public bool? SavedMemberIsAdmin = null;
        public bool SavedLdapSync = false;
        public string SavedLdapGuid = string.Empty;
        public string SavedLdapSearchRoot = string.Empty;
        public DateTime SavedSynchronizedTime = 0.ToDateTime();

        public bool TenantId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != TenantId;
            }
            return TenantId != SavedTenantId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool GroupId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != GroupId;
            }
            return GroupId != SavedGroupId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != GroupId);
        }

        public bool GroupName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != GroupName;
            }
            return GroupName != SavedGroupName && GroupName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != GroupName);
        }

        public bool Body_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Body;
            }
            return Body != SavedBody && Body != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Body);
        }

        public bool Disabled_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != Disabled;
            }
            return Disabled != SavedDisabled
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != Disabled);
        }

        public bool LdapSync_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != LdapSync;
            }
            return LdapSync != SavedLdapSync
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != LdapSync);
        }

        public bool LdapGuid_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != LdapGuid;
            }
            return LdapGuid != SavedLdapGuid && LdapGuid != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != LdapGuid);
        }

        public bool LdapSearchRoot_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != LdapSearchRoot;
            }
            return LdapSearchRoot != SavedLdapSearchRoot && LdapSearchRoot != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != LdapSearchRoot);
        }

        public bool SynchronizedTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != SynchronizedTime;
            }
            return SynchronizedTime != SavedSynchronizedTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != SynchronizedTime.Date);
        }

        public string CsvData(
            Context context,
            SiteSettings ss,
            Column column,
            ExportColumn exportColumn,
            List<string> mine,
            bool? encloseDoubleQuotes)
        {
            var value = string.Empty;
            switch (column.Name)
            {
                case "TenantId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TenantId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "GroupId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? GroupId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Ver.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "GroupName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? GroupName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Body":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Body.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Disabled":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Disabled.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LdapSync":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LdapSync.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LdapGuid":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LdapGuid.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LdapSearchRoot":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LdapSearchRoot.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SynchronizedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SynchronizedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Comments.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Creator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Updator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? CreatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UpdatedTime?.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                                    ?? String.Empty
                            : string.Empty;
                    break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetClass(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Num":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetNum(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Date":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetDate(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Description":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetDescription(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Check":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetCheck(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Attachments":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetAttachments(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        default: return string.Empty;
                    }
                    break;
            }
            return CsvUtilities.EncloseDoubleQuotes(
                value: value,
                encloseDoubleQuotes: encloseDoubleQuotes);
        }

        public List<int> SwitchTargets;

        public GroupModel()
        {
        }

        public GroupModel(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData = null,
            GroupApiModel groupApiModel = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (groupApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: groupApiModel);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public GroupModel(
            Context context,
            SiteSettings ss,
            int groupId,
            Dictionary<string, string> formData = null,
            GroupApiModel groupApiModel = null,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            GroupId = groupId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.GroupsWhereDefault(
                        context: context,
                        groupModel: this)
                            .Groups_Ver(context.QueryStrings.Int("ver")), ss: ss);
            }
            else
            {
                Get(
                    context: context,
                    ss: ss,
                    column: column);
            }
            if (clearSessions) ClearSessions(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (groupApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: groupApiModel);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public GroupModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public GroupModel Get(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            where = where ?? Rds.GroupsWhereDefault(
                context: context,
                groupModel: this);
            column = (column ?? Rds.GroupsDefaultColumns());
            join = join ?? Rds.GroupsJoinDefault();
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectGroups(
                    tableType: tableType,
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public GroupApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new GroupApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(context: context, noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "TenantId": data.TenantId = TenantId; break;
                    case "GroupId": data.GroupId = GroupId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "GroupName": data.GroupName = GroupName; break;
                    case "Body": data.Body = Body; break;
                    case "Disabled": data.Disabled = Disabled; break;
                    case "LdapSync": data.LdapSync = LdapSync; break;
                    case "LdapGuid": data.LdapGuid = LdapGuid; break;
                    case "LdapSearchRoot": data.LdapSearchRoot = LdapSearchRoot; break;
                    case "SynchronizedTime": data.SynchronizedTime = SynchronizedTime.ToLocal(context: context); break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                    default: 
                        data.Value(
                            context: context,
                            column: column,
                            value: GetValue(
                                context: context,
                                column: column,
                                toLocal: true));
                        break;
                }
            });
            if (GroupMembers != null) { data.GroupMembers = GroupMembers; }
            if (GroupChildren != null) { data.GroupChildren = GroupChildren; }
            return data;
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "TenantId":
                    return TenantId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupId":
                    return GroupId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupName":
                    return GroupName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Disabled":
                    return Disabled.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSync":
                    return LdapSync.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapGuid":
                    return LdapGuid.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSearchRoot":
                    return LdapSearchRoot.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SynchronizedTime":
                    return SynchronizedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "TenantId":
                    return TenantId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupId":
                    return GroupId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupName":
                    return GroupName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Disabled":
                    return Disabled.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MemberType":
                    return MemberType.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MemberKey":
                    return MemberKey.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MemberName":
                    return MemberName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MemberIsAdmin":
                    return MemberIsAdmin.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSync":
                    return LdapSync.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapGuid":
                    return LdapGuid.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSearchRoot":
                    return LdapSearchRoot.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SynchronizedTime":
                    return SynchronizedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "TenantId":
                    return TenantId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupId":
                    return GroupId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "GroupName":
                    return GroupName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Disabled":
                    return Disabled.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MemberType":
                    return MemberType.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MemberKey":
                    return MemberKey.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MemberName":
                    return MemberName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MemberIsAdmin":
                    return MemberIsAdmin.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSync":
                    return LdapSync.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapGuid":
                    return LdapGuid.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSearchRoot":
                    return LdapSearchRoot.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SynchronizedTime":
                    return SynchronizedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public ErrorData Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            GroupApiModel groupApiModel = null,
            string noticeType = "Created",
            bool otherInitValue = false,
            bool get = true)
        {
            TenantId = context.TenantId;
            var statements = new List<SqlStatement>();
            var groupMembers = groupApiModel != null
                ? GroupMembers
                : context.Forms.List("CurrentMembersAll");
            var addMyselfGroupmembers = groupApiModel == null || groupMembers == null;
            var groupChildren = groupApiModel != null
                ? GroupChildren
                : context.Forms.List("CurrentChildrenAll");
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                groupMembersUsing: addMyselfGroupmembers,
                otherInitValue: otherInitValue));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            GroupId = (response.Id ?? GroupId).ToInt();
            groupMembers?.ForEach(data =>
            {
                if (data.StartsWith("Dept,"))
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.InsertGroupMembers(
                            param: Rds.GroupMembersParam()
                                .GroupId(GroupId)
                                .DeptId(data.Split_2nd().ToInt())
                                .Admin(data.Split_3rd().ToBool())));
                }
                if (data.StartsWith("User,"))
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.InsertGroupMembers(
                            param: Rds.GroupMembersParam()
                                .GroupId(GroupId)
                                .UserId(data.Split_2nd().ToInt())
                                .Admin(data.Split_3rd().ToBool())));
                }
            });
            groupChildren?.ForEach(data =>
            {
                if (data.StartsWith("Group,"))
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.InsertGroupChildren(
                            param: Rds.GroupChildrenParam()
                                .GroupId(GroupId)
                                .ChildId(data.Split_2nd().ToInt())));
                }
            });
            GroupMemberUtilities.SyncGroupMembers(
                context: context,
                groupId: GroupId);
            if (get) Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool groupMembersUsing = true,
            bool otherInitValue = false)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertGroups(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.GroupsParamDefault(
                        context: context,
                        ss: ss,
                        groupModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
                Rds.InsertGroupMembers(
                    tableType: tableType,
                    param: param ?? Rds.GroupMembersParam()
                        .GroupId(raw: Def.Sql.Identity)
                        .UserId(context.UserId)
                        .Admin(true),
                    _using: groupMembersUsing),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.GroupsUpdated),
            });
            return statements;
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            bool refleshSiteInfo = true,
            bool updateGroupMembers = true,
            bool updateGroupChildren = true,
            GroupApiModel groupApiModel = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true,
            bool checkConflict = true)
        {
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            var verUp = Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp);
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements,
                checkConflict: checkConflict,
                verUp: verUp));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: GroupId);
            }
            var disabledUpdated = Disabled_Updated(context: context);
            if (get)
            {
                Get(context: context, ss: ss);
            }
            if (updateGroupMembers)
            {
                if (groupApiModel != null)
                {
                    RenewGroupMembers(context, GroupMembers);
                }
                else
                {
                    UpdateGroupMembers(context);
                }
            }
            if (updateGroupChildren)
            {
                if (groupApiModel != null)
                {
                    RenewGroupChildren(context, GroupChildren);
                }
                else
                {
                    UpdateGroupChildren(context);
                }
            }
            if (updateGroupMembers || updateGroupChildren || disabledUpdated)
            {
                GroupMemberUtilities.SyncGroupMembers(
                    context: context,
                    groupId: GroupId);
            }        
            if (refleshSiteInfo)
            {
                SiteInfo.Reflesh(context: context);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null,
            bool checkConflict = true,
            bool verUp = false)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.GroupsWhereDefault(
                context: context,
                groupModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.GroupsCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateGroups(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.GroupsParamDefault(
                        context: context,
                        ss: ss,
                        groupModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(GroupId))
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = GroupId
                },
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.GroupsUpdated),
            };
        }

        private void UpdateGroupMembers(Context context)
        {
            var deletedMembers = ParseGroupMembers(members: context.Forms.List("DeletedGroupMembers"));
            var updateMembers = GetUpdateGroupMembers(context: context);
            if (deletedMembers?.Any() == true)
            {
                var userIdIn = deletedMembers.Where(o => o.UserId != 0).Select(o => o.UserId);
                var deptIdIn = deletedMembers.Where(o => o.DeptId != 0).Select(o => o.DeptId);
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: Rds.PhysicalDeleteGroupMembers(
                        where: Rds.GroupMembersWhere()
                            .GroupId(GroupId)
                            .ChildGroup(false)
                            .Or(Rds.GroupMembersWhere()
                                .UserId_In(userIdIn, _using: userIdIn.Any())
                                .DeptId_In(deptIdIn, _using: deptIdIn.Any()))));
            }
            updateMembers?.ForEach(data =>
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new SqlStatement(
                        commandText: Def.Sql.UpsertGroupMember,
                        param: new SqlParamCollection
                            {
                                { "GroupId", GroupId },
                                { "UserId", data.UserId },
                                { "DeptId", data.DeptId },
                                { "Admin", data.Admin }
                            }));
            });
        }

        public static IEnumerable<(int UserId, int DeptId, bool Admin)> GetUpdateGroupMembers(Context context)
        {
            var addedMembers = ParseGroupMembers(context.Forms.List("AddedGroupMembers")).ToList();
            var modifiedMembers = ParseGroupMembers(context.Forms.List("ModifiedGroupMembers"));
            modifiedMembers.ForEach(modified =>
            {
                var item = addedMembers.FirstOrDefault(added =>
                    added.DeptId == modified.DeptId
                    && added.UserId == modified.UserId);
                if (item != default)
                {
                    addedMembers.Remove(item);    
                }
                addedMembers.Add(modified);
            });
            return addedMembers;
        }

        private static IEnumerable<(int UserId, int DeptId, bool Admin)> ParseGroupMembers(List<string> members)
        {
            return members?.Select(o =>
            (
                o.StartsWith("User,")
                    ? o.Split_2nd().ToInt()
                    : 0,
                o.StartsWith("Dept,")
                    ? o.Split_2nd().ToInt()
                    : 0,
                o.Split_3rd().ToBool()
            ))?? Enumerable.Empty<(int UserId, int DeptId, bool Admin)>();
        }

        private void RenewGroupMembers(Context context, List<string> groupMembers)
        {
            if (groupMembers != null)
            {
                Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(GroupId)
                        .ChildGroup(false)));
            }
            groupMembers?.ForEach(data =>
            {
                if (data.StartsWith("Dept,"))
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.InsertGroupMembers(
                            param: Rds.GroupMembersParam()
                                .GroupId(GroupId)
                                .DeptId(data.Split_2nd().ToInt())
                                .Admin(data.Split_3rd().ToBool())));
                }
                if (data.StartsWith("User,"))
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.InsertGroupMembers(
                            param: Rds.GroupMembersParam()
                                .GroupId(GroupId)
                                .UserId(data.Split_2nd().ToInt())
                                .Admin(data.Split_3rd().ToBool())));
                }
            });
        }

        private void UpdateGroupChildren(Context context)
        {
            var deletedChildren = ParseGroupChildren(children: context.Forms.List("DeletedGroupChildren"));
            var updateChildren = GetUpdateGroupChildren(context: context);
            if (deletedChildren?.Any() == true)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: Rds.PhysicalDeleteGroupChildren(
                        where: Rds.GroupChildrenWhere()
                            .GroupId(GroupId)
                            .Or(Rds.GroupChildrenWhere()
                                .ChildId_In(deletedChildren, _using: deletedChildren.Any()))));
            }
            updateChildren?.ForEach(childId =>
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new SqlStatement(
                        commandText: Def.Sql.UpsertGroupChild,
                        param: new SqlParamCollection
                            {
                                { "GroupId", GroupId },
                                { "ChildId", childId }
                            }));
            });
        }

        public static IEnumerable<int> GetUpdateGroupChildren(Context context)
        {
            var addedChildren = ParseGroupChildren(context.Forms.List("AddedGroupChildren")).ToList();
            var modifiedChildren = ParseGroupChildren(context.Forms.List("ModifiedGroupChildren"));
            modifiedChildren.ForEach(modified =>
            {
                var item = addedChildren.FirstOrDefault(added => added == modified);
                if (item != default)
                {
                    addedChildren.Remove(item);    
                }
                addedChildren.Add(modified);
            });
            return addedChildren;
        }

        private static IEnumerable<int> ParseGroupChildren(List<string> children)
        {
            return children?.Select(o =>
            (
                o.StartsWith("Group,")
                    ? o.Split_2nd().ToInt()
                    : 0
            )) ?? Enumerable.Empty<int>();
        }

        private static List<string> ParseGroupChildren(string children)
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(children ?? "[]");
        }

        private void RenewGroupChildren(Context context, List<string> groupChildren)
        {
            if (groupChildren != null)
            {
                Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteGroupChildren(
                    where: Rds.GroupChildrenWhere()
                        .GroupId(GroupId)));
            }
            groupChildren?.ForEach(data =>
            {
                if (data.StartsWith("Group,"))
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.InsertGroupChildren(
                            param: Rds.GroupChildrenParam()
                                .GroupId(GroupId)
                                .ChildId(data.Split_2nd().ToInt())));
                }
            });
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertGroups(
                    where: where ?? Rds.GroupsWhereDefault(
                        context: context,
                        groupModel: this),
                    param: param ?? Rds.GroupsParamDefault(
                        context: context,
                        ss: ss,
                        groupModel: this,
                        setDefault: true)),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.GroupsUpdated),
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            GroupId = (response.Id ?? GroupId).ToInt();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.GroupsWhere().GroupId(GroupId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteBinaries(
                    factory: context,
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(GroupId)
                        .BinaryType(value: "TenantManagementImages")),
                Rds.DeleteGroupMembers(
                    factory: context,
                    where: Rds.GroupMembersWhere()
                        .GroupId(GroupId)),
                Rds.DeleteGroupChildren(
                    factory: context,
                    where: Rds.GroupChildrenWhere()
                        .GroupId(GroupId)),
                GroupMemberUtilities.DeleteGroupMembers(
                    context: context,
                    groupId: GroupId),
                Rds.DeleteGroups(
                    factory: context,
                    where: where),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.GroupsUpdated),
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,int groupId)
        {
            GroupId = groupId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreGroups(
                        factory: context,
                        where: Rds.GroupsWhere().GroupId(GroupId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.GroupsUpdated),
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteGroups(
                    tableType: tableType,
                    where: Rds.GroupsWhere().GroupId(GroupId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData)
        {
            SetByFormData(
                context: context,
                ss: ss,
                formData: formData);
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Ver = context.QueryStrings.Int("ver");
            }
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        private void SetByFormData(Context context, SiteSettings ss, Dictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Groups_TenantId": TenantId = value.ToInt(); break;
                    case "Groups_GroupName": GroupName = value.ToString(); break;
                    case "Groups_Body": Body = value.ToString(); break;
                    case "Groups_Disabled": Disabled = value.ToBool(); break;
                    case "Groups_LdapSync": LdapSync = value.ToBool(); break;
                    case "Groups_LdapGuid": LdapGuid = value.ToString(); break;
                    case "Groups_LdapSearchRoot": LdapSearchRoot = value.ToString(); break;
                    case "Groups_SynchronizedTime": SynchronizedTime = value.ToDateTime().ToUniversal(context: context); break;
                    case "Groups_Timestamp": Timestamp = value.ToString(); break;
                    case "Comments": Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: value); break;
                    case "VerUp": VerUp = value.ToBool(); break;
                    case "CurrentChildrenAll": GroupChildren = ParseGroupChildren(value.ToString()); break;
                    default:
                        if (key.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: key.Substring("Comment".Length).ToInt(),
                                body: value);
                        }
                        else
                        {
                            var column = ss.GetColumn(
                                context: context,
                                columnName: key.Split_2nd('_'));
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.ColumnName,
                                        value: new Num(
                                            context: context,
                                            column: column,
                                            value: value));
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
        }

        public void SetByModel(GroupModel groupModel)
        {
            TenantId = groupModel.TenantId;
            GroupName = groupModel.GroupName;
            Body = groupModel.Body;
            Disabled = groupModel.Disabled;
            MemberType = groupModel.MemberType;
            MemberKey = groupModel.MemberKey;
            MemberName = groupModel.MemberName;
            MemberIsAdmin = groupModel.MemberIsAdmin;
            LdapSync = groupModel.LdapSync;
            LdapGuid = groupModel.LdapGuid;
            LdapSearchRoot = groupModel.LdapSearchRoot;
            SynchronizedTime = groupModel.SynchronizedTime;
            Comments = groupModel.Comments;
            Creator = groupModel.Creator;
            Updator = groupModel.Updator;
            CreatedTime = groupModel.CreatedTime;
            UpdatedTime = groupModel.UpdatedTime;
            VerUp = groupModel.VerUp;
            Comments = groupModel.Comments;
            ClassHash = groupModel.ClassHash;
            NumHash = groupModel.NumHash;
            DateHash = groupModel.DateHash;
            DescriptionHash = groupModel.DescriptionHash;
            CheckHash = groupModel.CheckHash;
            AttachmentsHash = groupModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss, GroupApiModel data)
        {
            if (data.TenantId != null) TenantId = data.TenantId.ToInt().ToInt();
            if (data.GroupName != null) GroupName = data.GroupName.ToString().ToString();
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.Disabled != null) Disabled = data.Disabled.ToBool().ToBool();
            if (data.LdapSync != null) LdapSync = data.LdapSync.ToBool().ToBool();
            if (data.LdapGuid != null) LdapGuid = data.LdapGuid.ToString().ToString();
            if (data.LdapSearchRoot != null) LdapSearchRoot = data.LdapSearchRoot.ToString().ToString();
            if (data.SynchronizedTime != null) SynchronizedTime = data.SynchronizedTime.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.GroupMembers != null) GroupMembers = data.GroupMembers;
            if (data.GroupChildren != null) GroupChildren = data.GroupChildren;
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => SetClass(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => SetNum(
                columnName: o.Key,
                value: new Num(
                    context: context,
                    column: ss.GetColumn(
                        context: context,
                        columnName: o.Key),
                    value: o.Value.ToString())));
            data.DateHash?.ForEach(o => SetDate(
                columnName: o.Key,
                value: o.Value.ToDateTime().ToUniversal(context: context)));
            data.DescriptionHash?.ForEach(o => SetDescription(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash?.ForEach(o => SetCheck(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash?.ForEach(o =>
            {
                string columnName = o.Key;
                Attachments newAttachments = o.Value;
                Attachments oldAttachments;
                if (columnName == "Attachments#Uploading")
                {
                    var kvp = AttachmentsHash
                        .FirstOrDefault(x => x.Value
                            .Any(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st()));
                    columnName = kvp.Key;
                    oldAttachments = kvp.Value;
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    if (column.OverwriteSameFileName == true)
                    {
                        var oldAtt = oldAttachments
                            .FirstOrDefault(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st());
                        if (oldAtt != null)
                        {
                            oldAtt.Deleted = true;
                            oldAtt.Overwritten = true;
                        }
                    }
                    newAttachments.ForEach(att => att.Guid = att.Guid.Split_2nd());
                }
                else
                {
                    oldAttachments = AttachmentsHash.Get(columnName);
                }
                if (oldAttachments != null)
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    var newGuidSet = new HashSet<string>(newAttachments.Select(x => x.Guid).Distinct());
                    var newNameSet = new HashSet<string>(newAttachments.Select(x => x.Name).Distinct());
                    newAttachments.ForEach(newAttachment =>
                    {
                        newAttachment.AttachmentAction(
                            context: context,
                            column: column,
                            oldAttachments: oldAttachments);
                    });
                    if (column.OverwriteSameFileName == true)
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) =>
                                !newGuidSet.Contains(oldvalue.Guid) &&
                                !newNameSet.Contains(oldvalue.Name)));
                    }
                    else
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) => !newGuidSet.Contains(oldvalue.Guid)));
                    }
                }
                SetAttachments(columnName: columnName, value: newAttachments);
            });
        }

        public string ReplacedDisplayValues(
            Context context,
            SiteSettings ss,
            string value)
        {
            ss.IncludedColumns(value: value).ForEach(column =>
                value = value.Replace(
                    $"[{column.ColumnName}]",
                    ToDisplay(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: Mine(context: context))));
            value = ReplacedContextValues(context, value);
            return value;
        }

        private string ReplacedContextValues(Context context, string value)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: GroupId);
            var mailAddress = MailAddressUtilities.Get(
                context: context,
                userId: context.UserId);
            value = value
                .Replace("{Url}", url)
                .Replace("{LoginId}", context.User.LoginId)
                .Replace("{UserName}", context.User.Name)
                .Replace("{MailAddress}", mailAddress);
            return value;
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "TenantId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                TenantId = dataRow[column.ColumnName].ToInt();
                                SavedTenantId = TenantId;
                            }
                            break;
                        case "GroupId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                GroupId = dataRow[column.ColumnName].ToInt();
                                SavedGroupId = GroupId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "GroupName":
                            GroupName = dataRow[column.ColumnName].ToString();
                            SavedGroupName = GroupName;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "Disabled":
                            Disabled = dataRow[column.ColumnName].ToBool();
                            SavedDisabled = Disabled;
                            break;
                        case "LdapSync":
                            LdapSync = dataRow[column.ColumnName].ToBool();
                            SavedLdapSync = LdapSync;
                            break;
                        case "LdapGuid":
                            LdapGuid = dataRow[column.ColumnName].ToString();
                            SavedLdapGuid = LdapGuid;
                            break;
                        case "LdapSearchRoot":
                            LdapSearchRoot = dataRow[column.ColumnName].ToString();
                            SavedLdapSearchRoot = LdapSearchRoot;
                            break;
                        case "SynchronizedTime":
                            SynchronizedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedSynchronizedTime = SynchronizedTime;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedClass(
                                        columnName: column.Name,
                                        value: GetClass(columnName: column.Name));
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.Name,
                                        value: new Num(
                                            dataRow: dataRow,
                                            name: column.ColumnName));
                                    SetSavedNum(
                                        columnName: column.Name,
                                        value: GetNum(columnName: column.Name).Value);
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SetSavedDate(
                                        columnName: column.Name,
                                        value: GetDate(columnName: column.Name));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedDescription(
                                        columnName: column.Name,
                                        value: GetDescription(columnName: column.Name));
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SetSavedCheck(
                                        columnName: column.Name,
                                        value: GetCheck(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SetSavedAttachments(
                                        columnName: column.Name,
                                        value: GetAttachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || TenantId_Updated(context: context)
                || GroupId_Updated(context: context)
                || Ver_Updated(context: context)
                || GroupName_Updated(context: context)
                || Body_Updated(context: context)
                || Disabled_Updated(context: context)
                || LdapSync_Updated(context: context)
                || LdapGuid_Updated(context: context)
                || LdapSearchRoot_Updated(context: context)
                || SynchronizedTime_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)));
        }

        public bool Updated(Context context, SiteSettings ss)
        {
            return UpdatedWithColumn(context: context, ss: ss)
                || TenantId_Updated(context: context)
                || GroupId_Updated(context: context)
                || Ver_Updated(context: context)
                || GroupName_Updated(context: context)
                || Body_Updated(context: context)
                || Disabled_Updated(context: context)
                || LdapSync_Updated(context: context)
                || LdapGuid_Updated(context: context)
                || LdapSearchRoot_Updated(context: context)
                || SynchronizedTime_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public override List<string> Mine(Context context)
        {
            if (MineCache == null)
            {
                var mine = new List<string>();
                var userId = context.UserId;
                if (SavedCreator == userId) mine.Add("Creator");
                if (SavedUpdator == userId) mine.Add("Updator");
                MineCache = mine;
            }
            return MineCache;
        }
    }
}
