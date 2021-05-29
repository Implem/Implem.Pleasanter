using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Lookups : List<Lookup>
    {
        public Lookups GetRecordingData()
        {
            var lookups = new Lookups();
            ForEach(lookup =>
                lookups.Add(lookup.GetRecordingData()));
            return lookups;
        }

        public Dictionary<string, string> LookupData(
            Context context,
            SiteSettings ss,
            Link link,
            long id)
        {
            var formData = new Dictionary<string, string>();
            var currentSs = ss.Destinations.Get(link.SiteId);
            link.Lookups.ForEach(lookup =>
            {
                formData.AddOrUpdate(
                    $"{currentSs.ReferenceType}_{lookup.To}",
                    string.Empty);
            });
            if (id > 0
                && currentSs != null
                && context.CanRead(ss: currentSs, id: id))
            {
                switch (currentSs.ReferenceType)
                {
                    case "Issues":
                        var issueModel = new IssueModel(
                            context: context,
                            ss: currentSs,
                            issueId: id);
                        link.Lookups.ForEach(lookup =>
                            formData.AddOrUpdate(
                                $"{currentSs.ReferenceType}_{lookup.To}",
                                lookup.Data(
                                    context: context,
                                    ss: currentSs,
                                    issueModel: issueModel)));
                        break;
                    case "Results":
                        var resultModel = new ResultModel(
                            context: context,
                            ss: currentSs,
                            resultId: id);
                        link.Lookups.ForEach(lookup =>
                            formData.AddOrUpdate(
                                $"{currentSs.ReferenceType}_{lookup.To}",
                                lookup.Data(
                                    context: context,
                                    ss: currentSs,
                                    resultModel: resultModel)));
                        break;
                }
            }
            return formData;
        }
    }
}