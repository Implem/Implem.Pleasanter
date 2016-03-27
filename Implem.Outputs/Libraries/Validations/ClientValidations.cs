using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Validations
{
    public static class ClientValidations
    {
        public static Dictionary<string, string> MessageCollection(string validations)
        {
            return validations.Split(',').Select(o => o.Trim()).ToDictionary(o =>
                "data-validate-" + o.Split(':')._1st(),
                o => Message(o.Split(':')._1st()));
        }

        private static string Message(string validationType)
        {
            switch (validationType)
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