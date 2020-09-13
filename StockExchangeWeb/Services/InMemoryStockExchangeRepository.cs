using System;
using System.Collections.Generic;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services.TradedEntitiesService;

namespace StockExchangeWeb.Services
{
    public class InMemoryStockExchangeRepository : IStockExchange
    {
        private ISecuritiesProvider _securitiesProvider;
        
        private Dictionary<string, OrderBookPerPrice> _orderBooks;
        
        private decimal _lastExecutedPrice;

        public InMemoryStockExchangeRepository(ISecuritiesProvider securitiesProvider)
        {
            _securitiesProvider = securitiesProvider;
            
            _orderBooks = new Dictionary<string, OrderBookPerPrice>
            {
                {"A", new OrderBookPerPrice()}
            };
        }
        
        public Order PlaceOrder(Order order)
        {
            string tkr = order.Ticker;
            decimal askPrice = order.AskPrice;
            
            // Place order
            bool executed = false;
            if (order.OrderType == OrderType.LIMIT_ORDER)
            {
                executed = _orderBooks[tkr][askPrice].PlaceAndTryExecute(order);
            } else if (order.OrderType == OrderType.LIMIT_ORDER_IMMEDIATE)
            {
                executed = _orderBooks[tkr][askPrice].TryExecute(order);
            }
            
            // Metadata
            if (order.OrderStatus == OrderStatus.EXECUTED)
                _lastExecutedPrice = askPrice;
            
            // TODO add market order
            // TODO add stop order and it's types
            
            // TODO execute order immediately if a corresponding order exists
            // TODO add order to history of orders

            return order;
        }

        public OrdersPlaced GetOrdersPlaced(string ticker)
        {
            OrdersPlaced ordersPlaced = new OrdersPlaced(ticker, _lastExecutedPrice);

            var enumberable = _orderBooks[ticker].SharesOrderBooks;
            foreach (var priceOrderBook in enumberable)
            {
                // To not overwhelm memory
                if (ordersPlaced.BuyOrders.Count > 10)
                    break;
                
                string price = priceOrderBook.Key.ToString();
                OrderBookPerShares orderBookPerShares = priceOrderBook.Value;
                
                // Init if not initialized
                if (!ordersPlaced.BuyOrders.ContainsKey(price))
                    ordersPlaced.BuyOrders.Add(price, 0);
                if (!ordersPlaced.SellOrders.ContainsKey(price))
                    ordersPlaced.SellOrders.Add(price, 0);

                ordersPlaced.BuyOrders[price] = orderBookPerShares.SharesToBuy;
                ordersPlaced.SellOrders[price] = orderBookPerShares.SharesToSell;
            }

            return ordersPlaced;
        }
    }
}