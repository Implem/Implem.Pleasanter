using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class ColumnAccessControl
    {
        [NonSerialized]
        public int? No;
        public string ColumnName;
        public Permissions.Types AllowedType;
        public IEnumerable<string> AllowedUsers;

        public ColumnAccessControl()
        {
        }

        public ColumnAccessControl(SiteSettings ss, Column column, string type)
        {
            No = column.No;
            ColumnName = column.ColumnName;
            AllowedType = DefaultType(ss, type);
        }

        public bool IsDefault(SiteSettings ss, string type)
        {
            return DefaultType(ss, type) == AllowedType && AllowedUsers?.Any() != true;
        }

        private Permissions.Types DefaultType(SiteSettings ss, string type)
        {
            switch (type)
            {
                case "Create":
                    return Permissions.Get(
                        ss.ColumnDefinitionHash.Get(ColumnName)?.CreateAccessControl);
                case "Read":
                    return Permissions.Get(
                        ss.ColumnDefinitionHash.Get(ColumnName)?.ReadAccessControl);
                case "Update":
                    return Permissions.Get(
                        ss.ColumnDefinitionHash.Get(ColumnName)?.UpdateAccessControl);
                default: return Permissions.Types.NotSet;
            }
        }

        public ControlData ControlData(SiteSettings ss, string type)
        {
            var column = ss.GetColumn(ColumnName);
            return column != null
                ? new ControlData(column.LabelText + (!IsDefault(ss, type)
                    ? " (" + Displays.Enabled() + ")"
                    : string.Empty))
                : new ControlData(string.Empty);
        }

        public bool Allowed(Permissions.Types? type, List<string> mine)
        {
            if (AllowedType == Permissions.Types.NotSet && AllowedUsers?.Any() != true)
            {
                return true;
            }
            else if (AllowedType > 0 && (AllowedType & type) == AllowedType)
            {
                return true;
            }
            else if (AllowedType > 0 && AllowedUsers?.Any() != true)
            {
                return false;
            }
            else if (mine == null)
            {
                return true;
            }
            else if (AllowedUsers?.Any(o => mine?.Contains(o) == true) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}