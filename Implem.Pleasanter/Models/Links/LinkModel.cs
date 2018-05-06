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
    public class LinkModel : BaseModel
    {
        public long DestinationId = 0;
        public long SourceId = 0;
        public string ReferenceType = string.Empty;
        public long SiteId = 0;
        public string Title = string.Empty;
        public string Subset = string.Empty;
        public string SiteTitle = string.Empty;
        [NonSerialized] public long SavedDestinationId = 0;
        [NonSerialized] public long SavedSourceId = 0;
        [NonSerialized] public string SavedReferenceType = string.Empty;
        [NonSerialized] public long SavedSiteId = 0;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public string SavedSubset = string.Empty;
        [NonSerialized] public string SavedSiteTitle = string.Empty;

        public bool DestinationId_Updated(Column column = null)
        {
            return DestinationId != SavedDestinationId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != DestinationId);
        }

        public bool SourceId_Updated(Column column = null)
        {
            return SourceId != SavedSourceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != SourceId);
        }

        public LinkModel(DataRow dataRow, string tableAlias = null)
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

        public LinkModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectLinks(
                tableType: tableType,
                column: column ?? Rds.LinksDefaultColumns(),
                join: join ??  Rds.LinksJoinDefault(),
                where: where ?? Rds.LinksWhereDefault(this),
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
                        case "DestinationId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                DestinationId = dataRow[column.ColumnName].ToLong();
                                SavedDestinationId = DestinationId;
                            }
                            break;
                        case "SourceId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SourceId = dataRow[column.ColumnName].ToLong();
                                SavedSourceId = SourceId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "ReferenceType":
                            ReferenceType = dataRow[column.ColumnName].ToString();
                            SavedReferenceType = ReferenceType;
                            break;
                        case "SiteId":
                            SiteId = dataRow[column.ColumnName].ToLong();
                            SavedSiteId = SiteId;
                            break;
                        case "Title":
                            Title = dataRow[column.ColumnName].ToString();
                            SavedTitle = Title;
                            break;
                        case "Subset":
                            Subset = dataRow[column.ColumnName].ToString();
                            SavedSubset = Subset;
                            break;
                        case "SiteTitle":
                            SiteTitle = dataRow[column.ColumnName].ToString();
                            SavedSiteTitle = SiteTitle;
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
                DestinationId_Updated() ||
                SourceId_Updated() ||
                Ver_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }
    }
}
