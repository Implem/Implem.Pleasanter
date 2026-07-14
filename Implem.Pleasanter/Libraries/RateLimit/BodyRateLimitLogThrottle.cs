using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.RateLimit
{
    public sealed class BodyRateLimitLogThrottle
    {
        internal const int DefaultWindowSeconds = 10;
        internal const int PerKeyDetail = 1;
        internal const int DefaultGlobalMaxPerWindow = 50;
        internal const int DefaultEvictAfterWindows = 3;
        internal const int DefaultMaxTrackedKeys = 10000;

        private sealed class WindowState
        {
            public DateTime WindowStart;
            public int Count;
            public string PolicyName;
            public string PartitionKey;
            public string Mode;
        }

        private readonly ConcurrentDictionary<(string Policy, string PartitionKey), WindowState> _dict
            = new ConcurrentDictionary<(string, string), WindowState>();

        private readonly object _globalLock = new object();
        private DateTime _globalWindowStart;
        private int _globalLoggedThisWindow;
        private int _globalSuppressedKeys;
        private int _globalSuppressedCount;

        private readonly Func<DateTime> _now;
        private readonly TimeSpan _window;
        private readonly int _windowSeconds;
        private readonly int _globalMaxPerWindow;
        private readonly TimeSpan _evictAge;
        private readonly int _maxTrackedKeys;

        internal int TrackedKeyCount => _dict.Count;

        public BodyRateLimitLogThrottle()
            : this(() => DateTime.UtcNow)
        {
        }

        internal BodyRateLimitLogThrottle(
            Func<DateTime> nowProvider,
            int windowSeconds = DefaultWindowSeconds,
            int globalMaxPerWindow = DefaultGlobalMaxPerWindow,
            int evictAfterWindows = DefaultEvictAfterWindows,
            int maxTrackedKeys = DefaultMaxTrackedKeys)
        {
            _now = nowProvider ?? (() => DateTime.UtcNow);
            _windowSeconds = Math.Max(1, windowSeconds);
            _window = TimeSpan.FromSeconds(_windowSeconds);
            _globalMaxPerWindow = Math.Max(1, globalMaxPerWindow);
            _evictAge = TimeSpan.FromSeconds((double)_windowSeconds * Math.Max(1, evictAfterWindows));
            _maxTrackedKeys = Math.Max(1, maxTrackedKeys);
            _globalWindowStart = _now();
        }

        public void Process(RateLimitLogEvent ev, Action<RateLimitLogEvent> emit)
        {
            if (ev == null || emit == null) return;
            var now = _now();
            var toEmit = new List<RateLimitLogEvent>();
            var crossedGlobalWindow = false;
            lock (_globalLock)
            {
                if (now - _globalWindowStart >= _window)
                {
                    if (_globalSuppressedCount > 0)
                    {
                        toEmit.Add(BuildGlobalSuppressed(_globalSuppressedKeys, _globalSuppressedCount));
                    }
                    _globalWindowStart = now;
                    _globalLoggedThisWindow = 0;
                    _globalSuppressedKeys = 0;
                    _globalSuppressedCount = 0;
                    crossedGlobalWindow = true;
                }
            }
            if (crossedGlobalWindow)
            {
                SweepStaleEntries(now);
            }

            var key = (ev.PolicyName ?? string.Empty, ev.PartitionKey ?? string.Empty);
            if (!_dict.TryGetValue(key, out var st))
            {
                if (_dict.Count >= _maxTrackedKeys)
                {
                    lock (_globalLock)
                    {
                        _globalSuppressedKeys++;
                        _globalSuppressedCount++;
                    }
                    EmitAll(toEmit, emit);
                    return;
                }
                st = _dict.GetOrAdd(key, _ => new WindowState
                {
                    WindowStart = now,
                    Count = 0,
                    PolicyName = ev.PolicyName,
                    PartitionKey = ev.PartitionKey,
                    Mode = ev.Mode
                });
            }

            lock (st)
            {
                if (now - st.WindowStart >= _window)
                {
                    if (st.Count > PerKeyDetail)
                    {
                        toEmit.Add(BuildPerKeySuppressed(st));
                    }
                    st.WindowStart = now;
                    st.Count = 0;
                    st.PolicyName = ev.PolicyName;
                    st.PartitionKey = ev.PartitionKey;
                    st.Mode = ev.Mode;
                }
                st.Count++;
                if (st.Count == PerKeyDetail)
                {
                    bool emitDetail;
                    lock (_globalLock)
                    {
                        if (_globalLoggedThisWindow < _globalMaxPerWindow)
                        {
                            _globalLoggedThisWindow++;
                            emitDetail = true;
                        }
                        else
                        {
                            _globalSuppressedKeys++;
                            _globalSuppressedCount++;
                            emitDetail = false;
                        }
                    }
                    if (emitDetail)
                    {
                        ev.Shape = RateLimitLogShape.Detail;
                        toEmit.Add(ev);
                    }
                }
            }

            EmitAll(toEmit, emit);
        }

        private static void EmitAll(List<RateLimitLogEvent> toEmit, Action<RateLimitLogEvent> emit)
        {
            foreach (var e in toEmit) emit(e);
        }

        private void SweepStaleEntries(DateTime now)
        {
            var threshold = now - _evictAge;
            foreach (var kv in _dict)
            {
                if (kv.Value.WindowStart < threshold)
                {
                    _dict.TryRemove(kv.Key, out _);
                }
            }
        }

        private RateLimitLogEvent BuildPerKeySuppressed(WindowState st)
            => new RateLimitLogEvent
            {
                Shape = RateLimitLogShape.PerKeySuppressed,
                EventType = st.Mode == "LogOnly"
                    ? "RateLimitWouldHaveRejectedSuppressed"
                    : "RateLimitRejectedSuppressed",
                Mode = st.Mode,
                PolicyName = st.PolicyName,
                PartitionKey = st.PartitionKey,
                SuppressedCount = st.Count - PerKeyDetail,
                WindowSeconds = _windowSeconds,
            };

        private RateLimitLogEvent BuildGlobalSuppressed(int keys, int count)
            => new RateLimitLogEvent
            {
                Shape = RateLimitLogShape.GlobalSuppressed,
                EventType = "RateLimitGlobalSuppressed",
                SuppressedKeys = keys,
                SuppressedCount = count,
                WindowSeconds = _windowSeconds,
                Note = $"+{count} requests across {keys} other keys suppressed (global cap)",
            };
    }
}
