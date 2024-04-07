using StackExchange.Redis;
using System;
using System.Configuration;

namespace Implem.Pleasanter.Libraries.Redis
{
    public class AzureCacheForRedisConnection
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            // Web.configに設定してもポータル上の場合でも、ConfigurationManagerで取得できます。
            string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"];
            return ConnectionMultiplexer.Connect(cacheConnection);
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
