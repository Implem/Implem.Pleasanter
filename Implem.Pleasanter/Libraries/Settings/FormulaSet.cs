using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class FormulaSet
    {
        public int Id;
        public string Target;
        public int? Condition;
        public Formula Formula;
        public Formula OutOfCondition;

        public string ToString(SiteSettings ss)
        {
            var data = string.Empty;
            var target = ss.FormulaColumn(Target);
            if (target != null)
            {
                data = target.LabelText + " = ";
                if (Formula != null)
                {
                    data += Formula.ToString(ss);
                }
                var view = ss.Views?.FirstOrDefault(o => o.Id == Condition);
                if (view != null)
                {
                    data += " | " + view.Name;
                    if (OutOfCondition != null)
                    {
                        data += " | " + OutOfCondition.ToString(ss);
                    }
                }
            }
            return data;
        }

        public Formula GetFormula(SiteSettings ss, SqlSelect sqlSelect, SqlWhereCollection where)
        {
            var view = ss.Views?.FirstOrDefault(o => o.Id == Condition);
            if (view != null)
            {
                sqlSelect.SqlWhereCollection = view.Where(ss, where);
                if (Rds.ExecuteTable(statements: sqlSelect).Rows.Count == 1)
                {
                    return Formula;
                }
                else
                {
                    return OutOfCondition;
                }
            }
            else
            {
                return Formula;
            }
        }
    }
}