using Implem.DefinitionAccessor;
using StackExchange.Redis;
using System;

namespace Implem.Pleasanter.Libraries.Redis
{
    public class CacheForRedisConnection
    {
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(Parameters.Kvs.ConnectionStringForSession);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return LazyConnection.Value;
            }
        }
    }
}
