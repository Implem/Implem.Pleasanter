using Implem.Libraries.Utilities;

namespace Implem.Pleasanter.Libraries.SiteManagement
{
    internal class Json2MermaidConvertor
    {
        internal static (string mermaidText, bool flowControl) Convert(SettingsJsonConverter dump)
        {
            if (dump?.ERDiagrams?.Tables == null) return (null, false);
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("erDiagram\n");
            foreach (var table in dump.ERDiagrams.Tables)
            {
                stringBuilder.Append(ConvertTable(table));
            }
            foreach (var table in dump.ERDiagrams.Tables)
            {
                stringBuilder.Append(ConvertRelation(table));
            }
            return (stringBuilder.ToString(), true);
        }

        private static string ConvertTable(
            SettingsJsonConverter.EntityRelationshipDiagramsData.Table table)
        {
            var cols = table.Columns?.ConvertAll(col =>
                {
                    var k = (col.Type == "ID") ? "PK" : (col.RelationSiteId.HasValue ? "FK" : "");
                    return $"  {col.TypeRaw} {col.Type} {k} \"{col.Label}\"";
                }).Join("\n") ?? string.Empty;
            return $"  TBL_{table.SiteId}[\"{table.Title}({table.SiteId})\"] {{\n{cols}\n}}\n";
        }

        private static string ConvertRelation(
            SettingsJsonConverter.EntityRelationshipDiagramsData.Table table)
        {
            return table.Columns?.ConvertAll(col =>
                {
                    if (col.RelationSiteId.HasValue)
                    {
                        return $"  TBL_{col.RelationSiteId.Value} |o--o{{ TBL_{table.SiteId} : \"{col.Label}\"\n";
                    }
                    return string.Empty;
                }).Join("") ?? string.Empty;
        }
    }
}