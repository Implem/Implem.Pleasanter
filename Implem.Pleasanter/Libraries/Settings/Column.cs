using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Column
    {
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
        public string Section;
        public string GridDesign;
        public bool? ValidateRequired;
        public bool? ValidateNumber;
        public bool? ValidateDate;
        public bool? ValidateEmail;
        public string ValidateEqualTo;
        public int? ValidateMaxLength;
        public int? DecimalPlaces;
        public decimal? Min;
        public decimal? Max;
        public decimal? Step;
        public bool? NoDuplication;
        public bool? CopyByDefault;
        public bool? EditorReadOnly;
        public bool? AllowImage;
        public string FieldCss;
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
        public bool UserColumn;
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

        public void SetChoiceHash(Context context, bool searchColumnOnly = false)
        {
            SetChoiceHash(
                context: context,
                siteId: SiteSettings.SiteId,
                linkHash: SiteSettings.LinkHash(
                    context: context,
                    columnName: Name,
                    searchColumnOnly: searchColumnOnly));
        }

        public void SetChoiceHash(
            Context context,
            long siteId,
            Dictionary<string, List<string>> linkHash = null,
            IEnumerable<string> searchIndexes = null)
        {
            ChoiceHash = new Dictionary<string, Choice>();
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
                        line: data.Line));
            if (searchIndexes?.Any() == true)
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
            string line)
        {
            switch (line)
            {
                case "[[Depts]]":
                    SiteInfo.TenantCaches.Get(context.TenantId)?
                        .DeptHash
                        .Where(o => o.Value.TenantId == context.TenantId)
                        .Where(o => searchIndexes?.Any() != true ||
                            searchIndexes.All(p =>
                                o.Key == p.ToInt() ||
                                o.Value.Name.RegexLike(p).Any()))
                        .ForEach(o => AddToChoiceHash(
                            o.Key.ToString(),
                            SiteInfo.Dept(
                                tenantId: context.TenantId,
                                deptId: o.Key).Name));
                    break;
                case "[[TimeZones]]":
                    TimeZoneInfo.GetSystemTimeZones()
                        .ForEach(o => AddToChoiceHash(
                            o.Id,
                            o.StandardName));
                    break;
                default:
                    if (line.RegexExists(@"^\[\[Users.*\]\]$"))
                    {
                        AddUsersToChoiceHash(
                            context: context,
                            siteId: siteId,
                            settings: line,
                            searchIndexes: searchIndexes);
                    }
                    else if (Linked())
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
                    else if (TypeName != "bit")
                    {
                        if (searchIndexes == null ||
                            searchIndexes.All(o => line.RegexLike(o).Any()))
                        {
                            AddToChoiceHash(line);
                        }
                    }
                    else
                    {
                        AddToChoiceHash((index == 0).ToOneOrZeroString(), line);
                    }
                    break;
            }
        }

        public void AddUsersToChoiceHash(
            Context context, long siteId, string settings, IEnumerable<string> searchIndexes)
        {
            IEnumerable<int> users = null;
            var showDeptName = false;
            settings?
                .RegexFirst(@"(?<=\[\[).+(?=\]\])")?
                .Split(',')
                .Select((o, i) => new { Index = i, Setting = o })
                .ForEach(data =>
                {
                    if (data.Index == 0)
                    {
                        users = data.Setting == "Users*"
                            ? SiteInfo.TenantCaches.Get(context.TenantId)?
                                .UserHash
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
            users
                .Select(userId => SiteInfo.User(
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
                .ForEach(user => AddToChoiceHash(
                    user.Id.ToString(),
                    SiteInfo.UserName(
                        context: context,
                        userId: user.Id,
                        showDeptName: showDeptName)));
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
            View view = null)
        {
            var hash = new Dictionary<string, ControlData>();
            var blank = UserColumn
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
            if (UserColumn)
            {
                AddNotIncludedUser(
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

        private void AddNotIncludedUser(Context context, string selectedValue)
        {
            if (ChoiceHash?.ContainsKey(selectedValue) == false)
            {
                var user = SiteInfo.User(
                    context: context,
                    userId: selectedValue.ToInt());
                if (!user.Anonymous())
                {
                    ChoiceHash.Add(selectedValue, new Choice(user.Name));
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
                if (UserColumn && recordingData == null)
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
            return (!Format.IsNullOrEmpty() && format
                ? value.ToString(
                    Format + (Format == "C" && DecimalPlaces.ToInt() == 0
                        ? string.Empty
                        : DecimalPlaces.ToString()),
                    context.CultureInfo())
                : DecimalPlaces.ToInt() == 0
                    ? value.ToString("0", "0")
                    : DisplayValue(value))
                        + (unit ? Unit : string.Empty);
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
                    + (EditorReadOnly == true || !CanUpdate
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
             return Math.Round(value, DecimalPlaces.ToInt(), MidpointRounding.AwayFromZero);
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
            return MaxNumber() * -1;
        }

        public decimal MaxNumber()
        {
            var length = Size.Split_1st().ToInt() - Size.Split_2nd().ToInt();
            return length > 0
                ? new string('9', length).ToDecimal()
                : 0;
        }

        public DateTime DefaultTime()
        {
            return DateTime.Now.AddDays(DefaultInput.ToInt());
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
                        case "[[Users]]":
                        case "[[Users*]]":
                            return context.UserId.ToString();
                    }
                    break;
            }
            return DefaultInput;
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
                tableName: tableName,
                columnName: "UpdatedTime");
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
                param);
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
            return UserColumn && value.IsNullOrEmpty()
                ? User.UserTypes.Anonymous.ToInt().ToString()
                : value;
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
                case "Tenants":
                    switch (columnName)
                    {
                        case "TenantId":
                            sql.Tenants_TenantId(tableName: path, _as: _as);
                            break;
                        case "Ver":
                            sql.Tenants_Ver(tableName: path, _as: _as);
                            break;
                        case "TenantName":
                            sql.Tenants_TenantName(tableName: path, _as: _as);
                            break;
                        case "Body":
                            sql.Tenants_Body(tableName: path, _as: _as);
                            break;
                        case "ContractSettings":
                            sql.Tenants_ContractSettings(tableName: path, _as: _as);
                            break;
                        case "ContractDeadline":
                            sql.Tenants_ContractDeadline(tableName: path, _as: _as);
                            break;
                        case "DisableAllUsersPermission":
                            sql.Tenants_DisableAllUsersPermission(tableName: path, _as: _as);
                            break;
                        case "LogoType":
                            sql.Tenants_LogoType(tableName: path, _as: _as);
                            break;
                        case "HtmlTitleTop":
                            sql.Tenants_HtmlTitleTop(tableName: path, _as: _as);
                            break;
                        case "HtmlTitleSite":
                            sql.Tenants_HtmlTitleSite(tableName: path, _as: _as);
                            break;
                        case "HtmlTitleRecord":
                            sql.Tenants_HtmlTitleRecord(tableName: path, _as: _as);
                            break;
                        case "Comments":
                            sql.Tenants_Comments(tableName: path, _as: _as);
                            break;
                        case "Creator":
                            sql.Tenants_Creator(tableName: path, _as: _as);
                            break;
                        case "Updator":
                            sql.Tenants_Updator(tableName: path, _as: _as);
                            break;
                        case "CreatedTime":
                            sql.Tenants_CreatedTime(tableName: path, _as: _as);
                            break;
                        case "UpdatedTime":
                            sql.Tenants_UpdatedTime(tableName: path, _as: _as);
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
