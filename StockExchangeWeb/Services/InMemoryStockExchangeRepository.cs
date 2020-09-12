using System.Collections.Generic;
using StockExchangeWeb.Models;

namespace StockExchangeWeb.Services
{
    public class InMemoryStockExchangeRepository : IStockExchange
    {
        private Dictionary<decimal, OrderBook> _orderBook = new Dictionary<decimal, OrderBook>();
        
        public Order PlaceOrder(Order order)
        {
            // To execute an order, the price and amount of shares must be the same thus there is
            // 2 layers below.
            
            // Add price to order book
            if (!_orderBook.ContainsKey(order.AskPrice))
                _orderBook.Add(order.AskPrice, new OrderBook());

            // Place order
            bool executed = _orderBook[order.AskPrice].PlaceAndTryExecute(order);
            
            // TODO execute order immediately if a corresponding order exists
            // TODO add order to history of orders

            return order;
        }
    }
}