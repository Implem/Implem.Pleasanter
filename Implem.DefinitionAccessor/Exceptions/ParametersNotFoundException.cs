using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.DefinitionAccessor.Exceptions
{
    public class ParametersNotFoundException : Exception
    {
        public ParametersNotFoundException(string message) : base(message)
        {
        }
    }
}
