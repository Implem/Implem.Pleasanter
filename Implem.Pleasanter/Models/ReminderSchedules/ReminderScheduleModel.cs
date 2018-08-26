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
    public class ReminderScheduleModel : BaseModel
    {
        public long SiteId = 0;
        public int Id = 0;
        public DateTime ScheduledTime = 0.ToDateTime();
        [NonSerialized] public long SavedSiteId = 0;
        [NonSerialized] public int SavedId = 0;
        [NonSerialized] public DateTime SavedScheduledTime = 0.ToDateTime();

        public bool SiteId_Updated(Context context, Column column = null)
        {
            return SiteId != SavedSiteId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != SiteId);
        }

        public bool Id_Updated(Context context, Column column = null)
        {
            return Id != SavedId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != Id);
        }

        public bool ScheduledTime_Updated(Context context, Column column = null)
        {
            return ScheduledTime != SavedScheduledTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != ScheduledTime.Date);
        }

        public ReminderScheduleModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null) Set(context, dataRow, tableAlias);
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions()
        {
        }

        public ReminderScheduleModel Get(
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
            Set(context, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectReminderSchedules(
                    tableType: tableType,
                    column: column ?? Rds.ReminderSchedulesDefaultColumns(),
                    join: join ??  Rds.ReminderSchedulesJoinDefault(),
                    where: where ?? Rds.ReminderSchedulesWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public void SetByModel(ReminderScheduleModel reminderScheduleModel)
        {
            SiteId = reminderScheduleModel.SiteId;
            Id = reminderScheduleModel.Id;
            ScheduledTime = reminderScheduleModel.ScheduledTime;
            Comments = reminderScheduleModel.Comments;
            Creator = reminderScheduleModel.Creator;
            Updator = reminderScheduleModel.Updator;
            CreatedTime = reminderScheduleModel.CreatedTime;
            UpdatedTime = reminderScheduleModel.UpdatedTime;
            VerUp = reminderScheduleModel.VerUp;
            Comments = reminderScheduleModel.Comments;
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
                        case "SiteId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SiteId = dataRow[column.ColumnName].ToLong();
                                SavedSiteId = SiteId;
                            }
                            break;
                        case "Id":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                Id = dataRow[column.ColumnName].ToInt();
                                SavedId = Id;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "ScheduledTime":
                            ScheduledTime = dataRow[column.ColumnName].ToDateTime();
                            SavedScheduledTime = ScheduledTime;
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
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
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
                Id_Updated(context: context) ||
                Ver_Updated(context: context) ||
                ScheduledTime_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }
    }
}
