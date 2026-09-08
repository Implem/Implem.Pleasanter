using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class AiConnect
    {
        public Rag Rag { get; set; } = new Rag();

        public Dictionary<string, string> ProviderEndpoints { get; set; }

        public bool AllowInsecureLoopbackEndpoint { get; set; }
    }
}
