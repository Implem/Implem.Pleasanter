using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
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
            var apiErrorData = Validators.ValidateApi(
                context: context,
                serverScript: false);
            switch (apiErrorData.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: apiErrorData);
            }
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

        public const string SupportCustomersUrl = "https://pleasanter.org/support_customers/?utm_source={0}&utm_medium=app&utm_campaign=customers&utm_content={1}";

        public static bool ShowSupportCustomersGuide(Context context)
        {
            return Parameters.CommercialLicense()
                && Parameters.General.PleasanterSource == "pleasanter-ee"
                && context.Language == "ja";
        }
    }
}
