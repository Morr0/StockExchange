using System.Collections.Generic;
using System.Threading.Tasks;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.CacheService
{
    // WHY?: to synchronise orders between different exchange clusters
    public interface IOrderCacheService
    {
        Task Cache(string key, Order order);
        Task<Order> Decache(string key);
        Task Decache(Dictionary<string, Order> ordersInvolved);
    }
}