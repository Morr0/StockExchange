using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services.HistoryService;
using StockExchangeWeb.Services.MarketTimesService;
using StockExchangeWeb.Services.OrderTracingService;
using StockExchangeWeb.Services.TradedEntitiesService;

namespace StockExchangeWeb.Services
{
    public class InMemoryStockExchangeRepository : IStockExchange
    {
        private ISecuritiesProvider _securitiesProvider;
        private IOrdersHistory _ordersHistory;
        private OrderTraceRepository _traceRepository;
        private MarketOpeningTimesRepository _marketTimes;

        private Dictionary<string, Order> _ordersById;
        
        private Dictionary<string, OrderBookPerPrice> _orderBooks;
        
        private decimal _lastExecutedPrice;
        // Ask/Bid
        private decimal _lastClosestAsk = 0;
        private decimal _lastClosestBid = 0;
        private decimal _lastClosestBidAskSpread;

        public InMemoryStockExchangeRepository(ISecuritiesProvider securitiesProvider, IOrdersHistory ordersHistory
            , OrderTraceRepository traceRepository, MarketOpeningTimesRepository marketTimes)
        {
            _securitiesProvider = securitiesProvider;
            _ordersHistory = ordersHistory;
            _traceRepository = traceRepository;
            _marketTimes = marketTimes;
            
            _ordersById = new Dictionary<string, Order>();
            _orderBooks = new Dictionary<string, OrderBookPerPrice>
            {
                {"A", new OrderBookPerPrice()}
            };
        }
        
        public async Task<Order> PlaceOrder(Order order)
        {
            string tkr = order.Ticker;
            decimal askPrice = order.AskPrice;
            
            // Place order
            _ordersById.Add(order.Id, order);
            
            // Trace
            _traceRepository.Trace(order);

            if (!_marketTimes.IsMarketOpen(order.Ticker))
            {
                // Place order in history
                await _ordersHistory.ArchiveOrder(new Dictionary<string, Order>
                {
                    {order.Id, order}
                });
                
                return order;
            }
            
            Dictionary<string, Order> ordersInvolved = null;
            if (order.LimitOrder)
            {
                if (order.OrderTimeInForce == OrderTimeInForce.GoodTillExecution)
                    ordersInvolved = _orderBooks[tkr][askPrice].PlaceAndTryExecute(order);
                else if (order.OrderTimeInForce == OrderTimeInForce.GoodOrKill)
                    ordersInvolved = _orderBooks[tkr][askPrice].TryExecute(order);
            }
            else
            {
                decimal price = order.BuyOrder ? _lastClosestBid : _lastClosestAsk;
                ordersInvolved = _orderBooks[tkr][price].PlaceAndTryExecute(order);
            }
            
            ReevaluatePricing(ref order);
            
            await _ordersHistory.ArchiveOrder(ordersInvolved);
            // Trace if an exchange occured
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


        public Order RemoveOrder(string orderId)
        {
            if (!_ordersById.ContainsKey(orderId))
                return null;

            Order order = _ordersById[orderId];
            if (order.OrderStatus == OrderStatus.Deleted)
                return null;
            
            if (order.OrderStatus != OrderStatus.InMarket)
                return order;

            order.OrderStatus = OrderStatus.Deleted;
            order.OrderDeletionTime = DateTime.UtcNow.ToString();

            _ordersHistory.ArchiveOrder(new Dictionary<string, Order>
            {
                {orderId, order}
            });
            // Trace
            _traceRepository.Trace(order);
            
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