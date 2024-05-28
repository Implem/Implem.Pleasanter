using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelApiItems
    {
        private readonly Context Context;
        private readonly bool OnTesting;

        public ServerScriptModelApiItems(Context context, bool onTesting)
        {
            Context = context;
            OnTesting = onTesting;
        }

        public ServerScriptModelApiModel[] Get(object id, string view = null)
        {
            if (OnTesting)
            {
                return new ServerScriptModelApiModel[0];
            }
            return ServerScriptUtilities.Get(
                context: Context,
                id: id.ToLong(),
                view: view,
                onTesting: OnTesting);
        }

        public ServerScriptModelApiModel[] GetSite(
            object id,
            string apiRequestBody = null)
        {
            var ret = ServerScriptUtilities.GetSite(
                context: Context,
                id: id.ToLong(),
                apiRequestBody: apiRequestBody);
            return ret;
        }

        public ServerScriptModelApiModel[] GetSiteByTitle(
            object title,
            string apiRequestBody = null)
        {
            var ret = ServerScriptUtilities.GetSite(
                context: Context,
                title: title?.ToString() ?? string.Empty,
                apiRequestBody: apiRequestBody);
            return ret;
        }

        public ServerScriptModelApiModel[] GetSiteByName(
            object siteName,
            string apiRequestBody = null)
        {
            var ret = ServerScriptUtilities.GetSite(
                context: Context,
                siteName: siteName?.ToString() ?? string.Empty,
                apiRequestBody: apiRequestBody);
            return ret;
        }

        public ServerScriptModelApiModel[] GetSiteByGroupName(
            object siteGroupName,
            string apiRequestBody = null)
        {
            var ret = ServerScriptUtilities.GetSite(
                context: Context,
                siteGroupName: siteGroupName?.ToString() ?? string.Empty,
                apiRequestBody: apiRequestBody);
            return ret;
        }

        public ServerScriptModelApiModel GetSiteIdBySiteName(
            object siteName,
            object id = null)
        {
            var ret = ServerScriptUtilities.GetSiteIdBySiteName(
                context: Context,
                id: id?.ToLong(),
                siteName: siteName?.ToString() ?? string.Empty);
            return ret;
        }

        public ServerScriptModelApiModel New()
        {
            return NewIssue();
        }

        public ServerScriptModelApiModel NewSite(string referenceType)
        {
            var siteModel = new SiteModel()
            {
                ReferenceType = referenceType
            };
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Items",
                action: "New",
                id: 0,
                apiRequestBody: string.Empty);
            var ss = new SiteSettings(
                context: apiContext,
                referenceType: referenceType);
            siteModel.SiteSettings = ss;
            var apiModel = new ServerScriptModelApiModel(
                context: Context,
                model: siteModel,
                onTesting: OnTesting);
            return apiModel;
        }

        public ServerScriptModelApiModel NewIssue()
        {
            var issueModel = new IssueModel();
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Items",
                action: "New",
                id: 0,
                apiRequestBody: string.Empty);
            var ss = new SiteSettings(
                context: apiContext,
                referenceType: "Issues");
            issueModel.SetDefault(
                context: apiContext,
                ss: ss);
            var apiModel = new ServerScriptModelApiModel(
                context: Context,
                model: issueModel,
                onTesting: OnTesting);
            return apiModel;
        }

        public ServerScriptModelApiModel NewResult()
        {
            var resultModel = new ResultModel();
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Items",
                action: "New",
                id: 0,
                apiRequestBody: string.Empty);
            var ss = new SiteSettings(
                context: apiContext,
                referenceType: "Results");
            resultModel.SetDefault(
                context: apiContext,
                ss: ss);
            var apiModel = new ServerScriptModelApiModel(
                context: Context,
                model: resultModel,
                onTesting: OnTesting);
            return apiModel;
        }

        public bool Create(object id, object model)
        {
            if (OnTesting)
            {
                return false;
            }
            return ServerScriptUtilities.Create(
                context: Context,
                id: id.ToLong(),
                model: model);
        }

        public bool Update(object id, object model)
        {
            if (OnTesting)
            {
                return false;
            }
            return ServerScriptUtilities.Update(
                context: Context,
                id: id.ToLong(),
                model: model);
        }

        public bool Upsert(object id, object model)
        {
            if (OnTesting)
            {
                return false;
            }
            return ServerScriptUtilities.Upsert(
                context: Context,
                id: id.ToLong(),
                model: model);
        }

        public bool Delete(object id)
        {
            if (OnTesting)
            {
                return false;
            }
            return ServerScriptUtilities.Delete(
                context: Context,
                id: id.ToLong());
        }

        public long BulkDelete(object id, string json)
        {
            if (OnTesting)
            {
                return 0;
            }
            return ServerScriptUtilities.BulkDelete(
                context: Context,
                id: id.ToLong(),
                apiRequestBody: json);
        }

        public decimal Sum(object siteId, string columnName, string view = null)
        {
            return CreateAggregate(
                siteId: siteId,
                columnName: columnName,
                view: view,
                function: Sqls.Functions.Sum);
        }

        public decimal Average(object siteId, string columnName, string view = null)
        {
            return CreateAggregate(
                siteId: siteId,
                columnName: columnName,
                view: view,
                function: Sqls.Functions.Avg);
        }

        public decimal Max(object siteId, string columnName, string view = null)
        {
            return CreateAggregate(
                siteId: siteId,
                columnName: columnName,
                view: view,
                function: Sqls.Functions.Max);
        }

        public decimal Min(object siteId, string columnName, string view = null)
        {
            return CreateAggregate(
                siteId: siteId,
                columnName: columnName,
                view: view,
                function: Sqls.Functions.Min);
        }

        public long Count(object siteId, string view = null)
        {
            var ss = SiteSettingsUtilities.Get(
                context: Context,
                siteId: siteId.ToLong());
            return ServerScriptUtilities.Aggregate(
                context: Context,
                ss: ss,
                view: view);
        }

        private decimal CreateAggregate(object siteId, string columnName, string view, Sqls.Functions function)
        {
            var ss = SiteSettingsUtilities.Get(
                context: Context,
                siteId: siteId.ToLong());
            return ServerScriptUtilities.Aggregate(
                context: Context,
                ss: ss,
                view: view,
                columnName: columnName,
                function: function);
        }
    }
}