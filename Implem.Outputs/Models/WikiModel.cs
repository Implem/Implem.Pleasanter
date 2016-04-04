using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Models
{
    public class WikiModel : BaseItemModel
    {
        public long Id { get { return WikiId; } }
        public override long UrlId { get { return WikiId; } }
        public long WikiId = 0;
        public TitleBody TitleBody { get { return new TitleBody(WikiId, Title.Value, Title.DisplayValue, Body); } }
        public long SavedWikiId = 0;
        public List<long> SwitchTargets;

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
            Permissions.Types permissionType,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            PermissionType = permissionType;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public WikiModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            long wikiId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            WikiId = wikiId;
            PermissionType = permissionType;
            SiteId = SiteSettings.SiteId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
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
                column: column ?? Rds.WikisColumnDefault(),
                join: join ??  Rds.WikisJoinDefault(),
                where: where ?? Rds.WikisWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public string Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var error = ValidateBeforeCreate();
            if (error != null) return error;
            OnCreating();
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
            Get();
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(WikisUtility.TitleDisplayValue(SiteSettings, this))
                        .Subset(Jsons.ToJson(new WikiSubset(this, SiteSettings))),
                    where: Rds.ItemsWhere().ReferenceId(WikiId)));
            OnCreated();
            return RecordResponse(this, Messages.Created(Title.ToString()));
        }

        private void OnCreating()
        {
        }

        private void OnCreated()
        {
        }

        private string ValidateBeforeCreate()
        {
            if (!PermissionType.CanCreate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Wikis_SiteId": if (!SiteSettings.AllColumn("SiteId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_WikiId": if (!SiteSettings.AllColumn("WikiId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Body": if (!SiteSettings.AllColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_TitleBody": if (!SiteSettings.AllColumn("TitleBody").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        public string Update(SqlParamCollection param = null, bool paramAll = false)
        {
            var error = ValidateBeforeUpdate();
            if (error != null) return error;
            SetBySession();
            OnUpdating(ref param);
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateWikis(
                        verUp: VerUp,
                        where: Rds.WikisWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.WikisParamDefault(this, paramAll: paramAll),
                        countRecord: true),
                    Rds.Conditions("@@rowcount = 1"),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(WikiId),
                        param: Rds.ItemsParam()
                            .SiteId(SiteId)
                            .Title(WikisUtility.TitleDisplayValue(SiteSettings, this))
                            .Subset(Jsons.ToJson(new WikiSubset(this, SiteSettings)))
                            .UpdateTarget(true)),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(WikiId)),
                    InsertLinks(SiteSettings),
                    Rds.End()
                });
            if (count == 0) return ResponseConflicts();
            SynchronizeSummary();
            Get();
            var responseCollection = new WikisResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
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
            return LinksUtility.Insert(link, selectIdentity);
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref WikisResponseCollection responseCollection)
        {
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanUpdate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Wikis_SiteId": if (!SiteSettings.AllColumn("SiteId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_WikiId": if (!SiteSettings.AllColumn("WikiId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Body": if (!SiteSettings.AllColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_TitleBody": if (!SiteSettings.AllColumn("TitleBody").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(WikisResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.DisplayValue + " - " + Displays.Edit())
                .Html("#RecordInfo", Html.Builder().RecordInfo(baseModel: this, tableName: "Wikis"))
                .Html("#RecordHistories", Html.Builder().RecordHistories(ver: Ver, verType: VerType))
                .Html("#Links", Html.Builder().Links(WikiId))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanUpdate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
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
            var responseCollection = new WikisResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref WikisResponseCollection responseCollection)
        {
        }

        public string Copy()
        {
            WikiId = 0;
            if (SiteSettings.AllColumn("Title").EditorVisible.ToBool())
            {
                Title.Value += Displays.SuffixCopy();
            }
            if (!Forms.Data("Dialog_ConfirmCopy_WithComments").ToBool())
            {
                Comments.Clear();
            }
            return Create(paramAll: true);
        }

        public string Move()
        {
            var siteId = Forms.Long("Dialog_MoveTargets");
            if (siteId == 0 || !Permissions.CanMove(SiteId, siteId))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SiteId = siteId;
            Rds.ExecuteNonQuery(statements: new SqlStatement[]
            {
                Rds.UpdateItems(
                    where: Rds.ItemsWhere().ReferenceId(WikiId),
                    param: Rds.ItemsParam().SiteId(SiteId)),
                Rds.UpdateWikis(
                    where: Rds.WikisWhere().WikiId(WikiId),
                    param: Rds.WikisParam().SiteId(SiteId))
            });
            return Editor();
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().ReferenceId(WikiId)),
                    Rds.DeleteWikis(
                        where: Rds.WikisWhere().SiteId(SiteId).WikiId(WikiId))
                });
            var responseCollection = new WikisResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.ItemIndex(SiteId));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref WikisResponseCollection responseCollection)
        {
        }

        public string Restore(long wikiId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            WikiId = wikiId;
            Rds.ExecuteNonQuery(
                connectionString: Def.Db.DbOwner,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(WikiId)),
                    Rds.RestoreWikis(
                        where: Rds.WikisWhere().WikiId(WikiId))
                });
            return new ResponseCollection().ToJson();
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteWikis(
                    tableType: tableType,
                    param: Rds.WikisParam().SiteId(SiteId).WikiId(WikiId)));
            var responseCollection = new WikisResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref WikisResponseCollection responseCollection)
        {
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
                "Wikis",
                summary.LinkColumn,
                summary.Type,
                summary.SourceColumn,
                id);
        }

        private long SynchronizeSummaryDestinationId(string linkColumn, bool saved = false)
        {
            switch (linkColumn)
            {
                default: return 0;
            }
        }

        public string Histories()
        {
            var hb = Html.Builder();
            hb.Table(
                attributes: Html.Attributes().Class("grid"),
                action: () =>
                {
                    hb.GridHeader(
                        columnCollection: SiteSettings.HistoryGridColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new WikiCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.WikisWhere().WikiId(WikiId),
                        orderBy: Rds.WikisOrderBy().UpdatedTime(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(wikiModel => hb
                            .Tr(
                                attributes: Html.Attributes()
                                    .Class("grid-row not-link")
                                    .OnClick(Def.JavaScript.HistoryAndCloseDialog
                                        .Params(wikiModel.Ver))
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-latest", 1, _using: wikiModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryGridColumnCollection().ForEach(column =>
                                        hb.TdValue(column, wikiModel))));
                });
            return new WikisResponseCollection(this).Html("#HistoriesForm", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.WikisWhere()
                    .WikiId(WikiId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerType(WikiId);
            SwitchTargets = WikisUtility.GetSwitchTargets(SiteSettings, SiteId);
            return Editor();
        }

        public string PreviousHistory()
        {
            Get(
                where: Rds.WikisWhere()
                    .WikiId(WikiId)
                    .Ver(Forms.Int("Ver"), _operator: "<"),
                orderBy: Rds.WikisOrderBy()
                    .Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = WikisUtility.GetSwitchTargets(SiteSettings, SiteId);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(WikiId, Versions.DirectioTypes.Previous);
                    return Editor();
                default:
                    return new WikisResponseCollection(this).ToJson();
            }
        }

        public string NextHistory()
        {
            Get(
                where: Rds.WikisWhere()
                    .WikiId(WikiId)
                    .Ver(Forms.Int("Ver"), _operator: ">"),
                orderBy: Rds.WikisOrderBy()
                    .Ver(SqlOrderBy.Types.asc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = WikisUtility.GetSwitchTargets(SiteSettings, SiteId);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(WikiId, Versions.DirectioTypes.Next);
                    return Editor();
                default:
                    return new WikisResponseCollection(this).ToJson();
            }
        }

        public string Previous()
        {
            var switchTargets = WikisUtility.GetSwitchTargets(SiteSettings, SiteId);
            var wikiModel = new WikiModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                wikiId: switchTargets.Previous(WikiId),
                switchTargets: switchTargets);
            return RecordResponse(wikiModel);
        }

        public string Next()
        {
            var switchTargets = WikisUtility.GetSwitchTargets(SiteSettings, SiteId);
            var wikiModel = new WikiModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                wikiId: switchTargets.Next(WikiId),
                switchTargets: switchTargets);
            return RecordResponse(wikiModel);
        }

        public string Reload()
        {
            SwitchTargets = WikisUtility.GetSwitchTargets(SiteSettings, SiteId);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            WikiModel wikiModel, Message message = null, bool pushState = true)
        {
            var siteModel = new SiteModel(SiteId);
            wikiModel.MethodType = BaseModel.MethodTypes.Edit;
            return new WikisResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    wikiModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? WikisUtility.Editor(siteModel, wikiModel)
                        : WikisUtility.Editor(siteModel, this))
                .Message(message)
                .PushState(
                    Navigations.Get("Items", WikiId.ToString(), "Reload"),
                    Navigations.Edit("Items", wikiModel.WikiId),
                    _using: pushState)
                .ClearFormData()
                .ToJson();
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
                DeleteCommentId = Forms.Data("ControlId").Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    default: break;
                }
            });
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
                }
            }
            if (SiteSettings != null)
            {
                Title.DisplayValue = WikisUtility.TitleDisplayValue(SiteSettings, this);
            }
        }

        private string Editor()
        {
            var siteModel = new SiteModel(SiteId);
            return new WikisResponseCollection(this)
                .Html("#MainContainer", WikisUtility.Editor(siteModel, this))
                .ToJson();
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Delete(Permissions.Types permissionType, bool redirect = true)
        {
            if (!permissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            var parentId = new SiteModel(SiteId).ParentId;
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().ReferenceId(SiteId)),
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere().ReferenceId(WikiId)),
                    Rds.DeleteSites(
                        where: Rds.SitesWhere().SiteId(SiteId)),
                    Rds.DeleteWikis(
                        where: Rds.WikisWhere().SiteId(SiteId).WikiId(WikiId))
                });
            var responseCollection = new WikisResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.ItemIndex(parentId));
            }
            return responseCollection.ToJson();
        }
    }

    public class WikiSubset
    {
        public long SiteId;
        public string SiteId_LabelText;
        public long WikiId;
        public string WikiId_LabelText;
        public int Ver;
        public string Ver_LabelText;
        public Title Title;
        public string Title_LabelText;
        public string Body;
        public string Body_LabelText;
        public TitleBody TitleBody;
        public string TitleBody_LabelText;
        public Comments Comments;
        public string Comments_LabelText;
        public User Creator;
        public string Creator_LabelText;
        public User Updator;
        public string Updator_LabelText;

        public WikiSubset()
        {
        }

        public WikiSubset(WikiModel wikiModel, SiteSettings siteSettings)
        {
            SiteId = wikiModel.SiteId;
            SiteId_LabelText = siteSettings.EditorColumn("SiteId")?.LabelText;
            WikiId = wikiModel.WikiId;
            WikiId_LabelText = siteSettings.EditorColumn("WikiId")?.LabelText;
            Ver = wikiModel.Ver;
            Ver_LabelText = siteSettings.EditorColumn("Ver")?.LabelText;
            Title = wikiModel.Title;
            Title_LabelText = siteSettings.EditorColumn("Title")?.LabelText;
            Body = wikiModel.Body;
            Body_LabelText = siteSettings.EditorColumn("Body")?.LabelText;
            TitleBody = wikiModel.TitleBody;
            TitleBody_LabelText = siteSettings.EditorColumn("TitleBody")?.LabelText;
            Comments = wikiModel.Comments;
            Comments_LabelText = siteSettings.EditorColumn("Comments")?.LabelText;
            Creator = wikiModel.Creator;
            Creator_LabelText = siteSettings.EditorColumn("Creator")?.LabelText;
            Updator = wikiModel.Updator;
            Updator_LabelText = siteSettings.EditorColumn("Updator")?.LabelText;
        }

        public Dictionary<string, int> SearchIndexCollection()
        {
            var searchIndexHash = new Dictionary<string, int>();
            WikiId.SearchIndexes(ref searchIndexHash, 1);
            Title.SearchIndexes(ref searchIndexHash, 4);
            Body.SearchIndexes(ref searchIndexHash, 200);
            Comments.SearchIndexes(ref searchIndexHash, 200);
            Creator.SearchIndexes(ref searchIndexHash, 100);
            Updator.SearchIndexes(ref searchIndexHash, 100);
            SearchIndexExtensions.OutgoingMailsSearchIndexes(searchIndexHash, "Wikis", WikiId);
            return searchIndexHash;
        }
    }

    public class WikiCollection : List<WikiModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public WikiCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null)
        {
            Set(siteSettings, permissionType, Get(
                column: column,
                join: join,
                where: where,
                orderBy: orderBy,
                param: param,
                tableType: tableType,
                distinct: distinct,
                top: top,
                offset: offset,
                pageSize: pageSize,
                countRecord: countRecord,
                aggregationCollection: aggregationCollection));
        }

        public WikiCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private WikiCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new WikiModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public WikiCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            string commandText,
            SqlParamCollection param = null)
        {
            Set(siteSettings, permissionType, Get(commandText, param));
        }

        private DataTable Get(
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool history = false,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null)
        {
            var statements = new List<SqlStatement>
            {
                Rds.SelectWikis(
                    dataTableName: "Main",
                    column: column ?? Rds.WikisColumnDefault(),
                    join: join ??  Rds.WikisJoinDefault(),
                    where: where ?? null,
                    orderBy: orderBy ?? null,
                    param: param ?? null,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord)
            };
            if (aggregationCollection != null)
            {
                statements.AddRange(Rds.WikisAggregations(aggregationCollection, where));
            }
            var dataSet = Rds.ExecuteDataSet(
                transactional: false,
                statements: statements.ToArray());
            Aggregations.Set(dataSet, aggregationCollection);
            return dataSet.Tables["Main"];
        }

        private DataTable Get(string commandText, SqlParamCollection param = null)
        {
            return Rds.ExecuteTable(
                transactional: false,
                statements: Rds.WikisStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class WikisUtility
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            return HtmlTemplates.NotFound().ToString();
        }

        private static WikiCollection WikiCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new WikiCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Wikis",
                    formData: formData,
                    where: Rds.WikisWhere().SiteId(siteSettings.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.WikisOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static string IndexScript(
            WikiCollection wikiCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            return string.Empty;
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            WikiCollection wikiCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    wikiCollection: wikiCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        public static string DataView(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            switch (DataViewSelectors.Get(siteSettings.SiteId))
            {
                default: return Grid(siteSettings: siteSettings, permissionType: permissionType);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            WikiCollection wikiCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: Html.Attributes()
                        .Id_Css("Grid", "grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            wikiCollection: wikiCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize.ToString())
                .MainCommands(
                    siteId: siteSettings.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                    bulkMoveButton: true,
                    bulkDeleteButton: true,
                    importButton: true,
                    exportButton: true);
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var wikiCollection = WikiCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", wikiCollection.Count > 0
                    ? Html.Builder().Grid(
                        siteSettings: siteSettings,
                        wikiCollection: wikiCollection,
                        permissionType: permissionType,
                        formData: formData)
                    : Html.Builder())
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: wikiCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        public static string GridRows(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResponseCollection responseCollection = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var formData = DataViewFilters.SessionFormData(siteSettings.SiteId);
            var wikiCollection = WikiCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", Html.Builder().GridRows(
                    siteSettings: siteSettings,
                    wikiCollection: wikiCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: wikiCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, wikiCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            WikiCollection wikiCollection,
            FormData formData,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            if (addHeader)
            {
                hb.GridHeader(
                    columnCollection: siteSettings.GridColumnCollection(), 
                    formData: formData,
                    checkAll: checkAll);
            }
            wikiCollection.ForEach(wikiModel => hb
                .Tr(
                    attributes: Html.Attributes()
                        .Class("grid-row")
                        .DataId(wikiModel.WikiId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: wikiModel.WikiId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    wikiModel: wikiModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.WikisColumn()
                .SiteId()
                .WikiId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "UpdatedTime": select.UpdatedTime(); break;
                    case "WikiId": select.WikiId(); break;
                    case "Ver": select.Ver(); break;
                    case "Title": select.Title(); break;
                    case "Body": select.Body(); break;
                    case "TitleBody": select.TitleBody(); break;
                    case "Comments": select.Comments(); break;
                    case "Creator": select.Creator(); break;
                    case "Updator": select.Updator(); break;
                    case "CreatedTime": select.CreatedTime(); break;
                }
            });
            return select;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, WikiModel wikiModel)
        {
            switch (column.ColumnName)
            {
                case "SiteId": return hb.Td(column: column, value: wikiModel.SiteId);
                case "UpdatedTime": return hb.Td(column: column, value: wikiModel.UpdatedTime);
                case "WikiId": return hb.Td(column: column, value: wikiModel.WikiId);
                case "Ver": return hb.Td(column: column, value: wikiModel.Ver);
                case "Title": return hb.Td(column: column, value: wikiModel.Title);
                case "Body": return hb.Td(column: column, value: wikiModel.Body);
                case "TitleBody": return hb.Td(column: column, value: wikiModel.TitleBody);
                case "Comments": return hb.Td(column: column, value: wikiModel.Comments);
                case "Creator": return hb.Td(column: column, value: wikiModel.Creator);
                case "Updator": return hb.Td(column: column, value: wikiModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: wikiModel.CreatedTime);
                default: return hb;
            }
        }

        public static string EditorNew(SiteModel siteModel, long siteId)
        {
            return Editor(
                siteModel,
                new WikiModel(
                    siteModel.WikisSiteSettings(),
                    siteModel.PermissionType,
                    methodType: BaseModel.MethodTypes.New)
                {
                    SiteId = siteId
                });
        }

        public static string Editor(SiteModel siteModel, long wikiId, bool clearSessions)
        {
            var siteSettings = siteModel.WikisSiteSettings();
            var wikiModel = new WikiModel(
                siteSettings: siteSettings,
                permissionType: siteModel.PermissionType,
                wikiId: wikiId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            wikiModel.SwitchTargets = WikisUtility.GetSwitchTargets(
                siteSettings, wikiModel.SiteId);
            return Editor(siteModel, wikiModel);
        }

        public static string Editor(SiteModel siteModel, WikiModel wikiModel)
        {
            var hb = Html.Builder();
            wikiModel.SiteSettings.SetLinks();
            return hb.Template(
                siteId: siteModel.SiteId,
                modelName: "Wiki",
                title: wikiModel.MethodType != BaseModel.MethodTypes.New
                    ? wikiModel.Title.DisplayValue + " - " + Displays.Edit()
                    : siteModel.Title.DisplayValue + " - " + Displays.New(),
                permissionType: wikiModel.PermissionType,
                verType: wikiModel.VerType,
                backUrl: Navigations.ItemIndex(wikiModel.SiteId),
                methodType: wikiModel.MethodType,
                allowAccess:
                    wikiModel.PermissionType.CanRead() &&
                    wikiModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    hb
                        .Editor(
                            siteSettings: wikiModel.SiteSettings,
                            wikiModel: wikiModel,
                            siteModel: siteModel)
                        .Hidden(controlId: "TableName", value: "Wikis")
                        .Hidden(controlId: "Id", value: wikiModel.WikiId.ToString());
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteModel siteModel,
            SiteSettings siteSettings,
            WikiModel wikiModel)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: Html.Attributes()
                        .Id_Css("WikiForm", "main-form")
                        .Action(Navigations.ItemAction(wikiModel.WikiId != 0 
                            ? wikiModel.WikiId
                            : siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            id: wikiModel.WikiId,
                            baseModel: wikiModel,
                            tableName: "Wikis",
                            switchTargets: wikiModel.SwitchTargets)
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: wikiModel.Comments,
                                verType: wikiModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(wikiModel: wikiModel)
                            .Fields(
                                wikiModel: wikiModel,
                                permissionType: siteModel.PermissionType,
                                siteSettings: siteSettings)
                            .Div(id: "LinkCreations", css: "links", action: () => hb
                                .LinkCreations(
                                    siteSettings: siteSettings,
                                    linkId: wikiModel.WikiId,
                                    methodType: wikiModel.MethodType))
                            .Div(id: "Links", css: "links", action: () => hb
                                .Links(linkId: wikiModel.WikiId))
                            .MainCommands(
                                siteId: siteModel.SiteId,
                                permissionType: siteModel.PermissionType,
                                verType: wikiModel.VerType,
                                backUrl: Navigations.ItemIndex(siteModel.ParentId),
                                referenceType: "items",
                                referenceId: wikiModel.WikiId,
                                updateButton: true,
                                copyButton: false,
                                moveButton: false,
                                mailButton: true,
                                deleteButton: true))
                        .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl())
                        .Hidden(
                            controlId: "Wikis_Timestamp",
                            css: "must-transport",
                            value: wikiModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: wikiModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Wikis", wikiModel.WikiId, wikiModel.Ver)
                .Dialog_Copy("items", wikiModel.WikiId)
                .Dialog_Move("items", wikiModel.WikiId)
                .Dialog_Histories(Navigations.ItemAction(wikiModel.WikiId))
                .Dialog_OutgoingMail());
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, WikiModel wikiModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic())));
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            WikiModel wikiModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.ColumnCollection
                    .Where(o => o.EditorVisible.ToBool())
                    .OrderBy(o => siteSettings.EditorOrder.IndexOf(o.ColumnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "WikiId": hb.Field(siteSettings, column, wikiModel.WikiId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Ver": hb.Field(siteSettings, column, wikiModel.Ver.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Title": hb.Field(siteSettings, column, wikiModel.Title.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Body": hb.Field(siteSettings, column, wikiModel.Body.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                        }
                    });
                hb.VerUpCheckBox(wikiModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            WikiModel wikiModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            WikiModel wikiModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<long> GetSwitchTargets(SiteSettings siteSettings, long siteId)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData(siteId);
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectWikis(
                        column: Rds.WikisColumn().WikiId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Wikis",
                            formData: formData,
                            where: Rds.WikisWhere().SiteId(siteId)),
                        orderBy: GridSorters.Get(
                            formData, Rds.WikisOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["WikiId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        public static string BulkMove(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var siteId = Forms.Long("Dialog_MoveTargets");
            if (Permissions.CanMove(siteSettings.SiteId, siteId))
            {
                var count = 0;
                if (Forms.Bool("GridCheckAll"))
                {
                    count = BulkMove(
                        siteId,
                        siteSettings,
                        GridItems("GridUnCheckedItems"),
                        negative: true);
                }
                else
                {
                    var checkedItems = GridItems("GridCheckedItems");
                    if (checkedItems.Count() > 0)
                    {
                        count = BulkMove(
                            siteId,
                            siteSettings,
                            checkedItems);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                return GridRows(
                    siteSettings,
                    permissionType,
                    clearCheck: true,
                    message: Messages.BulkMoved(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkMove(
            long siteId,
            SiteSettings siteSettings,
            IEnumerable<long> checkedItems,
            bool negative = false)
        {
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateWikis(
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Wikis",
                            formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                            where: Rds.WikisWhere()
                                .SiteId(siteSettings.SiteId)
                                .WikiId_In(
                                    value: checkedItems,
                                    negative: negative,
                                    _using: checkedItems.Count() > 0)),
                        param: Rds.WikisParam().SiteId(siteId),
                        countRecord: true),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectWikis(
                                    column: Rds.WikisColumn().WikiId(),
                                    where: Rds.WikisWhere().SiteId(siteId)))
                            .SiteId(siteId, _operator: "<>"),
                        param: Rds.ItemsParam().SiteId(siteId))
                });
        }

        public static string BulkDelete(
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            if (permissionType.CanDelete())
            {
                var count = 0;
                if (Forms.Bool("GridCheckAll"))
                {
                    count = BulkDelete(
                        siteSettings,
                        GridItems("GridUnCheckedItems"),
                        negative: true);
                }
                else
                {
                    var checkedItems = GridItems("GridCheckedItems");
                    if (checkedItems.Count() > 0)
                    {
                        count = BulkDelete(
                            siteSettings,
                            checkedItems);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                return GridRows(
                    siteSettings,
                    permissionType,
                    clearCheck: true,
                    message: Messages.BulkDeleted(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkDelete(
            SiteSettings siteSettings,
            IEnumerable<long> checkedItems,
            bool negative = false)
        {
            var where = DataViewFilters.Get(
                siteSettings: siteSettings,
                tableName: "Wikis",
                formData: DataViewFilters.SessionFormData(siteSettings.SiteId),
                where: Rds.WikisWhere()
                    .SiteId(siteSettings.SiteId)
                    .WikiId_In(
                        value: checkedItems,
                        negative: negative,
                        _using: checkedItems.Count() > 0));
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectWikis(
                                    column: Rds.WikisColumn().WikiId(),
                                    where: where))),
                    Rds.DeleteWikis(
                        where: where, 
                        countRecord: true)
                });
        }

        private static IEnumerable<long> GridItems(string name)
        {
            return Forms.Data(name)
                .Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .Distinct();
        }

        public static string Import(SiteModel siteModel)
        {
            if (!siteModel.PermissionType.CanCreate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var responseCollection = new ResponseCollection();
            Csv csv;
            try
            {
                csv = new Csv(Forms.File("Import"), Forms.Data("Encoding"));
            }
            catch
            {
                return Messages.ResponseFailedReadFile().ToJson();
            }
            if (csv != null && csv.Rows.Count() != 0)
            {
                var siteSettings = siteModel.WikisSiteSettings();
                var columnHash = new Dictionary<int, Column>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = siteSettings.ColumnCollection
                        .Where(o => o.LabelText == data.Header)
                        .FirstOrDefault();
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var error = Imports.ColumnValidate(siteSettings, columnHash.Values
                    .Select(o => o.ColumnName), "Title");
                if (error != null) return error;
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.WikisParam();
                    param.WikiId(raw: Def.Sql.Identity);
                    param.SiteId(siteModel.SiteId);
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            column.Value, data.Row[column.Key], siteModel.InheritPermission);
                        if (!param.Any(o => o.Name == column.Value.ColumnName))
                        {
                            switch (column.Value.ColumnName)
                            {
                                case "Title": param.Title(recordingData, _using: recordingData != null); break;
                                case "Body": param.Body(recordingData, _using: recordingData != null); break;
                            }
                        }
                    });
                    paramHash.Add(data.Index, param);
                });
                var errorTitle = Imports.Validate(
                    paramHash, siteSettings.AllColumn("Title"));
                if (errorTitle != null) return errorTitle;
                paramHash.Values.ForEach(param =>
                    new WikiModel(siteSettings, siteModel.PermissionType)
                    {
                        SiteId = siteModel.SiteId,
                        Title = new Title(param.FirstOrDefault(o =>
                            o.Name == "Title").Value.ToString()),
                        SiteSettings = siteSettings
                    }.Create(param: param));
                return GridRows(siteSettings, siteModel.PermissionType, responseCollection
                    .WindowScrollTop()
                    .CloseDialog("#Dialog_ImportSettings")
                    .Message(Messages.Imported(csv.Rows.Count().ToString())));
            }
            else
            {
                return Messages.ResponseFileNotFound().ToJson();
            }
        }

        private static object ImportRecordingData(
            Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(value, inheritPermission);
            return recordingData;
        }

        public static ResponseFile Export(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            SiteModel siteModel)
        {
            siteModel.SiteSettings.SetLinks();
            var formData = DataViewFilters.SessionFormData(siteModel.SiteId);
            var wikiCollection = new WikiCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                where: DataViewFilters.Get(
                    siteSettings: siteModel.SiteSettings,
                    tableName: "Wikis",
                    formData: formData,
                    where: Rds.WikisWhere().SiteId(siteModel.SiteId)),
                orderBy: GridSorters.Get(
                    formData, Rds.WikisOrderBy().UpdatedTime(SqlOrderBy.Types.desc)));
            var csv = new StringBuilder();
            var exportColumns = (Sessions.PageSession(
                siteModel.Id, 
                "ExportSettings_ExportColumns").ToString().Deserialize<ExportColumns>());
            var columnHash = exportColumns.ColumnHash(siteModel.WikisSiteSettings());
            if (Sessions.PageSession(siteModel.Id, "ExportSettings_AddHeader").ToBool())
            {
                var header = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn => header.Add(
                        "\"" + columnHash[exportColumn.Key].LabelText + "\""));
                csv.Append(header.Join(","), "\n");
            }
            wikiCollection.ForEach(wikiModel =>
            {
                var row = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            wikiModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key])));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(
            WikiModel wikiModel, string columnName, Column column)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId": value = wikiModel.SiteId.ToExport(column); break;
                case "UpdatedTime": value = wikiModel.UpdatedTime.ToExport(column); break;
                case "WikiId": value = wikiModel.WikiId.ToExport(column); break;
                case "Ver": value = wikiModel.Ver.ToExport(column); break;
                case "Title": value = wikiModel.Title.ToExport(column); break;
                case "Body": value = wikiModel.Body.ToExport(column); break;
                case "TitleBody": value = wikiModel.TitleBody.ToExport(column); break;
                case "Comments": value = wikiModel.Comments.ToExport(column); break;
                case "Creator": value = wikiModel.Creator.ToExport(column); break;
                case "Updator": value = wikiModel.Updator.ToExport(column); break;
                case "CreatedTime": value = wikiModel.CreatedTime.ToExport(column); break;
                default: return string.Empty;
            }
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, WikiModel wikiModel)
        {
            var displayValue = siteSettings.ColumnCollection
                .Where(o => o.TitleVisible.ToBool())
                .OrderBy(o => siteSettings.TitleOrder.IndexOf(o.ColumnName))
                .Select(column => TitleDisplayValue(column, wikiModel))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, WikiModel wikiModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(wikiModel.Title.Value).Text()
                    : wikiModel.Title.Value;
                default: return string.Empty;
            }
        }

        public static string TitleDisplayValue(SiteSettings siteSettings, DataRow dataRow)
        {
            var displayValue = siteSettings.ColumnCollection
                .Where(o => o.TitleVisible.ToBool())
                .OrderBy(o => siteSettings.TitleOrder.IndexOf(o.ColumnName))
                .Select(column => TitleDisplayValue(column, dataRow))
                .Where(o => o != string.Empty)
                .Join(siteSettings.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, DataRow dataRow)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(dataRow["Title"].ToString()).Text()
                    : dataRow["Title"].ToString();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <returns></returns>
        public static string Editor(
            SiteModel siteModel,
            SiteSettings siteSettings,
            WikiModel wikiModel)
        {
            var hb = Html.Builder();
            siteSettings.SetLinks();
            return hb.Template(
                siteId: siteModel.SiteId,
                modelName: "Wiki",
                title: wikiModel.MethodType != BaseModel.MethodTypes.New
                    ? wikiModel.Title.DisplayValue + " - " + Displays.Edit()
                    : siteModel.Title.DisplayValue + " - " + Displays.New(),
                permissionType: siteModel.PermissionType,
                verType: wikiModel.VerType,
                backUrl: Navigations.ItemIndex(siteModel.ParentId),
                methodType: wikiModel.MethodType,
                allowAccess:
                    siteModel.PermissionType.CanRead() &&
                    wikiModel.AccessStatus != Databases.AccessStatuses.NotFound,
                useNavigationButtons: false,
                action: () =>
                {
                    hb
                        .Editor(
                            siteSettings: siteSettings,
                            wikiModel: wikiModel,
                            siteModel: siteModel)
                        .Hidden(controlId: "TableName", value: "Wikis");
                }).ToString();
        }
    }
}
