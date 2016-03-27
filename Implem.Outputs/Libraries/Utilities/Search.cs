using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Views;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Utilities
{
    public static class Search
    {
        public static void CreateIndex(long id)
        {
            var itemModel = new ItemModel(id);
            var siteModel = new SiteModel().Get(where: Rds.SitesWhere().SiteId(itemModel.SiteId));
            if (Exclude(itemModel, siteModel)) return;
            SearchWordCollection(siteModel.SiteSettings, id, itemModel.ReferenceType)
                .Buffer(2000)
                .Select((o, i) => new { SearchWordCollection = o, First = (i == 0) })
                .ForEach(data =>
                {
                    try
                    {
                        Rds.ExecuteNonQuery(statements:
                            Statements(data.SearchWordCollection, id, data.First));
                    }
                    catch (Exception e) { new SysLogModel(e); }
                });
        }

        private static bool Exclude(ItemModel itemModel, SiteModel siteModel)
        {
            switch (itemModel.ReferenceType)
            {
                case "Sites":
                    switch (siteModel.ReferenceType)
                    {
                        case "Wikis": return true;
                        default: return false;
                    }
                default: return false;
            }
        }

        private static SqlStatement[] Statements(
            IList<KeyValuePair<string, int>> searchWordCollection, long id, bool first)
        {
            var statements = new List<SqlStatement>();
            if (first)
            {
                statements.Add(Rds.PhysicalDeleteSearchWords(
                    where: Rds.SearchWordsWhere().ReferenceId(id)));
            }
            searchWordCollection.ForEach(word =>
                statements.Add(Rds.InsertSearchWords(
                    param: Rds.SearchWordsParam()
                        .Word(word.Key)
                        .ReferenceId(raw: id.ToString())
                        .Priority(raw: word.Value.ToString()))));
            return statements.ToArray();
        }

        private static Dictionary<string, int> SearchWordCollection(
            SiteSettings siteSettings, long id, string referenceType)
        {
            switch (referenceType)
            {
                case "Sites": return new SiteSubset(
                    new SiteModel().Get(where: Rds.SitesWhere().SiteId(id)),
                    siteSettings)
                        .SearchWordCollection();
                case "Issues": return new IssueSubset(
                    new IssueModel(siteSettings, Permissions.Admins(), id),
                    siteSettings)
                        .SearchWordCollection();
                case "Results": return new ResultSubset(
                    new ResultModel(siteSettings, Permissions.Admins(), id),
                    siteSettings)
                        .SearchWordCollection();
                case "Wikis": return new WikiSubset(
                    new WikiModel(siteSettings, Permissions.Admins(), id),
                    siteSettings)
                        .SearchWordCollection();
                default: return null;
            }
        }

        public static HtmlBuilder Result(this HtmlBuilder hb, DataRow dataRow, string text)
        {
            string href;
            switch (dataRow["referenceType"].ToString())
            {
                case "Sites":
                    var siteSubset = dataRow["Subset"].ToString().Deserialize<SiteSubset>();
                    href = Navigations.ItemIndex(siteSubset.SiteId);
                    hb.Section(
                        attributes: Html.Attributes()
                            .Class("result")
                            .Add("data-href", href),
                        action: () => hb
                            .H(number:3, action: () => hb
                                .A(
                                    href: href,
                                    text: siteSubset.Title.Value))
                            .P(action: () => hb
                                .Text(text: siteSubset.Body)));
                    break;
                case "Issues":
                    var issueSubset = dataRow["Subset"].ToString().Deserialize<IssueSubset>();
                        href = Navigations.ItemEdit(issueSubset.IssueId);
                        hb.Section(
                            attributes: Html.Attributes()
                                .Class("result")
                                .Add("data-href", href),
                            action: () => hb
                                .H(number:3, action: () => hb
                                    .A(
                                        href: href,
                                        text: dataRow["Title"].ToString()))
                                .P(action: () => hb
                                    .Text(text: issueSubset.Body)));
                    break;
                case "Results":
                    var resultSubset = dataRow["Subset"].ToString().Deserialize<ResultSubset>();
                        href = Navigations.ItemEdit(resultSubset.ResultId);
                        hb.Section(
                            attributes: Html.Attributes()
                                .Class("result")
                                .Add("data-href", href),
                            action: () => hb
                                .H(number:3, action: () => hb
                                    .A(
                                        href: href,
                                        text: dataRow["Title"].ToString()))
                                .P(action: () => hb
                                    .Text(text: resultSubset.Body)));
                    break;
                case "Wikis":
                    var wikiSubset = dataRow["Subset"].ToString().Deserialize<WikiSubset>();
                        href = Navigations.ItemEdit(wikiSubset.WikiId);
                        hb.Section(
                            attributes: Html.Attributes()
                                .Class("result")
                                .Add("data-href", href),
                            action: () => hb
                                .H(number:3, action: () => hb
                                    .A(
                                        href: href,
                                        text: dataRow["Title"].ToString()))
                                .P(action: () => hb
                                    .Text(text: wikiSubset.Body)));
                    break;
            }
            return hb;
        }
    }
}
