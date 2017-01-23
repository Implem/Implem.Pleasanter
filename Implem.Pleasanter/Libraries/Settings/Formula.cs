using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Formula
    {
        public string ColumnName;
        public decimal? RawValue;
        public OperatorTypes OperatorType = OperatorTypes.NotSet;
        public List<Formula> Children;
        [NonSerialized]
        private bool Processed;
        [NonSerialized]
        private decimal Result = 0;

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

        public decimal GetResult(Dictionary<string, decimal> data, Column column)
        {
            return column != null
                ? column.Round(GetResult(data))
                : 0;
        }

        public decimal GetResult(Dictionary<string, decimal> data, bool root = true)
        {
            if (root) ClearResults();
            if (!Processed)
            {
                Result = ColumnName != null && data.ContainsKey(ColumnName)
                    ? data[ColumnName]
                    : RawValue.ToDecimal();
                Formula before = null;
                Children?.ForEach(formula =>
                {
                    var result = formula.GetResult(data, root: false);
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
                        default:
                            before = formula;
                            break;
                    }
                });
                Children?.ForEach(formula =>
                {
                    var result = formula.GetResult(data, root: false);
                    switch (formula.OperatorType)
                    {
                        case OperatorTypes.NotSet:
                        case OperatorTypes.Addition:
                            Result += result;
                            break;
                        case OperatorTypes.Subtraction:
                            Result -= result;
                            break;
                    }
                });
                Processed = true;
            }
            return Result;
        }

        private void ClearResults()
        {
            Processed = false;
            Result = 0;
            Children?.ForEach(formula => formula.ClearResults());
        }

        public bool Completion()
        {
            return
                ColumnName != null || RawValue != null ||
                (Children != null && (
                    Children.Last().ColumnName != null ||
                    Children.Last().RawValue != null));
        }

        public string ToString(SiteSettings ss, bool child = false)
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
                formula += ss.FormulaColumn(ColumnName).LabelText;
            }
            if (RawValue != null)
            {
                formula += RawValue.TrimEndZero();
            }
            if (Children != null)
            {
                if (Children.Count == 1 || !child)
                {
                    formula += ChildrenToString(ss);
                }
                else
                {
                    formula += "(" + ChildrenToString(ss) + ")";
                }
            }
            return formula;
        }

        private string ChildrenToString(SiteSettings ss)
        {
            return Children.Select(o => o.ToString(ss, child: true)).Join(string.Empty);
        }
    }
}