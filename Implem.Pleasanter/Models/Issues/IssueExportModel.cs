using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class IssueExportModel : IExportModel
    {
        public string ItemTitle;
        public long? SiteId;
        public Time UpdatedTime;
        public long? IssueId;
        public int? Ver;
        public Title Title;
        public string Body;
        public DateTime? StartTime;
        public CompletionTime CompletionTime;
        public WorkValue WorkValue;
        public ProgressRate ProgressRate;
        public decimal? RemainingWorkValue;
        public Status Status;
        public User Manager;
        public User Owner;
        public Comments Comments;
        public User Creator;
        public User Updator;
        public Time CreatedTime;
        public Dictionary<string, object> ClassHash = new Dictionary<string, object>();
        public Dictionary<string, decimal> NumHash = new Dictionary<string, decimal>();
        public Dictionary<string, DateTime> DateHash = new Dictionary<string, DateTime>();
        public Dictionary<string, string> DescriptionHash = new Dictionary<string, string>();
        public Dictionary<string, bool> CheckHash = new Dictionary<string, bool>();

        public object Class(Column column)
        {
            return Class(columnName: column.ColumnName);
        }

        public object Class(string columnName)
        {
            return ClassHash.Get(columnName);
        }

        public void Class(Column column, object value)
        {
            Class(
                columnName: column.ColumnName,
                value: value);
        }

        public void Class(string columnName, object value)
        {
            if (!ClassHash.ContainsKey(columnName))
            {
                ClassHash.Add(columnName, value);
            }
            else
            {
                ClassHash[columnName] = value;
            }
        }

        public decimal Num(Column column)
        {
            return Num(columnName: column.ColumnName);
        }

        public decimal Num(string columnName)
        {
            return NumHash.Get(columnName);
        }

        public void Num(Column column, decimal value)
        {
            Num(
                columnName: column.ColumnName,
                value: value);
        }

        public void Num(string columnName, decimal value)
        {
            if (!NumHash.ContainsKey(columnName))
            {
                NumHash.Add(columnName, value);
            }
            else
            {
                NumHash[columnName] = value;
            }
        }

        public DateTime Date(Column column)
        {
            return Date(columnName: column.ColumnName);
        }

        public DateTime Date(string columnName)
        {
            return DateHash.Get(columnName);
        }

        public void Date(Column column, DateTime value)
        {
            Date(
                columnName: column.ColumnName,
                value: value);
        }

        public void Date(string columnName, DateTime value)
        {
            if (!DateHash.ContainsKey(columnName))
            {
                DateHash.Add(columnName, value);
            }
            else
            {
                DateHash[columnName] = value;
            }
        }

        public string Description(Column column)
        {
            return Description(columnName: column.ColumnName);
        }

        public string Description(string columnName)
        {
            return DescriptionHash.Get(columnName);
        }

        public void Description(Column column, string value)
        {
            Description(
                columnName: column.ColumnName,
                value: value);
        }

        public void Description(string columnName, string value)
        {
            if (!DescriptionHash.ContainsKey(columnName))
            {
                DescriptionHash.Add(columnName, value);
            }
            else
            {
                DescriptionHash[columnName] = value;
            }
        }

        public bool Check(Column column)
        {
            return Check(columnName: column.ColumnName);
        }

        public bool Check(string columnName)
        {
            return CheckHash.Get(columnName);
        }

        public void Check(Column column, bool value)
        {
            Check(
                columnName: column.ColumnName,
                value: value);
        }

        public void Check(string columnName, bool value)
        {
            if (!CheckHash.ContainsKey(columnName))
            {
                CheckHash.Add(columnName, value);
            }
            else
            {
                CheckHash[columnName] = value;
            }
        }

        public List<IExportModel> Sources;

        public IssueExportModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            string tableAlias = null)
        {
            if (dataRow != null)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
        }

        private void Set(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            string tableAlias)
        {
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            SiteId = dataRow[column.ColumnName].ToLong();
                            break;
                        case "IssueId":
                            IssueId = dataRow[column.ColumnName].ToLong();
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            break;
                        case "Title":
                            Title = new Title(context: context, ss: ss, dataRow: dataRow, column: column);
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            break;
                        case "StartTime":
                            StartTime = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "CompletionTime":
                            CompletionTime = new CompletionTime(context: context, ss: ss, dataRow: dataRow, column: column);
                            break;
                        case "WorkValue":
                            WorkValue = new WorkValue(dataRow, column);
                            break;
                        case "ProgressRate":
                            ProgressRate = new ProgressRate(dataRow, column);
                            break;
                        case "RemainingWorkValue":
                            RemainingWorkValue = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "Status":
                            Status = new Status(dataRow, column);
                            break;
                        case "Manager":
                            Manager = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            break;
                        case "Owner":
                            Owner = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDecimal());
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public void AddDestination(IExportModel exportModel, string columnName)
        {
            switch (Def.ExtendedColumnTypes.Get(columnName))
            {
                case "Class":
                    Class(
                        columnName: columnName,
                        value: exportModel);
                    break;
            }
        }

        public void AddSource(IExportModel exportModel)
        {
            if (Sources == null)
            {
                Sources = new List<IExportModel>();
            }
            Sources.Add(exportModel);
        }
    }
}
