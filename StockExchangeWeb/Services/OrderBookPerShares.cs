using System;
using System.Collections.Generic;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Utilities;

namespace StockExchangeWeb.Services
{
    /// <summary>
    /// Independent of ask price. Holds the asked shares and order type.
    /// </summary>
    public class OrderBookPerShares
    {
        /// <summary>
        /// Key -> shares for each order
        /// </summary>
        private readonly Dictionary<uint, SecurityOrderQueue> _buySharesPerOrder = new Dictionary<uint, SecurityOrderQueue>();
        private readonly Dictionary<uint, SecurityOrderQueue> _sellSharesPerOrder = new Dictionary<uint, SecurityOrderQueue>();
        
        public uint SharesToSell { get; set; }
        public uint SharesToBuy { get; set; }

        /// <summary>
        /// Places an order and tries to execute.
        /// </summary>
        /// <param name="marketOpen"></param>
        /// <param name="order"></param>
        /// <returns>True -> Executed</returns>
        public Dictionary<string, Order> PlaceAndTryExecute(bool marketOpen, Order order)
        {
            PlaceOrder(ref order);
            return TryExecute(marketOpen, order);
        }

        private void PlaceOrder(ref Order order)
        {
            if (order.BuyOrder)
            {
                if (!_buySharesPerOrder.ContainsKey(order.Amount))
                    _buySharesPerOrder.Add(order.Amount, new SecurityOrderQueue());
                
                _buySharesPerOrder[order.Amount].Enqueue(ref order);
            
                SharesToBuy += order.Amount;
            }
            else
            {
                if (!_sellSharesPerOrder.ContainsKey(order.Amount))
                    _sellSharesPerOrder.Add(order.Amount, new SecurityOrderQueue());
                
                _sellSharesPerOrder[order.Amount].Enqueue(ref order);
            
                SharesToSell += order.Amount;
            }
        }

        /// <summary>
        /// Will try to execute order immediately else will not persist.
        /// </summary>
        /// <param name="marketOpen"></param>
        /// <param name="order"></param>
        /// <returns>True -> Executed</returns>
        public Dictionary<string, Order> TryExecute(bool marketOpen, Order order)
        {
            Dictionary<string, Order> ordersInvolved = new Dictionary<string, Order>
            {
                // Will always execute regardless
                {order.Id, order}
            };

            if (!marketOpen)
            {
                if (order.OrderTimeInForce == OrderTimeInForce.GoodOrKill)
                {
                    order.OrderStatus = OrderStatus.DeletedDueToMarketClose;
                }
                
                return ordersInvolved;
            }

            if (!OppositeOrderExists(ref order))
            {
                if (order.LimitOrder && order.OrderTimeInForce == OrderTimeInForce.GoodOrKill)
                    order.OrderStatus = OrderStatus.NoMatch;

                return ordersInvolved;
            }

            Order oppositeOrder = ExecuteOppositeOrder(ref order);
            if (oppositeOrder != null)
                ordersInvolved.Add(oppositeOrder.Id, oppositeOrder);

            return ordersInvolved;
        }
        
        private bool OppositeOrderExists(ref Order order)
        {
            return order.BuyOrder ? _sellSharesPerOrder.ContainsKey(order.Amount) 
                : _buySharesPerOrder.ContainsKey(order.Amount);
        }
        
        private Order ExecuteOppositeOrder(ref Order order)
        {
            var oppositeQueue = order.BuyOrder ? _sellSharesPerOrder[order.Amount] : _buySharesPerOrder[order.Amount];
            if (oppositeQueue.Count == 0)
                return null;
            
            // Execute
            Order oppositeOrder = null;
            do
            {
                oppositeOrder = oppositeQueue.Dequeue();
                if (oppositeOrder.OrderStatus == OrderStatus.InMarket)
                    break;
            } while (oppositeOrder.OrderStatus != OrderStatus.InMarket);

            // Using the oppositeOrder price so it conforms to market orders as well
            oppositeOrder.ExecutedPrice = oppositeOrder.AskPrice;
            order.ExecutedPrice = oppositeOrder.ExecutedPrice;

            // Metadata
            SharesToBuy -= order.Amount;
            SharesToSell -= order.Amount;

            order.OrderStatus = oppositeOrder.OrderStatus = OrderStatus.Executed;
            order.OrderExecutionTime = oppositeOrder.OrderExecutionTime = DateTime.UtcNow.ToString();

            return oppositeOrder;
        }
    }
}