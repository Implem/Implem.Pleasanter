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
    public static class OrderUtilities
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, OrderModel orderModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: orderModel.Ver);
                case "Comments": return hb.Td(column: column, value: orderModel.Comments);
                case "Creator": return hb.Td(column: column, value: orderModel.Creator);
                case "Updator": return hb.Td(column: column, value: orderModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: orderModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: orderModel.UpdatedTime);
                default: return hb;
            }
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, OrderModel orderModel)
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
    }
}
