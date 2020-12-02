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
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Models.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class SessionModel : BaseModel
    {
        public string SessionGuid = string.Empty;
        public string Key = string.Empty;
        public string Page = string.Empty;
        public string Value = string.Empty;
        public bool ReadOnce = false;
        public bool UserArea = false;
        public string SavedSessionGuid = string.Empty;
        public string SavedKey = string.Empty;
        public string SavedPage = string.Empty;
        public string SavedValue = string.Empty;
        public bool SavedReadOnce = false;
        public bool SavedUserArea = false;

        public bool SessionGuid_Updated(Context context, Column column = null)
        {
            return SessionGuid != SavedSessionGuid && SessionGuid != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != SessionGuid);
        }

        public bool Key_Updated(Context context, Column column = null)
        {
            return Key != SavedKey && Key != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Key);
        }

        public bool Page_Updated(Context context, Column column = null)
        {
            return Page != SavedPage && Page != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Page);
        }

        public bool Value_Updated(Context context, Column column = null)
        {
            return Value != SavedValue && Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Value);
        }

        public bool ReadOnce_Updated(Context context, Column column = null)
        {
            return ReadOnce != SavedReadOnce &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != ReadOnce);
        }

        public bool UserArea_Updated(Context context, Column column = null)
        {
            return UserArea != SavedUserArea &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != UserArea);
        }

        public SessionModel(
            Context context,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
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

        public SessionModel Get(
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
            where = where ?? Rds.SessionsWhereDefault(this);
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectSessions(
                    tableType: tableType,
                    column: column ?? Rds.SessionsDefaultColumns(),
                    join: join ??  Rds.SessionsJoinDefault(),
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public void SetByModel(SessionModel sessionModel)
        {
            SessionGuid = sessionModel.SessionGuid;
            Key = sessionModel.Key;
            Page = sessionModel.Page;
            Value = sessionModel.Value;
            ReadOnce = sessionModel.ReadOnce;
            UserArea = sessionModel.UserArea;
            Comments = sessionModel.Comments;
            Creator = sessionModel.Creator;
            Updator = sessionModel.Updator;
            CreatedTime = sessionModel.CreatedTime;
            UpdatedTime = sessionModel.UpdatedTime;
            VerUp = sessionModel.VerUp;
            Comments = sessionModel.Comments;
            ClassHash = sessionModel.ClassHash;
            NumHash = sessionModel.NumHash;
            DateHash = sessionModel.DateHash;
            DescriptionHash = sessionModel.DescriptionHash;
            CheckHash = sessionModel.CheckHash;
            AttachmentsHash = sessionModel.AttachmentsHash;
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
                        case "SessionGuid":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SessionGuid = dataRow[column.ColumnName].ToString();
                                SavedSessionGuid = SessionGuid;
                            }
                            break;
                        case "Key":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                Key = dataRow[column.ColumnName].ToString();
                                SavedKey = Key;
                            }
                            break;
                        case "Page":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                Page = dataRow[column.ColumnName].ToString();
                                SavedPage = Page;
                            }
                            break;
                        case "Value":
                            Value = dataRow[column.ColumnName].ToString();
                            SavedValue = Value;
                            break;
                        case "ReadOnce":
                            ReadOnce = dataRow[column.ColumnName].ToBool();
                            SavedReadOnce = ReadOnce;
                            break;
                        case "UserArea":
                            UserArea = dataRow[column.ColumnName].ToBool();
                            SavedUserArea = UserArea;
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
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
                || SessionGuid_Updated(context: context)
                || Key_Updated(context: context)
                || Page_Updated(context: context)
                || Value_Updated(context: context)
                || ReadOnce_Updated(context: context)
                || UserArea_Updated(context: context)
                || Ver_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }
    }
}
