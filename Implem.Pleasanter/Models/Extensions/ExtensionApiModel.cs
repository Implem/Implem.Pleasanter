using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
using MimeKit;

namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ExtensionApiModel : _MostBaseApiModel
    {
        public int? ExtensionId { get; set; }
        public int? TenantId { get; set; }
        public int? Ver { get; set; }
        public string ExtensionType { get; set; }
        public string ExtensionName { get; set; }
        public string ExtensionSettings { get; set; }
        public string Body { get; set; }
        public string Description { get; set; }
        public bool? Disabled { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public ExtensionApiModel()
        {
        }

        public object ObjectValue(string columnName)
        {
            return columnName switch
            {
                "ExtensionId" => ExtensionId,
                "TenantId" => TenantId,
                "Ver" => Ver,
                "ExtensionType" => ExtensionType,
                "ExtensionName" => ExtensionName,
                "ExtensionSettings" => ExtensionSettings,
                "Body" => Body,
                "Description" => Description,
                "Disabled" => Disabled,
                "Comments" => Comments,
                "Creator" => Creator,
                "Updator" => Updator,
                "CreatedTime" => CreatedTime,
                "UpdatedTime" => UpdatedTime,
                _ => null
            };
        }
    }
}
