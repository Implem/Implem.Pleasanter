using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.DefinitionAccessor.Exceptions
{
    public class ParametersIllegalSyntaxException : Exception
    {
        public ParametersIllegalSyntaxException(string message) : base(message)
        {
        }
    }
}
