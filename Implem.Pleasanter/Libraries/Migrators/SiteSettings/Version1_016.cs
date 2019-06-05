using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_016
    {
        public static void Migrate1_016(this SiteSettings ss)
        {
            if (ss.EditInDialog == true)
            {
                ss.GridEditorType = SiteSettings.GridEditorTypes.Dialog;
            }
            ss.Migrated = true;
        }
    }
}