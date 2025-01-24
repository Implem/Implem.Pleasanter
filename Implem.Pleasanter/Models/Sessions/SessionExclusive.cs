using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Runtime.CompilerServices;

namespace Implem.Pleasanter.Models.Sessions
{
    public class SessionExclusive : IDisposable
    {
        public class Lock
        {
            public long SiteId { get; set; }
            public int UserId { get; set; }
            public string Comment { get; set; }
            public DateTime UpdatedTime { get; set; }

            public Lock(long siteId, int userId, string comment)
            {
                SiteId = siteId;
                UserId = userId;
                Comment = comment;
                UpdatedTime = DateTime.MinValue;
            }

            public Lock() { }
        }
        protected readonly Lock LockObj;
        public readonly string Key;
        private readonly Context Context;
        private const string SessionExclusiveGuid = "SessionExclusive";

        protected SessionExclusive(Context context, bool enabled, string key, string comment = "")
        {
            Key = key;
            Context = context;
            LockObj = enabled
                ? new Lock(siteId: context.SiteId, userId: context.UserId, comment: comment)
                : null;
        }

        public bool TryLock()
        {
            if (LockObj == null) return true;
            Lock value = null;
            if (SessionUtilities.Get(
                context: Context,
                sessionGuid: SessionExclusiveGuid)
                    .TryGetValue(Key, out var rawValue))
            {
                value = rawValue.Deserialize<Lock>();
            }
            if (value == null || value.UpdatedTime.AddSeconds(120) < DateTime.UtcNow)
            {
                Refresh();
                return true;
            }
            return false;
        }

        public void Refresh()
        {
            if (LockObj == null) return;
            if (LockObj.UpdatedTime != DateTime.MinValue && LockObj.UpdatedTime.AddSeconds(30) > DateTime.UtcNow) return;
            LockObj.UpdatedTime = DateTime.UtcNow;
            SessionUtilities.Set(
                context: Context,
                key: Key,
                value: LockObj.ToJson(),
                sessionGuid: SessionExclusiveGuid);
        }

        private void Clear()
        {
            if (LockObj == null || LockObj.UpdatedTime == DateTime.MinValue) return;
            LockObj.UpdatedTime = DateTime.MinValue;
            SessionUtilities.Remove(
                context: Context,
                key: Key,
                page: false,
                sessionGuid: SessionExclusiveGuid);
        }

        public void Dispose()
        {
            Clear();
        }
    }

    public class TableExclusive : SessionExclusive
    {
        public TableExclusive(Context context, [CallerMemberName] string callerMethodName = "")
            : base(
                  context: context,
                  enabled: Parameters.General.BlockSiteTaskWhileRunning == true,
                  key: $"TableExclusive_SiteId={context.SiteId}",
                  comment: callerMethodName)
        {
        }
    }
}
