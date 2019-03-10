using Implem.Libraries.DataSources.SqlServer;
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
            return columns.ToDictionary(
                column => new
                {
                    column.Id,
                    column.ColumnName,
                    column.LabelText,
                    column.Type
                }.ToJson(),
                column => new ControlData(column.GetLabelText(withSiteTitle: true)));
        }

        public static ResponseFile Export(
            Context context,
            SiteSettings ss,
            Export export,
            SqlWhereCollection where)
        {
            switch (export.Type)
            {
                case Settings.Export.Types.Csv:
                    return Csv(
                        context: context,
                        ss: ss,
                        export: export,
                        where: where);
                case Settings.Export.Types.Json:
                    return Json(
                        context: context,
                        ss: ss,
                        export: export,
                        where: where);
                default:
                    return null;
            }
        }

        private static ResponseFile Csv(
            Context context,
            SiteSettings ss,
            Export export,
            SqlWhereCollection where)
        {
            var gridData = GridData(
                context: context,
                ss: ss,
                export: export,
                where: where);
            var csv = new System.Text.StringBuilder();
            if (export.Header == true)
            {
                csv.Append(export.Columns
                    .Where(o => o.Column.CanRead)
                    .Select(column =>
                        "\"" + column.GetLabelText() + "\"").Join(","), "\n");
            }
            gridData.Csv(
                context: context,
                ss: ss,
                csv: csv,
                exportColumns: export.Columns.Where(o => o.Column.CanRead));
            return new ResponseFile(
                fileContent: csv.ToString(),
                fileDownloadName: FileName(
                    context: context,
                    ss: ss,
                    name: export.Name,
                    extension: export.Type.ToString()));
        }

        private static ResponseFile Json(
            Context context,
            SiteSettings ss,
            Export export,
            SqlWhereCollection where)
        {
            return new ResponseFile(
                fileContent: GridData(
                    context: context,
                    ss: ss,
                    export: export,
                    where: where)
                        .Json(
                            context: context,
                            ss: ss,
                            export: export),
                fileDownloadName: FileName(
                    context: context,
                    ss: ss,
                    name: export.Name,
                    extension: export.Type.ToString()));
        }

        private static GridData GridData(
            Context context,
            SiteSettings ss,
            Export export,
            SqlWhereCollection where)
        {
            export.SetColumns(
                context: context,
                ss: ss);
            ss.SetColumnAccessControls(context: context);
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            view.GridColumns = export.Columns
                .Where(o => o.Column.CanRead)
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
            Context context, SiteSettings ss, string name, string extension = "csv")
        {
            return Files.ValidFileName("_".JoinParam(
                ss.Title, name, DateTime.Now.ToLocal(
                    context: context,
                    format: Displays.YmdhmsFormat(context: context)))
                        + "." + extension);
        }
    }
}
