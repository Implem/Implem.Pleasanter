using Implem.Libraries.Utilities;
using Implem.DefinitionAccessor;
using System.Collections.Generic;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class Parameter
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            Def.ParameterDefinitionCollection.ForEach(parameterDefinition =>
                Creators.SetCodeCollection(
                    ref code,
                    codeCollection,
                    codeDefinition,
                    dataContainer,
                    () => ReplaceCode(ref code, parameterDefinition)));
        }

        private static void ReplaceCode(ref string code, ParameterDefinition parameterDefinition)
        {
            code = code.Replace("#ParameterId#", parameterDefinition.Id);
            if (!parameterDefinition.String.IsNullOrEmpty())
            {
                code = code.Replace("#type#", "string").Replace("#Type#", "String");
            }
            else if (parameterDefinition.DateTime.NotZero())
            {
                code = code.Replace("#type#", "DateTime").Replace("#Type#", "DateTime");
            }
            else if (parameterDefinition.Decimal != 0)
            {
                code = code.Replace("#type#", "decimal").Replace("#Type#", "Decimal");
            }
            else
            {
                code = code.Replace("#type#", "int").Replace("#Type#", "Int");
            }
        }
    }
}
