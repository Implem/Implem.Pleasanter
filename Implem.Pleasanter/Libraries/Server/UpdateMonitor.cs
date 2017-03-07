using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Server
{
    public class UpdateMonitor
    {
        public static DateTime DeptsUpdatedTime;
        public static DateTime GroupsUpdatedTime;
        public static DateTime UsersUpdatedTime;
        public static DateTime PermissionsUpdatedTime;
        public static DateTime SitesUpdatedTime;
        public DateTime NowDeptsUpdatedTime;
        public DateTime NowGroupsUpdatedTime;
        public DateTime NowUsersUpdatedTime;
        public DateTime NowPermissionsUpdatedTime;
        public DateTime NowSitesUpdatedTime;
        public Dictionary<string, DateTime> UpdatedTimeHash;
        public bool Updated;
        public bool DeptsUpdated;
        public bool GroupsUpdated;
        public bool UsersUpdated;
        public bool PermissionsUpdated;
        public bool SitesUpdated;

        public UpdateMonitor()
        {
            Set();
            DeptsUpdated = DeptsUpdatedTime != NowDeptsUpdatedTime;
            GroupsUpdated = GroupsUpdatedTime != NowGroupsUpdatedTime;
            UsersUpdated = UsersUpdatedTime != NowUsersUpdatedTime;
            PermissionsUpdated = PermissionsUpdatedTime != NowPermissionsUpdatedTime;
            SitesUpdated = SitesUpdatedTime != NowSitesUpdatedTime;
            Updated = DeptsUpdated || GroupsUpdated || UsersUpdated || SitesUpdated || PermissionsUpdated;
        }

        private void Set()
        {
            var hash = StatusUtilities.Monitors();
            NowDeptsUpdatedTime = hash[StatusUtilities.Types.DeptsUpdated];
            NowGroupsUpdatedTime = hash[StatusUtilities.Types.GroupsUpdated];
            NowUsersUpdatedTime = hash[StatusUtilities.Types.UsersUpdated];
            NowPermissionsUpdatedTime = hash[StatusUtilities.Types.PermissionsUpdated];
            NowSitesUpdatedTime = hash[StatusUtilities.Types.SitesUpdated];
        }

        public void Update()
        {
            DeptsUpdatedTime = NowDeptsUpdatedTime;
            GroupsUpdatedTime = NowGroupsUpdatedTime;
            UsersUpdatedTime = NowUsersUpdatedTime;
            PermissionsUpdatedTime = NowPermissionsUpdatedTime;
            SitesUpdatedTime = NowSitesUpdatedTime;
        }
    }
}