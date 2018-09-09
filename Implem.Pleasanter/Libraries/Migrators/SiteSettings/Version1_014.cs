using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_014
    {
        public static void Migrate1_014(this SiteSettings ss)
        {
            ss.CreateColumnAccessControls?.ForEach(columnAccessControl =>
            {
                columnAccessControl.AllowedUsers?.ForEach(user =>
                {
                    if (columnAccessControl.RecordUsers == null)
                    {
                        columnAccessControl.RecordUsers = new List<string>();
                    }
                    columnAccessControl.RecordUsers.Add(user);
                });
                columnAccessControl.AllowedUsers = null;
            });
            ss.ReadColumnAccessControls?.ForEach(columnAccessControl =>
            {
                columnAccessControl.AllowedUsers?.ForEach(user =>
                {
                    if (columnAccessControl.RecordUsers == null)
                    {
                        columnAccessControl.RecordUsers = new List<string>();
                    }
                    columnAccessControl.RecordUsers.Add(user);
                });
                columnAccessControl.AllowedUsers = null;
            });
            ss.UpdateColumnAccessControls?.ForEach(columnAccessControl =>
            {
                columnAccessControl.AllowedUsers?.ForEach(user =>
                {
                    if (columnAccessControl.RecordUsers == null)
                    {
                        columnAccessControl.RecordUsers = new List<string>();
                    }
                    columnAccessControl.RecordUsers.Add(user);
                });
                columnAccessControl.AllowedUsers = null;
            });
            ss.Migrated = true;
        }
    }
}