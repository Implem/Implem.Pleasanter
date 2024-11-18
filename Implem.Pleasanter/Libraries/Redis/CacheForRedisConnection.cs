using Implem.DefinitionAccessor;
using StackExchange.Redis;
using System;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Redis
{
    public class CacheForRedisConnection
    {
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(Parameters.Kvs.ConnectionStringForSession, x => x.AllowAdmin = true);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return LazyConnection.Value;
            }
        }

        public static void Remove(string sessionGuid)
        {
            StackExchange.Redis.IDatabase iDatabase = Connection.GetDatabase();
            iDatabase.KeyDelete(sessionGuid);
        }

        public static void Clear(string sessionGuid)
        {
            Remove(sessionGuid);
        }
    }
}
