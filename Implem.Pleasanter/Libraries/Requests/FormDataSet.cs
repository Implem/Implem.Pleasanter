using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class FormDataSet : List<FormData>
    {
        public FormDataSet(Context context, SiteSettings ss)
        {
            var hash = new Dictionary<long, FormData>();
            context.Forms?.ForEach(data =>
            {
                var suffix = data.Key.RegexFirst("_\\d+_-?\\d+$");
                var hasSuffix = !suffix.IsNullOrEmpty();
                var siteId = hasSuffix
                    ? suffix.Split_2nd('_').ToLong()
                    : context.SiteId;
                var referenceId = hasSuffix
                    ? suffix.Split_3rd('_').ToLong()
                    : context.Id;
                if (!hash.ContainsKey(referenceId))
                {
                    hash.Add(referenceId, new FormData(
                        siteId: siteId,
                        id: referenceId,
                        suffix: suffix,
                        ss: ss.JoinedSsHash.Get(siteId)));
                }
                hash.Get(referenceId).Data.Add(
                    data.Key.Substring(0, data.Key.Length - suffix.Length),
                    data.Value);
            });
            hash.Values.ForEach(formData => Add(formData));
        }
    }
}