
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;

namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class Analy : List<AnalyPart>
    {
        public Analy(
            Context context,
            SiteSettings ss,
            List<EnumerableRowCollection<DataRow>> dataRowsSet)
        {
            dataRowsSet.ForEach(dataRows =>
            {
                var analyPart = new AnalyPart();
                dataRows.ForEach(dataRow =>
                {
                    var analyPartElement = new AnalyPartElement(
                        title: dataRow.String("GroupBy"),
                        value: dataRow.Decimal("Value"));
                    analyPart.Add(analyPartElement);
                });
                this.Add(analyPart);
            });
        }
    }
}