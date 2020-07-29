using Implem.DisplayAccessor;
using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedHtml : ExtendedBase
    {
        public string Language;
        public Dictionary<string, List<DisplayElement>> Html;
    }
}