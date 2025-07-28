using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Implem.Pleasanter.Libraries.SiteManagement
{
    public class SettingsJsonConverter
    {
        public class Log
        {
            public enum LogLevel
            {
                Fatal,
                Error,
                Warning,
                Info,
                Debug
            }
            [JsonConverter(typeof(StringEnumConverter))]
            public LogLevel Level;
            public string Section;
            public string Message;
            public long? SiteId;

            public Log() { }

            public Log(LogLevel level, string section, string message)
            {
                Level = level;
                Section = section;
                Message = message;
            }
        }

        public class Param
        {
            public List<SelectedSite> SelectedSites;
            public List<Log> Logs = new ();
        }

        public class SelectedSite : IEquatable<SelectedSite>
        {
            public long SiteId;
            public int Ver;

            public bool Equals(SelectedSite other)
            {
                return SiteId.Equals(other.SiteId) && Ver.Equals(other.Ver);
            }

            public override int GetHashCode()
            {
                return SiteId.GetHashCode() ^ Ver.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SelectedSite);
            }
        }

        public enum TableTypes
        {
            KeyValue,
            List,
            OneLineHeaderTable,
            TwoLineHeaderTable,
        }

        public interface ITableModel
        {
            TableTypes TableType { get; }
            string Label { get; set; }
        }

        public abstract class TableModelBase : ITableModel
        {
            protected TableModelBase(TableTypes tableType)
            {
                TableType = tableType;
            }
            [JsonProperty(Order = -10)]
            [JsonConverter(typeof(StringEnumConverter))]
            public TableTypes TableType { get; set; }
            [JsonProperty(Order = -9)]
            public string Label { get; set; }
        }

        public class FileInfoData
        {
            // TODO 出力した日など出力する
            public DateTime CreateDate = DateTime.Now;
            public List<Log> Logs;
        }

        public class SiteSettingInfoData
        {
            public List<Log> Logs = new ();
        }

        public class SiteSettingData
        {
            public SiteSettingInfoData Info;
            public List<Site> Sites;
        }

        public FileInfoData Info;
        public SiteSettingData SiteSetting;

        public static SettingsJsonConverter Convert(
            Context context,
            Param param)
        {
            var r = new SettingsJsonConverter();
            var info = new FileInfoData();
            r.Info = info;
            r.SiteSetting = SettingsJsonConverter.ConvertSiteSetting(context: context, param: param);
            if ((r.Info.Logs?.Count ?? 0) == 0) r.Info.Logs = null;
            return r;
        }

        private static SiteSettingData ConvertSiteSetting(
            Context context,
            Param param)
        {
            var s = new SiteSettingData();
            foreach (var siteId in param.SelectedSites)
            {
                var r = ConvertSite(context: context, param: param, siteId: siteId.SiteId, ver: siteId.Ver);
                if (r == null) continue;
                (s.Sites ??= new()).Add(r);
            }
            return s;
        }

        private static Site ConvertSite(
            Context context,
            Param param,
            long siteId,
            long ver = 0)
        {
            var siteModel = new SiteModel()
                .Get(
                    context: context,
                    where: DataSources.Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(siteId)
                        .Ver(ver, _using: ver != 0),
                    orderBy: DataSources.Rds.SitesOrderBy()
                        .Ver(SqlOrderBy.Types.desc),
                    tableType: ver != 0
                        ? Sqls.TableTypes.NormalAndHistory
                        : Sqls.TableTypes.Normal); 
            var ss = siteModel.SiteSettings;
            if (siteModel.SiteId == 0)
            {
                param.Logs.Add(new Log(Log.LogLevel.Error, "ConvertSite", $"SiteId {siteId} not found."));
                return null;
            }
            if (context.CanManageSite(ss: ss) == false)
            {
                param.Logs.Add(new Log(Log.LogLevel.Error, "ConvertSite", $"SiteId {siteId} access denied."));
                return null;
            }
            ss.Update_ColumnAccessControls();
            var logsOld = param.Logs;
            param.Logs = new ();
            Site site = null;
            try
            {
                site = new Site
                {
                    Info = new()
                    {
                        SiteId = siteId,
                        Ver = siteModel.Ver,
                        Title = siteModel.Title.DisplayValue.IsNotEmpty(),
                        ReferenceType = SiteUtilities.ReferenceTypeDisplayName(
                            context: context,
                            referenceType: siteModel.ReferenceType),
                    },
                    Tabs = new()
                    {
                        General = GeneralSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Guide = GuideSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Grid = GridSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Filter = FiltersSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Aggregations = AggregationsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Editor = EditorSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Links = LinksSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Histories = HistoriesSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Move = MoveSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Summaries = SummariesSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Formulas = FormulasSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Processes = ProcessesSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        StatusControls = StatusControlsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Views = ViewsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Notifications = NotificationsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Reminders = RemindersSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Imports = ImportsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Exports = ExportsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Calendar = CalendarSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Crosstab = CrosstabSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Gantt = GanttSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        BurnDown = BurnDownSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        TimeSeries = TimeSeriesSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Analy = AnalySettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Kamban = KambanSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        ImageLib = ImageLibSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Search = SearchSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Mail = MailSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        SiteIntegration = SiteIntegrationSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Styles = StylesSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Scripts = ScriptsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Htmls = HtmlsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        ServerScripts = ServerScriptsSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        Publish = PublishSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        SiteAccessControl = SiteAccessControlSettingsModel.Create(context: context, param: param, siteModel: siteModel),
                        RecordAccessControl = RecordAccessControlModel.Create(context: context, param: param, siteModel: siteModel),
                        ColumnAccessControl = ColumnAccessControlSettingsModel.Create(context: context, param: param, siteModel: siteModel)
                    }
                };
            }
            catch (Exception e)
            {
                var item = new Log(Log.LogLevel.Error, "ConvertSite", $"SiteId {siteId} ver {ver} convert failed. {e.Message}");
                if (site != null)
                {
                    param.Logs.Add(item);
                }
                else
                {
                    logsOld.Add(item);
                }
            }
            finally
            {
                if (param.Logs.Count != 0 && site != null) site.Info.Logs = param.Logs;
                param.Logs = logsOld;
            }
            return site;
        }

        public string RecordingJson(Context context)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            string json = JsonConvert.SerializeObject(this, settings);
            {
                // TODO Debug 後で削除
                var x = SettingsJsonConverter.Deserialize(json: json);
            }
            return json;
            //return this.ToJson(formatting: Newtonsoft.Json.Formatting.Indented);
        }

        public static SettingsJsonConverter Deserialize(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
            return JsonConvert.DeserializeObject<SettingsJsonConverter>(json, settings);
        }

        public class KeyValueHeader
        {
            public List<string> Labels;

            public static KeyValueHeader CreateDefault(Context context)
            {
                return new()
                {
                    Labels = DefaultLabel(context: context)
                };
            }

            public static List<string> DefaultLabel(Context context)
            {
                return new()
                {
                    Displays.SettingItems(context: context),
                    Displays.SettingValue(context: context),
                };
            }
        }

        public class KeyValueTableBase : TableModelBase
        {
            public KeyValueHeader Header;
            public List<ColumnModel> Columns;

            public class ColumnModel
            {
                public string Label;
                public string Name;
                public string Type;
                public string Value;
                [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
                public bool ReadOnly;
                [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
                public bool Changed;
            }

            public KeyValueTableBase() : base(TableTypes.KeyValue) { }
        }

        public class ListTableHeader
        {
            public List<string> Labels;
        }

        public class ListTableBase : TableModelBase
        {
            public ListTableHeader Header;
            public List<string> Columns;
            public ListTableBase() : base(TableTypes.List) { }
        }
        public class TableLabel
        {
            public string Key;
            public string Text;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ReadOnly;
            public TableLabel() { }
            public TableLabel(string key, string text, bool readOnly = false)
            {
                Key = key;
                Text = text;
                ReadOnly = readOnly;
            }
        }

        public class List2TableHeader
        {
            public List<TableLabel> Labels;
        }

        public class List2TableBase<TColumn> : TableModelBase
        {
            public List2TableHeader Header;
            public List<TColumn> Columns;
            public List2TableBase() : base(TableTypes.OneLineHeaderTable) { }
        }

        public class List3TableHeader
        {
            public class Tab
            {
                public KeyValuePair<string, string> TabName;
                public List<TableLabel> Labels;
                public Tab(KeyValuePair<string, string> tabName, List<TableLabel> labels)
                {
                    TabName = tabName;
                    Labels = labels;
                }
            }

            public List<List3TableHeader.Tab> Labels;

            public List3TableHeader()
            {
            }

            public List3TableHeader(List<List3TableHeader.Tab> labels)
            {
                Labels = labels;
            }
        }

        public class List3TableBase<TColumn> : TableModelBase
        {
            public List3TableHeader Header;
            public List<TColumn> Columns;
            public List3TableBase() : base(TableTypes.TwoLineHeaderTable) { }
        }

        public class ViewFiltersColumn
        {
            public List<FilterColumnItem> FilterColumns;
            public bool? KeepFilterState;
            public bool? Incomplete;
            public bool? Own;
            public bool? NearCompletionTime;
            public bool? Delay;
            public bool? Overdue;
            public string Search;
            public List<FilterConditionItem> ColumnFilterHash;
            public static ViewFiltersColumn SetViewFilters(
                Context context,
                View view,
                SiteSettings ss,
                string prefix = "")
            {
                var dst = new ViewFiltersColumn();
                if (prefix.IsNullOrEmpty())
                {
                    var filterColumns = view.FilterColumns ?? ss.FilterColumns;
                    var selecttable = ss
                        .ViewFilterSelectableOptions(
                            context: context,
                            filterColumns: view.FilterColumns ?? ss.FilterColumns);
                    dst.FilterColumns = filterColumns
                        .Select(v => new FilterColumnItem() {
                            Label = selecttable.Get(v).Title,
                            DbColumnName = v })
                        .ToList();
                }
                if (prefix.IsNullOrEmpty())
                {
                    dst.KeepFilterState = view.KeepFilterState == true;
                }
                if (view.HasIncompleteColumns(context: context, ss: ss))
                {
                    dst.Incomplete = view.Incomplete == true;
                }
                if (view.HasOwnColumns(context: context, ss: ss))
                {
                    dst.Own = view.Own == true;
                }
                if (view.HasNearCompletionTimeColumns(context: context, ss: ss))
                {
                    dst.NearCompletionTime = view.NearCompletionTime == true;
                }
                if (view.HasDelayColumns(context: context, ss: ss))
                {
                    dst.Delay = view.Delay == true;
                }
                if (view.HasOverdueColumns(context: context, ss: ss))
                {
                    dst.Overdue = view.Overdue == true;
                }
                dst.Search = view.Search;
                dst.ColumnFilterHash = view.ColumnFilterHash?
                    .Select(kv => ViewFilter(context: context, ss: ss, kv.Key, value: kv.Value))
                    .ToList() ?? new();
                return dst;
            }
            public class ColumnItemBase
            {
                [JsonProperty(Order = -10)]
                public string Label;
                [JsonProperty(Order = -9)]
                public string DbColumnName;
            }

            public class GridColumnItem : ColumnItemBase
            {
            }

            public class FilterConditionItem : ColumnItemBase
            {
                public string Value;
                public string Type;
            }

            public class FilterColumnItem : ColumnItemBase
            {
            }

            public class SorterColumnItem : ColumnItemBase
            {
                public string Order;
            }

            private static FilterConditionItem ViewFilter(
                Context context,
                SiteSettings ss,
                string columnName,
                string value = null)
            {
                var column = ss.GetColumn(context: context, columnName: columnName);
                var labelTitle = ss.LabelTitle(column: column);
                var retval = new FilterConditionItem();
                retval.Label = labelTitle;
                retval.DbColumnName = columnName;
                switch (column.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        switch (column.CheckFilterControlType)
                        {
                            case ColumnUtilities.CheckFilterControlTypes.OnOnly:
                                retval.Value = value.ToBool().ToString().ToLower();
                                retval.Type = "bool";
                                return retval;
                            case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                                retval.Value = ColumnUtilities
                                    .CheckFilterTypeOptions(context: context)
                                    .Where(o => o.Key == value)
                                    .Select(o => o.Value)
                                    .FirstOrDefault(string.Empty);
                                retval.Type = "string";
                                return retval;
                            default:
                                return null;
                        }
                    case Types.CsDateTime:
                        if (column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Default)
                        {
                            var select = value.Deserialize<List<string>>();
                            retval.Value = column.DateFilterOptions(context: context)
                                .Where(kv => select.Contains(kv.Key))
                                .Select(kv => kv.Value.Text)
                                .Join();
                            retval.Type = "string";
                            return retval;
                        }
                        else
                        {
                            retval.Value = HtmlViewFilters.GetDisplayDateFilterRange(
                                context: context,
                                value: value,
                                timepicker: column.DateTimepicker());
                            return retval;
                        }
                    case Types.CsNumeric:
                        if (column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Default)
                        {
                            var select = value.Deserialize<List<string>>();
                            retval.Value = (column.HasChoices()
                                ? HtmlFields.EditChoices(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    value: value,
                                    own: true,
                                    multiple: true,
                                    addNotSet: true)
                                : column.NumFilterOptions(context: context))
                                    .Where(kv => select.Contains(kv.Key))
                                    .Select(kv => kv.Value.Text)
                                    .Join();
                            retval.Type = "string";
                            return retval;
                        }
                        else
                        {
                            retval.Value = HtmlViewFilters.GetNumericFilterRange(value);
                            retval.Type = "string";
                            return retval;
                        }
                    case Types.CsString:
                        if (column.HasChoices())
                        {
                            var select = value.Deserialize<List<string>>();
                            retval.Value = HtmlFields.EditChoices(
                                context: context,
                                ss: ss,
                                column: column,
                                value: value,
                                own: true,
                                multiple: true,
                                addNotSet: true)
                                    .Where(kv => select.Contains(kv.Key))
                                    .Select(kv => kv.Value.Text)
                                    .Join();
                            retval.Type = "string";
                            return retval;
                        }
                        else
                        {
                            retval.Value = value;
                            retval.Type = "string";
                            return retval;
                        }
                    default:
                        break;
                }
                return null;
            }
        }

        public static class DumpUtils
        {
            public static string GetNotificationTypeText(
                Context context,
                Settings.Notification.Types type)
            {
                return (Parameters.Notification.ListOrder == null
                    ? NotificationUtilities.Types(context: context)
                    : NotificationUtilities.OrderTypes(context: context))
                        .Where(kv => kv.Key == type.ToInt().ToString())
                        .Select(kv => kv.Value)
                        .FirstOrDefault($"? {type.ToInt().ToString()}");
            }

            internal static string GetStatusText(
                Context context,
                SiteSettings ss,
                int statusId)
            {
                if (statusId == -1) return "*";
                return ss.GetColumn(
                        context: context,
                        columnName: "Status")
                    .ChoicesText?
                    .SplitReturn()
                    .Select(o => new Choice(o))
                    .GroupBy(o => o.Value)
                    .Where(o => o.Key == statusId.ToString())
                    .Select(o => o.First().Text)
                    .FirstOrDefault($"? {statusId}");
            }

            internal static string GetColumnLabelText(
                SiteSettings ss,
                string columnName,
                string missValue = null,
                bool addColumnName = false)
            {
                var labelText = ss.ColumnHash.Get(columnName)?.LabelText;
                return labelText == null
                    ? missValue
                    : addColumnName
                        ? $"{labelText}(@{columnName})"
                        : labelText;
            }
        }
        public class Site
        {
            public Information Info;
            public TabList Tabs;
            public class Information
            {
                public long SiteId;
                public int Ver;
                public string Title;
                public string ReferenceType;
                public List<Log> Logs;
            }
            public class TabList
            {
                public GeneralSettingsModel General;
                public GuideSettingsModel Guide;
                public GridSettingsModel Grid;
                public FiltersSettingsModel Filter;
                public AggregationsSettingsModel Aggregations;
                public EditorSettingsModel Editor;
                public LinksSettingsModel Links;
                public HistoriesSettingsModel Histories;
                public MoveSettingsModel Move;
                public SummariesSettingsModel Summaries;
                public FormulasSettingsModel Formulas;
                public ProcessesSettingsModel Processes;
                public StatusControlsSettingsModel StatusControls;
                public ViewsSettingsModel Views;
                public NotificationsSettingsModel Notifications;
                public RemindersSettingsModel Reminders;
                public ImportsSettingsModel Imports;
                public ExportsSettingsModel Exports;
                public CalendarSettingsModel Calendar;
                public CrosstabSettingsModel Crosstab;
                public GanttSettingsModel Gantt;
                public BurnDownSettingsModel BurnDown;
                public TimeSeriesSettingsModel TimeSeries;
                public AnalySettingsModel Analy;
                public KambanSettingsModel Kamban;
                public ImageLibSettingsModel ImageLib;
                public SearchSettingsModel Search;
                public MailSettingsModel Mail;
                public SiteIntegrationSettingsModel SiteIntegration;
                public StylesSettingsModel Styles;
                public ScriptsSettingsModel Scripts;
                public HtmlsSettingsModel Htmls;
                public ServerScriptsSettingsModel ServerScripts;
                public PublishSettingsModel Publish;
                public SiteAccessControlSettingsModel SiteAccessControl;
                public RecordAccessControlModel RecordAccessControl;
                public ColumnAccessControlSettingsModel ColumnAccessControl;
            }
        }
        public abstract class SettingsModelBase
        {
            public List<ITableModel> Tables;
            public string ButtonLabel;

        }

        public class GeneralSettingsModel : SettingsModelBase
        {
            internal static GeneralSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Sites/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new GeneralSettingsModel();
                obj.ButtonLabel = Displays.General(context: context);
                obj.Tables = new();
                var table = new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                };
                obj.Tables.Add(table);
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: siteModel.ReferenceType);
                var isWikis = siteModel.ReferenceType == "Wikis";
                var columns = new KeyValueTableBase.ColumnModel[]{
                        new ()
                        {
                            Label = Displays.Sites_SiteId(context: context),
                            Name = "SiteId",
                            Type = "long",
                            Value = siteModel.SiteId.ToString(),
                            ReadOnly = true
                        },
                        new ()
                        {
                            Label = Displays.Sites_Ver(context: context),
                            Name = "Ver",
                            Type = "long",
                            Value = siteModel.Ver.ToString(),
                            ReadOnly = true,
                        },
                        isWikis ? null : new ()
                        {
                            Label = Displays.Sites_Title(context: context),
                            Name = "Title",
                            Type = "string",
                            Value = siteModel.Title.DisplayValue,
                            Changed = siteNew.Title.DisplayValue != siteModel.Title.DisplayValue
                        },
                        new ()
                        {
                            Label = Displays.Sites_SiteName(context: context),
                            Name = "SiteName",
                            Type = "string",
                            Value = siteModel.SiteName,
                            Changed = siteNew.SiteName != siteModel.SiteName
                        },
                        isWikis ? null : new ()
                        {
                            Label = Displays.Sites_SiteGroupName(context: context),
                            Name = "SiteGroupName",
                            Type = "string",
                            Value = siteModel.SiteGroupName,
                            Changed = siteNew.SiteGroupName != siteModel.SiteGroupName
                        },
                        isWikis ? null : new ()
                        {
                            Label = Displays.Sites_TitleBody(context: context),
                            Name = "Body",
                            Type = "string",
                            Value = siteModel.Body,
                            Changed = siteNew.Body != siteModel.Body
                        },
                        new ()
                        {
                            Label = Displays.Sites_ReferenceType(context: context),
                            Name = "ReferenceType",
                            Type = "string",
                            Value = SiteUtilities.ReferenceTypeDisplayName(
                                context: context,
                                referenceType: siteModel.ReferenceType),
                            ReadOnly = true,
                        },
                        new ()
                        {
                            Label = Displays.DisableSiteConditions(context: context),
                            Name = "DisableSiteConditions",
                            Type = "bool",
                            Value = (Parameters.Site.DisableSiteConditions == true || ss.DisableSiteConditions == true).ToString().ToLower(),
                            Changed = ss.DisableSiteConditions != ssNew.DisableSiteConditions
                        }
                    };
                table.Columns.AddRange(columns.Where(v => v != null));
                return obj;
            }
        }

        public class GuideSettingsModel : SettingsModelBase
        {
            internal static GuideSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Sites/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new GuideSettingsModel();
                obj.ButtonLabel = Displays.Guide(context: context);
                obj.Tables = new();
                var table = new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                };
                obj.Tables.Add(table);
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(
                    context: context,
                    referenceType: siteModel.ReferenceType);
                var columns = new KeyValueTableBase.ColumnModel[]{
                        new ()
                        {
                            Label = Displays.CommonAllowExpand(context: context),
                            Name = "GuideAllowExpand",
                            Type = "bool",
                            Value = (ss.GuideAllowExpand ?? false).ToString(),
                            Changed = ssNew.GuideAllowExpand != ss.GuideAllowExpand
                        },
                        ss.GuideExpand.IsNullOrEmpty() ? null : new ()
                        {
                            Label = Displays.Expand(context: context),
                            Name = "GuideExpandField",
                            Type = "string",
                            Value = ss.GuideExpand == "0"
                                ? Displays.Close(context: context)
                                : Displays.Open(context:context),
                            Changed = ssNew.GuideExpand != ss.GuideExpand
                        },
                        (ss.ReferenceType == "Wikis") ? null : new ()
                        {
                            Label = ss.ReferenceType switch
                            {
                                "Sites" => Displays.MenuGuide(context: context),
                                "Dashboards"=> Displays.DashboardGuide(context: context),
                                _ => Displays.Sites_GridGuide(context: context)
                            },
                            Name = "Sites_GridGuide",
                            Type = "string",
                            Value = siteModel.GridGuide.IsNotEmpty(),
                            Changed = siteNew.GridGuide.IsNotEmpty() != siteModel.GridGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType != "Sites" && ss.ReferenceType != "Dashboards")? null: new ()
                        {
                            Label = Displays.Sites_EditorGuide(context: context),
                            Name = "Sites_EditorGuide",
                            Type = "string",
                            Value = siteModel.EditorGuide.IsNotEmpty(),
                            Changed = ssNew.EditorGuide.IsNotEmpty() != siteModel.EditorGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")? null : new ()
                        {
                            Label= Displays.Sites_CalendarGuide(context: context),
                            Name = "Sites_CalendarGuide",
                            Type = "string",
                            Value = siteModel.CalendarGuide.IsNotEmpty(),
                            Changed = siteNew.CalendarGuide.IsNotEmpty() != siteModel.CalendarGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")? null : new ()
                        {
                            Label= Displays.Sites_CrosstabGuide(context: context),
                            Name = "Sites_CrosstabGuide",
                            Type = "string",
                            Value = siteModel.CrosstabGuide.IsNotEmpty(),
                            Changed = ssNew.CrosstabGuide.IsNotEmpty() != siteModel.CrosstabGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType == "Issues")? null : new ()
                        {
                            Label= Displays.Sites_GanttGuide(context: context),
                            Name = "Sites_GanttGuide",
                            Type = "string",
                            Value = siteModel.GanttGuide.IsNotEmpty(),
                            Changed = siteNew.GanttGuide.IsNotEmpty() != siteModel.GanttGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType == "Issues")? null : new ()
                        {
                            Label= Displays.Sites_BurnDownGuide(context: context),
                            Name = "Sites_BurnDownGuide",
                            Type = "string",
                            Value = siteModel.BurnDownGuide.IsNotEmpty(),
                            Changed = ssNew.BurnDownGuide.IsNotEmpty() != siteModel.BurnDownGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")? null: new ()
                        {
                            Label= Displays.Sites_TimeSeriesGuide(context: context),
                            Name = "Sites_TimeSeriesGuide",
                            Type = "string",
                            Value = siteModel.TimeSeriesGuide.IsNotEmpty(),
                            Changed = ssNew.TimeSeriesGuide.IsNotEmpty() != siteModel.TimeSeriesGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")? null: new ()
                        {
                            Label= Displays.Sites_AnalyGuide(context: context),
                            Name = "Sites_AnalyGuide",
                            Type = "string",
                            Value = siteModel.AnalyGuide.IsNotEmpty(),
                            Changed = ssNew.AnalyGuide.IsNotEmpty() != siteModel.AnalyGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")? null: new ()
                        {
                            Label= Displays.Sites_KambanGuide(context: context),
                            Name = "Sites_KambanGuide",
                            Type = "string",
                            Value = siteModel.KambanGuide.IsNotEmpty(),
                            Changed = ssNew.KambanGuide.IsNotEmpty() != siteModel.KambanGuide.IsNotEmpty()
                        },
                        !(ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")? null: new ()
                        {
                            Label= Displays.Sites_ImageLibGuide(context: context),
                            Name = "Sites_ImageLibGuide",
                            Type = "string",
                            Value = siteModel.ImageLibGuide.IsNotEmpty(),
                            Changed = siteNew.ImageLibGuide.IsNotEmpty() != siteModel.ImageLibGuide.IsNotEmpty()
                        }
                    };
                table.Columns.AddRange(columns.Where(v => v != null));
                return obj;
            }
        }

        public class GridSettingsModel : SettingsModelBase
        {
            internal static GridSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new GridSettingsModel();
                obj.ButtonLabel = Displays.Grid(context: context);
                obj.Tables = new ITableModel[] {
                    ListTable.CreateGridListTable(context: context, param: param, siteModel: siteModel),
                    BulkUpdateTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateGridBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateGridBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var table = new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                };
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(
                    context: context,
                    referenceType: siteModel.ReferenceType);
                var columns = new KeyValueTableBase.ColumnModel[]{
                        new ()
                        {
                            Label = Displays.NumberPerPage(context: context),
                            Name = "GridPageSize",
                            Type = "long",
                            Value = ss.GridPageSize.ToDecimal().ToString(),
                            Changed = ssNew.GridPageSize != ss.GridPageSize
                        },
                        !(ss.Views?.Any() == true)? null: new ()
                        {
                            Label = Displays.DefaultView(context: context),
                            Name = "GridView",
                            Type = "string",
                            Value = ss.ViewSelectableOptions()
                                .Where(c => c.Key == ss.GridView.ToString())
                                .Select(c => c.Value.Text)
                                .FirstOrDefault(string.Empty),
                            Changed = ssNew.GridView != ss.GridView
                        },
                        !(ss.Views?.Any() == true)? null: new ()
                        {
                            Label = Displays.AllowViewReset(context: context),
                            Name = "AllowViewReset",
                            Type = "bool",
                            Value = (ss.AllowViewReset == true).ToString().ToLower(),
                            Changed = ssNew.AllowViewReset != ss.AllowViewReset
                        },
                        new ()
                        {
                            Label = Displays.GridEditorTypes(context: context),
                            Name = "GridEditorType",
                            Type = "string",
                            Value = ss.GridEditorType switch
                            {
                                SiteSettings.GridEditorTypes.Grid => Displays.EditInGrid(context: context),
                                SiteSettings.GridEditorTypes.Dialog => Displays.EditInDialog(context: context),
                                _ => string.Empty
                            },
                            Changed = (ssNew.GridEditorType ??  SiteSettings.GridEditorTypes.None)
                                != (ss.GridEditorType ??  SiteSettings.GridEditorTypes.None)
                        },
                        new ()
                        {
                            Label = Displays.HistoryOnGrid(context: context),
                            Name = "HistoryOnGrid",
                            Type = "bool",
                            Value = (ss.HistoryOnGrid == true).ToString().ToLower(),
                            Changed = ssNew.HistoryOnGrid != ss.HistoryOnGrid
                        },
                        new ()
                        {
                            Label = Displays.AlwaysRequestSearchCondition(context: context),
                            Name = "AlwaysRequestSearchCondition",
                            Type = "bool",
                            Value = (ss.AlwaysRequestSearchCondition == true).ToString().ToLower(),
                            Changed = ssNew.AlwaysRequestSearchCondition != ss.AlwaysRequestSearchCondition
                        },
                        new ()
                        {
                            Label = Displays.DisableLinkToEdit(context: context),
                            Name = "DisableLinkToEdit",
                            Type = "bool",
                            Value = (ss.DisableLinkToEdit == true).ToString().ToLower(),
                            Changed = ssNew.DisableLinkToEdit != ss.DisableLinkToEdit
                        },
                        new ()
                        {
                            Label = Displays.OpenEditInNewTab(context: context),
                            Name = "OpenEditInNewTab",
                            Type = "bool",
                            Value = (ss.OpenEditInNewTab == true).ToString().ToLower(),
                            Changed = ssNew.OpenEditInNewTab != ss.OpenEditInNewTab
                        },
                        !(Parameters.General.EnableExpandLinkPath == true)? null: new ()
                        {
                            Label = Displays.ExpandLinkPath(context: context),
                            Name = "EnableExpandLinkPath",
                            Type = "bool",
                            Value = (ss.EnableExpandLinkPath == true).ToString().ToLower(),
                            Changed = ssNew.EnableExpandLinkPath != ss.EnableExpandLinkPath
                        }
                    };
                table.Columns.AddRange(columns.Where(v => v != null));
                return table;
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                public ListTable()
                {
                    // Empty constructor for serialization
                }

                internal static ITableModel CreateGridListTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = new()
                    {
                        Labels = new()
                        {
                            new (){Key="ColumnName", Text= Displays.ColumnName(context: context), ReadOnly=true},
                            new (){Key="GridLabelText", Text= Displays.DisplayName(context: context)},
                            new (){Key="GridFormat", Text= Displays.GridFormat(context: context)},
                            new (){Key="ExtendedCellCss", Text= Displays.ExtendedCellCss(context: context)},
                            new (){Key="UseGridDesign", Text= Displays.UseCustomDesign(context: context)},
                            new (){Key="GridDesign", Text= Displays.CustomDesign(context: context) }
                        }
                    };
                    var ss = siteModel.SiteSettings;
                    table.Label = Displays.ListSettings(context: context);
                    table.Columns = new();
                    foreach (var column in ss.GetGridColumns(context: context))
                    {
                        var dst = new ListColumn();
                        table.Columns.Add(dst);
                        ListColumn.GridColumnDialogTab(
                            dst: dst,
                            context: context,
                            column: column,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.SetChangeColumnName(
                            dst: dst,
                            context: context,
                            column: column,
                            siteModel: siteModel);
                    }
                    return table;
                }

            }

            public class ListColumn
            {
                public string ColumnName;
                public string GridLabelText;
                public string GridFormat;
                public string ExtendedCellCss;
                public bool? UseGridDesign;
                public string GridDesign;
                public List<string> ChangedColumns = new ();

                internal static void GridColumnDialogTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Implem.Pleasanter.Libraries.Settings.Column column,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    if (column == null) return;
                    dst.ColumnName = column.ColumnName;
                    dst.GridLabelText = column.GridLabelText;
                    if (column.TypeName == "datetime")
                    {
                        dst.GridFormat = Displays.Get(
                            context: context,
                            id: column.GridFormat);
                    }
                    dst.ExtendedCellCss = column.ExtendedCellCss.IsNotEmpty();
                    dst.UseGridDesign = !column.GridDesign.IsNullOrEmpty();
                    dst.GridDesign = dst.UseGridDesign == true ? column.GridDesign : null;
                }

                internal static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Settings.Column column,
                    SiteModel siteModel)
                {
                    var ssNew = new SiteSettings(
                        context: context,
                        referenceType: siteModel.ReferenceType);
                    var inColumnNew = ssNew.ColumnHash.Get(dst.ColumnName);
                    var outColumnNew = new ListColumn();
                    ListColumn.GridColumnDialogTab(
                        dst: outColumnNew,
                        context: context,
                        column: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if ((v1 != null && v2 != null && !v1.Equals(v2))
                            || (v1 == null && v2 != null)
                            || (v1 != null && v2 == null))
                        {
                            dst.ChangedColumns.Add(n);
                        }
                    }
                }
            }

            public class BulkUpdateTable : List3TableBase<BulkUpdateColumn>
            {
                internal static BulkUpdateTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new BulkUpdateTable();
                    table.Label = Displays.BulkUpdateColumnSettings(context: context);
                    table.Header = BulkUpdateColumn.CreateHeader(context: context, param: param);
                    table.Columns = BulkUpdateColumn.CreateColumn(context: context, param: param, siteModel: siteModel);
                    return table;
                }
            }

            public class BulkUpdateColumn
            {
                public int Id;
                public string Title;
                public List<DetailColumn> Columns;

                internal static List3TableHeader CreateHeader(
                    Context context,
                    Param param)
                {
                    var labels = new List<List3TableHeader.Tab>
                    {
                        new (
                            tabName: new ("LockHeaderRight", ""),
                            labels: new ()
                            {
                                new ("Id",Displays.Id(context: context)),
                            }),
                        new (
                            tabName: new ("", ""),
                            labels: new ()
                            {
                                new ("Title",Displays.Title(context: context)),
                            }),
                        new (
                            tabName: new ("Columns",Displays.ColumnList(context: context)),
                            labels: new ()
                            {
                                new ("Columns.ColumnName",Displays.ColumnName(context: context)),
                                new ("Columns.ValidateRequired",Displays.Required(context: context)),
                                new ("Columns.EditorReadOnly",Displays.ReadOnly(context: context)),
                                new ("Columns.DefaultInput",Displays.DefaultInput(context: context)),
                            }),
                    };
                    return new List3TableHeader(labels: labels);
                }

                internal static List<BulkUpdateColumn> CreateColumn(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var list = new List<BulkUpdateColumn>();
                    var ss = siteModel.SiteSettings;
                    if (ss.BulkUpdateColumns != null)
                    {
                        foreach (var col in ss.BulkUpdateColumns)
                        {
                            var dst = new BulkUpdateColumn();
                            list.Add(dst);
                            GridColumnDialogTab(dst: dst, context: context, column: col, ss: ss);
                            SetChangeColumnName(dst: dst, context: context, column: col, ss: ss);
                        }
                    }
                    return list;
                }

                internal static void GridColumnDialogTab(
                    BulkUpdateColumn dst,
                    Context context,
                    Settings.BulkUpdateColumn column,
                    SiteSettings ss)
                {
                    dst.Id = column.Id;
                    dst.Title = column.Title;
                    dst.Columns = new List<DetailColumn>();
                    foreach (var key in column.Columns)
                    {
                        var v = column.Details.Get(key) ?? new BulkUpdateColumnDetail();
                        var detail = new DetailColumn();
                        DetailColumn.GridColumnDialogTab(dst: detail, columnName: key, v: v);
                        dst.Columns.Add(detail);
                    }
                }

                internal static void SetChangeColumnName(
                    BulkUpdateColumn dst,
                    Context context,
                    Settings.BulkUpdateColumn column,
                    SiteSettings ss)
                {
                    foreach (var item in dst.Columns)
                    {
                        DetailColumn.SetChangeColumnName(dst: item);
                    }
                }

                public class DetailColumn
                {
                    public string ColumnName;
                    public bool? ValidateRequired;
                    public bool? EditorReadOnly;
                    public string DefaultInput;
                    public List<string> ChangedColumns;
                    public List<string> ReadOnlyColumns;

                    internal static void GridColumnDialogTab(
                        DetailColumn dst,
                        string columnName,
                        BulkUpdateColumnDetail v)
                    {
                        dst.ColumnName = columnName;
                        dst.ValidateRequired = v.ValidateRequired == true;
                        dst.EditorReadOnly = v.EditorReadOnly == true;
                        dst.DefaultInput = v.DefaultInput.IsNotEmpty();
                    }

                    internal static void SetChangeColumnName(DetailColumn dst)
                    {
                        dst.ReadOnlyColumns = new List<string>() { "ColumnName" };
                        var changedColumns = new List<string>()
                        {
                            (dst.ValidateRequired != true)? null :"ValidateRequired",
                            (dst.EditorReadOnly != true)? null :"EditorReadOnly",
                            (dst.DefaultInput.IsNullOrEmpty())? null :"DefaultInput"
                        }
                            .Where(c => c != null).ToList();
                        if (changedColumns.Any()) dst.ChangedColumns = changedColumns;
                    }
                }
            }
        }

        public class FiltersSettingsModel : SettingsModelBase
        {
            internal static FiltersSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new FiltersSettingsModel();
                obj.ButtonLabel = Displays.Filters(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateFiltersBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public static ITableModel CreateFiltersBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var table = new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                };
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(
                    context: context,
                    referenceType: siteModel.ReferenceType);
                var columns = new KeyValueTableBase.ColumnModel[]{
                        new ()
                        {
                            Label = Displays.NearCompletionTimeAfterDays(context: context),
                            Name = "NearCompletionTimeAfterDays",
                            Type = "long",
                            Value = ss.NearCompletionTimeAfterDays.ToDecimal().ToString(),
                            Changed = ssNew.NearCompletionTimeAfterDays != ss.NearCompletionTimeAfterDays
                        },
                        new ()
                        {
                            Label = Displays.NearCompletionTimeBeforeDays(context: context),
                            Name = "NearCompletionTimeBeforeDays",
                            Type = "long",
                            Value = ss.NearCompletionTimeBeforeDays.ToDecimal().ToString(),
                            Changed = ssNew.NearCompletionTimeBeforeDays != ss.NearCompletionTimeBeforeDays
                        },
                        new ()
                        {
                            Label = Displays.UseFilterButton(context: context),
                            Name = "UseFilterButton",
                            Type = "bool",
                            Value = (ss.UseFilterButton == true).ToString().ToLower(),
                            Changed = ssNew.UseFilterButton != ss.UseFilterButton
                        },
                        new ()
                        {
                            Label = Displays.UseFiltersArea(context: context),
                            Name = "UseFiltersArea",
                            Type = "bool",
                            Value = (ss.UseFiltersArea == true).ToString().ToLower(),
                            Changed = ssNew.UseFiltersArea != ss.UseFiltersArea
                        },
                        new ()
                        {
                            Label = Displays.UseNegativeFilters(context: context),
                            Name = "UseNegativeFilters",
                            Type = "bool",
                            Value = (ss.UseNegativeFilters == true).ToString().ToLower(),
                            Changed = ssNew.UseNegativeFilters != ss.UseNegativeFilters
                        },
                        new ()
                        {
                            Label = Displays.UseGridHeaderFilters(context: context),
                            Name = "UseGridHeaderFilters",
                            Type = "bool",
                            Value = (ss.UseRelatingColumnsOnFilter == false && ss.UseGridHeaderFilters == true).ToString().ToLower(),
                            Changed = ssNew.UseRelatingColumnsOnFilter != ss.UseRelatingColumnsOnFilter
                                && ssNew.UseGridHeaderFilters != ss.UseGridHeaderFilters
                        },
                        new ()
                        {
                            Label = Displays.UseRelatingColumns(context: context),
                            Name = "UseRelatingColumnsOnFilter",
                            Type = "bool",
                            Value = (ss.UseRelatingColumnsOnFilter == true).ToString().ToLower(),
                            Changed = ssNew.UseRelatingColumnsOnFilter != ss.UseRelatingColumnsOnFilter
                        },
                        new ()
                        {
                            Label = Displays.UseIncompleteFilter(context: context),
                            Name = "UseIncompleteFilter",
                            Type = "bool",
                            Value = (ss.UseIncompleteFilter == true).ToString().ToLower(),
                            Changed = ssNew.UseIncompleteFilter != ss.UseIncompleteFilter
                        },
                        new ()
                        {
                            Label = Displays.UseOwnFilter(context: context),
                            Name = "UseOwnFilter",
                            Type = "bool",
                            Value = (ss.UseOwnFilter == true).ToString().ToLower(),
                            Changed = ssNew.UseOwnFilter != ss.UseOwnFilter
                        },
                        new ()
                        {
                            Label = Displays.UseNearCompletionTimeFilter(context: context),
                            Name = "UseNearCompletionTimeFilter",
                            Type = "bool",
                            Value = (ss.UseNearCompletionTimeFilter == true).ToString().ToLower(),
                            Changed = ssNew.UseNearCompletionTimeFilter != ss.UseNearCompletionTimeFilter
                        },
                        new ()
                        {
                            Label = Displays.UseDelayFilter(context: context),
                            Name = "UseDelayFilter",
                            Type = "bool",
                            Value = (ss.UseDelayFilter == true).ToString().ToLower(),
                            Changed = ssNew.UseDelayFilter != ss.UseDelayFilter
                        },
                        new ()
                        {
                            Label = Displays.UseOverdueFilter(context: context),
                            Name = "UseOverdueFilter",
                            Type = "bool",
                            Value = (ss.UseOverdueFilter == true).ToString().ToLower(),
                            Changed = ssNew.UseOverdueFilter != ss.UseOverdueFilter
                        },
                        new ()
                        {
                            Label = Displays.UseSearchFilter(context: context),
                            Name = "UseSearchFilter",
                            Type = "bool",
                            Value = (ss.UseSearchFilter == true).ToString().ToLower(),
                            Changed = ssNew.UseSearchFilter != ss.UseSearchFilter
                        },
                    };
                table.Columns.AddRange(columns.Where(v => v != null));
                return table;
            }

            public class ListTable : List3TableBase<ListColumn>
            {
                public static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaders(context: context, param: param);
                    table.Columns = ListColumn.CreateColumns(context: context, param: param, siteModel: siteModel);
                    return table;
                }
            }

            public class ListColumn
            {
                public string ColumnName;
                public string GridLabelText;
                public string DateFilterSetMode;
                public string CheckFilterControlType;
                public decimal? NumFilterMin;
                public decimal? NumFilterMax;
                public decimal? NumFilterStep;
                public decimal? DateFilterMinSpan;
                public decimal? DateFilterMaxSpan;
                public bool? DateFilterFy;
                public bool? DateFilterHalf;
                public bool? DateFilterQuarter;
                public bool? DateFilterMonth;
                public string SearchTypes;
                public List<string> ChangedColumns;
                public List<string> ReadOnlyColumns;

                internal static List<ListColumn> CreateColumns(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var columns = new List<ListColumn>();
                    var ss = siteModel.SiteSettings;
                    foreach (var column in ss.GetFilterColumns(context: context, view: null))
                    {
                        var dst = new ListColumn();
                        columns.Add(dst);
                        ListColumn.FilterColumnDialogTab(
                            dst: dst,
                            context: context,
                            column: column,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.SetChangeColumnName(
                            dst: dst,
                            context: context,
                            column: column,
                            siteModel: siteModel);
                    }
                    return columns;
                }

                internal static List3TableHeader CreateHeaders(
                    Context context,
                    Param param)
                {
                    var labels = new List<List3TableHeader.Tab>
                    {
                        new (
                            tabName: new ("LockHeaderRight", ""),
                            labels: new ()
                            {
                                new ("ColumnName", Displays.ColumnName(context: context)),
                            }),
                        new (
                            tabName: new ("General", Displays.General(context: context)),
                            labels: new ()
                            {
                                new ("GridLabelText",Displays.DisplayName(context: context)),
                                new ("DateFilterSetMode",Displays.DateFilterSetMode(context: context)),
                            }),
                        new (
                            tabName: new ("Types.CsBool",Displays.Check(context:context)),
                            labels: new ()
                            {
                                new ("CheckFilterControlType",Displays.ControlType(context: context)),
                            }),
                        new (
                            tabName: new ("Types.CsNumeric",Displays.Num(context: context)),
                            labels: new ()
                            {
                                new ("NumFilterMin",Displays.Min(context: context)),
                                new ("NumFilterMax",Displays.Max(context: context)),
                                new ("NumFilterStep",Displays.Step(context: context)),
                            }),
                        new (
                            tabName: new ("Types.CsDateTime", Displays.Date(context: context)),
                            labels: new ()
                            {
                                new ("DateFilterMinSpan",Displays.Min(context: context)),
                                new ("DateFilterMaxSpan",Displays.Max(context: context)),
                                new ("DateFilterFy",Displays.UseFy(context: context)),
                                new ("DateFilterHalf",Displays.UseHalf(context: context)),
                                new ("DateFilterQuarter",Displays.UseQuarter(context: context)),
                                new ("DateFilterMonth",Displays.UseMonth(context: context)),
                            }),
                        new (
                            tabName: new ("Types.CsString",Displays.ClassAndDescription(context: context)),
                            labels: new ()
                            {
                                new ("SearchTypes",Displays.SearchTypes(context: context)),
                            }),
                    };
                    return new List3TableHeader(labels: labels);
                }

                public static void FilterColumnDialogTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Implem.Pleasanter.Libraries.Settings.Column column,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    if (column == null) return;
                    var type = column.TypeName.CsTypeSummary();
                    var noSettings = new string[]
                    {
                        "Owner",
                        "Manager",
                        "Status"
                    }.Contains(column.ColumnName);
                    dst.ColumnName = column.ColumnName;
                    dst.GridLabelText = column.GridLabelText;
                    if ((type == Types.CsDateTime
                        || type == Types.CsNumeric)
                            && !noSettings)
                    {
                        dst.DateFilterSetMode = column.DateFilterSetMode switch
                        {
                            ColumnUtilities.DateFilterSetMode.Range => Displays.Range(context: context),
                            _ => Displays.Default(context: context)
                        };
                    }
                    if (type == Types.CsBool)
                    {
                        dst.CheckFilterControlType = ColumnUtilities
                            .CheckFilterControlTypeOptions(context: context)
                            .Where(kv => kv.Key == column.CheckFilterControlType.ToInt().ToString())
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                    }
                    else if (type == Types.CsNumeric)
                    {
                        if (!noSettings)
                        {
                            dst.NumFilterMin = column.NumFilterMin.ToDecimal();
                            dst.NumFilterMax = column.NumFilterMax.ToDecimal();
                            dst.NumFilterStep = column.NumFilterStep.ToDecimal();
                        }
                    }
                    else if (type == Types.CsDateTime)
                    {
                        dst.DateFilterMinSpan = column.DateFilterMinSpan.ToDecimal();
                        dst.DateFilterMaxSpan = column.DateFilterMaxSpan.ToDecimal();
                        dst.DateFilterFy = column.DateFilterFy == true;
                        dst.DateFilterHalf = column.DateFilterHalf == true;
                        dst.DateFilterQuarter = column.DateFilterQuarter == true;
                        dst.DateFilterMonth = column.DateFilterMonth == true;
                    }
                    else if (type == Types.CsString)
                    {
                        dst.SearchTypes = ColumnUtilities
                            .SearchTypeOptions(context: context)
                            .Where(kv => kv.Key == column.SearchType?.ToInt().ToString())
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                    }
                }

                public static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Settings.Column column,
                    SiteModel siteModel)
                {
                    var ssNew = new SiteSettings(
                        context: context,
                        referenceType: siteModel.ReferenceType);
                    var inColumnNew = ssNew.ColumnHash.Get(dst.ColumnName);
                    var outColumnNew = new ListColumn();
                    FilterColumnDialogTab(
                        dst: outColumnNew,
                        context: context,
                        column: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "GridLabelText") continue;
                        if (n == "ColumnName") continue;
                        if (n == "ChangedColumns") continue;
                        if (n == "ReadOnlyColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if ((v1 != null && v2 != null && !v1.Equals(v2))
                            || (v1 == null && v2 != null)
                            || (v1 != null && v2 == null))
                        {
                            (dst.ChangedColumns ??= new()).Add(n);
                        }
                    }
                    dst.ReadOnlyColumns = new List<string>()
                        {
                            "GridLabelText",
                        };
                }
            }
        }

        public class AggregationsSettingsModel : SettingsModelBase
        {
            internal static AggregationsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new AggregationsSettingsModel();
                obj.ButtonLabel = Displays.Aggregations(context: context);
                obj.Tables = new()
                {
                    new ListTableBase
                    {
                        Header = new ()
                        {
                            Labels = new () { Displays.SettingValue(context: context) },
                        },
                        Columns = siteModel.SiteSettings.AggregationDestination(context: context).Select(v => v.Value.Text).ToList()
                    }
                };
                return obj;
            }
        }

        public class EditorSettingsModel : SettingsModelBase
        {
            internal static EditorSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new EditorSettingsModel();
                obj.ButtonLabel = Displays.Editor(context: context);
                obj.Tables = new ITableModel[] {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    OtherTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    RelatingTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateEditorBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateEditorBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var table = new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                };
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(
                    context: context,
                    referenceType: siteModel.ReferenceType);
                var columns = new KeyValueTableBase.ColumnModel[]{
                        new ()
                        {
                            Label = Displays.AutoVerUpType(context: context),
                            Name = "AutoVerUpType",
                            Type = "string",
                            Value = ss.AutoVerUpType switch
                            {
                                Implem.Pleasanter.Libraries.Models.Versions.AutoVerUpTypes.Always=> Displays.Always(context: context),
                                Implem.Pleasanter.Libraries.Models.Versions.AutoVerUpTypes.Disabled=>Displays.Disabled(context: context),
                                _=>Displays.Default(context: context)
                            },
                            Changed = ssNew.GridPageSize != ss.GridPageSize
                        },
                        new ()
                        {
                            Label = Displays.AllowEditingComments(context: context),
                            Name = "AllowEditingComments",
                            Type = "bool",
                            Value = (ss.AllowEditingComments == true).ToString().ToLower(),
                            Changed = ssNew.AllowEditingComments != ss.AllowEditingComments
                        },
                        new ()
                        {
                            Label = Displays.AllowCopy(context: context),
                            Name = "AllowCopy",
                            Type = "bool",
                            Value = (ss.AllowCopy == true).ToString().ToLower(),
                            Changed = ssNew.AllowCopy != ss.AllowCopy
                        },
                        new ()
                        {
                            Label = Displays.AllowReferenceCopy(context: context),
                            Name = "AllowReferenceCopy",
                            Type = "bool",
                            Value = (ss.AllowReferenceCopy == true).ToString().ToLower(),
                            Changed = ssNew.AllowReferenceCopy != ss.AllowReferenceCopy
                        },
                        new ()
                        {
                            Label = Displays.CharToAddWhenCopying(context: context),
                            Name = "CharToAddWhenCopying",
                            Type = "string",
                            Value = ss.CharToAddWhenCopying.IsNotEmpty(),
                            Changed = ssNew.CharToAddWhenCopying != ss.CharToAddWhenCopying
                        },
                        !( ss.ReferenceType == "Issues")? null: new()
                        {
                            Label = Displays.AllowSeparate(context: context),
                            Name = "AllowSeparate",
                            Type = "bool",
                            Value = (ss.AllowSeparate == true).ToString().ToLower(),
                            Changed = ssNew.AllowSeparate != ss.AllowSeparate
                        },
                        new()
                        {
                            Label = Displays.AllowLockTable(context: context),
                            Name = "AllowLockTable",
                            Type = "bool",
                            Value = (ss.AllowLockTable == true).ToString().ToLower(),
                            Changed = ssNew.AllowLockTable != ss.AllowLockTable
                        },
                        new()
                        {
                            Label = Displays.HideLink(context: context),
                            Name = "HideLink",
                            Type = "bool",
                            Value = (ss.HideLink == true).ToString().ToLower(),
                            Changed = ssNew.HideLink != ss.HideLink
                        },
                        new()
                        {
                            Label = Displays.SwitchRecordWithAjax(context: context),
                            Name = "SwitchRecordWithAjax",
                            Type = "bool",
                            Value = (ss.SwitchRecordWithAjax == true).ToString().ToLower(),
                            Changed = ssNew.SwitchRecordWithAjax != ss.SwitchRecordWithAjax
                        },
                        new()
                        {
                            Label = Displays.SwitchCommandButtonsAutoPostBack(context: context),
                            Name = "SwitchCommandButtonsAutoPostBack",
                            Type = "bool",
                            Value = (ss.SwitchCommandButtonsAutoPostBack == true).ToString().ToLower(),
                            Changed = ssNew.SwitchCommandButtonsAutoPostBack != ss.SwitchCommandButtonsAutoPostBack
                        },
                        new()
                        {
                            Label = Displays.DeleteImageWhenDeleting(context: context),
                            Name = "DeleteImageWhenDeleting",
                            Type = "bool",
                            Value = (ss.DeleteImageWhenDeleting == true).ToString().ToLower(),
                            Changed = ssNew.DeleteImageWhenDeleting != ss.DeleteImageWhenDeleting
                        },
                    };
                table.Columns.AddRange(columns.Where(v => v != null));
                return table;
            }

            public class ListTable : List3TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaders(context: context, param: param);
                    table.Columns = ListColumn.CreateColumns(context: context, param: param, siteModel: siteModel);
                    return table;
                }

            }

            public class ListColumn
            {
                public string ColumnName;
                public string LabelText;
                public bool? Tab;
                public bool? Section;
                public bool? Links;
                public string TextAlign;
                public string ChoicesText;
                public string ChoicesControlType;
                public bool? UseSearch;
                public decimal? MaxLength;
                public string ViewerSwitchingType;
                public bool? ValidateRequired;
                public bool? AllowBulkUpdate;
                public bool? NoDuplication;
                public string MessageWhenDuplicated;
                public bool? CopyByDefault;
                public bool? EditorReadOnly;
                public string EditorFormat;
                public string DefaultInput;
                public string Format;
                public bool? Nullable;
                public string Unit;
                public int? DecimalPlaces;
                public string RoundingType;
                public string ControlType;
                public decimal? Min;
                public decimal? Max;
                public decimal? Step;
                public int? DateTimeStep;
                public bool? AllowDeleteAttachments;
                public bool? NotDeleteExistHistory;
                public string BinaryStorageProvider;
                public bool? OverwriteSameFileName;
                public decimal? LimitQuantity;
                public decimal? LimitSize;
                public decimal? TotalLimitSize;
                public decimal? LocalFolderLimitSize;
                public decimal? LocalFolderTotalLimitSize;
                public bool? AllowImage;
                public decimal? ThumbnailLimitSize;
                public bool? ImportKey;
                public string Description;
                public bool? MultipleSelections;
                public bool? NotInsertBlankChoice;
                public bool? Anchor;
                public bool? OpenAnchorNewTab;
                public string AnchorFormat;
                public string TitleColumns;
                public string TitleSeparator;
                public bool? AutoPostBack;
                public string ColumnsReturnedWhenAutomaticPostback;
                public bool? NoWrap;
                public bool? Hide;
                public string ExtendedFieldCss;
                public string ExtendedControlCss;
                public string FullTextType;
                public string AutoNumberingFormat;
                public string AutoNumberingResetType;
                public int? AutoNumberingDefault;
                public int? AutoNumberingStep;
                public string ExtendedHtmlBeforeField;
                public string ExtendedHtmlBeforeLabel;
                public string ExtendedHtmlBetweenLabelAndControl;
                public string ExtendedHtmlAfterControl;
                public string ExtendedHtmlAfterField;
                public string ClientRegexValidation;
                public string ServerRegexValidation;
                public string RegexValidationMessage;
                public string Custom;
                public int? SectionId;
                public string SectionLabelText;
                public bool? SectionAllowExpand;
                public string SectionExpand;
                public List<string> ChangedColumns = new ();

                internal static List3TableHeader CreateHeaders(
                    Context context,
                    Param param)
                {
                    var labels = new List<List3TableHeader.Tab>
                {
                    new (
                        tabName: new ("LockHeaderRight", ""),
                        labels: new ()
                        {
                            new ("ColumnName", Displays.ColumnName(context: context)),
                        }),
                    new (
                        tabName: new ("General", Displays.General(context: context)),
                        labels: new ()
                        {
                            new ("LabelText",Displays.DisplayName(context: context)),
                            new ("Tab",Displays.Tab(context: context)),
                            new ("Section",Displays.Section(context: context)),
                            new ("Links",Displays.Links(context: context)),
                            new ("TextAlign", Displays.TextAlign(context: context)),
                            new ("ChoicesText", Displays.OptionList(context: context)),
                            new ("ChoicesControlType", Displays.ControlType(context: context)),
                            new ("UseSearch", Displays.UseSearch(context: context)),
                            new ("MaxLength", Displays.MaxLength(context: context)),
                            new ("ViewerSwitchingType", Displays.ViewerSwitchingType(context: context)),
                            new ("ValidateRequired", Displays.Required(context: context)),
                            new ("AllowBulkUpdate", Displays.AllowBulkUpdate(context: context)),
                            new ("NoDuplication", Displays.NoDuplication(context: context)),
                            new ("MessageWhenDuplicated", Displays.MessageWhenDuplicated(context: context)),
                            new ("CopyByDefault", Displays.CopyByDefault(context: context)),
                            new ("EditorReadOnly", Displays.ReadOnly(context: context)),
                            new ("EditorFormat", Displays.EditorFormat(context: context)),
                            new ("DefaultInput", Displays.DefaultInput(context: context)),
                            new ("ImportKey", Displays.ImportKey(context: context)),
                            new ("Format", Displays.Format(context: context)),
                            new ("Custom", Displays.Custom(context: context)),
                            new ("Nullable", Displays.Nullable(context: context)),
                            new ("Unit", Displays.Unit(context: context)),
                            new ("DecimalPlaces", Displays.DecimalPlaces(context: context)),
                            new ("RoundingType", Displays.RoundingType(context: context)),
                            new ("ControlType", Displays.ControlType(context: context)),
                            new ("Min", Displays.Min(context: context)),
                            new ("Max", Displays.Max(context: context)),
                            new ("Step", Displays.Step(context: context)),
                            new ("DateTimeStep", Displays.MinutesStep(context: context)),
                            new ("AllowDeleteAttachments", Displays.AllowDeleteAttachments(context: context)),
                            new ("NotDeleteExistHistory", Displays.NotDeleteExistHistory(context: context)),
                            new ("BinaryStorageProvider", Displays.BinaryStorageProvider(context: context)),
                            new ("OverwriteSameFileName", Displays.OverwriteSameFileName(context: context)),
                            new ("LimitQuantity", Displays.LimitQuantity(context: context)),
                            new ("LimitSize", Displays.LimitSize(context: context)),
                            new ("TotalLimitSize", Displays.LimitTotalSize(context: context)),
                            new ("LocalFolderLimitSize", Displays.LocalFolder(context: context) + Displays.LimitSize(context: context)),
                            new ("LocalFolderTotalLimitSize", Displays.LocalFolder(context: context) + Displays.LimitTotalSize(context: context)),
                            new ("AllowImage", Displays.AllowImage(context: context)),
                            new ("ThumbnailLimitSize", Displays.ThumbnailLimitSize(context: context)),
                            new ("Description", Displays.Description(context: context)),
                            new ("MultipleSelections", Displays.MultipleSelections(context: context)),
                            new ("NotInsertBlankChoice", Displays.NotInsertBlankChoice(context: context)),
                            new ("Anchor", Displays.Anchor(context: context)),
                            new ("OpenAnchorNewTab", Displays.OpenAnchorNewTab(context: context)),
                            new ("AnchorFormat", Displays.AnchorFormat(context: context)),
                            new ("TitleColumns", Displays.CurrentSettings(context: context)),
                            new ("TitleSeparator", Displays.TitleSeparator(context: context)),
                            new ("AutoPostBack", Displays.AutoPostBack(context: context)),
                            new ("ColumnsReturnedWhenAutomaticPostback", Displays.ColumnsReturnedWhenAutomaticPostback(context: context)),
                            new ("NoWrap", Displays.NoWrap(context: context)),
                            new ("Hide", Displays.Hide(context: context)),
                            new ("ExtendedFieldCss", Displays.ExtendedFieldCss(context: context)),
                            new ("ExtendedControlCss", Displays.ExtendedControlCss(context: context)),
                            new ("FullTextTypes", Displays.FullTextTypes(context: context))
                        }),
                    new (
                        tabName: new("AutoNumbering", Displays.AutoNumbering(context: context)),
                        labels: new ()
                        {
                            new ("AutoNumberingFormat", Displays.Format(context: context)),
                            new ("AutoNumberingResetType", Displays.ResetType(context: context)),
                            new ("AutoNumberingDefault", Displays.DefaultInput(context: context)),
                            new ("AutoNumberingStep", Displays.Step(context: context))
                        }),
                    new (
                        tabName: new("ValidateInput", Displays.ValidateInput(context: context)),
                        labels: new ()
                        {
                            new ("ClientRegexValidation", Displays.ClientRegexValidation(context: context)),
                            new ("ServerRegexValidation", Displays.ServerRegexValidation(context: context)),
                            new ("RegexValidationMessage", Displays.RegexValidationMessage(context: context))
                        }),
                    new (
                        tabName: new("ExtendedHtml", Displays.ExtendedHtml(context: context)),
                        labels: new ()
                        {
                            new ("ExtendedHtmlBeforeField", Displays.ExtendedHtmlBeforeField(context: context)),
                            new ("ExtendedHtmlBeforeLabel", Displays.ExtendedHtmlBeforeLabel(context: context)),
                            new ("ExtendedHtmlBetweenLabelAndControl", Displays.ExtendedHtmlBetweenLabelAndControl(context: context)),
                            new ("ExtendedHtmlAfterControl", Displays.ExtendedHtmlAfterControl(context: context)),
                            new ("ExtendedHtmlAfterField", Displays.ExtendedHtmlAfterField(context: context))
                        }),
                    new (
                        tabName: new("Section", Displays.Section(context: context)),
                        labels: new ()
                        {
                            new ("SectionId", Displays.Id(context: context)),
                            new ("SectionLabelText", Displays.DisplayName(context: context)),
                            new ("SectionAllowExpand", Displays.AllowExpand(context: context)),
                            new ("SectionExpand", Displays.Expand(context: context)),
                        })
                    };
                    return new List3TableHeader(labels: labels);
                }

                internal static List<ListColumn> CreateColumns(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var ss = siteModel.SiteSettings;
                    var columns = new List<ListColumn>();
                    foreach (var tab in ss.EditorColumnHash)
                    {
                        columns.Add(CreateTabColumn(context: context, tabKey: tab.Key, ss: ss));
                        foreach (var columnName in tab.Value)
                        {
                            columns.Add(CreateNormalColumns(context: context, columnName: columnName, siteModel: siteModel, ss: ss));
                        }
                    }
                    return columns;
                }

                private static ListColumn CreateTabColumn(
                    Context context,
                    string tabKey,
                    SiteSettings ss)
                {
                    var dst = new ListColumn();
                    dst.Tab = true;
                    if (tabKey == "General")
                    {
                        dst.LabelText = ss.GeneralTabLabelText.IsNullOrEmpty()
                            ? Displays.General(context: context)
                            : ss.GeneralTabLabelText;
                    }
                    else
                    {
                        dst.LabelText = ss.Tabs?
                            .Where(v => v.Id == ss.TabId(tabKey))
                            .Select(v => v.LabelText)
                            .FirstOrDefault()
                            .IsNotEmpty();
                    }
                    return dst;
                }

                private static ListColumn CreateNormalColumns(
                    Context context,
                    string columnName,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    if (columnName.StartsWith("_Section-"))
                    {
                        var dst = new ListColumn();
                        var section = ss.Sections
                            .Where(v => v.Id == ss.SectionId(columnName: columnName))
                            .FirstOrDefault();
                        dst.LabelText = section.LabelText.IsNotEmpty();
                        dst.Section = true;
                        dst.SectionId = section.Id;
                        dst.SectionLabelText = section.LabelText.IsNotEmpty();
                        dst.SectionAllowExpand = section.AllowExpand == true;
                        dst.SectionExpand = section.Expand == true
                            ? Displays.Open(context: context)
                            : Displays.Close(context: context);
                        return dst;
                    }
                    else if (columnName.StartsWith("_Links-"))
                    {
                        var dst = new ListColumn();
                        dst.Links = true;
                        var linkId = ss.LinkId(columnName: columnName);
                        dst.LabelText = ss.Sources.Get(linkId)?.Title
                            ?? ss.Destinations.Get(linkId)?.Title
                            ?? string.Empty;
                        return dst;
                    }
                    else
                    {
                        var dst = new ListColumn();
                        var column = ss.ColumnHash.Get(columnName);
                        if (column.TypeName == "nvarchar"
                            && column.ControlType != "Attachments")
                        {
                            ListColumn.EditorColumnDialogTab(
                                dst: dst,
                                context: context,
                                column: column,
                                siteModel: siteModel,
                                ss: ss);
                            ListColumn.AutoNumberingSettingTab(
                                dst: dst,
                                context: context,
                                column: column,
                                siteModel: siteModel,
                                ss: ss);
                            ListColumn.EditorDetailsettingTab(
                                dst: dst,
                                context: context,
                                column: column,
                                siteModel: siteModel);
                            ListColumn.ExtendedHtmlSettingTab(
                                dst: dst,
                                context: context,
                                column: column,
                                siteModel: siteModel);
                        }
                        else
                        {
                            ListColumn.EditorColumnDialogTab(
                                dst: dst,
                                context: context,
                                column: column,
                                siteModel: siteModel,
                                ss: ss);
                            ListColumn.ExtendedHtmlSettingTab(
                                dst: dst,
                                context: context,
                                column: column,
                                siteModel: siteModel);
                        }
                        ListColumn.SetChangeColumnName(
                            dst: dst,
                            context: context,
                            column: column,
                            siteModel: siteModel);
                        return dst;
                    }
                }

                public static void EditorColumnDialogTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Implem.Pleasanter.Libraries.Settings.Column column,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    if (column == null) return;
                    var type = column.TypeName.CsTypeSummary();
                    dst.ColumnName = column.ColumnName;
                    dst.LabelText = column.LabelText;
                    dst.TextAlign = column.TextAlign switch
                    {
                        SiteSettings.TextAlignTypes.Left => Displays.LeftAlignment(context: context),
                        SiteSettings.TextAlignTypes.Right => Displays.RightAlignment(context: context),
                        SiteSettings.TextAlignTypes.Center => Displays.CenterAlignment(context: context),
                        _ => null
                    };
                    if (column.OtherColumn())
                    {
                        switch (column.ControlType)
                        {
                            case "ChoicesText":
                                dst.ChoicesText = column.ChoicesText;
                                dst.UseSearch = column.UseSearch;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (column.TypeName == "nvarchar" && column.ControlType != "Attachments")
                        {
                            dst.MaxLength = column.MaxLength > 0 ? column.MaxLength : null;
                        }
                        if (column.ColumnName != "Comments")
                        {
                            if (column.ControlType == "MarkDown")
                            {
                                dst.ViewerSwitchingType = column.ViewerSwitchingType switch
                                {
                                    Implem.Pleasanter.Libraries.Settings.Column.ViewerSwitchingTypes.Auto => Displays.Auto(context: context),
                                    Implem.Pleasanter.Libraries.Settings.Column.ViewerSwitchingTypes.Manual => Displays.Manual(context: context),
                                    Implem.Pleasanter.Libraries.Settings.Column.ViewerSwitchingTypes.Disabled => Displays.Disabled(context: context),
                                    _ => null
                                };
                            }
                            if (!column.Id_Ver && !column.NotUpdate)
                            {
                                dst.ValidateRequired = column.ValidateRequired;
                            }
                            if (!column.Id_Ver && !column.NotUpdate && column.ControlType != "Attachments")
                            {
                                dst.AllowBulkUpdate = column.AllowBulkUpdate;
                            }
                            switch (type)
                            {
                                case Types.CsNumeric:
                                case Types.CsDateTime:
                                case Types.CsString:
                                    if (!column.Id_Ver
                                        && !column.NotUpdate
                                        && column.ControlType != "Attachments"
                                        && column.ColumnName != "Comments")
                                    {
                                        dst.NoDuplication = column.NoDuplication == true;
                                        dst.MessageWhenDuplicated = column.MessageWhenDuplicated;
                                    }
                                    break;
                            }
                            if ((column.Required == false || column.TypeName == "datetime") && !column.NotUpdate)
                            {
                                dst.CopyByDefault = column.CopyByDefault == true;
                                dst.EditorReadOnly = column.EditorReadOnly == true;
                            }
                            if (column.TypeName == "datetime" && !column.NotUpdate)
                            {
                                dst.EditorFormat = column.EditorFormat switch
                                {
                                    "Ymd" => Displays.Ymd(context: context),
                                    "Ymdhm" => Displays.Ymdhm(context: context),
                                    "Ymdhms" => Displays.Ymdhms(context: context),
                                    _ => null
                                };
                            }
                            switch (type)
                            {
                                case Types.CsBool:
                                    if (!column.NotUpdate)
                                    {
                                        dst.DefaultInput = column.DefaultInput;
                                    }
                                    break;
                                case Types.CsNumeric:
                                    if (column.ColumnName == DataSources.Rds.IdColumn(tableName: ss.ReferenceType)
                                            || Implem.DefinitionAccessor.Def.ExtendedColumnTypes.Get(column.ColumnName) == "Num")
                                    {
                                        dst.ImportKey = column.ImportKey == true;
                                    }
                                    if (column.ControlType == "ChoicesText")
                                    {
                                        if (!column.Id_Ver)
                                        {
                                            dst.DefaultInput = column.DefaultInput;
                                        }
                                    }
                                    else
                                    {
                                        if (!column.Id_Ver && !column.NotUpdate)
                                        {
                                            if (!column.Id_Ver && !column.NotUpdate)
                                            {
                                                dst.DefaultInput = (!column.DefaultInput.IsNullOrEmpty()
                                                    ? column.DefaultInput.ToDecimal().ToString()
                                                    : string.Empty);
                                            }
                                        }
                                        if (!column.Id_Ver)
                                        {
                                            var s = (column.Format.IsNotEmpty()) switch
                                            {
                                                "" => "Standard",
                                                "C" => "Currency",
                                                "N" => "DigitGrouping",
                                                _ => "Custom"
                                            };
                                            dst.Format = Displays.Get(
                                                            context: context,
                                                            id: s);
                                            if (s == "Custom")
                                            {
                                                dst.Custom = column.Format;
                                            }
                                        }
                                        if (!column.Id_Ver && !column.NotUpdate)
                                        {
                                            dst.DefaultInput = !column.DefaultInput.IsNullOrEmpty()
                                                ? column.DefaultInput.ToDecimal().ToString()
                                                : string.Empty;
                                        }
                                        if (!column.Id_Ver && !column.NotUpdate
                                            && column.ColumnName != "WorkValue"
                                            && column.ColumnName != "ProgressRate")
                                        {
                                            dst.Nullable = column.Nullable;
                                        }
                                        if (!column.Id_Ver)
                                        {
                                            dst.Unit = column.Unit;
                                        }
                                        if (MaxDecimalPlaces(column) > 0 && !column.Id_Ver)
                                        {
                                            dst.DecimalPlaces = column.DecimalPlaces;
                                        }
                                        if (!column.Id_Ver)
                                        {
                                            dst.RoundingType = column.RoundingType switch
                                            {
                                                SiteSettings.RoundingTypes.AwayFromZero => Displays.AwayFromZero(context: context),
                                                SiteSettings.RoundingTypes.Ceiling => Displays.Ceiling(context: context),
                                                SiteSettings.RoundingTypes.Truncate => Displays.Truncate(context: context),
                                                SiteSettings.RoundingTypes.Floor => Displays.Floor(context: context),
                                                SiteSettings.RoundingTypes.ToEven => Displays.ToEven(context: context),
                                                _ => null
                                            };
                                        }
                                        if (!column.NotUpdate && !column.Id_Ver)
                                        {
                                            dst.ControlType = column.ControlType;
                                            dst.Min = column.Min;
                                            dst.Max = column.Max;
                                            dst.Step = column.Step;
                                        }
                                    }
                                    break;
                                case Types.CsDateTime:
                                    if (!column.NotUpdate)
                                    {
                                        dst.DefaultInput = column.DefaultInput != string.Empty
                                            ? column.DefaultInput.ToDecimal().ToString()
                                            : null;
                                    }
                                    dst.DateTimeStep = column.DateTimeStep;
                                    break;
                                case Types.CsString:
                                    switch (column.ControlType)
                                    {
                                        case "Attachments":
                                            dst.AllowDeleteAttachments = column.AllowDeleteAttachments == true;
                                            dst.NotDeleteExistHistory = column.NotDeleteExistHistory == true;
                                            dst.BinaryStorageProvider = column.BinaryStorageProvider switch
                                            {
                                                "DataBase" => Displays.Database(context: context),
                                                "LocalFolder" => Displays.LocalFolder(context: context),
                                                "AutoDataBaseOrLocalFolder" => Displays.AutoDataBaseOrLocalFolder(context: context),
                                                _ => null
                                            };
                                            dst.OverwriteSameFileName = column.OverwriteSameFileName == true;
                                            dst.LimitQuantity = column.LimitQuantity;
                                            dst.LimitSize = column.LimitSize;
                                            dst.TotalLimitSize = column.TotalLimitSize;
                                            dst.LocalFolderLimitSize = column.LocalFolderLimitSize;
                                            dst.LocalFolderTotalLimitSize = column.LocalFolderTotalLimitSize;
                                            break;

                                        default:
                                            if (column.ColumnName != "Comments" && !column.NotUpdate)
                                            {
                                                dst.ImportKey = column.ImportKey == true;
                                            }
                                            if (context.ContractSettings.Images()
                                                && (column.ControlType == "MarkDown"
                                                || column.ColumnName == "Comments"))
                                            {
                                                dst.AllowImage = column.AllowImage == true;
                                            }
                                            if (column.Max == -1)
                                            {
                                                dst.ThumbnailLimitSize = column.ThumbnailLimitSize == 0
                                                    ? null
                                                    : column.ThumbnailLimitSize;
                                            }
                                            if (column.ColumnName != "Comments" && !column.NotUpdate)
                                            {
                                                dst.DefaultInput = column.DefaultInput;
                                            }
                                            break;
                                    }
                                    break;
                            }
                            if (type == Types.CsString
                                        || type == Types.CsDateTime
                                        || Implem.DefinitionAccessor.Def.ExtendedColumnTypes.Get(column?.Name) == "Num")
                            {
                                dst.Description = column.Description;
                            }

                            switch (column.ControlType)
                            {
                                case "ChoicesText":
                                    dst.ChoicesText = column.ChoicesText;
                                    dst.ChoicesControlType = column.ChoicesControlType switch
                                    {
                                        "DropDown" => Displays.DropDownList(context: context),
                                        "Radio" => Displays.RadioButton(context: context),
                                        _ => null
                                    };
                                    dst.UseSearch = column.UseSearch == true;
                                    if (column.TypeName == "nvarchar")
                                    {
                                        dst.MultipleSelections = column.MultipleSelections == true;
                                    }
                                    dst.NotInsertBlankChoice = column.NotInsertBlankChoice == true;
                                    if (column.TypeName == "nvarchar"
                                                && !column.NotUpdate)
                                    {
                                        dst.Anchor = column.Anchor == true;
                                    }
                                    if (column.TypeName == "nvarchar"
                                                && !column.NotUpdate)
                                    {
                                        dst.OpenAnchorNewTab = column.OpenAnchorNewTab == true;
                                    }
                                    if (column.TypeName == "nvarchar"
                                                && !column.NotUpdate)
                                    {
                                        dst.AnchorFormat = column.AnchorFormat;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (column.ColumnName == "Title")
                            {
                                // hb.EditorColumnTitleProperties
                                dst.TitleColumns = ss.TitleColumns.Join(",");
                                dst.TitleSeparator = ss.TitleSeparator;
                            }
                            if (column.ColumnName != "Comments")
                            {
                                if (!column.Id_Ver
                                            && !column.NotUpdate)
                                {
                                    dst.AutoPostBack = column.AutoPostBack == true;
                                }
                                dst.ColumnsReturnedWhenAutomaticPostback = column.ColumnsReturnedWhenAutomaticPostback;
                                dst.NoWrap = column.NoWrap == true;
                                dst.Hide = column.Hide == true;
                                dst.ExtendedFieldCss = column.ExtendedFieldCss;
                                dst.ExtendedControlCss = column.ExtendedControlCss;
                            }
                            dst.FullTextType = ColumnUtilities.FullTextTypeOptions(context)
                                .Where(kv => kv.Key == column.FullTextType?.ToInt().ToString())
                                .Select(kv => kv.Value)
                                .FirstOrDefault();
                        }
                    }
                }

                private static int MaxDecimalPlaces(Implem.Pleasanter.Libraries.Settings.Column column)
                {
                    return column.Size.Split_2nd().ToInt();
                }

                public static void AutoNumberingSettingTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Implem.Pleasanter.Libraries.Settings.Column column,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    if (column == null) return;
                    if (!column.AutoNumberingColumn()) return;
                    dst.AutoNumberingFormat = ss.ColumnNameToLabelText(text: column.AutoNumberingFormat);
                    dst.AutoNumberingResetType = column.AutoNumberingResetType switch
                    {
                        Implem.Pleasanter.Libraries.Settings.Column.AutoNumberingResetTypes.Year => Displays.Year(context: context),
                        Implem.Pleasanter.Libraries.Settings.Column.AutoNumberingResetTypes.Month => Displays.Month(context: context),
                        Implem.Pleasanter.Libraries.Settings.Column.AutoNumberingResetTypes.Day => Displays.Day(context: context),
                        Implem.Pleasanter.Libraries.Settings.Column.AutoNumberingResetTypes.String => Displays.String(context: context),
                        _ => null
                    };
                    dst.AutoNumberingDefault = column.AutoNumberingDefault;
                    dst.AutoNumberingStep = column.AutoNumberingStep;
                }

                public static void ExtendedHtmlSettingTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Implem.Pleasanter.Libraries.Settings.Column column,
                    SiteModel siteModel)
                {
                    if (column == null) return;
                    if (column.OtherColumn() || column.ColumnName == "Comments") return;
                    dst.ExtendedHtmlBeforeField = column.ExtendedHtmlBeforeField.IsNotEmpty();
                    dst.ExtendedHtmlBeforeLabel = column.ExtendedHtmlBeforeLabel.IsNotEmpty();
                    dst.ExtendedHtmlBetweenLabelAndControl = column.ExtendedHtmlBetweenLabelAndControl.IsNotEmpty();
                    dst.ExtendedHtmlAfterControl = column.ExtendedHtmlAfterControl.IsNotEmpty();
                    dst.ExtendedHtmlAfterField = column.ExtendedHtmlAfterField.IsNotEmpty();
                }

                public static void EditorDetailsettingTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Implem.Pleasanter.Libraries.Settings.Column column,
                    SiteModel siteModel)
                {
                    if (column == null) return;
                    if (column.OtherColumn()) return;
                    dst.ClientRegexValidation = column.ClientRegexValidation;
                    dst.ServerRegexValidation = column.ServerRegexValidation;
                    dst.RegexValidationMessage = column.RegexValidationMessage;
                }

                public static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Settings.Column column,
                    SiteModel siteModel)
                {
                    var ssNew = new SiteSettings(
                        context: context,
                        referenceType: siteModel.ReferenceType);
                    var inColumnNew = ssNew.ColumnHash.Get(dst.ColumnName);
                    var outColumnNew = new ListColumn();
                    ListColumn.EditorColumnDialogTab(
                        dst: outColumnNew,
                        context: context,
                        column: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.AutoNumberingSettingTab(
                        dst: outColumnNew,
                        context: context,
                        column: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.ExtendedHtmlSettingTab(
                        dst: outColumnNew,
                        context: context,
                        column: inColumnNew,
                        siteModel: siteModel);
                    ListColumn.EditorDetailsettingTab(
                        dst: outColumnNew,
                        context: context,
                        column: inColumnNew,
                        siteModel: siteModel);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if ((v1 != null && v2 != null && !v1.Equals(v2))
                            || (v1 == null && v2 != null)
                            || (v1 != null && v2 == null))
                        {
                            dst.ChangedColumns.Add(n);
                        }
                    }
                }
            }

            public class OtherTable : List2TableBase<OtherColumn>
            {
                internal static OtherTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new OtherTable();
                    table.Label = Displays.OtherColumnsSettings(context: context);
                    table.Header = OtherColumn.CreateHeaderModel(context: context, param: param);
                    table.Columns = OtherColumn.CreateColumns(context: context, param: param, siteModel: siteModel);
                    return table;
                }
            }

            public class OtherColumn
            {
                public string ColumnName;
                public string LabelText;
                public string TextAlign;
                public string ChoicesText;
                public bool? UseSearch;
                public string FullTextType;
                public List<string> ChangedColumns;

                internal static List2TableHeader CreateHeaderModel(
                    Context context,
                    Param param)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new(){ Key="ColumnName",Text=Displays.ColumnName(context: context),ReadOnly=true},
                            new(){ Key="LabelText",Text=Displays.DisplayName(context: context)},
                            new(){ Key="TextAlign",Text=Displays.TextAlign(context: context)},
                            new(){ Key="ChoicesText",Text=Displays.OptionList(context: context)},
                            new(){ Key="UseSearch",Text=Displays.UseSearch(context: context)},
                            new(){ Key="FullTextType",Text=Displays.FullTextTypes(context: context)},
                        }
                    };
                }

                internal static List<OtherColumn> CreateColumns(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var list = new List<OtherColumn>();
                    var ss = siteModel.SiteSettings;
                    var columns = new string[] { "Creator", "Updator", "CreatedTime", "UpdatedTime" };
                    foreach (var columnName in columns)
                    {
                        var dst = new OtherColumn();
                        list.Add(dst);
                        SetData(dst: dst, columnName: columnName, context: context, siteModel: siteModel, ss: ss);
                        SetChangeColumnName(dst: dst, columnName: columnName, context: context, siteModel: siteModel, ss: ss);
                    }
                    return list;
                }

                private static void SetData(
                    OtherColumn dst,
                    Context context,
                    SiteModel siteModel,
                    SiteSettings ss,
                    string columnName)
                {
                    var column = ss.GetColumn(context: context, columnName: columnName);
                    dst.ColumnName = column.LabelTextDefault;
                    dst.LabelText = column.LabelText;
                    dst.TextAlign = column.TextAlign switch
                    {
                        SiteSettings.TextAlignTypes.Left => Displays.LeftAlignment(context: context),
                        SiteSettings.TextAlignTypes.Right => Displays.RightAlignment(context: context),
                        SiteSettings.TextAlignTypes.Center => Displays.CenterAlignment(context: context),
                        _ => string.Empty
                    };
                    if (column.ControlType == "ChoicesText")
                    {
                        dst.ChoicesText = column.ChoicesText;
                        dst.UseSearch = column.UseSearch == true;
                    }
                    dst.FullTextType = ColumnUtilities.FullTextTypeOptions(context)
                        .Where(kv => kv.Key == column.FullTextType?.ToInt().ToString())
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                }

                private static void SetChangeColumnName(
                    OtherColumn dst,
                    Context context,
                    SiteModel siteModel,
                    SiteSettings ss,
                    string columnName)
                {
                    var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                    var outColumnNew = new OtherColumn();
                    SetData(dst: outColumnNew, context: context, siteModel: siteModel, ss: ssNew, columnName: columnName);
                    var t = typeof(OtherColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if ((v1 != null && v2 != null && !v1.Equals(v2))
                            || (v1 == null && v2 != null)
                            || (v1 != null && v2 == null))
                        {
                            (dst.ChangedColumns ??= new()).Add(n);
                        }
                    }
                }

            }

            public class RelatingTable : List2TableBase<RelatingColumn>
            {
                internal static RelatingTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new RelatingTable();
                    table.Label = Displays.RelatingColumnSettings(context: context);
                    table.Header = RelatingColumn.CreateHeaderModel(context: context, param: param);
                    table.Columns = RelatingColumn.CreateColumns(context: context, param: param, siteModel: siteModel);
                    return table;
                }
            }

            public class RelatingColumn
            {
                public int Id;
                public string Title;
                public List<string> Columns;

                internal static List2TableHeader CreateHeaderModel(
                    Context context,
                    Param param)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new(){ Key="Id",Text=Displays.Id(context: context)},
                            new(){ Key="Title",Text=Displays.Title(context: context)},
                            new(){ Key="Columns",Text=Displays.ColumnList(context: context)},
                        }
                    };
                }

                internal static List<RelatingColumn> CreateColumns(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var list = new List<RelatingColumn>();
                    var ss = siteModel.SiteSettings;
                    foreach (var relating in ss.RelatingColumns)
                    {
                        var dst = new RelatingColumn();
                        list.Add(dst);
                        SetData(dst: dst, relating: relating, context: context, siteModel: siteModel, ss: ss);
                        SetChangeColumnName(dst: dst, relating: relating, context: context, siteModel: siteModel, ss: ss);
                    }
                    return list;
                }

                private static void SetData(
                    RelatingColumn dst,
                    Settings.RelatingColumn relating,
                    Context context,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    dst.Id = relating.Id;
                    dst.Title = relating.Title;
                    dst.Columns = relating.Columns
                        .Select(v => ss.GetColumn(context: context, columnName: v).LabelText)
                        .ToList();
                }

                private static void SetChangeColumnName(
                    RelatingColumn dst,
                    Settings.RelatingColumn relating,
                    Context context,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    // 差分抽出はしない
                }
            }
        }

        public class LinksSettingsModel : SettingsModelBase
        {
            internal static LinksSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new LinksSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Links(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateSetting(context: context, param: param, ss: ss)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateSetting(
                Context context,
                Param param,
                SiteSettings ss)
            {
                if (!(ss.Views?.Any() == true)) return null;
                return new KeyValueTableBase()
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new ()
                    {
                        new KeyValueTableBase.ColumnModel
                        {
                            Label = Displays.DefaultView(context: context),
                            Name = "LinkTableView",
                            Type = "string",
                            Value = ss.ViewSelectableOptions()
                                .Where(v => v.Key == ss.LinkTableView?.ToString())
                                .Select(v => v.Value.Text)
                                .FirstOrDefault(string.Empty),
                            Changed = ss.LinkTableView != null
                        }
                    }
                };
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = new()
                    {
                        Labels = new()
                        {
                            new(){Key="ColumnName",Text=Displays.ColumnName(context: context)},
                            new(){Key="LabelText",Text=Displays.DisplayName(context: context)}
                        }
                    };
                    table.Columns = new();
                    var ss = siteModel.SiteSettings;
                    foreach (var column in ss.GetLinkColumns(context: context))
                    {
                        var dst = new ListColumn();
                        table.Columns.Add(dst);
                        ListColumn.FilterColumnDialogTab(
                            dst: dst,
                            context: context,
                            column: column,
                            ss: ss);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public string ColumnName;
                public string LabelText;
                internal static void FilterColumnDialogTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Implem.Pleasanter.Libraries.Settings.Column column,
                    SiteSettings ss)
                {
                    dst.ColumnName = ss.ColumnHash[column.ColumnName].LabelTextDefault;
                    dst.LabelText = column.LabelText;
                }
            }
        }

        public class HistoriesSettingsModel : SettingsModelBase
        {
            internal static HistoriesSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new HistoriesSettingsModel();
                obj.ButtonLabel = Displays.Histories(context: context);
                obj.Tables = new ITableModel[]
                {
                    ColumnsTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateSetting(context: context, param: param, ss: siteModel.SiteSettings)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateSetting(
                Context context,
                Param param,
                SiteSettings ss)
            {
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase()
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new List<KeyValueTableBase.ColumnModel>()
                        {
                            new ()
                            {
                                Label = Displays.AllowRestoreHistories(context: context),
                                Name = "AllowRestoreHistories",
                                Type = "bool",
                                Value = (ss.AllowRestoreHistories == true).ToString().ToLower(),
                                Changed = ssNew.AllowRestoreHistories != ss.AllowRestoreHistories
                            },
                            new ()
                            {
                                Label = Displays.AllowPhysicalDeleteHistories(context: context),
                                Name = "AllowPhysicalDeleteHistories",
                                Type = "bool",
                                Value = (ss.AllowPhysicalDeleteHistories == true).ToString().ToLower(),
                                Changed = ssNew.AllowPhysicalDeleteHistories != ss.AllowPhysicalDeleteHistories
                            }
                        }
                };
            }

            public class ColumnsTable : List2TableBase<ColumnsColumn>
            {
                internal static ColumnsTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ColumnsTable();
                    table.Header = new()
                    {
                        Labels = new()
                        {
                            new(){Key="ColumnName",Text=Displays.ColumnName(context: context)},
                            new(){Key="LabelText",Text=Displays.DisplayName(context: context)}
                        }
                    };
                    table.Columns = new List<ColumnsColumn>();
                    var ss = siteModel.SiteSettings;
                    foreach (var column in ss.HistoryColumns.Select(v => ss.ColumnHash.Get(v)))
                    {
                        var dst = new ColumnsColumn();
                        table.Columns.Add(dst);
                        ColumnsColumn.FilterColumnDialogTab(
                            dst: dst,
                            context: context,
                            column: column,
                            ss: ss);
                    }
                    return table;
                }
            }

            public class ColumnsColumn
            {
                public string ColumnName;
                public string LabelText;
                internal  static void FilterColumnDialogTab(
                    ColumnsColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Implem.Pleasanter.Libraries.Settings.Column column,
                    SiteSettings ss)
                {
                    dst.ColumnName = ss.ColumnHash[column.ColumnName].LabelTextDefault;
                    dst.LabelText = column.LabelText;
                }
            }
        }

        public class MoveSettingsModel : SettingsModelBase
        {
            internal static MoveSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new MoveSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Move(context: context);
                obj.Tables = new ITableModel[]
                {
                    new ListTableBase(){
                        Header = new (){ Labels=new(){Displays.SettingValue(context: context) } },
                        Columns = SiteInfo.Sites(context: context)
                            .Where(o => ss.MoveTargets?.Contains(o.Key) == true)
                            .OrderBy(o => ss.MoveTargets.IndexOf(o.Key.ToLong()))
                            .Select(o => $"[{o.Key}] {o.Value.String("Title")}")
                            .ToList()
                    }
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }
        }

        public class SummariesSettingsModel : SettingsModelBase
        {
            internal static SummariesSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new SummariesSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Summaries(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var ss = siteModel.SiteSettings;
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    foreach (var summary in ss.Summaries)
                    {
                        var dst = new ListColumn();
                        ListColumn.SetData(dst: dst, context: context, summary: summary, ss: ss);
                        ListColumn.SetChangeColumnName(dst: dst, context: context, summary: summary, ss: ss);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string SiteId;
                public string DestinationColumn;
                public string DestinationCondition;
                public bool? SetZeroWhenOutOfCondition;
                public string LinkColumn;
                public string Type;
                public string SourceColumn;
                public string SourceCondition;
                public List<string> ChangedColumns = new ();
                public List<string> ReadonlyColumns = new ();

                internal static void SetData(
                    ListColumn dst,
                    Context context,
                    Summary summary,
                    SiteSettings ss)
                {
                    var destinationSiteHash = ss.Destinations
                        ?.Values
                        .ToDictionary(o => o.SiteId.ToString(), o => o.Title);
                    var destinationSs = ss.Destinations?.Get(summary.SiteId);
                    dst.Id = summary.Id;
                    dst.SiteId = destinationSiteHash
                        .Where(kv => kv.Key == summary.SiteId.ToString())
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.DestinationColumn = destinationSs?.Columns?
                        .Where(v => v.ColumnName == summary.DestinationColumn)
                        .Select(v => v.LabelText)
                        .FirstOrDefault(string.Empty);
                    dst.DestinationCondition = destinationSs?.Views?.Any() == true
                        ? destinationSs?.ViewSelectableOptions()
                            .Where(kv => kv.Key == summary.DestinationCondition.ToString())
                            .Select(kv => kv.Value.Text)
                            .FirstOrDefault(string.Empty)
                        : null;
                    dst.SetZeroWhenOutOfCondition = summary.DestinationCondition != null
                        ? summary.SetZeroWhenOutOfCondition == true
                        : null;
                    dst.LinkColumn = ss.Links?.Where(o => o.SiteId > 0)
                            .Where(o => o.SiteId == summary.SiteId)
                            .Select(o => ss.GetColumn(context: context, columnName: o.ColumnName).LabelText)
                            .FirstOrDefault(string.Empty);
                    dst.Type = summary.Type switch
                    {
                        "Count" => Displays.Count(context: context),
                        "Total" => Displays.Total(context: context),
                        "Average" => Displays.Average(context: context),
                        "Min" => Displays.Min(context: context),
                        "Max" => Displays.Max(context: context),
                        _ => string.Empty
                    };
                    dst.SourceColumn = summary.Type != "Count"
                        ? ss.Columns
                            .Where(o => o.ColumnName == summary.SourceColumn)
                            .Select(o => o.LabelText)
                            .FirstOrDefault(string.Empty)
                        : null;
                    dst.SourceCondition = ss?.Views?.Any() == true
                        ? ss?.ViewSelectableOptions()
                            .Where(kv => kv.Key == summary.SourceCondition.ToString())
                            .Select(kv => kv.Value.Text)
                            .FirstOrDefault(string.Empty)
                        : null;
                }

                internal static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Summary summary,
                    SiteSettings ss)
                {
                    // 差分抽出は不要
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Id",Text=Displays.Id(context: context),ReadOnly=true},
                            new (){Key="SiteId",Text=Displays.Sites(context: context)},
                            new (){Key="DestinationColumn",Text=Displays.Column(context:context)},
                            new (){Key="DestinationCondition",Text=Displays.Condition(context:context)},
                            new (){Key="SetZeroWhenOutOfCondition",Text=Displays.SetZeroWhenOutOfCondition(context:context)},
                            new (){Key="LinkColumn",Text=Displays.SummaryLinkColumn(context:context)},
                            new (){Key="Type",Text=Displays.SummaryType(context:context)},
                            new (){Key="SourceColumn",Text=Displays.SummarySourceColumn(context:context)},
                            new (){Key="SourceCondition",Text=Displays.Condition(context:context)},
                        }
                    };
                }
            }
        }

        public class FormulasSettingsModel : SettingsModelBase
        {
            internal static FormulasSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new FormulasSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Formulas(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateFormulasBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var ss = siteModel.SiteSettings;
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    foreach (var formula in ss.Formulas)
                    {
                        var dst = new ListColumn();
                        ListColumn.SetData(dst: dst, context: context, formula: formula, ss: ss);
                        ListColumn.SetChangeColumnName(dst: dst, context: context, formula: formula, ss: ss);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string CalculationMethod;
                public string Target;
                public string Formula;
                public bool? NotUseDisplayName;
                public bool? IsDisplayError;
                public string Condition;
                public string OutOfCondition;
                public List<string> ChangedColumns = new();
                public List<string> ReadonlyColumns = new();

                public static void SetData(
                    ListColumn dst,
                    Context context,
                    FormulaSet formula,
                    SiteSettings ss)
                {
                    dst.Id = formula.Id;
                    dst.CalculationMethod = ss.FormulaCalculationMethodSelectableOptions(context: context)
                        .Where(kv => kv.Key == formula.CalculationMethod)
                        .Select(kv => kv.Value.Text)
                        .FirstOrDefault(string.Empty);
                    dst.Target = ss.FormulaTargetSelectableOptions(formula.CalculationMethod)
                        .Where(kv => kv.Key == formula.Target?.ToString())
                        .Select(kv => kv.Value.Text)
                        .FirstOrDefault(string.Empty);
                    dst.Formula = (formula.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                        || string.IsNullOrEmpty(formula.CalculationMethod))
                            ? formula.Formula?.ToString(ss, notUseDisplayName: formula.NotUseDisplayName)
                            : FormulaBuilder.UpdateColumnDisplayText(
                                ss: ss,
                                formulaSet: formula).FormulaScript;
                    dst.NotUseDisplayName = formula.NotUseDisplayName == true;
                    dst.IsDisplayError = (formula.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                            || formula.CalculationMethod == null)
                            ? null
                            : formula.IsDisplayError == true;
                    dst.Condition = ss.Views?.Any() == true
                        ? ss.ViewSelectableOptions()
                            .Where(kv => kv.Key == formula.Condition?.ToString())
                            .Select(kv => kv.Value.Text)
                            .FirstOrDefault(string.Empty)
                        : null;
                    dst.OutOfCondition = ss.Views?.Any(o => o.Id == formula.Condition) == true
                        ? (formula.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                            || string.IsNullOrEmpty(formula.CalculationMethod))
                                ? formula.OutOfCondition?.ToString(ss)
                                : FormulaBuilder.UpdateColumnDisplayText(
                                    ss: ss,
                                    formulaSet: formula).FormulaScriptOutOfCondition
                        : null;
                }

                public static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    FormulaSet formula,
                    SiteSettings ss)
                {
                    // 差分抽出は不要
                    var isArray = new string[] { };
                    var inColumnNew = new FormulaSet();
                    var outColumnNew = new ListColumn();
                    ListColumn.SetData(
                        dst: outColumnNew,
                        context: context,
                        formula: inColumnNew,
                        ss: ss);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        if (n == "ReadonlyColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Id",Text=Displays.Id(context: context),ReadOnly=true},
                            new (){Key="CalculationMethod",Text=Displays.CalculationMethod(context: context)},
                            new (){Key="Target",Text=Displays.Target(context: context)},
                            new (){Key="Formula",Text=Displays.Formulas(context: context)},
                            new (){Key="NotUseDisplayName",Text=Displays.NotUseDisplayName(context: context)},
                            new (){Key="IsDisplayError",Text=Displays.FormulaIsDisplayError(context: context)},
                            new (){Key="Condition",Text=Displays.Condition(context: context)},
                            new (){Key="OutOfCondition",Text=Displays.OutOfCondition(context: context)},
                        }
                    };
                }
            }

            private static ITableModel CreateFormulasBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                var columns = new List<KeyValueTableBase.ColumnModel>
                {
                    new ()
                    {
                        Label = Displays.OutputLog(context: context),
                        Name = "OutputFormulaLogs",
                        Type = "bool",
                        Value = (ss.OutputFormulaLogs == true).ToString().ToLower(),
                        Changed = ss.OutputFormulaLogs != ssNew.OutputFormulaLogs
                    }
                }
                    .Where(v => v != null)
                    .ToList();
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = columns
                };
            }
        }

        public class ProcessesSettingsModel : SettingsModelBase
        {
            internal static ProcessesSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new ProcessesSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Processes(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateProcessesBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateProcessesBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                var columns = new List<KeyValueTableBase.ColumnModel>
                {
                    new ()
                    {
                        Label = Displays.OutputLog(context: context),
                        Name = "ProcessOutputFormulaLogs",
                        Type = "bool",
                        Value = (ss.ProcessOutputFormulaLogs == true).ToString().ToLower(),
                        Changed = ss.ProcessOutputFormulaLogs != ssNew.ProcessOutputFormulaLogs
                    }
                }
                    .Where(v => v != null)
                    .ToList();
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = columns
                };
            }

            public class ListTable : List3TableBase<ListColumn>
            {
                public ListTable()
                {
                    // Empty constructor for serialization
                }

                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    var ss = siteModel.SiteSettings;
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    table.Columns = new();
                    foreach (var process in ss.Processes)
                    {
                        var dst = new ListColumn();
                        table.Columns.Add(dst);
                        ListColumn.ProcessBasePanel(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.ProcessGeneralTab(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.ProcessValidateInputsTab(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.ProcessViewFiltersTab(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.ProcessAccessControlsTab(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.ProcessDataChangesTab(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.ProcessAutoNumberingTab(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.ProcessNotificationsTab(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                        ListColumn.SetChangeColumnName(
                            dst: dst,
                            context: context,
                            process: process,
                            siteModel: siteModel,
                            ss: ss);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string Name;
                public string DisplayName;
                public string ScreenType;
                public string CurrentStatus;
                public string ChangedStatus;
                public string Description;
                public string Tooltip;
                public string Icon;
                public string ConfirmationMessage;
                public string SuccessMessage;
                public string OnClick;
                public string ExecutionType;
                public string ActionType;
                public bool? AllowBulkProcessing;
                public string ValidationType;
                public List<ValidateInput> ValidateInputs;
                public bool? Incomplete;
                public bool? Own;
                public bool? NearCompletionTime;
                public bool? Delay;
                public bool? Overdue;
                public string Search;
                public List<ViewFiltersColumn.FilterConditionItem> ColumnFilterHash;
                public string ErrorMessage;
                public List<string> Permissions;
                public List<DataChange> DataChanges;
                public string AutoNumberingColumnName;
                public string AutoNumberingFormat;
                public string AutoNumberingResetType;
                public int? AutoNumberingDefault;
                public int? AutoNumberingStep;
                public List<Notification> Notifications;
                public List<string> ChangedColumns = new();

                public class ValidateInput
                {
                    public int Id;
                    public string ColumnName;
                    public bool? Required;
                    public string ClientRegexValidation;
                    public string ServerRegexValidation;
                    public string RegexValidationMessage;
                    public decimal? Min;
                    public decimal? Max;

                    internal static ValidateInput Create(
                        SiteSettings ss,
                        Settings.ValidateInput validateInput)
                    {
                        var dst = new ValidateInput();
                        dst.Id = validateInput.Id;
                        dst.ColumnName = DumpUtils.GetColumnLabelText(ss: ss, columnName: validateInput.ColumnName);
                        dst.Required = validateInput.Required == true;
                        var isString = Regex.IsMatch(validateInput.ColumnName, "^(Title|Body|Class.*|Description.*)$");
                        var isNum = validateInput.ColumnName.StartsWith("Num");
                        if (isString)
                        {
                            dst.ClientRegexValidation = validateInput.ClientRegexValidation.IsNotEmpty();
                            dst.ServerRegexValidation = validateInput.ServerRegexValidation.IsNotEmpty();
                            dst.RegexValidationMessage = validateInput.RegexValidationMessage.IsNotEmpty();
                        }
                        if (isNum)
                        {
                            dst.Min = validateInput.Min;
                            dst.Max = validateInput.Max;
                        }
                        return dst;
                    }

                    public static List<KeyValuePair<string, string>> GetLabels(Context context)
                    {
                        return new()
                            {
                                new ("Id",Displays.Id(context: context)),
                                new ("ColumnName",Displays.Column(context: context)),
                                new ("Required",Displays.Required(context: context)),
                                new ("ClientRegexValidation",Displays.ClientRegexValidation(context: context)),
                                new ("ServerRegexValidation",Displays.ServerRegexValidation(context: context)),
                                new ("RegexValidationMessage",Displays.RegexValidationMessage(context: context)),
                                new ("Min",Displays.Min(context: context)),
                                new ("Max",Displays.Max(context: context)),
                            };
                    }
                }

                public class DataChange
                {
                    public int Id;
                    public string Type;
                    public string ColumnName;
                    public string CopyFrom;
                    public string Value;
                    public string Formulas;
                    public bool? ValueFormulaNotUseDisplayName;
                    public bool? ValueFormulaIsDisplayError;
                    public string BaseDateTime;
                    public string ValueDateTime;
                    public string ValueDateTimePeriod;

                    internal static DataChange Create(
                        Context context,
                        SiteSettings ss,
                        Settings.DataChange dataChange)
                    {
                        var dst = new DataChange();
                        dst.Id = dataChange.Id;
                        dst.Type = DataChangeUtilities.Types(context: context)
                            .Where(kv => kv.Key == dataChange.Type.ToString())
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                        dst.ColumnName = DumpUtils.GetColumnLabelText(ss: ss, columnName: dataChange.ColumnName);
                        dst.CopyFrom = dataChange.Visible(type: "Column")
                            ? DumpUtils.GetColumnLabelText(ss: ss, columnName: dataChange.Value)
                            : null;
                        dst.Value = dataChange.Visible(type: "Value")
                            ? ss.ColumnNameToLabelText(text: dataChange.Value)
                            : null;
                        dst.Formulas = dataChange.Visible(type: "ValueFormula")
                            ? dataChange.Value
                            : null;
                        dst.ValueFormulaNotUseDisplayName = dataChange.Visible(type: "ValueFormula")
                            ? dataChange.ValueFormulaNotUseDisplayName == true
                            : null;
                        dst.ValueFormulaIsDisplayError = dataChange.Visible(type: "ValueFormula")
                            ? dataChange.ValueFormulaIsDisplayError == true
                            : null;
                        dst.BaseDateTime = dataChange.Visible(type: "DateTime")
                            ? dataChange.BaseDateTime switch
                            {
                                "CurrentDate" => Displays.CurrentDate(context: context),
                                "CurrentTime" => Displays.CurrentTime(context: context),
                                _ => Displays.CurrentTime(context: context)
                            }
                            : null;
                        dst.ValueDateTime = dataChange.Visible(type: "DateTime")
                            ? dataChange.Visible(type: "DateTime")
                                ? dataChange.DateTimeNumber().ToString()
                                : "0"
                            : null;
                        dst.ValueDateTimePeriod = dataChange.Visible(type: "DateTime")
                            ? dataChange.Visible(type: "DateTime")
                                ? dataChange.DateTimePeriod()
                                : string.Empty
                            : null;
                        return dst;
                    }

                    public static List<KeyValuePair<string, string>> GetLabels(Context context)
                    {
                        return new()
                            {
                                new ("Id",Displays.Id(context: context)),
                                new ("Type",Displays.ChangeTypes(context: context)),
                                new ("ColumnName",Displays.Column(context: context)),
                                new ("ValueColumnName",Displays.CopyFrom(context: context)),
                                new ("Value",Displays.Value(context: context)),
                                new ("ValueFormula",Displays.Formulas(context: context)),
                                new ("ValueFormulaNotUseDisplayName",Displays.NotUseDisplayName(context: context)),
                                new ("ValueFormulaIsDisplayError",Displays.FormulaIsDisplayError(context: context)),
                                new ("BaseDateTime",Displays.BaseDateTime(context: context)),
                                new ("ValueDateTime",Displays.Value(context: context)),
                                new ("ValueDateTimePeriod",Displays.Period(context: context)),
                            };
                    }
                }

                public class Notification
                {
                    public int Id;
                    public string Type;
                    public string Subject;
                    public string Address;
                    public string CcAddress;
                    public string BccAddress;
                    public string Token;
                    public string Body;

                    internal static Notification Create(
                        Context context,
                        SiteSettings ss,
                        Settings.Notification notification)
                    {
                        var dst = new Notification();
                        dst.Id = notification.Id;
                        dst.Type = DumpUtils.GetNotificationTypeText(context: context, type: notification.Type);
                        dst.Subject = ss.ColumnNameToLabelText(text: notification.Subject);
                        dst.Address = ss.ColumnNameToLabelText(text: notification.Address);
                        dst.CcAddress = notification.Type != Settings.Notification.Types.Mail
                            ? null
                            : ss.ColumnNameToLabelText(text: notification.CcAddress);
                        dst.BccAddress = notification.Type != Settings.Notification.Types.Mail
                            ? null
                            : ss.ColumnNameToLabelText(text: notification.BccAddress);
                        dst.Token = !NotificationUtilities.RequireToken(notification: notification)
                            ? null
                            : notification.Token;
                        dst.Body = ss.ColumnNameToLabelText(text: notification.Body);
                        return dst;
                    }

                    public static List<KeyValuePair<string, string>> GetLabels(Context context)
                    {
                        return new()
                            {
                                new ("Notifications.Id",Displays.Id(context: context)),
                                new ("Notifications.Type",Displays.NotificationType(context: context)),
                                new ("Notifications.Subject",Displays.Subject(context: context)),
                                new ("Notifications.Address",Displays.Address(context: context)),
                                new ("Notifications.CcAddress",Displays.Cc(context: context)),
                                new ("Notifications.BccAddress",Displays.Bcc(context: context)),
                                new ("Notifications.Token",Displays.Token(context: context)),
                                new ("Notifications.Body",Displays.Body(context: context)),
                            };
                    }
                }

                public static void ProcessBasePanel(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    dst.Id = process.Id;
                    dst.Name = process.Name;
                    dst.DisplayName = process.DisplayName;
                }

                public static void ProcessGeneralTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    dst.ScreenType = process.ScreenType switch
                    {
                        Settings.Process.ScreenTypes.New => Displays.New(context: context),
                        _ => Displays.Edit(context: context)
                    };
                    dst.CurrentStatus = DumpUtils.GetStatusText(context: context, ss: ss, statusId: process.CurrentStatus);
                    dst.ChangedStatus = DumpUtils.GetStatusText(context: context, ss: ss, statusId: process.ChangedStatus);
                    dst.Description = process.Description.IsNotEmpty();
                    dst.Tooltip = process.Tooltip.IsNotEmpty();
                    dst.Icon = process.Icon.IsNotEmpty();
                    dst.ConfirmationMessage = process.ConfirmationMessage.IsNotEmpty();
                    dst.SuccessMessage = ss.ColumnNameToLabelText(text: process.SuccessMessage).IsNotEmpty();
                    dst.OnClick = process.OnClick.IsNotEmpty();
                    dst.ExecutionType = process.ExecutionType switch
                    {
                        Settings.Process.ExecutionTypes.CreateOrUpdate => Displays.CreateOrUpdate(context: context),
                        Settings.Process.ExecutionTypes.AddedButtonOrCreateOrUpdate => Displays.AddedButtonOrCreateOrUpdate(context: context),
                        _ => Displays.AddedButton(context: context)
                    };
                    dst.ActionType = process.ActionType switch
                    {
                        Settings.Process.ActionTypes.PostBack => Displays.PostBack(context: context),
                        Settings.Process.ActionTypes.None => Displays.None(context: context),
                        _ => Displays.Save(context: context)
                    };
                    dst.AllowBulkProcessing = process.AllowBulkProcessing == true;
                }

                public static void ProcessValidateInputsTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    dst.ValidationType = process.ValidationType switch
                    {
                        Settings.Process.ValidationTypes.Replacement => Displays.Replacement(context: context),
                        Settings.Process.ValidationTypes.None => Displays.None(context: context),
                        _ => Displays.Merge(context: context)
                    };
                    dst.ValidateInputs = process.ValidateInputs?
                        .Select(v => ListColumn.ValidateInput.Create(ss: ss, validateInput: v))
                        .ToList();
                }

                public static void ProcessViewFiltersTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    var view = process.View ?? new View();
                    var viewColumn = ViewFiltersColumn.SetViewFilters(context: context, view: view, ss: ss);
                    dst.Incomplete = viewColumn.Incomplete;
                    dst.Own = viewColumn.Own;
                    dst.NearCompletionTime = viewColumn.NearCompletionTime;
                    dst.Delay = viewColumn.Delay;
                    dst.Overdue = viewColumn.Overdue;
                    dst.Search = viewColumn.Search;
                    dst.ColumnFilterHash = viewColumn.ColumnFilterHash;
                    dst.ErrorMessage = ss.ColumnNameToLabelText(text: process.ErrorMessage);
                }

                public static void ProcessAccessControlsTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    var currentPermissions = process.GetPermissions(ss: ss);
                    dst.Permissions = currentPermissions.ToDictionary(
                        o => o.Key(), o => o.ControlData(
                            context: context,
                            ss: ss,
                            withType: false))
                            .Select(kv => kv.Value.Text)
                            .ToList();
                }

                public static void ProcessDataChangesTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    dst.DataChanges = process.DataChanges?
                        .Select(v => ListColumn.DataChange.Create(context: context, ss: ss, dataChange: v))
                        .ToList();
                }

                public static void ProcessAutoNumberingTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    dst.AutoNumberingColumnName = ss.AutoNumberingColumnOptions(context: context)
                        .Where(kv => kv.Key == process.AutoNumbering?.ColumnName)
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.AutoNumberingFormat = ss.ColumnNameToLabelText(text: process.AutoNumbering?.Format);
                    dst.AutoNumberingResetType = process.AutoNumbering?.ResetType switch
                    {
                        Settings.Column.AutoNumberingResetTypes.Year => Displays.Year(context: context),
                        Settings.Column.AutoNumberingResetTypes.Month => Displays.Month(context: context),
                        Settings.Column.AutoNumberingResetTypes.Day => Displays.Day(context: context),
                        Settings.Column.AutoNumberingResetTypes.String => Displays.String(context: context),
                        _ => string.Empty
                    };
                    dst.AutoNumberingDefault = process.AutoNumbering?.Default;
                    dst.AutoNumberingStep = process.AutoNumbering?.Step;
                }

                public static void ProcessNotificationsTab(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    if (context.ContractSettings.Notice == false) return;
                    dst.Notifications = process.Notifications?
                        .Select(v => ListColumn.Notification.Create(context: context, ss: ss, notification: v))
                        .ToList();
                }

                public static void SetChangeColumnName(
                    ListColumn dst,
                    Implem.Pleasanter.Libraries.Requests.Context context,
                    Settings.Process process,
                    SiteModel siteModel,
                    SiteSettings ss)
                {
                    var ssNew = new SiteSettings(
                        context: context,
                        referenceType: siteModel.ReferenceType);
                    var inColumnNew = new Settings.Process();
                    var outColumnNew = new ListColumn();
                    ListColumn.ProcessBasePanel(
                        dst: outColumnNew,
                        context: context,
                        process: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.ProcessGeneralTab(
                        dst: outColumnNew,
                        context: context,
                        process: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.ProcessValidateInputsTab(
                        dst: outColumnNew,
                        context: context,
                        process: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.ProcessViewFiltersTab(
                        dst: outColumnNew,
                        context: context,
                        process: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.ProcessAccessControlsTab(
                        dst: outColumnNew,
                        context: context,
                        process: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.ProcessDataChangesTab(
                        dst: outColumnNew,
                        context: context,
                        process: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.ProcessAutoNumberingTab(
                        dst: outColumnNew,
                        context: context,
                        process: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    ListColumn.ProcessNotificationsTab(
                        dst: outColumnNew,
                        context: context,
                        process: inColumnNew,
                        siteModel: siteModel,
                        ss: ssNew);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if ((v1 != null && v2 != null && !v1.Equals(v2))
                            || (v1 == null && v2 != null)
                            || (v1 != null && v2 == null))
                        {
                            dst.ChangedColumns.Add(n);
                        }
                    }
                }

                internal static List3TableHeader CreateHeaderModel(Context context)
                {
                    var labels = new List<List3TableHeader.Tab>
                    {
                        new (
                            tabName: new ("LockHeaderRight", ""),
                            labels: new ()
                            {
                                new ("Id",Displays.Id(context: context)),
                            }),
                        new (
                            tabName: new ("", ""),
                            labels: new ()
                            {
                                new ("Name",Displays.Name(context: context)),
                                new ("DisplayName",Displays.DisplayName(context: context)),
                            }),
                        new (
                            tabName: new ("General", Displays.General(context: context)),
                            labels: new ()
                            {
                                new ("ScreenType",Displays.ScreenType(context: context)),
                                new ("CurrentStatus",Displays.CurrentStatus(context: context)),
                                new ("ChangedStatus",Displays.ChangedStatus(context: context)),
                                new ("Description",Displays.Description(context: context)),
                                new ("Tooltip",Displays.Tooltip(context: context)),
                                new ("Icon",Displays.Icon(context: context)),
                                new ("ConfirmationMessage",Displays.ConfirmationMessage(context: context)),
                                new ("SuccessMessage",Displays.SuccessMessage(context: context)),
                                new ("OnClick",Displays.OnClick(context: context)),
                                new ("ExecutionType",Displays.ExecutionTypes(context: context)),
                                new ("ActionType",Displays.ActionTypes(context: context)),
                                new ("AllowBulkProcessing",Displays.AllowBulkProcessing(context: context)),
                            }),
                        new (
                            tabName: new ("ValidateInputs", Displays.ValidateInput(context: context)),
                            labels: new ()
                            {
                                new ("ValidationType",Displays.InputValidationTypes(context: context)),
                                new ("ValidateInputs.Id",Displays.Id(context: context)),
                                new ("ValidateInputs.ColumnName",Displays.Column(context: context)),
                                new ("ValidateInputs.Required",Displays.Required(context: context)),
                                new ("ValidateInputs.ClientRegexValidation",Displays.ClientRegexValidation(context: context)),
                                new ("ValidateInputs.ServerRegexValidation",Displays.ServerRegexValidation(context: context)),
                                new ("ValidateInputs.RegexValidationMessage",Displays.RegexValidationMessage(context: context)),
                                new ("ValidateInputs.Min",Displays.Min(context: context)),
                                new ("ValidateInputs.Max",Displays.Max(context: context)),
                            }),
                        new (
                            tabName: new ("ViewFilters", Displays.Condition(context: context)),
                            labels: new ()
                            {
                                new ("Incomplete",Displays.Incomplete(context: context)),
                                new ("Own",Displays.Own(context: context)),
                                new ("NearCompletionTime",Displays.NearCompletionTime(context: context)),
                                new ("Delay",Displays.Delay(context: context)), 
                                new ("Overdue",Displays.Overdue(context: context)),
                                new ("Search",Displays.Search(context: context)),
                                new ("ColumnFilterHash",Displays.ColumnList(context: context)),
                                new ("ErrorMessage",Displays.ErrorMessage(context: context)),
                            }),
                        new (
                            tabName: new ("AccessControls", Displays.AccessControls(context: context)),
                            labels: new ()
                            {
                                new ("Permissions",Displays.Authority(context: context)),
                            }),
                        new (
                            tabName: new ("DataChanges", Displays.DataChanges(context: context)),
                            labels: new ()
                            {
                                new ("DataChanges.Id",Displays.Id(context: context)),
                                new ("DataChanges.Type",Displays.ChangeTypes(context: context)),
                                new ("DataChanges.ColumnName",Displays.Column(context: context)),
                                new ("DataChanges.CopyFrom",Displays.CopyFrom(context: context)),
                                new ("DataChanges.Value",Displays.Value(context: context)),
                                new ("DataChanges.Formulas",Displays.Formulas(context: context)),
                                new ("DataChanges.ValueFormulaNotUseDisplayName",Displays.NotUseDisplayName(context: context)),
                                new ("DataChanges.ValueFormulaIsDisplayError",Displays.FormulaIsDisplayError(context: context)),
                                new ("DataChanges.BaseDateTime",Displays.BaseDateTime(context: context)),
                                new ("DataChanges.DateTimeNumber",Displays.Value(context: context)),
                                new ("DataChanges.DateTimePeriod",Displays.Period(context: context)),
                            }),
                        new (
                            tabName: new ("AutoNumbering", Displays.AutoNumbering(context: context)),
                            labels: new ()
                            {
                                new ("AutoNumberingColumnName",Displays.Column(context: context)),
                                new ("AutoNumberingFormat",Displays.Format(context: context)),
                                new ("AutoNumberingResetType",Displays.ResetType(context: context)),
                                new ("AutoNumberingDefault",Displays.DefaultInput(context: context)),
                                new ("AutoNumberingStep",Displays.Step(context: context)),
                            }),
                        new (
                            tabName: new ("Notifications", Displays.Notifications(context: context)),
                            labels: new ()
                            {
                                new ("Notifications.Id",Displays.Id(context: context)),
                                new ("Notifications.Type",Displays.NotificationType(context: context)),
                                new ("Notifications.Subject",Displays.Subject(context: context)),
                                new ("Notifications.Address",Displays.Address(context: context)),
                                new ("Notifications.CcAddress",Displays.Cc(context: context)),
                                new ("Notifications.BccAddress",Displays.Bcc(context: context)),
                                new ("Notifications.Token",Displays.Token(context: context)),
                                new ("Notifications.Body",Displays.Body(context: context)),
                            }),
                    };
                    return new List3TableHeader(labels);
                }
            }
        }

        public class StatusControlsSettingsModel : SettingsModelBase
        {
            internal static StatusControlsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new StatusControlsSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.StatusControls(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateStatusControlsList(context: context, param: param, siteModel: siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateStatusControlsList(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                return new StatusControlsTable(context: context, param: param, siteModel: siteModel);
            }

            public class StatusControlsTable : List3TableBase<StatusControlsTable.ColumnModel>
            {
                public StatusControlsTable()
                {
                    // Empty constructor for serialization
                }

                public StatusControlsTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var ss = siteModel.SiteSettings;
                    Header = CreateHeaderModel(context: context);
                    Columns = new();
                    foreach (var statusControl in ss.StatusControls)
                    {
                        var dst = new ColumnModel();
                        Columns.Add(dst);
                        ColumnModel.StatusControlBasePanel(
                            dst: dst,
                            context: context,
                            statusControl: statusControl,
                            siteModel: siteModel,
                            ss: ss);
                        ColumnModel.StatusControlGeneralTab(
                            dst: dst,
                            context: context,
                            statusControl: statusControl,
                            siteModel: siteModel,
                            ss: ss);
                        ColumnModel.StatusControlViewFiltersTab(
                            dst: dst,
                            context: context,
                            statusControl: statusControl,
                            siteModel: siteModel,
                            ss: ss);
                        ColumnModel.StatusControlAccessControlsTab(
                            dst: dst,
                            context: context,
                            statusControl: statusControl,
                            siteModel: siteModel,
                            ss: ss);
                        ColumnModel.SetChangeColumnName(
                            dst: dst,
                            context: context,
                            statusControl: statusControl,
                            siteModel: siteModel);
                    }
                }

                private List3TableHeader CreateHeaderModel(Context context)
                {
                    var labels = new List<List3TableHeader.Tab>
                    {
                        new (
                            tabName: new ("LockHeaderRight", ""),
                            labels: new ()
                            {
                                new ("Id",Displays.Id(context: context)),
                            }),
                        new (
                            tabName: new ("", ""),
                            labels: new ()
                            {
                                new ("Name",Displays.Name(context: context)),
                                new ("Status",Displays.Status(context: context)),
                                new ("Description",Displays.Description(context: context)),
                            }),
                        new (
                            tabName: new ("GeneralTab", Displays.General(context: context)),
                            labels: new ()
                            {
                                new ("ReadOnly",Displays.ReadOnly(context: context)),
                                new ("ColumnHash",Displays.ColumnList(context: context)),
                            }),
                        new (
                            tabName: new ("ViewFiltersTab", Displays.Condition(context: context)),
                            labels: new ()
                            {
                                new ("Incomplete", Displays.Incomplete(context: context)),
                                new ("Own", Displays.Own(context: context)),
                                new ("NearCompletionTime", Displays.NearCompletionTime(context: context)),
                                new ("Delay", Displays.Delay(context: context)),
                                new ("Overdue", Displays.Overdue(context: context)),
                                new ("Search", Displays.Search(context: context)),
                                new ("ColumnFilterHash", Displays.ColumnList(context: context)),
                            }),
                        new (
                            tabName: new("AccessControlsTab", Displays.AccessControls(context: context)),
                            labels: new ()
                            {
                                new ("Permissions", "Permissions"/**/),
                            })
                    };
                    return new List3TableHeader(labels: labels);
                }

                public class ColumnModel
                {
                    public int Id;
                    public string Name;
                    public string Status;
                    public string Description;
                    public bool? ReadOnly;
                    public List<string> ColumnHash;
                    public bool? Incomplete;
                    public bool? Own;
                    public bool? NearCompletionTime;
                    public bool? Delay;
                    public bool? Overdue;
                    public string Search;
                    public List<ViewFiltersColumn.FilterConditionItem> ColumnFilterHash;
                    public List<string> Permissions;
                    public List<string> ChangedColumns = new();

                    public static void StatusControlBasePanel(
                        ColumnModel dst,
                        Implem.Pleasanter.Libraries.Requests.Context context,
                        StatusControl statusControl,
                        SiteModel siteModel,
                        SiteSettings ss)
                    {
                        var status = ss.GetColumn(
                            context: context,
                            columnName: "Status");
                        var optionCollection = $"-1,*\n{status.ChoicesText}".SplitReturn()
                            .Select(o => new Choice(o))
                            .GroupBy(o => o.Value)
                            .ToDictionary(
                                o => o.Key,
                                o => new ControlData(
                                    text: o.First().Text,
                                    css: o.First().CssClass));
                        dst.Id = statusControl.Id;
                        dst.Name = statusControl.Name;
                        dst.Status = optionCollection
                            .Where(kv => kv.Key == statusControl.Status.ToString())
                            .Select(kv => kv.Value.Text)
                            .FirstOrDefault(string.Empty);
                        dst.Description = statusControl.Description;
                    }

                    public static void StatusControlGeneralTab(
                        ColumnModel dst,
                        Implem.Pleasanter.Libraries.Requests.Context context,
                        StatusControl statusControl,
                        SiteModel siteModel,
                        SiteSettings ss)
                    {
                        dst.ReadOnly = statusControl.ReadOnly == true;
                        dst.ColumnHash = statusControl.ColumnHash?
                            .Select(kv => new { c = ss.ColumnHash.Get(kv.Key), t = kv.Value })
                            .OrderBy(o => o.c.EditorColumn)
                            .Select(o => $"{o.c.LabelText}({Displays.Get(context: context, o.t.ToString())})")
                            .ToList();
                    }

                    public static void StatusControlViewFiltersTab(
                        ColumnModel dst,
                        Implem.Pleasanter.Libraries.Requests.Context context,
                        StatusControl statusControl,
                        SiteModel siteModel,
                        SiteSettings ss)
                    {
                        var view = statusControl.View ?? new View();
                        var viewColumn = ViewFiltersColumn.SetViewFilters(context: context, view: view, ss: ss);
                        dst.Incomplete = viewColumn.Incomplete;
                        dst.Own = viewColumn.Own;
                        dst.NearCompletionTime = viewColumn.NearCompletionTime;
                        dst.Delay = viewColumn.Delay;
                        dst.Overdue = viewColumn.Overdue;
                        dst.Search = viewColumn.Search;
                        dst.ColumnFilterHash = viewColumn.ColumnFilterHash;
                    }

                    public static void StatusControlAccessControlsTab(
                        ColumnModel dst,
                        Implem.Pleasanter.Libraries.Requests.Context context,
                        StatusControl statusControl,
                        SiteModel siteModel,
                        SiteSettings ss)
                    {
                        var currentPermissions = statusControl.GetPermissions(ss: ss);
                        dst.Permissions = currentPermissions.ToDictionary(
                            o => o.Key(), o => o.ControlData(
                                context: context,
                                ss: ss,
                                withType: false))
                            .Select(kv => kv.Value.Text)
                            .ToList();
                    }

                    public static void SetChangeColumnName(
                        ColumnModel dst,
                        Context context,
                        StatusControl statusControl,
                        SiteModel siteModel)
                    {
                        var ssNew = new SiteSettings(
                            context: context,
                            referenceType: siteModel.ReferenceType);
                        var inColumnNew = new StatusControl();
                        var outColumnNew = new ColumnModel();
                        ColumnModel.StatusControlBasePanel(
                            dst: outColumnNew,
                            context: context,
                            statusControl: inColumnNew,
                            siteModel: siteModel,
                            ss: ssNew);
                        ColumnModel.StatusControlGeneralTab(
                            dst: outColumnNew,
                            context: context,
                            statusControl: inColumnNew,
                            siteModel: siteModel,
                            ss: ssNew);
                        ColumnModel.StatusControlViewFiltersTab(
                            dst: outColumnNew,
                            context: context,
                            statusControl: inColumnNew,
                            siteModel: siteModel,
                            ss: ssNew);
                        ColumnModel.StatusControlAccessControlsTab(
                            dst: outColumnNew,
                            context: context,
                            statusControl: inColumnNew,
                            siteModel: siteModel,
                            ss: ssNew);
                        var t = typeof(ColumnModel);
                        foreach (var f in t.GetFields())
                        {
                            var n = f.Name;
                            if (n == "ChangedColumns") continue;
                            var v1 = f.GetValue(outColumnNew);
                            var v2 = f.GetValue(dst);
                            if ((v1 != null && v2 != null && !v1.Equals(v2))
                                || (v1 == null && v2 != null)
                                || (v1 != null && v2 == null))
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }
            }
        }

        public class ViewsSettingsModel : SettingsModelBase
        {
            internal static ViewsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new ViewsSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.DataView(context: context);
                obj.Tables = new ITableModel[]
                {
                    ViewFilterTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateBaseTable(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.SaveViewType(context: context),
                            Name = "SaveViewType",
                            Type = "string",
                            Value = new Dictionary<string, string>()
                            {
                                {
                                    SiteSettings.SaveViewTypes.Session.ToInt().ToString(),
                                    Displays.SaveViewSession(context: context)
                                },
                                {
                                    SiteSettings.SaveViewTypes.User.ToInt().ToString(),
                                    Displays.SaveViewUser(context: context)
                                },
                                {
                                    SiteSettings.SaveViewTypes.None.ToInt().ToString(),
                                    Displays.SaveViewNone(context: context)
                                },
                            }
                                .Where((kv,idx)=>(ss.SaveViewType == 0 && idx == 0) || (kv.Key == ss.SaveViewType.ToInt().ToString()))
                                .Select(kv => kv.Value)
                                .FirstOrDefault(string.Empty),
                            Changed = ss.SaveViewType != ssNew.SaveViewType
                        },
                    }
                };
            }

            public class ViewFilterTable : List3TableBase<ViewFilterColumn>
            {
                internal static ViewFilterTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var ss = siteModel.SiteSettings;
                    var hasCalendar = Def.ViewModeDefinitionCollection
                        .Any(o => o.Name == "Calendar" && o.ReferenceType == ss.ReferenceType);
                    var hasCrosstab = Def.ViewModeDefinitionCollection
                        .Any(o => o.Name == "Crosstab" && o.ReferenceType == ss.ReferenceType);
                    var hasGantt = Def.ViewModeDefinitionCollection
                        .Any(o => o.Name == "Gantt" && o.ReferenceType == ss.ReferenceType);
                    var hasTimeSeries = Def.ViewModeDefinitionCollection
                        .Any(o => o.Name == "TimeSeries" && o.ReferenceType == ss.ReferenceType);
                    var hasAnaly = Def.ViewModeDefinitionCollection
                        .Any(o => o.Name == "Analy" && o.ReferenceType == ss.ReferenceType);
                    var hasKamban = Def.ViewModeDefinitionCollection
                        .Any(o => o.Name == "Kamban" && o.ReferenceType == ss.ReferenceType);
                    var header = ViewFilterColumn.CreateHeaderModel(context: context, siteModel: siteModel, hasCalendar: hasCalendar, hasCrosstab: hasCrosstab, hasGantt: hasGantt, hasTimeSeries: hasTimeSeries, hasAnaly: hasAnaly, hasKamban: hasKamban);
                    var columns = CreateColumns(context: context, siteModel: siteModel, hasCalendar: hasCalendar, hasCrosstab: hasCrosstab, hasGantt: hasGantt, hasTimeSeries: hasTimeSeries, hasAnaly: hasAnaly, hasKamban: hasKamban);
                    return new()
                    {
                        Header = header,
                        Columns = columns
                    };
                }

                private static List<ViewFilterColumn> CreateColumns(
                    Context context,
                    SiteModel siteModel,
                    bool hasCalendar,
                    bool hasCrosstab,
                    bool hasGantt,
                    bool hasTimeSeries,
                    bool hasAnaly,
                    bool hasKamban)
                {
                    var columns = new List<ViewFilterColumn>();
                    var ss = siteModel.SiteSettings;
                    if (ss.Views == null) return columns;
                    foreach (var view in ss.Views)
                    {
                        var dst = new ViewFilterColumn();
                        columns.Add(dst);
                        dst.Id = view.Id;
                        dst.Name = view.Name;
                        ViewFilterColumn.General(
                            dst: dst,
                            context: context,
                            column: view,
                            ss: ss);
                        ViewFilterColumn.ViewGridTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss);
                        ViewFilterColumn.ViewFiltersTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss);
                        ViewFilterColumn.ViewSortersTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss);
                        ViewFilterColumn.ViewEditorTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss);
                        ViewFilterColumn.ViewCalendarTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss,
                            hasCalendar: hasCalendar);
                        ViewFilterColumn.ViewCrosstabTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss,
                            hasCrosstab: hasCrosstab);
                        ViewFilterColumn.ViewGanttTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss,
                            hasGantt: hasGantt);
                        ViewFilterColumn.ViewTimeSeriesTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss,
                            hasTimeSeries: hasTimeSeries);
                        ViewFilterColumn.ViewAnalyTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss,
                            hasAnaly: hasAnaly);
                        ViewFilterColumn.ViewKambanTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss,
                            hasKamban: hasKamban);
                        ViewFilterColumn.ViewAccessControlTab(
                            dst: dst,
                            context: context,
                            view: view,
                            ss: ss);
                        ViewFilterColumn.SetChangeColumnName(
                            dst: dst,
                            context: context,
                            column: view,
                            ss: ss,
                            hasCalendar: hasCalendar, hasCrosstab: hasCrosstab, hasGantt: hasGantt, hasTimeSeries: hasTimeSeries, hasAnaly: hasAnaly, hasKamban: hasKamban);
                    }
                    return columns;
                }
            }

            public class ViewFilterColumn
            {
                public int Id;
                public string Name;
                public string DefaultMode;
                public List<ViewFiltersColumn.GridColumnItem> GridColumns;
                public string FiltersDisplayType;
                public string AggregationsDisplayType;
                public string BulkMoveTargetsCommand;
                public string BulkDeleteCommand;
                public string EditImportSettings;
                public string OpenExportSelectorDialogCommand;
                public string OpenBulkUpdateSelectorDialogCommand;
                public string EditOnGridCommand;
                public List<ViewFiltersColumn.FilterColumnItem> FilterColumns;
                public bool? KeepFilterState;
                public bool? Incomplete;
                public bool? Own;
                public bool? NearCompletionTime;
                public bool? Delay;
                public bool? Overdue;
                public string Search;
                public List<ViewFiltersColumn.FilterConditionItem> ColumnFilterHash;
                public bool? ShowHistory;
                public bool? KeepSorterState;
                public List<ViewFiltersColumn.SorterColumnItem> ColumnSorterHash;
                public string UpdateCommand;
                public string OpenCopyDialogCommand;
                public string ReferenceCopyCommand;
                public string MoveTargetsCommand;
                public string EditOutgoingMail;
                public string DeleteCommand;
                public string CalendarGroupBy;
                public string CrosstabGroupByX;
                public string CrosstabGroupByY;
                public List<ViewFiltersColumn.FilterColumnItem> CrosstabColumns;
                public string CrosstabAggregateType;
                public string CrosstabValue;
                public string CrosstabTimePeriod;
                public bool? CrosstabNotShowZeroRows;
                public string ExportCrosstabCommand;
                public string CalendarTimePeriod;
                public string CalendarViewType;
                public string CalendarFromTo;
                public bool? CalendarShowStatus;
                public string GanttGroupBy;
                public string GanttSortBy;
                public string TimeSeriesGroupBy;
                public string TimeSeriesAggregateType;
                public string TimeSeriesValue;
                public string KambanGroupByX;
                public string KambanGroupByY;
                public string KambanAggregateType;
                public string KambanValue;
                public int? KambanColumns;
                public bool? KambanAggregationView;
                public bool? KambanShowStatus;
                public List<string> Permissions;
                public List<string> ReadOnlyColumns = new();
                public List<string> ChangedColumns = new();

                internal static List3TableHeader CreateHeaderModel(
                    Context context,
                    SiteModel siteModel,
                    bool hasCalendar,
                    bool hasCrosstab,
                    bool hasGantt,
                    bool hasTimeSeries,
                    bool hasAnaly,
                    bool hasKamban)
                {
                    var labels = new List<List3TableHeader.Tab>
                    {
                        new (
                            tabName: new ("LockHeaderRight", ""),
                            labels: new ()
                            {
                                new ("Id",Displays.Id(context: context)),
                                new ("Name", Displays.Name(context: context)),
                                new ("DefaultMode", Displays.DefaultViewMode(context: context)),
                            }),
                        new (
                            tabName: new ("ViewGridTab", Displays.Grid(context: context)),
                            labels: new ()
                            {
                                new ("GridColumns", Displays.ColumnList(context: context)),
                                new ("FiltersDisplayType", Displays.FiltersDisplayType(context: context)),
                                new ("AggregationsDisplayType", Displays.AggregationsDisplayType(context: context)),
                                new ("BulkMoveTargetsCommand", Displays.BulkMove(context: context)),
                                new ("BulkDeleteCommand", Displays.BulkDelete(context: context)),
                                new ("EditImportSettings", Displays.Import(context: context)),
                                new ("OpenExportSelectorDialogCommand", Displays.Export(context: context)),
                                new ("OpenBulkUpdateSelectorDialogCommand", Displays.BulkUpdate(context: context)),
                                new ("EditOnGridCommand", Displays.EditMode(context: context)),
                            }),
                        new (
                            tabName: new ("ViewFiltersTab", Displays.Filters(context: context)),
                            labels: new ()
                            {
                                new ("FilterColumns", Displays.ColumnList(context: context)),
                                new ("KeepFilterState", Displays.KeepFilterState(context: context)),
                                new ("Incomplete", Displays.Incomplete(context: context)),
                                new ("Own", Displays.Own(context: context)),
                                new ("NearCompletionTime", Displays.NearCompletionTime(context: context)),
                                new ("Delay", Displays.Delay(context: context)),
                                new ("Overdue", Displays.Overdue(context: context)),
                                new ("Search", Displays.Search(context: context)),
                                new ("ColumnFilterHash", Displays.ColumnList(context: context)),
                                new ("ShowHistory", Displays.ShowHistory(context: context)),
                            }),
                        new (
                            tabName: new ("ViewSortersTab", Displays.Sorters(context: context)),
                            labels: new ()
                            {
                                new ("KeepSorterState", Displays.KeepSorterState(context: context)),
                                new ("ColumnSorterHash", Displays.ColumnList(context: context)),
                            }),
                        new (
                            tabName: new ("ViewEditorTab", Displays.Editor(context: context)),
                            labels: new ()
                            {
                                new ("UpdateCommand", Displays.Update(context: context)),
                                new ("OpenCopyDialogCommand", Displays.Copy(context: context)),
                                new ("ReferenceCopyCommand", Displays.ReferenceCopy(context: context)),
                                new ("MoveTargetsCommand", Displays.Move(context: context)),
                                new ("EditOutgoingMail", Displays.Mail(context: context)),
                                new ("DeleteCommand", Displays.Delete(context: context)),
                            }),
                        !hasCalendar? null :new (
                            tabName: new ("ViewCalendarTab", Displays.Calendar(context: context)),
                            labels: new ()
                            {
                                new ("CalendarGroupBy", Displays.GroupBy(context: context)),
                                new ("CalendarTimePeriod", Displays.Period(context: context)),
                                new ("CalendarViewType", Displays.Period(context: context)),
                                new ("CalendarFromTo", Displays.Column(context: context)),
                                new ("CalendarShowStatus", Displays.ShowStatus(context: context)),
                            }),
                        !hasCrosstab? null :new (
                            tabName: new ("ViewCrosstabTab", Displays.Crosstab(context: context)),
                            labels: new ()
                            {
                                new ("CrosstabGroupByX", Displays.GroupByX(context: context)),
                                new ("CrosstabGroupByY", Displays.GroupByY(context: context)),
                                new ("CrosstabColumns", Displays.ColumnList(context: context)),
                                new ("CrosstabAggregateType", Displays.AggregationType(context: context)),
                                new ("CrosstabValue", Displays.AggregationTarget(context: context)),
                                new ("CrosstabTimePeriod", Displays.Period(context: context)),
                                new ("CrosstabNotShowZeroRows", Displays.NotShowZeroRows(context: context)),
                                new ("ExportCrosstabCommand", Displays.Export(context: context)),
                            }),
                        !hasGantt? null :new (
                            tabName: new ("ViewGanttTab", Displays.Gantt(context: context)),
                            labels: new ()
                            {
                                new ("GanttGroupBy", Displays.GroupBy(context: context)),
                                new ("GanttSortBy", Displays.SortBy(context: context)),
                            }),
                        !hasTimeSeries? null :new (
                            tabName: new ("ViewTimeSeriesTab", Displays.TimeSeries(context: context)),
                            labels: new ()
                            {
                                new ("TimeSeriesGroupBy", Displays.GroupBy(context: context)),
                                new ("TimeSeriesAggregateType", Displays.AggregationType(context: context)),
                                new ("TimeSeriesValue", Displays.AggregationTarget(context: context)),
                            }),
                        !hasAnaly ? null : new (
                            tabName: new("ViewAnalyTab", Displays.Analy(context: context)),
                            labels: new()
                            {
                                // 設定値がない
                                new (null,"")
                            }),
                        !hasKamban? null : new (
                            tabName: new ("ViewKambanTab", Displays.Kamban(context: context)),
                            labels: new ()
                            {
                                new ("KambanGroupByX", Displays.GroupByX(context: context)),
                                new ("KambanGroupByY", Displays.GroupByY(context: context)),
                                new ("KambanAggregateType", Displays.AggregationType(context: context)),
                                new ("KambanValue", Displays.AggregationTarget(context: context)),
                                new ("KambanColumns", Displays.MaxColumns(context: context)),
                                new ("KambanAggregationView", Displays.AggregationView(context: context)),
                                new ("KambanShowStatus", Displays.ShowStatus(context: context)),
                            }),
                        new (
                            tabName: new ("ViewAccessControlTab", Displays.AccessControls(context: context)),
                            labels: new ()
                            {
                                new ("Permissions", Displays.Permissions(context: context)),
                            })
                    }
                        .Where(v => v != null).ToList();
                    return new List3TableHeader(labels: labels);
                }

                internal static void General(
                    ViewFilterColumn dst,
                    Context context,
                    View column,
                    SiteSettings ss)
                {
                    dst.Id = column.Id;
                    dst.Name = column.Name;
                    dst.DefaultMode = SiteUtilities.GetViewTypeOptionCollection(context: context, ss: ss)
                        .Where(kv => kv.Key == column.DefaultMode)
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                }

                internal static void ViewGridTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    string prefix = "")
                {
                    var displayTypeOptionCollection = SiteUtilities.GetDisplayTypeOptionCollection(context);
                    var commandDisplayTypeOptionCollection = SiteUtilities.GetCommandDisplayTypeOptionCollection(context);
                    dst.GridColumns = (view.GridColumns ?? ss.GridColumns)?
                        .Select(v => new ViewFiltersColumn.GridColumnItem()
                        {
                            Label = ss.GetColumn(context: context, columnName: v)?.LabelText ?? $"? {v}",
                            DbColumnName = v
                        })
                        .ToList() ?? new();
                    if (prefix.IsNullOrEmpty())
                    {
                        dst.FiltersDisplayType = displayTypeOptionCollection
                            .Where((kv, idx) => (view.FiltersDisplayType == null && idx == 0) || (kv.Key == (view.FiltersDisplayType?.ToInt().ToString())))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                        dst.AggregationsDisplayType = displayTypeOptionCollection
                            .Where((kv, idx) => (view.AggregationsDisplayType == null && idx == 0) || (kv.Key == view.AggregationsDisplayType?.ToInt().ToString()))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                    }
                    if (prefix.IsNullOrEmpty())
                    {
                        dst.BulkMoveTargetsCommand = commandDisplayTypeOptionCollection
                            .Where((kv, idx) => (view.BulkMoveTargetsCommand == null && idx == 0) || (kv.Key == (view.BulkMoveTargetsCommand?.ToInt().ToString())))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                        dst.BulkDeleteCommand = commandDisplayTypeOptionCollection
                            .Where((kv, idx) => (view.BulkDeleteCommand == null && idx == 0) || (kv.Key == (view.BulkDeleteCommand?.ToInt().ToString())))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                        dst.EditImportSettings = commandDisplayTypeOptionCollection
                            .Where((kv, idx) => (view.EditImportSettings == null && idx == 0) || (kv.Key == (view.EditImportSettings?.ToInt().ToString())))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                        dst.OpenExportSelectorDialogCommand = commandDisplayTypeOptionCollection
                            .Where((kv, idx) => (view.OpenExportSelectorDialogCommand == null && idx == 0) || (kv.Key == (view.OpenExportSelectorDialogCommand?.ToInt().ToString())))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                        dst.OpenBulkUpdateSelectorDialogCommand = commandDisplayTypeOptionCollection
                            .Where((kv, idx) => (view.OpenBulkUpdateSelectorDialogCommand == null && idx == 0) || (kv.Key == (view.OpenBulkUpdateSelectorDialogCommand?.ToInt().ToString())))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                        dst.EditOnGridCommand = commandDisplayTypeOptionCollection
                            .Where((kv, idx) => (view.EditOnGridCommand == null && idx == 0) || (kv.Key == (view.EditOnGridCommand?.ToInt().ToString())))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                    }
                }

                public static void ViewFiltersTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    string prefix = "")
                {
                    var viewColumn = ViewFiltersColumn.SetViewFilters(context: context, view: view, ss: ss, prefix: prefix);
                    dst.FilterColumns = viewColumn.FilterColumns;
                    dst.KeepFilterState = viewColumn.KeepFilterState;
                    dst.Incomplete = viewColumn.Incomplete;
                    dst.Own = viewColumn.Own;
                    dst.NearCompletionTime = viewColumn.NearCompletionTime;
                    dst.Delay = viewColumn.Delay;
                    dst.Overdue = viewColumn.Overdue;
                    dst.Search = viewColumn.Search;
                    dst.ColumnFilterHash = viewColumn.ColumnFilterHash;
                    if (ss.HistoryOnGrid == true)
                    {
                        dst.ShowHistory = view.ShowHistory == true;
                    }
                }

                public static void ViewSortersTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    string prefix = "")
                {
                    dst.KeepSorterState = view.KeepSorterState == true;
                    dst.ColumnSorterHash = view.ColumnSorterHash?
                        .Select(o => new ViewFiltersColumn.SorterColumnItem()
                        {
                            Label = ss.LabelTitle(context: context, columnName: o.Key),
                            DbColumnName = o.Key,
                            Order = SiteUtilities.DisplayOrder(context: context, type: o)
                        })
                        .ToList();
                }

                public static void ViewEditorTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    string prefix = "")
                {
                    var commandDisplayTypeOptionCollection = SiteUtilities.GetCommandDisplayTypeOptionCollection(context);
                    dst.UpdateCommand = commandDisplayTypeOptionCollection
                        .Where((kv, idx) => (view.UpdateCommand == null && idx == 0) || (kv.Key == (view.UpdateCommand?.ToInt().ToString())))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.OpenCopyDialogCommand = commandDisplayTypeOptionCollection
                        .Where((kv, idx) => (view.OpenCopyDialogCommand == null && idx == 0) || (kv.Key == (view.OpenCopyDialogCommand?.ToInt().ToString())))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.ReferenceCopyCommand = commandDisplayTypeOptionCollection
                        .Where((kv, idx) => (view.ReferenceCopyCommand == null && idx == 0) || (kv.Key == (view.ReferenceCopyCommand?.ToInt().ToString())))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.MoveTargetsCommand = commandDisplayTypeOptionCollection
                        .Where((kv, idx) => (view.MoveTargetsCommand == null && idx == 0) || (kv.Key == (view.MoveTargetsCommand?.ToInt().ToString())))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.EditOutgoingMail = commandDisplayTypeOptionCollection
                        .Where((kv, idx) => (view.EditOutgoingMail == null && idx == 0) || (kv.Key == (view.EditOutgoingMail?.ToInt().ToString())))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.DeleteCommand = commandDisplayTypeOptionCollection
                        .Where((kv, idx) => (view.DeleteCommand == null && idx == 0) || (kv.Key == (view.DeleteCommand?.ToInt().ToString())))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                }

                public static void ViewCalendarTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    string prefix = "",
                    bool hasCalendar = false)
                {
                    dst.CalendarGroupBy = ss.CalendarGroupByOptions(context: context)
                        .Where(kv => kv.Key == view.GetCalendarGroupBy())
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    if (ss.CalendarType == SiteSettings.CalendarTypes.Standard)
                    {
                        dst.CalendarTimePeriod = ss.CalendarTimePeriodOptions(context: context)
                            .Where(kv => kv.Key == view.GetCalendarTimePeriod(ss: ss))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                    }
                    if (ss.CalendarType == SiteSettings.CalendarTypes.FullCalendar)
                    {
                        dst.CalendarViewType = ss.CalendarViewTypeOptions(context: context)
                            .Where(kv => kv.Key == view.GetCalendarViewType())
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty);
                    }
                    dst.CalendarFromTo = ss.CalendarColumnOptions(context: context)
                        .Where(kv => kv.Key == view.GetCalendarFromTo(ss: ss))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.CalendarShowStatus = view.CalendarShowStatus == true;
                }

                public static void ViewCrosstabTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    string prefix = "",
                    bool hasCrosstab = false)
                {
                    var commandDisplayTypeOptionCollection = SiteUtilities.GetCommandDisplayTypeOptionCollection(context);
                    dst.CrosstabGroupByX = ss.CrosstabGroupByXOptions(context: context)
                        .Where(kv => kv.Key == view.GetCrosstabGroupByX(context: context, ss: ss))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.CrosstabGroupByY = ss.CrosstabGroupByYOptions(context: context)
                        .Where(kv => kv.Key == view.GetCrosstabGroupByY(context: context, ss: ss))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.CrosstabColumns = ss.CrosstabColumnsOptions(context: context)
                        .Where(kv => view.CrosstabColumns?.Contains(kv.Key) == true)
                        .Select(kv => new ViewFiltersColumn.FilterColumnItem() {
                            Label = kv.Value,
                            DbColumnName = kv.Key})
                        .ToList();
                    dst.CrosstabAggregateType = ss.CrosstabAggregationTypeOptions(context: context)
                        .Where(kv => kv.Key == view.GetCrosstabAggregateType(ss: ss))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.CrosstabValue = ss.CrosstabColumnsOptions(context: context)
                        .Where(kv => kv.Key == view.GetCrosstabValue(context: context, ss: ss))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.CrosstabTimePeriod = ss.CrosstabTimePeriodOptions(context: context)
                        .Where(kv => kv.Key == view.GetCrosstabTimePeriod(ss: ss))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.CrosstabNotShowZeroRows = view.CrosstabNotShowZeroRows == true;
                    dst.ExportCrosstabCommand = commandDisplayTypeOptionCollection
                        .Where((kv, idx) => (view.ExportCrosstabCommand == null && idx == 0) || (kv.Key == view.ExportCrosstabCommand?.ToInt().ToString()))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                }

                public static void ViewGanttTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    bool hasGantt)
                {
                    if (!hasGantt) return;
                    dst.GanttGroupBy = ss.GanttGroupByOptions(context: context)
                        .Where(kv => kv.Key == view.GetGanttGroupBy())
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.GanttSortBy = ss.GanttSortByOptions(context: context)
                        .Where(kv => kv.Key == view.GetGanttSortBy())
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                }

                public static void ViewTimeSeriesTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    bool hasTimeSeries)
                {
                    if (!hasTimeSeries) return;
                    dst.TimeSeriesGroupBy = ss.TimeSeriesGroupByOptions(context: context)
                        .Where((kv, idx) => (view.TimeSeriesGroupBy == null && idx == 0) || (kv.Key == view.TimeSeriesGroupBy))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.TimeSeriesAggregateType = ss.TimeSeriesAggregationTypeOptions(context: context)
                        .Where((kv, idx) => (view.TimeSeriesAggregateType == null && idx == 0) || (kv.Key == view.TimeSeriesAggregateType))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.TimeSeriesValue = ss.TimeSeriesValueOptions(context: context)
                        .Where((kv, idx) => (view.TimeSeriesValue == null && idx == 0) || (kv.Key == view.TimeSeriesValue))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                }

                public static void ViewAnalyTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    bool hasAnaly)
                {
                    if (!hasAnaly) return;
                }

                public static void ViewKambanTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss,
                    bool hasKamban)
                {
                    if (!hasKamban) return;
                    dst.KambanGroupByX = ss.KambanGroupByOptions(context: context)
                        .Where((kv, idx) => (view.KambanGroupByX == null && idx == 0) || (kv.Key == view.KambanGroupByX))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.KambanGroupByY = ss.KambanGroupByOptions(context: context, addNothing: true)
                        .Where((kv, idx) => (view.KambanGroupByY == null && idx == 0) || (kv.Key == view.KambanGroupByY))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.KambanAggregateType = ss.KambanAggregationTypeOptions(context: context)
                        .Where(kv => kv.Key == view.GetKambanAggregationType(context: context, ss: ss))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.KambanValue = ss.KambanValueOptions(context: context)
                        .Where((kv, idx) => (view.KambanValue == null && idx == 0) || (kv.Key == view.KambanValue))
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.KambanColumns = view.GetKambanColumns();
                    dst.KambanAggregationView = view.KambanAggregationView == true;
                    dst.KambanShowStatus = view.KambanShowStatus == true;
                }

                public static void ViewAccessControlTab(
                    ViewFilterColumn dst,
                    Context context,
                    View view,
                    SiteSettings ss)
                {
                    var currentPermissions = view.GetPermissions(ss: ss);
                    dst.Permissions = currentPermissions
                        .Select(o => o.ControlData(
                            context: context,
                            ss: ss,
                            withType: false).Text)
                        .ToList();
                }

                internal static void SetChangeColumnName(
                    ViewFilterColumn dst,
                    Context context,
                    View column,
                    SiteSettings ss,
                    bool hasCalendar,
                    bool hasCrosstab,
                    bool hasGantt,
                    bool hasTimeSeries,
                    bool hasAnaly,
                    bool hasKamban)
                {
                    var isArray = new string[]{
                            "GridColumns","FilterColumns","ColumnSorterHash","CrosstabColumns","Permissions","ColumnFilterHash"
                        };
                    var inViewNew = new View();
                    var outColumnNew = new ViewFilterColumn();
                    ViewFilterColumn.General(
                        dst: outColumnNew,
                        context: context,
                        column: inViewNew,
                        ss: ss);
                    ViewFilterColumn.ViewGridTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss);
                    ViewFilterColumn.ViewFiltersTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss);
                    ViewFilterColumn.ViewSortersTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss);
                    ViewFilterColumn.ViewEditorTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss);
                    ViewFilterColumn.ViewCalendarTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss,
                        hasCalendar: hasCalendar);
                    ViewFilterColumn.ViewCrosstabTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss,
                        hasCrosstab: hasCrosstab);
                    ViewFilterColumn.ViewGanttTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss,
                        hasGantt: hasGantt);
                    ViewFilterColumn.ViewTimeSeriesTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss,
                        hasTimeSeries: hasTimeSeries);
                    ViewFilterColumn.ViewAnalyTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss,
                        hasAnaly: hasAnaly);
                    ViewFilterColumn.ViewKambanTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss,
                        hasKamban: hasKamban);
                    ViewFilterColumn.ViewAccessControlTab(
                        dst: outColumnNew,
                        context: context,
                        view: inViewNew,
                        ss: ss);
                    var t = typeof(ViewFilterColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        if (n == "ReadOnlyColumns") continue;
                        if (n == "Id")
                        {
                            dst.ReadOnlyColumns.Add(n);
                            continue;
                        }
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }
            }
        }

        public class NotificationsSettingsModel : SettingsModelBase
        {
            internal static NotificationsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new NotificationsSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Notifications(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel : siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    var ss = siteModel.SiteSettings;
                    foreach (var notification in ss.Notifications)
                    {
                        var dst = new ListColumn();
                        ListColumn.SetData(dst: dst, context: context, notification: notification, ss: ss);
                        ListColumn.SetChangeColumnName(dst: dst, context: context, notification: notification, ss: ss);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string Type;
                public string Prefix;
                public string Subject;
                public string Address;
                public string CcAddress;
                public string BccAddress;
                public string Token;
                public string MethodType;
                public string Encoding;
                public string MediaType;
                public string Headers;
                public bool? UseCustomFormat;
                public string Format;
                public string BeforeCondition;
                public string Expression;
                public string AfterCondition;
                public bool? AfterCreate;
                public bool? AfterUpdate;
                public bool? AfterDelete;
                public bool? AfterCopy;
                public bool? AfterBulkUpdate;
                public bool? AfterBulkDelete;
                public bool? AfterImport;
                public bool? Disabled;
                public List<string> MonitorChangesColumns;
                public List<string> ChangedColumns = new ();

                internal static void SetData(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Notification notification,
                    SiteSettings ss)
                {
                    dst.Id = notification.Id;
                    dst.Type = DumpUtils.GetNotificationTypeText(context: context, type: notification.Type);
                    dst.Prefix = notification.Prefix.IsNotEmpty();
                    dst.Subject = ss.ColumnNameToLabelText(text: notification.Subject);
                    dst.Address = ss.ColumnNameToLabelText(text: notification.Address);
                    dst.CcAddress = notification.Type == Settings.Notification.Types.Mail
                        ? ss.ColumnNameToLabelText(text: notification.CcAddress)
                        : null;
                    dst.BccAddress = notification.Type == Settings.Notification.Types.Mail
                        ? ss.ColumnNameToLabelText(text: notification.BccAddress)
                        : null;
                    dst.Token = NotificationUtilities.RequireToken(notification: notification)
                        ? notification.Token
                        : null;
                    dst.MethodType = NotificationUtilities.NotificationType(notification: notification)
                        ? NotificationUtilities.MethodTypes(context: context)
                            .Where(kv => kv.Key == notification.MethodType?.ToInt().ToString())
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty)
                        : null;
                    dst.Encoding = NotificationUtilities.NotificationType(notification: notification)
                        ? NotificationUtilities.Encodings(context: context)
                            .Where(kv => kv.Key == notification.Encoding)
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty)
                        : null;
                    dst.MediaType = NotificationUtilities.NotificationType(notification: notification)
                        ? notification.MediaType
                        : null;
                    dst.Headers = NotificationUtilities.NotificationType(notification: notification)
                        ? notification.Headers
                        : null;
                    dst.UseCustomFormat = notification.UseCustomFormat == true;
                    dst.Format = notification.UseCustomFormat == true
                        ? ss.ColumnNameToLabelText(text: notification.GetFormat(
                            context: context,
                            ss: ss))
                        : null;
                    dst.BeforeCondition = ss.ViewSelectableOptions()
                        .Where(kv => kv.Key == notification.BeforeCondition.ToString())
                        .Select(kv => kv.Value.Text)
                        .FirstOrDefault(string.Empty);
                    dst.Expression = notification.Expression switch
                    {
                        Settings.Notification.Expressions.And => Displays.And(context: context),
                        _ => Displays.Or(context: context)
                    };
                    dst.AfterCondition = ss.ViewSelectableOptions()
                        .Where(kv => kv.Key == notification.AfterCondition.ToString())
                        .Select(kv => kv.Value.Text)
                        .FirstOrDefault(string.Empty);
                    dst.AfterCreate = notification.AfterCreate != false;
                    dst.AfterUpdate = notification.AfterUpdate != false;
                    dst.AfterDelete = notification.AfterDelete != false;
                    dst.AfterCopy = notification.AfterCopy != false;
                    dst.AfterBulkUpdate = notification.AfterBulkUpdate != false;
                    dst.AfterBulkDelete = notification.AfterBulkDelete != false;
                    dst.AfterImport = notification.AfterImport != false;
                    dst.Disabled = notification.Disabled == true;
                    dst.MonitorChangesColumns = ss
                        .MonitorChangesSelectableOptions(
                            context: context,
                            monitorChangesColumns: notification.MonitorChangesColumns)
                        .Select(kv => kv.Value.Title)
                        .ToList();
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Id",Text=Displays.Id(context:context),ReadOnly=true},
                            new (){Key="Type",Text=Displays.NotificationType(context:context)},
                            new (){Key="Prefix",Text=Displays.Prefix(context:context)},
                            new (){Key="Subject",Text=Displays.Subject(context:context)},
                            new (){Key="Address",Text=Displays.Address(context:context)},
                            new (){Key="CcAddress",Text=Displays.Cc(context:context)},
                            new (){Key="BccAddress",Text=Displays.Bcc(context:context)},
                            new (){Key="Token",Text=Displays.Token(context:context)},
                            new (){Key="MethodType",Text=Displays.MethodType(context:context)},
                            new (){Key="Encoding",Text=Displays.Encoding(context:context)},
                            new (){Key="MediaType",Text=Displays.MediaType(context:context)},
                            new (){Key="Headers",Text=Displays.HttpHeader(context:context)},
                            new (){Key="UseCustomFormat",Text=Displays.UseCustomDesign(context:context)},
                            new (){Key="Format",Text=Displays.Format(context:context)},
                            new (){Key="BeforeCondition",Text=Displays.BeforeCondition(context:context)},
                            new (){Key="Expression",Text=Displays.Expression(context:context)},
                            new (){Key="AfterCondition",Text=Displays.AfterCondition(context:context)},
                            new (){Key="AfterCreate",Text=Displays.AfterCreate(context:context)},
                            new (){Key="AfterUpdate",Text=Displays.AfterUpdate(context:context)},
                            new (){Key="AfterDelete",Text=Displays.AfterDelete(context:context)},
                            new (){Key="AfterCopy",Text=Displays.AfterCopy(context:context)},
                            new (){Key="AfterBulkUpdate",Text=Displays.AfterBulkUpdate(context:context)},
                            new (){Key="AfterBulkDelete",Text=Displays.AfterBulkDelete(context:context)},
                            new (){Key="AfterImport",Text=Displays.AfterImport(context:context)},
                            new (){Key="Disabled",Text=Displays.Disabled(context:context)},
                            new (){Key="MonitorChangesColumns",Text=Displays.MonitorChangesColumns(context:context)},
                        }
                    };
                }

                internal static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Notification notification,
                    SiteSettings ss)
                {
                    var isArray = new string[]{
                            "MonitorChangesColumns"
                        };
                    var column = new Implem.Pleasanter.Libraries.Settings.Notification(
                        type: Implem.Pleasanter.Libraries.Settings.Notification.Types.Mail,
                        monitorChangesColumns: ss
                            .ColumnDefinitionHash
                            .MonitorChangesDefinitions()
                            .Select(o => o.ColumnName)
                            .Where(o => ss.GetEditorColumnNames().Contains(o)
                                || o == "Comments")
                            .ToList());
                    var outColumnNew = new ListColumn();
                    ListColumn.SetData(
                        dst: outColumnNew,
                        context: context,
                        notification: column,
                        ss: ss);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }
            }
        }

        public class RemindersSettingsModel : SettingsModelBase
        {
            internal static RemindersSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new RemindersSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Reminders(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    var ss = siteModel.SiteSettings;
                    foreach (var reminder in ss.Reminders)
                    {
                        var dst = new ListColumn();
                        ListColumn.SetData(dst: dst, context: context, reminder: reminder, ss: ss);
                        ListColumn.SetChangeColumnName(dst: dst, context: context, reminder: reminder, ss: ss, siteModel: siteModel);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string ReminderType;
                public string Subject;
                public string Body;
                public string Line;
                public string From;
                public string To;
                public string Token;
                public string Column;
                public string StartDateTime;
                public string Type;
                public string Range;
                public bool? SendCompletedInPast;
                public bool? NotSendIfNotApplicable;
                public bool? NotSendHyperLink;
                public bool? ExcludeOverdue;
                public string Condition;
                public bool? Disabled;
                public List<string> ChangedColumns = new ();

                internal static void SetData(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Reminder reminder,
                    SiteSettings ss)
                {
                    dst.Id = reminder.Id;
                    dst.ReminderType = ReminderUtilities.Types(context: context)
                        .Where((kv, idx) => kv.Key == reminder.ReminderType.ToInt().ToString())
                        .Select(kv => kv.Value)
                        .FirstOrDefault(string.Empty);
                    dst.Subject = ss.ColumnNameToLabelText(text: reminder.Subject);
                    dst.Body = ss.ColumnNameToLabelText(text: reminder.Body);
                    dst.Line = ss.ColumnNameToLabelText(text: reminder.Line);
                    dst.From = ReminderUtilities.RequireFrom(reminder: reminder)
                        ? reminder.From
                        : null;
                    dst.To = ss.ColumnNameToLabelText( text:reminder.To);
                    dst.Token = ReminderUtilities.RequireToken(reminder: reminder)
                        ? reminder.Token
                        : null;
                    dst.Column = reminder.GetColumn(ss: ss) != null
                        ? ss.ColumnHash.Get(reminder.GetColumn(ss: ss)).LabelText
                        : string.Empty;
                    dst.StartDateTime = reminder.StartDateTime.InRange()
                        ? reminder.StartDateTime.ToString(Displays.Get(
                            context: context,
                            id: "YmdhmFormat"))
                        : null;
                    dst.Type = reminder.Type switch
                    {
                        Times.RepeatTypes.Daily => Displays.Daily(context: context),
                        Times.RepeatTypes.Weekly => Displays.Weekly(context: context),
                        Times.RepeatTypes.NumberWeekly => Displays.NumberWeekly(context: context),
                        Times.RepeatTypes.Monthly => Displays.Monthly(context: context),
                        Times.RepeatTypes.EndOfMonth => Displays.EndOfMonth(context: context),
                        Times.RepeatTypes.Yearly => Displays.Yearly(context: context),
                        _ => Displays.Daily(context: context)
                    };
                    dst.Range = reminder.Range.ToString();
                    dst.SendCompletedInPast = reminder.SendCompletedInPast == true;
                    dst.NotSendIfNotApplicable = reminder.NotSendIfNotApplicable == true;
                    dst.NotSendHyperLink = reminder.NotSendHyperLink == true;
                    dst.ExcludeOverdue = reminder.ExcludeOverdue == true;
                    dst.Condition = ss.ViewSelectableOptions()?.Any() == true
                        ? ss.ViewSelectableOptions()
                            .Where((kv, idx) => kv.Key == reminder.Condition.ToInt().ToString())
                            .Select(kv => kv.Value.Text)
                            .FirstOrDefault(string.Empty)
                        : null;
                    dst.Disabled = reminder.Disabled == true;
                }

                internal static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Reminder reminder,
                    SiteSettings ss,
                    SiteModel siteModel)
                {
                    var isArray = new string[]{
                            "MonitorChangesColumns"
                        };
                    var column = new Settings.Reminder(context: context)
                    {
                        ReminderType = Settings.ReminderUtilities.Types(context: context)
                            .Select(o => (Settings.Reminder.ReminderTypes)o.Key.ToInt())
                            .FirstOrDefault(),
                        Subject = siteModel.Title.DisplayValue
                    };
                    var outColumnNew = new ListColumn();
                    ListColumn.SetData(
                        dst: outColumnNew,
                        context: context,
                        reminder: column,
                        ss: ss);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Id",Text=Displays.Id(context:context),ReadOnly=true},
                            new (){Key="ReminderType",Text=Displays.ReminderType(context:context)},
                            new (){Key="Subject",Text=Displays.Subject(context:context)},
                            new (){Key="Body",Text=Displays.Body(context:context)},
                            new (){Key="Line",Text=Displays.Row(context:context)},
                            new (){Key="From",Text=Displays.From(context:context)},
                            new (){Key="To",Text=Displays.To(context:context)},
                            new (){Key="Token",Text=Displays.Token(context:context)},
                            new (){Key="Column",Text=Displays.Column(context:context)},
                            new (){Key="StartDateTime",Text=Displays.StartDateTime(context:context)},
                            new (){Key="Type",Text=Displays.PeriodType(context:context)},
                            new (){Key="Range",Text=Displays.Range(context:context)},
                            new (){Key="SendCompletedInPast",Text=Displays.SendCompletedInPast(context:context)},
                            new (){Key="NotSendIfNotApplicable",Text=Displays.NotSendIfNotApplicable(context:context)},
                            new (){Key="NotSendHyperLink",Text=Displays.NotSendHyperLink(context:context)},
                            new (){Key="ExcludeOverdue",Text=Displays.ExcludeOverdue(context:context)},
                            new (){Key="Condition",Text=Displays.Condition(context:context)},
                            new (){Key="Disabled",Text=Displays.Disabled(context:context)},
                        }
                    };
                }

            }
        }

        public class ImportsSettingsModel : SettingsModelBase
        {
            internal static ImportsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new ImportsSettingsModel();
                obj.ButtonLabel = Displays.Import(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateImportsTable(context: context, param: param, siteModel: siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateImportsTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var table = new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                };
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: siteModel.ReferenceType);
                var isWikis = siteModel.ReferenceType == "Wikis";
                var columns = new KeyValueTableBase.ColumnModel[]{
                    new ()
                    {
                        Label = Displays.CharacterCode(context: context),
                        Name = "ImportEncoding",
                        Type = "string",
                        Value = ss.ImportEncoding switch
                        {
                            "Shift-JIS" => "Shift-JIS",
                            "UTF-8" => "UTF-8",
                            _ => "Shift-JIS"
                        },
                        Changed = ss.ImportEncoding != ssNew.ImportEncoding
                    },
                    new()
                    {
                        Label = Displays.UpdatableImport(context: context),
                        Name = "UpdatableImport",
                        Type = "boolean",
                        Value = (ss.UpdatableImport == true).ToString().ToLower(),
                        Changed = ss.UpdatableImport != ssNew.UpdatableImport
                    },
                    new()
                    {
                        Label = Displays.DefaultImportKey(context: context),
                        Name = "DefaultImportKey",
                        Type = "string",
                        Value = ss.Columns?
                            .Where(o => o.ImportKey == true)
                            .OrderBy(o => o.No)
                            .ToDictionary(
                                o => o.ColumnName,
                                o => o.LabelText)
                            .Where((kv,idx) => (ss.DefaultImportKey == null && idx == 0) || (ss.DefaultImportKey == kv.Key))
                            .Select(kv => kv.Value)
                            .FirstOrDefault(string.Empty) ?? string.Empty,
                        Changed = ss.DefaultImportKey != ssNew.DefaultImportKey
                    },
                    new()
                    {
                        Label = Displays.RejectNullImport(context: context),
                        Name = "RejectNullImport",
                        Type = "boolean",
                        Value = (ss.RejectNullImport == true).ToString().ToLower(),
                        Changed = ss.RejectNullImport != ssNew.RejectNullImport
                    },
                    new()
                    {
                        Label = Displays.AllowMigrationMode(context: context),
                        Name = "AllowMigrationMode",
                        Type = "boolean",
                        Value = (ss.AllowMigrationMode == true).ToString().ToLower(),
                        Changed = ss.AllowMigrationMode != ssNew.AllowMigrationMode
                    },
                };
                table.Columns.AddRange(columns.Where(v => v != null));
                return table;
            }
        }

        public class ExportsSettingsModel : SettingsModelBase
        {
            internal static ExportsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new ExportsSettingsModel();
                obj.ButtonLabel = Displays.Export(context: context);
                obj.Tables = new ITableModel[]
                {
                    ExportTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateBaseTable(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.AllowStandardExport(context: context),
                            Name = "AllowStandardExport",
                            Type = "bool",
                            Value = (ss.AllowStandardExport == true).ToString().ToLower(),
                            Changed = ss.AllowStandardExport != ssNew.AllowStandardExport
                        },
                    }
                };
            }

            public class ExportTable : List3TableBase<ExportColumn>
            {
                internal static ExportTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    return new()
                    {
                        Header = ExportColumn.CreateHeaderModel(context: context, param: param, siteModel: siteModel),
                        Columns = CreateColumns(context: context, param: param, siteModel: siteModel),
                    };
                }

                private static List<ExportColumn> CreateColumns(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var columns = new List<ExportColumn>();
                    var ss = siteModel.SiteSettings;
                    if (ss.Views == null) return columns;
                    foreach (var export in ss.Exports)
                    {
                        var dst = new ExportColumn();
                        columns.Add(dst);
                        dst.Id = export.Id;
                        dst.Name = export.Name;
                        ExportColumn.ExportGeneralTab(
                            dst: dst,
                            context: context,
                            export: export,
                            ss: ss);
                        ExportColumn.ExportAccessControlTab(
                            dst: dst,
                            context: context,
                            export: export,
                            ss: ss);
                        ExportColumn.SetChangeColumnName(
                            dst: dst,
                            context: context,
                            export: export,
                            ss: ss);
                    }
                    return columns;
                }

            }

            public class ExportColumn
            {
                public int Id;
                public string Name;
                public string Type;
                public string DelimiterType;
                public bool? EncloseDoubleQuotes;
                public string ExecutionType;
                public bool? Headers;
                public List<string> Columns;
                public List<string> ReadOnlyColumns = new();
                public List<string> ChangedColumns = new();
                public List<string> Permissions;

                internal static List3TableHeader CreateHeaderModel(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var labels = new List<List3TableHeader.Tab>
                    {
                        new (
                            tabName: new ("LockHeaderRight", ""),
                            labels: new ()
                            {
                                new ("Id",Displays.Id(context: context)),
                            }),
                        new (
                            tabName: new ("ExportGeneralTab", Displays.General(context: context)),
                            labels: new ()
                            {
                                new ("Name", Displays.Name(context: context)),
                                new ("Type", Displays.ExportTypes(context: context)),
                                new ("DelimiterType", Displays.DelimiterTypes(context: context)),
                                new ("EncloseDoubleQuotes", Displays.EncloseDoubleQuotes(context: context)),
                                new ("ExecutionType", Displays.ExportExecutionType(context: context)),
                                new ("Headers", Displays.OutputHeader(context: context)),
                                new ("Columns", Displays.ExportColumns(context: context)),
                            }),
                        new (
                            tabName: new ("ViewAccessControlTab", Displays.AccessControls(context: context)),
                            labels: new ()
                            {
                                new ("Permissions", Displays.Permissions(context: context)),
                            })
                    }
                        .Where(v => v != null).ToList();
                    return new List3TableHeader(labels: labels);
                }

                internal static void ExportGeneralTab(
                    ExportColumn dst,
                    Context context,
                    Export export,
                    SiteSettings ss)
                {
                    export.SetColumns(
                        context: context,
                        ss: ss);
                    dst.Id = export.Id;
                    dst.Name = export.Name;
                    dst.Type = export.Type switch
                    {
                        Export.Types.Json => Displays.Json(context: context),
                        _ => Displays.Csv(context: context)
                    };
                    dst.DelimiterType = export.DelimiterType switch
                    {
                        Export.DelimiterTypes.Tab => Displays.Tab(context: context),
                        _ => Displays.Comma(context: context)
                    };
                    dst.EncloseDoubleQuotes = export.EncloseDoubleQuotes == true;
                    dst.ExecutionType = export.ExecutionType switch
                    {
                        Export.ExecutionTypes.MailNotify => Displays.MailNotify(context: context),
                        _ => Displays.Direct(context: context)
                    };
                    dst.Headers = export.Header == true;
                    dst.Columns = ExportUtilities
                        .ColumnOptions(export.Columns)
                        .Select(v => v.Value.Text)
                        .ToList();
                }

                internal static void ExportAccessControlTab(
                    ExportColumn dst,
                    Context context,
                    Export export,
                    SiteSettings ss)
                {
                    var currentPermissions = export.GetPermissions(ss: ss);
                    dst.Permissions = currentPermissions
                        .Select(o => o.ControlData(
                            context: context,
                            ss: ss,
                            withType: false).Text)
                        .ToList();
                }

                internal static void SetChangeColumnName(
                    ExportColumn dst,
                    Context context,
                    Export export,
                    SiteSettings ss)
                {
                    var isArray = new string[]{
                            "Permissions"
                        };
                    var exportNew = new Export(ss.DefaultExportColumns(context: context));
                    var outColumnNew = new ExportColumn();
                    ExportColumn.ExportGeneralTab(
                        dst: outColumnNew,
                        context: context,
                        export: exportNew,
                        ss: ss);
                    ExportColumn.ExportAccessControlTab(
                        dst: outColumnNew,
                        context: context,
                        export: exportNew,
                        ss: ss);
                    var t = typeof(ExportColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        if (n == "ReadOnlyColumns") continue;
                        if (n == "Id")
                        {
                            dst.ReadOnlyColumns.Add(n);
                            continue;
                        }
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }
            }
        }

        public class CalendarSettingsModel : SettingsModelBase
        {
            internal static CalendarSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new CalendarSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Calendar(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateBaseTable(context: context, param: param, siteModel : siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.Enabled(context: context),
                            Name = "EnableCalendar",
                            Type = "bool",
                            Value = (ss.EnableCalendar == true).ToString().ToLower(),
                            Changed = ss.EnableCalendar != ssNew.EnableCalendar
                        },
                        new ()
                        {
                            Label = Displays.CalendarType(context: context),
                            Name = "CalendarType",
                            Type = "string",
                            Value = ss.CalendarType switch
                            {
                                SiteSettings.CalendarTypes.Standard => Displays.Standard(context: context),
                                SiteSettings.CalendarTypes.FullCalendar => Displays.FullCalendar(context: context),
                                _ => Displays.FullCalendar(context: context)
                            },
                            Changed = ss.CalendarType != ssNew.CalendarType
                        }
                    }
                };
            }
        }

        public class CrosstabSettingsModel : SettingsModelBase
        {
            internal static CrosstabSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new CrosstabSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Crosstab(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateBaseTable(context: context, param: param, siteModel : siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.Enabled(context: context),
                            Name = "EnableCrosstab",
                            Type = "bool",
                            Value = (ss.EnableCrosstab == true).ToString().ToLower(),
                            Changed = ss.EnableCrosstab != ssNew.EnableCrosstab
                        },
                        new ()
                        {
                            Label = Displays.NoDisplayGraph(context: context),
                            Name = "NoDisplayCrosstabGraph",
                            Type = "bool",
                            Value = (ss.NoDisplayCrosstabGraph == true).ToString().ToLower(),
                            Changed = ss.NoDisplayCrosstabGraph != ssNew.NoDisplayCrosstabGraph
                        }
                    }
                };
            }
        }

        public class GanttSettingsModel : SettingsModelBase
        {
            internal static GanttSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new GanttSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Gantt(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateBaseTable(context: context, param: param, siteModel : siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.Enabled(context: context),
                            Name = "EnableGantt",
                            Type = "bool",
                            Value = (ss.EnableGantt == true).ToString().ToLower(),
                            Changed = ss.EnableGantt != ssNew.EnableGantt
                        },
                        new ()
                        {
                            Label = Displays.ShowProgressRate(context: context),
                            Name = "ShowGanttProgressRate",
                            Type = "bool",
                            Value = (ss.ShowGanttProgressRate == true).ToString().ToLower(),
                            Changed = ss.ShowGanttProgressRate != ssNew.ShowGanttProgressRate
                        }
                    }
                };
            }
        }

        public class BurnDownSettingsModel : SettingsModelBase
        {
            internal static BurnDownSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new BurnDownSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.BurnDown(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateBaseTable(context: context, param: param, siteModel : siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.Enabled(context: context),
                            Name = "EnableBurnDown",
                            Type = "bool",
                            Value = (ss.EnableBurnDown == true).ToString().ToLower(),
                            Changed = ss.EnableBurnDown != ssNew.EnableBurnDown
                        },
                    }
                };
            }
        }

        public class TimeSeriesSettingsModel : SettingsModelBase
        {
            internal static TimeSeriesSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new TimeSeriesSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.TimeSeries(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateBaseTable(context: context, param: param, siteModel : siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.Enabled(context: context),
                            Name = "EnableCrosstab",
                            Type = "bool",
                            Value = (ss.EnableTimeSeries == true).ToString().ToLower(),
                            Changed = ss.EnableTimeSeries != ssNew.EnableTimeSeries
                        }
                    }
                };
            }
        }

        public class AnalySettingsModel : SettingsModelBase
        {
            internal static AnalySettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new AnalySettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Analy(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateBaseTable(context: context, param: param, siteModel : siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.Enabled(context: context),
                            Name = "EnableCrosstab",
                            Type = "bool",
                            Value = (ss.EnableAnaly == true).ToString().ToLower(),
                            Changed = ss.EnableAnaly != ssNew.EnableAnaly
                        }
                    }
                };
            }
        }

        public class KambanSettingsModel : SettingsModelBase
        {
            internal static KambanSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new KambanSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Kamban(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateBaseTable(context: context, param: param, siteModel : siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.Enabled(context: context),
                            Name = "EnableCrosstab",
                            Type = "bool",
                            Value = (ss.EnableKamban == true).ToString().ToLower(),
                            Changed = ss.EnableKamban != ssNew.EnableKamban
                        }
                    }
                };
            }
        }

        public class ImageLibSettingsModel : SettingsModelBase
        {
            internal static ImageLibSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new ImageLibSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.ImageLib(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateBaseTable(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateBaseTable(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.Enabled(context: context),
                            Name = "EnableCrosstab",
                            Type = "bool",
                            Value = (ss.EnableImageLib == true).ToString().ToLower(),
                            Changed = ss.EnableImageLib != ssNew.EnableImageLib
                        },
                        new ()
                        {
                            Label = Displays.NumberPerPage(context: context),
                            Name = "ImageLibPageSize",
                            Type = "int",
                            Value = ss.ImageLibPageSize.ToDecimal().ToString(),
                            Changed = ss.ImageLibPageSize != ssNew.ImageLibPageSize
                        }
                    }
                };
            }
        }

        public class SearchSettingsModel : SettingsModelBase
        {
            internal static SearchSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new SearchSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Search(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateSearchGeneral(context: context, param: param, siteModel: siteModel),
                    SearchSettingsFulltext(context: context, param: param, siteModel: siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateSearchGeneral(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Label = Displays.SearchSettings(context: context),
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.SearchTypes(context: context),
                            Name = "SearchType",
                            Type = "string",
                            Value = ss.SearchType switch
                            {
                                SiteSettings.SearchTypes.FullText => Displays.FullText(context: context),
                                SiteSettings.SearchTypes.PartialMatch => Displays.PartialMatch(context: context),
                                SiteSettings.SearchTypes.MatchInFrontOfTitle => Displays.MatchInFrontOfTitle(context: context),
                                SiteSettings.SearchTypes.BroadMatchOfTitle => Displays.BroadMatchOfTitle(context: context),
                                _ => Displays.PartialMatch(context: context)
                            },
                            Changed = (ss.SearchType ?? SiteSettings.SearchTypes.PartialMatch) != (ssNew.SearchType ?? SiteSettings.SearchTypes.PartialMatch)
                        },
                        new KeyValueTableBase.ColumnModel
                        {
                            Label = Displays.Sites_DisableCrossSearch(context: context),
                            Name = "DisableCrossSearch",
                            Type = "bool",
                            Value = (siteModel.DisableCrossSearch == true).ToString().ToLower(),
                            Changed = siteModel.DisableCrossSearch != siteNew.DisableCrossSearch
                        }
                    }
                };
            }

            private static ITableModel SearchSettingsFulltext(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Label = Displays.FullTextSettings(context: context),
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.FullTextIncludeBreadcrumb(context: context),
                            Name = "FullTextIncludeBreadcrumb",
                            Type = "bool",
                            Value = (ss.FullTextIncludeBreadcrumb == true).ToString().ToLower(),
                            Changed = ss.FullTextIncludeBreadcrumb != ssNew.FullTextIncludeBreadcrumb
                        },
                        new ()
                        {
                            Label = Displays.FullTextIncludeSiteId(context: context),
                            Name = "FullTextIncludeSiteId",
                            Type = "int",
                            Value = ss.FullTextIncludeSiteId.ToDecimal().ToString(),
                            Changed = ss.FullTextIncludeSiteId != ssNew.FullTextIncludeSiteId
                        },
                        new ()
                        {
                            Label = Displays.FullTextIncludeSiteTitle(context: context),
                            Name = "FullTextIncludeSiteTitle",
                            Type = "bool",
                            Value = (ss.FullTextIncludeSiteTitle == true).ToString().ToLower(),
                            Changed = ss.FullTextIncludeSiteTitle != ssNew.FullTextIncludeSiteTitle
                        },
                        new ()
                        {
                            Label = Displays.FullTextNumberOfMails(context: context),
                            Name = "FullTextNumberOfMails",
                            Type = "int",
                            Value = ss.FullTextNumberOfMails.ToDecimal().ToString(),
                            Changed = ss.FullTextNumberOfMails != ssNew.FullTextNumberOfMails
                        }
                    }
                };
            }
        }

        public class MailSettingsModel : SettingsModelBase
        {
            internal static MailSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new MailSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Mail(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateMailGeneral(context: context, param: param, siteModel: siteModel),
                    MailDefaultDestinations(context: context, param: param, siteModel: siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateMailGeneral(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.DefaultAddressBook(context: context),
                            Name = "SearchType",
                            Type = "string",
                            Value = ss.AddressBook.ToStr(),
                            Changed = ss.AddressBook != ssNew.AddressBook
                        }
                    }
                };
            }

            private static ITableModel MailDefaultDestinations(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Label = Displays.DefaultDestinations(context: context),
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.OutgoingMails_To(context: context),
                            Name = "MailToDefault",
                            Type = "string",
                            Value = ss.MailToDefault.ToStr(),
                            Changed = ss.MailToDefault != ssNew.MailToDefault
                        },
                        new ()
                        {
                            Label = Displays.OutgoingMails_Cc(context: context),
                            Name = "MailCcDefault",
                            Type = "string",
                            Value = ss.MailCcDefault.ToStr(),
                            Changed = ss.MailCcDefault != ssNew.MailCcDefault
                        },
                        new ()
                        {
                            Label = Displays.OutgoingMails_Bcc(context: context),
                            Name = "MailBccDefault",
                            Type = "string",
                            Value = ss.MailBccDefault.ToStr(),
                            Changed = ss.MailBccDefault != ssNew.MailBccDefault
                        }
                    }
                };
            }
        }

        public class SiteIntegrationSettingsModel : SettingsModelBase
        {
            internal static SiteIntegrationSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new SiteIntegrationSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.SiteIntegration(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateSiteIntegrationGeneral(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateSiteIntegrationGeneral(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = new()
                    {
                        new ()
                        {
                            Label = Displays.SiteId(context: context),
                            Name = "IntegratedSites",
                            Type = "string",
                            Value = ss.IntegratedSites?.Join(),
                            Changed = ss.IntegratedSites?.Join() != ssNew.IntegratedSites?.Join()
                        }
                    }
                };
            }
        }

        public class StylesSettingsModel : SettingsModelBase
        {
            internal static StylesSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Sites/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new StylesSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Styles(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateStyleBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    var ss = siteModel.SiteSettings;
                    foreach (var style in ss.Styles)
                    {
                        var dst = new ListColumn();
                        ListColumn.SetData(dst: dst, context: context, style: style, ss: ss);
                        ListColumn.SetChangeColumnName(dst: dst, context: context, style: style, ss: ss, siteModel: siteModel);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string Title;
                public string Body;
                public bool? Disabled;
                public bool? All;
                public bool? New;
                public bool? Edit;
                public bool? Index;
                public bool? Calendar;
                public bool? Crosstab;
                public bool? Gantt;
                public bool? BurnDown;
                public bool? TimeSeries;
                public bool? Analy;
                public bool? Kamban;
                public bool? ImageLib;
                public List<string> ChangedColumns = new ();
                public List<string> ReadonlyColumns = new ();

                internal static void SetData(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Style style,
                    SiteSettings ss)
                {
                    dst.Id = style.Id;
                    dst.Title = style.Title;
                    dst.Body = style.Body;
                    dst.Disabled = style.Disabled == true;
                    if (ss.ReferenceType != "Dashboards")
                    {
                        dst.All = style.All == true;
                        dst.New = style.New == true;
                        dst.Edit = style.Edit == true;
                        dst.Index = style.Index == true;
                        dst.Calendar = style.Calendar == true;
                        dst.Crosstab = style.Crosstab == true;
                        dst.Gantt = style.Gantt == true;
                        dst.BurnDown = style.BurnDown == true;
                        dst.TimeSeries = style.TimeSeries == true;
                        dst.Analy = style.Analy == true;
                        dst.Kamban = style.Kamban == true;
                        dst.ImageLib = style.ImageLib == true;
                        dst.ReadonlyColumns.AddRange(style.All == true
                            ? new string[] { "New", "Edit", "Index", "Calendar", "Crosstab", "Gantt", "BurnDown", "TimeSeries", "Analy", "Kamban", "ImageLib" }
                            : new string[] { "All" });
                    }
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Id",Text=Displays.Id(context:context),ReadOnly=true},
                            new (){Key="Title",Text=Displays.Title(context:context)},
                            new (){Key="Body",Text=Displays.Style(context:context)},
                            new (){Key="Disabled",Text=Displays.Disabled(context:context)},
                            new (){Key="All",Text=Displays.All(context:context)},
                            new (){Key="New",Text=Displays.New(context:context)},
                            new (){Key="Edit",Text=Displays.Edit(context:context)},
                            new (){Key="Index",Text=Displays.Index(context:context)},
                            new (){Key="Calendar",Text=Displays.Calendar(context:context)},
                            new (){Key="Crosstab",Text=Displays.Crosstab(context:context)},
                            new (){Key="Gantt",Text=Displays.Gantt(context:context)},
                            new (){Key="BurnDown",Text=Displays.BurnDown(context:context)},
                            new (){Key="TimeSeries",Text=Displays.TimeSeries(context:context)},
                            new (){Key="Analy",Text=Displays.Analy(context:context)},
                            new (){Key="Kamban",Text=Displays.Kamban(context:context)},
                            new (){Key="ImageLib",Text=Displays.ImageLib(context:context)},
                        }
                    };
                }

                internal static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Style style,
                    SiteSettings ss,
                    SiteModel siteModel)
                {
                    var isArray = new string[]{
                        };
                    var inColumnNew = new Style() { All = true };
                    var outColumnNew = new ListColumn();
                    ListColumn.SetData(
                        dst: outColumnNew,
                        context: context,
                        style: inColumnNew,
                        ss: ss);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }
            }

            private static ITableModel CreateStyleBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                var columns = new List<KeyValueTableBase.ColumnModel>
                {
                    new ()
                    {
                        Label = Displays.AllDisabled(context: context),
                        Name = "AllDisabled",
                        Type = "bool",
                        Value = (ss.StylesAllDisabled == true).ToString().ToLower(),
                        Changed = ss.StylesAllDisabled != ssNew.StylesAllDisabled
                    },
                    (!Parameters.Mobile.Responsive)
                        ? null
                        : new ()
                        {
                            Label = Displays.Responsive(context: context),
                            Name = "Responsive",
                            Type = "bool",
                            Value = (ss.Responsive == true).ToString().ToLower(),
                            Changed = ss.Responsive != ssNew.Responsive
                        }
                }
                    .Where(v => v != null)
                    .ToList();
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = columns
                };
            }
        }

        public class ScriptsSettingsModel : SettingsModelBase
        {
            internal static ScriptsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Sites/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new ScriptsSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Scripts(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateScriptBase(context: context, param : param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    var ss = siteModel.SiteSettings;
                    foreach (var script in ss.Scripts)
                    {
                        var dst = new ListColumn();
                        ListColumn.SetData(dst: dst, context: context, script: script, ss: ss);
                        ListColumn.SetChangeColumnName(dst: dst, context: context, script: script, ss: ss, siteModel: siteModel);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string Title;
                public string Body;
                public bool? Disabled;
                public bool? All;
                public bool? New;
                public bool? Edit;
                public bool? Index;
                public bool? Calendar;
                public bool? Crosstab;
                public bool? Gantt;
                public bool? BurnDown;
                public bool? TimeSeries;
                public bool? Analy;
                public bool? Kamban;
                public bool? ImageLib;
                public List<string> ChangedColumns = new ();
                public List<string> ReadonlyColumns = new ();

                internal static void SetData(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Script script,
                    SiteSettings ss)
                {
                    dst.Id = script.Id;
                    dst.Title = script.Title;
                    dst.Body = script.Body;
                    dst.Disabled = script.Disabled == true;
                    if (ss.ReferenceType != "Dashboards")
                    {
                        dst.All = script.All == true;
                        dst.New = script.New == true;
                        dst.Edit = script.Edit == true;
                        dst.Index = script.Index == true;
                        dst.Calendar = script.Calendar == true;
                        dst.Crosstab = script.Crosstab == true;
                        dst.Gantt = script.Gantt == true;
                        dst.BurnDown = script.BurnDown == true;
                        dst.TimeSeries = script.TimeSeries == true;
                        dst.Analy = script.Analy == true;
                        dst.Kamban = script.Kamban == true;
                        dst.ImageLib = script.ImageLib == true;
                        dst.ReadonlyColumns.AddRange(script.All == true
                            ? new string[] { "New", "Edit", "Index", "Calendar", "Crosstab", "Gantt", "BurnDown", "TimeSeries", "Analy", "Kamban", "ImageLib" }
                            : new string[] { "All" });
                    }
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Id",Text=Displays.Id(context:context),ReadOnly=true},
                            new (){Key="Title",Text=Displays.Title(context:context)},
                            new (){Key="Body",Text=Displays.Script(context:context)},
                            new (){Key="Disabled",Text=Displays.Disabled(context:context)},
                            new (){Key="All",Text=Displays.All(context:context)},
                            new (){Key="New",Text=Displays.New(context:context)},
                            new (){Key="Edit",Text=Displays.Edit(context:context)},
                            new (){Key="Index",Text=Displays.Index(context:context)},
                            new (){Key="Calendar",Text=Displays.Calendar(context:context)},
                            new (){Key="Crosstab",Text=Displays.Crosstab(context:context)},
                            new (){Key="Gantt",Text=Displays.Gantt(context:context)},
                            new (){Key="BurnDown",Text=Displays.BurnDown(context:context)},
                            new (){Key="TimeSeries",Text=Displays.TimeSeries(context:context)},
                            new (){Key="Analy",Text=Displays.Analy(context:context)},
                            new (){Key="Kamban",Text=Displays.Kamban(context:context)},
                            new (){Key="ImageLib",Text=Displays.ImageLib(context:context)},
                        }
                    };
                }

                internal static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Script script,
                    SiteSettings ss,
                    SiteModel siteModel)
                {
                    var isArray = new string[]{};
                    var inColumnNew = new Script() { All = true };
                    var outColumnNew = new ListColumn();
                    ListColumn.SetData(
                        dst: outColumnNew,
                        context: context,
                        script: inColumnNew,
                        ss: ss);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        if (n == "ReadonlyColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }
            }

            private static ITableModel CreateScriptBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                var columns = new List<KeyValueTableBase.ColumnModel>
                {
                    new ()
                    {
                        Label = Displays.AllDisabled(context: context),
                        Name = "AllDisabled",
                        Type = "bool",
                        Value = (ss.ScriptsAllDisabled== true).ToString().ToLower(),
                        Changed = ss.ScriptsAllDisabled != ssNew.ScriptsAllDisabled
                    }
                }
                    .Where(v => v != null)
                    .ToList();
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = columns
                };
            }

        }

        public class HtmlsSettingsModel : SettingsModelBase
        {
            internal static HtmlsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Sites/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new HtmlsSettingsModel();
                var ss = siteModel.SiteSettings;
                obj.ButtonLabel = Displays.Html(context: context);
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(
                        context: context,
                        param: param,
                        siteModel: siteModel),
                    CreateHtmlBase(
                        context: context,
                        param: param,
                        siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    var ss = siteModel.SiteSettings;
                    foreach (var html in ss.Htmls)
                    {
                        var dst = new ListColumn();
                        ListColumn.SetData(dst: dst, context: context, html: html, ss: ss);
                        ListColumn.SetChangeColumnName(dst: dst, context: context, html: html, ss: ss, siteModel: siteModel);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string Title;
                public string PositionType;
                public string Body;
                public bool? Disabled;
                public bool? All;
                public bool? New;
                public bool? Edit;
                public bool? Index;
                public bool? Calendar;
                public bool? Crosstab;
                public bool? Gantt;
                public bool? BurnDown;
                public bool? TimeSeries;
                public bool? Analy;
                public bool? Kamban;
                public bool? ImageLib;
                public List<string> ChangedColumns = new ();
                public List<string> ReadonlyColumns = new ();

                public static void SetData(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Html html,
                    SiteSettings ss)
                {
                    dst.Id = html.Id;
                    dst.Title = html.Title;
                    dst.PositionType = html.PositionType switch
                    {
                        Settings.Html.PositionTypes.HeadTop => Displays.HtmlHeadTop(context: context),
                        Settings.Html.PositionTypes.HeadBottom => Displays.HtmlHeadBottom(context: context),
                        Settings.Html.PositionTypes.BodyScriptTop => Displays.HtmlBodyScriptTop(context: context),
                        Settings.Html.PositionTypes.BodyScriptBottom => Displays.HtmlBodyScriptBottom(context: context),
                        _ => string.Empty
                    };
                    dst.Body = html.Body;
                    dst.Disabled = html.Disabled == true;
                    if (ss.ReferenceType != "Dashboards")
                    {
                        dst.All = html.All == true;
                        dst.New = html.New == true;
                        dst.Edit = html.Edit == true;
                        dst.Index = html.Index == true;
                        dst.Calendar = html.Calendar == true;
                        dst.Crosstab = html.Crosstab == true;
                        dst.Gantt = html.Gantt == true;
                        dst.BurnDown = html.BurnDown == true;
                        dst.TimeSeries = html.TimeSeries == true;
                        dst.Analy = html.Analy == true;
                        dst.Kamban = html.Kamban == true;
                        dst.ImageLib = html.ImageLib == true;
                        dst.ReadonlyColumns.AddRange(html.All == true
                            ? new string[] { "New", "Edit", "Index", "Calendar", "Crosstab", "Gantt", "BurnDown", "TimeSeries", "Analy", "Kamban", "ImageLib" }
                            : new string[] { "All" });
                    }
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Id",Text=Displays.Id(context:context),ReadOnly=true},
                            new (){Key="Title",Text=Displays.Title(context:context)},
                            new (){Key="PositionType",Text=Displays.HtmlPositionType(context: context)},
                            new (){Key="Body",Text=Displays.Html(context:context)},
                            new (){Key="Disabled",Text=Displays.Disabled(context:context)},
                            new (){Key="All",Text=Displays.All(context:context)},
                            new (){Key="New",Text=Displays.New(context:context)},
                            new (){Key="Edit",Text=Displays.Edit(context:context)},
                            new (){Key="Index",Text=Displays.Index(context:context)},
                            new (){Key="Calendar",Text=Displays.Calendar(context:context)},
                            new (){Key="Crosstab",Text=Displays.Crosstab(context:context)},
                            new (){Key="Gantt",Text=Displays.Gantt(context:context)},
                            new (){Key="BurnDown",Text=Displays.BurnDown(context:context)},
                            new (){Key="TimeSeries",Text=Displays.TimeSeries(context:context)},
                            new (){Key="Analy",Text=Displays.Analy(context:context)},
                            new (){Key="Kamban",Text=Displays.Kamban(context:context)},
                            new (){Key="ImageLib",Text=Displays.ImageLib(context:context)},
                        }
                    };
                }

                internal static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.Html html,
                    SiteSettings ss,
                    SiteModel siteModel)
                {
                    var isArray = new string[]{};
                    var inColumnNew = new Settings.Html() { All = true };
                    var outColumnNew = new ListColumn();
                    ListColumn.SetData(
                        dst: outColumnNew,
                        context: context,
                        html: inColumnNew,
                        ss: ss);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }
            }

            private static ITableModel CreateHtmlBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                var columns = new List<KeyValueTableBase.ColumnModel>
                {
                    new ()
                    {
                        Label = Displays.AllDisabled(context: context),
                        Name = "AllDisabled",
                        Type = "bool",
                        Value = (ss.HtmlsAllDisabled == true).ToString().ToLower(),
                        Changed = ss.HtmlsAllDisabled != ssNew.HtmlsAllDisabled
                    }
                }
                    .Where(v => v != null)
                    .ToList();
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = columns
                };
            }

        }

        public class ServerScriptsSettingsModel : SettingsModelBase
        {
            internal static ServerScriptsSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var obj = new ServerScriptsSettingsModel();
                obj.ButtonLabel = Displays.ServerScript(context: context);
                var ss = siteModel.SiteSettings;
                obj.Tables = new ITableModel[]
                {
                    ListTable.CreateTable(context: context, param: param, siteModel: siteModel),
                    CreateHtmlBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateHtmlBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                var columns = new List<KeyValueTableBase.ColumnModel>
                {
                    new ()
                    {
                        Label = Displays.AllDisabled(context: context),
                        Name = "AllDisabled",
                        Type = "bool",
                        Value = (ss.ServerScriptsAllDisabled == true).ToString().ToLower(),
                        Changed = ss.ServerScriptsAllDisabled != ssNew.ServerScriptsAllDisabled
                    }
                }
                    .Where(v => v != null)
                    .ToList();
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = columns
                };
            }

            public class ListTable : List2TableBase<ListColumn>
            {
                internal static ListTable CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ListTable();
                    table.Header = ListColumn.CreateHeaderModel(context: context);
                    var ss = siteModel.SiteSettings;
                    foreach (var script in ss.ServerScripts)
                    {
                        var dst = new ListColumn();
                        ListColumn.SetData(dst: dst, context: context, script: script, ss: ss);
                        ListColumn.SetChangeColumnName(dst: dst, context: context, script: script, ss: ss, siteModel: siteModel);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class ListColumn
            {
                public int Id;
                public string Title;
                public string Name;
                public string Body;
                public bool? Disabled;
                public long? TimeOut;
                public bool? Functionalize;
                public bool? TryCatch;
                public bool? WhenloadingSiteSettings;
                public bool? WhenViewProcessing;
                public bool? WhenloadingRecord;
                public bool? BeforeFormula;
                public bool? AfterFormula;
                public bool? BeforeCreate;
                public bool? AfterCreate;
                public bool? BeforeUpdate;
                public bool? AfterUpdate;
                public bool? BeforeDelete;
                public bool? AfterDelete;
                public bool? BeforeBulkDelete;
                public bool? AfterBulkDelete;
                public bool? BeforeOpeningPage;
                public bool? BeforeOpeningRow;
                public bool? Shared;
                public List<string> ChangedColumns = new ();
                public List<string> ReadonlyColumns = new ();

                public static void SetData(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.ServerScript script,
                    SiteSettings ss)
                {
                    dst.Id = script.Id;
                    dst.Title = script.Title;
                    dst.Name = script.Name;
                    dst.Body = script.Body;
                    dst.Disabled = script.Disabled;
                    if (Parameters.Script.ServerScriptTimeOutChangeable)
                    {
                        dst.TimeOut = script.TimeOut ?? Parameters.Script.ServerScriptTimeOut;
                    }
                    dst.Functionalize = script.Functionalize == true;
                    dst.TryCatch = script.TryCatch == true;
                    dst.WhenloadingSiteSettings = script.WhenloadingSiteSettings == true;
                    dst.WhenViewProcessing = script.WhenViewProcessing == true;
                    dst.WhenloadingRecord = script.WhenloadingRecord == true;
                    dst.BeforeFormula = script.BeforeFormula == true;
                    dst.AfterFormula = script.AfterFormula == true;
                    dst.BeforeCreate = script.BeforeCreate == true;
                    dst.AfterCreate = script.AfterCreate == true;
                    dst.BeforeUpdate = script.BeforeUpdate == true;
                    dst.AfterUpdate = script.AfterUpdate == true;
                    dst.BeforeDelete = script.BeforeDelete == true;
                    dst.AfterDelete = script.AfterDelete == true;
                    dst.BeforeBulkDelete = script.BeforeBulkDelete == true;
                    dst.AfterBulkDelete = script.AfterBulkDelete == true;
                    dst.BeforeOpeningPage = script.BeforeOpeningPage == true;
                    dst.BeforeOpeningRow = script.BeforeOpeningRow == true;
                    dst.Shared = script.Shared == true;
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Id",Text=Displays.Id(context:context),ReadOnly=true},
                            new (){Key="Title",Text=Displays.Title(context:context)},
                            new (){Key="Name",Text=Displays.Name(context:context)},
                            new (){Key="Body",Text=Displays.Script(context:context)},
                            new (){Key="Disabled",Text=Displays.Disabled(context:context)},
                            new (){Key="TimeOut",Text=Displays.TimeOut(context:context),ReadOnly=Parameters.Script.ServerScriptTimeOutChangeable==false},
                            new (){Key="Functionalize",Text=Displays.Functionalize(context:context)},
                            new (){Key="TryCatch",Text=Displays.TryCatch(context:context)},
                            new (){Key="WhenloadingSiteSettings",Text=Displays.WhenloadingSiteSettings(context:context)},
                            new (){Key="WhenViewProcessing",Text=Displays.WhenViewProcessing(context:context)},
                            new (){Key="WhenloadingRecord",Text=Displays.WhenloadingRecord(context:context)},
                            new (){Key="BeforeFormula",Text=Displays.BeforeFormulas(context:context)},
                            new (){Key="AfterFormulas",Text=Displays.AfterFormulas(context:context)},
                            new (){Key="BeforeCreate",Text=Displays.BeforeCreate(context:context)},
                            new (){Key="AfterCreate",Text=Displays.AfterCreate(context:context)},
                            new (){Key="BeforeUpdate",Text=Displays.BeforeUpdate(context:context)},
                            new (){Key="AfterUpdate",Text=Displays.AfterUpdate(context:context)},
                            new (){Key="BeforeDelete",Text=Displays.BeforeDelete(context:context)},
                            new (){Key="AfterDelete",Text=Displays.AfterDelete(context:context)},
                            new (){Key="BeforeBulkDelete",Text=Displays.BeforeBulkDelete(context:context)},
                            new (){Key="AfterBulkDelete",Text=Displays.AfterBulkDelete(context:context)},
                            new (){Key="BeforeOpeningPage",Text=Displays.BeforeOpeningPage(context:context)},
                            new (){Key="BeforeOpeningRow",Text=Displays.BeforeOpeningRow(context:context)},
                            new (){Key="Shared",Text=Displays.Shared(context:context)},
                        }
                    };
                }

                internal static void SetChangeColumnName(
                    ListColumn dst,
                    Context context,
                    Implem.Pleasanter.Libraries.Settings.ServerScript script,
                    SiteSettings ss,
                    SiteModel siteModel)
                {
                    var isArray = new string[]{};
                    var inColumnNew = new ServerScript();
                    var outColumnNew = new ListColumn();
                    ListColumn.SetData(
                        dst: outColumnNew,
                        context: context,
                        script: inColumnNew,
                        ss: ss);
                    var t = typeof(ListColumn);
                    foreach (var f in t.GetFields())
                    {
                        var n = f.Name;
                        if (n == "ChangedColumns") continue;
                        if (n == "ReadonlyColumns") continue;
                        var v1 = f.GetValue(outColumnNew);
                        var v2 = f.GetValue(dst);
                        if (v1 != null || v2 != null)
                        {
                            var isChg = false;
                            if ((v1 == null) != (v2 == null))
                            {
                                isChg = true;
                            }
                            else if (isArray.Contains(n))
                            {
                                var vv1 = v1 as IEnumerable<string>;
                                var vv2 = v2 as IEnumerable<string>;
                                if ((vv1 != null && vv2 != null && vv1.SequenceEqual(vv2)) == false)
                                {
                                    isChg = true;
                                }
                            }
                            else if (!v1.Equals(v2))
                            {
                                isChg = true;
                            }
                            if (isChg)
                            {
                                dst.ChangedColumns.Add(n);
                            }
                        }
                    }
                }
            }

        }

        public class PublishSettingsModel : SettingsModelBase
        {
            internal static PublishSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                if (context.ContractSettings.Extensions.Get("Publish") != true) return null;
                var obj = new PublishSettingsModel();
                obj.ButtonLabel = Displays.Publish(context: context);
                var ss = siteModel.SiteSettings;
                obj.Tables = new ITableModel[]
                {
                    CreatePublishBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreatePublishBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                var columns = new List<KeyValueTableBase.ColumnModel>
                {
                    !(context.ContractSettings.Extensions.Get("Publish"))? null: new ()
                    {
                        Label = Displays.PublishToAnonymousUsers(context: context),
                        Name = "Publish",
                        Type = "bool",
                        Value = (siteModel.Publish == true).ToString().ToLower(),
                        Changed = siteModel.Publish
                    }
                }
                    .Where(v => v != null)
                    .ToList();
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = columns
                };
            }
        }

        public class SiteAccessControlSettingsModel : SettingsModelBase
        {
            internal static SiteAccessControlSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues/Sites/Wikis/Dashboards";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var ss = siteModel.SiteSettings;
                if (!context.CanManagePermission(ss)) return null;
                var obj = new SiteAccessControlSettingsModel();
                obj.ButtonLabel = Displays.SiteAccessControl(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateInheritPermission(context: context, param: param, siteModel: siteModel),
                    CreateSiteAccessPermissionList(context: context, param: param, siteModel: siteModel),
                    CreateSiteAccessControlBase(context: context, param: param, siteModel: siteModel)
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateInheritPermission(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                if (!context.CanManagePermission(ss: ss)) return null;
                return new ListTableBase()
                {
                    Header = new() { Labels = new() { Displays.InheritPermission(context: context) } },
                    Columns = new() { PermissionUtilities.InheritTargets(
                        context: context,
                        ss: siteModel.SiteSettings).OptionCollection
                            .Where(kv => kv.Key == siteModel.InheritPermission.ToString())
                            .Select(kv => kv.Value.Text)
                            .FirstOrDefault(string.Empty)}
                };
            }

            private static ITableModel CreateSiteAccessPermissionList(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var isSite = siteModel.ReferenceType == "Sites";
                if (!(!isSite || siteModel.SiteId == siteModel.InheritPermission))
                {
                    return null;
                }
                var currentPermissions = PermissionUtilities.CurrentCollection(
                    context: context,
                    referenceId: siteModel.SiteId);
                return SiteAccessControlSettingsModel.PermissionListTable.CreateTable(
                    context: context,
                    siteModel: siteModel,
                    ss: ss,
                    permissions: currentPermissions);
            }

            private static ITableModel CreateSiteAccessControlBase(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var isSite = true;
                var siteNew = new SiteModel();
                var ssNew = new SiteSettings(context: context, referenceType: ss.ReferenceType);
                var columns = new List<KeyValueTableBase.ColumnModel>
                {
                    !(isSite)? null: new ()
                    {
                        Label = Displays.NoDisplayIfReadOnly(context: context),
                        Name = "NoDisplayIfReadOnly",
                        Type = "bool",
                        Value = (ss.NoDisplayIfReadOnly == true).ToString().ToLower(),
                        Changed = ss.NoDisplayIfReadOnly != ssNew.NoDisplayIfReadOnly
                    },
                    !( ss.ReferenceType == "Sites")? null: new ()
                    {
                        Label = Displays.NoDisplayIfReadOnly(context: context),
                        Name = "NotInheritPermissionsWhenCreatingSite",
                        Type = "bool",
                        Value = (ss.NotInheritPermissionsWhenCreatingSite == true).ToString().ToLower(),
                        Changed = ss.NotInheritPermissionsWhenCreatingSite != ssNew.NotInheritPermissionsWhenCreatingSite
                    }
                }
                    .Where(v => v != null)
                    .ToList();
                return new KeyValueTableBase
                {
                    Header = KeyValueHeader.CreateDefault(context: context),
                    Columns = columns
                };
            }

            public class PermissionListTable : List2TableBase<PermissionListColumn>
            {
                public static PermissionListTable CreateTable(
                    Context context,
                    SiteModel siteModel,
                    IEnumerable<Permission> permissions,
                    SiteSettings ss,
                    string name = null)
                {
                    var table = new PermissionListTable();
                    if (name != null)
                    {
                        table.Label = name;
                    }
                    table.Header = PermissionListColumn.CreateHeaderModel(context: context);
                    var columns = permissions
                        .Select(x => (permission: x, text: x.ControlData(
                                context: context,
                                ss: ss).Text));
                    foreach (var permission in columns)
                    {
                        var dst = new PermissionListColumn();
                        PermissionListColumn.SetData(dst: dst, context: context, permission: permission, ss: ss);
                        PermissionListColumn.SetChangeColumnName(dst: dst, context: context, permission: permission, ss: ss, siteModel: siteModel);
                        (table.Columns ??= new()).Add(dst);
                    }
                    return table;
                }
            }

            public class PermissionListColumn
            {
                public List<string> ChangedColumns = new();
                public List<string> ReadonlyColumns = new();
                public string Text;
                public string Pattern;
                public bool? Read;
                public bool? Create;
                public bool? Update;
                public bool? Delete;
                public bool? SendMail;
                public bool? Export;
                public bool? Import;
                public bool? ManageSite;
                public bool? ManagePermission;

                internal static void SetData(
                    PermissionListColumn dst,
                    Context context,
                    (Permission permission, string text) permission,
                    SiteSettings ss)
                {
                    dst.Text = permission.text;
                    dst.Pattern = permission.permission.DisplayTypeName(context: context);
                    dst.Read = (permission.permission.Type & Permissions.Types.Read) > 0;
                    dst.Create = (permission.permission.Type & Permissions.Types.Create) > 0;
                    dst.Update = (permission.permission.Type & Permissions.Types.Update) > 0;
                    dst.Delete = (permission.permission.Type & Permissions.Types.Delete) > 0;
                    dst.SendMail = (permission.permission.Type & Permissions.Types.SendMail) > 0;
                    dst.Export = (permission.permission.Type & Permissions.Types.Export) > 0;
                    dst.Import = (permission.permission.Type & Permissions.Types.Import) > 0;
                    dst.ManageSite = (permission.permission.Type & Permissions.Types.ManageSite) > 0;
                    dst.ManagePermission = (permission.permission.Type & Permissions.Types.ManagePermission) > 0;
                }

                internal static void SetChangeColumnName(
                    PermissionListColumn dst,
                    Context context,
                    (Permission permission, string text) permission,
                    SiteSettings ss,
                    SiteModel siteModel)
                {
                    // 差分抽出は不要
                }

                internal static List2TableHeader CreateHeaderModel(Context context)
                {
                    return new()
                    {
                        Labels = new()
                        {
                            new (){Key="Text",Text=Displays.Authority(context: context)},
                            new (){Key="Pattern",Text=Displays.Pattern(context:context)},
                            new (){Key="Read",Text=Displays.Read(context:context)},
                            new (){Key="Create",Text=Displays.Create(context:context)},
                            new (){Key="Update",Text=Displays.Update(context:context)},
                            new (){Key="Delete",Text=Displays.Delete(context:context)},
                            new (){Key="SendMail",Text=Displays.SendMail(context:context)},
                            new (){Key="Export",Text=Displays.Export(context:context)},
                            new (){Key="Import",Text=Displays.Import(context:context)},
                            new (){Key="ManageSite",Text=Displays.ManageSite(context:context)},
                            new (){Key="ManagePermission",Text=Displays.ManagePermission(context:context)},
                        }
                    };
                }
            }
        }

        public class RecordAccessControlModel : SettingsModelBase
        {
            internal static RecordAccessControlModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var ss = siteModel.SiteSettings;
                if (!context.CanManagePermission(ss: ss)) return null;
                var obj = new RecordAccessControlModel();
                obj.ButtonLabel = Displays.RecordAccessControl(context: context);
                obj.Tables = new ITableModel[]
                {
                    CreateCreatePermissionList(
                        context: context,
                        param : param,
                        siteModel: siteModel),
                    CreateUpdatePermissionList(
                        context: context,
                        param: param,
                        siteModel: siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            private static ITableModel CreateCreatePermissionList(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var permissionsForCreating = PermissionUtilities.PermissionForCreating(ss).Where(o => !o.Source);
                return SiteAccessControlSettingsModel.PermissionListTable.CreateTable(
                    context: context,
                    siteModel: siteModel,
                    ss: ss,
                    permissions: permissionsForCreating,
                    name: Displays.PermissionForCreating(context: context));
            }

            private static ITableModel CreateUpdatePermissionList(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var ss = siteModel.SiteSettings;
                var permissionsForUpdating = PermissionUtilities.PermissionForUpdating(ss).Where(o => !o.Source).ToList();
                return SiteAccessControlSettingsModel.PermissionListTable.CreateTable(
                    context: context,
                    siteModel: siteModel,
                    ss: ss,
                    permissions: permissionsForUpdating,
                    name: Displays.PermissionForUpdating(context: context));
            }
        }

        public class ColumnAccessControlSettingsModel : SettingsModelBase
        {
            internal static ColumnAccessControlSettingsModel Create(
                Context context,
                Param param,
                SiteModel siteModel)
            {
                var referenceList = "Results/Issues";
                if (referenceList.IndexOf(siteModel.ReferenceType) < 0) return null;
                var ss = siteModel.SiteSettings;
                if (!context.CanManagePermission(ss: ss)) return null;
                var obj = new ColumnAccessControlSettingsModel();
                obj.ButtonLabel = Displays.ColumnAccessControl(context: context);
                obj.Tables = new ITableModel[]
                {
                    ColumnAccessTable.CreateTable(
                        context: context,
                        param: param,
                        siteModel: siteModel),
                }
                    .Where(v => v != null)
                    .ToList();
                return obj;
            }

            public class ColumnAccessTable : List3TableBase<ColumnAccessColumn>
            {
                internal static ITableModel CreateTable(
                    Context context,
                    Param param,
                    SiteModel siteModel)
                {
                    var table = new ColumnAccessTable();
                    table.Header = ColumnAccessColumn.CreateHeaderModel(context: context);
                    table.Columns = new();
                    var ss = siteModel.SiteSettings;
                    var c1 = ss.CreateColumnAccessControls.Where(v => !v.IsDefault(ss: ss, type: "Create")).ToDictionary(v => v.ColumnName, v => v);
                    var r1 = ss.ReadColumnAccessControls.Where(v => !v.IsDefault(ss: ss, type: "Read")).ToDictionary(v => v.ColumnName, v => v);
                    var u1 = ss.UpdateColumnAccessControls.Where(v => !v.IsDefault(ss: ss, type: "Update")).ToDictionary(v => v.ColumnName, v => v);
                    foreach (var columnName in ss.EditorColumnHash?.SelectMany(tab => tab.Value ?? Enumerable.Empty<string>()))
                    {
                        var c = c1.Get(columnName);
                        var r = r1.Get(columnName);
                        var u = u1.Get(columnName);
                        if (c != null || r != null || u != null)
                        {
                            var dst = new ColumnAccessColumn();
                            table.Columns.Add(dst);
                            ColumnAccessColumn.SetData(
                                dst: dst,
                                context: context,
                                columnName: columnName,
                                create: c,
                                read: r,
                                update: u,
                                ss: ss);
                            ColumnAccessColumn.SetChangeColumnName(
                                dst: dst,
                                context: context,
                                columnName: columnName,
                                create: c,
                                read: r,
                                update: u,
                                ss: ss);
                        }
                    }
                    return table;
                }

            }
            public class ColumnAccessColumn
            {
                public string ColumnName;
                public List<Access> Accesses;
                public List<string> ChangedColumns;

                internal static List3TableHeader CreateHeaderModel(Context context)
                {
                    var labels = new List<List3TableHeader.Tab>
                    {
                        new (
                            tabName: new ("LockHeaderRight", ""),
                            labels: new ()
                            {
                                new ("ColumnName",Displays.Column(context: context)),
                            }),
                        new (
                            tabName: new ("", ""),
                            labels: new ()
                            {
                                new ("Accesses.Type", Displays.ActionTypes(context: context), readOnly: true),
                                new ("Accesses.Holders", Displays.Authority(context:  context)),
                                new ("Accesses.Read", Displays.Read(context: context)),
                                new ("Accesses.Create", Displays.Create(context: context)),
                                new ("Accesses.Update", Displays.Update(context: context)),
                                new ("Accesses.Delete", Displays.Delete(context: context)),
                                new ("Accesses.SendMail", Displays.SendMail(context: context)),
                                new ("Accesses.Export", Displays.Export(context: context)),
                                new ("Accesses.Import", Displays.Import(context: context)),
                                new ("Accesses.ManageSite", Displays.ManageSite(context: context)),
                                new ("Accesses.ManagePermission", Displays.ManagePermission(context: context)),
                                new ("Accesses.Creator", Displays.Creator(context: context)),
                                new ("Accesses.Updator", Displays.Updator(context: context)),
                                new ("Accesses.Manager", Displays.Manager(context: context)),
                                new ("Accesses.Owner", Displays.Owner(context: context)),
                            }),
                    };
                    return new List3TableHeader(labels);
                }

                public class Access
                {
                    public string Type;
                    public bool? Read;
                    public bool? Create;
                    public bool? Update;
                    public bool? Delete;
                    public bool? SendMail;
                    public bool? Export;
                    public bool? Import;
                    public bool? ManageSite;
                    public bool? ManagePermission;
                    public bool? Creator;
                    public bool? Updator;
                    public bool? Manager;
                    public bool? Owner;
                    public List<string> Holders;
                    public List<string> ChangedColumns;

                    internal static Access GetData(
                        Context context,
                        SiteSettings ss,
                        ColumnAccessControl control,
                        string type)
                    {
                        if (control == null) return null;
                        var dst = new Access();
                        dst.ChangedColumns = new();
                        dst.Type = type switch
                        {
                            "Create" => Displays.Create(context: context),
                            "Read" => Displays.Read(context: context),
                            "Update" => Displays.Update(context: context),
                            _ => ""
                        };
                        dst.Read = (control.Type & Permissions.Types.Read) > 0;
                        if (dst.Read == true) dst.ChangedColumns.Add("Read");
                        dst.Create = (control.Type & Permissions.Types.Create) > 0;
                        if (dst.Create == true) dst.ChangedColumns.Add("Create");
                        dst.Update = (control.Type & Permissions.Types.Update) > 0;
                        if (dst.Update == true) dst.ChangedColumns.Add("Update");
                        dst.Delete = (control.Type & Permissions.Types.Delete) > 0;
                        if (dst.Delete == true) dst.ChangedColumns.Add("Delete");
                        dst.SendMail = (control.Type & Permissions.Types.SendMail) > 0;
                        if (dst.SendMail == true) dst.ChangedColumns.Add("SendMail");
                        dst.Export = (control.Type & Permissions.Types.Export) > 0;
                        if (dst.Export == true) dst.ChangedColumns.Add("Export");
                        dst.Import = (control.Type & Permissions.Types.Import) > 0;
                        if (dst.Import == true) dst.ChangedColumns.Add("Import");
                        dst.ManageSite = (control.Type & Permissions.Types.ManageSite) > 0;
                        if (dst.ManageSite == true) dst.ChangedColumns.Add("ManageSite");
                        dst.ManagePermission = (control.Type & Permissions.Types.ManagePermission) > 0;
                        if (dst.ManagePermission == true) dst.ChangedColumns.Add("ManagePermission");
                        if (type != "Create")
                        {
                            dst.Creator = control.RecordUsers?.Contains("Creator") ?? false;
                            if (dst.Creator == true) dst.ChangedColumns.Add("Creator");
                            dst.Updator = control.RecordUsers?.Contains("Updator") ?? false;
                            if (dst.Updator == true) dst.ChangedColumns.Add("Updator");
                            dst.Manager = control.RecordUsers?.Contains("Manager") ?? false;
                            if (dst.Manager == true) dst.ChangedColumns.Add("Manager");
                            dst.Owner = control.RecordUsers?.Contains("Owner") ?? false;
                            if (dst.Owner == true) dst.ChangedColumns.Add("Owner");
                        }
                        dst.Holders = control.GetPermissions(ss: ss)
                            .Select(v => v.ControlData(context: context, ss: ss, withType: false).Text)
                            .ToList();
                        dst.ChangedColumns.Add("Holders");
                        return dst;
                    }
                }

                internal static void SetData(
                    ColumnAccessColumn dst,
                    Context context,
                    string columnName,
                    ColumnAccessControl create,
                    ColumnAccessControl read,
                    ColumnAccessControl update,
                    SiteSettings ss)
                {
                    dst.ColumnName = columnName;
                    dst.Accesses = new List<Access>()
                    {
                        Access.GetData(context: context, ss: ss, control: create, type: "Create"),
                        Access.GetData(context: context, ss: ss, control: read, type: "Read"),
                        Access.GetData(context: context, ss: ss, control: update, type: "Update")
                    }
                        .Where(v => v != null)
                        .ToList();
                }

                internal static void SetChangeColumnName(
                    ColumnAccessColumn dst,
                    Context context,
                    string columnName,
                    ColumnAccessControl create,
                    ColumnAccessControl read,
                    ColumnAccessControl update,
                    SiteSettings ss)
                {
                    // 差分抽出しない
                }
            }
        }
    }
}
