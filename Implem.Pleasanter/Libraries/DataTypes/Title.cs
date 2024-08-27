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
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class Title : IConvertable
    {
        public long Id;
        public int Ver;
        public bool IsHistory = false;
        public string Value = string.Empty;
        public string DisplayValue;
        public List<string> PartCollection;
        public bool ItemTitle;

        public Title()
        {
        }

        public Title(DataRow dataRow, string name)
        {
            Id = dataRow.Long(name);
            Ver = dataRow.Int("Ver");
            IsHistory = dataRow.Bool("IsHistory");
            Value = dataRow.String("Title");
            DisplayValue = Value;
        }

        public Title(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            ColumnNameInfo column = null)
        {
            Id = dataRow.Long((column?.Joined == true
                ? column.TableAlias + ","
                : string.Empty) +
                    Rds.IdColumn(ss.ReferenceType));
            Ver = dataRow.Int("Ver");
            Value = dataRow.String(Rds.DataColumnName(column, "Title"));
            var itemTitlePath = Rds.DataColumnName(column, "ItemTitle");
            ItemTitle = dataRow.Table.Columns.Contains(itemTitlePath);
            var displayValue = ItemTitle
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

        public Title(
            Context context,
            SiteSettings ss,
            long id,
            int ver,
            bool isHistory,
            Dictionary<string, string> data,
            bool getLinkedTitle = false)
        {
            Id = id;
            Ver = ver;
            IsHistory = isHistory;
            Value = data.Get("Title");
            var displayValue = ss.GetTitleColumns(context: context)?
                .Select(column => GetDisplayValue(
                    context: context,
                    ss: ss,
                    column: column,
                    data: data,
                    getLinkedTitle: getLinkedTitle))
                .Where(o => !o.IsNullOrEmpty())
                .Join(ss.TitleSeparator);
            displayValue = displayValue.Length > ss.ColumnHash["Title"].Max.ToInt()
                ? displayValue.Substring(0, ss.ColumnHash["Title"].Max.ToInt())
                : displayValue;
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
            Context context,
            SiteSettings ss,
            Column column,
            DataRow dataRow,
            string path)
        {
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsNumeric:
                    return column.HasChoices()
                        ? column.Type != Column.Types.Normal
                            ? SiteInfo.Name(
                                context: context,
                                id: dataRow.Int(path),
                                type: column.Type)
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
            Context context,
            SiteSettings ss,
            Column column,
            Dictionary<string, string> data,
            bool getLinkedTitle = false)
        {
            if (column.Type != Column.Types.Normal)
            {
                return column.MultipleSelections == true
                    ? data.Get(column.ColumnName).Deserialize<List<string>>()
                        ?.Select(col =>
                            SiteInfo.Name(
                            context: context,
                            id: col.ToInt(),
                            type: column.Type))
                        .Join()
                    : SiteInfo.Name(
                        context: context,
                        id: data.Get(column.ColumnName).ToInt(),
                        type: column.Type);
            }
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsNumeric:
                    var value = data.Get(column.ColumnName);
                    return column.HasChoices()
                        ? column.Choice(value).Text
                        : column.Display(
                            context: context,
                            value: value.IsNullOrEmpty()
                                ? null
                                : value.ToDecimal(),
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
                        ? getLinkedTitle || column.Linked(
                            context: context,
                            withoutWiki: true)
                                ? column.LinkedTitleChoice(
                                    context: context,
                                    selectedValues: data.Get(column.ColumnName)).Text
                                : column.MultipleSelections == true
                                    ? data.Get(column.ColumnName).Deserialize<List<string>>()
                                        ?.Select(col => column.Choice(selectedValue: col).Text)
                                        .Join()
                                    : column.Choice(selectedValue: data.Get(column.ColumnName)).Text
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

        public virtual string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return DisplayValue;
        }

        public string ToLookup(Context context, SiteSettings ss, Column column, Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return Value;
            }
        }

        public override string ToString()
        {
            return Value;
        }

        public virtual HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .P(action: () => TdTitle(
                        hb: hb,
                        context: context,
                        column: column,
                        tabIndex: tabIndex)));
        }

        protected void TdTitle(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex)
        {
            var queryString = new[] {
                column.Joined
                    || column.SiteSettings.Linked
                    || column.SiteSettings?.IntegratedSites?.Any() == true
                       ? "back=1"
                       : string.Empty,
                IsHistory
                    ? "ver=" + Ver
                    : string.Empty,
                tabIndex.HasValue
                    ? $"FromTabIndex={tabIndex}"
                    : string.Empty
            }
                .Where(s => s != string.Empty)
                .Join("&");
            if (Id > 0
                && column.SiteSettings?.TableType == Sqls.TableTypes.Normal
                && column.SiteSettings?.GetNoDisplayIfReadOnly(context: context) != true)
            {
                if (column.SiteSettings?.DisableLinkToEdit == true)
                {
                    hb.Text(text: DisplayValue);
                }
                else if (column.SiteSettings?.OpenEditInNewTab == true)
                {
                    hb.A(
                        href: Locations.ItemEdit(
                            context: context,
                            id: Id)
                            + ((queryString == string.Empty)
                                ? string.Empty
                                : "?" + queryString),
                        target: "_blank",
                        text: DisplayValue);
                }
                else
                {
                    hb.A(
                        href: Locations.ItemEdit(
                            context: context,
                            id: Id)
                            + ((queryString == string.Empty)
                               ? string.Empty
                               : "?" + queryString),
                        text: DisplayValue);
                }
            }
        }

        public virtual string GridText(Context context, Column column)
        {
            return DisplayValue;
        }

        public virtual object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            return DisplayValue;
        }

        public virtual object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return Value;
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
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: Value,
                saved: saved,
                column: column,
                updated: updated,
                update: update);
        }

        public bool InitialValue(Context context)
        {
            return Value.IsNullOrEmpty();
        }

        public string MessageDisplay(Context context)
        {
            var title = Strings.CoalesceEmpty(
                DisplayValue,
                Value,
                $"ID: {context.Id}");
            return title;
        }
    }
}
