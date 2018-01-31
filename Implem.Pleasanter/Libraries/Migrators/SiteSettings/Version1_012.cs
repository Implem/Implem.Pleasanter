using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_012
    {
        public static void Migrate1_012(this SiteSettings ss)
        {
            MigrateStyles(ss);
            MigrateScripts(ss);
            ss.Migrated = true;
        }

        private static void MigrateStyles(SiteSettings ss)
        {
            ss.Styles = new SettingList<Style>();
            if (!ss.NewStyle.IsNullOrEmpty())
            {
                var id = ss.Styles.Count() + 1;
                ss.Styles.Add(new Style()
                {
                    Id = id,
                    Title = "Style" + id,
                    New = true,
                    Body = ss.NewStyle
                });
            }
            if (!ss.EditStyle.IsNullOrEmpty())
            {
                var id = ss.Styles.Count() + 1;
                ss.Styles.Add(new Style()
                {
                    Id = id,
                    Title = "Style" + id,
                    Edit = true,
                    Body = ss.EditStyle
                });
            }
            if (!ss.GridStyle.IsNullOrEmpty())
            {
                var id = ss.Styles.Count() + 1;
                ss.Styles.Add(new Style()
                {
                    Id = id,
                    Title = "Style" + id,
                    Index = true,
                    Body = ss.GridStyle
                });
            }
        }

        private static void MigrateScripts(SiteSettings ss)
        {
            ss.Scripts = new SettingList<Script>();
            if (!ss.NewScript.IsNullOrEmpty())
            {
                var id = ss.Scripts.Count() + 1;
                ss.Scripts.Add(new Script()
                {
                    Id = id,
                    Title = "Script" + id,
                    New = true,
                    Body = ss.NewScript
                });
            }
            if (!ss.EditScript.IsNullOrEmpty())
            {
                var id = ss.Scripts.Count() + 1;
                ss.Scripts.Add(new Script()
                {
                    Id = id,
                    Title = "Script" + id,
                    Edit = true,
                    Body = ss.EditScript
                });
            }
            if (!ss.GridScript.IsNullOrEmpty())
            {
                var id = ss.Scripts.Count() + 1;
                ss.Scripts.Add(new Script()
                {
                    Id = id,
                    Title = "Script" + id,
                    Index = true,
                    Body = ss.GridScript
                });
            }
        }
    }
}