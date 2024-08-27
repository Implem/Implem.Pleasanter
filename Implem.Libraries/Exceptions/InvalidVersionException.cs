using System;
namespace Implem.Libraries.Exceptions
{
    public class InvalidVersionException : Exception
    {
        public InvalidVersionException(string message) : base(message)
        {
        }
    }
}