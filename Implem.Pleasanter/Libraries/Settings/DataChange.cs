﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class DataChange : ISettingListItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Types
        {
            CopyValue,
            CopyDisplayValue,
            InputValue,
            InputValueFormula,
            InputDate,
            InputDateTime,
            InputUser,
            InputDept
        }

        public enum Periods
        {
            Days,
            Months,
            Years,
            Hours,
            Minutes,
            Seconds
        }
        public int Id { get; set; }
        public Types? Type { get; set; }
        public string ColumnName { get; set; }
        public string BaseDateTime { get; set; }
        public string Value { get; set; }
        public int? Delete { get; set; }
        public bool ValueFormulaNotUseDisplayName { get; set; }
        public bool ValueFormulaIsDisplayError { get; set; }

        public DataChange()
        {
        }

        public DataChange(
            int id,
            Types type,
            string columnName,
            string baseDateTime,
            string value,
            bool valueFormulaNotUseDisplayName,
            bool valueFormulaIsDisplayError)
        {
            Id = id;
            Type = type;
            ColumnName = columnName;
            BaseDateTime = baseDateTime;
            Value = value;
            ValueFormulaNotUseDisplayName = valueFormulaNotUseDisplayName;
            ValueFormulaIsDisplayError = valueFormulaIsDisplayError;
        }

        public void Update(
            Types type,
            string columnName,
            string baseDateTime,
            string value,
            bool valueFormulaNotUseDisplayName,
            bool valueFormulaIsDisplayError)
        {
            Type = type;
            ColumnName = columnName;
            BaseDateTime = baseDateTime;
            Value = value;
            ValueFormulaNotUseDisplayName = valueFormulaNotUseDisplayName;
            ValueFormulaIsDisplayError = valueFormulaIsDisplayError;
        }

        public DataChange GetRecordingData(
            Context context,
            SiteSettings ss)
        {
            var dataChange = new DataChange();
            dataChange.Id = Id;
            dataChange.Type = Type;
            if (!ColumnName.IsNullOrEmpty()) dataChange.ColumnName = ColumnName;
            switch (Type)
            {
                case Types.CopyValue:
                    break;
                case Types.CopyDisplayValue:
                    break;
                case Types.InputValue:
                    break;
                case Types.InputValueFormula:
                    dataChange.ValueFormulaIsDisplayError = ValueFormulaIsDisplayError;
                    dataChange.ValueFormulaNotUseDisplayName = ValueFormulaNotUseDisplayName;
                    break;
                case Types.InputDate:
                    if (BaseDateTime != "CurrentDate")
                    {
                        dataChange.BaseDateTime = BaseDateTime;
                    }
                    break;
                case Types.InputDateTime:
                    if (BaseDateTime != "CurrentDateTime")
                    {
                        dataChange.BaseDateTime = BaseDateTime;
                    }
                    break;
                case Types.InputUser:
                    break;
                case Types.InputDept:
                    break;
                default:
                    break;
            }
            dataChange.Value = Value;
            return dataChange;
        }

        public bool Visible(string type)
        {
            switch (Type)
            {
                case Types.CopyValue:
                case Types.CopyDisplayValue:
                    return type == "Column";
                case Types.InputValue:
                    return type == "Value";
                case Types.InputValueFormula:
                    return type == "ValueFormula";
                case Types.InputDate:
                case Types.InputDateTime:
                    return type == "DateTime";
                case Types.InputUser:
                case Types.InputDept:
                    return type == "Context";
                default:
                    return false;
            }
        }

        public string DisplayValue(Context context, SiteSettings ss)
        {
            if (Visible(type: "Column"))
            {
                return ss.GetColumn(
                    context: context,
                    columnName: Value)?.LabelText;
            }
            else if (Visible(type: "DateTime"))
            {
                var period = Displays.Get(
                    context: context,
                    id: DateTimePeriod());
                return $"{DateTimeNumber()} {period}";
            }
            else if (Visible(type: "ValueFormula"))
            {
                if (ValueFormulaNotUseDisplayName != false) return Value;
                return  FormulaBuilder.ParseFormulaLabel(
                    ss: ss,
                    formulaScript: Value);
            }
            else
            {
                return ss.ColumnNameToLabelText(Value);
            }
        }

        public int DateTimeNumber()
        {
            return Value?.Split_1st().ToInt() ?? 0;
        }

        public string DateTimePeriod()
        {
            return Value?.Split_2nd();
        }

        public string ValueData(Dictionary<string, string> KeyValues)
        {
            var ret = Value;
            if (KeyValues.Any())
            {
                var temp = new Dictionary<string, string>();
                KeyValues.ForEach(data =>
                {
                    var guid = $"[{Strings.NewGuid()}]";
                    ret = ret.Replace($"[{data.Key}]", guid);
                    temp.Add(guid, data.Key);
                });
                temp.ForEach(data =>
                {
                    ret = ret.Replace(data.Key, KeyValues.Get(data.Value) ?? string.Empty);
                });
            }
            return ret;
        }

        public string DateTimeValue(Context context, DateTime baseDateTime)
        {
            var ret = baseDateTime;
            switch (BaseDateTime ?? (Type == Types.InputDate
                ? "CurrentDate"
                : "CurrentTime"))
            {
                case "CurrentDate":
                    ret = DateTime.Now.ToLocal(context: context).Date;
                    break;
                case "CurrentTime":
                    ret = Type == Types.InputDate
                        ? DateTime.Now.ToLocal(context: context).Date
                        : DateTime.Now.ToLocal(context: context);
                    break;
                default:
                    if (!ret.InRange())
                    {
                        return string.Empty;
                    }
                    break;
            }
            var number = DateTimeNumber();
            if (number != 0)
            {
                var period = DateTimePeriod();
                switch (period)
                {
                    case "Days":
                        ret = ret.AddDays(number);
                        break;
                    case "Months":
                        ret = ret.AddMonths(number);
                        break;
                    case "Years":
                        ret = ret.AddYears(number);
                        break;
                    case "Hours":
                        ret = ret.AddHours(number);
                        break;
                    case "Minutes":
                        ret = ret.AddMinutes(number);
                        break;
                    case "Seconds":
                        ret = ret.AddSeconds(number);
                        break;
                }
            }
            switch (Type)
            {
                case Types.InputDate:
                    return ret.ToString("d", context.CultureInfoCurrency(context.Language));
                default:
                    return ret.ToString("G", context.CultureInfoCurrency(context.Language));
            }
        }
    }
}