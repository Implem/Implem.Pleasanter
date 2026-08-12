using System;
namespace Implem.Libraries.Exceptions
{
    public class GridDataTimeoutException : Exception
    {
        public GridDataTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
