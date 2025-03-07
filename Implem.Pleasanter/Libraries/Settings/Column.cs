using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Column
    {
        public enum Types
        {
            Normal,
            Dept,
            Group,
            User
        }

        public enum ViewerSwitchingTypes
        {
            Auto = 1,
            Manual = 2,
            Disabled = 3
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum SearchTypes : int
        {
            PartialMatch = 1,
            ExactMatch = 2,
            ForwardMatch = 3,
            PartialMatchMultiple = 11,
            ExactMatchMultiple = 12,
            ForwardMatchMultiple = 13
        }

        public enum FullTextTypes : int
        {
            None = 0,
            DisplayName = 1,
            Value = 2,
            ValueAndDisplayName = 3
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum AutoNumberingResetTypes : int
        {
            None = 0,
            Year = 10,
            Month = 20,
            Day = 30,
            String = 90
        }

        public enum OutputTypes
        {
            DisplayValue,
            Value
        }

        public string Id;
        public string ColumnName;
        public string LabelText;
        public string GridLabelText;
        public string Description;
        public string InputGuide;
        public string ChoicesText;
        public bool? UseSearch;
        public bool? MultipleSelections;
        public bool? NotInsertBlankChoice;
        public string DefaultInput;
        public bool? ImportKey;
        public bool? Anchor;
        public bool? OpenAnchorNewTab;
        public string AnchorFormat;
        public string GridFormat;
        public string EditorFormat;
        public string ExportFormat;
        public string ControlType;
        public string ChoicesControlType;
        public string Format;
        public bool? NoWrap;
        public bool? Hide;
        public string AutoNumberingFormat;
        public AutoNumberingResetTypes? AutoNumberingResetType;
        public int? AutoNumberingDefault;
        public int? AutoNumberingStep;
        public string ExtendedCellCss;
        public string ExtendedFieldCss;
        public string ExtendedControlCss;
        public string Section;
        public string GridDesign;
        public bool? ValidateRequired;
        public bool? ValidateNumber;
        public bool? ValidateDate;
        public bool? ValidateEmail;
        public decimal? MaxLength;
        public string ValidateEqualTo;
        public int? ValidateMaxLength;
        public string ClientRegexValidation;
        public string ServerRegexValidation;
        public string RegexValidationMessage;
        public string ExtendedHtmlBeforeField;
        public string ExtendedHtmlBeforeLabel;
        public string ExtendedHtmlBetweenLabelAndControl;
        public string ExtendedHtmlAfterControl;
        public string ExtendedHtmlAfterField;
        public int? DecimalPlaces;
        public bool? Nullable;
        public string Unit;
        public SiteSettings.RoundingTypes? RoundingType;
        public decimal? Min;
        public decimal? Max;
        public decimal? Step;
        public decimal? DefaultMinValue;
        public decimal? DefaultMaxValue;
        public bool? NoDuplication;
        public string MessageWhenDuplicated;
        public bool? CopyByDefault;
        public bool? EditorReadOnly;
        public bool? AutoPostBack;
        public string ColumnsReturnedWhenAutomaticPostback;
        public bool? AllowDeleteAttachments;
        public bool? NotDeleteExistHistory;
        public bool? AllowImage;
        public bool? AllowBulkUpdate;
        public string FieldCss;
        public ViewerSwitchingTypes? ViewerSwitchingType;
        public SiteSettings.TextAlignTypes? TextAlign;
        public bool? Link;
        public ColumnUtilities.CheckFilterControlTypes? CheckFilterControlType;
        public decimal? NumFilterMin;
        public decimal? NumFilterMax;
        public decimal? NumFilterStep;
        public ColumnUtilities.DateFilterSetMode? DateFilterSetMode;
        public SearchTypes? SearchType;
        public FullTextTypes? FullTextType;
        public int? DateFilterMinSpan;
        public int? DateFilterMaxSpan;
        public bool? DateFilterFy;
        public bool? DateFilterHalf;
        public bool? DateFilterQuarter;
        public bool? DateFilterMonth;
        public bool? OverwriteSameFileName;
        public decimal? LimitQuantity;
        public decimal? LimitSize;
        public decimal? TotalLimitSize;
        public decimal? ThumbnailLimitSize;
        public decimal? LocalFolderLimitSize;
        public decimal? LocalFolderTotalLimitSize;
        public int? DateTimeStep;
        [NonSerialized]
        public bool LowSchemaVersion;
        [NonSerialized]
        public int? No;
        [NonSerialized]
        public bool Id_Ver;
        [NonSerialized]
        public string Size;
        [NonSerialized]
        public bool DefaultNotNull;
        [NonSerialized]
        public bool Required;
        [NonSerialized]
        public bool RecordedTime;
        [NonSerialized]
        public bool NotSelect;
        [NonSerialized]
        public bool NotUpdate;
        [NonSerialized]
        public bool GridColumn;
        [NonSerialized]
        public bool FilterColumn;
        [NonSerialized]
        public bool EditSelf;
        [NonSerialized]
        public bool EditorColumn;
        [NonSerialized]
        public bool NotEditorSettings;
        [NonSerialized]
        public bool TitleColumn;
        [NonSerialized]
        public bool LinkColumn;
        [NonSerialized]
        public bool HistoryColumn;
        [NonSerialized]
        public bool Export;
        [NonSerialized]
        public string LabelTextDefault;
        [NonSerialized]
        public string TypeName;
        [NonSerialized]
        public string TypeCs;
        [NonSerialized]
        public bool DbNullable;
        [NonSerialized]
        public string JoinTableName;
        [NonSerialized]
        public Types Type = Types.Normal;
        [NonSerialized]
        public bool Hash;
        [NonSerialized]
        public string StringFormat;
        [NonSerialized]
        public string UnitDefault;
        [NonSerialized]
        public int Width;
        [NonSerialized]
        public string ControlCss;
        [NonSerialized]
        public string GridStyle;
        [NonSerialized]
        public bool Aggregatable;
        [NonSerialized]
        public bool Computable;
        [NonSerialized]
        public bool? FloatClear;
        [NonSerialized]
        public int TotalCount;
        [NonSerialized]
        public Dictionary<string, Choice> ChoiceHash;
        [NonSerialized]
        public bool AddChoiceHashByServerScript;
        [NonSerialized]
        public ServerScriptModelColumn ServerScriptModelColumn;
        [NonSerialized]
        public bool StatusReadOnly;
        [NonSerialized]
        public Dictionary<string, Choice> LinkedTitleHash = new Dictionary<string, Choice>();
        [NonSerialized]
        public Dictionary<string, string> ChoiceValueHash;
        [NonSerialized]
        public bool? CanCreateCache;
        [NonSerialized]
        public bool? CanReadCache;
        [NonSerialized]
        public bool? CanUpdateCache;
        [NonSerialized]
        public SiteSettings SiteSettings;
        [NonSerialized]
        public string Name;
        [NonSerialized]
        public string TableAlias;
        [NonSerialized]
        public long SiteId;
        [NonSerialized]
        public bool Joined;
        [NonSerialized]
        public bool Linking;
        [NonSerialized]
        public string After;
        [NonSerialized]
        public bool SqlParam;
        [NonSerialized]
        public OutputTypes OutputType;
        [NonSerialized]
        public bool ExcludeMe;
        // compatibility
        public bool? GridVisible;
        public bool? FilterVisible;
        public bool? EditorVisible;
        public bool? TitleVisible;
        public bool? LinkVisible;
        public bool? HistoryVisible;
        // compatibility Version 1.001
        public string GridDateTime;
        public string ControlDateTime;
        // compatibility Version 1.005
        public string ControlFormat;
        public string BinaryStorageProvider;
        public bool? AddSource;
        [NonSerialized]
        public string RecordingFormat;

        public Column()
        {
        }

        public Column(string columnName)
        {
            ColumnName = columnName;
        }

        public bool HasChoices()
        {
            return ControlType == "ChoicesText" && !ChoicesText.IsNullOrEmpty();
        }

        public SearchTypes SearchTypeDefault()
        {
            return HasChoices() && MultipleSelections != true
                ? SearchTypes.ExactMatch
                : SearchTypes.PartialMatch;
        }

        public string ChoiceHashKey()
        {
            return $"{ChoicesText.Trim()}{UseSearch}";
        }

        public void SetChoiceHash(
            Context context,
            SiteSettings ss,
            Link link,
            string searchText = null,
            IEnumerable<string> selectedValues = null,
            Column parentColumn = null,
            List<long> parentIds = null,
            int offset = 0,
            bool search = false,
            bool searchFormat = false,
            bool setAllChoices = false,
            bool setChoices = true)
        {
            if (setChoices) ChoiceHash = new Dictionary<string, Choice>();
            ExcludeMe = link.ExcludeMe == true;
            link.SetChoiceHash(
                context: context,
                ss: ss,
                column: this,
                searchText: searchText,
                selectedValues: selectedValues,
                parentColumn: parentColumn,
                parentIds: parentIds,
                offset: offset,
                search: search,
                searchFormat: searchFormat,
                setAllChoices: setAllChoices,
                setChoices: setChoices);
        }

        public void SetChoiceHash(
            Context context,
            long siteId,
            Dictionary<string, List<string>> linkHash = null,
            List<string> searchIndexes = null,
            bool setAllChoices = false,
            bool setChoices = true)
        {
            if (setChoices) ChoiceHash = new Dictionary<string, Choice>();
            ChoicesText.SplitReturn()
                .Where(o => o.Trim() != string.Empty)
                .Select((o, i) => new { Line = o.Trim(), Index = i })
                .ForEach(data =>
                    SetChoiceHash(
                        context: context,
                        siteId: siteId,
                        linkHash: linkHash,
                        searchIndexes: searchIndexes,
                        index: data.Index,
                        line: data.Line,
                        setAllChoices: setAllChoices,
                        setChoices: setChoices));
            if (searchIndexes?.Any() == true && setChoices)
            {
                ChoiceHash = ChoiceHash.Take(Parameters.General.DropDownSearchPageSize)
                    .ToDictionary(o => o.Key, o => o.Value);
            }
        }

        private void SetChoiceHash(
            Context context,
            long siteId,
            Dictionary<string, List<string>> linkHash,
            List<string> searchIndexes,
            int index,
            string line,
            bool setAllChoices,
            bool setChoices)
        {
            switch (line)
            {
                case "[[Depts]]":
                    Type = Types.Dept;
                    if (setChoices)
                    {
                        SiteInfo.TenantCaches.Get(context.TenantId)?
                            .DeptHash
                            .Where(o => o.Value.TenantId == context.TenantId)
                            .Where(o => !o.Value.Disabled)
                            .Where(o => searchIndexes?.Any() != true ||
                                searchIndexes.All(p =>
                                    o.Key == p.ToInt() ||
                                    o.Value.Name.RegexLike(p).Any()))
                            .OrderBy(o => o.Value.Name)
                            .ForEach(o => AddToChoiceHash(
                                o.Key.ToString(),
                                o.Value.Name));
                    }
                    break;
                case "[[Groups]]":
                    Type = Types.Group;
                    if (setChoices)
                    {
                        SiteInfo.SiteGroups(context: context, siteId: siteId)
                            .ToDictionary(o => o, o => SiteInfo.Group(context.TenantId, o))
                            .Where(o => o.Value.TenantId == context.TenantId)
                            .Where(o => !o.Value.Disabled)
                            .Where(o => searchIndexes?.Any() != true ||
                                searchIndexes.All(p =>
                                    o.Key == p.ToInt() ||
                                    o.Value.Name.RegexLike(p).Any()))
                            .OrderBy(o => o.Value.Name)
                            .ForEach(o => AddToChoiceHash(
                                o.Key.ToString(),
                                o.Value.Name));
                    }
                    break;
                case "[[Groups*]]":
                    Type = Types.Group;
                    if (setChoices)
                    {
                        SiteInfo.TenantCaches.Get(context.TenantId)?
                            .GroupHash
                            .Where(o => o.Value.TenantId == context.TenantId)
                            .Where(o => !o.Value.Disabled)
                            .Where(o => searchIndexes?.Any() != true ||
                                searchIndexes.All(p =>
                                    o.Key == p.ToInt() ||
                                    o.Value.Name.RegexLike(p).Any()))
                            .OrderBy(o => o.Value.Name)
                            .ForEach(o => AddToChoiceHash(
                                o.Key.ToString(),
                                o.Value.Name));
                    }
                    break;
                case "[[TimeZones]]":
                    if (setChoices)
                    {
                        TimeZoneInfo.GetSystemTimeZones()
                            .ForEach(o => AddToChoiceHash(
                                o.Id,
                                o?.StandardName ?? string.Empty));
                    }
                    break;
                default:
                    if (line.RegexExists(@"^\[\[Users.*\]\]$"))
                    {
                        Type = Types.User;
                        if (setChoices)
                        {
                            if (UseSearch != true || searchIndexes != null || setAllChoices)
                            {
                                AddUsersToChoiceHash(
                                    context: context,
                                    siteId: siteId,
                                    settings: line,
                                    searchIndexes: searchIndexes,
                                    setAllChoices: setAllChoices);
                            }
                        }
                    }
                    else if (Linked(context: context))
                    {
                        if (setChoices)
                        {
                            var key = "[[" + new Link(
                                columnName: ColumnName,
                                settings: line).SiteId + "]]";
                            if (linkHash != null && linkHash.ContainsKey(key))
                            {
                                if (Linked(
                                    context: context,
                                    withoutWiki: true))
                                {
                                    linkHash.Get(key)?
                                        .ToDictionary(
                                            o => o.Split_1st(),
                                            o => o.Contains(",")
                                                ? o.Substring(o.IndexOf(",") + 1)
                                                : o)
                                        .ForEach(o =>
                                            AddToChoiceHash(o.Key, o.Value));
                                }
                                else
                                {
                                    linkHash.Get(key)?
                                        .Select(o => new
                                        {
                                            Value = o.Split_1st(),
                                            Text = o.Split_2nd(),
                                            TextMini = o.Split_3rd(),
                                            CssClass = o.Split_4th()
                                        })
                                        .ForEach(o =>
                                            AddToChoiceHash(
                                                value: o.Value,
                                                text: o.Text,
                                                textMini: o.TextMini,
                                                cssClass: o.CssClass));
                                }
                            }
                        }
                    }
                    else if (TypeName != "bit")
                    {
                        if (setChoices)
                        {
                            if (searchIndexes == null ||
                                searchIndexes.All(o => line.RegexLike(o).Any()))
                            {
                                AddToChoiceHash(line);
                            }
                        }
                    }
                    else
                    {
                        if (setChoices)
                        {
                            AddToChoiceHash((index == 0).ToOneOrZeroString(), line);
                        }
                    }
                    break;
            }
        }

        public void AddUsersToChoiceHash(
            Context context,
            long siteId,
            string settings,
            IEnumerable<string> searchIndexes,
            bool setAllChoices)
        {
            IEnumerable<int> users = null;
            var showDeptName = false;
            settings
                ?.RegexFirst(@"(?<=\[\[).+(?=\]\])")
                ?.Split(',')
                .Select((o, i) => new { Index = i, Setting = o })
                .ForEach(data =>
                {
                    if (data.Index == 0)
                    {
                        users = data.Setting == "Users*"
                            ? SiteInfo.TenantCaches.Get(context.TenantId)
                                ?.UserHash
                                .Where(o => o.Value.TenantId == context.TenantId)
                                .Select(o => o.Value.Id)
                            : SiteInfo.SiteUsers(context: context, siteId: siteId);
                    }
                    else
                    {
                        switch (data.Setting)
                        {
                            case "ShowDeptName":
                                showDeptName = true;
                                break;
                        }
                    }
                });
            var take = setAllChoices
                ? int.MaxValue
                : Parameters.General.DropDownSearchPageSize;
            users
                ?.Select(userId => SiteInfo.User(
                    context: context,
                    userId: userId))
                .Where(user => !user.Disabled)
                .Where(user => searchIndexes?.Any() != true
                    || searchIndexes.All(p => " ".JoinParam(
                        user.UserCode,
                        user.Name,
                        user.LoginId,
                        user.Dept.Code,
                        user.Dept.Name).RegexLike(p).Any()))
                .Take(take)
                .Select(user => new
                {
                    user.Id,
                    Name = SiteInfo.UserName(
                        context: context,
                        userId: user.Id,
                        showDeptName: showDeptName)
                })
                .OrderBy(user => user.Name)
                .ForEach(user => AddToChoiceHash(
                    user.Id.ToString(),
                    user.Name));
        }

        private void AddToChoiceHash(
            string value,
            string text,
            string textMini = null,
            string cssClass = null)
        {
            if (!ChoiceHash.Keys.Contains(value))
            {
                ChoiceHash.Add(value, new Choice(
                    value: value,
                    text: text,
                    textMini: textMini,
                    cssClass: cssClass));
            }
        }

        private void AddToChoiceHash(string line)
        {
            var value = line.Split(',')._1st();
            if (!ChoiceHash.Keys.Contains(value))
            {
                ChoiceHash.Add(value, new Choice(line));
            }
        }

        public void AddToChoiceHash(Context context, string value)
        {
            if (!value.IsNullOrEmpty()
                && ChoiceHash?.ContainsKey(value) == false)
            {
                switch (Type)
                {
                    case Types.Normal:
                        if (Linked(context: context)
                            && SiteSettings?.Links
                                .Where(o => o.SiteId > 0)
                                .Where(o => o.ColumnName == ColumnName)
                                .All(o => Permissions.CanRead(
	                                context: context,
	                                siteId: o.SiteId)) == true)
                        {
                            var title = SiteSettings.LinkedItemTitle(
                                context: context,
                                referenceId: value.ToLong(),
                                siteIdList: SiteSettings.Links
                                    .Where(o => o.SiteId > 0)
                                    .Select(o => o.SiteId)
                                    .ToList());
                            ChoiceHash.AddIfNotConainsKey(
                                value,
                                new Choice(
                                    choice: title ?? "? " + value,
                                    raw: true,
                                    value: value));
                        }
                        break;
                    default:
                        ChoiceHash.AddIfNotConainsKey(
                            value,
                            new Choice(
                                value: value,
                                text: SiteInfo.Name(
                                    context: context,
                                    id: value.ToInt(),
                                    type: Type)));
                        break;
                }
            }
        }

        public Dictionary<string, ControlData> EditChoices(
            Context context,
            bool insertBlank = false,
            bool checkBlankInSelection = false,
            bool addNotSet = false,
            bool own = false,
            View view = null,
            int limit = 0)
        {
            var hash = new Dictionary<string, ControlData>();
            var blank = Type == Types.User
                ? SiteInfo.AnonymousId.ToString()
                : TypeName == "int"
                    ? "0"
                    : string.Empty;
            if (!HasChoices()) return hash;
            var selected = view?
                .ColumnFilter(ColumnName)?
                .Deserialize<List<string>>();
            if (addNotSet && NotRequiredOrUser())
            {
                hash.Add("\t", new ControlData(Displays.NotSet(context: context)));
            }
            if (own)
            {
                switch (Type)
                {
                    case Types.Dept:
                    case Types.User:
                        hash.Add("Own", new ControlData(Displays.Own(context: context)));
                        break;
                    default:
                        break;
                }
            }
            if (insertBlank && CanEmpty())
            {
                if (checkBlankInSelection == false
                    || selected?.Any() != true
                    || selected.Contains("\t"))
                {
                    hash.Add(blank, new ControlData(string.Empty));
                }
            }
            if (LinkedWithNewSet())
            {
                AddSource = true;
            }
            selected = selected?
                .Select(o => o == "\t" ? blank : o)
                .ToList();
            ChoiceHash?.Values
                .Where(o => !hash.ContainsKey(o.Value))
                .GroupBy(o => o.Value)
                .Select(o => o.FirstOrDefault())
                .Where(o => selected?.Any() != true || selected.Contains(o.Value))
                .Take(limit > 0 ? limit + 1 : int.MaxValue)
                .ForEach(choice =>
                    hash.Add(
                        choice.Value,
                        new ControlData(
                            text: choice.Text,
                            css: choice.CssClass,
                            style: choice.Style)));
            return hash;
        }

        private bool CanEmpty()
        {
            return !Required && ValidateRequired != true;
        }

        private bool NotRequiredOrUser()
        {
            return !Required || Type == Types.User;
        }

        public Choice Choice(string selectedValue, string nullCase = null)
        {
            return selectedValue != null
                && ChoiceHash != null
                && ChoiceHash.ContainsKey(selectedValue)
                    ? ChoiceHash[selectedValue]
                    : new Choice(nullCase, raw: true);
        }

        public Choice LinkedTitleChoice(Context context, string selectedValues, string nullCase = null)
        {
            var ids = (MultipleSelections == true
                ? selectedValues?.Deserialize<List<long>>()
                : selectedValues?.ToLong().ToSingleList())
                    ?.Where(o => o > 0)
                    .ToList();
            if (ids?.Any() == true && Linked(
                context: context,
                withoutWiki: true))
            {
                if (LinkedTitleHash.ContainsKey(selectedValues))
                {
                    return LinkedTitleHash[selectedValues];
                }
                var choice = new Choice(
                    choice: LinkedTitle(
                        context: context,
                        ids: ids),
                    raw: true,
                    value: selectedValues);
                LinkedTitleHash.AddIfNotConainsKey(selectedValues, choice);
                return LinkedTitleHash[selectedValues];
            }
            else if (ChoiceHash?.ContainsKey(selectedValues) == true)
            {
                return ChoiceHash[selectedValues];
            }
            return new Choice(nullCase, raw: true);
        }

        private string LinkedTitle(Context context, List<long> ids)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectItems(
                    column: Rds.ItemsColumn().Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "\"Sites\"",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "\"Items\".\"SiteId\"=\"Sites\".\"SiteId\"")),
                    where: Rds.ItemsWhere()
                        .SiteId_In(SiteSettings.Links
                            .Where(o => o.SiteId > 0)
                            .Where(o => o.ColumnName == ColumnName)
                            .Select(o => o.SiteId)
                            .ToList())
                        .ReferenceId_In(ids)
                        .CanRead(
                            context: context,
                            idColumnBracket: "\"Items\".\"ReferenceId\"")))
                                .AsEnumerable()
                                .Select(dataRow => dataRow.String("Title"))
                                .Join(", ");
        }

        public List<string> ChoiceParts(
            Context context,
            string selectedValues,
            ExportColumn.Types? type)
        {
            var values = (MultipleSelections == true
                ? selectedValues.Deserialize<List<string>>()
                : null)
                    ?? selectedValues.ToSingleList();
            return values.Select(value => ChoicePart(
                context: context,
                value: value,
                type: type))
                    .ToList();
        }

        private string ChoicePart(
            Context context,
            string value,
            ExportColumn.Types? type)
        {
            AddToChoiceHash(
                context: context,
                value: value);
            var choice = Choice(value, nullCase: value);
            switch (type)
            {
                case ExportColumn.Types.Value: return choice.Value;
                case ExportColumn.Types.Text: return choice.Text;
                case ExportColumn.Types.TextMini: return choice.TextMini;
                default: return choice.Text;
            }
        }

        public string RecordingData(Context context, string value, long siteId)
        {
            var userHash = SiteInfo.TenantCaches.Get(context.TenantId)?.UserHash;
            var recordingData = value;
            if (TypeCs == "Comments")
            {
                return new Comments().Prepend(
                    context: context,
                    ss: SiteSettings,
                    body: value)
                        .ToJson();
            }
            else if (TypeName == "datetime")
            {
                return value?.ToDateTime(format: RecordingFormat).ToUniversal(context: context).ToString()
                    ?? string.Empty;
            }
            else if (HasChoices())
            {
                if (ChoiceValueHash == null)
                {
                    ChoiceValueHash = ChoiceHash
                        .GroupBy(o => o.Value.Text)
                        .Select(o => o.First())
                        .ToDictionary(o => o.Value.Text, o => o.Key);
                }
                recordingData = MultipleSelections == true
                    ? value.Deserialize<List<string>>()
                        ?.Select(o => ChoiceValueHash.Get(o) ?? o)
                        .Where(o => !o.IsNullOrEmpty())
                        .ToJson()
                    : ChoiceValueHash.Get(value) ?? value;
            }
            return recordingData ?? string.Empty;
        }

        public string ExpressionColumnName()
        {
            switch (OutputType)
            {
                case OutputTypes.DisplayValue:
                    return ColumnName;
                default:
                    return "@" + ColumnName;
            }
        }

        public string Display(
            Context context, decimal? value, bool unit = false, bool format = true)
        {
            if (value == null && Nullable == true)
            {
                return string.Empty;
            }
            try
            {
                var ret = (!Format.IsNullOrEmpty() && format
                    ? value.ToDecimal().ToString(
                        Format + (Format == "C" || Format == "N"
                            ? DecimalPlaces.ToString()
                            : string.Empty), context.CultureInfoCurrency(context.Language))
                    : DecimalPlaces.ToInt() == 0
                        ? value.ToDecimal().ToString("0", "0")
                        : DisplayValue(value.ToDecimal()))
                            + (unit ? Unit : string.Empty);
                switch (context.Language)
                {
                    case "ja":
                        return ret.Replace("￥", "¥");
                    default:
                        return ret;
                }
            }
            catch (FormatException)
            {
                try
                {
                    return value.ToLong().ToString(Format);
                }
                catch (FormatException)
                {
                    return Nullable == true
                        ? string.Empty
                        : "0";
                }
            }
        }

        public string DisplayValue(decimal value)
        {
            return value.ToString("0", "0." + new string('0', DecimalPlaces.ToInt()))
                .ToDecimal()
                .TrimEndZero();
        }

        public string Display(Context context, SiteSettings ss, decimal value, bool format = true)
        {
            return Display(
                context: context,
                value: value,
                format: format)
                    + (GetEditorReadOnly()
                        || Permissions.ColumnPermissionType(
                            context: context,
                            ss: ss,
                            column: this,
                            baseModel: null) != Permissions.ColumnPermissionTypes.Update
                                ? Unit
                                : string.Empty);
        }

        public string DisplayGrid(Context context, DateTime value)
        {
            return value.Display(
                context: context,
                format: GridFormat);
        }

        public string DisplayControl(Context context, DateTime value)
        {
            return value.Display(
                context: context,
                format: EditorFormat);
        }

        public bool GetEditorReadOnly()
        {
            var readOnly = ServerScriptModelColumn?.GetReadOnly();
            if (readOnly != null)
            {
                return readOnly == true;
            }
            return StatusReadOnly || EditorReadOnly == true;
        }

        public bool GetHide()
        {
            var hide = ServerScriptModelColumn?.GetHide();
            if (hide != null)
            {
                return hide == true;
            }
            return Hide == true;
        }

        public bool GetValidateRequired()
        {
            var validateRequired = ServerScriptModelColumn?.GetValidateRequired();
            if (validateRequired != null)
            {
                return validateRequired == true;
            }
            return ValidateRequired == true;
        }

        public decimal Round(decimal value)
        {
            switch (RoundingType)
            {
                case SiteSettings.RoundingTypes.AwayFromZero:
                    return Math.Round(value, DecimalPlaces.ToInt(), MidpointRounding.AwayFromZero);
                case SiteSettings.RoundingTypes.Ceiling:
                    return Math.Ceiling(Adjustment(value)) / Math.Pow(10, DecimalPlaces.ToInt()).ToInt();
                case SiteSettings.RoundingTypes.Truncate:
                    return Math.Truncate(Adjustment(value)) / Math.Pow(10, DecimalPlaces.ToInt()).ToInt();
                case SiteSettings.RoundingTypes.Floor:
                    return Math.Floor(Adjustment(value)) / Math.Pow(10, DecimalPlaces.ToInt()).ToInt();
                case SiteSettings.RoundingTypes.ToEven:
                    return Math.Round(value, DecimalPlaces.ToInt(), MidpointRounding.ToEven);
                default:
                    return value;
            }
        }

        public decimal Adjustment(decimal value)
        {
            return value * Math.Pow(10, DecimalPlaces.ToInt()).ToInt();
        }

        public string DateTimeFormat(Context context)
        {
            switch (EditorFormat)
            {
                case "Ymdhm":
                    return Displays.YmdhmDatePickerFormat(context: context);
                case "Ymdhms":
                    return Displays.YmdhmsDatePickerFormat(context: context);
                default:
                    return Displays.YmdDatePickerFormat(context: context);
            }
        }

        public bool DateTimepicker()
        {
            switch (EditorFormat)
            {
                case "Ymdhm":
                case "Ymdhms":
                    return true;
                default:
                    return false;
            }
        }

        public decimal MinNumber()
        {
            return Math.Max(
                Min ?? DefaultMinValue ?? (MaxNumberFromSize() * -1),
                MaxNumberFromSize() * -1);
        }

        public decimal MaxNumber()
        {
            return Math.Min(
                Max ?? DefaultMaxValue ?? MaxNumberFromSize(),
                MaxNumberFromSize());
        }

        private decimal MaxNumberFromSize()
        {
            var length = Size.Split_1st().ToInt() - Size.Split_2nd().ToInt();
            return length > 0
                ? new string('9', length).ToDecimal()
                : 0;
        }

        public DateTime DefaultTime(Context context)
        {
            return DefaultInput.IsNullOrEmpty()
                ? 0.ToDateTime()
                : EditorFormat == "Ymd"
                    ? DateTime.Now.ToLocal(context: context).Date.AddDays(DefaultInput.ToInt()).ToUniversal(context: context)
                    : DateTime.Now.AddDays(DefaultInput.ToInt());
        }

        public string GetDefaultInput(Context context)
        {
            switch (DefaultInput)
            {
                case "[[Self]]":
                    switch (Type)
                    {
                        case Types.Dept:
                            return MultipleSelections == true
                                ? context.DeptId.ToString().ToSingleList().ToJson()
                                : context.DeptId.ToString();
                        case Types.Group:
                            return MultipleSelections == true
                                ? context.Groups.ToJson()
                                : DefaultGroupId(context: context).ToString();
                        case Types.User:
                            return MultipleSelections == true
                                ? context.UserId.ToString().ToSingleList().ToJson()
                                : context.UserId.ToString();
                    }
                    break;
            }
            return DefaultInput;
        }

        public string ResponseValOptions(ServerScriptModelColumn serverScriptModelColumn)
        {
            return new
            {
                Hide = Hide == true || serverScriptModelColumn?.Hide == true
            }.ToJson();
        }

        private int DefaultGroupId(Context context)
        {
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn().GroupId(),
                    where: Rds.GroupMembersWhere()
                        .Add(raw: Permissions.DeptOrUser(tableName: "GroupMembers"))))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Int("GroupId"))
                            .FirstOrDefault(groupId => ChoiceHash.ContainsKey(groupId.ToString()));
        }

        public bool OtherColumn()
        {
            switch (ColumnName)
            {
                case "Creator":
                case "Updator":
                case "CreatedTime":
                case "UpdatedTime":
                    return true;
                default:
                    return false;
            }
        }

        public bool AutoNumberingColumn()
        {
            return TypeName.CsTypeSummary() == Implem.Libraries.Utilities.Types.CsString
                && ColumnName != "Comments";
        }

        public SqlColumnCollection SqlColumnCollection()
        {
            var sql = new SqlColumnCollection();
            var tableName = Strings.CoalesceEmpty(JoinTableName, SiteSettings.ReferenceType);
            SqlColumnCollection(
                sql: sql,
                tableName: tableName);
            return sql;
        }

        public SqlColumnCollection SqlColumnWithUpdatedTimeCollection()
        {
            var sql = new SqlColumnCollection();
            var tableName = Strings.CoalesceEmpty(JoinTableName, SiteSettings.ReferenceType);
            SqlColumnCollection(
                sql: sql,
                tableName: tableName);
            sql.Add(
                columnBracket: "\"UpdatedTime\"",
                tableName: Joined
                    ? TableAlias
                    : tableName,
                columnName: "UpdatedTime",
                    _as: Joined
                        ? TableAlias + ",UpdatedTime"
                        : null);
            return sql;
        }

        private void SqlColumnCollection(SqlColumnCollection sql, string tableName)
        {
            SelectColumns(
                sql: sql,
                tableName: tableName,
                columnName: Name,
                path: Joined
                    ? TableAlias
                    : tableName,
                _as: Joined
                    ? ColumnName
                    : null);
        }

        public SqlStatement IfDuplicatedStatement(
            SqlParamCollection param, long siteId, long referenceId)
        {
            return new SqlStatement(
                Def.Sql.IfDuplicated.Params(
                    SiteSettings.ReferenceType,
                    siteId,
                    Rds.IdColumn(SiteSettings.ReferenceType),
                    referenceId,
                    ColumnName),
                param)
            { IfDuplicated = true };
        }

        public string IsNullValue(Context context)
        {
            if (!DbNullable) return null;
            switch (TypeName.CsTypeSummary())
            {
                case Implem.Libraries.Utilities.Types.CsBool:
                    return context.Sqls.FalseString;
                case Implem.Libraries.Utilities.Types.CsNumeric:
                    return Nullable == true
                        ? null
                        : "0";
                case Implem.Libraries.Utilities.Types.CsString:
                    return "''";
                default:
                    return null;
            }
        }

        public object DefaultNotNullValue()
        {
            switch (TypeName)
            {
                case "bit":
                    return false;
                case "int":
                case "long":
                case "decimal":
                case "float":
                    return 0;
                case "datetime":
                    return null;
                case "nchar":
                case "nvarchar":
                    return string.Empty;
                default:
                    return null;
            }
        }

        public string TableName()
        {
            return Strings.CoalesceEmpty(TableAlias, JoinTableName, SiteSettings?.ReferenceType);
        }

        public string TableItemTitleCases(Context context)
        {
            switch (context.Action)
            {
                // Upsertの場合にはItemTitleではなくModelのTitleで検索を行う。
                // 結合されたタイトルをAPIリクエストのTitleにセットすると結合されたタイトルでタイトルカラムの更新が行われるため、これを防ぐ。
                case "upsert":
                    return TableName();
                default:
                    switch (Name)
                    {
                        case "Title":
                            var tableName = Strings.CoalesceEmpty(TableAlias, JoinTableName, SiteSettings?.ReferenceType);
                            return tableName + "_Items";
                        default:
                            return TableName();
                    }
            }
        }

        public bool Linked(Context context, bool withoutWiki = false)
        {
            return SiteSettings?.Links?
                .Where(o => o.SiteId > 0)
                .Any(o => o.ColumnName == Name
                    && (!withoutWiki
                        || SiteInfo.Sites(context: context)
                            .Get(o.SiteId)
                            ?.String("ReferenceType") != "Wikis")) == true;
        }

        public bool LinkedWithNewSet()
        {
            return SiteSettings?.Links?.Any(o => o.ColumnName == Name && o.AddSource == true) == true;
        }

        public string ConvertIfUserColumn(DataRow dataRow)
        {
            var value = dataRow.String(ColumnName);
            return Type == Types.User && value.IsNullOrEmpty()
                ? SiteInfo.AnonymousId.ToString()
                : value;
        }

        public long? RelatingSrcId()
        {
            var link = ChoicesText.SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o.RegexExists(@"^\[\[.+\]\]$"))
                .Select(settings => new Link(
                    columnName: ColumnName,
                    settings: settings))
                .FirstOrDefault(o => o.SiteId != 0);
            return link?.SiteId;
        }

        public string CellCss(string css = null)
        {
            return new List<string>()
            {
                css,
                ExtendedCellCss,
                TextAlign switch
                {
                    SiteSettings.TextAlignTypes.Right => " right-align",
                    SiteSettings.TextAlignTypes.Center => " center-align",
                    _ => string.Empty
                }
        }

                .Select(o => o?.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .Join(" ");
        }

        public bool BlankValue(string value)
        {
            switch (TypeName)
            {
                case "bit":
                    return !value.ToBool();
                case "decimal":
                    return Nullable == true
                        ? value.IsNullOrEmpty()
                        : value.IsNullOrEmpty() || value == "0";
                case "datetime":
                    return value?.ToDateTime().InRange() != true;
                default:
                    return value.IsNullOrEmpty();
            }
        }

        public bool CanCreate(
            Context context,
            SiteSettings ss,
            List<string> mine,
            bool noCache = false)
        {
            if (noCache)
            {
                var columnAccessControl = ss.CreateColumnAccessControls?.FirstOrDefault(o => o.ColumnName == ColumnName)
                    ?? new ColumnAccessControl(ss, this, "Create");
                return columnAccessControl.Allowed(
                    context: context,
                    ss: ss,
                    mine: mine);
            }
            else if (CanCreateCache == null)
            {
                var columnAccessControl = ss.CreateColumnAccessControls?.FirstOrDefault(o => o.ColumnName == ColumnName)
                    ?? new ColumnAccessControl(ss, this, "Create");
                CanCreateCache = columnAccessControl.Allowed(
                    context: context,
                    ss: ss,
                    mine: mine);
                ss.ColumnAccessControlCaches.AddIfNotConainsKey(ColumnName, this);
            }
            return CanCreateCache == true;
        }

        public bool CanRead(
            Context context,
            SiteSettings ss,
            List<string> mine,
            bool noCache = false)
        {
            if (noCache)
            {
                var columnAccessControl = ss.ReadColumnAccessControls?.FirstOrDefault(o => o.ColumnName == ColumnName)
                    ?? new ColumnAccessControl(ss, this, "Read");
                return columnAccessControl.Allowed(
                    context: context,
                    ss: ss,
                    mine: mine);
            }
            else if (CanReadCache == null)
            {
                var columnAccessControl = ss.ReadColumnAccessControls?.FirstOrDefault(o => o.ColumnName == ColumnName)
                    ?? new ColumnAccessControl(ss, this, "Read");
                CanReadCache = columnAccessControl.Allowed(
                    context: context,
                    ss: ss,
                    mine: mine);
                ss.ColumnAccessControlCaches.AddIfNotConainsKey(ColumnName, this);
            }
            return CanReadCache == true;
        }

        public bool CanUpdate(
            Context context,
            SiteSettings ss,
            List<string> mine,
            bool noCache = false)
        {
            if (noCache)
            {
                var columnAccessControl = ss.UpdateColumnAccessControls?.FirstOrDefault(o => o.ColumnName == ColumnName)
                    ?? new ColumnAccessControl(ss, this, "Update");
                return columnAccessControl.Allowed(
                    context: context,
                    ss: ss,
                    mine: mine);
            }
            else if (CanUpdateCache == null)
            {
                var columnAccessControl = ss.UpdateColumnAccessControls?.FirstOrDefault(o => o.ColumnName == ColumnName)
                    ?? new ColumnAccessControl(ss, this, "Update");
                CanUpdateCache = columnAccessControl.Allowed(
                    context: context,
                    ss: ss,
                    mine: mine);
                ss.ColumnAccessControlCaches.AddIfNotConainsKey(ColumnName, this);
            }
            return CanUpdateCache == true;
        }

        public bool CanEdit(
            Context context,
            SiteSettings ss,
            List<string> mine,
            bool skipCanReadCheck = false,
            bool noCache = true)
        {
            if (context.IsNew)
            {
                return CanCreate(
                    context: context,
                    ss: ss,
                    mine: mine,
                    noCache: noCache);
            }
            else
            {
                if (!skipCanReadCheck)
                {
                    var canRead = CanRead(
                        context: context,
                        ss: ss,
                        mine: mine,
                        noCache: noCache);
                    if (!canRead) return false;
                }
                return CanUpdate(
                    context: context,
                    ss: ss,
                    mine: mine,
                    noCache: noCache);
            }
        }

        public DateTime ConvertDateTime(Context context, DateTime dt)
        {
            switch (Name)
            {
                case "CompletionTime":
                    dt = DateTimepicker()
                        ? dt
                        : dt.AddDays(1);
                    break;
            }
            return dt
                .ToDateTime()
                .ToUniversal(context: context);
        }

        public string ConvertDateTimeParam(
            Context context,
            DateTime dt,
            string format)
        {
            return ConvertDateTime(
                context: context,
                dt: dt)
                    .ToString(format);
        }

        private void SelectColumns(
            SqlColumnCollection sql,
            string tableName,
            string columnName,
            string path,
            string _as)
        {
            switch (tableName)
            {
                case "Depts":
                    switch (columnName)
                    {
                        case "TenantId":
                            sql.Depts_TenantId(tableName: path, _as: _as);
                            break;
                        case "DeptId":
                            sql.Depts_DeptId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Depts_Ver(tableName: path, _as: _as);
                            break;
                        case "DeptCode":
                            sql.Depts_DeptCode(tableName: path, _as: _as);
                            break;
                        case "Dept":
                            sql.Depts_Dept(tableName: path, _as: _as);
                            break;
                        case "DeptName":
                            sql.Depts_DeptName(tableName: path, _as: _as);
                            break;
                        case "Body":
                            sql.Depts_Body(tableName: path, _as: _as);
                            break;
                        case "Disabled":
                            sql.Depts_Disabled(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Depts_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Depts_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Depts_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Depts_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Depts_UpdatedTime(tableName: path, _as: _as);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
                case "Groups":
                    switch (columnName)
                    {
                        case "TenantId":
                            sql.Groups_TenantId(tableName: path, _as: _as);
                            break;
                        case "GroupId":
                            sql.Groups_GroupId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Groups_Ver(tableName: path, _as: _as);
                            break;
                        case "GroupName":
                            sql.Groups_GroupName(tableName: path, _as: _as);
                            break;
                        case "Body":
                            sql.Groups_Body(tableName: path, _as: _as);
                            break;
                        case "Disabled":
                            sql.Groups_Disabled(tableName: path, _as: _as);
                            break;
                        case "LdapSync":
                            sql.Groups_LdapSync(tableName: path, _as: _as);
                            break;
                        case "LdapGuid":
                            sql.Groups_LdapGuid(tableName: path, _as: _as);
                            break;
                        case "LdapSearchRoot":
                            sql.Groups_LdapSearchRoot(tableName: path, _as: _as);
                            break;
                        case "SynchronizedTime":
                            sql.Groups_SynchronizedTime(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Groups_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Groups_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Groups_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Groups_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Groups_UpdatedTime(tableName: path, _as: _as);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
                case "Registrations":
                    switch (columnName)
                    {
                        case "TenantId":
                            sql.Registrations_TenantId(tableName: path, _as: _as);
                            break;
                        case "RegistrationId":
                            sql.Registrations_RegistrationId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Registrations_Ver(tableName: path, _as: _as);
                            break;
                        case "MailAddress":
                            sql.Registrations_MailAddress(tableName: path, _as: _as);
                            break;
                        case "Invitee":
                            sql.Registrations_Invitee(tableName: path, _as: _as);
                            break;
                        case "InviteeName":
                            sql.Registrations_InviteeName(tableName: path, _as: _as);
                            break;
                        case "LoginId":
                            sql.Registrations_LoginId(tableName: path, _as: _as);
                            break;
                        case "Name":
                            sql.Registrations_Name(tableName: path, _as: _as);
                            break;
                        case "Password":
                            sql.Registrations_Password(tableName: path, _as: _as);
                            break;
                        case "Language":
                            sql.Registrations_Language(tableName: path, _as: _as);
                            break;
                        case "Passphrase":
                            sql.Registrations_Passphrase(tableName: path, _as: _as);
                            break;
                        case "Invitingflg":
                            sql.Registrations_Invitingflg(tableName: path, _as: _as);
                            break;
                        case "UserId":
                            sql.Registrations_UserId(tableName: path, _as: _as);
                            break;
                        case "DeptId":
                            sql.Registrations_DeptId(tableName: path, _as: _as);
                            break;
                        case "GroupId":
                            sql.Registrations_GroupId(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Registrations_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Registrations_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Registrations_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Registrations_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Registrations_UpdatedTime(tableName: path, _as: _as);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
                case "Sites":
                    switch (columnName)
                    {
                        case "TenantId":
                            sql.Sites_TenantId(tableName: path, _as: _as);
                            break;
                        case "SiteId":
                            sql.Sites_SiteId(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Sites_UpdatedTime(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Sites_Ver(tableName: path, _as: _as);
                            break;
                        case "Body":
                            sql.Sites_Body(tableName: path, _as: _as);
                            break;
                        case "SiteName":
                            sql.Sites_SiteName(tableName: path, _as: _as);
                            break;
                        case "SiteGroupName":
                            sql.Sites_SiteGroupName(tableName: path, _as: _as);
                            break;
                        case "GridGuide":
                            sql.Sites_GridGuide(tableName: path, _as: _as);
                            break;
                        case "EditorGuide":
                            sql.Sites_EditorGuide(tableName: path, _as: _as);
                            break;
                        case "CalendarGuide":
                            sql.Sites_CalendarGuide(tableName: path, _as: _as);
                            break;
                        case "CrosstabGuide":
                            sql.Sites_CrosstabGuide(tableName: path, _as: _as);
                            break;
                        case "GanttGuide":
                            sql.Sites_GanttGuide(tableName: path, _as: _as);
                            break;
                        case "BurnDownGuide":
                            sql.Sites_BurnDownGuide(tableName: path, _as: _as);
                            break;
                        case "TimeSeriesGuide":
                            sql.Sites_TimeSeriesGuide(tableName: path, _as: _as);
                            break;
                        case "AnalyGuide":
                            sql.Sites_AnalyGuide(tableName: path, _as: _as);
                            break;
                        case "KambanGuide":
                            sql.Sites_KambanGuide(tableName: path, _as: _as);
                            break;
                        case "ImageLibGuide":
                            sql.Sites_ImageLibGuide(tableName: path, _as: _as);
                            break;
                        case "ReferenceType":
                            sql.Sites_ReferenceType(tableName: path, _as: _as);
                            break;
                        case "ParentId":
                            sql.Sites_ParentId(tableName: path, _as: _as);
                            break;
                        case "InheritPermission":
                            sql.Sites_InheritPermission(tableName: path, _as: _as);
                            break;
                        case "SiteSettings":
                            sql.Sites_SiteSettings(tableName: path, _as: _as);
                            break;
                        case "Publish":
                            sql.Sites_Publish(tableName: path, _as: _as);
                            break;
                        case "DisableCrossSearch":
                            sql.Sites_DisableCrossSearch(tableName: path, _as: _as);
                            break;
                        case "LockedTime":
                            sql.Sites_LockedTime(tableName: path, _as: _as);
                            break;
                        case "LockedUser":
                            sql.Sites_LockedUser(tableName: path, _as: _as);
                            break;
                        case "ApiCountDate":
                            sql.Sites_ApiCountDate(tableName: path, _as: _as);
                            break;
                        case "ApiCount":
                            sql.Sites_ApiCount(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Sites_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Sites_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Sites_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Sites_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "TitleBody":
                            sql.Sites_Body(tableName: path, _as: Joined
                                ? path + ",Body"
                                : "Body");
                            goto case "Title";
                        case "Title":
                            sql
                                .Sites_Title(tableName: path, _as: _as)
                                .ItemTitle(
                                    tableName: path,
                                    _as: Joined
                                        ? path + ",ItemTitle"
                                        : "ItemTitle");
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
                case "SysLogs":
                    switch (columnName)
                    {
                        case "CreatedTime":
                            sql.SysLogs_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "SysLogId":
                            sql.SysLogs_SysLogId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.SysLogs_Ver(tableName: path, _as: _as);
                            break;
                        case "SysLogType":
                            sql.SysLogs_SysLogType(tableName: path, _as: _as);
                            break;
                        case "OnAzure":
                            sql.SysLogs_OnAzure(tableName: path, _as: _as);
                            break;
                        case "MachineName":
                            sql.SysLogs_MachineName(tableName: path, _as: _as);
                            break;
                        case "ServiceName":
                            sql.SysLogs_ServiceName(tableName: path, _as: _as);
                            break;
                        case "TenantName":
                            sql.SysLogs_TenantName(tableName: path, _as: _as);
                            break;
                        case "Application":
                            sql.SysLogs_Application(tableName: path, _as: _as);
                            break;
                        case "Class":
                            sql.SysLogs_Class(tableName: path, _as: _as);
                            break;
                        case "Method":
                            sql.SysLogs_Method(tableName: path, _as: _as);
                            break;
                        case "Api":
                            sql.SysLogs_Api(tableName: path, _as: _as);
                            break;
                        case "SiteId":
                            sql.SysLogs_SiteId(tableName: path, _as: _as);
                            break;
                        case "ReferenceId":
                            sql.SysLogs_ReferenceId(tableName: path, _as: _as);
                            break;
                        case "ReferenceType":
                            sql.SysLogs_ReferenceType(tableName: path, _as: _as);
                            break;
                        case "Status":
                            sql.SysLogs_Status(tableName: path, _as: _as);
                            break;
                        case "Description":
                            sql.SysLogs_Description(tableName: path, _as: _as);
                            break;
                        case "RequestData":
                            sql.SysLogs_RequestData(tableName: path, _as: _as);
                            break;
                        case "HttpMethod":
                            sql.SysLogs_HttpMethod(tableName: path, _as: _as);
                            break;
                        case "RequestSize":
                            sql.SysLogs_RequestSize(tableName: path, _as: _as);
                            break;
                        case "ResponseSize":
                            sql.SysLogs_ResponseSize(tableName: path, _as: _as);
                            break;
                        case "Elapsed":
                            sql.SysLogs_Elapsed(tableName: path, _as: _as);
                            break;
                        case "ApplicationAge":
                            sql.SysLogs_ApplicationAge(tableName: path, _as: _as);
                            break;
                        case "ApplicationRequestInterval":
                            sql.SysLogs_ApplicationRequestInterval(tableName: path, _as: _as);
                            break;
                        case "SessionAge":
                            sql.SysLogs_SessionAge(tableName: path, _as: _as);
                            break;
                        case "SessionRequestInterval":
                            sql.SysLogs_SessionRequestInterval(tableName: path, _as: _as);
                            break;
                        case "WorkingSet64":
                            sql.SysLogs_WorkingSet64(tableName: path, _as: _as);
                            break;
                        case "VirtualMemorySize64":
                            sql.SysLogs_VirtualMemorySize64(tableName: path, _as: _as);
                            break;
                        case "ProcessId":
                            sql.SysLogs_ProcessId(tableName: path, _as: _as);
                            break;
                        case "ProcessName":
                            sql.SysLogs_ProcessName(tableName: path, _as: _as);
                            break;
                        case "BasePriority":
                            sql.SysLogs_BasePriority(tableName: path, _as: _as);
                            break;
                        case "Url":
                            sql.SysLogs_Url(tableName: path, _as: _as);
                            break;
                        case "UrlReferer":
                            sql.SysLogs_UrlReferer(tableName: path, _as: _as);
                            break;
                        case "UserHostName":
                            sql.SysLogs_UserHostName(tableName: path, _as: _as);
                            break;
                        case "UserHostAddress":
                            sql.SysLogs_UserHostAddress(tableName: path, _as: _as);
                            break;
                        case "UserLanguage":
                            sql.SysLogs_UserLanguage(tableName: path, _as: _as);
                            break;
                        case "UserAgent":
                            sql.SysLogs_UserAgent(tableName: path, _as: _as);
                            break;
                        case "SessionGuid":
                            sql.SysLogs_SessionGuid(tableName: path, _as: _as);
                            break;
                        case "ErrMessage":
                            sql.SysLogs_ErrMessage(tableName: path, _as: _as);
                            break;
                        case "ErrStackTrace":
                            sql.SysLogs_ErrStackTrace(tableName: path, _as: _as);
                            break;
                        case "InDebug":
                            sql.SysLogs_InDebug(tableName: path, _as: _as);
                            break;
                        case "AssemblyVersion":
                            sql.SysLogs_AssemblyVersion(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.SysLogs_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.SysLogs_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.SysLogs_Updator(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.SysLogs_UpdatedTime(tableName: path, _as: _as);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
                case "Users":
                    switch (columnName)
                    {
                        case "TenantId":
                            sql.Users_TenantId(tableName: path, _as: _as);
                            break;
                        case "UserId":
                            sql.Users_UserId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Users_Ver(tableName: path, _as: _as);
                            break;
                        case "LoginId":
                            sql.Users_LoginId(tableName: path, _as: _as);
                            break;
                        case "GlobalId":
                            sql.Users_GlobalId(tableName: path, _as: _as);
                            break;
                        case "Name":
                            sql.Users_Name(tableName: path, _as: _as);
                            break;
                        case "UserCode":
                            sql.Users_UserCode(tableName: path, _as: _as);
                            break;
                        case "Password":
                            sql.Users_Password(tableName: path, _as: _as);
                            break;
                        case "LastName":
                            sql.Users_LastName(tableName: path, _as: _as);
                            break;
                        case "FirstName":
                            sql.Users_FirstName(tableName: path, _as: _as);
                            break;
                        case "Birthday":
                            sql.Users_Birthday(tableName: path, _as: _as);
                            break;
                        case "Gender":
                            sql.Users_Gender(tableName: path, _as: _as);
                            break;
                        case "Language":
                            sql.Users_Language(tableName: path, _as: _as);
                            break;
                        case "TimeZone":
                            sql.Users_TimeZone(tableName: path, _as: _as);
                            break;
                        case "TimeZoneInfo":
                            sql.Users_TimeZoneInfo(tableName: path, _as: _as);
                            break;
                        case "DeptCode":
                            sql.Users_DeptCode(tableName: path, _as: _as);
                            break;
                        case "DeptId":
                            sql.Users_DeptId(tableName: path, _as: _as);
                            break;
                        case "Dept":
                            sql.Users_Dept(tableName: path, _as: _as);
                            break;
                        case "Manager":
                            sql.Users_Manager(tableName: path, _as: _as);
                            break;
                        case "Theme":
                            sql.Users_Theme(tableName: path, _as: _as);
                            break;
                        case "FirstAndLastNameOrder":
                            sql.Users_FirstAndLastNameOrder(tableName: path, _as: _as);
                            break;
                        case "Body":
                            sql.Users_Body(tableName: path, _as: _as);
                            break;
                        case "LastLoginTime":
                            sql.Users_LastLoginTime(tableName: path, _as: _as);
                            break;
                        case "PasswordExpirationTime":
                            sql.Users_PasswordExpirationTime(tableName: path, _as: _as);
                            break;
                        case "PasswordChangeTime":
                            sql.Users_PasswordChangeTime(tableName: path, _as: _as);
                            break;
                        case "NumberOfLogins":
                            sql.Users_NumberOfLogins(tableName: path, _as: _as);
                            break;
                        case "NumberOfDenial":
                            sql.Users_NumberOfDenial(tableName: path, _as: _as);
                            break;
                        case "TenantManager":
                            sql.Users_TenantManager(tableName: path, _as: _as);
                            break;
                        case "ServiceManager":
                            sql.Users_ServiceManager(tableName: path, _as: _as);
                            break;
                        case "AllowCreationAtTopSite":
                            sql.Users_AllowCreationAtTopSite(tableName: path, _as: _as);
                            break;
                        case "AllowGroupAdministration":
                            sql.Users_AllowGroupAdministration(tableName: path, _as: _as);
                            break;
                        case "AllowGroupCreation":
                            sql.Users_AllowGroupCreation(tableName: path, _as: _as);
                            break;
                        case "AllowApi":
                            sql.Users_AllowApi(tableName: path, _as: _as);
                            break;
                        case "AllowMovingFromTopSite":
                            sql.Users_AllowMovingFromTopSite(tableName: path, _as: _as);
                            break;
                        case "EnableSecondaryAuthentication":
                            sql.Users_EnableSecondaryAuthentication(tableName: path, _as: _as);
                            break;
                        case "DisableSecondaryAuthentication":
                            sql.Users_DisableSecondaryAuthentication(tableName: path, _as: _as);
                            break;
                        case "Disabled":
                            sql.Users_Disabled(tableName: path, _as: _as);
                            break;
                        case "Lockout":
                            sql.Users_Lockout(tableName: path, _as: _as);
                            break;
                        case "LockoutCounter":
                            sql.Users_LockoutCounter(tableName: path, _as: _as);
                            break;
                        case "Developer":
                            sql.Users_Developer(tableName: path, _as: _as);
                            break;
                        case "UserSettings":
                            sql.Users_UserSettings(tableName: path, _as: _as);
                            break;
                        case "ApiKey":
                            sql.Users_ApiKey(tableName: path, _as: _as);
                            break;
                        case "PasswordHistries":
                            sql.Users_PasswordHistries(tableName: path, _as: _as);
                            break;
                        case "SecondaryAuthenticationCode":
                            sql.Users_SecondaryAuthenticationCode(tableName: path, _as: _as);
                            break;
                        case "SecondaryAuthenticationCodeExpirationTime":
                            sql.Users_SecondaryAuthenticationCodeExpirationTime(tableName: path, _as: _as);
                            break;
                        case "LdapSearchRoot":
                            sql.Users_LdapSearchRoot(tableName: path, _as: _as);
                            break;
                        case "SynchronizedTime":
                            sql.Users_SynchronizedTime(tableName: path, _as: _as);
                            break;
                        case "SecretKey":
                            sql.Users_SecretKey(tableName: path, _as: _as);
                            break;
                        case "EnableSecretKey":
                            sql.Users_EnableSecretKey(tableName: path, _as: _as);
                            break;
                        case "LoginExpirationLimit":
                            sql.Users_LoginExpirationLimit(tableName: path, _as: _as);
                            break;
                        case "LoginExpirationPeriod":
                            sql.Users_LoginExpirationPeriod(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Users_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Users_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Users_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Users_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Users_UpdatedTime(tableName: path, _as: _as);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
                case "Dashboards":
                    switch (columnName)
                    {
                        case "SiteId":
                            sql.Dashboards_SiteId(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Dashboards_UpdatedTime(tableName: path, _as: _as);
                            break;
                        case "DashboardId":
                            sql.Dashboards_DashboardId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Dashboards_Ver(tableName: path, _as: _as);
                            break;
                        case "Body":
                            sql.Dashboards_Body(tableName: path, _as: _as);
                            break;
                        case "Locked":
                            sql.Dashboards_Locked(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Dashboards_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Dashboards_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Dashboards_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Dashboards_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "TitleBody":
                            sql.Dashboards_Body(tableName: path, _as: Joined
                                ? path + ",Body"
                                : "Body");
                            goto case "Title";
                        case "Title":
                            sql
                                .Dashboards_Title(tableName: path, _as: _as)
                                .ItemTitle(
                                    tableName: path,
                                    _as: Joined
                                        ? path + ",ItemTitle"
                                        : "ItemTitle");
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
                case "Issues":
                    switch (columnName)
                    {
                        case "SiteId":
                            sql.Issues_SiteId(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Issues_UpdatedTime(tableName: path, _as: _as);
                            break;
                        case "IssueId":
                            sql.Issues_IssueId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Issues_Ver(tableName: path, _as: _as);
                            break;
                        case "Body":
                            sql.Issues_Body(tableName: path, _as: _as);
                            break;
                        case "StartTime":
                            sql.Issues_StartTime(tableName: path, _as: _as);
                            break;
                        case "CompletionTime":
                            sql.Issues_CompletionTime(tableName: path, _as: _as);
                            break;
                        case "WorkValue":
                            sql.Issues_WorkValue(tableName: path, _as: _as);
                            break;
                        case "ProgressRate":
                            sql.Issues_ProgressRate(tableName: path, _as: _as);
                            break;
                        case "RemainingWorkValue":
                            sql.Issues_RemainingWorkValue(tableName: path, _as: _as);
                            break;
                        case "Status":
                            sql.Issues_Status(tableName: path, _as: _as);
                            break;
                        case "Manager":
                            sql.Issues_Manager(tableName: path, _as: _as);
                            break;
                        case "Owner":
                            sql.Issues_Owner(tableName: path, _as: _as);
                            break;
                        case "Locked":
                            sql.Issues_Locked(tableName: path, _as: _as);
                            break;
                        case "SiteTitle":
                            sql.Issues_SiteTitle(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Issues_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Issues_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Issues_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Issues_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "TitleBody":
                            sql.Issues_Body(tableName: path, _as: Joined
                                ? path + ",Body"
                                : "Body");
                            goto case "Title";
                        case "Title":
                            sql
                                .Issues_Title(tableName: path, _as: _as)
                                .ItemTitle(
                                    tableName: path,
                                    _as: Joined
                                        ? path + ",ItemTitle"
                                        : "ItemTitle");
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
                case "Results":
                    switch (columnName)
                    {
                        case "SiteId":
                            sql.Results_SiteId(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Results_UpdatedTime(tableName: path, _as: _as);
                            break;
                        case "ResultId":
                            sql.Results_ResultId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Results_Ver(tableName: path, _as: _as);
                            break;
                        case "Body":
                            sql.Results_Body(tableName: path, _as: _as);
                            break;
                        case "Status":
                            sql.Results_Status(tableName: path, _as: _as);
                            break;
                        case "Manager":
                            sql.Results_Manager(tableName: path, _as: _as);
                            break;
                        case "Owner":
                            sql.Results_Owner(tableName: path, _as: _as);
                            break;
                        case "Locked":
                            sql.Results_Locked(tableName: path, _as: _as);
                            break;
                        case "SiteTitle":
                            sql.Results_SiteTitle(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Results_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Results_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Results_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Results_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "TitleBody":
                            sql.Results_Body(tableName: path, _as: Joined
                                ? path + ",Body"
                                : "Body");
                            goto case "Title";
                        case "Title":
                            sql
                                .Results_Title(tableName: path, _as: _as)
                                .ItemTitle(
                                    tableName: path,
                                    _as: Joined
                                        ? path + ",ItemTitle"
                                        : "ItemTitle");
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"\"{columnName}\"",
                                        tableName: path,
                                        columnName: columnName,
                                        _as: _as);
                                break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}
