using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services.CacheService;
using StockExchangeWeb.Services.ExchangeService.Utilities;

namespace StockExchangeWeb.Services.ExchangeService
{
    // Responsible for placing/removing/fetching orders
    public class OrderManager
    {
        private IOrderCacheService _orderCacheService;

        public OrderManager(IOrderCacheService orderCacheService)
        {
            _orderCacheService = orderCacheService;
        }

        public async Task<Dictionary<string, Order>> PutOrder(bool marketOpen, Order order)
        {
            Dictionary<string, Order> ordersInvolved = null;

            if (!marketOpen)
                return new Dictionary<string, Order>
                {
                    {order.Id, order}
                };

            // if (order.LimitOrder)
            // {
            //     ordersInvolved = order.OrderTimeInForce == OrderTimeInForce.GoodTillExecution 
            //         ? _orderBooks[tkr][askPrice].PlaceAndTryExecute(marketOpen, order) 
            //         : _orderBooks[tkr][askPrice].TryExecute(marketOpen, order);
            // }
            // else
            // {
            //     decimal price = order.BuyOrder ? _lastClosestBid : _lastClosestAsk;
            //     ordersInvolved = _orderBooks[tkr][price].PlaceAndTryExecute(marketOpen, order);
            // }

            if (order.LimitOrder)
            {
                ordersInvolved = order.OrderTimeInForce == OrderTimeInForce.GoodTillExecution
                    ? await PlaceOrderAndTryExecute(order)
                    : await TryExecute(order);
            }
            else
            {
                // TODO handle market orders
                ordersInvolved = new Dictionary<string, Order>
                {
                    {order.Id, order}
                };
            }

            return ordersInvolved;
        }

        private async Task<Dictionary<string, Order>> PlaceOrderAndTryExecute(Order order)
        {
            string key = CacheKeyGenerator.ToStoreKey(ref order);

            await _orderCacheService.Cache(key, order);

            return await TryExecute(order);
        }
        
        private async Task<Dictionary<string, Order>> TryExecute(Order order)
        {
            Dictionary<string, Order> ordersInvolved;
            bool oppositeSide = !order.BuyOrder;
            string cacheKey = CacheKeyGenerator.ToLookKey(ref oppositeSide, order.AskPrice, order.Amount);

            Order oppositeOrder = await GetOppositeOrder(cacheKey);
            if (oppositeOrder == null)
                return new Dictionary<string, Order>
                {
                    {order.Id, order}
                };

            ordersInvolved = await Execute(order, oppositeOrder);

            return ordersInvolved;
        }

        private async Task<Order> GetOppositeOrder(string cacheKey)
        {
            return await _orderCacheService.First(cacheKey);
        }
        
        private async Task<Dictionary<string, Order>> Execute(Order order, Order oppositeOrder)
        {
            Dictionary<string, Order> ordersInvolved = new Dictionary<string, Order>
            {
                {order.Id, order},
                {oppositeOrder.Id, oppositeOrder}
            };
            
            string orderKey = CacheKeyGenerator.ToStoreKey(ref order);
            string oppositeOrderKey = CacheKeyGenerator.ToStoreKey(ref oppositeOrder);

            await _orderCacheService.Decache(ordersInvolved);

            decimal execPrice = GetExecutionPrice(order, oppositeOrder);
            OrderExecutionDueDiligence(execPrice, ordersInvolved);

            return ordersInvolved;
        }

        private decimal GetExecutionPrice(Order order, Order oppositeOrder)
        {
            // TODO take care of possible combinations of over-paying
            return order.AskPrice;
        }

        // Takes care of bookkeeping
        private static void OrderExecutionDueDiligence(decimal price, Dictionary<string, Order> ordersInvolved)
        {
            foreach (var orderPair in ordersInvolved)
            {
                Order order = orderPair.Value;

                order.OrderStatus = OrderStatus.Executed;
                order.OrderExecutionTime = DateTime.UtcNow.ToString();
                order.ExecutedPrice = price;
            }
        }
    }
}