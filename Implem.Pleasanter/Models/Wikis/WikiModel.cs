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
                return new TitleBody(WikiId, Ver, VerType == Versions.VerTypes.History, Title.Value, Title.DisplayValue, Body);
            }
        }

        public long SavedWikiId = 0;

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
                default: return Value(
                    context: context,
                    columnName: name);
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
                    default:
                        hash.Add(name, Value(
                            context: context,
                            columnName: name));
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
            IDictionary<string, string> formData = null,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            SiteId = ss.SiteId;
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public WikiModel(
            Context context,
            SiteSettings ss,
            long wikiId,
            IDictionary<string, string> formData = null,
            bool setByApi = false,
            bool clearSessions = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            WikiId = wikiId;
            SiteId = ss.SiteId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    where: Rds.WikisWhereDefault(this)
                        .Wikis_Ver(context.QueryStrings.Int("ver")), ss: ss);
            }
            else
            {
                Get(context: context, ss: ss);
            }
            if (clearSessions) ClearSessions(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public WikiModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            IDictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
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
            Set(context, ss, Repository.ExecuteTable(
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
            var data = new WikiApiModel()
            {
                ApiVersion = context.ApiVersion
            };
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
                    default: 
                        data.Value(
                            context: context,
                            columnName: column.ColumnName,
                            value: Value(
                                context: context,
                                column: column,
                                toLocal: true));
                        break;
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
                    default:
                        FullText(
                            context: context,
                            column: ss.GetColumn(
                                context: context,
                                columnName: columnName),
                            fullText: fullText);
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

        public ErrorData Create(
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
            if (extendedSqls)
            {
                statements.OnCreatingExtendedSqls(SiteId);
            }
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            WikiId = (response.Id ?? WikiId).ToLong();
            if (context.ContractSettings.Notice != false && notice)
            {
                SetTitle(
                    context: context,
                    ss: ss);
                Notice(
                    context: context,
                    ss: ss,
                    notifications: GetNotifications(
                        context: context,
                        ss: ss,
                        notice: notice),
                    type: "Created");
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
            if (extendedSqls)
            {
                statements.OnCreatedExtendedSqls(
                    siteId: SiteId,
                    id: WikiId);
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (get && Rds.ExtendedSqls(
                siteId: SiteId,
                id: WikiId)
                    ?.Any(o => o.OnCreated) == true)
            {
                Get(
                    context: context,
                    ss: ss);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertItems(
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Wikis")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.InsertWikis(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    param: param ?? Rds.WikisParamDefault(
                        context: context,
                        wikiModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
            });
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            return statements;
        }

        public ErrorData Update(
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
            var notifications = GetNotifications(
                context: context,
                ss: ss,
                notice: notice,
                before: true);
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            if (extendedSqls)
            {
                statements.OnUpdatingExtendedSqls(
                    siteId: SiteId,
                    id: WikiId,
                    timestamp: Timestamp.ToDateTime());
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                permissions: permissions,
                permissionChanged: permissionChanged,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: WikiId);
            }
            if (Title_Updated(context: context))
            {
                Repository.ExecuteNonQuery(
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
                Notice(
                    context: context,
                    ss: ss,
                    notifications: NotificationUtilities.MeetConditions(
                        ss: ss,
                        before: notifications,
                        after: GetNotifications(
                            context: context,
                            ss: ss,
                            notice: notice)),
                    type: "Updated");
            }
            if (get)
            {
                Get(context: context, ss: ss);
            }
            UpdateRelatedRecords(
                context: context,
                ss: ss,
                extendedSqls: extendedSqls,
                get: get,
                addUpdatedTimeParam: true,
                addUpdatorParam: true,
                updateItems: true);
            SiteInfo.Reflesh(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.WikisWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(Rds.WikisCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            if (permissionChanged)
            {
                statements.UpdatePermissions(context, ss, WikiId, permissions);
            }
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateWikis(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.WikisParamDefault(
                        context: context,
                        wikiModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(WikiId)){ IfConflicted = true }
            };
        }

        private List<SqlStatement> UpdateAttachmentsStatements(Context context)
        {
            var statements = new List<SqlStatement>();
            ColumnNames()
                .Where(columnName => columnName.StartsWith("Attachments"))
                .Where(columnName => Attachments_Updated(columnName: columnName))
                .ForEach(columnName =>
                    Attachments(columnName: columnName).Write(
                        context: context,
                        statements: statements,
                        referenceId: WikiId));
            return statements;
        }

        public void UpdateRelatedRecords(
            Context context,
            SiteSettings ss,
            bool extendedSqls = false,
            bool get = false,
            bool addUpdatedTimeParam = true,
            bool addUpdatorParam = true,
            bool updateItems = true)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: UpdateRelatedRecordsStatements(
                    context: context,
                    ss: ss,
                    extendedSqls: extendedSqls,
                    addUpdatedTimeParam: addUpdatedTimeParam,
                    addUpdatorParam: addUpdatorParam,
                    updateItems: updateItems)
                        .ToArray());
            if (get && Rds.ExtendedSqls(
                siteId: SiteId,
                id: WikiId)
                    ?.Any(o => o.OnUpdated) == true)
            {
                Get(
                    context: context,
                    ss: ss);
            }
            if (ss.Sources?.Any() == true)
            {
                ItemUtilities.UpdateSourceTitles(
                    context: context,
                    ss: ss,
                    idList: WikiId.ToSingleList());
            }
        }

        public List<SqlStatement> UpdateRelatedRecordsStatements(
            Context context,
            SiteSettings ss,
            bool extendedSqls = false,
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
            if (extendedSqls)
            {
                statements.OnUpdatedExtendedSqls(
                    siteId: SiteId,
                    id: WikiId);
            }
            return statements;
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.InsertItems(
                    selectIdentity: true,
                    param: Rds.ItemsParam()
                        .ReferenceType("Wikis")
                        .SiteId(SiteId)
                        .Title(Title.DisplayValue)),
                Rds.UpdateOrInsertWikis(
                    where: where ?? Rds.WikisWhereDefault(this),
                    param: param ?? Rds.WikisParamDefault(
                        context: context, wikiModel: this, setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            WikiId = (response.Id ?? WikiId).ToLong();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            statements.OnDeletingExtendedSqls(SiteId, WikiId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteItems(
                    factory: context,
                    where: Rds.ItemsWhere().ReferenceId(WikiId)),
                Rds.DeleteWikis(
                    factory: context,
                    where: Rds.WikisWhere().SiteId(SiteId).WikiId(WikiId)),
                Rds.DeleteItems(
                    factory: context,
                    where: Rds.ItemsWhere().ReferenceId(SiteId)),
                Rds.DeleteSites(
                    factory: context,
                    where: Rds.SitesWhere().SiteId(SiteId))
            });
            statements.OnDeletedExtendedSqls(SiteId, WikiId);
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (context.ContractSettings.Notice != false && notice)
            {
                Notice(
                    context: context,
                    ss: ss,
                    notifications: GetNotifications(
                        context: context,
                        ss: ss,
                        notice: notice),
                    type: "Deleted");
            }
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,long wikiId)
        {
            WikiId = wikiId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere().ReferenceId(WikiId)),
                    Rds.RestoreWikis(
                        factory: context,
                        where: Rds.WikisWhere().WikiId(WikiId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteWikis(
                    tableType: tableType,
                    param: Rds.WikisParam().SiteId(SiteId).WikiId(WikiId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            IDictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Wikis_Title": Title = new Title(WikiId, value); break;
                    case "Wikis_Body": Body = value.ToString(); break;
                    case "Wikis_Timestamp": Timestamp = value.ToString(); break;
                    case "Comments": Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: value); break;
                    case "VerUp": VerUp = value.ToBool(); break;
                    default:
                        if (key.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: key.Substring("Comment".Length).ToInt(),
                                body: value);
                        }
                        else
                        {
                            var column = ss.GetColumn(
                                context: context,
                                columnName: key.Split_2nd('_'));
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.ColumnName,
                                        value: column.Round(value.ToDecimal(
                                            cultureInfo: context.CultureInfo())));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Ver = context.QueryStrings.Int("ver");
            }
            SetByFormula(context: context, ss: ss);
            SetChoiceHash(context: context, ss: ss);
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
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
            ClassHash = wikiModel.ClassHash;
            NumHash = wikiModel.NumHash;
            DateHash = wikiModel.DateHash;
            DescriptionHash = wikiModel.DescriptionHash;
            CheckHash = wikiModel.CheckHash;
            AttachmentsHash = wikiModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<WikiApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.Title != null) Title = new Title(data.WikiId.ToLong(), data.Title);
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash.ForEach(o => Class(
                columnName: o.Key,
                value: o.Value));
            data.NumHash.ForEach(o => Num(
                columnName: o.Key,
                value: o.Value));
            data.DateHash.ForEach(o => Date(
                columnName: o.Key,
                value: o.Value.ToUniversal(context: context)));
            data.DescriptionHash.ForEach(o => Description(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash.ForEach(o => Check(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash.ForEach(o => Attachments(
                columnName: o.Key,
                value: o.Value));
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
                        default:
                            if (Def.ExtendedColumnTypes.ContainsKey(formulaSet.Target))
                            {
                                param.Add(
                                    columnBracket: $"\"{formulaSet.Target}\"",
                                    name: formulaSet.Target,
                                    value: Num(formulaSet.Target));
                            }
                            break;
                    }
                });
            Repository.ExecuteNonQuery(
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
                data.AddRange(NumHash);
                var value = formula?.GetResult(
                    data: data,
                    column: ss.GetColumn(
                        context: context,
                        columnName: columnName)) ?? 0;
                switch (columnName)
                {
                    default:
                        Num(
                            columnName: columnName,
                            value: value);
                        break;
                }
            });
        }

        public void SetTitle(Context context, SiteSettings ss)
        {
            Title = new Title(
                context: context,
                ss: ss,
                id: WikiId,
                ver: Ver,
                isHistory: VerType == Versions.VerTypes.History,
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
                        case "UpdatedTime":
                            match = UpdatedTime.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Title":
                            match = Title.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "Body":
                            match = Body.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        case "CreatedTime":
                            match = CreatedTime.Value.Matched(
                                column: column,
                                condition: filter.Value);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(filter.Key))
                            {
                                case "Class":
                                    match = Class(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Num":
                                    match = Num(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Date":
                                    match = Date(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Description":
                                    match = Description(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                                case "Check":
                                    match = Check(column: column).Matched(
                                        column: column,
                                        condition: filter.Value);
                                    break;
                            }
                            break;
                    }
                    if (!match) return false;
                }
            }
            return true;
        }

        public List<Notification> GetNotifications(
            Context context,
            SiteSettings ss,
            bool notice,
            bool before = false,
            Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            if (context.ContractSettings.Notice == false || !notice)
            {
                return null;
            }
            var notifications = NotificationUtilities.Get(
                context: context,
                ss: ss);
            if (notifications?.Any() == true)
            {
                var dataSet = Repository.ExecuteDataSet(
                    context: context,
                    statements: notifications.Select(notification =>
                        Rds.SelectWikis(
                            dataTableName: notification.Index.ToString(),
                            tableType: tableTypes,
                            column: Rds.WikisColumn().WikiId(),
                            where: ss.Views?.Get(before
                                ? notification.BeforeCondition
                                : notification.AfterCondition)
                                    ?.Where(
                                        context: context,
                                        ss: ss,
                                        where: Rds.WikisWhere().WikiId(WikiId))
                                            ?? Rds.WikisWhere().WikiId(WikiId)))
                                                .ToArray());
                return notifications
                    .Where(notification =>
                        dataSet.Tables[notification.Index.ToString()].Rows.Count == 1 )
                    .ToList();
            }
            else
            {
                return null;
            }
        }

        public void Notice(
            Context context,
            SiteSettings ss,
            List<Notification> notifications,
            string type)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: WikiId);
            notifications?.ForEach(notification =>
            {
                if (notification.HasRelatedUsers())
                {
                    var users = new List<long>();
                    Repository.ExecuteTable(
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
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                body.Append(Class(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedClass(columnName: column.Name),
                                    column: column,
                                    updated: Class_Updated(columnName: column.Name),
                                    update: update));
                                break;
                            case "Num":
                                body.Append(Num(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedNum(columnName: column.Name),
                                    column: column,
                                    updated: Num_Updated(columnName: column.Name),
                                    update: update));
                                break;
                            case "Date":
                                body.Append(Date(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedDate(columnName: column.Name),
                                    column: column,
                                    updated: Date_Updated(columnName: column.Name),
                                    update: update));
                                break;
                            case "Description":
                                body.Append(Description(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedDescription(columnName: column.Name),
                                    column: column,
                                    updated: Description_Updated(columnName: column.Name),
                                    update: update));
                                break;
                            case "Check":
                                body.Append(Check(columnName: column.Name).ToNotice(
                                    context: context,
                                    saved: SavedCheck(columnName: column.Name),
                                    column: column,
                                    updated: Check_Updated(columnName: column.Name),
                                    update: update));
                                break;
                        }
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
            if (!ss.SetAllChoices)
            {
                ss.GetUseSearchLinks(context: context).ForEach(link =>
                {
                    var value = PropertyValue(
                        context: context,
                        name: link.ColumnName);
                    var column = ss.GetColumn(
                        context: context,
                        columnName: link.ColumnName);
                    if (!value.IsNullOrEmpty() 
                        && column?.ChoiceHash.Any(o => o.Value.Value == value) != true)
                    {
                        ss.SetChoiceHash(
                            context: context,
                            columnName: column.ColumnName,
                            selectedValues: value.ToSingleList());
                    }
                });
            }
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
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedClass(
                                        columnName: column.Name,
                                        value: Class(columnName: column.Name));
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDecimal());
                                    SavedNum(
                                        columnName: column.Name,
                                        value: Num(columnName: column.Name));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SavedDate(
                                        columnName: column.Name,
                                        value: Date(columnName: column.Name));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedDescription(
                                        columnName: column.Name,
                                        value: Description(columnName: column.Name));
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SavedCheck(
                                        columnName: column.Name,
                                        value: Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SavedAttachments(
                                        columnName: column.Name,
                                        value: Attachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || SiteId_Updated(context: context)
                || Ver_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public List<string> Mine(Context context)
        {
            var mine = new List<string>();
            var userId = context.UserId;
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }

        public string IdSuffix()
        {
            return $"_{SiteId}_{(WikiId == 0 ? -1 : WikiId)}";
        }
    }
}
