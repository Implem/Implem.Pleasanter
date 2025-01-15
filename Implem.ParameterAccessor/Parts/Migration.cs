using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Migration
    {
        public string Dbms;
        public string SourceConnectionString;
        public List<string> ExcludeTables;
        public bool IgnoreIfOverflow;
    }
}
