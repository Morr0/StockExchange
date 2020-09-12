using System.Collections.Generic;
using StockExchangeWeb.Models;

namespace StockExchangeWeb.Services
{
    internal class OrderBook
    {
        /// <summary>
        /// Key -> shares for each order
        /// </summary>
        public Dictionary<uint, Queue<Order>> SharesPerOrder = new Dictionary<uint, Queue<Order>>();
    }
}