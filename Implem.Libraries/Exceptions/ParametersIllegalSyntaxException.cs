using System;
namespace Implem.Libraries.Exceptions
{
    public class ParametersIllegalSyntaxException : Exception
    {
        public ParametersIllegalSyntaxException(string message) : base(message)
        {
        }
    }
}
