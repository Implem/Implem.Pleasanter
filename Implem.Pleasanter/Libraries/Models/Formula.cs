using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public class Formula
    {
        public string ColumnName;
        public decimal Value;
        public OperatorTypes OperatorType;
        public SortedList<int, Formula> Right;

        public enum OperatorTypes
        {
            Addition,
            Subtraction,
            Multiplication,
            Division
        }

        public Formula(string columnName)
        {
            ColumnName = columnName;
        }

        public Formula(decimal value)
        {
            Value = value;
        }

        public Formula Add(Formula formula)
        {
            if (Right == null) Right = new SortedList<int, Formula>();
            Right.Add(Right.Count(), formula);
            return formula;
        }

        public decimal Result(Dictionary<string, decimal> data, decimal left = 0)
        {
            var result = left;
            Right.Values.ForEach(fomura => result = fomura.Result(data, result));
            switch (OperatorType)
            {
                case OperatorTypes.Addition:
                    return result + GetValue(data);
                case OperatorTypes.Subtraction:
                    return result - GetValue(data);
                case OperatorTypes.Multiplication:
                    return result * GetValue(data);
                case OperatorTypes.Division:
                    var right = GetValue(data);
                    return right != 0
                        ? result / GetValue(data)
                        : 0;
                default:
                    return result;
            }
        }

        private decimal GetValue(Dictionary<string, decimal> data)
        {
            return ColumnName != null && data.ContainsKey(ColumnName)
                ? data[ColumnName]
                : Value;
        }
    }
}