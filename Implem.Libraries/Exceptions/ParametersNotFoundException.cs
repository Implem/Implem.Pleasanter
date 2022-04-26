using System;
namespace Implem.Libraries.Exceptions
{
    public class ParametersNotFoundException : Exception
    {
        public ParametersNotFoundException(string message) : base(message)
        {
        }
    }
}
