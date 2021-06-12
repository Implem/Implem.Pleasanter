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

        public ServerScriptModelApiModel New()
        {
            var itemModel = new IssueModel();
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                id: 0,
                apiRequestBody: string.Empty);
            var ss = new SiteSettings(
                context: apiContext,
                referenceType: "Issues");
            itemModel.SetDefault(
                context: apiContext,
                ss: ss);
            var apiModel = new ServerScriptModelApiModel(
                context: Context,
                model: itemModel,
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