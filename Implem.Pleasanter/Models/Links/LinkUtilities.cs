using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class LinkUtilities
    {
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
        public static HtmlBuilder LinkDialog(this HtmlBuilder hb)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("LinkDialog")
                .Class("dialog")
                .Title(Displays.Links()));
        }
    }
}
