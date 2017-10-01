using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
using System.Linq;
namespace Implem.Pleasanter.Models
{
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
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool DeptId_Updated { get { return DeptId != SavedDeptId; } }
        public bool GroupId_Updated { get { return GroupId != SavedGroupId; } }
        public bool UserId_Updated { get { return UserId != SavedUserId; } }
        public bool PermissionType_Updated { get { return PermissionType.ToLong() != SavedPermissionType; } }

        public PermissionModel(DataRow dataRow, string tableAlias = null)
        {
            OnConstructing();
            Set(dataRow, tableAlias);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        private void OnConstructed()
        {
        }

        public void ClearSessions()
        {
        }

        public PermissionModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectPermissions(
                tableType: tableType,
                column: column ?? Rds.PermissionsDefaultColumns(),
                join: join ??  Rds.PermissionsJoinDefault(),
                where: where ?? Rds.PermissionsWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        private void SetBySession()
        {
        }

        private void Set(DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(DataRow dataRow, string tableAlias = null)
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
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                ReferenceId_Updated ||
                DeptId_Updated ||
                GroupId_Updated ||
                UserId_Updated ||
                Ver_Updated ||
                PermissionType_Updated ||
                Comments_Updated ||
                Creator_Updated ||
                Updator_Updated ||
                CreatedTime_Updated ||
                UpdatedTime_Updated;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public PermissionModel(
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
                DeptName = SiteInfo.Dept(DeptId).Name;
            }
            if (groupId != 0)
            {
                GroupId = groupId;
                GroupName = new GroupModel(
                    SiteSettingsUtilities.GroupsSiteSettings(), GroupId).GroupName;
            }
            if (userId != 0)
            {
                UserId = userId;
                var user = SiteInfo.User(UserId);
                Name = user.Name;
            }
            PermissionType = permissionType;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <param name="dataRow"></param>
        public PermissionModel(
            long referenceId,
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            ReferenceId = referenceId;
            PermissionType = permissionType;
            Set(dataRow);
            OnConstructed();
        }
    }
}
