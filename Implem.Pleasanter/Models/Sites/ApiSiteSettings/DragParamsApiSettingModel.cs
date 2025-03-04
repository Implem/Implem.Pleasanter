using Fare;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class DragParamsApiSettingModel
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public DragStateApiSettingModel State { get; set; }

        //D&D用のパラメータを追加

        public DragParamsApiSettingModel()
        {
            State = new DragStateApiSettingModel();
        }

        public void SetType(Column column)
        {
            switch (column.ColumnName)
            {
                case "Title":
                case "CompletionTime":
                case "Manager":
                case "Owner":
                case "ProgressRate":
                case "RemainingWorkValue":
                case "Status":
                case "WorkValue":
                case "TitleBody":
                case "UpdatedTime":
                case "Comments":
                case "CreatedTime":
                case "Creator":
                case "Updator":
                    Type = column.TypeCs;
                    break;
                case "Body":
                    Type = "Description";
                    break;
                case "StartTime":
                    Type = "Date";
                    break;
                //近い項目の形がわかるようにする。
                //case "IssueId":
                //case "ResultsId":
                //case "Ver":
                //    break;
                default:
                    Type = Def.ExtendedColumnTypes.Get(column.ColumnName ?? string.Empty);
                    if (Type.IsNullOrEmpty())
                    {
                        Type = null;
                    }
                    break;
            }
        }
        public void SetCategory(Column column)
        {
            switch (column.ColumnName)
            {
                case "Title":
                case "CompletionTime":
                case "Manager":
                case "Owner":
                case "ProgressRate":
                case "RemainingWorkValue":
                case "Status":
                case "WorkValue":
                case "TitleBody":
                case "UpdatedTime":
                case "Comments":
                case "CreatedTime":
                case "Creator":
                case "Updator":
                case "Body":
                case "StartTime":
                case "IssueId":
                case "ResultsId":
                case "Ver":
                    Category = "preset";
                    break;
                default:
                    switch(Def.ExtendedColumnTypes.Get(column.ColumnName ?? string.Empty)){
                        case "Class":
                        case "Num":
                        case "Date":
                        case "Check":
                        case "Description":
                        case "Attachments":
                            Category = "custtom";
                            break;
                        default:
                            Category = "none";
                            break;
                    }
                    break;
            }
        }
    }
}