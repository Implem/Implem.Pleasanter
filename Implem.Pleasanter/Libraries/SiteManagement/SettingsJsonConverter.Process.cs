using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.SitePackages;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.SiteManagement
{
    public partial class SettingsJsonConverter
    {
        private static ProcessData ConvertProcess(
            Context context,
            Param param)
        {
            var s = new ProcessData();
            foreach (var siteId in param.SelectedSites.Select(v=>v.SiteId).Distinct())
            {
                var r = ProcessData.ConvertProcess(context: context, param: param, siteId: siteId);
                if (r == null) continue;
                (s.Processes ??= new()).Add(r);
            }
            s.Schema = ProcessData.GetSchema(context: context);
            return s;
        }

        public class ProcessesInfoData
        {
            public List<Log> Logs = new();
        }

        public class ProcessData
        {

            public class DisplaySchema
            {
                public string GeneralTitle;
                public List<FieldDef> GeneralFields;
                public List<SectionDef> Sections;
                public List<PermissionGroupDef> PermissionGroups;
                public Dictionary<string, List<string>> ArrayCandidates;
                public Dictionary<string, Dictionary<string, string>> EnumMaps;
            }

            public class FieldDef
            {
                public string Key;
                public string Label;
                public string Format;
                public string EnumMap;
                public object DefaultVal;
            }

            public class SectionColumnDef
            {
                public string Key;
                public string Label;
                public string Type;
                public string ArrayKey;
                public string Format;
                public string EnumMap;
                public object DefaultVal;
            }

            public class SectionDef
            {
                public string Title;
                public List<SectionColumnDef> Columns;
            }

            public class PermissionGroupDef
            {
                public string Title;
                public string DataKey;
                public string IdLabel;
                public string NameLabel;
            }


            public class Process
            {
                public long SiteId;
                public string Title;
                public string ReferenceType;
                public SitePackage SitePackage;
            }

            public ProcessesInfoData Info = new();
            public List<Process> Processes;
            public DisplaySchema Schema;

            internal static Process ConvertProcess(Context context, Param param, long siteId)
            {
                var siteModel = new SiteModel()
                    .Get(
                        context: context,
                        where: DataSources.Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId(siteId));
                if (siteModel.SiteId == 0)
                {
                    param.Logs.Add(new Log(Log.LogLevel.Error, "ConvertProcess", $"SiteId {siteId} not found."));
                    return null;
                }
                var ss = siteModel.SiteSettings;
                if (context.CanManageSite(ss: ss, site: true) == false)
                {
                    param.Logs.Add(new Log(Log.LogLevel.Error, "ConvertProcess", $"SiteId {siteId} not found."));
                    return null;
                }
                var siteList = new List<SitePackages.SelectedSite>
                {
                    new SitePackages.SelectedSite
                    {
                        SiteId = siteId,
                        IncludeData = false
                    }
                };
                var sitePackage = new SitePackage(
                    context: context,
                    siteList: siteList,
                    includeSitePermission: true,
                    includeRecordPermission: false,
                    includeColumnPermission: false,
                    includeNotifications: true,
                    includeReminders: true);
                if (sitePackage.HeaderInfo.Convertors.Count == 0) return null;
                var p = new Process
                {
                    SiteId = siteId,
                    Title = sitePackage.HeaderInfo.Convertors[0].SiteTitle,
                    ReferenceType = sitePackage.HeaderInfo.Convertors[0].ReferenceType,
                    SitePackage = sitePackage
                };
                return p;
            }


            internal static DisplaySchema GetSchema(Context context)
            {
                return new DisplaySchema
                {
                    GeneralTitle = Displays.General(context: context),
                    GeneralFields = GetGeneralFields(context),
                    Sections = GetSections(context),
                    PermissionGroups = GetPermissionGroups(context),
                    ArrayCandidates = GetArrayCandidates(),
                    EnumMaps = GetEnumMaps(context),
                };
            }

            private static List<FieldDef> GetGeneralFields(Context context)
            {
                return new List<FieldDef>
                {
                    new FieldDef { Key = "Id", Label = Displays.Id(context: context) },
                    new FieldDef { Key = "Name", Label = Displays.Name(context: context) },
                    new FieldDef { Key = "DisplayName", Label = Displays.DisplayName(context: context) },
                    new FieldDef { Key = "ScreenType", Label = Displays.ScreenType(context: context),
                        Format = "enum", EnumMap = "screen_type", DefaultVal = 20 },
                    new FieldDef { Key = "CurrentStatus", Label = Displays.CurrentStatus(context: context),
                        Format = "status" },
                    new FieldDef { Key = "ChangedStatus", Label = Displays.ChangedStatus(context: context),
                        Format = "status" },
                    new FieldDef { Key = "Description", Label = Displays.Description(context: context) },
                    new FieldDef { Key = "Tooltip", Label = Displays.Tooltip(context: context) },
                    new FieldDef { Key = "Icon", Label = Displays.Icon(context: context) },
                    new FieldDef { Key = "ConfirmationMessage", Label = Displays.ConfirmationMessage(context: context) },
                    new FieldDef { Key = "SuccessMessage", Label = Displays.SuccessMessage(context: context) },
                    new FieldDef { Key = "OnClick", Label = Displays.OnClick(context: context) },
                    new FieldDef { Key = "ExecutionType", Label = Displays.ExecutionTypes(context: context),
                        Format = "enum", EnumMap = "execution_type", DefaultVal = 0 },
                    new FieldDef { Key = "ActionType", Label = Displays.ActionTypes(context: context),
                        Format = "enum", EnumMap = "action_type", DefaultVal = 0 },
                    new FieldDef { Key = "AfterProcessStatusChangeActionType",
                        Label = Displays.AfterProcessStatusChangeActionType(context: context),
                        Format = "enum", EnumMap = "after_action_type", DefaultVal = 0 },
                    new FieldDef { Key = "AllowBulkProcessing", Label = Displays.AllowBulkProcessing(context: context),
                        Format = "bool" },
                };
            }

            private static List<SectionDef> GetSections(Context context)
            {
                return new List<SectionDef>
                {
                    new SectionDef
                    {
                        Title = Displays.ValidateInput(context: context),
                        Columns = new List<SectionColumnDef>
                        {
                            new SectionColumnDef { Key = "Id",
                                Label = Displays.Id(context: context),
                                Type = "nest", ArrayKey = "ValidateInputs" },
                            new SectionColumnDef { Key = "ColumnName",
                                Label = Displays.ColumnName(context: context),
                                Type = "nest", ArrayKey = "ValidateInputs", Format = "column" },
                            new SectionColumnDef { Key = "Required",
                                Label = Displays.Required(context: context),
                                Type = "nest", ArrayKey = "ValidateInputs", Format = "bool" },
                            new SectionColumnDef { Key = "ClientRegexValidation",
                                Label = Displays.ClientRegexValidation(context: context),
                                Type = "nest", ArrayKey = "ValidateInputs" },
                            new SectionColumnDef { Key = "ServerRegexValidation",
                                Label = Displays.ServerRegexValidation(context: context),
                                Type = "nest", ArrayKey = "ValidateInputs" },
                            new SectionColumnDef { Key = "RegexValidationMessage",
                                Label = Displays.RegexValidationMessage(context: context),
                                Type = "nest", ArrayKey = "ValidateInputs" },
                            new SectionColumnDef { Key = "Min",
                                Label = Displays.Min(context: context),
                                Type = "nest", ArrayKey = "ValidateInputs" },
                            new SectionColumnDef { Key = "Max",
                                Label = Displays.Max(context: context),
                                Type = "nest", ArrayKey = "ValidateInputs" },
                        }
                    },
                    new SectionDef
                    {
                        Title = Displays.Condition(context: context),
                        Columns = new List<SectionColumnDef>
                        {
                            new SectionColumnDef { Key = "Incomplete",
                                Label = Displays.Incomplete(context: context),
                                Type = "nest", ArrayKey = "Conditions", Format = "bool", DefaultVal = false },
                            new SectionColumnDef { Key = "Own",
                                Label = Displays.Own(context: context),
                                Type = "nest", ArrayKey = "Conditions", Format = "bool", DefaultVal = false },
                            new SectionColumnDef { Key = "NearCompletionTime",
                                Label = Displays.NearCompletionTime(context: context),
                                Type = "nest", ArrayKey = "Conditions", Format = "bool", DefaultVal = false },
                            new SectionColumnDef { Key = "Delay",
                                Label = Displays.Delay(context: context),
                                Type = "nest", ArrayKey = "Conditions", Format = "bool", DefaultVal = false },
                            new SectionColumnDef { Key = "Overdue",
                                Label = Displays.Overdue(context: context),
                                Type = "nest", ArrayKey = "Conditions", Format = "bool", DefaultVal = false },
                            new SectionColumnDef { Key = "Search",
                                Label = Displays.Search(context: context),
                                Type = "nest", ArrayKey = "Conditions" },
                            new SectionColumnDef { Key = "ColumnFilterHash",
                                Label = Displays.ColumnList(context: context),
                                Type = "nest", ArrayKey = "Conditions", Format = "columnFilterHash" },
                            new SectionColumnDef { Key = "ErrorMessage",
                                Label = Displays.ErrorMessage(context: context),
                                Type = "top" },
                        }
                    },
                    new SectionDef
                    {
                        Title = Displays.DataChanges(context: context),
                        Columns = new List<SectionColumnDef>
                        {
                            new SectionColumnDef { Key = "Id",
                                Label = Displays.Id(context: context),
                                Type = "nest", ArrayKey = "DataChanges" },
                            new SectionColumnDef { Key = "_Type",
                                Label = Displays.ChangeTypes(context: context),
                                Type = "nest", ArrayKey = "DataChanges",
                                Format = "enum", EnumMap = "data_change_type" },
                            new SectionColumnDef { Key = "ColumnName",
                                Label = Displays.ColumnName(context: context),
                                Type = "nest", ArrayKey = "DataChanges", Format = "column" },
                            new SectionColumnDef { Key = "_CopyFrom",
                                Label = Displays.CopyFrom(context: context),
                                Type = "nest", ArrayKey = "DataChanges", Format = "column" },
                            new SectionColumnDef { Key = "_Value",
                                Label = Displays.Value(context: context),
                                Type = "nest", ArrayKey = "DataChanges" },
                            new SectionColumnDef { Key = "_Formula",
                                Label = Displays.Formulas(context: context),
                                Type = "nest", ArrayKey = "DataChanges" },
                            new SectionColumnDef { Key = "_NotUseDisplayName",
                                Label = Displays.NotUseDisplayName(context: context),
                                Type = "nest", ArrayKey = "DataChanges" },
                            new SectionColumnDef { Key = "_IsDisplayError",
                                Label = Displays.ShowErrorMessage(context: context),
                                Type = "nest", ArrayKey = "DataChanges" },
                            new SectionColumnDef { Key = "_BaseDateTime",
                                Label = Displays.BaseDateTime(context: context),
                                Type = "nest", ArrayKey = "DataChanges",
                                Format = "enum", EnumMap = "base_datetime" },
                            new SectionColumnDef { Key = "_DateTimeValue",
                                Label = Displays.Value(context: context),
                                Type = "nest", ArrayKey = "DataChanges" },
                            new SectionColumnDef { Key = "_Period",
                                Label = Displays.Period(context: context),
                                Type = "nest", ArrayKey = "DataChanges",
                                Format = "enum", EnumMap = "data_change_period" },
                        }
                    },
                    new SectionDef
                    {
                        Title = Displays.AutoNumbering(context: context),
                        Columns = new List<SectionColumnDef>
                        {
                            new SectionColumnDef { Key = "ColumnName",
                                Label = Displays.ColumnName(context: context),
                                Type = "nest", ArrayKey = "AutoNumberings", Format = "column" },
                            new SectionColumnDef { Key = "Format",
                                Label = Displays.Format(context: context),
                                Type = "nest", ArrayKey = "AutoNumberings" },
                            new SectionColumnDef { Key = "ResetType",
                                Label = Displays.ResetType(context: context),
                                Type = "nest", ArrayKey = "AutoNumberings",
                                Format = "enum", EnumMap = "reset_type", DefaultVal = 0 },
                            new SectionColumnDef { Key = "Default",
                                Label = Displays.DefaultInput(context: context),
                                Type = "nest", ArrayKey = "AutoNumberings", DefaultVal = 1 },
                            new SectionColumnDef { Key = "Step",
                                Label = Displays.Step(context: context),
                                Type = "nest", ArrayKey = "AutoNumberings", DefaultVal = 1 },
                        }
                    },
                    new SectionDef
                    {
                        Title = Displays.Notifications(context: context),
                        Columns = new List<SectionColumnDef>
                        {
                            new SectionColumnDef { Key = "Id",
                                Label = Displays.Id(context: context),
                                Type = "nest", ArrayKey = "Notifications" },
                            new SectionColumnDef { Key = "Type",
                                Label = Displays.NotificationType(context: context),
                                Type = "nest", ArrayKey = "Notifications",
                                Format = "enum", EnumMap = "notification_type" },
                            new SectionColumnDef { Key = "Subject",
                                Label = Displays.Subject(context: context),
                                Type = "nest", ArrayKey = "Notifications" },
                            new SectionColumnDef { Key = "Address",
                                Label = Displays.Address(context: context),
                                Type = "nest", ArrayKey = "Notifications" },
                            new SectionColumnDef { Key = "CcAddress",
                                Label = Displays.Cc(context: context),
                                Type = "nest", ArrayKey = "Notifications" },
                            new SectionColumnDef { Key = "BccAddress",
                                Label = Displays.Bcc(context: context),
                                Type = "nest", ArrayKey = "Notifications" },
                            new SectionColumnDef { Key = "Token",
                                Label = Displays.Token(context: context),
                                Type = "nest", ArrayKey = "Notifications" },
                            new SectionColumnDef { Key = "Body",
                                Label = Displays.Body(context: context),
                                Type = "nest", ArrayKey = "Notifications" },
                        }
                    },
                };
            }

            private static List<PermissionGroupDef> GetPermissionGroups(Context context)
            {
                return new List<PermissionGroupDef>
                {
                    new PermissionGroupDef
                    {
                        Title = Displays.AccessControls(context: context),
                        DataKey = "Groups",
                        IdLabel = Displays.Permissions_GroupId(context: context),
                        NameLabel = Displays.Permissions_GroupName(context: context),
                    },
                    new PermissionGroupDef
                    {
                        DataKey = "Depts",
                        IdLabel = Displays.Permissions_DeptId(context: context),
                        NameLabel = Displays.Permissions_DeptName(context: context),
                    },
                    new PermissionGroupDef
                    {
                        DataKey = "Users",
                        IdLabel = Displays.Permissions_UserId(context: context),
                        NameLabel = Displays.Permissions_Name(context: context),
                    },
                };
            }

            private static Dictionary<string, List<string>> GetArrayCandidates()
            {
                return new Dictionary<string, List<string>>
                {
                    ["ValidateInputs"] = new List<string>
                        { "ValidateInputs", "ValidateInputList" },
                    ["Conditions"] = new List<string>
                        { "View", "Conditions", "ConditionList", "ProcessConditions", "ConditionSettings" },
                    ["DataChanges"] = new List<string>
                        { "DataChanges", "DataChangeList", "Changes", "ChangeColumns" },
                    ["AutoNumberings"] = new List<string>
                        { "AutoNumbering", "AutoNumberings", "AutoNumbers", "AutoNumber", "Numberings" },
                    ["Notifications"] = new List<string>
                        { "Notifications", "NotificationList", "Nots" },
                };
            }

            private static Dictionary<string, Dictionary<string, string>> GetEnumMaps(Context context)
            {
                return new Dictionary<string, Dictionary<string, string>>
                {
                    ["screen_type"] = new Dictionary<string, string>
                    {
                        ["10"] = Displays.New(context: context),
                        ["20"] = Displays.Edit(context: context),
                    },
                    ["execution_type"] = new Dictionary<string, string>
                    {
                        ["0"] = Displays.AddedButton(context: context),
                        ["10"] = Displays.CreateOrUpdate(context: context),
                        ["20"] = Displays.AddedButtonOrCreateOrUpdate(context: context),
                    },
                    ["action_type"] = new Dictionary<string, string>
                    {
                        ["0"] = Displays.Save(context: context),
                        ["10"] = Displays.PostBack(context: context),
                        ["90"] = Displays.None(context: context),
                    },
                    ["after_action_type"] = new Dictionary<string, string>
                    {
                        ["0"] = Displays.Default(context: context),
                        ["10"] = Displays.ReturnToList(context: context),
                    },
                    ["validate_input_type"] = new Dictionary<string, string>
                    {
                        ["0"] = Displays.Merge(context: context),
                        ["10"] = Displays.Replacement(context: context),
                        ["90"] = Displays.None(context: context),
                    },
                    ["data_change_type"] = new Dictionary<string, string>
                    {
                        ["CopyValue"] = Displays.CopyValue(context: context),
                        ["CopyDisplayValue"] = Displays.CopyDisplayValue(context: context),
                        ["InputValue"] = Displays.InputValue(context: context),
                        ["InputValueFormula"] = Displays.InputValueFormula(context: context),
                        ["InputDate"] = Displays.InputDate(context: context),
                        ["InputDateTime"] = Displays.InputDateTime(context: context),
                        ["InputDept"] = Displays.InputDept(context: context),
                        ["InputUser"] = Displays.InputUser(context: context),
                    },
                    ["data_change_period"] = new Dictionary<string, string>
                    {
                        ["Days"] = Displays.Days(context: context),
                        ["Months"] = Displays.Months(context: context),
                        ["Years"] = Displays.Years(context: context),
                        ["Hours"] = Displays.Hours(context: context),
                        ["Minutes"] = Displays.Minutes(context: context),
                        ["Seconds"] = Displays.Seconds(context: context),
                    },
                    ["base_datetime"] = new Dictionary<string, string>
                    {
                        ["CurrentDate"] = Displays.CurrentDate(context: context),
                        ["CurrentTime"] = Displays.CurrentTime(context: context),
                        ["CurrentDateTime"] = Displays.Get(
                            context: context,
                            id: "CurrentDateTime"),
                    },
                    ["notification_type"] = new Dictionary<string, string>
                    {
                        ["1"] = Displays.Mail(context: context),
                        ["2"] = "Slack",
                        ["3"] = "ChatWork",
                        ["4"] = "LINE",
                        ["5"] = Displays.LineGroup(context: context),
                        ["6"] = "Teams",
                        ["7"] = "Rocket.Chat",
                        ["8"] = "InCircle",
                        ["9"] = Displays.HttpClient(context: context),
                        ["10"] = "LINE WORKS",
                    },
                    ["reset_type"] = new Dictionary<string, string>
                    {
                        ["0"] = Displays.None(context: context),
                        ["10"] = Displays.Year(context: context),
                        ["20"] = Displays.Month(context: context),
                        ["30"] = Displays.Day(context: context),
                        ["90"] = Displays.String(context: context),
                        ["None"] = Displays.None(context: context),
                        ["Year"] = Displays.Year(context: context),
                        ["Month"] = Displays.Month(context: context),
                        ["Day"] = Displays.Day(context: context),
                        ["String"] = Displays.String(context: context),
                    },
                };
            }
        }
    }
}
