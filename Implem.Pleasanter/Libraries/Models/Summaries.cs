using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Summaries
    {
        public static string Synchronize(SiteSettings siteSettings, long siteId)
        {
            var summary = siteSettings.SummaryCollection.FirstOrDefault(
                o => o.Id == Forms.Long("SynchronizeSummaryId"));
            var destinationSiteModel = new SiteModel(summary.SiteId);
            var json = Synchronize(
                summary.SiteId,
                destinationSiteModel.ReferenceType,
                summary.DestinationColumn,
                siteId,
                siteSettings.ReferenceType,
                summary.LinkColumn,
                summary.Type,
                summary.SourceColumn);
            Formulas.Synchronize(destinationSiteModel);
            return json;
        }

        public static string Synchronize(
            long destinationSiteId,
            string destinationReferenceType,
            string destination,
            long sourceSiteId,
            string sourceReferenceType,
            string link,
            string type,
            string source,
            long id = 0)
        {
            switch (destinationReferenceType)
            {
                case "Issues": SynchronizeIssues(
                    destinationSiteId,
                    destination,
                    sourceSiteId,
                    sourceReferenceType,
                    link,
                    type,
                    source,
                    id); break;
                case "Results": SynchronizeResults(
                    destinationSiteId,
                    destination,
                    sourceSiteId,
                    sourceReferenceType,
                    link,
                    type,
                    source,
                    id); break;
                case "Wikis": SynchronizeWikis(
                    destinationSiteId,
                    destination,
                    sourceSiteId,
                    sourceReferenceType,
                    link,
                    type,
                    source,
                    id); break;
            }
            return Messages.ResponseSynchronizationCompleted().ToJson();        
        }

        private static void SynchronizeIssues(
            long destinationSiteId,
            string destination,
            long sourceSiteId,
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
                        sourceSiteId,
                        sourceReferenceType,
                        link,
                        type,
                        source),
                    where: Rds.IssuesWhere()
                        .SiteId(destinationSiteId)
                        .IssueId(issueId, _using: issueId != 0),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        private static Rds.IssuesParamCollection IssuesParamCollection(
            string destination,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string link,
            string type,
            string source)
        {
            switch (destination)
            {
                case "WorkValue": return Rds.IssuesParam().WorkValue(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumA": return Rds.IssuesParam().NumA(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumB": return Rds.IssuesParam().NumB(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumC": return Rds.IssuesParam().NumC(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumD": return Rds.IssuesParam().NumD(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumE": return Rds.IssuesParam().NumE(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumF": return Rds.IssuesParam().NumF(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumG": return Rds.IssuesParam().NumG(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumH": return Rds.IssuesParam().NumH(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumI": return Rds.IssuesParam().NumI(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumJ": return Rds.IssuesParam().NumJ(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumK": return Rds.IssuesParam().NumK(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumL": return Rds.IssuesParam().NumL(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumM": return Rds.IssuesParam().NumM(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumN": return Rds.IssuesParam().NumN(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumO": return Rds.IssuesParam().NumO(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumP": return Rds.IssuesParam().NumP(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumQ": return Rds.IssuesParam().NumQ(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumR": return Rds.IssuesParam().NumR(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumS": return Rds.IssuesParam().NumS(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumT": return Rds.IssuesParam().NumT(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumU": return Rds.IssuesParam().NumU(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumV": return Rds.IssuesParam().NumV(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumW": return Rds.IssuesParam().NumW(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumX": return Rds.IssuesParam().NumX(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumY": return Rds.IssuesParam().NumY(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumZ": return Rds.IssuesParam().NumZ(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                default: return null;
            }
        }

        private static void SynchronizeResults(
            long destinationSiteId,
            string destination,
            long sourceSiteId,
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
                        sourceSiteId,
                        sourceReferenceType,
                        link,
                        type,
                        source),
                    where: Rds.ResultsWhere()
                        .SiteId(destinationSiteId)
                        .ResultId(resultId, _using: resultId != 0),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        private static Rds.ResultsParamCollection ResultsParamCollection(
            string destination,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string link,
            string type,
            string source)
        {
            switch (destination)
            {
                case "NumA": return Rds.ResultsParam().NumA(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumB": return Rds.ResultsParam().NumB(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumC": return Rds.ResultsParam().NumC(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumD": return Rds.ResultsParam().NumD(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumE": return Rds.ResultsParam().NumE(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumF": return Rds.ResultsParam().NumF(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumG": return Rds.ResultsParam().NumG(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumH": return Rds.ResultsParam().NumH(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumI": return Rds.ResultsParam().NumI(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumJ": return Rds.ResultsParam().NumJ(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumK": return Rds.ResultsParam().NumK(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumL": return Rds.ResultsParam().NumL(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumM": return Rds.ResultsParam().NumM(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumN": return Rds.ResultsParam().NumN(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumO": return Rds.ResultsParam().NumO(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumP": return Rds.ResultsParam().NumP(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumQ": return Rds.ResultsParam().NumQ(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumR": return Rds.ResultsParam().NumR(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumS": return Rds.ResultsParam().NumS(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumT": return Rds.ResultsParam().NumT(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumU": return Rds.ResultsParam().NumU(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumV": return Rds.ResultsParam().NumV(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumW": return Rds.ResultsParam().NumW(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumX": return Rds.ResultsParam().NumX(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumY": return Rds.ResultsParam().NumY(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                case "NumZ": return Rds.ResultsParam().NumZ(sub:
                    Select(destinationPk, sourceSiteId, sourceReferenceType, link, type, source));
                default: return null;
            }
        }

        private static void SynchronizeWikis(
            long destinationSiteId,
            string destination,
            long sourceSiteId,
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
                        sourceSiteId,
                        sourceReferenceType,
                        link,
                        type,
                        source),
                    where: Rds.WikisWhere()
                        .SiteId(destinationSiteId)
                        .WikiId(wikiId, _using: wikiId != 0),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }

        private static Rds.WikisParamCollection WikisParamCollection(
            string destination,
            string destinationPk,
            long sourceSiteId,
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
            long sourceSiteId,
            string sourceReferenceType,
            string link,
            string type,
            string source)
        {
            switch (type)
            {
                case "Count": return SelectCount(
                    destinationPk, sourceSiteId, sourceReferenceType, link);
                case "Total": return SelectTotal(
                    destinationPk, sourceSiteId, sourceReferenceType, link, source);
                case "Average": return SelectAverage(
                    destinationPk, sourceSiteId, sourceReferenceType, link, source);
                case "Max": return SelectMax(
                    destinationPk, sourceSiteId, sourceReferenceType, link, source);
                case "Min": return SelectMin(
                    destinationPk, sourceSiteId, sourceReferenceType, link, source);
                default: return null;
            }
        }

        private static SqlSelect SelectCount(
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string link)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    where: IssuesWhere(destinationPk, sourceSiteId, link));
                case "Results": return Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    where: ResultsWhere(destinationPk, sourceSiteId, link));
                case "Wikis": return Rds.SelectWikis(
                    column: Rds.WikisColumn().WikisCount(),
                    where: WikisWhere(destinationPk, sourceSiteId, link));
                default: return null;
            }
        }

        private static SqlSelect SelectTotal(
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string link,
            string source)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesTotalColumn(source),
                    where: IssuesWhere(destinationPk, sourceSiteId, link));
                case "Results": return Rds.SelectResults(
                    column: ResultsTotalColumn(source),
                    where: ResultsWhere(destinationPk, sourceSiteId, link));
                case "Wikis": return Rds.SelectWikis(
                    column: WikisTotalColumn(source),
                    where: WikisWhere(destinationPk, sourceSiteId, link));
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
                case "NumI": return Rds.IssuesColumn().NumITotal();
                case "NumJ": return Rds.IssuesColumn().NumJTotal();
                case "NumK": return Rds.IssuesColumn().NumKTotal();
                case "NumL": return Rds.IssuesColumn().NumLTotal();
                case "NumM": return Rds.IssuesColumn().NumMTotal();
                case "NumN": return Rds.IssuesColumn().NumNTotal();
                case "NumO": return Rds.IssuesColumn().NumOTotal();
                case "NumP": return Rds.IssuesColumn().NumPTotal();
                case "NumQ": return Rds.IssuesColumn().NumQTotal();
                case "NumR": return Rds.IssuesColumn().NumRTotal();
                case "NumS": return Rds.IssuesColumn().NumSTotal();
                case "NumT": return Rds.IssuesColumn().NumTTotal();
                case "NumU": return Rds.IssuesColumn().NumUTotal();
                case "NumV": return Rds.IssuesColumn().NumVTotal();
                case "NumW": return Rds.IssuesColumn().NumWTotal();
                case "NumX": return Rds.IssuesColumn().NumXTotal();
                case "NumY": return Rds.IssuesColumn().NumYTotal();
                case "NumZ": return Rds.IssuesColumn().NumZTotal();
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
                case "NumI": return Rds.ResultsColumn().NumITotal();
                case "NumJ": return Rds.ResultsColumn().NumJTotal();
                case "NumK": return Rds.ResultsColumn().NumKTotal();
                case "NumL": return Rds.ResultsColumn().NumLTotal();
                case "NumM": return Rds.ResultsColumn().NumMTotal();
                case "NumN": return Rds.ResultsColumn().NumNTotal();
                case "NumO": return Rds.ResultsColumn().NumOTotal();
                case "NumP": return Rds.ResultsColumn().NumPTotal();
                case "NumQ": return Rds.ResultsColumn().NumQTotal();
                case "NumR": return Rds.ResultsColumn().NumRTotal();
                case "NumS": return Rds.ResultsColumn().NumSTotal();
                case "NumT": return Rds.ResultsColumn().NumTTotal();
                case "NumU": return Rds.ResultsColumn().NumUTotal();
                case "NumV": return Rds.ResultsColumn().NumVTotal();
                case "NumW": return Rds.ResultsColumn().NumWTotal();
                case "NumX": return Rds.ResultsColumn().NumXTotal();
                case "NumY": return Rds.ResultsColumn().NumYTotal();
                case "NumZ": return Rds.ResultsColumn().NumZTotal();
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
            long sourceSiteId,
            string sourceReferenceType,
            string link,
            string source)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesAverageColumn(source),
                    where: IssuesWhere(destinationPk, sourceSiteId, link));
                case "Results": return Rds.SelectResults(
                    column: ResultsAverageColumn(source),
                    where: ResultsWhere(destinationPk, sourceSiteId, link));
                case "Wikis": return Rds.SelectWikis(
                    column: WikisAverageColumn(source),
                    where: WikisWhere(destinationPk, sourceSiteId, link));
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
                case "NumI": return Rds.IssuesColumn().NumIAverage();
                case "NumJ": return Rds.IssuesColumn().NumJAverage();
                case "NumK": return Rds.IssuesColumn().NumKAverage();
                case "NumL": return Rds.IssuesColumn().NumLAverage();
                case "NumM": return Rds.IssuesColumn().NumMAverage();
                case "NumN": return Rds.IssuesColumn().NumNAverage();
                case "NumO": return Rds.IssuesColumn().NumOAverage();
                case "NumP": return Rds.IssuesColumn().NumPAverage();
                case "NumQ": return Rds.IssuesColumn().NumQAverage();
                case "NumR": return Rds.IssuesColumn().NumRAverage();
                case "NumS": return Rds.IssuesColumn().NumSAverage();
                case "NumT": return Rds.IssuesColumn().NumTAverage();
                case "NumU": return Rds.IssuesColumn().NumUAverage();
                case "NumV": return Rds.IssuesColumn().NumVAverage();
                case "NumW": return Rds.IssuesColumn().NumWAverage();
                case "NumX": return Rds.IssuesColumn().NumXAverage();
                case "NumY": return Rds.IssuesColumn().NumYAverage();
                case "NumZ": return Rds.IssuesColumn().NumZAverage();
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
                case "NumI": return Rds.ResultsColumn().NumIAverage();
                case "NumJ": return Rds.ResultsColumn().NumJAverage();
                case "NumK": return Rds.ResultsColumn().NumKAverage();
                case "NumL": return Rds.ResultsColumn().NumLAverage();
                case "NumM": return Rds.ResultsColumn().NumMAverage();
                case "NumN": return Rds.ResultsColumn().NumNAverage();
                case "NumO": return Rds.ResultsColumn().NumOAverage();
                case "NumP": return Rds.ResultsColumn().NumPAverage();
                case "NumQ": return Rds.ResultsColumn().NumQAverage();
                case "NumR": return Rds.ResultsColumn().NumRAverage();
                case "NumS": return Rds.ResultsColumn().NumSAverage();
                case "NumT": return Rds.ResultsColumn().NumTAverage();
                case "NumU": return Rds.ResultsColumn().NumUAverage();
                case "NumV": return Rds.ResultsColumn().NumVAverage();
                case "NumW": return Rds.ResultsColumn().NumWAverage();
                case "NumX": return Rds.ResultsColumn().NumXAverage();
                case "NumY": return Rds.ResultsColumn().NumYAverage();
                case "NumZ": return Rds.ResultsColumn().NumZAverage();
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
            long sourceSiteId,
            string sourceReferenceType,
            string link,
            string source)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesMaxColumn(source),
                    where: IssuesWhere(destinationPk, sourceSiteId, link));
                case "Results": return Rds.SelectResults(
                    column: ResultsMaxColumn(source),
                    where: ResultsWhere(destinationPk, sourceSiteId, link));
                case "Wikis": return Rds.SelectWikis(
                    column: WikisMaxColumn(source),
                    where: WikisWhere(destinationPk, sourceSiteId, link));
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
                case "NumI": return Rds.IssuesColumn().NumIMax();
                case "NumJ": return Rds.IssuesColumn().NumJMax();
                case "NumK": return Rds.IssuesColumn().NumKMax();
                case "NumL": return Rds.IssuesColumn().NumLMax();
                case "NumM": return Rds.IssuesColumn().NumMMax();
                case "NumN": return Rds.IssuesColumn().NumNMax();
                case "NumO": return Rds.IssuesColumn().NumOMax();
                case "NumP": return Rds.IssuesColumn().NumPMax();
                case "NumQ": return Rds.IssuesColumn().NumQMax();
                case "NumR": return Rds.IssuesColumn().NumRMax();
                case "NumS": return Rds.IssuesColumn().NumSMax();
                case "NumT": return Rds.IssuesColumn().NumTMax();
                case "NumU": return Rds.IssuesColumn().NumUMax();
                case "NumV": return Rds.IssuesColumn().NumVMax();
                case "NumW": return Rds.IssuesColumn().NumWMax();
                case "NumX": return Rds.IssuesColumn().NumXMax();
                case "NumY": return Rds.IssuesColumn().NumYMax();
                case "NumZ": return Rds.IssuesColumn().NumZMax();
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
                case "NumI": return Rds.ResultsColumn().NumIMax();
                case "NumJ": return Rds.ResultsColumn().NumJMax();
                case "NumK": return Rds.ResultsColumn().NumKMax();
                case "NumL": return Rds.ResultsColumn().NumLMax();
                case "NumM": return Rds.ResultsColumn().NumMMax();
                case "NumN": return Rds.ResultsColumn().NumNMax();
                case "NumO": return Rds.ResultsColumn().NumOMax();
                case "NumP": return Rds.ResultsColumn().NumPMax();
                case "NumQ": return Rds.ResultsColumn().NumQMax();
                case "NumR": return Rds.ResultsColumn().NumRMax();
                case "NumS": return Rds.ResultsColumn().NumSMax();
                case "NumT": return Rds.ResultsColumn().NumTMax();
                case "NumU": return Rds.ResultsColumn().NumUMax();
                case "NumV": return Rds.ResultsColumn().NumVMax();
                case "NumW": return Rds.ResultsColumn().NumWMax();
                case "NumX": return Rds.ResultsColumn().NumXMax();
                case "NumY": return Rds.ResultsColumn().NumYMax();
                case "NumZ": return Rds.ResultsColumn().NumZMax();
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
            long sourceSiteId,
            string sourceReferenceType,
            string link,
            string source)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesMinColumn(source),
                    where: IssuesWhere(destinationPk, sourceSiteId, link));
                case "Results": return Rds.SelectResults(
                    column: ResultsMinColumn(source),
                    where: ResultsWhere(destinationPk, sourceSiteId, link));
                case "Wikis": return Rds.SelectWikis(
                    column: WikisMinColumn(source),
                    where: WikisWhere(destinationPk, sourceSiteId, link));
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
                case "NumI": return Rds.IssuesColumn().NumIMin();
                case "NumJ": return Rds.IssuesColumn().NumJMin();
                case "NumK": return Rds.IssuesColumn().NumKMin();
                case "NumL": return Rds.IssuesColumn().NumLMin();
                case "NumM": return Rds.IssuesColumn().NumMMin();
                case "NumN": return Rds.IssuesColumn().NumNMin();
                case "NumO": return Rds.IssuesColumn().NumOMin();
                case "NumP": return Rds.IssuesColumn().NumPMin();
                case "NumQ": return Rds.IssuesColumn().NumQMin();
                case "NumR": return Rds.IssuesColumn().NumRMin();
                case "NumS": return Rds.IssuesColumn().NumSMin();
                case "NumT": return Rds.IssuesColumn().NumTMin();
                case "NumU": return Rds.IssuesColumn().NumUMin();
                case "NumV": return Rds.IssuesColumn().NumVMin();
                case "NumW": return Rds.IssuesColumn().NumWMin();
                case "NumX": return Rds.IssuesColumn().NumXMin();
                case "NumY": return Rds.IssuesColumn().NumYMin();
                case "NumZ": return Rds.IssuesColumn().NumZMin();
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
                case "NumI": return Rds.ResultsColumn().NumIMin();
                case "NumJ": return Rds.ResultsColumn().NumJMin();
                case "NumK": return Rds.ResultsColumn().NumKMin();
                case "NumL": return Rds.ResultsColumn().NumLMin();
                case "NumM": return Rds.ResultsColumn().NumMMin();
                case "NumN": return Rds.ResultsColumn().NumNMin();
                case "NumO": return Rds.ResultsColumn().NumOMin();
                case "NumP": return Rds.ResultsColumn().NumPMin();
                case "NumQ": return Rds.ResultsColumn().NumQMin();
                case "NumR": return Rds.ResultsColumn().NumRMin();
                case "NumS": return Rds.ResultsColumn().NumSMin();
                case "NumT": return Rds.ResultsColumn().NumTMin();
                case "NumU": return Rds.ResultsColumn().NumUMin();
                case "NumV": return Rds.ResultsColumn().NumVMin();
                case "NumW": return Rds.ResultsColumn().NumWMin();
                case "NumX": return Rds.ResultsColumn().NumXMin();
                case "NumY": return Rds.ResultsColumn().NumYMin();
                case "NumZ": return Rds.ResultsColumn().NumZMin();
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

        private static SqlWhereCollection IssuesWhere(string destinationPk, long sourceSiteId, string link)
        {
            switch (link)
            {
                case "ClassA": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassA(raw: destinationPk);
                case "ClassB": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassB(raw: destinationPk);
                case "ClassC": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassC(raw: destinationPk);
                case "ClassD": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassD(raw: destinationPk);
                case "ClassE": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassE(raw: destinationPk);
                case "ClassF": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassF(raw: destinationPk);
                case "ClassG": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassG(raw: destinationPk);
                case "ClassH": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassH(raw: destinationPk);
                case "ClassI": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassI(raw: destinationPk);
                case "ClassJ": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassJ(raw: destinationPk);
                case "ClassK": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassK(raw: destinationPk);
                case "ClassL": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassL(raw: destinationPk);
                case "ClassM": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassM(raw: destinationPk);
                case "ClassN": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassN(raw: destinationPk);
                case "ClassO": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassO(raw: destinationPk);
                case "ClassP": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassP(raw: destinationPk);
                case "ClassQ": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassQ(raw: destinationPk);
                case "ClassR": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassR(raw: destinationPk);
                case "ClassS": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassS(raw: destinationPk);
                case "ClassT": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassT(raw: destinationPk);
                case "ClassU": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassU(raw: destinationPk);
                case "ClassV": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassV(raw: destinationPk);
                case "ClassW": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassW(raw: destinationPk);
                case "ClassX": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassX(raw: destinationPk);
                case "ClassY": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassY(raw: destinationPk);
                case "ClassZ": return Rds.IssuesWhere()
                    .SiteId(sourceSiteId)
                    .ClassZ(raw: destinationPk);
                default: return null;
            }
        }

        private static SqlWhereCollection ResultsWhere(string destinationPk, long sourceSiteId, string link)
        {
            switch (link)
            {
                case "ClassA": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassA(raw: destinationPk);
                case "ClassB": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassB(raw: destinationPk);
                case "ClassC": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassC(raw: destinationPk);
                case "ClassD": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassD(raw: destinationPk);
                case "ClassE": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassE(raw: destinationPk);
                case "ClassF": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassF(raw: destinationPk);
                case "ClassG": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassG(raw: destinationPk);
                case "ClassH": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassH(raw: destinationPk);
                case "ClassI": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassI(raw: destinationPk);
                case "ClassJ": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassJ(raw: destinationPk);
                case "ClassK": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassK(raw: destinationPk);
                case "ClassL": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassL(raw: destinationPk);
                case "ClassM": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassM(raw: destinationPk);
                case "ClassN": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassN(raw: destinationPk);
                case "ClassO": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassO(raw: destinationPk);
                case "ClassP": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassP(raw: destinationPk);
                case "ClassQ": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassQ(raw: destinationPk);
                case "ClassR": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassR(raw: destinationPk);
                case "ClassS": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassS(raw: destinationPk);
                case "ClassT": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassT(raw: destinationPk);
                case "ClassU": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassU(raw: destinationPk);
                case "ClassV": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassV(raw: destinationPk);
                case "ClassW": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassW(raw: destinationPk);
                case "ClassX": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassX(raw: destinationPk);
                case "ClassY": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassY(raw: destinationPk);
                case "ClassZ": return Rds.ResultsWhere()
                    .SiteId(sourceSiteId)
                    .ClassZ(raw: destinationPk);
                default: return null;
            }
        }

        private static SqlWhereCollection WikisWhere(string destinationPk, long sourceSiteId, string link)
        {
            switch (link)
            {
                default: return null;
            }
        }
    }
}
