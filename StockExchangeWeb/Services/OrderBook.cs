﻿using System;
using System.Collections.Generic;
using StockExchangeWeb.Models;
using StockExchangeWeb.Models.Orders;

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
        
        public uint SharesToSell { get; private set; }
        public uint SharesToBuy { get; private set; }

        private Dictionary<uint, Queue<Order>> CorresspondingShares(ref Order order)
        {
            // Init if is not
            if (!_sellSharesPerOrder.ContainsKey(order.Amount))
                _sellSharesPerOrder.Add(order.Amount, new Queue<Order>());
            if (!_buySharesPerOrder.ContainsKey(order.Amount))
                _buySharesPerOrder.Add(order.Amount, new Queue<Order>());

            return order.BuyOrder ? _sellSharesPerOrder : _buySharesPerOrder;
        }

        private void CorrespondingSharesTotal(ref Order order, bool add)
        {
            if (order.BuyOrder)
                SharesToSell += (uint)(add ? order.Amount : -order.Amount);
            else
                SharesToBuy += (uint) (add ? order.Amount : -order.Amount);
        }

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
                if (!_buySharesPerOrder.ContainsKey(order.Amount))
                    _buySharesPerOrder.Add(order.Amount, new Queue<Order>());
                
                Console.WriteLine($"Pricce: {order.Amount}");
                _buySharesPerOrder[order.Amount].Enqueue(order);
            
                SharesToBuy += order.Amount;
                Console.WriteLine($"SHArres: {SharesToBuy}");
            }
            else
            {
                if (!_sellSharesPerOrder.ContainsKey(order.Amount))
                    _sellSharesPerOrder.Add(order.Amount, new Queue<Order>());
                
                _sellSharesPerOrder[order.Amount].Enqueue(order);
            
                SharesToSell += order.Amount;
            }
        }

        /// <summary>
        /// Will try to execute order immediately else will not persist.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>True -> Executed</returns>
        public bool TryExecute(Order order)
        {
            if (!OppositeOrderExists(ref order))
                return false;

            Order oppositeOrder = ExecuteOppositeOrder(ref order);
            return oppositeOrder != null;
        }
        
        private bool OppositeOrderExists(ref Order order)
        {
            if (order.BuyOrder)
                return _sellSharesPerOrder.ContainsKey(order.Amount);
            else
                return _buySharesPerOrder.ContainsKey(order.Amount);
        }
        
        private Order ExecuteOppositeOrder(ref Order order)
        {
            var oppositeQueue = order.BuyOrder ? _sellSharesPerOrder[order.Amount] : _buySharesPerOrder[order.Amount];
            if (oppositeQueue.Count == 0)
                return null;
            
            // Execute
            Order oppositeOrder = oppositeQueue.Dequeue();
            
            // Metadata
            SharesToBuy -= order.Amount;
            SharesToSell -= order.Amount;
            
            order.OrderStatus = oppositeOrder.OrderStatus = OrderStatus.EXECUTED;
            order.OrderExecutionTime = oppositeOrder.OrderExecutionTime = DateTime.UtcNow.ToString();

            return oppositeOrder;
        }
    }
}