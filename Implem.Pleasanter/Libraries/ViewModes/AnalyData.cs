using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;

namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class AnalyData
    {
        public AnalyPartSetting AnalyPartSetting { get; set; }
        public Dictionary<long, AnalyDataRow> Data { get; set; } = new Dictionary<long, AnalyDataRow>();
        public DataSet DataSet { get; set; }

        public AnalyData(
            AnalyPartSetting analyPartSetting,
            DataSet dataSet)
        {
            AnalyPartSetting = analyPartSetting;
            DataSet = dataSet;
            if (analyPartSetting.TimePeriodValue > 0)
            {
                SetData(
                    analyPartSetting: analyPartSetting,
                    dataTableName: "History");
            }
            SetData(
                analyPartSetting: analyPartSetting,
                dataTableName: "Normal");
        }

        private void SetData(
            AnalyPartSetting analyPartSetting,
            string dataTableName)
        {
            DataSet.Tables[dataTableName].AsEnumerable().ForEach(dataRow =>
            {
                if (!Data.ContainsKey(dataRow.Long("Id")))
                {
                    Data.Add(dataRow.Long("Id"), new AnalyDataRow(
                        analyPartSetting: analyPartSetting,
                        dataRow: dataRow));
                }
                else if (Data.Get(dataRow.Long("Id")).Ver < dataRow.Long("Ver"))
                {
                    Data[dataRow.Long("Id")] = new AnalyDataRow(
                        analyPartSetting: analyPartSetting,
                        dataRow: dataRow);
                }
            });
        }
    }
}