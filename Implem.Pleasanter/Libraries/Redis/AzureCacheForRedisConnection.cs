using StackExchange.Redis;
using System;
using System.Configuration;
using Implem.DefinitionAccessor;

namespace Implem.Pleasanter.Libraries.Redis
{
    public class AzureCacheForRedisConnection
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(Parameters.Kvs.ConnectionString);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
