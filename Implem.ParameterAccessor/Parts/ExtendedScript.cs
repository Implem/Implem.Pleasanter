using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedScript
    {
        public string Name;
        public string Path;
        public string Description;
        public bool Disabled;
        public List<long> SiteIdList;
        public List<long> IdList;
        public List<string> Controllers;
        public List<string> Actions;
        public string Script;
    }
}