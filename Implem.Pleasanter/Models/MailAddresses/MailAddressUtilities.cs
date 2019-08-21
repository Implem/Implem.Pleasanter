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
        public static string Get(Context context, int userId, bool withFullName = false)
        {
            var mailAddress = Repository.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectMailAddresses(
                    top: 1,
                    column: Rds.MailAddressesColumn().MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId(userId)
                        .OwnerType("Users"),
                    orderBy: Rds.MailAddressesOrderBy().MailAddressId()));
            return withFullName
                ? Get(context.User?.Name, mailAddress)
                : mailAddress;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Get(string fullName, string mailAddress)
        {
            return $"\"{fullName}\" <{mailAddress}>";
        }
    }
}
