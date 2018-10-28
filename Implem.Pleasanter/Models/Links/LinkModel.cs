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

        public bool DestinationId_Updated(Context context, Column column = null)
        {
            return DestinationId != SavedDestinationId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != DestinationId);
        }

        public bool SourceId_Updated(Context context, Column column = null)
        {
            return SourceId != SavedSourceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != SourceId);
        }

        public LinkModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
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

        public LinkModel Get(
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
                statements: Rds.SelectLinks(
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

        public void SetByModel(LinkModel linkModel)
        {
            DestinationId = linkModel.DestinationId;
            SourceId = linkModel.SourceId;
            ReferenceType = linkModel.ReferenceType;
            SiteId = linkModel.SiteId;
            Title = linkModel.Title;
            Subset = linkModel.Subset;
            SiteTitle = linkModel.SiteTitle;
            Comments = linkModel.Comments;
            Creator = linkModel.Creator;
            Updator = linkModel.Updator;
            CreatedTime = linkModel.CreatedTime;
            UpdatedTime = linkModel.UpdatedTime;
            VerUp = linkModel.VerUp;
            Comments = linkModel.Comments;
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
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                DestinationId_Updated(context: context) ||
                SourceId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }
    }
}
