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
    public class LoginKeyModel : BaseModel
    {
        public string LoginId = string.Empty;
        public string Key = string.Empty;
        public string TenantNames = string.Empty;
        public int TenantId = 0;
        public int UserId = 0;
        public string SavedLoginId = string.Empty;
        public string SavedKey = string.Empty;
        public string SavedTenantNames = string.Empty;
        public int SavedTenantId = 0;
        public int SavedUserId = 0;
        public bool LoginId_Updated { get { return LoginId != SavedLoginId && LoginId != null; } }
        public bool Key_Updated { get { return Key != SavedKey && Key != null; } }
        public bool TenantNames_Updated { get { return TenantNames != SavedTenantNames && TenantNames != null; } }
        public bool TenantId_Updated { get { return TenantId != SavedTenantId; } }
        public bool UserId_Updated { get { return UserId != SavedUserId; } }

        public LoginKeyModel(DataRow dataRow, string tableAlias = null)
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

        public LoginKeyModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectLoginKeys(
                tableType: tableType,
                column: column ?? Rds.LoginKeysDefaultColumns(),
                join: join ??  Rds.LoginKeysJoinDefault(),
                where: where ?? Rds.LoginKeysWhereDefault(this),
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
                var column = new Column(tableAlias, dataColumn);
                var name = column.DataColumnName;
                switch (column.ColumnName)
                {
                    case "LoginId": if (dataRow[name] != DBNull.Value) { LoginId = dataRow[name].ToString(); SavedLoginId = LoginId; } break;
                    case "Key": if (dataRow[name] != DBNull.Value) { Key = dataRow[name].ToString(); SavedKey = Key; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "TenantNames": TenantNames = dataRow[name].ToString(); SavedTenantNames = TenantNames; break;
                    case "TenantId": TenantId = dataRow[name].ToInt(); SavedTenantId = TenantId; break;
                    case "UserId": UserId = dataRow[name].ToInt(); SavedUserId = UserId; break;
                    case "Comments": Comments = dataRow[name].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, name); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, name); Timestamp = dataRow.Field<DateTime>(name).ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        public bool Updated()
        {
            return
                LoginId_Updated ||
                Key_Updated ||
                Ver_Updated ||
                TenantNames_Updated ||
                TenantId_Updated ||
                UserId_Updated ||
                Comments_Updated ||
                Creator_Updated ||
                Updator_Updated ||
                CreatedTime_Updated ||
                UpdatedTime_Updated;
        }
    }
}
