using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Summaries
    {
        public static void Synchronize(SiteSettings ss, int id)
        {
            var destinationSs = ss.Destinations?.Get(ss.Summaries?.Get(id)?.SiteId ?? 0);
            var summary = ss.Summaries?.Get(id);
            if (destinationSs != null && summary != null)
            {
                Synchronize(
                    ss,
                    summary.SiteId,
                    summary.DestinationReferenceType,
                    summary.DestinationColumn,
                    destinationSs.Views?.Get(summary.DestinationCondition),
                    summary.SetZeroWhenOutOfCondition == true,
                    ss.SiteId,
                    ss.ReferenceType,
                    summary.LinkColumn,
                    summary.Type,
                    summary.SourceColumn,
                    ss.Views?.Get(summary.SourceCondition));
                FormulaUtilities.Synchronize(new SiteModel(summary.SiteId));
            }
        }

        public static string Synchronize(
            SiteSettings ss,
            long destinationSiteId,
            string destinationReferenceType,
            string destinationColumn,
            View destinationCondition,
            bool setZeroWhenOutOfCondition,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition,
            long id = 0)
        {
            switch (destinationReferenceType)
            {
                case "Issues":
                    SynchronizeIssues(
                        ss,
                        destinationSiteId,
                        destinationColumn,
                        destinationCondition,
                        setZeroWhenOutOfCondition,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition,
                        id);
                    break;
                case "Results":
                    SynchronizeResults(
                        ss,
                        destinationSiteId,
                        destinationColumn,
                        destinationCondition,
                        setZeroWhenOutOfCondition,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition,
                        id);
                    break;
            }
            return Messages.ResponseSynchronizationCompleted().ToJson();
        }

        private static void SynchronizeIssues(
            SiteSettings ss,
            long destinationSiteId,
            string destinationColumn,
            View destinationCondition,
            bool setZeroWhenOutOfCondition,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition,
            long issueId = 0)
        {
            var statements = new List<SqlStatement>
            {
                Rds.UpdateIssues(
                    param: IssuesParam(
                        ss,
                        destinationColumn,
                        "convert(nvarchar, [Issues].[IssueId])",
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition),
                    where: Where(
                        ss,
                        destinationCondition,
                        Rds.IssuesWhere()
                            .SiteId(destinationSiteId)
                            .IssueId(issueId, _using: issueId != 0)),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false)
            };
            if (destinationCondition != null && setZeroWhenOutOfCondition)
            {
                statements.Add(Rds.UpdateIssues(
                    param: IssuesZeroParam(destinationColumn),
                    where: Rds.IssuesWhere()
                        .SiteId(destinationSiteId)
                        .IssueId(issueId, _using: issueId != 0)
                        .IssueId_In(
                            sub: Rds.SelectIssues(
                                column: Rds.IssuesColumn().IssueId(),
                                where: destinationCondition.Where(ss)),
                            negative: true),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
            }
            Rds.ExecuteNonQuery(transactional: true, statements: statements.ToArray());
        }

        private static Rds.IssuesParamCollection IssuesParam(
            SiteSettings ss,
            string destinationColumn,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (destinationColumn)
            {
                case "WorkValue": return Rds.IssuesParam().WorkValue(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumA": return Rds.IssuesParam().NumA(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumB": return Rds.IssuesParam().NumB(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumC": return Rds.IssuesParam().NumC(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumD": return Rds.IssuesParam().NumD(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumE": return Rds.IssuesParam().NumE(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumF": return Rds.IssuesParam().NumF(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumG": return Rds.IssuesParam().NumG(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumH": return Rds.IssuesParam().NumH(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumI": return Rds.IssuesParam().NumI(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumJ": return Rds.IssuesParam().NumJ(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumK": return Rds.IssuesParam().NumK(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumL": return Rds.IssuesParam().NumL(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumM": return Rds.IssuesParam().NumM(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumN": return Rds.IssuesParam().NumN(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumO": return Rds.IssuesParam().NumO(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumP": return Rds.IssuesParam().NumP(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumQ": return Rds.IssuesParam().NumQ(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumR": return Rds.IssuesParam().NumR(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumS": return Rds.IssuesParam().NumS(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumT": return Rds.IssuesParam().NumT(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumU": return Rds.IssuesParam().NumU(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumV": return Rds.IssuesParam().NumV(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumW": return Rds.IssuesParam().NumW(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumX": return Rds.IssuesParam().NumX(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumY": return Rds.IssuesParam().NumY(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumZ": return Rds.IssuesParam().NumZ(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                default: return null;
            }
        }

        private static Rds.IssuesParamCollection IssuesZeroParam(
            string destinationColumn)
        {
            switch (destinationColumn)
            {
                case "WorkValue": return Rds.IssuesParam().WorkValue(0);
                case "NumA": return Rds.IssuesParam().NumA(0);
                case "NumB": return Rds.IssuesParam().NumB(0);
                case "NumC": return Rds.IssuesParam().NumC(0);
                case "NumD": return Rds.IssuesParam().NumD(0);
                case "NumE": return Rds.IssuesParam().NumE(0);
                case "NumF": return Rds.IssuesParam().NumF(0);
                case "NumG": return Rds.IssuesParam().NumG(0);
                case "NumH": return Rds.IssuesParam().NumH(0);
                case "NumI": return Rds.IssuesParam().NumI(0);
                case "NumJ": return Rds.IssuesParam().NumJ(0);
                case "NumK": return Rds.IssuesParam().NumK(0);
                case "NumL": return Rds.IssuesParam().NumL(0);
                case "NumM": return Rds.IssuesParam().NumM(0);
                case "NumN": return Rds.IssuesParam().NumN(0);
                case "NumO": return Rds.IssuesParam().NumO(0);
                case "NumP": return Rds.IssuesParam().NumP(0);
                case "NumQ": return Rds.IssuesParam().NumQ(0);
                case "NumR": return Rds.IssuesParam().NumR(0);
                case "NumS": return Rds.IssuesParam().NumS(0);
                case "NumT": return Rds.IssuesParam().NumT(0);
                case "NumU": return Rds.IssuesParam().NumU(0);
                case "NumV": return Rds.IssuesParam().NumV(0);
                case "NumW": return Rds.IssuesParam().NumW(0);
                case "NumX": return Rds.IssuesParam().NumX(0);
                case "NumY": return Rds.IssuesParam().NumY(0);
                case "NumZ": return Rds.IssuesParam().NumZ(0);
                default: return null;
            }
        }

        private static void SynchronizeResults(
            SiteSettings ss,
            long destinationSiteId,
            string destinationColumn,
            View destinationCondition,
            bool setZeroWhenOutOfCondition,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition,
            long resultId = 0)
        {
            var statements = new List<SqlStatement>
            {
                Rds.UpdateResults(
                    param: ResultsParam(
                        ss,
                        destinationColumn,
                        "convert(nvarchar, [Results].[ResultId])",
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition),
                    where: Where(
                        ss,
                        destinationCondition,
                        Rds.ResultsWhere()
                            .SiteId(destinationSiteId)
                            .ResultId(resultId, _using: resultId != 0)),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false)
            };
            if (destinationCondition != null && setZeroWhenOutOfCondition)
            {
                statements.Add(Rds.UpdateResults(
                    param: ResultsZeroParam(destinationColumn),
                    where: Rds.ResultsWhere()
                        .SiteId(destinationSiteId)
                        .ResultId(resultId, _using: resultId != 0)
                        .ResultId_In(
                            sub: Rds.SelectResults(
                                column: Rds.ResultsColumn().ResultId(),
                                where: destinationCondition.Where(ss)),
                            negative: true),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
            }
            Rds.ExecuteNonQuery(transactional: true, statements: statements.ToArray());
        }

        private static Rds.ResultsParamCollection ResultsParam(
            SiteSettings ss,
            string destinationColumn,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (destinationColumn)
            {
                case "NumA": return Rds.ResultsParam().NumA(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumB": return Rds.ResultsParam().NumB(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumC": return Rds.ResultsParam().NumC(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumD": return Rds.ResultsParam().NumD(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumE": return Rds.ResultsParam().NumE(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumF": return Rds.ResultsParam().NumF(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumG": return Rds.ResultsParam().NumG(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumH": return Rds.ResultsParam().NumH(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumI": return Rds.ResultsParam().NumI(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumJ": return Rds.ResultsParam().NumJ(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumK": return Rds.ResultsParam().NumK(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumL": return Rds.ResultsParam().NumL(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumM": return Rds.ResultsParam().NumM(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumN": return Rds.ResultsParam().NumN(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumO": return Rds.ResultsParam().NumO(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumP": return Rds.ResultsParam().NumP(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumQ": return Rds.ResultsParam().NumQ(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumR": return Rds.ResultsParam().NumR(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumS": return Rds.ResultsParam().NumS(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumT": return Rds.ResultsParam().NumT(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumU": return Rds.ResultsParam().NumU(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumV": return Rds.ResultsParam().NumV(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumW": return Rds.ResultsParam().NumW(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumX": return Rds.ResultsParam().NumX(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumY": return Rds.ResultsParam().NumY(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                case "NumZ": return Rds.ResultsParam().NumZ(sub:
                    Select(
                        ss,
                        destinationPk,
                        sourceSiteId,
                        sourceReferenceType,
                        linkColumn,
                        type,
                        sourceColumn,
                        sourceCondition));
                default: return null;
            }
        }

        private static Rds.ResultsParamCollection ResultsZeroParam(
            string destinationColumn)
        {
            switch (destinationColumn)
            {
                case "NumA": return Rds.ResultsParam().NumA(0);
                case "NumB": return Rds.ResultsParam().NumB(0);
                case "NumC": return Rds.ResultsParam().NumC(0);
                case "NumD": return Rds.ResultsParam().NumD(0);
                case "NumE": return Rds.ResultsParam().NumE(0);
                case "NumF": return Rds.ResultsParam().NumF(0);
                case "NumG": return Rds.ResultsParam().NumG(0);
                case "NumH": return Rds.ResultsParam().NumH(0);
                case "NumI": return Rds.ResultsParam().NumI(0);
                case "NumJ": return Rds.ResultsParam().NumJ(0);
                case "NumK": return Rds.ResultsParam().NumK(0);
                case "NumL": return Rds.ResultsParam().NumL(0);
                case "NumM": return Rds.ResultsParam().NumM(0);
                case "NumN": return Rds.ResultsParam().NumN(0);
                case "NumO": return Rds.ResultsParam().NumO(0);
                case "NumP": return Rds.ResultsParam().NumP(0);
                case "NumQ": return Rds.ResultsParam().NumQ(0);
                case "NumR": return Rds.ResultsParam().NumR(0);
                case "NumS": return Rds.ResultsParam().NumS(0);
                case "NumT": return Rds.ResultsParam().NumT(0);
                case "NumU": return Rds.ResultsParam().NumU(0);
                case "NumV": return Rds.ResultsParam().NumV(0);
                case "NumW": return Rds.ResultsParam().NumW(0);
                case "NumX": return Rds.ResultsParam().NumX(0);
                case "NumY": return Rds.ResultsParam().NumY(0);
                case "NumZ": return Rds.ResultsParam().NumZ(0);
                default: return null;
            }
        }

        private static SqlSelect Select(
            SiteSettings ss,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (type)
            {
                case "Count": return SelectCount(
                    ss,
                    destinationPk,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceCondition);
                case "Total": return SelectTotal(
                    ss,
                    destinationPk,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceColumn,
                    sourceCondition);
                case "Average": return SelectAverage(
                    ss,
                    destinationPk,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceColumn,
                    sourceCondition);
                case "Min": return SelectMin(
                    ss,
                    destinationPk,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceColumn,
                    sourceCondition);
                case "Max": return SelectMax(
                    ss,
                    destinationPk,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceColumn,
                    sourceCondition);
                default: return null;
            }
        }

        private static SqlSelect SelectCount(
            SiteSettings ss,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    _as: "s",
                    column: Rds.IssuesColumn().IssuesCount(),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinationPk, sourceSiteId, linkColumn)));
                case "Results": return Rds.SelectResults(
                    _as: "s",
                    column: Rds.ResultsColumn().ResultsCount(),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinationPk, sourceSiteId, linkColumn)));
                default: return null;
            }
        }

        private static SqlSelect SelectTotal(
            SiteSettings ss,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    _as: "s",
                    column: IssuesTotalColumn(sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinationPk, sourceSiteId, linkColumn)));
                case "Results": return Rds.SelectResults(
                    _as: "s",
                    column: ResultsTotalColumn(sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinationPk, sourceSiteId, linkColumn)));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesTotalColumn(string sourceColumn)
        {
            switch (sourceColumn)
            {
                case "WorkValue":
                    return Rds.IssuesColumn()
                        .WorkValue(tableName: "s", function: Sqls.Functions.Sum);
                case "RemainingWorkValue":
                    return Rds.IssuesColumn()
                        .RemainingWorkValue(tableName: "s", function: Sqls.Functions.Sum);
                case "NumA":
                    return Rds.IssuesColumn()
                        .NumA(tableName: "s", function: Sqls.Functions.Sum);
                case "NumB":
                    return Rds.IssuesColumn()
                        .NumB(tableName: "s", function: Sqls.Functions.Sum);
                case "NumC":
                    return Rds.IssuesColumn()
                        .NumC(tableName: "s", function: Sqls.Functions.Sum);
                case "NumD":
                    return Rds.IssuesColumn()
                        .NumD(tableName: "s", function: Sqls.Functions.Sum);
                case "NumE":
                    return Rds.IssuesColumn()
                        .NumE(tableName: "s", function: Sqls.Functions.Sum);
                case "NumF":
                    return Rds.IssuesColumn()
                        .NumF(tableName: "s", function: Sqls.Functions.Sum);
                case "NumG":
                    return Rds.IssuesColumn()
                        .NumG(tableName: "s", function: Sqls.Functions.Sum);
                case "NumH":
                    return Rds.IssuesColumn()
                        .NumH(tableName: "s", function: Sqls.Functions.Sum);
                case "NumI":
                    return Rds.IssuesColumn()
                        .NumI(tableName: "s", function: Sqls.Functions.Sum);
                case "NumJ":
                    return Rds.IssuesColumn()
                        .NumJ(tableName: "s", function: Sqls.Functions.Sum);
                case "NumK":
                    return Rds.IssuesColumn()
                        .NumK(tableName: "s", function: Sqls.Functions.Sum);
                case "NumL":
                    return Rds.IssuesColumn()
                        .NumL(tableName: "s", function: Sqls.Functions.Sum);
                case "NumM":
                    return Rds.IssuesColumn()
                        .NumM(tableName: "s", function: Sqls.Functions.Sum);
                case "NumN":
                    return Rds.IssuesColumn()
                        .NumN(tableName: "s", function: Sqls.Functions.Sum);
                case "NumO":
                    return Rds.IssuesColumn()
                        .NumO(tableName: "s", function: Sqls.Functions.Sum);
                case "NumP":
                    return Rds.IssuesColumn()
                        .NumP(tableName: "s", function: Sqls.Functions.Sum);
                case "NumQ":
                    return Rds.IssuesColumn()
                        .NumQ(tableName: "s", function: Sqls.Functions.Sum);
                case "NumR":
                    return Rds.IssuesColumn()
                        .NumR(tableName: "s", function: Sqls.Functions.Sum);
                case "NumS":
                    return Rds.IssuesColumn()
                        .NumS(tableName: "s", function: Sqls.Functions.Sum);
                case "NumT":
                    return Rds.IssuesColumn()
                        .NumT(tableName: "s", function: Sqls.Functions.Sum);
                case "NumU":
                    return Rds.IssuesColumn()
                        .NumU(tableName: "s", function: Sqls.Functions.Sum);
                case "NumV":
                    return Rds.IssuesColumn()
                        .NumV(tableName: "s", function: Sqls.Functions.Sum);
                case "NumW":
                    return Rds.IssuesColumn()
                        .NumW(tableName: "s", function: Sqls.Functions.Sum);
                case "NumX":
                    return Rds.IssuesColumn()
                        .NumX(tableName: "s", function: Sqls.Functions.Sum);
                case "NumY":
                    return Rds.IssuesColumn()
                        .NumY(tableName: "s", function: Sqls.Functions.Sum);
                case "NumZ":
                    return Rds.IssuesColumn()
                        .NumZ(tableName: "s", function: Sqls.Functions.Sum);
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsTotalColumn(string sourceColumn)
        {
            switch (sourceColumn)
            {
                case "NumA":
                    return Rds.ResultsColumn()
                        .NumA(tableName: "s", function: Sqls.Functions.Sum);
                case "NumB":
                    return Rds.ResultsColumn()
                        .NumB(tableName: "s", function: Sqls.Functions.Sum);
                case "NumC":
                    return Rds.ResultsColumn()
                        .NumC(tableName: "s", function: Sqls.Functions.Sum);
                case "NumD":
                    return Rds.ResultsColumn()
                        .NumD(tableName: "s", function: Sqls.Functions.Sum);
                case "NumE":
                    return Rds.ResultsColumn()
                        .NumE(tableName: "s", function: Sqls.Functions.Sum);
                case "NumF":
                    return Rds.ResultsColumn()
                        .NumF(tableName: "s", function: Sqls.Functions.Sum);
                case "NumG":
                    return Rds.ResultsColumn()
                        .NumG(tableName: "s", function: Sqls.Functions.Sum);
                case "NumH":
                    return Rds.ResultsColumn()
                        .NumH(tableName: "s", function: Sqls.Functions.Sum);
                case "NumI":
                    return Rds.ResultsColumn()
                        .NumI(tableName: "s", function: Sqls.Functions.Sum);
                case "NumJ":
                    return Rds.ResultsColumn()
                        .NumJ(tableName: "s", function: Sqls.Functions.Sum);
                case "NumK":
                    return Rds.ResultsColumn()
                        .NumK(tableName: "s", function: Sqls.Functions.Sum);
                case "NumL":
                    return Rds.ResultsColumn()
                        .NumL(tableName: "s", function: Sqls.Functions.Sum);
                case "NumM":
                    return Rds.ResultsColumn()
                        .NumM(tableName: "s", function: Sqls.Functions.Sum);
                case "NumN":
                    return Rds.ResultsColumn()
                        .NumN(tableName: "s", function: Sqls.Functions.Sum);
                case "NumO":
                    return Rds.ResultsColumn()
                        .NumO(tableName: "s", function: Sqls.Functions.Sum);
                case "NumP":
                    return Rds.ResultsColumn()
                        .NumP(tableName: "s", function: Sqls.Functions.Sum);
                case "NumQ":
                    return Rds.ResultsColumn()
                        .NumQ(tableName: "s", function: Sqls.Functions.Sum);
                case "NumR":
                    return Rds.ResultsColumn()
                        .NumR(tableName: "s", function: Sqls.Functions.Sum);
                case "NumS":
                    return Rds.ResultsColumn()
                        .NumS(tableName: "s", function: Sqls.Functions.Sum);
                case "NumT":
                    return Rds.ResultsColumn()
                        .NumT(tableName: "s", function: Sqls.Functions.Sum);
                case "NumU":
                    return Rds.ResultsColumn()
                        .NumU(tableName: "s", function: Sqls.Functions.Sum);
                case "NumV":
                    return Rds.ResultsColumn()
                        .NumV(tableName: "s", function: Sqls.Functions.Sum);
                case "NumW":
                    return Rds.ResultsColumn()
                        .NumW(tableName: "s", function: Sqls.Functions.Sum);
                case "NumX":
                    return Rds.ResultsColumn()
                        .NumX(tableName: "s", function: Sqls.Functions.Sum);
                case "NumY":
                    return Rds.ResultsColumn()
                        .NumY(tableName: "s", function: Sqls.Functions.Sum);
                case "NumZ":
                    return Rds.ResultsColumn()
                        .NumZ(tableName: "s", function: Sqls.Functions.Sum);
                default: return null;
            }
        }

        private static SqlSelect SelectAverage(
            SiteSettings ss,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    _as: "s",
                    column: IssuesAverageColumn(sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinationPk, sourceSiteId, linkColumn)));
                case "Results": return Rds.SelectResults(
                    _as: "s",
                    column: ResultsAverageColumn(sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinationPk, sourceSiteId, linkColumn)));
                default: return null;
            }
        }

        private static SqlSelect SelectMin(
            SiteSettings ss,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    _as: "s",
                    column: IssuesMinColumn(sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinationPk, sourceSiteId, linkColumn)));
                case "Results": return Rds.SelectResults(
                    _as: "s",
                    column: ResultsMinColumn(sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinationPk, sourceSiteId, linkColumn)));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesMinColumn(string sourceColumn)
        {
            switch (sourceColumn)
            {
                case "WorkValue":
                    return Rds.IssuesColumn()
                        .WorkValue(tableName: "s", function: Sqls.Functions.Min);
                case "RemainingWorkValue":
                    return Rds.IssuesColumn()
                        .RemainingWorkValue(tableName: "s", function: Sqls.Functions.Min);
                case "NumA":
                    return Rds.IssuesColumn()
                        .NumA(tableName: "s", function: Sqls.Functions.Min);
                case "NumB":
                    return Rds.IssuesColumn()
                        .NumB(tableName: "s", function: Sqls.Functions.Min);
                case "NumC":
                    return Rds.IssuesColumn()
                        .NumC(tableName: "s", function: Sqls.Functions.Min);
                case "NumD":
                    return Rds.IssuesColumn()
                        .NumD(tableName: "s", function: Sqls.Functions.Min);
                case "NumE":
                    return Rds.IssuesColumn()
                        .NumE(tableName: "s", function: Sqls.Functions.Min);
                case "NumF":
                    return Rds.IssuesColumn()
                        .NumF(tableName: "s", function: Sqls.Functions.Min);
                case "NumG":
                    return Rds.IssuesColumn()
                        .NumG(tableName: "s", function: Sqls.Functions.Min);
                case "NumH":
                    return Rds.IssuesColumn()
                        .NumH(tableName: "s", function: Sqls.Functions.Min);
                case "NumI":
                    return Rds.IssuesColumn()
                        .NumI(tableName: "s", function: Sqls.Functions.Min);
                case "NumJ":
                    return Rds.IssuesColumn()
                        .NumJ(tableName: "s", function: Sqls.Functions.Min);
                case "NumK":
                    return Rds.IssuesColumn()
                        .NumK(tableName: "s", function: Sqls.Functions.Min);
                case "NumL":
                    return Rds.IssuesColumn()
                        .NumL(tableName: "s", function: Sqls.Functions.Min);
                case "NumM":
                    return Rds.IssuesColumn()
                        .NumM(tableName: "s", function: Sqls.Functions.Min);
                case "NumN":
                    return Rds.IssuesColumn()
                        .NumN(tableName: "s", function: Sqls.Functions.Min);
                case "NumO":
                    return Rds.IssuesColumn()
                        .NumO(tableName: "s", function: Sqls.Functions.Min);
                case "NumP":
                    return Rds.IssuesColumn()
                        .NumP(tableName: "s", function: Sqls.Functions.Min);
                case "NumQ":
                    return Rds.IssuesColumn()
                        .NumQ(tableName: "s", function: Sqls.Functions.Min);
                case "NumR":
                    return Rds.IssuesColumn()
                        .NumR(tableName: "s", function: Sqls.Functions.Min);
                case "NumS":
                    return Rds.IssuesColumn()
                        .NumS(tableName: "s", function: Sqls.Functions.Min);
                case "NumT":
                    return Rds.IssuesColumn()
                        .NumT(tableName: "s", function: Sqls.Functions.Min);
                case "NumU":
                    return Rds.IssuesColumn()
                        .NumU(tableName: "s", function: Sqls.Functions.Min);
                case "NumV":
                    return Rds.IssuesColumn()
                        .NumV(tableName: "s", function: Sqls.Functions.Min);
                case "NumW":
                    return Rds.IssuesColumn()
                        .NumW(tableName: "s", function: Sqls.Functions.Min);
                case "NumX":
                    return Rds.IssuesColumn()
                        .NumX(tableName: "s", function: Sqls.Functions.Min);
                case "NumY":
                    return Rds.IssuesColumn()
                        .NumY(tableName: "s", function: Sqls.Functions.Min);
                case "NumZ":
                    return Rds.IssuesColumn()
                        .NumZ(tableName: "s", function: Sqls.Functions.Min);
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsMinColumn(string sourceColumn)
        {
            switch (sourceColumn)
            {
                case "NumA":
                    return Rds.ResultsColumn()
                        .NumA(tableName: "s", function: Sqls.Functions.Min);
                case "NumB":
                    return Rds.ResultsColumn()
                        .NumB(tableName: "s", function: Sqls.Functions.Min);
                case "NumC":
                    return Rds.ResultsColumn()
                        .NumC(tableName: "s", function: Sqls.Functions.Min);
                case "NumD":
                    return Rds.ResultsColumn()
                        .NumD(tableName: "s", function: Sqls.Functions.Min);
                case "NumE":
                    return Rds.ResultsColumn()
                        .NumE(tableName: "s", function: Sqls.Functions.Min);
                case "NumF":
                    return Rds.ResultsColumn()
                        .NumF(tableName: "s", function: Sqls.Functions.Min);
                case "NumG":
                    return Rds.ResultsColumn()
                        .NumG(tableName: "s", function: Sqls.Functions.Min);
                case "NumH":
                    return Rds.ResultsColumn()
                        .NumH(tableName: "s", function: Sqls.Functions.Min);
                case "NumI":
                    return Rds.ResultsColumn()
                        .NumI(tableName: "s", function: Sqls.Functions.Min);
                case "NumJ":
                    return Rds.ResultsColumn()
                        .NumJ(tableName: "s", function: Sqls.Functions.Min);
                case "NumK":
                    return Rds.ResultsColumn()
                        .NumK(tableName: "s", function: Sqls.Functions.Min);
                case "NumL":
                    return Rds.ResultsColumn()
                        .NumL(tableName: "s", function: Sqls.Functions.Min);
                case "NumM":
                    return Rds.ResultsColumn()
                        .NumM(tableName: "s", function: Sqls.Functions.Min);
                case "NumN":
                    return Rds.ResultsColumn()
                        .NumN(tableName: "s", function: Sqls.Functions.Min);
                case "NumO":
                    return Rds.ResultsColumn()
                        .NumO(tableName: "s", function: Sqls.Functions.Min);
                case "NumP":
                    return Rds.ResultsColumn()
                        .NumP(tableName: "s", function: Sqls.Functions.Min);
                case "NumQ":
                    return Rds.ResultsColumn()
                        .NumQ(tableName: "s", function: Sqls.Functions.Min);
                case "NumR":
                    return Rds.ResultsColumn()
                        .NumR(tableName: "s", function: Sqls.Functions.Min);
                case "NumS":
                    return Rds.ResultsColumn()
                        .NumS(tableName: "s", function: Sqls.Functions.Min);
                case "NumT":
                    return Rds.ResultsColumn()
                        .NumT(tableName: "s", function: Sqls.Functions.Min);
                case "NumU":
                    return Rds.ResultsColumn()
                        .NumU(tableName: "s", function: Sqls.Functions.Min);
                case "NumV":
                    return Rds.ResultsColumn()
                        .NumV(tableName: "s", function: Sqls.Functions.Min);
                case "NumW":
                    return Rds.ResultsColumn()
                        .NumW(tableName: "s", function: Sqls.Functions.Min);
                case "NumX":
                    return Rds.ResultsColumn()
                        .NumX(tableName: "s", function: Sqls.Functions.Min);
                case "NumY":
                    return Rds.ResultsColumn()
                        .NumY(tableName: "s", function: Sqls.Functions.Min);
                case "NumZ":
                    return Rds.ResultsColumn()
                        .NumZ(tableName: "s", function: Sqls.Functions.Min);
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesAverageColumn(string sourceColumn)
        {
            switch (sourceColumn)
            {
                case "WorkValue":
                    return Rds.IssuesColumn()
                        .WorkValue(tableName: "s", function: Sqls.Functions.Avg);
                case "RemainingWorkValue":
                    return Rds.IssuesColumn()
                        .RemainingWorkValue(tableName: "s", function: Sqls.Functions.Avg);
                case "NumA":
                    return Rds.IssuesColumn()
                        .NumA(tableName: "s", function: Sqls.Functions.Avg);
                case "NumB":
                    return Rds.IssuesColumn()
                        .NumB(tableName: "s", function: Sqls.Functions.Avg);
                case "NumC":
                    return Rds.IssuesColumn()
                        .NumC(tableName: "s", function: Sqls.Functions.Avg);
                case "NumD":
                    return Rds.IssuesColumn()
                        .NumD(tableName: "s", function: Sqls.Functions.Avg);
                case "NumE":
                    return Rds.IssuesColumn()
                        .NumE(tableName: "s", function: Sqls.Functions.Avg);
                case "NumF":
                    return Rds.IssuesColumn()
                        .NumF(tableName: "s", function: Sqls.Functions.Avg);
                case "NumG":
                    return Rds.IssuesColumn()
                        .NumG(tableName: "s", function: Sqls.Functions.Avg);
                case "NumH":
                    return Rds.IssuesColumn()
                        .NumH(tableName: "s", function: Sqls.Functions.Avg);
                case "NumI":
                    return Rds.IssuesColumn()
                        .NumI(tableName: "s", function: Sqls.Functions.Avg);
                case "NumJ":
                    return Rds.IssuesColumn()
                        .NumJ(tableName: "s", function: Sqls.Functions.Avg);
                case "NumK":
                    return Rds.IssuesColumn()
                        .NumK(tableName: "s", function: Sqls.Functions.Avg);
                case "NumL":
                    return Rds.IssuesColumn()
                        .NumL(tableName: "s", function: Sqls.Functions.Avg);
                case "NumM":
                    return Rds.IssuesColumn()
                        .NumM(tableName: "s", function: Sqls.Functions.Avg);
                case "NumN":
                    return Rds.IssuesColumn()
                        .NumN(tableName: "s", function: Sqls.Functions.Avg);
                case "NumO":
                    return Rds.IssuesColumn()
                        .NumO(tableName: "s", function: Sqls.Functions.Avg);
                case "NumP":
                    return Rds.IssuesColumn()
                        .NumP(tableName: "s", function: Sqls.Functions.Avg);
                case "NumQ":
                    return Rds.IssuesColumn()
                        .NumQ(tableName: "s", function: Sqls.Functions.Avg);
                case "NumR":
                    return Rds.IssuesColumn()
                        .NumR(tableName: "s", function: Sqls.Functions.Avg);
                case "NumS":
                    return Rds.IssuesColumn()
                        .NumS(tableName: "s", function: Sqls.Functions.Avg);
                case "NumT":
                    return Rds.IssuesColumn()
                        .NumT(tableName: "s", function: Sqls.Functions.Avg);
                case "NumU":
                    return Rds.IssuesColumn()
                        .NumU(tableName: "s", function: Sqls.Functions.Avg);
                case "NumV":
                    return Rds.IssuesColumn()
                        .NumV(tableName: "s", function: Sqls.Functions.Avg);
                case "NumW":
                    return Rds.IssuesColumn()
                        .NumW(tableName: "s", function: Sqls.Functions.Avg);
                case "NumX":
                    return Rds.IssuesColumn()
                        .NumX(tableName: "s", function: Sqls.Functions.Avg);
                case "NumY":
                    return Rds.IssuesColumn()
                        .NumY(tableName: "s", function: Sqls.Functions.Avg);
                case "NumZ":
                    return Rds.IssuesColumn()
                        .NumZ(tableName: "s", function: Sqls.Functions.Avg);
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsAverageColumn(string sourceColumn)
        {
            switch (sourceColumn)
            {
                case "NumA":
                    return Rds.ResultsColumn()
                        .NumA(tableName: "s", function: Sqls.Functions.Avg);
                case "NumB":
                    return Rds.ResultsColumn()
                        .NumB(tableName: "s", function: Sqls.Functions.Avg);
                case "NumC":
                    return Rds.ResultsColumn()
                        .NumC(tableName: "s", function: Sqls.Functions.Avg);
                case "NumD":
                    return Rds.ResultsColumn()
                        .NumD(tableName: "s", function: Sqls.Functions.Avg);
                case "NumE":
                    return Rds.ResultsColumn()
                        .NumE(tableName: "s", function: Sqls.Functions.Avg);
                case "NumF":
                    return Rds.ResultsColumn()
                        .NumF(tableName: "s", function: Sqls.Functions.Avg);
                case "NumG":
                    return Rds.ResultsColumn()
                        .NumG(tableName: "s", function: Sqls.Functions.Avg);
                case "NumH":
                    return Rds.ResultsColumn()
                        .NumH(tableName: "s", function: Sqls.Functions.Avg);
                case "NumI":
                    return Rds.ResultsColumn()
                        .NumI(tableName: "s", function: Sqls.Functions.Avg);
                case "NumJ":
                    return Rds.ResultsColumn()
                        .NumJ(tableName: "s", function: Sqls.Functions.Avg);
                case "NumK":
                    return Rds.ResultsColumn()
                        .NumK(tableName: "s", function: Sqls.Functions.Avg);
                case "NumL":
                    return Rds.ResultsColumn()
                        .NumL(tableName: "s", function: Sqls.Functions.Avg);
                case "NumM":
                    return Rds.ResultsColumn()
                        .NumM(tableName: "s", function: Sqls.Functions.Avg);
                case "NumN":
                    return Rds.ResultsColumn()
                        .NumN(tableName: "s", function: Sqls.Functions.Avg);
                case "NumO":
                    return Rds.ResultsColumn()
                        .NumO(tableName: "s", function: Sqls.Functions.Avg);
                case "NumP":
                    return Rds.ResultsColumn()
                        .NumP(tableName: "s", function: Sqls.Functions.Avg);
                case "NumQ":
                    return Rds.ResultsColumn()
                        .NumQ(tableName: "s", function: Sqls.Functions.Avg);
                case "NumR":
                    return Rds.ResultsColumn()
                        .NumR(tableName: "s", function: Sqls.Functions.Avg);
                case "NumS":
                    return Rds.ResultsColumn()
                        .NumS(tableName: "s", function: Sqls.Functions.Avg);
                case "NumT":
                    return Rds.ResultsColumn()
                        .NumT(tableName: "s", function: Sqls.Functions.Avg);
                case "NumU":
                    return Rds.ResultsColumn()
                        .NumU(tableName: "s", function: Sqls.Functions.Avg);
                case "NumV":
                    return Rds.ResultsColumn()
                        .NumV(tableName: "s", function: Sqls.Functions.Avg);
                case "NumW":
                    return Rds.ResultsColumn()
                        .NumW(tableName: "s", function: Sqls.Functions.Avg);
                case "NumX":
                    return Rds.ResultsColumn()
                        .NumX(tableName: "s", function: Sqls.Functions.Avg);
                case "NumY":
                    return Rds.ResultsColumn()
                        .NumY(tableName: "s", function: Sqls.Functions.Avg);
                case "NumZ":
                    return Rds.ResultsColumn()
                        .NumZ(tableName: "s", function: Sqls.Functions.Avg);
                default: return null;
            }
        }

        private static SqlSelect SelectMax(
            SiteSettings ss,
            string destinationPk,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    _as: "s",
                    column: IssuesMaxColumn(sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinationPk, sourceSiteId, linkColumn)));
                case "Results": return Rds.SelectResults(
                    _as: "s",
                    column: ResultsMaxColumn(sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinationPk, sourceSiteId, linkColumn)));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesMaxColumn(string sourceColumn)
        {
            switch (sourceColumn)
            {
                case "WorkValue":
                    return Rds.IssuesColumn()
                        .WorkValue(tableName: "s", function: Sqls.Functions.Max);
                case "RemainingWorkValue":
                    return Rds.IssuesColumn()
                        .RemainingWorkValue(tableName: "s", function: Sqls.Functions.Max);
                case "NumA":
                    return Rds.IssuesColumn()
                        .NumA(tableName: "s", function: Sqls.Functions.Max);
                case "NumB":
                    return Rds.IssuesColumn()
                        .NumB(tableName: "s", function: Sqls.Functions.Max);
                case "NumC":
                    return Rds.IssuesColumn()
                        .NumC(tableName: "s", function: Sqls.Functions.Max);
                case "NumD":
                    return Rds.IssuesColumn()
                        .NumD(tableName: "s", function: Sqls.Functions.Max);
                case "NumE":
                    return Rds.IssuesColumn()
                        .NumE(tableName: "s", function: Sqls.Functions.Max);
                case "NumF":
                    return Rds.IssuesColumn()
                        .NumF(tableName: "s", function: Sqls.Functions.Max);
                case "NumG":
                    return Rds.IssuesColumn()
                        .NumG(tableName: "s", function: Sqls.Functions.Max);
                case "NumH":
                    return Rds.IssuesColumn()
                        .NumH(tableName: "s", function: Sqls.Functions.Max);
                case "NumI":
                    return Rds.IssuesColumn()
                        .NumI(tableName: "s", function: Sqls.Functions.Max);
                case "NumJ":
                    return Rds.IssuesColumn()
                        .NumJ(tableName: "s", function: Sqls.Functions.Max);
                case "NumK":
                    return Rds.IssuesColumn()
                        .NumK(tableName: "s", function: Sqls.Functions.Max);
                case "NumL":
                    return Rds.IssuesColumn()
                        .NumL(tableName: "s", function: Sqls.Functions.Max);
                case "NumM":
                    return Rds.IssuesColumn()
                        .NumM(tableName: "s", function: Sqls.Functions.Max);
                case "NumN":
                    return Rds.IssuesColumn()
                        .NumN(tableName: "s", function: Sqls.Functions.Max);
                case "NumO":
                    return Rds.IssuesColumn()
                        .NumO(tableName: "s", function: Sqls.Functions.Max);
                case "NumP":
                    return Rds.IssuesColumn()
                        .NumP(tableName: "s", function: Sqls.Functions.Max);
                case "NumQ":
                    return Rds.IssuesColumn()
                        .NumQ(tableName: "s", function: Sqls.Functions.Max);
                case "NumR":
                    return Rds.IssuesColumn()
                        .NumR(tableName: "s", function: Sqls.Functions.Max);
                case "NumS":
                    return Rds.IssuesColumn()
                        .NumS(tableName: "s", function: Sqls.Functions.Max);
                case "NumT":
                    return Rds.IssuesColumn()
                        .NumT(tableName: "s", function: Sqls.Functions.Max);
                case "NumU":
                    return Rds.IssuesColumn()
                        .NumU(tableName: "s", function: Sqls.Functions.Max);
                case "NumV":
                    return Rds.IssuesColumn()
                        .NumV(tableName: "s", function: Sqls.Functions.Max);
                case "NumW":
                    return Rds.IssuesColumn()
                        .NumW(tableName: "s", function: Sqls.Functions.Max);
                case "NumX":
                    return Rds.IssuesColumn()
                        .NumX(tableName: "s", function: Sqls.Functions.Max);
                case "NumY":
                    return Rds.IssuesColumn()
                        .NumY(tableName: "s", function: Sqls.Functions.Max);
                case "NumZ":
                    return Rds.IssuesColumn()
                        .NumZ(tableName: "s", function: Sqls.Functions.Max);
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsMaxColumn(string sourceColumn)
        {
            switch (sourceColumn)
            {
                case "NumA":
                    return Rds.ResultsColumn()
                        .NumA(tableName: "s", function: Sqls.Functions.Max);
                case "NumB":
                    return Rds.ResultsColumn()
                        .NumB(tableName: "s", function: Sqls.Functions.Max);
                case "NumC":
                    return Rds.ResultsColumn()
                        .NumC(tableName: "s", function: Sqls.Functions.Max);
                case "NumD":
                    return Rds.ResultsColumn()
                        .NumD(tableName: "s", function: Sqls.Functions.Max);
                case "NumE":
                    return Rds.ResultsColumn()
                        .NumE(tableName: "s", function: Sqls.Functions.Max);
                case "NumF":
                    return Rds.ResultsColumn()
                        .NumF(tableName: "s", function: Sqls.Functions.Max);
                case "NumG":
                    return Rds.ResultsColumn()
                        .NumG(tableName: "s", function: Sqls.Functions.Max);
                case "NumH":
                    return Rds.ResultsColumn()
                        .NumH(tableName: "s", function: Sqls.Functions.Max);
                case "NumI":
                    return Rds.ResultsColumn()
                        .NumI(tableName: "s", function: Sqls.Functions.Max);
                case "NumJ":
                    return Rds.ResultsColumn()
                        .NumJ(tableName: "s", function: Sqls.Functions.Max);
                case "NumK":
                    return Rds.ResultsColumn()
                        .NumK(tableName: "s", function: Sqls.Functions.Max);
                case "NumL":
                    return Rds.ResultsColumn()
                        .NumL(tableName: "s", function: Sqls.Functions.Max);
                case "NumM":
                    return Rds.ResultsColumn()
                        .NumM(tableName: "s", function: Sqls.Functions.Max);
                case "NumN":
                    return Rds.ResultsColumn()
                        .NumN(tableName: "s", function: Sqls.Functions.Max);
                case "NumO":
                    return Rds.ResultsColumn()
                        .NumO(tableName: "s", function: Sqls.Functions.Max);
                case "NumP":
                    return Rds.ResultsColumn()
                        .NumP(tableName: "s", function: Sqls.Functions.Max);
                case "NumQ":
                    return Rds.ResultsColumn()
                        .NumQ(tableName: "s", function: Sqls.Functions.Max);
                case "NumR":
                    return Rds.ResultsColumn()
                        .NumR(tableName: "s", function: Sqls.Functions.Max);
                case "NumS":
                    return Rds.ResultsColumn()
                        .NumS(tableName: "s", function: Sqls.Functions.Max);
                case "NumT":
                    return Rds.ResultsColumn()
                        .NumT(tableName: "s", function: Sqls.Functions.Max);
                case "NumU":
                    return Rds.ResultsColumn()
                        .NumU(tableName: "s", function: Sqls.Functions.Max);
                case "NumV":
                    return Rds.ResultsColumn()
                        .NumV(tableName: "s", function: Sqls.Functions.Max);
                case "NumW":
                    return Rds.ResultsColumn()
                        .NumW(tableName: "s", function: Sqls.Functions.Max);
                case "NumX":
                    return Rds.ResultsColumn()
                        .NumX(tableName: "s", function: Sqls.Functions.Max);
                case "NumY":
                    return Rds.ResultsColumn()
                        .NumY(tableName: "s", function: Sqls.Functions.Max);
                case "NumZ":
                    return Rds.ResultsColumn()
                        .NumZ(tableName: "s", function: Sqls.Functions.Max);
                default: return null;
            }
        }

        private static SqlWhereCollection IssuesWhere(
            string destinationPk, long sourceSiteId, string linkColumn)
        {
            switch (linkColumn)
            {
                case "ClassA": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassA(tableName: "s", raw: destinationPk);
                case "ClassB": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassB(tableName: "s", raw: destinationPk);
                case "ClassC": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassC(tableName: "s", raw: destinationPk);
                case "ClassD": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassD(tableName: "s", raw: destinationPk);
                case "ClassE": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassE(tableName: "s", raw: destinationPk);
                case "ClassF": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassF(tableName: "s", raw: destinationPk);
                case "ClassG": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassG(tableName: "s", raw: destinationPk);
                case "ClassH": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassH(tableName: "s", raw: destinationPk);
                case "ClassI": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassI(tableName: "s", raw: destinationPk);
                case "ClassJ": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassJ(tableName: "s", raw: destinationPk);
                case "ClassK": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassK(tableName: "s", raw: destinationPk);
                case "ClassL": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassL(tableName: "s", raw: destinationPk);
                case "ClassM": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassM(tableName: "s", raw: destinationPk);
                case "ClassN": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassN(tableName: "s", raw: destinationPk);
                case "ClassO": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassO(tableName: "s", raw: destinationPk);
                case "ClassP": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassP(tableName: "s", raw: destinationPk);
                case "ClassQ": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassQ(tableName: "s", raw: destinationPk);
                case "ClassR": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassR(tableName: "s", raw: destinationPk);
                case "ClassS": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassS(tableName: "s", raw: destinationPk);
                case "ClassT": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassT(tableName: "s", raw: destinationPk);
                case "ClassU": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassU(tableName: "s", raw: destinationPk);
                case "ClassV": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassV(tableName: "s", raw: destinationPk);
                case "ClassW": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassW(tableName: "s", raw: destinationPk);
                case "ClassX": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassX(tableName: "s", raw: destinationPk);
                case "ClassY": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassY(tableName: "s", raw: destinationPk);
                case "ClassZ": return Rds.IssuesWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassZ(tableName: "s", raw: destinationPk);
                default: return null;
            }
        }

        private static SqlWhereCollection ResultsWhere(
            string destinationPk, long sourceSiteId, string linkColumn)
        {
            switch (linkColumn)
            {
                case "ClassA": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassA(tableName: "s", raw: destinationPk);
                case "ClassB": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassB(tableName: "s", raw: destinationPk);
                case "ClassC": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassC(tableName: "s", raw: destinationPk);
                case "ClassD": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassD(tableName: "s", raw: destinationPk);
                case "ClassE": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassE(tableName: "s", raw: destinationPk);
                case "ClassF": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassF(tableName: "s", raw: destinationPk);
                case "ClassG": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassG(tableName: "s", raw: destinationPk);
                case "ClassH": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassH(tableName: "s", raw: destinationPk);
                case "ClassI": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassI(tableName: "s", raw: destinationPk);
                case "ClassJ": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassJ(tableName: "s", raw: destinationPk);
                case "ClassK": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassK(tableName: "s", raw: destinationPk);
                case "ClassL": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassL(tableName: "s", raw: destinationPk);
                case "ClassM": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassM(tableName: "s", raw: destinationPk);
                case "ClassN": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassN(tableName: "s", raw: destinationPk);
                case "ClassO": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassO(tableName: "s", raw: destinationPk);
                case "ClassP": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassP(tableName: "s", raw: destinationPk);
                case "ClassQ": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassQ(tableName: "s", raw: destinationPk);
                case "ClassR": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassR(tableName: "s", raw: destinationPk);
                case "ClassS": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassS(tableName: "s", raw: destinationPk);
                case "ClassT": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassT(tableName: "s", raw: destinationPk);
                case "ClassU": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassU(tableName: "s", raw: destinationPk);
                case "ClassV": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassV(tableName: "s", raw: destinationPk);
                case "ClassW": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassW(tableName: "s", raw: destinationPk);
                case "ClassX": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassX(tableName: "s", raw: destinationPk);
                case "ClassY": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassY(tableName: "s", raw: destinationPk);
                case "ClassZ": return Rds.ResultsWhere()
                    .SiteId(tableName: "s", value: sourceSiteId)
                    .ClassZ(tableName: "s", raw: destinationPk);
                default: return null;
            }
        }

        private static SqlWhereCollection Where(
            SiteSettings ss, View view, SqlWhereCollection where)
        {
            return view != null
                ? view.Where(ss, where)
                : where;
        }
    }
}
