using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public class Formula
    {
        public string ColumnName;
        public decimal? Value;
        public OperatorTypes OperatorType = OperatorTypes.NotSet;
        public List<Formula> Children;
        [NonSerialized]
        public decimal Result;

        public enum OperatorTypes
        {
            NotSet,
            Addition,
            Subtraction,
            Multiplication,
            Division
        }

        public Formula()
        {
        }

        public Formula(OperatorTypes operatorType)
        {
            OperatorType = operatorType;
        }

        public Formula Add(Formula formula)
        {
            if (Children == null) Children = new List<Formula>();
            Children.Add(formula);
            return formula;
        }

        public decimal GetResult(Dictionary<string, decimal> data, decimal left = 0)
        {
            Result = left;
            Children.Where(o =>
                o.OperatorType == OperatorTypes.Multiplication ||
                o.OperatorType == OperatorTypes.Subtraction).ForEach(fomura =>
                    Result = fomura.GetResult(data, Result));
            Children.Where(o =>
                o.OperatorType == OperatorTypes.Addition ||
                o.OperatorType == OperatorTypes.Subtraction).ForEach(fomura =>
                    Result = fomura.GetResult(data, Result));
            switch (OperatorType)
            {
                case OperatorTypes.Addition:
                    return Result + GetValue(data);
                case OperatorTypes.Subtraction:
                    return Result - GetValue(data);
                case OperatorTypes.Multiplication:
                    return Result * GetValue(data);
                case OperatorTypes.Division:
                    var right = GetValue(data);
                    return right != 0
                        ? Result / GetValue(data)
                        : 0;
                default:
                    return Result;
            }
        }

        private decimal GetValue(Dictionary<string, decimal> data)
        {
            return ColumnName != null && data.ContainsKey(ColumnName)
                ? data[ColumnName]
                : Value.ToDecimal();
        }

        public string Text()
        {
            return string.Empty;
        }

        public bool Completion()
        {
            return
                ColumnName != null || Value != null ||
                (Children != null && (Children.Last().ColumnName != null || Children.Last().Value != null));
        }

        public string ToString(SiteSettings siteSettings, bool child = false)
        {
            var formula = string.Empty;
            switch (OperatorType)
            {
                case OperatorTypes.Addition: formula += " + "; break;
                case OperatorTypes.Subtraction: formula += " - "; break;
                case OperatorTypes.Multiplication: formula += " * "; break;
                case OperatorTypes.Division: formula += " / "; break;
            }
            if (ColumnName != null)
            {
                formula += siteSettings.FormulaColumn(ColumnName).LabelText;
            }
            if (Value != null)
            {
                formula += Value;
            }
            if (Children != null)
            {
                if (Children.Count == 1 || !child)
                {
                    formula += RightToString(siteSettings);
                }
                else
                {
                    formula += "(" + RightToString(siteSettings) + ")";
                }
            }
            return formula;
        }

        private string RightToString(SiteSettings siteSettings)
        {
            return Children.Select(o => o.ToString(siteSettings, child: true)).Join(string.Empty);
        }
    }
}