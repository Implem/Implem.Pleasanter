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
    public static class LinkUtilities
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, LinkModel linkModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: linkModel.Ver);
                case "Comments": return hb.Td(column: column, value: linkModel.Comments);
                case "Creator": return hb.Td(column: column, value: linkModel.Creator);
                case "Updator": return hb.Td(column: column, value: linkModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: linkModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: linkModel.UpdatedTime);
                default: return hb;
            }
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, LinkModel linkModel)
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
        public static SqlInsert Insert(
            Dictionary<long, long> link, bool selectIdentity = false)
        {
            return Rds.InsertLinks(
                param: Rds.LinksParam()
                    .DestinationId()
                    .SourceId(),
                select: Rds.Raw(link.Select(o => "select @_U,@_U,{0},{1} "
                    .Params(
                        o.Key.ToString(),
                        selectIdentity
                            ? Def.Sql.Identity
                            : o.Value.ToString()))
                                .Join("union ") + ";\n"),
                _using: link.Count > 0);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <returns></returns>
        public static SqlJoinCollection JoinByDestination()
        {
            return Rds.LinksJoin()
                .Add("inner join [Items] as [t1] on [t0].[DestinationId]=[t1].[ReferenceId]")
                .Add("inner join [Sites] as [t2] on [t1].[SiteId]=[t2].[SiteId]");
        }
    }
}
