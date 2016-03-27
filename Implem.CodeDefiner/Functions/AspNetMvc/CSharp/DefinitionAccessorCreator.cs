using Implem.DefinitionAccessor;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class DefinitionAccessorCreator
    {
        internal static void Create()
        {
            CreateCode();
        }

        private static void CreateCode()
        {
            Def.CodeDefinitionCollection.Where(o => o.Source == "Def").ForEach(codeDefinition =>
            {
                var code = Creators.Create(codeDefinition, new DataContainer("DefinitionFile"));
                if (code != string.Empty)
                {
                    Merger.Merge(codeDefinition.OutputPath, code, codeDefinition.MergeToExisting);
                }
            });
        }
    }
}
