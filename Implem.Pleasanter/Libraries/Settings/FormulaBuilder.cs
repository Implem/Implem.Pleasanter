using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class FormulaBuilder
    {
        public static Error.Types AddFormula(
            this SiteSettings ss,
            string calculationMethod,
            string target,
            int? condition,
            string formula,
            bool notUseDisplayName,
            string outOfCondition)
        {
            if (ss.FormulaColumn(target, calculationMethod) == null)
            {
                return Error.Types.InvalidFormula;
            }
            var formulaSet = new FormulaSet()
            {
                Id = ss.Formulas?.Any() == true
                    ? ss.Formulas.Max(o => o.Id) + 1
                    : 1,
                CalculationMethod = calculationMethod,
                Target = target,
                Condition = ss.Views?.Get(condition) != null
                    ? condition
                    : null,
                NotUseDisplayName = notUseDisplayName
            };
            IEnumerable<string> formulaParts = null;
            if (string.IsNullOrEmpty(calculationMethod) || calculationMethod == FormulaSet.CalculationMethods.Default.ToString())
            {
                formulaParts = Parts(formula);
                if (!formulaParts.Any())
                {
                    return Error.Types.InvalidFormula;
                }
            }
            else
            {
                formulaSet.FormulaScript = formula;
            }
            var outOfConditionParts = Parts(outOfCondition);
            if (!outOfCondition.IsNullOrEmpty() && !outOfConditionParts.Any())
            {
                return Error.Types.InvalidFormula;
            }
            if (string.IsNullOrEmpty(calculationMethod) || calculationMethod == FormulaSet.CalculationMethods.Default.ToString())
            {
                var error1 = Get(ss, formulaParts, out formulaSet.Formula);
                if (error1.Has()) return error1;
            }
            if (formulaSet.Condition != null && outOfConditionParts != null)
            {
                var error2 = Get(ss, outOfConditionParts, out formulaSet.OutOfCondition);
                if (error2.Has()) return error2;
            }
            if (ss.Formulas == null) ss.Formulas = new SettingList<FormulaSet>();
            ss.Formulas.Add(formulaSet);
            return Error.Types.None;
        }

        public static Error.Types UpdateFormula(
            this SiteSettings ss,
            int id,
            string calculationMethod,
            string target,
            int? condition,
            string formula,
            bool notUseDisplayName,
            string outOfCondition)
        {
            var formulaSet = ss.Formulas.FirstOrDefault(o => o.Id == id);
            if (formulaSet == null)
            {
                return Error.Types.NotFound;
            }
            if (ss.FormulaColumn(target, calculationMethod) == null)
            {
                return Error.Types.InvalidFormula;
            }
            formulaSet.CalculationMethod = calculationMethod;
            formulaSet.Target = target;
            formulaSet.Condition = ss.Views?.Get(condition) != null
                ? condition
                : null;
            formulaSet.NotUseDisplayName = notUseDisplayName;
            IEnumerable<string> formulaParts = null;
            if (string.IsNullOrEmpty(calculationMethod) || calculationMethod == FormulaSet.CalculationMethods.Default.ToString())
            {
                formulaParts = Parts(formula);
                if (!formulaParts.Any())
                {
                    return Error.Types.InvalidFormula;
                }
            }
            else
            {
                formulaSet.FormulaScript = formula;
            }
            var outOfConditionParts = Parts(outOfCondition);
            if (!outOfCondition.IsNullOrEmpty() && !outOfConditionParts.Any())
            {
                return Error.Types.InvalidFormula;
            }
            if (string.IsNullOrEmpty(calculationMethod) || calculationMethod == FormulaSet.CalculationMethods.Default.ToString())
            {
                var error1 = Get(ss, formulaParts, out formulaSet.Formula);
                if (error1.Has()) return error1;
            }
            if (formulaSet.Condition != null && outOfConditionParts != null)
            {
                var error2 = Get(ss, outOfConditionParts, out formulaSet.OutOfCondition);
                if (error2.Has()) return error2;
            }
            else
            {
                formulaSet.OutOfCondition = null;
            }
            return Error.Types.None;
        }

        private static IEnumerable<string> Parts(string formula)
        {
            return formula?
                .Replace("(", "( ")
                .Replace(")", " )")
                .Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
        }

        private static Error.Types Get(
            SiteSettings ss, IEnumerable<string> parts, out Formula formula)
        {
            var stack = new Stack<Formula>();
            formula = new Formula();
            var current = new Formula();
            var error = Error.Types.None;
            formula.Add(current);
            stack.Push(formula);
            foreach (var part in parts)
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