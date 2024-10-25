using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using System;
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
            bool isDisplayError,
            string outOfCondition)
        {
            var formulaSet = new FormulaSet();
            var err = SetFormula(
                formulaSet: formulaSet,
                ss: ss,
                calculationMethod: calculationMethod,
                target: target,
                condition: condition,
                formula: formula,
                notUseDisplayName: notUseDisplayName,
                isDisplayError: isDisplayError,
                outOfCondition: outOfCondition);
            if (err == Error.Types.None)
            {
                formulaSet.Id = ss.Formulas?.Max(o => o.Id + 1) ?? 1;
                (ss.Formulas ??= new SettingList<FormulaSet>()).Add(formulaSet);
            }
            return err;
        }

        public static Error.Types UpdateFormula(
            this SiteSettings ss,
            int id,
            string calculationMethod,
            string target,
            int? condition,
            string formula,
            bool notUseDisplayName,
            bool isDisplayError,
            string outOfCondition)
        {
            var formulaSet = ss.Formulas.FirstOrDefault(o => o.Id == id);
            if (formulaSet == null)
            {
                return Error.Types.NotFound;
            }
            return SetFormula(
                formulaSet: formulaSet,
                ss: ss,
                calculationMethod: calculationMethod,
                target: target,
                condition: condition,
                formula: formula,
                notUseDisplayName: notUseDisplayName,
                isDisplayError: isDisplayError,
                outOfCondition: outOfCondition);
        }

        public static Error.Types SetFormula(
            FormulaSet formulaSet,
            SiteSettings ss,
            string calculationMethod,
            string target,
            int? condition,
            string formula,
            bool notUseDisplayName,
            bool isDisplayError,
            string outOfCondition)
        {
            if (ss.FormulaColumn(target, calculationMethod) == null)
            {
                return Error.Types.InvalidFormula;
            }
            formulaSet.CalculationMethod = string.IsNullOrEmpty(calculationMethod)
                ? FormulaSet.CalculationMethods.Default.ToString()
                : calculationMethod;
            formulaSet.Target = target;
            formulaSet.Condition = ss.Views?.Get(condition) != null
                ? condition
                : null;
            formulaSet.NotUseDisplayName = notUseDisplayName;
            formulaSet.IsDisplayError = isDisplayError;
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
                formulaSet = UpdateColumnDisplayText(
                    ss: ss,
                    formulaSet: formulaSet);
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

        public static string ParseFormulaScript(SiteSettings ss, string formulaScript, string calculationMethod)
        {
            var columns = System.Text.RegularExpressions.Regex.Matches(formulaScript, @"\[([^]]*)\]");
            var columnList = ss.FormulaColumnList();
            foreach (var column in columns)
            {
                var columnParam = column.ToString()[1..^1];
                if (columnList.Any(o => o.ColumnName == columnParam))
                {
                    formulaScript = formulaScript.Replace(column.ToString(), $"model.{columnParam}");
                }
            }
            foreach (var column in columnList)
            {
                formulaScript = System.Text.RegularExpressions.Regex.Replace(
                    input: formulaScript,
                    pattern: System.Text.RegularExpressions.Regex.Escape(column.LabelText)
                        + $"(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)",
                    replacement: $"model.{column.ColumnName}");
            }
            return formulaScript.Replace("true", "true", StringComparison.InvariantCultureIgnoreCase)
                .Replace("\"true\"", "true", StringComparison.InvariantCultureIgnoreCase)
                .Replace("false", "false", StringComparison.InvariantCultureIgnoreCase)
                .Replace("\"false\"", "false", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string ParseFormulaColumnName(SiteSettings ss, string formulaScript)
        {
            var columnList = ss.FormulaColumnList();
            foreach (var column in columnList)
            {
                formulaScript = System.Text.RegularExpressions.Regex.Replace(
                    input: formulaScript,
                    pattern: System.Text.RegularExpressions.Regex.Escape(column.LabelText)
                        + $"(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)",
                    replacement: $"[{column.ColumnName}]");
            }
            return formulaScript;
        }

        public static string ParseFormulaLabel(SiteSettings ss, string formulaScript)
        {
            var columnList = ss.FormulaColumnList();
            var columns = System.Text.RegularExpressions.Regex.Matches(formulaScript, @"\[([^]]*)\]");
            foreach (var column in columns)
            {
                var columnParam = column.ToString()[1..^1];
                if (ss.FormulaColumn(columnParam, "Extended") != null)
                {
                    formulaScript = formulaScript.Replace(
                        oldValue: column.ToString(),
                        newValue: columnList.SingleOrDefault(o => o.ColumnName == columnParam).LabelText);
                }
            }
            return formulaScript;
        }

        public static FormulaSet UpdateColumnDisplayText(SiteSettings ss, FormulaSet formulaSet)
        {
            var columnList = ss.FormulaColumnList();
            var formulaDictionary = new Dictionary<string, string>();
            var oldMapping = Jsons.Deserialize<Dictionary<string, string>>(formulaSet.FormulaMapping);
            foreach (var column in columnList)
            {
                if (oldMapping != null && oldMapping.ContainsKey(column.ColumnName))
                {
                    formulaSet.FormulaScript = System.Text.RegularExpressions.Regex.Replace(
                        input: formulaSet.FormulaScript,
                        pattern: System.Text.RegularExpressions.Regex.Escape(oldMapping.Get(column.ColumnName))
                            + $"(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)",
                        replacement: column.LabelText);
                }
                var isMatched = System.Text.RegularExpressions.Regex.IsMatch(
                    input: formulaSet.FormulaScript,
                    pattern: System.Text.RegularExpressions.Regex.Escape(column.LabelText)
                        + $"(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                if (isMatched)
                {
                    formulaDictionary.Add(column.ColumnName, column.LabelText);
                }
            }
            formulaSet.FormulaMapping = formulaDictionary.ToJson();
            return formulaSet;
        }
    }
}