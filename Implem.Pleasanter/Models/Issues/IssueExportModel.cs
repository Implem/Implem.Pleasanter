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
        public decimal? NumA;
        public decimal? NumB;
        public decimal? NumC;
        public decimal? NumD;
        public decimal? NumE;
        public decimal? NumF;
        public decimal? NumG;
        public decimal? NumH;
        public decimal? NumI;
        public decimal? NumJ;
        public decimal? NumK;
        public decimal? NumL;
        public decimal? NumM;
        public decimal? NumN;
        public decimal? NumO;
        public decimal? NumP;
        public decimal? NumQ;
        public decimal? NumR;
        public decimal? NumS;
        public decimal? NumT;
        public decimal? NumU;
        public decimal? NumV;
        public decimal? NumW;
        public decimal? NumX;
        public decimal? NumY;
        public decimal? NumZ;
        public DateTime? DateA;
        public DateTime? DateB;
        public DateTime? DateC;
        public DateTime? DateD;
        public DateTime? DateE;
        public DateTime? DateF;
        public DateTime? DateG;
        public DateTime? DateH;
        public DateTime? DateI;
        public DateTime? DateJ;
        public DateTime? DateK;
        public DateTime? DateL;
        public DateTime? DateM;
        public DateTime? DateN;
        public DateTime? DateO;
        public DateTime? DateP;
        public DateTime? DateQ;
        public DateTime? DateR;
        public DateTime? DateS;
        public DateTime? DateT;
        public DateTime? DateU;
        public DateTime? DateV;
        public DateTime? DateW;
        public DateTime? DateX;
        public DateTime? DateY;
        public DateTime? DateZ;
        public string DescriptionA;
        public string DescriptionB;
        public string DescriptionC;
        public string DescriptionD;
        public string DescriptionE;
        public string DescriptionF;
        public string DescriptionG;
        public string DescriptionH;
        public string DescriptionI;
        public string DescriptionJ;
        public string DescriptionK;
        public string DescriptionL;
        public string DescriptionM;
        public string DescriptionN;
        public string DescriptionO;
        public string DescriptionP;
        public string DescriptionQ;
        public string DescriptionR;
        public string DescriptionS;
        public string DescriptionT;
        public string DescriptionU;
        public string DescriptionV;
        public string DescriptionW;
        public string DescriptionX;
        public string DescriptionY;
        public string DescriptionZ;
        public bool? CheckA;
        public bool? CheckB;
        public bool? CheckC;
        public bool? CheckD;
        public bool? CheckE;
        public bool? CheckF;
        public bool? CheckG;
        public bool? CheckH;
        public bool? CheckI;
        public bool? CheckJ;
        public bool? CheckK;
        public bool? CheckL;
        public bool? CheckM;
        public bool? CheckN;
        public bool? CheckO;
        public bool? CheckP;
        public bool? CheckQ;
        public bool? CheckR;
        public bool? CheckS;
        public bool? CheckT;
        public bool? CheckU;
        public bool? CheckV;
        public bool? CheckW;
        public bool? CheckX;
        public bool? CheckY;
        public bool? CheckZ;
        public Comments Comments;
        public User Creator;
        public User Updator;
        public Time CreatedTime;
        public object ClassA;
        public object ClassB;
        public object ClassC;
        public object ClassD;
        public object ClassE;
        public object ClassF;
        public object ClassG;
        public object ClassH;
        public object ClassI;
        public object ClassJ;
        public object ClassK;
        public object ClassL;
        public object ClassM;
        public object ClassN;
        public object ClassO;
        public object ClassP;
        public object ClassQ;
        public object ClassR;
        public object ClassS;
        public object ClassT;
        public object ClassU;
        public object ClassV;
        public object ClassW;
        public object ClassX;
        public object ClassY;
        public object ClassZ;
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
                        case "ClassA":
                            ClassA = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassB":
                            ClassB = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassC":
                            ClassC = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassD":
                            ClassD = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassE":
                            ClassE = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassF":
                            ClassF = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassG":
                            ClassG = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassH":
                            ClassH = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassI":
                            ClassI = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassJ":
                            ClassJ = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassK":
                            ClassK = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassL":
                            ClassL = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassM":
                            ClassM = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassN":
                            ClassN = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassO":
                            ClassO = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassP":
                            ClassP = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassQ":
                            ClassQ = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassR":
                            ClassR = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassS":
                            ClassS = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassT":
                            ClassT = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassU":
                            ClassU = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassV":
                            ClassV = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassW":
                            ClassW = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassX":
                            ClassX = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassY":
                            ClassY = dataRow[column.ColumnName].ToString();
                            break;
                        case "ClassZ":
                            ClassZ = dataRow[column.ColumnName].ToString();
                            break;
                        case "NumA":
                            NumA = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumB":
                            NumB = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumC":
                            NumC = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumD":
                            NumD = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumE":
                            NumE = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumF":
                            NumF = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumG":
                            NumG = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumH":
                            NumH = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumI":
                            NumI = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumJ":
                            NumJ = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumK":
                            NumK = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumL":
                            NumL = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumM":
                            NumM = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumN":
                            NumN = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumO":
                            NumO = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumP":
                            NumP = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumQ":
                            NumQ = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumR":
                            NumR = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumS":
                            NumS = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumT":
                            NumT = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumU":
                            NumU = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumV":
                            NumV = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumW":
                            NumW = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumX":
                            NumX = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumY":
                            NumY = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "NumZ":
                            NumZ = dataRow[column.ColumnName].ToDecimal();
                            break;
                        case "DateA":
                            DateA = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateB":
                            DateB = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateC":
                            DateC = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateD":
                            DateD = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateE":
                            DateE = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateF":
                            DateF = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateG":
                            DateG = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateH":
                            DateH = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateI":
                            DateI = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateJ":
                            DateJ = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateK":
                            DateK = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateL":
                            DateL = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateM":
                            DateM = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateN":
                            DateN = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateO":
                            DateO = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateP":
                            DateP = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateQ":
                            DateQ = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateR":
                            DateR = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateS":
                            DateS = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateT":
                            DateT = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateU":
                            DateU = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateV":
                            DateV = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateW":
                            DateW = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateX":
                            DateX = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateY":
                            DateY = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DateZ":
                            DateZ = dataRow[column.ColumnName].ToDateTime();
                            break;
                        case "DescriptionA":
                            DescriptionA = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionB":
                            DescriptionB = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionC":
                            DescriptionC = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionD":
                            DescriptionD = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionE":
                            DescriptionE = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionF":
                            DescriptionF = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionG":
                            DescriptionG = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionH":
                            DescriptionH = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionI":
                            DescriptionI = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionJ":
                            DescriptionJ = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionK":
                            DescriptionK = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionL":
                            DescriptionL = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionM":
                            DescriptionM = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionN":
                            DescriptionN = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionO":
                            DescriptionO = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionP":
                            DescriptionP = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionQ":
                            DescriptionQ = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionR":
                            DescriptionR = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionS":
                            DescriptionS = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionT":
                            DescriptionT = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionU":
                            DescriptionU = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionV":
                            DescriptionV = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionW":
                            DescriptionW = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionX":
                            DescriptionX = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionY":
                            DescriptionY = dataRow[column.ColumnName].ToString();
                            break;
                        case "DescriptionZ":
                            DescriptionZ = dataRow[column.ColumnName].ToString();
                            break;
                        case "CheckA":
                            CheckA = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckB":
                            CheckB = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckC":
                            CheckC = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckD":
                            CheckD = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckE":
                            CheckE = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckF":
                            CheckF = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckG":
                            CheckG = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckH":
                            CheckH = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckI":
                            CheckI = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckJ":
                            CheckJ = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckK":
                            CheckK = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckL":
                            CheckL = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckM":
                            CheckM = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckN":
                            CheckN = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckO":
                            CheckO = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckP":
                            CheckP = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckQ":
                            CheckQ = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckR":
                            CheckR = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckS":
                            CheckS = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckT":
                            CheckT = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckU":
                            CheckU = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckV":
                            CheckV = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckW":
                            CheckW = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckX":
                            CheckX = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckY":
                            CheckY = dataRow[column.ColumnName].ToBool();
                            break;
                        case "CheckZ":
                            CheckZ = dataRow[column.ColumnName].ToBool();
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
                    }
                }
            }
        }

        public void AddDestination(IExportModel exportModel, string columnName)
        {
            switch (columnName)
            {
                case "ClassA": ClassA = exportModel; break;
                case "ClassB": ClassB = exportModel; break;
                case "ClassC": ClassC = exportModel; break;
                case "ClassD": ClassD = exportModel; break;
                case "ClassE": ClassE = exportModel; break;
                case "ClassF": ClassF = exportModel; break;
                case "ClassG": ClassG = exportModel; break;
                case "ClassH": ClassH = exportModel; break;
                case "ClassI": ClassI = exportModel; break;
                case "ClassJ": ClassJ = exportModel; break;
                case "ClassK": ClassK = exportModel; break;
                case "ClassL": ClassL = exportModel; break;
                case "ClassM": ClassM = exportModel; break;
                case "ClassN": ClassN = exportModel; break;
                case "ClassO": ClassO = exportModel; break;
                case "ClassP": ClassP = exportModel; break;
                case "ClassQ": ClassQ = exportModel; break;
                case "ClassR": ClassR = exportModel; break;
                case "ClassS": ClassS = exportModel; break;
                case "ClassT": ClassT = exportModel; break;
                case "ClassU": ClassU = exportModel; break;
                case "ClassV": ClassV = exportModel; break;
                case "ClassW": ClassW = exportModel; break;
                case "ClassX": ClassX = exportModel; break;
                case "ClassY": ClassY = exportModel; break;
                case "ClassZ": ClassZ = exportModel; break;
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
