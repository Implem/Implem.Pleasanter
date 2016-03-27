using Implem.DefinitionAccessor;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal class Form
    {
        internal static void SetCodeCollection(
            CodeDefinition codeDefinition,
            List<string> codeCollection,
            DataContainer dataContainer)
        {
            var code = string.Empty;
            Def.ColumnDefinitionCollection
                .Where(o => o.TableName == dataContainer.TableName)
                .Select(o => new { ModelName = o.ModelName, FormName = o.FormName })
                .Distinct()
                .ForEach(data =>
            {
                dataContainer.FormName = data.FormName;
                Creators.SetCodeCollection(
                    ref code,
                    codeCollection,
                    codeDefinition,
                    dataContainer,
                    () => ReplaceCode(ref code, codeDefinition, data.FormName, data.ModelName));
            });
        }

        private static void ReplaceCode(
            ref string code,
            CodeDefinition codeDefinition,
            string formName,
            string modelName)
        {
            code = code.Replace("#FormName#", formName != string.Empty
                ? formName
                : modelName + "Form");
            if (codeDefinition.ReplaceOld != string.Empty)
            {
                code = code.Replace(codeDefinition.ReplaceOld, codeDefinition.ReplaceNew);
            }
        }
    }
}