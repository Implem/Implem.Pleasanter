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
    public class WikiModel : BaseItemModel
    {
        public long WikiId = 0;

        public TitleBody TitleBody
        {
            get
            {
                return new TitleBody(WikiId, Title.Value, Title.DisplayValue, Body);
            }
        }

        [NonSerialized] public long SavedWikiId = 0;

        public string PropertyValue(Context context, string name)
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

        public Dictionary<string, string> PropertyValues(Context context, IEnumerable<string> names)
        {
            var hash = new Dictionary<string, string>();
            names?.ForEach(name =>
            {
                switch (name)
                {
                    case "SiteId":
                        hash.Add("SiteId", SiteId.ToString());
                        break;
                    case "UpdatedTime":
                        hash.Add("UpdatedTime", UpdatedTime.Value.ToString());
                        break;
                    case "WikiId":
                        hash.Add("WikiId", WikiId.ToString());
                        break;
                    case "Ver":
                        hash.Add("Ver", Ver.ToString());
                        break;
                    case "Title":
                        hash.Add("Title", Title.Value);
                        break;
                    case "Body":
                        hash.Add("Body", Body);
                        break;
                    case "TitleBody":
                        hash.Add("TitleBody", TitleBody.ToString());
                        break;
                    case "Comments":
                        hash.Add("Comments", Comments.ToJson());
                        break;
                    case "Creator":
                        hash.Add("Creator", Creator.Id.ToString());
                        break;
                    case "Updator":
                        hash.Add("Updator", Updator.Id.ToString());
                        break;
                    case "CreatedTime":
                        hash.Add("CreatedTime", CreatedTime.Value.ToString());
                        break;
                    case "VerUp":
                        hash.Add("VerUp", VerUp.ToString());
                        break;
                    case "Timestamp":
                        hash.Add("Timestamp", Timestamp);
                        break;
                }
            });
            return hash;
        }

        public WikiModel()
        {
        }

        public WikiModel(
            Context context,
            SiteSettings ss,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            SiteId = ss.SiteId;
            if (setByForm) SetByForm(context: context, ss: ss);
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public WikiModel(
            Context context,
            SiteSettings ss,
            long wikiId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            WikiId = wikiId;
            SiteId = ss.SiteId;
            Get(context: context, ss: ss);
            if (clearSessions) ClearSessions(context: context);
            if (setByForm) SetByForm(context: context, ss: ss);
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public WikiModel(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null) Set(context, ss, dataRow, tableAlias);
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

        public WikiModel Get(
            Context context,
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
            Set(context, ss, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectWikis(
                    tableType: tableType,
                    column: column ?? Rds.WikisEditorColumns(ss),
                    join: join ??  Rds.WikisJoinDefault(),
                    where: where ?? Rds.WikisWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public WikiApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new WikiApiModel();
            ss.ReadableColumns(noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "SiteId": data.SiteId = SiteId; break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "WikiId": data.WikiId = WikiId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "Title": data.Title = Title.Value; break;
                    case "Body": data.Body = Body; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                }
            });
            data.ItemTitle = Title.DisplayValue;
            return data;
        }

        public string FullText(
            Context context,
            SiteSettings ss,
            bool backgroundTask = false,
            bool onCreating = false)
        {
            if (Parameters.Search.Provider != "FullText") return null;
            if (!Parameters.Search.CreateIndexes && !backgroundTask) return null;
            if (AccessStatus == Databases.AccessStatuses.NotFound) return null;
            var fullText = new List<string>();
            SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu.Breadcrumb(context: context, siteId: SiteId)
                .FullText(context, fullText);
            SiteId.FullText(context, fullText);
            ss.EditorColumns.ForEach(columnName =>
            {
                switch (columnName)
                {
                    case "WikiId":
                        WikiId.FullText(context, fullText);
                        break;
                    case "Title":
                        Title.FullText(context, fullText);
                        break;
                    case "Body":
                        Body.FullText(context, fullText);
                        break;
                    case "Comments":
                        Comments.FullText(context, fullText);
                        break;
                }
            });
            Creator.FullText(context, fullText);
            Updator.FullText(context, fullText);
            CreatedTime.FullText(context, fullText);
            UpdatedTime.FullText(context, fullText);
            if (!onCreating)
            {
                FullTextExtensions.OutgoingMailsFullText(
                    context: context,
                    fullText: fullText,
                    referenceType: "Wikis",
                    referenceId: WikiId);
            }
            return fullText
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Distinct()
                .Join(" ");
        }

        public Dictionary<string, int> SearchIndexHash(Context context, SiteSettings ss)
        {
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            else
            {
                var searchIndexHash = new Dictionary<string, int>();
                SiteInfo.TenantCaches.Get(context.TenantId)?
                    .SiteMenu
                    .Breadcrumb(context: context, siteId: SiteId)
                    .SearchIndexes(context, searchIndexHash, 100);
                SiteId.SearchIndexes(context, searchIndexHash, 200);
                UpdatedTime.SearchIndexes(context, searchIndexHash, 200);
                WikiId.SearchIndexes(context, searchIndexHash, 1);
                Title.SearchIndexes(context, searchIndexHash, 4);
                Body.SearchIndexes(context, searchIndexHash, 200);
                Comments.SearchIndexes(context, searchIndexHash, 200);
                Creator.SearchIndexes(context, searchIndexHash, 100);
                Updator.SearchIndexes(context, searchIndexHash, 100);
                CreatedTime.SearchIndexes(context, searchIndexHash, 200);
                SearchIndexExtensions.OutgoingMailsSearchIndexes(
                    context: context,
                    searchIndexHash: searchIndexHash,
                    referenceType: "Wikis",
                    referenceId: WikiId);
                return searchIndexHash;
            }
        }

        public Error.Types Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool extendedSqls = true,
            bool notice = false,
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            if (extendedSqls) statements.OnCreatingExtendedSqls(SiteId);
            CreateStatements(context, ss, statements, tableType, param, otherInitValue);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            WikiId = (response.Identity ?? WikiId).ToLong();
            if (context.ContractSettings.Notice != false && notice)
            {
                SetTitle(context: context, ss: ss);
                CheckNotificationConditions(context: context, ss: ss);
                Notice(context: context, ss: ss, type: "Created");
            }
            if (get) Get(context: context, ss: ss);
            var fullText = FullText(context, ss: ss, onCreating: true);
            statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                param: Rds.ItemsParam()
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                where: Rds.ItemsWhere().ReferenceId(WikiId)));
            statements.Add(BinaryUtilities.UpdateReferenceId(
                context: context,
                ss: ss,
                referenceId: WikiId,
                values: fullText));
            if (extendedSqls) statements.OnCreatedExtendedSqls(SiteId, WikiId);
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            Libraries.Search.Indexes.Create(context, ss, this);
            if (get && Rds.ExtendedSqls(SiteId, WikiId)?.Any(o => o.OnCreated) == true)
            {
                Get(context: context, ss: ss);
            }
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertItems(
                    setIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Wikis")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.InsertWikis(
                    tableType: tableType,
                    param: param ?? Rds.WikisParamDefault(
                        context: context,
                        wikiModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
            });
            return statements;
        }

        public Error.Types Update(
            Context context,
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            bool extendedSqls = true,
            bool synchronizeSummary = true,
            bool forceSynchronizeSourceSummary = false,
            bool notice = false,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (context.ContractSettings.Notice != false && notice)
            {
                CheckNotificationConditions(context: context, ss: ss, before: true);
            }
            if (setBySession) SetBySession(context: context);
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            if (extendedSqls) statements.OnUpdatingExtendedSqls(SiteId, WikiId, timestamp);
            UpdateStatements(
                context: context,
                ss: ss,
                statements: statements,
                timestamp: timestamp,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements);
            if (permissionChanged)
            {
                statements.UpdatePermissions(context, ss, WikiId, permissions);
            }
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Count == 0) return Error.Types.UpdateConflicts;
            if (Title_Updated(context: context))
            {
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.UpdateSites(
                            where: Rds.SitesWhere()
                                .TenantId(context.TenantId)
                                .SiteId(SiteId),
                            param: Rds.SitesParam().Title(Title.Value),
                            addUpdatedTimeParam: false,
                            addUpdatorParam: false),
                        StatusUtilities.UpdateStatus(
                            tenantId: context.TenantId,
                            type: StatusUtilities.Types.SitesUpdated)
                    });
            }
            if (context.ContractSettings.Notice != false && notice)
            {
                CheckNotificationConditions(context: context, ss: ss);
                Notice(context: context, ss: ss, type: "Updated");
            }
            if (get) Get(context: context, ss: ss);
            UpdateRelatedRecords(context: context, ss: ss, extendedSqls: extendedSqls);
            if (get && Rds.ExtendedSqls(SiteId, WikiId)?.Any(o => o.OnUpdated) == true)
            {
                Get(context: context, ss: ss);
            }
            SiteInfo.Reflesh(context: context);
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.WikisWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateWikis(
                    where: where,
                    param: param ?? Rds.WikisParamDefault(
                        context: context, wikiModel: this, otherInitValue: otherInitValue),
                    countRecord: true)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.WikisColumnCollection();
            var param = new Rds.WikisParamCollection();
            column.SiteId(function: Sqls.Functions.SingleColumn); param.SiteId();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            column.WikiId(function: Sqls.Functions.SingleColumn); param.WikiId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.Title(function: Sqls.Functions.SingleColumn); param.Title();
            column.Body(function: Sqls.Functions.SingleColumn); param.Body();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            return Rds.InsertWikis(
                tableType: tableType,
                param: param,
                select: Rds.SelectWikis(column: column, where: where),
                addUpdatorParam: false);
        }

        public void UpdateRelatedRecords(
            Context context,
            SiteSettings ss,
            bool extendedSqls,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            var fullText = FullText(context, ss: ss);
            var statements = new List<SqlStatement>();
            statements.Add(Rds.UpdateItems(
                where: Rds.ItemsWhere().ReferenceId(WikiId),
                param: Rds.ItemsParam()
                    .SiteId(SiteId)
                    .Title(Title.DisplayValue)
                    .FullText(fullText, _using: fullText != null)
                    .SearchIndexCreatedTime(DateTime.Now, _using: fullText != null),
                addUpdatedTimeParam: addUpdatedTimeParam,
                addUpdatorParam: addUpdatorParam,
                _using: updateItems));
            statements.Add(Rds.UpdateSites(
                where: Rds.SitesWhere().SiteId(SiteId),
                param: Rds.SitesParam().Title(Title.Value)));
            if (extendedSqls) statements.OnUpdatedExtendedSqls(SiteId, WikiId);
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (ss.Sources?.Any() == true)
            {
                ItemUtilities.UpdateTitles(
                    context: context,
                    siteId: SiteId,
                    id: WikiId);
            }
            Libraries.Search.Indexes.Create(context, ss, this);
        }

        public Error.Types UpdateOrCreate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    setIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Wikis")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertWikis(
                    where: where ?? Rds.WikisWhereDefault(this),
                    param: param ?? Rds.WikisParamDefault(
                        context: context, wikiModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            WikiId = (response.Identity ?? WikiId).ToLong();
            Get(context: context, ss: ss);
            Libraries.Search.Indexes.Create(context, ss, this);
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            statements.OnDeletingExtendedSqls(SiteId, WikiId);
            statements.AddRange(new List<SqlStatement>
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
            statements.OnDeletedExtendedSqls(SiteId, WikiId);
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (context.ContractSettings.Notice != false && notice)
            {
                Notice(
                    context: context,
                    ss: ss,
                    type: "Deleted");
            }
            return Error.Types.None;
        }

        public Error.Types Restore(Context context, SiteSettings ss,long wikiId)
        {
            WikiId = wikiId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        where: Rds.ItemsWhere().ReferenceId(WikiId)),
                    Rds.RestoreWikis(
                        where: Rds.WikisWhere().WikiId(WikiId))
                });
            Libraries.Search.Indexes.Create(context, ss, this);
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteWikis(
                    tableType: tableType,
                    param: Rds.WikisParam().SiteId(SiteId).WikiId(WikiId)));
            Libraries.Search.Indexes.Create(context, ss, this);
            return Error.Types.None;
        }

        public void SetByForm(Context context, SiteSettings ss)
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Wikis_Title": Title = new Title(WikiId, Forms.Data(controlId)); break;
                    case "Wikis_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Wikis_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(context: context, ss: ss, body: Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: controlId.Substring("Comment".Length).ToInt(),
                                body: Forms.Data(controlId));
                        }
                        break;
                }
            });
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
            if (context.Action == "deletecomment")
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
        }

        public void SetByModel(WikiModel wikiModel)
        {
            SiteId = wikiModel.SiteId;
            UpdatedTime = wikiModel.UpdatedTime;
            Title = wikiModel.Title;
            Body = wikiModel.Body;
            Comments = wikiModel.Comments;
            Creator = wikiModel.Creator;
            Updator = wikiModel.Updator;
            CreatedTime = wikiModel.CreatedTime;
            VerUp = wikiModel.VerUp;
            Comments = wikiModel.Comments;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = Forms.String().Deserialize<WikiApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.Title != null) Title = new Title(data.WikiId.ToLong(), data.Title);
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
        }

        public void UpdateFormulaColumns(
            Context context, SiteSettings ss, IEnumerable<int> selected = null)
        {
            SetByFormula(context: context, ss: ss);
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
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateWikis(
                    param: param,
                    where: Rds.WikisWhereDefault(this),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        public void SetByFormula(Context context, SiteSettings ss)
        {
            ss.Formulas?.ForEach(formulaSet =>
            {
                var columnName = formulaSet.Target;
                var formula = formulaSet.Formula;
                var view = ss.Views?.Get(formulaSet.Condition);
                if (view != null && !Matched(context: context, ss: ss, view: view))
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

        public void SetTitle(Context context, SiteSettings ss)
        {
            Title = new Title(
                context: context,
                ss: ss,
                id: WikiId,
                data: PropertyValues(
                    context: context,
                    names: ss.TitleColumns));
        }

        private bool Matched(Context context, SiteSettings ss, View view)
        {
            if (view.ColumnFilterHash != null)
            {
                foreach (var filter in view.ColumnFilterHash)
                {
                    var match = true;
                    var column = ss.GetColumn(context: context, columnName: filter.Key);
                    switch (filter.Key)
                    {
                        case "UpdatedTime": match = UpdatedTime.Value.Matched(column, filter.Value); break;
                        case "Title": match = Title.Value.Matched(column, filter.Value); break;
                        case "CreatedTime": match = CreatedTime.Value.Matched(column, filter.Value); break;
                    }
                    if (!match) return false;
                }
            }
            return true;
        }

        private void CheckNotificationConditions(Context context, SiteSettings ss, bool before = false)
        {
            if (ss.Notifications.Any())
            {
                ss.Notifications?.CheckConditions(
                    views: ss.Views,
                    before: before,
                    dataSet: Rds.ExecuteDataSet(
                        context: context,
                        statements: ss.Notifications.Select((o, i) =>
                            Rds.SelectWikis(
                                column: Rds.WikisColumn().WikiId(),
                                where: ss.Views?.Get(before
                                    ? o.BeforeCondition
                                    : o.AfterCondition)?
                                        .Where(
                                            context: context,
                                            ss: ss,
                                            where: Rds.WikisWhere().WikiId(WikiId)) ??
                                                Rds.WikisWhere().WikiId(WikiId)))
                                                    .ToArray()));
            }
        }

        private void Notice(Context context, SiteSettings ss, string type)
        {
            var url = Locations.ItemEditAbsoluteUri(WikiId);
            ss.Notifications.Where(o => o.Enabled).ForEach(notification =>
            {
                if (notification.HasRelatedUsers())
                {
                    var users = new List<long>();
                    Rds.ExecuteTable(
                        context: context,
                        statements: Rds.SelectWikis(
                            tableType: Sqls.TableTypes.All,
                            distinct: true,
                            column: Rds.WikisColumn()
                                .Creator()
                                .Updator(),
                            where: Rds.WikisWhere().WikiId(WikiId)))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                {
                                    users.Add(dataRow.Long("Creator"));
                                    users.Add(dataRow.Long("Updator"));
                                });
                    notification.ReplaceRelatedUsers(
                        context: context,
                        users: users);
                }
                switch (type)
                {
                    case "Created":
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Created(
                                context: context,
                                data: Title.DisplayValue).ToString(),
                            url: url,
                            body: NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification));
                        break;
                    case "Updated":
                        var body = NoticeBody(
                            context: context,
                            ss: ss,
                            notification: notification, update: true);
                        if (body.Length > 0)
                        {
                            notification.Send(
                                context: context,
                                ss: ss,
                                title: Displays.Updated(
                                    context: context,
                                    data: Title.DisplayValue).ToString(),
                                url: url,
                                body: body);
                        }
                        break;
                    case "Deleted":
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Deleted(
                                context: context,
                                data: Title.DisplayValue).ToString(),
                            url: url,
                            body: NoticeBody(
                                context: context,
                                ss: ss,
                                notification: notification));
                        break;
                }
            });
        }

        private string NoticeBody(
            Context context, SiteSettings ss, Notification notification, bool update = false)
        {
            var body = new System.Text.StringBuilder();
            notification.ColumnCollection(context, ss, update)?.ForEach(column =>
            {
                switch (column.Name)
                {
                    case "Title":
                        body.Append(Title.ToNotice(
                            context: context,
                            saved: SavedTitle,
                            column: column,
                            updated: Title_Updated(context: context),
                            update: update));
                        break;
                    case "Body":
                        body.Append(Body.ToNotice(
                            context: context,
                            saved: SavedBody,
                            column: column,
                            updated: Body_Updated(context: context),
                            update: update));
                        break;
                    case "Comments":
                        body.Append(Comments.ToNotice(
                            context: context,
                            saved: SavedComments,
                            column: column,
                            updated: Comments_Updated(context: context),
                            update: update));
                        break;
                    case "Creator":
                        body.Append(Creator.ToNotice(
                            context: context,
                            saved: SavedCreator,
                            column: column,
                            updated: Creator_Updated(context: context),
                            update: update));
                        break;
                    case "Updator":
                        body.Append(Updator.ToNotice(
                            context: context,
                            saved: SavedUpdator,
                            column: column,
                            updated: Updator_Updated(context: context),
                            update: update));
                        break;
                }
            });
            return body.ToString();
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
            SetChoiceHash(context: context, ss: ss);
        }

        public void SetChoiceHash(Context context, SiteSettings ss)
        {
            ss.GetUseSearchLinks(context: context).ForEach(link =>
            {
                var value = PropertyValue(context: context, name: link.ColumnName);
                if (!value.IsNullOrEmpty() &&
                    ss.GetColumn(context: context, columnName: link.ColumnName)?
                        .ChoiceHash.Any(o => o.Value.Value == value) != true)
                {
                    ss.SetChoiceHash(
                        context: context,
                        columnName: link.ColumnName,
                        selectedValues: value.ToSingleList());
                }
            });
            SetTitle(context: context, ss: ss);
        }

        private void Set(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SiteId = dataRow[column.ColumnName].ToLong();
                                SavedSiteId = SiteId;
                            }
                            break;
                        case "UpdatedTime":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                                SavedUpdatedTime = UpdatedTime.Value;
                            }
                            break;
                        case "WikiId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                WikiId = dataRow[column.ColumnName].ToLong();
                                SavedWikiId = WikiId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Title":
                            Title = new Title(context: context, ss: ss, dataRow: dataRow, column: column);
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
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
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                SiteId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                Title_Updated(context: context) ||
                Body_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        public List<string> Mine(Context context)
        {
            var mine = new List<string>();
            var userId = context.UserId;
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }
    }
}
