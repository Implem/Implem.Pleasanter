using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
    public class LinkModel : BaseModel
    {
        public long DestinationId = 0;
        public long SourceId = 0;
        public string ReferenceType = string.Empty;
        public long SiteId = 0;
        public string Title = string.Empty;
        public string Subset = string.Empty;
        public string SiteTitle = string.Empty;
        public long SavedDestinationId = 0;
        public long SavedSourceId = 0;
        public string SavedReferenceType = string.Empty;
        public long SavedSiteId = 0;
        public string SavedTitle = string.Empty;
        public string SavedSubset = string.Empty;
        public string SavedSiteTitle = string.Empty;
        public bool DestinationId_Updated { get { return DestinationId != SavedDestinationId; } }
        public bool SourceId_Updated { get { return SourceId != SavedSourceId; } }

        public LinkModel(
            DataRow dataRow)
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
                column: column ?? Rds.EditorLinksColumn(SiteSettings),
                join: join ??  Rds.LinksJoinDefault(),
                where: where ?? Rds.LinksWhereDefault(this),
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
                    case "DestinationId": if (dataRow[name] != DBNull.Value) { DestinationId = dataRow[name].ToLong(); SavedDestinationId = DestinationId; } break;
                    case "SourceId": if (dataRow[name] != DBNull.Value) { SourceId = dataRow[name].ToLong(); SavedSourceId = SourceId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "ReferenceType": ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; break;
                    case "SiteId": SiteId = dataRow[name].ToLong(); SavedSiteId = SiteId; break;
                    case "Title": Title = dataRow[name].ToString(); SavedTitle = Title; break;
                    case "Subset": Subset = dataRow[name].ToString(); SavedSubset = Subset; break;
                    case "SiteTitle": SiteTitle = dataRow[name].ToString(); SavedSiteTitle = SiteTitle; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }
    }
}
