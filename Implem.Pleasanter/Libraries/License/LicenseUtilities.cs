using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Linq;

namespace Implem.Pleasanter.Libraries.License
{
    public class LicenseUtilities
    {
        public class LicenseInfoApi
        {
            public string AppTypes { get; set; }
        }

        public static ContentResultInheritance GetLicenseInfo(Context context)
        {
            var licenseInfo = context.RequestDataString.Deserialize<LicenseInfoApi>();
            var apps = (licenseInfo?.AppTypes ?? string.Empty).ToLower().Split(',');
            object siteVisualizer = null;
            if (apps.Contains("sitevisualizer"))
            {
                if (Parameters.PleasanterExtensions.SiteVisualizer.Disabled == true)
                {
                    siteVisualizer = new
                    {
                        Disabled = true
                    };
                }
            }
            var json = new
            {
                StatusCode = 200,
                Response = new
                {
                    CommercialLicense = Parameters.CommercialLicense(),
                    LicenseDeadline = Parameters.LicenseDeadline(),
                    Environment = Parameters.Environment(),
                    Version = Environments.AssemblyVersion,
                    SiteVisualizer = siteVisualizer
                }
            }.ToJson();
            return ApiResults.Get(json);
        }
    }
}
