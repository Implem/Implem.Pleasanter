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

        public bool LoginId_Updated(Context context, Column column = null)
        {
            return LoginId != SavedLoginId && LoginId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != LoginId);
        }

        public bool Key_Updated(Context context, Column column = null)
        {
            return Key != SavedKey && Key != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Key);
        }

        public bool TenantNames_Updated(Context context, Column column = null)
        {
            return TenantNames != SavedTenantNames && TenantNames != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != TenantNames);
        }

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool UserId_Updated(Context context, Column column = null)
        {
            return UserId != SavedUserId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != UserId);
        }

        public LoginKeyModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (dataRow != null) Set(context, dataRow, tableAlias);
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

        public LoginKeyModel Get(
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
            Set(context, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectLoginKeys(
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

        public void SetByModel(LoginKeyModel loginKeyModel)
        {
            LoginId = loginKeyModel.LoginId;
            Key = loginKeyModel.Key;
            TenantNames = loginKeyModel.TenantNames;
            TenantId = loginKeyModel.TenantId;
            UserId = loginKeyModel.UserId;
            Comments = loginKeyModel.Comments;
            Creator = loginKeyModel.Creator;
            Updator = loginKeyModel.Updator;
            CreatedTime = loginKeyModel.CreatedTime;
            UpdatedTime = loginKeyModel.UpdatedTime;
            VerUp = loginKeyModel.VerUp;
            Comments = loginKeyModel.Comments;
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
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
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

        public bool Updated(Context context)
        {
            return
                LoginId_Updated(context: context) ||
                Key_Updated(context: context) ||
                Ver_Updated(context: context) ||
                TenantNames_Updated(context: context) ||
                TenantId_Updated(context: context) ||
                UserId_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }
    }
}
