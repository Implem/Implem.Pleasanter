using System;
using System.Threading;

namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class AppState
    {
        public static DateTime StartedAt { get; } = DateTime.UtcNow;
        private static int _restartRequested = 0;

        public static bool TryRequestRestart()
        {
            return Interlocked.CompareExchange(
                ref _restartRequested,
                1,
                0) == 0;
        }
    }
}
