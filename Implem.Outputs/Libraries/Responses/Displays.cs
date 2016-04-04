using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Views;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Displays
    {
        public static string Get(string id, params string[] data)
        {
            var language = "_" + Sessions.Language();
            var screen =
                Def.DisplayDefinitionCollection
                    .Where(o => o.Id == id + language)
                    .Select(o => o.Content).FirstOrDefault() ??
                Def.DisplayDefinitionCollection
                    .Where(o => o.Id == id)
                    .Select(o => o.Content).FirstOrDefault() ??
                id;
             if (data.Count() == 0)
             {
                 return screen;
             }
             else
             {
                 return screen.Params(data);
             }
        }

        public static HtmlBuilder Displays_Login(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Login", data)); }
        public static string Login(params string[] data) { return Get("Login", data); }
        public static HtmlBuilder Displays_Top(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Top", data)); }
        public static string Top(params string[] data) { return Get("Top", data); }
        public static HtmlBuilder Displays_List(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("List", data)); }
        public static string List(params string[] data) { return Get("List", data); }
        public static HtmlBuilder Displays_New(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("New", data)); }
        public static string New(params string[] data) { return Get("New", data); }
        public static HtmlBuilder Displays_EditSettings(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("EditSettings", data)); }
        public static string EditSettings(params string[] data) { return Get("EditSettings", data); }
        public static HtmlBuilder Displays_Create(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Create", data)); }
        public static string Create(params string[] data) { return Get("Create", data); }
        public static HtmlBuilder Displays_Update(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Update", data)); }
        public static string Update(params string[] data) { return Get("Update", data); }
        public static HtmlBuilder Displays_Save(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Save", data)); }
        public static string Save(params string[] data) { return Get("Save", data); }
        public static HtmlBuilder Displays_Change(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Change", data)); }
        public static string Change(params string[] data) { return Get("Change", data); }
        public static HtmlBuilder Displays_Add(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Add", data)); }
        public static string Add(params string[] data) { return Get("Add", data); }
        public static HtmlBuilder Displays_Reset(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Reset", data)); }
        public static string Reset(params string[] data) { return Get("Reset", data); }
        public static HtmlBuilder Displays_Copy(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Copy", data)); }
        public static string Copy(params string[] data) { return Get("Copy", data); }
        public static HtmlBuilder Displays_Move(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Move", data)); }
        public static string Move(params string[] data) { return Get("Move", data); }
        public static HtmlBuilder Displays_BulkMove(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("BulkMove", data)); }
        public static string BulkMove(params string[] data) { return Get("BulkMove", data); }
        public static HtmlBuilder Displays_Mail(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Mail", data)); }
        public static string Mail(params string[] data) { return Get("Mail", data); }
        public static HtmlBuilder Displays_SendMail(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SendMail", data)); }
        public static string SendMail(params string[] data) { return Get("SendMail", data); }
        public static HtmlBuilder Displays_Delete(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Delete", data)); }
        public static string Delete(params string[] data) { return Get("Delete", data); }
        public static HtmlBuilder Displays_Separate(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Separate", data)); }
        public static string Separate(params string[] data) { return Get("Separate", data); }
        public static HtmlBuilder Displays_BulkDelete(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("BulkDelete", data)); }
        public static string BulkDelete(params string[] data) { return Get("BulkDelete", data); }
        public static HtmlBuilder Displays_Import(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Import", data)); }
        public static string Import(params string[] data) { return Get("Import", data); }
        public static HtmlBuilder Displays_Export(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Export", data)); }
        public static string Export(params string[] data) { return Get("Export", data); }
        public static HtmlBuilder Displays_Cancel(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Cancel", data)); }
        public static string Cancel(params string[] data) { return Get("Cancel", data); }
        public static HtmlBuilder Displays_GoBack(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("GoBack", data)); }
        public static string GoBack(params string[] data) { return Get("GoBack", data); }
        public static HtmlBuilder Displays_Synchronize(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Synchronize", data)); }
        public static string Synchronize(params string[] data) { return Get("Synchronize", data); }
        public static HtmlBuilder Displays_Operations(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Operations", data)); }
        public static string Operations(params string[] data) { return Get("Operations", data); }
        public static HtmlBuilder Displays_Date(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Date", data)); }
        public static string Date(params string[] data) { return Get("Date", data); }
        public static HtmlBuilder Displays_Count(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Count", data)); }
        public static string Count(params string[] data) { return Get("Count", data); }
        public static HtmlBuilder Displays_Total(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Total", data)); }
        public static string Total(params string[] data) { return Get("Total", data); }
        public static HtmlBuilder Displays_Average(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Average", data)); }
        public static string Average(params string[] data) { return Get("Average", data); }
        public static HtmlBuilder Displays_Max(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Max", data)); }
        public static string Max(params string[] data) { return Get("Max", data); }
        public static HtmlBuilder Displays_Min(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Min", data)); }
        public static string Min(params string[] data) { return Get("Min", data); }
        public static HtmlBuilder Displays_Step(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Step", data)); }
        public static string Step(params string[] data) { return Get("Step", data); }
        public static HtmlBuilder Displays_DecimalPlaces(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("DecimalPlaces", data)); }
        public static string DecimalPlaces(params string[] data) { return Get("DecimalPlaces", data); }
        public static HtmlBuilder Displays_ToUpper(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ToUpper", data)); }
        public static string ToUpper(params string[] data) { return Get("ToUpper", data); }
        public static HtmlBuilder Displays_Up(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Up", data)); }
        public static string Up(params string[] data) { return Get("Up", data); }
        public static HtmlBuilder Displays_Down(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Down", data)); }
        public static string Down(params string[] data) { return Get("Down", data); }
        public static HtmlBuilder Displays_Previous(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Previous", data)); }
        public static string Previous(params string[] data) { return Get("Previous", data); }
        public static HtmlBuilder Displays_Next(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Next", data)); }
        public static string Next(params string[] data) { return Get("Next", data); }
        public static HtmlBuilder Displays_Reload(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Reload", data)); }
        public static string Reload(params string[] data) { return Get("Reload", data); }
        public static HtmlBuilder Displays_Latest(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Latest", data)); }
        public static string Latest(params string[] data) { return Get("Latest", data); }
        public static HtmlBuilder Displays_Visible(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Visible", data)); }
        public static string Visible(params string[] data) { return Get("Visible", data); }
        public static HtmlBuilder Displays_Hide(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Hide", data)); }
        public static string Hide(params string[] data) { return Get("Hide", data); }
        public static HtmlBuilder Displays_PlannedValue(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("PlannedValue", data)); }
        public static string PlannedValue(params string[] data) { return Get("PlannedValue", data); }
        public static HtmlBuilder Displays_EarnedValue(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("EarnedValue", data)); }
        public static string EarnedValue(params string[] data) { return Get("EarnedValue", data); }
        public static HtmlBuilder Displays_Difference(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Difference", data)); }
        public static string Difference(params string[] data) { return Get("Difference", data); }
        public static HtmlBuilder Displays_ChangePassword(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ChangePassword", data)); }
        public static string ChangePassword(params string[] data) { return Get("ChangePassword", data); }
        public static HtmlBuilder Displays_ResetPassword(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ResetPassword", data)); }
        public static string ResetPassword(params string[] data) { return Get("ResetPassword", data); }
        public static HtmlBuilder Displays_ReadOnly(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ReadOnly", data)); }
        public static string ReadOnly(params string[] data) { return Get("ReadOnly", data); }
        public static HtmlBuilder Displays_ReadWrite(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ReadWrite", data)); }
        public static string ReadWrite(params string[] data) { return Get("ReadWrite", data); }
        public static HtmlBuilder Displays_Leader(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Leader", data)); }
        public static string Leader(params string[] data) { return Get("Leader", data); }
        public static HtmlBuilder Displays_Manager(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Manager", data)); }
        public static string Manager(params string[] data) { return Get("Manager", data); }
        public static HtmlBuilder Displays_DeletePermission(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("DeletePermission", data)); }
        public static string DeletePermission(params string[] data) { return Get("DeletePermission", data); }
        public static HtmlBuilder Displays_AddPermission(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("AddPermission", data)); }
        public static string AddPermission(params string[] data) { return Get("AddPermission", data); }
        public static HtmlBuilder Displays_NotInheritPermission(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NotInheritPermission", data)); }
        public static string NotInheritPermission(params string[] data) { return Get("NotInheritPermission", data); }
        public static HtmlBuilder Displays_NoTitle(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NoTitle", data)); }
        public static string NoTitle(params string[] data) { return Get("NoTitle", data); }
        public static HtmlBuilder Displays_Error(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Error", data)); }
        public static string Error(params string[] data) { return Get("Error", data); }
        public static HtmlBuilder Displays_Index(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Index", data)); }
        public static string Index(params string[] data) { return Get("Index", data); }
        public static HtmlBuilder Displays_Edit(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Edit", data)); }
        public static string Edit(params string[] data) { return Get("Edit", data); }
        public static HtmlBuilder Displays_History(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("History", data)); }
        public static string History(params string[] data) { return Get("History", data); }
        public static HtmlBuilder Displays_HistoryList(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("HistoryList", data)); }
        public static string HistoryList(params string[] data) { return Get("HistoryList", data); }
        public static HtmlBuilder Displays_NextRecord(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NextRecord", data)); }
        public static string NextRecord(params string[] data) { return Get("NextRecord", data); }
        public static HtmlBuilder Displays_PreviousRecord(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("PreviousRecord", data)); }
        public static string PreviousRecord(params string[] data) { return Get("PreviousRecord", data); }
        public static HtmlBuilder Displays_Links(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links", data)); }
        public static string Links(params string[] data) { return Get("Links", data); }
        public static HtmlBuilder Displays_LinkDestinations(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LinkDestinations", data)); }
        public static string LinkDestinations(params string[] data) { return Get("LinkDestinations", data); }
        public static HtmlBuilder Displays_LinkSources(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LinkSources", data)); }
        public static string LinkSources(params string[] data) { return Get("LinkSources", data); }
        public static HtmlBuilder Displays_LinkCreations(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LinkCreations", data)); }
        public static string LinkCreations(params string[] data) { return Get("LinkCreations", data); }
        public static HtmlBuilder Displays_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Comments", data)); }
        public static string Comments(params string[] data) { return Get("Comments", data); }
        public static HtmlBuilder Displays_SentMail(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SentMail", data)); }
        public static string SentMail(params string[] data) { return Get("SentMail", data); }
        public static HtmlBuilder Displays_Reply(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Reply", data)); }
        public static string Reply(params string[] data) { return Get("Reply", data); }
        public static HtmlBuilder Displays_OriginalMessage(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OriginalMessage", data)); }
        public static string OriginalMessage(params string[] data) { return Get("OriginalMessage", data); }
        public static HtmlBuilder Displays_Logout(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Logout", data)); }
        public static string Logout(params string[] data) { return Get("Logout", data); }
        public static HtmlBuilder Displays_EditProfile(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("EditProfile", data)); }
        public static string EditProfile(params string[] data) { return Get("EditProfile", data); }
        public static HtmlBuilder Displays_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CreatedTime", data)); }
        public static string CreatedTime(params string[] data) { return Get("CreatedTime", data); }
        public static HtmlBuilder Displays_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("UpdatedTime", data)); }
        public static string UpdatedTime(params string[] data) { return Get("UpdatedTime", data); }
        public static HtmlBuilder Displays_PermissionDestination(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("PermissionDestination", data)); }
        public static string PermissionDestination(params string[] data) { return Get("PermissionDestination", data); }
        public static HtmlBuilder Displays_PermissionSource(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("PermissionSource", data)); }
        public static string PermissionSource(params string[] data) { return Get("PermissionSource", data); }
        public static HtmlBuilder Displays_SettingColumnList(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingColumnList", data)); }
        public static string SettingColumnList(params string[] data) { return Get("SettingColumnList", data); }
        public static HtmlBuilder Displays_SettingTitleColumn(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingTitleColumn", data)); }
        public static string SettingTitleColumn(params string[] data) { return Get("SettingTitleColumn", data); }
        public static HtmlBuilder Displays_SettingTitleSeparator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingTitleSeparator", data)); }
        public static string SettingTitleSeparator(params string[] data) { return Get("SettingTitleSeparator", data); }
        public static HtmlBuilder Displays_SettingGroupByList(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingGroupByList", data)); }
        public static string SettingGroupByList(params string[] data) { return Get("SettingGroupByList", data); }
        public static HtmlBuilder Displays_SettingNotGroupBy(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingNotGroupBy", data)); }
        public static string SettingNotGroupBy(params string[] data) { return Get("SettingNotGroupBy", data); }
        public static HtmlBuilder Displays_SettingNearDeadlineBeforeDays(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingNearDeadlineBeforeDays", data)); }
        public static string SettingNearDeadlineBeforeDays(params string[] data) { return Get("SettingNearDeadlineBeforeDays", data); }
        public static HtmlBuilder Displays_SettingNearDeadlineAfterDays(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingNearDeadlineAfterDays", data)); }
        public static string SettingNearDeadlineAfterDays(params string[] data) { return Get("SettingNearDeadlineAfterDays", data); }
        public static HtmlBuilder Displays_SettingGridPageSize(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingGridPageSize", data)); }
        public static string SettingGridPageSize(params string[] data) { return Get("SettingGridPageSize", data); }
        public static HtmlBuilder Displays_SettingLabel(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingLabel", data)); }
        public static string SettingLabel(params string[] data) { return Get("SettingLabel", data); }
        public static HtmlBuilder Displays_SettingEnable(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingEnable", data)); }
        public static string SettingEnable(params string[] data) { return Get("SettingEnable", data); }
        public static HtmlBuilder Displays_SettingOrder(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingOrder", data)); }
        public static string SettingOrder(params string[] data) { return Get("SettingOrder", data); }
        public static HtmlBuilder Displays_SettingUnit(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingUnit", data)); }
        public static string SettingUnit(params string[] data) { return Get("SettingUnit", data); }
        public static HtmlBuilder Displays_SettingControlDateTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingControlDateTime", data)); }
        public static string SettingControlDateTime(params string[] data) { return Get("SettingControlDateTime", data); }
        public static HtmlBuilder Displays_SettingGridDateTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingGridDateTime", data)); }
        public static string SettingGridDateTime(params string[] data) { return Get("SettingGridDateTime", data); }
        public static HtmlBuilder Displays_SettingSelectionList(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingSelectionList", data)); }
        public static string SettingSelectionList(params string[] data) { return Get("SettingSelectionList", data); }
        public static HtmlBuilder Displays_SettingChoicesVisible(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingChoicesVisible", data)); }
        public static string SettingChoicesVisible(params string[] data) { return Get("SettingChoicesVisible", data); }
        public static HtmlBuilder Displays_SettingLimitDefault(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingLimitDefault", data)); }
        public static string SettingLimitDefault(params string[] data) { return Get("SettingLimitDefault", data); }
        public static HtmlBuilder Displays_SettingColumns(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingColumns", data)); }
        public static string SettingColumns(params string[] data) { return Get("SettingColumns", data); }
        public static HtmlBuilder Displays_SettingAggregations(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingAggregations", data)); }
        public static string SettingAggregations(params string[] data) { return Get("SettingAggregations", data); }
        public static HtmlBuilder Displays_SettingAggregationType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingAggregationType", data)); }
        public static string SettingAggregationType(params string[] data) { return Get("SettingAggregationType", data); }
        public static HtmlBuilder Displays_SettingAggregationTarget(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SettingAggregationTarget", data)); }
        public static string SettingAggregationTarget(params string[] data) { return Get("SettingAggregationTarget", data); }
        public static HtmlBuilder Displays_CurrentPassword(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CurrentPassword", data)); }
        public static string CurrentPassword(params string[] data) { return Get("CurrentPassword", data); }
        public static HtmlBuilder Displays_ReEnter(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ReEnter", data)); }
        public static string ReEnter(params string[] data) { return Get("ReEnter", data); }
        public static HtmlBuilder Displays_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("VerUp", data)); }
        public static string VerUp(params string[] data) { return Get("VerUp", data); }
        public static HtmlBuilder Displays_Hidden(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Hidden", data)); }
        public static string Hidden(params string[] data) { return Get("Hidden", data); }
        public static HtmlBuilder Displays_Output(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Output", data)); }
        public static string Output(params string[] data) { return Get("Output", data); }
        public static HtmlBuilder Displays_NotOutput(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NotOutput", data)); }
        public static string NotOutput(params string[] data) { return Get("NotOutput", data); }
        public static HtmlBuilder Displays_CopyWithComments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CopyWithComments", data)); }
        public static string CopyWithComments(params string[] data) { return Get("CopyWithComments", data); }
        public static HtmlBuilder Displays_Hyphen(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Hyphen", data)); }
        public static string Hyphen(params string[] data) { return Get("Hyphen", data); }
        public static HtmlBuilder Displays_Search(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Search", data)); }
        public static string Search(params string[] data) { return Get("Search", data); }
        public static HtmlBuilder Displays_Admin(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Admin", data)); }
        public static string Admin(params string[] data) { return Get("Admin", data); }
        public static HtmlBuilder Displays_AddressBook(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("AddressBook", data)); }
        public static string AddressBook(params string[] data) { return Get("AddressBook", data); }
        public static HtmlBuilder Displays_DefaultDestinations(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("DefaultDestinations", data)); }
        public static string DefaultDestinations(params string[] data) { return Get("DefaultDestinations", data); }
        public static HtmlBuilder Displays_LimitJust(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitJust", data)); }
        public static string LimitJust(params string[] data) { return Get("LimitJust", data); }
        public static HtmlBuilder Displays_LimitAfterSecond(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterSecond", data)); }
        public static string LimitAfterSecond(params string[] data) { return Get("LimitAfterSecond", data); }
        public static HtmlBuilder Displays_LimitAfterSeconds(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterSeconds", data)); }
        public static string LimitAfterSeconds(params string[] data) { return Get("LimitAfterSeconds", data); }
        public static HtmlBuilder Displays_LimitAfterMinute(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterMinute", data)); }
        public static string LimitAfterMinute(params string[] data) { return Get("LimitAfterMinute", data); }
        public static HtmlBuilder Displays_LimitAfterMinutes(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterMinutes", data)); }
        public static string LimitAfterMinutes(params string[] data) { return Get("LimitAfterMinutes", data); }
        public static HtmlBuilder Displays_LimitAfterHour(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterHour", data)); }
        public static string LimitAfterHour(params string[] data) { return Get("LimitAfterHour", data); }
        public static HtmlBuilder Displays_LimitAfterHours(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterHours", data)); }
        public static string LimitAfterHours(params string[] data) { return Get("LimitAfterHours", data); }
        public static HtmlBuilder Displays_LimitAfterDay(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterDay", data)); }
        public static string LimitAfterDay(params string[] data) { return Get("LimitAfterDay", data); }
        public static HtmlBuilder Displays_LimitAfterDays(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterDays", data)); }
        public static string LimitAfterDays(params string[] data) { return Get("LimitAfterDays", data); }
        public static HtmlBuilder Displays_LimitAfterMonth(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterMonth", data)); }
        public static string LimitAfterMonth(params string[] data) { return Get("LimitAfterMonth", data); }
        public static HtmlBuilder Displays_LimitAfterMonths(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterMonths", data)); }
        public static string LimitAfterMonths(params string[] data) { return Get("LimitAfterMonths", data); }
        public static HtmlBuilder Displays_LimitAfterYear(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterYear", data)); }
        public static string LimitAfterYear(params string[] data) { return Get("LimitAfterYear", data); }
        public static HtmlBuilder Displays_LimitAfterYears(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitAfterYears", data)); }
        public static string LimitAfterYears(params string[] data) { return Get("LimitAfterYears", data); }
        public static HtmlBuilder Displays_LimitBeforeSecond(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeSecond", data)); }
        public static string LimitBeforeSecond(params string[] data) { return Get("LimitBeforeSecond", data); }
        public static HtmlBuilder Displays_LimitBeforeSeconds(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeSeconds", data)); }
        public static string LimitBeforeSeconds(params string[] data) { return Get("LimitBeforeSeconds", data); }
        public static HtmlBuilder Displays_LimitBeforeMinute(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeMinute", data)); }
        public static string LimitBeforeMinute(params string[] data) { return Get("LimitBeforeMinute", data); }
        public static HtmlBuilder Displays_LimitBeforeMinutes(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeMinutes", data)); }
        public static string LimitBeforeMinutes(params string[] data) { return Get("LimitBeforeMinutes", data); }
        public static HtmlBuilder Displays_LimitBeforeHour(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeHour", data)); }
        public static string LimitBeforeHour(params string[] data) { return Get("LimitBeforeHour", data); }
        public static HtmlBuilder Displays_LimitBeforeHours(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeHours", data)); }
        public static string LimitBeforeHours(params string[] data) { return Get("LimitBeforeHours", data); }
        public static HtmlBuilder Displays_LimitBeforeDay(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeDay", data)); }
        public static string LimitBeforeDay(params string[] data) { return Get("LimitBeforeDay", data); }
        public static HtmlBuilder Displays_LimitBeforeDays(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeDays", data)); }
        public static string LimitBeforeDays(params string[] data) { return Get("LimitBeforeDays", data); }
        public static HtmlBuilder Displays_LimitBeforeMonth(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeMonth", data)); }
        public static string LimitBeforeMonth(params string[] data) { return Get("LimitBeforeMonth", data); }
        public static HtmlBuilder Displays_LimitBeforeMonths(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeMonths", data)); }
        public static string LimitBeforeMonths(params string[] data) { return Get("LimitBeforeMonths", data); }
        public static HtmlBuilder Displays_LimitBeforeYear(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeYear", data)); }
        public static string LimitBeforeYear(params string[] data) { return Get("LimitBeforeYear", data); }
        public static HtmlBuilder Displays_LimitBeforeYears(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LimitBeforeYears", data)); }
        public static string LimitBeforeYears(params string[] data) { return Get("LimitBeforeYears", data); }
        public static HtmlBuilder Displays_WriteComment(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("WriteComment", data)); }
        public static string WriteComment(params string[] data) { return Get("WriteComment", data); }
        public static HtmlBuilder Displays_Filters(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Filters", data)); }
        public static string Filters(params string[] data) { return Get("Filters", data); }
        public static HtmlBuilder Displays_Incomplete(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Incomplete", data)); }
        public static string Incomplete(params string[] data) { return Get("Incomplete", data); }
        public static HtmlBuilder Displays_Blank(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Blank", data)); }
        public static string Blank(params string[] data) { return Get("Blank", data); }
        public static HtmlBuilder Displays_Own(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Own", data)); }
        public static string Own(params string[] data) { return Get("Own", data); }
        public static HtmlBuilder Displays_NearDeadline(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NearDeadline", data)); }
        public static string NearDeadline(params string[] data) { return Get("NearDeadline", data); }
        public static HtmlBuilder Displays_Delay(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Delay", data)); }
        public static string Delay(params string[] data) { return Get("Delay", data); }
        public static HtmlBuilder Displays_Overdue(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Overdue", data)); }
        public static string Overdue(params string[] data) { return Get("Overdue", data); }
        public static HtmlBuilder Displays_OrderAsc(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OrderAsc", data)); }
        public static string OrderAsc(params string[] data) { return Get("OrderAsc", data); }
        public static HtmlBuilder Displays_OrderDesc(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OrderDesc", data)); }
        public static string OrderDesc(params string[] data) { return Get("OrderDesc", data); }
        public static HtmlBuilder Displays_OrderRelease(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OrderRelease", data)); }
        public static string OrderRelease(params string[] data) { return Get("OrderRelease", data); }
        public static HtmlBuilder Displays_ResetOrder(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ResetOrder", data)); }
        public static string ResetOrder(params string[] data) { return Get("ResetOrder", data); }
        public static HtmlBuilder Displays_Csv(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Csv", data)); }
        public static string Csv(params string[] data) { return Get("Csv", data); }
        public static HtmlBuilder Displays_CsvFile(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CsvFile", data)); }
        public static string CsvFile(params string[] data) { return Get("CsvFile", data); }
        public static HtmlBuilder Displays_CharacterCode(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CharacterCode", data)); }
        public static string CharacterCode(params string[] data) { return Get("CharacterCode", data); }
        public static HtmlBuilder Displays_Excel(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Excel", data)); }
        public static string Excel(params string[] data) { return Get("Excel", data); }
        public static HtmlBuilder Displays_Setting(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Setting", data)); }
        public static string Setting(params string[] data) { return Get("Setting", data); }
        public static HtmlBuilder Displays_AdvancedSetting(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("AdvancedSetting", data)); }
        public static string AdvancedSetting(params string[] data) { return Get("AdvancedSetting", data); }
        public static HtmlBuilder Displays_Basic(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Basic", data)); }
        public static string Basic(params string[] data) { return Get("Basic", data); }
        public static HtmlBuilder Displays_Style(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Style", data)); }
        public static string Style(params string[] data) { return Get("Style", data); }
        public static HtmlBuilder Displays_Normal(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Normal", data)); }
        public static string Normal(params string[] data) { return Get("Normal", data); }
        public static HtmlBuilder Displays_Wide(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wide", data)); }
        public static string Wide(params string[] data) { return Get("Wide", data); }
        public static HtmlBuilder Displays_Auto(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Auto", data)); }
        public static string Auto(params string[] data) { return Get("Auto", data); }
        public static HtmlBuilder Displays_ControlType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ControlType", data)); }
        public static string ControlType(params string[] data) { return Get("ControlType", data); }
        public static HtmlBuilder Displays_Spinner(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Spinner", data)); }
        public static string Spinner(params string[] data) { return Get("Spinner", data); }
        public static HtmlBuilder Displays_SiteImageSettingsEditor(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SiteImageSettingsEditor", data)); }
        public static string SiteImageSettingsEditor(params string[] data) { return Get("SiteImageSettingsEditor", data); }
        public static HtmlBuilder Displays_GridSettingsEditor(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("GridSettingsEditor", data)); }
        public static string GridSettingsEditor(params string[] data) { return Get("GridSettingsEditor", data); }
        public static HtmlBuilder Displays_EditorSettingsEditor(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("EditorSettingsEditor", data)); }
        public static string EditorSettingsEditor(params string[] data) { return Get("EditorSettingsEditor", data); }
        public static HtmlBuilder Displays_MailerSettingsEditor(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailerSettingsEditor", data)); }
        public static string MailerSettingsEditor(params string[] data) { return Get("MailerSettingsEditor", data); }
        public static HtmlBuilder Displays_SummarySettingsEditor(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SummarySettingsEditor", data)); }
        public static string SummarySettingsEditor(params string[] data) { return Get("SummarySettingsEditor", data); }
        public static HtmlBuilder Displays_PermissionSetting(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("PermissionSetting", data)); }
        public static string PermissionSetting(params string[] data) { return Get("PermissionSetting", data); }
        public static HtmlBuilder Displays_EditPermissions(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("EditPermissions", data)); }
        public static string EditPermissions(params string[] data) { return Get("EditPermissions", data); }
        public static HtmlBuilder Displays_CopySettings(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CopySettings", data)); }
        public static string CopySettings(params string[] data) { return Get("CopySettings", data); }
        public static HtmlBuilder Displays_AggregationDetails(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("AggregationDetails", data)); }
        public static string AggregationDetails(params string[] data) { return Get("AggregationDetails", data); }
        public static HtmlBuilder Displays_MoveSettings(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MoveSettings", data)); }
        public static string MoveSettings(params string[] data) { return Get("MoveSettings", data); }
        public static HtmlBuilder Displays_Destination(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Destination", data)); }
        public static string Destination(params string[] data) { return Get("Destination", data); }
        public static HtmlBuilder Displays_SeparateSettings(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SeparateSettings", data)); }
        public static string SeparateSettings(params string[] data) { return Get("SeparateSettings", data); }
        public static HtmlBuilder Displays_SummarySiteId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SummarySiteId", data)); }
        public static string SummarySiteId(params string[] data) { return Get("SummarySiteId", data); }
        public static HtmlBuilder Displays_SummaryDestinationColumn(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SummaryDestinationColumn", data)); }
        public static string SummaryDestinationColumn(params string[] data) { return Get("SummaryDestinationColumn", data); }
        public static HtmlBuilder Displays_SummaryLinkColumn(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SummaryLinkColumn", data)); }
        public static string SummaryLinkColumn(params string[] data) { return Get("SummaryLinkColumn", data); }
        public static HtmlBuilder Displays_SummaryType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SummaryType", data)); }
        public static string SummaryType(params string[] data) { return Get("SummaryType", data); }
        public static HtmlBuilder Displays_SummarySourceColumn(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SummarySourceColumn", data)); }
        public static string SummarySourceColumn(params string[] data) { return Get("SummarySourceColumn", data); }
        public static HtmlBuilder Displays_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Title", data)); }
        public static string Title(params string[] data) { return Get("Title", data); }
        public static HtmlBuilder Displays_WorkValue(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("WorkValue", data)); }
        public static string WorkValue(params string[] data) { return Get("WorkValue", data); }
        public static HtmlBuilder Displays_SeparateNumber(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SeparateNumber", data)); }
        public static string SeparateNumber(params string[] data) { return Get("SeparateNumber", data); }
        public static HtmlBuilder Displays_OutgoingMail(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMail", data)); }
        public static string OutgoingMail(params string[] data) { return Get("OutgoingMail", data); }
        public static HtmlBuilder Displays_Icon(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Icon", data)); }
        public static string Icon(params string[] data) { return Get("Icon", data); }
        public static HtmlBuilder Displays_File(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("File", data)); }
        public static string File(params string[] data) { return Get("File", data); }
        public static HtmlBuilder Displays_NotSet(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NotSet", data)); }
        public static string NotSet(params string[] data) { return Get("NotSet", data); }
        public static HtmlBuilder Displays_Internal(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Internal", data)); }
        public static string Internal(params string[] data) { return Get("Internal", data); }
        public static HtmlBuilder Displays_All(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("All", data)); }
        public static string All(params string[] data) { return Get("All", data); }
        public static HtmlBuilder Displays_Quantity(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Quantity", data)); }
        public static string Quantity(params string[] data) { return Get("Quantity", data); }
        public static HtmlBuilder Displays_SuffixCopy(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SuffixCopy", data)); }
        public static string SuffixCopy(params string[] data) { return Get("SuffixCopy", data); }
        public static HtmlBuilder Displays_DataViewSelector(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("DataViewSelector", data)); }
        public static string DataViewSelector(params string[] data) { return Get("DataViewSelector", data); }
        public static HtmlBuilder Displays_Grid(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Grid", data)); }
        public static string Grid(params string[] data) { return Get("Grid", data); }
        public static HtmlBuilder Displays_BurnDown(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("BurnDown", data)); }
        public static string BurnDown(params string[] data) { return Get("BurnDown", data); }
        public static HtmlBuilder Displays_Gantt(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Gantt", data)); }
        public static string Gantt(params string[] data) { return Get("Gantt", data); }
        public static HtmlBuilder Displays_NoData(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NoData", data)); }
        public static string NoData(params string[] data) { return Get("NoData", data); }
        public static HtmlBuilder Displays_NoLinks(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NoLinks", data)); }
        public static string NoLinks(params string[] data) { return Get("NoLinks", data); }
        public static HtmlBuilder Displays_Ymd(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Ymd", data)); }
        public static string Ymd(params string[] data) { return Get("Ymd", data); }
        public static HtmlBuilder Displays_Ymda(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Ymda", data)); }
        public static string Ymda(params string[] data) { return Get("Ymda", data); }
        public static HtmlBuilder Displays_Ym(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Ym", data)); }
        public static string Ym(params string[] data) { return Get("Ym", data); }
        public static HtmlBuilder Displays_Ymdhms(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Ymdhms", data)); }
        public static string Ymdhms(params string[] data) { return Get("Ymdhms", data); }
        public static HtmlBuilder Displays_Ymdahms(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Ymdahms", data)); }
        public static string Ymdahms(params string[] data) { return Get("Ymdahms", data); }
        public static HtmlBuilder Displays_Ymdhm(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Ymdhm", data)); }
        public static string Ymdhm(params string[] data) { return Get("Ymdhm", data); }
        public static HtmlBuilder Displays_Ymdahm(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Ymdahm", data)); }
        public static string Ymdahm(params string[] data) { return Get("Ymdahm", data); }
        public static HtmlBuilder Displays_YmdFormat(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("YmdFormat", data)); }
        public static string YmdFormat(params string[] data) { return Get("YmdFormat", data); }
        public static HtmlBuilder Displays_YmdaFormat(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("YmdaFormat", data)); }
        public static string YmdaFormat(params string[] data) { return Get("YmdaFormat", data); }
        public static HtmlBuilder Displays_YmFormat(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("YmFormat", data)); }
        public static string YmFormat(params string[] data) { return Get("YmFormat", data); }
        public static HtmlBuilder Displays_YmdhmsFormat(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("YmdhmsFormat", data)); }
        public static string YmdhmsFormat(params string[] data) { return Get("YmdhmsFormat", data); }
        public static HtmlBuilder Displays_YmdahmsFormat(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("YmdahmsFormat", data)); }
        public static string YmdahmsFormat(params string[] data) { return Get("YmdahmsFormat", data); }
        public static HtmlBuilder Displays_YmdhmFormat(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("YmdhmFormat", data)); }
        public static string YmdhmFormat(params string[] data) { return Get("YmdhmFormat", data); }
        public static HtmlBuilder Displays_YmdahmFormat(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("YmdahmFormat", data)); }
        public static string YmdahmFormat(params string[] data) { return Get("YmdahmFormat", data); }
        public static HtmlBuilder Displays_ExceptionTitle(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExceptionTitle", data)); }
        public static string ExceptionTitle(params string[] data) { return Get("ExceptionTitle", data); }
        public static HtmlBuilder Displays_ExceptionBody(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExceptionBody", data)); }
        public static string ExceptionBody(params string[] data) { return Get("ExceptionBody", data); }
        public static HtmlBuilder Displays_IncorrectArgument(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("IncorrectArgument", data)); }
        public static string IncorrectArgument(params string[] data) { return Get("IncorrectArgument", data); }
        public static HtmlBuilder Displays_InvalidRequest(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("InvalidRequest", data)); }
        public static string InvalidRequest(params string[] data) { return Get("InvalidRequest", data); }
        public static HtmlBuilder Displays_Authentication(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Authentication", data)); }
        public static string Authentication(params string[] data) { return Get("Authentication", data); }
        public static HtmlBuilder Displays_UpdateConflicts(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("UpdateConflicts", data)); }
        public static string UpdateConflicts(params string[] data) { return Get("UpdateConflicts", data); }
        public static HtmlBuilder Displays_DeleteConflicts(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("DeleteConflicts", data)); }
        public static string DeleteConflicts(params string[] data) { return Get("DeleteConflicts", data); }
        public static HtmlBuilder Displays_HasNotPermission(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("HasNotPermission", data)); }
        public static string HasNotPermission(params string[] data) { return Get("HasNotPermission", data); }
        public static HtmlBuilder Displays_IncorrectCurrentPassword(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("IncorrectCurrentPassword", data)); }
        public static string IncorrectCurrentPassword(params string[] data) { return Get("IncorrectCurrentPassword", data); }
        public static HtmlBuilder Displays_PermissionNotSelfChange(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("PermissionNotSelfChange", data)); }
        public static string PermissionNotSelfChange(params string[] data) { return Get("PermissionNotSelfChange", data); }
        public static HtmlBuilder Displays_DefinitionNotFound(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("DefinitionNotFound", data)); }
        public static string DefinitionNotFound(params string[] data) { return Get("DefinitionNotFound", data); }
        public static HtmlBuilder Displays_CantSetAtTopOfSite(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CantSetAtTopOfSite", data)); }
        public static string CantSetAtTopOfSite(params string[] data) { return Get("CantSetAtTopOfSite", data); }
        public static HtmlBuilder Displays_NotFound(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NotFound", data)); }
        public static string NotFound(params string[] data) { return Get("NotFound", data); }
        public static HtmlBuilder Displays_RequireMailAddresses(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("RequireMailAddresses", data)); }
        public static string RequireMailAddresses(params string[] data) { return Get("RequireMailAddresses", data); }
        public static HtmlBuilder Displays_RequireColumn(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("RequireColumn", data)); }
        public static string RequireColumn(params string[] data) { return Get("RequireColumn", data); }
        public static HtmlBuilder Displays_ExternalMailAddress(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExternalMailAddress", data)); }
        public static string ExternalMailAddress(params string[] data) { return Get("ExternalMailAddress", data); }
        public static HtmlBuilder Displays_BadMailAddress(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("BadMailAddress", data)); }
        public static string BadMailAddress(params string[] data) { return Get("BadMailAddress", data); }
        public static HtmlBuilder Displays_FileNotFound(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("FileNotFound", data)); }
        public static string FileNotFound(params string[] data) { return Get("FileNotFound", data); }
        public static HtmlBuilder Displays_NotRequiredColumn(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("NotRequiredColumn", data)); }
        public static string NotRequiredColumn(params string[] data) { return Get("NotRequiredColumn", data); }
        public static HtmlBuilder Displays_InvalidCsvData(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("InvalidCsvData", data)); }
        public static string InvalidCsvData(params string[] data) { return Get("InvalidCsvData", data); }
        public static HtmlBuilder Displays_FailedReadFile(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("FailedReadFile", data)); }
        public static string FailedReadFile(params string[] data) { return Get("FailedReadFile", data); }
        public static HtmlBuilder Displays_CanNotHide(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CanNotHide", data)); }
        public static string CanNotHide(params string[] data) { return Get("CanNotHide", data); }
        public static HtmlBuilder Displays_AlreadyAdded(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("AlreadyAdded", data)); }
        public static string AlreadyAdded(params string[] data) { return Get("AlreadyAdded", data); }
        public static HtmlBuilder Displays_SelectTargets(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SelectTargets", data)); }
        public static string SelectTargets(params string[] data) { return Get("SelectTargets", data); }
        public static HtmlBuilder Displays_LoginIn(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("LoginIn", data)); }
        public static string LoginIn(params string[] data) { return Get("LoginIn", data); }
        public static HtmlBuilder Displays_Deleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Deleted", data)); }
        public static string Deleted(params string[] data) { return Get("Deleted", data); }
        public static HtmlBuilder Displays_BulkMoved(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("BulkMoved", data)); }
        public static string BulkMoved(params string[] data) { return Get("BulkMoved", data); }
        public static HtmlBuilder Displays_BulkDeleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("BulkDeleted", data)); }
        public static string BulkDeleted(params string[] data) { return Get("BulkDeleted", data); }
        public static HtmlBuilder Displays_Separated(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Separated", data)); }
        public static string Separated(params string[] data) { return Get("Separated", data); }
        public static HtmlBuilder Displays_Imported(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Imported", data)); }
        public static string Imported(params string[] data) { return Get("Imported", data); }
        public static HtmlBuilder Displays_Created(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Created", data)); }
        public static string Created(params string[] data) { return Get("Created", data); }
        public static HtmlBuilder Displays_Updated(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Updated", data)); }
        public static string Updated(params string[] data) { return Get("Updated", data); }
        public static HtmlBuilder Displays_CodeDefinerCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CodeDefinerCompleted", data)); }
        public static string CodeDefinerCompleted(params string[] data) { return Get("CodeDefinerCompleted", data); }
        public static HtmlBuilder Displays_CodeDefinerDbCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CodeDefinerDbCompleted", data)); }
        public static string CodeDefinerDbCompleted(params string[] data) { return Get("CodeDefinerDbCompleted", data); }
        public static HtmlBuilder Displays_CodeDefinerDefCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CodeDefinerDefCompleted", data)); }
        public static string CodeDefinerDefCompleted(params string[] data) { return Get("CodeDefinerDefCompleted", data); }
        public static HtmlBuilder Displays_CodeDefinerMvcCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CodeDefinerMvcCompleted", data)); }
        public static string CodeDefinerMvcCompleted(params string[] data) { return Get("CodeDefinerMvcCompleted", data); }
        public static HtmlBuilder Displays_CodeDefinerCssCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CodeDefinerCssCompleted", data)); }
        public static string CodeDefinerCssCompleted(params string[] data) { return Get("CodeDefinerCssCompleted", data); }
        public static HtmlBuilder Displays_CodeDefinerBackupCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CodeDefinerBackupCompleted", data)); }
        public static string CodeDefinerBackupCompleted(params string[] data) { return Get("CodeDefinerBackupCompleted", data); }
        public static HtmlBuilder Displays_MailTransmissionCompletion(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailTransmissionCompletion", data)); }
        public static string MailTransmissionCompletion(params string[] data) { return Get("MailTransmissionCompletion", data); }
        public static HtmlBuilder Displays_ChangingPasswordComplete(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ChangingPasswordComplete", data)); }
        public static string ChangingPasswordComplete(params string[] data) { return Get("ChangingPasswordComplete", data); }
        public static HtmlBuilder Displays_PasswordResetCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("PasswordResetCompleted", data)); }
        public static string PasswordResetCompleted(params string[] data) { return Get("PasswordResetCompleted", data); }
        public static HtmlBuilder Displays_FileUpdateCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("FileUpdateCompleted", data)); }
        public static string FileUpdateCompleted(params string[] data) { return Get("FileUpdateCompleted", data); }
        public static HtmlBuilder Displays_SynchronizationCompleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SynchronizationCompleted", data)); }
        public static string SynchronizationCompleted(params string[] data) { return Get("SynchronizationCompleted", data); }
        public static HtmlBuilder Displays_ConfirmDelete(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ConfirmDelete", data)); }
        public static string ConfirmDelete(params string[] data) { return Get("ConfirmDelete", data); }
        public static HtmlBuilder Displays_ConfirmSeparate(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ConfirmSeparate", data)); }
        public static string ConfirmSeparate(params string[] data) { return Get("ConfirmSeparate", data); }
        public static HtmlBuilder Displays_ConfirmSendMail(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ConfirmSendMail", data)); }
        public static string ConfirmSendMail(params string[] data) { return Get("ConfirmSendMail", data); }
        public static HtmlBuilder Displays_ConfirmSynchronize(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ConfirmSynchronize", data)); }
        public static string ConfirmSynchronize(params string[] data) { return Get("ConfirmSynchronize", data); }
        public static HtmlBuilder Displays_CanNotUpdate(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("CanNotUpdate", data)); }
        public static string CanNotUpdate(params string[] data) { return Get("CanNotUpdate", data); }
        public static HtmlBuilder Displays_ReadOnlyBecausePreviousVer(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ReadOnlyBecausePreviousVer", data)); }
        public static string ReadOnlyBecausePreviousVer(params string[] data) { return Get("ReadOnlyBecausePreviousVer", data); }
        public static HtmlBuilder Displays_InCopying(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("InCopying", data)); }
        public static string InCopying(params string[] data) { return Get("InCopying", data); }
        public static HtmlBuilder Displays_InCompression(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("InCompression", data)); }
        public static string InCompression(params string[] data) { return Get("InCompression", data); }
        public static HtmlBuilder Displays_HasBeenMoved(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("HasBeenMoved", data)); }
        public static string HasBeenMoved(params string[] data) { return Get("HasBeenMoved", data); }
        public static HtmlBuilder Displays_HasBeenDeleted(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("HasBeenDeleted", data)); }
        public static string HasBeenDeleted(params string[] data) { return Get("HasBeenDeleted", data); }
        public static HtmlBuilder Displays_ValidateRequired(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ValidateRequired", data)); }
        public static string ValidateRequired(params string[] data) { return Get("ValidateRequired", data); }
        public static HtmlBuilder Displays_ValidateDate(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ValidateDate", data)); }
        public static string ValidateDate(params string[] data) { return Get("ValidateDate", data); }
        public static HtmlBuilder Displays_ValidateMail(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ValidateMail", data)); }
        public static string ValidateMail(params string[] data) { return Get("ValidateMail", data); }
        public static HtmlBuilder Displays_ValidateEqualTo(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ValidateEqualTo", data)); }
        public static string ValidateEqualTo(params string[] data) { return Get("ValidateEqualTo", data); }
        public static HtmlBuilder Displays_Tenants_TenantId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_TenantId", data)); }
        public static string Tenants_TenantId(params string[] data) { return Get("Tenants_TenantId", data); }
        public static HtmlBuilder Displays_Tenants_TenantName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_TenantName", data)); }
        public static string Tenants_TenantName(params string[] data) { return Get("Tenants_TenantName", data); }
        public static HtmlBuilder Displays_Tenants_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_Title", data)); }
        public static string Tenants_Title(params string[] data) { return Get("Tenants_Title", data); }
        public static HtmlBuilder Displays_Tenants_Body(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_Body", data)); }
        public static string Tenants_Body(params string[] data) { return Get("Tenants_Body", data); }
        public static HtmlBuilder Displays_SysLogs_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_CreatedTime", data)); }
        public static string SysLogs_CreatedTime(params string[] data) { return Get("SysLogs_CreatedTime", data); }
        public static HtmlBuilder Displays_SysLogs_SysLogId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_SysLogId", data)); }
        public static string SysLogs_SysLogId(params string[] data) { return Get("SysLogs_SysLogId", data); }
        public static HtmlBuilder Displays_SysLogs_StartTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_StartTime", data)); }
        public static string SysLogs_StartTime(params string[] data) { return Get("SysLogs_StartTime", data); }
        public static HtmlBuilder Displays_SysLogs_EndTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_EndTime", data)); }
        public static string SysLogs_EndTime(params string[] data) { return Get("SysLogs_EndTime", data); }
        public static HtmlBuilder Displays_SysLogs_SysLogType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_SysLogType", data)); }
        public static string SysLogs_SysLogType(params string[] data) { return Get("SysLogs_SysLogType", data); }
        public static HtmlBuilder Displays_SysLogs_OnAzure(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_OnAzure", data)); }
        public static string SysLogs_OnAzure(params string[] data) { return Get("SysLogs_OnAzure", data); }
        public static HtmlBuilder Displays_SysLogs_MachineName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_MachineName", data)); }
        public static string SysLogs_MachineName(params string[] data) { return Get("SysLogs_MachineName", data); }
        public static HtmlBuilder Displays_SysLogs_ServiceName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_ServiceName", data)); }
        public static string SysLogs_ServiceName(params string[] data) { return Get("SysLogs_ServiceName", data); }
        public static HtmlBuilder Displays_SysLogs_TenantName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_TenantName", data)); }
        public static string SysLogs_TenantName(params string[] data) { return Get("SysLogs_TenantName", data); }
        public static HtmlBuilder Displays_SysLogs_Application(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Application", data)); }
        public static string SysLogs_Application(params string[] data) { return Get("SysLogs_Application", data); }
        public static HtmlBuilder Displays_SysLogs_Class(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Class", data)); }
        public static string SysLogs_Class(params string[] data) { return Get("SysLogs_Class", data); }
        public static HtmlBuilder Displays_SysLogs_Method(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Method", data)); }
        public static string SysLogs_Method(params string[] data) { return Get("SysLogs_Method", data); }
        public static HtmlBuilder Displays_SysLogs_RequestData(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_RequestData", data)); }
        public static string SysLogs_RequestData(params string[] data) { return Get("SysLogs_RequestData", data); }
        public static HtmlBuilder Displays_SysLogs_HttpMethod(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_HttpMethod", data)); }
        public static string SysLogs_HttpMethod(params string[] data) { return Get("SysLogs_HttpMethod", data); }
        public static HtmlBuilder Displays_SysLogs_RequestSize(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_RequestSize", data)); }
        public static string SysLogs_RequestSize(params string[] data) { return Get("SysLogs_RequestSize", data); }
        public static HtmlBuilder Displays_SysLogs_ResponseSize(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_ResponseSize", data)); }
        public static string SysLogs_ResponseSize(params string[] data) { return Get("SysLogs_ResponseSize", data); }
        public static HtmlBuilder Displays_SysLogs_Elapsed(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Elapsed", data)); }
        public static string SysLogs_Elapsed(params string[] data) { return Get("SysLogs_Elapsed", data); }
        public static HtmlBuilder Displays_SysLogs_ApplicationAge(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_ApplicationAge", data)); }
        public static string SysLogs_ApplicationAge(params string[] data) { return Get("SysLogs_ApplicationAge", data); }
        public static HtmlBuilder Displays_SysLogs_ApplicationRequestInterval(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_ApplicationRequestInterval", data)); }
        public static string SysLogs_ApplicationRequestInterval(params string[] data) { return Get("SysLogs_ApplicationRequestInterval", data); }
        public static HtmlBuilder Displays_SysLogs_SessionAge(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_SessionAge", data)); }
        public static string SysLogs_SessionAge(params string[] data) { return Get("SysLogs_SessionAge", data); }
        public static HtmlBuilder Displays_SysLogs_SessionRequestInterval(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_SessionRequestInterval", data)); }
        public static string SysLogs_SessionRequestInterval(params string[] data) { return Get("SysLogs_SessionRequestInterval", data); }
        public static HtmlBuilder Displays_SysLogs_WorkingSet64(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_WorkingSet64", data)); }
        public static string SysLogs_WorkingSet64(params string[] data) { return Get("SysLogs_WorkingSet64", data); }
        public static HtmlBuilder Displays_SysLogs_VirtualMemorySize64(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_VirtualMemorySize64", data)); }
        public static string SysLogs_VirtualMemorySize64(params string[] data) { return Get("SysLogs_VirtualMemorySize64", data); }
        public static HtmlBuilder Displays_SysLogs_ProcessId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_ProcessId", data)); }
        public static string SysLogs_ProcessId(params string[] data) { return Get("SysLogs_ProcessId", data); }
        public static HtmlBuilder Displays_SysLogs_ProcessName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_ProcessName", data)); }
        public static string SysLogs_ProcessName(params string[] data) { return Get("SysLogs_ProcessName", data); }
        public static HtmlBuilder Displays_SysLogs_BasePriority(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_BasePriority", data)); }
        public static string SysLogs_BasePriority(params string[] data) { return Get("SysLogs_BasePriority", data); }
        public static HtmlBuilder Displays_SysLogs_Url(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Url", data)); }
        public static string SysLogs_Url(params string[] data) { return Get("SysLogs_Url", data); }
        public static HtmlBuilder Displays_SysLogs_UrlReferer(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_UrlReferer", data)); }
        public static string SysLogs_UrlReferer(params string[] data) { return Get("SysLogs_UrlReferer", data); }
        public static HtmlBuilder Displays_SysLogs_UserHostName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_UserHostName", data)); }
        public static string SysLogs_UserHostName(params string[] data) { return Get("SysLogs_UserHostName", data); }
        public static HtmlBuilder Displays_SysLogs_UserHostAddress(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_UserHostAddress", data)); }
        public static string SysLogs_UserHostAddress(params string[] data) { return Get("SysLogs_UserHostAddress", data); }
        public static HtmlBuilder Displays_SysLogs_UserLanguage(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_UserLanguage", data)); }
        public static string SysLogs_UserLanguage(params string[] data) { return Get("SysLogs_UserLanguage", data); }
        public static HtmlBuilder Displays_SysLogs_UserAgent(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_UserAgent", data)); }
        public static string SysLogs_UserAgent(params string[] data) { return Get("SysLogs_UserAgent", data); }
        public static HtmlBuilder Displays_SysLogs_SessionGuid(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_SessionGuid", data)); }
        public static string SysLogs_SessionGuid(params string[] data) { return Get("SysLogs_SessionGuid", data); }
        public static HtmlBuilder Displays_SysLogs_ErrMessage(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_ErrMessage", data)); }
        public static string SysLogs_ErrMessage(params string[] data) { return Get("SysLogs_ErrMessage", data); }
        public static HtmlBuilder Displays_SysLogs_ErrStackTrace(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_ErrStackTrace", data)); }
        public static string SysLogs_ErrStackTrace(params string[] data) { return Get("SysLogs_ErrStackTrace", data); }
        public static HtmlBuilder Displays_SysLogs_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Title", data)); }
        public static string SysLogs_Title(params string[] data) { return Get("SysLogs_Title", data); }
        public static HtmlBuilder Displays_SysLogs_InDebug(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_InDebug", data)); }
        public static string SysLogs_InDebug(params string[] data) { return Get("SysLogs_InDebug", data); }
        public static HtmlBuilder Displays_SysLogs_AssemblyVersion(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_AssemblyVersion", data)); }
        public static string SysLogs_AssemblyVersion(params string[] data) { return Get("SysLogs_AssemblyVersion", data); }
        public static HtmlBuilder Displays_Depts_TenantId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_TenantId", data)); }
        public static string Depts_TenantId(params string[] data) { return Get("Depts_TenantId", data); }
        public static HtmlBuilder Displays_Depts_DeptId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_DeptId", data)); }
        public static string Depts_DeptId(params string[] data) { return Get("Depts_DeptId", data); }
        public static HtmlBuilder Displays_Depts_ParentDeptId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_ParentDeptId", data)); }
        public static string Depts_ParentDeptId(params string[] data) { return Get("Depts_ParentDeptId", data); }
        public static HtmlBuilder Displays_Depts_ParentDept(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_ParentDept", data)); }
        public static string Depts_ParentDept(params string[] data) { return Get("Depts_ParentDept", data); }
        public static HtmlBuilder Displays_Depts_DeptCode(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_DeptCode", data)); }
        public static string Depts_DeptCode(params string[] data) { return Get("Depts_DeptCode", data); }
        public static HtmlBuilder Displays_Depts_Dept(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_Dept", data)); }
        public static string Depts_Dept(params string[] data) { return Get("Depts_Dept", data); }
        public static HtmlBuilder Displays_Depts_DeptName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_DeptName", data)); }
        public static string Depts_DeptName(params string[] data) { return Get("Depts_DeptName", data); }
        public static HtmlBuilder Displays_Depts_Body(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_Body", data)); }
        public static string Depts_Body(params string[] data) { return Get("Depts_Body", data); }
        public static HtmlBuilder Displays_Depts_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_Title", data)); }
        public static string Depts_Title(params string[] data) { return Get("Depts_Title", data); }
        public static HtmlBuilder Displays_Users_TenantId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_TenantId", data)); }
        public static string Users_TenantId(params string[] data) { return Get("Users_TenantId", data); }
        public static HtmlBuilder Displays_Users_UserId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_UserId", data)); }
        public static string Users_UserId(params string[] data) { return Get("Users_UserId", data); }
        public static HtmlBuilder Displays_Users_LoginId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_LoginId", data)); }
        public static string Users_LoginId(params string[] data) { return Get("Users_LoginId", data); }
        public static HtmlBuilder Displays_Users_Disabled(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Disabled", data)); }
        public static string Users_Disabled(params string[] data) { return Get("Users_Disabled", data); }
        public static HtmlBuilder Displays_Users_UserCode(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_UserCode", data)); }
        public static string Users_UserCode(params string[] data) { return Get("Users_UserCode", data); }
        public static HtmlBuilder Displays_Users_Password(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Password", data)); }
        public static string Users_Password(params string[] data) { return Get("Users_Password", data); }
        public static HtmlBuilder Displays_Users_PasswordValidate(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_PasswordValidate", data)); }
        public static string Users_PasswordValidate(params string[] data) { return Get("Users_PasswordValidate", data); }
        public static HtmlBuilder Displays_Users_PasswordDummy(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_PasswordDummy", data)); }
        public static string Users_PasswordDummy(params string[] data) { return Get("Users_PasswordDummy", data); }
        public static HtmlBuilder Displays_Users_RememberMe(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_RememberMe", data)); }
        public static string Users_RememberMe(params string[] data) { return Get("Users_RememberMe", data); }
        public static HtmlBuilder Displays_Users_LastName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_LastName", data)); }
        public static string Users_LastName(params string[] data) { return Get("Users_LastName", data); }
        public static HtmlBuilder Displays_Users_FirstName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_FirstName", data)); }
        public static string Users_FirstName(params string[] data) { return Get("Users_FirstName", data); }
        public static HtmlBuilder Displays_Users_FullName1(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_FullName1", data)); }
        public static string Users_FullName1(params string[] data) { return Get("Users_FullName1", data); }
        public static HtmlBuilder Displays_Users_FullName2(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_FullName2", data)); }
        public static string Users_FullName2(params string[] data) { return Get("Users_FullName2", data); }
        public static HtmlBuilder Displays_Users_Birthday(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Birthday", data)); }
        public static string Users_Birthday(params string[] data) { return Get("Users_Birthday", data); }
        public static HtmlBuilder Displays_Users_Sex(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Sex", data)); }
        public static string Users_Sex(params string[] data) { return Get("Users_Sex", data); }
        public static HtmlBuilder Displays_Users_Language(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Language", data)); }
        public static string Users_Language(params string[] data) { return Get("Users_Language", data); }
        public static HtmlBuilder Displays_Users_TimeZone(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_TimeZone", data)); }
        public static string Users_TimeZone(params string[] data) { return Get("Users_TimeZone", data); }
        public static HtmlBuilder Displays_Users_TimeZoneInfo(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_TimeZoneInfo", data)); }
        public static string Users_TimeZoneInfo(params string[] data) { return Get("Users_TimeZoneInfo", data); }
        public static HtmlBuilder Displays_Users_DeptId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_DeptId", data)); }
        public static string Users_DeptId(params string[] data) { return Get("Users_DeptId", data); }
        public static HtmlBuilder Displays_Users_Dept(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Dept", data)); }
        public static string Users_Dept(params string[] data) { return Get("Users_Dept", data); }
        public static HtmlBuilder Displays_Users_FirstAndLastNameOrder(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_FirstAndLastNameOrder", data)); }
        public static string Users_FirstAndLastNameOrder(params string[] data) { return Get("Users_FirstAndLastNameOrder", data); }
        public static HtmlBuilder Displays_Users_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Title", data)); }
        public static string Users_Title(params string[] data) { return Get("Users_Title", data); }
        public static HtmlBuilder Displays_Users_LastLoginTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_LastLoginTime", data)); }
        public static string Users_LastLoginTime(params string[] data) { return Get("Users_LastLoginTime", data); }
        public static HtmlBuilder Displays_Users_PasswordChangeTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_PasswordChangeTime", data)); }
        public static string Users_PasswordChangeTime(params string[] data) { return Get("Users_PasswordChangeTime", data); }
        public static HtmlBuilder Displays_Users_NumberOfLogins(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_NumberOfLogins", data)); }
        public static string Users_NumberOfLogins(params string[] data) { return Get("Users_NumberOfLogins", data); }
        public static HtmlBuilder Displays_Users_NumberOfDenial(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_NumberOfDenial", data)); }
        public static string Users_NumberOfDenial(params string[] data) { return Get("Users_NumberOfDenial", data); }
        public static HtmlBuilder Displays_Users_TenantAdmin(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_TenantAdmin", data)); }
        public static string Users_TenantAdmin(params string[] data) { return Get("Users_TenantAdmin", data); }
        public static HtmlBuilder Displays_Users_ServiceAdmin(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_ServiceAdmin", data)); }
        public static string Users_ServiceAdmin(params string[] data) { return Get("Users_ServiceAdmin", data); }
        public static HtmlBuilder Displays_Users_Developer(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Developer", data)); }
        public static string Users_Developer(params string[] data) { return Get("Users_Developer", data); }
        public static HtmlBuilder Displays_Users_OldPassword(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_OldPassword", data)); }
        public static string Users_OldPassword(params string[] data) { return Get("Users_OldPassword", data); }
        public static HtmlBuilder Displays_Users_ChangedPassword(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_ChangedPassword", data)); }
        public static string Users_ChangedPassword(params string[] data) { return Get("Users_ChangedPassword", data); }
        public static HtmlBuilder Displays_Users_ChangedPasswordValidation(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_ChangedPasswordValidation", data)); }
        public static string Users_ChangedPasswordValidation(params string[] data) { return Get("Users_ChangedPasswordValidation", data); }
        public static HtmlBuilder Displays_Users_AfterResetPassword(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_AfterResetPassword", data)); }
        public static string Users_AfterResetPassword(params string[] data) { return Get("Users_AfterResetPassword", data); }
        public static HtmlBuilder Displays_Users_AfterResetPasswordValidation(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_AfterResetPasswordValidation", data)); }
        public static string Users_AfterResetPasswordValidation(params string[] data) { return Get("Users_AfterResetPasswordValidation", data); }
        public static HtmlBuilder Displays_Users_SessionGuid(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_SessionGuid", data)); }
        public static string Users_SessionGuid(params string[] data) { return Get("Users_SessionGuid", data); }
        public static HtmlBuilder Displays_MailAddresses_OwnerId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_OwnerId", data)); }
        public static string MailAddresses_OwnerId(params string[] data) { return Get("MailAddresses_OwnerId", data); }
        public static HtmlBuilder Displays_MailAddresses_OwnerType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_OwnerType", data)); }
        public static string MailAddresses_OwnerType(params string[] data) { return Get("MailAddresses_OwnerType", data); }
        public static HtmlBuilder Displays_MailAddresses_MailAddressId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_MailAddressId", data)); }
        public static string MailAddresses_MailAddressId(params string[] data) { return Get("MailAddresses_MailAddressId", data); }
        public static HtmlBuilder Displays_MailAddresses_MailAddress(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_MailAddress", data)); }
        public static string MailAddresses_MailAddress(params string[] data) { return Get("MailAddresses_MailAddress", data); }
        public static HtmlBuilder Displays_MailAddresses_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_Title", data)); }
        public static string MailAddresses_Title(params string[] data) { return Get("MailAddresses_Title", data); }
        public static HtmlBuilder Displays_Permissions_ReferenceType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_ReferenceType", data)); }
        public static string Permissions_ReferenceType(params string[] data) { return Get("Permissions_ReferenceType", data); }
        public static HtmlBuilder Displays_Permissions_ReferenceId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_ReferenceId", data)); }
        public static string Permissions_ReferenceId(params string[] data) { return Get("Permissions_ReferenceId", data); }
        public static HtmlBuilder Displays_Permissions_DeptId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_DeptId", data)); }
        public static string Permissions_DeptId(params string[] data) { return Get("Permissions_DeptId", data); }
        public static HtmlBuilder Displays_Permissions_UserId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_UserId", data)); }
        public static string Permissions_UserId(params string[] data) { return Get("Permissions_UserId", data); }
        public static HtmlBuilder Displays_Permissions_DeptName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_DeptName", data)); }
        public static string Permissions_DeptName(params string[] data) { return Get("Permissions_DeptName", data); }
        public static HtmlBuilder Displays_Permissions_FullName1(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_FullName1", data)); }
        public static string Permissions_FullName1(params string[] data) { return Get("Permissions_FullName1", data); }
        public static HtmlBuilder Displays_Permissions_FullName2(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_FullName2", data)); }
        public static string Permissions_FullName2(params string[] data) { return Get("Permissions_FullName2", data); }
        public static HtmlBuilder Displays_Permissions_FirstAndLastNameOrder(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_FirstAndLastNameOrder", data)); }
        public static string Permissions_FirstAndLastNameOrder(params string[] data) { return Get("Permissions_FirstAndLastNameOrder", data); }
        public static HtmlBuilder Displays_Permissions_PermissionType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_PermissionType", data)); }
        public static string Permissions_PermissionType(params string[] data) { return Get("Permissions_PermissionType", data); }
        public static HtmlBuilder Displays_OutgoingMails_ReferenceType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_ReferenceType", data)); }
        public static string OutgoingMails_ReferenceType(params string[] data) { return Get("OutgoingMails_ReferenceType", data); }
        public static HtmlBuilder Displays_OutgoingMails_ReferenceId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_ReferenceId", data)); }
        public static string OutgoingMails_ReferenceId(params string[] data) { return Get("OutgoingMails_ReferenceId", data); }
        public static HtmlBuilder Displays_OutgoingMails_ReferenceVer(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_ReferenceVer", data)); }
        public static string OutgoingMails_ReferenceVer(params string[] data) { return Get("OutgoingMails_ReferenceVer", data); }
        public static HtmlBuilder Displays_OutgoingMails_OutgoingMailId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_OutgoingMailId", data)); }
        public static string OutgoingMails_OutgoingMailId(params string[] data) { return Get("OutgoingMails_OutgoingMailId", data); }
        public static HtmlBuilder Displays_OutgoingMails_Host(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Host", data)); }
        public static string OutgoingMails_Host(params string[] data) { return Get("OutgoingMails_Host", data); }
        public static HtmlBuilder Displays_OutgoingMails_Port(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Port", data)); }
        public static string OutgoingMails_Port(params string[] data) { return Get("OutgoingMails_Port", data); }
        public static HtmlBuilder Displays_OutgoingMails_From(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_From", data)); }
        public static string OutgoingMails_From(params string[] data) { return Get("OutgoingMails_From", data); }
        public static HtmlBuilder Displays_OutgoingMails_To(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_To", data)); }
        public static string OutgoingMails_To(params string[] data) { return Get("OutgoingMails_To", data); }
        public static HtmlBuilder Displays_OutgoingMails_Cc(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Cc", data)); }
        public static string OutgoingMails_Cc(params string[] data) { return Get("OutgoingMails_Cc", data); }
        public static HtmlBuilder Displays_OutgoingMails_Bcc(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Bcc", data)); }
        public static string OutgoingMails_Bcc(params string[] data) { return Get("OutgoingMails_Bcc", data); }
        public static HtmlBuilder Displays_OutgoingMails_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Title", data)); }
        public static string OutgoingMails_Title(params string[] data) { return Get("OutgoingMails_Title", data); }
        public static HtmlBuilder Displays_OutgoingMails_Body(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Body", data)); }
        public static string OutgoingMails_Body(params string[] data) { return Get("OutgoingMails_Body", data); }
        public static HtmlBuilder Displays_OutgoingMails_SentTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_SentTime", data)); }
        public static string OutgoingMails_SentTime(params string[] data) { return Get("OutgoingMails_SentTime", data); }
        public static HtmlBuilder Displays_OutgoingMails_DestinationSearchRange(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_DestinationSearchRange", data)); }
        public static string OutgoingMails_DestinationSearchRange(params string[] data) { return Get("OutgoingMails_DestinationSearchRange", data); }
        public static HtmlBuilder Displays_OutgoingMails_DestinationSearchText(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_DestinationSearchText", data)); }
        public static string OutgoingMails_DestinationSearchText(params string[] data) { return Get("OutgoingMails_DestinationSearchText", data); }
        public static HtmlBuilder Displays_SearchIndexes_Word(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Word", data)); }
        public static string SearchIndexes_Word(params string[] data) { return Get("SearchIndexes_Word", data); }
        public static HtmlBuilder Displays_SearchIndexes_ReferenceId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_ReferenceId", data)); }
        public static string SearchIndexes_ReferenceId(params string[] data) { return Get("SearchIndexes_ReferenceId", data); }
        public static HtmlBuilder Displays_SearchIndexes_Priority(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Priority", data)); }
        public static string SearchIndexes_Priority(params string[] data) { return Get("SearchIndexes_Priority", data); }
        public static HtmlBuilder Displays_SearchIndexes_ReferenceType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_ReferenceType", data)); }
        public static string SearchIndexes_ReferenceType(params string[] data) { return Get("SearchIndexes_ReferenceType", data); }
        public static HtmlBuilder Displays_SearchIndexes_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Title", data)); }
        public static string SearchIndexes_Title(params string[] data) { return Get("SearchIndexes_Title", data); }
        public static HtmlBuilder Displays_SearchIndexes_Subset(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Subset", data)); }
        public static string SearchIndexes_Subset(params string[] data) { return Get("SearchIndexes_Subset", data); }
        public static HtmlBuilder Displays_SearchIndexes_PermissionType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_PermissionType", data)); }
        public static string SearchIndexes_PermissionType(params string[] data) { return Get("SearchIndexes_PermissionType", data); }
        public static HtmlBuilder Displays_Items_ReferenceId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_ReferenceId", data)); }
        public static string Items_ReferenceId(params string[] data) { return Get("Items_ReferenceId", data); }
        public static HtmlBuilder Displays_Items_ReferenceType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_ReferenceType", data)); }
        public static string Items_ReferenceType(params string[] data) { return Get("Items_ReferenceType", data); }
        public static HtmlBuilder Displays_Items_SiteId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_SiteId", data)); }
        public static string Items_SiteId(params string[] data) { return Get("Items_SiteId", data); }
        public static HtmlBuilder Displays_Items_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_Title", data)); }
        public static string Items_Title(params string[] data) { return Get("Items_Title", data); }
        public static HtmlBuilder Displays_Items_Subset(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_Subset", data)); }
        public static string Items_Subset(params string[] data) { return Get("Items_Subset", data); }
        public static HtmlBuilder Displays_Items_UpdateTarget(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_UpdateTarget", data)); }
        public static string Items_UpdateTarget(params string[] data) { return Get("Items_UpdateTarget", data); }
        public static HtmlBuilder Displays_Items_Site(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_Site", data)); }
        public static string Items_Site(params string[] data) { return Get("Items_Site", data); }
        public static HtmlBuilder Displays_Sites_TenantId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_TenantId", data)); }
        public static string Sites_TenantId(params string[] data) { return Get("Sites_TenantId", data); }
        public static HtmlBuilder Displays_Sites_SiteId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_SiteId", data)); }
        public static string Sites_SiteId(params string[] data) { return Get("Sites_SiteId", data); }
        public static HtmlBuilder Displays_Sites_ReferenceType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_ReferenceType", data)); }
        public static string Sites_ReferenceType(params string[] data) { return Get("Sites_ReferenceType", data); }
        public static HtmlBuilder Displays_Sites_ParentId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_ParentId", data)); }
        public static string Sites_ParentId(params string[] data) { return Get("Sites_ParentId", data); }
        public static HtmlBuilder Displays_Sites_InheritPermission(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_InheritPermission", data)); }
        public static string Sites_InheritPermission(params string[] data) { return Get("Sites_InheritPermission", data); }
        public static HtmlBuilder Displays_Sites_PermissionType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_PermissionType", data)); }
        public static string Sites_PermissionType(params string[] data) { return Get("Sites_PermissionType", data); }
        public static HtmlBuilder Displays_Sites_SiteSettings(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_SiteSettings", data)); }
        public static string Sites_SiteSettings(params string[] data) { return Get("Sites_SiteSettings", data); }
        public static HtmlBuilder Displays_Sites_Ancestors(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_Ancestors", data)); }
        public static string Sites_Ancestors(params string[] data) { return Get("Sites_Ancestors", data); }
        public static HtmlBuilder Displays_Sites_PermissionSourceCollection(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_PermissionSourceCollection", data)); }
        public static string Sites_PermissionSourceCollection(params string[] data) { return Get("Sites_PermissionSourceCollection", data); }
        public static HtmlBuilder Displays_Sites_PermissionDestinationCollection(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_PermissionDestinationCollection", data)); }
        public static string Sites_PermissionDestinationCollection(params string[] data) { return Get("Sites_PermissionDestinationCollection", data); }
        public static HtmlBuilder Displays_Sites_SiteMenu(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_SiteMenu", data)); }
        public static string Sites_SiteMenu(params string[] data) { return Get("Sites_SiteMenu", data); }
        public static HtmlBuilder Displays_Orders_ReferenceId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_ReferenceId", data)); }
        public static string Orders_ReferenceId(params string[] data) { return Get("Orders_ReferenceId", data); }
        public static HtmlBuilder Displays_Orders_ReferenceType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_ReferenceType", data)); }
        public static string Orders_ReferenceType(params string[] data) { return Get("Orders_ReferenceType", data); }
        public static HtmlBuilder Displays_Orders_OwnerId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_OwnerId", data)); }
        public static string Orders_OwnerId(params string[] data) { return Get("Orders_OwnerId", data); }
        public static HtmlBuilder Displays_Orders_Data(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_Data", data)); }
        public static string Orders_Data(params string[] data) { return Get("Orders_Data", data); }
        public static HtmlBuilder Displays_ExportSettings_ReferenceType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_ReferenceType", data)); }
        public static string ExportSettings_ReferenceType(params string[] data) { return Get("ExportSettings_ReferenceType", data); }
        public static HtmlBuilder Displays_ExportSettings_ReferenceId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_ReferenceId", data)); }
        public static string ExportSettings_ReferenceId(params string[] data) { return Get("ExportSettings_ReferenceId", data); }
        public static HtmlBuilder Displays_ExportSettings_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_Title", data)); }
        public static string ExportSettings_Title(params string[] data) { return Get("ExportSettings_Title", data); }
        public static HtmlBuilder Displays_ExportSettings_ExportSettingId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_ExportSettingId", data)); }
        public static string ExportSettings_ExportSettingId(params string[] data) { return Get("ExportSettings_ExportSettingId", data); }
        public static HtmlBuilder Displays_ExportSettings_AddHeader(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_AddHeader", data)); }
        public static string ExportSettings_AddHeader(params string[] data) { return Get("ExportSettings_AddHeader", data); }
        public static HtmlBuilder Displays_ExportSettings_ExportColumns(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_ExportColumns", data)); }
        public static string ExportSettings_ExportColumns(params string[] data) { return Get("ExportSettings_ExportColumns", data); }
        public static HtmlBuilder Displays_Links_DestinationId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_DestinationId", data)); }
        public static string Links_DestinationId(params string[] data) { return Get("Links_DestinationId", data); }
        public static HtmlBuilder Displays_Links_SourceId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_SourceId", data)); }
        public static string Links_SourceId(params string[] data) { return Get("Links_SourceId", data); }
        public static HtmlBuilder Displays_Links_ReferenceType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_ReferenceType", data)); }
        public static string Links_ReferenceType(params string[] data) { return Get("Links_ReferenceType", data); }
        public static HtmlBuilder Displays_Links_SiteId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_SiteId", data)); }
        public static string Links_SiteId(params string[] data) { return Get("Links_SiteId", data); }
        public static HtmlBuilder Displays_Links_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_Title", data)); }
        public static string Links_Title(params string[] data) { return Get("Links_Title", data); }
        public static HtmlBuilder Displays_Links_Subset(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_Subset", data)); }
        public static string Links_Subset(params string[] data) { return Get("Links_Subset", data); }
        public static HtmlBuilder Displays_Links_SiteTitle(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_SiteTitle", data)); }
        public static string Links_SiteTitle(params string[] data) { return Get("Links_SiteTitle", data); }
        public static HtmlBuilder Displays_Binaries_ReferenceId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_ReferenceId", data)); }
        public static string Binaries_ReferenceId(params string[] data) { return Get("Binaries_ReferenceId", data); }
        public static HtmlBuilder Displays_Binaries_BinaryId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_BinaryId", data)); }
        public static string Binaries_BinaryId(params string[] data) { return Get("Binaries_BinaryId", data); }
        public static HtmlBuilder Displays_Binaries_BinaryType(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_BinaryType", data)); }
        public static string Binaries_BinaryType(params string[] data) { return Get("Binaries_BinaryType", data); }
        public static HtmlBuilder Displays_Binaries_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Title", data)); }
        public static string Binaries_Title(params string[] data) { return Get("Binaries_Title", data); }
        public static HtmlBuilder Displays_Binaries_Body(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Body", data)); }
        public static string Binaries_Body(params string[] data) { return Get("Binaries_Body", data); }
        public static HtmlBuilder Displays_Binaries_Bin(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Bin", data)); }
        public static string Binaries_Bin(params string[] data) { return Get("Binaries_Bin", data); }
        public static HtmlBuilder Displays_Binaries_Thumbnail(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Thumbnail", data)); }
        public static string Binaries_Thumbnail(params string[] data) { return Get("Binaries_Thumbnail", data); }
        public static HtmlBuilder Displays_Binaries_Icon(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Icon", data)); }
        public static string Binaries_Icon(params string[] data) { return Get("Binaries_Icon", data); }
        public static HtmlBuilder Displays_Binaries_FileName(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_FileName", data)); }
        public static string Binaries_FileName(params string[] data) { return Get("Binaries_FileName", data); }
        public static HtmlBuilder Displays_Binaries_Extension(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Extension", data)); }
        public static string Binaries_Extension(params string[] data) { return Get("Binaries_Extension", data); }
        public static HtmlBuilder Displays_Binaries_Size(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Size", data)); }
        public static string Binaries_Size(params string[] data) { return Get("Binaries_Size", data); }
        public static HtmlBuilder Displays_Binaries_BinarySettings(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_BinarySettings", data)); }
        public static string Binaries_BinarySettings(params string[] data) { return Get("Binaries_BinarySettings", data); }
        public static HtmlBuilder Displays_Issues_IssueId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_IssueId", data)); }
        public static string Issues_IssueId(params string[] data) { return Get("Issues_IssueId", data); }
        public static HtmlBuilder Displays_Issues_StartTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_StartTime", data)); }
        public static string Issues_StartTime(params string[] data) { return Get("Issues_StartTime", data); }
        public static HtmlBuilder Displays_Issues_CompletionTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_CompletionTime", data)); }
        public static string Issues_CompletionTime(params string[] data) { return Get("Issues_CompletionTime", data); }
        public static HtmlBuilder Displays_Issues_WorkValue(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_WorkValue", data)); }
        public static string Issues_WorkValue(params string[] data) { return Get("Issues_WorkValue", data); }
        public static HtmlBuilder Displays_Issues_ProgressRate(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ProgressRate", data)); }
        public static string Issues_ProgressRate(params string[] data) { return Get("Issues_ProgressRate", data); }
        public static HtmlBuilder Displays_Issues_RemainingWorkValue(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_RemainingWorkValue", data)); }
        public static string Issues_RemainingWorkValue(params string[] data) { return Get("Issues_RemainingWorkValue", data); }
        public static HtmlBuilder Displays_Issues_Status(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Status", data)); }
        public static string Issues_Status(params string[] data) { return Get("Issues_Status", data); }
        public static HtmlBuilder Displays_Issues_Manager(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Manager", data)); }
        public static string Issues_Manager(params string[] data) { return Get("Issues_Manager", data); }
        public static HtmlBuilder Displays_Issues_Owner(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Owner", data)); }
        public static string Issues_Owner(params string[] data) { return Get("Issues_Owner", data); }
        public static HtmlBuilder Displays_Issues_ClassA(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassA", data)); }
        public static string Issues_ClassA(params string[] data) { return Get("Issues_ClassA", data); }
        public static HtmlBuilder Displays_Issues_ClassB(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassB", data)); }
        public static string Issues_ClassB(params string[] data) { return Get("Issues_ClassB", data); }
        public static HtmlBuilder Displays_Issues_ClassC(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassC", data)); }
        public static string Issues_ClassC(params string[] data) { return Get("Issues_ClassC", data); }
        public static HtmlBuilder Displays_Issues_ClassD(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassD", data)); }
        public static string Issues_ClassD(params string[] data) { return Get("Issues_ClassD", data); }
        public static HtmlBuilder Displays_Issues_ClassE(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassE", data)); }
        public static string Issues_ClassE(params string[] data) { return Get("Issues_ClassE", data); }
        public static HtmlBuilder Displays_Issues_ClassF(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassF", data)); }
        public static string Issues_ClassF(params string[] data) { return Get("Issues_ClassF", data); }
        public static HtmlBuilder Displays_Issues_ClassG(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassG", data)); }
        public static string Issues_ClassG(params string[] data) { return Get("Issues_ClassG", data); }
        public static HtmlBuilder Displays_Issues_ClassH(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassH", data)); }
        public static string Issues_ClassH(params string[] data) { return Get("Issues_ClassH", data); }
        public static HtmlBuilder Displays_Issues_ClassI(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassI", data)); }
        public static string Issues_ClassI(params string[] data) { return Get("Issues_ClassI", data); }
        public static HtmlBuilder Displays_Issues_ClassJ(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassJ", data)); }
        public static string Issues_ClassJ(params string[] data) { return Get("Issues_ClassJ", data); }
        public static HtmlBuilder Displays_Issues_ClassK(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassK", data)); }
        public static string Issues_ClassK(params string[] data) { return Get("Issues_ClassK", data); }
        public static HtmlBuilder Displays_Issues_ClassL(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassL", data)); }
        public static string Issues_ClassL(params string[] data) { return Get("Issues_ClassL", data); }
        public static HtmlBuilder Displays_Issues_ClassM(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassM", data)); }
        public static string Issues_ClassM(params string[] data) { return Get("Issues_ClassM", data); }
        public static HtmlBuilder Displays_Issues_ClassN(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassN", data)); }
        public static string Issues_ClassN(params string[] data) { return Get("Issues_ClassN", data); }
        public static HtmlBuilder Displays_Issues_ClassO(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassO", data)); }
        public static string Issues_ClassO(params string[] data) { return Get("Issues_ClassO", data); }
        public static HtmlBuilder Displays_Issues_ClassP(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_ClassP", data)); }
        public static string Issues_ClassP(params string[] data) { return Get("Issues_ClassP", data); }
        public static HtmlBuilder Displays_Issues_NumA(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_NumA", data)); }
        public static string Issues_NumA(params string[] data) { return Get("Issues_NumA", data); }
        public static HtmlBuilder Displays_Issues_NumB(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_NumB", data)); }
        public static string Issues_NumB(params string[] data) { return Get("Issues_NumB", data); }
        public static HtmlBuilder Displays_Issues_NumC(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_NumC", data)); }
        public static string Issues_NumC(params string[] data) { return Get("Issues_NumC", data); }
        public static HtmlBuilder Displays_Issues_NumD(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_NumD", data)); }
        public static string Issues_NumD(params string[] data) { return Get("Issues_NumD", data); }
        public static HtmlBuilder Displays_Issues_NumE(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_NumE", data)); }
        public static string Issues_NumE(params string[] data) { return Get("Issues_NumE", data); }
        public static HtmlBuilder Displays_Issues_NumF(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_NumF", data)); }
        public static string Issues_NumF(params string[] data) { return Get("Issues_NumF", data); }
        public static HtmlBuilder Displays_Issues_NumG(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_NumG", data)); }
        public static string Issues_NumG(params string[] data) { return Get("Issues_NumG", data); }
        public static HtmlBuilder Displays_Issues_NumH(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_NumH", data)); }
        public static string Issues_NumH(params string[] data) { return Get("Issues_NumH", data); }
        public static HtmlBuilder Displays_Issues_DateA(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_DateA", data)); }
        public static string Issues_DateA(params string[] data) { return Get("Issues_DateA", data); }
        public static HtmlBuilder Displays_Issues_DateB(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_DateB", data)); }
        public static string Issues_DateB(params string[] data) { return Get("Issues_DateB", data); }
        public static HtmlBuilder Displays_Issues_DateC(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_DateC", data)); }
        public static string Issues_DateC(params string[] data) { return Get("Issues_DateC", data); }
        public static HtmlBuilder Displays_Issues_DateD(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_DateD", data)); }
        public static string Issues_DateD(params string[] data) { return Get("Issues_DateD", data); }
        public static HtmlBuilder Displays_Issues_DateE(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_DateE", data)); }
        public static string Issues_DateE(params string[] data) { return Get("Issues_DateE", data); }
        public static HtmlBuilder Displays_Issues_DateF(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_DateF", data)); }
        public static string Issues_DateF(params string[] data) { return Get("Issues_DateF", data); }
        public static HtmlBuilder Displays_Issues_DateG(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_DateG", data)); }
        public static string Issues_DateG(params string[] data) { return Get("Issues_DateG", data); }
        public static HtmlBuilder Displays_Issues_DateH(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_DateH", data)); }
        public static string Issues_DateH(params string[] data) { return Get("Issues_DateH", data); }
        public static HtmlBuilder Displays_Results_ResultId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ResultId", data)); }
        public static string Results_ResultId(params string[] data) { return Get("Results_ResultId", data); }
        public static HtmlBuilder Displays_Results_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Title", data)); }
        public static string Results_Title(params string[] data) { return Get("Results_Title", data); }
        public static HtmlBuilder Displays_Results_Manager(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Manager", data)); }
        public static string Results_Manager(params string[] data) { return Get("Results_Manager", data); }
        public static HtmlBuilder Displays_Results_Owner(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Owner", data)); }
        public static string Results_Owner(params string[] data) { return Get("Results_Owner", data); }
        public static HtmlBuilder Displays_Results_ClassA(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ClassA", data)); }
        public static string Results_ClassA(params string[] data) { return Get("Results_ClassA", data); }
        public static HtmlBuilder Displays_Results_ClassB(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ClassB", data)); }
        public static string Results_ClassB(params string[] data) { return Get("Results_ClassB", data); }
        public static HtmlBuilder Displays_Results_ClassC(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ClassC", data)); }
        public static string Results_ClassC(params string[] data) { return Get("Results_ClassC", data); }
        public static HtmlBuilder Displays_Results_ClassD(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ClassD", data)); }
        public static string Results_ClassD(params string[] data) { return Get("Results_ClassD", data); }
        public static HtmlBuilder Displays_Results_ClassE(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ClassE", data)); }
        public static string Results_ClassE(params string[] data) { return Get("Results_ClassE", data); }
        public static HtmlBuilder Displays_Results_ClassF(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ClassF", data)); }
        public static string Results_ClassF(params string[] data) { return Get("Results_ClassF", data); }
        public static HtmlBuilder Displays_Results_ClassG(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ClassG", data)); }
        public static string Results_ClassG(params string[] data) { return Get("Results_ClassG", data); }
        public static HtmlBuilder Displays_Results_ClassH(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_ClassH", data)); }
        public static string Results_ClassH(params string[] data) { return Get("Results_ClassH", data); }
        public static HtmlBuilder Displays_Results_NumA(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_NumA", data)); }
        public static string Results_NumA(params string[] data) { return Get("Results_NumA", data); }
        public static HtmlBuilder Displays_Results_NumB(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_NumB", data)); }
        public static string Results_NumB(params string[] data) { return Get("Results_NumB", data); }
        public static HtmlBuilder Displays_Results_NumC(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_NumC", data)); }
        public static string Results_NumC(params string[] data) { return Get("Results_NumC", data); }
        public static HtmlBuilder Displays_Results_NumD(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_NumD", data)); }
        public static string Results_NumD(params string[] data) { return Get("Results_NumD", data); }
        public static HtmlBuilder Displays_Results_NumE(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_NumE", data)); }
        public static string Results_NumE(params string[] data) { return Get("Results_NumE", data); }
        public static HtmlBuilder Displays_Results_NumF(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_NumF", data)); }
        public static string Results_NumF(params string[] data) { return Get("Results_NumF", data); }
        public static HtmlBuilder Displays_Results_NumG(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_NumG", data)); }
        public static string Results_NumG(params string[] data) { return Get("Results_NumG", data); }
        public static HtmlBuilder Displays_Results_NumH(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_NumH", data)); }
        public static string Results_NumH(params string[] data) { return Get("Results_NumH", data); }
        public static HtmlBuilder Displays_Results_DateA(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_DateA", data)); }
        public static string Results_DateA(params string[] data) { return Get("Results_DateA", data); }
        public static HtmlBuilder Displays_Results_DateB(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_DateB", data)); }
        public static string Results_DateB(params string[] data) { return Get("Results_DateB", data); }
        public static HtmlBuilder Displays_Results_DateC(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_DateC", data)); }
        public static string Results_DateC(params string[] data) { return Get("Results_DateC", data); }
        public static HtmlBuilder Displays_Results_DateD(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_DateD", data)); }
        public static string Results_DateD(params string[] data) { return Get("Results_DateD", data); }
        public static HtmlBuilder Displays_Results_DateE(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_DateE", data)); }
        public static string Results_DateE(params string[] data) { return Get("Results_DateE", data); }
        public static HtmlBuilder Displays_Results_DateF(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_DateF", data)); }
        public static string Results_DateF(params string[] data) { return Get("Results_DateF", data); }
        public static HtmlBuilder Displays_Results_DateG(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_DateG", data)); }
        public static string Results_DateG(params string[] data) { return Get("Results_DateG", data); }
        public static HtmlBuilder Displays_Results_DateH(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_DateH", data)); }
        public static string Results_DateH(params string[] data) { return Get("Results_DateH", data); }
        public static HtmlBuilder Displays_Wikis_WikiId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_WikiId", data)); }
        public static string Wikis_WikiId(params string[] data) { return Get("Wikis_WikiId", data); }
        public static HtmlBuilder Displays_Tenants_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_Ver", data)); }
        public static string Tenants_Ver(params string[] data) { return Get("Tenants_Ver", data); }
        public static HtmlBuilder Displays_Tenants_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_Comments", data)); }
        public static string Tenants_Comments(params string[] data) { return Get("Tenants_Comments", data); }
        public static HtmlBuilder Displays_Tenants_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_Creator", data)); }
        public static string Tenants_Creator(params string[] data) { return Get("Tenants_Creator", data); }
        public static HtmlBuilder Displays_Tenants_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_Updator", data)); }
        public static string Tenants_Updator(params string[] data) { return Get("Tenants_Updator", data); }
        public static HtmlBuilder Displays_Tenants_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_CreatedTime", data)); }
        public static string Tenants_CreatedTime(params string[] data) { return Get("Tenants_CreatedTime", data); }
        public static HtmlBuilder Displays_Tenants_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_UpdatedTime", data)); }
        public static string Tenants_UpdatedTime(params string[] data) { return Get("Tenants_UpdatedTime", data); }
        public static HtmlBuilder Displays_Tenants_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_VerUp", data)); }
        public static string Tenants_VerUp(params string[] data) { return Get("Tenants_VerUp", data); }
        public static HtmlBuilder Displays_Tenants_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants_Timestamp", data)); }
        public static string Tenants_Timestamp(params string[] data) { return Get("Tenants_Timestamp", data); }
        public static HtmlBuilder Displays_SysLogs_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Ver", data)); }
        public static string SysLogs_Ver(params string[] data) { return Get("SysLogs_Ver", data); }
        public static HtmlBuilder Displays_SysLogs_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Comments", data)); }
        public static string SysLogs_Comments(params string[] data) { return Get("SysLogs_Comments", data); }
        public static HtmlBuilder Displays_SysLogs_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Creator", data)); }
        public static string SysLogs_Creator(params string[] data) { return Get("SysLogs_Creator", data); }
        public static HtmlBuilder Displays_SysLogs_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Updator", data)); }
        public static string SysLogs_Updator(params string[] data) { return Get("SysLogs_Updator", data); }
        public static HtmlBuilder Displays_SysLogs_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_UpdatedTime", data)); }
        public static string SysLogs_UpdatedTime(params string[] data) { return Get("SysLogs_UpdatedTime", data); }
        public static HtmlBuilder Displays_SysLogs_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_VerUp", data)); }
        public static string SysLogs_VerUp(params string[] data) { return Get("SysLogs_VerUp", data); }
        public static HtmlBuilder Displays_SysLogs_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs_Timestamp", data)); }
        public static string SysLogs_Timestamp(params string[] data) { return Get("SysLogs_Timestamp", data); }
        public static HtmlBuilder Displays_Depts_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_Ver", data)); }
        public static string Depts_Ver(params string[] data) { return Get("Depts_Ver", data); }
        public static HtmlBuilder Displays_Depts_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_Comments", data)); }
        public static string Depts_Comments(params string[] data) { return Get("Depts_Comments", data); }
        public static HtmlBuilder Displays_Depts_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_Creator", data)); }
        public static string Depts_Creator(params string[] data) { return Get("Depts_Creator", data); }
        public static HtmlBuilder Displays_Depts_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_Updator", data)); }
        public static string Depts_Updator(params string[] data) { return Get("Depts_Updator", data); }
        public static HtmlBuilder Displays_Depts_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_CreatedTime", data)); }
        public static string Depts_CreatedTime(params string[] data) { return Get("Depts_CreatedTime", data); }
        public static HtmlBuilder Displays_Depts_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_UpdatedTime", data)); }
        public static string Depts_UpdatedTime(params string[] data) { return Get("Depts_UpdatedTime", data); }
        public static HtmlBuilder Displays_Depts_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_VerUp", data)); }
        public static string Depts_VerUp(params string[] data) { return Get("Depts_VerUp", data); }
        public static HtmlBuilder Displays_Depts_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts_Timestamp", data)); }
        public static string Depts_Timestamp(params string[] data) { return Get("Depts_Timestamp", data); }
        public static HtmlBuilder Displays_Users_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Ver", data)); }
        public static string Users_Ver(params string[] data) { return Get("Users_Ver", data); }
        public static HtmlBuilder Displays_Users_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Comments", data)); }
        public static string Users_Comments(params string[] data) { return Get("Users_Comments", data); }
        public static HtmlBuilder Displays_Users_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Creator", data)); }
        public static string Users_Creator(params string[] data) { return Get("Users_Creator", data); }
        public static HtmlBuilder Displays_Users_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Updator", data)); }
        public static string Users_Updator(params string[] data) { return Get("Users_Updator", data); }
        public static HtmlBuilder Displays_Users_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_CreatedTime", data)); }
        public static string Users_CreatedTime(params string[] data) { return Get("Users_CreatedTime", data); }
        public static HtmlBuilder Displays_Users_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_UpdatedTime", data)); }
        public static string Users_UpdatedTime(params string[] data) { return Get("Users_UpdatedTime", data); }
        public static HtmlBuilder Displays_Users_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_VerUp", data)); }
        public static string Users_VerUp(params string[] data) { return Get("Users_VerUp", data); }
        public static HtmlBuilder Displays_Users_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users_Timestamp", data)); }
        public static string Users_Timestamp(params string[] data) { return Get("Users_Timestamp", data); }
        public static HtmlBuilder Displays_MailAddresses_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_Ver", data)); }
        public static string MailAddresses_Ver(params string[] data) { return Get("MailAddresses_Ver", data); }
        public static HtmlBuilder Displays_MailAddresses_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_Comments", data)); }
        public static string MailAddresses_Comments(params string[] data) { return Get("MailAddresses_Comments", data); }
        public static HtmlBuilder Displays_MailAddresses_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_Creator", data)); }
        public static string MailAddresses_Creator(params string[] data) { return Get("MailAddresses_Creator", data); }
        public static HtmlBuilder Displays_MailAddresses_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_Updator", data)); }
        public static string MailAddresses_Updator(params string[] data) { return Get("MailAddresses_Updator", data); }
        public static HtmlBuilder Displays_MailAddresses_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_CreatedTime", data)); }
        public static string MailAddresses_CreatedTime(params string[] data) { return Get("MailAddresses_CreatedTime", data); }
        public static HtmlBuilder Displays_MailAddresses_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_UpdatedTime", data)); }
        public static string MailAddresses_UpdatedTime(params string[] data) { return Get("MailAddresses_UpdatedTime", data); }
        public static HtmlBuilder Displays_MailAddresses_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_VerUp", data)); }
        public static string MailAddresses_VerUp(params string[] data) { return Get("MailAddresses_VerUp", data); }
        public static HtmlBuilder Displays_MailAddresses_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses_Timestamp", data)); }
        public static string MailAddresses_Timestamp(params string[] data) { return Get("MailAddresses_Timestamp", data); }
        public static HtmlBuilder Displays_Permissions_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_Ver", data)); }
        public static string Permissions_Ver(params string[] data) { return Get("Permissions_Ver", data); }
        public static HtmlBuilder Displays_Permissions_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_Comments", data)); }
        public static string Permissions_Comments(params string[] data) { return Get("Permissions_Comments", data); }
        public static HtmlBuilder Displays_Permissions_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_Creator", data)); }
        public static string Permissions_Creator(params string[] data) { return Get("Permissions_Creator", data); }
        public static HtmlBuilder Displays_Permissions_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_Updator", data)); }
        public static string Permissions_Updator(params string[] data) { return Get("Permissions_Updator", data); }
        public static HtmlBuilder Displays_Permissions_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_CreatedTime", data)); }
        public static string Permissions_CreatedTime(params string[] data) { return Get("Permissions_CreatedTime", data); }
        public static HtmlBuilder Displays_Permissions_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_UpdatedTime", data)); }
        public static string Permissions_UpdatedTime(params string[] data) { return Get("Permissions_UpdatedTime", data); }
        public static HtmlBuilder Displays_Permissions_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_VerUp", data)); }
        public static string Permissions_VerUp(params string[] data) { return Get("Permissions_VerUp", data); }
        public static HtmlBuilder Displays_Permissions_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions_Timestamp", data)); }
        public static string Permissions_Timestamp(params string[] data) { return Get("Permissions_Timestamp", data); }
        public static HtmlBuilder Displays_OutgoingMails_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Ver", data)); }
        public static string OutgoingMails_Ver(params string[] data) { return Get("OutgoingMails_Ver", data); }
        public static HtmlBuilder Displays_OutgoingMails_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Comments", data)); }
        public static string OutgoingMails_Comments(params string[] data) { return Get("OutgoingMails_Comments", data); }
        public static HtmlBuilder Displays_OutgoingMails_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Creator", data)); }
        public static string OutgoingMails_Creator(params string[] data) { return Get("OutgoingMails_Creator", data); }
        public static HtmlBuilder Displays_OutgoingMails_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Updator", data)); }
        public static string OutgoingMails_Updator(params string[] data) { return Get("OutgoingMails_Updator", data); }
        public static HtmlBuilder Displays_OutgoingMails_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_CreatedTime", data)); }
        public static string OutgoingMails_CreatedTime(params string[] data) { return Get("OutgoingMails_CreatedTime", data); }
        public static HtmlBuilder Displays_OutgoingMails_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_UpdatedTime", data)); }
        public static string OutgoingMails_UpdatedTime(params string[] data) { return Get("OutgoingMails_UpdatedTime", data); }
        public static HtmlBuilder Displays_OutgoingMails_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_VerUp", data)); }
        public static string OutgoingMails_VerUp(params string[] data) { return Get("OutgoingMails_VerUp", data); }
        public static HtmlBuilder Displays_OutgoingMails_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails_Timestamp", data)); }
        public static string OutgoingMails_Timestamp(params string[] data) { return Get("OutgoingMails_Timestamp", data); }
        public static HtmlBuilder Displays_SearchIndexes_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Ver", data)); }
        public static string SearchIndexes_Ver(params string[] data) { return Get("SearchIndexes_Ver", data); }
        public static HtmlBuilder Displays_SearchIndexes_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Comments", data)); }
        public static string SearchIndexes_Comments(params string[] data) { return Get("SearchIndexes_Comments", data); }
        public static HtmlBuilder Displays_SearchIndexes_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Creator", data)); }
        public static string SearchIndexes_Creator(params string[] data) { return Get("SearchIndexes_Creator", data); }
        public static HtmlBuilder Displays_SearchIndexes_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Updator", data)); }
        public static string SearchIndexes_Updator(params string[] data) { return Get("SearchIndexes_Updator", data); }
        public static HtmlBuilder Displays_SearchIndexes_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_CreatedTime", data)); }
        public static string SearchIndexes_CreatedTime(params string[] data) { return Get("SearchIndexes_CreatedTime", data); }
        public static HtmlBuilder Displays_SearchIndexes_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_UpdatedTime", data)); }
        public static string SearchIndexes_UpdatedTime(params string[] data) { return Get("SearchIndexes_UpdatedTime", data); }
        public static HtmlBuilder Displays_SearchIndexes_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_VerUp", data)); }
        public static string SearchIndexes_VerUp(params string[] data) { return Get("SearchIndexes_VerUp", data); }
        public static HtmlBuilder Displays_SearchIndexes_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes_Timestamp", data)); }
        public static string SearchIndexes_Timestamp(params string[] data) { return Get("SearchIndexes_Timestamp", data); }
        public static HtmlBuilder Displays_Items_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_Ver", data)); }
        public static string Items_Ver(params string[] data) { return Get("Items_Ver", data); }
        public static HtmlBuilder Displays_Items_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_Comments", data)); }
        public static string Items_Comments(params string[] data) { return Get("Items_Comments", data); }
        public static HtmlBuilder Displays_Items_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_Creator", data)); }
        public static string Items_Creator(params string[] data) { return Get("Items_Creator", data); }
        public static HtmlBuilder Displays_Items_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_Updator", data)); }
        public static string Items_Updator(params string[] data) { return Get("Items_Updator", data); }
        public static HtmlBuilder Displays_Items_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_CreatedTime", data)); }
        public static string Items_CreatedTime(params string[] data) { return Get("Items_CreatedTime", data); }
        public static HtmlBuilder Displays_Items_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_UpdatedTime", data)); }
        public static string Items_UpdatedTime(params string[] data) { return Get("Items_UpdatedTime", data); }
        public static HtmlBuilder Displays_Items_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_VerUp", data)); }
        public static string Items_VerUp(params string[] data) { return Get("Items_VerUp", data); }
        public static HtmlBuilder Displays_Items_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items_Timestamp", data)); }
        public static string Items_Timestamp(params string[] data) { return Get("Items_Timestamp", data); }
        public static HtmlBuilder Displays_Sites_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_UpdatedTime", data)); }
        public static string Sites_UpdatedTime(params string[] data) { return Get("Sites_UpdatedTime", data); }
        public static HtmlBuilder Displays_Sites_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_Title", data)); }
        public static string Sites_Title(params string[] data) { return Get("Sites_Title", data); }
        public static HtmlBuilder Displays_Sites_Body(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_Body", data)); }
        public static string Sites_Body(params string[] data) { return Get("Sites_Body", data); }
        public static HtmlBuilder Displays_Sites_TitleBody(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_TitleBody", data)); }
        public static string Sites_TitleBody(params string[] data) { return Get("Sites_TitleBody", data); }
        public static HtmlBuilder Displays_Sites_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_Ver", data)); }
        public static string Sites_Ver(params string[] data) { return Get("Sites_Ver", data); }
        public static HtmlBuilder Displays_Sites_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_Comments", data)); }
        public static string Sites_Comments(params string[] data) { return Get("Sites_Comments", data); }
        public static HtmlBuilder Displays_Sites_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_Creator", data)); }
        public static string Sites_Creator(params string[] data) { return Get("Sites_Creator", data); }
        public static HtmlBuilder Displays_Sites_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_Updator", data)); }
        public static string Sites_Updator(params string[] data) { return Get("Sites_Updator", data); }
        public static HtmlBuilder Displays_Sites_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_CreatedTime", data)); }
        public static string Sites_CreatedTime(params string[] data) { return Get("Sites_CreatedTime", data); }
        public static HtmlBuilder Displays_Sites_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_VerUp", data)); }
        public static string Sites_VerUp(params string[] data) { return Get("Sites_VerUp", data); }
        public static HtmlBuilder Displays_Sites_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites_Timestamp", data)); }
        public static string Sites_Timestamp(params string[] data) { return Get("Sites_Timestamp", data); }
        public static HtmlBuilder Displays_Orders_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_Ver", data)); }
        public static string Orders_Ver(params string[] data) { return Get("Orders_Ver", data); }
        public static HtmlBuilder Displays_Orders_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_Comments", data)); }
        public static string Orders_Comments(params string[] data) { return Get("Orders_Comments", data); }
        public static HtmlBuilder Displays_Orders_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_Creator", data)); }
        public static string Orders_Creator(params string[] data) { return Get("Orders_Creator", data); }
        public static HtmlBuilder Displays_Orders_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_Updator", data)); }
        public static string Orders_Updator(params string[] data) { return Get("Orders_Updator", data); }
        public static HtmlBuilder Displays_Orders_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_CreatedTime", data)); }
        public static string Orders_CreatedTime(params string[] data) { return Get("Orders_CreatedTime", data); }
        public static HtmlBuilder Displays_Orders_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_UpdatedTime", data)); }
        public static string Orders_UpdatedTime(params string[] data) { return Get("Orders_UpdatedTime", data); }
        public static HtmlBuilder Displays_Orders_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_VerUp", data)); }
        public static string Orders_VerUp(params string[] data) { return Get("Orders_VerUp", data); }
        public static HtmlBuilder Displays_Orders_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders_Timestamp", data)); }
        public static string Orders_Timestamp(params string[] data) { return Get("Orders_Timestamp", data); }
        public static HtmlBuilder Displays_ExportSettings_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_Ver", data)); }
        public static string ExportSettings_Ver(params string[] data) { return Get("ExportSettings_Ver", data); }
        public static HtmlBuilder Displays_ExportSettings_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_Comments", data)); }
        public static string ExportSettings_Comments(params string[] data) { return Get("ExportSettings_Comments", data); }
        public static HtmlBuilder Displays_ExportSettings_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_Creator", data)); }
        public static string ExportSettings_Creator(params string[] data) { return Get("ExportSettings_Creator", data); }
        public static HtmlBuilder Displays_ExportSettings_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_Updator", data)); }
        public static string ExportSettings_Updator(params string[] data) { return Get("ExportSettings_Updator", data); }
        public static HtmlBuilder Displays_ExportSettings_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_CreatedTime", data)); }
        public static string ExportSettings_CreatedTime(params string[] data) { return Get("ExportSettings_CreatedTime", data); }
        public static HtmlBuilder Displays_ExportSettings_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_UpdatedTime", data)); }
        public static string ExportSettings_UpdatedTime(params string[] data) { return Get("ExportSettings_UpdatedTime", data); }
        public static HtmlBuilder Displays_ExportSettings_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_VerUp", data)); }
        public static string ExportSettings_VerUp(params string[] data) { return Get("ExportSettings_VerUp", data); }
        public static HtmlBuilder Displays_ExportSettings_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings_Timestamp", data)); }
        public static string ExportSettings_Timestamp(params string[] data) { return Get("ExportSettings_Timestamp", data); }
        public static HtmlBuilder Displays_Links_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_Ver", data)); }
        public static string Links_Ver(params string[] data) { return Get("Links_Ver", data); }
        public static HtmlBuilder Displays_Links_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_Comments", data)); }
        public static string Links_Comments(params string[] data) { return Get("Links_Comments", data); }
        public static HtmlBuilder Displays_Links_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_Creator", data)); }
        public static string Links_Creator(params string[] data) { return Get("Links_Creator", data); }
        public static HtmlBuilder Displays_Links_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_Updator", data)); }
        public static string Links_Updator(params string[] data) { return Get("Links_Updator", data); }
        public static HtmlBuilder Displays_Links_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_CreatedTime", data)); }
        public static string Links_CreatedTime(params string[] data) { return Get("Links_CreatedTime", data); }
        public static HtmlBuilder Displays_Links_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_UpdatedTime", data)); }
        public static string Links_UpdatedTime(params string[] data) { return Get("Links_UpdatedTime", data); }
        public static HtmlBuilder Displays_Links_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_VerUp", data)); }
        public static string Links_VerUp(params string[] data) { return Get("Links_VerUp", data); }
        public static HtmlBuilder Displays_Links_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Links_Timestamp", data)); }
        public static string Links_Timestamp(params string[] data) { return Get("Links_Timestamp", data); }
        public static HtmlBuilder Displays_Binaries_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Ver", data)); }
        public static string Binaries_Ver(params string[] data) { return Get("Binaries_Ver", data); }
        public static HtmlBuilder Displays_Binaries_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Comments", data)); }
        public static string Binaries_Comments(params string[] data) { return Get("Binaries_Comments", data); }
        public static HtmlBuilder Displays_Binaries_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Creator", data)); }
        public static string Binaries_Creator(params string[] data) { return Get("Binaries_Creator", data); }
        public static HtmlBuilder Displays_Binaries_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Updator", data)); }
        public static string Binaries_Updator(params string[] data) { return Get("Binaries_Updator", data); }
        public static HtmlBuilder Displays_Binaries_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_CreatedTime", data)); }
        public static string Binaries_CreatedTime(params string[] data) { return Get("Binaries_CreatedTime", data); }
        public static HtmlBuilder Displays_Binaries_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_UpdatedTime", data)); }
        public static string Binaries_UpdatedTime(params string[] data) { return Get("Binaries_UpdatedTime", data); }
        public static HtmlBuilder Displays_Binaries_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_VerUp", data)); }
        public static string Binaries_VerUp(params string[] data) { return Get("Binaries_VerUp", data); }
        public static HtmlBuilder Displays_Binaries_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries_Timestamp", data)); }
        public static string Binaries_Timestamp(params string[] data) { return Get("Binaries_Timestamp", data); }
        public static HtmlBuilder Displays_Issues_SiteId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_SiteId", data)); }
        public static string Issues_SiteId(params string[] data) { return Get("Issues_SiteId", data); }
        public static HtmlBuilder Displays_Issues_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_UpdatedTime", data)); }
        public static string Issues_UpdatedTime(params string[] data) { return Get("Issues_UpdatedTime", data); }
        public static HtmlBuilder Displays_Issues_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Title", data)); }
        public static string Issues_Title(params string[] data) { return Get("Issues_Title", data); }
        public static HtmlBuilder Displays_Issues_Body(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Body", data)); }
        public static string Issues_Body(params string[] data) { return Get("Issues_Body", data); }
        public static HtmlBuilder Displays_Issues_TitleBody(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_TitleBody", data)); }
        public static string Issues_TitleBody(params string[] data) { return Get("Issues_TitleBody", data); }
        public static HtmlBuilder Displays_Issues_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Ver", data)); }
        public static string Issues_Ver(params string[] data) { return Get("Issues_Ver", data); }
        public static HtmlBuilder Displays_Issues_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Comments", data)); }
        public static string Issues_Comments(params string[] data) { return Get("Issues_Comments", data); }
        public static HtmlBuilder Displays_Issues_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Creator", data)); }
        public static string Issues_Creator(params string[] data) { return Get("Issues_Creator", data); }
        public static HtmlBuilder Displays_Issues_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Updator", data)); }
        public static string Issues_Updator(params string[] data) { return Get("Issues_Updator", data); }
        public static HtmlBuilder Displays_Issues_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_CreatedTime", data)); }
        public static string Issues_CreatedTime(params string[] data) { return Get("Issues_CreatedTime", data); }
        public static HtmlBuilder Displays_Issues_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_VerUp", data)); }
        public static string Issues_VerUp(params string[] data) { return Get("Issues_VerUp", data); }
        public static HtmlBuilder Displays_Issues_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues_Timestamp", data)); }
        public static string Issues_Timestamp(params string[] data) { return Get("Issues_Timestamp", data); }
        public static HtmlBuilder Displays_Results_SiteId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_SiteId", data)); }
        public static string Results_SiteId(params string[] data) { return Get("Results_SiteId", data); }
        public static HtmlBuilder Displays_Results_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_UpdatedTime", data)); }
        public static string Results_UpdatedTime(params string[] data) { return Get("Results_UpdatedTime", data); }
        public static HtmlBuilder Displays_Results_Body(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Body", data)); }
        public static string Results_Body(params string[] data) { return Get("Results_Body", data); }
        public static HtmlBuilder Displays_Results_TitleBody(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_TitleBody", data)); }
        public static string Results_TitleBody(params string[] data) { return Get("Results_TitleBody", data); }
        public static HtmlBuilder Displays_Results_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Ver", data)); }
        public static string Results_Ver(params string[] data) { return Get("Results_Ver", data); }
        public static HtmlBuilder Displays_Results_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Comments", data)); }
        public static string Results_Comments(params string[] data) { return Get("Results_Comments", data); }
        public static HtmlBuilder Displays_Results_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Creator", data)); }
        public static string Results_Creator(params string[] data) { return Get("Results_Creator", data); }
        public static HtmlBuilder Displays_Results_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Updator", data)); }
        public static string Results_Updator(params string[] data) { return Get("Results_Updator", data); }
        public static HtmlBuilder Displays_Results_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_CreatedTime", data)); }
        public static string Results_CreatedTime(params string[] data) { return Get("Results_CreatedTime", data); }
        public static HtmlBuilder Displays_Results_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_VerUp", data)); }
        public static string Results_VerUp(params string[] data) { return Get("Results_VerUp", data); }
        public static HtmlBuilder Displays_Results_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results_Timestamp", data)); }
        public static string Results_Timestamp(params string[] data) { return Get("Results_Timestamp", data); }
        public static HtmlBuilder Displays_Wikis_SiteId(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_SiteId", data)); }
        public static string Wikis_SiteId(params string[] data) { return Get("Wikis_SiteId", data); }
        public static HtmlBuilder Displays_Wikis_UpdatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_UpdatedTime", data)); }
        public static string Wikis_UpdatedTime(params string[] data) { return Get("Wikis_UpdatedTime", data); }
        public static HtmlBuilder Displays_Wikis_Title(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_Title", data)); }
        public static string Wikis_Title(params string[] data) { return Get("Wikis_Title", data); }
        public static HtmlBuilder Displays_Wikis_Body(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_Body", data)); }
        public static string Wikis_Body(params string[] data) { return Get("Wikis_Body", data); }
        public static HtmlBuilder Displays_Wikis_TitleBody(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_TitleBody", data)); }
        public static string Wikis_TitleBody(params string[] data) { return Get("Wikis_TitleBody", data); }
        public static HtmlBuilder Displays_Wikis_Ver(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_Ver", data)); }
        public static string Wikis_Ver(params string[] data) { return Get("Wikis_Ver", data); }
        public static HtmlBuilder Displays_Wikis_Comments(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_Comments", data)); }
        public static string Wikis_Comments(params string[] data) { return Get("Wikis_Comments", data); }
        public static HtmlBuilder Displays_Wikis_Creator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_Creator", data)); }
        public static string Wikis_Creator(params string[] data) { return Get("Wikis_Creator", data); }
        public static HtmlBuilder Displays_Wikis_Updator(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_Updator", data)); }
        public static string Wikis_Updator(params string[] data) { return Get("Wikis_Updator", data); }
        public static HtmlBuilder Displays_Wikis_CreatedTime(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_CreatedTime", data)); }
        public static string Wikis_CreatedTime(params string[] data) { return Get("Wikis_CreatedTime", data); }
        public static HtmlBuilder Displays_Wikis_VerUp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_VerUp", data)); }
        public static string Wikis_VerUp(params string[] data) { return Get("Wikis_VerUp", data); }
        public static HtmlBuilder Displays_Wikis_Timestamp(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis_Timestamp", data)); }
        public static string Wikis_Timestamp(params string[] data) { return Get("Wikis_Timestamp", data); }
        public static HtmlBuilder Displays_Tenants(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Tenants", data)); }
        public static string Tenants(params string[] data) { return Get("Tenants", data); }
        public static HtmlBuilder Displays_SysLogs(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SysLogs", data)); }
        public static string SysLogs(params string[] data) { return Get("SysLogs", data); }
        public static HtmlBuilder Displays_Depts(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Depts", data)); }
        public static string Depts(params string[] data) { return Get("Depts", data); }
        public static HtmlBuilder Displays_Users(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Users", data)); }
        public static string Users(params string[] data) { return Get("Users", data); }
        public static HtmlBuilder Displays_MailAddresses(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("MailAddresses", data)); }
        public static string MailAddresses(params string[] data) { return Get("MailAddresses", data); }
        public static HtmlBuilder Displays_Permissions(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Permissions", data)); }
        public static string Permissions(params string[] data) { return Get("Permissions", data); }
        public static HtmlBuilder Displays_OutgoingMails(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("OutgoingMails", data)); }
        public static string OutgoingMails(params string[] data) { return Get("OutgoingMails", data); }
        public static HtmlBuilder Displays_SearchIndexes(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("SearchIndexes", data)); }
        public static string SearchIndexes(params string[] data) { return Get("SearchIndexes", data); }
        public static HtmlBuilder Displays_Items(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Items", data)); }
        public static string Items(params string[] data) { return Get("Items", data); }
        public static HtmlBuilder Displays_Sites(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Sites", data)); }
        public static string Sites(params string[] data) { return Get("Sites", data); }
        public static HtmlBuilder Displays_Orders(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Orders", data)); }
        public static string Orders(params string[] data) { return Get("Orders", data); }
        public static HtmlBuilder Displays_ExportSettings(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("ExportSettings", data)); }
        public static string ExportSettings(params string[] data) { return Get("ExportSettings", data); }
        public static HtmlBuilder Displays_Binaries(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Binaries", data)); }
        public static string Binaries(params string[] data) { return Get("Binaries", data); }
        public static HtmlBuilder Displays_Issues(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Issues", data)); }
        public static string Issues(params string[] data) { return Get("Issues", data); }
        public static HtmlBuilder Displays_Results(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Results", data)); }
        public static string Results(params string[] data) { return Get("Results", data); }
        public static HtmlBuilder Displays_Wikis(this HtmlBuilder hb, params string[] data) { return hb.Text(Get("Wikis", data)); }
        public static string Wikis(params string[] data) { return Get("Wikis", data); }
    }
}
