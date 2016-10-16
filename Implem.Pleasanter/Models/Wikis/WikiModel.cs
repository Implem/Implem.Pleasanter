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
    public class WikiModel : BaseItemModel
    {
        public long Id { get { return WikiId; } }
        public override long UrlId { get { return WikiId; } }
        public long WikiId = 0;
        public TitleBody TitleBody { get { return new TitleBody(WikiId, Title.Value, Title.DisplayValue, Body); } }
        public long SavedWikiId = 0;

        public string PropertyValue(string name)
        {
            switch (name)
            {
                case "SiteId": return SiteId.ToString();
                case "UpdatedTime": return UpdatedTime.Value.ToString();
                case "WikiId": return WikiId.ToString();
                case "Ver": return Ver.ToString();
                case "Title": return Title.Value;
                case "Body": return Body;
                case "TitleBody": return TitleBody.ToString();
                case "Comments": return Comments.ToJson();
                case "Creator": return Creator.Id.ToString();
                case "Updator": return Updator.Id.ToString();
                case "CreatedTime": return CreatedTime.Value.ToString();
                case "VerUp": return VerUp.ToString();
                case "Timestamp": return Timestamp;
                default: return null;
            }
        }

        public WikiModel(SiteSettings siteSettings)
        {
            SiteSettings = siteSettings;
        }

        public WikiModel(long wikiId, long siteId, bool setByForm = false)
        {
            WikiId = wikiId;
            SiteId = siteId;
            if (setByForm) SetByForm();
            Get();
        }

        public WikiModel()
        {
        }

        public WikiModel(
            SiteSettings siteSettings, 
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteId = siteSettings.SiteId;
            SiteSettings = siteSettings;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public WikiModel(
            SiteSettings siteSettings, 
            long wikiId,
            bool clearSessions = false,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            WikiId = wikiId;
            SiteId = siteSettings.SiteId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public WikiModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = siteSettings;
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

        public WikiModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectWikis(
                tableType: tableType,
                column: column ?? Rds.WikisEditorColumns(SiteSettings),
                join: join ??  Rds.WikisJoinDefault(),
                where: where ?? Rds.WikisWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public Error.Types Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Wikis")
                            .SiteId(SiteId)
                            .Title(Title.Value)),
                    Rds.InsertWikis(
                        tableType: tableType,
                        param: param ?? Rds.WikisParamDefault(
                            this, setDefault: true, paramAll: paramAll)),
                    InsertLinks(SiteSettings, selectIdentity: true),
                });
            WikiId = newId != 0 ? newId : WikiId;
            SynchronizeSummary();
            Notice("Created");
            Get();
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(WikiUtilities.TitleDisplayValue(SiteSettings, this))
                        .Subset(Jsons.ToJson(new WikiSubset(this, SiteSettings))),
                    where: Rds.ItemsWhere().ReferenceId(WikiId)));
            return Error.Types.None;
        }

        public Error.Types Update(SqlParamCollection param = null, bool paramAll = false)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateWikis(
                        verUp: VerUp,
                        where: Rds.WikisWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: param ?? Rds.WikisParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return Error.Types.UpdateConflicts;
            SynchronizeSummary();
            Notice("Updated");
            Get();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                    where: Rds.ItemsWhere().ReferenceId(WikiId),
                    param: Rds.ItemsParam()
                        .SiteId(SiteId)
                        .Title(WikiUtilities.TitleDisplayValue(SiteSettings, this))
                        .Subset(Jsons.ToJson(new WikiSubset(this, SiteSettings)))
                        .MaintenanceTarget(true)),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(WikiId)),
                    InsertLinks(SiteSettings),
                    Rds.UpdateSites(
                        where: Rds.ItemsWhere().SiteId(SiteId),
                        param: Rds.ItemsParam().Title(Title.Value))
                });
            SiteInfo.SiteMenu.Set(SiteId);
            return Error.Types.None;
        }

        private SqlInsert InsertLinks(
            SiteSettings siteSettings, bool selectIdentity = false)
        {
            var link = new Dictionary<long, long>();
            siteSettings.ColumnCollection.Where(o => o.Link.ToBool()).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    default: break;
                }
            });
            return LinkUtilities.Insert(link, selectIdentity);
        }

        public Error.Types UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertItems(
                        selectIdentity: true,
                        param: Rds.ItemsParam()
                            .ReferenceType("Wikis")
                            .SiteId(SiteId)
                            .Title(Title.Value)),
                    Rds.UpdateOrInsertWikis(
                        selectIdentity: true,
                        where: where ?? Rds.WikisWhereDefault(this),
                        param: param ?? Rds.WikisParamDefault(this, setDefault: true))
                });
            WikiId = newId != 0 ? newId : WikiId;
            Get();
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types Delete()
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().ReferenceId(WikiId)),
                    Rds.DeleteWikis(
                        where: Rds.WikisWhere().SiteId(SiteId).WikiId(WikiId)),
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().ReferenceId(SiteId)),
                    Rds.DeleteSites(
                        where: Rds.SitesWhere().SiteId(SiteId))
                });
            return Error.Types.None;
        }

        public Error.Types Restore(long wikiId)
        {
            WikiId = wikiId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(WikiId)),
                    Rds.RestoreWikis(
                        where: Rds.WikisWhere().WikiId(WikiId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteWikis(
                    tableType: tableType,
                    param: Rds.WikisParam().SiteId(SiteId).WikiId(WikiId)));
            return Error.Types.None;
        }

        private void SynchronizeSummary()
        {
            SiteSettings.SummaryCollection.ForEach(summary =>
            {
                var id = SynchronizeSummaryDestinationId(summary.LinkColumn);
                var savedId = SynchronizeSummaryDestinationId(summary.LinkColumn, saved: true);
                if (id != 0)
                {
                    SynchronizeSummary(summary, id);
                }
                if (savedId != 0 && id != savedId)
                {
                    SynchronizeSummary(summary, savedId);
                }
            });
        }

        private void SynchronizeSummary(Summary summary, long id)
        {
            Summaries.Synchronize(
                summary.SiteId,
                summary.DestinationReferenceType,
                summary.DestinationColumn,
                SiteId,
                "Wikis",
                summary.LinkColumn,
                summary.Type,
                summary.SourceColumn,
                id);
            FormulaUtilities.Update(id);
        }

        private long SynchronizeSummaryDestinationId(string linkColumn, bool saved = false)
        {
            switch (linkColumn)
            {
                default: return 0;
            }
        }

        public void UpdateFormulaColumns()
        {
            SetByFormula();
            var param = Rds.WikisParam();
            SiteSettings.FormulaHash.Keys.ForEach(columnName =>
            {
                switch (columnName)
                {
                    default: break;
                }
            });
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateWikis(
                    param: param,
                    where: Rds.WikisWhereDefault(this),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Wikis_Title": Title = new Title(WikiId, Forms.Data(controlId)); break;
                    case "Wikis_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Wikis_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default: break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.ControlId().Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    default: break;
                }
            });
            SetByFormula();
        }

        private void SetByFormula()
        {
            if (SiteSettings.FormulaHash?.Count > 0)
            {
                var data = new Dictionary<string, decimal>
                {
                };
                SiteSettings.FormulaHash.Keys.ForEach(columnName =>
                {
                    switch (columnName)
                    {
                        default: break;
                    }
                });
            }
        }

        private void Notice(string type)
        {
            var title = WikiUtilities.TitleDisplayValue(SiteSettings, this);
            switch (type)
            {
                case "Created":
                    SiteSettings.Notifications.ForEach(notification =>
                        notification.Send(
                            Displays.Created(title).ToString(),
                            NoticeBody(notification)));
                    break;
                case "Updated":
                    SiteSettings.Notifications.ForEach(notification =>
                    {
                        var body = NoticeBody(notification, update: true);
                        if (body.Length > 0)
                        {
                            notification.Send(
                                Displays.Updated(title).ToString(),
                                body);
                        }
                    });
                    break;
                case "Deleted":
                    SiteSettings.Notifications.ForEach(notification =>
                        notification.Send(
                            Displays.Deleted(title).ToString(),
                            NoticeBody(notification)));
                    break;
            }
        }

        private string NoticeBody(Notification notification, bool update = false)
        {
            var body = new System.Text.StringBuilder();
            notification.MonitorChangesColumnCollection(SiteSettings).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "Title": body.Append(Title.ToNotice(SavedTitle, column, Title_Updated, update)); break;
                    case "Body": body.Append(Body.ToNotice(SavedBody, column, Body_Updated, update)); break;
                    case "Comments": body.Append(Comments.ToNotice(SavedComments, column, Comments_Updated, update)); break;
                    case "Creator": body.Append(Creator.ToNotice(SavedCreator, column, Creator_Updated, update)); break;
                    case "Updator": body.Append(Updator.ToNotice(SavedUpdator, column, Updator_Updated, update)); break;
                    case "CreatedTime": body.Append(CreatedTime.ToNotice(SavedCreatedTime, column, CreatedTime_Updated, update)); break;
                }
            });
            return body.ToString();
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
                    case "SiteId": if (dataRow[name] != DBNull.Value) { SiteId = dataRow[name].ToLong(); SavedSiteId = SiteId; } break;
                    case "UpdatedTime": if (dataRow[name] != DBNull.Value) { UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; } break;
                    case "WikiId": if (dataRow[name] != DBNull.Value) { WikiId = dataRow[name].ToLong(); SavedWikiId = WikiId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "Title": Title = new Title(dataRow, "WikiId"); SavedTitle = Title.Value; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
            if (SiteSettings != null)
            {
                Title.DisplayValue = WikiUtilities.TitleDisplayValue(SiteSettings, this);
            }
        }
    }
}
