using System;
namespace Implem.Libraries.Exceptions
{
    public class GridDataException : Exception
    {
        public GridDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
