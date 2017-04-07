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

        public WikiModel(
            SiteSettings ss, 
            long wikiId,
            long siteId,
            bool setByForm = false)
        {
            WikiId = wikiId;
            SiteId = siteId;
            if (setByForm) SetByForm(ss);
            Get(ss);
        }

        public WikiModel()
        {
        }

        public WikiModel(
            SiteSettings ss, 
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteId = ss.SiteId;
            if (setByForm) SetByForm(ss);
            MethodType = methodType;
            OnConstructed();
        }

        public WikiModel(
            SiteSettings ss, 
            long wikiId,
            bool clearSessions = false,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            WikiId = wikiId;
            SiteId = ss.SiteId;
            Get(ss);
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm(ss);
            MethodType = methodType;
            OnConstructed();
        }

        public WikiModel(SiteSettings ss, DataRow dataRow)
        {
            OnConstructing();
            Set(ss, dataRow);
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
            SiteSettings ss, 
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(ss, Rds.ExecuteTable(statements: Rds.SelectWikis(
                tableType: tableType,
                column: column ?? Rds.WikisEditorColumns(ss),
                join: join ??  Rds.WikisJoinDefault(),
                where: where ?? Rds.WikisWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public Dictionary<string, int> SearchIndexHash()
        {
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            else
            {
                var searchIndexHash = new Dictionary<string, int>();
                SiteInfo.SiteMenu.Breadcrumb(SiteId).SearchIndexes(searchIndexHash, 100);
                SiteId.SearchIndexes(searchIndexHash, 200);
                UpdatedTime.SearchIndexes(searchIndexHash, 200);
                WikiId.SearchIndexes(searchIndexHash, 1);
                Title.SearchIndexes(searchIndexHash, 4);
                Body.SearchIndexes(searchIndexHash, 200);
                Comments.SearchIndexes(searchIndexHash, 200);
                Creator.SearchIndexes(searchIndexHash, 100);
                Updator.SearchIndexes(searchIndexHash, 100);
                CreatedTime.SearchIndexes(searchIndexHash, 200);
                SearchIndexExtensions.OutgoingMailsSearchIndexes(
                    searchIndexHash, "Wikis", WikiId);
                return searchIndexHash;
            }
        }

        public Error.Types Create(
            SiteSettings ss, 
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool notice = false,
            bool paramAll = false)
        {
            var statements = new List<SqlStatement>
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
                    InsertLinks(ss, selectIdentity: true),
            };
            var newId = Rds.ExecuteScalar_long(
                transactional: true, statements: statements.ToArray());
            WikiId = newId != 0 ? newId : WikiId;
            if (notice)
            {
                CheckNotificationConditions(ss);
                Notice(ss, "Created");
            }
            Get(ss);
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(WikiUtilities.TitleDisplayValue(ss, this)),
                    where: Rds.ItemsWhere().ReferenceId(WikiId)));
            Libraries.Search.Indexes.Create(ss, WikiId);
            return Error.Types.None;
        }

        public Error.Types Update(
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            bool paramAll = false)
        {
            if (notice) CheckNotificationConditions(ss, before: true);
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>
            {
                Rds.UpdateWikis(
                    verUp: VerUp,
                    where: Rds.WikisWhereDefault(this)
                        .UpdatedTime(timestamp, _using: timestamp.InRange()),
                    param: Rds.WikisParamDefault(this, paramAll: paramAll),
                    countRecord: true)
            };
            if (permissionChanged)
            {
                statements.UpdatePermissions(ss, WikiId, permissions);
            }
            var count = Rds.ExecuteScalar_int(
                transactional: true, statements: statements.ToArray());
            if (count == 0) return Error.Types.UpdateConflicts;
            if (notice)
            {
                CheckNotificationConditions(ss);
                Notice(ss, "Updated");
            }
            Get(ss);
            UpdateRelatedRecords(ss);
            SiteInfo.Reflesh();
            return Error.Types.None;
        }

        public void UpdateRelatedRecords(
            SiteSettings ss, 
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(WikiId),
                        param: Rds.ItemsParam()
                            .SiteId(SiteId)
                            .Title(WikiUtilities.TitleDisplayValue(ss, this)),
                        addUpdatedTimeParam: addUpdatedTimeParam,
                        addUpdatorParam: addUpdatorParam,
                        _using: updateItems),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(WikiId)),
                    InsertLinks(ss),
                    Rds.UpdateSites(
                        where: Rds.SitesWhere().SiteId(SiteId),
                        param: Rds.SitesParam().Title(Title.Value))
                });
            Libraries.Search.Indexes.Create(ss, WikiId);
        }

        private SqlInsert InsertLinks(SiteSettings ss, bool selectIdentity = false)
        {
            var link = new Dictionary<long, long>();
            ss.Columns.Where(o => o.Link.ToBool()).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    default: break;
                }
            });
            return LinkUtilities.Insert(link, selectIdentity);
        }

        public Error.Types UpdateOrCreate(
            SiteSettings ss, 
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var statements = new List<SqlStatement>
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
            };
            var newId = Rds.ExecuteScalar_long(
                transactional: true, statements: statements.ToArray());
            WikiId = newId != 0 ? newId : WikiId;
            Get(ss);
            Libraries.Search.Indexes.Create(ss, WikiId);
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types Delete(SiteSettings ss, bool notice = false)
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
            if (notice) Notice(ss, "Deleted");
            return Error.Types.None;
        }

        public Error.Types Restore(SiteSettings ss, long wikiId)
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
            Libraries.Search.Indexes.Create(ss, WikiId);
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            SiteSettings ss, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteWikis(
                    tableType: tableType,
                    param: Rds.WikisParam().SiteId(SiteId).WikiId(WikiId)));
            Libraries.Search.Indexes.Create(ss, WikiId);
            return Error.Types.None;
        }

        public void SetByForm(SiteSettings ss)
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
            SetByFormula(ss);
        }

        public void UpdateFormulaColumns(SiteSettings ss, IEnumerable<int> selected = null)
        {
            SetByFormula(ss);
            var param = Rds.WikisParam();
            ss.Formulas?
                .Where(o => selected == null || selected.Contains(o.Id))
                .ForEach(formulaSet =>
                {
                    switch (formulaSet.Target)
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

        private void SetByFormula(SiteSettings ss)
        {
            ss.Formulas?.ForEach(formulaSet =>
            {
                var columnName = formulaSet.Target;
                var formula = formulaSet.Formula;
                var view = ss.Views?.Get(formulaSet.Condition);
                if (view != null && !Matched(ss, view))
                {
                    if (formulaSet.OutOfCondition != null)
                    {
                        formula = formulaSet.OutOfCondition;
                    }
                    else
                    {
                        return;
                    }
                }
                var data = new Dictionary<string, decimal>
                {
                };
                switch (columnName)
                {
                    default: break;
                }
            });
        }

        private bool Matched(SiteSettings ss, View view)
        {
            if (view.ColumnFilterHash != null)
            {
                foreach (var filter in view.ColumnFilterHash)
                {
                    var match = true;
                    var column = ss.GetColumn(filter.Key);
                    switch (filter.Key)
                    {
                        case "UpdatedTime": match = UpdatedTime.Value.Matched(column, filter.Value); break;
                        case "CreatedTime": match = CreatedTime.Value.Matched(column, filter.Value); break;
                    }
                    if (!match) return false;
                }
            }
            return true;
        }

        private void CheckNotificationConditions(SiteSettings ss, bool before = false)
        {
            if (ss.Notifications.Any())
            {
                ss.EnableNotifications(
                    before: before,
                    dataSet: Rds.ExecuteDataSet(statements:
                        ss.Notifications.Select((o, i) =>
                            Rds.SelectWikis(
                                column: Rds.WikisColumn().WikiId(),
                                where: ss.Views?.Get(before
                                    ? o.BeforeCondition
                                    : o.AfterCondition)?
                                        .Where(
                                            ss,
                                            Rds.WikisWhere().WikiId(WikiId)) ??
                                                Rds.WikisWhere().WikiId(WikiId)))
                                                    .ToArray()));
            }
        }

        private void Notice(SiteSettings ss, string type)
        {
            var title = WikiUtilities.TitleDisplayValue(ss, this);
            var url = Url.AbsoluteUri().Replace(
                Url.AbsolutePath(), Locations.ItemEdit(WikiId));
            ss.Notifications.Where(o => o.Enabled).ForEach(notification =>
            {
                if (notification.HasRelatedUsers())
                {
                    var users = new List<long>();
                    Rds.ExecuteTable(statements: Rds.SelectWikis(
                        tableType: Sqls.TableTypes.All,
                        distinct: true,
                        column: Rds.WikisColumn()
                            .Creator()
                            .Updator(),
                        where: Rds.WikisWhere().WikiId(WikiId)))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                            {
                                users.Add(dataRow["Creator"].ToLong());
                                users.Add(dataRow["Updator"].ToLong());
                            });
                    notification.ReplaceRelatedUsers(users);
                }
                switch (type)
                {
                    case "Created":
                        notification.Send(
                            Displays.Created(title).ToString(),
                            url,
                            NoticeBody(ss, notification));
                        break;
                    case "Updated":
                        var body = NoticeBody(ss, notification, update: true);
                        if (body.Length > 0)
                        {
                            notification.Send(
                                Displays.Updated(title).ToString(),
                                url,
                                body);
                        }
                        break;
                    case "Deleted":
                        notification.Send(
                            Displays.Deleted(title).ToString(),
                            url,
                            NoticeBody(ss, notification));
                        break;
                }
            });
        }

        private string NoticeBody(SiteSettings ss, Notification notification, bool update = false)
        {
            var body = new System.Text.StringBuilder();
            notification.MonitorChangesColumnCollection(ss).ForEach(column =>
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

        private void Set(SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(SiteSettings ss, DataRow dataRow)
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
            if (ss != null)
            {
                Title.DisplayValue = WikiUtilities.TitleDisplayValue(ss, this);
            }
        }

        public List<string> Mine()
        {
            var mine = new List<string>();
            var userId = Sessions.UserId();
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }
    }
}
