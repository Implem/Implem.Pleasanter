using System;
namespace Implem.Libraries.Exceptions
{
    public class InvalidLanguageException : Exception
    {
        public InvalidLanguageException(string message) : base(message)
        {
        }
    }
}
