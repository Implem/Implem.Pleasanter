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
            StackExchange.Redis.IDatabase iDatabase = Connection.GetDatabase();
            var endpoints = Connection.GetEndPoints(true);
            var hash = Connection.GetHashCode();
            foreach (var endpoint in endpoints)
            {
                foreach( var key in Connection.GetServer(endpoint).Keys(hash))
                {
                    var authenticationTiecketExists = iDatabase.HashGetAll(key)
                        .Where(dataRow =>
                            dataRow.Name.ToString() == "AuthenticationTicket")
                        .Count();
                    if(authenticationTiecketExists > 0)
                    {
                        Remove(key);
                    }
                }
            }
            Remove(sessionGuid);
        }
    }
}
