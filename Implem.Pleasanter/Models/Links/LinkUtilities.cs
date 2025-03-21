using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class LinkUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlInsert Insert(Dictionary<long, long> link, bool setIdentity = false)
        {
            return Rds.InsertLinks(
                param: Rds.LinksParam()
                    .DestinationId()
                    .SourceId(),
                select: Rds.Raw(link.Select(o => $"select {Parameters.Parameter.SqlParameterPrefix}U,{Parameters.Parameter.SqlParameterPrefix}U,{{0}},{{1}} "
                    .Params(o.Key.ToString(), setIdentity
                        ? Def.Sql.Identity
                        : o.Value.ToString()))
                            .Join("union ") + ";\n"),
                _using: link.Count > 0);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder LinkDialog(this HtmlBuilder hb, Context context)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("LinkDialog")
                .Class("dialog")
                .Title(Displays.Links(context: context)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Rds.LinksJoinCollection LinkJoins()
        {
            return Rds.LinksJoinDefault()
                .Add(new SqlJoin(
                    tableBracket: "\"Sites\"",
                    joinType: SqlJoin.JoinTypes.Inner,
                    joinExpression: "\"Links\".\"SourceId\"=\"SourceSites\".\"SiteId\"",
                    _as: "SourceSites"))
                .Add(new SqlJoin(
                    tableBracket: "\"Sites\"",
                    joinType: SqlJoin.JoinTypes.Inner,
                    joinExpression: "\"Links\".\"DestinationId\"=\"DestinationSites\".\"SiteId\"",
                    _as: "DestinationSites"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlWhereCollection LinksWhere(
            this Rds.LinksWhereCollection where,
            Context context,
            long[] ids)
        {
            return where
                .Sites_TenantId(
                    value: context.TenantId,
                    tableName: "SourceSites")
                .Sites_ReferenceType(
                    tableName: "SourceSites",
                    _operator: " in ",
                    raw: "('Issues','Results')")
                .Sites_TenantId(
                    value: context.TenantId,
                    tableName: "DestinationSites")
                .Sites_ReferenceType(
                    tableName: "DestinationSites",
                    _operator: " in ",
                    raw: "('Issues','Results')");
        }
    }
}
