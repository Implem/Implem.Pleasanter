using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Utilities
{
    public static class Summaries
    {
        public static string Synchronize(SiteSettings siteSettings)
        {
            var summary = siteSettings.SummaryCollection.FirstOrDefault(
                o => o.Id == Forms.Data("ControlId").Split(',')._2nd().ToLong());
            return Synchronize(
                summary.SiteId,
                new SiteModel(summary.SiteId).ReferenceType,
                summary.DestinationColumn,
                siteSettings.ReferenceType,
                summary.LinkColumn,
                summary.Type,
                summary.SourceColumn);
        }

        public static string Synchronize(
            long siteId,
            string destinationReferenceType,
            string destination,
            string sourceReferenceType,
            string link,
            string type,
            string source,
            long id = 0)
        {
            switch (destinationReferenceType)
            {
                case "Issues": SynchronizeIssues(
                    siteId, destination, sourceReferenceType, link, type, source, id); break;
                case "Results": SynchronizeResults(
                    siteId, destination, sourceReferenceType, link, type, source, id); break;
                case "Wikis": SynchronizeWikis(
                    siteId, destination, sourceReferenceType, link, type, source, id); break;
            }
            return Messages.ResponseSynchronizationCompleted().ToJson();        
        }

        private static void SynchronizeIssues(
            long siteId,
            string destination,
            string sourceReferenceType,
            string link,
            string type,
            string source,
            long issueId = 0)
        {
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateIssues(
                    param: IssuesParamCollection(
                        destination,
                        "convert(nvarchar, [Issues].[IssueId])",
                        sourceReferenceType,
                        link,
                        type,
                        source),
                    where: Rds.IssuesWhere()
                        .SiteId(siteId)
                        .IssueId(issueId, _using: issueId != 0),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        private static Rds.IssuesParamCollection IssuesParamCollection(
            string destination,
            string destinationPk,
            string sourceReferenceType,
            string link,
            string type,
            string source)
        {
            switch (destination)
            {
                case "WorkValue": return Rds.IssuesParam().WorkValue(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumA": return Rds.IssuesParam().NumA(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumB": return Rds.IssuesParam().NumB(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumC": return Rds.IssuesParam().NumC(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumD": return Rds.IssuesParam().NumD(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumE": return Rds.IssuesParam().NumE(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumF": return Rds.IssuesParam().NumF(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumG": return Rds.IssuesParam().NumG(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumH": return Rds.IssuesParam().NumH(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                default: return null;
            }
        }

        private static void SynchronizeResults(
            long siteId,
            string destination,
            string sourceReferenceType,
            string link,
            string type,
            string source,
            long resultId = 0)
        {
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateResults(
                    param: ResultsParamCollection(
                        destination,
                        "convert(nvarchar, [Results].[ResultId])",
                        sourceReferenceType,
                        link,
                        type,
                        source),
                    where: Rds.ResultsWhere()
                        .SiteId(siteId)
                        .ResultId(resultId, _using: resultId != 0),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        private static Rds.ResultsParamCollection ResultsParamCollection(
            string destination,
            string destinationPk,
            string sourceReferenceType,
            string link,
            string type,
            string source)
        {
            switch (destination)
            {
                case "NumA": return Rds.ResultsParam().NumA(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumB": return Rds.ResultsParam().NumB(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumC": return Rds.ResultsParam().NumC(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumD": return Rds.ResultsParam().NumD(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumE": return Rds.ResultsParam().NumE(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumF": return Rds.ResultsParam().NumF(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumG": return Rds.ResultsParam().NumG(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                case "NumH": return Rds.ResultsParam().NumH(sub: Select(
                    destinationPk, sourceReferenceType, link, type, source));
                default: return null;
            }
        }

        private static void SynchronizeWikis(
            long siteId,
            string destination,
            string sourceReferenceType,
            string link,
            string type,
            string source,
            long wikiId = 0)
        {
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateWikis(
                    param: WikisParamCollection(
                        destination,
                        "convert(nvarchar, [Wikis].[WikiId])",
                        sourceReferenceType,
                        link,
                        type,
                        source),
                    where: Rds.WikisWhere()
                        .SiteId(siteId)
                        .WikiId(wikiId, _using: wikiId != 0),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        private static Rds.WikisParamCollection WikisParamCollection(
            string destination,
            string destinationPk,
            string sourceReferenceType,
            string link,
            string type,
            string source)
        {
            switch (destination)
            {
                default: return null;
            }
        }

        private static SqlSelect Select(
            string destinationPk,
            string sourceReferenceType,
            string link,
            string type,
            string source)
        {
            switch (type)
            {
                case "Count": return SelectCount(
                    destinationPk, sourceReferenceType, link);
                case "Total": return SelectTotal(
                    destinationPk, sourceReferenceType, link, source);
                case "Average": return SelectAverage(
                    destinationPk, sourceReferenceType, link, source);
                case "Max": return SelectMax(
                    destinationPk, sourceReferenceType, link, source);
                case "Min": return SelectMin(
                    destinationPk, sourceReferenceType, link, source);
                default: return null;
            }
        }

        private static SqlSelect SelectCount(
            string destinationPk,
            string sourceReferenceType,
            string link)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    where: IssuesWhere(destinationPk, link));
                case "Results": return Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    where: ResultsWhere(destinationPk, link));
                case "Wikis": return Rds.SelectWikis(
                    column: Rds.WikisColumn().WikisCount(),
                    where: WikisWhere(destinationPk, link));
                default: return null;
            }
        }

        private static SqlSelect SelectTotal(
            string destinationPk,
            string sourceReferenceType,
            string link,
            string source)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesTotalColumn(source),
                    where: IssuesWhere(destinationPk, link));
                case "Results": return Rds.SelectResults(
                    column: ResultsTotalColumn(source),
                    where: ResultsWhere(destinationPk, link));
                case "Wikis": return Rds.SelectWikis(
                    column: WikisTotalColumn(source),
                    where: WikisWhere(destinationPk, link));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesTotalColumn(string source)
        {
            switch (source)
            {
                case "WorkValue": return Rds.IssuesColumn().WorkValueTotal();
                case "RemainingWorkValue": return Rds.IssuesColumn().RemainingWorkValueTotal();
                case "NumA": return Rds.IssuesColumn().NumATotal();
                case "NumB": return Rds.IssuesColumn().NumBTotal();
                case "NumC": return Rds.IssuesColumn().NumCTotal();
                case "NumD": return Rds.IssuesColumn().NumDTotal();
                case "NumE": return Rds.IssuesColumn().NumETotal();
                case "NumF": return Rds.IssuesColumn().NumFTotal();
                case "NumG": return Rds.IssuesColumn().NumGTotal();
                case "NumH": return Rds.IssuesColumn().NumHTotal();
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsTotalColumn(string source)
        {
            switch (source)
            {
                case "NumA": return Rds.ResultsColumn().NumATotal();
                case "NumB": return Rds.ResultsColumn().NumBTotal();
                case "NumC": return Rds.ResultsColumn().NumCTotal();
                case "NumD": return Rds.ResultsColumn().NumDTotal();
                case "NumE": return Rds.ResultsColumn().NumETotal();
                case "NumF": return Rds.ResultsColumn().NumFTotal();
                case "NumG": return Rds.ResultsColumn().NumGTotal();
                case "NumH": return Rds.ResultsColumn().NumHTotal();
                default: return null;
            }
        }

        private static SqlColumnCollection WikisTotalColumn(string source)
        {
            switch (source)
            {
                default: return null;
            }
        }

        private static SqlSelect SelectAverage(
            string destinationPk,
            string sourceReferenceType,
            string link,
            string source)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesAverageColumn(source),
                    where: IssuesWhere(destinationPk, link));
                case "Results": return Rds.SelectResults(
                    column: ResultsAverageColumn(source),
                    where: ResultsWhere(destinationPk, link));
                case "Wikis": return Rds.SelectWikis(
                    column: WikisAverageColumn(source),
                    where: WikisWhere(destinationPk, link));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesAverageColumn(string source)
        {
            switch (source)
            {
                case "WorkValue": return Rds.IssuesColumn().WorkValueAverage();
                case "RemainingWorkValue": return Rds.IssuesColumn().RemainingWorkValueAverage();
                case "NumA": return Rds.IssuesColumn().NumAAverage();
                case "NumB": return Rds.IssuesColumn().NumBAverage();
                case "NumC": return Rds.IssuesColumn().NumCAverage();
                case "NumD": return Rds.IssuesColumn().NumDAverage();
                case "NumE": return Rds.IssuesColumn().NumEAverage();
                case "NumF": return Rds.IssuesColumn().NumFAverage();
                case "NumG": return Rds.IssuesColumn().NumGAverage();
                case "NumH": return Rds.IssuesColumn().NumHAverage();
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsAverageColumn(string source)
        {
            switch (source)
            {
                case "NumA": return Rds.ResultsColumn().NumAAverage();
                case "NumB": return Rds.ResultsColumn().NumBAverage();
                case "NumC": return Rds.ResultsColumn().NumCAverage();
                case "NumD": return Rds.ResultsColumn().NumDAverage();
                case "NumE": return Rds.ResultsColumn().NumEAverage();
                case "NumF": return Rds.ResultsColumn().NumFAverage();
                case "NumG": return Rds.ResultsColumn().NumGAverage();
                case "NumH": return Rds.ResultsColumn().NumHAverage();
                default: return null;
            }
        }

        private static SqlColumnCollection WikisAverageColumn(string source)
        {
            switch (source)
            {
                default: return null;
            }
        }

        private static SqlSelect SelectMax(
            string destinationPk,
            string sourceReferenceType,
            string link,
            string source)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesMaxColumn(source),
                    where: IssuesWhere(destinationPk, link));
                case "Results": return Rds.SelectResults(
                    column: ResultsMaxColumn(source),
                    where: ResultsWhere(destinationPk, link));
                case "Wikis": return Rds.SelectWikis(
                    column: WikisMaxColumn(source),
                    where: WikisWhere(destinationPk, link));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesMaxColumn(string source)
        {
            switch (source)
            {
                case "WorkValue": return Rds.IssuesColumn().WorkValueMax();
                case "RemainingWorkValue": return Rds.IssuesColumn().RemainingWorkValueMax();
                case "NumA": return Rds.IssuesColumn().NumAMax();
                case "NumB": return Rds.IssuesColumn().NumBMax();
                case "NumC": return Rds.IssuesColumn().NumCMax();
                case "NumD": return Rds.IssuesColumn().NumDMax();
                case "NumE": return Rds.IssuesColumn().NumEMax();
                case "NumF": return Rds.IssuesColumn().NumFMax();
                case "NumG": return Rds.IssuesColumn().NumGMax();
                case "NumH": return Rds.IssuesColumn().NumHMax();
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsMaxColumn(string source)
        {
            switch (source)
            {
                case "NumA": return Rds.ResultsColumn().NumAMax();
                case "NumB": return Rds.ResultsColumn().NumBMax();
                case "NumC": return Rds.ResultsColumn().NumCMax();
                case "NumD": return Rds.ResultsColumn().NumDMax();
                case "NumE": return Rds.ResultsColumn().NumEMax();
                case "NumF": return Rds.ResultsColumn().NumFMax();
                case "NumG": return Rds.ResultsColumn().NumGMax();
                case "NumH": return Rds.ResultsColumn().NumHMax();
                default: return null;
            }
        }

        private static SqlColumnCollection WikisMaxColumn(string source)
        {
            switch (source)
            {
                default: return null;
            }
        }

        private static SqlSelect SelectMin(
            string destinationPk,
            string sourceReferenceType,
            string link,
            string source)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesMinColumn(source),
                    where: IssuesWhere(destinationPk, link));
                case "Results": return Rds.SelectResults(
                    column: ResultsMinColumn(source),
                    where: ResultsWhere(destinationPk, link));
                case "Wikis": return Rds.SelectWikis(
                    column: WikisMinColumn(source),
                    where: WikisWhere(destinationPk, link));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesMinColumn(string source)
        {
            switch (source)
            {
                case "WorkValue": return Rds.IssuesColumn().WorkValueMin();
                case "RemainingWorkValue": return Rds.IssuesColumn().RemainingWorkValueMin();
                case "NumA": return Rds.IssuesColumn().NumAMin();
                case "NumB": return Rds.IssuesColumn().NumBMin();
                case "NumC": return Rds.IssuesColumn().NumCMin();
                case "NumD": return Rds.IssuesColumn().NumDMin();
                case "NumE": return Rds.IssuesColumn().NumEMin();
                case "NumF": return Rds.IssuesColumn().NumFMin();
                case "NumG": return Rds.IssuesColumn().NumGMin();
                case "NumH": return Rds.IssuesColumn().NumHMin();
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsMinColumn(string source)
        {
            switch (source)
            {
                case "NumA": return Rds.ResultsColumn().NumAMin();
                case "NumB": return Rds.ResultsColumn().NumBMin();
                case "NumC": return Rds.ResultsColumn().NumCMin();
                case "NumD": return Rds.ResultsColumn().NumDMin();
                case "NumE": return Rds.ResultsColumn().NumEMin();
                case "NumF": return Rds.ResultsColumn().NumFMin();
                case "NumG": return Rds.ResultsColumn().NumGMin();
                case "NumH": return Rds.ResultsColumn().NumHMin();
                default: return null;
            }
        }

        private static SqlColumnCollection WikisMinColumn(string source)
        {
            switch (source)
            {
                default: return null;
            }
        }

        private static SqlWhereCollection IssuesWhere(string destinationPk, string link)
        {
            switch (link)
            {
                case "ClassA": return Rds.IssuesWhere().ClassA(raw: destinationPk);
                case "ClassB": return Rds.IssuesWhere().ClassB(raw: destinationPk);
                case "ClassC": return Rds.IssuesWhere().ClassC(raw: destinationPk);
                case "ClassD": return Rds.IssuesWhere().ClassD(raw: destinationPk);
                case "ClassE": return Rds.IssuesWhere().ClassE(raw: destinationPk);
                case "ClassF": return Rds.IssuesWhere().ClassF(raw: destinationPk);
                case "ClassG": return Rds.IssuesWhere().ClassG(raw: destinationPk);
                case "ClassH": return Rds.IssuesWhere().ClassH(raw: destinationPk);
                case "ClassI": return Rds.IssuesWhere().ClassI(raw: destinationPk);
                case "ClassJ": return Rds.IssuesWhere().ClassJ(raw: destinationPk);
                case "ClassK": return Rds.IssuesWhere().ClassK(raw: destinationPk);
                case "ClassL": return Rds.IssuesWhere().ClassL(raw: destinationPk);
                case "ClassM": return Rds.IssuesWhere().ClassM(raw: destinationPk);
                case "ClassN": return Rds.IssuesWhere().ClassN(raw: destinationPk);
                case "ClassO": return Rds.IssuesWhere().ClassO(raw: destinationPk);
                case "ClassP": return Rds.IssuesWhere().ClassP(raw: destinationPk);
                default: return null;
            }
        }

        private static SqlWhereCollection ResultsWhere(string destinationPk, string link)
        {
            switch (link)
            {
                case "ClassA": return Rds.ResultsWhere().ClassA(raw: destinationPk);
                case "ClassB": return Rds.ResultsWhere().ClassB(raw: destinationPk);
                case "ClassC": return Rds.ResultsWhere().ClassC(raw: destinationPk);
                case "ClassD": return Rds.ResultsWhere().ClassD(raw: destinationPk);
                case "ClassE": return Rds.ResultsWhere().ClassE(raw: destinationPk);
                case "ClassF": return Rds.ResultsWhere().ClassF(raw: destinationPk);
                case "ClassG": return Rds.ResultsWhere().ClassG(raw: destinationPk);
                case "ClassH": return Rds.ResultsWhere().ClassH(raw: destinationPk);
                case "ClassI": return Rds.ResultsWhere().ClassI(raw: destinationPk);
                case "ClassJ": return Rds.ResultsWhere().ClassJ(raw: destinationPk);
                case "ClassK": return Rds.ResultsWhere().ClassK(raw: destinationPk);
                case "ClassL": return Rds.ResultsWhere().ClassL(raw: destinationPk);
                case "ClassM": return Rds.ResultsWhere().ClassM(raw: destinationPk);
                case "ClassN": return Rds.ResultsWhere().ClassN(raw: destinationPk);
                case "ClassO": return Rds.ResultsWhere().ClassO(raw: destinationPk);
                case "ClassP": return Rds.ResultsWhere().ClassP(raw: destinationPk);
                default: return null;
            }
        }

        private static SqlWhereCollection WikisWhere(string destinationPk, string link)
        {
            switch (link)
            {
                default: return null;
            }
        }
    }
}
