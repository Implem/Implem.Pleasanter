using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
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
            Dictionary<string, IEnumerable<string>> linkHash,
            IEnumerable<string> searchIndexes)
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
                ChoiceHash = ChoiceHash.Take(Parameters.General.DropDownSearchLimit)
                    .ToDictionary(o => o.Key, o => o.Value);
            }
        }

        private void SetChoiceHash(
            int tenantId,
            long siteId,
            Dictionary<string, IEnumerable<string>> linkHash,
            IEnumerable<string> searchIndexes,
            int index,
            string line)
        {
            switch (line)
            {
                case "[[Depts]]":
                    SiteInfo.TenantCaches[tenantId].DeptHash
                        .Where(o => o.Value.TenantId == tenantId)
                        .ForEach(o => AddToChoiceHash(
                            o.Key.ToString(),
                            SiteInfo.Dept(o.Key).Name));
                    break;
                case "[[Users]]":
                    SiteInfo.SiteUsers(tenantId, siteId)
                        .ToDictionary(o => o.ToString(), o => SiteInfo.UserName(o))
                        .Where(o => searchIndexes?.Any() != true ||
                            searchIndexes.All(p =>
                                o.Key.Contains(p) ||
                                o.Value.Contains(p)))
                        .ForEach(o => AddToChoiceHash(o.Key, o.Value));
                    break;
                case "[[Users*]]":
                    SiteInfo.TenantCaches[tenantId].UserHash
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
                    if (linkHash != null && linkHash.ContainsKey(line))
                    {
                        linkHash[line].ForEach(value =>
                            AddToChoiceHash(value));
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
            if (addNotSet && !Required)
            {
                hash.Add("\t", new ControlData(Displays.NotSet()));
            }
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
            var choice = Choice(selectedValue, nullCase: selectedValue);
            switch (type)
            {
                case ExportColumn.Types.Value: return choice.Value;
                case ExportColumn.Types.Text: return choice.Text;
                case ExportColumn.Types.TextMini: return choice.TextMini;
                default: return choice.Text;
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

        private static string TrimZero(string str)
        {
            return str.Contains(".")
                ? str.TrimEnd('0')
                : str;
        }

        public string Display(SiteSettings ss, decimal value, bool format = true)
        {
            return Display(value, format: format) + (EditorReadOnly.ToBool() || !CanUpdate
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
    }
}