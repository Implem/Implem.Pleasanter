using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class DataView
    {
        public bool? Incomplete;
        public bool? Own;
        public bool? NearCompletionTime;
        public bool? Delay;
        public bool? Overdue;
        public Dictionary<string, string> ColumnFilters = new Dictionary<string, string>();
        public string Search;
        public Dictionary<string, SqlOrderBy.Types> ColumnSorters =
            new Dictionary<string, SqlOrderBy.Types>();

        public DataView(SiteSettings ss)
        {
            foreach (string controlId in HttpContext.Current.Request.Form)
            {
                switch (controlId)
                {
                    case "DataViewFilters_Incomplete":
                        Incomplete = Bool(controlId);
                        break;
                    case "DataViewFilters_Own":
                        Own = Bool(controlId);
                        break;
                    case "DataViewFilters_NearCompletionTime":
                        NearCompletionTime = Bool(controlId);
                        break;
                    case "DataViewFilters_Delay":
                        Delay = Bool(controlId);
                        break;
                    case "DataViewFilters_Overdue":
                        Overdue = Bool(controlId);
                        break;
                    case "DataViewFilters_Search":
                        Search = String(controlId);
                        break;
                    default:
                        if (controlId.StartsWith("DataViewFilters_"))
                        {
                            SetFilters(ss, controlId);
                        }
                        else if (controlId.StartsWith("DataViewSorters_"))
                        {
                            SetSorters(controlId);
                        }
                        break;
                }
            }
        }

        private bool? Bool(string controlId)
        {
            var data = Forms.Bool(controlId);
            if (data)
            {
                return true;
            }
            else
            {
                return null;
            }
        }

        private string String(string controlId)
        {
            var data = Forms.Data(controlId);
            if (data != string.Empty)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        private void SetFilters(SiteSettings ss, string controlId)
        {
            var data = Forms.Data(controlId);
            var columnName = controlId.Split_2nd('_');
            if (ss.ColumnCollection.Any(o => o.ColumnName == columnName))
            {
                if (data != string.Empty)
                {
                    if (ColumnFilters.ContainsKey(columnName))
                    {
                        ColumnFilters[columnName] = data;
                    }
                    else
                    {
                        ColumnFilters.Add(columnName, data);
                    }
                }
                else if (ColumnFilters.ContainsKey(columnName))
                {
                    ColumnFilters.Remove(columnName);
                }
            }
        }

        private void SetSorters(string controlId)
        {
            var data = GridSorters.Type(Forms.Data(controlId));
            switch (data)
            {
                case SqlOrderBy.Types.asc:
                case SqlOrderBy.Types.desc:
                    if (ColumnSorters.ContainsKey(controlId))
                    {
                        ColumnSorters[controlId] = data;
                    }
                    else
                    {
                        ColumnSorters.Add(controlId, data);
                    }
                    break;
                case SqlOrderBy.Types.release:
                    if (ColumnSorters.ContainsKey(controlId))
                    {
                        ColumnSorters.Remove(controlId);
                    }
                    break;
            }
        }

        public string RecordingJson()
        {
            if (!ColumnFilters.Any()) ColumnFilters = null;
            if (!ColumnSorters.Any()) ColumnSorters = null;
            return this.ToJson();
        }
    }
}