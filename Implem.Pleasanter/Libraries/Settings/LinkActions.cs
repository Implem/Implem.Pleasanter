using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class LinkActions : List<LinkAction>
    {
        public LinkActions GetRecordingData(Context context, SiteSettings ss)
        {
            var linkActions = new LinkActions();
            ForEach(linkAction =>
                linkActions.Add(linkAction.GetRecordingData(
                    context: context,
                    ss: ss)));
            return linkActions;
        }
    }
}