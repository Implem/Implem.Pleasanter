using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
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
    public static class StatusUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public enum Types : int
        {
            DeptsUpdated = 210,
            GroupsUpdated = 220,
            UsersUpdated = 230,
            PermissionsUpdated = 240,
            SitesUpdated = 250
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<Types, DateTime> Monitors()
        {
            var now = DateTime.Now;
            var hash = new Dictionary<Types, DateTime>
            {
                { Types.DeptsUpdated, now },
                { Types.GroupsUpdated, now },
                { Types.UsersUpdated, now },
                { Types.PermissionsUpdated, now },
                { Types.SitesUpdated, now }
            };
            Rds.ExecuteTable(statements: Rds.SelectStatuses(
                column: Rds.StatusesColumn()
                    .StatusId()
                    .UpdatedTime(),
                where: Rds.StatusesWhere()
                    .StatusId_In(hash.Keys.Select(o => o.ToInt()))))
                        .AsEnumerable()
                        .ForEach(dataRow =>
                        {
                            var type = (Types)dataRow["StatusId"].ToInt();
                            if (hash.ContainsKey(type))
                            {
                                hash[type] = dataRow["UpdatedTime"].ToDateTime();
                            }
                        });
            return hash;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlUpdateOrInsert UpdateStatus(Types type, string value = "")
        {
            return Rds.UpdateOrInsertStatuses(
                where: Rds.StatusesWhere().StatusId(type),
                param: Rds.StatusesParam()
                    .StatusId(type)
                    .Value(value));
        }
    }
}
