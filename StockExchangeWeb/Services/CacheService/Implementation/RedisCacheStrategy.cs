using System;
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

        public override async Task<Order> Get(string key)
        {
            CacheValue<Order> val = await _cachingProvider.GetAsync<Order>(key);
            return val.Value;
        }
    }
}