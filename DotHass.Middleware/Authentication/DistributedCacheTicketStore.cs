using DotHass.Middleware.Authentication.Abstractions;
using DotHass.Middleware.Authentication.Serializer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DotHass.Middleware.Authentication
{
    public class DistributedCacheTicketStore : ITicketStore
    {
        private const string KeyPrefix = "__AuthSessionStore";
        private IDistributedCache _cache;
        protected AuthenticationOptions Options;

        public DistributedCacheTicketStore(IDistributedCache cache, IOptions<AuthenticationOptions> options)
        {
            Options = options.Value;
            _cache = cache;
        }

        public async Task<string> StoreAsync(string key, AuthenticationTicket ticket)
        {
            await RenewAsync(key, ticket);
            return key;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new DistributedCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;

            //设置相对过期时间
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }

            if (Options.SlidingExpiration == true)
            {
                options.SetSlidingExpiration(Options.ExpireTimeSpan);
            }

            var v = TicketSerializer.Default.Serialize(ticket);

            _cache.Set(GetRegionKey(key), v, options);

            return Task.FromResult(0);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var value = _cache.Get(GetRegionKey(key));

            AuthenticationTicket ticket;

            if (value == null)
            {
                ticket = default(AuthenticationTicket);
            }
            else
            {
                ticket = TicketSerializer.Default.Deserialize(value);
            }
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(GetRegionKey(key));
            return Task.FromResult(0);
        }

        public string GetRegionKey(string key)
        {
            return KeyPrefix + ":" + key;
        }
    }
}