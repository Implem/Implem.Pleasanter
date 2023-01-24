using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Models
{
    public static class DemoValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnRegistering(
            Context context,
            DemoApiModel demoApiModel)
        {
            if ((!Parameters.Api.Enabled
                || context.ContractSettings.Api == false
                || context.UserSettings?.AllowApi(context: context) == false))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (demoApiModel == null)
            {
                return new ErrorData(type: Error.Types.InvalidJsonData);
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
