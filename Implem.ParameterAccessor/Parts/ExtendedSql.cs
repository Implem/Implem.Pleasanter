using System;
using System.Linq;
using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class ExtendedSql : ExtendedBase
    {
        public bool Api;
        public bool Html;
        public bool OnCreating;
        public bool OnCreated;
        public bool OnUpdating;
        public bool OnUpdated;
        public bool OnUpdatingByGrid;
        public bool OnUpdatedByGrid;
        public bool OnDeleting;
        public bool OnDeleted;
        public bool OnBulkUpdating;
        public bool OnBulkUpdated;
        public bool OnBulkDeleting;
        public bool OnBulkDeleted;
        public bool OnImporting;
        public bool OnImported;
        public bool OnSelectingColumn;
        public bool OnSelectingWhere;
        public bool OnSelectingWherePermissionsDepts;
        public bool OnSelectingWherePermissionsGroups;
        public bool OnSelectingWherePermissionsUsers;
        public List<string> OnSelectingWhereParams;
        public bool OnUseSecondaryAuthentication;
        public string CommandText;

        public string ReplacedCommandText(
            long siteId,
            long id,
            DateTime? timestamp,
            Dictionary<string, string> columnPlaceholders = null)
        {
            var commandText = CommandText
                .Replace("{{SiteId}}", siteId.ToString())
                .Replace("{{Id}}", id.ToString())
                .Replace("{{Timestamp}}", timestamp?.ToString("yyyy/M/d H:m:s.fff"));
            if (columnPlaceholders != null)
            {
                foreach (var columnPlaceholder in columnPlaceholders)
                {
                    commandText = commandText.Replace("{{" + columnPlaceholder.Key + "}}", columnPlaceholder.Value);
                }
            }
            return commandText;
        }
    }
}