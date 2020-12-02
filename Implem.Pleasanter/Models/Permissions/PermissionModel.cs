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
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Models.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class PermissionModel : BaseModel
    {
        public long ReferenceId = 0;
        public int DeptId = 0;
        public int GroupId = 0;
        public int UserId = 0;
        public string DeptName = string.Empty;
        public string GroupName = string.Empty;
        public string Name = string.Empty;
        public Permissions.Types PermissionType = (Permissions.Types)31;
        public long SavedReferenceId = 0;
        public int SavedDeptId = 0;
        public int SavedGroupId = 0;
        public int SavedUserId = 0;
        public string SavedDeptName = string.Empty;
        public string SavedGroupName = string.Empty;
        public string SavedName = string.Empty;
        public long SavedPermissionType = 31;

        public bool ReferenceId_Updated(Context context, Column column = null)
        {
            return ReferenceId != SavedReferenceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != ReferenceId);
        }

        public bool DeptId_Updated(Context context, Column column = null)
        {
            return DeptId != SavedDeptId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != DeptId);
        }

        public bool GroupId_Updated(Context context, Column column = null)
        {
            return GroupId != SavedGroupId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != GroupId);
        }

        public bool UserId_Updated(Context context, Column column = null)
        {
            return UserId != SavedUserId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != UserId);
        }

        public bool PermissionType_Updated(Context context, Column column = null)
        {
            return PermissionType.ToLong() != SavedPermissionType &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != PermissionType.ToLong());
        }

        public PermissionModel(
            Context context,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
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

        public PermissionModel Get(
            Context context,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            where = where ?? Rds.PermissionsWhereDefault(this);
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectPermissions(
                    tableType: tableType,
                    column: column ?? Rds.PermissionsDefaultColumns(),
                    join: join ??  Rds.PermissionsJoinDefault(),
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public void SetByModel(PermissionModel permissionModel)
        {
            ReferenceId = permissionModel.ReferenceId;
            DeptId = permissionModel.DeptId;
            GroupId = permissionModel.GroupId;
            UserId = permissionModel.UserId;
            DeptName = permissionModel.DeptName;
            GroupName = permissionModel.GroupName;
            Name = permissionModel.Name;
            PermissionType = permissionModel.PermissionType;
            Comments = permissionModel.Comments;
            Creator = permissionModel.Creator;
            Updator = permissionModel.Updator;
            CreatedTime = permissionModel.CreatedTime;
            UpdatedTime = permissionModel.UpdatedTime;
            VerUp = permissionModel.VerUp;
            Comments = permissionModel.Comments;
            ClassHash = permissionModel.ClassHash;
            NumHash = permissionModel.NumHash;
            DateHash = permissionModel.DateHash;
            DescriptionHash = permissionModel.DescriptionHash;
            CheckHash = permissionModel.CheckHash;
            AttachmentsHash = permissionModel.AttachmentsHash;
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "ReferenceId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceId = dataRow[column.ColumnName].ToLong();
                                SavedReferenceId = ReferenceId;
                            }
                            break;
                        case "DeptId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                DeptId = dataRow[column.ColumnName].ToInt();
                                SavedDeptId = DeptId;
                            }
                            break;
                        case "GroupId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                GroupId = dataRow[column.ColumnName].ToInt();
                                SavedGroupId = GroupId;
                            }
                            break;
                        case "UserId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                UserId = dataRow[column.ColumnName].ToInt();
                                SavedUserId = UserId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "DeptName":
                            DeptName = dataRow[column.ColumnName].ToString();
                            SavedDeptName = DeptName;
                            break;
                        case "GroupName":
                            GroupName = dataRow[column.ColumnName].ToString();
                            SavedGroupName = GroupName;
                            break;
                        case "Name":
                            Name = dataRow[column.ColumnName].ToString();
                            SavedName = Name;
                            break;
                        case "PermissionType":
                            PermissionType = (Permissions.Types)dataRow[column.ColumnName].ToLong();
                            SavedPermissionType = PermissionType.ToLong();
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
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedClass(
                                        columnName: column.Name,
                                        value: Class(columnName: column.Name));
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDecimal());
                                    SavedNum(
                                        columnName: column.Name,
                                        value: Num(columnName: column.Name));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SavedDate(
                                        columnName: column.Name,
                                        value: Date(columnName: column.Name));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedDescription(
                                        columnName: column.Name,
                                        value: Description(columnName: column.Name));
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SavedCheck(
                                        columnName: column.Name,
                                        value: Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SavedAttachments(
                                        columnName: column.Name,
                                        value: Attachments(columnName: column.Name).ToJson());
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
                || ReferenceId_Updated(context: context)
                || DeptId_Updated(context: context)
                || GroupId_Updated(context: context)
                || UserId_Updated(context: context)
                || Ver_Updated(context: context)
                || PermissionType_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public PermissionModel(
            Context context,
            long referenceId,
            int deptId,
            int groupId,
            int userId,
            Permissions.Types permissionType)
        {
            ReferenceId = referenceId;
            if (deptId != 0)
            {
                DeptId = deptId;
                DeptName = SiteInfo.Dept(
                    tenantId: context.TenantId,
                    deptId: DeptId)?
                        .Name;
            }
            if (groupId != 0)
            {
                GroupId = groupId;
                GroupName = new GroupModel(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                    groupId: GroupId)?
                        .GroupName;
            }
            if (userId != 0)
            {
                UserId = userId;
                var user = SiteInfo.User(
                    context: context,
                    userId: UserId);
                Name = user?.Name;
            }
            PermissionType = permissionType;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <param name="dataRow"></param>
        public PermissionModel(
            Context context,
            long referenceId,
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing(context: context);
            ReferenceId = referenceId;
            PermissionType = permissionType;
            Set(context: context, dataRow: dataRow);
            OnConstructed(context: context);
        }
    }
}
