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
    public class ExportSettingModel : BaseModel
    {
        public string ReferenceType = "Sites";
        public long ReferenceId = 0;
        public Title Title = new Title();
        public long ExportSettingId = 0;
        public bool AddHeader = true;
        public ExportColumns ExportColumns = new ExportColumns();
        public string SavedReferenceType = "Sites";
        public long SavedReferenceId = 0;
        public string SavedTitle = string.Empty;
        public long SavedExportSettingId = 0;
        public bool SavedAddHeader = true;
        public string SavedExportColumns = string.Empty;
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool Title_Updated { get { return Title.Value != SavedTitle && Title.Value != null; } }
        public bool ExportSettingId_Updated { get { return ExportSettingId != SavedExportSettingId; } }
        public bool AddHeader_Updated { get { return AddHeader != SavedAddHeader; } }
        public bool ExportColumns_Updated { get { return ExportColumns.ToJson() != SavedExportColumns && ExportColumns.ToJson() != null; } }

        public Title Session_Title()
        {
            return this.PageSession("Title") != null
                ? this.PageSession("Title") as Title
                : Title;
        }

        public void  Session_Title(object value)
        {
            this.PageSession("Title", value);
        }

        public bool Session_AddHeader()
        {
            return this.PageSession("AddHeader") != null
                ? this.PageSession("AddHeader").ToBool()
                : AddHeader;
        }

        public void  Session_AddHeader(object value)
        {
            this.PageSession("AddHeader", value);
        }

        public ExportColumns Session_ExportColumns()
        {
            return this.PageSession("ExportColumns") != null
                ? this.PageSession("ExportColumns")?.ToString().Deserialize<ExportColumns>() ?? new ExportColumns(ReferenceType)
                : ExportColumns;
        }

        public void  Session_ExportColumns(object value)
        {
            this.PageSession("ExportColumns", value);
        }

        public ExportSettingModel()
        {
        }

        public ExportSettingModel(
            SiteSettings ss,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = ss;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public ExportSettingModel(
            SiteSettings ss,
            long exportSettingId,
            bool clearSessions = false,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = ss;
            ExportSettingId = exportSettingId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public ExportSettingModel(
            SiteSettings ss,
            Permissions.Types pt,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = ss;
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
            Session_Title(null);
            Session_AddHeader(null);
            Session_ExportColumns(null);
        }

        public ExportSettingModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectExportSettings(
                tableType: tableType,
                column: column ?? Rds.ExportSettingsDefaultColumns(),
                join: join ??  Rds.ExportSettingsJoinDefault(),
                where: where ?? Rds.ExportSettingsWhereDefault(this),
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
                    Rds.InsertExportSettings(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.ExportSettingsParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            ExportSettingId = newId != 0 ? newId : ExportSettingId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Update(bool paramAll = false)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateExportSettings(
                        verUp: VerUp,
                        where: Rds.ExportSettingsWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: Rds.ExportSettingsParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return Error.Types.UpdateConflicts;
            Get();
            return Error.Types.None;
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
                    Rds.UpdateOrInsertExportSettings(
                        selectIdentity: true,
                        where: where ?? Rds.ExportSettingsWhereDefault(this),
                        param: param ?? Rds.ExportSettingsParamDefault(this, setDefault: true))
                });
            ExportSettingId = newId != 0 ? newId : ExportSettingId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteExportSettings(
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId))
                });
            return Error.Types.None;
        }

        public Error.Types Restore(long exportSettingId)
        {
            ExportSettingId = exportSettingId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreExportSettings(
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteExportSettings(
                    tableType: tableType,
                    param: Rds.ExportSettingsParam().ExportSettingId(ExportSettingId)));
            return Error.Types.None;
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "ExportSettings_ReferenceType": ReferenceType = Forms.Data(controlId).ToString(); break;
                    case "ExportSettings_ReferenceId": ReferenceId = Forms.Data(controlId).ToLong(); break;
                    case "ExportSettings_Title": Title = new Title(ExportSettingId, Forms.Data(controlId)); break;
                    case "ExportSettings_AddHeader": AddHeader = Forms.Data(controlId).ToBool(); break;
                    case "ExportSettings_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
            if (!Forms.HasData("ExportSettings_Title")) Title = Session_Title();
            if (!Forms.HasData("ExportSettings_AddHeader")) AddHeader = Session_AddHeader();
            if (!Forms.HasData("ExportSettings_ExportColumns")) ExportColumns = Session_ExportColumns();
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
                    case "ReferenceType": if (dataRow[name] != DBNull.Value) { ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; } break;
                    case "ReferenceId": if (dataRow[name] != DBNull.Value) { ReferenceId = dataRow[name].ToLong(); SavedReferenceId = ReferenceId; } break;
                    case "Title": if (dataRow[name] != DBNull.Value) { Title = new Title(dataRow, "ExportSettingId"); SavedTitle = Title.Value; } break;
                    case "ExportSettingId": if (dataRow[name] != DBNull.Value) { ExportSettingId = dataRow[name].ToLong(); SavedExportSettingId = ExportSettingId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "AddHeader": AddHeader = dataRow[name].ToBool(); SavedAddHeader = AddHeader; break;
                    case "ExportColumns": ExportColumns = dataRow.String("ExportColumns").Deserialize<ExportColumns>() ?? new ExportColumns(ReferenceType); SavedExportColumns = ExportColumns.ToJson(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ExportSettingModel(
            string referenceType,
            long referenceId,
            bool withTitle = false)
        {
            OnConstructing();
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            if (withTitle)
            {
                Title.Value = Forms.Data("ExportSettings_Title");
                Get(where: Rds.ExportSettingsWhere()
                    .ReferenceType(ReferenceType)
                    .ReferenceId(ReferenceId)
                    .Title(Title.Value));
            }
            else
            {
                Get(
                    where: Rds.ExportSettingsWhere()
                        .ReferenceType(ReferenceType)
                        .ReferenceId(ReferenceId),
                    orderBy: Rds.ExportSettingsOrderBy()
                        .Title(),
                    top: 1);
            }
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                ExportColumns = new ExportColumns(ReferenceType);
            }
            SetByForm();
            OnConstructed();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public enum FormatTypes : int
        {
            Csv = 1,
            Excel = 2
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Set()
        {
            var res = new ExportSettingsResponseCollection(this);
            ExportColumns = Session_ExportColumns();
            ExportColumns.SetExport(
                res,
                Forms.ControlId(),
                Forms.Data("ExportSettings_Columns")?.Split(';'),
                GetSiteSettings());
            ExportSettingUtilities.SetSessions(this);
            return res.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Dictionary<string, string> ExportColumnHash()
        {
            return ExportColumns.ExportColumnHash(GetSiteSettings());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SiteSettings GetSiteSettings()
        {
            return Url.RouteData("reference").ToLower() == "items"
                ? new ItemModel(ReferenceId).GetSite().SiteSettings
                : new SiteSettings(ReferenceType);
        }
    }
}
