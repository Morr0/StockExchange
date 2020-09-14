using System.Collections.Generic;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Utilities
{
    // A custom queue concerned with allowing market orders to go first
    public class SecurityOrderQueue
    {
        private readonly Queue<Order> _marketOrders = new Queue<Order>();
        private readonly Queue<Order> _remainingOrders = new Queue<Order>();
        
        public int Count
        {
            get => _marketOrders.Count + _remainingOrders.Count;
        }

        public void Enqueue(ref Order order)
        {
            if (order.OrderType == OrderType.MARKET_ORDER)
                _marketOrders.Enqueue(order);
            else
                _remainingOrders.Enqueue(order);
        }

        public Order Dequeue()
        {
            if (_marketOrders.Count > 0)
                return _marketOrders.Dequeue();
            else if (_remainingOrders.Count > 0)
                return _remainingOrders.Dequeue();
            else
                return null;
                
        }
    }
}