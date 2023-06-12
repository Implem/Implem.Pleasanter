﻿using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class DashboardApiModel : _BaseApiModel
    {
        public long? SiteId { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DashboardId { get; set; }
        public int? Ver { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool? Locked { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ItemTitle { get; set; }
        public int? ProcessId { get; set; }

        public DashboardApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "SiteId": return SiteId;
                case "UpdatedTime": return UpdatedTime;
                case "DashboardId": return DashboardId;
                case "Ver": return Ver;
                case "Title": return Title;
                case "Body": return Body;
                case "Locked": return Locked;
                case "Comments": return Comments;
                case "Creator": return Creator;
                case "Updator": return Updator;
                case "CreatedTime": return CreatedTime;
                default: return base.ObjectValue(columnName: columnName);
            }
        }
    }
}
