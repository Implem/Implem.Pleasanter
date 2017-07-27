using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Summaries
    {
        public static void Synchronize(SiteSettings ss, int id)
        {
            var destinationSs = SiteSettingsUtilities.Get(
                ss.Summaries.FirstOrDefault(o => o.Id == id)?.SiteId ?? 0);
            var summary = ss.Summaries?.Get(id);
            if (destinationSs != null && summary != null)
            {
                Synchronize(
                    ss,
                    destinationSs,
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
            }
        }

        public static string Synchronize(
            SiteSettings ss,
            SiteSettings destinationSs,
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
                        destinationSs,
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
                        destinationSs,
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
            SiteSettings destinationSs,
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
            if (destinationSs.CanUpdate())
            {
                var where = Rds.IssuesWhere()
                    .SiteId(destinationSiteId)
                    .IssueId(issueId, _using: issueId != 0);
                var issueCollection = new IssueCollection(
                    ss: destinationSs, where: Where(destinationSs, null, where));
                var matchingConditions = destinationCondition != null
                    ? Rds.ExecuteTable(statements: Rds.SelectIssues(
                        column: Rds.IssuesColumn().IssueId(),
                        where: Where(destinationSs, destinationCondition, where)))
                            .AsEnumerable()
                            .Select(o => o["IssueId"].ToLong())
                            .ToList()
                    : issueCollection
                        .Select(o => o.IssueId)
                        .ToList();
                var data = Data(
                    ss: ss,
                    destinationColumn: destinationColumn,
                    destinations: issueCollection.Select(o => o.IssueId),
                    sourceSiteId: sourceSiteId,
                    sourceReferenceType: sourceReferenceType,
                    linkColumn: linkColumn,
                    type: type,
                    sourceColumn: sourceColumn,
                    sourceCondition: sourceCondition);
                issueCollection.ForEach(issueModel =>
                {
                    if (matchingConditions.Any(o => o == issueModel.IssueId))
                    {
                        Set(
                            issueModel,
                            destinationColumn,
                            data.Get(issueModel.IssueId));
                    }
                    else if (setZeroWhenOutOfCondition)
                    {
                        Set(issueModel, destinationColumn, 0);
                    }
                    if (issueModel.Updated())
                    {
                        issueModel.SetByFormula(destinationSs);
                        issueModel.VerUp = Versions.MustVerUp(issueModel);
                        issueModel.Update(
                            ss: destinationSs,
                            synchronizeSummary: false,
                            get: false);
                    }
                });
            }
        }

        private static EnumerableRowCollection<DataRow> IssuesDataRows(
            SiteSettings ss,
            string destinationColumn,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (destinationColumn)
            {
                case "WorkValue": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumA": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumB": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumC": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumD": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumE": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumF": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumG": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumH": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumI": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumJ": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumK": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumL": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumM": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumN": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumO": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumP": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumQ": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumR": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumS": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumT": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumU": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumV": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumW": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumX": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumY": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumZ": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
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

        private static void Set(
            IssueModel issueModel, string destinationColumn, decimal value)
        {
            switch (destinationColumn)
            {
                case "WorkValue": issueModel.WorkValue.Value = value; break;
                case "NumA": issueModel.NumA = value; break;
                case "NumB": issueModel.NumB = value; break;
                case "NumC": issueModel.NumC = value; break;
                case "NumD": issueModel.NumD = value; break;
                case "NumE": issueModel.NumE = value; break;
                case "NumF": issueModel.NumF = value; break;
                case "NumG": issueModel.NumG = value; break;
                case "NumH": issueModel.NumH = value; break;
                case "NumI": issueModel.NumI = value; break;
                case "NumJ": issueModel.NumJ = value; break;
                case "NumK": issueModel.NumK = value; break;
                case "NumL": issueModel.NumL = value; break;
                case "NumM": issueModel.NumM = value; break;
                case "NumN": issueModel.NumN = value; break;
                case "NumO": issueModel.NumO = value; break;
                case "NumP": issueModel.NumP = value; break;
                case "NumQ": issueModel.NumQ = value; break;
                case "NumR": issueModel.NumR = value; break;
                case "NumS": issueModel.NumS = value; break;
                case "NumT": issueModel.NumT = value; break;
                case "NumU": issueModel.NumU = value; break;
                case "NumV": issueModel.NumV = value; break;
                case "NumW": issueModel.NumW = value; break;
                case "NumX": issueModel.NumX = value; break;
                case "NumY": issueModel.NumY = value; break;
                case "NumZ": issueModel.NumZ = value; break;
            }
        }

        private static void SynchronizeResults(
            SiteSettings ss,
            SiteSettings destinationSs,
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
            if (destinationSs.CanUpdate())
            {
                var where = Rds.ResultsWhere()
                    .SiteId(destinationSiteId)
                    .ResultId(resultId, _using: resultId != 0);
                var resultCollection = new ResultCollection(
                    ss: destinationSs, where: Where(destinationSs, null, where));
                var matchingConditions = destinationCondition != null
                    ? Rds.ExecuteTable(statements: Rds.SelectResults(
                        column: Rds.ResultsColumn().ResultId(),
                        where: Where(destinationSs, destinationCondition, where)))
                            .AsEnumerable()
                            .Select(o => o["ResultId"].ToLong())
                            .ToList()
                    : resultCollection
                        .Select(o => o.ResultId)
                        .ToList();
                var data = Data(
                    ss: ss,
                    destinationColumn: destinationColumn,
                    destinations: resultCollection.Select(o => o.ResultId),
                    sourceSiteId: sourceSiteId,
                    sourceReferenceType: sourceReferenceType,
                    linkColumn: linkColumn,
                    type: type,
                    sourceColumn: sourceColumn,
                    sourceCondition: sourceCondition);
                resultCollection.ForEach(resultModel =>
                {
                    if (matchingConditions.Any(o => o == resultModel.ResultId))
                    {
                        Set(
                            resultModel,
                            destinationColumn,
                            data.Get(resultModel.ResultId));
                    }
                    else if (setZeroWhenOutOfCondition)
                    {
                        Set(resultModel, destinationColumn, 0);
                    }
                    if (resultModel.Updated())
                    {
                        resultModel.SetByFormula(destinationSs);
                        resultModel.VerUp = Versions.MustVerUp(resultModel);
                        resultModel.Update(
                            ss: destinationSs,
                            synchronizeSummary: false,
                            get: false);
                    }
                });
            }
        }

        private static EnumerableRowCollection<DataRow> ResultsDataRows(
            SiteSettings ss,
            string destinationColumn,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (destinationColumn)
            {
                case "NumA": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumB": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumC": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumD": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumE": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumF": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumG": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumH": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumI": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumJ": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumK": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumL": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumM": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumN": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumO": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumP": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumQ": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumR": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumS": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumT": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumU": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumV": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumW": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumX": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumY": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
                case "NumZ": return Rds.ExecuteTable(statements: Select(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    type,
                    sourceColumn,
                    sourceCondition)).AsEnumerable();
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

        private static void Set(
            ResultModel resultModel, string destinationColumn, decimal value)
        {
            switch (destinationColumn)
            {
                case "NumA": resultModel.NumA = value; break;
                case "NumB": resultModel.NumB = value; break;
                case "NumC": resultModel.NumC = value; break;
                case "NumD": resultModel.NumD = value; break;
                case "NumE": resultModel.NumE = value; break;
                case "NumF": resultModel.NumF = value; break;
                case "NumG": resultModel.NumG = value; break;
                case "NumH": resultModel.NumH = value; break;
                case "NumI": resultModel.NumI = value; break;
                case "NumJ": resultModel.NumJ = value; break;
                case "NumK": resultModel.NumK = value; break;
                case "NumL": resultModel.NumL = value; break;
                case "NumM": resultModel.NumM = value; break;
                case "NumN": resultModel.NumN = value; break;
                case "NumO": resultModel.NumO = value; break;
                case "NumP": resultModel.NumP = value; break;
                case "NumQ": resultModel.NumQ = value; break;
                case "NumR": resultModel.NumR = value; break;
                case "NumS": resultModel.NumS = value; break;
                case "NumT": resultModel.NumT = value; break;
                case "NumU": resultModel.NumU = value; break;
                case "NumV": resultModel.NumV = value; break;
                case "NumW": resultModel.NumW = value; break;
                case "NumX": resultModel.NumX = value; break;
                case "NumY": resultModel.NumY = value; break;
                case "NumZ": resultModel.NumZ = value; break;
            }
        }

        private static Dictionary<long, decimal> Data(
            SiteSettings ss,
            string destinationColumn,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues":
                    return IssuesDataRows(
                        ss: ss,
                        destinationColumn: destinationColumn,
                        destinations: destinations,
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition)
                            .ToDictionary(
                                o => o["Id"].ToLong(),
                                o => o["Value"].ToDecimal());
                case "Results":
                    return ResultsDataRows(
                        ss: ss,
                        destinationColumn: destinationColumn,
                        destinations: destinations,
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition)
                            .ToDictionary(
                                o => o["Id"].ToLong(),
                                o => o["Value"].ToDecimal());
                default: return null;
            }
        }

        private static SqlSelect Select(
            SiteSettings ss,
            IEnumerable<long> destinations,
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
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceCondition);
                case "Total": return SelectTotal(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceColumn,
                    sourceCondition);
                case "Average": return SelectAverage(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceColumn,
                    sourceCondition);
                case "Min": return SelectMin(
                    ss,
                    destinations,
                    sourceSiteId,
                    sourceReferenceType,
                    linkColumn,
                    sourceColumn,
                    sourceCondition);
                case "Max": return SelectMax(
                    ss,
                    destinations,
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
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: Rds.IssuesColumn()
                        .IssuesColumn(linkColumn, _as: "Id")
                        .IssuesCount(_as: "Value"),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: Rds.ResultsColumn()
                        .ResultsColumn(linkColumn, _as: "Id")
                        .ResultsCount(_as: "Value"),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlSelect SelectTotal(
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesTotalColumn(linkColumn, sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: ResultsTotalColumn(linkColumn, sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesTotalColumn(
            string linkColumn, string sourceColumn)
        {
            var issuesColumn = Rds.IssuesColumn().IssuesColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "WorkValue":
                    return issuesColumn.WorkValue(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "RemainingWorkValue":
                    return issuesColumn.RemainingWorkValue(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumA":
                    return issuesColumn.NumA(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumB":
                    return issuesColumn.NumB(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumC":
                    return issuesColumn.NumC(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumD":
                    return issuesColumn.NumD(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumE":
                    return issuesColumn.NumE(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumF":
                    return issuesColumn.NumF(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumG":
                    return issuesColumn.NumG(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumH":
                    return issuesColumn.NumH(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumI":
                    return issuesColumn.NumI(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumJ":
                    return issuesColumn.NumJ(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumK":
                    return issuesColumn.NumK(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumL":
                    return issuesColumn.NumL(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumM":
                    return issuesColumn.NumM(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumN":
                    return issuesColumn.NumN(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumO":
                    return issuesColumn.NumO(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumP":
                    return issuesColumn.NumP(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumQ":
                    return issuesColumn.NumQ(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumR":
                    return issuesColumn.NumR(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumS":
                    return issuesColumn.NumS(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumT":
                    return issuesColumn.NumT(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumU":
                    return issuesColumn.NumU(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumV":
                    return issuesColumn.NumV(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumW":
                    return issuesColumn.NumW(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumX":
                    return issuesColumn.NumX(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumY":
                    return issuesColumn.NumY(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumZ":
                    return issuesColumn.NumZ(
                        _as: "Value", function: Sqls.Functions.Sum);
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsTotalColumn(
            string linkColumn, string sourceColumn)
        {
            var resultsColumn = Rds.ResultsColumn().ResultsColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "NumA":
                    return resultsColumn.NumA(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumB":
                    return resultsColumn.NumB(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumC":
                    return resultsColumn.NumC(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumD":
                    return resultsColumn.NumD(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumE":
                    return resultsColumn.NumE(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumF":
                    return resultsColumn.NumF(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumG":
                    return resultsColumn.NumG(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumH":
                    return resultsColumn.NumH(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumI":
                    return resultsColumn.NumI(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumJ":
                    return resultsColumn.NumJ(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumK":
                    return resultsColumn.NumK(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumL":
                    return resultsColumn.NumL(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumM":
                    return resultsColumn.NumM(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumN":
                    return resultsColumn.NumN(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumO":
                    return resultsColumn.NumO(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumP":
                    return resultsColumn.NumP(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumQ":
                    return resultsColumn.NumQ(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumR":
                    return resultsColumn.NumR(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumS":
                    return resultsColumn.NumS(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumT":
                    return resultsColumn.NumT(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumU":
                    return resultsColumn.NumU(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumV":
                    return resultsColumn.NumV(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumW":
                    return resultsColumn.NumW(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumX":
                    return resultsColumn.NumX(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumY":
                    return resultsColumn.NumY(
                        _as: "Value", function: Sqls.Functions.Sum);
                case "NumZ":
                    return resultsColumn.NumZ(
                        _as: "Value", function: Sqls.Functions.Sum);
                default: return null;
            }
        }

        private static SqlSelect SelectAverage(
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesAverageColumn(linkColumn, sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: ResultsAverageColumn(linkColumn, sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlSelect SelectMin(
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesMinColumn(linkColumn, sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: ResultsMinColumn(linkColumn, sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesMinColumn(
            string linkColumn, string sourceColumn)
        {
            var issuesColumn = Rds.IssuesColumn().IssuesColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "WorkValue":
                    return issuesColumn.WorkValue(
                        _as: "Value", function: Sqls.Functions.Min);
                case "RemainingWorkValue":
                    return issuesColumn.RemainingWorkValue(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumA":
                    return issuesColumn.NumA(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumB":
                    return issuesColumn.NumB(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumC":
                    return issuesColumn.NumC(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumD":
                    return issuesColumn.NumD(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumE":
                    return issuesColumn.NumE(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumF":
                    return issuesColumn.NumF(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumG":
                    return issuesColumn.NumG(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumH":
                    return issuesColumn.NumH(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumI":
                    return issuesColumn.NumI(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumJ":
                    return issuesColumn.NumJ(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumK":
                    return issuesColumn.NumK(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumL":
                    return issuesColumn.NumL(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumM":
                    return issuesColumn.NumM(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumN":
                    return issuesColumn.NumN(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumO":
                    return issuesColumn.NumO(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumP":
                    return issuesColumn.NumP(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumQ":
                    return issuesColumn.NumQ(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumR":
                    return issuesColumn.NumR(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumS":
                    return issuesColumn.NumS(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumT":
                    return issuesColumn.NumT(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumU":
                    return issuesColumn.NumU(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumV":
                    return issuesColumn.NumV(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumW":
                    return issuesColumn.NumW(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumX":
                    return issuesColumn.NumX(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumY":
                    return issuesColumn.NumY(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumZ":
                    return issuesColumn.NumZ(
                        _as: "Value", function: Sqls.Functions.Min);
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsMinColumn(
            string linkColumn, string sourceColumn)
        {
            var resultsColumn = Rds.ResultsColumn().ResultsColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "NumA":
                    return resultsColumn.NumA(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumB":
                    return resultsColumn.NumB(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumC":
                    return resultsColumn.NumC(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumD":
                    return resultsColumn.NumD(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumE":
                    return resultsColumn.NumE(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumF":
                    return resultsColumn.NumF(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumG":
                    return resultsColumn.NumG(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumH":
                    return resultsColumn.NumH(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumI":
                    return resultsColumn.NumI(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumJ":
                    return resultsColumn.NumJ(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumK":
                    return resultsColumn.NumK(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumL":
                    return resultsColumn.NumL(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumM":
                    return resultsColumn.NumM(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumN":
                    return resultsColumn.NumN(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumO":
                    return resultsColumn.NumO(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumP":
                    return resultsColumn.NumP(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumQ":
                    return resultsColumn.NumQ(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumR":
                    return resultsColumn.NumR(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumS":
                    return resultsColumn.NumS(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumT":
                    return resultsColumn.NumT(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumU":
                    return resultsColumn.NumU(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumV":
                    return resultsColumn.NumV(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumW":
                    return resultsColumn.NumW(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumX":
                    return resultsColumn.NumX(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumY":
                    return resultsColumn.NumY(
                        _as: "Value", function: Sqls.Functions.Min);
                case "NumZ":
                    return resultsColumn.NumZ(
                        _as: "Value", function: Sqls.Functions.Min);
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesAverageColumn(
            string linkColumn, string sourceColumn)
        {
            var issuesColumn = Rds.IssuesColumn().IssuesColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "WorkValue":
                    return issuesColumn.WorkValue(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "RemainingWorkValue":
                    return issuesColumn.RemainingWorkValue(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumA":
                    return issuesColumn.NumA(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumB":
                    return issuesColumn.NumB(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumC":
                    return issuesColumn.NumC(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumD":
                    return issuesColumn.NumD(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumE":
                    return issuesColumn.NumE(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumF":
                    return issuesColumn.NumF(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumG":
                    return issuesColumn.NumG(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumH":
                    return issuesColumn.NumH(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumI":
                    return issuesColumn.NumI(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumJ":
                    return issuesColumn.NumJ(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumK":
                    return issuesColumn.NumK(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumL":
                    return issuesColumn.NumL(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumM":
                    return issuesColumn.NumM(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumN":
                    return issuesColumn.NumN(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumO":
                    return issuesColumn.NumO(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumP":
                    return issuesColumn.NumP(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumQ":
                    return issuesColumn.NumQ(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumR":
                    return issuesColumn.NumR(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumS":
                    return issuesColumn.NumS(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumT":
                    return issuesColumn.NumT(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumU":
                    return issuesColumn.NumU(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumV":
                    return issuesColumn.NumV(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumW":
                    return issuesColumn.NumW(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumX":
                    return issuesColumn.NumX(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumY":
                    return issuesColumn.NumY(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumZ":
                    return issuesColumn.NumZ(
                        _as: "Value", function: Sqls.Functions.Avg);
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsAverageColumn(
            string linkColumn, string sourceColumn)
        {
            var resultsColumn = Rds.ResultsColumn().ResultsColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "NumA":
                    return resultsColumn.NumA(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumB":
                    return resultsColumn.NumB(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumC":
                    return resultsColumn.NumC(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumD":
                    return resultsColumn.NumD(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumE":
                    return resultsColumn.NumE(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumF":
                    return resultsColumn.NumF(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumG":
                    return resultsColumn.NumG(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumH":
                    return resultsColumn.NumH(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumI":
                    return resultsColumn.NumI(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumJ":
                    return resultsColumn.NumJ(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumK":
                    return resultsColumn.NumK(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumL":
                    return resultsColumn.NumL(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumM":
                    return resultsColumn.NumM(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumN":
                    return resultsColumn.NumN(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumO":
                    return resultsColumn.NumO(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumP":
                    return resultsColumn.NumP(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumQ":
                    return resultsColumn.NumQ(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumR":
                    return resultsColumn.NumR(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumS":
                    return resultsColumn.NumS(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumT":
                    return resultsColumn.NumT(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumU":
                    return resultsColumn.NumU(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumV":
                    return resultsColumn.NumV(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumW":
                    return resultsColumn.NumW(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumX":
                    return resultsColumn.NumX(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumY":
                    return resultsColumn.NumY(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "NumZ":
                    return resultsColumn.NumZ(
                        _as: "Value", function: Sqls.Functions.Avg);
                default: return null;
            }
        }

        private static SqlSelect SelectMax(
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesMaxColumn(linkColumn, sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: ResultsMaxColumn(linkColumn, sourceColumn),
                    where: Where(
                        ss,
                        sourceCondition,
                        ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesMaxColumn(
            string linkColumn, string sourceColumn)
        {
            var issuesColumn = Rds.IssuesColumn().IssuesColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "WorkValue":
                    return issuesColumn.WorkValue(
                        _as: "Value", function: Sqls.Functions.Max);
                case "RemainingWorkValue":
                    return issuesColumn.RemainingWorkValue(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumA":
                    return issuesColumn.NumA(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumB":
                    return issuesColumn.NumB(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumC":
                    return issuesColumn.NumC(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumD":
                    return issuesColumn.NumD(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumE":
                    return issuesColumn.NumE(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumF":
                    return issuesColumn.NumF(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumG":
                    return issuesColumn.NumG(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumH":
                    return issuesColumn.NumH(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumI":
                    return issuesColumn.NumI(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumJ":
                    return issuesColumn.NumJ(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumK":
                    return issuesColumn.NumK(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumL":
                    return issuesColumn.NumL(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumM":
                    return issuesColumn.NumM(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumN":
                    return issuesColumn.NumN(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumO":
                    return issuesColumn.NumO(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumP":
                    return issuesColumn.NumP(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumQ":
                    return issuesColumn.NumQ(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumR":
                    return issuesColumn.NumR(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumS":
                    return issuesColumn.NumS(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumT":
                    return issuesColumn.NumT(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumU":
                    return issuesColumn.NumU(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumV":
                    return issuesColumn.NumV(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumW":
                    return issuesColumn.NumW(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumX":
                    return issuesColumn.NumX(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumY":
                    return issuesColumn.NumY(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumZ":
                    return issuesColumn.NumZ(
                        _as: "Value", function: Sqls.Functions.Max);
                default: return null;
            }
        }

        private static SqlColumnCollection ResultsMaxColumn(
            string linkColumn, string sourceColumn)
        {
            var resultsColumn = Rds.ResultsColumn().ResultsColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "NumA":
                    return resultsColumn.NumA(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumB":
                    return resultsColumn.NumB(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumC":
                    return resultsColumn.NumC(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumD":
                    return resultsColumn.NumD(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumE":
                    return resultsColumn.NumE(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumF":
                    return resultsColumn.NumF(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumG":
                    return resultsColumn.NumG(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumH":
                    return resultsColumn.NumH(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumI":
                    return resultsColumn.NumI(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumJ":
                    return resultsColumn.NumJ(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumK":
                    return resultsColumn.NumK(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumL":
                    return resultsColumn.NumL(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumM":
                    return resultsColumn.NumM(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumN":
                    return resultsColumn.NumN(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumO":
                    return resultsColumn.NumO(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumP":
                    return resultsColumn.NumP(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumQ":
                    return resultsColumn.NumQ(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumR":
                    return resultsColumn.NumR(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumS":
                    return resultsColumn.NumS(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumT":
                    return resultsColumn.NumT(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumU":
                    return resultsColumn.NumU(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumV":
                    return resultsColumn.NumV(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumW":
                    return resultsColumn.NumW(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumX":
                    return resultsColumn.NumX(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumY":
                    return resultsColumn.NumY(
                        _as: "Value", function: Sqls.Functions.Max);
                case "NumZ":
                    return resultsColumn.NumZ(
                        _as: "Value", function: Sqls.Functions.Max);
                default: return null;
            }
        }

        private static SqlWhereCollection IssuesWhere(
            IEnumerable<long> destinations, long sourceSiteId, string linkColumn)
        {
            switch (linkColumn)
            {
                case "ClassA": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassA(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassB": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassB(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassC": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassC(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassD": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassD(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassE": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassE(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassF": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassF(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassG": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassG(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassH": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassH(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassI": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassI(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassJ": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassJ(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassK": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassK(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassL": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassL(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassM": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassM(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassN": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassN(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassO": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassO(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassP": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassP(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassQ": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassQ(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassR": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassR(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassS": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassS(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassT": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassT(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassU": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassU(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassV": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassV(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassW": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassW(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassX": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassX(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassY": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassY(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassZ": return Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassZ(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                default: return null;
            }
        }

        private static SqlWhereCollection ResultsWhere(
            IEnumerable<long> destinations, long sourceSiteId, string linkColumn)
        {
            switch (linkColumn)
            {
                case "ClassA": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassA(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassB": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassB(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassC": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassC(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassD": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassD(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassE": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassE(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassF": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassF(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassG": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassG(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassH": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassH(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassI": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassI(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassJ": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassJ(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassK": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassK(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassL": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassL(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassM": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassM(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassN": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassN(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassO": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassO(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassP": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassP(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassQ": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassQ(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassR": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassR(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassS": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassS(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassT": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassT(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassU": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassU(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassV": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassV(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassW": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassW(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassX": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassX(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassY": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassY(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
                case "ClassZ": return Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .ClassZ(
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ");
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
