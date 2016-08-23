using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class ItemUtilities
    {
        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, ItemModel itemModel)
        {
            Forms.All().Keys.ForEach(key =>
            {
                switch (key)
                {
                    default: break;
                }
            });
            return responseCollection;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Maintain()
        {
            MaintenanceTargets().ForEach(target =>
            {
                Libraries.Search.Indexes.Create(target.Key);
                switch (target.Value)
                {
                    case "Sites": SiteInfo.SiteMenu.Set(target.Key); break;
                }
                UpdateMaintenanceTarget(target.Key);
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<long, string> MaintenanceTargets()
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectItems(
                    top: 100,
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .ReferenceType(),
                    where: Rds.ItemsWhere().MaintenanceTarget(true)))
                        .AsEnumerable()
                        .ToDictionary(
                            o => o["ReferenceId"].ToLong(),
                            o => o["ReferenceType"].ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void UpdateMaintenanceTarget(long referenceId)
        {
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateItems(
                    param: Rds.ItemsParam().MaintenanceTarget(false),
                    where: Rds.ItemsWhere().ReferenceId(referenceId),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }
    }
}
