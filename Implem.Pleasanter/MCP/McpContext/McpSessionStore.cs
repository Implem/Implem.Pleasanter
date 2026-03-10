using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Implem.Pleasanter.MCP.McpContext
{
    public static class McpSessionStore
    {
        private static readonly ConcurrentDictionary<string, McpSessionInfo> _sessions = new();
        private static Timer _cleanupTimer;
        private static readonly TimeSpan SessionTimeout = TimeSpan.FromHours(24);
        private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(30);

        static McpSessionStore()
        {
            _cleanupTimer = new Timer(
                callback: _ => CleanupExpiredSessions(),
                state: null,
                dueTime: CleanupInterval,
                period: CleanupInterval);
        }

        public static McpSessionInfo GetOrCreate(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }

            return _sessions.GetOrAdd(sessionId, id => new McpSessionInfo
            {
                SessionId = id,
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow
            });
        }

        public static McpSessionInfo Get(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }

            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.LastAccessedAt = DateTime.UtcNow;
                return session;
            }

            return null;
        }

        public static void UpdateClientInfo(
            string sessionId,
            string clientName,
            string clientVersion,
            string protocolVersion)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }

            var session = GetOrCreate(sessionId);
            if (session != null)
            {
                session.ClientName = clientName;
                session.ClientVersion = clientVersion;
                session.ProtocolVersion = protocolVersion;
                session.LastAccessedAt = DateTime.UtcNow;
            }
        }

        public static void SetAuthInfo(string sessionId, int tenantId, int userId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }

            var session = Get(sessionId);
            if (session != null)
            {
                session.TenantId = tenantId;
                session.UserId = userId;
                session.LastAccessedAt = DateTime.UtcNow;
            }
        }

        public static void Remove(string sessionId)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                _sessions.TryRemove(sessionId, out _);
            }
        }

        private static void CleanupExpiredSessions()
        {
            var threshold = DateTime.UtcNow - SessionTimeout;
            var expiredKeys = _sessions
                .Where(kvp => kvp.Value.LastAccessedAt < threshold)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _sessions.TryRemove(key, out _);
            }
        }

        public static int Count => _sessions.Count;
    }

    public class McpSessionInfo
    {
        public string SessionId { get; set; }

        public string ClientName { get; set; }

        public string ClientVersion { get; set; }

        public string ProtocolVersion { get; set; }

        public int TenantId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastAccessedAt { get; set; }
    }
}
