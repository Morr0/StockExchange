using System;
using System.Collections.Generic;
using StockExchangeWeb.Models;

namespace StockExchangeWeb.Services
{
    /// <summary>
    /// Independent of ask price. Holds the asked shares and order type.
    /// </summary>
    internal class OrderBook
    {
        /// <summary>
        /// Key -> shares for each order
        /// </summary>
        private Dictionary<uint, Queue<Order>> _buySharesPerOrder = new Dictionary<uint, Queue<Order>>();
        private Dictionary<uint, Queue<Order>> _sellSharesPerOrder = new Dictionary<uint, Queue<Order>>();

        /// <summary>
        /// Places an order and tries to execute.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>True -> Executed</returns>
        public bool PlaceAndTryExecute(Order order)
        {
            PlaceOrder(ref order);
            return TryExecute(order);
        }

        private void PlaceOrder(ref Order order)
        {
            if (order.BuyOrder)
            {
                if (!_sellSharesPerOrder.ContainsKey(order.Amount))
                    _sellSharesPerOrder.Add(order.Amount, new Queue<Order>());
                
                _sellSharesPerOrder[order.Amount].Enqueue(order);
            }
            else
            {
                if (!_buySharesPerOrder.ContainsKey(order.Amount))
                    _buySharesPerOrder.Add(order.Amount, new Queue<Order>());
                
                _buySharesPerOrder[order.Amount].Enqueue(order);
            }
        }

        /// <summary>
        /// Will try to execute order immediately else will not persist.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>True -> Executed</returns>
        public bool TryExecute(Order order)
        {
            if (order.BuyOrder)
            {
                if (_sellSharesPerOrder.ContainsKey(order.Amount))
                {
                    _sellSharesPerOrder[order.Amount].Dequeue();
                    
                    order.OrderStatus = OrderStatus.EXECUTED;
                    order.OrderExecutionTime = DateTime.UtcNow.ToString();
                }
                else
                    return false;
            }
            else
            {
                if (_buySharesPerOrder.ContainsKey(order.Amount))
                {
                    _buySharesPerOrder[order.Amount].Dequeue();
                    
                    order.OrderStatus = OrderStatus.EXECUTED;
                    order.OrderExecutionTime = DateTime.UtcNow.ToString();
                }
                else
                    return false;
            }

            return true;
        }
    }
}