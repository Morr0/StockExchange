using System.Collections.Generic;
using System.Threading.Tasks;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.CacheService.Implementation
{
    public abstract class CacheStrategy
    {
        public abstract Task Set(string key, Order value);
        public abstract Task<Order> Get(string key, bool firstPrefix = false);

        public abstract Task<bool> RemoveMany(IEnumerable<string> ordersInvolved);
    }
}