using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class SimpleMode
    {
        public bool Enabled { get; set; }
        public bool Default { get; set; }
        public bool DisplaySwitch { get; set; }
        public List<string> Tabs { get; set; } = new();
    }
}