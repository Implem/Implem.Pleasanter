using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Charts
{
    public class Gantt : List<GanttElement>
    {
        public int Height;

        public Gantt(SiteSettings siteSettings, IEnumerable<DataRow> dataRows)
        {
            dataRows.ForEach(dataRow =>
            {
                Add(new GanttElement(
                    dataRow["Id"].ToLong(),
                    TitleUtility.DisplayValue(siteSettings, dataRow),
                    dataRow["WorkValue"].ToDecimal(),
                    dataRow["StartTime"].ToDateTime(),
                    dataRow["CompletionTime"].ToDateTime(),
                    dataRow["ProgressRate"].ToDecimal(),
                    dataRow["Status"].ToInt(),
                    dataRow["Owner"].ToInt(),
                    dataRow["Updator"].ToInt(),
                    dataRow["CreatedTime"].ToDateTime(),
                    dataRow["UpdatedTime"].ToDateTime(),
                    siteSettings.AllColumn("Status"),
                    siteSettings.AllColumn("WorkValue")));
            });
            Height = this.Count * 25 + 80;
        }

        public string GanttGraphJson()
        {
            return Jsons.ToJson(this
                .OrderBy(o => o.StartTime)
                .ThenBy(o => o.CompletionTime)
                .ThenBy(o => o.Title));
        }
    }
}
