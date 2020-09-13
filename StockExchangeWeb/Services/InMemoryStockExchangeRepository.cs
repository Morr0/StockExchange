using System.Collections.Generic;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Services
{
    public class InMemoryStockExchangeRepository : IStockExchange
    {
        private Dictionary<decimal, OrderBook> _orderBooks = new Dictionary<decimal, OrderBook>();
        
        private decimal _lastExecutedPrice;
        
        public Order PlaceOrder(Order order)
        {
            // To execute an order, the price and amount of shares must be the same thus there is
            // 2 layers below.
            
            // Add price to order book
            if (!_orderBooks.ContainsKey(order.AskPrice))
                _orderBooks.Add(order.AskPrice, new OrderBook());

            // Place order
            bool executed = false;
            if (order.OrderType == OrderType.LIMIT_ORDER)
            {
                executed = _orderBooks[order.AskPrice].PlaceAndTryExecute(order);
            } else if (order.OrderType == OrderType.LIMIT_ORDER_IMMEDIATE)
            {
                executed = _orderBooks[order.AskPrice].TryExecute(order);
            }
            
            // Metadata
            if (order.OrderStatus == OrderStatus.EXECUTED)
                _lastExecutedPrice = order.AskPrice;
            
            // TODO add market order
            // TODO add stop order and it's types
            
            // TODO execute order immediately if a corresponding order exists
            // TODO add order to history of orders

            return order;
        }

        public OrdersPlaced GetOrdersPlaced(string ticker)
        {
            OrdersPlaced ordersPlaced = new OrdersPlaced(ticker, _lastExecutedPrice);
            
            foreach (var orderBookPerPrice in _orderBooks)
            {
                // To not overwhelm memory
                if (ordersPlaced.BuyOrders.Count > 10)
                    break;
                
                string price = orderBookPerPrice.Key.ToString();
                OrderBook orderBook = orderBookPerPrice.Value;
                
                // Init if not initialized
                if (!ordersPlaced.BuyOrders.ContainsKey(price))
                    ordersPlaced.BuyOrders.Add(price, 0);
                if (!ordersPlaced.SellOrders.ContainsKey(price))
                    ordersPlaced.SellOrders.Add(price, 0);

                ordersPlaced.BuyOrders[price] = orderBook.SharesToBuy;
                ordersPlaced.SellOrders[price] = orderBook.SharesToSell;
            }

            return ordersPlaced;
        }
    }
}