using System.Collections.Generic;
using System.Threading.Tasks;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services.CacheService.Implementation;

namespace StockExchangeWeb.Services.CacheService
{
    public class OrderCacheRepository : IOrderCacheService
    {
        private CacheStrategy _strategy;

        public OrderCacheRepository(CacheStrategy strategy)
        {
            _strategy = strategy;
        }
        
        public async Task Cache(string key, Order order)
        {
            await _strategy.Set(key, order);
        }

        public async Task Decache(Dictionary<string, Order> ordersInvolved)
        {
            await RemoveFromCache(ordersInvolved.Keys);
        }

        public async Task<Order> First(string cacheKey)
        {
            return await _strategy.Get(cacheKey);
        }

        private async Task<bool> RemoveFromCache(IEnumerable<string> ordersInvolved)
        {
            return await _strategy.RemoveMany(ordersInvolved);
        }
    }
}