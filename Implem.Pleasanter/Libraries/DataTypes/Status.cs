﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using Implem.Pleasanter.Libraries.Requests;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
using System.Linq;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class Status : IConvertable
    {
        public int Value;

        public Status()
        {
        }

        public Status(DataRow dataRow, ColumnNameInfo column)
        {
            Value = dataRow.Int(Rds.DataColumnName(column, "Status"));
        }

        public Status(int value)
        {
            Value = value;
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return Value.ToString();
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return column.EditorReadOnly != true
                ? Value.ToString()
                : column.Choice(ToString()).Text;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptValues)
        {
            var choice = column.Choice(Value.ToString());
            return hb.Td(
                css: column.CellCss(serverScriptValues?.ExtendedCellCss),
                action: () => hb
                    .P(
                        attributes: new HtmlAttributes()
                            .Class(choice.CssClass)
                            .Style(choice.Style),
                        action: () => hb
                            .Text(column.ChoiceHash.Get(Value.ToString()) == null
                                ? Value == 0
                                    ? null
                                    : "?" + Value
                                : choice.TextMini)));
        }

        public bool Incomplete()
        {
            return Value < Parameters.General.CompletionCode;
        }

        public string GridText(Context context, Column column)
        {
            return column.Choice(ToString()).TextMini;
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return Value == 0 && !column.ChoiceHash.ContainsKey(ToString())
                ? null
                : column.ChoiceParts(
                    context: context,
                    selectedValues: ToString(),
                    type: exportColumn?.Type ?? ExportColumn.Types.Text)
                        .FirstOrDefault();
        }

        public string ToNotice(
            Context context,
            int saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: column.Choice(Value.ToString()).Text,
                saved: column.Choice(saved.ToString()).Text,
                column: column,
                updated: updated,
                update: update);
        }

        public bool InitialValue(Context context)
        {
            return Value == 0;
        }
    }
}