using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Migration
    {
        public string Dbms;
        public string Provider;
        public string ServiceName;
        public string SourceConnectionString;
        public List<string> ExcludeTables;
        public bool AbortWhenException;
    }
}
