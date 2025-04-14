using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelGrid
    {
        public Context Context { get; set; }
        public SiteSettings SiteSettings { get; set; }
        public int TotalCount { get; set; }

        public ServerScriptModelGrid(
            Context context,
            SiteSettings ss,
            int totalCount)
        {
            Context = context;
            SiteSettings = ss;
            TotalCount = totalCount;
        }

        public List<long> SelectedIds()
        {
            switch (SiteSettings.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.SelectedIdsByServerScript(
                        context: Context,
                        ss: SiteSettings);
                case "Results":
                    return ResultUtilities.SelectedIdsByServerScript(
                        context: Context,
                        ss: SiteSettings);
                default:
                    return null;
            }
        }

        public List<long> GetBulkDeleteIds()
        {
            switch (SiteSettings.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.GetBulkDeleteIds();
                case "Results":
                    return ResultUtilities.GetBulkDeleteIds();
                default:
                    return null;
            }
        }
    }
}
