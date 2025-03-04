using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.Libraries.Exceptions
{
    public class CanNotGridSortException : Exception
    {
        public CanNotGridSortException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
