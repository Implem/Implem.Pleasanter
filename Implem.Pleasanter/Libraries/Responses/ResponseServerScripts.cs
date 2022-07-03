using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseServerScripts
    {
        public static ResponseCollection ServerScriptResponses(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            View view,
            ServerScriptModelResponses responses)
        {
            responses?.ForEach(response =>
            {
                switch (response.Type)
                {
                    case ServerScriptModelResponse.Types.Filter:
                        var column = ss.GetColumn(
                            context: context,
                            columnName: response.ColumnName);
                        if (column != null)
                        {
                            var hb = new HtmlBuilder();
                            HtmlViewFilters.Column(
                                hb: hb,
                                context: context,
                                ss: ss,
                                view: view,
                                column: column);
                            res.ReplaceAll($"[id=\"ViewFilters__{column.ColumnName}Field\"]", hb);
                        }
                        break;
                }
            });
            return res;
        }
    }
}
