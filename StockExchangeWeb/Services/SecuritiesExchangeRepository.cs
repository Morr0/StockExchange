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

        private Dictionary<string, Order> _ordersById;
        
        private Dictionary<string, OrderBookPerPrice> _orderBooks;
        
        private decimal _lastExecutedPrice;
        // Ask/Bid
        private decimal _lastClosestAsk = 0;
        private decimal _lastClosestBid = 0;
        private decimal _lastClosestBidAskSpread;

        public InMemoryStockExchangeRepository(ISecuritiesProvider securitiesProvider)
        {
            _securitiesProvider = securitiesProvider;
            
            _ordersById = new Dictionary<string, Order>();
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
            _ordersById.Add(order.Id, order);
            
            bool executed = false;
            switch (order.OrderType)
            {
                case OrderType.MARKET_ORDER:
                    // TODO know the mechanics of which price makes the market
                    decimal price = order.BuyOrder ? _lastClosestBid : _lastClosestAsk;
                    executed = _orderBooks[tkr][price].PlaceAndTryExecute(order);
                    break;
                case OrderType.LIMIT_ORDER:
                    executed = _orderBooks[tkr][askPrice].PlaceAndTryExecute(order);
                    break;
                case OrderType.LIMIT_ORDER_IMMEDIATE:
                    executed = _orderBooks[tkr][askPrice].TryExecute(order);
                    break;
            }
            
            ReevaluatePricing(ref order);
            
            // TODO add order to history of orders

            return order;
        }

        // Re-evaluates the bid/ask prices to the closest differences
        private void ReevaluatePricing(ref Order order)
        {
            if (order.OrderStatus == OrderStatus.NO_MATCH)
                return;
            
            decimal price = order.OrderStatus == OrderStatus.EXECUTED ? order.ExecutedPrice : order.AskPrice;
            
            if (order.BuyOrder)
                _lastClosestAsk = price;
            else
                _lastClosestBid = price;

            if (order.OrderStatus == OrderStatus.EXECUTED)
                _lastExecutedPrice = order.ExecutedPrice;
            
            _lastClosestBidAskSpread = Math.Abs(_lastClosestBid - _lastClosestAsk);
        }


        public Order RemoveOrder(string orderId)
        {
            if (!_ordersById.ContainsKey(orderId))
                return null;

            Order order = _ordersById[orderId];
            if (order.OrderStatus == OrderStatus.DELETED)
                return null;
            
            if (order.OrderStatus != OrderStatus.IN_MARKET)
                return order;

            order.OrderStatus = OrderStatus.DELETED;
            order.OrderDeletionTime = DateTime.UtcNow.ToString();
            
            // Edit order shares metadata from order book based on order type
            if (order.BuyOrder)
                _orderBooks[order.Ticker][order.AskPrice].SharesToBuy -= order.Amount;
            else
                _orderBooks[order.Ticker][order.AskPrice].SharesToSell -= order.Amount;
            
            ReevaluatePricing(ref order);
            
            return order;
        }

        public OrdersPlaced GetOrdersPlaced(string ticker)
        {
            OrdersPlaced ordersPlaced = new OrdersPlaced(ticker, _lastExecutedPrice, 
                _lastClosestAsk, _lastClosestBid, _lastClosestBidAskSpread);

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