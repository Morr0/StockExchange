﻿using System.Threading.Tasks;
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

        public async Task<Order> Decache(string key)
        {
            return await _strategy.Get(key);
        }
    }
}