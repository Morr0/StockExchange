﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services.CacheService;
using StockExchangeWeb.Services.HistoryService;
using StockExchangeWeb.Services.MarketTimesService;
using StockExchangeWeb.Services.OrderTracingService;
using StockExchangeWeb.Services.TradedEntitiesService;

namespace StockExchangeWeb.Services.ExchangeService
{
    // TODO currently the cache is not considering other clusters if they double execute
    
    public class InMemoryStockExchangeRepository : IStockExchange
    {
        private ISecuritiesProvider _securitiesProvider;
        private IOrdersHistory _ordersHistory;
        private OrderTraceRepository _traceRepository;
        private IMarketOpeningTimesService _marketTimes;
        
        private OrderManager _orderManager;

        private Dictionary<string, OrderBookPerPrice> _orderBooks;
        
        private decimal _lastExecutedPrice;
        // Ask/Bid
        private decimal _lastClosestAsk = 0;
        private decimal _lastClosestBid = 0;
        private decimal _lastClosestBidAskSpread;
        private IOrderCacheService _orderCacheService;

        public InMemoryStockExchangeRepository(ISecuritiesProvider securitiesProvider, IOrdersHistory ordersHistory
            , OrderTraceRepository traceRepository, IMarketOpeningTimesService marketTimes, 
            IOrderCacheService orderCacheService)
        {
            _securitiesProvider = securitiesProvider;
            _ordersHistory = ordersHistory;
            _traceRepository = traceRepository;
            _marketTimes = marketTimes;
            _orderCacheService = orderCacheService;
            
            _orderManager = new OrderManager(orderCacheService);
            
            InitOrderBooks();
        }

        private void InitOrderBooks()
        {
            _orderBooks = new Dictionary<string, OrderBookPerPrice>();
            foreach (var securityPair in _securitiesProvider.Securities)
            {
                _orderBooks.Add(securityPair.Key, new OrderBookPerPrice());
            }
        }
        
        public async Task<Order> PlaceOrder(Order order)
        {
            _traceRepository.Trace(order);

            bool marketOpen = _marketTimes.IsMarketOpen(order.Ticker);
            var ordersInvolved = await _orderManager.PutOrder(marketOpen, order);
            ReevaluatePricing(ref order);
            
            await _ordersHistory.ArchiveOrder(ordersInvolved);
            if (ordersInvolved.Count > 1)
                _traceRepository.Trace(ordersInvolved);

            return order;
        }

        // Re-evaluates the bid/ask prices to the closest differences
        private void ReevaluatePricing(ref Order order)
        {
            if (order.OrderStatus == OrderStatus.NoMatch)
                return;
            
            decimal price = order.OrderStatus == OrderStatus.Executed ? order.ExecutedPrice : order.AskPrice;
            
            if (order.BuyOrder)
                _lastClosestAsk = price;
            else
                _lastClosestBid = price;

            if (order.OrderStatus == OrderStatus.Executed)
                _lastExecutedPrice = order.ExecutedPrice;
            
            _lastClosestBidAskSpread = Math.Abs(_lastClosestBid - _lastClosestAsk);
        }


        public async Task<Order> RemoveOrder(string orderDeletionKey)
        {
            Order order = await _orderManager.RemoveOrder(orderDeletionKey);
            if (order == null)
                return null;

            Console.WriteLine("Beyond point 2");
            
            await _ordersHistory.ArchiveOrder(new Dictionary<string, Order>
            {
                {orderDeletionKey, order}
            });
            
            _traceRepository.Trace(order);
            
            // TODO shares lookup: take care
            // if (order.BuyOrder)
            //     _orderBooks[order.Ticker][order.AskPrice].SharesToBuy -= order.Amount;
            // else
            //     _orderBooks[order.Ticker][order.AskPrice].SharesToSell -= order.Amount;
            
            ReevaluatePricing(ref order);

            return order;
        }

        public OrdersPlaced GetOrdersPlaced(string ticker)
        {
            OrdersPlaced ordersPlaced = new OrdersPlaced(_marketTimes.IsMarketOpen(ticker), ticker, _lastExecutedPrice, 
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