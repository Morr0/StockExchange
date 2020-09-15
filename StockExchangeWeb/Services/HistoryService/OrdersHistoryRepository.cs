using System.Collections.Generic;
using System.Threading.Tasks;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services.HistoryService
{
    public class OrdersHistoryRepository : IOrdersHistory
    {
        private Dictionary<string, Order> _archivedOrders = new Dictionary<string, Order>();
        
        public async Task ArchiveOrder(Dictionary<string, Order> orders)
        {
            foreach (var ordersPair in orders)
            {
                string orderId = ordersPair.Key;
                Order order = ordersPair.Value;

                if (_archivedOrders.ContainsKey(orderId))
                    _archivedOrders[orderId] = order;
                else
                    _archivedOrders.Add(orderId, order);
            }
        }
    }
}