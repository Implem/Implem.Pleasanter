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
        public string ReferenceType = "Sites";
        public long ReferenceId = 0;
        public int DeptId = 0;
        public int GroupId = 0;
        public int UserId = 0;
        public string DeptName = string.Empty;
        public string GroupName = string.Empty;
        public string FullName1 = string.Empty;
        public string FullName2 = string.Empty;
        public Names.FirstAndLastNameOrders FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)1;
        public Permissions.Types PermissionType = (Permissions.Types)31;
        public string SavedReferenceType = "Sites";
        public long SavedReferenceId = 0;
        public int SavedDeptId = 0;
        public int SavedGroupId = 0;
        public int SavedUserId = 0;
        public string SavedDeptName = string.Empty;
        public string SavedGroupName = string.Empty;
        public string SavedFullName1 = string.Empty;
        public string SavedFullName2 = string.Empty;
        public int SavedFirstAndLastNameOrder = 1;
        public long SavedPermissionType = 31;
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool DeptId_Updated { get { return DeptId != SavedDeptId; } }
        public bool GroupId_Updated { get { return GroupId != SavedGroupId; } }
        public bool UserId_Updated { get { return UserId != SavedUserId; } }
        public bool PermissionType_Updated { get { return PermissionType.ToLong() != SavedPermissionType; } }

        public PermissionModel(DataRow dataRow)
        {
            OnConstructing();
            Set(dataRow);
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

        private void Set(DataRow dataRow)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var name = dataColumn.ColumnName;
                switch(name)
                {
                    case "ReferenceType": if (dataRow[name] != DBNull.Value) { ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; } break;
                    case "ReferenceId": if (dataRow[name] != DBNull.Value) { ReferenceId = dataRow[name].ToLong(); SavedReferenceId = ReferenceId; } break;
                    case "DeptId": if (dataRow[name] != DBNull.Value) { DeptId = dataRow[name].ToInt(); SavedDeptId = DeptId; } break;
                    case "GroupId": if (dataRow[name] != DBNull.Value) { GroupId = dataRow[name].ToInt(); SavedGroupId = GroupId; } break;
                    case "UserId": if (dataRow[name] != DBNull.Value) { UserId = dataRow[name].ToInt(); SavedUserId = UserId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "DeptName": DeptName = dataRow[name].ToString(); SavedDeptName = DeptName; break;
                    case "GroupName": GroupName = dataRow[name].ToString(); SavedGroupName = GroupName; break;
                    case "FullName1": FullName1 = dataRow[name].ToString(); SavedFullName1 = FullName1; break;
                    case "FullName2": FullName2 = dataRow[name].ToString(); SavedFullName2 = FullName2; break;
                    case "FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)dataRow[name].ToInt(); SavedFirstAndLastNameOrder = FirstAndLastNameOrder.ToInt(); break;
                    case "PermissionType": PermissionType = (Permissions.Types)dataRow[name].ToLong(); SavedPermissionType = PermissionType.ToLong(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <param name="dataRow"></param>
        public PermissionModel(
            string referenceType,
            long referenceId,
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            PermissionType = permissionType;
            Set(dataRow);
            OnConstructed();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string PermissionId
        {
            get
            {
                if (DeptId != 0)
                {
                    return "Dept," + DeptId;
                }
                else if (GroupId != 0)
                {
                    return "Group," + GroupId;
                }
                else
                {
                    return "User," + UserId;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string PermissionTitle
        {
            get
            {
                if (DeptId != 0)
                {
                    return "[" + Displays.Depts() + " " + DeptId + "] " + DeptName;
                }
                else if (GroupId != 0)
                {
                    return "[" + Displays.Groups() + " " + GroupId + "] " + GroupName;
                }
                else
                {
                    return "[" + Displays.Users() + " " + UserId + "] " + 
                        Names.FullName(FirstAndLastNameOrder, FullName1, FullName2);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string PermissionTypeName
        {
            get
            {
                var permissionType = PermissionType.ToLong();
                return Parameters.Permissions.Pattern.ContainsValue(permissionType)
                    ? Displays.Get(Parameters.Permissions.Pattern.First(o =>
                        o.Value == permissionType).Key)
                    : Displays.Special();
            }
        }
    }
}
