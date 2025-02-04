using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System;
using Implem.DefinitionAccessor;

namespace Implem.Pleasanter.Libraries.Security
{
    public class AuthenticationTicketStore : ITicketStore
    {
        private static readonly string sessionKey = "AuthenticationTicket";
        public AuthenticationTicketStore()
        {
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var guid = Strings.NewGuid();
            var serializedTicket = TicketSerializer.Default.Serialize(ticket);
            SessionUtilities.Set(
                context: GetContext(),
                key: sessionKey,
                value: Convert.ToBase64String(serializedTicket),
                sessionGuid: guid);
            return Task.FromResult(guid);
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var serializedTicket = TicketSerializer.Default.Serialize(ticket);
            SessionUtilities.Set(
                context: GetContext(),
                key: sessionKey,
                value: Convert.ToBase64String(serializedTicket),
                sessionGuid: key);
            return Task.CompletedTask;
        }

        private Context GetContext()
        {
            return new Context(
                request: false,
                sessionData: false,
                sessionStatus: false,
                user: false,
                setPermissions: false);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var sessionData = SessionUtilities.Get(
                context: GetContext(),
                sessionGuid: key);
            if (sessionData.TryGetValue(sessionKey, out string base64))
            {
                var serializedTicket = TicketSerializer.Default.Deserialize(Convert.FromBase64String(base64));
                return Task.FromResult(serializedTicket);
            }
            return Task.FromResult<AuthenticationTicket>(default);
        }

        public Task RemoveAsync(string key)
        {
            SessionUtilities.Remove(
                context: GetContext(),
                key: sessionKey,
                page: false,
                sessionGuid: key);
            if (Parameters.Session.UseKeyValueStore)
            {
                Implem.Pleasanter.Libraries.Redis.CacheForRedisConnection.Clear(key);
            }
            return Task.CompletedTask;
        }
    }
}