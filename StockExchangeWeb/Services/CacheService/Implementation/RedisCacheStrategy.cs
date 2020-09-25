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
        private IEasyCachingProvider _cachingProvider;

        public RedisCacheStrategy(IEasyCachingProviderFactory cachingProviderFactory)
        {
            _cachingProvider = cachingProviderFactory.GetCachingProvider("redis1");
        }
        
        public override async Task Set(string key, Order value)
        {
            await _cachingProvider.SetAsync<Order>(key, value, TimeSpan.FromDays(1));
        }

        public override async Task<Order> Get(string key)
        {
            // LIMIT TO ONE
            var cached = await _cachingProvider.GetByPrefixAsync<Order>(key);
            Console.WriteLine("point -2");
            if (cached.Count == 0)
                return null;
            Console.WriteLine("point -1");
            return cached.Values.First().Value;
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