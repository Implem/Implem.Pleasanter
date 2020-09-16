﻿using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class ExportUtilities
    {
        public static Dictionary<string, ControlData> ColumnOptions(
            IEnumerable<ExportColumn> columns)
        {
            return columns
                .Where(o => o.Column.TypeCs != "Attachments")
                .ToDictionary(
                    column => column.GetRecordingData().ToJson(),
                    column => new ControlData(column.GetLabelText(withSiteTitle: true)));
        }

        public static string Export(
            Context context,
            SiteSettings ss,
            Export export,
            SqlWhereCollection where,
            View view)
        {
            switch (export.Type)
            {
                case Settings.Export.Types.Csv:
                    return Csv(
                        context: context,
                        ss: ss,
                        export: export,
                        where: where,
                        view: view);
                case Settings.Export.Types.Json:
                    return Json(
                        context: context,
                        ss: ss,
                        export: export,
                        where: where,
                        view: view);
                default:
                    return null;
            }
        }

        private static string Csv(
            Context context,
            SiteSettings ss,
            Export export,
            SqlWhereCollection where,
            View view)
        {
            var gridData = GridData(
                context: context,
                ss: ss,
                export: export,
                where: where,
                view: view);
            var csv = new System.Text.StringBuilder();
            if (export.Header == true)
            {
                csv.Append(export.Columns
                    .Where(o => o.Column.CanRead)
                    .Where(o => o.Column.TypeCs != "Attachments")
                    .Select(column =>
                        "\"" + column.GetLabelText() + "\"").Join(","), "\n");
            }
            gridData.Csv(
                context: context,
                ss: ss,
                csv: csv,
                exportColumns: export.Columns
                    .Where(o => o.Column.CanRead)
                    .Where(o => o.Column.TypeCs != "Attachments"));
            return csv.ToString();
        }

        private static string Json(
            Context context,
            SiteSettings ss,
            Export export,
            SqlWhereCollection where,
            View view)
        {
            return GridData(
                context: context,
                ss: ss,
                export: export,
                where: where,
                view: view)
                    .Json(
                        context: context,
                        ss: ss,
                        export: export);
        }

        private static GridData GridData(
            Context context,
            SiteSettings ss,
            Export export,
            SqlWhereCollection where,
            View view)
        {
            export.SetColumns(
                context: context,
                ss: ss);
            ss.SetColumnAccessControls(context: context);
            view.GridColumns = export.Columns
                .Where(o => o.Column.CanRead)
                .Where(o => o.Column.TypeCs != "Attachments")
                .Select(o => o.ColumnName)
                .ToList();
            var gridData = new GridData(
                context: context,
                ss: ss,
                view: view,
                where: where);
            return gridData;
        }

        public static string FileName(
            Context context, string title = null, string name = null, string extension = "csv")
        {
            return Files.ValidFileName("_".JoinParam(
                title, name, DateTime.Now.ToLocal(
                    context: context,
                    format: Displays.YmdhmsFormat(context: context)))
                        + "." + extension);
        }
    }
}
