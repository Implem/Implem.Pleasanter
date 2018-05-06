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
    public class OrderModel : BaseModel
    {
        public long ReferenceId = 0;
        public string ReferenceType = string.Empty;
        public int OwnerId = 0;
        public List<long> Data = new List<long>();
        [NonSerialized] public long SavedReferenceId = 0;
        [NonSerialized] public string SavedReferenceType = string.Empty;
        [NonSerialized] public int SavedOwnerId = 0;
        [NonSerialized] public string SavedData = "[]";

        public bool ReferenceId_Updated(Column column = null)
        {
            return ReferenceId != SavedReferenceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != ReferenceId);
        }

        public bool ReferenceType_Updated(Column column = null)
        {
            return ReferenceType != SavedReferenceType && ReferenceType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ReferenceType);
        }

        public bool OwnerId_Updated(Column column = null)
        {
            return OwnerId != SavedOwnerId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != OwnerId);
        }

        public bool Data_Updated(Column column = null)
        {
            return Data.ToJson() != SavedData && Data.ToJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Data.ToJson());
        }

        public OrderModel(DataRow dataRow, string tableAlias = null)
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

        public OrderModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectOrders(
                tableType: tableType,
                column: column ?? Rds.OrdersDefaultColumns(),
                join: join ??  Rds.OrdersJoinDefault(),
                where: where ?? Rds.OrdersWhereDefault(this),
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
                        case "ReferenceId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceId = dataRow[column.ColumnName].ToLong();
                                SavedReferenceId = ReferenceId;
                            }
                            break;
                        case "ReferenceType":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceType = dataRow[column.ColumnName].ToString();
                                SavedReferenceType = ReferenceType;
                            }
                            break;
                        case "OwnerId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                OwnerId = dataRow[column.ColumnName].ToInt();
                                SavedOwnerId = OwnerId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Data":
                            Data = dataRow[column.ColumnName].ToString().Deserialize<List<long>>() ?? new List<long>();
                            SavedData = Data.ToJson();
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
                ReferenceId_Updated() ||
                ReferenceType_Updated() ||
                OwnerId_Updated() ||
                Ver_Updated() ||
                Data_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public OrderModel()
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public OrderModel(long referenceId, string referenceType)
        {
            ReferenceId = referenceId;
            ReferenceType = referenceType;
            OwnerId = referenceId == 0
                ? Sessions.UserId()
                : 0;
            Get();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertOrders(
                        selectIdentity: true,
                        where: where ?? Rds.OrdersWhereDefault(this),
                        param: param ?? Rds.OrdersParamDefault(this, setDefault: true),
                        tableType: tableType)
                });
            return new ResponseCollection().ToJson();
        }
    }
}
