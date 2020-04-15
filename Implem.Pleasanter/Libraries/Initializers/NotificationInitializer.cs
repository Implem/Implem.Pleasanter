using Implem.DefinitionAccessor;
using System;
using System.Net;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public class NotificationInitializer
    {
        public static void Initialize()
        {
            ServicePointManager.SecurityProtocol =
                Enum.TryParse(Parameters.Notification.SecurityProtocolType, out SecurityProtocolType sp)
                    ? ServicePointManager.SecurityProtocol = sp
                    : SecurityProtocolType.Tls12;
        }
    }
}