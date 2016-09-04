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
                column: column ?? Rds.WikisEditorColumns(SiteSettings),
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
                        .Title(WikiUtilities.TitleDisplayValue(SiteSettings, this))
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
                    case "Wikis_SiteId": if (!SiteSettings.GetColumn("SiteId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_WikiId": if (!SiteSettings.GetColumn("WikiId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Ver": if (!SiteSettings.GetColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Title": if (!SiteSettings.GetColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Body": if (!SiteSettings.GetColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_TitleBody": if (!SiteSettings.GetColumn("TitleBody").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Comments": if (!SiteSettings.GetColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Creator": if (!SiteSettings.GetColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Updator": if (!SiteSettings.GetColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_VerUp": if (!SiteSettings.GetColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: param ?? Rds.WikisParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            SynchronizeSummary();
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
            return LinkUtilities.Insert(link, selectIdentity);
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
                    case "Wikis_SiteId": if (!SiteSettings.GetColumn("SiteId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_WikiId": if (!SiteSettings.GetColumn("WikiId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Ver": if (!SiteSettings.GetColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Title": if (!SiteSettings.GetColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Body": if (!SiteSettings.GetColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_TitleBody": if (!SiteSettings.GetColumn("TitleBody").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Comments": if (!SiteSettings.GetColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Creator": if (!SiteSettings.GetColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Updator": if (!SiteSettings.GetColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_VerUp": if (!SiteSettings.GetColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Wikis_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                .FormResponse(this)
                .Formula(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.DisplayValue)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "Wikis"))
                .Html("#Links", new HtmlBuilder().Links(WikiId))
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
            if (SiteSettings.EditorColumnsOrder.Contains("Title"))
            {
                Title.Value += Displays.SuffixCopy();
            }
            if (!Forms.Data("CopyWithComments").ToBool())
            {
                Comments.Clear();
            }
            return Create(paramAll: true);
        }

        public string Move()
        {
            var siteId = Forms.Long("MoveTargets");
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
            Get();
            Sessions.Set("Message", Messages.Moved(Title.Value).Html);
            return Editor();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var redirectUrl = Navigations.ItemIndex(new SiteModel(SiteId).ParentId);
            OnDeleting();
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
            var responseCollection = new WikisResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(redirectUrl);
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
                connectionString: Parameters.Rds.OwnerConnectionString,
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
                SiteId,
                "Wikis",
                summary.LinkColumn,
                summary.Type,
                summary.SourceColumn,
                id);
            Formulas.Update(id);
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

        public string Histories()
        {
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columnCollection: SiteSettings.HistoryColumnCollection(),
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new WikiCollection(
                            siteSettings: SiteSettings,
                            permissionType: PermissionType,
                            where: Rds.WikisWhere().WikiId(WikiId),
                            orderBy: Rds.WikisOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(wikiModel => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(wikiModel.Ver)
                                            .DataLatest(1, _using: wikiModel.Ver == Ver),
                                        action: () =>
                                            SiteSettings.HistoryColumnCollection()
                                                .ForEach(column => hb
                                                    .TdValue(column, wikiModel))))));
            return new WikisResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
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
                : Versions.VerTypes.History;
            SwitchTargets = WikiUtilities.GetSwitchTargets(SiteSettings, SiteId);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = WikiUtilities.GetSwitchTargets(SiteSettings, SiteId);
            var wikiModel = new WikiModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                wikiId: switchTargets.Previous(WikiId),
                switchTargets: switchTargets);
            return RecordResponse(wikiModel);
        }

        public string Next()
        {
            var switchTargets = WikiUtilities.GetSwitchTargets(SiteSettings, SiteId);
            var wikiModel = new WikiModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                wikiId: switchTargets.Next(WikiId),
                switchTargets: switchTargets);
            return RecordResponse(wikiModel);
        }

        public string Reload()
        {
            SwitchTargets = WikiUtilities.GetSwitchTargets(SiteSettings, SiteId);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            WikiModel wikiModel, Message message = null, bool pushState = true)
        {
            var siteModel = new SiteModel(SiteId);
            wikiModel.MethodType = BaseModel.MethodTypes.Edit;
            return new WikisResponseCollection(this)
                .Invoke("clearDialogs")
                .ReplaceAll(
                    "#MainContainer",
                    wikiModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? WikiUtilities.Editor(siteModel, wikiModel, byRest: true)
                        : WikiUtilities.Editor(siteModel, this, byRest: true))
                .Invoke("validateWikis")
                .Message(message)
                .PushState(
                    "Edit",
                    Navigations.ItemEdit(wikiModel.WikiId),
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

        private string Editor()
        {
            var siteModel = new SiteModel(SiteId);
            return new WikisResponseCollection(this)
                .ReplaceAll(
                    "#MainContainer",
                    WikiUtilities.Editor(siteModel, this, byRest: true))
                .Invoke("validateWikis")
                .ToJson();
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName()).ToJson()
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
}
