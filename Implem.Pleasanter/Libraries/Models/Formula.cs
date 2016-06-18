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
        public decimal Result = 0;

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

        public decimal GetResult(Dictionary<string, decimal> data)
        {
            Children?.ForEach(formula => formula.SetValue(data));
            Formula before = null;
            Children?.ForEach(formula =>
            {
                var result = formula.GetResult(data);
                switch (formula.OperatorType)
                {
                    case OperatorTypes.Multiplication:
                        before.Result *= result;
                        break;
                    case OperatorTypes.Division:
                        before.Result = result != 0
                            ? before.Result / result
                            : 0;
                        break;
                }
                before = formula;
            });
            Children?.ForEach(formula =>
            {
                switch (formula.OperatorType)
                {
                    case OperatorTypes.NotSet:
                    case OperatorTypes.Addition:
                        Result += formula.GetResult(data);
                        break;
                    case OperatorTypes.Subtraction:
                        Result -= formula.GetResult(data);
                        break;
                }
            });
            return Result;
        }

        public void SetValue(Dictionary<string, decimal> data)
        {
            Result = ColumnName != null && data.ContainsKey(ColumnName)
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