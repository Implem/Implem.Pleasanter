using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class FormulaBuilder
    {
        public static Error.Types AddFormula(this SiteSettings ss, string formula)
        {
            var parts = formula
                .Replace("(", "( ")
                .Replace(")", " )")
                .Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
            if (parts.Count() < 3)
            {
                return Error.Types.InvalidFormula;
            }
            var target = ss.FormulaColumn(parts.Take(1).Last());
            if (target == null)
            {
                return Error.Types.InvalidFormula;
            }
            if (ss.FormulaHash.Keys.Contains(target.ColumnName))
            {
                return Error.Types.AlreadyAdded;
            }
            if (parts.Take(2).Last() != "=")
            {
                return Error.Types.InvalidFormula;
            }
            var stack = new Stack<Formula>();
            var root = new Formula();
            var current = new Formula();
            var error = Error.Types.None;
            root.Add(current);
            stack.Push(root);
            foreach (var part in parts.Skip(2))
            {
                switch (part)
                {
                    case "+":
                        error = AddFormulaOperator(
                            stack, current, Formula.OperatorTypes.Addition);
                        if (error.Has()) return error;
                        break;
                    case "-":
                        error = AddFormulaOperator(
                            stack, current, Formula.OperatorTypes.Subtraction);
                        if (error.Has()) return error;
                        break;
                    case "*":
                        error = AddFormulaOperator(
                            stack, current, Formula.OperatorTypes.Multiplication);
                        if (error.Has()) return error;
                        break;
                    case "/":
                        error = AddFormulaOperator(
                            stack, current, Formula.OperatorTypes.Division);
                        if (error.Has()) return error;
                        break;
                    case "(":
                        Formula container;
                        if (stack.First().Children.Last().OperatorType !=
                            Formula.OperatorTypes.NotSet)
                        {
                            container = stack.First().Children.Last();
                        }
                        else
                        {
                            container = new Formula();
                            stack.First().Add(container);
                        }
                        container.Add(new Formula());
                        stack.Push(container);
                        break;
                    case ")":
                        if (stack.Count <= 1)
                        {
                            return Error.Types.InvalidFormula;
                        }
                        stack.Pop();
                        break;
                    default:
                        var columnName = ss.FormulaColumn(part)?.ColumnName;
                        if (columnName != null)
                        {
                            stack.First().Children.Last().ColumnName = columnName;
                        }
                        else if (part.RegexExists(@"^[0-9]*\.?[0-9]+$"))
                        {
                            stack.First().Children.Last().RawValue = part.ToDecimal();
                        }
                        else
                        {
                            return Error.Types.InvalidFormula;
                        }
                        break;
                }
            }
            if (stack.Count != 1)
            {
                return Error.Types.InvalidFormula;
            }
            ss.FormulaHash.Add(target.ColumnName, root);
            return Error.Types.None;
        }

        private static Error.Types AddFormulaOperator(
            Stack<Formula> stack,
            Formula current,
            Formula.OperatorTypes operatorType)
        {
            if (!stack.First().Children.Last().Completion())
            {
                return Error.Types.InvalidFormula;
            }
            else
            {
                current = new Formula(operatorType);
                stack.First().Add(current);
                return Error.Types.None;
            }
        }
    }
}