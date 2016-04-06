using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Validators
{
    public static class ClientValidators
    {
        public static Dictionary<string, string> MessageCollection(string validators)
        {
            return validators.Split(',').Select(o => o.Trim()).ToDictionary(o =>
                "data-validate-" + o.Split(':')._1st(),
                o => Message(o.Split(':')._1st()));
        }

        private static string Message(string validatorType)
        {
            switch (validatorType)
            {
                case "required": return Displays.ValidateRequired();
                case "date": return Displays.ValidateDate();
                case "mail": return Displays.ValidateMail();
                case "equalTo": return Displays.ValidateEqualTo();
                default: return string.Empty;
            }
        }
    }
}