using Implem.DefinitionAccessor;
using StackExchange.Redis;
using System;

namespace Implem.Pleasanter.Libraries.Redis
{
    public class DataProtectionForRedisConnection
    {
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(Parameters.Kvs.ConnectionStringForDataProtection, x => x.AllowAdmin = true);
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
