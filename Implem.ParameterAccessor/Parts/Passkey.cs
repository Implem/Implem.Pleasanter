using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.ParameterAccessor.Parts
{
    public class Passkey
    {
        public bool Enabled;
        public string ServerDomain;
        public string ServerName;
        public HashSet<string> Origins;
    }
}