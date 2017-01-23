using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_005
    {
        public static void Migrate1_005(this Settings.SiteSettings ss)
        {
            var id = 1;
            ss.FormulaHash?.ForEach(data =>
            {
                if (ss.Formulas == null) ss.Formulas = new SettingList<FormulaSet>();
                ss.Formulas.Add(new FormulaSet()
                {
                    Id = id,
                    Target = data.Key,
                    Formula = data.Value
                });
                id++;
            });
            ss.FormulaHash = null;
            ss.Migrated = true;
        }
    }
}