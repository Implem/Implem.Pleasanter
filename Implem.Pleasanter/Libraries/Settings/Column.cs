﻿using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public string Id;
        public string ColumnName;
        public string LabelText;
        public string GridLabelText;
        public string Description;
        public string ChoicesText;
        public bool? UseSearch;
        public string DefaultInput;
        public string GridFormat;
        public string EditorFormat;
        public string ExportFormat;
        public string ControlType;
        public string Format;
        public bool? NoWrap;
        public bool? Hide;
        public string ExtendedCellCss;
        public string ExtendedFieldCss;
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
        public int? DecimalPlaces;
        public SiteSettings.RoundingTypes? RoundingType;
        public decimal? Min;
        public decimal? Max;
        public decimal? Step;
        public decimal? DefaultMinValue;
        public decimal? DefaultMaxValue;
        public bool? NoDuplication;
        public bool? CopyByDefault;
        public bool? EditorReadOnly;
        public bool? AllowImage;
        public bool? AllowBulkUpdate;
        public string FieldCss;
        public SiteSettings.TextAlignTypes? TextAlign;
        public string Unit;
        public bool? Link;
        public ColumnUtilities.CheckFilterControlTypes? CheckFilterControlType;
        public decimal? NumFilterMin;
        public decimal? NumFilterMax;
        public decimal? NumFilterStep;
        public ColumnUtilities.DateFilterSetMode? DateFilterSetMode;
        public int? DateFilterMinSpan;
        public int? DateFilterMaxSpan;
        public bool? DateFilterFy;
        public bool? DateFilterHalf;
        public bool? DateFilterQuarter;
        public bool? DateFilterMonth;
        public decimal? LimitQuantity;
        public decimal? LimitSize;
        public decimal? TotalLimitSize;
        public decimal? ThumbnailLimitSize;
        [NonSerialized]
        public int? No;
        [NonSerialized]
        public bool Id_Ver;
        [NonSerialized]
        public string Size;
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
        public Dictionary<string, string> ChoiceValueHash;
        [NonSerialized]
        public Dictionary<string, int> SiteUserHash;
        [NonSerialized]
        public bool CanCreate = true;
        [NonSerialized]
        public bool CanRead = true;
        [NonSerialized]
        public bool CanUpdate = true;
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

        public bool HasChoices()
        {
            return ControlType == "ChoicesText" && !ChoicesText.IsNullOrEmpty();
        }

        public Column()
        {
        }

        public Column(string columnName)
        {
            ColumnName = columnName;
        }

        public void SetChoiceHash(
            Context context,
            long siteId,
            Dictionary<string, List<string>> linkHash = null,
            IEnumerable<string> searchIndexes = null,
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
            IEnumerable<string> searchIndexes,
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
                    else if (Linked())
                    {
                        if (setChoices) 
                        {
                            var key = "[[" + new Link(
                                columnName: ColumnName,
                                settings: line).SiteId + "]]";
                            if (linkHash != null && linkHash.ContainsKey(key))
                            {
                                linkHash.Get(key)?
                                    .ToDictionary(
                                        o => o.Split_1st(),
                                        o => Strings.CoalesceEmpty(o.Split_2nd(), o.Split_1st()))
                                    .ForEach(o =>
                                        AddToChoiceHash(o.Key, o.Value));
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

        public bool Linked(SiteSettings ss, long fromSiteId)
        {
            return
                fromSiteId != 0 &&
                ss.Links.Any(o =>
                    o.ColumnName == ColumnName && o.SiteId == fromSiteId);
        }

        private void AddToChoiceHash(string value, string text)
        {
            if (!ChoiceHash.Keys.Contains(value))
            {
                ChoiceHash.Add(value, new Choice(value, text));
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

        public Dictionary<string, ControlData> EditChoices(
            Context context,
            bool insertBlank = false,
            bool addNotSet = false,
            View view = null,
            int limit = 0)
        {
            var hash = new Dictionary<string, ControlData>();
            var blank = Type == Types.User
                ? User.UserTypes.Anonymous.ToInt().ToString()
                : TypeName == "int"
                    ? "0"
                    : string.Empty;
            if (!HasChoices()) return hash;
            if (addNotSet && !Required)
            {
                hash.Add("\t", new ControlData(Displays.NotSet(context: context)));
            }
            if (insertBlank && CanEmpty())
            {
                hash.Add(blank, new ControlData(string.Empty));
            }
            var selected = view?
                .ColumnFilter(ColumnName)?
                .Deserialize<List<string>>()?
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

        public Choice Choice(string selectedValue, string nullCase = null)
        {
            return selectedValue != null
                && ChoiceHash != null
                && ChoiceHash.ContainsKey(selectedValue)
                    ? ChoiceHash[selectedValue]
                    : new Choice(nullCase);
        }

        public string ChoicePart(Context context, string selectedValue, ExportColumn.Types? type)
        {
            if (Type != Types.Normal)
            {
                AddNotIncludedValue(
                    context: context,
                    selectedValue: selectedValue);
            }
            var choice = Choice(selectedValue, nullCase: selectedValue);
            switch (type)
            {
                case ExportColumn.Types.Value: return choice.Value;
                case ExportColumn.Types.Text: return choice.Text;
                case ExportColumn.Types.TextMini: return choice.TextMini;
                default: return choice.Text;
            }
        }

        private void AddNotIncludedValue(Context context, string selectedValue)
        {
            if (ChoiceHash?.ContainsKey(selectedValue) == false)
            {
                switch (Type)
                {
                    case Types.User:
                        var user = SiteInfo.User(
                            context: context,
                            userId: selectedValue.ToInt());
                        if (!user.Anonymous())
                        {
                            ChoiceHash.Add(selectedValue, new Choice(user.Name));
                        }
                        break;
                    default:
                        var name = SiteInfo.Name(
                            context: context,
                            id: selectedValue.ToInt(),
                            type: Type);
                        if (!name.IsNullOrEmpty())
                        {
                            ChoiceHash.Add(selectedValue, new Choice(name));
                        }
                        break;
                }
            }
        }

        public string RecordingData(Context context, string value, long siteId)
        {
            var userHash = SiteInfo.TenantCaches.Get(context.TenantId)?.UserHash;
            var recordingData = value;
            if (TypeCs == "Comments")
            {
                return new Comments().Prepend(
                    context: context, ss: SiteSettings, body: value).ToJson();
            }
            else if (TypeName == "datetime")
            {
                return value?.ToDateTime().ToUniversal(context: context).ToString()
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
                recordingData = ChoiceValueHash.Get(value);
                if (Type == Types.User && recordingData == null)
                {
                    if (SiteUserHash == null)
                    {
                        SiteUserHash = SiteInfo.SiteUsers(context: context, siteId: siteId)
                            .Where(id => userHash.ContainsKey(id))
                            .GroupBy(id => userHash.Get(id)?.Name)
                            .Select(id => id.First())
                            .ToDictionary(id => userHash.Get(id)?.Name, o => o);
                    }
                    var userId = SiteUserHash.Get(value);
                    recordingData = userId != 0
                        ? userId.ToString()
                        : User.UserTypes.Anonymous.ToInt().ToString();
                }
                recordingData = recordingData ?? value;
            }
            return recordingData ?? string.Empty;
        }

        public string Display(
            Context context, decimal value, bool unit = false, bool format = true)
        {
            try
            {
                return (!Format.IsNullOrEmpty() && format
                    ? value.ToString(
                        Format + (Format == "C" || Format == "N"
                            ? DecimalPlaces.ToString()
                            : string.Empty),
                        context.CultureInfo())
                    : DecimalPlaces.ToInt() == 0
                        ? value.ToString("0", "0")
                        : DisplayValue(value))
                            + (unit ? Unit : string.Empty);
            }
            catch (FormatException)
            {
                return ((int)value).ToString(Format);
            }
        }

        private string DisplayValue(decimal value)
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
                    + (EditorReadOnly == true
                        || this.ColumnPermissionType(context: context) != Permissions.ColumnPermissionTypes.Update
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
                case "Ymdhms":
                    return Displays.YmdhmDatePickerFormat(context: context);
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

        public DateTime DefaultTime()
        {
            return DefaultInput.IsNullOrEmpty()
                ? 0.ToDateTime()
                : DateTime.Now.AddDays(DefaultInput.ToInt());
        }

        public string GetDefaultInput(Context context)
        {
            switch (DefaultInput)
            {
                case "[[Self]]":
                    switch (ChoicesText.SplitReturn().FirstOrDefault())
                    {
                        case "[[Depts]]":
                            return context.DeptId.ToString();
                        case "[[Groups]]":
                        case "[[Groups*]]":
                            return DefaultGroupId(context: context).ToString();
                        case "[[Users]]":
                        case "[[Users*]]":
                            return context.UserId.ToString();
                    }
                    break;
            }
            return DefaultInput;
        }

        private int DefaultGroupId(Context context)
        {
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn().GroupId(),
                    where: Rds.GroupMembersWhere()
                        .Or(or: Rds.GroupMembersWhere()
                            .DeptId(context.DeptId)
                            .UserId(context.UserId))))
                                .AsEnumerable()
                                .Select(dataRow => dataRow.Int("GroupId"))
                                .FirstOrDefault(groupId => ChoiceHash.ContainsKey(groupId.ToString()));
        }

        public SqlColumnCollection SqlColumnCollection()
        {
            var sql = new SqlColumnCollection();
            var tableName = Strings.CoalesceEmpty(JoinTableName, SiteSettings.ReferenceType);
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
            sql.Add(
                columnBracket: "[UpdatedTime]",
                tableName: Joined
                    ? TableAlias
                    : tableName,
                columnName: "UpdatedTime",
                    _as: Joined
                        ? TableAlias + ",UpdatedTime"
                        : null);
            return sql;
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

        public string TableName()
        {
            return Strings.CoalesceEmpty(TableAlias, JoinTableName, SiteSettings.ReferenceType);
        }

        public string ParamName()
        {
            return ColumnName
                .Replace(",", "_")
                .Replace("-", "_")
                .Replace("~", "_");
        }

        public bool Linked()
        {
            return SiteSettings?.Links?.Any(o => o.ColumnName == Name) == true;
        }

        public string ConvertIfUserColumn(DataRow dataRow)
        {
            var value = dataRow.String(ColumnName);
            return Type == Types.User && value.IsNullOrEmpty()
                ? User.UserTypes.Anonymous.ToInt().ToString()
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
                TextAlign == SiteSettings.TextAlignTypes.Right
                    ? "right-align"
                    : string.Empty
            }
                .Select(o => o?.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .Join(" ");
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
                            switch (Def.ExtendedColumnTypes.Get(columnName))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"[{columnName}]",
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
                            switch (Def.ExtendedColumnTypes.Get(columnName))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"[{columnName}]",
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
                            switch (Def.ExtendedColumnTypes.Get(columnName))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"[{columnName}]",
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
                            switch (Def.ExtendedColumnTypes.Get(columnName))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"[{columnName}]",
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
                        case "GridGuide":
                            sql.Sites_GridGuide(tableName: path, _as: _as);
                            break;
                        case "EditorGuide":
                            sql.Sites_EditorGuide(tableName: path, _as: _as);
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
                            switch (Def.ExtendedColumnTypes.Get(columnName))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"[{columnName}]",
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
                            switch (Def.ExtendedColumnTypes.Get(columnName))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"[{columnName}]",
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
                            switch (Def.ExtendedColumnTypes.Get(columnName))
                            {
                                case "Class":
                                case "Num":
                                case "Date":
                                case "Description":
                                case "Check":
                                case "Attachments":
                                    sql.Add(
                                        columnBracket: $"[{columnName}]",
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
