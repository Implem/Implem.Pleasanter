using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
        public bool? EditorReadOnly;
        public bool? AllowImage;
        public string FieldCss;
        public string Unit;
        public bool? Link;
        public ColumnUtilities.CheckFilterControlTypes? CheckFilterControlType;
        public decimal? NumFilterMin;
        public decimal? NumFilterMax;
        public decimal? NumFilterStep;
        public int? DateFilterMinSpan;
        public int? DateFilterMaxSpan;
        public bool? DateFilterFy;
        public bool? DateFilterHalf;
        public bool? DateFilterQuarter;
        public bool? DateFilterMonth;
        public int? LimitQuantity;
        public int? LimitSize;
        public int? TotalLimitSize;
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
            return ControlType == "ChoicesText" && ChoicesText.Trim() != string.Empty;
        }

        public Column()
        {
        }

        public Column(string columnName)
        {
            ColumnName = columnName;
        }

        public void SetChoiceHash(
            long siteId,
            Dictionary<string, List<string>> linkHash = null,
            IEnumerable<string> searchIndexes = null)
        {
            var tenantId = Sessions.TenantId();
            ChoiceHash = new Dictionary<string, Choice>();
            ChoicesText.SplitReturn()
                .Where(o => o.Trim() != string.Empty)
                .Select((o, i) => new { Line = o.Trim(), Index = i })
                .ForEach(data =>
                    SetChoiceHash(
                        tenantId,
                        siteId,
                        linkHash,
                        searchIndexes,
                        data.Index,
                        data.Line));
            if (searchIndexes?.Any() == true)
            {
                ChoiceHash = ChoiceHash.Take(Parameters.General.DropDownSearchPageSize)
                    .ToDictionary(o => o.Key, o => o.Value);
            }
        }

        private void SetChoiceHash(
            int tenantId,
            long siteId,
            Dictionary<string, List<string>> linkHash,
            IEnumerable<string> searchIndexes,
            int index,
            string line)
        {
            switch (line)
            {
                case "[[Depts]]":
                    SiteInfo.TenantCaches.Get(tenantId)?
                        .DeptHash
                        .Where(o => o.Value.TenantId == tenantId)
                        .Where(o => searchIndexes?.Any() != true ||
                            searchIndexes.All(p =>
                                o.Key == p.ToInt() ||
                                o.Value.Name.Contains(p)))
                        .ForEach(o => AddToChoiceHash(
                            o.Key.ToString(),
                            SiteInfo.Dept(o.Key).Name));
                    break;
                case "[[Users]]":
                    SiteInfo.SiteUsers(tenantId, siteId)?
                        .Where(o => !SiteInfo.User(o).Disabled)
                        .ToDictionary(o => o.ToString(), o => SiteInfo.UserName(o))
                        .Where(o => searchIndexes?.Any() != true ||
                            searchIndexes.All(p =>
                                o.Key.Contains(p) ||
                                o.Value.Contains(p)))
                        .ForEach(o => AddToChoiceHash(o.Key, o.Value));
                    break;
                case "[[Users*]]":
                    SiteInfo.TenantCaches.Get(tenantId)?
                        .UserHash
                        .Where(o => o.Value.TenantId == tenantId)
                        .ToDictionary(o => o.Key.ToString(), o => o.Value.Name)
                        .Where(o => searchIndexes?.Any() != true ||
                            searchIndexes.All(p =>
                                o.Key.Contains(p) ||
                                o.Value.Contains(p)))
                        .ForEach(o => AddToChoiceHash(o.Key, o.Value));
                    break;
                case "[[TimeZones]]":
                    TimeZoneInfo.GetSystemTimeZones()
                        .ForEach(o => AddToChoiceHash(
                            o.Id,
                            o.StandardName));
                    break;
                default:
                    if (Linked())
                    {
                        if (linkHash != null && linkHash.ContainsKey(line))
                        {
                            linkHash[line].ForEach(value =>
                                AddToChoiceHash(value));
                        }
                    }
                    else if (TypeName != "bit")
                    {
                        if (searchIndexes == null ||
                            searchIndexes.All(o => line.Contains(o)))
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
            bool insertBlank = false, bool shorten = false, bool addNotSet = false)
        {
            var tenantId = Sessions.TenantId();
            var hash = new Dictionary<string, ControlData>();
            if (!HasChoices()) return hash;
            if (addNotSet && !Required)
            {
                hash.Add("\t", new ControlData(Displays.NotSet()));
            }
            if (insertBlank && CanEmpty())
            {
                hash.Add(
                    UserColumn
                        ? User.UserTypes.Anonymous.ToInt().ToString()
                        : string.Empty,
                    new ControlData(string.Empty));
            }
            ChoiceHash?.Values
                .Where(o => !hash.ContainsKey(o.Value))
                .GroupBy(o => o.Value)
                .Select(o => o.FirstOrDefault())
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
            return ChoiceHash != null && ChoiceHash.ContainsKey(selectedValue)
                ? ChoiceHash[selectedValue]
                : new Choice(nullCase);
        }

        public string ChoicePart(string selectedValue, ExportColumn.Types? type)
        {
            if (UserColumn) AddNotIncludedUser(selectedValue);
            var choice = Choice(selectedValue, nullCase: selectedValue);
            switch (type)
            {
                case ExportColumn.Types.Value: return choice.Value;
                case ExportColumn.Types.Text: return choice.Text;
                case ExportColumn.Types.TextMini: return choice.TextMini;
                default: return choice.Text;
            }
        }

        private void AddNotIncludedUser(string selectedValue)
        {
            if (ChoiceHash?.ContainsKey(selectedValue) == false)
            {
                var user = SiteInfo.User(selectedValue.ToInt());
                if (!user.Anonymous())
                {
                    ChoiceHash.Add(selectedValue, new Choice(user.Name));
                }
            }
        }

        public string RecordingData(string value, long siteId)
        {
            var tenantId = Sessions.TenantId();
            var userHash = SiteInfo.TenantCaches[tenantId].UserHash;
            var recordingData = value;
            if (UserColumn)
            {
                if (SiteUserHash == null)
                {
                    SiteUserHash = SiteInfo.SiteUsers(tenantId, siteId)
                        .Where(o => userHash.ContainsKey(o))
                        .ToDictionary(o => userHash[o].Name, o => o);
                }
                var userId = SiteUserHash.Get(value);
                recordingData = userId != 0
                    ? userId.ToString()
                    : User.UserTypes.Anonymous.ToInt().ToString();
            }
            else if (TypeCs == "Comments")
            {
                return new Comments().Prepend(value).ToJson();
            }
            else if (TypeName == "datetime")
            {
                return value?.ToDateTime().ToUniversal().ToString() ?? string.Empty;
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
                recordingData = ChoiceValueHash.Get(value) ?? value;
            }
            return recordingData ?? string.Empty;
        }

        public string Display(decimal value, bool unit = false, bool format = true)
        {
            return (!Format.IsNullOrEmpty() && format
                ? value.ToString(
                    Format + (Format == "C" && DecimalPlaces.ToInt() == 0
                        ? string.Empty
                        : DecimalPlaces.ToString()),
                    Sessions.CultureInfo())
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

        public string Display(SiteSettings ss, decimal value, bool format = true)
        {
            return Display(value, format: format) + (EditorReadOnly == true || !CanUpdate
                ? Unit
                : string.Empty);
        }

        public string DisplayGrid(DateTime value)
        {
            return value.Display(GridFormat);
        }

        public string DisplayControl(DateTime value)
        {
            return value.Display(EditorFormat);
        }

        public decimal Round(decimal value)
        {
             return Math.Round(value, DecimalPlaces.ToInt(), MidpointRounding.AwayFromZero);
        }

        public string DateTimeFormat()
        {
            switch (EditorFormat)
            {
                case "Ymdhm":
                case "Ymdhms":
                    return Displays.YmdhmDatePickerFormat();
                default:
                    return Displays.YmdDatePickerFormat();
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

        public SqlColumnCollection SqlColumnCollection(SiteSettings ss)
        {
            var sql = new SqlColumnCollection();
            SelectColumns(
                sql: sql,
                tableName: SiteSettings.ReferenceType,
                columnName: Name,
                path: Joined
                    ? TableAlias
                    : SiteSettings.ReferenceType,
                _as: Joined
                    ? ColumnName
                    : null);
            LinkedSqlColumnCollection(ss, sql);
            return sql;
        }

        public SqlColumnCollection LinkedSqlColumnCollection(
            SiteSettings ss, SqlColumnCollection sql)
        {
            var link = SiteSettings.Links?
                .FirstOrDefault(o => o.ColumnName == Name);
            if (link != null)
            {
                if (ss.JoinedSsHash?.ContainsKey(link.SiteId) == true)
                {
                    sql.Add(
                        sub: Rds.SelectItems(
                            column: Rds.ItemsColumn().Title(),
                            where: Rds.ItemsWhere()
                                .SiteId(raw: link.SiteId.ToString())
                                .ReferenceId(raw: "try_cast([{0}].[{1}] as bigint)"
                                    .Params(TableName(), link.ColumnName))),
                        _as: "Linked__" + ColumnName);
                }
            }
            return sql;
        }

        public SqlJoinCollection SqlJoinCollection(SiteSettings ss)
        {
            var sql = new SqlJoinCollection();
            if (!TableAlias.IsNullOrEmpty())
            {
                var left = new List<string>();
                var path = new List<string>();
                foreach (var part in TableAlias.Split('-'))
                {
                    var siteId = part.Split_2nd('~').ToLong();
                    var currentSs = ss.JoinedSsHash?.Get(siteId);
                    var tableName = currentSs?.ReferenceType;
                    var name = currentSs?.GetColumn(part.Split_1st('~'))?.Name;
                    path.Add(part);
                    var alias = path.Join("-");
                    if (tableName != null && name != null)
                    {
                        sql.Add(new SqlJoin(
                            tableBracket: "[" + tableName + "]",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: JoinExpression(
                                left.Any()
                                    ? left.Join("-")
                                    : ss.ReferenceType,
                                tableName,
                                name,
                                alias,
                                siteId),
                            _as: alias));
                        left.Add(part);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return sql;
        }

        private static string JoinExpression(
            string left, string tableName, string name, string alias, long siteId)
        {
            return "[{2}].[SiteId]={4} and try_cast([{0}].[{1}] as bigint)=[{2}].[{3}]"
                .Params(left, name, alias, Rds.IdColumn(tableName), siteId);
        }

        public string TableName()
        {
            return TableAlias ?? SiteSettings.ReferenceType;
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
                        case "Developer":
                            sql.Users_Developer(tableName: path, _as: _as);
                            break;
                        case "UserSettings":
                            sql.Users_UserSettings(tableName: path, _as: _as);
                            break;
                        case "ApiKey":
                            sql.Users_ApiKey(tableName: path, _as: _as);
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
                        case "ClassA":
                            sql.Issues_ClassA(tableName: path, _as: _as);
                            break;
                        case "ClassB":
                            sql.Issues_ClassB(tableName: path, _as: _as);
                            break;
                        case "ClassC":
                            sql.Issues_ClassC(tableName: path, _as: _as);
                            break;
                        case "ClassD":
                            sql.Issues_ClassD(tableName: path, _as: _as);
                            break;
                        case "ClassE":
                            sql.Issues_ClassE(tableName: path, _as: _as);
                            break;
                        case "ClassF":
                            sql.Issues_ClassF(tableName: path, _as: _as);
                            break;
                        case "ClassG":
                            sql.Issues_ClassG(tableName: path, _as: _as);
                            break;
                        case "ClassH":
                            sql.Issues_ClassH(tableName: path, _as: _as);
                            break;
                        case "ClassI":
                            sql.Issues_ClassI(tableName: path, _as: _as);
                            break;
                        case "ClassJ":
                            sql.Issues_ClassJ(tableName: path, _as: _as);
                            break;
                        case "ClassK":
                            sql.Issues_ClassK(tableName: path, _as: _as);
                            break;
                        case "ClassL":
                            sql.Issues_ClassL(tableName: path, _as: _as);
                            break;
                        case "ClassM":
                            sql.Issues_ClassM(tableName: path, _as: _as);
                            break;
                        case "ClassN":
                            sql.Issues_ClassN(tableName: path, _as: _as);
                            break;
                        case "ClassO":
                            sql.Issues_ClassO(tableName: path, _as: _as);
                            break;
                        case "ClassP":
                            sql.Issues_ClassP(tableName: path, _as: _as);
                            break;
                        case "ClassQ":
                            sql.Issues_ClassQ(tableName: path, _as: _as);
                            break;
                        case "ClassR":
                            sql.Issues_ClassR(tableName: path, _as: _as);
                            break;
                        case "ClassS":
                            sql.Issues_ClassS(tableName: path, _as: _as);
                            break;
                        case "ClassT":
                            sql.Issues_ClassT(tableName: path, _as: _as);
                            break;
                        case "ClassU":
                            sql.Issues_ClassU(tableName: path, _as: _as);
                            break;
                        case "ClassV":
                            sql.Issues_ClassV(tableName: path, _as: _as);
                            break;
                        case "ClassW":
                            sql.Issues_ClassW(tableName: path, _as: _as);
                            break;
                        case "ClassX":
                            sql.Issues_ClassX(tableName: path, _as: _as);
                            break;
                        case "ClassY":
                            sql.Issues_ClassY(tableName: path, _as: _as);
                            break;
                        case "ClassZ":
                            sql.Issues_ClassZ(tableName: path, _as: _as);
                            break;
                        case "NumA":
                            sql.Issues_NumA(tableName: path, _as: _as);
                            break;
                        case "NumB":
                            sql.Issues_NumB(tableName: path, _as: _as);
                            break;
                        case "NumC":
                            sql.Issues_NumC(tableName: path, _as: _as);
                            break;
                        case "NumD":
                            sql.Issues_NumD(tableName: path, _as: _as);
                            break;
                        case "NumE":
                            sql.Issues_NumE(tableName: path, _as: _as);
                            break;
                        case "NumF":
                            sql.Issues_NumF(tableName: path, _as: _as);
                            break;
                        case "NumG":
                            sql.Issues_NumG(tableName: path, _as: _as);
                            break;
                        case "NumH":
                            sql.Issues_NumH(tableName: path, _as: _as);
                            break;
                        case "NumI":
                            sql.Issues_NumI(tableName: path, _as: _as);
                            break;
                        case "NumJ":
                            sql.Issues_NumJ(tableName: path, _as: _as);
                            break;
                        case "NumK":
                            sql.Issues_NumK(tableName: path, _as: _as);
                            break;
                        case "NumL":
                            sql.Issues_NumL(tableName: path, _as: _as);
                            break;
                        case "NumM":
                            sql.Issues_NumM(tableName: path, _as: _as);
                            break;
                        case "NumN":
                            sql.Issues_NumN(tableName: path, _as: _as);
                            break;
                        case "NumO":
                            sql.Issues_NumO(tableName: path, _as: _as);
                            break;
                        case "NumP":
                            sql.Issues_NumP(tableName: path, _as: _as);
                            break;
                        case "NumQ":
                            sql.Issues_NumQ(tableName: path, _as: _as);
                            break;
                        case "NumR":
                            sql.Issues_NumR(tableName: path, _as: _as);
                            break;
                        case "NumS":
                            sql.Issues_NumS(tableName: path, _as: _as);
                            break;
                        case "NumT":
                            sql.Issues_NumT(tableName: path, _as: _as);
                            break;
                        case "NumU":
                            sql.Issues_NumU(tableName: path, _as: _as);
                            break;
                        case "NumV":
                            sql.Issues_NumV(tableName: path, _as: _as);
                            break;
                        case "NumW":
                            sql.Issues_NumW(tableName: path, _as: _as);
                            break;
                        case "NumX":
                            sql.Issues_NumX(tableName: path, _as: _as);
                            break;
                        case "NumY":
                            sql.Issues_NumY(tableName: path, _as: _as);
                            break;
                        case "NumZ":
                            sql.Issues_NumZ(tableName: path, _as: _as);
                            break;
                        case "DateA":
                            sql.Issues_DateA(tableName: path, _as: _as);
                            break;
                        case "DateB":
                            sql.Issues_DateB(tableName: path, _as: _as);
                            break;
                        case "DateC":
                            sql.Issues_DateC(tableName: path, _as: _as);
                            break;
                        case "DateD":
                            sql.Issues_DateD(tableName: path, _as: _as);
                            break;
                        case "DateE":
                            sql.Issues_DateE(tableName: path, _as: _as);
                            break;
                        case "DateF":
                            sql.Issues_DateF(tableName: path, _as: _as);
                            break;
                        case "DateG":
                            sql.Issues_DateG(tableName: path, _as: _as);
                            break;
                        case "DateH":
                            sql.Issues_DateH(tableName: path, _as: _as);
                            break;
                        case "DateI":
                            sql.Issues_DateI(tableName: path, _as: _as);
                            break;
                        case "DateJ":
                            sql.Issues_DateJ(tableName: path, _as: _as);
                            break;
                        case "DateK":
                            sql.Issues_DateK(tableName: path, _as: _as);
                            break;
                        case "DateL":
                            sql.Issues_DateL(tableName: path, _as: _as);
                            break;
                        case "DateM":
                            sql.Issues_DateM(tableName: path, _as: _as);
                            break;
                        case "DateN":
                            sql.Issues_DateN(tableName: path, _as: _as);
                            break;
                        case "DateO":
                            sql.Issues_DateO(tableName: path, _as: _as);
                            break;
                        case "DateP":
                            sql.Issues_DateP(tableName: path, _as: _as);
                            break;
                        case "DateQ":
                            sql.Issues_DateQ(tableName: path, _as: _as);
                            break;
                        case "DateR":
                            sql.Issues_DateR(tableName: path, _as: _as);
                            break;
                        case "DateS":
                            sql.Issues_DateS(tableName: path, _as: _as);
                            break;
                        case "DateT":
                            sql.Issues_DateT(tableName: path, _as: _as);
                            break;
                        case "DateU":
                            sql.Issues_DateU(tableName: path, _as: _as);
                            break;
                        case "DateV":
                            sql.Issues_DateV(tableName: path, _as: _as);
                            break;
                        case "DateW":
                            sql.Issues_DateW(tableName: path, _as: _as);
                            break;
                        case "DateX":
                            sql.Issues_DateX(tableName: path, _as: _as);
                            break;
                        case "DateY":
                            sql.Issues_DateY(tableName: path, _as: _as);
                            break;
                        case "DateZ":
                            sql.Issues_DateZ(tableName: path, _as: _as);
                            break;
                        case "DescriptionA":
                            sql.Issues_DescriptionA(tableName: path, _as: _as);
                            break;
                        case "DescriptionB":
                            sql.Issues_DescriptionB(tableName: path, _as: _as);
                            break;
                        case "DescriptionC":
                            sql.Issues_DescriptionC(tableName: path, _as: _as);
                            break;
                        case "DescriptionD":
                            sql.Issues_DescriptionD(tableName: path, _as: _as);
                            break;
                        case "DescriptionE":
                            sql.Issues_DescriptionE(tableName: path, _as: _as);
                            break;
                        case "DescriptionF":
                            sql.Issues_DescriptionF(tableName: path, _as: _as);
                            break;
                        case "DescriptionG":
                            sql.Issues_DescriptionG(tableName: path, _as: _as);
                            break;
                        case "DescriptionH":
                            sql.Issues_DescriptionH(tableName: path, _as: _as);
                            break;
                        case "DescriptionI":
                            sql.Issues_DescriptionI(tableName: path, _as: _as);
                            break;
                        case "DescriptionJ":
                            sql.Issues_DescriptionJ(tableName: path, _as: _as);
                            break;
                        case "DescriptionK":
                            sql.Issues_DescriptionK(tableName: path, _as: _as);
                            break;
                        case "DescriptionL":
                            sql.Issues_DescriptionL(tableName: path, _as: _as);
                            break;
                        case "DescriptionM":
                            sql.Issues_DescriptionM(tableName: path, _as: _as);
                            break;
                        case "DescriptionN":
                            sql.Issues_DescriptionN(tableName: path, _as: _as);
                            break;
                        case "DescriptionO":
                            sql.Issues_DescriptionO(tableName: path, _as: _as);
                            break;
                        case "DescriptionP":
                            sql.Issues_DescriptionP(tableName: path, _as: _as);
                            break;
                        case "DescriptionQ":
                            sql.Issues_DescriptionQ(tableName: path, _as: _as);
                            break;
                        case "DescriptionR":
                            sql.Issues_DescriptionR(tableName: path, _as: _as);
                            break;
                        case "DescriptionS":
                            sql.Issues_DescriptionS(tableName: path, _as: _as);
                            break;
                        case "DescriptionT":
                            sql.Issues_DescriptionT(tableName: path, _as: _as);
                            break;
                        case "DescriptionU":
                            sql.Issues_DescriptionU(tableName: path, _as: _as);
                            break;
                        case "DescriptionV":
                            sql.Issues_DescriptionV(tableName: path, _as: _as);
                            break;
                        case "DescriptionW":
                            sql.Issues_DescriptionW(tableName: path, _as: _as);
                            break;
                        case "DescriptionX":
                            sql.Issues_DescriptionX(tableName: path, _as: _as);
                            break;
                        case "DescriptionY":
                            sql.Issues_DescriptionY(tableName: path, _as: _as);
                            break;
                        case "DescriptionZ":
                            sql.Issues_DescriptionZ(tableName: path, _as: _as);
                            break;
                        case "CheckA":
                            sql.Issues_CheckA(tableName: path, _as: _as);
                            break;
                        case "CheckB":
                            sql.Issues_CheckB(tableName: path, _as: _as);
                            break;
                        case "CheckC":
                            sql.Issues_CheckC(tableName: path, _as: _as);
                            break;
                        case "CheckD":
                            sql.Issues_CheckD(tableName: path, _as: _as);
                            break;
                        case "CheckE":
                            sql.Issues_CheckE(tableName: path, _as: _as);
                            break;
                        case "CheckF":
                            sql.Issues_CheckF(tableName: path, _as: _as);
                            break;
                        case "CheckG":
                            sql.Issues_CheckG(tableName: path, _as: _as);
                            break;
                        case "CheckH":
                            sql.Issues_CheckH(tableName: path, _as: _as);
                            break;
                        case "CheckI":
                            sql.Issues_CheckI(tableName: path, _as: _as);
                            break;
                        case "CheckJ":
                            sql.Issues_CheckJ(tableName: path, _as: _as);
                            break;
                        case "CheckK":
                            sql.Issues_CheckK(tableName: path, _as: _as);
                            break;
                        case "CheckL":
                            sql.Issues_CheckL(tableName: path, _as: _as);
                            break;
                        case "CheckM":
                            sql.Issues_CheckM(tableName: path, _as: _as);
                            break;
                        case "CheckN":
                            sql.Issues_CheckN(tableName: path, _as: _as);
                            break;
                        case "CheckO":
                            sql.Issues_CheckO(tableName: path, _as: _as);
                            break;
                        case "CheckP":
                            sql.Issues_CheckP(tableName: path, _as: _as);
                            break;
                        case "CheckQ":
                            sql.Issues_CheckQ(tableName: path, _as: _as);
                            break;
                        case "CheckR":
                            sql.Issues_CheckR(tableName: path, _as: _as);
                            break;
                        case "CheckS":
                            sql.Issues_CheckS(tableName: path, _as: _as);
                            break;
                        case "CheckT":
                            sql.Issues_CheckT(tableName: path, _as: _as);
                            break;
                        case "CheckU":
                            sql.Issues_CheckU(tableName: path, _as: _as);
                            break;
                        case "CheckV":
                            sql.Issues_CheckV(tableName: path, _as: _as);
                            break;
                        case "CheckW":
                            sql.Issues_CheckW(tableName: path, _as: _as);
                            break;
                        case "CheckX":
                            sql.Issues_CheckX(tableName: path, _as: _as);
                            break;
                        case "CheckY":
                            sql.Issues_CheckY(tableName: path, _as: _as);
                            break;
                        case "CheckZ":
                            sql.Issues_CheckZ(tableName: path, _as: _as);
                            break;
                        case "AttachmentsA":
                            sql.Issues_AttachmentsA(tableName: path, _as: _as);
                            break;
                        case "AttachmentsB":
                            sql.Issues_AttachmentsB(tableName: path, _as: _as);
                            break;
                        case "AttachmentsC":
                            sql.Issues_AttachmentsC(tableName: path, _as: _as);
                            break;
                        case "AttachmentsD":
                            sql.Issues_AttachmentsD(tableName: path, _as: _as);
                            break;
                        case "AttachmentsE":
                            sql.Issues_AttachmentsE(tableName: path, _as: _as);
                            break;
                        case "AttachmentsF":
                            sql.Issues_AttachmentsF(tableName: path, _as: _as);
                            break;
                        case "AttachmentsG":
                            sql.Issues_AttachmentsG(tableName: path, _as: _as);
                            break;
                        case "AttachmentsH":
                            sql.Issues_AttachmentsH(tableName: path, _as: _as);
                            break;
                        case "AttachmentsI":
                            sql.Issues_AttachmentsI(tableName: path, _as: _as);
                            break;
                        case "AttachmentsJ":
                            sql.Issues_AttachmentsJ(tableName: path, _as: _as);
                            break;
                        case "AttachmentsK":
                            sql.Issues_AttachmentsK(tableName: path, _as: _as);
                            break;
                        case "AttachmentsL":
                            sql.Issues_AttachmentsL(tableName: path, _as: _as);
                            break;
                        case "AttachmentsM":
                            sql.Issues_AttachmentsM(tableName: path, _as: _as);
                            break;
                        case "AttachmentsN":
                            sql.Issues_AttachmentsN(tableName: path, _as: _as);
                            break;
                        case "AttachmentsO":
                            sql.Issues_AttachmentsO(tableName: path, _as: _as);
                            break;
                        case "AttachmentsP":
                            sql.Issues_AttachmentsP(tableName: path, _as: _as);
                            break;
                        case "AttachmentsQ":
                            sql.Issues_AttachmentsQ(tableName: path, _as: _as);
                            break;
                        case "AttachmentsR":
                            sql.Issues_AttachmentsR(tableName: path, _as: _as);
                            break;
                        case "AttachmentsS":
                            sql.Issues_AttachmentsS(tableName: path, _as: _as);
                            break;
                        case "AttachmentsT":
                            sql.Issues_AttachmentsT(tableName: path, _as: _as);
                            break;
                        case "AttachmentsU":
                            sql.Issues_AttachmentsU(tableName: path, _as: _as);
                            break;
                        case "AttachmentsV":
                            sql.Issues_AttachmentsV(tableName: path, _as: _as);
                            break;
                        case "AttachmentsW":
                            sql.Issues_AttachmentsW(tableName: path, _as: _as);
                            break;
                        case "AttachmentsX":
                            sql.Issues_AttachmentsX(tableName: path, _as: _as);
                            break;
                        case "AttachmentsY":
                            sql.Issues_AttachmentsY(tableName: path, _as: _as);
                            break;
                        case "AttachmentsZ":
                            sql.Issues_AttachmentsZ(tableName: path, _as: _as);
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
                                    idColumn: Rds.IdColumn(SiteSettings.ReferenceType),
                                    _as: Joined
                                        ? path + ",ItemTitle"
                                        : "ItemTitle");
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
                        case "ClassA":
                            sql.Results_ClassA(tableName: path, _as: _as);
                            break;
                        case "ClassB":
                            sql.Results_ClassB(tableName: path, _as: _as);
                            break;
                        case "ClassC":
                            sql.Results_ClassC(tableName: path, _as: _as);
                            break;
                        case "ClassD":
                            sql.Results_ClassD(tableName: path, _as: _as);
                            break;
                        case "ClassE":
                            sql.Results_ClassE(tableName: path, _as: _as);
                            break;
                        case "ClassF":
                            sql.Results_ClassF(tableName: path, _as: _as);
                            break;
                        case "ClassG":
                            sql.Results_ClassG(tableName: path, _as: _as);
                            break;
                        case "ClassH":
                            sql.Results_ClassH(tableName: path, _as: _as);
                            break;
                        case "ClassI":
                            sql.Results_ClassI(tableName: path, _as: _as);
                            break;
                        case "ClassJ":
                            sql.Results_ClassJ(tableName: path, _as: _as);
                            break;
                        case "ClassK":
                            sql.Results_ClassK(tableName: path, _as: _as);
                            break;
                        case "ClassL":
                            sql.Results_ClassL(tableName: path, _as: _as);
                            break;
                        case "ClassM":
                            sql.Results_ClassM(tableName: path, _as: _as);
                            break;
                        case "ClassN":
                            sql.Results_ClassN(tableName: path, _as: _as);
                            break;
                        case "ClassO":
                            sql.Results_ClassO(tableName: path, _as: _as);
                            break;
                        case "ClassP":
                            sql.Results_ClassP(tableName: path, _as: _as);
                            break;
                        case "ClassQ":
                            sql.Results_ClassQ(tableName: path, _as: _as);
                            break;
                        case "ClassR":
                            sql.Results_ClassR(tableName: path, _as: _as);
                            break;
                        case "ClassS":
                            sql.Results_ClassS(tableName: path, _as: _as);
                            break;
                        case "ClassT":
                            sql.Results_ClassT(tableName: path, _as: _as);
                            break;
                        case "ClassU":
                            sql.Results_ClassU(tableName: path, _as: _as);
                            break;
                        case "ClassV":
                            sql.Results_ClassV(tableName: path, _as: _as);
                            break;
                        case "ClassW":
                            sql.Results_ClassW(tableName: path, _as: _as);
                            break;
                        case "ClassX":
                            sql.Results_ClassX(tableName: path, _as: _as);
                            break;
                        case "ClassY":
                            sql.Results_ClassY(tableName: path, _as: _as);
                            break;
                        case "ClassZ":
                            sql.Results_ClassZ(tableName: path, _as: _as);
                            break;
                        case "NumA":
                            sql.Results_NumA(tableName: path, _as: _as);
                            break;
                        case "NumB":
                            sql.Results_NumB(tableName: path, _as: _as);
                            break;
                        case "NumC":
                            sql.Results_NumC(tableName: path, _as: _as);
                            break;
                        case "NumD":
                            sql.Results_NumD(tableName: path, _as: _as);
                            break;
                        case "NumE":
                            sql.Results_NumE(tableName: path, _as: _as);
                            break;
                        case "NumF":
                            sql.Results_NumF(tableName: path, _as: _as);
                            break;
                        case "NumG":
                            sql.Results_NumG(tableName: path, _as: _as);
                            break;
                        case "NumH":
                            sql.Results_NumH(tableName: path, _as: _as);
                            break;
                        case "NumI":
                            sql.Results_NumI(tableName: path, _as: _as);
                            break;
                        case "NumJ":
                            sql.Results_NumJ(tableName: path, _as: _as);
                            break;
                        case "NumK":
                            sql.Results_NumK(tableName: path, _as: _as);
                            break;
                        case "NumL":
                            sql.Results_NumL(tableName: path, _as: _as);
                            break;
                        case "NumM":
                            sql.Results_NumM(tableName: path, _as: _as);
                            break;
                        case "NumN":
                            sql.Results_NumN(tableName: path, _as: _as);
                            break;
                        case "NumO":
                            sql.Results_NumO(tableName: path, _as: _as);
                            break;
                        case "NumP":
                            sql.Results_NumP(tableName: path, _as: _as);
                            break;
                        case "NumQ":
                            sql.Results_NumQ(tableName: path, _as: _as);
                            break;
                        case "NumR":
                            sql.Results_NumR(tableName: path, _as: _as);
                            break;
                        case "NumS":
                            sql.Results_NumS(tableName: path, _as: _as);
                            break;
                        case "NumT":
                            sql.Results_NumT(tableName: path, _as: _as);
                            break;
                        case "NumU":
                            sql.Results_NumU(tableName: path, _as: _as);
                            break;
                        case "NumV":
                            sql.Results_NumV(tableName: path, _as: _as);
                            break;
                        case "NumW":
                            sql.Results_NumW(tableName: path, _as: _as);
                            break;
                        case "NumX":
                            sql.Results_NumX(tableName: path, _as: _as);
                            break;
                        case "NumY":
                            sql.Results_NumY(tableName: path, _as: _as);
                            break;
                        case "NumZ":
                            sql.Results_NumZ(tableName: path, _as: _as);
                            break;
                        case "DateA":
                            sql.Results_DateA(tableName: path, _as: _as);
                            break;
                        case "DateB":
                            sql.Results_DateB(tableName: path, _as: _as);
                            break;
                        case "DateC":
                            sql.Results_DateC(tableName: path, _as: _as);
                            break;
                        case "DateD":
                            sql.Results_DateD(tableName: path, _as: _as);
                            break;
                        case "DateE":
                            sql.Results_DateE(tableName: path, _as: _as);
                            break;
                        case "DateF":
                            sql.Results_DateF(tableName: path, _as: _as);
                            break;
                        case "DateG":
                            sql.Results_DateG(tableName: path, _as: _as);
                            break;
                        case "DateH":
                            sql.Results_DateH(tableName: path, _as: _as);
                            break;
                        case "DateI":
                            sql.Results_DateI(tableName: path, _as: _as);
                            break;
                        case "DateJ":
                            sql.Results_DateJ(tableName: path, _as: _as);
                            break;
                        case "DateK":
                            sql.Results_DateK(tableName: path, _as: _as);
                            break;
                        case "DateL":
                            sql.Results_DateL(tableName: path, _as: _as);
                            break;
                        case "DateM":
                            sql.Results_DateM(tableName: path, _as: _as);
                            break;
                        case "DateN":
                            sql.Results_DateN(tableName: path, _as: _as);
                            break;
                        case "DateO":
                            sql.Results_DateO(tableName: path, _as: _as);
                            break;
                        case "DateP":
                            sql.Results_DateP(tableName: path, _as: _as);
                            break;
                        case "DateQ":
                            sql.Results_DateQ(tableName: path, _as: _as);
                            break;
                        case "DateR":
                            sql.Results_DateR(tableName: path, _as: _as);
                            break;
                        case "DateS":
                            sql.Results_DateS(tableName: path, _as: _as);
                            break;
                        case "DateT":
                            sql.Results_DateT(tableName: path, _as: _as);
                            break;
                        case "DateU":
                            sql.Results_DateU(tableName: path, _as: _as);
                            break;
                        case "DateV":
                            sql.Results_DateV(tableName: path, _as: _as);
                            break;
                        case "DateW":
                            sql.Results_DateW(tableName: path, _as: _as);
                            break;
                        case "DateX":
                            sql.Results_DateX(tableName: path, _as: _as);
                            break;
                        case "DateY":
                            sql.Results_DateY(tableName: path, _as: _as);
                            break;
                        case "DateZ":
                            sql.Results_DateZ(tableName: path, _as: _as);
                            break;
                        case "DescriptionA":
                            sql.Results_DescriptionA(tableName: path, _as: _as);
                            break;
                        case "DescriptionB":
                            sql.Results_DescriptionB(tableName: path, _as: _as);
                            break;
                        case "DescriptionC":
                            sql.Results_DescriptionC(tableName: path, _as: _as);
                            break;
                        case "DescriptionD":
                            sql.Results_DescriptionD(tableName: path, _as: _as);
                            break;
                        case "DescriptionE":
                            sql.Results_DescriptionE(tableName: path, _as: _as);
                            break;
                        case "DescriptionF":
                            sql.Results_DescriptionF(tableName: path, _as: _as);
                            break;
                        case "DescriptionG":
                            sql.Results_DescriptionG(tableName: path, _as: _as);
                            break;
                        case "DescriptionH":
                            sql.Results_DescriptionH(tableName: path, _as: _as);
                            break;
                        case "DescriptionI":
                            sql.Results_DescriptionI(tableName: path, _as: _as);
                            break;
                        case "DescriptionJ":
                            sql.Results_DescriptionJ(tableName: path, _as: _as);
                            break;
                        case "DescriptionK":
                            sql.Results_DescriptionK(tableName: path, _as: _as);
                            break;
                        case "DescriptionL":
                            sql.Results_DescriptionL(tableName: path, _as: _as);
                            break;
                        case "DescriptionM":
                            sql.Results_DescriptionM(tableName: path, _as: _as);
                            break;
                        case "DescriptionN":
                            sql.Results_DescriptionN(tableName: path, _as: _as);
                            break;
                        case "DescriptionO":
                            sql.Results_DescriptionO(tableName: path, _as: _as);
                            break;
                        case "DescriptionP":
                            sql.Results_DescriptionP(tableName: path, _as: _as);
                            break;
                        case "DescriptionQ":
                            sql.Results_DescriptionQ(tableName: path, _as: _as);
                            break;
                        case "DescriptionR":
                            sql.Results_DescriptionR(tableName: path, _as: _as);
                            break;
                        case "DescriptionS":
                            sql.Results_DescriptionS(tableName: path, _as: _as);
                            break;
                        case "DescriptionT":
                            sql.Results_DescriptionT(tableName: path, _as: _as);
                            break;
                        case "DescriptionU":
                            sql.Results_DescriptionU(tableName: path, _as: _as);
                            break;
                        case "DescriptionV":
                            sql.Results_DescriptionV(tableName: path, _as: _as);
                            break;
                        case "DescriptionW":
                            sql.Results_DescriptionW(tableName: path, _as: _as);
                            break;
                        case "DescriptionX":
                            sql.Results_DescriptionX(tableName: path, _as: _as);
                            break;
                        case "DescriptionY":
                            sql.Results_DescriptionY(tableName: path, _as: _as);
                            break;
                        case "DescriptionZ":
                            sql.Results_DescriptionZ(tableName: path, _as: _as);
                            break;
                        case "CheckA":
                            sql.Results_CheckA(tableName: path, _as: _as);
                            break;
                        case "CheckB":
                            sql.Results_CheckB(tableName: path, _as: _as);
                            break;
                        case "CheckC":
                            sql.Results_CheckC(tableName: path, _as: _as);
                            break;
                        case "CheckD":
                            sql.Results_CheckD(tableName: path, _as: _as);
                            break;
                        case "CheckE":
                            sql.Results_CheckE(tableName: path, _as: _as);
                            break;
                        case "CheckF":
                            sql.Results_CheckF(tableName: path, _as: _as);
                            break;
                        case "CheckG":
                            sql.Results_CheckG(tableName: path, _as: _as);
                            break;
                        case "CheckH":
                            sql.Results_CheckH(tableName: path, _as: _as);
                            break;
                        case "CheckI":
                            sql.Results_CheckI(tableName: path, _as: _as);
                            break;
                        case "CheckJ":
                            sql.Results_CheckJ(tableName: path, _as: _as);
                            break;
                        case "CheckK":
                            sql.Results_CheckK(tableName: path, _as: _as);
                            break;
                        case "CheckL":
                            sql.Results_CheckL(tableName: path, _as: _as);
                            break;
                        case "CheckM":
                            sql.Results_CheckM(tableName: path, _as: _as);
                            break;
                        case "CheckN":
                            sql.Results_CheckN(tableName: path, _as: _as);
                            break;
                        case "CheckO":
                            sql.Results_CheckO(tableName: path, _as: _as);
                            break;
                        case "CheckP":
                            sql.Results_CheckP(tableName: path, _as: _as);
                            break;
                        case "CheckQ":
                            sql.Results_CheckQ(tableName: path, _as: _as);
                            break;
                        case "CheckR":
                            sql.Results_CheckR(tableName: path, _as: _as);
                            break;
                        case "CheckS":
                            sql.Results_CheckS(tableName: path, _as: _as);
                            break;
                        case "CheckT":
                            sql.Results_CheckT(tableName: path, _as: _as);
                            break;
                        case "CheckU":
                            sql.Results_CheckU(tableName: path, _as: _as);
                            break;
                        case "CheckV":
                            sql.Results_CheckV(tableName: path, _as: _as);
                            break;
                        case "CheckW":
                            sql.Results_CheckW(tableName: path, _as: _as);
                            break;
                        case "CheckX":
                            sql.Results_CheckX(tableName: path, _as: _as);
                            break;
                        case "CheckY":
                            sql.Results_CheckY(tableName: path, _as: _as);
                            break;
                        case "CheckZ":
                            sql.Results_CheckZ(tableName: path, _as: _as);
                            break;
                        case "AttachmentsA":
                            sql.Results_AttachmentsA(tableName: path, _as: _as);
                            break;
                        case "AttachmentsB":
                            sql.Results_AttachmentsB(tableName: path, _as: _as);
                            break;
                        case "AttachmentsC":
                            sql.Results_AttachmentsC(tableName: path, _as: _as);
                            break;
                        case "AttachmentsD":
                            sql.Results_AttachmentsD(tableName: path, _as: _as);
                            break;
                        case "AttachmentsE":
                            sql.Results_AttachmentsE(tableName: path, _as: _as);
                            break;
                        case "AttachmentsF":
                            sql.Results_AttachmentsF(tableName: path, _as: _as);
                            break;
                        case "AttachmentsG":
                            sql.Results_AttachmentsG(tableName: path, _as: _as);
                            break;
                        case "AttachmentsH":
                            sql.Results_AttachmentsH(tableName: path, _as: _as);
                            break;
                        case "AttachmentsI":
                            sql.Results_AttachmentsI(tableName: path, _as: _as);
                            break;
                        case "AttachmentsJ":
                            sql.Results_AttachmentsJ(tableName: path, _as: _as);
                            break;
                        case "AttachmentsK":
                            sql.Results_AttachmentsK(tableName: path, _as: _as);
                            break;
                        case "AttachmentsL":
                            sql.Results_AttachmentsL(tableName: path, _as: _as);
                            break;
                        case "AttachmentsM":
                            sql.Results_AttachmentsM(tableName: path, _as: _as);
                            break;
                        case "AttachmentsN":
                            sql.Results_AttachmentsN(tableName: path, _as: _as);
                            break;
                        case "AttachmentsO":
                            sql.Results_AttachmentsO(tableName: path, _as: _as);
                            break;
                        case "AttachmentsP":
                            sql.Results_AttachmentsP(tableName: path, _as: _as);
                            break;
                        case "AttachmentsQ":
                            sql.Results_AttachmentsQ(tableName: path, _as: _as);
                            break;
                        case "AttachmentsR":
                            sql.Results_AttachmentsR(tableName: path, _as: _as);
                            break;
                        case "AttachmentsS":
                            sql.Results_AttachmentsS(tableName: path, _as: _as);
                            break;
                        case "AttachmentsT":
                            sql.Results_AttachmentsT(tableName: path, _as: _as);
                            break;
                        case "AttachmentsU":
                            sql.Results_AttachmentsU(tableName: path, _as: _as);
                            break;
                        case "AttachmentsV":
                            sql.Results_AttachmentsV(tableName: path, _as: _as);
                            break;
                        case "AttachmentsW":
                            sql.Results_AttachmentsW(tableName: path, _as: _as);
                            break;
                        case "AttachmentsX":
                            sql.Results_AttachmentsX(tableName: path, _as: _as);
                            break;
                        case "AttachmentsY":
                            sql.Results_AttachmentsY(tableName: path, _as: _as);
                            break;
                        case "AttachmentsZ":
                            sql.Results_AttachmentsZ(tableName: path, _as: _as);
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
                                    idColumn: Rds.IdColumn(SiteSettings.ReferenceType),
                                    _as: Joined
                                        ? path + ",ItemTitle"
                                        : "ItemTitle");
                            break;
                    }
                    break;
            }
        }
    }
}
