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
using System.Linq;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class LoginKeyModel : BaseModel
    {
        public string LoginId = string.Empty;
        public string Key = string.Empty;
        public string TenantNames = string.Empty;
        public int TenantId = 0;
        public int UserId = 0;
        [NonSerialized] public string SavedLoginId = string.Empty;
        [NonSerialized] public string SavedKey = string.Empty;
        [NonSerialized] public string SavedTenantNames = string.Empty;
        [NonSerialized] public int SavedTenantId = 0;
        [NonSerialized] public int SavedUserId = 0;

        public bool LoginId_Updated(Column column = null)
        {
            return LoginId != SavedLoginId && LoginId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != LoginId);
        }

        public bool Key_Updated(Column column = null)
        {
            return Key != SavedKey && Key != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Key);
        }

        public bool TenantNames_Updated(Column column = null)
        {
            return TenantNames != SavedTenantNames && TenantNames != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != TenantNames);
        }

        public bool TenantId_Updated(Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantId);
        }

        public bool UserId_Updated(Column column = null)
        {
            return UserId != SavedUserId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != UserId);
        }

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
                orderBy: orderBy,
                param: param,
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
                        case "LoginId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                LoginId = dataRow[column.ColumnName].ToString();
                                SavedLoginId = LoginId;
                            }
                            break;
                        case "Key":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                Key = dataRow[column.ColumnName].ToString();
                                SavedKey = Key;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "TenantNames":
                            TenantNames = dataRow[column.ColumnName].ToString();
                            SavedTenantNames = TenantNames;
                            break;
                        case "TenantId":
                            TenantId = dataRow[column.ColumnName].ToInt();
                            SavedTenantId = TenantId;
                            break;
                        case "UserId":
                            UserId = dataRow[column.ColumnName].ToInt();
                            SavedUserId = UserId;
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
                LoginId_Updated() ||
                Key_Updated() ||
                Ver_Updated() ||
                TenantNames_Updated() ||
                TenantId_Updated() ||
                UserId_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }
    }
}
