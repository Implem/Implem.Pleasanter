using System.Linq;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;

namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlBackgroundJobNotices
    {
        public static HtmlBuilder BackgroundJobNotice(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            if (context.Publish)
            {
                return hb;
            }
            return BackgroundJobQueue.ReservedJobTypeLabels(
                context: context,
                ss: ss).Any()
                    ? hb.Div(
                        css: "background-job-notice",
                        action: () => hb
                            .Text(text: Displays.BackgroundJobReserved(
                                context: context,
                                data: Displays.BackgroundJobs(context: context))))
                    : hb;
        }
    }
}
