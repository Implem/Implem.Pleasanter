using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Implem.Libraries.DataSources.SqlServer;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class Title : IConvertable
    {
        public long Id;
        public string Value = string.Empty;
        public string DisplayValue;
        public List<string> PartCollection;

        public Title()
        {
        }

        public Title(DataRow dataRow, string name)
        {
            Id = dataRow.Long(name);
            Value = dataRow.String("Title");
            DisplayValue = Value;
        }

        public Title(
            Context context, SiteSettings ss, DataRow dataRow, ColumnNameInfo column = null)
        {
            Id = dataRow.Long((column?.Joined == true
                ? column.TableAlias + ","
                : string.Empty) +
                    Rds.IdColumn(ss.ReferenceType));
            Value = dataRow.String(Rds.DataColumnName(column, "Title"));
            var itemTitlePath = Rds.DataColumnName(column, "ItemTitle");
            var displayValue = dataRow.Table.Columns.Contains(itemTitlePath)
                ? dataRow.String(itemTitlePath)
                : ss.GetTitleColumns(context: context)
                    .Select(o => GetDisplayValue(
                        context: context,
                        ss: ss,
                        column: o,
                        dataRow: dataRow,
                        path: Rds.DataColumnName(column, o.ColumnName)))
                    .Where(o => !o.IsNullOrEmpty())
                    .Join(ss.TitleSeparator);
            DisplayValue = GetNoTitle(
                context: context,
                displayValue: displayValue);
        }

        public Title(Context context, SiteSettings ss, long id, Dictionary<string, string> data)
        {
            Id = id;
            Value = data.Get("Title");
            var displayValue = ss.GetTitleColumns(context: context)?
                .Select(column => GetDisplayValue(
                    context: context,
                    ss: ss,
                    column: column,
                    data: data))
                .Where(o => !o.IsNullOrEmpty())
                .Join(ss.TitleSeparator);
            DisplayValue = GetNoTitle(
                context: context,
                displayValue: displayValue);
        }

        public Title(long id, string value)
        {
            Id = id;
            Value = value;
        }

        public Title(string value)
        {
            Value = value;
        }

        private string GetDisplayValue(
            Context context, SiteSettings ss, Column column, DataRow dataRow, string path)
        {
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsNumeric:
                    return column.HasChoices()
                        ? column.UserColumn
                            ? SiteInfo.UserName(
                                context: context,
                                userId: dataRow.Int(path))
                            : column.Choice(dataRow.Long(path).ToString()).Text
                        : column.Display(
                            context: context,
                            value: dataRow.Decimal(path),
                            unit: true);
                case Types.CsDateTime:
                    switch (path)
                    {
                        case "CompletionTime":
                            return column.DisplayControl(
                                context: context,
                                value: new CompletionTime(
                                    context: context,
                                    ss: ss,
                                    dataRow: dataRow,
                                    column: new ColumnNameInfo(path)).DisplayValue);
                        default:
                            return column.DisplayControl(
                                context: context,
                                value: dataRow.DateTime(path).ToLocal(context: context));
                    }
                case Types.CsString:
                    return column.HasChoices()
                        ? column.Choice(dataRow.String(path)).Text
                        : dataRow.String(path);
                default:
                    return dataRow.String(path);
            }
        }

        private string GetDisplayValue(
            Context context, SiteSettings ss, Column column, Dictionary<string, string> data)
        {
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsNumeric:
                    return column.HasChoices()
                        ? column.UserColumn
                            ? SiteInfo.UserName(
                                context: context,
                                userId: data.Get(column.ColumnName).ToInt())
                            : column.Choice(data.Get(column.ColumnName)).Text
                        : column.Display(
                            context: context,
                            value: data.Get(column.ColumnName).ToDecimal(),
                            unit: true);
                case Types.CsDateTime:
                    switch (column.ColumnName)
                    {
                        case "CompletionTime":
                            return column.DisplayControl(
                                context: context,
                                value: new CompletionTime(
                                    context: context,
                                    ss: ss,
                                    value: data.Get(column.ColumnName).ToDateTime()).DisplayValue);
                        default:
                            return column.DisplayControl(
                                context: context,
                                value: data.Get(column.ColumnName)
                                    .ToDateTime()
                                    .ToLocal(context: context));
                    }
                case Types.CsString:
                    return column.HasChoices()
                        ? column.Choice(data.Get(column.ColumnName)).Text
                        : data.Get(column.ColumnName);
                default:
                    return data.Get(column.ColumnName);
            }
        }

        private string GetNoTitle(Context context, string displayValue)
        {
            if (!displayValue.IsNullOrEmpty())
            {
                return displayValue;
            }
            else
            {
                switch (context.Action)
                {
                    case "export":
                        return string.Empty;
                    default:
                        return Displays.NoTitle(context: context);
                }
            }
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return Value;
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public virtual HtmlBuilder Td(HtmlBuilder hb, Context context, Column column)
        {
            return hb.Td(action: () => hb
                .P(action: () => TdTitle(hb: hb, context: context, column: column)));
        }

        protected void TdTitle(HtmlBuilder hb, Context context, Column column)
        {
            switch (column.SiteSettings.TableType)
            {
                case Sqls.TableTypes.Normal:
                    hb.A(
                        href: Locations.ItemEdit(
                            context: context,
                            id: Id)
                                + (column.Joined
                                    || column.SiteSettings.Linked
                                    || column.SiteSettings?.IntegratedSites?.Any() == true
                                        ? "?back=1"
                                        : string.Empty),
                        text: DisplayValue);
                    break;
                default:
                    hb.Text(text: DisplayValue);
                    break;
            }
        }

        public virtual string GridText(Context context, Column column)
        {
            return DisplayValue;
        }

        public virtual string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            switch (exportColumn.Type)
            {
                case ExportColumn.Types.Value:
                    return Value;
                default:
                    return DisplayValue;
            }
        }

        public string ToNotice(
            Context context,
            string saved,
            Column column,
            bool updated,
            bool update)
        {
            return Value.ToNoticeLine(
                context: context,
                saved: saved,
                column: column,
                updated: updated,
                update: update);
        }

        public bool InitialValue(Context context)
        {
            return Value.IsNullOrEmpty();
        }
    }
}
