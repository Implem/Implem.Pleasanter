using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Data;
namespace Implem.Pleasanter.Libraries.Search
{
    public static class Responses
    {
        public static HtmlBuilder Get(HtmlBuilder hb, DataRow dataRow, string text)
        {
            string href;
            switch (dataRow["referenceType"].ToString())
            {
                case "Sites":
                    var siteSubset = dataRow["Subset"].ToString().Deserialize<SiteSubset>();
                    href = Navigations.ItemIndex(siteSubset.SiteId);
                    hb.Section(
                        attributes: new HtmlAttributes()
                            .Class("result")
                            .Add("data-href", href),
                        action: () => hb
                            .Breadcrumb(siteSubset.ParentId)
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
                        attributes: new HtmlAttributes()
                            .Class("result")
                            .Add("data-href", href),
                        action: () => hb
                            .Breadcrumb(issueSubset.SiteId)
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
                        attributes: new HtmlAttributes()
                            .Class("result")
                            .Add("data-href", href),
                        action: () => hb
                            .Breadcrumb(resultSubset.SiteId)
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
                        attributes: new HtmlAttributes()
                            .Class("result")
                            .Add("data-href", href),
                        action: () => hb
                            .Breadcrumb(wikiSubset.SiteId)
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
