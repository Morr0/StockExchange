using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.CacheService.Implementation
{
    public class RedisCacheStrategy : CacheStrategy
    {
        private IEasyCachingProviderFactory _cachingProviderFactory;
        private IEasyCachingProvider _cachingProvider;

        public RedisCacheStrategy(IEasyCachingProviderFactory cachingProviderFactory)
        {
            _cachingProviderFactory = cachingProviderFactory;
            _cachingProvider = cachingProviderFactory.GetCachingProvider("redis1");
        }
        
        public override async Task Set(string key, Order value)
        {
            await _cachingProvider.SetAsync<Order>(key, value, TimeSpan.FromDays(1));
        }

        public override async Task<Order> Get(string key, bool firstPrefix = false)
        {
            if (firstPrefix)
            {
                // LIMIT TO ONE
                var cached = await _cachingProvider.GetByPrefixAsync<Order>(key);
                if (cached.Count == 0)
                    return null;
                else
                    return cached.Values.First().Value;
            }
            else
                return (await _cachingProvider.GetAsync<Order>(key)).Value;
        }

        public override async Task<bool> RemoveMany(IEnumerable<string> ordersInvolved)
        {
            try
            {
                await _cachingProvider.RemoveAllAsync(ordersInvolved);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}