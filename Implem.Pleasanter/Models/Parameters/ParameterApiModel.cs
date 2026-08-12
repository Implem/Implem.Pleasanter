using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ParameterApiModel : _BaseApiModel
    {
        public int? ParameterId { get; set; }
        public int? Ver { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public ParameterApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "ParameterId": return ParameterId;
                case "Ver": return Ver;
                case "Title": return Title;
                case "Body": return Body;
                case "Comments": return Comments;
                case "Creator": return Creator;
                case "Updator": return Updator;
                case "CreatedTime": return CreatedTime;
                case "UpdatedTime": return UpdatedTime;
                default: return base.ObjectValue(columnName: columnName);
            }
        }
    }
}
