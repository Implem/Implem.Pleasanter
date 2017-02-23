using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
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
        public bool Nullable;
        [NonSerialized]
        public bool RecordedTime;
        [NonSerialized]
        public Permissions.Types ReadPermission;
        [NonSerialized]
        public Permissions.Types CreatePermission;
        [NonSerialized]
        public Permissions.Types UpdatePermission;
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
        public bool MarkDown;
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
                    SiteInfo.DeptHash
                        .Where(o => o.Value.TenantId == tenantId)
                        .ForEach(o => AddToChoiceHash(
                            o.Key.ToString(),
                            SiteInfo.Dept(o.Key).Name));
                    break;
                case "[[Users]]":
                    SiteInfo.SiteUsers(siteId)
                        .ToDictionary(o => o.ToString(), o => SiteInfo.UserFullName(o))
                        .Where(o => searchIndexes?.Any() != true ||
                            searchIndexes.All(p =>
                                o.Key.Contains(p) ||
                                o.Value.Contains(p)))
                        .ForEach(o => AddToChoiceHash(o.Key, o.Value));
                    break;
                case "[[Users*]]":
                    SiteInfo.UserHash
                        .Where(o => o.Value.TenantId == tenantId)
                        .ToDictionary(o => o.Key.ToString(), o => o.Value.FullName())
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
            var editChoices = new Dictionary<string, ControlData>();
            if (!HasChoices()) return editChoices;
            if (insertBlank && CanEmpty())
            {
                editChoices.Add(
                    UserColumn
                        ? User.UserTypes.Anonymous.ToInt().ToString()
                        : string.Empty,
                    new ControlData(string.Empty));
            }
            ChoiceHash?.Values
                .GroupBy(o => o.Value)
                .Select(o => o.FirstOrDefault())
                .ForEach(choice =>
                    editChoices.Add(
                        choice.Value,
                        new ControlData(
                            text: choice.Text,
                            css: choice.CssClass,
                            style: choice.Style)));
            if (addNotSet && Nullable)
            {
                editChoices.Add("\t", new ControlData(Displays.NotSet()));
            }
            return editChoices;
        }

        private bool CanEmpty()
        {
            return Nullable && ValidateRequired != true;
        }

        public Choice Choice(string selectedValue)
        {
            return ChoiceHash != null && ChoiceHash.ContainsKey(selectedValue)
                ? ChoiceHash[selectedValue]
                : new Choice(string.Empty);
        }

        public object RecordingData(string value, long siteId)
        {
            object recordingData = value;
            if (UserColumn)
            {
                recordingData = SiteInfo.UserHash
                    .Where(o => o.Value.FullName() == value)
                    .Select(o => o.Value.Id)
                    .FirstOrDefault(o => SiteInfo.SiteUsers(siteId).Any(p => p == o));
            }
            else if (TypeCs == "Comments")
            {
                return new Comments().Prepend(value).ToJson();
            }
            else if (HasChoices())
            {
                recordingData = ChoiceHash.Where(o => o.Value.Text == value)
                    .Select(o => o.Value.Value)
                    .FirstOrDefault();
            }
            if (recordingData == null) return null;
            switch (TypeName)
            {
                case "bit": return recordingData.ToBool();
                case "int": return recordingData.ToInt();
                case "bigint": return recordingData.ToLong();
                case "float": return recordingData.ToFloat();
                case "decimal": return recordingData.ToDecimal();
                case "datetime": return recordingData.ToDateTime().ToUniversal();
                case "nchar":
                case "nvarchar": return recordingData;
                default: return recordingData;
            }
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

        public string Display(decimal value, Permissions.Types pt)
        {
            return Display(value) + (EditorReadOnly.ToBool() || !this.CanUpdate(pt)
                ? Unit
                : string.Empty);
        }

        public string DisplayGrid(DateTime value)
        {
            return Display(value, GridFormat);
        }

        public string DisplayControl(DateTime value)
        {
            return Display(value, EditorFormat);
        }

        public string DisplayExport(DateTime value)
        {
            return Display(value, ExportFormat);
        }

        private string Display(DateTime value, string format)
        {
            return value.InRange()
                ? !format.IsNullOrEmpty()
                    ? value.ToString(
                        Displays.Get(format + "Format"),
                        Sessions.CultureInfo())
                    : value.ToString(Sessions.CultureInfo())
                : string.Empty;
        }

        public decimal Round(decimal value)
        {
             return Math.Round(value, DecimalPlaces.ToInt(), MidpointRounding.AwayFromZero);
        }
    }
}