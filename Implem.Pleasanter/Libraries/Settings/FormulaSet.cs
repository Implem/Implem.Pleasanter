using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class FormulaSet : ISettingListItem
    {
        public int Id { get; set; }
        public string CalculationMethod;
        public string Target;
        public int? Condition;
        public Formula Formula;
        public bool? NotUseDisplayName;
        public bool IsDisplayError;
        public Formula OutOfCondition;
        public string FormulaScript;
        public string FormulaMapping;

        public enum CalculationMethods
        {
            Default,
            Script
        }

        public FormulaSet GetRecordingData()
        {
            var formulaSet = new FormulaSet();
            formulaSet.Id = Id;
            formulaSet.CalculationMethod = CalculationMethod;
            formulaSet.Target = Target;
            formulaSet.Condition = Condition;
            formulaSet.Formula = Formula;
            formulaSet.NotUseDisplayName = NotUseDisplayName;
            formulaSet.IsDisplayError = IsDisplayError;
            formulaSet.OutOfCondition = OutOfCondition;
            formulaSet.FormulaScript = FormulaScript;
            formulaSet.FormulaMapping = FormulaMapping;
            return formulaSet;
        }
    }
}