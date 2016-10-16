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
    public static class MailAddressUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string From(bool withFullName = false)
        {
            var mailAddress = Rds.ExecuteScalar_string(statements:
                Rds.SelectMailAddresses(
                    top: 1,
                    column: Rds.MailAddressesColumn().MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId(Sessions.UserId())
                        .OwnerType("Users"),
                    orderBy: Rds.MailAddressesOrderBy().MailAddressId()));
            return withFullName
                ? "\"{0}\" <{1}>".Params(Sessions.User().FullName(), mailAddress)
                : mailAddress;
        }
    }
}
