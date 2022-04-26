using System;
namespace Implem.Libraries.Exceptions
{
    public class IllegalSiteSettingsException : Exception
    {
        public IllegalSiteSettingsException(string message) : base(message)
        {
        }
    }
}
