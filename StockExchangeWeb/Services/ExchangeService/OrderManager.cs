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
        // INSTRUCTIONS:
        // - ALL DATA TO BE STORED IN CACHE USING A KEY GENERATED USING CacheKeyGenerator.cs
        
        private IOrderCacheService _orderCacheService;

        // TODO refactor verification from execution
        
        public OrderManager(IOrderCacheService orderCacheService)
        {
            _orderCacheService = orderCacheService;
        }

        #region Commons

        private async Task<Order> GetOrderByKey(string key)
        {
            return await _orderCacheService.First(key);
        }

        #endregion

        #region Put order

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
            order.DeletionReferenceKey = key;
            
            await _orderCacheService.Cache(key, order);

            return await TryExecute(order);
        }
        
        private async Task<Dictionary<string, Order>> TryExecute(Order order)
        {
            Dictionary<string, Order> ordersInvolved;
            bool oppositeSide = !order.BuyOrder;
            string cacheKey = CacheKeyGenerator.ToLookKey(ref oppositeSide, order.AskPrice, order.Amount);

            Order oppositeOrder = await GetOrderByKey(cacheKey);
            if (oppositeOrder == null)
                return new Dictionary<string, Order>
                {
                    {order.Id, order}
                };

            ordersInvolved = await Execute(order, oppositeOrder);

            return ordersInvolved;
        }

        private async Task<Dictionary<string, Order>> Execute(Order order, Order oppositeOrder)
        {
            string orderKey = CacheKeyGenerator.ToStoreKey(ref order);
            string oppositeOrderKey = CacheKeyGenerator.ToStoreKey(ref oppositeOrder);
            
            Dictionary<string, Order> ordersInvolved = new Dictionary<string, Order>
            {
                {orderKey, order},
                {oppositeOrderKey, oppositeOrder}
            };

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
        
        #endregion

        #region Remove Order

        public async Task<Order> RemoveOrder(string orderDeletionKey)
        {
            Order order = await _orderCacheService.First(orderDeletionKey);
            if (!VerifiedOrder(ref order))
                return null;
            
            Console.WriteLine("Beyond point 1");
            
            // Remove order from cache
            await _orderCacheService.Decache(new Dictionary<string, Order>
            {
                {orderDeletionKey, order}
            });

            OrderRemovalDueDiligence(ref order);
            return order;
        }
        
        private bool VerifiedOrder(ref Order order)
        {
            if (order == null)
                return false;
            Console.WriteLine("Beyond point 0");

            return order.OrderStatus != OrderStatus.Deleted && order.OrderStatus == OrderStatus.InMarket;
        }
        
        private void OrderRemovalDueDiligence(ref Order order)
        {
            order.OrderStatus = OrderStatus.Deleted;
            order.OrderDeletionTime = DateTime.UtcNow.ToString();
        }

        #endregion
    }
}