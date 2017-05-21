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
    public class TemplateModel : BaseModel
    {
        public int TemplateId = 0;
        public string Title = string.Empty;
        public bool Standard = false;
        public string Body = string.Empty;
        public string Tags = string.Empty;
        public string SiteSettingsTemplate = string.Empty;
        public int SavedTemplateId = 0;
        public string SavedTitle = string.Empty;
        public bool SavedStandard = false;
        public string SavedBody = string.Empty;
        public string SavedTags = string.Empty;
        public string SavedSiteSettingsTemplate = string.Empty;
        public bool TemplateId_Updated { get { return TemplateId != SavedTemplateId; } }
        public bool Title_Updated { get { return Title != SavedTitle && Title != null; } }
        public bool Standard_Updated { get { return Standard != SavedStandard; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
        public bool Tags_Updated { get { return Tags != SavedTags && Tags != null; } }
        public bool SiteSettingsTemplate_Updated { get { return SiteSettingsTemplate != SavedSiteSettingsTemplate && SiteSettingsTemplate != null; } }

        public TemplateModel()
        {
        }

        public TemplateModel(
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public TemplateModel(
            int templateId,
            bool clearSessions = false,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            TemplateId = templateId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public TemplateModel(DataRow dataRow)
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

        public TemplateModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectTemplates(
                tableType: tableType,
                column: column ?? Rds.TemplatesDefaultColumns(),
                join: join ??  Rds.TemplatesJoinDefault(),
                where: where ?? Rds.TemplatesWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public Error.Types Create(
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var statements = CreateStatements(tableType, param, paramAll);
            var newId = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            TemplateId = newId != 0 ? newId : TemplateId;
            Get();
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            return new List<SqlStatement>
            {
                Rds.InsertTemplates(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.TemplatesParamDefault(
                        this, setDefault: true, paramAll: paramAll))
            };
        }

        public Error.Types Update(
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            SetBySession();
            var statements = UpdateStatements(param, paramAll);
            var count = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            if (count == 0) return Error.Types.UpdateConflicts;
            Get();
            return Error.Types.None;
        }

        public List<SqlStatement> UpdateStatements(
            SqlParamCollection param, bool paramAll = false)
        {
            var timestamp = Timestamp.ToDateTime();
            return new List<SqlStatement>
            {
                Rds.UpdateTemplates(
                    verUp: VerUp,
                    where: Rds.TemplatesWhereDefault(this)
                        .UpdatedTime(timestamp, _using: timestamp.InRange()),
                    param: param ?? Rds.TemplatesParamDefault(this, paramAll: paramAll),
                    countRecord: true)
            };
        }

        public Error.Types UpdateOrCreate(
            RdsUser rdsUser = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertTemplates(
                    selectIdentity: true,
                    where: where ?? Rds.TemplatesWhereDefault(this),
                    param: param ?? Rds.TemplatesParamDefault(this, setDefault: true))
            };
            var newId = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            TemplateId = newId != 0 ? newId : TemplateId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteTemplates(
                        where: Rds.TemplatesWhere().TemplateId(TemplateId))
                });
            return Error.Types.None;
        }

        public Error.Types Restore(int templateId)
        {
            TemplateId = templateId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreTemplates(
                        where: Rds.TemplatesWhere().TemplateId(TemplateId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteTemplates(
                    tableType: tableType,
                    param: Rds.TemplatesParam().TemplateId(TemplateId)));
            return Error.Types.None;
        }

        public void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Templates_Title": Title = Forms.Data(controlId).ToString(); break;
                    case "Templates_Standard": Standard = Forms.Data(controlId).ToBool(); break;
                    case "Templates_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Templates_Tags": Tags = Forms.Data(controlId).ToString(); break;
                    case "Templates_SiteSettingsTemplate": SiteSettingsTemplate = Forms.Data(controlId).ToString(); break;
                    case "Templates_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                    case "TemplateId": if (dataRow[name] != DBNull.Value) { TemplateId = dataRow[name].ToInt(); SavedTemplateId = TemplateId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "Title": Title = dataRow[name].ToString(); SavedTitle = Title; break;
                    case "Standard": Standard = dataRow[name].ToBool(); SavedStandard = Standard; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "Tags": Tags = dataRow[name].ToString(); SavedTags = Tags; break;
                    case "SiteSettingsTemplate": SiteSettingsTemplate = dataRow[name].ToString(); SavedSiteSettingsTemplate = SiteSettingsTemplate; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }
    }
}
